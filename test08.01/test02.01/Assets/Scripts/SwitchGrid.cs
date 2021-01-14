using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SwitchGrid : MonoBehaviour
{
    public GameObject grid1;
    public GameObject grid2;
    // Start is called before the first frame update
    void Start()
    {
        var args = System.Environment.GetCommandLineArgs();
        var prefix = (from arg in args where arg.StartsWith("--second-grid") select arg).FirstOrDefault();
        grid1?.SetActive(string.IsNullOrEmpty(prefix));
        grid2?.SetActive(!string.IsNullOrEmpty(prefix));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
