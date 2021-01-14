using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using UnityEngine;

public class rot : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.R))
        {
            Vector3 vec = new Vector3 (5.6f, -6.04f, 21.17f);
            transform.localPosition = vec;
            transform.localRotation = Quaternion.Euler(-90f, -180f, -90f);
        }
    }
}
