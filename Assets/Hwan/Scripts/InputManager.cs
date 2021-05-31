using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public float accel;
    public float steer;
    public bool brake;
    public bool boost;

    private void FixedUpdate()
    {
        accel = Input.GetAxis("Vertical");
        steer = Input.GetAxis("Horizontal");
        brake = Input.GetAxis("Jump") != 0 ? true : false;
        boost = Input.GetKey(KeyCode.Alpha1);
    }
}
