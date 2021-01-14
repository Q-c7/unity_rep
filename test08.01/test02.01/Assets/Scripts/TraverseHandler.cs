using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraverseHandler : MonoBehaviour
{
    public float x;
    public float y;
    public float z;
    public bool show = false;
    public int traverseId = 0;
    //private Vector3 initialPosition;
    public Vector3 currentPosition;
    public Vector3 diff;
    public TraverseStruct traverseParams;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        x = traverseParams.length;
        y = traverseParams.height;
        z = traverseParams.width;
        gameObject.transform.position = currentPosition + diff;
        var tmp = gameObject.GetComponentInChildren<BoxCollider>(true);
        var internalShow = show;
        tmp?.gameObject.SetActive(internalShow);
        
        if (internalShow)
        {
            var traverse = traverseParams;
            //gameObject.transform.position += new Vector3(0, -traverse.height * Globals.scaleY / 2 - 0.1f, 0);
            tmp.transform.localScale = new Vector3(traverse.length * Globals.scaleX, traverse.height * Globals.scaleY, traverse.width * Globals.scaleZ);
        }
    }
}
