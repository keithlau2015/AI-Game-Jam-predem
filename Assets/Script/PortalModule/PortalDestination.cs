using UnityEngine;

namespace PortalModule
{
    public class PortalDestination : MonoBehaviour
    {
        [SerializeField]
        private string portalId;

        [SerializeField]
        private Transform spawnPoint;

        [SerializeField]
        private bool useSpawnRotation = true;

        public string PortalId => portalId;
        public Transform SpawnTransform => spawnPoint != null ? spawnPoint : transform;

        public Vector3 GetSpawnPosition()
        {
            return SpawnTransform.position;
        }

        public Quaternion GetSpawnRotation()
        {
            if (!useSpawnRotation)
                return Quaternion.identity;

            return SpawnTransform.rotation;
        }

        public void Configure(string id, Transform spawn, bool spawnRotation)
        {
            portalId = id;
            spawnPoint = spawn;
            useSpawnRotation = spawnRotation;
        }

        private void OnEnable()
        {
            PortalService service = PortalService.Resolve();
            if (service != null)
                service.RegisterDestination(this);
        }

        private void Start()
        {
            PortalService service = PortalService.Resolve();
            if (service != null)
                service.RegisterDestination(this);
        }

        private void OnDisable()
        {
            PortalService service = FindObjectOfType<PortalService>();
            if (service != null)
                service.UnregisterDestination(this);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Transform point = spawnPoint != null ? spawnPoint : transform;
            Gizmos.color = new Color(0.2f, 0.8f, 1f, 0.8f);
            Gizmos.DrawWireSphere(point.position, 0.5f);
            Gizmos.DrawLine(point.position, point.position + point.forward * 1.5f);
        }
#endif
    }
}
