using System.Collections.Generic;
using System.Linq;
using CombatUnitModule;
using Newtonsoft.Json;

namespace SaveLoadModule.DocumentDto.Participants
{
    public sealed class CombatUnitSaveDto
    {
        public string key;
        public string id;
        public bool isPlayer;
        public int maxHp;
        public int hp;
        public int atk;
        public int def;
        public int hit;
        public int dodge;
        public int cri;
        public int criDmg;
        public int spd;
        public int inspectRange;
        public int counterInspectRange;
        public int shield;
        public int maxShield;
        public int shieldRegenSpd;
        public int shieldEfficiency;
    }

    public sealed class CombatUnitSaveParticipant : ISaveParticipant
    {
        public string SectionId => "combatUnits";

        public string CaptureJson()
        {
            List<CombatUnitSaveDto> dtos = CombatUnitData.map.Values
                .Where(u => u != null)
                .Select(u => new CombatUnitSaveDto
                {
                    key = u.key?.ToString(),
                    id = u.id?.ToString(),
                    isPlayer = u.isPlayer,
                    maxHp = u.maxHp,
                    hp = u.hp,
                    atk = u.atk,
                    def = u.def,
                    hit = u.hit,
                    dodge = u.dodge,
                    cri = u.cri,
                    criDmg = u.criDmg,
                    spd = u.spd,
                    inspectRange = u.inspectRange,
                    counterInspectRange = u.counterInspectRange,
                    shield = u.shield,
                    maxShield = u.maxShield,
                    shieldRegenSpd = u.shieldRegenSpd,
                    shieldEfficiency = u.shieldEfficiency
                })
                .ToList();
            return JsonConvert.SerializeObject(dtos);
        }

        public void ClearRuntime()
        {
            CombatUnitData.map.Clear();
            CombatUnitData.ClearPlayerShipCache();
        }

        public void RestoreJson(string json)
        {
            List<CombatUnitSaveDto> dtos = JsonConvert.DeserializeObject<List<CombatUnitSaveDto>>(json);
            if (dtos == null)
                return;

            foreach (CombatUnitSaveDto dto in dtos)
            {
                if (dto == null || string.IsNullOrEmpty(dto.key))
                    continue;
                CombatUnitData.FromSave(
                    dto.key, dto.id, dto.isPlayer,
                    dto.maxHp, dto.hp, dto.atk, dto.def, dto.hit, dto.dodge, dto.cri, dto.criDmg, dto.spd,
                    dto.inspectRange, dto.counterInspectRange, dto.shield, dto.maxShield, dto.shieldRegenSpd, dto.shieldEfficiency);
            }
        }
    }
}
