using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointTranslation : MonoBehaviour {
    private float x=0.0f;
    private float y = 0.0f;
    private float z = 0.0f;
    // Use this for initialization
    void Start () {
        transform.position = new Vector3(2.0f, 0.0f, 0.0f);
        GetComponent<Renderer>().material.SetColor("_Color", Color.yellow);
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if (Input.GetKey("z"))
        {
            y++;
        }
        if (Input.GetKey("s"))
        {
            y--;
        }
        if (Input.GetKey("q"))
        {
            z--;
        }
        if (Input.GetKey("d"))
        {
            z++;
        }
        if (Input.GetKey("a"))
        {
            x--;
        }
        if (Input.GetKey("e"))
        {
            x++;
        }
        transform.position = new Vector3(x+2.0f,y+ 0.0f, z+0.0f);
    }

    public float GetX()
    {
        return x;
    }
    public float Gety()
    {
        return y;
    }
    public float GetZ()
    {
        return z;
    }
}
