using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class DropDown : MonoBehaviour {

	// Use this for initialization
	void Start () {
        char[] delimiterChars = { '/', '\\' };
        this.GetComponent<Dropdown>().options.Clear();
        DropDown dr = this.GetComponent<DropDown>();
        foreach (string c in getFiles())
        {
            this.GetComponent<Dropdown>().options.Add(new Dropdown.OptionData() { text = System.IO.Path.GetFileName(c) });
        }
    }
    private string[] getFiles()
    {
        string path = "Assets/SaveDataBSpline/";
        string[] filePaths = Directory.GetFiles(@path, "*.pts");
        foreach (string file in filePaths)
        {
            Debug.Log(file);
        }
        return filePaths;
    }
}
