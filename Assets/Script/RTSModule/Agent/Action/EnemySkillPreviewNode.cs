using AbilityModule;
using BehaviorTree;
using UnityEngine;

namespace CombatUnitModule
{
    public class EnemySkillPreviewNode : CombatUnitBaseNode
    {
        private readonly Skill skill;
        private readonly float previewDuration;
        private float previewElapsed;
        private bool previewStarted;

        public EnemySkillPreviewNode(Skill skill, TreeExecutor treeExecutor, float previewDuration = 1.5f) : base(treeExecutor)
        {
            this.skill = skill;
            this.previewDuration = previewDuration;
        }

        protected override NodeStatus OnExecute(CombatUnitAgent agent)
        {
            if (!SkillCombatRules.UsesEnemyPreviewBeforeExecute(agent, skill))
                return NodeStatus.Success;

            if (!skill.IsPreviewingRange())
                previewStarted = false;

            if (!SkillPreviewUtility.CanPreviewNonAutoSkill(agent, skill, Blackboard))
            {
                skill.CancelPreviewRange();
                previewStarted = false;
                previewElapsed = 0f;
                return NodeStatus.Failure;
            }

            if (!previewStarted)
            {
                skill.PreviewRange();
                previewStarted = true;
                previewElapsed = 0f;
            }

            SkillPreviewUtility.AimEquipmentForPreview(agent, skill, Blackboard);

            previewElapsed += Time.deltaTime;
            if (previewElapsed < previewDuration)
                return NodeStatus.Running;

            previewStarted = false;
            previewElapsed = 0f;
            return NodeStatus.Success;
        }
    }
}
