using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject speedNeedle;
    public CarController carController;
    float startPosition = 14.57f;
    float endPosition = -242f;
    float currPostion;
    float vehicleSpeed;

    

    void Update()
    {
        vehicleSpeed = carController.KPH;
        UpdateSpeedNeedle();
    }

    public void UpdateSpeedNeedle()
    {
        currPostion = startPosition - endPosition;
        float temp = vehicleSpeed / 350;
        speedNeedle.transform.localEulerAngles = new Vector3(0, 0, startPosition - temp * currPostion);
    }
}
