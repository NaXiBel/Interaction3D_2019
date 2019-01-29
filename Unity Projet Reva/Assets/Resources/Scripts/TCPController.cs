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
                string serverSays = myTCP.readSocket();
                String[] data;
                Debug.Log("serverSays " + serverSays);
                if (serverSays != "")
                {
                    //            Debug.Log("[SERVER]" + serverSays);
                    data = serverSays.Split(';');
                    //int periph = Int32.Parse(data[0]);


                //              DontDestroyOnLoad(this.can);
                //      SceneManager.LoadScene(1);
                launch = false;
                //SceneManager.LoadScene(periph);
                }
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
        //DEBUG process.Kill();
    }

    public static IEnumerator StartServer()
    {
        process = new System.Diagnostics.Process();
        System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
        // DEBUG startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
        startInfo.FileName = Application.dataPath + "/server.exe";
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
}
