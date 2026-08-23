using AbilityModule;
using BehaviorTree;
using Model;
using System.Linq;

namespace CombatUnitModule
{
    public class ExecuteSkillNode : CombatUnitBaseNode
    {
        private readonly Skill skill;

        public ExecuteSkillNode(Skill skill, TreeExecutor treeExecutor) : base(treeExecutor)
        {
            this.skill = skill;
        }

        protected override NodeStatus OnExecute(CombatUnitAgent agent)
        {
            if (skill.isCoolingDown || !skill.IsActive)
                return NodeStatus.Failure;

            if (RequiresTargetsToExecute(skill)
                && (skill.targets == null || skill.targets.Count == 0))
                return NodeStatus.Failure;

            skill.Execute();

            if (SkillCombatRules.UsesEnemyPreviewBeforeExecute(agent, skill))
                skill.CancelPreviewRange();

            if (skill.targets != null && skill.targets.Count > 0)
            {
                skill.UnselectTarget(skill.targets.Where(x =>
                {
                    CombatUnitAgent targetAgent = null;
                    if (!x.TryGetComponent(out targetAgent))
                        return false;
                    return !targetAgent.isAlive;
                }).Select(x => x.gameObject).ToList());
            }

            return NodeStatus.Success;
        }

        private static bool RequiresTargetsToExecute(Skill skill)
        {
            if (SkillCombatRules.RequiresAssignedTargetsToExecute(skill))
                return true;

            if (skill == null || skill.Model == null || string.IsNullOrEmpty(skill.Model.value))
                return false;

            ProjectileModel projectileModel = null;
            if (!ProjectileModel.map.TryGetValue(skill.Model.value, out projectileModel))
                return false;

            return projectileModel.isTracker != 0;
        }

    }
}
