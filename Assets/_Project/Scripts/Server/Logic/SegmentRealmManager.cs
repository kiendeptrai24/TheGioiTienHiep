
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Unity.Netcode;
using UnityEngine;

public class SegmentRealmManager : SingletonNetwork<SegmentRealmManager>, ISegmentSystem
{
    public event Action<bool> OnRealmUplevelResult;
    public event Action<UpgradeState> OnRealmUpgrade;
    private bool isUpdating = false;
    private readonly Dictionary<string, UpgradeState> activeUpgradeStates = new();

    private readonly Dictionary<string, UpgradeState> _realmSegment = new();
    private readonly SortedSet<UpgradeState> _sortedQueue = new();
    private UpgradeState curUpdatestate;
    public bool GetIsUpdating()
    {
        return isUpdating;
    }

    public bool GetIsUpdating(string playerId)
    {
        if (string.IsNullOrEmpty(playerId))
            return false;

        return activeUpgradeStates.ContainsKey(playerId);
    }

    public UpgradeState GetUpgradeState(string playerId)
    {
        if (string.IsNullOrEmpty(playerId))
            return null;

        if (activeUpgradeStates.TryGetValue(playerId, out var state))
            return state;

        return null;
    }

    public void AddRealmSegment(LevelUpValidationResult upgradeState)
    {
        if (!IsServer) return;
        var realmDataSegment = new UpgradeState();
        realmDataSegment.playerId = upgradeState.playerId;
        realmDataSegment.upgradeId = upgradeState.instanceId;
        realmDataSegment.startTime = upgradeState.startTime;
        realmDataSegment.endTime = upgradeState.endTime;
        realmDataSegment.result = upgradeState.result;
        realmDataSegment.rewardPotentialPoint = upgradeState.rewardPotentialPoint;
        realmDataSegment.rewardSkillPoint = upgradeState.rewardSkillPoint;
        realmDataSegment.isCompleted = false;
        _realmSegment.Add(realmDataSegment.playerId, realmDataSegment);
        _sortedQueue.Add(realmDataSegment);
        ApplyLevel(realmDataSegment.playerId);
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
            result.isCompleted = upgradeState.isCompleted;
            result.rewardPotentialPoint = upgradeState.rewardPotentialPoint;
            result.rewardSkillPoint = upgradeState.rewardSkillPoint;
            result.result = upgradeState.result;
            if (upgradeState.isCompleted)
            {
                var nextRealm = GameDataCenterManager.Instance.GetItemById(upgradeState.upgradeId);
                if (nextRealm == null)
                {
                    result.messege = "Không tìm thấy cảnh tiếp theo";
                }
                else
                {
                    string res = result.result ? "Đột phá thành công" : "Đột phá thất bại";
                    string realmTxt = TextColorUtil.Color(EnumTranslator.ToVietnamese(nextRealm.realmType), Color.green);
                    string realm = result.result ? $"cảnh giới hiện tại là {realmTxt}" : "";
                    result.messege = $"{TextColorUtil.Color(res, Color.green)} {realm}";
                    var playerObject = clientData.networkClient;
                    var playerProfile = playerObject.PlayerObject.GetComponent<PlayerProfile>();
                    string reward = "";
                    reward = GetRewardMessege(upgradeState, result, reward);
                    if (!string.IsNullOrEmpty(reward))
                    {
                        result.messege += $"\n{reward}";
                    }
                    if (result.result)
                    {
                        playerProfile.SetPotentialPoint(upgradeState.rewardPotentialPoint);
                        playerProfile.SetSkillPoint(upgradeState.rewardSkillPoint);
                    }
                }
                var json = JsonConvert.SerializeObject(result);
                NotifiResultToClientRpc(json, RpcTargetUtils.Single(clientData.networkClient.PlayerObject.OwnerClientId));
                RemoveRealmSegment(playerId);
            }
            else
            {
                result.messege = "đang bế quan";
                var json = JsonConvert.SerializeObject(result);
                NotifiResultToClientRpc(json, RpcTargetUtils.Single(clientData.networkClient.PlayerObject.OwnerClientId));
            }
        }
    }

    private static string GetRewardMessege(UpgradeState upgradeState, LevelUpValidationResult result, string reward)
    {
        if (result.result)
        {
            List<string> rewards = new List<string>();

            if (upgradeState.rewardPotentialPoint > 0)
            {
                rewards.Add($"{TextColorUtil.Color(upgradeState.rewardPotentialPoint.ToString(), Color.green)} điểm tiềm năng");
            }

            if (upgradeState.rewardSkillPoint > 0)
            {
                rewards.Add($"{TextColorUtil.Color(upgradeState.rewardSkillPoint.ToString(), Color.green)} điểm kỹ năng");
            }

            if (rewards.Count > 0)
            {
                reward = $"Bạn nhận được {string.Join(" và ", rewards)}";
            }
        }

        return reward;
    }

    public void DisconnectSegment(ClientData data)
    {
        if (!IsServer) return;
    }
    [ClientRpc]
    private void NotifiResultToClientRpc(string message, ClientRpcParams clientRpcParams)
    {
        var messege = JsonConvert.DeserializeObject<LevelUpValidationResult>(message);
        if (messege == null) return;
        TopNotificationUI.Instance.ShowNotification(messege.messege);
        if (messege.isCompleted)
        {
            if (messege.result)
            {
                UpgradeSystemManager.Instance.TryUpgrade(UpgradeSystemManager.RealmUpgradeId, messege.playerId);
                isUpdating = false;
                if (string.IsNullOrEmpty(messege.playerId) == false)
                    activeUpgradeStates.Remove(messege.playerId);
                OnRealmUplevelResult?.Invoke(true);
            }
            else if (string.IsNullOrEmpty(messege.playerId) == false)
            {
                activeUpgradeStates.Remove(messege.playerId);
            }
        }
        else
        {
            UpgradeState upgradeState = new();
            upgradeState.playerId = messege.playerId;
            upgradeState.upgradeId = messege.instanceId;
            upgradeState.startTime = messege.startTime;
            upgradeState.endTime = messege.endTime;
            upgradeState.result = messege.result;
            upgradeState.rewardPotentialPoint = messege.rewardPotentialPoint;
            upgradeState.rewardSkillPoint = messege.rewardSkillPoint;
            curUpdatestate = upgradeState;
            isUpdating = true;
            if (string.IsNullOrEmpty(upgradeState.playerId) == false)
                activeUpgradeStates[upgradeState.playerId] = upgradeState;
            OnRealmUpgrade?.Invoke(curUpdatestate);
        }
    }
    public void RefreshUpgradeState()
    {
        if (curUpdatestate != null)
        {
            OnRealmUpgrade?.Invoke(curUpdatestate);
        }
    }
}