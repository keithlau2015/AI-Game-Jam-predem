using System.Collections;
using UnityEngine;

namespace PortalEscort.Portal
{
    public enum PortalPlacementState
    {
        Idle,
        SelectingEntrancePosition,
        SelectingEntranceDirection,
        SelectingExitPosition,
        SelectingExitDirection,
        ActiveLocked,
        ActiveReconfigurable
    }

    public class PortalPlacementController : MonoBehaviour
    {
        public static event System.Action<PortalPlacementState> OnPortalStateChanged;

        [Header("State")]
        public PortalPlacementState state = PortalPlacementState.Idle;

        [Header("Tunables (Inspector-driven)")]
        public float maxPortalDistance = 6f;
        public float reconfigurationCooldown = 3f;

        [Header("Layers")]
        public LayerMask groundMask;
        public LayerMask invalidMask;

        [Header("References")]
        public PortalPairController portalPair;
        public PortalEndpoint entranceEndpoint;
        public PortalEndpoint exitEndpoint;

        private Vector2 entrancePos;
        private Vector2 exitPos;
        private Direction entranceDir;
        private Direction exitDir;

        private void Start()
        {
            SetState(PortalPlacementState.Idle);
        }

        private void Update()
        {
            if (state == PortalPlacementState.ActiveLocked)
            {
                return;
            }

            if (state == PortalPlacementState.Idle
                || state == PortalPlacementState.ActiveReconfigurable)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    BeginPlacement();
                }
                return;
            }

            if (!Input.GetMouseButtonDown(0))
            {
                return;
            }

            Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            switch (state)
            {
                case PortalPlacementState.SelectingEntrancePosition:
                    HandleEntrancePosition(mouseWorld);
                    break;
                case PortalPlacementState.SelectingEntranceDirection:
                    HandleEntranceDirection(mouseWorld);
                    break;
                case PortalPlacementState.SelectingExitPosition:
                    HandleExitPosition(mouseWorld);
                    break;
                case PortalPlacementState.SelectingExitDirection:
                    HandleExitDirection(mouseWorld);
                    break;
            }
        }

        private void BeginPlacement()
        {
            entrancePos = Vector2.zero;
            exitPos = Vector2.zero;
            entranceDir = Direction.Up;
            exitDir = Direction.Up;
            SetState(PortalPlacementState.SelectingEntrancePosition);
        }

        private bool IsValidGroundPosition(Vector2 worldPos)
        {
            Collider2D groundHit = Physics2D.OverlapPoint(worldPos, groundMask);
            if (groundHit == null)
            {
                return false;
            }
            Collider2D invalidHit = Physics2D.OverlapPoint(worldPos, invalidMask);
            return invalidHit == null;
        }

        private void HandleEntrancePosition(Vector2 worldPos)
        {
            if (!IsValidGroundPosition(worldPos))
            {
                return;
            }
            entrancePos = worldPos;
            SetState(PortalPlacementState.SelectingEntranceDirection);
        }

        private void HandleEntranceDirection(Vector2 worldPos)
        {
            Vector2 delta = worldPos - entrancePos;
            entranceDir = DirectionUtility.FromDelta(delta);
            if (entranceEndpoint != null)
            {
                entranceEndpoint.direction = entranceDir;
                entranceEndpoint.transform.position = entrancePos;
            }
            SetState(PortalPlacementState.SelectingExitPosition);
        }

        private void HandleExitPosition(Vector2 worldPos)
        {
            if (!IsValidGroundPosition(worldPos))
            {
                return;
            }
            if (Vector2.Distance(entrancePos, worldPos) > maxPortalDistance)
            {
                return;
            }
            exitPos = worldPos;
            SetState(PortalPlacementState.SelectingExitDirection);
        }

        private void HandleExitDirection(Vector2 worldPos)
        {
            Vector2 delta = worldPos - exitPos;
            exitDir = DirectionUtility.FromDelta(delta);
            if (exitEndpoint != null)
            {
                exitEndpoint.direction = exitDir;
                exitEndpoint.transform.position = exitPos;
            }
            CompletePair();
        }

        private void CompletePair()
        {
            if (portalPair != null)
            {
                portalPair.entrance = entranceEndpoint;
                portalPair.exit = exitEndpoint;
                portalPair.IsComplete = true;
            }
            SetState(PortalPlacementState.ActiveLocked);
            StartCoroutine(ReconfigurationCooldown());
        }

        private IEnumerator ReconfigurationCooldown()
        {
            yield return new WaitForSeconds(reconfigurationCooldown);
            if (state == PortalPlacementState.ActiveLocked)
            {
                SetState(PortalPlacementState.ActiveReconfigurable);
            }
        }

        private void SetState(PortalPlacementState newState)
        {
            state = newState;
            OnPortalStateChanged?.Invoke(state);
        }
    }
}
