# Hướng dẫn & Workflow Dự án Thế Giới Tiên Hiệp (The Gioi Tien Hiep)

Tài liệu này cung cấp cái nhìn tổng quan về kiến trúc, các hệ thống cốt lõi và quy trình làm việc (workflow) khi phát triển dự án "Thế Giới Tiên Hiệp".

## 1. Cấu trúc thư mục chính (`Assets/_Project`)

Dự án được tổ chức theo module, chủ yếu nằm trong thư mục `Assets/_Project`:

- **Data/**: Chứa ScriptableObjects, dữ liệu tĩnh của game.
- **Scenes/**: Chứa các scene của game (LoadingScene, SampleScene, BattleMap, MapDev, UI,...).
- **Scripts/**: Chứa toàn bộ logic code của game.
  - **Authen/**: Hệ thống xác thực người dùng (đăng nhập/đăng ký/quên mật khẩu).
  - **Core/**: Các script cốt lõi (Singleton, ObjectPool, CameraManager).
  - **Gameplay/**: Logic trò chơi (Nhân vật, Kỹ năng, Inventory, Battle, Stats,...).
  - **Managers/**: Các trình quản lý tổng thể.
  - **Networking/**: Logic mạng cơ bản (chạy PlayFab Multiplayer / Photon nếu có).
  - **Playfab/**: Cấu hình và dịch vụ liên quan đến PlayFab (Lobby, Matchmaking,...).
  - **Scene/**: Quản lý chuyển cảnh, nạp/xả Scene (SceneLoadManager, ScreenManager).
  - **Server/**: Quản lý và nạp dữ liệu từ máy chủ (GameDataCenterManager).
  - **Systems/**: Các hệ thống như Save/Load.
  - **UI/**: Các script quản lý giao diện người dùng (Popup, Billboard, InWorlds, Buttons,...).

---

## 2. Các Hệ thống Chính (Core Systems)

### 2.1. Xác thực & Dữ liệu Người chơi (PlayFab)
- **AuthManager (`Authen/AuthManager.cs`)**: Lớp Wrapper quản lý luồng đăng nhập (AutoLogin, Login, Register). Giao tiếp thông qua interface `IAuthService` (được triển khai bởi `PlayFabAuthService`).
- **Lưu/Tải Dữ liệu (`Systems/SaveLoad/`)**: Hỗ trợ lưu trữ dữ liệu người chơi đa nền tảng thông qua các class như `SaveLoadJson`, `SaveLoadOS`, `SaveLoadPlayfab`.

### 2.2. Quản lý Dữ liệu Game (Game Data Center)
- **GameDataCenterManager (`Server/GameDataCenterManager.cs`)**: Là trái tim của hệ thống dữ liệu tĩnh. Quản lý toàn bộ danh sách item, nhân vật, trang bị, kỹ năng, cảnh giới (realm), v.v.
- Hoạt động: So sánh `game_data_version` giữa máy chủ (PlayFab Title Data) và Local Cache. Nếu giống nhau thì load từ thiết bị, nếu khác sẽ tải file Json cấu hình từ PlayFab, cập nhật và lưu cache.
- Mọi truy xuất item nên thông qua `GameDataCenterManager.Instance.GetItemById(id)`.

### 2.3. Gameplay: State Machine (FSM)
- Hệ thống nhân vật (Hero, Player, Champion, Enemy) được xây dựng dựa trên mô hình **State Machine**.
- Mỗi loại đối tượng có một Machine riêng (VD: `HeroStateMachine`, `PlayerStateMachine`) và một Factory quản lý trạng thái (`HeroStateFactory`, `PlayerStateFactory`).
- **Workflow**: Nếu muốn thêm hành động mới (VD: "Stunned"), cần tạo một lớp kế thừa State cơ sở, thêm logic `Enter()`, `Update()`, `Exit()` và đăng ký vào Factory.

### 2.4. Gameplay: Chỉ số (Stats) & Túi đồ (Inventory)
- **Hệ thống Stat (`Stats/Stat.cs`)**: Quản lý điểm chỉ số (HP, ATK, DEF...). Có khả năng cộng dồn hoặc tính phần trăm từ các Modifier (`StatsEquipmentModifier`, `StatsRealmModifier`, `StatsSkillModifier`).
- **InventoryCenterManager**: Quản lý trang bị, túi đồ của người chơi. Tương tác chặt chẽ với hệ thống Stat để cập nhật chỉ số khi mặc/tháo đồ.

### 2.5. Gameplay: Kỹ năng (Skills) & Trận chiến (Battle)
- **Skills**: Được thiết kế dưới dạng Component/ScriptableObject linh hoạt (`SkillContext`, `SkillController`). Bao gồm Caster (người tung chiêu), Target (mục tiêu) và Condition (điều kiện cast).
- **BattleScene**: Chia thành **BattleRealTime** (Đánh thời gian thực trên map) và **BattleSimulator** (Mô phỏng trận đấu).

---

## 3. Workflow Phát Triển (Development Workflow)

### 3.1. Thêm/Sửa Dữ liệu Item, Trang Bị, Kỹ Năng
1. **Dữ liệu cấu hình** thường được lưu dưới dạng JSON hoặc ScriptableObject.
2. Nếu thêm Item mới:
   - Khai báo ID, Name, Stats trong file cấu hình JSON trên **PlayFab Title Data** (hoặc cấu hình local tuỳ theo môi trường build: `LOCAL_CLIENT` vs `REMOTE_CLIENT`).
   - Khởi động game, hệ thống sẽ tải JSON từ PlayFab về, map dữ liệu qua `GameDataCenterManager` và lưu vào cache.
   - Gọi `GameDataCenterManager.Instance.GetItemById("new_item_id")` để sử dụng trong game.

### 3.2. Tạo Trạng thái (State) mới cho Nhân Vật
1. Xác định nhân vật cần thêm (Player, Hero, Enemy...).
2. Vào thư mục `Scripts/Gameplay/[Tên_Nhân_Vật]/States`.
3. Tạo script kế thừa lớp base State của hệ thống đó.
4. Mở `[Tên_Nhân_Vật]StateFactory` (ví dụ `PlayerStateFactory`) để khởi tạo instance của State vừa tạo.
5. Định nghĩa luồng chuyển trạng thái (Transition) trong các State liên quan.

### 3.3. Xử lý Giao Diện (UI)
1. Tạo UI Prefab.
2. Gắn Script UI (ví dụ `Popup/MyNewPopup.cs`).
3. Sử dụng `ScreenManager` để Pop/Push các màn hình (Quản lý theo dạng Stack, màn hình này đè lên màn hình kia, dễ dàng ấn nút 'Back' hoặc 'Đóng').
4. Bind data thông qua các Event hoặc thao tác trực tiếp với Manager (VD: Lấy data từ `InventoryCenterManager`).

### 3.4. Lưu Dữ Liệu Của Người Chơi
1. Khi có thay đổi trạng thái (Ví dụ: Nhận Vàng, Nhận EXP).
2. Update dữ liệu ở logic máy khách (hoặc máy chủ mô phỏng).
3. Gọi thông qua hệ thống **SaveLoad** (`SaveLoadManager`) để lưu file xuống OS (Offline) hoặc đồng bộ lên `PlayfabDataManager` (Online).

---

## 4. Best Practices & Quy Tắc
- **Tránh việc hardcode ID**: Các ID của Item/Skill nên được tạo dạng hằng số (Const) hoặc Enum nếu ít, nếu nhiều thì quản lý trên file JSON/DataCenter.
- **Quản lý Event Cẩn Thận**: Khi đăng ký Event (Action, Delegate), đặc biệt là trong UI, LUÔN LUÔN phải huỷ đăng ký (Unsubscribe) trong hàm `OnDestroy` hoặc `OnDisable` để tránh rò rỉ bộ nhớ (Memory Leak).
- **Tuân thủ Singleton**: Các Manager cốt lõi đã kế thừa base `Singleton<T>`. Gọi qua `ClassName.Instance.Method()`. Tránh gọi ở hàm `Awake()` của script khác nếu không chắc chắn Singleton đã khởi tạo, nên ưu tiên dùng ở `Start()`.
- **UI & Logic Tách Biệt**: UI chỉ nên làm nhiệm vụ hiển thị (View) và nhận thao tác. Mọi logic tính toán (Cộng tiền, kiểm tra điều kiện trừ máu) nên đẩy về Manager / Hệ thống (Model/Controller).
