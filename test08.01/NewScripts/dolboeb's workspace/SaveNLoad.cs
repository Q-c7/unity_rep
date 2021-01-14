using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.UI;

public class SaveNLoad : MonoBehaviour
{
    public char id;
    public Crane CraneScript; //Crane.cs
    //private SaveSystem SaveSystem;
    //System.Timers.Timer LeTimer;
    //private int TimeSecs;
    private uint savenum;

    public static float TimeSinceSaved;
    public static double GlobalTime;
    public bool timeStarted = false;
    private bool LoadingMode = false;

    public static float buttonTime = 0.0F;
    private float buttonDelta = 0.2F;
    public int currentPos = 0;

    public Dictionary<DateTime, CraneData> SavesStack = new Dictionary<DateTime, CraneData>();
    public Dictionary<DateTime, CraneData> AnotherStack = null;
    public List<DateTime> TimeDates;
    public List<CraneData> CraneDatas;

    void Start()
    {
        if ((id != '1') && (id != '2') && (id != '3'))
            id = '0';
        CraneScript = GetComponent<Crane>();

        //TimeSecs = 0;

        timeStarted = true;
        LoadingMode = false;

        GlobalTime = 0;
        TimeSinceSaved = 0;

        //LeTimer = new System.Timers.Timer();
        //LeTimer.Interval = 1000;
        //LeTimer.Elapsed += (object sender, System.Timers.ElapsedEventArgs e) => TimeSecs++;
        //This function increases TimeSecs every second
        //(object sender, System.Timers.ElapsedEventArgs e) => TimeSecs++;
        //LeTimer.AutoReset = true;

        SaveSystem.InitCrane(id);
        //Debug.Log(Application.persistentDataPath);
    }

    void Update()
    {
        GlobalTime += (double)Time.deltaTime;

        if (timeStarted == true)
            TimeSinceSaved += Time.deltaTime;

        if (TimeSinceSaved >= 5) //(TimeSecs >= 0)
        {
            SaveSystem.ExpandStack(ref SavesStack, ref CraneScript);
            //TimeSecs -= 5;
            TimeSinceSaved = 0;
        }

        if ((GlobalTime >= 61) && (timeStarted == true)) //(TimeSecs >= 0)
        {
            Debug.Log("id " + id + " called saver");
            SaveSystem.SaveCraneStack(ref SavesStack, id);
            timeStarted = false;
            LoadingMode = true;
        }

        //-------------------------------LOADING MODE---------------------------------------

        if (LoadingMode == true) //we end the recording and now look at what was written;
        {
            if (AnotherStack == null)
            {
                AnotherStack = SaveSystem.LoadAllCraneDatas(id);
                Debug.Log("ZHOPA");
                TimeDates = new List<DateTime>(AnotherStack.Keys);
                CraneDatas = new List<CraneData>(AnotherStack.Values);
            }
                
            buttonTime += Time.deltaTime;

            //we unpack the dictionary and shit into memory

            if (Input.GetButton("Change1") && buttonTime > buttonDelta)
            {
                currentPos = (currentPos + 1) % (CraneDatas.Count);
                SaveSystem.SetCurrentState(ref TimeDates, ref CraneDatas, currentPos, ref CraneScript, id);
                buttonTime = 0.0F;
            }


        }

    }


}
