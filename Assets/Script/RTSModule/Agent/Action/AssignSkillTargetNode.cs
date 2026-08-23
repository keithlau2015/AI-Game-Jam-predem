using AbilityModule;
using BehaviorTree;

namespace CombatUnitModule
{
    public class AssignSkillTargetNode : CombatUnitBaseNode
    {
        private readonly Skill skill;
        private readonly ISkillTargetAssignStrategy strategy;

        public AssignSkillTargetNode(Skill skill, TreeExecutor treeExecutor) : base(treeExecutor)
        {
            this.skill = skill;
            strategy = SkillTargetAssignStrategyFactory.Create(skill);
        }

        protected override NodeStatus OnExecute(CombatUnitAgent agent)
        {
            if (skill.range == null)
                return NodeStatus.Failure;

            SkillTargetAssignContext context = new SkillTargetAssignContext(agent, skill, Blackboard);
            NodeStatus status = strategy.Assign(context);

            if (status == NodeStatus.Failure && SkillCombatRules.UsesEnemyPreviewBeforeExecute(agent, skill))
                skill.CancelPreviewRange();

            return status;
        }
    }
}
