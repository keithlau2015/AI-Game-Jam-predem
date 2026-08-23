using System;
using UnityEngine;

public abstract class StateMachine : MonoBehaviour
{
    public event Action<string> OnErrorOccur;
    protected State currentState { get; private set; }
    public bool isSwitchingState { get; private set; } = false;
    public void SetState(State state)
    {
        isSwitchingState = true;
        if(currentState != null)
            currentState.OnExit();
        currentState = state;
        currentState.OnEnter();
        isSwitchingState = false;
    }

    public void SetErrorCode(string errorCode)
    {
        OnErrorOccur?.Invoke(errorCode);
    }

    protected virtual void Update()
    {
        if(currentState != null)
            currentState.OnLogicUpdate();
    }

    protected virtual void FixedUpdate()
    {
        if(currentState != null)
            currentState.OnPhysicsUpdate();
    }

    protected virtual void OnDestroy()
    {
        isSwitchingState = true;
        if(currentState != null)
            currentState.OnExit();
        isSwitchingState = false;
        currentState = null;
    }
}