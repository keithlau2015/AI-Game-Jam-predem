using PortalModule;
using UnityEngine;

namespace EvtModule
{
    public class PortalObserver : EvtObserver
    {
        [SerializeField]
        private PortalTransitionSettings transition = new PortalTransitionSettings();

        [SerializeField]
        private bool useRootGameObject = true;

        protected override void OnExecute(EvtNotifyData notifyData)
        {
            GameObject traveler = ResolveTraveler(notifyData);
            if (traveler == null)
            {
                Debug.LogWarning("[PortalObserver] No traveler found in notify data.", this);
                return;
            }

            if (PortalService.singleton == null)
            {
                Debug.LogError("[PortalObserver] PortalService is missing.", this);
                return;
            }

            PortalService.singleton.ExecuteTransition(transition, traveler);
        }

        private GameObject ResolveTraveler(EvtNotifyData notifyData)
        {
            if (notifyData?.values != null
                && notifyData.values.TryGetValue("gameObject", out object goObj)
                && goObj is GameObject go
                && go != null)
            {
                return useRootGameObject ? go.transform.root.gameObject : go;
            }

            return null;
        }
    }
}
