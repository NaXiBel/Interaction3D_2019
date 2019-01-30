using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TCPController : MonoBehaviour {

    public static TCPConnection myTCP;
    public static bool hasToken;
    public static int userId;
    public static string ip;
    public static int port;
    private bool connecting = true;
    public static bool launch = false;
    public static System.Diagnostics.Process process;
    public static int userHasToken;
    public static bool isIdObtained = false;
    public static bool isHosting = false;
    //public Canvas can;
    void Awake()
    {
        //add a copy of TCPConnection to this game object
        Debug.Log("Awake");
        myTCP = gameObject.AddComponent<TCPConnection>();
       // myTCP.socketReady = false;
    }
    public void Start()
    {
        DontDestroyOnLoad(this.gameObject);

    }

    public static void Connection(GameObject gameObject)
    {

       // Destroy(gameObject.GetComponent<TCPConnection>());

        if (launch)
            try
            {
               
                if (myTCP.socketReady == false)
                {
                    Debug.Log("Attempting to connect..");
                    myTCP.setupSocket();
                    Debug.Log("connected");
                }
                launch = false;
                /*string serverSays = myTCP.readSocket();
                String[] data;
                Debug.Log("serverSays " + serverSays);
                Debug.Log("launch " + launch);
                if (serverSays != "")

                {
                    data = serverSays.Split(';');
                    if (data[0] == "0")
                    {
                        if (!TCPController.isIdObtained)
                        {
                            //First update : 0;id;posx;posy;posz
                            TCPController.UserId = Int32.Parse(data[1]);
                            Debug.Log("Id has been obtained");
                            TCPController.isIdObtained = true;
                            TheController.usersList.Add(TCPController.UserId, null);

                        }
                        //data[2],data[3],data[4] are positions
                        //data[5] = id1,id2,id3,id4
                        string[] ids = data[5].Split(',');
                        Debug.Log("ids length : " + ids.Length + "ids : " + ids);
                        for (int j = 0; j < ids.Length; ++j)
                        {
                            Debug.Log("ids[j] : " + ids[j]);
                            int parsedId = Int32.Parse(ids[j]);
                            if (TCPController.UserId != parsedId)
                            {
                                //Si c'est pas le meme
                                Debug.Log("About to create User : " + parsedId);
                                if (!TheController.usersList.ContainsKey(parsedId))
                                {
                                    Debug.Log("Create User : " + parsedId);
                                    GameObject tmp = (GameObject)Instantiate(Resources.Load("Prefabs/User"));
                                    TheController.usersList.Add(parsedId, tmp);
                                    Debug.Log("GameObject tmp :" + tmp);
                                }
                            }
                        }
                        UserScrollViewIhm.UpdateUserList();
                        
                    }
                    TCPController.launch = false;
                    Debug.Log("launch 2 " + launch);
                }*/
            }
            catch (Exception e)
            {
                Debug.Log(e);

            }

    }
    // Update is called once per frame
    void Update() {
        Connection(this.gameObject);
    }

    private void OnDestroy()
    {
        myTCP.closeSocket();
        if(TCPController.isHosting)process.Kill();
    }

    public static IEnumerator StartServer()
    {
        process = new System.Diagnostics.Process();
        System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
        //DEBUG startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
        startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
        startInfo.FileName = Application.dataPath + "/server.exe";
        Debug.Log(Application.dataPath + "/server.exe");
        process.StartInfo = startInfo;
        process.Start();
        yield return new WaitForSeconds(10);
    }
    public static int UserId {
        get { return userId; }
        set { userId = value;}
    }
    public static string Ip {
        get { return ip; }
        set {
            ip = value;
            myTCP.conHost = value;
        }
    }
    public static int Port {
        get { return port; }
        set {
            port = value;
            myTCP.conPort = value;
        }
    }
    public static void RequestToken()
    {
        if (userId > -1) {
            Debug.Log("Request Token");
            string update = "1;" + userId;
            myTCP.writeSocket(update);
        }
    }
    public static void ReturnToken()
    {
        if(userId > -1)
        {
            Debug.Log("Return Token");
            string update = "2;" + userId;
            myTCP.writeSocket(update);
        }
    }
    public static void AskSummary()
    {
        if (userId > -1)
        {
            Debug.Log("Ask Summary");
            string update = "5;" + userId;
            myTCP.writeSocket(update);
        }
    }
}
