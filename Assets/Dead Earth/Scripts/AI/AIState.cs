using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIState : MonoBehaviour {
    public void SetStateMachine(AIStateMachine asm) { this._stateMachine = asm; }

    public virtual void OnEnterState() { }
    public virtual void OnExitState() { }
    public virtual void OnAnimatorUpdated() { }
    public virtual void OnAnimatorIKUpdated() { }
    public virtual void OnTriggerEvent() { }
    public virtual void OnDestnationReached(bool isReached) { }


    public abstract AIStateType GetStateType();
    public abstract AIStateType OnUpdate();


    protected AIStateMachine _stateMachine;
}
