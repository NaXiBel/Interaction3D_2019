﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserScrollViewIhm : MonoBehaviour {
    public GameObject m_ScrollVIew;
    public GameObject m_Panel;
    private Dictionary<int, string> m_UserNames; // Remplace par ton tableau 
    // Use this for initialization
    void Start () {
        m_UserNames = new Dictionary<int, string>();
        m_UserNames.Add(1, "User 1"); // TEST 
        m_UserNames.Add(2, "User 2"); // TEST
        m_UserNames.Add(3, "User 3"); // TEST
        int cpt = 0;
        foreach (int id in m_UserNames.Keys)
        {
            GameObject newUser = (GameObject)Instantiate(Resources.Load("Prefabs/BT_Username"), Vector3.zero, Quaternion.identity);
            Button newUserButton = newUser.GetComponent<Button>();

            newUserButton.GetComponentInChildren<RectTransform>().SetParent(m_ScrollVIew.transform, false);

            newUserButton.GetComponentInChildren<RectTransform>().anchoredPosition = new Vector2(-122f, 88f - 60f * cpt);
            // active c'est ligne avec la bonne condition, pour que le maître soit activé d'un couleur différente. 
/*            if (isMaitre)
            {
                ColorBlock cb = newUserButton.colors;
                cb.normalColor = Color.green;
                newUserButton.colors = cb;

            }
*/

            newUserButton.GetComponentInChildren<Text>().text = m_UserNames[id];
            newUser.GetComponent<UserButton>().Id = id;
            newUser.GetComponent<UserButton>().Username = m_UserNames[id];
            ++cpt;
        }
    }
	
	// Update is called once per frame
	void Update () {
       
    }
}