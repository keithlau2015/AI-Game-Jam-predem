using UnityEngine;

namespace PortalEscort.Debug
{
    /// <summary>
    /// Points a transform toward a Direction using DirectionUtility.GetRotationZ (Contracts §1).
    /// Useful for entrance/exit direction indicators and spawn-direction arrows.
    /// </summary>
    public class DirectionArrow : MonoBehaviour
    {
        [SerializeField] private Direction direction = Direction.Up;

        private void Start()
        {
            ApplyRotation();
        }

        private void Update()
        {
            ApplyRotation();
        }

        private void ApplyRotation()
        {
            float z = DirectionUtility.GetRotationZ(direction);
            transform.rotation = Quaternion.Euler(0f, 0f, z);
        }

        public void SetDirection(Direction newDirection)
        {
            direction = newDirection;
            ApplyRotation();
        }
    }
}
