using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeColor : MonoBehaviour
{

    public GameObject crane;
    public GameObject cart;
    public GameObject rail;
    public GameObject wallLL, wallAR;
    public GameObject MagSph;
    public GameObject partner;
    public bool[] StopMovement = new bool[4]; //W A S D
    private int RailColor = 0; //default green yellow red
    private int TempRailColor = 0;
    private int CartColor = 0; //default green yellow red
    private int TempCartColor = 0;

    public Collider RailColl;
    public Collider CartColl;

    public Material[] MMM;

    //private double distX1, distZ1, distX2, distZ2;


    // Start is called before the first frame update
    
    void Start()
    {
        RailColl = rail.GetComponent<Collider>();
        CartColl = cart.GetComponent<Collider>();
    }

    /*void ChangeMaterial(Material newMat, GameObject obbj)
    {
        Renderer[] children;
        children = obbj.GetComponentsInChildren<Renderer>();
        foreach (Renderer rend in children)
        {
            var mats = new Material[rend.materials.Length];
            for (var j = 0; j < rend.materials.Length; j++)
            {
                mats[j] = newMat;
            }
            rend.materials = mats;
        }
    }*/

    void ChangeMaterial(Material material, GameObject obj)
    {
        var meshRenderers = obj.GetComponentsInChildren<MeshRenderer>();
        foreach (var meshRenderer in meshRenderers)
        {
            var materials = meshRenderer.materials;
            if (material != null)
            {
                for (int i = 0; i < materials.Length; i++)
                {
                    Destroy(materials[i]);
                    materials[i] = material;
                }
            }
            meshRenderer.materials = materials;
        }

    }

    void ReDrawCart(int NewColor)
    {   
        if (NewColor == CartColor)
            return;
        switch (NewColor)
        {
        case 0: 
            ChangeMaterial(MMM[0], cart);  //it's a toss-up, really...
            break;
        case 1:
            ChangeMaterial(MMM[4], cart);
            break;
        case 2:
            ChangeMaterial(MMM[3], cart);
            break;
        case 3:
            ChangeMaterial(MMM[2], cart);
            break;
        }
        CartColor = NewColor;
    }

    void ReDrawRail(int NewColor)
    {
        if (NewColor == RailColor)
            return;
        switch (NewColor)
        {
            case 0: 
                ChangeMaterial(MMM[1], rail); //it's a toss-up, really...
            break;
            case 1:
                ChangeMaterial(MMM[4], rail);
            break;
            case 2:
                ChangeMaterial(MMM[3], rail);
            break;
            case 3:
                ChangeMaterial(MMM[2], rail);
            break;
        }
        RailColor = NewColor;
    }

    void CheckBounds() //has to be called 1st 
    {
        double distX1 = System.Math.Abs(crane.transform.position.x - wallLL.transform.position.x);
        double distZ1 = System.Math.Abs(cart.transform.position.z - wallLL.transform.position.z);
        double distX2 = System.Math.Abs(crane.transform.position.x - wallAR.transform.position.x);
        double distZ2 = System.Math.Abs(cart.transform.position.z - wallAR.transform.position.z);

        if ((distX1 < 0.5) || (distX2 < 0.5))
        {
            TempRailColor = 3;
            if (distX1 < 0.5)
                StopMovement[1] = true;
            else
                StopMovement[3] = true;
        }
        else
        {
            TempRailColor = 0;
            StopMovement[1] = false;
            StopMovement[3] = false;
        }

        if ((distZ1 < 0.2) || (distZ2 < 0.2))
        {
            TempCartColor = 3;
            if (distZ1 < 0.2)
                StopMovement[2] = true;
            else
                StopMovement[0] = true;
        }
        else
        {
            TempCartColor = 0;
            StopMovement[2] = false;
            StopMovement[0] = false;
        }
    }

    void CheckBoundsColl() //has to be called 1st 
    {
        //CartPoint1 = CartColl.ClosestPointOnBounds(wallLL.transform.position);
        //RailPoint1 = RailColl.ClosestPointOnBounds(wallLL.transform.position);
        //CartPoint2 = CartColl.ClosestPointOnBounds(wallAR.transform.position);
        //RailPoint2 = RailColl.ClosestPointOnBounds(wallAR.transform.position);


        //crane.transform.position.x
        //cart.transform.position.z
        //System.Math.Abs

        double distX1 = (RailColl.bounds.center.x - RailColl.bounds.size.x / 2 - wallLL.transform.position.x); 
        double distZ1 = (CartColl.bounds.center.z - CartColl.bounds.size.z / 2 - wallLL.transform.position.z);
        double distX2 = (RailColl.bounds.center.x + RailColl.bounds.size.x / 2 - wallAR.transform.position.x);
        double distZ2 = (CartColl.bounds.center.z + CartColl.bounds.size.z / 2 - wallAR.transform.position.z);

        if ((distX1 < 0.1) || (distX2 > -0.1))
        {
            TempRailColor = 3;
            if (distX1 < 0.1)
                StopMovement[1] = true;
            else
                StopMovement[3] = true;
        }
        else
        {
            TempRailColor = 0;
            StopMovement[1] = false;
            StopMovement[3] = false;
        }

        if ((distZ1 < 0.1) || (distZ2 > -0.1))
        {
            TempCartColor = 3;
            if (distZ1 < 0.1)
                StopMovement[2] = true;
            else
                StopMovement[0] = true;
        }
        else
        {
            TempCartColor = 0;
            StopMovement[2] = false;
            StopMovement[0] = false;
        }
    }

    void CheckRailKostyl()
    {
        double projX = (crane.transform.position.x - partner.transform.position.x);
        double distX = System.Math.Abs(projX);
        if ((distX < 3) && (TempRailColor == 0))
            TempRailColor = 1;
        if ((distX < 2) && (TempRailColor < 2))
            TempRailColor = 2;
        if (distX < 1)
        {
            TempRailColor = 3;
            if (projX > 0)
                StopMovement[1] = true;
            else
                StopMovement[3] = true;
        }
            
        return;        
    }

    void CheckSphere()
    {
        double distRail = System.Math.Sqrt((crane.transform.position.x - MagSph.transform.position.x) * (crane.transform.position.x - MagSph.transform.position.x) + (crane.transform.position.y - MagSph.transform.position.y) * (crane.transform.position.y - MagSph.transform.position.y)  + ((crane.transform.position.z - MagSph.transform.position.z) * (crane.transform.position.z - MagSph.transform.position.z)));
        double distCart = System.Math.Sqrt((cart.transform.position.x - MagSph.transform.position.x) * (cart.transform.position.x - MagSph.transform.position.x) + (cart.transform.position.y - MagSph.transform.position.y) * (cart.transform.position.y - MagSph.transform.position.y) + ((cart.transform.position.z - MagSph.transform.position.z) * (cart.transform.position.z - MagSph.transform.position.z)));
        double projX = crane.transform.position.x - MagSph.transform.position.x;
        double projZ = cart.transform.position.z - MagSph.transform.position.z;

        if ((distRail < 3) && (TempRailColor == 0))
            TempRailColor = 1;
        if ((distRail < 2) && (TempRailColor < 2))
            TempRailColor = 2;
        if (distRail < 1)
        {
            TempRailColor = 3;
            if (projX > 0)
                StopMovement[1] = true;
            else
                StopMovement[3] = true;
        }

        if ((distCart < 1) && (TempCartColor == 0))
            TempCartColor = 1;
        if ((distCart < 0.5) && (TempCartColor < 2))
            TempCartColor = 2;
        if (distCart < 0.05)
        {
            TempCartColor = 3;
            if (projZ > 0)
                StopMovement[0] = true;
            else
                StopMovement[2] = true;
        }

        return;
    }

    void CheckSphereColl()
    {
        Vector3 SphPoint = MagSph.transform.position;
        Vector3 CartPoint = CartColl.ClosestPointOnBounds(SphPoint);
        Vector3 RailPoint = RailColl.ClosestPointOnBounds(SphPoint);

        double distCart = Vector3.Distance(CartPoint, SphPoint);
        double distRail = Vector3.Distance(RailPoint, SphPoint);

        double projX = crane.transform.position.x - SphPoint.x;
        double projZ = cart.transform.position.z - SphPoint.z;

        if ((distRail < 1) && (TempRailColor == 0))
            TempRailColor = 1;
        if ((distRail < 0.5) && (TempRailColor < 2))
            TempRailColor = 2;
        if (distRail < 0.25)
        {
            TempRailColor = 3;
            if (projX > 0)
                StopMovement[1] = true;
            else
                StopMovement[3] = true;
        }

        if ((distCart < 1) && (TempCartColor == 0))
            TempCartColor = 1;
        if ((distCart < 0.5) && (TempCartColor < 2))
            TempCartColor = 2;
        if (distCart < 0.25)
        {
            TempCartColor = 3;
            if (projZ > 0)
                StopMovement[0] = true;
            else
                StopMovement[2] = true;
        }

        return;
    }


    // Update is called once per frame
    void Update()
    {
        //CheckBounds();
        CheckBoundsColl();

        if (partner != null)
            CheckRailKostyl(); //implement collider techniques, also ban entry on color == 3

        //CheckSphere(); //same
        CheckSphereColl();

        ReDrawCart(TempCartColor);
        ReDrawRail(TempRailColor);
        


    }
}
