using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour {
    public bool isClosed = false;
    public bool isGrabbing = false;
    public Material sphereMaterialRef;
    public Material translateMaterialRef;
    public Material rotateMaterialRef;
    public Material canSelectMaterialRef;
    public Material selectMaterialRef;


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public bool Closed {
        get { return isClosed; }
        set { isClosed = value; }
    }

    public bool Grab {
        get { return isGrabbing; }
        set { isGrabbing = value; }
    }
}
