using UnityEngine;
using UnityEngine.InputSystem;

namespace PortalModule
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PortalTestPlayer2D : MonoBehaviour
    {
        [SerializeField]
        private float moveSpeed = 6f;

        private Rigidbody2D body;

        private Vector2 moveInput;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            body.gravityScale = 0f;
            body.freezeRotation = true;
            body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }

        private void Update()
        {
            moveInput = ReadMoveInput();
        }

        private void FixedUpdate()
        {
            body.velocity = moveInput.normalized * moveSpeed;
        }

        private static Vector2 ReadMoveInput()
        {
            Vector2 input = Vector2.zero;
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null)
                return input;

            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
                input.x -= 1f;
            if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
                input.x += 1f;
            if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed)
                input.y -= 1f;
            if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed)
                input.y += 1f;

            return input;
        }
    }
}
