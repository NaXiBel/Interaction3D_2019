using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointRotation : MonoBehaviour {
    public GameObject translation;
    private float x = 0.0f;
    private float y = 0.0f;
    private float z = 0.0f;
    // Use this for initialization
    void Start () {
        transform.position = new Vector3(0.0f, 4.0f, 0.0f);
        GetComponent<Renderer>().material = Resources.Load("Rotation", typeof(Material)) as Material;
    }

}
