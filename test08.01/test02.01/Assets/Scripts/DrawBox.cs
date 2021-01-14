using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.Linq;
using UnityEngine.SceneManagement;
using System;

public struct Obstacle
{
    public float x;
    public float y;
    public float z;
    public float l;
    public float w;
    public float h;
};

public class DrawBox : MonoBehaviour
{
    // Start is called before the first frame update
    private bool drawing = false;
    private bool panelDisableRequest = false;
    public Vector3 startPosition;
    public Vector3 currentPosition;
    public GameObject box;
    public GameObject dialog;
    public Button confirmButton;
    public InputField height;
    public List<Obstacle> obstacles;
    private static readonly int k = 5000;
    public GameObject cubePrefab;
    public List<GameObject> obstacleObjects;
    private bool deleteMenuShowed;
    public GameObject deleteMenu;
    public GameObject deleteListItemPrefab;
    public GameObject deleteList;
    public List<int> selectedForDelete;
    public GameObject confirmDeleteDialog;
    private string paramsPrefix = "";

    void Start()
    {
        paramsPrefix = Storage.GetPrefix();
        if (confirmButton != null)
            confirmButton.onClick.AddListener(delegate { OnConfirm(); });        
        obstacleObjects = new List<GameObject>();
        RefreshObstacles();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Delete"))
        {
            deleteMenuShowed = !deleteMenuShowed;
        }
        if (deleteMenu != null)
        {
            deleteMenu.SetActive(deleteMenuShowed);
        }
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Input.GetButtonDown("Fire1"))
        {
            if(Physics.Raycast(ray, out hit, 100) && !dialog.activeSelf && !deleteMenuShowed)
            {
                drawing = true;
                startPosition = hit.point;
                startPosition.y = 0;
                Debug.Log(startPosition);
                box = Instantiate(cubePrefab);
                if (box != null)
                {
                    box.transform.position = startPosition/* + new Vector3(0.5f, 0.5f, 0.5f)*/;
                }
            }
        }
        if (Input.GetButtonUp("Fire1"))
        {
            drawing = false;
            if (dialog != null && !panelDisableRequest && !deleteMenuShowed)
            {
                dialog.SetActive(true);
            }else if (panelDisableRequest)
            {
                dialog.SetActive(false);
                panelDisableRequest = false;
            }                
        }
        if (Input.GetButtonDown("Cancel"))
        {
            deleteMenuShowed = false;
            dialog.SetActive(false);
            if (box)
            {
                UnityEngine.Object.Destroy(box);
                box = null;
            }
        }
        if (drawing)
        {
            Vector3 tmp = new Vector3(1,1,1);
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
            {
                tmp = (hit.point-startPosition)/0.2f;
                tmp.y = 1;
            }
            tmp.y = 1;
            box.transform.localScale = tmp;
        }
    }

    void OnConfirm()
    {
        if(dialog != null)
        {
            var pos = box.transform.position * k;
            var size = box.transform.localScale * k;
            if(size.x < 0)
            {
                size.x = -size.x;
                pos.x -= size.x * 0.2f;
            }
            if(size.z < 0)
            {
                size.z = -size.z;
                pos.z -= size.z * 0.2f;
            }
            var obstacle = new Obstacle
            {
                x = pos.x < 0 ? 0 : pos.x,
                y = 0,
                z = pos.z < 0 ? 0 : pos.z,
                h = size.y,
                l = size.x,
                w = size.z
            };
            if (height != null && obstacle.l > 100.0f && obstacle.w > 1000f)
            {
                obstacle.h = int.Parse(height.text) * k / 1000;
                if (obstacle.h == 0)
                {
                    obstacle.h = k;
                }
                GetObstacles();
                obstacles.Add(obstacle);
                SaveObstacles();
            }
            else
                Debug.Log("To small object");
            
            panelDisableRequest = true;
        }
        RefreshObstacles();
    }

    private void SaveObstacles()
    {
        var json = JsonConvert.SerializeObject(obstacles);
        PlayerPrefs.SetString(paramsPrefix + "obstacles", json);
        PlayerPrefs.Save();
    }

    private void GetObstacles()
    {
        if (PlayerPrefs.HasKey(paramsPrefix + "obstacles"))
        {
            obstacles = JsonConvert.DeserializeObject<List<Obstacle>>(PlayerPrefs.GetString(paramsPrefix + "obstacles"));
        }
        else
        {
            obstacles = new List<Obstacle>();
        }
    }

    void RefreshObstacles()
    {
        cleanObstacles();
        GetObstacles();
        foreach (var obstacle in obstacles)
        {
            if(cubePrefab != null)
            {
                var instance = Instantiate(cubePrefab);
                instance.transform.position = (new Vector3(obstacle.x, obstacle.y, obstacle.z)) / k;
                instance.transform.localScale = (new Vector3(obstacle.l, obstacle.h, obstacle.w)) / k;
                obstacleObjects.Add(instance);
            }
        }
        if (box != null) {
            Destroy(box);
            box = null;
        };
        FillDeleteMenu();
    }

    void cleanObstacles()
    {
        foreach (var obstacle in obstacleObjects)
        {
            Destroy(obstacle);
        }
        obstacleObjects.Clear();
    }

    void CleanDeleteList()
    {
        var listItems = deleteList.GetComponentsInChildren<Toggle>();
        foreach (var item in listItems)
        {
            var listItem = item.transform.parent.gameObject;
            Destroy(listItem);
        }
    }

    void FillDeleteMenu()
    {
        CleanDeleteList();
        for(int key = 0; key<obstacles.Count; key++)
        {
            var item = obstacles[key];
            if(deleteListItemPrefab != null && deleteList != null)
            {
                var deleteListItem = Instantiate(deleteListItemPrefab, deleteList.transform);
                var toggle = deleteListItem.GetComponentInChildren<Toggle>();
                var heightInput = deleteListItem.GetComponentInChildren<InputField>();

                if (toggle != null)
                {
                    toggle.name = string.Format("{0}", key);
                    toggle.onValueChanged.AddListener(delegate { HandleDeleteSelection(toggle); });
                    var toggleLabel = toggle.GetComponentInChildren<Text>();
                    if (toggleLabel != null)
                    {
                        toggleLabel.text = string.Format("Положение: {0}, {1}, Габариты(ШхГхВ): {2}х{3}х{4}", item.x, item.z,
                            item.w/5, item.l/5, item.h/5);
                    }
                }

                if(heightInput != null)
                {
                    heightInput.name = string.Format("{0}", key);
                    heightInput.text = (item.h/5).ToString();
                    heightInput.onEndEdit.AddListener(delegate { HandleHeightEdit(heightInput); });
                }
            }
        }
    }

    private void HandleHeightEdit(InputField heightInput)
    {
        var obstacleIndex = int.Parse(heightInput.name);
        var obstacle = obstacles[obstacleIndex];
        obstacle.h = Int32.Parse(heightInput.text)*5;
        obstacles[obstacleIndex] = obstacle;
        SaveObstacles();
        RefreshObstacles();
    }

    void HandleDeleteSelection(Toggle toggle)
    {
        var obstacleIndex = int.Parse(toggle.name);
        if(selectedForDelete == null)
        {
            selectedForDelete = new List<int>();
        }
        if(obstacleIndex >= 0)
        {
            if (toggle.isOn)
                selectedForDelete.Add(obstacleIndex);
            else
                selectedForDelete.Remove(obstacleIndex);
            UpdateSelectedHighlight();
        }
    }

    void UpdateSelectedHighlight()
    {
        foreach (var obj in obstacleObjects)
        {
            obj.GetComponent<HeightDisplay>().select = false;
        }
        foreach (var selectedIndex in selectedForDelete)
        {
            var box = obstacleObjects[selectedIndex];
            box.GetComponent<HeightDisplay>().select = true;
        }
    }

    public void HandleDelete()
    {
        if(confirmDeleteDialog != null)
        {
            confirmDeleteDialog.SetActive(true);
        }
    }

    public void HandleConfirmDelete()
    {
        var newObstacles = obstacles.Select((obstacle, index) => new { obstacle, index }).ToDictionary(o => o.index, o => o.obstacle);
        var newObstacleObjects = obstacleObjects.Select((obstacle, index) => new { obstacle, index }).ToDictionary(o => o.index, o => o.obstacle);

        foreach (var selectedIndex in selectedForDelete)
        {
            Destroy(newObstacleObjects[selectedIndex]);
            newObstacles.Remove(selectedIndex);
            newObstacleObjects.Remove(selectedIndex);
        }
        obstacleObjects.Clear();
        obstacleObjects = newObstacleObjects.Values.ToList();
        obstacles = newObstacles.Values.ToList();

        selectedForDelete.Clear();
        SaveObstacles();
        FillDeleteMenu();
        confirmDeleteDialog.SetActive(false);
    }

    public void HandleCancelDelete()
    {
        if (confirmDeleteDialog != null)
        {
            confirmDeleteDialog.SetActive(false);
        }
    }

    public void HandleReturnToMain()
    {
        dialog.SetActive(false);
        SceneManager.LoadScene("Factory");
    }
}
