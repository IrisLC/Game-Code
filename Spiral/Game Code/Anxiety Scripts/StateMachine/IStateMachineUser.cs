public interface IStateMachineUser
{
    StateMachine GetStateMachine();
    IState GetInitialState();
}
