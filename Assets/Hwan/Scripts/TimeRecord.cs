using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeRecord : MonoBehaviour
{
    public float time;
    public float bestTime;
    bool isPassLine = false;
    int rapCount;

    void Update()
    {
        TimeRecordSetting();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("FinalLine"))
        {
            print("Ãæµ¹");
            isPassLine = true;
            rapCount++;
            print(rapCount);

            if (rapCount > 1)
            {
                PlayerPrefs.SetFloat("BestTime", time);
                bestTime = PlayerPrefs.GetFloat("BestTime");
            }

        }
    }

    void TimeRecordSetting()
    {
        if (isPassLine)
        {
            time += Time.deltaTime;
            //print(time);
            
        }
    }
}
