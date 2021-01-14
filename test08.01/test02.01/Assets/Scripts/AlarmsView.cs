using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlarmsView : MonoBehaviour
{
    // Start is called before the first frame update
    public Alarms alarmsServiceInstance;
    public GameObject alarmsParent;
    public GameObject alarmInstance;
    private float timeout = 0.0f;
    private bool initialized = false;
    private bool noRefresh = false;
    private bool needRefresh = true;
    float refreshTimeout = 0.0f;

    void Start()
    {
        //alarmsParent?.transform.DetachChildren();
        
    }

    private void FixedUpdate()
    {
        if(timeout > 2.0f && !initialized)
        {
            
            if (alarmsServiceInstance != null && alarmInstance != null)
                if (alarmsServiceInstance.stateGood)
                {
                    alarmsServiceInstance.OnNewEvent += OnNewEvent;
                    RefreshAlarms();
                }
            initialized = true;
        }else
            timeout += Time.deltaTime;
        if (needRefresh)
            if(refreshTimeout > 2.0f)
            {
                RefreshAlarms();
                needRefresh = false;
            }else
            refreshTimeout += Time.deltaTime;
        else
            refreshTimeout = 0.0f;
    }

    private void RefreshAlarms()
    {
        if (noRefresh) return;
        Debug.Log("Refresh");
        if (alarmsParent != null)
        {
            var children = alarmsParent.GetComponentsInChildren<AlarmItem>();
            alarmsParent.transform.DetachChildren();
            foreach (var item in children)
            {
                if (!Application.isEditor || Application.isPlaying)
                    Destroy(item.gameObject);
                else
                    DestroyImmediate(item.gameObject);
            }
            
        }
        var alarms = alarmsServiceInstance.GetAlarms();
        foreach (var item in alarms)
        {
            var alarmRow = Instantiate(alarmInstance);
            var alarmScript = alarmRow.GetComponent<AlarmItem>();
            alarmScript.time = item.time;
            alarmScript.message = item.text;
            alarmRow?.transform.SetParent(alarmsParent.transform);
        }
    }

    private void OnNewEvent(int id)
    {
        needRefresh = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        //Debug.Log("Destroy");
        noRefresh = true;
        if(alarmsParent != null)
        {
            var children = alarmsParent.GetComponentsInChildren<AlarmItem>();
            alarmsParent.transform.DetachChildren();
            if (Application.isEditor && !Application.isPlaying)
                foreach (var item in children)
                {
                        //Destroy(item.gameObject);
                        DestroyImmediate(item.gameObject);
                }
            else
                foreach (var item in children)
                {
                    Destroy(item.gameObject);
                    //DestroyImmediate(item.gameObject);
                }
        }
        
    }
}
