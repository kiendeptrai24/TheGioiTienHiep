# RECONNECT FLOW - Mine Re-linking System

## 🎯 OVERVIEW

Hệ thống permet players tiếp tục khai thác mỏ sau khi offline:

```
┌─────────────────────────────────────────────────────────────────┐
│                   PLAYER RECONNECT FLOW                         │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  1. CONNECT PHASE                                               │
│     Client → PlayFab Authentication                             │
│     PlayFab → Server: Load GameData (mines list)               │
│                                                                 │
│  2. DATA LOAD PHASE                                             │
│     SaveLoadPlayfab.OnDataReadyToLoad event triggered          │
│     OfflineMiningManager.HandleOfflineCoinsOnLoad() called     │
│                                                                 │
│  3. RE-LINK PHASE                                               │
│     OfflineMiningManager sends RequestMineRelinkServerRpc      │
│     → PlayerMineRelinker processes each mine:                  │
│        - Check if mine still exists                            │
│        - Check if mine is claimed by player                    │
│        - If free: RE-LINK player + add pending coins           │
│        - If stolen: Calculate stolen coins + add coins         │
│                                                                 │
│  4. COMPLETION PHASE                                            │
│     Server returns RelinkData results                          │
│     Client resumes mining or receives coins                    │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

---

## 📋 DETAILED FLOW

### Phase 1: CONNECT (PlayFab Authentication)
```
Client.Authenticate() → PlayFab Server
                            ↓
                    Load Player Profile
                    Load Mine Offline Data
                            ↓
                    PlayfabDataManager.OnLoadGameFormPlayfab
                            ↓
                    SaveLoadPlayfab.OnItemPlayerLoad(GameData)
                            ↓
                    SaveLoadPlayfab.OnDataReadyToLoad.Invoke()
```

### Phase 2: DATA READY (On Load Trigger)
```
OfflineMiningManager.HandleOfflineCoinsOnLoad(GameData)
    │
    ├─ Check if mineOfflineDataList has mines
    │
    └─ StartCoroutine(ProcessMineRelinking(gameData))
           │
           ├─ Wait for network ready
           ├─ Wait for player object to spawn  
           ├─ Collect mine IDs from mineOfflineDataList
           └─ RequestMineRelinking(minesToRelink)
```

### Phase 3: RE-LINK REQUEST (RPC to Server)
```
OfflineMiningManager.RequestMineRelinking(mineNetworkIds)
       │
       └─ PlayerMineRelinker.RequestMineRelinkServerRpc(mineNetworkIds)
              │
              └─ [SERVER SIDE]
                 For each mine:
                    ProcessMineRelink(mine, playerObject, clientId)
```

### Phase 4: SERVER PROCESSING (Mine Validation)
```
ProcessMineRelink(spiritMine, playerNetObject, clientId)
    │
    ├─ Check: mine.HasOwner() ?
    │
    ├─ NO OWNER (Mine is FREE)
    │  │
    │  ├─ mine.SetOwner(playerNetObject.NetworkObjectId)
    │  │  └─ Re-link player to mine
    │  │
    │  ├─ pendingCoins = mineData.accumulatedOfflineCoins
    │  │
    │  ├─ storage.AddOfflineCoins(pendingCoins)
    │  │  └─ Sync via NetworkVariable to client
    │  │
    │  └─ mineData.accumulatedOfflineCoins = 0  // Clear mine data
    │
    │
    ├─ HAS OWNER (Mine is STOLEN)
    │  │
    │  ├─ stolenCoins = CalculateStolenCoins(mineData)
    │  │  └─ = yieldPerSecond × (now - lastOwnerClaimTime)
    │  │
    │  ├─ storage.AddOfflineCoins(stolenCoins)
    │  │  └─ Sync via NetworkVariable to client
    │  │
    │  ├─ mineData.accumulatedOfflineCoins = 0  // Clear mine data
    │  │
    │  └─ Don't re-link (mine stays with thief)
    │
    └─ Return MineRelinkData
           └─ {mineId, relinked, pendingCoins, stolenByPlayerId}
```

---

## 🔑 KEY COMPONENTS

### 1. **OfflineMiningManager** (Reconnect Coordinator)
```csharp
// On data load, triggers re-linking
HandleOfflineCoinsOnLoad(GameData)

// Coroutine to process relinking
ProcessMineRelinking(GameData)

// Sends RPC request to server
RequestMineRelinking(ulong[] mineNetworkIds)

// Utility methods
GetTotalOfflineCoins(GameData)
GetMineOfflineCoins(GameData, mineId)
AddMineOfflineCoins(GameData, mineId, amount, owner)
ClearMineOfflineCoins(GameData, mineId)
```

### 2. **PlayerMineRelinker** (Server Mine Validator)
```csharp
// RPC: Client requests mine relinking
RequestMineRelinkServerRpc(ulong[] mineNetworkObjectIds)

// Process single mine
ProcessMineRelink(SpiritStoneMine, NetworkObject player, ulong clientId)

// Calculate stolen coins
CalculateStolenCoins(ItemResourseData mineData)

// Check mine ownership
IsMineOwnedByPlayer(SpiritStoneMine, NetworkObject player)

// Find all active mines
GetAllActiveMines()
```

### 3. **Data Classes**
```csharp
// Per-mine offline data
public class MineOfflineData
{
    public ulong mineId;                    // Network ID
    public ulong accumulatedOfflineCoins;   // Pending coins
    public double lastClaimTime;            // Server time last claimed
    public string playerId;                 // Owner character ID
}

// List wrapper (JSON serializable)
public class MineOfflineDataList
{
    public List<MineOfflineData> mines;
    
    // Methods: GetMine(), AddOrUpdate(), Remove(), Clear()
}
```

---

## 🔄 EXAMPLE SCENARIOS

### Scenario 1: Player Offline 1 Hour, Mine Still Free
```
Last Claim Time: T0
Offline Duration: 1 hour (3600 seconds)
Mine State: No current owner
Yield Per Harvest: 100 coins
Mining Time: 10 seconds
Yield Per Second: 100/10 = 10 coins/sec

Calculation:
Pending Coins = 10 coins/sec × 3600 sec = 36,000 coins

Action:
1. Server calls mine.SetOwner(playerObject)
2. Server calls storage.AddOfflineCoins(36000)
3. Player receives 36,000 coins
4. Player continues mining
```

### Scenario 2: Player Offline 1 Hour, Mine Stolen
```
Last Claim Time: T0
Player Goes Offline: T0 + 30 min
Other Player Steals: T0 + 45 min
Player Comes Back Online: T0 + 60 min

Calculation for Offline Player:
- Mining period: T0 + 30min → T0 + 45min = 15 minutes
- Stolen Coins = 10 coins/sec × 900 sec = 9,000 coins

Action:
1. Server detects mine has owner (thief)
2. Server calculates stolenCoins = 9,000
3. Server calls storage.AddOfflineCoins(9000)
4. Player receives 9,000 coins
5. Mine stays with thief (not re-linked)
```

---

## 🛠️ INTEGRATION CHECKLIST

- [ ] **PlayerMineRelinker** added to Server scene
- [ ] **OfflineMiningManager** added to persistent scene
- [ ] **MineOfflineData.cs** compiled & referenced
- [ ] **SaveLoadPlayfab.OnDataReadyToLoad** properly fires
- [ ] **PlayFab ProfileService** saves/loads mineOfflineDataList
- [ ] **SpiritStoneMine** methods working:
  - [ ] `SetOwner()` - Re-links owner
  - [ ] `HasOwner()` - Checks if claimed
  - [ ] `GetItemResourseData()` - Returns mining data
- [ ] **ResourceStorage.AddOfflineCoins()** method exists
- [ ] Network communication working:
  - [ ] RPC calls functional
  - [ ] NetworkVariable coins syncs to clients

---

## 🔍 DEBUG LOGS

```csharp
// OfflineMiningManager
[OfflineMiningManager] No offline data to process
[OfflineMiningManager] Processing X mines for reconnect
[OfflineMiningManager] Requesting relink for X mines
[OfflineMiningManager] PlayerMineRelinker not found in scene

// PlayerMineRelinker
[PlayerMineRelinker] SetOwner: {netId}, offlineStart={time}
[PlayerMineRelinker] Mine {id} is free, re-linking player {clientId}
[PlayerMineRelinker] Mine {id} is owned by another player
[PlayerMineRelinker] Calculated stolen coins: {coins} (duration: {sec}s)
[PlayerMineRelinker] Player {clientId} received {coins} stolen coins
[PlayerMineRelinker] Processed {count} mines for player {clientId}

// SpiritStoneMine
[SpiritStoneMine] Owner is back online!
[OfflineCoins] Added {amount} coins. Total: {total}
```

---

## ⚠️ IMPORTANT NOTES

### Server Time Authority
- Always use `NetworkManager.ServerTime.Time` for calculations
- Never trust client clock
- Prevents cheating via clock manipulation

### Coin Sync
- Coins are sent via `ResourceStorage.AddOfflineCoins()`
- Triggers `NetworkVariable<ulong> Coins` update
- All clients receive event: `OnCoinsChanged`

### Data Persistence
- Pending coins in mine saved to PlayFab
- On reconnect, server calculates new coins based on server time
- Even if server restarts, coins are in PlayFab

### Edge Cases
1. **Mine Deleted**: Server checks if spawn object still exists
2. **Multiple Players**: Each get their own calculation
3. **Network Issues**: RPC will retry or timeout gracefully
4. **Concurrent Access**: Server-side validation prevents conflicts

---

## 🚀 FUTURE ENHANCEMENTS

### 1. Multiplier Support
```csharp
float multiplier = GetPlayerMultiplier(playerId);  // VIP, events, etc
ulong bonusCoins = (ulong)(baseCoins × (multiplier - 1));
```

### 2. Max Offline Mining Cap
```csharp
const double MAX_OFFLINE_HOURS = 24;
double cappedDuration = Math.Min(actualDuration, MAX_OFFLINE_HOURS * 3600);
```

### 3. Afk Penalty
```csharp
if (realTimeDaysOffline > 7)
    yield_reduction = 0.5f;  // 50% reduction
```

### 4. Mining Boost Events
```csharp
if (IsEventActive("DoubleCoins"))
    coins *= 2;
```

---

## 📊 DATA FLOW DIAGRAM

```
PlayFab Server
     │
     ├─ profile data (coins, character)
     └─ mineOfflineDataList (mines with pending coins)
             │
             ▼
         GameData object
             │
             ├─ coins (regular + offline bonus)
             └─ mineOfflineDataList
                      │
                      ▼
            MineOfflineDataList
                      │
                      ├─ Mine 1: 5000 coins
                      ├─ Mine 2: 3000 coins
                      └─ Mine 3: 0 coins (just stolen last min)
                           
                           
On Reconnect:
GameData → OfflineMiningManager
    │
    ├─ Extract Mine IDs
    │
    └─ RequestMineRelinkServerRpc()
             │
             ▼
      PlayerMineRelinker (Server)
             │
             ├─ Check Mine 1: FREE
             │  └─ Re-link + Add 5000 coins
             │
             ├─ Check Mine 2: STOLEN
             │  └─ Calculate + Add 2500 coins (partial)
             │
             └─ Check Mine 3: STOLEN
                └─ Calculate + Add 0 coins (just stolen)


Result: Player gets coins + re-linked to free mines
```

---

**Status**: ✅ Implementation Complete - Ready for Testing

**Created**: 2026-03-27  
**System**: Offline Mining - Reconnect Flow
