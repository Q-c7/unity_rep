using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CargoDrawer : MonoBehaviour
{
    public GameObject weightPrefab;
    public bool Visible;
    public Vector3 position;
    public Vector3 scale;
    public GameObject weight;
    public string ignoreTag;
    private GameObject innerObject;
    private Transform parent;
    public bool useOverall;
    public Vector3 overallSize;
    public Vector3 overallPosition;

    // Start is called before the first frame update
    void Start()
    {
        if (weightPrefab != null)
        {
            weight = Instantiate(weightPrefab);
            innerObject = weight.transform.Find("weight")?.gameObject;
            parent = weight.transform.parent;
        }
            
    }

    // Update is called once per frame
    void Update()
    {
        //weight.SetActive(Visible);
        if (!Visible)
        {
            weight.gameObject.transform.position = new Vector3(-10, -10, -10);
            return;
        }
        if (innerObject != null)
        {
            innerObject.tag = ignoreTag;
        }
        if(weight != null)
        {
            weight.gameObject.transform.localScale = scale;
            weight.gameObject.transform.position = position;
        }
        var colliders = gameObject.GetComponentsInChildren<CollidersSizeCalc>();
        
        foreach (var item in colliders)
        {
            switch (item.type)
            {
                case ColliderType.None:
                    break;
                case ColliderType.Up:
                    break;
                case ColliderType.Down:
                    if (useOverall)
                    {
                        item.sizeAdditions = !item.isWeightColliders ? new Vector3() : new Vector3(overallSize.y, Math.Max(scale.x, overallSize.x), overallSize.z);
                        item.posCorrections = new Vector3(overallPosition.z, overallPosition.y, overallPosition.x);
                    }
                    else
                    {
                        item.sizeAdditions = !item.isWeightColliders ? new Vector3() : new Vector3(scale.z, scale.x, scale.y);
                        item.posCorrections = new Vector3();
                    }
                    
                    //item.addition = scale.y / 2;
                    break;
                case ColliderType.Forward:
                case ColliderType.Backward:
                    if (useOverall)
                    {
                        item.sizeAdditions = !item.isWeightColliders ? new Vector3() : new Vector3(Math.Max(scale.x, overallSize.x), overallSize.z, overallSize.y/2);
                        item.posCorrections = new Vector3(overallPosition.z, overallPosition.y, overallPosition.x);
                    }
                    else
                    {
                        item.sizeAdditions = !item.isWeightColliders ? new Vector3() : new Vector3(scale.x, scale.y, scale.z/2);
                        item.posCorrections = new Vector3();
                    }
                    
                    //item.addition = scale.z / 2;
                    break;
                case ColliderType.Left:
                case ColliderType.Right:
                    if (useOverall)
                    {
                        item.sizeAdditions = !item.isWeightColliders ? new Vector3() : new Vector3(overallSize.y, overallSize.z, overallSize.x / 2);
                        item.posCorrections = new Vector3(overallPosition.z, overallPosition.y, overallPosition.x);
                    }
                    else
                    {
                        item.sizeAdditions = !item.isWeightColliders ? new Vector3() : new Vector3(scale.z, scale.y, scale.x / 2);
                        item.posCorrections = new Vector3();
                    }
                    break;
                default:
                    break;
            }
        }
    }

    private void OnDestroy()
    {
        Destroy(weight);
    }
}
