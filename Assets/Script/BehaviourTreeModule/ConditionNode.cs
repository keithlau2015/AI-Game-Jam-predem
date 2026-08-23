using System;

namespace BehaviorTree
{
    // Condition Node
    public class ConditionNode : Node
    {
        private Func<bool> condition;

        public ConditionNode(TreeExecutor treeExecutor, Func<bool> condition) : base(treeExecutor)
        {
            this.condition = condition;
        }

        public override NodeStatus Execute()
        {
            return condition() ? NodeStatus.Success : NodeStatus.Failure;
        }
    }
}