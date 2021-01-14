using System.Collections;  
using System.Collections.Generic;  
using UnityEngine;


public class Trigger1 : MonoBehaviour
{


    private void OnTriggerEnter(Collider other)
    {
        trigger.trigger1 = true;
    }
   
}
