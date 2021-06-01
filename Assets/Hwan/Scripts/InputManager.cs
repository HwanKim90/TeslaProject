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
    public bool handleGrabed;

    float wheelSpeed = 5f;
    Transform gripedSteer;
    public Transform gripedSteerRight;

    private void FixedUpdate()
    {
        accel = Input.GetAxis("Vertical");
        steer = Input.GetAxis("Horizontal");
        brake = Input.GetAxis("Jump") != 0 ? true : false;
        boost = Input.GetKey(KeyCode.Alpha1);

        ovrAccel = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.RTouch);
        ovrSteer = -ovrRightHand.transform.localPosition.y * wheelSpeed; 
        ovrBrake = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.LTouch) != 0 ? true : false;
        ovrBoost = OVRInput.GetDown(OVRInput.Button.One);

        GripSteering();
    }

    public void GripSteering()
    {
        if (OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger) != 0)
        {
            print("누름");
            int layer = 1 << LayerMask.NameToLayer("Steer");
            Collider[] hits = Physics.OverlapSphere(ovrRightHand.transform.position, 0.005f, layer);
            
            if (hits.Length > 0)
            {
                print("핸들잡음");
                handleGrabed = true;
               
                ovrRightHand.transform.SetParent(SteeringWheel.transform);
                ovrLeftHand.transform.SetParent(SteeringWheel.transform);
                //ovrRightHand.transform.localPosition = gripedSteerRight.transform.localPosition;
            }
        }
        else
        {
            print("핸들놓음");
            handleGrabed = false;
            ovrRightHand.transform.SetParent(originParent.transform);
            ovrLeftHand.transform.SetParent(originParent.transform);
        }
    }
}
