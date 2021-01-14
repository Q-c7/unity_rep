using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

public struct CollisionStatuses
{
    public bool bridgeLeftWarning;
    public bool bridgeLeftAlarm;
    public bool bridgeRightWarning;
    public bool bridgeRightAlarm;
    public bool mainTrolleyForwardWarning;
    public bool mainTrolleyForwardAlarm;
    public bool mainTrolleyBackwardWarning;
    public bool mainTrolleyBackwardAlarm;
    public bool auxTrolleyForwardWarning;
    public bool auxTrolleyForwardAlarm;
    public bool auxTrolleyBackwardWarning;
    public bool auxTrolleyBackwardAlarm;

    public bool mtMhDownWarning;
    public bool mtMhDownAlarm;
    public bool mtMhUpWarning;
    public bool mtMhUpAlarm;
    public bool mtAhDownWarning;
    public bool mtAhDownAlarm;
    public bool mtAhUpWarning;
    public bool mtAhUpAlarm;

    public bool atMhDownWarning;
    public bool atMhDownAlarm;
    public bool atMhUpWarning;
    public bool atMhUpAlarm;
    public bool atAhDownWarning;
    public bool atAhDownAlarm;
    public bool atAhUpWarning;
    public bool atAhUpAlarm;
}

public class CraneCollisions : MonoBehaviour
{
    public Alarms alarmsLogger;
    // По коллайдеру на каждый подъем двух тележек, предупреждения
    public List<CollisionDetector> BridgeRightWarningColliders = new List<CollisionDetector>();
    public List<CollisionDetector> BridgeLeftWarningColliders = new List<CollisionDetector>();
    public List<CollisionDetector> BridgeRightAlarmColliders = new List<CollisionDetector>();
    public List<CollisionDetector> BridgeLeftAlarmColliders = new List<CollisionDetector>();

    // Главная тележка от двух крюков
    public List<CollisionDetector> MtBackwardWarningColliders = new List<CollisionDetector>();
    public List<CollisionDetector> MtForwardWarningColliders = new List<CollisionDetector>();
    public List<CollisionDetector> MtBackwardAlarmColliders = new List<CollisionDetector>();
    public List<CollisionDetector> MtForwardAlarmColliders = new List<CollisionDetector>();

    public CollisionDetector MtMhDownWarningCollider;
    public CollisionDetector MtMhDownAlarmCollider;
    public CollisionDetector MtMhUpWarningCollider;
    public CollisionDetector MtMhUpAlarmCollider;

    public CollisionDetector MtAhDownWarningCollider;
    public CollisionDetector MtAhUpWarningCollider;
    public CollisionDetector MtAhDownAlarmCollider;
    public CollisionDetector MtAhUpAlarmCollider;

    // Вспом. тележка от двух крюков
    public List<CollisionDetector> AtBackwardWarningColliders = new List<CollisionDetector>();
    public List<CollisionDetector> AtForwardWarningColliders = new List<CollisionDetector>();
    public List<CollisionDetector> AtBackwardAlarmColliders = new List<CollisionDetector>();
    public List<CollisionDetector> AtForwardAlarmColliders = new List<CollisionDetector>();

    public CollisionDetector AtMhDownWarningCollider;
    public CollisionDetector AtMhDownAlarmCollider;
    public CollisionDetector AtMhUpWarningCollider;
    public CollisionDetector AtMhUpAlarmCollider;

    public CollisionDetector AtAhDownWarningCollider;
    public CollisionDetector AtAhDownAlarmCollider;
    public CollisionDetector AtAhUpWarningCollider;
    public CollisionDetector AtAhUpAlarmCollider;

    public ImageIndicator bridgeLeft;
    public ImageIndicator bridgeRight;
    public ImageIndicator MainTrolleyForward;
    public ImageIndicator MainTrolleyBackward;
    public ImageIndicator MtMhUp;
    public ImageIndicator MtMhDown;
    public ImageIndicator MtAhUp;
    public ImageIndicator MtAhDown;
    public ImageIndicator AuxTrolleyForward;
    public ImageIndicator AuxTrolleyBackward;
    public ImageIndicator AtMhUp;
    public ImageIndicator AtMhDown;
    public ImageIndicator AtAhUp;
    public ImageIndicator AtAhDown;

    public MaterialSwitch bridge;
    public MaterialSwitch mainTrolley;
    public MaterialSwitch mtMh;
    public MaterialSwitch mtAh;
    public MaterialSwitch auxTrolley;
    public MaterialSwitch atMh;
    public MaterialSwitch atAh;

    //public string message;
    public GameObject alarmOverlay;
    public Text alarmText;
    //public int alarmCount;
    private bool anyAlarm = false;
    public CollisionStatuses CollisionStatuses;
    public List<CollisionDetector> allColliders = new List<CollisionDetector>();

    void Start()
    {
        var colliders = GetComponentsInChildren<CollidersSizeCalc>(false);
        foreach (var collider in colliders)
        {
            var collisionDetector = collider.gameObject.GetComponent<CollisionDetector>();
            switch (collider.type)
            {
                // Мост
                case ColliderType.Left:
                    if (collider.isWarning)
                    {
                        BridgeLeftWarningColliders.Add(collisionDetector);
                    }
                    else
                    {
                        BridgeLeftAlarmColliders.Add(collisionDetector);
                    }
                    break;
                case ColliderType.Right:
                    if (collider.isWarning)
                    {
                        BridgeRightWarningColliders.Add(collisionDetector);
                    }
                    else
                    {
                        BridgeRightAlarmColliders.Add(collisionDetector);
                    }
                    break;
                // Тележки
                case ColliderType.Backward:
                    switch (collider.target)
                    {
                        case ColliderTarget.MainTrolley:
                            if (collider.isWarning)
                            {
                                MtBackwardWarningColliders.Add(collisionDetector);
                            }
                            else
                            {
                                MtBackwardAlarmColliders.Add(collisionDetector);
                            }
                            break;
                        case ColliderTarget.AuxTrolley:
                            if (collider.isWarning)
                            {
                                AtBackwardWarningColliders.Add(collisionDetector);
                            }
                            else
                            {
                                AtBackwardAlarmColliders.Add(collisionDetector);
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                case ColliderType.Forward:
                    switch (collider.target)
                    {
                        case ColliderTarget.MainTrolley:
                            if (collider.isWarning)
                            {
                                MtForwardWarningColliders.Add(collisionDetector);
                            }
                            else
                            {
                                MtForwardAlarmColliders.Add(collisionDetector);
                            }
                            break;
                        case ColliderTarget.AuxTrolley:
                            if (collider.isWarning)
                            {
                                AtForwardWarningColliders.Add(collisionDetector);
                            }
                            else
                            {
                                AtForwardAlarmColliders.Add(collisionDetector);
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                case ColliderType.Up:
                case ColliderType.Down:
                    switch (collider.target)
                    {
                        case ColliderTarget.MtMh:
                            if (collider.isWarning)
                            {
                                MtMhDownWarningCollider = collisionDetector;
                            }
                            else
                            {
                                MtMhDownAlarmCollider = collisionDetector;
                            }
                            break;
                        case ColliderTarget.MtAh:
                            if (collider.isWarning)
                            {
                                MtAhDownWarningCollider = collisionDetector;
                            }
                            else
                            {
                                MtAhDownAlarmCollider = collisionDetector;
                            }
                            break;
                        case ColliderTarget.AtMh:
                            if (collider.isWarning)
                            {
                                AtMhDownWarningCollider = collisionDetector;
                            }
                            else
                            {
                                AtMhDownAlarmCollider = collisionDetector;
                            }
                            break;
                        case ColliderTarget.AtAh:
                            if (collider.isWarning)
                            {
                                AtAhDownWarningCollider = collisionDetector;
                            }
                            else
                            {
                                AtAhDownAlarmCollider = collisionDetector;
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
        }
        if (bridgeRight != null)
            SetListeners(bridgeRight, BridgeRightWarningColliders, BridgeRightAlarmColliders, bridge);
        if (bridgeLeft != null)
            SetListeners(bridgeLeft, BridgeLeftWarningColliders, BridgeLeftAlarmColliders, bridge);
        if (MainTrolleyForward != null)
            SetListeners(MainTrolleyForward, MtForwardWarningColliders, MtForwardAlarmColliders, mainTrolley);
        if (MainTrolleyBackward != null)
            SetListeners(MainTrolleyBackward, MtBackwardWarningColliders, MtBackwardAlarmColliders, mainTrolley);
        if (MainTrolleyBackward != null)
            SetListeners(MainTrolleyBackward, MtBackwardWarningColliders, MtBackwardAlarmColliders, mainTrolley);
        if (MtMhUp != null)
            SetListeners(MtMhUp, MtMhUpWarningCollider, MtMhUpAlarmCollider, mtMh);
        if (MtMhDown != null)
            SetListeners(MtMhDown, MtMhDownWarningCollider, MtMhDownAlarmCollider, mtMh);
        if (MtAhUp != null)
            SetListeners(MtAhUp, MtAhUpWarningCollider, MtAhUpAlarmCollider, mtAh);
        if (MtAhDown != null)
            SetListeners(MtAhDown, MtAhDownWarningCollider, MtAhDownAlarmCollider, mtAh);
        if (AuxTrolleyForward != null)
            SetListeners(AuxTrolleyForward, AtForwardWarningColliders, AtForwardAlarmColliders, auxTrolley);
        if (AuxTrolleyBackward != null)
            SetListeners(AuxTrolleyBackward, AtBackwardWarningColliders, AtBackwardAlarmColliders, auxTrolley);
        if (AtMhUp != null)
            SetListeners(AtMhUp, AtMhUpWarningCollider, AtMhUpAlarmCollider, atMh);
        if (AtMhDown != null)
            SetListeners(AtMhDown, AtMhDownWarningCollider, AtMhDownAlarmCollider, atMh);
        if (AtAhUp != null)
            SetListeners(AtAhUp, AtAhUpWarningCollider, AtAhUpAlarmCollider, atAh);
        if (AtAhDown != null)
            SetListeners(AtAhDown, AtAhDownWarningCollider, AtAhDownAlarmCollider, atAh);
    }

    private void SetListeners(ImageIndicator image, CollisionDetector warning, CollisionDetector alarm, MaterialSwitch source)
    {
        allColliders.Add(alarm);
        if(image != null)
        {
            image.good = true;
            if (alarm != null)
            {
                alarm.message = image.objectName;
                alarm.OnEnter += (target) =>
                {
                    image.alarm = true;
                    /*var ms = target.GetComponent<MaterialSwitch>();
                    if (ms != null)
                    {
                        ms.alarm = true;
                    }*/
                    if (source != null) source.alarm = true;
                    if(alarmsLogger != null)
                    {
                        alarmsLogger.Alarm("Опасность столкновения: " + image.objectName);
                    }
                };
                alarm.OnExit += (target) =>
                {
                    image.alarm = false;
                    //message = "";
                    /*var ms = target.GetComponent<MaterialSwitch>();
                    if (ms != null)
                    {
                        ms.alarm = false;
                    }*/
                    if (source != null) source.alarm = false;
                };
            }
            if(warning!= null)
            {
                warning.OnEnter += (target) =>
                {
                    image.warning = true;
                    /*var ms = target.GetComponent<MaterialSwitch>();
                    if (ms != null)
                    {
                        ms.warning = true;
                    }*/
                    if (source != null) source.warning = true;
                    if (alarmsLogger != null)
                    {
                        alarmsLogger.Alarm("Предупреждение о приближении: " + image.objectName, 1);
                    }
                };
                warning.OnExit += (target) =>
                {
                    image.warning = false;
                    /*var ms = target.GetComponent<MaterialSwitch>();
                    if (ms != null)
                    {
                        ms.warning = false;
                    }*/
                    if (source != null) source.warning = false;
                };
            }   
            
        }
    }

    private void SetListeners(ImageIndicator image, List<CollisionDetector> warnings, List<CollisionDetector> alarms, MaterialSwitch source)
    {
        allColliders.AddRange(alarms);
        foreach (var item in alarms)
        {
            
            if (image != null)
            {
                item.message = image.objectName;
                image.good = true;
                item.OnEnter += (target) =>
                {
                    image.enters++;
                    image.alarm = true;
                    var ms = target.GetComponent<MaterialSwitch>();
                    if(ms != null)
                    {
                        ms.alarm = true;
                    }
                    if (source != null) source.alarm = true;
                    if (alarmsLogger != null)
                    {
                        alarmsLogger.Alarm("Опасность столкновения: " + image.objectName);
                    }
                };
                item.OnExit += (target) =>
                {
                    image.exits++;
                    var alarmExists = (from alarm in alarms
                                       where alarm.triggered
                                       select alarm).Count() > 0;
                    image.alarm = alarmExists;
                    //message = "";
                    var ms = target.GetComponent<MaterialSwitch>();
                    if (ms != null)
                    {
                        ms.alarm = false;
                    }
                    if (source != null) source.alarm = alarmExists;

                    Debug.Log("Alarm exists: " + image.objectName + " " + alarmExists);
                };
            }
        }
        foreach (var item in warnings)
        {
            if (image != null)
            {
                image.good = true;
                item.OnEnter += (target) =>
                {
                    image.enters++;
                    image.warning = true;
                    var message = image.objectName;
                    var ms = target.GetComponent<MaterialSwitch>();
                    if (ms != null)
                    {
                        ms.warning = true;
                    }
                    if (source != null) source.warning = true;
                    if (alarmsLogger != null)
                    {
                        alarmsLogger.Alarm("Предупреждение о приближении: " + message);
                    }
                };
                item.OnExit += (target) =>
                {
                    image.exits++;
                    var warningExists = (from warning in warnings
                                         where warning.triggered
                                         select warning).Count() > 0;
                    image.warning = warningExists;
                    var ms = target.GetComponent<MaterialSwitch>();
                    if (ms != null)
                    {
                        ms.warning = false;
                    }
                    if (source != null) source.warning = warningExists;
                };
            }
        }
    }

    void UpdateStatuses()
    {
        CollisionStatuses.bridgeLeftAlarm = StatusCalc(BridgeLeftAlarmColliders);
        CollisionStatuses.bridgeLeftWarning = StatusCalc(BridgeLeftWarningColliders);
        CollisionStatuses.bridgeRightAlarm = StatusCalc(BridgeRightAlarmColliders);
        CollisionStatuses.bridgeRightWarning = StatusCalc(BridgeRightWarningColliders);

        CollisionStatuses.mainTrolleyBackwardAlarm = StatusCalc(MtBackwardAlarmColliders);
        CollisionStatuses.mainTrolleyBackwardWarning = StatusCalc(MtBackwardWarningColliders);
        CollisionStatuses.mainTrolleyForwardAlarm = StatusCalc(MtForwardAlarmColliders);
        CollisionStatuses.mainTrolleyForwardWarning = StatusCalc(MtForwardWarningColliders);

        CollisionStatuses.auxTrolleyBackwardAlarm = StatusCalc(AtBackwardAlarmColliders);
        CollisionStatuses.auxTrolleyBackwardWarning = StatusCalc(AtBackwardWarningColliders);
        CollisionStatuses.auxTrolleyForwardAlarm = StatusCalc(AtForwardAlarmColliders);
        CollisionStatuses.auxTrolleyForwardWarning = StatusCalc(AtForwardWarningColliders);

        CollisionStatuses.mtMhDownAlarm =   MtMhDownAlarmCollider != null ? MtMhDownAlarmCollider.triggered ? true : false : false;
        CollisionStatuses.mtMhDownWarning = MtMhDownWarningCollider != null ? MtMhDownWarningCollider.triggered ? true : false : false;
        CollisionStatuses.mtMhUpAlarm =     MtMhUpAlarmCollider != null ? MtMhUpAlarmCollider.triggered ? true : false : false;
        CollisionStatuses.mtMhUpWarning =   MtMhUpWarningCollider != null ? MtMhUpWarningCollider.triggered ? true : false : false;

        CollisionStatuses.mtAhDownAlarm =   MtAhDownAlarmCollider != null ? MtAhDownAlarmCollider.triggered ? true : false : false;
        CollisionStatuses.mtAhDownWarning = MtAhDownWarningCollider != null ? MtAhDownWarningCollider.triggered ? true : false : false;
        CollisionStatuses.mtAhUpAlarm =     MtAhUpAlarmCollider != null ? MtAhUpAlarmCollider.triggered ? true : false : false;
        CollisionStatuses.mtAhUpWarning =   MtAhUpWarningCollider != null ? MtAhUpWarningCollider.triggered ? true : false : false;

        CollisionStatuses.atMhDownAlarm =   AtMhDownAlarmCollider != null ? AtMhDownAlarmCollider.triggered ? true : false : false;
        CollisionStatuses.atMhDownWarning = AtMhDownWarningCollider != null ? AtMhDownWarningCollider.triggered ? true : false : false;
        CollisionStatuses.atMhUpAlarm =     AtMhUpAlarmCollider != null ? AtMhUpAlarmCollider.triggered ? true : false : false;
        CollisionStatuses.atMhUpWarning =   AtMhUpWarningCollider != null ? AtMhUpWarningCollider.triggered ? true : false : false;

        CollisionStatuses.atAhDownAlarm =   AtAhDownAlarmCollider != null ? AtAhDownAlarmCollider.triggered ? true : false : false;
        CollisionStatuses.atAhDownWarning = AtAhDownWarningCollider != null ? AtAhDownWarningCollider.triggered ? true : false : false;
        CollisionStatuses.atAhUpAlarm =     AtAhUpAlarmCollider != null ? AtAhUpAlarmCollider.triggered ? true : false : false;
        CollisionStatuses.atAhUpWarning =   AtAhUpWarningCollider != null ? AtAhUpWarningCollider.triggered ? true : false : false;
    }

    private bool StatusCalc(List<CollisionDetector> list)
    {
        if (list != null)
        return (from collider in list
                where collider != null ? collider.triggered : false
                select collider).Count() > 0;
        return false;
    }

    // Update is called once per frame
    void Update()
    {
        anyAlarm = StatusCalc(allColliders);
        var message = (from collider in allColliders
                       where collider != null ? collider.triggered : false
                       select collider.message).FirstOrDefault();
        if (alarmOverlay != null)
        {
            alarmOverlay.SetActive(anyAlarm);
            alarmText.text = "Опасность столкновения: " + message;
        }
        UpdateStatuses();
        /*UpdateMaterial(BridgeLeftAlarmColliders);
        UpdateMaterial(BridgeLeftWarningColliders);
        UpdateMaterial(BridgeRightWarningColliders);
        UpdateMaterial(BridgeRightAlarmColliders);*/
        UpdateMaterial(MtBackwardAlarmColliders.Concat(MtBackwardWarningColliders).Concat(MtForwardAlarmColliders).Concat(MtForwardWarningColliders).ToList(), mainTrolley);
        //UpdateMaterial(MtBackwardWarningColliders, mainTrolley);
        //UpdateMaterial(MtForwardAlarmColliders, mainTrolley);
        //UpdateMaterial(MtForwardWarningColliders, mainTrolley);
    }

    private void UpdateMaterial(List<CollisionDetector> colliders, MaterialSwitch source)
    {
        var filtered = (from collider in colliders
                         where collider != null
                         select collider);
        var warnings = filtered.Where(cd => (bool)cd.GetComponent<CollidersSizeCalc>()?.isWarning);
        var alarms = filtered.Where(cd => !(bool)cd.GetComponent<CollidersSizeCalc>()?.isWarning);
        source.warning = warnings.Count(cd => cd.triggered) > 0;
        source.alarm = alarms.Count(cd => cd.triggered) > 0;
        foreach (var collider in filtered)
        {
            var csc = collider.gameObject.GetComponent<CollidersSizeCalc>();
            MaterialSwitch twms = null;
            if(collider.triggeredWith != null)
            {
                twms = collider.triggeredWith?.GetComponent<MaterialSwitch>();
            }
            if (csc.isWarning)
            {
                if(twms != null)
                    twms.warning = collider.triggered;
            }
            else
            {
                if (twms != null)
                    twms.alarm = collider.triggered;
            }
        }
    }

    private void UpdateMaterial(MaterialSwitch material, List<CollisionDetector> warnings, List<CollisionDetector> alarms)
    {
        if(material != null)
        {
            if(warnings != null)
            {
                material.warning = (from collider in warnings
                                    where collider.triggered
                                    select collider).Count() > 0;
                if (material.warning)
                {
                    material.targetObject = (from collider in warnings
                                              where collider.triggered
                                              select collider.triggeredWith).First();
                }
                
            }
            if(alarms != null)
            {
                material.alarm = (from collider in alarms
                                  where collider.triggered
                                  select collider).Count() > 0;
                if (material.alarm)
                {
                    material.targetObject = (from collider in alarms
                                              where collider.triggered
                                              select collider.triggeredWith).First();
                }
                anyAlarm |= material.alarm;
            }
            
        }
        
    }

    private void UpdateMaterial(MaterialSwitch material, CollisionDetector warning, CollisionDetector alarm)
    {
        if(material != null)
        {
            if (warning != null)
            {
                material.warning = warning.triggered;
                material.targetObject = warning.triggeredWith;
            }
            if (alarm != null)
            {
                material.alarm = alarm.triggered;
                material.targetObject = warning.triggeredWith;
            }
            anyAlarm |= alarm.triggered;
        }               
    }

    /*private void UpdateImage(ImageIndicator indicator, List<CollisionDetector> warnings, List<CollisionDetector> alarms)
    {
        indicator.good = warnings.Count > 0;
        indicator.warning = (from collider in warnings
                               where collider.triggered
                               select collider).Count() > 0;
        
        indicator.alarm = (from collider in alarms
                             where collider.triggered
                             select collider).Count() > 0;

        if (indicator.alarm)
        {
            message = indicator.objectName;
        }
    }

    private void UpdateImage(ImageIndicator indicator, CollisionDetector warning, CollisionDetector alarm)
    {
        if(indicator != null)
        {
            indicator.good = warning != null;
            if(warning != null)
            {
                indicator.warning = warning.triggered;
            }
            if(alarm != null)
            {
                indicator.alarm = alarm.triggered;
            }
            if (indicator.alarm)
            {
                message = indicator.objectName;
            }
        }        
    }*/
}
