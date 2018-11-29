using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using UnityEngine.SceneManagement;

public class OculusTouchController : MonoBehaviour
{

    //variables
    private bool _threadRunning;
    private Thread _thread;
    private TCPConnection myTCP;
    private string serverMsg;
    public string msgToServer;
    public bool canRead = true;
    private Vector3 positionLeftHand;
    private Quaternion leftHandRotation;
    private bool triggeredLeftHand;

    private Vector3 positionRightHand;
    private Quaternion rightHandRotation;
    private bool triggeredRightHand;

    private Vector3 positionHead;
    private Quaternion rotationHead;

    private Vector3 leftControl;
    private Vector3 rightControl;

    private bool aPressed;
    private bool bPressed;

    float scale;

    void Awake()
    {

        //add a copy of TCPConnection to this game object
        myTCP = TCPController.myTCP;

    }

    void Start()
    {

        positionLeftHand = new Vector3();
        positionRightHand = new Vector3();
		
        StartCoroutine(YieldingWork());
        scale = transform.localScale.x;
        Debug.Log(scale);

    }

    private void Update()
    {

        transform.localPosition = transform.localPosition + transform.localRotation * leftControl * Time.deltaTime * (float)Math.Log(scale,2);
        transform.localRotation *= Quaternion.Euler(rightControl);

        if (aPressed && bPressed)
            return;

        if (aPressed)
        {
            scale = Math.Max(scale / 1.02f, 5);
            transform.localScale = new Vector3(scale, scale, scale);
        }

        if (bPressed)
        {
            scale = Math.Min(scale * 1.02f, 20);
            transform.localScale = new Vector3(scale, scale, scale);
        }

    }

    IEnumerator YieldingWork()
    {
        bool workDone = false;

        while (!workDone)
        {
            SocketResponse();
            yield return null;

        }
    }
    
    //socket reading script
    void SocketResponse()
    {
        if (canRead)
        {
            string serverSays = myTCP.readSocket();

            if (serverSays != "")
            {

                Debug.Log("[SERVER]" + serverSays);

            }

            String[] s = serverSays.Split(':');

            if (s.Length > 31 && s[0].Equals("OT"))
            {

                positionLeftHand.x = float.Parse(s[1]);
                positionLeftHand.y = float.Parse(s[2]);
                positionLeftHand.z = -float.Parse(s[3]);

                leftHandRotation.x = float.Parse(s[4]);
                leftHandRotation.y = float.Parse(s[5]);
                leftHandRotation.z = float.Parse(s[6]);
                leftHandRotation.w = float.Parse(s[7]);

                triggeredLeftHand = bool.Parse(s[8]);

                positionRightHand.x = float.Parse(s[9]);
                positionRightHand.y = float.Parse(s[10]);
                positionRightHand.z = -float.Parse(s[11]);

                rightHandRotation.x = float.Parse(s[12]);
                rightHandRotation.y = float.Parse(s[13]);
                rightHandRotation.z = float.Parse(s[14]);
                rightHandRotation.w = float.Parse(s[15]);

                triggeredRightHand = bool.Parse(s[16]);

                positionHead.x = float.Parse(s[17]);
                positionHead.y = float.Parse(s[18]);
                positionHead.z = -float.Parse(s[19]);

                rotationHead.x = float.Parse(s[20]);
                rotationHead.y = float.Parse(s[21]);
                rotationHead.z = float.Parse(s[22]);
                rotationHead.w = float.Parse(s[23]);

                leftControl.x = float.Parse(s[24]);
                leftControl.y = float.Parse(s[25]);
                leftControl.z = float.Parse(s[26]);

                rightControl.x = float.Parse(s[27]);
                rightControl.y = float.Parse(s[28]);
                rightControl.z = float.Parse(s[29]);

                aPressed = bool.Parse(s[30]);
                bPressed = bool.Parse(s[31]);

            }
        }
        else
        {

        }


    }
    IEnumerator Example()
    {
        yield return new WaitForSeconds(3);
        myTCP.closeSocket();
        TCPController.myTCP.closeSocket();
        TCPController.launch = true;
        Destroy(GameObject.Find("cont").GetComponent<TCPConnection>().gameObject);
        Destroy(GameObject.Find("cont"));
        //TCPController.Connection(GameObject.Find("cont").gameObject);
        Debug.Log("Socket Closed");
        //SceneManager.LoadScene(0, LoadSceneMode.Single);
        SceneManager.LoadScene(0);
    }
    //send message to the server
    public void SendToServer()
    {

        myTCP.writeSocket("OKkkkkkkkkk");


        canRead = false;

        StartCoroutine(Example());
    }
    public Vector3 GetLeftHand()
    {
        return this.positionLeftHand;
    }

    public Quaternion GetLeftQuaternions()
    {
        return leftHandRotation;
    }

    public bool IsLeftHandTriggered()
    {
        return triggeredLeftHand;
    }

    public Vector3 GetRightHand()
    {
        return this.positionRightHand;
    }

    public Quaternion GetRightQuaternion()
    {
        return rightHandRotation;
    }

    public bool IsRightHandTriggered()
    {
        return triggeredRightHand;
    }

    public Vector3 GetHead()
    {
        return positionHead;
    }

    public Quaternion GetRotationHead()
    {
        return rotationHead;
    }

    public bool IsAPressed()
    {
        return aPressed;
    }

    public bool IsBPressed()
    {
        return bPressed;
    }

    public float GetScale()
    {
        return scale;
    }

}
