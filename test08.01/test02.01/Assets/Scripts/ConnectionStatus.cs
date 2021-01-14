using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionStatus : MonoBehaviour
{
    public CraneConnection connection;
    public Sprite good;
    public Sprite bad;
    public bool connected;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var imgContainer = gameObject.GetComponent<Image>();
        if(connection != null && good != null && bad != null && imgContainer != null)
        {
            connected = connection.connected;
            if (connection.connected)
            {
                imgContainer.sprite = good;
            }
            else
            {
                imgContainer.sprite = bad;
            }
        }
    }
}
