using AbilityModule;
using BehaviorTree;

namespace CombatUnitModule
{
    public class PreviewSkillRangeNode : CombatUnitBaseNode
    {
        private Skill skill;

        public PreviewSkillRangeNode(Skill skill, TreeExecutor treeExecutor) : base(treeExecutor)
        {
            this.skill = skill;
        }

        protected override bool RequiresInitializedAgent => false;

        protected override bool RequiresUnpausedBattle => false;

        protected override NodeStatus OnExecute(CombatUnitAgent agent)
        {
            if (skill.isCoolingDown)
                return NodeStatus.Failure;

            skill.PreviewRange();
            return NodeStatus.Success;
        }
    }
}
