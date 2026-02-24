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

            skillId = "",

            fromX = 0,
            fromY = 0,
            toX = 0,
            toY = 0
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
                break;

            case BattleEventSkill s:
                dto.targetTeam = s.targetTeam;
                dto.attackerUid = s.attackerUid;
                dto.targetUid = s.targetUid;
                dto.damage = s.damage;
                dto.isCrit = s.isCrit;
                dto.targetHpAfter = s.targetHpAfter;

                dto.skillId = s.skillId;
                break;

            case BattleEventAttack a:
                dto.targetTeam = a.targetTeam;
                dto.attackerUid = a.attackerUid;
                dto.targetUid = a.targetUid;
                dto.damage = a.damage;
                dto.isCrit = a.isCrit;
                dto.targetHpAfter = a.targetHpAfter;
                break;
            case BattleEventDealth d:
                dto.targetTeam = d.targetTeam;
                dto.targetUid = d.targetUid;
                dto.attackerTeam = d.attackerTeam;
                dto.attackerUid = d.attackerUid;
                break;
            case BattleEventInit b:
                dto.ownerUid = b.ownerUid;
                dto.cell = b.cell;
                dto.maxHp = b.maxHp;
                dto.curHp = b.curtHp;
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
                    targetTeam = dto.targetTeam,
                    attackerUid = dto.attackerUid,
                    targetUid = dto.targetUid,
                    damage = dto.damage,
                    isCrit = dto.isCrit,
                    targetHpAfter = dto.targetHpAfter,
                    skillId = dto.skillId,
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
                return new BattleEventInit
                {
                    time = dto.t,
                    team = dto.team,
                    ownerUid = dto.ownerUid,
                    cell = dto.cell,
                    type = dto.type,
                    maxHp = dto.maxHp,
                    curtHp = dto.curHp
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
