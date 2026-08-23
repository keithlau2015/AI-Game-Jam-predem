using AbilityModule;
using Model;
using static Model.SkillModel;

namespace CombatUnitModule
{
    public static class SkillTargetAssignStrategyFactory
    {
        public static ISkillTargetAssignStrategy Create(Skill skill)
        {
            if (skill == null || skill.Model == null)
                return AutoSkillTargetAssignStrategy.Instance;

            TargetType targetType = (TargetType)skill.Model.targetType;
            switch (targetType)
            {
                case TargetType.assignable:
                    return AssignableSkillTargetAssignStrategy.Instance;
                case TargetType.direction:
                    return DirectionSkillTargetAssignStrategy.Instance;
                case TargetType.auto:
                default:
                    return AutoSkillTargetAssignStrategy.Instance;
            }
        }
    }
}
