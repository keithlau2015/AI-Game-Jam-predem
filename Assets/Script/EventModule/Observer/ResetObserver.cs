using UnityEngine;

namespace EvtModule
{
    public class ResetObserver : EvtObserver
    {
        [SerializeField]
        private EvtObserable evtNameIdToReset;
        protected override void OnExecute(EvtNotifyData evtNotifyData)
        {
            if(evtNameIdToReset != null)
            {
                evtNameIdToReset.Reset();
            }
        }
    }
}