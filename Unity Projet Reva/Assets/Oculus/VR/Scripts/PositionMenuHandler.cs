using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PositionMenuHandler : MonoBehaviour {

    public Text m_X;
    public Text m_Y;
    public Text m_Z;
    public Text m_Step;
    public float m_Sensibility = 0.1f;

    public void Initialize()
    {
        m_X.text = Const.m_ControlPoints[0].transform.position.x.ToString();
        m_Y.text = Const.m_ControlPoints[0].transform.position.y.ToString();
        m_Z.text = Const.m_ControlPoints[0].transform.position.z.ToString();
    }

    public void CloseClick()
    {
        this.GetComponent<Canvas>().enabled = false;
    }


    public void DecreaseStep()
    {
        float convert;
        float.TryParse(m_Step.text, out convert);
        if (m_Sensibility > 0.00001f)
        {
            m_Sensibility = convert / 10f;
            m_Step.text = m_Sensibility.ToString();
        }
    }

    public void IncreaseStep()
    {
        float convert;
        float.TryParse(m_Step.text, out convert);
        if (m_Sensibility < 100000)
        {
            m_Sensibility = convert * 10f;
            m_Step.text = m_Sensibility.ToString();
        }

    }


    public void DecreaseX()
    {
        Const.m_ControlPoints[0].transform.Translate(new Vector3(-m_Sensibility, 0, 0));
        m_X.text = Const.m_ControlPoints[0].transform.position.x.ToString();
    }

    public void IncreaseX()
    {
        Const.m_ControlPoints[0].transform.Translate(new Vector3(m_Sensibility, 0, 0));
        m_X.text = Const.m_ControlPoints[0].transform.position.x.ToString();
    }


    public void DecreaseY()
    {
        Const.m_ControlPoints[0].transform.Translate(new Vector3(0, -m_Sensibility, 0));
        m_Y.text = Const.m_ControlPoints[0].transform.position.y.ToString();
    }

    public void IncreaseY()
    {
        Const.m_ControlPoints[0].transform.Translate(new Vector3(0, m_Sensibility, 0));
        m_Y.text = Const.m_ControlPoints[0].transform.position.y.ToString();
    }

    public void DecreaseZ()
    {
        Const.m_ControlPoints[0].transform.Translate(new Vector3(0, 0, -m_Sensibility));
        m_Z.text = Const.m_ControlPoints[0].transform.position.z.ToString();
    }

    public void IncreaseZ()
    {
        Const.m_ControlPoints[0].transform.Translate(new Vector3(0, 0, m_Sensibility));
        m_Z.text = Const.m_ControlPoints[0].transform.position.z.ToString();
    }


}
