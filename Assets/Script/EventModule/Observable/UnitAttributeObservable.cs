using AttributeModule;
using ObjetPoolModule;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using static Model.AttributeModel;

namespace EvtModule
{
    /// <summary>
    /// Watches pooled objects that implement IAttributeHolder and notifies when
    /// a chosen attribute crosses a comparison threshold.
    /// Works for any entity type that exposes AttributeModule attributes.
    /// </summary>
    public class UnitAttributeObservable : EvtObserable
    {
        private enum Operator
        {
            Equal = 0,
            LessOrEqual = 1,
            Lesser = 2,
            GreaterOrEqual = 3,
            Greater = 4
        }

        [SerializeField]
        private string value;

        [SerializeField]
        private AttributeType attributeType;

        [SerializeField]
        private Operator operatorValue;

        [Tooltip("Optional tag filter on spawned objects. Empty = all IAttributeHolders.")]
        [SerializeField]
        private string filterTag;

        private void Awake()
        {
            if (ObjectPoolManager.singleton != null)
                ObjectPoolManager.singleton.onObjectSpawn += OnObjectSpawn;
        }

        private void OnEnable()
        {
            if (ObjectPoolManager.singleton != null)
            {
                ObjectPoolManager.singleton.onObjectSpawn -= OnObjectSpawn;
                ObjectPoolManager.singleton.onObjectSpawn += OnObjectSpawn;
            }
        }

        private void OnDisable()
        {
            if (ObjectPoolManager.singleton != null)
                ObjectPoolManager.singleton.onObjectSpawn -= OnObjectSpawn;
        }

        private void OnObjectSpawn(PoolObjectProperty poolObjectProperty)
        {
            if (poolObjectProperty == null)
                return;

            GameObject go = poolObjectProperty.gameObject;
            if (!string.IsNullOrEmpty(filterTag) && !go.CompareTag(filterTag))
                return;

            if (!go.TryGetComponent(out IAttributeHolder attributeHolder))
            {
                attributeHolder = go.GetComponentInParent<IAttributeHolder>();
                if (attributeHolder == null)
                    return;
            }

            if (!attributeHolder.attributes.TryGetValue((int)attributeType, out AttributeData attributeData))
                return;

            attributeData.onValuePostChange -= OnValuePostChange;
            attributeData.onValuePostChange += OnValuePostChange;
        }

        private void OnValuePostChange(int dir, BigInteger changeValue, BigInteger currentValue, BigInteger maxValue)
        {
            if (!BigInteger.TryParse(this.value, out BigInteger targetValue))
                return;

            bool isFulfilled = false;
            if (operatorValue == Operator.Equal)
                isFulfilled = targetValue == currentValue;
            else if (operatorValue == Operator.LessOrEqual)
                isFulfilled = currentValue <= targetValue;
            else if (operatorValue == Operator.Lesser)
                isFulfilled = currentValue < targetValue;
            else if (operatorValue == Operator.GreaterOrEqual)
                isFulfilled = currentValue >= targetValue;
            else if (operatorValue == Operator.Greater)
                isFulfilled = currentValue > targetValue;

            if (!isFulfilled)
                return;

            Notify(new EvtNotifyData()
            {
                observable = this,
                values = new Dictionary<string, object>()
                {
                    { "attributeType", attributeType },
                    { "currentValue", currentValue },
                    { "changeValue", changeValue },
                    { "maxValue", maxValue },
                }
            });
        }
    }
}
