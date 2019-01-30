using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuHandler : MonoBehaviour
{
    public Dropdown m_Files;
    public Dropdown m_Controller;
    private InputField m_ServerNameText;

    private void Start()
    {
        m_ServerNameText = GameObject.Find("ServerInput").GetComponent<InputField>();
        Cursor.visible = true;
    }

    public void OnPlayClick()
    {
        if (m_ServerNameText.text != "") JoinServer();
        else HostServer();
        Const.Controller = m_Controller.value;
        Const.m_FileNameControlPoint = "Assets/SaveDataBSpline/" + m_Files.options[m_Files.value].text;
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }
    /*
     * User host his server, hasToken is set because he is the first user.
     * ip and port is needed
     **/
    public void HostServer() {
        Debug.Log("Hosting server on port 5000");
        //We wait the server before logging in
        StartCoroutine(TCPController.StartServer());
        TCPController.isHosting = true;
        TCPController.Ip = "127.0.0.1";
        TCPController.Port = 25128;
        TCPController.userId = 1;
        TCPController.hasToken = true;
        TCPController.launch = true;

    }

    /*
     * User join other server, hasToken need to be set to false, he is an observer
     * ip and port is needed
     * */
    public void JoinServer() {
        Debug.Log("Joining server");
        string[] data = m_ServerNameText.text.Split(':');
        if (data.Length == 2)
        {
            Debug.Log("Adresse " + data[0] + " port :" + int.Parse(data[1]));
            TCPController.isHosting = false;
            TCPController.Ip = data[0];
            TCPController.Port = int.Parse(data[1]);
            TCPController.UserId = -1;
            Debug.Log("Server at " + TCPController.Ip + ":" + TCPController.Port);
            TCPController.launch = true;
        }
        else
        {
            Debug.Log("Arguments not taken : 2 is needed {ip:port}");
        }
    }
    private void OnDestroy()
    {
        if (!TCPController.launch && !TCPController.myTCP.socketReady && TCPController.isHosting) TCPController.process.Kill();
    }

}
