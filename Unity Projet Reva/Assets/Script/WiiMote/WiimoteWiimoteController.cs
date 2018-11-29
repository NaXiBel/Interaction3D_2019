using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using UnityEngine.SceneManagement;

public class WiimoteWiimoteController : MonoBehaviour
{
    //variables
    private bool _threadRunning;
    private Thread _thread;
    private TCPConnection myTCP;
    private string serverMsg;
    public string msgToServer;

    public float smooth = 10.0f;
    public GameObject cam;
    public GameObject wiiIndicator;

    public double deadzoneXH = 50.00;
    public double deadzoneZH = 50.00;
    public double deadzoneXL = -50.00;
    public double deadzoneZL = -50.00;

    public double limitXH = 70.00;
    public double limitZH = 60.00;
    public double limitXL = -70.00;
    public double limitZL = -60.00;

    private double wiiIndOriginX = 0.0;
    private double wiiIndOriginZ = 0.0;
    public static double wiiIndRotZ = 0.0;
    public static double wiiIndRotX = 0.0;

    private bool wiiIndB = false;
    public static bool wiiIndA = false;
    private bool wiiIndHome = false;
    private bool wiiIndPlus = false;
    private bool wiiIndMinus = false;
    private bool wiiIndOne = false;
    private bool wiiIndTwo = false;
    private bool wiiIndUp = false;
    private bool wiiIndDown = false;
    private bool wiiIndRight = false;
    private bool wiiIndLeft = false;

    private bool canRead = true;
    public GameObject WiiMote;

    public GameObject cursor;
    public LineRenderer lineCursor;

    public Vector3 originTransformCursor;
    public Vector3 originTransformWiiMote;

    private GameObject objectToMove;

    private float sensibility = 100;

   
    void Awake()
    {
        //add a copy of TCPConnection to this game object
        //myTCP = TCPController.myTCP;
        myTCP = TCPController.myTCP;
    }
   
    void Start()
    {
       
        StartCoroutine(YieldingWork());
        originTransformCursor = this.cursor.transform.position;
        originTransformWiiMote = this.cam.transform.position;
        objectToMove = this.cursor;
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


    void MoveCamera()
    {

        if (this.wiiIndUp)
        {

            this.cam.transform.Rotate(Vector3.right * (float)20 / this.sensibility);
        }

        if (this.wiiIndDown)
        {
            this.cam.transform.Rotate(Vector3.left * (float)20 / this.sensibility);
        }

        if (this.wiiIndB)
        {
            if (this.wiiIndRight)
            {
                this.cam.transform.Rotate(Vector3.forward * (float)20 / this.sensibility);
            }

            if (this.wiiIndLeft)
            {
                this.cam.transform.Rotate(Vector3.back * (float)20 / this.sensibility);
            }

        }
        else
        {

            if (this.wiiIndRight)
            {
                this.cam.transform.Rotate(Vector3.up * (float)20 / this.sensibility);
            }
            if (this.wiiIndLeft)
            {
                this.cam.transform.Rotate(Vector3.down * (float)20 / this.sensibility);
            }

        }

    }

    void Update()
    {

        if (this.wiiIndTwo)
        {
            objectToMove = this.cam;
        }
        else if (this.wiiIndOne)
        {
            objectToMove = this.cursor;
        }

        if (this.wiiIndPlus && this.sensibility > 20)
        {
            this.sensibility = sensibility - 20;
        }
        else if (this.wiiIndMinus && this.sensibility < 200)
        {
            this.sensibility = sensibility + 20;
        }


        if (this.wiiIndHome)
        {
            this.cursor.transform.position = originTransformCursor;
            this.cam.transform.position = originTransformWiiMote;

        }


        /*if (wiiIndA)
        {
            this. WiiMote.GetComponent<Renderer>().material.color = Color.green;
        } else
        {
            this.WiiMote.GetComponent< Renderer>().material.color = Color.white;
        }*/
        Vector3 startMarker = objectToMove.transform.position;

        float nextRotX = (float)(wiiIndRotX - this.wiiIndOriginX);
        float nextPosY = 0;
        float nextPosZ = 0;
        if (this.wiiIndB)
        {
            nextPosZ = (float)wiiIndRotX / this.sensibility;
        }
        else
        {
            nextPosY = (float)-wiiIndRotX / this.sensibility;
        }

        float nextRotZ = (float)-(wiiIndRotZ - this.wiiIndOriginZ);
        float nextPosX = (float)-wiiIndRotZ / this.sensibility;

        Vector3 endMarker;
        if (this.wiiIndB)
        {
            endMarker = startMarker + new Vector3(nextPosX, 0, -nextPosZ);
        }
        else
        {
            endMarker = startMarker + new Vector3(nextPosX, nextPosY, 0);

        }

        objectToMove.transform.position = Vector3.Lerp(startMarker, endMarker, Time.deltaTime * this.smooth);
        this.transform.LookAt(this.cursor.transform);

        if (objectToMove == this.cam)
        {
            MoveCamera();
        }
    }

    //socket reading script
    void SocketResponse()
    {
        if (canRead)
        {
            string serverSays = myTCP.readSocket();
            String[] data;

            if (serverSays != "")
            {
                Debug.Log("[SERVER]" + serverSays);
                data = serverSays.Split(';');
                wiiIndA = Int32.Parse(data[0]) != 0;
                wiiIndRotZ = Double.Parse(data[1]);
                wiiIndRotX = Double.Parse(data[2]);
                this.wiiIndB = Int32.Parse(data[3]) != 0;
                this.wiiIndHome = Int32.Parse(data[4]) != 0;
                this.wiiIndPlus = Int32.Parse(data[5]) != 0;
                this.wiiIndMinus = Int32.Parse(data[6]) != 0;
                this.wiiIndOne = Int32.Parse(data[7]) != 0;
                this.wiiIndTwo = Int32.Parse(data[8]) != 0;
                this.wiiIndUp = Int32.Parse(data[9]) != 0;
                this.wiiIndRight = Int32.Parse(data[10]) != 0;
                this.wiiIndDown = Int32.Parse(data[11]) != 0;
                this.wiiIndLeft = Int32.Parse(data[12]) != 0;
                //Debug.Log("A : " + wiiIndA + " rot Z : " + this.wiiIndRotZ + " rot X : " + this.wiiIndRotX + "B : " + this.wiiIndB + " Home " + this.wiiIndHome + " Plus " + this.wiiIndPlus + " Minus " + this.wiiIndMinus + " One " + this.wiiIndOne + " Two " + this.wiiIndTwo);
            }

        }
        else 
        {
           
            //TCPController.launch = true;
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


}