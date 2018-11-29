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
        GetComponent<Renderer>().material = Resources.Load("Translation", typeof(Material)) as Material;
    }
	
}
