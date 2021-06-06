using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineSound : MonoBehaviour
{
    public CarController cc;
    public AudioSource engineSound;
    InputManager inputManager;

    float maxSpeed = 120f;

    void Start()
    {
        engineSound.pitch = 0.5f;
        inputManager = GetComponent<InputManager>();
    }

    
    void Update()
    {
        SetEngineSound();
    }

    void SetEngineSound()
    {
        
        if (cc.KPH > 0)
        {
            engineSound.volume = Mathf.Lerp(0.5f, 1f, cc.KPH / maxSpeed);
            engineSound.pitch = Mathf.Lerp(0.5f, 1.7f, cc.KPH / maxSpeed);
        }

    }
}
