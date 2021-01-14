using System.Collections;
using System.Collections.Generic;
using cakeslice;
using UnityEngine;
using System.Linq;
public class ChangeColor1 : MonoBehaviour
{

    public GameObject crane;
    public GameObject cart;
    public GameObject cart2;
    public List<GameObject> Collumns = new List<GameObject>();
    public List<Collider> CollumnsColliders = new List<Collider>();
    public List<double> dist;
    public List<Outline1> CollumsOutlines = new List<Outline1>();
    private List<int> CollumnsColors;
    private List<int> SpheresColors;
    public List<GameObject> Spheres = new List<GameObject>();
    private int RailColor = 0; //default green yellow red
    private int TempRailColor = 0;
    private int CartColor = 0; //default green yellow red
    private int TempCartColor = 0;
    
    
    
    
    void Start()
    {
        
        for (int i = 0; i < Collumns.Count; i++)
        {
            CollumnsColliders[i] = (Collumns[i].GetComponent<BoxCollider>());
        }
        for (int i = 0; i < Collumns.Count; i++)
        {
            CollumsOutlines[i] = (Collumns[i].GetComponent<Outline1>());
        }
        dist = new List<double>();
        for (int i = 0; i < Collumns.Count*Spheres.Count; i++)
        {
            dist.Add(0);
        }
        
        CollumnsColors = Enumerable.Repeat(-1, Collumns.Count).ToList();
        SpheresColors = Enumerable.Repeat(-1, Collumns.Count).ToList();
    }


    void CheckSphereColl()
    {
        if (Spheres[0] == null)
        { 
            for (int i = 1; i < Spheres.Count+1; i++)
            {
                Spheres[i-1] = GameObject.Find("point"+i);
            }
        }
        
        CollumnsColors = Enumerable.Repeat(-1, Collumns.Count).ToList();
        SpheresColors = Enumerable.Repeat(-1, Collumns.Count).ToList();
        //Vector3 SphPoint = MagSph.transform.position;
        for (int j = 0; j < Spheres.Count; j++)
        {
            for (int i = 0; i < Collumns.Count; i++)
            {
                float currentDist = Vector3.Distance(CollumnsColliders[i].ClosestPointOnBounds(Spheres[j].transform.position), Spheres[j].transform.position);
                int color = -1;
                if (currentDist < 0.5)
                {
                    //color = (currentDist < 0.25) ? 1 : 0;
                    if (currentDist < 0.25)
                    {
                        color = 1;
                        Spheres[j].GetComponentInParent<Outline1>().color = 1;
                        Spheres[j].GetComponentInParent<Outline1>().eraseRenderer = false;
                    }
                    else
                    {
                        color = 0;
                        Spheres[j].GetComponentInParent<Outline1>().color = 0;
                        Spheres[j].GetComponentInParent<Outline1>().eraseRenderer = false;
                    }
                }
                CollumnsColors[i] = Mathf.Max(color, CollumnsColors[i]);
                
            }
        }

        for (int i = 0; i < CollumnsColors.Count; i++)
        {
            CollumsOutlines[i].color = CollumnsColors[i];
            CollumsOutlines[i].eraseRenderer = (CollumnsColors[i] == -1) ? true : false;
        }
       
        
        //Vector3 CartPoint = CartColl.ClosestPointOnBounds(SphPoint);
        //Vector3 RailPoint = RailColl.ClosestPointOnBounds(SphPoint);
//
        //double distCart = Vector3.Distance(CartPoint, SphPoint);
        //double distRail = Vector3.Distance(RailPoint, SphPoint);
//
        //double projX = crane.transform.position.x - SphPoint.x;
        //double projZ = cart.transform.position.z - SphPoint.z;


        //if (distRail < 0.5)
        //{
        //    outlineCrane.color = (distRail < 0.25) ? 0 : 1;
        //    outlineCrane.eraseRenderer = false;
        //}
        //else
        //{
        //    outlineCrane.eraseRenderer = true;
        //}
        //
        //if (distCart < 0.5)
        //{
        //    outlineCart.color = (distCart < 0.25) ? 0 : 1;
        //    outlineCart.eraseRenderer = false;
        //}
        //else
        //{
        //    outlineCart.eraseRenderer = true;
        //}
        
        //if ( cart2 != null)
        //{
        //    double distCart2 = 0;
        //    Vector3 CartPoint2;
        //    CartPoint2 = CartColl2.ClosestPointOnBounds(SphPoint);
        //    distCart2 = Vector3.Distance(CartPoint2, SphPoint);
        //    if (distCart2 < 0.5)
        //    {
        //        outlineCart2.color = (distCart2 < 0.25) ? 0 : 1;
        //        outlineCart2.eraseRenderer = false;
        //    }
        //    else
        //    {
        //        outlineCart2.eraseRenderer = true;
        //    }
        //}
        //for (int m = 0; m < Collumns.Count; m++)
        //{
        //    for (int l = m * Spheres.Count; l < m * Spheres.Count + Spheres.Count; l++)
        //    {
        //        if (dist[l] < 0.5)
        //        {
        //            CollumsOutlines[m].color = (dist[l] < 0.25) ? 0 : 1;
        //            CollumsOutlines[m].eraseRenderer = false;
        //        }
        //        //else
        //        //{
        //        //    CollumsOutlines[m].eraseRenderer = true;
        //        //}
        //    }
        //    
        //}
    }


    // Update is called once per frame
    void Update()
    {
        CheckSphereColl();

    }
}
