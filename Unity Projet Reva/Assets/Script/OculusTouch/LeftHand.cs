using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftHand : HandOculus
{

    private GameObject positions;
    private Vector3 position;
    private bool triggered;
    private bool changed;
    private bool grabbed;
    private GameObject closedOne;
    private GameObject openOne;

    // Use this for initialization
    void Awake () {

        datas = GameObject.Find("Viewer").GetComponent<OculusTouchController>();
        Debug.Log(datas);
        changed = false;
        triggered = false;
        grabbed = false;
        positions = GameObject.Find("Viewer");
        closedOne = GameObject.Find("closedLeft");
        openOne = GameObject.Find("openLeft");

    }
	
	// Update is called once per frame
	void Update () {

        Vector3 pos = positions.GetComponent<OculusTouchController>().GetLeftHand();
        transform.localPosition = pos;
        position = pos;

        transform.localScale = new Vector3(5f, 5f, 5f) / datas.GetScale();

        transform.localRotation = positions.GetComponent<OculusTouchController>().GetLeftQuaternions();
        transform.localRotation = Quaternion.Euler(-transform.localRotation.eulerAngles.x, -transform.localRotation.eulerAngles.y, transform.localRotation.eulerAngles.z);
        //transform.Rotate(new Vector3(-90, 180, 0));
        transform.localRotation *= Quaternion.Euler(new Vector3(-90, 180, 0));

        changed = triggered;
        triggered = positions.GetComponent<OculusTouchController>().IsLeftHandTriggered();

        if (triggered)
        {
            closedOne.SetActive(true);
            openOne.SetActive(false);
        }
        else
        {
            closedOne.SetActive(false);
            openOne.SetActive(true);
        }

        changed = triggered != changed;

        grabbed = (changed && triggered && somethingTouched) || (grabbed && triggered);

        TryToChangePosition();

	}

    protected override bool Grabbed()
    {
        return grabbed;
    }

    protected override Vector3 HandPosition()
    {
        return transform.position;
    }

    protected override Quaternion getRotation()
    {
        return transform.rotation;
    }

}
