using UnityEngine;

namespace PortalModule
{
    [RequireComponent(typeof(Collider), typeof(Rigidbody))]
    public class PortalLevelGoalTrigger : MonoBehaviour
    {
        public enum FilterMode
        {
            Any = 0,
            Tag = 1,
            Layer = 2,
            TagOrLayer = 3,
            TagAndLayer = 4,
        }

        [SerializeField]
        private PortalLevelAdvanceSettings advanceSettings = new PortalLevelAdvanceSettings();

        [SerializeField]
        private FilterMode filterMode = FilterMode.Tag;

        [SerializeField]
        private string requiredTag = "Player";

        [SerializeField]
        private LayerMask layerMask = ~0;

        [SerializeField]
        private bool useRootGameObject = true;

        private Collider triggerCollider;
        private Rigidbody triggerBody;

        public PortalLevelAdvanceSettings AdvanceSettings => advanceSettings;

        private void Awake()
        {
            triggerCollider = GetComponent<Collider>();
            triggerCollider.isTrigger = true;

            triggerBody = GetComponent<Rigidbody>();
            triggerBody.isKinematic = true;
            triggerBody.useGravity = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            TryHandleTrigger(other.gameObject, other.transform.root.gameObject);
        }

        private void TryHandleTrigger(GameObject colliderObject, GameObject rootObject)
        {
            if (colliderObject == null)
                return;

            GameObject candidate = useRootGameObject ? rootObject : colliderObject;
            if (!PassesFilter(candidate, colliderObject))
                return;

            PortalGameRuleController controller = PortalGameRuleController.Instance;
            if (controller == null)
            {
                Debug.LogError("[PortalLevelGoalTrigger] PortalGameRuleController is missing.", this);
                return;
            }

            controller.RegisterVictory(candidate, advanceSettings);
        }

        private bool PassesFilter(GameObject root, GameObject colliderObject)
        {
            bool tagMatch = string.IsNullOrEmpty(requiredTag)
                || root.CompareTag(requiredTag)
                || colliderObject.CompareTag(requiredTag);

            bool layerMatch = ((1 << colliderObject.layer) & layerMask.value) != 0
                || ((1 << root.layer) & layerMask.value) != 0;

            switch (filterMode)
            {
                case FilterMode.Any:
                    return true;
                case FilterMode.Tag:
                    return tagMatch;
                case FilterMode.Layer:
                    return layerMatch;
                case FilterMode.TagOrLayer:
                    return tagMatch || layerMatch;
                case FilterMode.TagAndLayer:
                    return tagMatch && layerMatch;
                default:
                    return true;
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(0.3f, 1f, 0.45f, 0.35f);
            Collider col = GetComponent<Collider>();
            if (col is BoxCollider box)
            {
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawCube(box.center, box.size);
            }
            else if (col is SphereCollider sphere)
            {
                Gizmos.DrawSphere(transform.TransformPoint(sphere.center), sphere.radius * transform.lossyScale.x);
            }
        }
#endif
    }
}
