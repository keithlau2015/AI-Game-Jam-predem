using Cysharp.Threading.Tasks;
using System;

public abstract class State
{
    protected virtual StateMachine stateMachine { get; set; }

    public State(StateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    public abstract void OnEnterAsync();

    public abstract void OnLogicUpdate();

    public abstract void OnPhysicsUpdate();

    public abstract void OnExitAsync();
}