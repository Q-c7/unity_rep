using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraneMovement : MonoBehaviour
{
    public Dropdown craneSelector;
    public Dropdown trolleySelector;
    public Dropdown hoistSelector;
    public Crane crane1;
    public Crane crane2;
    public Crane crane3;
    public Info info;
    public Crane currentCrane;
    public int currentTrolley;
    public int currentHoist;
    private const float speed = 100.0f;
    // Start is called before the first frame update
    void Start()
    {
        if (craneSelector != null)
        {
            craneSelector.onValueChanged.AddListener(delegate { onCraneSelectionChanged(craneSelector); });
            trolleySelector.onValueChanged.AddListener(delegate { onTrolleySelectionChanged(trolleySelector); });
            hoistSelector.onValueChanged.AddListener(delegate { onHoistSelectionChanged(hoistSelector); });
        }
    }

    private void onHoistSelectionChanged(Dropdown hoistSelector)
    {
        currentHoist = hoistSelector.value;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        if ((currentCrane != null))
        {
            currentCrane.bridgePosition += Input.GetAxis("Horizontal") * speed;
            switch (currentTrolley)
            {
                case 0:
                default:
                    currentCrane.mtPosition += Input.GetAxis("Vertical") * speed;
                    break;
                case 1:
                    currentCrane.atPosition += Input.GetAxis("Vertical") * speed;
                    break;
            }
            if (currentTrolley == 1)
            {
                if (currentHoist == 0)
                {
                    currentCrane.atMhPosition += Input.GetAxis("Mouse ScrollWheel") * speed * 3;
                }
                else
                {
                    currentCrane.atAhPosition += Input.GetAxis("Mouse ScrollWheel") * speed * 3;
                }
            }
            else
            {
                if (currentHoist == 0)
                {
                    currentCrane.mtMhPosition += Input.GetAxis("Mouse ScrollWheel") * speed;
                }
                else
                {
                    currentCrane.mtAhPosition += Input.GetAxis("Mouse ScrollWheel") * speed;
                }
            }
        }
    }

    void onCraneSelectionChanged(Dropdown dropdown)
    {
        switch (dropdown.value)
        {
            case 1:
                //Debug.Log(dropdown.value.ToString());
                currentCrane = crane1;
                break;
            case 2:
                //Debug.Log(dropdown.value.ToString());
                currentCrane = crane2;
                break;
            case 3:
                //Debug.Log(dropdown.value.ToString());
                currentCrane = crane3;
                break;
            case 0:
            default:
                //Debug.Log(dropdown.value.ToString());
                currentCrane = null;
                break;
        }
        if(info != null) info.crane = currentCrane;
    }

    void onTrolleySelectionChanged(Dropdown dropdown)
    {
        currentTrolley = dropdown.value;
    }
}
