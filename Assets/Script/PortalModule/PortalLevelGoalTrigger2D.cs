using UnityEngine;

namespace PortalModule
{
    [RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
    public class PortalLevelGoalTrigger2D : MonoBehaviour
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

        private Collider2D triggerCollider;
        private Rigidbody2D triggerBody;

        public PortalLevelAdvanceSettings AdvanceSettings => advanceSettings;

        private void Awake()
        {
            triggerCollider = GetComponent<Collider2D>();
            triggerCollider.isTrigger = true;

            triggerBody = GetComponent<Rigidbody2D>();
            triggerBody.bodyType = RigidbodyType2D.Kinematic;
            triggerBody.gravityScale = 0f;
        }

        private void OnTriggerEnter2D(Collider2D other)
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
                Debug.LogError("[PortalLevelGoalTrigger2D] PortalGameRuleController is missing.", this);
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
            Collider2D col = GetComponent<Collider2D>();
            if (col is BoxCollider2D box)
            {
                Vector3 center = transform.TransformPoint(box.offset);
                Vector3 size = Vector3.Scale(box.size, transform.lossyScale);
                Gizmos.DrawCube(center, new Vector3(size.x, size.y, 0.1f));
            }
        }
#endif
    }
}
