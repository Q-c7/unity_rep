using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeSizeCalc : MonoBehaviour
{
    //public float zMaxPoint = 29000f;
    private Vector3 initialPosition;
    private Vector3 initialScale, parentInitialPosition;
    // Start is called before the first frame update
    void Start()
    {
        initialPosition = gameObject.transform.position;
        initialScale = gameObject.transform.localScale;
        parentInitialPosition = gameObject.transform.parent.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = gameObject.transform.position;
        Vector3 scale = initialScale;
        float dY = parentInitialPosition.y - gameObject.transform.parent.position.y;
        scale.y = initialScale.y + dY / (gameObject.transform.parent.localScale.y);
        pos.y = initialPosition.y - dY/2;// / (gameObject.transform.parent.localScale.y);

        gameObject.transform.position = pos;
        gameObject.transform.localScale = scale;
    }
}
