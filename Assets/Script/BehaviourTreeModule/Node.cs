using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    public abstract class Node
    {

        private TreeExecutor executor;

        protected TreeExecutor GetExecutor() => executor;

        // Node Status Enum
        public enum NodeStatus
        {
            Running,
            Success,
            Failure
        }

        public abstract NodeStatus Execute();

        public List<Node> child = new List<Node>();


        public GameObject Actor()
        {
            if (executor != null)
                return executor.gameObject;

            return null;
        }

        public Node(TreeExecutor treeExecutor)
        {
            this.executor = treeExecutor;
        }
    }
}