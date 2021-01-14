using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ObstaclesLoader : MonoBehaviour
{
    public Vector3 zeroPoint;
    public GameObject obstaclePrefab;
    private List<Obstacle> obstacles;
    public List<GameObject> obstacleObjects;
    private static readonly int k = 5000;
    private string paramsPrefix = "";
    // Start is called before the first frame update
    void Start()
    {
        paramsPrefix = Storage.GetPrefix();
        RefreshObstacles();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void RefreshObstacles()
    {
        cleanObstacles();
        GetObstacles();
        foreach (var obstacle in obstacles)
        {
            if (obstaclePrefab != null)
            {
                var instance = Instantiate(obstaclePrefab);
                instance.transform.localScale = (new Vector3(obstacle.l * 1000 * Globals.scaleX, obstacle.h * 1000 * Globals.scaleY, obstacle.w * 1000 * Globals.scaleZ)) / k;
                instance.transform.position = (new Vector3(
                    obstacle.x * Globals.scaleX + (instance.transform.localScale.x / 2), 
                    obstacle.y * Globals.scaleY + (instance.transform.localScale.y / 2), 
                    obstacle.z * Globals.scaleZ + (instance.transform.localScale.z / 2)
                ));
                obstacleObjects.Add(instance);
            }
        }
    }

    void cleanObstacles()
    {
        foreach (var obstacle in obstacleObjects)
        {
            Destroy(obstacle);
        }
        obstacleObjects.Clear();
    }

    private void GetObstacles()
    {
        if (PlayerPrefs.HasKey(paramsPrefix + "obstacles"))
        {
            Debug.Log(PlayerPrefs.GetString(paramsPrefix + "obstacles"));
            obstacles = JsonConvert.DeserializeObject<List<Obstacle>>(PlayerPrefs.GetString(paramsPrefix + "obstacles"));
        }
        else
        {
            obstacles = new List<Obstacle>();
        }
    }

    public List<Obstacle> GetObstaclesObject()
    {
        return obstacles;
        ///
    }

    public List<GameObject> GetObstacleInstances()
    {
        return obstacleObjects;
    }
}
