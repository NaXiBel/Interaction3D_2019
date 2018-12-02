using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point : MonoBehaviour
{
    // Use this for initialization

    private GameObject m_TextCoord = null;
    
    void Start()
    {
        this.m_TextCoord = new GameObject();
        this.m_TextCoord.AddComponent<TextMesh>();
        TextMesh pointTextMesh = this.m_TextCoord.GetComponent<TextMesh>();
        pointTextMesh.fontSize = 4;
        pointTextMesh.anchor = TextAnchor.LowerCenter;

        this.m_TextCoord.transform.parent = this.transform;
        this.m_TextCoord.transform.localPosition = Vector3.zero;
        this.m_TextCoord.SetActive(false);


    }

    // Update is called once per frame
    void Update()
    {   
        Vector3 displayedPosition = this.transform.position;
        this.m_TextCoord.GetComponent<TextMesh>().text = "(" + displayedPosition.x + ", " + displayedPosition.y + ", " + displayedPosition.z + ")";
    }
}
