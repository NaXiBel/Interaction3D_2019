using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuHandler : MonoBehaviour
{
    public Dropdown m_Files;
    public Dropdown m_Controller;

    public void OnPlayClick()
    {

        Const.Controller = m_Controller.value;
        Const.m_FileNameControlPoint = "Assets/SaveDataBSpline/" + m_Files.options[m_Files.value].text;
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }
    /*
     * User host his server, hasToken is set because he is the first user.
     * ip and port is needed
     **/
    public void HostServer() {

    }

    /*
     * User join other server, hasToken need to be set to false, he is an observer
     * ip and port is needed
     * */
    public void JoinServer() {

    }
}
