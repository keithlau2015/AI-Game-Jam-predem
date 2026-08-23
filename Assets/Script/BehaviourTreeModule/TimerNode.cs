using UnityEngine;

namespace BehaviorTree
{
    // Timer Node
    public class TimerNode : Node
    {
        private float duration;
        private float elapsedTime;

        public TimerNode(TreeExecutor treeExecutor, Node child, float duration) : base(treeExecutor)
        {
            this.child.Add(child);
            this.duration = duration;
            this.elapsedTime = 0;
        }

        public override NodeStatus Execute()
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime < duration)
                return child[0].Execute();
            return NodeStatus.Success;
        }
    }
}