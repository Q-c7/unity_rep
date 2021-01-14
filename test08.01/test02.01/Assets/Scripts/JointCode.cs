using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class trigger
{
    public static bool trigger1;
    public static bool trigger2;

}

public class JointCode : MonoBehaviour
{
    public GameObject container;
    public Rigidbody spreder;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    { 

        if (trigger.trigger1 && trigger.trigger2)
        {
            print("A");
            if (Input.GetKey(KeyCode.L))
            {
                container.GetComponent<FixedJoint>().connectedBody = spreder;
                print("Connected");
            
            }
        
            }


    }
}
