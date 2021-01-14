using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CableLength : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject hoistpoint;
    public GameObject hoist1;
    public GameObject hoist2;
    public GameObject cable1;
    public GameObject cable2;
    public GameObject cable3;
    public GameObject cable4;
    public GameObject cable1_2;
    public GameObject cable2_2;
    public GameObject cable3_2;
    public GameObject cable4_2;
    private float startscale;
    private float startlength;
    private float startscale2;
    private float startlength2;
    void Start()
    {
        startlength = hoistpoint.transform.localPosition[1] - hoist1.transform.localPosition[1];
        startscale = cable1.transform.localScale[2];
        startlength2 = hoistpoint.transform.localPosition[1] - hoist2.transform.localPosition[1];
        startscale2 = cable1_2.transform.localScale[2];
        
    }

    // Update is called once per frame
    void Update()
    {
        float scale = (hoistpoint.transform.localPosition[1] - hoist1.transform.localPosition[1])*startscale/startlength;
        float scale2 = (hoistpoint.transform.localPosition[1] - hoist2.transform.localPosition[1])*startscale2/startlength2;
        //print(scale);
        cable1.transform.localScale = new Vector3(cable1.transform.localScale.x, cable1.transform.localScale.y, scale*1.07f);
        cable2.transform.localScale = new Vector3(cable2.transform.localScale.x, cable2.transform.localScale.y, scale*1.07f);
        cable3.transform.localScale = new Vector3(cable3.transform.localScale.x, cable3.transform.localScale.y, scale*1.07f);
        cable4.transform.localScale = new Vector3(cable4.transform.localScale.x, cable4.transform.localScale.y, scale*1.07f);
        cable1_2.transform.localScale = new Vector3(cable1_2.transform.localScale.x, cable1_2.transform.localScale.y, scale2*1.07f);
        cable2_2.transform.localScale = new Vector3(cable2_2.transform.localScale.x, cable2_2.transform.localScale.y, scale2*1.07f);
        cable3_2.transform.localScale = new Vector3(cable3_2.transform.localScale.x, cable3_2.transform.localScale.y, scale2*1.07f);
        cable4_2.transform.localScale = new Vector3(cable4_2.transform.localScale.x, cable4_2.transform.localScale.y, scale2*1.07f);
    }
}
