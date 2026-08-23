using System.Collections.Generic;

namespace BehaviorTree
{
    // Composite Node: Sequence
    public class Sequence : Node
    {
        private List<Node> children = new List<Node>();

        public Sequence(TreeExecutor treeExecutor) : base(treeExecutor) 
        {

        }

        public void AddChild(Node child)
        {
            children.Add(child);
        }

        public override NodeStatus Execute()
        {
            foreach (var child in children)
            {
                var status = child.Execute();
                if (status == NodeStatus.Failure)
                    return NodeStatus.Failure;
                if (status == NodeStatus.Running)
                    return NodeStatus.Running;
            }
            return NodeStatus.Success;
        }
    }
}