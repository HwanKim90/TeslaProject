using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineSound : MonoBehaviour
{
    public CarController cc;
    public AudioSource idleSound;
    public AudioSource lowSound;
    public AudioSource madSound;
    public AudioSource hignSound;
    public AudioSource MaxSound;

    float startVolume = 0.5f;
    float maxVolume = 1f;

    float startPitch = 0.5f;
    float maxPitch = 1.7f;

    private void Start()
    {
        idleSound.volume = 0;
        lowSound.volume = 0;
        madSound.volume = 0;
        hignSound.volume = 0;
        MaxSound.volume = 0;
    }

    void Update()
    {
        AccelSound();
    }

    void AccelSound()
    {
        // 0~30, 35~ 60, 65~80, 85~100, 105~130, 135~180;
        if (cc.KPH >= 0 && cc.KPH <= 30)
        {
            idleSound.volume = Mathf.Lerp(startVolume, maxVolume, cc.KPH / 30);
            idleSound.pitch = Mathf.Lerp(startPitch, maxPitch, cc.KPH / 30);

            
            madSound.volume = 0;
            lowSound.volume = 0;
            hignSound.volume = 0;
            MaxSound.volume = 0;

        }

        if (cc.KPH >= 35 && cc.KPH <= 60)
        {
            lowSound.volume = Mathf.Lerp(startVolume, maxVolume, cc.KPH / 80);
            lowSound.pitch = Mathf.Lerp(startPitch, maxPitch, cc.KPH / 80);

            idleSound.volume = 0;
            madSound.volume = 0;

            hignSound.volume = 0;
            MaxSound.volume = 0;
        }

        if (cc.KPH >= 65 && cc.KPH <= 80)
        {
            madSound.volume = Mathf.Lerp(startVolume, maxVolume, cc.KPH / 60);
            madSound.pitch = Mathf.Lerp(startPitch, maxPitch, cc.KPH / 60);

            idleSound.volume = 0;

            lowSound.volume = 0;
            hignSound.volume = 0;
            MaxSound.volume = 0;
        }

        if (cc.KPH >= 85 && cc.KPH <= 100)
        {
            hignSound.volume = Mathf.Lerp(startVolume, maxVolume, cc.KPH / 100);
            hignSound.pitch = Mathf.Lerp(startPitch, maxPitch, cc.KPH / 100);

            idleSound.volume = 0;
            madSound.volume = 0;
            lowSound.volume = 0;
            
            MaxSound.volume = 0;
        }

        if (cc.KPH >= 105 && cc.KPH <= 130)
        {
            hignSound.volume = Mathf.Lerp(startVolume, maxVolume, cc.KPH / 100);
            hignSound.pitch = Mathf.Lerp(maxPitch, 2, cc.KPH / 100);

            idleSound.volume = 0;
            madSound.volume = 0;
            lowSound.volume = 0;
            
            MaxSound.volume = 0;
        }

        if (cc.KPH >= 135 )
        {
            MaxSound.volume = Mathf.Lerp(startVolume, maxVolume, cc.KPH / 180);
            MaxSound.pitch = Mathf.Lerp(startPitch, maxPitch, cc.KPH / 180);

            idleSound.volume = 0;
            madSound.volume = 0;
            lowSound.volume = 0;
            hignSound.volume = 0;
           
        }
    }
}
