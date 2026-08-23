using UnityEngine;

namespace PortalModule
{
    public class PortalTeleportContext
    {
        public GameObject traveler;
        public PortalTrigger sourceTrigger;
        public PortalDestination destination;
        public string sourcePortalId;
        public string destinationPortalId;
        public bool isCrossScene;
    }
}
