using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteControl : MonoBehaviour
{
    // Start is called before the first frame update
    private bool deleteMenuShowed = false;
    public GameObject deleteMenu;
    public List<GameObject> obstacles;
    void Start()
    {
        
    }
    private void FixedUpdate()
    {
        if (Input.GetButtonDown("Delete"))
        {
            deleteMenuShowed = !deleteMenuShowed;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if(deleteMenu != null)
        {
            deleteMenu.SetActive(deleteMenuShowed);
        }
    }
}
