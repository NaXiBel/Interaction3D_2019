using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderCursor : MonoBehaviour {

    private bool CanMove = false;
    private GameObject obj;
    private bool taken = false;

    public GameObject closedOne;
    public GameObject openOne;
    private void Update()
    {
        if (WiimoteMainController.wiiIndA)
        {
            WiimoteMainController.triggered = true;
          //            GetComponent<Renderer>().material.color = Color.green;
            CanMove = true;
        }
        else
        {
            CanMove = false;
            obj = null;
        }
        if (!WiimoteMainController.wiiIndA && !taken)
        {
            WiimoteMainController.triggered = false;
            closedOne.GetComponent<Renderer>().material.color = Color.white;
            openOne.GetComponent<Renderer>().material.color = Color.white;
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
        closedOne.GetComponent<Renderer>().material.color = Color.white;
        //        openOne.GetComponent<Renderer>().material.color = Color.white;
        if (other.tag != "Trans" && other.tag != "Rot")
            other.GetComponent<Renderer>().material = Resources.Load("Control", typeof(Material)) as Material;
        else if (other.tag == "Trans")
            other.GetComponent<Renderer>().material = Resources.Load("Translation", typeof(Material)) as Material;
        else if (other.tag == "Rot")
            other.GetComponent<Renderer>().material = Resources.Load("Rotation", typeof(Material)) as Material;

        taken = false;
    }
    void OnTriggerStay(Collider other)
    {
        closedOne.GetComponent<Renderer>().material.color = Color.magenta;
        openOne.GetComponent<Renderer>().material.color = Color.magenta;
        taken = true;
        if (CanMove)
        {
            other.GetComponent<Renderer>().material = Resources.Load("ControlSelect", typeof(Material)) as Material;
            obj = other.gameObject;
        }
        else
        {
            other.GetComponent<Renderer>().material = Resources.Load("ControlCanSelect", typeof(Material)) as Material;
        }

    }
}
