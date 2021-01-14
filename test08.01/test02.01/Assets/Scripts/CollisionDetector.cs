using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollisionDetector : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject triggeredWith;
    public GameObject collisionSource;
    public bool triggered;
    public delegate void Collision(GameObject target);
    public List<string> ignoreList;
    public event Collision OnEnter;
    public event Collision OnExit;
    private bool externalTrigger;
    private bool internalFlag;
    public List<CollisionDetector> propagateTo;
    public string message;
    public bool disabled = false;
    //public GameObject parent;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        triggered = internalFlag | externalTrigger;
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag("Sensor"))
        {
            internalFlag = false;
            triggered = internalFlag | externalTrigger;
            triggeredWith = null;
            //Debug.Log("OnEnter: " + other.gameObject.name + " -> " + gameObject.name);
            OnExit?.Invoke(other.gameObject);
            /*foreach (var target in propagateTo)
            {
                var scriptsParent = target.gameObject.GetComponentInParent<ParentDetector>();
                var rigidbodyParent = other.GetComponentInParent<ParentDetector>();
                if (scriptsParent == rigidbodyParent && scriptsParent != null)
                    target.SetUntriggered(scriptsParent.gameObject);
            }          */  
        }
    }

    void OnTriggerEnter(Collider other)
    {
        bool ignore = false;
        if(ignoreList != null)
            ignore = ignoreList.Contains(other.gameObject.tag);
        if (!ignore)
        {
            internalFlag = true;
            triggeredWith = other.gameObject;
            triggered = internalFlag | externalTrigger;
            //Debug.Log("OnExit: " + other.gameObject.name + " -> " + gameObject.name);
            OnEnter?.Invoke(other.gameObject);

            /*foreach (var target in propagateTo)
            {
                var scriptsParent = target.gameObject.GetComponentInParent<ParentDetector>();
                var rigidbodyParent = other.GetComponentInParent<ParentDetector>();
                if (scriptsParent == rigidbodyParent && scriptsParent != null)
                    target.SetTriggered(scriptsParent.gameObject);
            }*/
            
        }        
    }

    public void SetTriggered(GameObject triggerWith)
    {
        externalTrigger = true;
        OnEnter?.Invoke(triggerWith);
    }

    public void SetUntriggered(GameObject triggerWith)
    {
        externalTrigger = false;
        triggeredWith = null;
        OnExit?.Invoke(triggerWith);
    }
}
