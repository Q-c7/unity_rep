using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger2 : MonoBehaviour
{


    private void OnTriggerEnter(Collider other)
    {
        trigger.trigger2 = true;
    }
    
}