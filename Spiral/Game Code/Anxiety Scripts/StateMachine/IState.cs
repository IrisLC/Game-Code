
public interface IState
{
    /// <summary>
    /// Assigns the Parent and sets up any other necessary variables
    /// </summary>
    /// <param name="Parent">The Script that this state will be affecting</param>
    public abstract void OnInitialize(IStateMachineUser Parent);
    /// <summary>
    /// Treat this like an update method
    /// </summary>
    public virtual void Tick() { }
    /// <summary>
    /// Stuff to do when this state begins
    /// </summary>
    public virtual void OnEnter() { }
    /// <summary>
    /// Stuff to do when this state is being transitioned to something else
    /// </summary>
    public virtual void OnExit() { }

}
