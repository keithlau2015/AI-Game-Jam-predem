using BehaviorTree;
using static BehaviorTree.Node;

namespace CombatUnitModule
{
    public interface ISkillTargetAssignStrategy
    {
        NodeStatus Assign(SkillTargetAssignContext context);
    }
}
