var SESSION_KEYS = {
    sessionId: "sessionId",
    isOnline: "isOnline",
    lastHeartbeat: "lastHeartbeat",
    loginAt: "loginAt"
};

function getInternalValue(data, key, defaultValue) {
    if (!data || !data.Data || !data.Data[key] || data.Data[key].Value === undefined || data.Data[key].Value === null) {
        return defaultValue;
    }

    return data.Data[key].Value;
}

handlers.RequestSession = function (args, context) {
    if (!args || !args.sessionId) {
        throw "sessionId is required";
    }

    var now = new Date().toISOString();
    var data = server.GetUserInternalData({
        PlayFabId: currentPlayerId
    });

    var previousSessionId = getInternalValue(data, SESSION_KEYS.sessionId, "");
    var previousIsOnline = getInternalValue(data, SESSION_KEYS.isOnline, "false") === "true";
    var kickedPreviousSession = previousIsOnline && previousSessionId !== "" && previousSessionId !== args.sessionId;

    server.UpdateUserInternalData({
        PlayFabId: currentPlayerId,
        Data: {
            sessionId: args.sessionId,
            isOnline: "true",
            loginAt: now,
            lastHeartbeat: now
        }
    });

    return {
        success: true,
        kickedPreviousSession: kickedPreviousSession,
        previousSessionId: previousSessionId,
        activeSessionId: args.sessionId
    };
};

handlers.Heartbeat = function (args, context) {
    if (!args || !args.sessionId) {
        throw "sessionId is required";
    }

    var now = new Date().toISOString();
    var data = server.GetUserInternalData({
        PlayFabId: currentPlayerId
    });

    var currentSessionId = getInternalValue(data, SESSION_KEYS.sessionId, "");
    var isOnline = getInternalValue(data, SESSION_KEYS.isOnline, "false") === "true";

    if (!isOnline || currentSessionId === "" || currentSessionId !== args.sessionId) {
        return {
            valid: false,
            shouldLogout: true,
            reason: "SESSION_REVOKED"
        };
    }

    server.UpdateUserInternalData({
        PlayFabId: currentPlayerId,
        Data: {
            lastHeartbeat: now
        }
    });

    return {
        valid: true,
        shouldLogout: false,
        reason: ""
    };
};

handlers.ReleaseSession = function (args, context) {
    if (!args || !args.sessionId) {
        throw "sessionId is required";
    }

    var data = server.GetUserInternalData({
        PlayFabId: currentPlayerId
    });

    var currentSessionId = getInternalValue(data, SESSION_KEYS.sessionId, "");

    if (currentSessionId !== args.sessionId) {
        return {
            released: false
        };
    }

    server.UpdateUserInternalData({
        PlayFabId: currentPlayerId,
        Data: {
            sessionId: "",
            isOnline: "false"
        }
    });

    return {
        released: true
    };
};
