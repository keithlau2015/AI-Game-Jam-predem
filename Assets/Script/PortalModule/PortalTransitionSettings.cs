using System;
using UnityEngine;

namespace PortalModule
{
    public enum PortalTransitionMode
    {
        SameSceneDestination = 0,
        LoadSceneByName = 1,
        LoadLevelByKey = 2,
    }

    [Serializable]
    public class PortalTransitionSettings
    {
        public PortalTransitionMode mode = PortalTransitionMode.SameSceneDestination;
        public string sourcePortalId;
        public string destinationPortalId;
        public string targetSceneName;
        public string targetLevelKey;
    }
}
