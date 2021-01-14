using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.UI;

public class SaveLoad3 : MonoBehaviour
{
    public char id;
    private List<Crane> CraneScripts = new List<Crane>(3);
    public List<GameObjects> CraneObjects = new List<GameObject>(3);

    public static float TimeSinceSaved;
    public static double GlobalTime;
    public bool timeStarted = false;
    private bool LoadingMode = false;

    public static float buttonTime = 0.0F;
    private float buttonDelta = 0.2F;
    public int currentPos = 0;

    public Dictionary<DateTime, AllCranesData> SavesStack = new Dictionary<DateTime, AllCranesData>();
    public Dictionary<DateTime, AllCranesData> AnotherStack = null;
    public List<DateTime> TimeDates;
    public List<AllCranesData> CraneDatas;

    void Start()
    {
        if ((id != '1') && (id != '2') && (id != '3'))
            id = '0';

        //CraneScript = GetComponent<Crane>(); 
        //---------------------------------------------------------------- we have 3 cranes now and have to link these three scripts MANUALLY-------------------------------------------------
        
        for (int i = 0; i < 3; i++)
            CraneScripts[i] = CraneObjects[i].GetComponent<Crane>();

        timeStarted = true;
        LoadingMode = false;

        GlobalTime = 0;
        TimeSinceSaved = 0;

        BigSaveSystem.InitCranes();
        //Debug.Log(Application.persistentDataPath);
    }

    void Update()
    {
        GlobalTime += (double)Time.deltaTime;

        if (timeStarted == true)
            TimeSinceSaved += Time.deltaTime;

        if (TimeSinceSaved >= 5) //(TimeSecs >= 0)
        {
            BigSaveSystem.ExpandBigStack(ref SavesStack, ref CraneScripts);
            //TimeSecs -= 5;
            TimeSinceSaved = 0;
        }

        if ((GlobalTime >= 61) && (timeStarted == true)) //(TimeSecs >= 0)
        {
            //Debug.Log("id " + id + " called saver");
            BigSaveSystem.SaveBigStack(ref SavesStack);
            timeStarted = false;
            LoadingMode = true;
        }

        //-------------------------------LOADING MODE---------------------------------------

        if (LoadingMode == true) //we end the recording and now look at what was written;
        {
            if (AnotherStack == null)
            {
                AnotherStack = BigSaveSystem.LoadAllCraneDatas();
                //Debug.Log("ZHOPA");
                TimeDates = new List<DateTime>(AnotherStack.Keys);
                CraneDatas = new List<AllCranesData>(AnotherStack.Values);
            }
                
            buttonTime += Time.deltaTime;

            //we unpack the dictionary and shit into memory

            if (Input.GetButton("Change1") && buttonTime > buttonDelta)
            {
                currentPos = (currentPos + 1) % (CraneDatas.Count);
                BigSaveSystem.SetCurrentState(ref TimeDates, ref CraneDatas, currentPos, ref CraneScripts);
                buttonTime = 0.0F;
            }


        }

    }


}
