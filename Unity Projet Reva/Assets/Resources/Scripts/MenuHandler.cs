using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuHandler : MonoBehaviour
{

    public void LeapMotionClick()
    {
        Const.Controller = (int) Const.ControllerName.LeapMotion;
        
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }
    public void OculusClick()
    {
        Const.Controller = (int)Const.ControllerName.Oculus;
        SceneManager.LoadScene(1, LoadSceneMode.Single);

    }
}
