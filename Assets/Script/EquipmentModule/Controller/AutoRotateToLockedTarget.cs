using UnityEngine;
namespace EquipmentModule
{
    public class AutoRotateToLockedTarget : MonoBehaviour
    {
        private Transform target;

        [SerializeField]
        public float rotateSpeed = 10f; // ±ÛÂà³t«×

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }

        private void Update()
        {
            if (target == null)
            {

                Quaternion forward = Quaternion.LookRotation(transform.parent.forward);
                transform.rotation = Quaternion.Slerp(transform.rotation, forward, rotateSpeed * Time.deltaTime);
                return;
            }

            Vector3 direction = target.position - transform.position;
            direction.y = 0;

            if (direction.sqrMagnitude < 0.001f) return;

            Quaternion targetRotation = Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
        }
    }
}