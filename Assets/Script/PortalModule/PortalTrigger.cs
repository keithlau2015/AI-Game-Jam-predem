using UnityEngine;

namespace PortalModule
{
    [RequireComponent(typeof(Collider))]
    public class PortalTrigger : MonoBehaviour
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
        private string sourcePortalId;

        [SerializeField]
        private PortalTransitionSettings transition = new PortalTransitionSettings();

        [SerializeField]
        private FilterMode filterMode = FilterMode.Tag;

        [SerializeField]
        private string requiredTag = "Player";

        [SerializeField]
        private LayerMask layerMask = ~0;

        [SerializeField]
        private bool useRootGameObject = true;

        [SerializeField]
        private float cooldownSeconds = 1f;

        private Collider triggerCollider;

        public string SourcePortalId => sourcePortalId;
        public PortalTransitionSettings Transition => transition;
        public float CooldownSeconds
        {
            get => cooldownSeconds;
            set => cooldownSeconds = Mathf.Max(0f, value);
        }

        private void Awake()
        {
            triggerCollider = GetComponent<Collider>();
            triggerCollider.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other == null)
                return;

            GameObject candidate = useRootGameObject ? other.transform.root.gameObject : other.gameObject;
            if (!PassesFilter(candidate, other.gameObject))
                return;

            PortalService service = PortalService.Resolve();
            if (service == null)
            {
                Debug.LogError("[PortalTrigger] PortalService is missing.", this);
                return;
            }

            if (!service.CanUsePortal(candidate))
                return;

            if (service.ExecuteTransition(transition, candidate, this))
                service.RegisterCooldown(candidate, cooldownSeconds);
        }

        public bool TryTeleport(GameObject traveler)
        {
            PortalService service = PortalService.Resolve();
            if (traveler == null || service == null)
                return false;

            if (!service.CanUsePortal(traveler))
                return false;

            if (!service.ExecuteTransition(transition, traveler, this))
                return false;

            service.RegisterCooldown(traveler, cooldownSeconds);
            return true;
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
            Gizmos.color = new Color(0.9f, 0.4f, 1f, 0.35f);
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
