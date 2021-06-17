using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager instance;
    public GameObject startSignal;
    public AudioSource startSound;

    public Transform[] startPos;
    public bool[] isEmpty;
    public bool isStart;

    int emptyIndex;
    int playerCount;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        isEmpty = new bool[startPos.Length];
        
    }

    private void Update()
    {  
        playerCount = PhotonNetwork.PlayerList.Length;

        if (playerCount == 4
            || Input.GetKeyDown(KeyCode.Alpha3)
            || OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.LTouch))
        {
            startSignal.SetActive(true);
            StartCoroutine(delyOneSec());
        }

        if (startSignal.GetComponent<StartSignal>().greenNum >= 5)
        {
            isStart = true;
        }
    }

    IEnumerator delyOneSec()
    {
        yield return new WaitForSeconds(1.2f);
        startSound.Play();
    }

    public Vector3 GetEmptyStartPos()
    {
        //for(int i = 0; i < startPos.Length; i++)
        //{
        //    if(isEmpty[i] == false)
        //    {
        //        isEmpty[i] = true;
        //        return startPos[i].position;
        //    }
        //}

        int n = emptyIndex;
        isEmpty[n] = true;
        emptyIndex++;
        return startPos[n].position;
    }

}
