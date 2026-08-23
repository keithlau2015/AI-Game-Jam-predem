using AttributeModule;
using ObjetPoolModule;
using System.Numerics;
using UnityEngine;
using static Model.AttributeModel;

namespace EvtModule
{
    /// <summary>
    /// Generic replacement for battle "survivor count" logic.
    /// Counts active tagged objects (optionally requiring alive HP via IAttributeHolder)
    /// and notifies when the count matches the comparison.
    /// </summary>
    public class EntityCountObservable : EvtObserable
    {
        private enum Operator
        {
            Equal = 0,
            LessOrEqual = 1,
            Lesser = 2,
            GreaterOrEqual = 3,
            Greater = 4,
        }

        [SerializeField]
        private string entityTag = "Enemy";

        [SerializeField]
        private int targetCount;

        [SerializeField]
        private Operator operatorValue = Operator.Equal;

        [Tooltip("If true, only count IAttributeHolders whose alive attribute is above zero.")]
        [SerializeField]
        private bool requireAliveHp = true;

        [SerializeField]
        private AttributeType aliveAttribute = AttributeType.HP;

        private void Awake()
        {
            if (ObjectPoolManager.singleton != null)
            {
                ObjectPoolManager.singleton.onObjectSpawn += OnPoolChanged;
                ObjectPoolManager.singleton.onObjectDiscard += OnPoolChanged;
            }
        }

        private void OnEnable()
        {
            if (ObjectPoolManager.singleton != null)
            {
                ObjectPoolManager.singleton.onObjectSpawn -= OnPoolChanged;
                ObjectPoolManager.singleton.onObjectDiscard -= OnPoolChanged;
                ObjectPoolManager.singleton.onObjectSpawn += OnPoolChanged;
                ObjectPoolManager.singleton.onObjectDiscard += OnPoolChanged;
            }
        }

        private void OnDisable()
        {
            if (ObjectPoolManager.singleton != null)
            {
                ObjectPoolManager.singleton.onObjectSpawn -= OnPoolChanged;
                ObjectPoolManager.singleton.onObjectDiscard -= OnPoolChanged;
            }
        }

        private void OnPoolChanged(PoolObjectProperty _)
        {
            Evaluate();
        }

        private void Evaluate()
        {
            if (string.IsNullOrEmpty(entityTag))
                return;

            GameObject[] tagged = GameObject.FindGameObjectsWithTag(entityTag);
            int count = 0;
            for (int i = 0; i < tagged.Length; i++)
            {
                GameObject go = tagged[i];
                if (go == null || !go.activeInHierarchy)
                    continue;

                if (requireAliveHp)
                {
                    IAttributeHolder holder = go.GetComponentInParent<IAttributeHolder>();
                    if (holder == null || holder.attributes == null)
                        continue;
                    if (!holder.attributes.TryGetValue((int)aliveAttribute, out AttributeData attr))
                        continue;
                    if (attr.value <= BigInteger.Zero)
                        continue;
                }

                count++;
            }

            bool fulfilled = false;
            if (operatorValue == Operator.Equal)
                fulfilled = count == targetCount;
            else if (operatorValue == Operator.LessOrEqual)
                fulfilled = count <= targetCount;
            else if (operatorValue == Operator.Lesser)
                fulfilled = count < targetCount;
            else if (operatorValue == Operator.GreaterOrEqual)
                fulfilled = count >= targetCount;
            else if (operatorValue == Operator.Greater)
                fulfilled = count > targetCount;

            if (fulfilled)
                Notify();
        }
    }
}
