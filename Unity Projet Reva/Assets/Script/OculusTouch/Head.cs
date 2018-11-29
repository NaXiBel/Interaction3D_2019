using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Head : MonoBehaviour
{

    private GameObject positions;

    // Use this for initialization
    void Awake()
    {

        positions = GameObject.Find("Viewer");

    }

    // Update is called once per frame
    void Update()
    {

        OculusTouchController t = positions.GetComponent<OculusTouchController>();
        transform.localPosition = t.GetHead();
        transform.localRotation = t.GetRotationHead();
        transform.localRotation = Quaternion.Euler(-transform.localRotation.eulerAngles.x,-transform.localRotation.eulerAngles.y,transform.localRotation.eulerAngles.z);

    }

}

