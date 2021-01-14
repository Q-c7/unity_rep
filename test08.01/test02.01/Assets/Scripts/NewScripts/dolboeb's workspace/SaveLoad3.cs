using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.UI;

public class SaveLoad3 : MonoBehaviour
{
    //public char id;
    private List<Crane> CraneScripts = new List<Crane>(new Crane[3]);
    public List<GameObject> CraneObjects = new List<GameObject>(new GameObject[3]);

    public static float TimeSinceSaved;
    public static double GlobalTime;
    public bool timeStarted = false;
    private bool LoadingMode = false;

    public static float buttonTime = 0.0F;
    private float buttonDelta = 0.2F;
    public int currentPos = 0;

    public Dictionary<DateTime, AllCranesData> SavesStack = new Dictionary<DateTime, AllCranesData>();
    public Dictionary<DateTime, AllCranesData> AnotherStack = null;
    private List<DateTime> TimeDates;
    private List<AllCranesData> CraneDatas;

    private Slider Genius_slider;
    private Text Slider_text;


    void Start()
    {
        //if ((id != '1') && (id != '2') && (id != '3'))
        //     id = '0';

        //CraneScript = GetComponent<Crane>(); 
        //---------------------------------------------------------------- we have 3 cranes now and have to link these three scripts MANUALLY-------------------------------------------------

        Genius_slider = GameObject.Find("Sl1der").GetComponent<Slider>();
        Slider_text = GameObject.Find("Sl1der").transform.Find("Text228").GetComponent<Text>();
        Slider_text.text = "Slider does NOT work before loading mode";
        Genius_slider.wholeNumbers = true;
        Genius_slider.enabled = false;
        Genius_slider.onValueChanged.AddListener(delegate { ValueChangeCheck(); });

        for (int i = 0; i < 3; i++)
        {
            //GameObject TempCrane = CraneObjects[i];
            CraneScripts[i] = CraneObjects[i].GetComponent<Crane>();
        }

        timeStarted = true;
        LoadingMode = false;

        GlobalTime = 0;
        TimeSinceSaved = 0;

        BigSaveSystem.InitCranes(); //BEAR IN MIND THAT THE PROGRAM CAN'T LOAD FILES YET BECAUSE IT WILL OVERWRITE THE SAVE FILE! IT CAN LOAD ONLY WHAT HAS JUST BEEN SAVED IN A SINGLE ITERATION
        //Debug.Log(Application.persistentDataPath);
    }

    // ebat' kakoy zhe eto K O S T Y L!!!! Current state loading is now dependant on the slider value, which is bad
    // the right thing to do is write here ONLY the slider text change, and write OnDrag() where currentPos is changed and BigSaveSystem.SetCurrentState is called. 
    void ValueChangeCheck() 
    {
        //Debug.Log(Genius_slider.value);
        currentPos = (int) Genius_slider.value;
        Slider_text.text = TimeDates[currentPos].ToString("dd MMMM yyyy hh:mm:ss tt");
        BigSaveSystem.SetCurrentState(ref TimeDates, ref CraneDatas, currentPos, ref CraneScripts);
    }

    void Update()
    {
        GlobalTime += (double)Time.deltaTime;

        if (timeStarted == true)
            TimeSinceSaved += Time.deltaTime;

        if (TimeSinceSaved >= 5) //Saving is made each 5 seconds
        {
            BigSaveSystem.ExpandBigStack(ref SavesStack, ref CraneScripts);
            //TimeSecs -= 5;
            TimeSinceSaved = 0;
        }

        if ((GlobalTime >= 61) && (timeStarted == true)) // we can easily stick some kind of a button here instead of counting 61 seconds. One day means 86400 seconds => 17280 time intervals! 
        {
            //Debug.Log("id " + id + " called saver");
            BigSaveSystem.SaveBigStack(ref SavesStack);
            timeStarted = false;
            LoadingMode = true;
        }

        //-------------------------------LOADING MODE---------------------------------------

        if (LoadingMode == true) //we end the recording and now look at what was written;
        {
            if (AnotherStack == null) //TRIGGERS ONLY ONCE!
            {
                AnotherStack = BigSaveSystem.LoadAllCraneDatas();
                //Debug.Log("ZHOPA");
                TimeDates = new List<DateTime>(AnotherStack.Keys); //we unpack the dictionary and shit into memory but that's OK i guess? Only ~10 MB of RAM for a DAY of work
                CraneDatas = new List<AllCranesData>(AnotherStack.Values);
                Genius_slider.maxValue = CraneDatas.Count - 1;
                Genius_slider.enabled = true;
                Slider_text.text = "Time not initialized. Press SPACE or pull slider";
            }
                
            buttonTime += Time.deltaTime; //somewhat redundant, it's just for having the button pressed in slight intervals

            if (Input.GetButton("Change1") && buttonTime > buttonDelta) //need to create 2 buttons for curPos + 1 // curPos - 1, 10 if pressed with SHIFT, 100 if pressed with CTRL, 1000 if SHIFT + CTRL00
            {
                currentPos = (currentPos + 1) % (CraneDatas.Count);
                Genius_slider.value = currentPos;
                //BigSaveSystem.SetCurrentState(ref TimeDates, ref CraneDatas, currentPos, ref CraneScripts); MOVED TO ValueChangeCheck() but needs to be written here so loading does not depend on slider
                buttonTime = 0.0F;
            }

        }

    }


}
