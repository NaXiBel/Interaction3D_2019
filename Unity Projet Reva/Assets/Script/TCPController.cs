using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TCPController : MonoBehaviour {

    public static TCPConnection myTCP;
    private bool connecting = true;
    public  static bool launch = true;
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
                    int periph = Int32.Parse(data[0]);

                //              DontDestroyOnLoad(this.can);
                //      SceneManager.LoadScene(1);
                launch = false;
                SceneManager.LoadScene(periph);
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
}
