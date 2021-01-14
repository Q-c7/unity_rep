using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraneCollisions2 : MonoBehaviour
{
    public Alarms alarmsLogger;
    public GameObject alarmOverlay;
    public Info imagesContainer;
    public MaterialSwitch bridge;
    public MaterialSwitch mainTrolley;
    public MaterialSwitch auxTrolley;
    public MaterialSwitch mainTrolleyMainHoist;
    public MaterialSwitch mainTrolleyAuxHoist;
    public MaterialSwitch auxTrolleyMainHoist;
    public MaterialSwitch auxTrolleyAuxHoist;
    public CollisionStatuses collisions;
    public bool[] collidersValues = new bool[(int)ColliderTargetAndType.count];
    private Dictionary<ColliderTargetAndType, List<CollisionDetector2>> collidersDict = new Dictionary<ColliderTargetAndType, List<CollisionDetector2>>();
    private readonly Dictionary<ColliderTargetAndType, ImageIndicator> images = new Dictionary<ColliderTargetAndType, ImageIndicator>();
    public static List<ColliderTargetAndType> warnings = new List<ColliderTargetAndType>() {
        ColliderTargetAndType.bridgeLeftWarning,
        ColliderTargetAndType.bridgeRightWarning,
        ColliderTargetAndType.mainTrolleyForwardWarning,
        ColliderTargetAndType.mainTrolleyBackwardWarning,
        ColliderTargetAndType.auxTrolleyForwardWarning,
        ColliderTargetAndType.auxTrolleyForwardWarning,
        ColliderTargetAndType.auxTrolleyBackwardWarning,
        ColliderTargetAndType.mtMhDownWarning,
        ColliderTargetAndType.mtMhDownWarning,
        ColliderTargetAndType.mtMhDownWarning,
        ColliderTargetAndType.mtMhUpWarning,
        ColliderTargetAndType.mtAhDownWarning,
        ColliderTargetAndType.mtAhUpWarning,
        ColliderTargetAndType.atMhDownWarning,
        ColliderTargetAndType.atMhUpWarning,
        ColliderTargetAndType.atAhDownWarning,
        ColliderTargetAndType.atAhUpWarning,
    };
    public static List<ColliderTargetAndType> alarms = new List<ColliderTargetAndType>()
    {
        ColliderTargetAndType.bridgeLeftAlarm,
        ColliderTargetAndType.bridgeRightAlarm,
        ColliderTargetAndType.mainTrolleyForwardAlarm,
        ColliderTargetAndType.mainTrolleyBackwardAlarm,
        ColliderTargetAndType.auxTrolleyForwardAlarm,
        ColliderTargetAndType.auxTrolleyBackwardAlarm,
        ColliderTargetAndType.mtMhDownAlarm,
        ColliderTargetAndType.mtMhUpAlarm,
        ColliderTargetAndType.mtAhDownAlarm,
        ColliderTargetAndType.mtAhUpAlarm,
        ColliderTargetAndType.atMhDownAlarm,
        ColliderTargetAndType.atMhUpAlarm,
        ColliderTargetAndType.atAhDownAlarm,
        ColliderTargetAndType.atAhUpAlarm,
    };

    void Start()
    {
        InitDicts();
        Debug.Log("Initializing colliders: " + gameObject.name);
        var boundColliders = gameObject.GetComponentsInChildren<CollisionDetector2>();
        foreach (var collider in boundColliders)
        {
            collidersDict[collider.type]?.Add(collider);
        }
        InitImageIndicators();
        Debug.Log("Initializing colliders done: " + gameObject.name);
    }

    private void InitImageIndicators()
    {
        if (imagesContainer != null)
        {
            var boundImages = imagesContainer.gameObject.GetComponentsInChildren<ImageIndicator>(false);
            foreach (var image in boundImages)
            {
                switch (image.type)
                {
                    case ColliderTargetAndType.none:
                    case ColliderTargetAndType.count:
                    default:
                        break;
                    case ColliderTargetAndType.bridgeLeftWarning:
                    case ColliderTargetAndType.bridgeLeftAlarm:
                        images.Add(ColliderTargetAndType.bridgeLeftWarning, image);
                        images.Add(ColliderTargetAndType.bridgeLeftAlarm, image);
                        break;
                    case ColliderTargetAndType.bridgeRightAlarm:
                    case ColliderTargetAndType.bridgeRightWarning:
                        images.Add(ColliderTargetAndType.bridgeRightAlarm, image);
                        images.Add(ColliderTargetAndType.bridgeRightWarning, image);
                        break;
                    case ColliderTargetAndType.mainTrolleyForwardWarning:
                    case ColliderTargetAndType.mainTrolleyForwardAlarm:
                        images.Add(ColliderTargetAndType.mainTrolleyForwardWarning, image);
                        images.Add(ColliderTargetAndType.mainTrolleyForwardAlarm, image);
                        break;
                    case ColliderTargetAndType.mainTrolleyBackwardAlarm:
                    case ColliderTargetAndType.mainTrolleyBackwardWarning:
                        images.Add(ColliderTargetAndType.mainTrolleyBackwardAlarm, image);
                        images.Add(ColliderTargetAndType.mainTrolleyBackwardWarning, image);
                        break;
                    case ColliderTargetAndType.auxTrolleyForwardWarning:
                    case ColliderTargetAndType.auxTrolleyForwardAlarm:
                        images.Add(ColliderTargetAndType.auxTrolleyForwardWarning, image);
                        images.Add(ColliderTargetAndType.auxTrolleyForwardAlarm, image);
                        break;
                    case ColliderTargetAndType.auxTrolleyBackwardAlarm:
                    case ColliderTargetAndType.auxTrolleyBackwardWarning:
                        images.Add(ColliderTargetAndType.auxTrolleyBackwardAlarm, image);
                        images.Add(ColliderTargetAndType.auxTrolleyBackwardWarning, image);
                        break;
                    case ColliderTargetAndType.mtMhDownWarning:
                    case ColliderTargetAndType.mtMhDownAlarm:
                        images.Add(ColliderTargetAndType.mtMhDownWarning, image);
                        images.Add(ColliderTargetAndType.mtMhDownAlarm, image);
                        break;
                    case ColliderTargetAndType.mtMhUpWarning:
                    case ColliderTargetAndType.mtMhUpAlarm:
                        images.Add(ColliderTargetAndType.mtMhUpWarning, image);
                        images.Add(ColliderTargetAndType.mtMhUpAlarm, image);
                        break;
                    case ColliderTargetAndType.mtAhDownWarning:
                    case ColliderTargetAndType.mtAhDownAlarm:
                        images.Add(ColliderTargetAndType.mtAhDownWarning, image);
                        images.Add(ColliderTargetAndType.mtAhDownAlarm, image);
                        break;
                    case ColliderTargetAndType.mtAhUpWarning:
                    case ColliderTargetAndType.mtAhUpAlarm:
                        images.Add(ColliderTargetAndType.mtAhUpWarning, image);
                        images.Add(ColliderTargetAndType.mtAhUpAlarm, image);
                        break;
                    case ColliderTargetAndType.atMhDownWarning:
                    case ColliderTargetAndType.atMhDownAlarm:
                        images.Add(ColliderTargetAndType.atMhDownWarning, image);
                        images.Add(ColliderTargetAndType.atMhDownAlarm, image);
                        break;
                    case ColliderTargetAndType.atMhUpWarning:
                    case ColliderTargetAndType.atMhUpAlarm:
                        images.Add(ColliderTargetAndType.atMhUpWarning, image);
                        images.Add(ColliderTargetAndType.atMhUpAlarm, image);
                        break;
                    case ColliderTargetAndType.atAhDownWarning:
                    case ColliderTargetAndType.atAhDownAlarm:
                        images.Add(ColliderTargetAndType.atAhDownWarning, image);
                        images.Add(ColliderTargetAndType.atAhDownAlarm, image);
                        break;
                    case ColliderTargetAndType.atAhUpWarning:
                    case ColliderTargetAndType.atAhUpAlarm:
                        images.Add(ColliderTargetAndType.atAhUpWarning, image);
                        images.Add(ColliderTargetAndType.atAhUpAlarm, image);
                        break;

                }
            }
        }
    }

    private void UpdateImages()
    {
        for (int i = 0; i < collidersValues.Length; i++)
        {
            var craneConnected = (bool)gameObject.GetComponent<CraneConnection>()?.connected;
            var cvt = (ColliderTargetAndType)i;
            if (!images.ContainsKey(cvt)) continue;
            images[cvt].good = craneConnected;
            switch (cvt)
            {

                case ColliderTargetAndType.bridgeLeftWarning:
                case ColliderTargetAndType.bridgeRightWarning:
                case ColliderTargetAndType.mainTrolleyForwardWarning:
                case ColliderTargetAndType.mainTrolleyBackwardWarning:
                case ColliderTargetAndType.auxTrolleyForwardWarning:
                case ColliderTargetAndType.auxTrolleyBackwardWarning:
                case ColliderTargetAndType.mtMhDownWarning:
                case ColliderTargetAndType.mtMhUpWarning:
                case ColliderTargetAndType.mtAhDownWarning:
                case ColliderTargetAndType.mtAhUpWarning:
                case ColliderTargetAndType.atMhDownWarning:
                case ColliderTargetAndType.atMhUpWarning:
                case ColliderTargetAndType.atAhDownWarning:
                case ColliderTargetAndType.atAhUpWarning:
                    images[cvt].warning = collidersValues[(int)cvt];
                    break;
                case ColliderTargetAndType.bridgeLeftAlarm:
                case ColliderTargetAndType.bridgeRightAlarm:
                case ColliderTargetAndType.mainTrolleyForwardAlarm:
                case ColliderTargetAndType.mainTrolleyBackwardAlarm:
                case ColliderTargetAndType.auxTrolleyForwardAlarm:
                case ColliderTargetAndType.auxTrolleyBackwardAlarm:
                case ColliderTargetAndType.mtMhDownAlarm:
                case ColliderTargetAndType.mtMhUpAlarm:
                case ColliderTargetAndType.mtAhDownAlarm:
                case ColliderTargetAndType.mtAhUpAlarm:
                case ColliderTargetAndType.atMhDownAlarm:
                case ColliderTargetAndType.atMhUpAlarm:
                case ColliderTargetAndType.atAhDownAlarm:
                case ColliderTargetAndType.atAhUpAlarm:
                    images[cvt].alarm = collidersValues[(int)cvt];
                    break;
                case ColliderTargetAndType.count:
                case ColliderTargetAndType.none:
                default:
                    break;
            }
        }
        foreach (var image in images)
        {
            
        }
    }

    private void UpdateOverlay()
    {
        if (alarmOverlay == null) return;
        bool anyAlarm = false;
        string message = "";
        for (int i = 0; i < collidersValues.Length; i++)
        {
            var cvt = (ColliderTargetAndType)i;
            if (!images.ContainsKey(cvt)) continue;
            switch (cvt)
            {
                case ColliderTargetAndType.none:
                case ColliderTargetAndType.bridgeLeftWarning:
                case ColliderTargetAndType.bridgeRightWarning:
                case ColliderTargetAndType.mainTrolleyForwardWarning:
                case ColliderTargetAndType.mainTrolleyBackwardWarning:
                case ColliderTargetAndType.auxTrolleyForwardWarning:
                case ColliderTargetAndType.auxTrolleyBackwardWarning:
                case ColliderTargetAndType.mtMhDownWarning:
                case ColliderTargetAndType.mtMhUpWarning:
                case ColliderTargetAndType.mtAhDownWarning:
                case ColliderTargetAndType.mtAhUpWarning:
                case ColliderTargetAndType.atMhDownWarning:
                case ColliderTargetAndType.atMhUpWarning:
                case ColliderTargetAndType.atAhDownWarning:
                case ColliderTargetAndType.atAhUpWarning:
                case ColliderTargetAndType.count:
                default:
                    break;
                case ColliderTargetAndType.bridgeLeftAlarm:
                case ColliderTargetAndType.bridgeRightAlarm:
                case ColliderTargetAndType.mainTrolleyForwardAlarm:
                case ColliderTargetAndType.mainTrolleyBackwardAlarm:
                case ColliderTargetAndType.auxTrolleyForwardAlarm:
                case ColliderTargetAndType.auxTrolleyBackwardAlarm:
                case ColliderTargetAndType.mtMhDownAlarm:
                case ColliderTargetAndType.mtMhUpAlarm:
                case ColliderTargetAndType.mtAhDownAlarm:
                case ColliderTargetAndType.mtAhUpAlarm:
                case ColliderTargetAndType.atMhDownAlarm:
                case ColliderTargetAndType.atMhUpAlarm:
                case ColliderTargetAndType.atAhDownAlarm:
                case ColliderTargetAndType.atAhUpAlarm:
                    anyAlarm = anyAlarm || collidersValues[i];
                    if(collidersValues[i])
                        message = "Опасность столкновения: " + images[cvt].objectName;
                    break;
            }
        }
        alarmOverlay.SetActive(anyAlarm);
        var text = alarmOverlay.GetComponentInChildren<Text>(true);
        if(text != null)
        {
            text.text = message;
        }
    }

    private void UpdateMaterials()
    {
        if (bridge != null)
        {
            bridge.alarm = collidersValues[(int)ColliderTargetAndType.bridgeLeftAlarm] || collidersValues[(int)ColliderTargetAndType.bridgeRightAlarm];
            bridge.warning = collidersValues[(int)ColliderTargetAndType.bridgeRightWarning] || collidersValues[(int)ColliderTargetAndType.bridgeLeftWarning];
        }
        if (mainTrolley != null)
        {
            mainTrolley.alarm = collidersValues[(int)ColliderTargetAndType.mainTrolleyBackwardAlarm] || collidersValues[(int)ColliderTargetAndType.mainTrolleyForwardAlarm];
            mainTrolley.warning = collidersValues[(int)ColliderTargetAndType.mainTrolleyBackwardWarning] || collidersValues[(int)ColliderTargetAndType.mainTrolleyForwardWarning];
        }
        if(auxTrolley != null)
        {
            auxTrolley.alarm = collidersValues[(int)ColliderTargetAndType.auxTrolleyBackwardAlarm] || collidersValues[(int)ColliderTargetAndType.auxTrolleyForwardAlarm];
            auxTrolley.warning = collidersValues[(int)ColliderTargetAndType.auxTrolleyBackwardWarning] || collidersValues[(int)ColliderTargetAndType.auxTrolleyForwardWarning];
        }
        foreach (var colliders in collidersDict)
        {
            foreach (var collider in colliders.Value)
            {
                foreach (var target in collider.triggeredWith)
                {
                    var materialSwitch = target.gameObject.GetComponent<MaterialSwitch>();
                    if(materialSwitch != null)
                    {
                        if (warnings.Contains(collider.type))
                        {
                            if (collider.triggered)
                            {
                                if (!materialSwitch.warnings.Contains(collider))
                                {
                                    materialSwitch.warnings.Add(collider);
                                }
                            }
                            else
                            {
                                if (materialSwitch.warnings.Contains(collider))
                                {
                                    materialSwitch.warnings.Remove(collider);
                                }
                            }
                        }
                        if (alarms.Contains(collider.type))
                        {
                            if (collider.triggered)
                            {
                                if (!materialSwitch.alarms.Contains(collider))
                                {
                                    materialSwitch.alarms.Add(collider);
                                }
                            }
                            else
                            {
                                if (materialSwitch.alarms.Contains(collider))
                                {
                                    materialSwitch.alarms.Remove(collider);
                                }
                            }
                        }
                        //materialSwitch.warning |= warnings.Contains(collider.type) && collider.triggered;
                        //materialSwitch.alarm |= alarms.Contains(collider.type) && collider.triggered;
                    }
                }
            }
        }
    }

    void Update()
    {
        for (int i = 0; i < (int)ColliderTargetAndType.count; i++)
        {
            var key = (ColliderTargetAndType)i;
            if (collidersDict.ContainsKey(key))
                SetColliderValue(key, collidersDict[key]);
        }
        UpdateImages();
        UpdateOverlay();
        
    }

    private void LateUpdate()
    {
        UpdateMaterials();
    }

    private void InitDicts()
    {
        for (int i = 0; i < (int)ColliderTargetAndType.count; i++)
        {
            collidersDict.Add((ColliderTargetAndType)i, new List<CollisionDetector2>());
        }
        
    }

    private void SetColliderValue(ColliderTargetAndType target, List<CollisionDetector2> colliders) {
        var prev = collidersValues[(int)target];
        var newVal = colliders.FindAll(c => c.triggered).Count > 0;
        if(!prev && newVal)
        {
            if (alarmsLogger != null)
            {
                if (alarms.Contains(target) && images.ContainsKey(target))
                {
                    alarmsLogger.Alarm("Опасность столкновения: " + images[target].objectName);
                }
                if (warnings.Contains(target))
                {
                    alarmsLogger.Alarm("Предупреждение о приближении: " + images[target].objectName);
                }
                
            }
            Debug.Log("Вход: " + target);
        }
        if(prev && !newVal)
        {
            Debug.Log("Выход: " + target);
        }
        collidersValues[(int)target] = newVal;
    }
}
