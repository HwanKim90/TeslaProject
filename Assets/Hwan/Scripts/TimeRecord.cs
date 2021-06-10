using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeRecord : MonoBehaviour
{
    public float timer;
    public float[] lapTime;
    public float bestTime;
    public int lap = 0;
    bool timerStart;
    bool firstChecker;

    private void Start()
    {
        lapTime = new float[2];
    }

    void Update()
    {
        TimeRecordSetting();
    }

    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("FirstChecker"))
        {
            firstChecker = true;
        }

        if (other.CompareTag("FinalLine"))
        {
            timerStart = true;
            if (lap == 0)
            {
                lap++;
            }

            if (lap == 1 && firstChecker)
            {
                lapTime[0] = timer;
                timer = 0.0f;
                lap++;
            }

            if (lap == 2)
            {
                lapTime[1] = timer;
            }
        }
    }

    void TimeRecordSetting()
    {
        if (timerStart)
        {
            timer += Time.deltaTime;
            float minutes = Mathf.FloorToInt(timer / 60);
            float second = Mathf.FloorToInt(timer - minutes * 60);

            string time = string.Format("{0:0}:{1:00}", minutes, second);
            print(time);
        }
        
        
    }
}
