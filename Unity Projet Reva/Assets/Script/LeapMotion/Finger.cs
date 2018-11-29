using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Finger : MonoBehaviour {

    public static List<string> fingers =  new List<string>(new string[]{"Thumb","Index", "Middle", "Ring", "Pinky","Pal","PalmPink" });
    public GameObject hand;
    public GameObject cam;
    private Hand h;
    private camerascript cameraSc;
	// Use this for initialization
	void Start () {
        h = hand.GetComponent<Hand>();
        cameraSc = cam.GetComponent<camerascript>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    
    void OnTriggerEnter(Collider col) {
        if (!fingers.Contains(col.gameObject.name.Substring(0, col.gameObject.name.Length-1))) {
            col.GetComponent<Renderer>().material = h.canSelectMaterialRef;
            if (h.Closed && !h.Grab) {
                h.Grab = true;
                cameraSc.objectToMove = col.gameObject;
            }
        }

    }
    void OnTriggerExit(Collider col) {
        //Debug.Log("No longer in contact with " + col.transform.name);
        if (!fingers.Contains(col.gameObject.name.Substring(0, col.gameObject.name.Length - 1))) {
            if (col.transform.name == "SphereRotation") {
                col.GetComponent<Renderer>().material = h.rotateMaterialRef;
            }else if(col.transform.name == "SphereTranslation") {
                col.GetComponent<Renderer>().material = h.translateMaterialRef;
            } else col.GetComponent<Renderer>().material = h.sphereMaterialRef;
            if (!h.Closed && h.Grab) {
                h.Grab = false;
                cameraSc.objectToMove = null;
            }

        }
        
    }
  
}
