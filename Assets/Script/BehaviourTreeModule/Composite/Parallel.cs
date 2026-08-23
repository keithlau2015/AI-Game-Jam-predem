using System.Collections.Generic;

namespace BehaviorTree
{
    public class Parallel : Node
    {
        private readonly ParallelPolicy policy;
        private readonly List<Node> children = new List<Node>();

        public Parallel(TreeExecutor treeExecutor, ParallelPolicy policy = ParallelPolicy.RequireAll) : base(treeExecutor)
        {
            this.policy = policy;
        }

        public void AddChild(Node child)
        {
            children.Add(child);
        }

        public override NodeStatus Execute()
        {
            if (children.Count == 0)
                return NodeStatus.Failure;

            int successCount = 0;
            bool anyRunning = false;

            foreach (Node child in children)
            {
                NodeStatus status = child.Execute();
                if (status == NodeStatus.Success)
                    successCount++;
                else if (status == NodeStatus.Running)
                    anyRunning = true;
            }

            switch (policy)
            {
                case ParallelPolicy.RequireOne:
                    if (successCount > 0)
                        return NodeStatus.Success;
                    return anyRunning ? NodeStatus.Running : NodeStatus.Failure;

                case ParallelPolicy.AlwaysRun:
                    if (anyRunning)
                        return NodeStatus.Running;
                    return NodeStatus.Success;

                case ParallelPolicy.RequireAll:
                default:
                    if (successCount >= children.Count)
                        return NodeStatus.Success;
                    return anyRunning ? NodeStatus.Running : NodeStatus.Failure;
            }
        }
    }
}
