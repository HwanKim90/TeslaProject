using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject speedNeedle;
    public GameObject RPMNeedle;
    public CarController carController;

    // ¼Óµµ ¹Ù´Ã
    float startPositionSN = 14.57f;
    float endPositionSN = -242f;
    float currPostionSN;

    // RPM ¹Ù´Ã
    float startPostionRN = 0f;
    float endPositionRN = -233.53f;
    float currPositonRN;

    float vehicleSpeed;

    

    void Update()
    {
        vehicleSpeed = carController.KPH;
        UpdateSpeedNeedle();
        UpdateRPMNeedle();
    }

    void UpdateSpeedNeedle()
    {
        currPostionSN = startPositionSN - endPositionSN;
        float temp = vehicleSpeed / 350;
        speedNeedle.transform.localEulerAngles = new Vector3(0, 0, startPositionSN - temp * currPostionSN);
    }

    void UpdateRPMNeedle()
    {
        currPositonRN = startPostionRN - endPositionRN;
        float temp = carController.engineRPM / 10000;
        RPMNeedle.transform.localEulerAngles = new Vector3(0, 0, startPostionRN - temp * currPositonRN);
    }
}
