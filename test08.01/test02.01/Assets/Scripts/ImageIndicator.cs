using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageIndicator : MonoBehaviour
{
    // Start is called before the first frame update
    private Image indicator;
    public Sprite gray;
    public Sprite green;
    public Sprite yellow;
    public Sprite red;
    public bool good;
    public bool warning;
    public bool alarm;
    public int enters = 0;
    public int exits = 0;
    public string objectName;
    public ColliderTargetAndType type;

    void Start()
    {
        indicator = gameObject.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if(indicator != null)
        {
            if(!good && !warning && !alarm)
            {
                if(gray != null)
                {
                    indicator.sprite = gray;
                }
            }
            if(good && !warning && !alarm)
            {
                if(green != null)
                {
                    indicator.sprite = green;
                }
            }
            if(warning && !alarm)
            {
                if(yellow != null)
                {
                    indicator.sprite = yellow;
                }
            }
            if (alarm)
            {
                if(red != null)
                {
                    indicator.sprite = red;
                }
            }
        }
    }
}
