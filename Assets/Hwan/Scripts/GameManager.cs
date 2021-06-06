using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Transform[] startPos;
    public bool[] isEmpty;
    int emptyIndex;

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
