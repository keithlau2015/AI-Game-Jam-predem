using AbilityModule;
using BehaviorTree;
using EquipmentModule;
using Model;
using UnityEngine;
using static Model.SkillModel;

namespace CombatUnitModule
{
    public static class SkillPreviewUtility
    {
        public static void AimEquipmentForPreview(CombatUnitAgent agent, Skill skill, Blackboard blackboard)
        {
            Transform aimTransform = ResolveAimTransform(agent, skill, blackboard);
            if (aimTransform == null)
                return;

            int equipmentIndex = agent.GetEquipmentIndexBySkill(skill.Model.key.ToString());
            if (equipmentIndex < 0 || equipmentIndex >= agent.equipmentSlots.Count)
                return;

            GameObject slotGo = agent.equipmentSlots[equipmentIndex];
            if (slotGo == null || slotGo.transform.childCount == 0)
                return;

            Transform equipmentTran = slotGo.transform.GetChild(0);
            AutoRotateToLockedTarget autoRotate = equipmentTran.GetComponent<AutoRotateToLockedTarget>();
            if (autoRotate != null)
                autoRotate.SetTarget(aimTransform);

            if (agent.team == Team.Red)
                skill.AimRangeVisual(aimTransform);
        }

        static Transform ResolveAimTransform(CombatUnitAgent agent, Skill skill, Blackboard blackboard)
        {
            if (skill.targets != null && skill.targets.Count > 0)
            {
                CombatUnitAgent targetAgent = null;
                if (skill.targets[0].TryGetComponent(out targetAgent) && targetAgent != null && targetAgent.isAlive)
                    return targetAgent.transform;
            }

            CombatUnitAgent chaseTarget = null;
            if (BattleBlackboardAccess.TryGetPrimaryChaseTarget(blackboard, out chaseTarget))
                return chaseTarget.transform;

            return null;
        }

        public static bool CanPreviewNonAutoSkill(CombatUnitAgent agent, Skill skill, Blackboard blackboard)
        {
            if (skill == null || skill.isCoolingDown)
                return false;

            if (skill.Model.targetType.Equals((int)TargetType.assignable))
                return skill.targets != null && skill.targets.Count > 0;

            if (skill.Model.targetType.Equals((int)TargetType.direction))
                return ResolveAimTransform(agent, skill, blackboard) != null;

            return false;
        }
    }
}
