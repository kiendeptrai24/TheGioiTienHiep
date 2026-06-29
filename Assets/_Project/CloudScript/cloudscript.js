var SESSION_KEYS = {
    sessionId: "sessionId",
    isOnline: "isOnline",
    lastHeartbeat: "lastHeartbeat"
};

var STALE_SESSION_TIMEOUT_SECONDS = 60;
var FORCE_TAKEOVER_WAIT_SECONDS = 5;
var SESSION_STATUS_ACTIVE = "ACTIVE";
var SESSION_STATUS_WAITING = "WAITING";

function getInternalValue(data, key, defaultValue) {
    if (!data || !data.Data || !data.Data[key] || data.Data[key].Value === undefined || data.Data[key].Value === null) {
        return defaultValue;
    }

    return data.Data[key].Value;
}

function isSessionStale(lastHeartbeat, now) {
    if (!lastHeartbeat) {
        return true;
    }

    var diffSeconds = (new Date(now) - new Date(lastHeartbeat)) / 1000;
    return diffSeconds > STALE_SESSION_TIMEOUT_SECONDS;
}

function hasWaitedLongEnough(requestStartedAt, now) {
    if (!requestStartedAt) {
        return false;
    }

    var diffSeconds = (new Date(now) - new Date(requestStartedAt)) / 1000;
    return diffSeconds >= FORCE_TAKEOVER_WAIT_SECONDS;
}

function buildSessionResponse(success, status, previousSessionId, activeSessionId, message, errorCode) {
    return {
        success: success,
        status: status,
        kickedPreviousSession: false,
        previousSessionId: previousSessionId || "",
        activeSessionId: activeSessionId || "",
        message: message || "",
        errorCode: errorCode || ""
    };
}

handlers.RequestSession = function (args, context) {
    if (!args || !args.sessionId) {
        throw "sessionId is required";
    }

    var now = new Date().toISOString();
    var data = server.GetUserInternalData({
        PlayFabId: currentPlayerId
    });

    var currentSessionId = getInternalValue(data, SESSION_KEYS.sessionId, "");
    var currentIsOnline = getInternalValue(data, SESSION_KEYS.isOnline, "false") === "true";
    var currentLastHeartbeat = getInternalValue(data, SESSION_KEYS.lastHeartbeat, "");
    var currentSessionIsActive = currentIsOnline && currentSessionId !== "" && !isSessionStale(currentLastHeartbeat, now);

    if (currentSessionId === args.sessionId && currentIsOnline) {
        server.UpdateUserInternalData({
            PlayFabId: currentPlayerId,
            Data: {
                sessionId: args.sessionId,
                isOnline: "true",
                lastHeartbeat: now
            }
        });

        return buildSessionResponse(true, SESSION_STATUS_ACTIVE, "", args.sessionId, "", "");
    }

    if (currentSessionIsActive && !hasWaitedLongEnough(args.requestStartedAt, now)) {
        return buildSessionResponse(false, SESSION_STATUS_WAITING, currentSessionId, "", "Dang doi phien cu dong bo va dang xuat.", "");
    }

    var kickedPrevious = currentIsOnline && currentSessionId !== "" && currentSessionId !== args.sessionId;
    server.UpdateUserInternalData({
        PlayFabId: currentPlayerId,
        Data: {
            sessionId: args.sessionId,
            isOnline: "true",
            lastHeartbeat: now
        }
    });

    var activeResponse = buildSessionResponse(true, SESSION_STATUS_ACTIVE, currentSessionId, args.sessionId, "", "");
    activeResponse.kickedPreviousSession = kickedPrevious;
    return activeResponse;
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
    var lastHeartbeat = getInternalValue(data, SESSION_KEYS.lastHeartbeat, "");

    if (!isOnline || currentSessionId === "" || currentSessionId !== args.sessionId || isSessionStale(lastHeartbeat, now)) {
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
            released: false,
            pendingActivated: false
        };
    }

    server.UpdateUserInternalData({
        PlayFabId: currentPlayerId,
        Data: {
            sessionId: "",
            isOnline: "false",
            lastHeartbeat: ""
        }
    });

    return {
        released: true,
        pendingActivated: false
    };
};

handlers.DeleteAllInternalData = function (args, context) {
    server.UpdateUserInternalData({
        PlayFabId: currentPlayerId,
        Data: {
            sessionId: null,
            isOnline: null,
            lastHeartbeat: null
        }
    });

    return {
        success: true,
        message: "Da xoa tat ca Internal data"
    };
};
