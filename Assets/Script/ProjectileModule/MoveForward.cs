using UnityEngine;

namespace ProjectileModule
{
    public class MoveForward : MonoBehaviour
    {
        [SerializeField]
        private float baseSpeed = 1f;
        public float speed = 1f;
        private void Update()
        {
            if (GameStateController.singleton.IsPause)
                return;

            transform.Translate(Vector3.forward * speed * baseSpeed * Time.deltaTime);
        }
    }
}