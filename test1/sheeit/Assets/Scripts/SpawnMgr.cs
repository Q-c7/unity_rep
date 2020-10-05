using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class SpawnMgr : MonoBehaviour
{
    RoadSpawner roadSpawner;
    TestCamCtrl cam;

    public bool AlreadyRotated = false;

    private float yTurn = -90;
    public Vector3 offset = new Vector3(0, 0.45f, 0);
    public Vector3 force = new Vector3(0, 0, 5);

    // Start is called before the first frame update
    void Start()
    {
        roadSpawner = GetComponent<RoadSpawner>();
        cam = GetComponent<TestCamCtrl>();

        offset.x = 0f;
        //offset.y = 2f;
        //offset.z = -5f;
    }

    // Update is called once per frame

    public void RotateCam()
    {
        //if (AlreadyRotated)
        //    return;
        cam.cameraman.Rotate(0, yTurn, 0);
        offset = Quaternion.Euler(0, -90, 0) * offset;
        force = Quaternion.Euler(0, -90, 0) * force;
        AlreadyRotated = true;
    }

    public void RotateForce()
    {


    }

    public void SpawnTriggerEntered()
    {
        RotateCam();
        roadSpawner.MoveRoad();
    }

    public void SpawnTriggerLeft()
    {
        AlreadyRotated = false;
        roadSpawner.AlreadySpawned = false;
    }


}
