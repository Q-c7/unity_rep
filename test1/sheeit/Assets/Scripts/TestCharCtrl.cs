using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;

public class TestCharCtrl : MonoBehaviour
{
    public float char_ms = 10f;
    public SpawnMgr spawnManager;
    //public Vector3 force = new Vector3(5, 0, 0);

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        float hMovement = Input.GetAxis("Horizontal") * char_ms + spawnManager.force.x;
        float vMovement = Input.GetAxis("Vertical") * char_ms + spawnManager.force.z;

        transform.Translate(new Vector3(hMovement, 0, vMovement) * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        spawnManager.SpawnTriggerEntered();
    }


    private void OnTriggerExit(Collider other)
    {
        spawnManager.SpawnTriggerLeft();
    }
}
