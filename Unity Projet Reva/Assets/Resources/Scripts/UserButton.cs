using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserButton : MonoBehaviour {
    private int m_Id; 
    private string m_Username;


    public void Awake()
    {
        this.GetComponent<Button>().onClick.AddListener((() => OnClickGetToken()));
    }
    public void OnClickGetToken()
    {
        Debug.Log("Button user click ");
        // envoie le token avec l'id de ce bouton
        // fait tout ce qu'il faut pour changer le possesseur du token dans cette fonction
    }
    public int Id
    {
        get
        {
            return m_Id;
        }
        set
        {
            m_Id = value;
        }
    }

    public string Username
    {
        get
        {
            return m_Username;
        }
        set
        {
            m_Username = value;
        }
    }
}
