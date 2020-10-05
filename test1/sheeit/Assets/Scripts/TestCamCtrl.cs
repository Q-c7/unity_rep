using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using UnityEngine;

public class TestCamCtrl : MonoBehaviour
{
    private Transform player;
    public Transform cameraman;
    public SpawnMgr spawnManager;

    //public Vector3 offset = new Vector3(0, 0, 0);
    //public bool AlreadyRotated = false;

    private float yTurn = -90;

    // Start is called before the first frame update
    void Start()
    {
        cameraman = GameObject.Find("Main Camera").transform;
        player = GameObject.Find("Player").transform;

        //offset.x = 0f;
       //offset.y = 2f;
        //offset.z = -5f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (transform != null)
            transform.position = new Vector3(player.position.x + spawnManager.offset.x, player.position.y + spawnManager.offset.y, player.position.z + spawnManager.offset.z);
    }

    /*
    public void RotateCam()
    {
        //Quaternion rot = cameraman.rotation;
        if (AlreadyRotated)
            return;
        cameraman.Rotate(0, yTurn, 0);
        //offset = Quaternion.Euler(0, -90, 0) * offset;
        //offset.x = -offset.z;
        //offset.z = offset.x;

        AlreadyRotated = true;
        //cameraman.rotation = new Quaternion(rot.x, rot.y + yTurn, rot.z, rot.w);
    }*/
    
}
