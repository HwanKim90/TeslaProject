using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
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
    float[] slips = new float[4];

    public float totalPower;
    public AnimationCurve enginePower;
    public float KPH;
    public float wheelsRPM;
    public float engineRPM;
    public int gearNum;

    //public float torque = 200f;
    public float radius = 6f; // 애커먼 장토식 회전각도
    public float downForce = 50f;
    public float brakePower = 10000f;
    public float boostPower = 1000f;
    //public float steerMaxRot = 10f;
    float smoothTime = 0.01f;

    InputManager inputManager;
    Rigidbody rigidbody;
   
    void Start()
    {
        inputManager = GetComponent<InputManager>();
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.centerOfMass = centerOfMess.transform.localPosition;
    }

    private void FixedUpdate()
    {
        animateWheels();
        //MoveVehicle();
        SteerVehicle();
        AddDownForce();
        //GetFriction();
        GearShifter();
        CalculateEnginePower();
    }

    void CalculateEnginePower()
    {
        WheelRPM();

        totalPower = enginePower.Evaluate(engineRPM) * gears[gearNum] * inputManager.accel;
        float velocity = 0.0f;
        engineRPM = Mathf.SmoothDamp(engineRPM, Mathf.Abs(wheelsRPM) * 3.6f * gears[gearNum], ref velocity, smoothTime);
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
        if (Input.GetKeyDown(KeyCode.Alpha2)) gearNum++;
        if (Input.GetKeyDown(KeyCode.Alpha2)) gearNum--;
    }

    public void MoveVehicle()
    {
        

        // rigidbody 속력을 KPH로 바꿔주는 공식 3.6은 (60 * 60 / 1000)
        KPH = rigidbody.velocity.magnitude * 3.6f;

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

        // 브레이크
        if (inputManager.brake)
        {
            wheelCollider[2].brakeTorque = brakePower;
            wheelCollider[3].brakeTorque = brakePower;
        }
        else
        {
            wheelCollider[2].brakeTorque = 0;
            wheelCollider[3].brakeTorque = 0;
        }

        // 부스트
        if (inputManager.boost)
        {
            rigidbody.AddRelativeForce(Vector3.forward * boostPower);
        }
        
    }

    void SteerVehicle()
    {
        // 애커먼 스티어링 공식
        // SteeringAngel = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius + (1.5f / 2))) * horizontalInput;
        // Rad2Deg 라디안 값을 각도로 변환해줌 ex) radian 값 * Mathf.Rad2Deg = 각도

        if (inputManager.steer > 0) // 오른쪽 회전
        {
            wheelCollider[0].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius + (1.5f / 2))) * inputManager.steer;
            wheelCollider[1].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius - (1.5f / 2))) * inputManager.steer;
        }
        else if (inputManager.steer < 0) // 왼쪽 회전
        {
            wheelCollider[0].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius - (1.5f / 2))) * inputManager.steer;
            wheelCollider[1].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius + (1.5f / 2))) * inputManager.steer;
        }
        else
        {
            wheelCollider[0].steerAngle = 0;
            wheelCollider[1].steerAngle = 0;
        }

        //for (int i = 0; i < wheelCollider.Length - 2; i++)
        //{
        //    wheelCollider[i].steerAngle = inputManager.steer * steerMaxRot;
        //}
    }
    
    void animateWheels()
    {
        // 휠 콜라이더에 담긴 정보를 넣어줄거임. 
        Vector3 wheelPosition = Vector3.zero;
        Quaternion wheelRotation = Quaternion.identity;

        for (int i = 0; i < wheelMesh.Length; i++)
        {
            // GetWorldPose : 땅접촉, 서스펜션 한계, 스티어링 각도 World로 가지고옴
            wheelCollider[i].GetWorldPose(out wheelPosition, out wheelRotation);
            // 담긴 정보를 휠 매쉬에 넣어준다.
            wheelMesh[i].transform.position = wheelPosition;
            wheelMesh[i].transform.rotation = wheelRotation;
        }
    }

    void AddDownForce()
    {
        rigidbody.AddForce(-transform.up * downForce * rigidbody.velocity.magnitude);
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
