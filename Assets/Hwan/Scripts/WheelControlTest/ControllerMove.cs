using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerMove : MonoBehaviour
{
    public GameObject Wheel;

    void Update()
    {
        Move();
        AnimationWheel();
        print(VectorCrossTest());
    }

    void Move()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        Vector3 dir = new Vector3(x, y, 0);

        transform.position += dir * 5 * Time.deltaTime;
    }

    void AnimationWheel()
    {
        Wheel.transform.eulerAngles = VectorCrossTest() * 5;
    }

    Vector3 VectorCrossTest()
    {
        Vector3 v1 = Vector3.forward;
        Vector3 v2 = Wheel.transform.position - transform.position;
        Vector3 dir =  Vector3.Cross(v1, v2);
        return Vector3.Cross(v2, dir);
    }
}
