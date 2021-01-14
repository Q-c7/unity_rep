using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CommandLine : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static Dictionary<string, string> GetCommandLineArgs()
    {
        var args = System.Environment.GetCommandLineArgs();
        var retval = new Dictionary<string, string>();
        foreach (var arg in args)
        {
            var tmp = arg.Split('=');
            retval.Add(tmp[0].Replace("--", ""), tmp.Length > 1 ? tmp[1] : null);
        }
        return retval;
    }
}
