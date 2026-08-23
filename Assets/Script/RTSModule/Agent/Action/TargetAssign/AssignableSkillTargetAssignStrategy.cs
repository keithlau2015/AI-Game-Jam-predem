using BehaviorTree;
using static BehaviorTree.Node;

namespace CombatUnitModule
{
    public class AssignableSkillTargetAssignStrategy : ISkillTargetAssignStrategy
    {
        public static readonly AssignableSkillTargetAssignStrategy Instance = new AssignableSkillTargetAssignStrategy();

        public NodeStatus Assign(SkillTargetAssignContext context)
        {
            return ManualSkillTargetAssignStrategy.Instance.Assign(context);
        }
    }
}
