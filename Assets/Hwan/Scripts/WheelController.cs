using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class WheelController : MonoBehaviourPun
{
    // 오른손
    public GameObject rightHand;
    public Transform rightHandOriginParent;
    public bool rightHandOnWheel = false;

    // 왼손
    public GameObject leftHand;
    public Transform leftHandOriginParent;
    public bool leftHandOnWheel = false;

    public Transform[] snapPositions;
    public PhotonView pv;
    //public float currWheelRotation = 0;

    void Update()
    {
        if (pv.IsMine)
        {
            ConvertHandToWheel();
            ReleaseHandOnWheel();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        
        if (other.CompareTag("PlayerHand"))
        {   
            if (rightHandOnWheel == false && OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.RTouch))
            {   
                PlaceHandOnWheel(ref rightHand, ref rightHandOriginParent, ref rightHandOnWheel);
            }
        }

        if (other.CompareTag("PlayerHand"))
        {
            if (leftHandOnWheel == false && OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.LTouch))
            {
                PlaceHandOnWheel(ref leftHand, ref leftHandOriginParent, ref leftHandOnWheel);
            }
        }
    }

    void PlaceHandOnWheel(ref GameObject hand, ref Transform originalParent, ref bool handOnWheel)
    {
        // 일단 최초값 넣고
        var shortDist = Vector3.Distance(snapPositions[0].position, hand.transform.position);
        var bestSnap = snapPositions[0];

        foreach (var snapPos in snapPositions)
        {   
            if (snapPos.childCount == 0)
            {
                var distance = Vector3.Distance(snapPos.position, hand.transform.position);

                if (distance < shortDist)
                {
                    shortDist = distance;
                    bestSnap = snapPos;
                }
            }
        }

        originalParent = hand.transform.parent;
        hand.transform.SetParent(bestSnap);
        hand.transform.position = bestSnap.transform.position;

        handOnWheel = true;

    }

    void ConvertHandToWheel()
    {
        if (rightHandOnWheel == true && leftHandOnWheel == false)
        {
            Quaternion newRot = Quaternion.Euler(0, 0, rightHandOriginParent.transform.eulerAngles.z);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, newRot, 0.3f);
            //transform.parent = transform;
            
        }
        else if(rightHandOnWheel == false && leftHandOnWheel == true)
        {
            Quaternion newRot = Quaternion.Euler(0, 0, leftHandOriginParent.transform.eulerAngles.z);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, newRot, 0.3f);
            //transform.parent = transform;
        }
        else if (rightHandOnWheel == true && leftHandOnWheel == true)
        {
            Quaternion newRotRight = Quaternion.Euler(0, 0, rightHandOriginParent.transform.eulerAngles.z);
            Quaternion newRotLeft = Quaternion.Euler(0, 0, leftHandOriginParent.transform.eulerAngles.z);
            Quaternion finalRot = Quaternion.Slerp(newRotRight, newRotLeft, 0.3f);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, finalRot, 0.3f);
            //transform.parent = transform;
        }
    }

    void ReleaseHandOnWheel()
    {
        if (rightHandOnWheel == true && OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.RTouch))
        {
            rightHand.transform.SetParent(rightHandOriginParent);
            rightHand.transform.position = rightHandOriginParent.position;
            rightHand.transform.rotation = rightHandOriginParent.rotation;
            rightHandOnWheel = false;
        }

        if (leftHandOnWheel == false && OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.LTouch))
        {
            leftHand.transform.SetParent(leftHandOriginParent);
            leftHand.transform.position = leftHandOriginParent.position;
            leftHand.transform.rotation = leftHandOriginParent.rotation;
            leftHandOnWheel = false;
        }
    }
}
