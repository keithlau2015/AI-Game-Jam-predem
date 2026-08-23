using UnityEngine;
using UnityEngine.InputSystem;

namespace PortalModule
{
    public class PortalPlacementController : MonoBehaviour
    {
        private enum PlacementPhase
        {
            Idle,
            PreviewFirst,
            PreviewSecond,
            DragSecondDirection
        }

        [SerializeField]
        private Camera placementCamera;

        [SerializeField]
        private LayerMask placementMask = ~0;

        [SerializeField]
        private float maxPlacementDistance = 40f;

        [SerializeField]
        private float spawnOffsetDistance = 2.8f;

        [SerializeField]
        private float portalCooldownSeconds = 0.5f;

        [SerializeField]
        private float previewAlpha = 0.55f;

        [SerializeField]
        private float minDragPixels = 12f;

        [SerializeField]
        private Color firstPortalColor = new Color(0.25f, 0.75f, 1f);

        [SerializeField]
        private Color secondPortalColor = new Color(1f, 0.5f, 0.25f);

        [SerializeField]
        private bool blockMovementWhilePlacing = true;

        [SerializeField]
        private bool directionRelativeToPlayer = true;

        [SerializeField]
        private bool showControlsHint = true;

        private PlacementPhase phase = PlacementPhase.Idle;
        private GameObject firstPreview;
        private GameObject secondPreview;
        private GameObject activePairRoot;
        private Vector3 firstPortalPosition;
        private Quaternion firstPortalRotation;
        private Vector3 secondPortalPosition;
        private Quaternion secondPortalRotation;
        private Vector2 dragStartScreen;
        private Vector3 snappedDirection = Vector3.forward;
        private int pairCounter;
        private PortalTestPlayerTopDown movement;
        private bool movementWasEnabled;

        private void Awake()
        {
            movement = GetComponent<PortalTestPlayerTopDown>();
            if (placementCamera == null)
                placementCamera = Camera.main;
        }

        private void Update()
        {
            if (placementCamera == null)
                return;

            switch (phase)
            {
                case PlacementPhase.Idle:
                    HandleIdleInput();
                    break;
                case PlacementPhase.PreviewFirst:
                    UpdateFirstPreview();
                    HandlePreviewFirstInput();
                    break;
                case PlacementPhase.PreviewSecond:
                    UpdateSecondPreview();
                    HandlePreviewSecondInput();
                    break;
                case PlacementPhase.DragSecondDirection:
                    UpdateDragSecondPreview();
                    HandleDragSecondInput();
                    break;
            }
        }

        private void HandleIdleInput()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null || !keyboard.spaceKey.wasPressedThisFrame)
                return;

            BeginPlacement();
        }

        private void BeginPlacement()
        {
            ClearActivePair();
            DestroyPreview(ref firstPreview);
            DestroyPreview(ref secondPreview);

            phase = PlacementPhase.PreviewFirst;
            firstPreview = PortalRuntimeBuilder.CreatePreviewVisual(firstPortalColor, previewAlpha);
            SetMovementBlocked(true);
        }

        private void CancelPlacement()
        {
            DestroyPreview(ref firstPreview);
            DestroyPreview(ref secondPreview);
            phase = PlacementPhase.Idle;
            SetMovementBlocked(false);
        }

        private void UpdateFirstPreview()
        {
            if (!TryGetPlacementRaycast(out RaycastHit hit))
                return;

            Quaternion rotation = ResolvePlacementRotation(hit, transform.forward);
            PortalRuntimeBuilder.UpdatePreviewVisual(firstPreview, hit.point, rotation, firstPortalColor, previewAlpha, true);
        }

        private void HandlePreviewFirstInput()
        {
            if (WasCancelPressed())
            {
                CancelPlacement();
                return;
            }

            Mouse mouse = Mouse.current;
            if (mouse == null || !mouse.leftButton.wasPressedThisFrame)
                return;

            if (!TryGetPlacementRaycast(out RaycastHit hit))
                return;

            firstPortalPosition = hit.point;
            firstPortalRotation = ResolvePlacementRotation(hit, transform.forward);
            PortalRuntimeBuilder.UpdatePreviewVisual(firstPreview, firstPortalPosition, firstPortalRotation, firstPortalColor, 1f, true);

            secondPreview = PortalRuntimeBuilder.CreatePreviewVisual(secondPortalColor, previewAlpha);
            phase = PlacementPhase.PreviewSecond;
        }

        private void UpdateSecondPreview()
        {
            if (!TryGetPlacementRaycast(out RaycastHit hit))
                return;

            Vector3 direction = directionRelativeToPlayer ? GetFlatForward(transform) : Vector3.forward;
            Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
            PortalRuntimeBuilder.UpdatePreviewVisual(secondPreview, hit.point, rotation, secondPortalColor, previewAlpha, true);
        }

        private void HandlePreviewSecondInput()
        {
            if (WasCancelPressed())
            {
                CancelPlacement();
                return;
            }

            Mouse mouse = Mouse.current;
            if (mouse == null || !mouse.leftButton.wasPressedThisFrame)
                return;

            if (!TryGetPlacementRaycast(out RaycastHit hit))
                return;

            secondPortalPosition = hit.point;
            snappedDirection = directionRelativeToPlayer ? GetFlatForward(transform) : Vector3.forward;
            secondPortalRotation = Quaternion.LookRotation(snappedDirection, Vector3.up);
            dragStartScreen = mouse.position.ReadValue();
            PortalRuntimeBuilder.UpdatePreviewVisual(secondPreview, secondPortalPosition, secondPortalRotation, secondPortalColor, previewAlpha, true);
            phase = PlacementPhase.DragSecondDirection;
        }

        private void UpdateDragSecondPreview()
        {
            Mouse mouse = Mouse.current;
            if (mouse == null)
                return;

            Vector2 currentScreen = mouse.position.ReadValue();
            Vector2 dragDelta = currentScreen - dragStartScreen;
            if (dragDelta.sqrMagnitude >= minDragPixels * minDragPixels)
                snappedDirection = SnapDirectionFromScreenDelta(dragDelta);

            secondPortalRotation = Quaternion.LookRotation(snappedDirection, Vector3.up);
            PortalRuntimeBuilder.UpdatePreviewVisual(secondPreview, secondPortalPosition, secondPortalRotation, secondPortalColor, previewAlpha, true);
        }

        private void HandleDragSecondInput()
        {
            if (WasCancelPressed())
            {
                CancelPlacement();
                return;
            }

            Mouse mouse = Mouse.current;
            if (mouse == null || !mouse.leftButton.wasReleasedThisFrame)
                return;

            FinalizePortalPair(secondPortalPosition, secondPortalRotation);
        }

        private void FinalizePortalPair(Vector3 secondPosition, Quaternion secondRotation)
        {
            DestroyPreview(ref firstPreview);
            DestroyPreview(ref secondPreview);

            string firstId = GetPortalId("A");
            string secondId = GetPortalId("B");

            ClearActivePair();

            GameObject pairRoot = new GameObject($"PlayerPortalPair_{pairCounter}");
            activePairRoot = pairRoot;

            PortalRuntimeBuilder.PortalSideBuild firstSide = CreatePortalSideUnder(
                pairRoot.transform,
                "Portal_A",
                firstPortalPosition,
                firstPortalRotation,
                firstPortalColor,
                firstId,
                secondId);

            PortalRuntimeBuilder.PortalSideBuild secondSide = CreatePortalSideUnder(
                pairRoot.transform,
                "Portal_B",
                secondPosition,
                secondRotation,
                secondPortalColor,
                secondId,
                firstId);

            PortalService service = PortalService.Resolve();
            if (service != null)
            {
                service.RegisterDestination(firstSide.destination);
                service.RegisterDestination(secondSide.destination);
            }

            pairCounter++;
            phase = PlacementPhase.Idle;
            SetMovementBlocked(false);
        }

        private PortalRuntimeBuilder.PortalSideBuild CreatePortalSideUnder(
            Transform parent,
            string name,
            Vector3 position,
            Quaternion rotation,
            Color color,
            string portalId,
            string destinationPortalId)
        {
            PortalRuntimeBuilder.PortalSideBuild build = PortalRuntimeBuilder.CreatePortalSide(
                name,
                position,
                rotation,
                color,
                portalId,
                destinationPortalId,
                spawnOffsetDistance,
                portalCooldownSeconds);
            build.root.transform.SetParent(parent, true);
            return build;
        }

        private bool TryGetPlacementRaycast(out RaycastHit hit)
        {
            hit = default;
            Mouse mouse = Mouse.current;
            if (mouse == null)
                return false;

            Vector2 screenPoint = mouse.position.ReadValue();
            Ray ray = placementCamera.ScreenPointToRay(screenPoint);
            return Physics.Raycast(ray, out hit, maxPlacementDistance, placementMask, QueryTriggerInteraction.Ignore);
        }

        private static Quaternion ResolvePlacementRotation(RaycastHit hit, Vector3 fallbackForward)
        {
            Vector3 forward = fallbackForward;
            forward.y = 0f;
            if (forward.sqrMagnitude < 0.0001f)
                forward = Vector3.forward;

            if (Mathf.Abs(hit.normal.y) < 0.5f)
                forward = -hit.normal;

            forward.y = 0f;
            if (forward.sqrMagnitude < 0.0001f)
                forward = Vector3.forward;

            return Quaternion.LookRotation(forward.normalized, Vector3.up);
        }

        private Vector3 SnapDirectionFromScreenDelta(Vector2 screenDelta)
        {
            Vector3 cameraRight = placementCamera.transform.right;
            cameraRight.y = 0f;
            Vector3 cameraForward = placementCamera.transform.forward;
            cameraForward.y = 0f;

            if (cameraRight.sqrMagnitude < 0.0001f)
                cameraRight = Vector3.right;
            else
                cameraRight.Normalize();

            if (cameraForward.sqrMagnitude < 0.0001f)
                cameraForward = Vector3.forward;
            else
                cameraForward.Normalize();

            Vector3 worldDelta = cameraRight * screenDelta.x + cameraForward * screenDelta.y;
            if (worldDelta.sqrMagnitude < 0.0001f)
                return directionRelativeToPlayer ? GetFlatForward(transform) : Vector3.forward;

            worldDelta.Normalize();
            Vector3 forward = directionRelativeToPlayer ? GetFlatForward(transform) : Vector3.forward;
            Vector3 back = -forward;
            Vector3 right = directionRelativeToPlayer ? GetFlatRight(transform) : Vector3.right;
            Vector3 left = -right;

            Vector3[] directions = { forward, back, right, left };
            Vector3 best = forward;
            float bestDot = float.MinValue;
            for (int i = 0; i < directions.Length; i++)
            {
                float dot = Vector3.Dot(worldDelta, directions[i]);
                if (dot > bestDot)
                {
                    bestDot = dot;
                    best = directions[i];
                }
            }

            return best;
        }

        private static Vector3 GetFlatForward(Transform source)
        {
            Vector3 forward = source.forward;
            forward.y = 0f;
            return forward.sqrMagnitude < 0.0001f ? Vector3.forward : forward.normalized;
        }

        private static Vector3 GetFlatRight(Transform source)
        {
            Vector3 right = source.right;
            right.y = 0f;
            return right.sqrMagnitude < 0.0001f ? Vector3.right : right.normalized;
        }

        private string GetPortalId(string suffix)
        {
            return $"PlayerPortal_{pairCounter}_{suffix}";
        }

        private void ClearActivePair()
        {
            if (activePairRoot == null)
                return;

            Object.Destroy(activePairRoot);
            activePairRoot = null;
        }

        private static void DestroyPreview(ref GameObject preview)
        {
            if (preview == null)
                return;

            Object.Destroy(preview);
            preview = null;
        }

        private void SetMovementBlocked(bool blocked)
        {
            if (movement == null || !blockMovementWhilePlacing)
                return;

            if (blocked)
            {
                movementWasEnabled = movement.enabled;
                movement.enabled = false;
                Rigidbody body = GetComponent<Rigidbody>();
                if (body != null)
                {
                    body.velocity = Vector3.zero;
                    body.angularVelocity = Vector3.zero;
                }
            }
            else if (movementWasEnabled)
            {
                movement.enabled = true;
            }
        }

        private static bool WasCancelPressed()
        {
            Keyboard keyboard = Keyboard.current;
            return keyboard != null && keyboard.escapeKey.wasPressedThisFrame;
        }

        private void OnGUI()
        {
            if (!showControlsHint)
                return;

            string message;
            switch (phase)
            {
                case PlacementPhase.PreviewFirst:
                    message = "Space: placing portal 1 | Move mouse to preview | Left Click: confirm | Esc: cancel";
                    break;
                case PlacementPhase.PreviewSecond:
                case PlacementPhase.DragSecondDirection:
                    message = "Portal 1 locked | Move mouse to pick portal 2 spot | Hold Left Click + drag direction | Release: place | Esc: cancel";
                    break;
                default:
                    message = "Auto walk forward | Space: place portals | Esc: cancel";
                    break;
            }

            GUIStyle style = new GUIStyle(GUI.skin.box)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 14
            };
            GUI.Box(new Rect(12f, 12f, 760f, 34f), message, style);
        }
    }
}
