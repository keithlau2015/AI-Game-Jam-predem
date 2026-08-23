namespace BehaviorTree
{
    // Decorator Node: Inverter
    public class Inverter : Node
    {

        public Inverter(TreeExecutor treeExecutor, Node child) : base(treeExecutor)
        {
            this.child.Add(child);
        }

        public override NodeStatus Execute()
        {
            var status = this.child[0].Execute();
            return status switch
            {
                NodeStatus.Success => NodeStatus.Failure,
                NodeStatus.Failure => NodeStatus.Success,
                _ => NodeStatus.Running,
            };
        }
    }
}