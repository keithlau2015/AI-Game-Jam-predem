public abstract class GameState : State
{
    protected GameStateMachine gameStateMachine;
    protected override StateMachine stateMachine { get => gameStateMachine; set => gameStateMachine = (GameStateMachine)value; }

    public GameState(GameStateMachine stateMachine) : base(stateMachine)
    {
        this.gameStateMachine = stateMachine;
    }
}
