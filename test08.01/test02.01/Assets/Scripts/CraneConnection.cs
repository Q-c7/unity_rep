using S7ConnectLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json;

class TimerState
{

}

public enum CraneType
{
    None,
    Crane25,
    Crane120,
    Crane320,
}

struct Dummy
{
    bool dummy;
}

public struct CraneStatuses
{
    public bool powerOn;
    public bool mainContactorOn;
    public bool generalFault;
    public bool weightLimit;
    public int mode;
}

public struct CraneDisables
{
    public bool bridgeLeftWarning;
    public bool bridgeLeftAlarm;
    public bool bridgeRightAlarm;
    public bool bridgeRightWarning;

    public bool mtForwardWarning;
    public bool mtForwardAlarm;
    public bool mtBackwardAlarm;
    public bool mtBackwardWarning;

    public bool mtMhDownWarning;
    public bool mtMhDownAlarm;
    public bool mtMhUpAlarm;
    public bool mtMhUpWarning;

    public bool mtAhDownWarning;
    public bool mtAhDownAlarm;
    public bool mtAhUpAlarm;
    public bool mtAhUpWarning;

    public bool atForwardWarning;
    public bool atForwardAlarm;
    public bool atBackwardAlarm;
    public bool atBackwardWarning;

    public bool atMhDownWarning;
    public bool atMhDownAlarm;
    public bool atMhUpAlarm;
    public bool atMhUpWarning;

    public bool atAhDownWarning;
    public bool atAhDownAlarm;
    public bool atAhUpAlarm;
    public bool atAhUpWarning;
}

public struct PositionSensors
{
    public bool bridge;
    public bool mainTrolley;
    public bool auxTrolley;
    public bool mtMainHoist;
    public bool mtAuxHoist;
    public bool atMainHoist;
    public bool atAuxHoist;
}

public struct CranePositions
{
    public float bridgePosition;
    public float mtPosition;
    public float atPosition;

    public float mtMhPosition;
    public int mtMhTraverse;
    public Vector3 mtMhWeightDimensions;

    public float mtAhPosition;
    public int mtAhTraverse;
    public Vector3 mtAhWeightDimensions;

    public float atMhPosition;
    public int atMhTraverse;
    public Vector3 atMhWeightDimensions;

    public float atAhPosition;
    public int atAhTraverse;
    public Vector3 atAhWeightDimensions;
}

public struct Hoist
{
    public float position;
}

public struct NormalizedCraneStatus
{
    public CraneStatuses statuses;
    public PositionSensors sensors;
    public CranePositions dimensions;
}

public class CraneConnection : MonoBehaviour
{
    // Start is called before the first frame update
    public bool connected = false;
    public string ip = "192.168.1.35";
    public string craneID = "crane";
    public int db = 16;
    public string controller = "1500";
    public float startXtrigger = 10000.0f;
    //private bool triggerFlag = false;
    private const int triggerFrames = 2;
    //private int framesElapsed = 0;
    private S7Connect plc;
    private float delta = 0;
    public Alarms logger;
    public string craneName = "Кран 120/120 25/5";
    private AutoResetEvent autoEvent = new AutoResetEvent(true);
    private Timer reconnectTimeout;
    private bool reconnectTimerEnabled = false;
    private const float timeoutTime = 10.0f;
    private float reconnectTimerElapsed = 0;
    public Crane crane;
    public CraneType craneType;
    //public TraverseHandler traverse;
    public GameObject traversePrefab;
    public CargoDrawer mtMhWeight;
    public CargoDrawer mtAhWeight;
    public CargoDrawer atMhWeight;
    public CargoDrawer atAhWeight;
    public NormalizedCraneStatus status;
    public CraneDisables disables;
    public CraneCollisions2 collisions;
    public object sendData;
    public List<GameObject> traversesList;
    public Dictionary<int, TraverseStruct> traversesCollection;
    private const float minWeight = 100.0f;
    public object readedStruct;

    void Start()
    {
        //crane.bridgePosition = -startXtrigger;
        GetTraverses();
        Init();
        
    }

    private async void Init()
    {
        try
        {
            if(traversePrefab != null)
            {
                traversesList = new List<GameObject>();
                for (int i = 0; i < 4; i++)
                {
                    var instance = Instantiate(traversePrefab);
                    var script = instance.GetComponent<TraverseHandler>();
                    traversesList.Add(instance);
                }                
            }
        }catch(Exception e)
        {
            logger?.Alarm(craneName + " ошибка создания траверс, возможно не привязан префаб?");
            Debug.LogError(e);
        }
        try
        {
            var args = CommandLine.GetCommandLineArgs();
            if (args.ContainsKey(craneID))
            {
                ip = args[craneID];
            }
            await ReconnectToPlcAsync();
            plc.OnDisconnect += Plc_OnDisconnect;
        }
        catch (Exception e)
        {
            //Debug.LogError(e);
            logger?.Alarm(craneName + "(" + ip + ")" + ": " + e.Message);
        }
        switch (craneType)
        {
            case CraneType.None:
                break;
            case CraneType.Crane25:
                sendData = new Crane25Disables();
                break;
            case CraneType.Crane120:
                sendData = new Crane120Write();
                break;
            case CraneType.Crane320:
                sendData = new Recv();
                break;
            default:
                break;
        }
    }

    private void GetTraverses()
    {
        var paramsPrefix = Storage.GetPrefix();
        if (PlayerPrefs.HasKey(paramsPrefix + "traverses"))
        {
            var travJson = PlayerPrefs.GetString(paramsPrefix + "traverses");
            //Debug.Log(travJson);
            traversesCollection = JsonConvert.DeserializeObject<Dictionary<int, TraverseStruct>>(travJson);
        }
        else
        {
            traversesCollection = new Dictionary<int, TraverseStruct>();
        }
    }

    private void Plc_reconnectEvent()
    {
        //Debug.Log("Reconnect timeout event");
        ReconnectToPlcAsync();
        //reconnectTimeout.Dispose();
    }

    async Task ReconnectToPlcAsync()
    {
        try
        {
            plc = await S7Connect.CreateAndConnectAsync(controller, ip, 0, 1);
            logger?.Alarm(craneName + " (" + ip + "): Соединение успешно установлено", 2);
            connected = true;
        }
        catch (Exception e)
        {
            logger?.Alarm(craneName + ": " + e.Message);
            Plc_OnDisconnect();
            throw e;
        }
        
    }

    private void Plc_OnDisconnect()
    {
        Debug.Log("OnDisconnect event");
        if (plc != null)
        {
            plc = null;
        }
            
        logger?.Alarm(craneName + " (" + ip + "): Произошло отключение от ПЛК");
        reconnectTimerEnabled = true;
        reconnectTimerElapsed = 0;
        connected = false;
    }

    private void Disconnect()
    {
        if (plc != null)
        {
            plc.Close();
        }
    }

    // Update is called once per frame
    void Update()
    {
            
    }

    private void FixedUpdate()
    {
        if (reconnectTimerEnabled)
        {
            reconnectTimerElapsed += Time.deltaTime;
            if(reconnectTimerElapsed >= timeoutTime)
            {
                reconnectTimerEnabled = false;
                reconnectTimerElapsed = 0;
                Plc_reconnectEvent();
            }
        }
        delta += Time.deltaTime;
        if(delta > 0.5f)
        {
            delta = 0;
            if (plc != null && plc.Connected)
            {
                switch (craneType)
                {
                    case CraneType.None:
                        break;
                    case CraneType.Crane25:
                        HandleCrane25Async();
                        break;
                    case CraneType.Crane120:
                        HandleCrane120Async();
                        break;
                    case CraneType.Crane320:
                        HandleCrane320Async();
                        break;
                    default:
                        break;
                }
                //var craneData = plc.ReadStruct(craneStructs[(int)craneType], 16, 0);
            }
        }
    }

    private void FillNormalizedStatus(Crane120Read data)
    {
        status.statuses.powerOn = data.PowerPresent;
        status.statuses.mainContactorOn = data.MainContactorON;
        status.statuses.generalFault = data.GeneralFault;
        status.statuses.weightLimit = data.WeightLimit;
        
        status.statuses.mode = (Convert.ToInt32(data.Hoist12Mode) << 1) | (Convert.ToInt32(data.Hoist123Mode) << 2);

        status.sensors.bridge = data.BridgePositionSensorOK;
        status.sensors.mainTrolley = data.MainTrolleyPositionSensorOK;
        status.sensors.auxTrolley = data.AuxTrolleyPositionSensorOK;
        status.sensors.mtMainHoist = data.MainTrolleyMainHoistSensorOK;
        status.sensors.mtAuxHoist = data.MainTrolleyAuxHoistSensorOK;
        status.sensors.atMainHoist = data.AuxTrolleyMainHoistSensorOK;
        status.sensors.atAuxHoist = data.AuxTrolleyAuxHoistSensorOK;

        status.dimensions.bridgePosition = data.bridgePosition;
        status.dimensions.mtPosition = data.mainTrolleyPosition;

        status.dimensions.mtMhPosition = data.mainTrolleyMainHoistPosition;
        status.dimensions.mtMhTraverse = data.mtMhTraverseNumber;
        status.dimensions.mtMhWeightDimensions = new Vector3(data.mtMhLoadDimensionX, data.mtMhloadDimensionY, data.mtMhloadDimensionZ);

        status.dimensions.mtAhPosition = data.mainTrolleyAuxHoistPosition;
        status.dimensions.mtAhTraverse = data.mtMhTraverseNumber;
        status.dimensions.mtAhWeightDimensions = new Vector3(data.mtAhLoadDimensionX, data.mtAhloadDimensionY, data.mtAhloadDimensionZ);

        status.dimensions.atPosition = data.auxTrolleyPosition;

        status.dimensions.atMhPosition = data.auxTrolleyMainHoistPosition;
        status.dimensions.atMhTraverse = data.atMhTraverseNumber;
        status.dimensions.atMhWeightDimensions = new Vector3(data.atMhLoadDimensionX, data.atMhloadDimensionY, data.atMhloadDimensionZ);

        status.dimensions.atAhPosition = data.auxTrolleyAuxHoistPosition;
        status.dimensions.atAhTraverse = data.atAhTraverseNumber;
        status.dimensions.atAhWeightDimensions = new Vector3(data.atAhLoadDimensionX, data.atAhloadDimensionY, data.atAhloadDimensionZ);
    }

    private void FillNormalizedStatus(Send data)
    {
        status.statuses.powerOn = data.craneState.general.PowerPresent;
        status.statuses.mainContactorOn = data.craneState.general.MainContactorON;
        status.statuses.generalFault = data.craneState.general.GeneralFault;
        status.statuses.weightLimit = data.craneState.general.WeightLimit;

        status.statuses.mode = (Convert.ToInt32(data.craneState.general.Hoist12Mode) << 1) | (Convert.ToInt32(data.craneState.general.Hoist123Mode) << 2);

        status.sensors.bridge = data.craneState.general.BridgePositionSensorOK;
        status.sensors.mainTrolley = data.craneState.general.MainTrolleyPositionSensorOK;
        status.sensors.auxTrolley = data.craneState.general.AuxTrolleyPositionSensorOK;
        status.sensors.mtMainHoist = data.craneState.general.MainTrolleyMainHoistSensorOK;
        status.sensors.mtAuxHoist = data.craneState.general.MainTrolleyAuxHoistSensorOK;
        status.sensors.atMainHoist = data.craneState.general.AuxTrolleyMainHoistSensorOK;
        status.sensors.atAuxHoist = data.craneState.general.AuxTrolleyAuxHoistSensorOK;

        status.dimensions.bridgePosition = data.bridgePosition;
        status.dimensions.mtPosition = data.mainTrolleyPosition;

        status.dimensions.mtMhPosition = data.mtMh.position;
        status.dimensions.mtMhTraverse = data.mtMh.traverse;
        status.dimensions.mtMhWeightDimensions = new Vector3(data.mtMh.Size_X, data.mtMh.Size_Y, data.mtMh.Size_Z);

        status.dimensions.mtAhPosition = data.mtAh.position;
        status.dimensions.mtAhTraverse = data.mtAh.traverse;
        status.dimensions.mtAhWeightDimensions = new Vector3(data.mtAh.Size_X, data.mtAh.Size_Y, data.mtAh.Size_Z);

        status.dimensions.atPosition = data.auxTrolleyPosition;

        status.dimensions.atMhPosition = data.atMh.position;
        status.dimensions.atMhTraverse = data.atMh.traverse;
        status.dimensions.atMhWeightDimensions = new Vector3(data.atMh.Size_X, data.atMh.Size_Y, data.atMh.Size_Z);

        status.dimensions.atAhPosition = 0;
        status.dimensions.atAhTraverse = -1;
        status.dimensions.atAhWeightDimensions = new Vector3(0,0,0);
    }

    private void FillNormalizedStatus(Crane25Read data)
    {
        status.statuses.powerOn = data.PowerPresent;
        status.statuses.mainContactorOn = data.MainContactorON;
        status.statuses.generalFault = data.GeneralFault;
        status.statuses.weightLimit = data.WeightLimit;

        status.statuses.mode = 0;

        status.sensors.bridge = data.BridgePositionSensorOK;
        status.sensors.mainTrolley = data.MainTrolleyPositionSensorOK;
        status.sensors.auxTrolley = true;
        status.sensors.mtMainHoist = data.MainTrolleyMainHoistSensorOK;
        status.sensors.mtAuxHoist = data.MainTrolleyAuxHoistSensorOK;
        status.sensors.atMainHoist = true;
        status.sensors.atAuxHoist = true;

        status.dimensions.bridgePosition = data.bridgePosition;
        status.dimensions.mtPosition = data.mainTrolleyPosition;

        status.dimensions.mtMhPosition = data.mainTrolleyMainHoistPosition;
        status.dimensions.mtMhTraverse = -1;
        status.dimensions.mtMhWeightDimensions = new Vector3(data.mtMhLoadDimensionX, data.mtMhloadDimensionY, data.mtMhloadDimensionZ);

        status.dimensions.mtAhPosition = data.mainTrolleyAuxHoistPosition;
        status.dimensions.mtAhTraverse = -1;
        status.dimensions.mtAhWeightDimensions = new Vector3(data.mtAhLoadDimensionX, data.mtAhloadDimensionY, data.mtAhloadDimensionZ);

        status.dimensions.atPosition = 0;

        status.dimensions.atMhPosition = 0;
        status.dimensions.atMhTraverse = -1;
        status.dimensions.atMhWeightDimensions = new Vector3();

        status.dimensions.atAhPosition = 0;
        status.dimensions.atAhTraverse = -1;
        status.dimensions.atAhWeightDimensions = new Vector3();
    }

    private async void HandleCrane120Async()
    {
        //var craneData = await plc.ReadStructAsync<Crane120Read>(db);
        //Task<Crane120Read?> craneDataTask;
        try
        {
            var craneData = plc.ReadStruct<Crane120Read>(db);
            readedStruct = craneData;
            //craneDataTask = plc.ReadStructAsync<Crane120Read>(db);
            if (craneData != null)
            {
                if (crane != null)
                {
                    //crane.mtMhMaxHeight = Globals.hoistsMaxHeight;
                    crane.bridgePosition = craneData.Value.bridgePosition;
                    crane.mtPosition = craneData.Value.mainTrolleyPosition;
                    crane.atPosition = craneData.Value.auxTrolleyPosition;
                    crane.mtMhPosition = craneData.Value.mainTrolleyMainHoistPosition;
                    crane.mtAhPosition = craneData.Value.mainTrolleyAuxHoistPosition;
                    crane.atMhPosition = craneData.Value.auxTrolleyMainHoistPosition;
                    crane.atAhPosition = craneData.Value.auxTrolleyAuxHoistPosition;

                    FillNormalizedStatus(craneData.Value);
                    
                }
                if (traversesList != null)
                {
                    var traverseIdx = 0; //Main trolley main hoist
                    var betweenHooks = 1.0f;
                    var twoHooksAddition = 0.0f;
                    bool twoHooks = craneData.Value.Hoist12Mode || craneData.Value.Hoist123Mode;
                    if (traversesCollection.ContainsKey(craneData.Value.mtMhTraverseNumber) && twoHooks)
                    {
                        if (traversesCollection[craneData.Value.mtMhTraverseNumber].twoHooks)
                        {
                            twoHooksAddition = betweenHooks / 2;
                        }
                        else
                            twoHooks = false;
                    }
                    SetTraversePosition(new TraverseCalcStruct()
                    {
                        idx = traverseIdx,
                        show = traversesCollection.ContainsKey(craneData.Value.mtMhTraverseNumber),
                        x = craneData.Value.bridgePosition,
                        y = craneData.Value.mainTrolleyMainHoistPosition,
                        z = Globals.zZeroFix320 - craneData.Value.mainTrolleyPosition,
                        traverseNum = craneData.Value.mtMhTraverseNumber,
                        diff = new Vector3(0.064f + twoHooksAddition, -0.12f, -0.48f)
                    });

                    traverseIdx = 1; //Main trolley aux hoist
                    SetTraversePosition(new TraverseCalcStruct()
                    {
                        idx = traverseIdx,
                        show = traversesCollection.ContainsKey(craneData.Value.mtAhTraverseNumber) && !twoHooks,
                        x = craneData.Value.bridgePosition,
                        y = craneData.Value.mainTrolleyAuxHoistPosition,
                        z = Globals.zZeroFix320 - craneData.Value.mainTrolleyPosition,
                        traverseNum = craneData.Value.mtAhTraverseNumber,
                        diff = new Vector3(0.064f + betweenHooks, -0.12f, -0.48f)
                    });

                    traverseIdx = 2; //Aux trolley aux hoist
                    SetTraversePosition(new TraverseCalcStruct()
                    {
                        idx = traverseIdx,
                        show = traversesCollection.ContainsKey(craneData.Value.atAhTraverseNumber),
                        x = craneData.Value.bridgePosition,
                        y = craneData.Value.auxTrolleyMainHoistPosition,
                        z = Globals.zZeroFix320 - craneData.Value.auxTrolleyPosition,
                        traverseNum = craneData.Value.atMhTraverseNumber,
                        diff = new Vector3(0.064f + betweenHooks/2, -0.12f, -0.48f-0.06f)
                    });
                }
                UpdateWeight(craneData);
            }
        }
        catch (Exception e)
        {
            Plc_OnDisconnect();
            Debug.LogError(e);
            return;
        }
        
        
        Crane120Write recv = (Crane120Write)sendData;
            
        recv.bridgeRightSlowDown                = disables.bridgeRightWarning;
        recv.bridgeRightStop                    = disables.bridgeRightAlarm;
        recv.bridgeLeftSlowDown                 = disables.bridgeLeftWarning;
        recv.bridgeLeftStop                     = disables.bridgeLeftAlarm;

        recv.mainTrolleyForwardSlowDown         = disables.mtForwardWarning;
        recv.mainTrolleyForwardStop             = disables.mtForwardAlarm;
        recv.mainTrolleyBackwardSlowDown        = disables.mtBackwardWarning;
        recv.mainTrolleyBackwardStop            = disables.mtBackwardAlarm;

        recv.auxTrolleyForwardSlowDown          = disables.atBackwardWarning;
        recv.auxTrolleyForwardStop              = disables.atForwardAlarm;
        recv.auxTrolleyBackwardSlowDown         = disables.atBackwardWarning;
        recv.auxTrolleyBackwardStop             = disables.atBackwardWarning;

        recv.mainHoistMainTrolleyUpSlowDown     = disables.mtMhUpWarning;
        recv.mainHoistMainTrolleyUpStop         = disables.mtMhUpAlarm;
        recv.mainHoistMainTrolleyDownSlowDown   = disables.mtMhDownWarning;
        recv.mainHoistMainTrolleyDownStop       = disables.mtMhDownAlarm;

        recv.auxHoistMainTrolleyUpSlowDown      = disables.mtAhUpWarning;
        recv.auxHoistMainTrolleyUpStop          = disables.mtAhUpAlarm;
        recv.auxHoistMainTrolleyDownSlowDown    = disables.mtAhDownWarning;
        recv.auxHoistMainTrolleyDownStop        = disables.mtAhDownAlarm;

        recv.mainHoistAuxTrolleyUpSlowDown      = disables.atMhUpWarning;
        recv.mainHoistAuxTrolleyUpStop          = disables.atMhUpAlarm;
        recv.mainHoistAuxTrolleyDownSlowDown    = disables.atMhDownWarning;
        recv.mainHoistAuxTrolleyDownStop        = disables.atMhDownAlarm;

        recv.auxHoistAuxTrolleyUpSlowDown       = disables.atAhUpWarning;
        recv.auxHoistAuxTrolleyUpStop           = disables.atAhUpAlarm;
        recv.auxHoistAuxTrolleyDownSlowDown     = disables.atAhDownWarning;
        recv.auxHoistAuxTrolleyDownStop         = disables.atAhDownAlarm;

        try
        {
            await plc.WriteStructAsync(recv, db, 120);
        }
        catch (Exception e)
        {
            Plc_OnDisconnect();
            Debug.LogError(e);
        }
        sendData = recv;

        
    }

    private async void HandleCrane320Async()
    {
        //Task<Crane320?> craneDataTask;
        try{
            var craneData = plc.ReadStruct<Crane320>(db);
            readedStruct = craneData;
            //craneDataTask = plc.ReadStructAsync<Crane320>(db);
            if (craneData != null)
            {
                if (crane != null)
                {
                    //crane.yZeroFix = Globals.yZeroFix320;
                    //crane.mtMhMaxHeight = Globals.hoistsMaxHeight320;
                    crane.bridgePosition = craneData.Value.Send.bridgePosition;
                    crane.mtPosition = craneData.Value.Send.mainTrolleyPosition;
                    crane.atPosition = craneData.Value.Send.auxTrolleyPosition;
                    crane.mtMhPosition = craneData.Value.Send.mtMh.position;
                    crane.mtAhPosition = craneData.Value.Send.mtAh.position;
                    crane.atMhPosition = craneData.Value.Send.atMh.position;

                    FillNormalizedStatus(craneData.Value.Send);
                }
                if (traversesList != null)
                {
                    var traverseIdx = 0; //Main trolley main hoist
                    var betweenHooks = 1.122f;
                    var twoHooksAddition = 0.0f;
                    bool twoHooks = craneData.Value.Send.craneState.general.Hoist12Mode || craneData.Value.Send.craneState.general.Hoist123Mode;
                    if (traversesCollection.ContainsKey(craneData.Value.Send.mtMh.traverse) && twoHooks)
                    {
                        if (traversesCollection[craneData.Value.Send.mtMh.traverse].twoHooks)
                        {
                            twoHooksAddition = betweenHooks / 2;
                        }
                        else
                            twoHooks = false;
                    }
                    SetTraversePosition(new TraverseCalcStruct() {
                        idx = traverseIdx,
                        show = traversesCollection.ContainsKey(craneData.Value.Send.mtMh.traverse),
                        x = craneData.Value.Send.bridgePosition,
                        y = craneData.Value.Send.mtMh.position,
                        z = Globals.zZeroFix320 - craneData.Value.Send.mainTrolleyPosition,
                        traverseNum = craneData.Value.Send.mtMh.traverse,
                        diff = new Vector3(0.147f + betweenHooks - twoHooksAddition, 0, -0.4f)
                    });

                    traverseIdx = 1; //Main trolley aux hoist
                    SetTraversePosition(new TraverseCalcStruct()
                    {
                        idx = traverseIdx,
                        show = traversesCollection.ContainsKey(craneData.Value.Send.mtAh.traverse) && !twoHooks,
                        x = craneData.Value.Send.bridgePosition,
                        y = craneData.Value.Send.mtAh.position,
                        z = Globals.zZeroFix320 - craneData.Value.Send.mainTrolleyPosition,
                        traverseNum = craneData.Value.Send.mtAh.traverse,
                        diff = new Vector3(0.147f, 0, -0.4f)
                    });

                    traverseIdx = 2; //Aux trolley aux hoist
                    SetTraversePosition(new TraverseCalcStruct()
                    {
                        idx = traverseIdx,
                        show = traversesCollection.ContainsKey(craneData.Value.Send.atMh.traverse),
                        x = craneData.Value.Send.bridgePosition,
                        y = craneData.Value.Send.atMh.position,
                        z = Globals.zZeroFix320 - craneData.Value.Send.auxTrolleyPosition,
                        traverseNum = craneData.Value.Send.atMh.traverse,
                        diff = new Vector3(0.717f, 0, -0.178f)
                    });
                }
                UpdateWeight(craneData);
            }
        }
        catch (Exception e)
        {
            Plc_OnDisconnect();
            Debug.LogError(e);
            return;
        }
        
        Recv recv = (Recv)sendData;
        recv.systemOk = true;
        recv.number += 1;
        recv.slowDown.bridgeRightSlowDown               = disables.bridgeRightWarning;
        recv.stop.bridgeRightStop                       = disables.bridgeRightAlarm;
        recv.slowDown.bridgeLeftSlowDown                = disables.bridgeLeftWarning;
        recv.stop.bridgeLeftStop                        = disables.bridgeLeftAlarm;

        recv.slowDown.mainTrolleyForwardSlowDown        = disables.mtForwardWarning;
        recv.stop.mainTrolleyForwardStop                = disables.mtForwardAlarm;
        recv.slowDown.mainTrolleyBackwardSlowDown       = disables.mtBackwardWarning;
        recv.stop.mainTrolleyBackwardStop               = disables.mtBackwardAlarm;

        recv.slowDown.auxTrolleyForwardSlowDown         = disables.atBackwardWarning;
        recv.stop.auxTrolleyForwardStop                 = disables.atForwardAlarm;
        recv.slowDown.auxTrolleyBackwardSlowDown        = disables.atBackwardWarning;
        recv.stop.auxTrolleyBackwardStop                = disables.atBackwardWarning;

        recv.slowDown.mainHoistMainTrolleyUpSlowDown    = disables.mtMhUpWarning;
        recv.stop.mainHoistMainTrolleyUpStop            = disables.mtMhUpAlarm;
        recv.slowDown.mainHoistMainTrolleyDownSlowDown  = disables.mtMhDownWarning;
        recv.stop.mainHoistMainTrolleyDownStop          = disables.mtMhDownAlarm;

        recv.slowDown.auxHoistMainTrolleyUpSlowDown     = disables.mtAhUpWarning;
        recv.stop.auxHoistMainTrolleyUpStop             = disables.mtAhUpAlarm;
        recv.slowDown.auxHoistMainTrolleyDownSlowDown   = disables.mtAhDownWarning;
        recv.stop.auxHoistMainTrolleyDownStop           = disables.mtAhDownAlarm;

        recv.slowDown.mainHoistAuxTrolleyUpSlowDown     = disables.atMhUpWarning;
        recv.stop.mainHoistAuxTrolleyUpStop             = disables.atMhUpAlarm;
        recv.slowDown.mainHoistAuxTrolleyDownSlowDown   = disables.atMhDownWarning;
        recv.stop.mainHoistAuxTrolleyDownStop           = disables.atMhDownAlarm;
        try
        {                                               
            await plc.WriteStructAsync(recv, db);       
        }                                               
        catch (Exception e)                             
        {
            Plc_OnDisconnect();
            Debug.LogError(e);
        }
        sendData = recv;

    }

    struct TraverseCalcStruct
    {
        public int idx;
        public bool show;
        public float x;
        public float y;
        public float z;
        public int traverseNum;
        public Vector3 diff;
    }

    private void SetTraversePosition(TraverseCalcStruct traverse)
    {
        var mtMhTraverseScript = traversesList[traverse.idx].GetComponent<TraverseHandler>();

        mtMhTraverseScript.show = traverse.show;//craneData.Value.Send.mtMh.traverse != 0;
        mtMhTraverseScript.currentPosition.x = traverse.x;//craneData.Value.Send.bridgePosition;
        //traverse.x = traverse.currentPosition.x * Globals.scaleX;
        /*if (craneData.Value.Send.craneState.general.Hoist12Mode)
        {
            mtMhTraverseScript.currentPosition.y = (craneData.Value.Send.mtMh.position + craneData.Value.Send.mtAh.position) / 2;
            //traverse.y = (traverse.currentPosition.y - Globals.hoistsMaxHeight320) * Globals.scaleY;
            mtMhTraverseScript.currentPosition.z = craneData.Value.Send.mainTrolleyPosition;
            //traverse.z = traverse.currentPosition.z * Globals.scaleZ;
        }
        else if (craneData.Value.Send.craneState.general.Hoist123Mode)
        {
            mtMhTraverseScript.currentPosition.y = (craneData.Value.Send.mtMh.position + craneData.Value.Send.mtAh.position + craneData.Value.Send.atMh.position) / 3;
            //traverse.y = (traverse.currentPosition.y - Globals.hoistsMaxHeight320) * Globals.scaleY;
            mtMhTraverseScript.currentPosition.z = (craneData.Value.Send.mainTrolleyPosition + craneData.Value.Send.auxTrolleyPosition) / 2;
            //traverse.z = Globals.scaleZ * traverse.currentPosition.z;
        }
        else
        {*/
        //craneData.Value.Send.mtMh.position;
        mtMhTraverseScript.currentPosition.z = traverse.z;//craneData.Value.Send.mainTrolleyPosition;
        mtMhTraverseScript.diff = traverse.diff;//new Vector3(0.147f, -0.041f, 0.4f);
        //}

        mtMhTraverseScript.currentPosition *= Globals.scaleX;
        if (traversesCollection.ContainsKey(/*craneData.Value.Send.mtMh.traverse*/ traverse.traverseNum))
        {
            mtMhTraverseScript.currentPosition.y = (traverse.y - traversesCollection[traverse.traverseNum].height / 2) * Globals.scaleY;
            mtMhTraverseScript.traverseParams = traversesCollection[traverse.traverseNum];
        }
    }

    private async void HandleCrane25Async()
    {
        //Task<Crane25Read?> craneDataTask;
        try
        {
            var craneData = plc.ReadStruct<Crane25Read>(db);
            readedStruct = craneData;
            if (craneData != null)
            {
                if (crane != null)
                {
                    //crane.mtMhMaxHeight = Globals.hoistsMaxHeight;
                    crane.bridgePosition = craneData.Value.bridgePosition;
                    crane.mtPosition = craneData.Value.mainTrolleyPosition;
                    crane.mtMhPosition = craneData.Value.mainTrolleyMainHoistPosition;
                    crane.mtAhPosition = craneData.Value.mainTrolleyAuxHoistPosition;

                    FillNormalizedStatus(craneData.Value);
                }
                UpdateWeight(craneData);
            }
        }
        catch (Exception e)
        {
            Plc_OnDisconnect();
            Debug.LogError(e);
            return;
        }        

        if (collisions)
        {
            Crane25Write recv = new Crane25Write();

            //bridge
            recv.bridgeBackwardWarning      = disables.bridgeRightWarning;
            recv.bridgeBackwardDisable      = disables.bridgeRightAlarm;
            recv.bridgeForwardWarning       = disables.bridgeLeftWarning;
            recv.bridgeForwardDisable       = disables.bridgeLeftAlarm;

            //main trolley
            recv.mainTrolleyRightWarning    = disables.mtForwardWarning;
            recv.mainTrolleyRightDisable    = disables.mtForwardAlarm;
            recv.mainTrolleyLeftWarning     = disables.mtBackwardWarning;
            recv.mainTrolleyLeftDisable     = disables.mtBackwardAlarm;

            //main hoist
            recv.mtMhDownWarning            = disables.mtMhUpWarning;
            recv.mtMhDownDisable            = disables.mtMhUpAlarm;
            recv.mtMhUpWarning              = disables.mtMhDownWarning;
            recv.mtMhUpDisable              = disables.mtMhDownAlarm;

            //main hoist
            recv.mtAhDownWarning            = disables.mtAhUpWarning;
            recv.mtAhDownDisable            = disables.mtAhUpAlarm;
            recv.mtAhUpWarning              = disables.mtAhDownWarning;
            recv.mtAhUpDisable              = disables.mtAhDownAlarm;

            try
            {
                await plc.WriteStructAsync(recv, db, 72);
            }
            catch (Exception e)
            {
                Plc_OnDisconnect();
                Debug.LogError(e);
            }
            sendData = recv;
            
        }
    }

    private void UpdateWeight(Crane120Read? craneData)
    {
        var twoHooks = craneData.Value.Hoist12Mode;
        var threeHooks = craneData.Value.Hoist123Mode;
        const float distanceBetweenHooks = 1.0f;
        var xCorrection = (twoHooks || threeHooks) ? 0.06f + distanceBetweenHooks / 2 : 0.06f;

        const float weightZCorrection = 0.185f;
        float zCorrection = -weightZCorrection;
        float yCorrection = -0.12f;
        float hookScale = 0.2f;
        float distanceBetweenTrolleys = (craneData.Value.mainTrolleyPosition - craneData.Value.auxTrolleyPosition) * Globals.scaleZ;
        Vector3 traverseSize = new Vector3();
        if (threeHooks)
            zCorrection += distanceBetweenTrolleys / 2;
        if (traversesCollection.ContainsKey(craneData.Value.mtMhTraverseNumber))
        {
            var traverse = traversesCollection[craneData.Value.mtMhTraverseNumber];
            yCorrection -= traverse.height * Globals.scaleY;
            traverseSize = new Vector3(traverse.length * Globals.scaleX, traverse.height * Globals.scaleY, traverse.width * Globals.scaleZ);
        }
        var overallSize = new Vector3();
        var useOverall = false;
        var overallPosition = new Vector3();
        var showWeight = craneData.Value.mtMhLoadDimensionX > 0 && craneData.Value.mtMhloadDimensionY > 0 && craneData.Value.mtMhloadDimensionZ > 0;

        if (twoHooks || threeHooks)
        {
            overallSize = new Vector3(
                Math.Max(craneData.Value.mtMhLoadDimensionX * Globals.scaleX, Math.Max(traverseSize.x, distanceBetweenHooks)),
                //traverseSize.y > craneData.Value.Send.mtMh.Size_Y * Globals.scaleY ? traverseSize.y : craneData.Value.Send.mtMh.Size_Y * Globals.scaleY,
                Math.Max(Math.Max(craneData.Value.mtMhloadDimensionY * Globals.scaleY, traverseSize.y), Math.Abs(distanceBetweenTrolleys) * (threeHooks ? 1 : 0)),
                traverseSize.z + craneData.Value.mtMhloadDimensionZ * Globals.scaleZ + 0.15f
            );
            useOverall = true;
            overallPosition = new Vector3(distanceBetweenHooks / hookScale / 2, 0, -zCorrection - weightZCorrection);
        }
        else if (traversesCollection.ContainsKey(craneData.Value.mtMhTraverseNumber))
        {
            overallSize = new Vector3(
                Math.Max(craneData.Value.mtMhLoadDimensionX * Globals.scaleX, traverseSize.x),
                //traverseSize.y > craneData.Value.Send.mtMh.Size_Y * Globals.scaleY ? traverseSize.y : craneData.Value.Send.mtMh.Size_Y * Globals.scaleY,
                Math.Max(craneData.Value.mtMhloadDimensionY * Globals.scaleY, traverseSize.y),
                traverseSize.z + craneData.Value.mtMhloadDimensionZ * Globals.scaleZ + 0.15f
            );
            useOverall = true;
        }

        ActivateColliders(mtMhWeight, showWeight);

        CalculateWeightPosition(new WeightDrawerArgs
        {
            constDiff = new Vector3(xCorrection, yCorrection, zCorrection),
            bridgePosition = craneData.Value.bridgePosition,
            trolleyPosition = craneData.Value.mainTrolleyPosition,
            hoistPosition = craneData.Value.mainTrolleyMainHoistPosition,
            size_x = craneData.Value.mtMhLoadDimensionX,
            size_z = craneData.Value.mtMhloadDimensionY,
            size_y = craneData.Value.mtMhloadDimensionZ,
            weightObject = mtMhWeight,
            show = showWeight,
            useOverallSize = useOverall,
            overallSize = overallSize,
            overallPosition = overallPosition,
        });

        useOverall = false;
        showWeight = craneData.Value.mtAhLoadDimensionX > 0 && craneData.Value.mtAhloadDimensionY > 0 && craneData.Value.mtAhloadDimensionZ > 0
            && !craneData.Value.Hoist12Mode && !craneData.Value.Hoist123Mode;
        if (traversesCollection.ContainsKey(craneData.Value.mtAhTraverseNumber))
        {
            var traverse = traversesCollection[craneData.Value.mtAhTraverseNumber];
            yCorrection = -traverse.height * Globals.scaleY;
            traverseSize = new Vector3(traverse.length * Globals.scaleX, traverse.height * Globals.scaleY, traverse.width * Globals.scaleZ);
        }
        if (traversesCollection.ContainsKey(craneData.Value.mtAhTraverseNumber) && !twoHooks && !threeHooks)
        {
            overallSize = new Vector3(
                Math.Max(craneData.Value.mtAhLoadDimensionX * Globals.scaleX * (showWeight ? 1 : 0), traverseSize.x),
                //traverseSize.y > craneData.Value.Send.mtMh.Size_Y * Globals.scaleY ? traverseSize.y : craneData.Value.Send.mtMh.Size_Y * Globals.scaleY,
                Math.Max(craneData.Value.mtAhloadDimensionY * Globals.scaleY * (showWeight ? 1 : 0), traverseSize.y),
                traverseSize.z + craneData.Value.mtAhloadDimensionZ * Globals.scaleZ * (showWeight ? 1 : 0) + 0.15f
            );
            useOverall = true;
            overallPosition = new Vector3(0, 0, 0);
        }

        ActivateColliders(mtAhWeight, showWeight);

        CalculateWeightPosition(new WeightDrawerArgs
        {
            constDiff = new Vector3(1.03f, -0.12f, -0.185f),
            bridgePosition = craneData.Value.bridgePosition,
            trolleyPosition = craneData.Value.mainTrolleyPosition,
            hoistPosition = craneData.Value.mainTrolleyAuxHoistPosition,
            size_x = craneData.Value.mtAhLoadDimensionX,
            size_z = craneData.Value.mtAhloadDimensionY,
            size_y = craneData.Value.mtAhloadDimensionZ,
            weightObject = mtAhWeight,
            show = showWeight,
            useOverallSize = useOverall,
            overallSize = overallSize,
            overallPosition = overallPosition,
        });

        useOverall = false;
        showWeight = craneData.Value.atMhLoadDimensionX > 0 && craneData.Value.atMhloadDimensionY > 0 && craneData.Value.atMhloadDimensionZ > 0 && !craneData.Value.Hoist123Mode;
        if (traversesCollection.ContainsKey(craneData.Value.atMhTraverseNumber))
        {
            var traverse = traversesCollection[craneData.Value.atMhTraverseNumber];
            yCorrection = -traverse.height * Globals.scaleY;
            traverseSize = new Vector3(traverse.length * Globals.scaleX, traverse.height * Globals.scaleY, traverse.width * Globals.scaleZ);
        }
        if (traversesCollection.ContainsKey(craneData.Value.atMhTraverseNumber) && !threeHooks)
        {
            overallSize = new Vector3(
                Math.Max(craneData.Value.atMhLoadDimensionX * Globals.scaleX * (showWeight ? 1 : 0), traverseSize.x),
                //traverseSize.y > craneData.Value.Send.mtMh.Size_Y * Globals.scaleY ? traverseSize.y : craneData.Value.Send.mtMh.Size_Y * Globals.scaleY,
                Math.Max(craneData.Value.atMhloadDimensionY * Globals.scaleY * (showWeight ? 1 : 0), traverseSize.y),
                traverseSize.z + craneData.Value.atMhloadDimensionZ * Globals.scaleZ * (showWeight ? 1 : 0) + 0.15f
            );
            useOverall = true;
            overallPosition = new Vector3(0, 0, 0);
        }

        ActivateColliders(atMhWeight, showWeight);

        CalculateWeightPosition(new WeightDrawerArgs
        {
            constDiff = new Vector3(0.55f, -0.15f, -0.222f),
            bridgePosition = craneData.Value.bridgePosition,
            trolleyPosition = craneData.Value.auxTrolleyPosition,
            hoistPosition = craneData.Value.auxTrolleyMainHoistPosition,
            size_x = craneData.Value.atMhLoadDimensionX,
            size_z = craneData.Value.atMhloadDimensionY,
            size_y = craneData.Value.atMhloadDimensionZ,
            weightObject = atMhWeight,
            show = showWeight,
            useOverallSize = useOverall,
            overallSize = overallSize,
            overallPosition = overallPosition,
        });

        useOverall = false;
        showWeight = craneData.Value.atAhLoadDimensionX > 0 && craneData.Value.atAhloadDimensionY > 0 && craneData.Value.atAhloadDimensionZ > 0;
        if (traversesCollection.ContainsKey(craneData.Value.atAhTraverseNumber))
        {
            var traverse = traversesCollection[craneData.Value.atAhTraverseNumber];
            yCorrection = -traverse.height * Globals.scaleY;
            traverseSize = new Vector3(traverse.length * Globals.scaleX, traverse.height * Globals.scaleY, traverse.width * Globals.scaleZ);
        }
        if (traversesCollection.ContainsKey(craneData.Value.atAhTraverseNumber) && !threeHooks)
        {
            overallSize = new Vector3(
                Math.Max(craneData.Value.atAhLoadDimensionX * Globals.scaleX * (showWeight ? 1 : 0), traverseSize.x),
                //traverseSize.y > craneData.Value.Send.mtMh.Size_Y * Globals.scaleY ? traverseSize.y : craneData.Value.Send.mtMh.Size_Y * Globals.scaleY,
                Math.Max(craneData.Value.atAhloadDimensionY * Globals.scaleY * (showWeight ? 1 : 0), traverseSize.y),
                traverseSize.z + craneData.Value.atAhloadDimensionZ * Globals.scaleZ * (showWeight ? 1 : 0) + 0.15f
            );
            useOverall = true;
            overallPosition = new Vector3(0, 0, 0);
        }

        ActivateColliders(atAhWeight, showWeight);

        CalculateWeightPosition(new WeightDrawerArgs
        {
            constDiff = new Vector3(0.55f, -0.15f, -0.0f),
            bridgePosition = craneData.Value.bridgePosition,
            trolleyPosition = craneData.Value.auxTrolleyPosition,
            hoistPosition = craneData.Value.auxTrolleyAuxHoistPosition,
            size_x = craneData.Value.atAhLoadDimensionX,
            size_z = craneData.Value.atAhloadDimensionY,
            size_y = craneData.Value.atAhloadDimensionZ,
            weightObject = atAhWeight,
            show = showWeight,
            useOverallSize = useOverall,
            overallSize = overallSize,
            overallPosition = overallPosition,
        });

    }

    private static void ActivateColliders(CargoDrawer weight, bool enable)
    {
        if (weight != null)
        {
            var colliders = weight.gameObject.GetComponentsInChildren<CollisionDetector2>(true);
            foreach (var collider in colliders)
            {
                //var boxCollider = collider.gameObject.GetComponent<BoxCollider>();
                //boxCollider.enabled = enable;
                var colliderSizeScript = collider.gameObject.GetComponent<CollidersSizeCalc>();
                if (colliderSizeScript != null && !colliderSizeScript.isWeightColliders && colliderSizeScript.type != ColliderType.Down) continue;
                /*if (!enable)
                {
                    collider.triggered = false;
                }*/
                collider.colliderEnabled = enable;
            }
            /*foreach (var collider in colliders)
            {
                if (!enable && collider.triggeredWith != null)
                {
                    collider.SetUntriggered(collider.triggeredWith);
                }
            }*/
        }
    }

    struct WeightDrawerArgs
    {
        public bool show;
        //public float weight;
        public CargoDrawer weightObject;
        public float bridgePosition;
        public float trolleyPosition;
        public float hoistPosition;
        public float size_x;
        public float size_y;
        public float size_z;
        public Vector3 constDiff;
        public bool useOverallSize;
        public Vector3 overallSize;
        public Vector3 overallPosition;
        //public TraverseStruct? traverse;
        //public bool twoHooks;
        //public bool threeHooks;
    }

    

    private void UpdateWeight(Crane320? craneData)
    {
        const float distanceBetweenHooks = 2.25f / 2;
        var twoHooks = craneData.Value.Send.craneState.general.Hoist12Mode;
        var threeHooks = craneData.Value.Send.craneState.general.Hoist123Mode;
        var xCorrection = ( twoHooks || threeHooks ) ? 0.71f : 1.27f;
        const float weightZCorrection = 0.1f;
        //const float weightCollidersYCorrection = 0.0f;
        float yCorrection = 0.0f;
        float zCorrection = -weightZCorrection;
        float distanceBetweenTrolleys = (craneData.Value.Send.mainTrolleyPosition - craneData.Value.Send.auxTrolleyPosition) * Globals.scaleZ;
        Vector3 traverseSize = new Vector3();

        if (threeHooks)
            zCorrection = distanceBetweenTrolleys / 2;
        if (traversesCollection.ContainsKey(craneData.Value.Send.mtMh.traverse))
        {
            var traverse = traversesCollection[craneData.Value.Send.mtMh.traverse];
            yCorrection -= traverse.height * Globals.scaleY;
            traverseSize = new Vector3(traverse.length * Globals.scaleX, traverse.height * Globals.scaleY, traverse.width * Globals.scaleZ);
        }
        var overallSize = new Vector3();
        var useOverall = false;
        var overallPosition = new Vector3();
        var showWeight = craneData.Value.Send.mtMh.Size_X > 0 && craneData.Value.Send.mtMh.Size_Y > 0 && craneData.Value.Send.mtMh.Size_Z > 0;
        if (twoHooks || threeHooks)
        {
            overallSize = new Vector3(
                Math.Max(craneData.Value.Send.mtMh.Size_X * Globals.scaleX, Math.Max(traverseSize.x, distanceBetweenHooks)),
                //traverseSize.y > craneData.Value.Send.mtMh.Size_Y * Globals.scaleY ? traverseSize.y : craneData.Value.Send.mtMh.Size_Y * Globals.scaleY,
                Math.Max(Math.Max(craneData.Value.Send.mtMh.Size_Y * Globals.scaleY, traverseSize.y), Math.Abs(distanceBetweenTrolleys) * (threeHooks?1:0)),
                traverseSize.z + craneData.Value.Send.mtMh.Size_Z * Globals.scaleZ + 0.15f
            );
            useOverall = true;
            overallPosition = new Vector3(-5.65f / 2, 0, -zCorrection-weightZCorrection);
        }
        else if (traversesCollection.ContainsKey(craneData.Value.Send.mtMh.traverse))
        {
            overallSize = new Vector3(
                Math.Max(craneData.Value.Send.mtMh.Size_X * Globals.scaleX, traverseSize.x),
                //traverseSize.y > craneData.Value.Send.mtMh.Size_Y * Globals.scaleY ? traverseSize.y : craneData.Value.Send.mtMh.Size_Y * Globals.scaleY,
                //Размеры z и y поменяны местами, т.к. траверса в координатах Юнити, а размеры груза в координатах чертежа (z - высота, y - глубина)
                Math.Max(craneData.Value.Send.mtMh.Size_Y * Globals.scaleY, traverseSize.z),
                traverseSize.y + craneData.Value.Send.mtMh.Size_Z * Globals.scaleZ + 0.15f
            );
            useOverall = true;
        }


        ActivateColliders(mtMhWeight, showWeight);

        CalculateWeightPosition(new WeightDrawerArgs
        {
            constDiff = new Vector3(xCorrection, yCorrection, zCorrection),
            bridgePosition = craneData.Value.Send.bridgePosition,
            trolleyPosition = craneData.Value.Send.mainTrolleyPosition,
            hoistPosition = craneData.Value.Send.mtMh.position,
            size_x = craneData.Value.Send.mtMh.Size_X,
            size_z = craneData.Value.Send.mtMh.Size_Y,
            size_y = craneData.Value.Send.mtMh.Size_Z,
            weightObject = mtMhWeight,
            show = showWeight,
            useOverallSize = useOverall,
            overallSize = overallSize,
            overallPosition = overallPosition,
        });

        useOverall = false;
        showWeight = craneData.Value.Send.mtAh.Size_X > 0 && craneData.Value.Send.mtAh.Size_Y > 0 && craneData.Value.Send.mtAh.Size_Z > 0 && !craneData.Value.Send.craneState.general.Hoist12Mode && !craneData.Value.Send.craneState.general.Hoist123Mode;
        if (traversesCollection.ContainsKey(craneData.Value.Send.mtAh.traverse))
        {
            var traverse = traversesCollection[craneData.Value.Send.mtAh.traverse];
            yCorrection = -traverse.height * Globals.scaleY;
            traverseSize = new Vector3(traverse.length * Globals.scaleX, traverse.height * Globals.scaleY, traverse.width * Globals.scaleZ);
        }
        if (traversesCollection.ContainsKey(craneData.Value.Send.mtAh.traverse) && !twoHooks && !threeHooks)
        {
            overallSize = new Vector3(
                Math.Max(craneData.Value.Send.mtAh.Size_X * Globals.scaleX * (showWeight?1:0), traverseSize.x),
                //traverseSize.y > craneData.Value.Send.mtMh.Size_Y * Globals.scaleY ? traverseSize.y : craneData.Value.Send.mtMh.Size_Y * Globals.scaleY,
                Math.Max(craneData.Value.Send.mtMh.Size_Y * Globals.scaleY * (showWeight ? 1 : 0), traverseSize.y),
                traverseSize.z + craneData.Value.Send.mtMh.Size_Z * Globals.scaleZ * (showWeight ? 1 : 0) + 0.15f
            );
            useOverall = true;
            overallPosition = new Vector3(0, 0, 0);
        }

        ActivateColliders(mtAhWeight, showWeight);

        CalculateWeightPosition(new WeightDrawerArgs
        {
            constDiff = new Vector3(0.15f, 0.0f, -0.1f),
            bridgePosition = craneData.Value.Send.bridgePosition,
            trolleyPosition = craneData.Value.Send.mainTrolleyPosition,
            hoistPosition = craneData.Value.Send.mtAh.position,
            size_x = craneData.Value.Send.mtAh.Size_X,
            size_z = craneData.Value.Send.mtAh.Size_Y,
            size_y = craneData.Value.Send.mtAh.Size_Z,
            weightObject = mtAhWeight,
            show = showWeight,
            useOverallSize = useOverall,
            overallSize = overallSize,
            overallPosition = overallPosition,
        });

        useOverall = false;
        showWeight = craneData.Value.Send.atMh.Size_X > 0 && craneData.Value.Send.atMh.Size_Y > 0 && craneData.Value.Send.atMh.Size_Z > 0 && !craneData.Value.Send.craneState.general.Hoist123Mode;
        if (traversesCollection.ContainsKey(craneData.Value.Send.atMh.traverse))
        {
            var traverse = traversesCollection[craneData.Value.Send.atMh.traverse];
            yCorrection = -traverse.height * Globals.scaleY;
            traverseSize = new Vector3(traverse.length * Globals.scaleX, traverse.height * Globals.scaleY, traverse.width * Globals.scaleZ);
        }
        if (traversesCollection.ContainsKey(craneData.Value.Send.atMh.traverse) && !threeHooks)
        {
            overallSize = new Vector3(
                Math.Max(craneData.Value.Send.atMh.Size_X * Globals.scaleX * (showWeight ? 1 : 0), traverseSize.x),
                //traverseSize.y > craneData.Value.Send.mtMh.Size_Y * Globals.scaleY ? traverseSize.y : craneData.Value.Send.mtMh.Size_Y * Globals.scaleY,
                Math.Max(craneData.Value.Send.atMh.Size_Y * Globals.scaleY * (showWeight ? 1 : 0), traverseSize.y),
                traverseSize.z + craneData.Value.Send.atMh.Size_Z * Globals.scaleZ * (showWeight ? 1 : 0) + 0.15f
            );
            useOverall = true;
            overallPosition = new Vector3(0, 0, 0);
        }


        ActivateColliders(atMhWeight, showWeight);

        CalculateWeightPosition(new WeightDrawerArgs
        {
            constDiff = new Vector3(0.72f, yCorrection, /*Globals.scaleZ*craneData.Value.Send.atMh.Size_Z/2 +*/ 0.11f),
            bridgePosition = craneData.Value.Send.bridgePosition,
            trolleyPosition = craneData.Value.Send.auxTrolleyPosition,
            hoistPosition = craneData.Value.Send.atMh.position,
            size_x = craneData.Value.Send.atMh.Size_X,
            size_z = craneData.Value.Send.atMh.Size_Y,
            size_y = craneData.Value.Send.atMh.Size_Z,
            weightObject = atMhWeight,
            show = showWeight,
            useOverallSize = useOverall,
            overallSize = overallSize,
            overallPosition = overallPosition
        });
    }

    private void UpdateWeight(Crane25Read? craneData)
    {
        CalculateWeightPosition(new WeightDrawerArgs
        {
            constDiff = new Vector3(0.44f, /*craneData.Value.mtMhloadDimensionY * Globals.scaleY/2*/-0.1f, (craneData.Value.mtMhloadDimensionY * Globals.scaleZ / 2)-0.164f),
            bridgePosition = craneData.Value.bridgePosition,
            trolleyPosition = craneData.Value.mainTrolleyPosition,
            hoistPosition = craneData.Value.mainTrolleyMainHoistPosition,
            size_x = craneData.Value.mtMhLoadDimensionX,
            size_z = craneData.Value.mtMhloadDimensionY,
            size_y = craneData.Value.mtMhloadDimensionZ,
            weightObject = mtMhWeight,
            show = craneData.Value.mtMhLoadWeight > minWeight,
        });

        CalculateWeightPosition(new WeightDrawerArgs
        {
            constDiff = new Vector3(0.44f, /*craneData.Value.mtAhloadDimensionY * Globals.scaleY / 2*/-0.1f, (craneData.Value.mtAhloadDimensionY * Globals.scaleZ / 2) + 0.05f),
            bridgePosition = craneData.Value.bridgePosition,
            trolleyPosition = craneData.Value.mainTrolleyPosition,
            hoistPosition = craneData.Value.mainTrolleyAuxHoistPosition,
            size_x = craneData.Value.mtAhLoadDimensionX,
            size_z = craneData.Value.mtAhloadDimensionY,
            size_y = craneData.Value.mtAhloadDimensionZ,
            weightObject = mtAhWeight,
            show = craneData.Value.mtAhLoadWeight > minWeight,
        });
    }

    private void CalculateWeightPosition(WeightDrawerArgs wda)
    {
        //bool mtMhWeight;
        if (wda.weightObject != null)
        {
            wda.weightObject.Visible = wda.show;
            wda.weightObject.useOverall = wda.useOverallSize;
            wda.weightObject.overallSize = wda.overallSize;
            wda.weightObject.overallPosition = wda.overallPosition;
            wda.weightObject.position = (new Vector3(wda.bridgePosition, wda.hoistPosition, Globals.zZeroFix - wda.trolleyPosition)) * Globals.scaleX;
            if (wda.size_x > 0 && wda.size_y > 0 && wda.size_z > 0)
            {
                wda.weightObject.scale = new Vector3(wda.size_x * Globals.scaleX, wda.size_y * Globals.scaleY, wda.size_z * Globals.scaleZ);
            }
            else
                wda.weightObject.scale = new Vector3(Globals.defaultWeightLength * Globals.scaleX, Globals.defaultWeightHeight * Globals.scaleY, Globals.defaultWeightWidth * Globals.scaleZ);
            if (!wda.show)
            {
                wda.weightObject.scale = new Vector3();
            }

            wda.weightObject.position += new Vector3(/*wda.weightObject.scale.x /2 */0, -wda.weightObject.scale.y / 2, /*-wda.weightObject.scale.z / 2*/0) + wda.constDiff;
        }
        else
        {
            if (wda.weightObject != null)
                wda.weightObject.Visible = wda.show;
        }
    }
}
