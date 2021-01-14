using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void CollisionEventDelegate(GameObject target);

public enum ColliderTargetAndType
{
    none,
    bridgeLeftWarning,
    bridgeLeftAlarm,
    bridgeRightAlarm,
    bridgeRightWarning,
    mainTrolleyForwardWarning,
    mainTrolleyForwardAlarm,
    mainTrolleyBackwardAlarm,
    mainTrolleyBackwardWarning,
    auxTrolleyForwardWarning,
    auxTrolleyForwardAlarm,
    auxTrolleyBackwardAlarm,
    auxTrolleyBackwardWarning,

    mtMhDownWarning,
    mtMhDownAlarm,
    mtMhUpWarning,
    mtMhUpAlarm,

    mtAhDownWarning,
    mtAhDownAlarm,
    mtAhUpWarning,
    mtAhUpAlarm,

    atMhDownWarning,
    atMhDownAlarm,
    atMhUpWarning,
    atMhUpAlarm,

    atAhDownWarning,
    atAhDownAlarm,
    atAhUpWarning,
    atAhUpAlarm,
    count,
}

public class CollisionDetector2 : MonoBehaviour
{
    public bool colliderEnabled = true;
    public List<Collider> triggeredWith = new List<Collider>();
    public bool triggered = false;
    private bool lastTriggered = false;
    public List<string> ignoreList;
    public event CollisionEventDelegate OnEnter;
    public event CollisionEventDelegate OnExit;
    public string message;
    public ColliderTargetAndType type;
    private CollidersSizeCalc csc;

    // Start is called before the first frame update
    void Start()
    {
        csc = gameObject.GetComponent<CollidersSizeCalc>();
        if(csc != null)
        {
            switch (csc.target)
            {
                case ColliderTarget.Bridge:
                    switch (csc.type)
                    {
                        case ColliderType.Backward:
                        case ColliderType.Forward:
                        case ColliderType.Down:
                        case ColliderType.Up:
                        case ColliderType.None:
                            type = ColliderTargetAndType.none;
                            break;
                        case ColliderType.Left:
                            type = csc.isWarning ? ColliderTargetAndType.bridgeLeftWarning : ColliderTargetAndType.bridgeLeftAlarm;
                            break;
                        case ColliderType.Right:
                            type = csc.isWarning ? ColliderTargetAndType.bridgeRightWarning : ColliderTargetAndType.bridgeRightAlarm;
                            break;
                        default:
                            type = ColliderTargetAndType.none;
                            break;
                    }
                    break;
                case ColliderTarget.MainTrolley:
                    switch (csc.type)
                    {
                        case ColliderType.None:
                        case ColliderType.Up:
                        case ColliderType.Left:
                        case ColliderType.Down:
                        case ColliderType.Right:
                        default:
                            type = ColliderTargetAndType.none;
                            break;
                        case ColliderType.Forward:
                            type = csc.isWarning ? ColliderTargetAndType.mainTrolleyForwardWarning : ColliderTargetAndType.mainTrolleyForwardAlarm;
                            break;
                        case ColliderType.Backward:
                            type = csc.isWarning ? ColliderTargetAndType.mainTrolleyBackwardWarning : ColliderTargetAndType.mainTrolleyBackwardAlarm;
                            break;
                    }
                    break;
                case ColliderTarget.AuxTrolley:
                    switch (csc.type)
                    {
                        case ColliderType.None:
                        case ColliderType.Up:
                        case ColliderType.Left:
                        case ColliderType.Down:
                        case ColliderType.Right:
                        default:
                            type = ColliderTargetAndType.none;
                            break;
                        case ColliderType.Forward:
                            type = csc.isWarning ? ColliderTargetAndType.auxTrolleyForwardWarning : ColliderTargetAndType.auxTrolleyForwardAlarm;
                            break;
                        case ColliderType.Backward:
                            type = csc.isWarning ? ColliderTargetAndType.auxTrolleyBackwardWarning : ColliderTargetAndType.auxTrolleyBackwardAlarm;
                            break;
                    }
                    break;
                case ColliderTarget.MtMh:
                    switch (csc.type)
                    {
                        case ColliderType.None:
                        case ColliderType.Forward:
                        case ColliderType.Backward:
                        case ColliderType.Left:
                        case ColliderType.Right:
                        default:
                            type = ColliderTargetAndType.none;
                            break;
                        case ColliderType.Up:
                            type = csc.isWarning ? ColliderTargetAndType.mtMhUpWarning : ColliderTargetAndType.mtMhUpAlarm;
                            break;
                        case ColliderType.Down:
                            type = csc.isWarning ? ColliderTargetAndType.mtMhDownWarning : ColliderTargetAndType.mtMhDownAlarm;
                            break;
                    }
                    break;
                case ColliderTarget.MtAh:
                    switch (csc.type)
                    {
                        case ColliderType.None:
                        case ColliderType.Forward:
                        case ColliderType.Backward:
                        case ColliderType.Left:
                        case ColliderType.Right:
                        default:
                            type = ColliderTargetAndType.none;
                            break;
                        case ColliderType.Up:
                            type = csc.isWarning ? ColliderTargetAndType.mtAhUpWarning : ColliderTargetAndType.mtAhUpAlarm;
                            break;
                        case ColliderType.Down:
                            type = csc.isWarning ? ColliderTargetAndType.mtAhDownWarning : ColliderTargetAndType.mtAhDownAlarm;
                            break;
                    }
                    break;
                case ColliderTarget.AtMh:
                    switch (csc.type)
                    {
                        case ColliderType.None:
                        case ColliderType.Forward:
                        case ColliderType.Backward:
                        case ColliderType.Left:
                        case ColliderType.Right:
                        default:
                            type = ColliderTargetAndType.none;
                            break;
                        case ColliderType.Up:
                            type = csc.isWarning ? ColliderTargetAndType.atMhUpWarning : ColliderTargetAndType.atMhUpAlarm;
                            break;
                        case ColliderType.Down:
                            type = csc.isWarning ? ColliderTargetAndType.atMhDownWarning : ColliderTargetAndType.atMhDownAlarm;
                            break;
                    }
                    break;
                case ColliderTarget.AtAh:
                    switch (csc.type)
                    {
                        case ColliderType.None:
                        case ColliderType.Forward:
                        case ColliderType.Backward:
                        case ColliderType.Left:
                        case ColliderType.Right:
                        default:
                            type = ColliderTargetAndType.none;
                            break;
                        case ColliderType.Up:
                            type = csc.isWarning ? ColliderTargetAndType.atAhUpWarning : ColliderTargetAndType.atAhUpAlarm;
                            break;
                        case ColliderType.Down:
                            type = csc.isWarning ? ColliderTargetAndType.atAhDownWarning : ColliderTargetAndType.atAhDownAlarm;
                            break;
                    }
                    break;
                default:
                    break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        triggered = triggeredWith.Count > 0 && colliderEnabled;
        if(lastTriggered != triggered)
        {
            if (triggered)
            {
                OnEnter?.Invoke(gameObject);
                //Debug.Log("Enter");
            }
            else
            {
                OnExit?.Invoke(gameObject);
                //Debug.Log("Exit");
            }
        }
        lastTriggered = triggered;
    }

    private void OnTriggerEnter(Collider other)
    {
        bool ignore = false;
        if (ignoreList != null)
            ignore = ignoreList.Contains(other.gameObject.tag);
        if (!triggeredWith.Contains(other) && !other.gameObject.CompareTag("Sensor") && !ignore)
        {
            triggeredWith.Add(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (triggeredWith.Contains(other))
        {
            var ms = other.gameObject.GetComponent<MaterialSwitch>();
            
            triggeredWith.Remove(other);
            if (ms != null)
            {
                if (CraneCollisions2.warnings.Contains(type))
                {
                    ms.warnings.Remove(this);
                }
                if (CraneCollisions2.alarms.Contains(type))
                {
                    ms.alarms.Remove(this);
                }
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        /*if (!triggeredWith.Contains(other))
        {
            triggeredWith.Add(other);
        }
        */
    }
}
