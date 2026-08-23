using AbilityModule;
using Model;
using static Model.SkillModel;

namespace CombatUnitModule
{
    public static class SkillCombatRules
    {
        public static bool IsAutoTargetSkill(Skill skill)
        {
            if (skill == null || skill.Model == null)
                return true;

            return skill.Model.targetType.Equals((int)TargetType.auto);
        }

        public static bool UsesEnemyPreviewBeforeExecute(CombatUnitAgent agent, Skill skill)
        {
            return agent != null
                && agent.team == Team.Red
                && !IsAutoTargetSkill(skill);
        }

        public static bool RequiresAssignedTargetsToExecute(Skill skill)
        {
            if (skill == null || skill.Model == null)
                return true;

            return skill.Model.targetType.Equals((int)TargetType.assignable);
        }
    }
}
