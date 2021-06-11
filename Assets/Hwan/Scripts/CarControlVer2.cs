using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarControlVer2 : MonoBehaviour
{
    public enum DriveType
    {
        FrontWheelDrive,
        RearWheelDrive,
        AllWheelDrive
    }

    public DriveType driveType;

    public WheelCollider[] wheelCol;
    public GameObject[] wheelMesh;
    public GameObject centerOfMass;
    public GameObject steerWheel;

    public float torque = 200;
    public float engineRPM;
    public float KPH;
    public float brakePower;
    public float booster = 1000;
    public float steerMaxAngle;

    Rigidbody rb;
    InputManager inputManager;
    public WheelController wc;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        inputManager = GetComponent<InputManager>();
    }

    
    void Update()
    {
        MoveCar();
        SteerCar();
        AniWheelMove();
        GetEngineRpm();
    }

    void MoveCar()
    {
        KPH = rb.velocity.magnitude * 3.6f;

        // »ç·û, ÈÄ·û, Àü·û
        if (driveType == DriveType.AllWheelDrive)
        {
            for (int i = 0; i < wheelCol.Length; i++)
            {
                wheelCol[i].motorTorque = torque * inputManager.accel;
            }
        }
        else if (driveType == DriveType.RearWheelDrive)
        {
            for (int i = 2; i < wheelCol.Length; i++)
            {
                wheelCol[i].motorTorque = torque * inputManager.accel;
            }
        }
        else
        {
            for (int i = 0; i < wheelCol.Length - 2; i++)
            {
                wheelCol[i].motorTorque = torque * inputManager.accel;
            }
        }

        // ÈÄÁø
      
        // ºê·¹ÀÌÅ©
        if (inputManager.brake || inputManager.ovrBrake)
        {
            for (int i = 2; i < wheelCol.Length; i++)
            {
                wheelCol[i].brakeTorque = brakePower;
            }
        }
        else
        {
            for (int i = 0; i < wheelCol.Length; i++)
            {
                wheelCol[i].brakeTorque = 0;
            }
        }

        // ºÎ½ºÆ®
        if (inputManager.boost || inputManager.ovrBoost)
        {
            rb.AddRelativeForce(Vector3.forward * booster);
        }
    }

    void SteerCar()
    {
        for (int i = 0; i < wheelCol.Length - 2; i++)
        {
            if (wc.leftHandOnWheel || wc.rightHandOnWheel)
            {
                wheelCol[i].steerAngle = steerMaxAngle * inputManager.ovrAccel;
                AniSteerWheelMove(wheelCol[i].steerAngle);
            }
            else
            {
                wheelCol[i].steerAngle = steerMaxAngle * inputManager.steer;
                AniSteerWheelMove(wheelCol[i].steerAngle);
            }
        }
    }
    
    void AniSteerWheelMove(float wheelRotation)
    {
        steerWheel.transform.localEulerAngles = new Vector3(0, 0, -wheelRotation * 2f);
    }

    void AniWheelMove()
    {
        Vector3 wheelPosition = Vector3.zero;
        Quaternion wheelRotation = Quaternion.identity;

        for (int i = 0; i < wheelCol.Length; i++)
        {
            wheelCol[i].GetWorldPose(out wheelPosition, out wheelRotation);
            wheelCol[i].transform.position = wheelPosition;
            wheelCol[i].transform.rotation = wheelRotation;
        }
    }

    void GetEngineRpm()
    {   
        float sum = 0;

        for (int i = 0; i < wheelCol.Length; i++)
        {
            sum += wheelCol[i].rpm;
        }

        engineRPM = sum / 4;
    }
}
