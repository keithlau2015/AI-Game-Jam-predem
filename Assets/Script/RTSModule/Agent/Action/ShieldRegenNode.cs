using AttributeModule;
using BehaviorTree;
using Model;
using System.Numerics;
using UnityEngine;

namespace CombatUnitModule
{
    public class ShieldRegenNode : CombatUnitBaseNode
    {
        private BigInteger regenRate = new BigInteger(-1f);

        public ShieldRegenNode(TreeExecutor treeExecutor) : base(treeExecutor)
        {
        }

        protected override NodeStatus OnExecute(CombatUnitAgent agent)
        {
            if (agent.shieldCoolDownTime > 0)
                return NodeStatus.Failure;

            AttributeData shield = null;
            if (!agent.attributes.TryGetValue((int)AttributeModel.AttributeType.SHIELD, out shield))
            {
                GameLog.logger.Error("Attack Action missing Shield Attribute");
                return NodeStatus.Failure;
            }

            AttributeData shieldRegen = null;
            if (!agent.attributes.TryGetValue((int)AttributeModel.AttributeType.SHIELD_REGEN_SPD, out shieldRegen))
            {
                GameLog.logger.Error("Attack Action missing Shield Attribute");
                return NodeStatus.Failure;
            }

            if (shield.IsMax())
                return NodeStatus.Success;

            if (regenRate == -1)
            {
                regenRate = new BigInteger(((float)shield.maxValue * (float)((float)shieldRegen.value * 0.01f)) * 0.1f);

                if (agent.team == Team.Blue)
                {
                    PlayerCombatUnitAgent playerCombatUnitAgent = agent.GetComponent<PlayerCombatUnitAgent>();
                    if (playerCombatUnitAgent != null)
                    {
                        float speed = playerCombatUnitAgent.agent.speed;
                        float speedModifier = playerCombatUnitAgent.currentSpeedModifier;
                        float rate = (speed - speedModifier) / speed;
                        regenRate = new BigInteger((float)regenRate * rate);
                    }
                }
            }

            if (shield.value < shield.maxValue)
            {
                BigInteger regenAmount = new BigInteger((float)regenRate * (float)Time.deltaTime);
                shield.SetValue(regenAmount, AttributeData.EditMode.Add);
            }

            return NodeStatus.Running;
        }
    }
}
