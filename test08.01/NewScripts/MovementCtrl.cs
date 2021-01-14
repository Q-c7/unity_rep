using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MovementCtrl : MonoBehaviour
{
    public float char_ms = 0.5f;
    public float buttonDelta = 0.2F;
    //private float nextPress = 0.5F;
    private float myTime = 0.0F;
    public int currentPos = 0;

    private float xMovement, zMovement, yMovement;

    ChangeColor ColorScript;

    public List<GameObject> cart_list;
    public List<GameObject> rail_list;

    GameObject CurrentCart;
    GameObject CurrentRail;
    // Start is called before the first frame update
    void Start()
    {

        // INITIALIZE cart_list ARRAY!!!
    }


    //void BlockMovement()
    //{
    //    if ((ColorScript.StopMovement[0]) && (zMovement > 0))
    //        zMovement = 0;
    //    if ((ColorScript.StopMovement[1]) && (xMovement < 0))
    //        xMovement = 0;
    //    if ((ColorScript.StopMovement[2]) && (zMovement < 0))
    //        zMovement = 0;
    //    if ((ColorScript.StopMovement[3]) && (xMovement > 0))
    //        xMovement = 0;
    //}

    // Update is called once per frame
    void Update()
    {
        myTime = myTime + Time.deltaTime;

        if (Input.GetButton("Change1") && myTime > buttonDelta)
        {
            currentPos = (currentPos + 1) % (cart_list.Count);

            myTime = 0.0F;
        }

        CurrentRail = rail_list[currentPos];
        CurrentCart = cart_list[currentPos];
        ColorScript = CurrentCart.GetComponent<ChangeColor>();

        xMovement = Input.GetAxis("Horizontal") * char_ms;
        zMovement = Input.GetAxis("Vertical") * 1.5f * char_ms;
        if (currentPos == 5)
            yMovement = Input.GetAxis("FlyUpAxis") * 0.5f * char_ms;

        //if (currentPos != 5)
            //BlockMovement();       

        CurrentRail.transform.Translate(new Vector3(xMovement, 0, 0) * Time.deltaTime);
        //if (currentPos == 1)
        //    CurrentCart.transform.Translate(new Vector3(-zMovement * ((float)System.Math.Sqrt(2)), 0, -zMovement) * (Time.deltaTime)); //(float) System.Math.Sqrt(2)
        //if (currentPos == 2)
        //    CurrentCart.transform.Translate(new Vector3(-zMovement, 0, 0) * Time.deltaTime);
        //else
        CurrentCart.transform.Translate(new Vector3(0, 0, zMovement) * Time.deltaTime);
        if (currentPos == 5)
            CurrentCart.transform.Translate(new Vector3(0, yMovement, 0) * Time.deltaTime);


    }
}