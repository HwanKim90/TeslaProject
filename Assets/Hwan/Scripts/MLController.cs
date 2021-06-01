using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MLController : MonoBehaviourPun, IPunObservable
{
    public enum DriveType
    {
        FrontWheelDrive,
        RearWheelDrive,
        AllWheelDrive
    }

    public DriveType driveType;

    public WheelCollider[] wheelCollider = new WheelCollider[4];
    public GameObject[] wheelMesh = new GameObject[4];
    public float[] gears = new float[5];
    public GameObject centerOfMess;
    public GameObject steerWheel;
    float[] slips = new float[4];

    public float totalPower;
    public AnimationCurve enginePower;
    public float KPH;
    public float wheelsRPM;
    public float engineRPM;
    public int gearNum;

    //public float torque = 200f;
    public float radius = 6f; // ��Ŀ�� ����� ȸ������
    public float downForce = 50f;
    public float brakePower = 10000f;
    public float boostPower = 1000f;
    //public float steerMaxRot = 10f;
    float smoothTime = 0.01f;

    InputManager inputManager;
    Rigidbody rb;

    // �������
    //public GameObject myCar;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }

        if (stream.IsReading)
        {

        }
    }

    void Start()
    {
        inputManager = GetComponent<InputManager>();
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = centerOfMess.transform.localPosition;
    }

    private void FixedUpdate()
    {
        animateWheels();
        SteerVehicle();
        AddDownForce();
        //GetFriction();
        GearShifter();
        CalculateEnginePower();
    }

    void CalculateEnginePower()
    {
        WheelRPM();

        if (OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.RTouch) != 0)
        {
            // vr
            totalPower = enginePower.Evaluate(engineRPM) * gears[gearNum] * inputManager.ovrAccel;
        }
        else
        {
            // Ű����
            totalPower = enginePower.Evaluate(engineRPM) * gears[gearNum] * inputManager.accel;
        }

        float velocity = 0.0f;
        engineRPM = Mathf.SmoothDamp(engineRPM, Mathf.Abs(wheelsRPM) * 3.6f * gears[gearNum], ref velocity, smoothTime);

        MoveVehicle();
    }

    void WheelRPM()
    {
        float sum = 0f;
        int R = 0;

        for (int i = 0; i < wheelCollider.Length; i++)
        {
            sum += wheelCollider[i].rpm;
            R++;
        }
        wheelsRPM = (R != 0) ? sum / R : 0;
        
    }

    void GearShifter()
    {
        gearNum = Mathf.Clamp(gearNum, 0, 4);
        if (Input.GetKeyDown(KeyCode.Alpha2)) gearNum++;
        if (Input.GetKeyDown(KeyCode.Alpha3)) gearNum--;
    }

    public void MoveVehicle()
    {
        // rigidbody �ӷ��� KPH�� �ٲ��ִ� ���� 3.6�� (60 * 60 / 1000)
        KPH = rb.velocity.magnitude * 3.6f;

        if (driveType == DriveType.AllWheelDrive)
        {
            for (int i = 0; i < wheelCollider.Length; i++)
            {
                wheelCollider[i].motorTorque = totalPower / 4;
            }
        }
        else if (driveType == DriveType.RearWheelDrive)
        {
            for (int i = 2; i < wheelCollider.Length; i++)
            {
                wheelCollider[i].motorTorque = totalPower / 2;
            }
        }
        else 
        {
            for (int i = 0; i < wheelCollider.Length - 2; i++)
            {
                wheelCollider[i].motorTorque = totalPower / 2;
            }
        }

        // �극��ũ
        if (inputManager.brake || inputManager.ovrBrake)
        {
            for (int i = 0; i < wheelCollider.Length; i++)
            {
                wheelCollider[i].brakeTorque = brakePower;
            }
        }
        else
        {
            for (int i = 0; i < wheelCollider.Length; i++)
            {
                wheelCollider[i].brakeTorque = 0;
            }
        }

        // �ν�Ʈ
        if (inputManager.boost)
        {
            rb.AddRelativeForce(Vector3.forward * boostPower);
        }
        
    }

    void SteerVehicle()
    {
        // ��Ŀ�� ��Ƽ� ����
        // SteeringAngel = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius + (1.5f / 2))) * horizontalInput;
        // Rad2Deg ���� ���� ������ ��ȯ���� ex) radian �� * Mathf.Rad2Deg = ����

        if (inputManager.steer > 0 || inputManager.ovrSteer > 0) 
        {
            wheelCollider[0].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius + (1.5f / 2))) * inputManager.steer;
            wheelCollider[1].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius - (1.5f / 2))) * inputManager.steer;

            if (OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.RTouch) != 0)
            {
                wheelCollider[0].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius + (1.5f / 2))) * inputManager.ovrSteer;
                wheelCollider[1].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius - (1.5f / 2))) * inputManager.ovrSteer;
            }

            AnimateSteerWheel(wheelCollider[0].steerAngle);
        }
        else if (inputManager.steer < 0 || inputManager.ovrSteer < 0) // ���� ȸ��
        {
            wheelCollider[0].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius - (1.5f / 2))) * inputManager.steer;
            wheelCollider[1].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius + (1.5f / 2))) * inputManager.steer;

            if (OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.RTouch) != 0)
            {
                wheelCollider[0].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius - (1.5f / 2))) * inputManager.ovrSteer;
                wheelCollider[1].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius + (1.5f / 2))) * inputManager.ovrSteer;
            }

            AnimateSteerWheel(wheelCollider[1].steerAngle);
        }
        else
        {
            wheelCollider[0].steerAngle = 0;
            wheelCollider[1].steerAngle = 0;
            AnimateSteerWheel(wheelCollider[0].steerAngle);
        }

        //for (int i = 0; i < wheelCollider.Length - 2; i++)
        //{
        //    wheelCollider[i].steerAngle = inputManager.steer * steerMaxRot;
        //}
    }

    void AnimateSteerWheel(float wheelRotation)
    { 
        steerWheel.transform.localEulerAngles = new Vector3(0, 0, -wheelRotation * 2f);
    }

    void animateWheels()
    {
        // �� �ݶ��̴��� ��� ������ �־��ٰ���. 
        Vector3 wheelPosition = Vector3.zero;
        Quaternion wheelRotation = Quaternion.identity;

        for (int i = 0; i < wheelMesh.Length; i++)
        {
            // GetWorldPose : ������, ������� �Ѱ�, ��Ƽ� ���� World�� ��������
            wheelCollider[i].GetWorldPose(out wheelPosition, out wheelRotation);
            // ��� ������ �� �Ž��� �־��ش�.
            wheelMesh[i].transform.position = wheelPosition;
            wheelMesh[i].transform.rotation = wheelRotation;
        }
    }

    void AddDownForce()
    {
        rb.AddForce(-transform.up * downForce * rb.velocity.magnitude);
    }
    
    void GetFriction()
    {
        for (int i = 0; i < wheelCollider.Length; i++)
        {
            WheelHit wheelHit;
            wheelCollider[i].GetGroundHit(out wheelHit);
            slips[i] = wheelHit.forwardSlip;
        }
    }

    
}