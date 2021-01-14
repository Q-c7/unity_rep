using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveByKeyboard : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Upper Crane"))
            Debug.Log("Enter");
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Upper Crane"))
            Debug.Log("Exit");
    }

    void FixedUpdate()
    {
        gameObject.transform.position += new Vector3(Input.GetAxis("Horizontal")*0.05f, 0);
    }
}
