using System.Collections.Generic;
using UnityEngine;

namespace EvtModule
{
    /// <summary>
    /// Fires when a collider enters this trigger volume.
    /// Filter by tag / layer so it works for any game, not a specific unit type.
    /// Notify payload: "gameObject" = the entering object (root or collider owner).
    /// </summary>
    [RequireComponent(typeof(Collider), typeof(Rigidbody))]
    public class EnterAreaObservable : EvtObserable
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
        private FilterMode filterMode = FilterMode.Any;

        [SerializeField]
        private string requiredTag = "Player";

        [SerializeField]
        private LayerMask layerMask = ~0;

        [Tooltip("If set, only objects with IAttributeHolder (self or parent) pass the filter.")]
        [SerializeField]
        private bool requireAttributeHolder;

        [Tooltip("Notify using the parent root instead of the collider GameObject.")]
        [SerializeField]
        private bool useRootGameObject = true;

        private Collider areaCollider;
        private Rigidbody areaRigidbody;

        private void Awake()
        {
            areaCollider = GetComponent<Collider>();
            areaCollider.enabled = true;
            areaCollider.isTrigger = true;

            areaRigidbody = GetComponent<Rigidbody>();
            areaRigidbody.isKinematic = true;
            areaRigidbody.useGravity = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other == null)
                return;

            GameObject candidate = useRootGameObject ? other.transform.root.gameObject : other.gameObject;
            if (!PassesFilter(candidate, other.gameObject))
                return;

            Notify(new EvtNotifyData()
            {
                observable = this,
                values = new Dictionary<string, object>()
                {
                    { "gameObject", candidate },
                    { "collider", other },
                }
            });
        }

        private bool PassesFilter(GameObject root, GameObject colliderObject)
        {
            bool tagMatch = string.IsNullOrEmpty(requiredTag)
                || root.CompareTag(requiredTag)
                || colliderObject.CompareTag(requiredTag);

            bool layerMatch = ((1 << colliderObject.layer) & layerMask.value) != 0
                || ((1 << root.layer) & layerMask.value) != 0;

            bool filterOk;
            switch (filterMode)
            {
                case FilterMode.Any:
                    filterOk = true;
                    break;
                case FilterMode.Tag:
                    filterOk = tagMatch;
                    break;
                case FilterMode.Layer:
                    filterOk = layerMatch;
                    break;
                case FilterMode.TagOrLayer:
                    filterOk = tagMatch || layerMatch;
                    break;
                case FilterMode.TagAndLayer:
                    filterOk = tagMatch && layerMatch;
                    break;
                default:
                    filterOk = true;
                    break;
            }

            if (!filterOk)
                return false;

            if (requireAttributeHolder)
            {
                if (root.GetComponentInParent<AttributeModule.IAttributeHolder>() == null
                    && colliderObject.GetComponentInParent<AttributeModule.IAttributeHolder>() == null)
                    return false;
            }

            return true;
        }
    }
}
