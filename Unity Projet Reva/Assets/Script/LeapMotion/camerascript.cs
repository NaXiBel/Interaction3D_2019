using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camerascript : MonoBehaviour {

    public GameObject LeftHand;
    public GameObject lPalm;
    public GameObject rightHand;
    public GameObject rPalm;
    public GameObject objectToMove;
    public GameObject sphereT;
    public GameObject sphereR;
    public GameObject controller;
    public float velocity = 1.0f;
    public float threshold = 1.0f;

    private Hand lH;
    private Hand rH;
    private PointTranslationLeap pT;
    private PointRotationLeap pR;
    private Vector3 offset;
    private Vector3 offsetR;
    private float newPositionZ;
    private float newPositionX;
    private Vector3 newPosition;
    private Vector3 newPositionR;
    private bool holdL = false;
    private bool holdR = false;
    private bool moveL = false;
    private bool moveR = false;
    private bool holdRotate = false;
    private float offsetZ;
    private float offsetX;
    private Quaternion rotation = Quaternion.identity;

    void Start() {
        lH = LeftHand.GetComponent<Hand>();
        rH = rightHand.GetComponent<Hand>();
        pT = sphereT.GetComponent<PointTranslationLeap>();
        pR = sphereR.GetComponent<PointRotationLeap>();
        offsetZ = 0f;
        offsetX = 0f;
        newPositionZ = 0f;
        newPositionX = 0f;
        InvokeRepeating("Camera", 0f, 0.01f);  //1s delay, repeat every 1s

    }
    public GameObject toMove {
        get { return objectToMove; }
        set { objectToMove = value; }
    }
    void Camera() {
        //Debug.Log("Hello");
        /*if (lH.Closed && !lH.Grab && rH.Closed && !rH.Grab) {
            /*rotation
            if (!holdRotate) {
                offset = lPalm.transform.localPosition;
                offsetR = rPalm.transform.localPosition;
                holdRotate = true;
            } else {
                newPosition = lPalm.transform.localPosition;
                newPositionR = rPalm.transform.localPosition;
                rotation = Quaternion.identity;
                float deltatime = Time.deltaTime;
                Vector3 rotationOffset = new Vector3();
                Debug.Log("DeltaTime : " + deltatime);
                if (offset.z-newPosition.z > threshold) {
                    rotationOffset = new Vector3(0f, 10f, 0f) * deltatime;
                    Debug.Log("rot + : " + rotationOffset);
                } else if (offset.z - newPosition.z < threshold) {
                    rotationOffset = new Vector3(0f, -10f, 0f) * deltatime;
                    Debug.Log("rot - : " + rotationOffset);
                }
                transform.Rotate(rotationOffset);
                offset = lPalm.transform.localPosition;
                offsetR = rPalm.transform.localPosition;
            
            }*/
        if (lH.Closed) {
            if (lH.Grab) {
                objectToMove.GetComponent<Renderer>().material = lH.selectMaterialRef;
                newPosition = LeftHand.transform.Find("Palm").position;
                if (objectToMove.name == "Sphere") {
                    objectToMove.transform.position = newPosition;  
                } else if (objectToMove == sphereR) {
                    //pR.setVec3(newPosition);
                    sphereR.transform.Rotate(Vector3.up);
                } else pT.setVec3(newPosition);
                moveL = false;
            } else if(!moveR){
                //Mouvement
                moveL = true;
                if (!holdL) {
                    offset = transform.position - lPalm.transform.position;
                    offsetZ = transform.position.z - lPalm.transform.position.z;
                    offsetX = transform.position.x - lPalm.transform.position.x;
                    holdL = !holdL;
                } else {
                    newPositionZ = transform.position.z - lPalm.transform.position.z;
                    newPositionX = transform.position.x - lPalm.transform.position.x;
                    newPosition = transform.position - lPalm.transform.position;
                    this.transform.position = transform.position - (offset - newPosition) * velocity; //new Vector3(transform.position.x - (offsetX - newPositionX), transform.position.y, transform.position.z - (offsetZ - newPositionZ));
                    offsetZ = transform.position.z - lPalm.transform.position.z;
                    offsetX = transform.position.x - lPalm.transform.position.x;
                    offset = transform.position - lPalm.transform.position;
                }
            }
        } else {
            holdL = false;
            moveL = false;
        }
        if (rH.Closed) {
            if (rH.Grab) {
                objectToMove.GetComponent<Renderer>().material = rH.selectMaterialRef;
                newPositionR = rightHand.transform.Find("Palm").position;
                if (objectToMove == sphereT) {
                    pT.setVec3(newPositionR);  
                } else if (objectToMove == sphereR) {
                    //pR.setVec3(newPositionR);
                    sphereR.transform.Rotate(Vector3.up);
                } else if (objectToMove.name == "Sphere") {
                    objectToMove.transform.position = newPositionR;
                }
                moveR = false;
            } else if(!moveL){
                moveR = true;
                if (!holdR) {
                    offsetR = transform.position - rPalm.transform.position;
                    offsetZ = transform.position.z - rPalm.transform.position.z;
                    offsetX = transform.position.x - rPalm.transform.position.x;
                    holdR = !holdR;
                } else {
                    newPositionZ = transform.position.z - rPalm.transform.position.z;
                    newPositionX = transform.position.x - rPalm.transform.position.x;
                    newPositionR = transform.position - rPalm.transform.position;
                    //this.transform.position = transform.position - (offset ;//new Vector3(transform.position.x - (offsetX - newPositionX), transform.position.y, transform.position.z - (offsetZ - newPositionZ));
                    this.transform.position = transform.position - (offsetR - newPositionR) * velocity;
                    offsetZ = transform.position.z - rPalm.transform.position.z;
                    offsetX = transform.position.x - rPalm.transform.position.x;
                    offsetR = transform.position - rPalm.transform.position;
                }
            }
        } else {
            holdR = false;
            moveR = false;
        }
    }

    // Use this for initialization
    void Update() {
        
    }
}

