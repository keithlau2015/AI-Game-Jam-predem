using BehaviorTree;
using System.Collections.Generic;

namespace CombatUnitModule
{
    public static class BattleBlackboardAccess
    {
        public static void BeginTick(Blackboard blackboard)
        {
            if (blackboard == null)
                return;

            blackboard.Set(BattleBlackboardKeys.IsBattlePaused, GameStateController.singleton.stateMachine.IsPauseState());
            blackboard.Remove(BattleBlackboardKeys.PrimaryChaseTarget);
            blackboard.Remove(BattleBlackboardKeys.EnemiesInInspectRange);
        }

        public static void SetPrimaryChaseTarget(Blackboard blackboard, CombatUnitAgent target)
        {
            if (blackboard == null)
                return;

            if (target == null || !target.isAlive)
                blackboard.Remove(BattleBlackboardKeys.PrimaryChaseTarget);
            else
                blackboard.Set(BattleBlackboardKeys.PrimaryChaseTarget, target);
        }

        public static bool TryGetPrimaryChaseTarget(Blackboard blackboard, out CombatUnitAgent target)
        {
            target = null;
            if (blackboard == null)
                return false;

            if (!blackboard.TryGet(BattleBlackboardKeys.PrimaryChaseTarget, out CombatUnitAgent resolved))
                return false;

            if (resolved == null || !resolved.isAlive)
            {
                blackboard.Remove(BattleBlackboardKeys.PrimaryChaseTarget);
                return false;
            }

            target = resolved;
            return true;
        }

        public static void SetEnemiesInInspectRange(Blackboard blackboard, List<CombatUnitAgent> enemies)
        {
            if (blackboard == null)
                return;

            blackboard.Set(BattleBlackboardKeys.EnemiesInInspectRange, enemies);
        }

        public static bool TryGetEnemiesInInspectRange(Blackboard blackboard, out List<CombatUnitAgent> enemies)
        {
            enemies = null;
            if (blackboard == null)
                return false;

            return blackboard.TryGet(BattleBlackboardKeys.EnemiesInInspectRange, out enemies) && enemies != null;
        }

        public static void SyncPrimaryChaseFromAgent(Blackboard blackboard, CombatUnitAgent agent)
        {
            if (blackboard == null || agent == null || agent.prioritytargets == null || agent.prioritytargets.Count == 0)
            {
                SetPrimaryChaseTarget(blackboard, null);
                return;
            }

            CombatUnitAgent chaseTarget = agent.prioritytargets[agent.prioritytargets.Count - 1];
            SetPrimaryChaseTarget(blackboard, chaseTarget != null && chaseTarget.isAlive ? chaseTarget : null);
        }
    }
}
