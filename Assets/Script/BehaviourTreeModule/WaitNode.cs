using UnityEngine;

namespace BehaviorTree
{
    // Leaf Node: Wait
    public class WaitNode : Node
    {
        private float waitTime;
        private float elapsedTime;

        public WaitNode(TreeExecutor treeExecutor, float waitTime) : base(treeExecutor)
        {
            this.waitTime = waitTime;
            this.elapsedTime = 0;
        }

        public override NodeStatus Execute()
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= waitTime)
            {
                elapsedTime = 0f;
                return NodeStatus.Success;
            }
            return NodeStatus.Running;
        }
    }
}