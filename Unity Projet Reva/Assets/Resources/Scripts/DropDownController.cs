using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropDownController : MonoBehaviour {

	void Awake()
    {
        
        this.GetComponent<Dropdown>().options.Clear();
        this.GetComponent<Dropdown>().options.Add(new Dropdown.OptionData() { text = "Oculus Touch" });
        this.GetComponent<Dropdown>().options.Add(new Dropdown.OptionData() { text = "Leap Motion " });
        
    }

}
