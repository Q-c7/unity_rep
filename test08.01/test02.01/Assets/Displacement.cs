using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Displacement : MonoBehaviour
{
    // Start is called before the first frame update
    

    // Update is called once per frame
    void Update()
    {
        Vector3 position = transform.position;
        float horizontalInput = Input.GetAxis("Horizontal");

        if (Input.GetAxis("Horizontal") != null)
        {
            position.z += horizontalInput * Time.deltaTime;
            transform.position = position;
        }
    }
}
