using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState
{
    // States can include any kind of constructors or instance variables they need
    /// <summary>
    /// Treat this like an update method
    /// </summary>
    void Tick();
    /// <summary>
    /// Stuff to do when this state begins
    /// </summary>
    void OnEnter();
    /// <summary>
    /// Stuff to do when this state is being transitioned to something else
    /// </summary>
    void OnExit();
}
