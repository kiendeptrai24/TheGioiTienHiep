handlers.RequestSession = function (args, context) {

    var now = new Date().toISOString();

    var data = server.GetUserInternalData({
        PlayFabId: currentPlayerId
    });

    var currentSession = null;

    if (data.Data != null && data.Data["sessionId"] != null) {
        currentSession = data.Data["sessionId"].Value;
    }

    // Ghi session mới (overwrite luôn)
    server.UpdateUserInternalData({
        PlayFabId: currentPlayerId,
        Data: {
            sessionId: args.sessionId,
            lastHeartbeat: now
        }
    });

    return {
        success: true,
        oldSession: currentSession
    };
};
handlers.Heartbeat = function (args, context) {

    var now = new Date().toISOString();

    var data = server.GetUserInternalData({
        PlayFabId: currentPlayerId
    });

    var currentSession = data.Data["sessionId"].Value;

    // Nếu session không khớp → bị kick
    if (currentSession !== args.sessionId) {
        return {
            valid: false
        };
    }

    // Update heartbeat
    server.UpdateUserInternalData({
        PlayFabId: currentPlayerId,
        Data: {
            lastHeartbeat: now
        }
    });

    return {
        valid: true
    };
};