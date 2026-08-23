using Model;
using UnityEngine;

namespace EvtModule
{
    public class NotifyCountObserable : EvtObserable
    {
        private enum Operator
        {
            Equal = 0,
            GreaterEqual = 1,
            LesserrEqual = 2,
            Lesser = 3,
            Greater = 4
        }

        [SerializeField]
        private string evtId;

        [SerializeField]
        private Operator operatorValue;

        [SerializeField]
        private int evtTriggeredCount;


        private EvtRecordData evtRecord;

        private void Awake()
        {
            if (!EvtRecordData.mapByEvtName.TryGetValue(evtId, out evtRecord))
            {
                evtRecord = new EvtRecordData(evtId);
            }
            evtRecord.onValueChanged += OnValueChanged;

            //Trigger for setting default fullfilled state
            OnValueChanged(evtRecord);
        }

        private void OnValueChanged(EvtRecordData recordData)
        {
            Debug.Log("Check Event Cond EvtTrigger Count");
            bool isFullfilled = false;
            if (this.evtRecord == null)
            {
                Debug.LogWarning("EvtTriggerCount event record data is null!");
                isFullfilled = false;
                return;
            }

            if (operatorValue == Operator.Equal)
            {
                isFullfilled = recordData.value == evtTriggeredCount;
            }
            else if (operatorValue == Operator.GreaterEqual)
            {
                isFullfilled = recordData.value >= evtTriggeredCount;
            }
            else if (operatorValue == Operator.LesserrEqual)
            {
                isFullfilled = recordData.value <= evtTriggeredCount;
            }
            else if (operatorValue == Operator.Lesser)
            {
                isFullfilled = recordData.value < evtTriggeredCount;
            }
            else if (operatorValue == Operator.Greater)
            {
                isFullfilled = recordData.value > evtTriggeredCount;
            }

            Debug.Log($"EvtTriggerCount current value: {recordData.value}, target value: {operatorValue}, isFullfilled: {isFullfilled}");

            if (isFullfilled)
            {
                Notify();
            }
        }
    }
}