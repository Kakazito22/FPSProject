﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NavmeshExample : MonoBehaviour {

    public AIWaypointNetwork waypointNetwork = null;
    public int curWaypointIdx = 0;
    public bool hasPath = false;
    public bool pathPending = false;
    public bool pathStale = false;
    public NavMeshPathStatus pathState = NavMeshPathStatus.PathInvalid;
    public AnimationCurve jumpCurve;

    private NavMeshAgent _agent = null;

	void Start () {
        // 缓存 NavMeshAgent 引用
        _agent = GetComponent<NavMeshAgent>();
        SetNextDestination(false);
    }

    // 计算当前导航点的下一个目标点，并事 Agent 移动到下一目标点
    void SetNextDestination(bool isIncrease)
    {
        int increase = isIncrease ? 1 : 0;
        int index = (curWaypointIdx + increase) >= waypointNetwork.waypoints.Count ? 0 : curWaypointIdx + increase;

        Transform nextPoint = waypointNetwork.waypoints[index];
        if(nextPoint != null)
        {
            curWaypointIdx = index;
            _agent.SetDestination(nextPoint.position);
            return;
        }
        // 防止没有找到有效的路径点
        curWaypointIdx++;
    }
	
	// Update is called once per frame
	void Update () {
        hasPath = _agent.hasPath;
        pathPending = _agent.pathPending;
        pathStale = _agent.isPathStale;
        pathState = _agent.pathStatus;

        // 当物体在offMeshLink上时，执行jump动作
        if (_agent.isOnOffMeshLink)
        {
            StartCoroutine(Jump(1));
            return;
        }

        if ((!hasPath && !pathPending) || pathState == NavMeshPathStatus.PathInvalid)
            SetNextDestination(true);
        else if(_agent.isPathStale)
            SetNextDestination(false);
    }

    IEnumerator Jump(float duration)
    {
        Debug.Log("Jump!!!");
        OffMeshLinkData data = _agent.currentOffMeshLinkData;
        Vector3 startPos = _agent.transform.position;
        Vector3 endPos = data.endPos + (_agent.baseOffset * Vector3.up);
        float time = 0;

        while (time <= duration)
        {
            float t = time / duration;
            _agent.transform.position = Vector3.Lerp(startPos, endPos, t) + (jumpCurve.Evaluate(t) * Vector3.up);
            time += Time.deltaTime;
            yield return null;
        }

        _agent.CompleteOffMeshLink();
    }
}
