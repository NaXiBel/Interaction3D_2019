using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class Const{

    private static int m_Controller;
    private static string m_ServerName;

    public enum ControllerName {Oculus, LeapMotion};



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
