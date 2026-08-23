using AttributeModule;
using Model;
using System.Collections.Generic;
using UnityEngine;

namespace AbilityModule
{
    /// <summary>
    /// Factory / registry helpers for skills bound to an ICombatUnit.
    /// </summary>
    public class AbilityController
    {
        private readonly ICombatUnit owner;
        private readonly List<Skill> skills = new List<Skill>();

        public IReadOnlyList<Skill> Skills => skills;

        public AbilityController(ICombatUnit owner)
        {
            this.owner = owner;
        }

        public Skill CreateSkill(string skillKey)
        {
            if (owner == null)
            {
                Debug.LogError("[Ability] CreateSkill called with null owner.");
                return null;
            }

            if (!SkillModel.map.TryGetValue(skillKey, out SkillModel model))
            {
                Debug.LogError($"[Ability] Unknown skill key '{skillKey}'");
                return null;
            }

            Skill skill = new Skill(owner, model);
            skills.Add(skill);
            return skill;
        }

        public Skill CreateSkill(SkillModel model)
        {
            if (owner == null || model == null)
                return null;

            Skill skill = new Skill(owner, model);
            skills.Add(skill);
            return skill;
        }

        public void Clear()
        {
            for (int i = 0; i < skills.Count; i++)
                skills[i].StopCoolingDown();
            skills.Clear();
        }
    }
}
