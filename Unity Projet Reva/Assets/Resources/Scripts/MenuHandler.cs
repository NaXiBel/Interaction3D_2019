using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuHandler : MonoBehaviour
{
    public Dropdown m_Files;
    
    public void LeapMotionClick()
    {

        Const.Controller = (int) Const.ControllerName.LeapMotion;
        Const.m_FileNameControlPoint = "Assets/SaveDataBSpline/" + m_Files.options[m_Files.value].text;
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }
    public void OculusClick()
    {
        Const.Controller = (int)Const.ControllerName.Oculus;
        Const.m_FileNameControlPoint = "Assets/SaveDataBSpline/" + m_Files.options[m_Files.value].text;
        SceneManager.LoadScene(1, LoadSceneMode.Single);

    }
}
