using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Const{

    private static int m_Controller;
    private static string m_ServerName;
    public enum ControllerName {Oculus, LeapMotion};
    public static List<GameObject> m_ControlPoints = new List<GameObject>();
    public static int m_NumberControlPoints = 25; // change
    public static string m_FileNameControlPoint = "Assets/SaveDataBSpline/points.pts"; 

    public static int Controller
    {
        get
        {
            return m_Controller;
        }
        set
        {
            m_Controller = value;
        }
    }

    public static string ServerName
    {
        get
        {
            return m_ServerName;
        }
        set
        {
            m_ServerName = value;
        }
    }

}
