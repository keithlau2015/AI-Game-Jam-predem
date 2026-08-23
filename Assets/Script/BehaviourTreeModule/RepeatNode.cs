// Repeat Node
namespace BehaviorTree
{
    public class RepeatNode : Node
    {
        private int remainingRepeats;

        public RepeatNode(TreeExecutor treeExecutor, Node child, int times) : base(treeExecutor)
        {
            this.child.Add(child);
            this.remainingRepeats = times;
        }

        public RepeatNode(TreeExecutor treeExecutor, int times) : base(treeExecutor)
        {
            this.remainingRepeats= times;
        }

        public override NodeStatus Execute()
        {
            if (remainingRepeats > 0 || remainingRepeats == -1)
            {
                NodeStatus status = child[0].Execute();
                if (status == NodeStatus.Success)
                {
                    if (remainingRepeats > 0)
                    {
                        remainingRepeats--;
                        return remainingRepeats == 0 ? NodeStatus.Success : NodeStatus.Running;
                    }
                    return NodeStatus.Running;
                }
                return status;
            }
            return NodeStatus.Success;
        }
    }
}