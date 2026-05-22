
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Unity.Netcode;
using UnityEngine;

public class SegmentRealmManager : SingletonNetwork<SegmentRealmManager>, ISegmentSystem
{
    public event Action<bool> OnRealmUplevelResult;

    private readonly Dictionary<string, UpgradeState> _realmSegment = new();
    private readonly SortedSet<UpgradeState> _sortedQueue = new();
    public void AddRealmSegment(LevelUpValidationResult upgradeState)
    {
        if (!IsServer) return;
        var realmDataSegment = new UpgradeState();
        realmDataSegment.playerId = upgradeState.playerId;
        realmDataSegment.upgradeId = upgradeState.instanceId;
        realmDataSegment.startTime = upgradeState.startTime;
        realmDataSegment.endTime = upgradeState.endTime;
        realmDataSegment.result = upgradeState.result;
        realmDataSegment.isCompleted = false;
        _realmSegment.Add(realmDataSegment.playerId, realmDataSegment);
        _sortedQueue.Add(realmDataSegment);
    }
    public void Update()
    {
        if (!IsServer) return;
        long now = TimeUtils.DateTimeOffset();
        if (_sortedQueue.Count <= 0) return;

        UpgradeState shortestData = _sortedQueue.Min;
        if (now > shortestData.endTime)
        {
            shortestData.isCompleted = true;
            _sortedQueue.Remove(shortestData);
            ApplyLevel(shortestData.playerId);
        }
    }
    public void RemoveRealmSegment(string playerId)
    {
        if (!IsServer) return;
        if (_realmSegment.TryGetValue(playerId, out var upgradeState))
        {
            _realmSegment.Remove(playerId);
        }
    }
    public void ConnectSegment(ClientData data)
    {
        if (!IsServer) return;
        ApplyLevel(data.playerId);
    }

    private void ApplyLevel(string playerId)
    {
        if (ClientManager.Instance.ClientOnline(playerId) == false) return;

        var clientData = ClientManager.Instance.GetClientData(playerId);

        if (_realmSegment.TryGetValue(playerId, out var upgradeState))
        {
            long endTime = upgradeState.endTime;
            LevelUpValidationResult result = new();
            result.instanceId = upgradeState.upgradeId;
            result.playerId = upgradeState.playerId;
            result.endTime = upgradeState.endTime;
            result.startTime = upgradeState.startTime;
            result.result = upgradeState.result;

            if (upgradeState.isCompleted)
            {
                var nextRealm = GameDataCenterManager.Instance.GetItemById(upgradeState.upgradeId);
                if (nextRealm == null)
                {
                    result.message = "Không tìm thấy realm tiếp theo";
                }
                else
                {
                    string res = result.result ? "Đột phá thành công" : "Đột phá thất bại";
                    string realmTxt = TextColorUtil.Color(EnumTranslator.ToVietnamese(nextRealm.realmType), Color.green);
                    string realm = result.result ? $"cảnh giới hiện tại là {realmTxt}" : "";
                    result.message = $"{TextColorUtil.Color(res, Color.green)} {realm}";
                    var json = JsonConvert.SerializeObject(result);
                    NotifiToClientRpc(json, RpcTargetUtils.Single(clientData.playerObject.OwnerClientId));
                }
                RemoveRealmSegment(playerId);
            }
            else
            {
                result.message = "Đang đợi kết thúc";
                var json = JsonConvert.SerializeObject(result);
                NotifiToClientRpc(json, RpcTargetUtils.Single(clientData.playerObject.OwnerClientId));
            }
        }
    }

    public void DisconnectSegment(ClientData data)
    {
        if (!IsServer) return;
    }
    [ClientRpc]
    private void NotifiToClientRpc(string message, ClientRpcParams clientRpcParams)
    {
        var messege = JsonConvert.DeserializeObject<LevelUpValidationResult>(message);
        TopNotificationUI.Instance.ShowNotification(messege.message);
        var now = TimeUtils.DateTimeOffset();
        if (now > messege.endTime)
        {
            if (messege.result)
            {
                UpgradeSystemManager.Instance.TryUpgrade(UpgradeSystemManager.RealmUpgradeId);
                OnRealmUplevelResult?.Invoke(true);
            }
        }
        else
        {
            // time is updated here
        }
    }
}