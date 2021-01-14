using System.Collections;
using System.Collections.Generic;
using AsImpL.Examples;
using UnityEngine;

public class AddCollider : MonoBehaviour
{
    // Start is called before the first frame update
    private string name;
    void Start()
    {
        name = GameObject.Find("Importer").GetComponent<AsImpLSample>().objectName;
    }

    // Update is called once per frame
    void Update()
    {

        GameObject.Find(name).AddComponent<BoxCollider>();
    }
}
