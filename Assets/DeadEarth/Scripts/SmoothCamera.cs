using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothCamera : MonoBehaviour {
    public Transform MoveCam;
    public float Speed = 5;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void LateUpdate () {
        // 相机跟随
        transform.position = Vector3.Lerp(transform.position, MoveCam.position, Time.deltaTime * Speed);
        transform.rotation = Quaternion.Slerp(transform.rotation, MoveCam.rotation, Time.deltaTime * Speed);
	}
}
