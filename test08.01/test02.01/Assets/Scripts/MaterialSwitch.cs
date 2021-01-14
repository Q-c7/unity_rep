using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialSwitch : MonoBehaviour
{
    // Start is called before the first frame update
    private int prevState = 0;
    public Material defaultMaterial;
    public bool alarm;
    public Material alarmMaterial;
    public bool warning;
    public Material warningMaterial;
    private int state;
    public GameObject targetObject;
    public List<CollisionDetector2> warnings;
    public List<CollisionDetector2> alarms;
    public bool generalAlarm = false;
    public bool generalWarning = false;
    //public GameObject triggeredWith;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        generalAlarm = alarms.Count > 0 || alarm;
        generalWarning = warnings.Count > 0 || warning;
        if (generalAlarm)
        {
            state = 2;
        }
        else if (generalWarning)
        {
            state = 1;
        }
        else
        {
            state = 0;
        }
        if (prevState != state)
        {
            UpdateMaterials();
        }
        prevState = state;

    }

    private void FixedUpdate()
    {
        //warnings.Clear();
        //alarms.Clear();
    }

    private void UpdateMaterials()
    {
        switch (state)
        {
            case 0:
                if (defaultMaterial != null)
                {
                    ApplyMaterial(gameObject, defaultMaterial);
                    if (targetObject != null)
                    {
                        ApplyMaterial(targetObject, defaultMaterial);
                    }
                }
                break;
            case 1:
                if (warningMaterial != null)
                {
                    ApplyMaterial(gameObject, warningMaterial);
                    if (targetObject != null)
                    {
                        ApplyMaterial(targetObject, warningMaterial);
                    }
                }
                break;
            case 2:
                if (alarmMaterial != null)
                {
                    ApplyMaterial(gameObject, alarmMaterial);
                    if (targetObject != null)
                    {
                        ApplyMaterial(targetObject, alarmMaterial);
                    }
                }
                break;
            default:
                break;
        }        
    }

    private void LateUpdate()
    {
        
    }

    public void ClearAlarm()
    {
        alarm = false;
    }

    public void ClearWarning()
    {
        warning = false;
    }

    void ApplyMaterial(GameObject obj, Material material)
    {
        var meshRenderers = obj.GetComponentsInChildren<MeshRenderer>();
        foreach (var meshRenderer in meshRenderers)
        {
            var materials = meshRenderer.materials;
            if (material != null)
            {                
                for (int i = 0; i < materials.Length; i++)
                {
                    materials[i] = material;
                }
            }
            meshRenderer.materials = materials;
        }

    }
}
