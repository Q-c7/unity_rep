using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class BigSaveSystem
{
    // Start is called before the first frame update

    public static void InitCranes ()
    {
        string path = Application.persistentDataPath + "/big_sav.zxc";
        FileStream stream = new FileStream(path, FileMode.Create);

        stream.Close();
    }

    public static void ExpandBigStack(ref Dictionary<DateTime, AllCranesData> SaveStack, ref List<Crane> CraneScripts)
    {
        AllCranesData dota = new AllCranesData();

        dota = AllCranesData.InitData(CraneScripts[0], CraneScripts[1], CraneScripts[2]);

        //Debug.Log("ADDING " + dota.atAhPos.ToString());
        SaveStack.Add(DateTime.Now, dota);
    }

    public static void SaveBigStack (ref Dictionary<DateTime, AllCranesData> SaveStack)
    {
        BinaryFormatter form = new BinaryFormatter();

        string path = Application.persistentDataPath + "/big_sav.zxc";
        FileStream stream = new FileStream(path, FileMode.Append);

        //Debug.Log("WRITING " + SaveStack.Values.First().atAhPos.ToString());

        form.Serialize(stream, SaveStack);
        stream.Close();
        //Debug.Log("FUKKEN SAVED");
    }

    public static Dictionary<DateTime, AllCranesData> LoadAllCraneDatas ()
    {
        string path = Application.persistentDataPath + "/big_sav.zxc";

        if (File.Exists(path))
        {
            BinaryFormatter form = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            Dictionary<DateTime, AllCranesData> SavesArray;
            SavesArray = form.Deserialize(stream) as Dictionary<DateTime, AllCranesData>; //как отшагнуть n кусков?
            stream.Close();

            //Debug.Log("READING " + SavesArray.Values.First().atAhPos.ToString());
            
            return SavesArray;
        }

        Debug.LogError("Save file not found in " + path);
        return null;
    }

    public static void SetSingleCrane (ref List<float> PosArr, ref List<int> TravArr, int craneno, ref List<Crane> CraneScripts)
    {
        const int N = 7;
        const int M = 4;

        CraneScripts[craneno].bridgePosition = PosArr[craneno * N];
        CraneScripts[craneno].mtPosition = PosArr[craneno * N + 1];
        CraneScripts[craneno].atPosition = PosArr[craneno * N + 2];
        CraneScripts[craneno].mtMhPosition = PosArr[craneno * N + 3];
        CraneScripts[craneno].mtAhPosition = PosArr[craneno * N + 4];
        CraneScripts[craneno].atMhPosition = PosArr[craneno * N + 5];
        CraneScripts[craneno].atAhPosition = PosArr[craneno * N + 6];

        CraneScripts[craneno].mtMhTraverse = TravArr[craneno * M];
        CraneScripts[craneno].mtAhTraverse = TravArr[craneno * M + 1];
        CraneScripts[craneno].atMhTraverse = TravArr[craneno * M + 2];
        CraneScripts[craneno].atAhTraverse = TravArr[craneno * M + 3];
    }


    //--------------------------------------------------------for CraneScripts i need REF in order to change values in program and not in some abstract copy of the Crane script, but I can't pass REF with CraneScript[0] - compiler fucks me
    public static void SetCurrentState (ref List<DateTime> time, ref List<AllCranesData> values, int saveno, ref List<Crane> CraneScripts) 
    {
        //Debug.Log("save no. " + pos.ToString() + " at " + time[pos].ToString() + " id " + id);

        //здесь можно что-то от времени высрать

        AllCranesData dota = values[saveno];

        SetSingleCrane(ref dota.PosArray, ref dota.TravArray, 0, ref CraneScripts);
        SetSingleCrane(ref dota.PosArray, ref dota.TravArray, 1, ref CraneScripts);
        SetSingleCrane(ref dota.PosArray, ref dota.TravArray, 2, ref CraneScripts);
    }

}
