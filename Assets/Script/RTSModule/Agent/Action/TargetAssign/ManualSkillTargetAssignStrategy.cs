using BehaviorTree;
using static BehaviorTree.Node;

namespace CombatUnitModule
{
    public class ManualSkillTargetAssignStrategy : ISkillTargetAssignStrategy
    {
        public static readonly ManualSkillTargetAssignStrategy Instance = new ManualSkillTargetAssignStrategy();

        public NodeStatus Assign(SkillTargetAssignContext context)
        {
            if (SkillTargetAssignSupport.TryAssignFromCursorSelection(context))
                return NodeStatus.Success;

            bool canAcquireNewTargets = !context.Agent.team.Equals(Team.Blue);
            SkillTargetAssignSupport.ScanUnits(context, canAcquireNewTargets);
            return SkillTargetAssignSupport.ResolveStatus(context);
        }
    }
}
