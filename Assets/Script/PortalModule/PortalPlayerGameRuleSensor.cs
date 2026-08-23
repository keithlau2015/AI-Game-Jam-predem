using UnityEngine;

namespace PortalModule
{
    public class PortalPlayerGameRuleSensor : MonoBehaviour
    {
        [SerializeField]
        private bool useRootGameObject = true;

        private void OnCollisionEnter(Collision collision)
        {
            HandleCollision(collision.collider);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            HandleCollision(collision.collider);
        }

        private void HandleCollision(Component otherCollider)
        {
            if (otherCollider == null)
                return;

            PortalGameRuleController controller = PortalGameRuleController.Instance;
            if (controller == null || !controller.IsPlaying)
                return;

            if (otherCollider.GetComponentInParent<PortalWallHazard>() == null)
                return;

            GameObject player = useRootGameObject ? transform.root.gameObject : gameObject;
            controller.RegisterDefeat(player);
        }
    }
}
