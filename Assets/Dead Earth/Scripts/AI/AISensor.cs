﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISensor : MonoBehaviour {

    private AIStateMachine _parentStateMachine = null;
	public AIStateMachine ParentStateMachine { set { _parentStateMachine = value; } }

    private void OnTriggerEnter(Collider other)
    {
        if (_parentStateMachine != null)
            _parentStateMachine.OnTriggerEvent(AiTriggerEventType.Enter, other);
    }

    private void OnTriggerStay(Collider other)
    {
        if (_parentStateMachine != null)
            _parentStateMachine.OnTriggerEvent(AiTriggerEventType.Stay, other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (_parentStateMachine != null)
            _parentStateMachine.OnTriggerEvent(AiTriggerEventType.Exit, other);
    }
}
