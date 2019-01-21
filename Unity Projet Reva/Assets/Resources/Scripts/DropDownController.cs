using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropDownController : MonoBehaviour {

	void Start()
    {
        
        this.GetComponent<Dropdown>().options.Clear();
        DropDown dr = this.GetComponent<DropDown>();
        this.GetComponent<Dropdown>().options.Add(new Dropdown.OptionData() { text = "Oculus Touch" });
        this.GetComponent<Dropdown>().options.Add(new Dropdown.OptionData() { text = "Leap Motion " });
        
    }

}
