using System.Collections.Generic;
using UnityEngine;

public static class BattleEventMapper
{
    public static BattleEventDTO ToDTO(BattleEvent ev)
    {
        // default dto
        var dto = new BattleEventDTO
        {
            t = ev.time,
            type = ev.type,
            team = ev.team,
            ownerUid = ev.ownerUid,
            attackerUid = "",
            targetUid = "",

            damage = 0,
            isCrit = false,
            targetHpAfter = 0,

            skillId0 = "",
            skillId1 = "",
            skillId2 = "",
            skillId3 = "",
            skillId4 = "",

            fromX = 0,
            fromY = 0,
            toX = 0,
            toY = 0,
            castTime = 0f
        };
        if (ev.ownerUid == null)
        {
            dto.ownerUid = "";
        }

        switch (ev)
        {
            case BattleEventMove m:
                dto.fromX = (short)m.from.x;
                dto.fromY = (short)m.from.y;
                dto.toX = (short)m.to.x;
                dto.toY = (short)m.to.y;
                dto.targetTeam = m.targetTeam;
                dto.targetUid = m.targetUid;
                break;

            case BattleEventSkill s:
                dto.targetTeam = s.targetTeam;
                dto.attackerUid = s.attackerUid;
                dto.targetUid = s.targetUid;
                dto.damage = s.damage;
                dto.isCrit = s.isCrit;
                dto.targetHpAfter = s.targetHpAfter;
                dto.castTime = s.castTime;
                dto.skillId0 = s.skillId;
                break;

            case BattleEventAttack a:
                dto.targetTeam = a.targetTeam;
                dto.attackerUid = a.attackerUid;
                dto.targetUid = a.targetUid;
                dto.damage = a.damage;
                dto.isCrit = a.isCrit;
                dto.targetHpAfter = a.targetHpAfter;
                dto.castTime = a.castTime;
                break;
            case BattleEventDealth d:
                dto.targetTeam = d.targetTeam;
                dto.targetUid = d.targetUid;
                dto.attackerTeam = d.attackerTeam;
                dto.attackerUid = d.attackerUid;
                break;
            case BattleEventInit b:
                dto.cell = b.cell;
                dto.maxHp = b.maxHp;
                dto.moveSpeed = b.moveSpeed;
                dto.curHp = b.curtHp;
                for (int i = 0; i < b.skillIds.Count; i++)
                {
                    if (i == 0)
                    {
                        dto.skillId0 = b.skillIds[i];
                    }
                    else if (i == 1)
                    {
                        dto.skillId1 = b.skillIds[i];
                    }
                    else if (i == 2)
                    {
                        dto.skillId2 = b.skillIds[i];
                    }
                    else if (i == 3)
                    {
                        dto.skillId3 = b.skillIds[i];
                    }
                    else if (i == 4)
                    {
                        dto.skillId4 = b.skillIds[i];
                    }
                }
                break;
            default:
                // nothing extra
                break;
        }
        return dto;
    }

    public static BattleEvent FromDTO(in BattleEventDTO dto)
    {
        switch (dto.type)
        {
            case BattleEventType.Move:
                return new BattleEventMove
                {
                    time = dto.t,
                    type = dto.type,
                    team = dto.team,
                    ownerUid = dto.ownerUid,
                    targetTeam = dto.targetTeam,
                    targetUid = dto.targetUid,
                    from = new Vector2Int(dto.fromX, dto.fromY),
                    to = new Vector2Int(dto.toX, dto.toY),
                };

            case BattleEventType.Skill:
                return new BattleEventSkill
                {
                    time = dto.t,
                    type = dto.type,
                    ownerUid = dto.ownerUid,
                    team = dto.team,
                    castTime = dto.castTime,
                    targetTeam = dto.targetTeam,
                    attackerUid = dto.attackerUid,
                    targetUid = dto.targetUid,
                    damage = dto.damage,
                    isCrit = dto.isCrit,
                    targetHpAfter = dto.targetHpAfter,
                    skillId = dto.skillId0,
                };

            case BattleEventType.Attack:
                return new BattleEventAttack
                {
                    time = dto.t,
                    type = dto.type,
                    team = dto.team,
                    targetTeam = dto.targetTeam,
                    ownerUid = dto.ownerUid,
                    attackerUid = dto.attackerUid,
                    targetUid = dto.targetUid,
                    damage = dto.damage,
                    isCrit = dto.isCrit,
                    targetHpAfter = dto.targetHpAfter,
                    castTime = dto.castTime
                };
            case BattleEventType.Death:
                return new BattleEventDealth
                {
                    time = dto.t,
                    type = dto.type,

                    team = dto.team,
                    targetTeam = dto.targetTeam,
                    ownerUid = dto.ownerUid,
                    targetUid = dto.targetUid,

                    attackerTeam = dto.attackerTeam,
                    attackerUid = dto.attackerUid,
                };
            case BattleEventType.Init:
                List<string> skillIds = new();
                if (string.IsNullOrEmpty(dto.skillId0) == false)
                    skillIds.Add(dto.skillId0);
                if (string.IsNullOrEmpty(dto.skillId1) == false)
                    skillIds.Add(dto.skillId1);
                if (string.IsNullOrEmpty(dto.skillId2) == false)
                    skillIds.Add(dto.skillId2);
                if (string.IsNullOrEmpty(dto.skillId3) == false)
                    skillIds.Add(dto.skillId3);
                if (string.IsNullOrEmpty(dto.skillId4) == false)
                    skillIds.Add(dto.skillId4);
                return new BattleEventInit
                {
                    time = dto.t,
                    team = dto.team,
                    ownerUid = dto.ownerUid,
                    cell = dto.cell,
                    type = dto.type,
                    maxHp = dto.maxHp,
                    curtHp = dto.curHp,
                    skillIds = skillIds
                };
            default:
                return new BattleEvent
                {
                    time = dto.t,
                    type = dto.type,
                    team = dto.team,
                    ownerUid = dto.ownerUid,
                };
        }
    }
}
