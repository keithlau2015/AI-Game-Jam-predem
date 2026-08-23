using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    public abstract class TreeExecutor : MonoBehaviour
    {
        public bool isAutoInit = true;

        protected Node root;
        public Blackboard Blackboard { get; } = new Blackboard();

        public abstract void ConstructTree();

        private void Start()
        {
            if(isAutoInit)
                ConstructTree();
        }

        protected virtual void OnBeforeTick()
        {
        }

        private void Update()
        {
            if (root == null)
                return;

            OnBeforeTick();
            root.Execute();
        }
    }
}