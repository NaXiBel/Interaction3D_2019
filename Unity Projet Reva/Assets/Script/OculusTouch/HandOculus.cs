using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HandOculus : MonoBehaviour {

    protected OculusTouchController datas;
    protected GameObject touchedObject;
    protected bool somethingTouched;

    public Vector3 delta;

    public bool SomethingTouched {
        set {
            somethingTouched = value;
        }

        get {
            return somethingTouched;
        }
    }
    private void OnTriggerEnter(Collider other)
    {

        if(touchedObject == null && other.GetComponent<Renderer>() != null)
        {
            touchedObject = other.gameObject;
            touchedObject.GetComponent<Renderer>().material = Resources.Load("ControlCanSelect", typeof(Material)) as Material;
            somethingTouched = true;
        }

    }

    private void OnTriggerExit(Collider other)
    {

        if (other.gameObject != touchedObject)
            return;

        if (touchedObject != null && touchedObject.GetComponent<Renderer>() != null)
        {

            if(touchedObject.tag == "rotation")
                touchedObject.GetComponent<MeshRenderer>().material = Resources.Load("Rotation", typeof(Material)) as Material;
            else if(touchedObject.tag == "translation")
                touchedObject.GetComponent<MeshRenderer>().material = Resources.Load("Translation", typeof(Material)) as Material;
            else
                touchedObject.GetComponent<MeshRenderer>().material = Resources.Load("Control", typeof(Material)) as Material;

            touchedObject = null;
            somethingTouched = false;
        }

    }

    protected abstract Vector3 HandPosition();
    protected abstract bool Grabbed();
    protected abstract Quaternion getRotation();

    protected void TryToChangePosition()
    {
        
        if (somethingTouched && Grabbed())
        {

            touchedObject.GetComponent<Renderer>().material = Resources.Load("ControlSelect", typeof(Material)) as Material;
            if (touchedObject.tag == "rotation")
            {
                touchedObject.transform.rotation = getRotation();
            }
            else
            {
                touchedObject.transform.position = HandPosition() + getRotation() * delta*5;
            }

        } 
        else if (somethingTouched)
        {
            touchedObject.GetComponent<Renderer>().material = Resources.Load("ControlCanSelect", typeof(Material)) as Material;
        }

    }


    private Vector3 ProduitVect(Vector3 v1, Vector3 v2)
    {
        return new Vector3(v1.x * v2.x, v1.y * v2.y, v1.z * v2.z);
    }

}
