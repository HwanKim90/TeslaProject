using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeRecord : MonoBehaviour
{
    public float timer;
    public float[] lapTime;
    public float bestTime;
    public Text visualTimer;
    public Text Lap1Time;
    //public Text Lap2Time;
    public int lap = 0;

    
    bool timerStart;
    public bool firstChecker;
    public bool secondChecker;
    public bool finalPass;

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
        if (other.CompareTag("FirstChecker")) firstChecker = true;
       
        if (other.CompareTag("SecondChecker")) secondChecker = true;

        if (other.CompareTag("FinalLine"))
        {
            timerStart = true;

            if (lap == 0) lap++;
            
            if (lap == 1 && firstChecker) //&& secondChecker)
            {
                lapTime[0] = timer;
                bestTime = lapTime[0];
                timer = 0.0f;
                lap++;
                float minutes = Mathf.FloorToInt(lapTime[0] / 60);
                float second = Mathf.FloorToInt(lapTime[0] - minutes * 60);

                string time = string.Format("Lap1 : {0:0}:{1:00}", minutes, second);
                Lap1Time.text = time;
                
                finalPass = true;

                print("bestTime : " + bestTime);
            }

            //if (lap == 2)
            //{
            //    lapTime[1] = timer;

            //    float minutes = Mathf.FloorToInt(lapTime[1] / 60);
            //    float second = Mathf.FloorToInt(lapTime[1] - minutes * 60);

            //    string time = string.Format("Lap2 : {0:0}:{1:00}", minutes, second);
            //    Lap2Time.text = time;
            //    if (bestTime > timer)
            //    {
            //        //bestTime = lapTime[1];
            //        print("bestTime : " + bestTime);
            //    }
            //}
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
            visualTimer.text = time;
        }
    }

    
}
