using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Newtonsoft.Json;
using System;

public struct TraverseStruct
{
    public bool twoHooks;
    public string name;
    public float length;
    public float width;
    public float height;
    public int id;
}

public class TraversesEditor : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject editor;
    public GameObject traverseLinePrefab;
    public GameObject listHolder;
    public Dictionary<int, GameObject> traversesList = new Dictionary<int, GameObject>();
    public Dictionary<int, TraverseStruct> traverses = new Dictionary<int, TraverseStruct>();
    public InputField traverseNameEditBox;
    public InputField traverseIdEditBox;
    public InputField traverseLenghtEditBox;
    public InputField traverseWidthEditBox;
    public InputField traverseHeightEditBox;
    public Toggle hookOption;
    public GameObject confirmDeleteDialog;
    private GameObject currentTraverseInEditor;
    private bool editorEnabled = false;
    private TraverseStruct currentTraverse;
    private bool editNew = false;
    private int currentIdx;
    private string paramsPrefix = "";
    public bool initialized = false;

    void Start()
    {
        Init();
        
    }

    public void Init()
    {
        paramsPrefix = Storage.GetPrefix();
        RefreshTraverses();
        initialized = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(editor != null)
        {
            editor.SetActive(editorEnabled);
        }
    }

    void RefreshTraverses ()
    {
        cleanTraverses();
        GetTraverses();
        foreach (var traverse in traverses)
        {
            if (traverseLinePrefab != null)
            {
                var instance = Instantiate(traverseLinePrefab);

                FillTraverseRow(traverse.Value, instance);
                traversesList.Add(traverse.Value.id, instance);
                UpdateTraversesList(listHolder, traversesList);
                //обработчики кнопок
                var editButton = instance.transform.Find("editButton").gameObject.GetComponent<Button>();
                var deleteButton = instance.transform.Find("deleteButton").gameObject.GetComponent<Button>();
                editButton.onClick.AddListener(() =>
                {
                    handleEditTraverse(traverse.Value.id);
                });
                deleteButton.onClick.AddListener(() =>
                {
                    handleDeleteTraverse(traverse.Value.id);
                });
            }
        }
    }

    private static void FillTraverseRow(TraverseStruct traverse, GameObject instance)
    {
        var traverseScript = instance.GetComponent<Traverse>();
        traverseScript.nameText.text = traverse.name;
        traverseScript.hookText.text = traverse.twoHooks ? "да" : "нет";
        traverseScript.lengthText.text = traverse.length.ToString() + " мм";
        traverseScript.widthText.text = traverse.width.ToString() + " мм";
        traverseScript.heightText.text = traverse.height.ToString() + " мм";
        traverseScript.idText.text = traverse.id.ToString();
    }

    private void SaveTraverses()
    {
        var json = JsonConvert.SerializeObject(traverses);
        PlayerPrefs.SetString(paramsPrefix + "traverses", json);
        PlayerPrefs.Save();
    }

    private void cleanTraverses()
    {
        foreach (var traverse in traversesList)
        {
            Destroy(traverse.Value);
        }
        traversesList.Clear();
    }

    private void GetTraverses()
    {
        if (PlayerPrefs.HasKey(paramsPrefix + "traverses"))
        {
            var travJson = PlayerPrefs.GetString(paramsPrefix + "traverses");
            //Debug.Log(travJson);
            traverses = JsonConvert.DeserializeObject<Dictionary<int, TraverseStruct>>(travJson);
        }
        else
        {
            traverses = new Dictionary<int, TraverseStruct>();
        }
    }

    public void HandleAddTreverse()
    {
        if (!editorEnabled)
        {
            editNew = true;
            editorEnabled = true;
            currentTraverse = new TraverseStruct();
            FillEditorFields();
        }
    }

    private void FillEditorFields()
    {
        traverseNameEditBox.text = currentTraverse.name;
        traverseIdEditBox.text = currentTraverse.id.ToString();
        hookOption.isOn = currentTraverse.twoHooks;
        traverseLenghtEditBox.text = currentTraverse.length.ToString();
        traverseWidthEditBox.text = currentTraverse.width.ToString();
        traverseHeightEditBox.text = currentTraverse.height.ToString();

    }

    public void HandleSaveEditedTraverse()
    {
        if (editorEnabled)
        {
            
            if (editNew)
            {
                currentTraverse.name = traverseNameEditBox.text;
                currentTraverse.twoHooks = hookOption.isOn;
                currentTraverse.id = int.Parse(traverseIdEditBox.text);
                currentTraverse.length = float.Parse(traverseLenghtEditBox.text);
                currentTraverse.width = float.Parse(traverseWidthEditBox.text);
                currentTraverse.height = float.Parse(traverseHeightEditBox.text);
                traverses.Add(currentTraverse.id, currentTraverse);
                var listItem = Instantiate(traverseLinePrefab);
                var idx = currentTraverse.id;
                FillTraverseRow(currentTraverse, listItem);
                traversesList.Add(currentTraverse.id, listItem);
                UpdateTraversesList(listHolder, traversesList);
                var editButton = listItem.transform.Find("editButton").gameObject.GetComponent<Button>();
                var deleteButton = listItem.transform.Find("deleteButton").gameObject.GetComponent<Button>();
                editButton.onClick.AddListener(() => {
                    handleEditTraverse(idx);
                });
                deleteButton.onClick.AddListener(() => {
                    handleDeleteTraverse(idx);
                });
            }
            else
            {
                var traverse = traverses[currentIdx];
                traverse.name = traverseNameEditBox.text;
                traverse.twoHooks = hookOption.isOn;
                traverse.id = int.Parse(traverseIdEditBox.text);
                traverse.length = float.Parse(traverseLenghtEditBox.text);
                traverse.width = float.Parse(traverseWidthEditBox.text);
                traverse.height = float.Parse(traverseHeightEditBox.text);
                traverses[currentIdx] = traverse;
                FillTraverseRow(traverse, traversesList[currentIdx]);
            }
            SaveTraverses();
            editorEnabled = false;
        }
    }

    public void handleEditTraverse(int index)
    {
        editorEnabled = true;
        editNew = false;
        currentTraverse = traverses[index];
        currentIdx = index;
        FillEditorFields();
    }

    public void handleDeleteTraverse(int index)
    {
        currentIdx = index;
        confirmDeleteDialog?.SetActive(true);
    }

    public void handleDeleteConfirm()
    {
        traverses.Remove(currentIdx);
        Destroy(traversesList[currentIdx]);
        traversesList.Remove(currentIdx);
        confirmDeleteDialog?.SetActive(false);
        UpdateTraversesList(listHolder, traversesList);
        SaveTraverses();
    }

    public void handleDeleteCancel()
    {
        confirmDeleteDialog?.SetActive(false);
    }

    public void UpdateTraversesList(GameObject parent, Dictionary<int, GameObject> traverses)
    {
        parent.transform.DetachChildren();
        foreach (var traverse in traverses)
        {
            traverse.Value.transform.SetParent(parent.transform);
        }
    }

    private void OnDisable()
    {
        editorEnabled = false;
    }
}
