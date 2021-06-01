using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public AudioSource bgmAudio;
    public AudioSource eftAudio;
    public AudioClip[] bgms;
    public AudioClip[] efts;

    public enum BGM_TYPE
    {
        racing,
        ending
    }

    public enum EFT_TYPE
    {
        idle,
        accel,
        brake
    }

    
    private void Awake()
    {
        instance = this;
    }

    public void PlayBGM(BGM_TYPE type)
    {
        bgmAudio.clip = bgms[(int)type];
        bgmAudio.Play();
    }

    public void PlayEFT(EFT_TYPE type)
    {
        bgmAudio.PlayOneShot(efts[(int)type]);
    }
}
