using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CarController : MonoBehaviourPun, IPunObservable
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
    //public float[] gears = new float[5];
    public GameObject centerOfMess;
    public GameObject steerWheel;
    float[] slips = new float[4];

    public float totalPower;
    public AnimationCurve enginePower;
    public float KPH;
    public float wheelsRPM;
    public float engineRPM;
    public int gearPower = 3;

    //public float torque = 200f;
    public float radius = 6f; // 애커먼 장토식 회전각도
    public float downForce = 50f;
    public float brakePower = 10000f;
    public float boostPower = 1000f;
    public int gearNum = 0;
    //public float steerMaxRot = 10f;
    float smoothTime = 0.01f;


    InputManager inputManager;
    Rigidbody rb;
    public WheelController wheelControl;

    // 포톤관련
    public GameObject cameraRig;
    
    Vector3 otherCarPos;
    Quaternion otherCarRot;
    

    float otherTimer;

    public GameObject childrenParent;
    

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(GetComponent<TimeRecord>().bestTime);
        }

        if (stream.IsReading)
        {
            otherCarPos = (Vector3)stream.ReceiveNext();
            otherCarRot = (Quaternion)stream.ReceiveNext();
            otherTimer = (float)stream.ReceiveNext();
        }
    }

    void Start()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            Vector3 pos = GameManager.instance.GetEmptyStartPos();
            photonView.RPC(nameof(SetInit), RpcTarget.AllBuffered, pos);
        }

        inputManager = GetComponent<InputManager>();
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = centerOfMess.transform.localPosition;

        cameraRig.SetActive(photonView.IsMine);
    }

    private void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            animateWheels();
            SteerVehicle();
            AddDownForce();
            CalculateEnginePower();
        }
        else
        { 
            transform.position = Vector3.Lerp(transform.position, otherCarPos, 0.2f);
            transform.rotation = Quaternion.Lerp(transform.rotation, otherCarRot, 0.2f);
        }
    }

    void CalculateEnginePower()
    {
        WheelRPM();

        if (wheelControl.leftHandOnWheel || wheelControl.rightHandOnWheel)
        {
            // vr
            totalPower = enginePower.Evaluate(engineRPM) * gearPower * inputManager.ovrAccel;
        }
        else
        {
            //AutoTransMission();
            // 키보드
            totalPower = enginePower.Evaluate(engineRPM) * gearPower * inputManager.accel;
        }

        float velocity = 0.0f;
        engineRPM = Mathf.SmoothDamp(engineRPM, Mathf.Abs(wheelsRPM) * 0.03f * gearPower, ref velocity, smoothTime);

        if (GameManager.instance.isStart == false)
            return;

        MoveVehicle();
    }

    void WheelRPM()
    {
        float sum = 0f;
        for (int i = 0; i < wheelCollider.Length; i++)
        {
            sum += wheelCollider[i].rpm;
        }
        wheelsRPM = sum / 4;
        
    }

    public void MoveVehicle()
    {
        // rigidbody 속력을 KPH로 바꿔주는 공식 3.6은 (60 * 60 / 1000)
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
            print("전륜");
            wheelCollider[0].motorTorque = totalPower * 4;
            wheelCollider[1].motorTorque = totalPower * 4;

        }

        // 브레이크
        if (inputManager.brake || inputManager.ovrBrake)
        {
            for (int i = 2; i < wheelCollider.Length; i++)
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

        // 부스트
        if (inputManager.boost || inputManager.ovrBoost)
        {
            rb.AddRelativeForce(Vector3.forward * boostPower);
        }
        
    }


    public float angle;
    void SteerVehicle()
    {
        // 애커먼 스티어링 공식
        // SteeringAngel = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius + (1.5f / 2))) * horizontalInput;
        // Rad2Deg 라디안 값을 각도로 변환해줌 ex) radian 값 * Mathf.Rad2Deg = 각도
        //if(Input.GetKeyDown(KeyCode.Alpha0))
        //{
        //    angle++;
        //}
        //inputManager.ovrSteer = angle * Mathf.Deg2Rad;

        //wheelCollider[0].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius + (1.5f / 2))) * inputManager.ovrSteer;
        //wheelCollider[1].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius - (1.5f / 2))) * inputManager.ovrSteer;

        //return;


        if (inputManager.steer > 0 || inputManager.ovrSteer > 0) 
        {
            // vr input
            if (wheelControl.leftHandOnWheel || wheelControl.rightHandOnWheel)
            {
                wheelCollider[0].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius + (1.5f / 2))) * inputManager.ovrSteer;
                wheelCollider[1].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius - (1.5f / 2))) * inputManager.ovrSteer;
                //AnimateSteerWheel(wheelCollider[0].steerAngle);
            }
            else
            {
                wheelCollider[0].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius + (1.5f / 2))) * inputManager.steer;
                wheelCollider[1].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius - (1.5f / 2))) * inputManager.steer;
                AnimateSteerWheel(wheelCollider[0].steerAngle);
            }

            
        }
        else if (inputManager.steer < 0 || inputManager.ovrSteer < 0) // 왼쪽 회전
        {
            

            // vr input
            if (wheelControl.leftHandOnWheel || wheelControl.rightHandOnWheel)
            {
                wheelCollider[0].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius - (1.5f / 2))) * inputManager.ovrSteer;
                wheelCollider[1].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius + (1.5f / 2))) * inputManager.ovrSteer;
                //AnimateSteerWheel(wheelCollider[1].steerAngle);
            }
            else
            {
                wheelCollider[0].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius - (1.5f / 2))) * inputManager.steer;
                wheelCollider[1].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius + (1.5f / 2))) * inputManager.steer;
                AnimateSteerWheel(wheelCollider[1].steerAngle);
            }

        }
        else
        {
            if (!wheelControl.leftHandOnWheel && !wheelControl.rightHandOnWheel)
            {
                wheelCollider[0].steerAngle = 0;
                wheelCollider[1].steerAngle = 0;
                AnimateSteerWheel(wheelCollider[0].steerAngle);
            }
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

    [PunRPC]
    void SetInit(Vector3 pos)
    {
        transform.position = pos;
        childrenParent.SetActive(true);
    }
}
