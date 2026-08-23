using BehaviorTree;
using System.Linq;

namespace CombatUnitModule
{
    public class EngagingNode : CombatUnitBaseNode
    {
        public EngagingNode(TreeExecutor treeExecutor) : base(treeExecutor)
        {
        }

        protected override NodeStatus OnExecute(CombatUnitAgent agent)
        {
            CombatUnitAgent chaseTarget = null;
            if (!BattleBlackboardAccess.TryGetPrimaryChaseTarget(Blackboard, out chaseTarget))
            {
                if (agent.prioritytargets == null || agent.prioritytargets.Count == 0 || !agent.prioritytargets.Any(x => x != null && x.isAlive))
                    return NodeStatus.Failure;

                chaseTarget = agent.prioritytargets[agent.prioritytargets.Count - 1];
                BattleBlackboardAccess.SetPrimaryChaseTarget(Blackboard, chaseTarget);
            }

            if (chaseTarget == null || !chaseTarget.isAlive)
                return NodeStatus.Failure;

            agent.agent.SetDestination(chaseTarget.transform.position);
            agent.agent.stoppingDistance = agent.agent.radius * 2f;
            return NodeStatus.Running;
        }
    }
}
