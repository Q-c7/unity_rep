using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlarmItem : MonoBehaviour
{
    public System.DateTime time = System.DateTime.Now;
    public string message = "";
    // Start is called before the first frame update
    public Text timeLabel;
    public Text messageLabel;
    void Start()
    {
        //time = new System.DateTime();
    }

    // Update is called once per frame
    void Update()
    {
        if (timeLabel != null)
        {
            timeLabel.text = time.ToString();
        }
        if(messageLabel != null)
        {
            messageLabel.text = message;
        }
    }
}
