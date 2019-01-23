using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using Leap.Unity.Interaction;
using System;

public class LeapMotionManager : MonoBehaviour {

    public static bool m_IsGrabbedControlPoint = false;

    public Hand m_HandRight;
    public Hand m_HandLeft;
    private float m_epsilon = 0.0001f;
    private Vector3 offset;
    private Vector3 offsetR;
    private float newPositionZ;
    private float newPositionX;
    private Vector3 newPosition;
    private Vector3 newPositionR;
    private float velocity = 0.003f;
    
    private bool holdL = false;
    private bool holdR = false;
    private bool moveL = false;
    private bool moveR = false;
    private bool holdRotate = false;
    private float offsetZ;
    private float offsetX;
    public GameObject m_Menu = null;
    public GameObject m_Camera = null;
    private bool m_HandLeftClosed = false;
    private bool m_HandRightClosed = false;
    private Controller m_controller;
    // Use this for initialization
    public GameObject m_RightIndex;
    public GameObject hitObject = null;
    public Vector3 hitObjectLastPos = Vector3.zero;


    private bool m_RayActive = false;
    private GameObject m_HoverredObject;
    private Vector3 m_Start;
    private Vector3 m_Direction;
    public Material m_Mat;
	void Start () {
		m_controller = new Controller();
        offsetZ = 0f;
        offsetX = 0f;
        newPositionZ = 0f;
        newPositionX = 0f;
        m_Camera = GameObject.FindGameObjectWithTag("Camera2");
        m_Menu = GameObject.FindGameObjectWithTag("Menu");
    }

    // Update is called once per frame
    void Update()
    {
        if (m_controller.IsConnected)
        {
            GestureHands();
            MouvementHandler();
            MenuHandler();
        }

        if (m_RayActive)
        {
            LineRenderer lr = this.gameObject.GetComponent<LineRenderer>();

            Vector3 start = m_RightIndex.transform.position;
            Vector3 direction = new Vector3(m_HandRight.Fingers[1].Direction.x, m_HandRight.Fingers[1].Direction.y, -m_HandRight.Fingers[1].Direction.z);
            lr.SetPosition(0, start);
            
            lr.SetPosition(1, start + direction * 100f);
            m_Start = start;
            m_Direction = direction;
            RaycastHit hit;
            if (hitObject != null)
            {
                hitObject.transform.position = m_Start + m_Direction * Vector3.Distance(hitObjectLastPos, m_Start);

            }

            if (Physics.Raycast(start, direction, out hit, 100))
            {
                   // hit.collider.gameObject.GetComponent<InteractionBehaviour>().isHovered = true;
                if (hit.collider.gameObject.tag == "pointable" || hit.collider.gameObject.tag == "pointableSpine")
                {
               //     Debug.Log("HIT" + hit.transform.position + "  " + hit.collider.gameObject.GetType());
                    this.GetComponent<RayInteraction>().OnHoverEnter(hit.collider.transform);
                    m_HoverredObject = hit.collider.gameObject;
                    if (hit.collider.gameObject.tag == "pointableSpline")
                        Debug.Log("Trigger B_Spline");
                }


            } else
            {
                if (m_HoverredObject != null)
                {
                    if (m_HoverredObject.tag == "pointable" || m_HoverredObject.tag == "pointableSpine")
                    {
                        this.GetComponent<RayInteraction>().OnHoverExit(m_HoverredObject.transform);
                        m_HoverredObject = null;
                    }

                }
                hitObject = null;
                hitObjectLastPos = Vector3.zero;

            }
            Debug.DrawRay(start, direction * 100, Color.red);
        }

    }
    void MenuHandler()
    {
        Vector3 HLPositionPalm = new Vector3(m_HandLeft.PalmPosition.x, m_HandLeft.PalmPosition.y, m_HandLeft.PalmPosition.z);
        Vector3 HRPositionPalm = new Vector3(m_HandRight.PalmPosition.x, m_HandRight.PalmPosition.y, m_HandRight.PalmPosition.z);
        
        if (Mathf.Abs(Vector3.Angle(HLPositionPalm, HRPositionPalm)) <= 15f && Mathf.Abs(Vector3.Angle(HLPositionPalm, HRPositionPalm)) >= 10.0f )
        {
            m_Menu.GetComponent<Canvas>().enabled = true;
            m_Menu.transform.position = new Vector3(m_Camera.transform.position.x, m_Camera.transform.position.y, m_Camera.transform.position.z + 0.3f);
        }
       // Debug.Log(Mathf.Abs(Vector3.Angle(HLPositionPalm, HRPositionPalm)));
    }


    public void IndexDetectorActivate()
    {
      //  Debug.Log("Detector Index");
        this.gameObject.AddComponent<LineRenderer>();
        LineRenderer lr = this.gameObject.GetComponent<LineRenderer>();
        lr.useWorldSpace = true;
        lr.startWidth = 0.02f;
        lr.endWidth = 0.02f;
        
        Vector3 start = m_RightIndex.transform.position;
        Vector3 direction = - new Vector3(m_HandRight.Fingers[1].Direction.x, m_HandRight.Fingers[1].Direction.y, m_HandRight.Fingers[1].Direction.z);
        lr.SetPosition(0, start);
        
        lr.SetPosition(1, start + direction * 100f);
        lr.material = m_Mat;


        m_RayActive = true;
     
    }

    public void IndexDetectorDesactivate()
    {
        m_RayActive = false;
        Destroy(GetComponent<LineRenderer>());

        if (m_HoverredObject != null)
        {
            if (m_HoverredObject.tag == "pointable")
            {
                this.GetComponent<RayInteraction>().OnHoverExit(m_HoverredObject.transform);
                m_HoverredObject = null;
            }
            Debug.Log("Trigger on exit");
        }
        hitObject = null;
        hitObjectLastPos = Vector3.zero;
    }


    void MouvementHandler()
    {
        if (m_HandLeftClosed && m_IsGrabbedControlPoint == false)
        {
            if (m_HoverredObject != null)
            {
                this.GetComponent<RayInteraction>().OnSelected(m_HoverredObject.transform);
                if (m_HoverredObject.tag == "pointableSpine")
                {
                    hitObject = GameObject.Find("Controller");
                    hitObjectLastPos = GameObject.Find("Controller").transform.position;
                    hitObject.transform.position = m_Start + m_Direction * Vector3.Distance(hitObjectLastPos, m_Start);

                }
                else
                {
                    hitObject = m_HoverredObject.transform.gameObject;
                    hitObjectLastPos = m_HoverredObject.transform.position;
                    hitObject.transform.position = m_Start + m_Direction * Vector3.Distance(hitObjectLastPos, m_Start);

                }


                Debug.Log("On selected trigger");
            }
            if (!moveR)
            {
                //Mouvement
                moveL = true;
                Vector3 HLPositionPalm = new Vector3(m_HandLeft.PalmPosition.x, m_HandLeft.PalmPosition.y, -m_HandLeft.PalmPosition.z);

                if (!holdL)
                {
                    offset = transform.position - HLPositionPalm;
                    offsetZ = transform.position.z - m_HandLeft.PalmPosition.z;
                    offsetX = transform.position.x - m_HandLeft.PalmPosition.x;
                    holdL = !holdL;
                }
                else
                {

                    newPositionZ = transform.position.z - m_HandLeft.PalmPosition.z;
                    newPositionX = transform.position.x - m_HandLeft.PalmPosition.x;
                    newPosition = transform.position - HLPositionPalm;
                    this.transform.position = transform.position - (offset - newPosition) * velocity; //new Vector3(transform.position.x - (offsetX - newPositionX), transform.position.y, transform.position.z - (offsetZ - newPositionZ));
                    offsetZ = transform.position.z - m_HandLeft.PalmPosition.z;
                    offsetX = transform.position.x - m_HandLeft.PalmPosition.x;
                    offset = transform.position - HLPositionPalm;
                }
            }
        }
        else
        {
            holdL = false;
            moveL = false;
            if (m_HoverredObject != null)
            {
                if (m_HoverredObject.tag == "pointable" || m_HoverredObject.tag == "pointableSpine")
                {
                    this.GetComponent<RayInteraction>().OnHoverExit(m_HoverredObject.transform);
                    m_HoverredObject = null;
                }

            }
            hitObject = null;
            hitObjectLastPos = Vector3.zero;
        }
        if (m_HandRightClosed && m_IsGrabbedControlPoint == false)
        {
            if (!moveL)
            {
                moveR = true;
                Vector3 HRPositionPalm = new Vector3(m_HandRight.PalmPosition.x, m_HandRight.PalmPosition.y, -m_HandRight.PalmPosition.z);

                if (!holdR)
                {
                    offsetR = transform.position - HRPositionPalm;
                    offsetZ = transform.position.z - m_HandRight.PalmPosition.z;
                    offsetX = transform.position.x - m_HandRight.PalmPosition.x;
                    holdR = !holdR;
                }
                else
                {
                    newPositionZ = transform.position.z - m_HandRight.PalmPosition.z;
                    newPositionX = transform.position.x - m_HandRight.PalmPosition.x;
                    newPositionR = transform.position - HRPositionPalm;
                    //this.transform.position = transform.position - (offset ;//new Vector3(transform.position.x - (offsetX - newPositionX), transform.position.y, transform.position.z - (offsetZ - newPositionZ));
                    this.transform.position = transform.position - (offsetR - newPositionR) * velocity;
                    offsetZ = transform.position.z - m_HandRight.PalmPosition.z;
                    offsetX = transform.position.x - m_HandRight.PalmPosition.x;
                    offsetR = transform.position - HRPositionPalm;
                }
            }
        }
        else
        {
            holdR = false;
            moveR = false;
        }
    }

    void GestureHands()
    {
        Frame frame = m_controller.Frame();
        if (frame.Hands.Count > 0)
        {
            List<Hand> hands = frame.Hands;
            for (int h = 0; h < frame.Hands.Count; h++)
            {
                Hand leapHand = frame.Hands[h];
                if (leapHand.IsLeft)
                {
                    m_HandLeft = leapHand;
                    if (m_HandLeft.GrabStrength >= 1-m_epsilon && m_HandLeft.GrabStrength <= 1 + m_epsilon)
                    {
                        m_HandLeftClosed = true;
                        //Debug.Log("HandLeft Closed");
                    } else
                    {
                        m_HandLeftClosed = false;
                    }
                }
                else
                {
                    m_HandRight = leapHand;
                    if (m_HandRight.GrabStrength >= 1 - m_epsilon && m_HandRight.GrabStrength <= 1 + m_epsilon)
                    {
                        m_HandRightClosed = true;
                      //  Debug.Log("HandRight Closed");
                    } else
                    {
                        m_HandRightClosed = false;
                    }

                }
            }
        }

    }
}
