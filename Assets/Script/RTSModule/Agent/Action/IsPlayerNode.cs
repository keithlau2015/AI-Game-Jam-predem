using BehaviorTree;

namespace CombatUnitModule
{
    public class IsPlayerNode : CombatUnitBaseNode
    {
        public IsPlayerNode(TreeExecutor treeExecutor) : base(treeExecutor)
        {
        }

        protected override bool RequiresInitializedAgent => false;

        protected override bool RequiresUnpausedBattle => false;

        protected override NodeStatus OnExecute(CombatUnitAgent agent)
        {
            if (agent.team == Team.Blue)
                return NodeStatus.Success;

            return NodeStatus.Failure;
        }
    }
}
