using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DisplayMode { None, Connect, Path };

public class AIWaypointNetwork : MonoBehaviour {
    [HideInInspector]
    public DisplayMode displayMode = DisplayMode.Connect;
    [HideInInspector]
    public int UIStart;
    [HideInInspector]
    public int UIEnd;
    public List<Transform> waypoints = new List<Transform>();
}
