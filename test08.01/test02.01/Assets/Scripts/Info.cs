using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Info : MonoBehaviour
{
    public Crane crane;
    public Text bridgePosition;
    public Text mtPosition;
    public Text atPosition;
    public Text mtMhPosition;
    public Text mtAhPosition;
    public Text atMhPosition;
    public Text atAhPosition;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(crane != null)
        {
            if(bridgePosition != null)
            {
                bridgePosition.text = (crane.bridgePosition / 1000.0).ToString("F3") + " м";
            }
            if(mtPosition != null)
            {
                mtPosition.text = (crane.mtPosition / 1000.0).ToString("F3") + " м";
            }
            if (mtMhPosition != null)
            {
                mtMhPosition.text = (crane.mtMhPosition / 1000.0).ToString("F3") + " м";
            }
            if (mtAhPosition != null)
            {
                mtAhPosition.text = (crane.mtAhPosition / 1000.0).ToString("F3") + " м";
            }
            if (atPosition != null)
            {
                atPosition.text = (crane.atPosition / 1000.0).ToString("F3") + " м";
            }
            if (atMhPosition != null)
            {
                atMhPosition.text = (crane.atMhPosition / 1000.0).ToString("F3") + " м";
            }
            if (atAhPosition != null)
            {
                atAhPosition.text = (crane.atAhPosition / 1000.0).ToString("F3") + " м";
            }
        }
    }
}
