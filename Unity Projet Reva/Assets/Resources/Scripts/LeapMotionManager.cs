using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
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
    private float velocity = 0.01f;
    
    private bool holdL = false;
    private bool holdR = false;
    private bool moveL = false;
    private bool moveR = false;
    private bool holdRotate = false;
    private float offsetZ;
    private float offsetX;

    private bool m_HandLeftClosed = false;
    private bool m_HandRightClosed = false;
    private Controller m_controller;
	// Use this for initialization
	void Start () {
		m_controller = new Controller();
        offsetZ = 0f;
        offsetX = 0f;
        newPositionZ = 0f;
        newPositionX = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_controller.IsConnected)
        {
            GestureHands();
            MouvementHandler();
        }

    }

    void MouvementHandler()
    {
        if (m_HandLeftClosed && m_IsGrabbedControlPoint == false)
        {
            if (!moveR)
            {
                //Mouvement
                moveL = true;
                Vector3 HLPositionPalm = new Vector3(m_HandLeft.PalmPosition.x, m_HandLeft.PalmPosition.y, m_HandLeft.PalmPosition.z);

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
        }
        if (m_HandRightClosed && m_IsGrabbedControlPoint == false)
        {
            if (!moveL)
            {
                moveR = true;
                Vector3 HRPositionPalm = new Vector3(m_HandRight.PalmPosition.x, m_HandRight.PalmPosition.y, m_HandRight.PalmPosition.z);

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
                        Debug.Log("HandLeft Closed");
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
                        Debug.Log("HandRight Closed");
                    } else
                    {
                        m_HandRightClosed = false;
                    }

                }
            }
        }

    }
}
