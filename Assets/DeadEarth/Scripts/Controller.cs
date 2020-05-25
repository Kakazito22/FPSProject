using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    private Animator ani;
    // Use this for initialization
    void Start()
    {
        ani = GetComponent<Animator>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // 攻击触发
        if (Input.GetMouseButtonDown(0))
        {
            ani.SetTrigger("Attack");
        }

        // 移动
        float xAxis = Input.GetAxis("Horizontal") * 2.32f;
        float yAxis = Input.GetAxis("Vertical") * 5.66f;

        ani.SetFloat("Horizontal", xAxis, 0.1f, Time.deltaTime);
        ani.SetFloat("Vertical", yAxis, 1f, Time.deltaTime);
    }
}