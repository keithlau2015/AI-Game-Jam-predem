using UnityEngine;
using UnityEngine.InputSystem;

namespace PortalModule
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PortalTestPlayer2D : MonoBehaviour
    {
        [SerializeField]
        private float moveSpeed = 6f;

        [SerializeField]
        private PortalPlayerMoveMode moveMode = PortalPlayerMoveMode.Free;

        [SerializeField]
        private Vector2 moveDirection = Vector2.up;

        [SerializeField]
        private bool directionIsLocal = true;

        private Rigidbody2D body;
        private Vector2 moveInput;
        private Vector2 resolvedMoveDirection;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            body.gravityScale = 0f;
            body.freezeRotation = true;
            body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            resolvedMoveDirection = ResolveMoveDirection();
        }

        private void OnValidate()
        {
            resolvedMoveDirection = ResolveMoveDirection();
        }

        private void Update()
        {
            moveInput = ApplyMoveMode(ReadMoveInput());
        }

        private void FixedUpdate()
        {
            body.velocity = moveInput.normalized * moveSpeed;
        }

        private Vector2 ResolveMoveDirection()
        {
            Vector2 direction = directionIsLocal
                ? (Vector2)transform.TransformDirection(moveDirection)
                : moveDirection;
            if (direction.sqrMagnitude < 0.0001f)
                return Vector2.up;

            return direction.normalized;
        }

        private Vector2 ApplyMoveMode(Vector2 input)
        {
            if (moveMode == PortalPlayerMoveMode.Free || input.sqrMagnitude < 0.0001f)
                return input;

            resolvedMoveDirection = ResolveMoveDirection();
            float amount = Vector2.Dot(input.normalized, resolvedMoveDirection);
            if (moveMode == PortalPlayerMoveMode.ForwardOnly && amount <= 0f)
                return Vector2.zero;

            return resolvedMoveDirection * Mathf.Abs(amount);
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
