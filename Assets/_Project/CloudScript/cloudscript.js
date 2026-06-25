var SESSION_KEYS = {
    sessionId: "sessionId",
    isOnline: "isOnline",
    lastHeartbeat: "lastHeartbeat",
    loginAt: "loginAt",
    pendingSessionId: "pendingSessionId",
    pendingSince: "pendingSince"
};

var STALE_SESSION_TIMEOUT_SECONDS = 60;
var TAKEOVER_WAIT_TIMEOUT_SECONDS = 90;
var SESSION_STATUS_ACTIVE = "ACTIVE";
var SESSION_STATUS_WAITING = "WAITING";
var SESSION_STATUS_ERROR = "ERROR";
var SESSION_STATUS_TIMEOUT = "TIMEOUT";

function getInternalValue(data, key, defaultValue) {
    if (!data || !data.Data || !data.Data[key] || data.Data[key].Value === undefined || data.Data[key].Value === null) {
        return defaultValue;
    }

    return data.Data[key].Value;
}

function isSessionStale(lastHeartbeat, now) {
    if (!lastHeartbeat) return true;
    var lastHb = new Date(lastHeartbeat);
    var diffSeconds = (new Date(now) - lastHb) / 1000;
    return diffSeconds > STALE_SESSION_TIMEOUT_SECONDS;
}

function isTakeoverTimedOut(pendingSince, now) {
    if (!pendingSince) return false;
    var pendingAt = new Date(pendingSince);
    var diffSeconds = (new Date(now) - pendingAt) / 1000;
    return diffSeconds > TAKEOVER_WAIT_TIMEOUT_SECONDS;
}

function buildSessionResponse(success, status, previousSessionId, activeSessionId, pendingSessionId, message, errorCode) {
    return {
        success: success,
        status: status,
        kickedPreviousSession: false,
        previousSessionId: previousSessionId || "",
        activeSessionId: activeSessionId || "",
        pendingSessionId: pendingSessionId || "",
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
    var pendingSessionId = getInternalValue(data, SESSION_KEYS.pendingSessionId, "");
    var pendingSince = getInternalValue(data, SESSION_KEYS.pendingSince, "");
    var currentSessionIsStale = isSessionStale(currentLastHeartbeat, now);

    if (currentSessionId === args.sessionId && currentIsOnline) {
        server.UpdateUserInternalData({
            PlayFabId: currentPlayerId,
            Data: {
                sessionId: args.sessionId,
                isOnline: "true",
                loginAt: now,
                lastHeartbeat: now,
                pendingSessionId: "",
                pendingSince: ""
            }
        });

        return buildSessionResponse(true, SESSION_STATUS_ACTIVE, "", args.sessionId, "", "", "");
    }

    if (pendingSessionId !== "" && pendingSessionId !== args.sessionId) {
        return buildSessionResponse(false, SESSION_STATUS_ERROR, currentSessionId, "", pendingSessionId, "Tai khoan nay dang co mot phien dang cho tiep quan.", "SESSION_TAKEOVER_ALREADY_PENDING");
    }

    if (pendingSessionId === args.sessionId) {
        if (currentIsOnline && currentSessionId !== "" && !currentSessionIsStale) {
            if (isTakeoverTimedOut(pendingSince, now)) {
                server.UpdateUserInternalData({
                    PlayFabId: currentPlayerId,
                    Data: {
                        pendingSessionId: "",
                        pendingSince: ""
                    }
                });

                return buildSessionResponse(false, SESSION_STATUS_TIMEOUT, currentSessionId, "", "", "Het thoi gian cho phien cu dong bo va dang xuat.", "SESSION_TAKEOVER_TIMEOUT");
            }

            return buildSessionResponse(false, SESSION_STATUS_WAITING, currentSessionId, "", args.sessionId, "Dang doi phien cu dong bo va dang xuat.", "");
        }

        server.UpdateUserInternalData({
            PlayFabId: currentPlayerId,
            Data: {
                sessionId: args.sessionId,
                isOnline: "true",
                loginAt: now,
                lastHeartbeat: now,
                pendingSessionId: "",
                pendingSince: ""
            }
        });

        return buildSessionResponse(true, SESSION_STATUS_ACTIVE, currentSessionId, args.sessionId, "", "", "");
    }

    if (currentIsOnline && currentSessionId !== "" && !currentSessionIsStale) {
        server.UpdateUserInternalData({
            PlayFabId: currentPlayerId,
            Data: {
                pendingSessionId: args.sessionId,
                pendingSince: now
            }
        });

        return buildSessionResponse(false, SESSION_STATUS_WAITING, currentSessionId, "", args.sessionId, "Dang doi phien cu dong bo va dang xuat.", "");
    }

    var kickedPrevious = currentIsOnline && currentSessionId !== "" && currentSessionId !== args.sessionId;
    server.UpdateUserInternalData({
        PlayFabId: currentPlayerId,
        Data: {
            sessionId: args.sessionId,
            isOnline: "true",
            loginAt: now,
            lastHeartbeat: now,
            pendingSessionId: "",
            pendingSince: ""
        }
    });

    var activeResponse = buildSessionResponse(true, SESSION_STATUS_ACTIVE, currentSessionId, args.sessionId, "", "", "");
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
    var pendingSessionId = getInternalValue(data, SESSION_KEYS.pendingSessionId, "");

    if (pendingSessionId !== "" && pendingSessionId !== args.sessionId) {
        return {
            valid: false,
            shouldLogout: true,
            reason: "SESSION_TAKEOVER_PENDING"
        };
    }

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

    var now = new Date().toISOString();
    var data = server.GetUserInternalData({
        PlayFabId: currentPlayerId
    });

    var currentSessionId = getInternalValue(data, SESSION_KEYS.sessionId, "");
    var pendingSessionId = getInternalValue(data, SESSION_KEYS.pendingSessionId, "");

    if (currentSessionId !== args.sessionId) {
        return {
            released: false,
            pendingActivated: false
        };
    }

    if (pendingSessionId !== "" && pendingSessionId !== args.sessionId) {
        server.UpdateUserInternalData({
            PlayFabId: currentPlayerId,
            Data: {
                sessionId: pendingSessionId,
                isOnline: "true",
                lastHeartbeat: now,
                pendingSessionId: "",
                pendingSince: ""
            }
        });
        return {
            released: true,
            pendingActivated: true
        };
    }

    server.UpdateUserInternalData({
        PlayFabId: currentPlayerId,
        Data: {
            sessionId: "",
            isOnline: "false",
            pendingSessionId: "",
            pendingSince: ""
        }
    });

    return {
        released: true,
        pendingActivated: false
    };
};
