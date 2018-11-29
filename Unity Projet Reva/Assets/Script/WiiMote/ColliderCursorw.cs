using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderCursorw : MonoBehaviour {

    private bool CanMove = false;
    private GameObject obj;
    private bool taken = false; 
    public GameObject WiiMote;
    
    private void Update()
    {
        Debug.Log("************************************************************");
        if (WiimoteWiimoteController.wiiIndA)
        {
            GetComponent<Renderer>().material.color = Color.red;
            WiiMote.gameObject.GetComponent<Renderer>().material.color = Color.red;
            
            CanMove = true;
        }
        else
        {
            GetComponent<Renderer>().material.color = Color.white;
            CanMove = false;
            obj = null;
        }
        if (!WiimoteWiimoteController.wiiIndA && !taken)
        {
            WiiMote.gameObject.GetComponent<Renderer>().material.color = Color.white;
        }

        if (obj != null)
        {
            if (obj.gameObject.tag != "Rot")
                obj.gameObject.transform.position = Vector3.Lerp(obj.gameObject.transform.position, this.transform.position, Time.time);
            else
                obj.gameObject.transform.Rotate(Vector3.up);
        }
    }

    
    void OnTriggerExit(Collider other)
    {
        WiiMote.gameObject.GetComponent<Renderer>().material.color = Color.white;
        if (other.tag != "Trans" && other.tag != "Rot")
            other.GetComponent<Renderer>().material = Resources.Load("Control", typeof(Material)) as Material;
        else if (other.tag == "Trans")
            other.GetComponent<Renderer>().material = Resources.Load("Translation", typeof(Material)) as Material;
        else if (other.tag == "Rot")
            other.GetComponent<Renderer>().material = Resources.Load("Rotation", typeof(Material)) as Material;

        // other.gameObject.GetComponent<Renderer>().material.color = Color.white;
        taken = false;
    }
    void OnTriggerStay(Collider other)
    {
        WiiMote.gameObject.GetComponent<Renderer>().material.color = Color.magenta;
        //        other.gameObject.GetComponent<Renderer>().material.color = Color.cyan;
        taken = true;

        if (CanMove)
        {
            other.GetComponent<Renderer>().material = Resources.Load("ControlSelect", typeof(Material)) as Material;
            obj = other.gameObject;
        } else
        {
            other.GetComponent<Renderer>().material = Resources.Load("ControlCanSelect", typeof(Material)) as Material;
        }

    }
}
