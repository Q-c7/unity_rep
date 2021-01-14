using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public struct LocalVector
{
    public float x;
    public float y;
    public float z;
}

public struct MainSettings
{
    public bool alarmScreen;
    public bool craneInfo;
    public bool fpsCounter;
    public bool connectionStatus;
    public bool craneReadynessWindow;
    public float warningDistance;
    public float alarmDistance;
    public float scaleX;
    public float scaleY;
    public float scaleZ;
    public float weightDefaultLength;
    public float weightDefaultHeight;
    public float weightDefaultWidth;
    public float crane320_x;
    public float crane320_y;
    public float crane320_z;
    public float crane120_x;
    public float crane120_y;
    public float crane120_z;
    public float crane25_x;
    public float crane25_y;
    public float crane25_z;
    public float cameraPosition_x;
    public float cameraPosition_y;
    public float cameraPosition_z;
    public float cameraAngle_x;
    public float cameraAngle_y;
    public float cameraAngle_z;
}

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenu;
    public Text modeText;
    // Start is called before the first frame update
    //private bool flyMode = false;    
    //public Movement flyScript;
    //public CraneMovement movementScript;
    public Toggle alarmScreenToggle;
    public GameObject alarmScreen;
    public Toggle craneInfoToggle;
    public GameObject craneInfoScreen;
    public Toggle fpsToggle;
    public GameObject fpsCounter;
    public Toggle connectionStatusToggle;
    public GameObject connectionStatusWindow;
    public Toggle craneReadynessWindowToggle;
    public Toggle movementSimulationToggle;
    public GameObject movementSimulationScreen;
    public GameObject craneReadynessWindow;
    
    public Button editTraversesButton;
    public GameObject traversesEditor;
    private bool traversesEditorEnabled;

    public InputField warningDistanceEdit;
    public InputField alarmDistanceEdit;
    public InputField scaleXEdit;
    public InputField scaleYEdit;
    public InputField scaleZEdit;
    public InputField weightDefaultWidthEdit;
    public InputField weightDefaultHeightEdit;
    public InputField weightDefaultLengthEdit;
    public InputField xAdditionEdit;
    public InputField yAdditionEdit;
    public InputField zAdditionEdit;
    public MainSettings settings;
    private string paramsPrefix = "";
    

    void Start()
    {
        paramsPrefix = Storage.GetPrefix();
        modeText.text = "Режим передвижения";
        //flyScript.enabled = flyMode;
        //movementScript.enabled = !flyMode;
        if(alarmScreenToggle != null)
            alarmScreenToggle.onValueChanged.AddListener(delegate { OnAlarmScreenToggle(alarmScreenToggle); });
        if (craneInfoToggle != null)
            craneInfoToggle.onValueChanged.AddListener(delegate { OnCraneInfoToggle(craneInfoToggle); });
        if (fpsToggle != null)
            fpsToggle.onValueChanged.AddListener(delegate { OnFpsToggle(fpsToggle); });
        movementSimulationToggle?.onValueChanged.AddListener(delegate { OnMovementSimulationChange(movementSimulationToggle); });
        if(connectionStatusToggle != null)
        {
            connectionStatusToggle.onValueChanged.AddListener(delegate { OnConnectionStatusToggle(connectionStatusToggle); });
        }
        if(warningDistanceEdit != null)
        {
            warningDistanceEdit.onEndEdit.AddListener(delegate { OnWarningDistanceChange(warningDistanceEdit.text); });
        }
        if(alarmDistanceEdit != null)
        {
            alarmDistanceEdit.onEndEdit.AddListener(delegate { OnAlarmDistanceChange(alarmDistanceEdit.text); });
        }
        craneReadynessWindowToggle?.onValueChanged.AddListener(delegate { OnCraneReadynessWindowToggle(craneReadynessWindowToggle); });
        scaleXEdit?.onValueChanged.AddListener(delegate { OnKXChange(scaleXEdit.text); });
        scaleYEdit?.onValueChanged.AddListener(delegate { OnKYChange(scaleYEdit.text); });
        scaleZEdit?.onValueChanged.AddListener(delegate { OnKZChange(scaleZEdit.text); });
        weightDefaultWidthEdit?.onValueChanged.AddListener(delegate { OnDefaultWidthChange(weightDefaultWidthEdit.text); });
        weightDefaultHeightEdit?.onValueChanged.AddListener(delegate { OnDefaultHeightChange(weightDefaultHeightEdit.text); });
        weightDefaultLengthEdit?.onValueChanged.AddListener(delegate { OnDefaultLengthChange(weightDefaultLengthEdit.text); });
        //GetSettings();
        UpdateUIState();
        UpdateFields();
    }

    private void OnMovementSimulationChange(Toggle movementSimulationToggle)
    {
        movementSimulationScreen?.SetActive( movementSimulationToggle.isOn);
        if (!movementSimulationToggle.isOn)
        {
            //flyScript.enabled = false;
            //movementScript.enabled = false;
        }
        
    }

    public void GetSettings()
    {
        settings = LoadSettings();
        if (connectionStatusToggle != null)
            connectionStatusToggle.isOn = settings.connectionStatus;
        if (alarmScreenToggle != null)
            alarmScreenToggle.isOn = settings.alarmScreen;
        if (fpsToggle != null) fpsToggle.isOn = settings.fpsCounter;
        if (craneReadynessWindowToggle != null) craneReadynessWindowToggle.isOn = settings.craneReadynessWindow;
        if (craneInfoToggle != null)
            craneInfoToggle.isOn = settings.craneInfo;
        PropagateGlobals(settings);
    }

    public static MainSettings LoadSettings()
    {
        var paramsPrefix = Storage.GetPrefix();
        if (PlayerPrefs.HasKey(paramsPrefix + "settings"))
        {
            return JsonConvert.DeserializeObject<MainSettings>(PlayerPrefs.GetString(paramsPrefix + "settings"));
        }
        else
        {
            return new MainSettings();
        }
    }

    public static void PropagateGlobals(MainSettings settings)
    {
        Globals.warningDistance = settings.warningDistance;
        Globals.alarmDistance = settings.alarmDistance;
        Globals.defaultWeightHeight = settings.weightDefaultHeight;
        Globals.defaultWeightLength = settings.weightDefaultLength;
        Globals.defaultWeightWidth = settings.weightDefaultWidth;
        Globals.scaleX = settings.scaleX;
        Globals.scaleY = settings.scaleY;
        Globals.scaleZ = settings.scaleZ;

        Globals.crane320_x = settings.crane320_x;
        Globals.crane320_y = settings.crane320_y;
        Globals.crane320_z = settings.crane320_z;

        Globals.crane120_x = settings.crane120_x;
        Globals.crane120_y = settings.crane120_y;
        Globals.crane120_z = settings.crane120_z;

        Globals.crane25_x = settings.crane25_x;
        Globals.crane25_y = settings.crane25_y;
        Globals.crane25_z = settings.crane25_z;
    }

    public static void SaveSettings(MainSettings settings)
    {
        var paramsPrefix = Storage.GetPrefix();
        var json = JsonConvert.SerializeObject(settings);
        PlayerPrefs.SetString(paramsPrefix + "settings", json);
        PlayerPrefs.Save();
    }

    private void UpdateUIState()
    {
        connectionStatusWindow?.SetActive(settings.connectionStatus);
        alarmScreen?.SetActive(settings.alarmScreen);
        craneInfoScreen?.SetActive(settings.craneInfo);
        fpsCounter?.SetActive(settings.fpsCounter);
        craneReadynessWindow?.SetActive(settings.craneReadynessWindow);
        //flyScript.gameObject.transform.position = new Vector3(settings.cameraPosition_x, settings.cameraPosition_y, settings.cameraPosition_z);
        //`    fr\flyScript.gameObject.transform.eulerAngles = new Vector3(settings.cameraAngle_x, settings.cameraAngle_y, settings.cameraAngle_z);

    }

    private void OnConnectionStatusToggle(Toggle connectionStatusToggle)
    {
        settings.connectionStatus = connectionStatusToggle.isOn;
        UpdateUIState();
        SaveSettings(settings);
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetButtonDown("Cancel"))
        {
            if (traversesEditorEnabled)
            {
                traversesEditor.SetActive(false);
                traversesEditorEnabled = false;
            }
            else
            {
                mainMenu.SetActive(!mainMenu.activeSelf);
            }   
        }
        //if (Input.GetButtonDown("Tab"))
        //{
        //    if (flyMode)
        //    {
        //        settings.cameraAngle_x = flyScript.gameObject.transform.eulerAngles.x;
        //        settings.cameraAngle_y = flyScript.gameObject.transform.eulerAngles.y;
        //        settings.cameraAngle_z = flyScript.gameObject.transform.eulerAngles.z;
        //        settings.cameraPosition_x = flyScript.gameObject.transform.position.x;
        //        settings.cameraPosition_y = flyScript.gameObject.transform.position.y;
        //        settings.cameraPosition_z = flyScript.gameObject.transform.position.z;
        //        SaveSettings(settings);
        //    }
        //    if(movementSimulationToggle != null)
        //    {
        //        if (movementSimulationToggle.isOn || true)
        //        {
        //            flyMode = !flyMode;
        //            flyScript.enabled = flyMode;
        //            movementScript.enabled = !flyMode;
        //            modeText.text = flyMode ? "Режим передвижения" : "Режим управления кранами";
        //        }
        //    }            
        //}
    }

    void FixedUpdate()
    {
        
    }

    void OnAlarmScreenToggle(Toggle checkBox)
    {
        settings.alarmScreen = checkBox.isOn;
        UpdateUIState();
        SaveSettings(settings);
    }

    void OnCraneInfoToggle(Toggle checkBox)
    {
        settings.craneInfo = checkBox.isOn;
        UpdateUIState();
        SaveSettings(settings);
    }

    void OnFpsToggle(Toggle checkBox)
    {
        settings.fpsCounter = checkBox.isOn;
        UpdateUIState();
        SaveSettings(settings);
    }

    void OnCraneReadynessWindowToggle(Toggle toggle)
    {
        settings.craneReadynessWindow = toggle.isOn;
        UpdateUIState();
        SaveSettings(settings);
    }

    public void OnTraversesEdit()
    {
        traversesEditorEnabled = true;
        traversesEditor.SetActive(true);
    }

    public void LoadObstacles()
    {
        SceneManager.LoadScene("ObjectPlacer");
    }

    private void UpdateFields()
    {
        if (warningDistanceEdit != null)
            warningDistanceEdit.text = Globals.warningDistance.ToString();
        if (alarmDistanceEdit != null)
            alarmDistanceEdit.text = Globals.alarmDistance.ToString();
        if (scaleXEdit != null)
            scaleXEdit.text = Globals.scaleX.ToString();
        if (scaleYEdit != null)
            scaleYEdit.text = Globals.scaleY.ToString();
        if (scaleZEdit != null)
            scaleZEdit.text = Globals.scaleZ.ToString();
        if (weightDefaultHeightEdit != null)
            weightDefaultHeightEdit.text = Globals.defaultWeightHeight.ToString();
        if (weightDefaultLengthEdit != null)
            weightDefaultLengthEdit.text = Globals.defaultWeightLength.ToString();
        if (weightDefaultWidthEdit != null)
            weightDefaultWidthEdit.text = Globals.defaultWeightWidth.ToString();
    }

    public void OnWarningDistanceChange(string distance)
    {
        Globals.warningDistance = float.Parse(EmptyToZero(distance));
        settings.warningDistance = Globals.warningDistance;
        SaveSettings(settings);
    }

    public void OnAlarmDistanceChange(string distance)
    {
        Globals.alarmDistance = float.Parse(EmptyToZero(distance));
        settings.alarmDistance = Globals.alarmDistance;
        SaveSettings(settings);
    }

    public void OnKXChange(string distance)
    {
        var temp = EmptyToZero(distance);
        Globals.scaleX = float.Parse(temp);
        settings.scaleX = Globals.scaleX;
        SaveSettings(settings);
    }

    public void OnKYChange(string distance)
    {
        Globals.scaleY = float.Parse(EmptyToZero(distance));
        settings.scaleY = Globals.scaleY;
        SaveSettings(settings);
    }
    public void OnKZChange(string distance)
    {
        Globals.scaleZ = float.Parse(EmptyToZero(distance));
        settings.scaleZ = Globals.scaleZ;
        SaveSettings(settings);
    }

    void OnDefaultWidthChange(string width)
    {
        Globals.defaultWeightWidth = float.Parse(EmptyToZero(width));
        settings.weightDefaultWidth = Globals.defaultWeightWidth;
        SaveSettings(settings);
    }

    void OnDefaultHeightChange(string height)
    {
        Globals.defaultWeightHeight = float.Parse(EmptyToZero(height));
        settings.weightDefaultHeight = Globals.defaultWeightHeight;
        SaveSettings(settings);
    }

    void OnDefaultLengthChange(string length)
    {
        Globals.defaultWeightLength = float.Parse(EmptyToZero(length));
        settings.weightDefaultLength = Globals.defaultWeightLength;
        SaveSettings(settings);
    }

    public static string EmptyToZero(string val) => val == "" ? "0" : val;
}
