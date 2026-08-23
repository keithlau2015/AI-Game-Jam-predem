using UnityEngine;
using UnityEngine.InputSystem;

namespace PortalModule
{
    public enum PortalPlayerMoveMode
    {
        Free,
        AxisLocked,
        ForwardOnly
    }

    [RequireComponent(typeof(Rigidbody))]
    public class PortalTestPlayerTopDown : MonoBehaviour, IPortalTeleportable
    {
        [SerializeField]
        private float moveSpeed = 7f;

        [SerializeField]
        private PortalPlayerMoveMode moveMode = PortalPlayerMoveMode.ForwardOnly;

        [SerializeField]
        private Vector3 moveDirection = Vector3.forward;

        [SerializeField]
        private bool directionIsLocal = false;

        [SerializeField]
        private bool autoMove = true;

        private Rigidbody body;
        private Vector3 moveInput;
        private Vector3 resolvedMoveDirection;

        private void Awake()
        {
            body = GetComponent<Rigidbody>();
            body.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
            body.interpolation = RigidbodyInterpolation.Interpolate;
            body.collisionDetectionMode = CollisionDetectionMode.Continuous;
            resolvedMoveDirection = ResolveMoveDirection();
        }

        private void OnValidate()
        {
            resolvedMoveDirection = ResolveMoveDirection();
        }

        private void Update()
        {
            if (autoMove && moveMode != PortalPlayerMoveMode.Free)
                moveInput = resolvedMoveDirection;
            else
                moveInput = ApplyMoveMode(ReadMoveInput());
        }

        private void FixedUpdate()
        {
            Vector3 velocity = moveInput.sqrMagnitude > 0.0001f ? moveInput.normalized * moveSpeed : Vector3.zero;
            body.velocity = new Vector3(velocity.x, body.velocity.y, velocity.z);

            if (moveInput.sqrMagnitude > 0.01f)
            {
                Quaternion look = Quaternion.LookRotation(moveInput.normalized, Vector3.up);
                body.MoveRotation(Quaternion.Slerp(body.rotation, look, 18f * Time.fixedDeltaTime));
            }
        }

        public bool OnBeforePortalTeleport(PortalTeleportContext context)
        {
            return true;
        }

        public void OnAfterPortalTeleport(PortalTeleportContext context)
        {
            if (context.destination == null)
                return;

            Vector3 exitDirection = context.destination.transform.forward;
            exitDirection.y = 0f;
            if (exitDirection.sqrMagnitude < 0.0001f)
                return;

            ApplyTravelDirection(exitDirection.normalized);
        }

        public void ApplyTravelDirection(Vector3 worldDirection)
        {
            worldDirection.y = 0f;
            if (worldDirection.sqrMagnitude < 0.0001f)
                return;

            worldDirection.Normalize();
            if (directionIsLocal)
                moveDirection = transform.InverseTransformDirection(worldDirection);
            else
                moveDirection = worldDirection;

            resolvedMoveDirection = worldDirection;
            body.rotation = Quaternion.LookRotation(worldDirection, Vector3.up);
        }

        private Vector3 ResolveMoveDirection()
        {
            Vector3 direction = directionIsLocal ? transform.TransformDirection(moveDirection) : moveDirection;
            direction.y = 0f;
            if (direction.sqrMagnitude < 0.0001f)
                return Vector3.forward;

            return direction.normalized;
        }

        private Vector3 ApplyMoveMode(Vector3 input)
        {
            if (moveMode == PortalPlayerMoveMode.Free || input.sqrMagnitude < 0.0001f)
                return input;

            resolvedMoveDirection = ResolveMoveDirection();
            float amount = Vector3.Dot(input.normalized, resolvedMoveDirection);
            if (moveMode == PortalPlayerMoveMode.ForwardOnly && amount <= 0f)
                return Vector3.zero;

            return resolvedMoveDirection * Mathf.Abs(amount);
        }

        private static Vector3 ReadMoveInput()
        {
            Vector3 input = Vector3.zero;
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null)
                return input;

            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
                input.x -= 1f;
            if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
                input.x += 1f;
            if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed)
                input.z -= 1f;
            if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed)
                input.z += 1f;

            return input;
        }
    }
}
