using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SizeTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(gameObject.transform.localScale);
        Debug.Log(gameObject.transform.lossyScale);
        Debug.Log(gameObject.transform.localPosition);
        Debug.Log(gameObject.transform.position);
    }
}
