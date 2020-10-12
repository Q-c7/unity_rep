using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Random = UnityEngine.Random;
using UnityEngine;
using System.Collections.Specialized;

public class RoadSpawner : MonoBehaviour
{
    public List<GameObject> roads;
    //private float offset = 50;

    public bool AlreadySpawned = false;

    public List<GameObject> roads_spwn;

    // Start is called before the first frame update
    void Start()
    {
        if (roads != null && roads.Count > 0)
        {
            //roads = roads.OrderByDescending(r => r.transform.rotation.y).ToList(); //OrderBy
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    Vector3 next_pos (Vector3 pos)
    {
        float new_x;
        float new_z;

        if (pos.x == 0)
        {
            new_z = 50;
            if (pos.z == 0)
                new_x = 0;
            else
                new_x = -50;
        }
        else
        {
            new_z = 0;
            if (pos.z == 0)
                new_x = 0;
            else
                new_x = -50;
        }

        return new Vector3(new_x, 0, new_z);
    }

    public void MoveRoad()
    {
        if (AlreadySpawned)
            return;

        GameObject deletedRoad = roads[0];
        GameObject currentRoad = roads[1]; //KOSTYL
        Vector3 pos = currentRoad.transform.position;
        Quaternion rot = currentRoad.transform.rotation;

        roads.Remove(deletedRoad);
        //float newZ = roads[roads.Count - 1].transform.position.z + offset;
        //movedRoad.transform.position = new Vector3(0, 0, newZ);
        GameObject newRoad = UnityEngine.Object.Instantiate(roads_spwn[Random.Range(0, roads_spwn.Count)]);
        newRoad.transform.position = next_pos(pos);
        newRoad.transform.rotation = rot * Quaternion.Euler(Vector3.up * -90);
        roads.Add(newRoad);
        UnityEngine.Object.Destroy(deletedRoad);
    }
}
