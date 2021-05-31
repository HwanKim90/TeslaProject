using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    public GameObject followObj;
    CarController carController;

    public float followSpeed = 2f;
    public float defaultFOV;
    public float boostFOV = 120;

    private void Start()
    {
        carController = player.GetComponent<CarController>();
        defaultFOV = Camera.main.fieldOfView;
    }

    private void FixedUpdate()
    {
        FollowPlayer();
        BoostFOV();
        //followSpeed = carController.KPH >= 50 ? 20 : carController.KPH / 4;
    }

    void FollowPlayer()
    {
        followSpeed = Mathf.Lerp(followSpeed, carController.KPH / 5, Time.deltaTime);

        transform.position = Vector3.Lerp(transform.position, followObj.transform.position, Time.deltaTime * followSpeed);
        transform.LookAt(player.transform.position);
    }

    void BoostFOV()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, boostFOV, Time.deltaTime * 2);
        }
        else
        {
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, defaultFOV, Time.deltaTime * 2);
        }
    }
}
