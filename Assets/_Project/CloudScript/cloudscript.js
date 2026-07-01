// ─────────────────────────────────────────────────────────────────────────────
//  Session CloudScript
//  Server tự xác định user qua currentPlayerId (token PlayFab).
//  Client KHÔNG gửi userId/playFabId.
//
//  Hàm:
//    CreateSession    – tạo sessionId mới sau khi đăng nhập
//    SessionHeartbeat – kiểm tra session hợp lệ mỗi 2 giây
//    LogoutSession    – xóa sessionId, xóa lastHeartbeat
// ─────────────────────────────────────────────────────────────────────────────

var SESSION_KEY_ID = "sessionId";
var SESSION_KEY_HEARTBEAT = "lastHeartbeat";

// Ngưỡng xác định "đang online" khi CreateSession: 5 giây (2.5× heartbeat interval).
// Nếu lastHeartbeat cách hiện tại ≤ ONLINE_THRESHOLD_SECONDS → tài khoản vẫn online.
var ONLINE_THRESHOLD_SECONDS = 2;

// ── Helpers ──────────────────────────────────────────────────────────────────

function readUserData() {
    return server.GetUserInternalData({ PlayFabId: currentPlayerId });
}

function getField(data, key, defaultValue) {
    if (!data || !data.Data || !data.Data[key] || data.Data[key].Value === undefined) {
        return defaultValue;
    }
    return data.Data[key].Value;
}

function generateSessionId() {
    return "xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx".replace(/[xy]/g, function (c) {
        var r = Math.random() * 16 | 0;
        return (c === "x" ? r : (r & 0x3 | 0x8)).toString(16);
    });
}

function writeSession(sessionId) {
    var data = { lastHeartbeat: new Date().toISOString() };
    if (sessionId !== null) data[SESSION_KEY_ID] = sessionId;
    server.UpdateUserInternalData({ PlayFabId: currentPlayerId, Data: data });
}

// ── CreateSession ─────────────────────────────────────────────────────────────
//  Luồng:
//  1. Đọc trạng thái hiện tại của user.
//  2. Nếu isOnline=true VÀ session chưa stale → trả shouldWait=true.
//     Client sẽ chờ 3 giây rồi gọi lại.
//  3. Nếu isOnline=false HOẶC session stale → tạo sessionId mới, ghi đè,
//     trả sessionId mới về client.

handlers.CreateSession = function (args) {
    var data = readUserData();
    var storedSessionId = getField(data, SESSION_KEY_ID, "");
    var lastHeartbeat = getField(data, SESSION_KEY_HEARTBEAT, "");
    var forceOverride = !!(args && args.forceOverride);

    // Tài khoản đang online nếu có sessionId và heartbeat gần đây (≤ ONLINE_THRESHOLD_SECONDS)
    var isCurrentlyOnline = storedSessionId !== "" && lastHeartbeat !== "" &&
        ((new Date() - new Date(lastHeartbeat)) / 1000) <= ONLINE_THRESHOLD_SECONDS;

    // Tạo sessionId mới, ghi đè session cũ
    var newSessionId = generateSessionId();
    writeSession(newSessionId);

    return {
        success: true,
        sessionId: newSessionId,
        shouldWait: isCurrentlyOnline && !forceOverride,
        message: isCurrentlyOnline && !forceOverride
            ? "Tài khoản đang online ở thiết bị khác. Đang chờ..."
            : ""
    };
};

// ── SessionHeartbeat ──────────────────────────────────────────────────────────
//  Client gửi: { sessionId, isOnline }
//  Server so sánh sessionId với sessionId đang lưu.
//  Nếu khác → shouldLogout=true (thiết bị khác đã đăng nhập và ghi đè).

handlers.SessionHeartbeat = function (args) {
    if (!args || !args.sessionId) {
        return { isValid: false, shouldLogout: true, reason: "SESSION_ID_MISSING" };
    }

    var data = readUserData();
    var storedSessionId = getField(data, SESSION_KEY_ID, "");

    // sessionId không khớp → session bị ghi đè bởi thiết bị khác → kick ngay
    if (storedSessionId !== args.sessionId) {
        return { isValid: false, shouldLogout: true, reason: "SESSION_REVOKED" };
    }

    // Hợp lệ → cập nhật lastHeartbeat (server dùng timestamp này để tính online)
    writeSession(null);

    return { isValid: true, shouldLogout: false, reason: "" };
};

// ── LogoutSession ─────────────────────────────────────────────────────────────
//  Client gửi: { sessionId }
//  Server đặt isOnline=false và xóa sessionId.
//  Chỉ xóa nếu sessionId khớp (tránh xóa phiên của thiết bị khác).

handlers.LogoutSession = function (args) {
    if (!args || !args.sessionId) {
        return { success: false };
    }

    var data = readUserData();
    var storedSessionId = getField(data, SESSION_KEY_ID, "");

    // Chỉ logout nếu đúng session này đang giữ lock
    if (storedSessionId === args.sessionId) {
        server.UpdateUserInternalData({
            PlayFabId: currentPlayerId,
            Data: {
                sessionId: "",
                lastHeartbeat: ""
            }
        });
    }

    return { success: true };
};

// ── DeleteAllInternalData (debug only) ───────────────────────────────────────

handlers.DeleteAllInternalData = function () {
    server.UpdateUserInternalData({
        PlayFabId: currentPlayerId,
        Data: { sessionId: null, lastHeartbeat: null }
    });
    return { success: true, message: "Đã xóa toàn bộ internal data." };
};
