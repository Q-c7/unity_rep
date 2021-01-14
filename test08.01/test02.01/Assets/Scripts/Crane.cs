using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crane : MonoBehaviour
{
    public float mtMhMaxHeight;
    public float mtAhMaxHeight;
    public float atMhMaxHeight;
    public float atAhMaxHeight;
    public GameObject mainTrolley;
    public GameObject auxiliaryTrolley;
    public GameObject bridge;
    public GameObject mtMainHoist;
    public GameObject mtAuxHoist;
    public GameObject atMainHoist;
    public GameObject atAuxHoist;
    // Start is called before the first frame update
    public float bridgePosition;
    public float mtPosition;
    public float atPosition;
    public float mtMhPosition;
    public float mtAhPosition;
    public float atMhPosition;
    public float atAhPosition;
    //public float zZeroFix = 0.0f;

    public int mtMhTraverse = 0;
    public int mtAhTraverse = 0;
    public int atMhTraverse = 0;
    public int atAhTraverse = 0;

    private Vector3 initialBridgePosition;
    private Vector3 initialMainTrolleyPosition;
    private Vector3 initialAuxiliaryTrolleyPosition;
    private Vector3 initialMtMhPosition;
    private Vector3 initialMtAhPosition;
    private Vector3 initialAtMhPosition;
    private Vector3 initialAtAhPosition;

    void Start()
    {
        if (bridge != null)
            initialBridgePosition = bridge.transform.position;
        if (mainTrolley != null)
            initialMainTrolleyPosition = mainTrolley.transform.position;
        if (auxiliaryTrolley != null)
            initialAuxiliaryTrolleyPosition = auxiliaryTrolley.transform.position;
        if (mtMainHoist != null)
            initialMtMhPosition = mtMainHoist.transform.position;
        if (mtAuxHoist != null)
            initialMtAhPosition = mtAuxHoist.transform.position;
        if (atMainHoist != null)
            initialAtMhPosition = atMainHoist.transform.position;
        if (atAuxHoist != null)
            initialAtAhPosition = atAuxHoist.transform.position;
        //Debug.Log(initialBridgePosition.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        float mtPositionFixed = - mtPosition;
        float atPositionFixed = - atPosition;
        //float mtMhPosFixed = zZeroFix - mtMhPosition;
        //float mtAhPosFixed = zZeroFix - mtAhPosition;

        if (bridge != null)
        {
            bridge.transform.position = initialBridgePosition + new Vector3(bridgePosition*Globals.scaleX, 0);
        }
        if (mainTrolley != null)
        {
            mainTrolley.transform.position = initialMainTrolleyPosition + new Vector3(bridgePosition * Globals.scaleX, 0, mtPositionFixed * Globals.scaleZ);
        }
        if (auxiliaryTrolley != null)
        {
            auxiliaryTrolley.transform.position = initialAuxiliaryTrolleyPosition + new Vector3(bridgePosition * Globals.scaleX, 0, atPositionFixed * Globals.scaleZ);
        }
        if (mtMainHoist != null)
        {
            mtMainHoist.transform.position = initialMtMhPosition + new Vector3(bridgePosition * Globals.scaleX, (mtMhPosition - mtMhMaxHeight) * Globals.scaleY, mtPositionFixed * Globals.scaleZ);
        }
        if (mtAuxHoist != null)
        {
            mtAuxHoist.transform.position = initialMtAhPosition + new Vector3(bridgePosition * Globals.scaleX, (mtAhPosition - mtAhMaxHeight) * Globals.scaleY, mtPositionFixed * Globals.scaleZ);
        }
        if(atMainHoist != null)
        {
            atMainHoist.transform.position = initialAtMhPosition + new Vector3(bridgePosition * Globals.scaleX, (atMhPosition - atMhMaxHeight) * Globals.scaleY, atPositionFixed * Globals.scaleZ);
        }
        if (atAuxHoist != null)
        {
            atAuxHoist.transform.position = initialAtAhPosition + new Vector3(bridgePosition * Globals.scaleX, (atAhPosition - atAhMaxHeight) * Globals.scaleY, atPositionFixed * Globals.scaleZ);
        }
    }

    public void move(Vector3 diff)
    {
        if (bridge != null)
        {
            bridge.transform.position += diff;
        }
    }

    public void setBridgePosition(Vector3 position)
    {
        if (bridge != null)
        {
            bridge.transform.position = position;
        }
    }

    public void moveMainTrolley(Vector3 diff)
    {
        if(mainTrolley != null)
        {
            mainTrolley.transform.position += diff;
        }
    }

    public void moveAuxTrolley(Vector3 diff)
    {
        if (auxiliaryTrolley != null)
        {
            auxiliaryTrolley.transform.position += diff;
        }
    }
}
