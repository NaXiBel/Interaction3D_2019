using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class ContextualHandler : MonoBehaviour {
    public GameObject m_ContextualMenu;
    public GameObject m_Controller;
    public GameObject m_Bspline;
    private bool m_IsWireframe = true;
    public void ResetClick()
    {
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }

    public void SaveClick()
    {
        m_Bspline = GameObject.Find("Bspline");
        FileStream file = File.Open("Assets/SaveDataBSpline/B_Spline_" +System.DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss") + ".pts", FileMode.OpenOrCreate, FileAccess.ReadWrite);
   
        StreamWriter sr = new StreamWriter(file);
        string write = "";
        // x
        for (int i = 0; i < Const.m_NumberControlPoints; ++i)
        {
            write += m_Bspline.GetComponent<Bspline>().xcontr[i] + " ";
        }
        sr.WriteLine(write);
        write = "";
        // y
        for (int i = 0; i < Const.m_NumberControlPoints; ++i)
        {
            write += m_Bspline.GetComponent<Bspline>().ycontr[i] + " ";
        }
        sr.WriteLine(write);
        write = "";
        // z
        for (int i = 0; i < Const.m_NumberControlPoints; ++i)
        {
            write += m_Bspline.GetComponent<Bspline>().zcontr[i] + " ";
        }
        sr.WriteLine(write);
        write = "";
        sr.Close();
    }

    public void CloseClick()
    {
        m_ContextualMenu.GetComponent<Canvas>().enabled = false;
    }

    public void AlignX()
    {
        float averageY = 0.0f;
        float averageZ = 0.0f;

        foreach (GameObject point in Const.m_ControlPoints)
        {
            averageY += point.transform.position.y;
            averageZ += point.transform.position.z;
        }
        averageY = averageY / Const.m_ControlPoints.Count;
        averageZ = averageZ / Const.m_ControlPoints.Count;

        foreach (GameObject point in Const.m_ControlPoints)
        {
            point.transform.position = new Vector3(point.transform.position.x, averageY, averageZ);
        }
    }
    public void AlignY()
    {
        float averageX = 0.0f;
        float averageZ = 0.0f;

        foreach (GameObject point in Const.m_ControlPoints)
        {
            averageX += point.transform.position.x;
            averageZ += point.transform.position.z;
        }
        averageX = averageX / Const.m_ControlPoints.Count;
        averageZ = averageZ / Const.m_ControlPoints.Count;

        foreach (GameObject point in Const.m_ControlPoints)
        {
            point.transform.position = new Vector3(averageX, point.transform.position.y, averageZ);
        }
    }
    public void AlignZ()
    {
        float averageX = 0.0f;
        float averageY = 0.0f;

        foreach (GameObject point in Const.m_ControlPoints)
        {
            averageX += point.transform.position.x;
            averageY += point.transform.position.y;
        }
        averageX = averageX / Const.m_ControlPoints.Count;
        averageY = averageY / Const.m_ControlPoints.Count;

        foreach (GameObject point in Const.m_ControlPoints)
        {
            point.transform.position = new Vector3(averageX, averageY, point.transform.position.z);
        }
    }
    public void Bound()
    {

    }


    public void RemoveControlePoints()
    {
        foreach (GameObject point in Const.m_ControlPoints)
        {
            m_Controller.GetComponent<TheController>().RemovePoint(point.GetComponent<Point>().Indice);
            --Const.m_NumberControlPoints;
            Debug.Log("Remove : " + Const.m_NumberControlPoints);
            Destroy(point);
        }
    }

    public void WireFrame()
    {

        m_Bspline = GameObject.Find("Bspline");
        if (m_IsWireframe)
        {
            m_Bspline.GetComponent<Renderer>().material = Resources.Load("Materials/Wireframe", typeof(Material)) as Material;
            m_IsWireframe = false;
        } else
        {
            m_Bspline.GetComponent<Renderer>().material = Resources.Load("Materials/B_Spline", typeof(Material)) as Material;
            m_IsWireframe = true;
        }
    }
}
