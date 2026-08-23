using BehaviorTree;
using static BehaviorTree.Node;

namespace CombatUnitModule
{
    public class DirectionSkillTargetAssignStrategy : ISkillTargetAssignStrategy
    {
        public static readonly DirectionSkillTargetAssignStrategy Instance = new DirectionSkillTargetAssignStrategy();

        public NodeStatus Assign(SkillTargetAssignContext context)
        {
            return ManualSkillTargetAssignStrategy.Instance.Assign(context);
        }
    }
}
