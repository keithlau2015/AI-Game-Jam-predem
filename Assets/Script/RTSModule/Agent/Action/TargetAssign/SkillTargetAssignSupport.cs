using AbilityModule;
using BehaviorTree;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static BehaviorTree.Node;

namespace CombatUnitModule
{
    public static class SkillTargetAssignSupport
    {
        public static bool HasValidTargetsInRange(SkillTargetAssignContext context)
        {
            Skill skill = context.Skill;
            CombatUnitAgent agent = context.Agent;

            if (skill.targets == null || skill.targets.Count == 0 || skill.range == null)
                return false;

            float attackRange = (float)skill.range.value;
            foreach (GameObject targetGo in skill.targets)
            {
                CombatUnitAgent target = null;
                if (!targetGo.TryGetComponent(out target) || !target.isAlive)
                    continue;

                if (Vector3.Distance(target.transform.position, agent.transform.position) <= attackRange)
                    return true;
            }

            return false;
        }

        public static bool TryAssignFromCursorSelection(SkillTargetAssignContext context)
        {
            Skill skill = context.Skill;
            CombatUnitAgent agent = context.Agent;

            if (!agent.team.Equals(Team.Blue))
                return false;

            if (CursorManager.singleton.selectableList == null || CursorManager.singleton.selectableList.Count == 0)
                return false;

            float attackRange = (float)skill.range.value;
            for (int i = CursorManager.singleton.selectableList.Count - 1; i >= 0; i--)
            {
                GameObject go = CursorManager.singleton.selectableList[i].GetGameObject();
                CombatUnitAgent target = null;
                if (!go.TryGetComponent(out target))
                    continue;

                if (target.team.Equals(agent.team))
                    continue;

                if (Vector3.Distance(target.transform.position, agent.transform.position) <= attackRange && target.isAlive)
                {
                    List<GameObject> selection = new List<GameObject> { target.gameObject };
                    skill.SelectTarget(selection);
                    context.MarkTargetsAdded();
                    return true;
                }
            }

            return false;
        }

        static List<CombatUnitAgent> GetUnitsToScan(SkillTargetAssignContext context)
        {
            if (context.Blackboard == null || context.Skill.range == null)
                return null;

            if (!BattleBlackboardAccess.TryGetEnemiesInInspectRange(context.Blackboard, out List<CombatUnitAgent> inspectEnemies)
                || inspectEnemies.Count == 0)
                return null;

            CombatUnitAgent agent = context.Agent;
            float attackRange = (float)context.Skill.range.value;
            List<CombatUnitAgent> filtered = new List<CombatUnitAgent>();
            for (int i = 0; i < inspectEnemies.Count; i++)
            {
                CombatUnitAgent unitEntity = inspectEnemies[i];
                if (unitEntity == null || unitEntity.team.Equals(agent.team) || unitEntity.Equals(agent))
                    continue;
                if (!unitEntity.isAlive)
                    continue;
                if (Vector3.Distance(unitEntity.transform.position, agent.transform.position) <= attackRange)
                    filtered.Add(unitEntity);
            }

            return filtered.Count > 0 ? filtered : null;
        }

        public static void ScanUnits(SkillTargetAssignContext context, bool canAcquireNewTargets)
        {
            List<CombatUnitAgent> unitsToScan = GetUnitsToScan(context);

            if (unitsToScan == null)
            {
                for (int i = CombatUnitAgent.allUnitEntities.Count - 1; i >= 0; i--)
                    ProcessUnit(context, CombatUnitAgent.allUnitEntities[i], canAcquireNewTargets);
            }
            else
            {
                for (int i = unitsToScan.Count - 1; i >= 0; i--)
                    ProcessUnit(context, unitsToScan[i], canAcquireNewTargets);

                RemoveOutOfRangeSelectedTargets(context);
            }

            FlushPendingAndRemoved(context);
        }

        static void ProcessUnit(SkillTargetAssignContext context, CombatUnitAgent unitEntity, bool canAcquireNewTargets)
        {
            Skill skill = context.Skill;
            CombatUnitAgent agent = context.Agent;
            List<GameObject> pendingTargets = context.PendingTargets;
            List<GameObject> removeTargets = context.RemoveTargets;

            if (unitEntity == null)
                return;
            if (unitEntity.team.Equals(agent.team))
                return;
            if (unitEntity.Equals(agent))
                return;

            if (skill.range == null)
                return;

            if (Vector3.Distance(unitEntity.transform.position, agent.transform.position) <= (float)skill.range.value)
            {
                if (skill.targets != null && skill.targets.Count > 0 && !skill.targets.Contains(unitEntity.gameObject))
                {
                    foreach (GameObject targetGo in skill.targets)
                    {
                        CombatUnitAgent target = null;
                        if (!targetGo.TryGetComponent(out target))
                            continue;
                        if (target == null)
                            continue;
                        if (!target.isAlive)
                        {
                            removeTargets.Add(targetGo);
                            Debug.Log($"AssignSkillTargetAction: rmeove target [{target.name}]");
                        }
                        else if (!target.targetIndicator.activeSelf && agent.showTargetIndicator)
                        {
                            target.targetIndicator.SetActive(true);
                        }
                    }
                }
                else if (canAcquireNewTargets)
                {
                    pendingTargets.Add(unitEntity.gameObject);
                    Debug.Log($"[{agent.team}] {unitEntity.name}: set target: [{unitEntity.team}] {unitEntity.name}");
                }
            }
            else if (skill.targets != null && skill.targets.Count > 0 && skill.targets.Contains(unitEntity.gameObject))
            {
                unitEntity.targetIndicator.SetActive(false);
                removeTargets.Add(unitEntity.gameObject);
                Debug.Log($"AssignSkillTargetAction: rmeove target [{unitEntity.name}]");
            }
        }

        static void RemoveOutOfRangeSelectedTargets(SkillTargetAssignContext context)
        {
            Skill skill = context.Skill;
            CombatUnitAgent agent = context.Agent;
            List<GameObject> removeTargets = context.RemoveTargets;

            if (skill.targets == null || skill.targets.Count == 0 || skill.range == null)
                return;

            float attackRange = (float)skill.range.value;
            for (int i = skill.targets.Count - 1; i >= 0; i--)
            {
                GameObject targetGo = skill.targets[i];
                CombatUnitAgent target = null;
                if (!targetGo.TryGetComponent(out target))
                    continue;

                if (Vector3.Distance(target.transform.position, agent.transform.position) > attackRange)
                {
                    if (target.targetIndicator.activeSelf)
                        target.targetIndicator.SetActive(false);
                    removeTargets.Add(targetGo);
                }
            }
        }

        static void FlushPendingAndRemoved(SkillTargetAssignContext context)
        {
            Skill skill = context.Skill;
            List<GameObject> pendingTargets = context.PendingTargets;
            List<GameObject> removeTargets = context.RemoveTargets;

            if (pendingTargets.Count > 0)
            {
                skill.SelectTarget(pendingTargets.Where(x =>
                {
                    CombatUnitAgent targetAgent = null;
                    if (!x.TryGetComponent(out targetAgent))
                        return false;
                    return targetAgent.isAlive;
                }).Select(x => x.gameObject).ToList());

                foreach (GameObject target in pendingTargets)
                {
                    CombatUnitAgent targetAgent = null;
                    if (!target.TryGetComponent(out targetAgent))
                        continue;
                    if (!targetAgent.targetIndicator.activeSelf)
                        targetAgent.targetIndicator.SetActive(true);
                }

                context.MarkTargetsAdded();
                pendingTargets.Clear();
            }

            if (removeTargets.Count > 0)
            {
                skill.UnselectTarget(removeTargets.Distinct().ToList());

                foreach (GameObject target in removeTargets)
                {
                    CombatUnitAgent targetAgent = null;
                    if (!target.TryGetComponent(out targetAgent))
                        continue;
                    if (targetAgent.targetIndicator.activeSelf)
                        targetAgent.targetIndicator.SetActive(false);
                }

                removeTargets.Clear();
            }
        }

        public static NodeStatus ResolveStatus(SkillTargetAssignContext context)
        {
            if (context.AddedTargetsThisTick || HasValidTargetsInRange(context))
                return NodeStatus.Success;

            return NodeStatus.Failure;
        }
    }
}
