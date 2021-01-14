using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CreateObject : MonoBehaviour
{
    public Camera myCamera;
    public Camera mainCamera;
    public GameObject newObj1;
    GameObject newObj;
    public GameObject name;
    public LayerMask selectionMask = Physics.DefaultRaycastLayers;
    public string clickedName = "";
    public GameObject xCoord;
    public GameObject yCoord;
    public GameObject xScale;
    public GameObject yScale;
    public GameObject zScale;
    public GameObject xCoordText;
    public GameObject yCoordText;
    public GameObject xScaleText;
    public GameObject yScaleText;
    public GameObject zScaleText;

    private void Start()
    {
        myCamera.enabled = false;
        mainCamera.enabled = true;
    }

    private void Update()
    {
        if (name.GetComponent<TextMeshProUGUI>().text.Length > 1 )
        {
            gameObject.GetComponent<Button>().interactable = true;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            myCamera.enabled = !myCamera.enabled;
            mainCamera.enabled = !mainCamera.enabled;
        }
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hitInfo;
            if (Physics.Raycast(myCamera.ScreenPointToRay(Input.mousePosition), out hitInfo, Mathf.Infinity, selectionMask))
            {
                Transform target = hitInfo.transform;
                if (target.tag == "Move")
                {
                    clickedName = target.name;
                }
        
            }
        }
        

        if (clickedName != "")
        {
            xCoord.GetComponent<TextMeshProUGUI>().text = ( Mathf.RoundToInt(-10000*GameObject.Find(clickedName).transform.position.z)).ToString();
            yCoord.GetComponent<TextMeshProUGUI>().text = ( Mathf.RoundToInt(10000*GameObject.Find(clickedName).transform.position.x)).ToString();
            xScale.GetComponent<TextMeshProUGUI>().text = ( Mathf.RoundToInt(10000*GameObject.Find(clickedName).transform.localScale.y)).ToString();
            yScale.GetComponent<TextMeshProUGUI>().text = ( Mathf.RoundToInt(10000*GameObject.Find(clickedName).transform.localScale.x)).ToString();
            zScale.GetComponent<TextMeshProUGUI>().text = ( Mathf.RoundToInt(10000*GameObject.Find(clickedName).transform.localScale.z)).ToString();
            
        }
        
        
        
    }
    

    public void Create()
    {
        newObj= (GameObject)Instantiate(newObj1, new Vector3(0f, 0f, 0f), Quaternion.Euler(-90f, 0f, 0f));
        newObj.name = name.GetComponent<TextMeshProUGUI>().text;
        newObj.tag = "Move";
    }

    public void changeX()
    {
        if (yCoordText.GetComponent<TMP_InputField>().text == "") yCoordText.GetComponent<TMP_InputField>().text = yCoord.GetComponent<TextMeshProUGUI>().text;
        if (xCoordText.GetComponent<TMP_InputField>().text == "") xCoordText.GetComponent<TMP_InputField>().text = xCoord.GetComponent<TextMeshProUGUI>().text;
        if (xScaleText.GetComponent<TMP_InputField>().text == "") xScaleText.GetComponent<TMP_InputField>().text = xScale.GetComponent<TextMeshProUGUI>().text;
        if (yScaleText.GetComponent<TMP_InputField>().text == "") yScaleText.GetComponent<TMP_InputField>().text = yScale.GetComponent<TextMeshProUGUI>().text;
        if (zScaleText.GetComponent<TMP_InputField>().text == "") zScaleText.GetComponent<TMP_InputField>().text = zScale.GetComponent<TextMeshProUGUI>().text;
        GameObject.Find(clickedName).transform.position = new Vector3(float.Parse(yCoordText.GetComponent<TMP_InputField>().text)/(10000), 0f, float.Parse(xCoordText.GetComponent<TMP_InputField>().text)/(-10000));
        GameObject.Find(clickedName).transform.localScale = new Vector3(float.Parse(yScaleText.GetComponent<TMP_InputField>().text)/(10000), float.Parse(xScaleText.GetComponent<TMP_InputField>().text)/(10000), float.Parse(zScaleText.GetComponent<TMP_InputField>().text)/(10000));
        yCoordText.GetComponent<TMP_InputField>().text = "";
        xCoordText.GetComponent<TMP_InputField>().text = "";
        xScaleText.GetComponent<TMP_InputField>().text = "";
        yScaleText.GetComponent<TMP_InputField>().text = "";
        zScaleText.GetComponent<TMP_InputField>().text = "";
        
    }

}
