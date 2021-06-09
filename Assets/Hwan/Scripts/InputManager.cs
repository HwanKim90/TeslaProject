using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public GameObject ovrRightHand;
    public GameObject ovrLeftHand;
    public GameObject SteeringWheel;
    public GameObject originParent;

    public float accel;
    public float steer;
    public bool brake;
    public bool boost;

    public float ovrAccel;
    public float ovrSteer;
    public bool ovrBrake;
    public bool ovrBoost;
    //public bool handleGrabed;

    //float wheelSpeed = 4f;
    
    public GameObject steerWheel;

    float angle;

    private void FixedUpdate()
    {
        accel = Input.GetAxis("Vertical");
        steer = Input.GetAxis("Horizontal");
        brake = Input.GetAxis("Jump") != 0 ? true : false;
        boost = Input.GetKey(KeyCode.Alpha1);

        ovrAccel = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.RTouch);

        //ovrSteer = -ovrRightHand.transform.localPosition.y * wheelSpeed; 
        //ovrSteer = Mathf.Clamp(ovrSteer, -1, 1);
        //ovrSteer = steerWheel.transform.eulerAngles.z * Mathf.Deg2Rad;
        ovrSteer = OvrAccelSetting();

        //print(ovrSteer);
       
        ovrBrake = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.LTouch) != 0 ? true : false;
        ovrBoost = OVRInput.Get(OVRInput.Button.One);

        //GripSteering();
        
    }

    float OvrAccelSetting()
    {

        float angle = steerWheel.transform.eulerAngles.z;// * Mathf.Deg2Rad;
        
        //if (angle < -360)
        //{
        //    angle += 360;
        //}

        //angle *= Mathf.Deg2Rad;

        if(0 < angle && angle <= 180)
        {
            //print(angle);
            return -(angle / 2)  * Mathf.Deg2Rad;
        }
        else if(180 < angle && angle < 360)
        {
            angle = 360 - angle;
            //print(angle);
            return (angle / 2) * Mathf.Deg2Rad;
        }
        else
        {
            return 0;
        }


        //if (angle >= 0 && angle <= 2)
        //{
        //    return angle / 2 * -1;
        //}
        //else if (angle >= 4 && angle <= 6)
        //{
        //    return (6 - angle) / 2;
        //}
        //else
        //{
        //    return 0;
        //}
    }
}
