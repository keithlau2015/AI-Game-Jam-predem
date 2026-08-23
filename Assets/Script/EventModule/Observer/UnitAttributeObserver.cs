using AttributeModule;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using static Model.AttributeModel;

namespace EvtModule
{
    /// <summary>
    /// Applies an attribute change to IAttributeHolder targets.
    /// Targets can come from the notifying observable, a tag search, or all holders in scene.
    /// </summary>
    public class UnitAttributeObserver : EvtObserver
    {
        private enum Operator
        {
            Add = 0,
            Subtract = 1,
            Multiply = 2,
            Divide = 3,
        }

        private enum TargetType
        {
            ThroughObservable = 0,
            FindByTag = 1,
            AllInScene = 2,
        }

        [SerializeField]
        private TargetType targetType = TargetType.ThroughObservable;

        [SerializeField]
        private string targetTag = "Player";

        [SerializeField]
        private AttributeType attributeType;

        [SerializeField]
        private Operator operatorType;

        [SerializeField]
        private string affectValue = "0";

        protected override void OnExecute(EvtNotifyData notifyData)
        {
            if (!BigInteger.TryParse(affectValue, out BigInteger parsedAffectValue))
                return;

            List<IAttributeHolder> attributeHolders = CollectTargets(notifyData);
            for (int i = 0; i < attributeHolders.Count; i++)
            {
                IAttributeHolder holder = attributeHolders[i];
                if (holder == null || holder.attributes == null)
                    continue;
                if (!holder.attributes.TryGetValue((int)attributeType, out AttributeData targetAttributeData))
                    continue;
                Apply(targetAttributeData, parsedAffectValue);
            }
        }

        private List<IAttributeHolder> CollectTargets(EvtNotifyData notifyData)
        {
            List<IAttributeHolder> attributeHolders = new List<IAttributeHolder>();

            if (targetType == TargetType.ThroughObservable)
            {
                if (notifyData?.values == null)
                    return attributeHolders;

                if (!notifyData.values.TryGetValue("gameObject", out object gameObjectObj))
                    return attributeHolders;

                GameObject gameObject = gameObjectObj as GameObject;
                if (gameObject == null)
                    return attributeHolders;

                IAttributeHolder attributeHolder = gameObject.GetComponentInParent<IAttributeHolder>();
                if (attributeHolder != null)
                    attributeHolders.Add(attributeHolder);
            }
            else if (targetType == TargetType.FindByTag)
            {
                if (string.IsNullOrEmpty(targetTag))
                    return attributeHolders;

                GameObject[] tagged = GameObject.FindGameObjectsWithTag(targetTag);
                for (int i = 0; i < tagged.Length; i++)
                {
                    IAttributeHolder holder = tagged[i].GetComponentInParent<IAttributeHolder>();
                    if (holder != null && !attributeHolders.Contains(holder))
                        attributeHolders.Add(holder);
                }
            }
            else if (targetType == TargetType.AllInScene)
            {
                MonoBehaviour[] behaviours = FindObjectsOfType<MonoBehaviour>();
                for (int i = 0; i < behaviours.Length; i++)
                {
                    if (behaviours[i] is IAttributeHolder holder && !attributeHolders.Contains(holder))
                        attributeHolders.Add(holder);
                }
            }

            return attributeHolders;
        }

        private void Apply(AttributeData targetAttributeData, BigInteger parsedAffectValue)
        {
            if (operatorType == Operator.Add)
            {
                targetAttributeData.SetValue(parsedAffectValue, AttributeData.EditMode.Add);
                return;
            }

            if (operatorType == Operator.Subtract)
            {
                targetAttributeData.SetValue(-parsedAffectValue, AttributeData.EditMode.Add);
                return;
            }

            if (operatorType == Operator.Multiply)
            {
                targetAttributeData.SetValue(parsedAffectValue, AttributeData.EditMode.Multiply);
                return;
            }

            if (parsedAffectValue == 0)
                return;

            targetAttributeData.SetValue(targetAttributeData.value / parsedAffectValue, AttributeData.EditMode.Replace);
        }
    }
}
