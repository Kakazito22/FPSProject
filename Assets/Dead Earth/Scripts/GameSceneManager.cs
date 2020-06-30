using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneManager : MonoBehaviour {

    private static GameSceneManager _instance = null;

    public static GameSceneManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<GameSceneManager>();
            return _instance;
        }
    }

    private Dictionary<int, AIStateMachine> _stateMachines = new Dictionary<int, AIStateMachine>();

    public void RegisterAIStateMachines(int key, AIStateMachine machine)
    {
        if (!_stateMachines.ContainsKey(key))
        {
            _stateMachines[key] = machine;
        }
    }

    public AIStateMachine GetAIStateMachine(int key)
    {
        return _stateMachines.ContainsKey(key) ? _stateMachines[key] : null;
    }
}
