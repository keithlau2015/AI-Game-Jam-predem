using AttributeModule;
using BehaviorTree;
using Model;
using System.Collections.Generic;
using UnityEngine;

namespace CombatUnitModule
{
    public class SearchingInspectTargetNode : CombatUnitBaseNode
    {
        public SearchingInspectTargetNode(TreeExecutor treeExecutor) : base(treeExecutor)
        {
        }

        protected override NodeStatus OnExecute(CombatUnitAgent agent)
        {
            List<CombatUnitAgent> enemiesInInspectRange = new List<CombatUnitAgent>();

            if (CursorManager.singleton.selectableList != null && CursorManager.singleton.selectableList.Count > 0)
            {
                AttributeData inspectRangeIns = null;
                if (!agent.attributes.TryGetValue((int)AttributeModel.AttributeType.INSPECT_RANGE, out inspectRangeIns))
                    return NodeStatus.Failure;
                AttributeData counterInspectRangeIns = null;
                if (!agent.attributes.TryGetValue((int)AttributeModel.AttributeType.COUNTER_INSPECT_RANGE, out counterInspectRangeIns))
                    return NodeStatus.Failure;
                System.Numerics.BigInteger finalInspectRange = inspectRangeIns.value - counterInspectRangeIns.value;
                if (finalInspectRange < inspectRangeIns.minValue)
                    finalInspectRange = inspectRangeIns.value;

                for (int i = 0; i < CursorManager.singleton.selectableList.Count; i++)
                {
                    GameObject go = CursorManager.singleton.selectableList[i].GetGameObject();
                    CombatUnitAgent target = null;
                    if (!go.TryGetComponent(out target))
                        continue;

                    if (Vector3.Distance(target.transform.position, agent.transform.position) <= (float)finalInspectRange)
                    {
                        if (target.isAlive)
                            return NodeStatus.Failure;
                    }
                }
            }

            NodeStatus result = NodeStatus.Success;

            foreach (CombatUnitAgent unitEntity in CombatUnitAgent.allUnitEntities)
            {
                if (unitEntity == null)
                    continue;
                if (unitEntity.team == agent.team)
                    continue;
                if (unitEntity.Equals(agent))
                    continue;
                AttributeData inspectRangeIns = null;
                if (!agent.attributes.TryGetValue((int)AttributeModel.AttributeType.INSPECT_RANGE, out inspectRangeIns))
                    continue;
                AttributeData counterInspectRangeIns = null;
                if (!unitEntity.attributes.TryGetValue((int)AttributeModel.AttributeType.COUNTER_INSPECT_RANGE, out counterInspectRangeIns))
                    continue;
                System.Numerics.BigInteger finalInspectRange = inspectRangeIns.value - counterInspectRangeIns.value;
                if (finalInspectRange < inspectRangeIns.minValue)
                    finalInspectRange = inspectRangeIns.value;

                if (Vector3.Distance(unitEntity.transform.position, agent.transform.position) <= (float)finalInspectRange)
                {
                    enemiesInInspectRange.Add(unitEntity);

                    if (unitEntity.prioritytargets.Count > 0)
                    {
                        CombatUnitAgent target = unitEntity.prioritytargets[unitEntity.prioritytargets.Count - 1];
                        if (!target.isAlive || target.Equals(unitEntity))
                        {
                            unitEntity.prioritytargets.Remove(target);
                            result = NodeStatus.Failure;
                        }
                        else
                        {
                            result = NodeStatus.Success;
                        }
                    }
                    else if (!unitEntity.Equals(agent))
                    {
                        unitEntity.prioritytargets.Add(unitEntity);
                        result = NodeStatus.Success;
                    }
                }
                else if (unitEntity.prioritytargets.Count > 0 && unitEntity.prioritytargets.Equals(unitEntity))
                {
                    CombatUnitAgent target = unitEntity.prioritytargets[unitEntity.prioritytargets.Count - 1];
                    unitEntity.prioritytargets.Remove(target);
                    result = NodeStatus.Failure;
                }
            }

            BattleBlackboardAccess.SetEnemiesInInspectRange(Blackboard, enemiesInInspectRange);
            BattleBlackboardAccess.SyncPrimaryChaseFromAgent(Blackboard, agent);

            return result;
        }
    }
}
