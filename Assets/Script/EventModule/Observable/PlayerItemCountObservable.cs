
using ItemModule;
using SaveLoadModule;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerItemCountObservable : EvtObserable
{
    private enum Operator
    {
        Equal = 0,
        NotEqual = 1,
        GreaterThan = 2,
        LessThan = 3,
        LessThanOrEqual = 4,
        GreaterThanOrEqual = 5,
    }

    [SerializeField]
    private string itemID;

    [SerializeField]
    private Operator operatorValue;

    [SerializeField]
    private long targetCount;

    
    private ItemData targetItemData;

    private void Awake()
    {
        targetItemData = ItemData.GetStackableItemDataByOwnerNID(SaveLoadController.currentSaveKey.ToString(), itemID);
        if (targetItemData == null)
        {
            //Create a new ItemData if it doesn't exist prevent unable to observer bug
            targetItemData = new ItemData(itemID);
            targetItemData.ownerUID = SaveLoadController.currentSaveKey.ToString();
        }
        else
        {
            targetItemData = ItemData.GetStackableItemDataByOwnerNID(SaveLoadController.currentSaveKey.ToString(), itemID);
        }

        targetItemData.OnItemCountChanged += OnItemCountChanged;
    }
    
    private void OnItemCountChanged(long addonAmount)
    {
        EvtNotifyData notifyData = new EvtNotifyData()
        {
            observable = this,
            values = new System.Collections.Generic.Dictionary<string, object>()
                {
                    { "itemID", itemID },
                    { "currentCount", targetItemData.count },
                    { "targetCount", targetCount },
                    { "addonAmount", addonAmount },
                }
        };

        
        if(operatorValue == Operator.NotEqual)
        {
            if(targetItemData.count != targetCount)
            {
                Notify(notifyData);
            }
        }
        else if(operatorValue == Operator.Equal)
        {
            if(targetItemData.count == targetCount)
            {
                Notify(notifyData);
            }
        }
        else if(operatorValue == Operator.GreaterThan)
        {
            if(targetItemData.count > targetCount)
            {
                Notify(notifyData);
            }
        }
        else if(operatorValue == Operator.LessThan)
        {
            if(targetItemData.count < targetCount)
            {
                Notify(notifyData);
            }
        }
        else if(operatorValue == Operator.GreaterThanOrEqual)
        {
            if(targetItemData.count >= targetCount)
            {
                Notify(notifyData);
            }
        }
        else if(operatorValue == Operator.LessThanOrEqual)
        {
            if(targetItemData.count <= targetCount)
            {
                Notify(notifyData);
            }
        }
    }
}