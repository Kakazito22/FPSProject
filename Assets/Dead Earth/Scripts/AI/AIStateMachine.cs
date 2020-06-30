using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum AIStateType { None, Idle, Alerted, Patrol, Attack, Feeding, Pursuit, Dead}
public enum AITargetType { None, WayPoint, Visual_Player, Visual_Light, Visual_Food, Audio}
public enum AiTriggerEventType { Enter, Stay, Exit }

public struct AITarget
{
    private AITargetType _type;          // 当前目标 type
    private Collider _collider;          // 碰撞器
    private Vector3 _position;           // 当前世界坐标
    private float _distance;             // 与 player 距离
    private float _time;

    public AITargetType type { get { return _type; } }
    public Collider collider { get { return _collider; } }
    public Vector3 position { get { return _position; } }
    public float distance { get { return _distance; } set { _distance = value; } }
    public float time { get { return _time; } }

    public void Set(AITargetType t, Collider c, Vector3 p, float d)
    {
        this._type = t;
        this._collider = c;
        this._position = p;
        this._distance = d;
        this._time = UnityEngine.Time.time;
    }

    public void Clear()
    {
        this._type = AITargetType.None;
        this._collider = null;
        this._position = Vector3.zero;
        this._distance = Mathf.Infinity;
        this._time = 0;
    }
}

public abstract class AIStateMachine : MonoBehaviour {
    public AITarget visualThreat = new AITarget();
    public AITarget audioThreat = new AITarget();

    // 保存object上的所有AIState
    protected Dictionary<AIStateType, AIState> _allStates = new Dictionary<AIStateType, AIState>();
    protected AITarget _target = new AITarget();
    protected AIState _currentState = null;

    [SerializeField] protected AIStateType _currentStateType = AIStateType.Idle;
    [SerializeField] protected SphereCollider _targetTrigger = null;
    [SerializeField] protected SphereCollider _sensorTrigger = null;
    [SerializeField] [Range(0, 15)]protected float _stoppingDistance = 1.0f;

    // component cache
    protected Animator _ani = null;
    protected NavMeshAgent _navAgent = null;
    protected Collider _collider = null;

    protected virtual void Awake()
    {
        // 缓存所有常用的 components
        _ani = GetComponent<Animator>();
        _navAgent = GetComponent<NavMeshAgent>();
        _collider = GetComponent<Collider>();

        if (GameSceneManager.Instance != null)
        {
            if (_collider)
                GameSceneManager.Instance.RegisterAIStateMachines(_collider.GetInstanceID(), this);
            if (_sensorTrigger)
                GameSceneManager.Instance.RegisterAIStateMachines(_sensorTrigger.GetInstanceID(), this);
        }
    }

    protected virtual void Start()
    {
        if (_sensorTrigger != null)
        {
            AISensor sensor = _sensorTrigger.GetComponent<AISensor>();
        }

        // 获取物体上所有的AIState
        AIState[] states = GetComponents<AIState>();

        foreach (AIState s in states)
        {
            if (s != null && !_allStates.ContainsKey(s.GetStateType()))
            {
                _allStates[s.GetStateType()] = s;
                s.SetStateMachine(this);
            }
        }

        if (_allStates.ContainsKey(_currentStateType))
        {
            _currentState = _allStates[_currentStateType];
            _currentState.OnEnterState();
        }
        else
        {
            _currentState = null;
        }
    }

    protected virtual void FixedUpdate()
    {
        visualThreat.Clear();
        audioThreat.Clear();

        if (_target.type != AITargetType.None)
        {
            _target.distance = Vector3.Distance(transform.position, _target.position);
        }
    }

    protected virtual void Update()
    {
        if (_currentState == null) return;

        AIStateType newStateType = _currentState.OnUpdate();
        if (newStateType != _currentStateType)
        {
            AIState newState = null;
            if (_allStates.TryGetValue(newStateType, out newState))
            {
                _currentState.OnExitState();
                newState.OnEnterState();
                _currentState = newState;
            }else if (_allStates.TryGetValue(AIStateType.Idle , out newState))
            {
                _currentState.OnExitState();
                newState.OnEnterState();
                _currentState = newState;
            }
            _currentStateType = newStateType;
        }
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (_targetTrigger == null || _targetTrigger != other) return;

        if (_currentState)
            _currentState.OnDestnationReached(true);
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (_targetTrigger == null || _targetTrigger != other) return;
        if (_currentState)
            _currentState.OnDestnationReached(false);
    }

    public virtual void OnTriggerEvent(AiTriggerEventType type, Collider other)
    {
        if (_currentState != null)
            _currentState.OnTriggerEvent(type, other);
    }

    public void SetTarget(AITargetType t, Collider c, Vector3 p, float d)
    {
        _target.Set(t, c, p, d);

        if (_targetTrigger != null)
        {
            _targetTrigger.radius = _stoppingDistance;
            _targetTrigger.transform.position = p;
            _targetTrigger.enabled = true;
        }
    }

    public void SetTarget(AITargetType t, Collider c, Vector3 p, float d, float s)
    {
        _target.Set(t, c, p, d);

        if (_targetTrigger != null)
        {
            _targetTrigger.radius = s;
            _targetTrigger.transform.position = p;
            _targetTrigger.enabled = true;
        }
    }

    public void SetTarget(AITarget t)
    {
        _target = t;

        if (_targetTrigger != null)
        {
            _targetTrigger.radius = _stoppingDistance;
            _targetTrigger.transform.position = t.position;
            _targetTrigger.enabled = true;
        }
    }

    public void ClearTarget()
    {
        _target.Clear();
        if (_targetTrigger != null)
        {
            _targetTrigger.enabled = false;
        }
    }
}
