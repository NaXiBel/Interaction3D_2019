using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using UnityEngine.SceneManagement;

public class LeapMotionController : MonoBehaviour {

    //variables
    private bool _threadRunning;
    private Thread _thread;
    private TCPConnection myTCP;
    private string serverMsg;
    public string msgToServer;
    private bool canRead = true;
    public float smooth = 10.0f;
    public GameObject lHand;
    public GameObject rHand;
    public GameObject cam;

    public GameObject Palm;
    //public GameObject thumb;
    //public GameObject index;
    //public GameObject middle;
    //public GameObject ring;
    //public GameObject pinky;

    public GameObject PalmR;
    //public GameObject thumbR;
    //public GameObject indexR;
    //public GameObject middleR;
    //public GameObject ringR;
    //public GameObject pinkyR;
    public float camDistX;
    public float camDistY;
    public float camDistZ;
    void Awake() {
        //add a copy of TCPConnection to this game object
        myTCP = TCPController.myTCP;
        
    }

    void Start() {
        //StartCoroutine(YieldingWork());
        InvokeRepeating("SocketResponse", 0f, 0.01f);  //1s delay, repeat every 1s
    }

    IEnumerator YieldingWork() {
        bool workDone = false;
        while (!workDone) {
            SocketResponse();
            yield return null;
        }
    }

    void Update() {


        //keep checking the server for messages, if a message is received from server, it gets logged in the Debug console (see function below)
    }

    //socket reading script
    void SocketResponse() {


        if (canRead)
        {


            string serverSays = myTCP.readSocket();
            String[] data;
            LineRenderer line;
            LineRenderer linePalmPinky;
            LineRenderer lRendererThumb;
            LineRenderer lRendererIndex;
            LineRenderer lRendererMiddle;
            LineRenderer lRendererRing;
            LineRenderer lRendererPinky;
            LineRenderer lRMeta;
            LineRenderer lRPro;
            LineRenderer lRMid;
            LineRenderer lRDist;

            if (serverSays != "")
            {
                //Debug.Log("[SERVER]" + serverSays);
                data = serverSays.Split(';');
                //Debug.Log("debug" + data.Length);
                String isClosedL = data[0];
                if (isClosedL == "1") lHand.GetComponent<Hand>().Closed = true;
                else lHand.GetComponent<Hand>().Closed = false;

                //Get Palm
                String[] fingCoord = data[1].Split('/');
                Vector3 pos = new Vector3();
                pos.x = (float)Double.Parse(fingCoord[0]);
                pos.x /= 100;
                pos.y = (float)Double.Parse(fingCoord[1]);
                pos.y /= 100;
                pos.z = (float)Double.Parse(fingCoord[2]);
                pos.z /= -100;

                lHand.transform.localPosition = new Vector3(camDistX, camDistY, camDistZ);
                Palm.transform.localPosition = new Vector3(pos.x, pos.y, pos.z);

                lRMeta = lHand.transform.Find("ThumbM").GetComponent<LineRenderer>();
                lRendererThumb = lRMeta;
                lRPro = lHand.transform.Find("ThumbP").GetComponent<LineRenderer>();
                lRMid = lHand.transform.Find("ThumbI").GetComponent<LineRenderer>();
                lRDist = lHand.transform.Find("ThumbD").GetComponent<LineRenderer>();

                //Get PalmPinky
                fingCoord = data[2].Split('/');
                pos = new Vector3();
                pos.x = (float)Double.Parse(fingCoord[0]);
                pos.x /= 100;
                pos.y = (float)Double.Parse(fingCoord[1]);
                pos.y /= 100;
                pos.z = (float)Double.Parse(fingCoord[2]);
                pos.z /= -100;

                lHand.transform.Find("PalmPinky").localPosition = new Vector3(pos.x, pos.y, pos.z);
                linePalmPinky = lHand.transform.Find("PalmPinky").GetComponent<LineRenderer>();
                linePalmPinky.SetPosition(0, lHand.transform.Find("PalmPinky").position);
                lRMeta.SetPosition(1, lHand.transform.Find("PalmPinky").position);

                //Get Thumb


                fingCoord = data[3].Split('/');
                pos = new Vector3();
                pos.x = (float)Double.Parse(fingCoord[0]);
                pos.x /= 100;
                pos.y = (float)Double.Parse(fingCoord[1]);
                pos.y /= 100;
                pos.z = (float)Double.Parse(fingCoord[2]);
                pos.z /= -100;

                lHand.transform.Find("ThumbM").localPosition = new Vector3(pos.x, pos.y, pos.z);
                lRMeta.SetPosition(0, lHand.transform.Find("ThumbM").position);
                lRPro.SetPosition(1, lHand.transform.Find("ThumbM").position);

                fingCoord = data[4].Split('/');
                pos = new Vector3();
                pos.x = (float)Double.Parse(fingCoord[0]);
                pos.x /= 100;
                pos.y = (float)Double.Parse(fingCoord[1]);
                pos.y /= 100;
                pos.z = (float)Double.Parse(fingCoord[2]);
                pos.z /= -100;

                lHand.transform.Find("ThumbP").localPosition = new Vector3(pos.x, pos.y, pos.z);
                lRPro.SetPosition(0, lHand.transform.Find("ThumbP").position);
                lRMid.SetPosition(1, lHand.transform.Find("ThumbP").position);

                fingCoord = data[5].Split('/');
                pos = new Vector3();
                pos.x = (float)Double.Parse(fingCoord[0]);
                pos.x /= 100;
                pos.y = (float)Double.Parse(fingCoord[1]);
                pos.y /= 100;
                pos.z = (float)Double.Parse(fingCoord[2]);
                pos.z /= -100;

                lHand.transform.Find("ThumbI").localPosition = new Vector3(pos.x, pos.y, pos.z);
                lRMid.SetPosition(0, lHand.transform.Find("ThumbI").position);
                lRDist.SetPosition(1, lHand.transform.Find("ThumbI").position);

                fingCoord = data[6].Split('/');
                pos = new Vector3();
                pos.x = (float)Double.Parse(fingCoord[0]);
                pos.x /= 100;
                pos.y = (float)Double.Parse(fingCoord[1]);
                pos.y /= 100;
                pos.z = (float)Double.Parse(fingCoord[2]);
                pos.z /= -100;

                lHand.transform.Find("ThumbD").localPosition = new Vector3(pos.x, pos.y, pos.z);
                lRDist.SetPosition(0, lHand.transform.Find("ThumbD").position);

                //Get Index
                lRMeta = lHand.transform.Find("IndexM").GetComponent<LineRenderer>();
                lRendererIndex = lRMeta;
                lRPro = lHand.transform.Find("IndexP").GetComponent<LineRenderer>();
                lRMid = lHand.transform.Find("IndexI").GetComponent<LineRenderer>();
                lRDist = lHand.transform.Find("IndexD").GetComponent<LineRenderer>();

                fingCoord = data[7].Split('/');
                pos = new Vector3();
                pos.x = (float)Double.Parse(fingCoord[0]);
                pos.x /= 100;
                pos.y = (float)Double.Parse(fingCoord[1]);
                pos.y /= 100;
                pos.z = (float)Double.Parse(fingCoord[2]);
                pos.z /= -100;

                lHand.transform.Find("IndexM").localPosition = new Vector3(pos.x, pos.y, pos.z);
                lRMeta.SetPosition(0, lHand.transform.Find("IndexM").position);
                lRPro.SetPosition(1, lHand.transform.Find("IndexM").position);

                fingCoord = data[8].Split('/');
                pos = new Vector3();
                pos.x = (float)Double.Parse(fingCoord[0]);
                pos.x /= 100;
                pos.y = (float)Double.Parse(fingCoord[1]);
                pos.y /= 100;
                pos.z = (float)Double.Parse(fingCoord[2]);
                pos.z /= -100;

                lHand.transform.Find("IndexP").localPosition = new Vector3(pos.x, pos.y, pos.z);
                lRPro.SetPosition(0, lHand.transform.Find("IndexP").position);
                lRMid.SetPosition(1, lHand.transform.Find("IndexP").position);

                fingCoord = data[9].Split('/');
                pos = new Vector3();
                pos.x = (float)Double.Parse(fingCoord[0]);
                pos.x /= 100;
                pos.y = (float)Double.Parse(fingCoord[1]);
                pos.y /= 100;
                pos.z = (float)Double.Parse(fingCoord[2]);
                pos.z /= -100;

                lHand.transform.Find("IndexI").localPosition = new Vector3(pos.x, pos.y, pos.z);
                lRMid.SetPosition(0, lHand.transform.Find("IndexI").position);
                lRDist.SetPosition(1, lHand.transform.Find("IndexI").position);

                fingCoord = data[10].Split('/');
                pos = new Vector3();
                pos.x = (float)Double.Parse(fingCoord[0]);
                pos.x /= 100;
                pos.y = (float)Double.Parse(fingCoord[1]);
                pos.y /= 100;
                pos.z = (float)Double.Parse(fingCoord[2]);
                pos.z /= -100;

                lHand.transform.Find("IndexD").localPosition = new Vector3(pos.x, pos.y, pos.z);
                lRDist.SetPosition(0, lHand.transform.Find("IndexD").position);

                //---------- Get Middle ----------
                lRMeta = lHand.transform.Find("MiddleM").GetComponent<LineRenderer>();
                lRendererMiddle = lRMeta;
                lRPro = lHand.transform.Find("MiddleP").GetComponent<LineRenderer>();
                lRMid = lHand.transform.Find("MiddleI").GetComponent<LineRenderer>();
                lRDist = lHand.transform.Find("MiddleD").GetComponent<LineRenderer>();

                fingCoord = data[11].Split('/');
                pos = new Vector3();
                pos.x = (float)Double.Parse(fingCoord[0]);
                pos.x /= 100;
                pos.y = (float)Double.Parse(fingCoord[1]);
                pos.y /= 100;
                pos.z = (float)Double.Parse(fingCoord[2]);
                pos.z /= -100;

                lHand.transform.Find("MiddleM").localPosition = new Vector3(pos.x, pos.y, pos.z);
                lRMeta.SetPosition(0, lHand.transform.Find("MiddleM").position);
                lRPro.SetPosition(1, lHand.transform.Find("MiddleM").position);

                fingCoord = data[12].Split('/');
                pos = new Vector3();
                pos.x = (float)Double.Parse(fingCoord[0]);
                pos.x /= 100;
                pos.y = (float)Double.Parse(fingCoord[1]);
                pos.y /= 100;
                pos.z = (float)Double.Parse(fingCoord[2]);
                pos.z /= -100;

                lHand.transform.Find("MiddleP").localPosition = new Vector3(pos.x, pos.y, pos.z);
                lRPro.SetPosition(0, lHand.transform.Find("MiddleP").position);
                lRMid.SetPosition(1, lHand.transform.Find("MiddleP").position);

                fingCoord = data[13].Split('/');
                pos = new Vector3();
                pos.x = (float)Double.Parse(fingCoord[0]);
                pos.x /= 100;
                pos.y = (float)Double.Parse(fingCoord[1]);
                pos.y /= 100;
                pos.z = (float)Double.Parse(fingCoord[2]);
                pos.z /= -100;

                lHand.transform.Find("MiddleI").localPosition = new Vector3(pos.x, pos.y, pos.z);
                lRMid.SetPosition(0, lHand.transform.Find("MiddleI").position);
                lRDist.SetPosition(1, lHand.transform.Find("MiddleI").position);

                fingCoord = data[14].Split('/');
                pos = new Vector3();
                pos.x = (float)Double.Parse(fingCoord[0]);
                pos.x /= 100;
                pos.y = (float)Double.Parse(fingCoord[1]);
                pos.y /= 100;
                pos.z = (float)Double.Parse(fingCoord[2]);
                pos.z /= -100;

                lHand.transform.Find("MiddleD").localPosition = new Vector3(pos.x, pos.y, pos.z);
                lRDist.SetPosition(0, lHand.transform.Find("MiddleD").position);

                //Get Ring
                lRMeta = lHand.transform.Find("RingM").GetComponent<LineRenderer>();
                lRendererRing = lRMeta;
                lRPro = lHand.transform.Find("RingP").GetComponent<LineRenderer>();
                lRMid = lHand.transform.Find("RingI").GetComponent<LineRenderer>();
                lRDist = lHand.transform.Find("RingD").GetComponent<LineRenderer>();

                fingCoord = data[15].Split('/');
                pos = new Vector3();
                pos.x = (float)Double.Parse(fingCoord[0]);
                pos.x /= 100;
                pos.y = (float)Double.Parse(fingCoord[1]);
                pos.y /= 100;
                pos.z = (float)Double.Parse(fingCoord[2]);
                pos.z /= -100;

                lHand.transform.Find("RingM").localPosition = new Vector3(pos.x, pos.y, pos.z);
                lRMeta.SetPosition(0, lHand.transform.Find("RingM").position);
                lRPro.SetPosition(1, lHand.transform.Find("RingM").position);

                fingCoord = data[16].Split('/');
                pos = new Vector3();
                pos.x = (float)Double.Parse(fingCoord[0]);
                pos.x /= 100;
                pos.y = (float)Double.Parse(fingCoord[1]);
                pos.y /= 100;
                pos.z = (float)Double.Parse(fingCoord[2]);
                pos.z /= -100;

                lHand.transform.Find("RingP").localPosition = new Vector3(pos.x, pos.y, pos.z);
                lRPro.SetPosition(0, lHand.transform.Find("RingP").position);
                lRMid.SetPosition(1, lHand.transform.Find("RingP").position);

                fingCoord = data[17].Split('/');
                pos = new Vector3();
                pos.x = (float)Double.Parse(fingCoord[0]);
                pos.x /= 100;
                pos.y = (float)Double.Parse(fingCoord[1]);
                pos.y /= 100;
                pos.z = (float)Double.Parse(fingCoord[2]);
                pos.z /= -100;

                lHand.transform.Find("RingI").localPosition = new Vector3(pos.x, pos.y, pos.z);
                lRMid.SetPosition(0, lHand.transform.Find("RingI").position);
                lRDist.SetPosition(1, lHand.transform.Find("RingI").position);

                fingCoord = data[18].Split('/');
                pos = new Vector3();
                pos.x = (float)Double.Parse(fingCoord[0]);
                pos.x /= 100;
                pos.y = (float)Double.Parse(fingCoord[1]);
                pos.y /= 100;
                pos.z = (float)Double.Parse(fingCoord[2]);
                pos.z /= -100;

                lHand.transform.Find("RingD").localPosition = new Vector3(pos.x, pos.y, pos.z);
                lRDist.SetPosition(0, lHand.transform.Find("RingD").position);

                //Get Pinky
                lRMeta = lHand.transform.Find("PinkyM").GetComponent<LineRenderer>();
                lRendererPinky = lRMeta;
                lRPro = lHand.transform.Find("PinkyP").GetComponent<LineRenderer>();
                lRMid = lHand.transform.Find("PinkyI").GetComponent<LineRenderer>();
                lRDist = lHand.transform.Find("PinkyD").GetComponent<LineRenderer>();

                fingCoord = data[19].Split('/');
                pos = new Vector3();
                pos.x = (float)Double.Parse(fingCoord[0]);
                pos.x /= 100;
                pos.y = (float)Double.Parse(fingCoord[1]);
                pos.y /= 100;
                pos.z = (float)Double.Parse(fingCoord[2]);
                pos.z /= -100;

                lHand.transform.Find("PinkyM").localPosition = new Vector3(pos.x, pos.y, pos.z);
                linePalmPinky.SetPosition(1, lHand.transform.Find("PinkyM").position);
                lRMeta.SetPosition(0, lHand.transform.Find("PinkyM").position);
                lRPro.SetPosition(1, lHand.transform.Find("PinkyM").position);

                fingCoord = data[20].Split('/');
                pos = new Vector3();
                pos.x = (float)Double.Parse(fingCoord[0]);
                pos.x /= 100;
                pos.y = (float)Double.Parse(fingCoord[1]);
                pos.y /= 100;
                pos.z = (float)Double.Parse(fingCoord[2]);
                pos.z /= -100;

                lHand.transform.Find("PinkyP").localPosition = new Vector3(pos.x, pos.y, pos.z);
                lRPro.SetPosition(0, lHand.transform.Find("PinkyP").position);
                lRMid.SetPosition(1, lHand.transform.Find("PinkyP").position);

                fingCoord = data[21].Split('/');
                pos = new Vector3();
                pos.x = (float)Double.Parse(fingCoord[0]);
                pos.x /= 100;
                pos.y = (float)Double.Parse(fingCoord[1]);
                pos.y /= 100;
                pos.z = (float)Double.Parse(fingCoord[2]);
                pos.z /= -100;

                lHand.transform.Find("PinkyI").localPosition = new Vector3(pos.x, pos.y, pos.z);
                lRMid.SetPosition(0, lHand.transform.Find("PinkyI").position);
                lRDist.SetPosition(1, lHand.transform.Find("PinkyI").position);

                fingCoord = data[22].Split('/');
                pos = new Vector3();
                pos.x = (float)Double.Parse(fingCoord[0]);
                pos.x /= 100;
                pos.y = (float)Double.Parse(fingCoord[1]);
                pos.y /= 100;
                pos.z = (float)Double.Parse(fingCoord[2]);
                pos.z /= -100;

                lHand.transform.Find("PinkyD").localPosition = new Vector3(pos.x, pos.y, pos.z);
                lRDist.SetPosition(0, lHand.transform.Find("PinkyD").position);

                /*Link all meta except thumb meta and palmPinkyMeta*/
                lRendererPinky.SetPosition(1, lRendererRing.GetPosition(0));
                lRendererRing.SetPosition(1, lRendererMiddle.GetPosition(0));
                lRendererMiddle.SetPosition(1, lRendererIndex.GetPosition(0));
                lRendererIndex.SetPosition(1, lRendererThumb.GetPosition(0));

                if (data.Length > 23)
                {
                    isClosedL = data[23];
                    if (isClosedL == "1") rHand.GetComponent<Hand>().Closed = true;
                    else rHand.GetComponent<Hand>().Closed = false;

                    //Get Palm
                    fingCoord = data[24].Split('/');
                    pos = new Vector3();
                    pos.x = (float)Double.Parse(fingCoord[0]);
                    pos.x /= 100;
                    pos.y = (float)Double.Parse(fingCoord[1]);
                    pos.y /= 100;
                    pos.z = (float)Double.Parse(fingCoord[2]);
                    pos.z /= -100;

                    rHand.transform.localPosition = new Vector3(camDistX, camDistY, camDistZ);
                    PalmR.transform.localPosition = new Vector3(pos.x, pos.y, pos.z);

                    lRMeta = rHand.transform.Find("ThumbM").GetComponent<LineRenderer>();
                    lRendererThumb = lRMeta;
                    lRPro = rHand.transform.Find("ThumbP").GetComponent<LineRenderer>();
                    lRMid = rHand.transform.Find("ThumbI").GetComponent<LineRenderer>();
                    lRDist = rHand.transform.Find("ThumbD").GetComponent<LineRenderer>();

                    //Get PalmPinky
                    fingCoord = data[25].Split('/');
                    pos = new Vector3();
                    pos.x = (float)Double.Parse(fingCoord[0]);
                    pos.x /= 100;
                    pos.y = (float)Double.Parse(fingCoord[1]);
                    pos.y /= 100;
                    pos.z = (float)Double.Parse(fingCoord[2]);
                    pos.z /= -100;

                    rHand.transform.Find("PalmPinky").localPosition = new Vector3(pos.x, pos.y, pos.z);
                    linePalmPinky = rHand.transform.Find("PalmPinky").GetComponent<LineRenderer>();
                    linePalmPinky.SetPosition(0, rHand.transform.Find("PalmPinky").position);
                    lRMeta.SetPosition(1, rHand.transform.Find("PalmPinky").position);

                    //Get Thumb
                    fingCoord = data[26].Split('/');
                    pos = new Vector3();
                    pos.x = (float)Double.Parse(fingCoord[0]);
                    pos.x /= 100;
                    pos.y = (float)Double.Parse(fingCoord[1]);
                    pos.y /= 100;
                    pos.z = (float)Double.Parse(fingCoord[2]);
                    pos.z /= -100;

                    rHand.transform.Find("ThumbM").localPosition = new Vector3(pos.x, pos.y, pos.z);
                    lRMeta.SetPosition(0, rHand.transform.Find("ThumbM").position);
                    lRPro.SetPosition(1, rHand.transform.Find("ThumbM").position);

                    fingCoord = data[27].Split('/');
                    pos = new Vector3();
                    pos.x = (float)Double.Parse(fingCoord[0]);
                    pos.x /= 100;
                    pos.y = (float)Double.Parse(fingCoord[1]);
                    pos.y /= 100;
                    pos.z = (float)Double.Parse(fingCoord[2]);
                    pos.z /= -100;

                    rHand.transform.Find("ThumbP").localPosition = new Vector3(pos.x, pos.y, pos.z);
                    lRPro.SetPosition(0, rHand.transform.Find("ThumbP").position);
                    lRMid.SetPosition(1, rHand.transform.Find("ThumbP").position);

                    fingCoord = data[28].Split('/');
                    pos = new Vector3();
                    pos.x = (float)Double.Parse(fingCoord[0]);
                    pos.x /= 100;
                    pos.y = (float)Double.Parse(fingCoord[1]);
                    pos.y /= 100;
                    pos.z = (float)Double.Parse(fingCoord[2]);
                    pos.z /= -100;

                    rHand.transform.Find("ThumbI").localPosition = new Vector3(pos.x, pos.y, pos.z);
                    lRMid.SetPosition(0, rHand.transform.Find("ThumbI").position);
                    lRDist.SetPosition(1, rHand.transform.Find("ThumbI").position);

                    fingCoord = data[29].Split('/');
                    pos = new Vector3();
                    pos.x = (float)Double.Parse(fingCoord[0]);
                    pos.x /= 100;
                    pos.y = (float)Double.Parse(fingCoord[1]);
                    pos.y /= 100;
                    pos.z = (float)Double.Parse(fingCoord[2]);
                    pos.z /= -100;

                    rHand.transform.Find("ThumbD").localPosition = new Vector3(pos.x, pos.y, pos.z);
                    lRDist.SetPosition(0, rHand.transform.Find("ThumbD").position);

                    //Get Index
                    lRMeta = rHand.transform.Find("IndexM").GetComponent<LineRenderer>();
                    lRendererIndex = lRMeta;
                    lRPro = rHand.transform.Find("IndexP").GetComponent<LineRenderer>();
                    lRMid = rHand.transform.Find("IndexI").GetComponent<LineRenderer>();
                    lRDist = rHand.transform.Find("IndexD").GetComponent<LineRenderer>();

                    fingCoord = data[30].Split('/');
                    pos = new Vector3();
                    pos.x = (float)Double.Parse(fingCoord[0]);
                    pos.x /= 100;
                    pos.y = (float)Double.Parse(fingCoord[1]);
                    pos.y /= 100;
                    pos.z = (float)Double.Parse(fingCoord[2]);
                    pos.z /= -100;

                    rHand.transform.Find("IndexM").localPosition = new Vector3(pos.x, pos.y, pos.z);
                    lRMeta.SetPosition(0, rHand.transform.Find("IndexM").position);
                    lRPro.SetPosition(1, rHand.transform.Find("IndexM").position);

                    fingCoord = data[31].Split('/');
                    pos = new Vector3();
                    pos.x = (float)Double.Parse(fingCoord[0]);
                    pos.x /= 100;
                    pos.y = (float)Double.Parse(fingCoord[1]);
                    pos.y /= 100;
                    pos.z = (float)Double.Parse(fingCoord[2]);
                    pos.z /= -100;

                    rHand.transform.Find("IndexP").localPosition = new Vector3(pos.x, pos.y, pos.z);
                    lRPro.SetPosition(0, rHand.transform.Find("IndexP").position);
                    lRMid.SetPosition(1, rHand.transform.Find("IndexP").position);

                    fingCoord = data[32].Split('/');
                    pos = new Vector3();
                    pos.x = (float)Double.Parse(fingCoord[0]);
                    pos.x /= 100;
                    pos.y = (float)Double.Parse(fingCoord[1]);
                    pos.y /= 100;
                    pos.z = (float)Double.Parse(fingCoord[2]);
                    pos.z /= -100;

                    rHand.transform.Find("IndexI").localPosition = new Vector3(pos.x, pos.y, pos.z);
                    lRMid.SetPosition(0, rHand.transform.Find("IndexI").position);
                    lRDist.SetPosition(1, rHand.transform.Find("IndexI").position);

                    fingCoord = data[33].Split('/');
                    pos = new Vector3();
                    pos.x = (float)Double.Parse(fingCoord[0]);
                    pos.x /= 100;
                    pos.y = (float)Double.Parse(fingCoord[1]);
                    pos.y /= 100;
                    pos.z = (float)Double.Parse(fingCoord[2]);
                    pos.z /= -100;

                    rHand.transform.Find("IndexD").localPosition = new Vector3(pos.x, pos.y, pos.z);
                    lRDist.SetPosition(0, rHand.transform.Find("IndexD").position);

                    //Get Middle
                    lRMeta = rHand.transform.Find("MiddleM").GetComponent<LineRenderer>();
                    lRendererMiddle = lRMeta;
                    lRPro = rHand.transform.Find("MiddleP").GetComponent<LineRenderer>();
                    lRMid = rHand.transform.Find("MiddleI").GetComponent<LineRenderer>();
                    lRDist = rHand.transform.Find("MiddleD").GetComponent<LineRenderer>();

                    fingCoord = data[34].Split('/');
                    pos = new Vector3();
                    pos.x = (float)Double.Parse(fingCoord[0]);
                    pos.x /= 100;
                    pos.y = (float)Double.Parse(fingCoord[1]);
                    pos.y /= 100;
                    pos.z = (float)Double.Parse(fingCoord[2]);
                    pos.z /= -100;

                    rHand.transform.Find("MiddleM").localPosition = new Vector3(pos.x, pos.y, pos.z);
                    lRMeta.SetPosition(0, rHand.transform.Find("MiddleM").position);
                    lRPro.SetPosition(1, rHand.transform.Find("MiddleM").position);

                    fingCoord = data[35].Split('/');
                    pos = new Vector3();
                    pos.x = (float)Double.Parse(fingCoord[0]);
                    pos.x /= 100;
                    pos.y = (float)Double.Parse(fingCoord[1]);
                    pos.y /= 100;
                    pos.z = (float)Double.Parse(fingCoord[2]);
                    pos.z /= -100;

                    rHand.transform.Find("MiddleP").localPosition = new Vector3(pos.x, pos.y, pos.z);
                    lRPro.SetPosition(0, rHand.transform.Find("MiddleP").position);
                    lRMid.SetPosition(1, rHand.transform.Find("MiddleP").position);

                    fingCoord = data[36].Split('/');
                    pos = new Vector3();
                    pos.x = (float)Double.Parse(fingCoord[0]);
                    pos.x /= 100;
                    pos.y = (float)Double.Parse(fingCoord[1]);
                    pos.y /= 100;
                    pos.z = (float)Double.Parse(fingCoord[2]);
                    pos.z /= -100;

                    rHand.transform.Find("MiddleI").localPosition = new Vector3(pos.x, pos.y, pos.z);
                    lRMid.SetPosition(0, rHand.transform.Find("MiddleI").position);
                    lRDist.SetPosition(1, rHand.transform.Find("MiddleI").position);

                    fingCoord = data[37].Split('/');
                    pos = new Vector3();
                    pos.x = (float)Double.Parse(fingCoord[0]);
                    pos.x /= 100;
                    pos.y = (float)Double.Parse(fingCoord[1]);
                    pos.y /= 100;
                    pos.z = (float)Double.Parse(fingCoord[2]);
                    pos.z /= -100;

                    rHand.transform.Find("MiddleD").localPosition = new Vector3(pos.x, pos.y, pos.z);
                    lRDist.SetPosition(0, rHand.transform.Find("MiddleD").position);

                    //Get Ring
                    lRMeta = rHand.transform.Find("RingM").GetComponent<LineRenderer>();
                    lRendererRing = lRMeta;
                    lRPro = rHand.transform.Find("RingP").GetComponent<LineRenderer>();
                    lRMid = rHand.transform.Find("RingI").GetComponent<LineRenderer>();
                    lRDist = rHand.transform.Find("RingD").GetComponent<LineRenderer>();
                    fingCoord = data[38].Split('/');
                    pos = new Vector3();
                    pos.x = (float)Double.Parse(fingCoord[0]);
                    pos.x /= 100;
                    pos.y = (float)Double.Parse(fingCoord[1]);
                    pos.y /= 100;
                    pos.z = (float)Double.Parse(fingCoord[2]);
                    pos.z /= -100;

                    rHand.transform.Find("RingM").localPosition = new Vector3(pos.x, pos.y, pos.z);
                    lRMeta.SetPosition(0, rHand.transform.Find("RingM").position);
                    lRPro.SetPosition(1, rHand.transform.Find("RingM").position);

                    fingCoord = data[39].Split('/');
                    pos = new Vector3();
                    pos.x = (float)Double.Parse(fingCoord[0]);
                    pos.x /= 100;
                    pos.y = (float)Double.Parse(fingCoord[1]);
                    pos.y /= 100;
                    pos.z = (float)Double.Parse(fingCoord[2]);
                    pos.z /= -100;

                    rHand.transform.Find("RingP").localPosition = new Vector3(pos.x, pos.y, pos.z);
                    lRPro.SetPosition(0, rHand.transform.Find("RingP").position);
                    lRMid.SetPosition(1, rHand.transform.Find("RingP").position);

                    fingCoord = data[40].Split('/');
                    pos = new Vector3();
                    pos.x = (float)Double.Parse(fingCoord[0]);
                    pos.x /= 100;
                    pos.y = (float)Double.Parse(fingCoord[1]);
                    pos.y /= 100;
                    pos.z = (float)Double.Parse(fingCoord[2]);
                    pos.z /= -100;

                    rHand.transform.Find("RingI").localPosition = new Vector3(pos.x, pos.y, pos.z);
                    lRMid.SetPosition(0, rHand.transform.Find("RingI").position);
                    lRDist.SetPosition(1, rHand.transform.Find("RingI").position);

                    fingCoord = data[41].Split('/');
                    pos = new Vector3();
                    pos.x = (float)Double.Parse(fingCoord[0]);
                    pos.x /= 100;
                    pos.y = (float)Double.Parse(fingCoord[1]);
                    pos.y /= 100;
                    pos.z = (float)Double.Parse(fingCoord[2]);
                    pos.z /= -100;

                    rHand.transform.Find("RingD").localPosition = new Vector3(pos.x, pos.y, pos.z);
                    lRDist.SetPosition(0, rHand.transform.Find("RingD").position);

                    //Get Pinky
                    lRMeta = rHand.transform.Find("PinkyM").GetComponent<LineRenderer>();
                    lRendererPinky = lRMeta;
                    lRPro = rHand.transform.Find("PinkyP").GetComponent<LineRenderer>();
                    lRMid = rHand.transform.Find("PinkyI").GetComponent<LineRenderer>();
                    lRDist = rHand.transform.Find("PinkyD").GetComponent<LineRenderer>();

                    fingCoord = data[42].Split('/');
                    pos = new Vector3();
                    pos.x = (float)Double.Parse(fingCoord[0]);
                    pos.x /= 100;
                    pos.y = (float)Double.Parse(fingCoord[1]);
                    pos.y /= 100;
                    pos.z = (float)Double.Parse(fingCoord[2]);
                    pos.z /= -100;

                    rHand.transform.Find("PinkyM").localPosition = new Vector3(pos.x, pos.y, pos.z);
                    linePalmPinky.SetPosition(1, rHand.transform.Find("PinkyM").position);
                    lRMeta.SetPosition(0, rHand.transform.Find("PinkyM").position);
                    lRPro.SetPosition(1, rHand.transform.Find("PinkyM").position);
                    fingCoord = data[43].Split('/');
                    pos = new Vector3();
                    pos.x = (float)Double.Parse(fingCoord[0]);
                    pos.x /= 100;
                    pos.y = (float)Double.Parse(fingCoord[1]);
                    pos.y /= 100;
                    pos.z = (float)Double.Parse(fingCoord[2]);
                    pos.z /= -100;

                    rHand.transform.Find("PinkyP").localPosition = new Vector3(pos.x, pos.y, pos.z);
                    lRPro.SetPosition(0, rHand.transform.Find("PinkyP").position);
                    lRMid.SetPosition(1, rHand.transform.Find("PinkyP").position);

                    fingCoord = data[44].Split('/');
                    pos = new Vector3();
                    pos.x = (float)Double.Parse(fingCoord[0]);
                    pos.x /= 100;
                    pos.y = (float)Double.Parse(fingCoord[1]);
                    pos.y /= 100;
                    pos.z = (float)Double.Parse(fingCoord[2]);
                    pos.z /= -100;

                    rHand.transform.Find("PinkyI").localPosition = new Vector3(pos.x, pos.y, pos.z);
                    lRMid.SetPosition(0, rHand.transform.Find("PinkyI").position);
                    lRDist.SetPosition(1, rHand.transform.Find("PinkyI").position);

                    fingCoord = data[45].Split('/');
                    pos = new Vector3();
                    pos.x = (float)Double.Parse(fingCoord[0]);
                    pos.x /= 100;
                    pos.y = (float)Double.Parse(fingCoord[1]);
                    pos.y /= 100;
                    pos.z = (float)Double.Parse(fingCoord[2]);
                    pos.z /= -100;

                    rHand.transform.Find("PinkyD").localPosition = new Vector3(pos.x, pos.y, pos.z);
                    lRDist.SetPosition(0, rHand.transform.Find("PinkyD").position);

                    /*Link all meta except thumb meta and palmPinkyMeta*/
                    lRendererPinky.SetPosition(1, lRendererRing.GetPosition(0));
                    lRendererRing.SetPosition(1, lRendererMiddle.GetPosition(0));
                    lRendererMiddle.SetPosition(1, lRendererIndex.GetPosition(0));
                    lRendererIndex.SetPosition(1, lRendererThumb.GetPosition(0));
                }
                /*
                String[] fingCoord = data[1].Split('/');
                Vector3 pos = new Vector3();
                pos.x = (float)Double.Parse(fingCoord[0]);
                pos.x /= 100;
                pos.y = (float)Double.Parse(fingCoord[1]);
                pos.y /= 100;
                pos.z = (float)Double.Parse(fingCoord[2]);
                pos.z /= -100;

                //Palm.transform.position = pos;
                //lHand.transform.position = pos;

                lHand.transform.localPosition = new Vector3(0,-camDist,1);


                Palm.transform.localPosition = new Vector3(pos.x, pos.y, pos.z);

                fingCoord = data[2].Split('/');
                pos = new Vector3();
                pos.x = (float) Double.Parse(fingCoord[0]);
                pos.x /= 100;
                pos.y = (float) Double.Parse(fingCoord[1]);
                pos.y /= 100;
                pos.z = (float)Double.Parse(fingCoord[2]);
                pos.z /= -100;

                //thumb.transform.position = pos;
                thumb.transform.localPosition = new Vector3(pos.x, pos.y, pos.z);


                fingCoord = data[3].Split('/');
                pos = new Vector3();
                pos.x = (float)Double.Parse(fingCoord[0]);
                pos.x /= 100;
                pos.y = (float)Double.Parse(fingCoord[1]);
                pos.y /= 100;
                pos.z = (float)Double.Parse(fingCoord[2]);
                pos.z /= -100;

                //index.transform.position = pos;
                index.transform.localPosition = new Vector3( pos.x, pos.y , pos.z);


                fingCoord = data[4].Split('/');
                pos = new Vector3();
                pos.x = (float)Double.Parse(fingCoord[0]);
                pos.x /= 100;
                pos.y = (float)Double.Parse(fingCoord[1]);
                pos.y /= 100;
                pos.z = (float)Double.Parse(fingCoord[2]);
                pos.z /= -100;

                //middle.transform.position = pos;
                middle.transform.localPosition = new Vector3( pos.x,  pos.y , pos.z);

                fingCoord = data[5].Split('/');
                pos = new Vector3();
                pos.x = (float)Double.Parse(fingCoord[0]);
                pos.x /= 100;
                pos.y = (float)Double.Parse(fingCoord[1]);
                pos.y /= 100;
                pos.z = (float)Double.Parse(fingCoord[2]);
                pos.z /= -100;

                //ring.transform.position = pos;
                ring.transform.localPosition = new Vector3(pos.x, pos.y ,  pos.z );


                fingCoord = data[6].Split('/');
                pos = new Vector3();
                pos.x = (float)Double.Parse(fingCoord[0]);
                pos.x /= 100;
                pos.y = (float)Double.Parse(fingCoord[1]);
                pos.y /= 100;
                pos.z = (float)Double.Parse(fingCoord[2]);
                pos.z /= -100;

                //pinky.transform.position = pos;
                pinky.transform.localPosition = new Vector3( pos.x, pos.y , pos.z );


                if (data.Length > 7) {
                    String isClosedR = data[7];
                    if (isClosedR == "1") rHand.GetComponent<Hand>().Closed = true;
                    else rHand.GetComponent<Hand>().Closed = false;
                    fingCoord = data[8].Split('/');
                    pos = new Vector3();

                    pos.x = (float)Double.Parse(fingCoord[0]);
                    pos.x /= 100;
                    pos.y = (float)Double.Parse(fingCoord[1]);
                    pos.y /= 100;
                    pos.z = (float)Double.Parse(fingCoord[2]);
                    pos.z /= -100;
                
                    //rHand.transform.position = cam.transform.position + pos;
                    //rHand.transform.position = new Vector3(cam.transform.position.x + pos.x, cam.transform.position.y + pos.y - camDist, cam.transform.position.z + pos.z + camDist);
                    rHand.transform.localPosition = new Vector3(0, -camDist, 1);
                    PalmR.transform.localPosition = pos;

                    fingCoord = data[9].Split('/');
                    pos = new Vector3();
                    pos.x = (float)Double.Parse(fingCoord[0]);
                    pos.x /= 100;
                    pos.y = (float)Double.Parse(fingCoord[1]);
                    pos.y /= 100;
                    pos.z = (float)Double.Parse(fingCoord[2]);
                    pos.z /= -100;

                    //thumbR.transform.position = pos;
                    //thumbR.transform.position = new Vector3(cam.transform.position.x + pos.x, cam.transform.position.y + pos.y - camDist, cam.transform.position.z + pos.z + camDist);
                    thumbR.transform.localPosition = new Vector3(pos.x, pos.y, pos.z);

                    fingCoord = data[10].Split('/');
                    pos = new Vector3();
                    pos.x = (float)Double.Parse(fingCoord[0]);
                    pos.x /= 100;
                    pos.y = (float)Double.Parse(fingCoord[1]);
                    pos.y /= 100;
                    pos.z = (float)Double.Parse(fingCoord[2]);
                    pos.z /= -100;

                    //indexR.transform.position = pos;
                    //indexR.transform.position = new Vector3(cam.transform.position.x + pos.x, cam.transform.position.y + pos.y - camDist, cam.transform.position.z + pos.z + camDist);
                    indexR.transform.localPosition = new Vector3(pos.x, pos.y, pos.z);
                    fingCoord = data[11].Split('/');
                    pos = new Vector3();
                    pos.x = (float)Double.Parse(fingCoord[0]);
                    pos.x /= 100;
                    pos.y = (float)Double.Parse(fingCoord[1]);
                    pos.y /= 100;
                    pos.z = (float)Double.Parse(fingCoord[2]);
                    pos.z /= -100;

                    //middleR.transform.position = pos;
                    //middleR.transform.position = new Vector3(cam.transform.position.x + pos.x, cam.transform.position.y + pos.y - camDist, cam.transform.position.z + pos.z + camDist);
                    middleR.transform.localPosition = new Vector3(pos.x, pos.y, pos.z);


                    fingCoord = data[12].Split('/');
                    pos = new Vector3();
                    pos.x = (float)Double.Parse(fingCoord[0]);
                    pos.x /= 100;
                    pos.y = (float)Double.Parse(fingCoord[1]);
                    pos.y /= 100;
                    pos.z = (float)Double.Parse(fingCoord[2]);
                    pos.z /= -100;


                    //ringR.transform.position = pos;
                    //ringR.transform.position = new Vector3(cam.transform.position.x + pos.x, cam.transform.position.y + pos.y - camDist, cam.transform.position.z + pos.z + camDist);
                    ringR.transform.localPosition = new Vector3(pos.x, pos.y, pos.z);


                    fingCoord = data[13].Split('/');
                    pos = new Vector3();
                    pos.x = (float)Double.Parse(fingCoord[0]);
                    pos.x /= 100;
                    pos.y = (float)Double.Parse(fingCoord[1]);
                    pos.y /= 100;
                    pos.z = (float)Double.Parse(fingCoord[2]);
                    pos.z /= -100;

                    //pinkyR.transform.position = pos;
                    //pinkyR.transform.position = new Vector3(cam.transform.position.x + pos.x, cam.transform.position.y + pos.y - camDist, cam.transform.position.z + pos.z + camDist);
                    pinkyR.transform.localPosition = new Vector3(pos.x, pos.y, pos.z);
                    */


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

}
