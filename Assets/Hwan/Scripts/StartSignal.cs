using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class StartSignal : MonoBehaviourPun
{
    public GameObject[] signals;
    public Material green;
    public int greenNum = 0;
   

    private void Start()
    {
        StartCoroutine(ChangeGreenSec());
    }

   
    IEnumerator ChangeGreenSec()
    {
        for (int i = 0; i < signals.Length; i++)
        {
            yield return new WaitForSeconds(1);
            signals[i].GetComponent<MeshRenderer>().material = green;
            greenNum++;
            //print(greenNum);
        }
    }

}
