using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointRotationLeap : MonoBehaviour {
    public GameObject translation;
    private float x = 0.0f;
    private float y = 0.0f;
    private float z = 0.0f;
    // Use this for initialization
    void Start () {
        transform.position = new Vector3(x, y, z);
        //GetComponent<Renderer>().material.SetColor("_Color", Color.green);
    }
	
	// Update is called once per frame
	void FixedUpdate() {
        if (Input.GetKey("y"))
        {
            y++;
        }
        if (Input.GetKey("h"))
        {
            y--;
        }
        if (Input.GetKey("g"))
        {
            z--;
        }
        if (Input.GetKey("j"))
        {
            z++;
        }
        if (Input.GetKey("t"))
        {
            x--;
        }
        if (Input.GetKey("u"))
        {
            x++;
        }
        //transform.position = new Vector3(x, y, z + 0.0f);
    }
    public void setVec3(Vector3 vec) {
        x = vec.x;
        y = vec.y;
        z = vec.z;
    }
}
