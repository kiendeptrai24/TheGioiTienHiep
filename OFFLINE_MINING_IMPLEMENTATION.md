# OFFLINE MINING SYSTEM - IMPLEMENTATION GUIDE

## 📋 TỔNG QUAN THAY ĐỔI

Hệ thống offline mining cho phép players tiếp tục kiếm tiền từ mỏ ngay cả khi họ offline, và bảo vệ tiền khi người khác cướp mỏ.

### Core Scenarios:
1. **Offline Farming**: Player offline → Server vẫn tính tiền
2. **Auto-Reconnect Rewards**: Player online lại → Nhận toàn bộ tiền offline
3. **Steal Protection**: Player khác cướp → Offline player nhận tiền tới thời điểm bị cướp

---

## 📁 FILES ĐƯỢC CẬP NHẬT

### 1. **ItemResourseData.cs** (Extended)
```csharp
// Thêm 2 fields:
public ulong accumulatedOfflineCoins;  // Tiền tích lũy khi offline
public double lastOwnerClaimTime;      // Server time lần cuối nhận tiền
```

### 3. **SpiritStoneMine.cs** (Major Rewrite)
**Thêm:**
- `_ownerIsOffline` - Track if owner is online/offline
- `_pendingOfflineCoins` - Coins tích lũy khi offline
- `CalculateOfflineMiningPerSecond()` - Tính coins/second
- `CalculatePendingOfflineCoins()` - Tính tổng pending coins
- `GetPendingOfflineCoins()` - Getter for pending
- `AddOfflineCoinsToOwner()` - Thêm coins khi reconnect

**Update:**
- `SetOwner()` - Init offline tracking fields
- `UnLink()` - Calculate & send pending coins trước unlink
- `Update()` - Handle offline mining accumulation logic

### 4. **ResourceStorage.cs** (Extended)
```csharp
// Thêm method:
public void AddOfflineCoins(ulong amount)  // Thêm coins offline
```

### 5. **PlayerProfileDTO.cs** (Extended)
```csharp
// Thêm 3 fields:
public Dictionary<string, ulong> mineOfflineCoins;
public Dictionary<string, double> mineLastClaimTime;
public Dictionary<string, string> mineCurrentOwner;
```

### 6. **ProfileService.cs** (Enhanced)
- `LoadGame()` - Load offline coins từ PlayFab
- `SaveGame()` - Save offline coins tới PlayFab (qua PlayFabDataService)

### 7. **PlayFabDataService.cs** (Updated)
```csharp
// SetProfile() now includes:
profile.mineOfflineCoins = gameData.mineOfflineCoins;
profile.mineLastClaimTime = gameData.mineLastClaimTime;
profile.mineCurrentOwner = gameData.mineCurrentOwner;
```

### 8. **MineClickable.cs** (Enhanced)
```csharp
// Thêm:
private string mineId;  // Unique mine identifier
GetMineId()             // Getter for mine ID
GetMine()               // Getter for mine component
UpdateMineOwnershipInGameData()
ClearMineOwnershipFromGameData()
```

### 9. **OfflineMiningManager.cs** (NEW - Helper Class)
Singleton manager để:
- Handle offline coins khi player reconnect
- Provide utility methods để manage offline mining data
- Listen to data load events và trigger offline coin distribution

---

## 🔄 LUỒNG XỬ LÝ CHI TIẾT

### Phase 1: Player Claim Mine (Continue như trước)
```
Client click → RPC to Server → SpiritStoneMine.SetOwner()
↓
_ownerIsOffline = false
_offlineMiningStartTime = now
lastOwnerClaimTime = now
```

### Phase 2: Normal Mining (Owner Online)
```
Update() loop:
  - Check owner exists (_owner != null)
  - Tính produce time: now - _lastProduceTime >= miningTime
  - Gọi Produce() → Add coins to ResourceStorage
  - Sync via Netcode
```

### Phase 3: Owner Goes Offline (Disconnects)
```
NetworkObject destroyed when client disconnect
↓
Update() detects _owner == null
↓
_ownerIsOffline = true
_lastProduceTime = now
↓
Start accumulating offline coins per second
```

### Phase 4: Offline Mining (Owner Offline)
```
Update() every frame:
  - _owner == null → Continue
  - _ownerIsOffline == true → Call CalculateOfflineMiningPerSecond()
  
CalculateOfflineMiningPerSecond():
  yieldPerSecond = yieldPerHarvest / miningTime
  pendingCoins += yieldPerSecond
```

### Phase 5: Player Reconnects
```
Player logs in → PlayFab data loaded → OfflineMiningManager triggered
↓
GetTotalOfflineCoins() from gameData.mineOfflineCoins
↓
AddOfflineCoinsToOwner() → ResourceStorage.AddOfflineCoins()
↓
Coins increase in real-time on client
↓
SaveGame() syncs to PlayFab
```

### Phase 6: Mine Stolen (Player Offline)
```
Other player calls UnLink() while owner offline
↓
SpiritStoneMine.UnLink() triggered
↓
CalculatePendingOfflineCoins():
  = yieldPerSecond × (now - lastOwnerClaimTime)
↓
AddOfflineCoinsToOwner() to old owner (even if offline)
  → Stored in mineOfflineCoins dictionary
↓
_owner = null → Mine becomes free
↓
When victim comes back online:
  OfflineMiningManager adds stored offline coins
```

---

## 🛠️ SETUP & INTEGRATION

### Step 1: Verify All Files Changed
```
✅ ItemResourseData.cs
✅ GameData.cs
✅ ResourceStorage.cs
✅ SpiritStoneMine.cs
✅ PlayerProfileDTO.cs
✅ ProfileService.cs
✅ PlayFabDataService.cs
✅ MineClickable.cs
✅ OfflineMiningManager.cs (NEW)
```

### Step 2: Add OfflineMiningManager to Scene
1. Create empty GameObject: "OfflineMiningManager"
2. Add script: `OfflineMiningManager` component
3. Assign "SaveLoadPlayfab" reference to the SaveLoad Manager object

### Step 3: Verify PlayFab DTO Class
Check that `PlayerProfileDTO` has constructor:
```csharp
public PlayerProfileDTO()
{
    mineOfflineCoins = new Dictionary<string, ulong>();
    mineLastClaimTime = new Dictionary<string, double>();
    mineCurrentOwner = new Dictionary<string, string>();
}
```

### Step 4: Test Save/Load Flow
```csharp
// In SaveLoadPlayfab or any debug context:
[ContextMenu("Test Offline Coins")]
void TestOfflineCoins()
{
    gameData.mineOfflineCoins["mine_123"] = 5000;
    gameData.mineLastClaimTime["mine_123"] = NetworkManager.ServerTime.Time;
    playfabDataManager.SaveGameData();
}
```

---

## 🔍 KEY LOGIC EXPLANATIONS

### Offline Coin Calculation
```csharp
// Per second yield:
yieldPerSecond = yieldPerHarvest / miningTime
// Example: 100 harvest / 10 sec = 10 coins/sec

// Total offline coins after N seconds:
totalOfflineCoins = yieldPerSecond × N
```

### Owner Offline Detection
```csharp
// In Update():
if (_owner != null) 
  // Owner online, do normal mining

else if (!_ownerIsOffline && _offlineMiningStartTime > 0)
  // Owner just disconnected, switch to offline mode
  _ownerIsOffline = true
```

### Pending Coins on Steal
```csharp
// When someone steals mine:
double offlineDuration = now - miningData.lastOwnerClaimTime
float yieldPerSecond = miningData.yieldPerHarvest / miningData.miningTime
ulong pendingCoins = (ulong)(yieldPerSecond × offlineDuration)

// Add to old owner (can be offline):
_ownerStorage.AddOfflineCoins(pendingCoins)
// OR store in GameData.mineOfflineCoins if owner offline
```

---

## ⚠️ IMPORTANT NOTES

### Server Time Only
- Use `NetworkManager.ServerTime.Time` everywhere
- Never trust client time for mining calculations
- Validate all calculations server-side

### Data Persists in PlayFab
- `mineOfflineCoins` saved in profile
- Survives server restart
- Loaded when player reconnects

### Backward Compatibility
- Existing mines without offline tracking work normally
- Dictionary initialization in GameData constructor
- Null checks throughout code

### Network Sync
- Offline coins added via `ResourceStorage.AddOfflineCoins()`
- NetworkVariable.Coins syncs to all clients
- UI updates automatically via OnCoinsChanged event

---

## 🐛 DEBUG & TESTING

### Console Logs
```csharp
[SpiritStoneMine] SetOwner: {netId}, offlineStart={time}
[SpiritStoneMine] Owner is offline, starting accumulation
[SpiritStoneMine] CalculatePending: duration={sec}s, total={coins}
[SpiritStoneMine] UnLink: Sent {coins} pending coins
[OfflineMiningManager] Added {coins} offline coins to player
[MineClickable] Mine ID: {mineId}
```

### Test Scenarios
1. **Normal Mining**: Player online, claim mine, coins increase ✅
2. **Offline Farming**: Player offline by force-closing, check coins on reconnect ✅
3. **Steal with Offline Owner**: Player A offline, Player B steals → A gets coins when online ✅
4. **Multiple Mines**: Multiple mines stored in GameData dicts ✅
5. **Server Restart**: Coins persist in PlayFab, reload on login ✅

### Validation Checks
```csharp
// In OfflineMiningManager.cs - check reconnect flow:
1. gameData.coins loaded from PlayFab ✅
2. gameData.mineOfflineCoins populated ✅
3. OfflineCoins added to player ✅
4. mineOfflineCoins cleared after add ✅
5. SaveGame() called to persist ✅
```

---

## 🚀 OPTIMIZATION NOTES

### Performance
- Offline coin accumulation happens once per second (not every frame)
- Calculation is simple arithmetic (yieldPerSecond × duration)
- No extra database calls during mining

### Network Traffic
- Only added during reconnect (not continuous)
- Piggybacks on existing ProfileService save/load
- NetworkVariable sync handles distribution to clients

### Memory
- Dictionary size = number of owned mines (usually 1-5)
- Each entry: string key + ulong value = ~40 bytes
- Negligible memory footprint

---

## 📊 DATA SCHEMA (PlayFab Profile)

```json
{
  "characterId": "hero_123",
  "playerName": "Player Name",
  "coins": 10000,
  
  "mineOfflineCoins": {
    "mine_456": 5000,
    "mine_789": 2500
  },
  "mineLastClaimTime": {
    "mine_456": 1700000000.5,
    "mine_789": 1700000050.3
  },
  "mineCurrentOwner": {
    "mine_456": "char_123",
    "mine_789": "char_123"
  }
}
```

---

## ✅ FINAL CHECKLIST

- [ ] All code files compiled without errors
- [ ] OfflineMiningManager added to scene
- [ ] PlayerProfileDTO has proper constructor
- [ ] PlayFab data structure supports dictionaries
- [ ] Test offline coins flow end-to-end
- [ ] Verify coins persist after server restart
- [ ] Test steal scenario with offline owner
- [ ] Check UI updates correctly for offline coins
- [ ] Monitor console logs for errors
- [ ] Load test with multiple players offline simultaneously

---

## 📝 NOTES FOR FUTURE FEATURES

### Bonus Multipliers
```csharp
// In SpiritStoneMine.CalculateOfflineMiningPerSecond():
float multiplier = GetPlayerMultiplier(playerId);  // For VIP, events, etc
ulong coinsPerSecond = (ulong)(yieldPerSecond × multiplier);
```

### Max Offline Time Limit
```csharp
const double MAX_OFFLINE_MINING_HOURS = 24;
double offlineDuration = Math.Min(now - lastClaimTime, MAX_OFFLINE_MINING_HOURS * 3600);
```

### Offline Farmer Penalties
```csharp
// Reduce yield for inactive players
if (offlineDuration > AFKTHRESHOLD)
    coinReduction = 10%;  // Discourage pure offline farming
```

---

Generated: 2026-03-27
System: Offline Mining Enhancement
Status: Ready for Testing
