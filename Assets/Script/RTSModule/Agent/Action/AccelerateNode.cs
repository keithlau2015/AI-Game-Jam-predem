using AttributeModule;
using BehaviorTree;
using Model;
using System.Numerics;
using UnityEngine;

namespace CombatUnitModule
{
    public class AccelerateNode : CombatUnitBaseNode
    {
        private AttributeData spdIns = null;
        private BigInteger targetSpd;

        public AccelerateNode(TreeExecutor treeExecutor) : base(treeExecutor)
        {
            combatUnitAgent.attributes.TryGetValue((int)AttributeModel.AttributeType.SPD, out spdIns);
            targetSpd = 0;
        }

        protected override bool RequiresInitializedAgent => false;

        protected override bool RequiresUnpausedBattle => false;

        protected override NodeStatus OnExecute(CombatUnitAgent agent)
        {
            if (spdIns == null)
            {
                Debug.LogError("Speed Attribute Instance is NULL");
                return NodeStatus.Failure;
            }

            if (spdIns.value < targetSpd)
            {
                spdIns.SetValue(new BigInteger(Mathf.Lerp((float)spdIns.value, (float)targetSpd, Time.deltaTime)), AttributeData.EditMode.Add);
                return NodeStatus.Running;
            }

            if (spdIns.value > targetSpd)
            {
                spdIns.SetValue(targetSpd - spdIns.value, AttributeData.EditMode.Add);
                return NodeStatus.Running;
            }

            return NodeStatus.Success;
        }
    }
}
