using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Prefix : MonoBehaviour
{
    public Text prefixText;
    // Start is called before the first frame update
    void Start()
    {
        if(prefixText != null)
            prefixText.text = Storage.GetPrefix();
        if (prefixText.text == "")
            prefixText.text = "Пролет 1";

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
