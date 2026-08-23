using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.InputSystem.InputAction;

public class ControllableUnit : MonoBehaviour
{
    private NavMeshAgent agent;
    private ObstacleAgent obstacleAgent;
    private float smoothing = 0.25f;
    private Vector3 targetDir;
    private float lerpTime = 0;
    private Vector3 lastDir;
    private Vector3 movement;

    public void SetUp()
    {
        //TODO: add error handling
        TryGetComponent(out agent);
        TryGetComponent(out obstacleAgent);

        if (!InputManager.singleton.playerControl.Player.Move.enabled) InputManager.singleton.playerControl.Player.Move.Enable();
        InputManager.singleton.playerControl.Player.Move.started += MovementHandler;
        InputManager.singleton.playerControl.Player.Move.canceled += MovementHandler;
        InputManager.singleton.playerControl.Player.Move.performed += MovementHandler;
    }

    private void MovementOnCancelHandler(CallbackContext ctx)
    {
        Vector2 input = ctx.ReadValue<Vector2>();
        float moveToward = 0;
        //Speed up logic
        if(input.x > 0)
        {

        }
        //Slow down logic
        else if(input.x < 0)
        {

        }
        
        movement = new Vector3(moveToward, 0, input.y);

        //_agent.enabled = false;
        //obstacleAgent.enabled = true;
    }

    private void MovementHandler(CallbackContext ctx)
    {
        Vector2 input = ctx.ReadValue<Vector2>();
        movement = new Vector3(input.x, 0, input.y);

        //_agent.enabled = true;
        //obstacleAgent.enabled = false;
    }

    private void SpeedUpHandler()
    {

    }

    private void Update()
    {
        movement.Normalize();
        if(movement != lastDir)
        {
            lerpTime = 0;
        }

        lastDir = movement;
        targetDir = Vector3.Lerp(targetDir, movement, Mathf.Clamp01(lerpTime * (1 - smoothing)));

        obstacleAgent.SetDestination(targetDir * agent.speed * Time.deltaTime);
        Vector3 lookDir = movement;
        if(lookDir != Vector3.zero)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(lookDir), Mathf.Clamp01(lerpTime * (1 - smoothing)));
        }

        lerpTime += Time.deltaTime;
    }

    private bool ReachedDestinationOrGaveUp()
    {
        if (!agent.pathPending && agent.enabled)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {                    
                    return true;
                }
            }
        }
        return false;
    }
}
