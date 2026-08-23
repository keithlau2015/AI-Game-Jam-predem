using CombatUnitModule;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class FacingToAim : MonoBehaviour
{
    private Transform target;
    private Vector2 currentMousePos;
    private float rotationSpeed = 10f;
    private float originalXRotation;
    private float originalZRotation;
    private bool useCursor;

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    private void Awake()
    {
        CombatUnitAgent ownerAgent = GetComponentInParent<CombatUnitAgent>();
        useCursor = ownerAgent != null && ownerAgent.team == Team.Blue;
        originalXRotation = transform.eulerAngles.x;
        originalZRotation = transform.eulerAngles.z;

        if (useCursor)
        {
            InputManager.singleton.playerControl.UI.Point.Enable();
            InputManager.singleton.playerControl.UI.Point.performed += GetPlayerMousePos;
        }
    }

    private void Update()
    {
        if (GameStateController.singleton != null && GameStateController.singleton.IsPause)
            return;

        Vector3 aimPoint;
        if (useCursor)
        {
            if (!TryGetCursorWorldPoint(out aimPoint))
                return;
        }
        else
        {
            if (target == null)
                return;
            aimPoint = target.position;
        }

        Vector3 direction = aimPoint - transform.position;
        direction.y = 0;

        if (direction.magnitude <= 0.1f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        float targetYRotation = targetRotation.eulerAngles.y;
        Vector3 currentRotation = transform.eulerAngles;
        float newYRotation = Mathf.LerpAngle(currentRotation.y, targetYRotation, rotationSpeed * Time.deltaTime);
        transform.eulerAngles = new Vector3(originalXRotation, newYRotation, originalZRotation);
    }

    private bool TryGetCursorWorldPoint(out Vector3 worldPoint)
    {
        worldPoint = default;
        if (Camera.main == null)
            return false;

        Ray ray = Camera.main.ScreenPointToRay(new Vector3(currentMousePos.x, currentMousePos.y, 0));
        Plane groundPlane = new Plane(Vector3.up, transform.position);
        if (!groundPlane.Raycast(ray, out float distance))
            return false;

        worldPoint = ray.GetPoint(distance);
        return true;
    }

    private void GetPlayerMousePos(CallbackContext ctx)
    {
        currentMousePos = ctx.ReadValue<Vector2>();
    }
}
