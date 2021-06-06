using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelController : MonoBehaviour
{
    // 오른손
    public GameObject rightHand;
    Transform rightHandOriginParent;
    public bool rightHandOnWheel = false;

    // 왼손
    public GameObject leftHand;
    Transform leftHandOriginParent;
    public bool leftHandOnWheel = false;

    public Transform[] snapPositions;
    //public float currWheelRotation = 0;

    void Update()
    {
        ConvertHandToWheel();
        ReleaseHandOnWheel();
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
            Quaternion newRot = Quaternion.Euler(0, 0, rightHandOriginParent.transform.rotation.eulerAngles.z);
            transform.rotation = newRot;
            transform.parent = transform;
        }
        else if(rightHandOnWheel == false && leftHandOnWheel == true)
        {
            Quaternion newRot = Quaternion.Euler(0, 0, leftHandOriginParent.transform.rotation.eulerAngles.z);
            transform.rotation = newRot;
            transform.parent = transform;
        }
        else
        {
            Quaternion newRotRight = Quaternion.Euler(0, 0, rightHandOriginParent.transform.rotation.eulerAngles.z);
            Quaternion newRotLeft = Quaternion.Euler(0, 0, leftHandOriginParent.transform.rotation.eulerAngles.z);
            Quaternion finalRot = Quaternion.Slerp(newRotRight, newRotLeft, 0.5f);
            transform.rotation = finalRot;
            transform.parent = transform;
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
