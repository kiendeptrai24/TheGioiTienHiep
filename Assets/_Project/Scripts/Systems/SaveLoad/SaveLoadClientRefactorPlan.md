# SaveLoad Client Refactor Plan

## Mục tiêu

- Giảm số lượng request `GetUserData` của client càng nhiều càng tốt.
- Tạo một request duy nhất để load toàn bộ dữ liệu liên quan tới `characterId`.
- Giữ lại cấu trúc phân tách các phần dữ liệu (`hero inventory`, `inventory`, `inventory used`, `team`, `profile`) nhưng tải chúng trong một lần gọi chung.
- Dựng client theo kiểu giống `SaveLoadServer` trong `AllGameDataSerice.cs`.

## Hiện trạng

### Client

Client hiện tại gọi nhiều service khác nhau trong `PlayfabDataManager.OnCharacterLoaded`:

- `ProfileService` -> `service.LoadProfile(characterId, callback)`
- `ShopClientService` -> `service.LoadShopData(callback)`
- `PlayerUsedItemInventoryService` -> `service.LoadPlayerDatasUsed(characterId, callback)`
- `TeamInventoryService` -> `service.LoadTeamData(characterId, callback)`
- `PlayerItemInventoryService` -> `service.LoadPlayerData(characterId, callback)`
- `PlayerHeroItemInventoryService` -> `service.LoadPlayerHeroData(characterId, callback)`
- `ItemCharacterService` -> `service.LoadCharacter(callback)`

Các phương thức `LoadPlayerHeroData`, `LoadPlayerData`, `LoadPlayerDatasUsed`, `LoadTeamData`, `LoadProfile` đều gọi `LoadUserData(...)` riêng biệt, dẫn đến nhiều lần `GetUserData`.

### Server

`AllGameDataSerice.cs` chỉ gọi `service.LoadAllGameData(...)` một lần, rồi phân phối kết quả cho các `ILoadGameData`
- `LoadRealmData`
- `LoadEssenceAndRaceData`
- `LoadEquipmentData`
- `LoadChampionData`
- `LoadCharacterData`
- `LoadShopData`

Đây là mẫu tốt: chỉ 1 request, sau đó chia nhỏ dữ liệu.

## Đề xuất kiến trúc refactor client

### 1. Tạo DTO gộp cho player save data

Trong `Assets/_Project/Scripts/Systems/SaveLoad/Data/PlayerSaveDataDTO.cs`:

- `HeroDataDTO heroInventory`
- `ItemDataDTO inventory`
- `ItemDataDTO inventoryUsed`
- `HeroInTeamDataDTO team`
- `PlayerProfileDTO profile`

Nó sẽ là payload chung khi load dữ liệu player theo `characterId`.

### 2. Thay đổi `PlayFabDataClientService`

- Giữ các phương thức public cũ để không phá vỡ interface:
  - `LoadPlayerHeroData`
  - `LoadPlayerData`
  - `LoadPlayerDatasUsed`
  - `LoadTeamData`
  - `LoadProfile`
  - `LoadCharacter`

- Cơ chế mới:
  - `LoadPlayerSaveData(string characterId, Action<PlayerSaveDataDTO> callback)`
  - Chỉ gọi `clientApi.GetUserData(new GetUserDataRequest(), ...)` một lần
  - Trong callback, giải mã/deserialize về `PlayerSaveDataDTO`
  - Phân phối:
    - `heroInventory` cho `LoadPlayerHeroData`
    - `inventory` cho `LoadPlayerData`
    - `inventoryUsed` cho `LoadPlayerDatasUsed`
    - `team` cho `LoadTeamData`
    - `profile` cho `LoadProfile`

- Nếu cần, thêm cache tạm cho cùng `characterId` để tránh load lại trong cùng phiên làm việc.

### 3. Giữ `LoadCharacter` nguyên trạng

- `LoadCharacter` vẫn gọi `LoadUserData("character", callback)` vì dữ liệu character list không cùng dạng với player-specific `characterId`.

### 4. Điều chỉnh `PlayfabDataManager` hoặc các service nếu cần

- Các service client hiện tại có thể vẫn dùng interface cũ.
- Nếu muốn tối ưu hơn, có thể thêm `IPlayerSaveDataClient` hoặc `ILoadSaveRemoteClient` để `PlayFabDataClientService` implement.

## Các bước cài đặt cụ thể

1. Tạo/kiểm tra `PlayerSaveDataDTO.cs` trong `SaveLoad/Data/`.
2. Sửa `PlayFabDataClientService`:
   - thêm các biến trạng thái cache/đang load
   - thêm `LoadPlayerSaveData` và `CreatePlayerSaveDataDto`
   - redirect các load public hiện có vào batch loader
3. Giữ `SaveUserData` hiện tại không thay đổi, chỉ thêm invalidate cache nếu cần.
4. Chạy kiểm tra compile.
5. Nếu cần, thêm unit test hoặc debug logging cho route batch mới.

## Tài liệu tham khảo

- `Assets/_Project/Scripts/Systems/SaveLoad/SaveLoadServer/AllGameDataSerice.cs`
- `Assets/_Project/Scripts/Systems/SaveLoad/PlayfabDataManager.cs`
- `Assets/_Project/Scripts/Systems/SaveLoad/SaveLoadClient/ProfileService.cs`
- `Assets/_Project/Scripts/Systems/SaveLoad/SaveLoadClient/PlayerHeroItemInventoryService.cs`

## Ghi chú

- Nếu `GetUserData` trả toàn bộ UserData, client batch có thể tận dụng chung kết quả.
- Nếu chỉ trả partial theo key, vẫn cần chỉ gọi 1 `GetUserData` và parse nhiều key.
- Mục tiêu là: 1 request `GetUserData` cho player `characterId` + 1 request `GetUserData` cho `character` list riêng (nếu cần).
