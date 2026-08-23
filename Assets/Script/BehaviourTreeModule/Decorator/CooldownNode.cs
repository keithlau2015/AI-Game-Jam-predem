using UnityEngine;

namespace BehaviorTree
{
    public class CooldownNode : Node
    {
        private readonly float _cooldownSeconds;
        private float _lastSuccessTime = -999f;

        public CooldownNode(TreeExecutor treeExecutor, Node child, float cooldownSeconds) : base(treeExecutor)
        {
            _cooldownSeconds = cooldownSeconds;
            this.child.Add(child);
        }

        public override NodeStatus Execute()
        {
            if (child == null || child.Count == 0)
                return NodeStatus.Failure;

            if (Time.time - _lastSuccessTime < _cooldownSeconds)
                return NodeStatus.Failure;

            var status = child[0].Execute();
            if (status == NodeStatus.Success)
                _lastSuccessTime = Time.time;
            return status;
        }
    }
}
