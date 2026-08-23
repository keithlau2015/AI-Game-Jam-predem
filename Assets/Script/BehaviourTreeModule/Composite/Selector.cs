using System.Collections.Generic;

namespace BehaviorTree
{
    // Composite Node: Selector
    public class Selector : Node
    {
        private List<Node> children = new List<Node>();

        public Selector(TreeExecutor treeExecutor) : base(treeExecutor)
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
                if (status == NodeStatus.Success)
                    return NodeStatus.Success;
                if (status == NodeStatus.Running)
                    return NodeStatus.Running;
            }
            return NodeStatus.Failure;
        }
    }

}