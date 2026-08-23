using BehaviorTree;
using static BehaviorTree.Node;

namespace CombatUnitModule
{
    public class AutoSkillTargetAssignStrategy : ISkillTargetAssignStrategy
    {
        public static readonly AutoSkillTargetAssignStrategy Instance = new AutoSkillTargetAssignStrategy();

        public NodeStatus Assign(SkillTargetAssignContext context)
        {
            SkillTargetAssignSupport.ScanUnits(context, canAcquireNewTargets: true);
            return SkillTargetAssignSupport.ResolveStatus(context);
        }
    }
}
