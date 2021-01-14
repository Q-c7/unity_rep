using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class Storage
{
    public static Dictionary<string, string> GetArgs()
    {
        var args = new Dictionary<string, string>();
        var cmdline = System.Environment.GetCommandLineArgs();
        foreach (var arg in cmdline)
        {
            if (arg.StartsWith("--"))
            {
                args.Add(arg, null);
            }else
            {
                if(args.Count > 0)
                {
                    args[args.Last().Key] = arg;
                }
            }
        }
        return args;
    }
    public static string GetPrefix()
    {
        var args = GetArgs();
        var prefix = (from arg in args where arg.Key == "--config-prefix" select arg).FirstOrDefault().Value;
        //prefix = string.IsNullOrEmpty(prefix) ? "" : prefix.Split('=')[1];
        //Debug.LogError(prefix);
        return prefix;
        
    }

    public static string GetName()
    {
        return "";
    }
}
