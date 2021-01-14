using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    // Start is called before the first frame update

    public static void InitCrane (char id)
    {
        string path = Application.persistentDataPath + "/sav" + id + ".zxc";
        FileStream stream = new FileStream(path, FileMode.Create);

        stream.Close();
    }

    public static void ExpandStack(ref Dictionary<DateTime, CraneData> SaveStack, ref Crane CraneScript)
    {
        CraneData dota = new CraneData();
        dota = CraneData.InitData(ref CraneScript);

        //Debug.Log("ADDING " + dota.atAhPos.ToString());
        SaveStack.Add(DateTime.Now, dota);
    }

    public static void SaveCraneStack (ref Dictionary<DateTime, CraneData> SaveStack, char id)
    {
        BinaryFormatter form = new BinaryFormatter();

        string path = Application.persistentDataPath + "/sav" + id + ".zxc";
        FileStream stream = new FileStream(path, FileMode.Append);

        //Debug.Log("WRITING " + SaveStack.Values.First().atAhPos.ToString());

        form.Serialize(stream, SaveStack);
        stream.Close();
        //Debug.Log("FUKKEN SAVED");
    }

    public static Dictionary<DateTime, CraneData> LoadAllCraneDatas (char id)
    {
        string path = Application.persistentDataPath + "/sav" + id + ".zxc";

        if (File.Exists(path))
        {
            BinaryFormatter form = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            Dictionary<DateTime, CraneData> SavesArray;
            SavesArray = form.Deserialize(stream) as Dictionary<DateTime, CraneData>; //как отшагнуть n кусков?
            stream.Close();

            //Debug.Log("READING " + SavesArray.Values.First().atAhPos.ToString());
            
            return SavesArray;
        }

        Debug.LogError("Save file not found in " + path);
        return null;
    }

    public static void SetCurrentState(ref List<DateTime> time, ref List<CraneData> values, int pos, ref Crane CraneScript, char id)
    {
        Debug.Log("save no. " + pos.ToString() + " at " + time[pos].ToString() + " id " + id);
        CraneData dota = values[pos];

        CraneScript.bridgePosition = dota.bridgePos;
        CraneScript.mtPosition = dota.mtPos;
        CraneScript.atPosition = dota.atPos;
        CraneScript.mtMhPosition = dota.mtMhPos;
        CraneScript.mtAhPosition = dota.mtAhPos;
        CraneScript.atMhPosition = dota.atMhPos;
        CraneScript.atAhPosition = dota.atAhPos;

        CraneScript.mtMhTraverse = dota.mtMhTrav;
        CraneScript.mtAhTraverse = dota.mtAhTrav;
        CraneScript.atMhTraverse = dota.atMhTrav;
        CraneScript.atAhTraverse = dota.atAhTrav;
    }

    /*
    public static CraneData LoadCraneData (char id, uint n)
    {
        string path = Application.persistentDataPath + "/sav" + id + ".zxc";

        if (File.Exists(path))
          {
            BinaryFormatter form = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            CraneData dota = form.Deserialize(stream) as CraneData; //как отшагнуть n кусков?
            stream.Close();

            return dota;
          }

        Debug.LogError("Save file not found in " + path);
        return null;      
    }
     */

}
