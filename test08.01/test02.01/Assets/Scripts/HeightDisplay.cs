using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeightDisplay : MonoBehaviour
{
    public Text heightLabel;
    // Start is called before the first frame update
    public Material normal;
    public Material selected;
    public bool select;
    private GameObject cube;
    public Vector3 initialScale;
    public Vector3 InitialCanvasScale;
    private Canvas canvas;
    void Start()
    {
        canvas = gameObject.GetComponentInChildren<Canvas>();
        if(canvas != null)
        {
            initialScale = gameObject.transform.localScale;
            InitialCanvasScale = canvas.transform.localScale;
        }        
        cube = gameObject.transform.Find("CubeInt").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (canvas != null)
        {
            Vector3 tmpScale = canvas.transform.localScale;
            if(gameObject.transform.localScale.x != 0 && gameObject.transform.localScale.z != 0)
            {
                tmpScale.x = 0.03f / gameObject.transform.localScale.x;
                tmpScale.y = 0.03f / gameObject.transform.localScale.z;
                canvas.transform.localScale = tmpScale;
                initialScale = tmpScale;
            }            
        }
        if(heightLabel != null)
        {
            heightLabel.text = string.Format("{0} мм", gameObject.transform.localScale.y * 1000);
        }
        if (cube != null)
        {
            cube.GetComponent<MeshRenderer>().material = select ? selected : normal;
        }
    }
}
