using BehaviorTree;
using UnityEngine;

namespace CombatUnitModule
{
    public abstract class CombatUnitBaseNode : Node
    {
        protected CombatUnitBaseNode(TreeExecutor treeExecutor) : base(treeExecutor)
        {
        }

        protected virtual bool RequiresInitializedAgent => true;

        protected virtual bool RequiresUnpausedBattle => true;

        protected Blackboard Blackboard => GetExecutor()?.Blackboard;

        public sealed override NodeStatus Execute()
        {
            CombatUnitAgent agent = ResolveAgent();
            if (agent == null)
                return NodeStatus.Failure;

            if (RequiresInitializedAgent && !agent.isInit)
                return NodeStatus.Failure;

            if (RequiresUnpausedBattle && IsBattlePaused())
                return NodeStatus.Failure;

            return OnExecute(agent);
        }

        protected abstract NodeStatus OnExecute(CombatUnitAgent agent);

        protected bool IsBattlePaused()
        {
            if (Blackboard != null && Blackboard.TryGet(BattleBlackboardKeys.IsBattlePaused, out bool isPaused))
                return isPaused;

            return GameStateController.singleton != null && GameStateController.singleton.IsPause;
        }

        protected CombatUnitAgent ResolveAgent()
        {
            GameObject actor = Actor();
            if (actor == null)
                return null;

            return actor.GetComponent<CombatUnitAgent>();
        }

        public CombatUnitAgent combatUnitAgent => ResolveAgent();
    }
}
