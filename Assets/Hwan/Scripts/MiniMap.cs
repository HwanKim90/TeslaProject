using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMap : MonoBehaviour
{
    LineRenderer lr;
    GameObject TrackPath;
    
    void Start()
    {
        lr = GetComponent<LineRenderer>();
        TrackPath = gameObject;

        int numOfPath = TrackPath.transform.childCount;
        lr.positionCount = numOfPath + 1;

        for (int x = 0; x < numOfPath; x++)
        {
            lr.SetPosition(x, new Vector3(TrackPath.transform.GetChild(x).transform.position.x,
                    20, TrackPath.transform.GetChild(x).transform.position.z));
        }

        lr.SetPosition(numOfPath, lr.GetPosition(0));
        lr.startWidth = 20;
        lr.endWidth = 20;
    }

    
    void Update()
    {
        
    }
}
