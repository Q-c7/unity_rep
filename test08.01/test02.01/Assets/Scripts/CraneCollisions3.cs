using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
// using System.IO;
using UnityEngine;

public class CraneCollisions3 : MonoBehaviour
{    
    /**
     * Скрипты кранов
     * должны идти в том-же порядке, что и элементы массива crane
     */
    public List<Crane> craneScripts;
    public ObstaclesLoader obstaclesScript;
    public List<GameObject> collonades;
    /**
     * массив элементов, в которых находятся пиктограммы запретов
     * должны идти в том-же порядке, что и craneScripts
     */
    public List<Info> imageContainers;
    /**
     * массив элементов, в которых находятся перекрывающие сообщения о столкновении
     * должны идти в том-же порядке, что и craneScripts
     */
    public List<AlarmOverlay> alarmOverlays;
    public Alarms alarmsView;
    public Alarms alarmsView_320;
    public Alarms alarmsView_120;
    public Alarms alarmsView_25;
    // словарь кранов с подсловарем картинок согласно типу столкновения
    private Dictionary<CraneType, Dictionary<ColliderTargetAndType, ImageIndicator>> images = new Dictionary<CraneType, Dictionary<ColliderTargetAndType, ImageIndicator>>();
    // ======================================================================================
    //private Dictionary<int, TraverseStruct> traversesCollection_ald;
    private TraverseStruct[] TR_ald;
    private int razmer_arr_tr = 0;
    #region ПР101 - ОПРЕДЕЛЯЕМ ТИП ДАННЫХ - ОПРЕДЕЛЕНИЕ ПЕРЕМЕННЫХ
    //
    public int w_e_wr_DBSQL = 0;
    public bool first_start = false;
    public bool first_start_tr = false;
    public bool flag_first_start_tr = false;

    private CRANEs[] crane; // private
    int i = 0;   // НОМЕР УСЛОВНО СТАТИЧЕСКИХ ЭЛЕМЕНТОВ - СТЕНЫ, ЭЛЕМЕНТЫ КРАНОВ, КОЛОННЫ, ПРЕПЯТСТИВИЯ И Т.П. 
    int NEW_i = 0;
    int temp_i = 0;   // для смещения 
    int j = 0;   // НОМЕР КРАНА 1 - КРАН 320Т/ 2 - КРАН 120Т/ 3 - КРАН 25Т
    int f = 0;   // НОМЕР ЭЛЕМЕНТА КРАНА СМ. ФАЙЛ EXSEL
                 //
                 // float temp_x = 0;
                 // float temp_y = 0;
                 // float temp_z = 0;
                 //
                 //float temp_obj_x = 0;
                 // float temp_obj_y = 0;
                 // float temp_obj_z = 0;
    #endregion
    // ======================================================================================    
    private Dictionary<int, TraverseStruct> traversesCollection;
    private const float traversesRefreshTime = 10f;
    private float elapsed = 0.0f;
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
    private void FixedUpdate()
    {
        if (elapsed >= traversesRefreshTime)
        {
            elapsed = 0;
            GetTraverses();
            TR_ald = traversesCollection.Values.ToArray();
            razmer_arr_tr = traversesCollection.Count;
            flag_first_start_tr = false;
        }
        elapsed += Time.deltaTime;
    }
    // ======================================================================================
    #region ПР103 - ОПРЕДЕЛЯЕМ ТИП ДАННЫХ - ОБЪЕКТЫ - СТАТИЧЕСКИЕ ОБЪЕКТЫ В ЦЕХЕ
    // НОМЕР условно статического объекта - i
    // 1-20 - КРАН 1 - 320Т
    // 0/21-50 - КРАН 1 - 320Т - РЕЗЕРВ
    // 31-99 - ТОЛЬКО СТАТИЧНЫЕ ОБЪЕКТЫ - КОЛОННЫ И СТЕНЫ
    // 101-120 - КРАН 2 - 120Т
    // 100/121-150 - КРАН 2 - 120Т - РЕЗЕРВ
    // 151-199 - ТОЛЬКО СТАТИЧНЫЕ ОБЪЕКТЫ - РЕЗЕРВ
    // 201-220 - КРАН 3 - 25Т
    // 200/221-250 - КРАН 3 - 25Т - РЕЗЕРВ
    // 251-299 - ТОЛЬКО СТАТИЧНЫЕ ОБЪЕКТЫ - РЕЗЕРВ
    // 301 - 811 - ТОЛЬКО СТАТИЧНЫЕ ОБЪЕКТЫ - ПРЕПЯТСТВИЯ
    // ФЛАГ СТОЛКНОВЕНИЯ
    public bool[] Obj_ALL_XYZ_KRASH_warn = new bool[812];
    public bool[] Obj_ALL_XYZ_KRASH_err = new bool[812];
    // ГАБАРИТЫ
    public float[] Obj_ALL_X = new float[812];
    public float[] Obj_ALL_Y = new float[812];
    public float[] Obj_ALL_Z = new float[812];
    // БАЗОВАЯ ТОЧКА
    public float[] Obj_ALL_ST_P_X = new float[812];
    public float[] Obj_ALL_ST_P_Y = new float[812];
    public float[] Obj_ALL_ST_P_Z = new float[812];
    // 2020_08_08
    // ФОРМИРУЕТСЯ НА ОСНОВЕ АНАЛИЗА ДАННЫХ ГАБАРИТОВ ОБЪЕКТА, ЕСЛИ ОДИН ИЗ ГАБАРИТОВ МЕНЬШЕ ИЛИ РАВЕН НУЛЮ
    // ОБЪЕКТ НЕ ПРИНМАЮТСЯ ДЛЯ АНАЛИЗА
    public bool[] Obj_ALL_Y_N = new bool[812];
    // **************************************************************************************
    // 2020 08 27
    // ======================================================================================
    
    //
    public bool FLAF_COMMPARE_X = false;
    public bool FLAF_COMMPARE_Y = false;
    public bool FLAF_COMMPARE_Z = false;
    //
    public bool FLAF_COMMPARE_ST_P_X = false;
    public bool FLAF_COMMPARE_ST_P_Y = false;
    public bool FLAF_COMMPARE_ST_P_Z = false;
    //
    public bool FLAF_COMMPARE_ALL = false;
    //
    public bool FLAF_COMMPARE_TR_id = false;
    public bool FLAF_COMMPARE_TR_TWO_HOOK = false;
    public bool FLAF_COMMPARE_TR_NAME = false;
    // ***************************************************************************************

    // ======================================================================================
    #endregion
    // ======================================================================================
    // Start is called before the first frame update
    void Start()
    {
        // GetTraverses();
        Globals.FLAG_F_START_1MAIL = true;
        Globals.FLAG_F_START_2MAIL = true;
        first_start = true;
        first_start_tr = true;
        flag_first_start_tr = true;
        // Инициализация массива картинок
        if (craneScripts.Count == 3)
        {
            foreach (var script in craneScripts)
            {
                var craneConnectionScript = script.gameObject.GetComponent<CraneConnection>();
                var craneType = craneConnectionScript.craneType;
                var j = craneScripts.FindIndex(c => c == script);
                images.Add(craneType, new Dictionary<ColliderTargetAndType, ImageIndicator>());
                if (imageContainers[j] != null)
                {
                    var ic = imageContainers[j];
                    var boundImages = ic.gameObject.GetComponentsInChildren<ImageIndicator>(true);
                    foreach (var image in boundImages)
                    {
                        images[craneType].Add(image.type, image);
                    }
                }
            }

        }
        // ======================================================================================
        #region ПР102 - ОПРЕДЕЛЯЕМ ТИП ДАННЫХ - КРАНЫ
        // ===================================================================
        crane = new CRANEs[5];
        // НОМЕР КРАНА - j
        // j = 1 - КРАН 320Т
        // j = 2 - КРАН 120Т
        // j = 3 - КРАН 25Т
        // ОПИСАНИЕ ДАННЫХ КРАНА - CLASS - CRANEs
        for (j = 1; j <= 4; j++)
        {
            crane[j] = new CRANEs();
        }
        // ===================================================================
        #endregion
        // ======================================================================================
        // ======================================================================================
        #region ПР104 - КРАН 1/2/3 - ОПРЕДЕЛЕНИЕ КАКИЕ ЭЛЕМЕНТЫ АНАЛИЗИРУЮТСЯ НА СТОЛКНОВЕНИЕ

        for (j = 1; j <= 3; j++)
        {
            // temp_i = 0 - КРАН 1 - 320 Т - ОПРЕДЕЛЕНИЕ КАКИЕ ЭЛЕМЕНТЫ АНАЛИЗИРУЮТСЯ НА СТОЛКНОВЕНИЕ
            // temp_i = 100 - КРАН 2 - 120 Т - ОПРЕДЕЛЕНИЕ КАКИЕ ЭЛЕМЕНТЫ АНАЛИЗИРУЮТСЯ НА СТОЛКНОВЕНИЕ
            // temp_i = 200 - КРАН 3 - 25 Т - ОПРЕДЕЛЕНИЕ КАКИЕ ЭЛЕМЕНТЫ АНАЛИЗИРУЮТСЯ НА СТОЛКНОВЕНИЕ
            if (j == 1)
            {
                temp_i = 0;
            }
            if (j == 2)
            {
                temp_i = 100;
            }
            if (j == 3)
            {
                temp_i = 200;
            }
            #region МОСТ
            f = 1;
            for (i = 0; i < 812; i++)
            {
                crane[j].KRASH_cntrl_yn[f, i] = true;
            }
            // Мост не анализируется с колоннами!!!!!!!!!!!!!!!!!!!
            for (i = 31; i <= 99; i++)
            {
                crane[j].KRASH_cntrl_yn[f, i] = false;
            }

            crane[j].KRASH_cntrl_yn[f, (temp_i + 1)] = false; // XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX   МОСТ
            crane[j].KRASH_cntrl_yn[f, (temp_i + 2)] = false; // ТЕЛЕЖКА 1
                                                              //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 3)] = false; // ТЕЛЕЖКА 1 - ПОДЪЕМ 1 - ТРОСС + КРЮК
            crane[j].KRASH_cntrl_yn[f, (temp_i + 4)] = false; // ТЕЛЕЖКА 1 - ПОДЪЕМ 1 - тросс + крюк + траверса => ВСЕГДА FALSE
            crane[j].KRASH_cntrl_yn[f, (temp_i + 5)] = true; // ТЕЛЕЖКА 1 - ПОДЪЕМ 1 - траверса
            crane[j].KRASH_cntrl_yn[f, (temp_i + 6)] = true; // ТЕЛЕЖКА 1 - ПОДЪЕМ 1 - груз
                                                             //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 7)] = false; // ТЕЛЕЖКА 1 - ПОДЪЕМ 2 - ТРОСС + КРЮК
            crane[j].KRASH_cntrl_yn[f, (temp_i + 8)] = false; // ТЕЛЕЖКА 1 - ПОДЪЕМ 2 - тросс + крюк + траверса => ВСЕГДА FALSE
            crane[j].KRASH_cntrl_yn[f, (temp_i + 9)] = true; // ТЕЛЕЖКА 1 - ПОДЪЕМ 2 - траверса
            crane[j].KRASH_cntrl_yn[f, (temp_i + 10)] = true; // ТЕЛЕЖКА 1 - ПОДЪЕМ 2 - груз
                                                              //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 11)] = false; // тросс + крюк + траверса + 3 подъем тросс + крюк + траверса => ВСЕГДА FALSE
                                                               //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 12)] = false; // ТЕЛЕЖКА 2
                                                               //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 13)] = false; // ТЕЛЕЖКА 2 - ПОДЪЕМ 1 - ТРОСС + КРЮК
            crane[j].KRASH_cntrl_yn[f, (temp_i + 14)] = false; // ТЕЛЕЖКА 2 - ПОДЪЕМ 1 - тросс + крюк + траверса => ВСЕГДА FALSE
            crane[j].KRASH_cntrl_yn[f, (temp_i + 15)] = true; // ТЕЛЕЖКА 2 - ПОДЪЕМ 1 - траверса
            crane[j].KRASH_cntrl_yn[f, (temp_i + 16)] = true; // ТЕЛЕЖКА 2 - ПОДЪЕМ 1 - груз
                                                              //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 17)] = false; // ТЕЛЕЖКА 2 - ПОДЪЕМ 2 - ТРОСС + КРЮК
            crane[j].KRASH_cntrl_yn[f, (temp_i + 18)] = false; // ТЕЛЕЖКА 2 - ПОДЪЕМ 2 - тросс + крюк + траверса => ВСЕГДА FALSE
            crane[j].KRASH_cntrl_yn[f, (temp_i + 19)] = true; // ТЕЛЕЖКА 2 - ПОДЪЕМ 2 - траверса
            crane[j].KRASH_cntrl_yn[f, (temp_i + 20)] = true; // ТЕЛЕЖКА 2 - ПОДЪЕМ 2 - груз
                                                              //       
            #endregion
            //
            #region ТЕЛЕЖКА 1       
            f = 2;
            for (i = 0; i < 812; i++)
            {
                crane[j].KRASH_cntrl_yn[f, i] = true;
            }
            crane[j].KRASH_cntrl_yn[f, (temp_i + 1)] = false; // МОСТ
            crane[j].KRASH_cntrl_yn[f, (temp_i + 2)] = false; // XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX ТЕЛЕЖКА 1
                                                              //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 3)] = false; // ТЕЛЕЖКА 1 - ПОДЪЕМ 1 - ТРОСС + КРЮК
            crane[j].KRASH_cntrl_yn[f, (temp_i + 4)] = false; // ТЕЛЕЖКА 1 - ПОДЪЕМ 1 - тросс + крюк + траверса => ВСЕГДА FALSE
            crane[j].KRASH_cntrl_yn[f, (temp_i + 5)] = true; // ТЕЛЕЖКА 1 - ПОДЪЕМ 1 - траверса
            crane[j].KRASH_cntrl_yn[f, (temp_i + 6)] = true; // ТЕЛЕЖКА 1 - ПОДЪЕМ 1 - груз
                                                             //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 7)] = false; // ТЕЛЕЖКА 1 - ПОДЪЕМ 2 - ТРОСС + КРЮК
            crane[j].KRASH_cntrl_yn[f, (temp_i + 8)] = false; // ТЕЛЕЖКА 1 - ПОДЪЕМ 2 - тросс + крюк + траверса => ВСЕГДА FALSE
            crane[j].KRASH_cntrl_yn[f, (temp_i + 9)] = true; // ТЕЛЕЖКА 1 - ПОДЪЕМ 2 - траверса
            crane[j].KRASH_cntrl_yn[f, (temp_i + 10)] = true; // ТЕЛЕЖКА 1 - ПОДЪЕМ 2 - груз
                                                              //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 11)] = false; // тросс + крюк + траверса + 3 подъем тросс + крюк + траверса => ВСЕГДА FALSE
                                                               //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 12)] = false; // ТЕЛЕЖКА 2
                                                               //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 13)] = false; // ТЕЛЕЖКА 2 - ПОДЪЕМ 1 - ТРОСС + КРЮК
            crane[j].KRASH_cntrl_yn[f, (temp_i + 14)] = false; // ТЕЛЕЖКА 2 - ПОДЪЕМ 1 - тросс + крюк + траверса => ВСЕГДА FALSE
            crane[j].KRASH_cntrl_yn[f, (temp_i + 15)] = true; // ТЕЛЕЖКА 2 - ПОДЪЕМ 1 - траверса
            crane[j].KRASH_cntrl_yn[f, (temp_i + 16)] = true; // ТЕЛЕЖКА 2 - ПОДЪЕМ 1 - груз
                                                              //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 17)] = false; // ТЕЛЕЖКА 2 - ПОДЪЕМ 2 - ТРОСС + КРЮК
            crane[j].KRASH_cntrl_yn[f, (temp_i + 18)] = false; // ТЕЛЕЖКА 2 - ПОДЪЕМ 2 - тросс + крюк + траверса => ВСЕГДА FALSE
            crane[j].KRASH_cntrl_yn[f, (temp_i + 19)] = true; // ТЕЛЕЖКА 2 - ПОДЪЕМ 2 - траверса
            crane[j].KRASH_cntrl_yn[f, (temp_i + 20)] = true; // ТЕЛЕЖКА 2 - ПОДЪЕМ 2 - груз
                                                              //       
            #endregion
            //
            #region ПОДЪЕМ 1 - ТРОСС + КРЮК
            f = 3;
            for (i = 0; i < 812; i++)
            {
                crane[j].KRASH_cntrl_yn[f, i] = true;
            }
            crane[j].KRASH_cntrl_yn[f, (temp_i + 1)] = false; // МОСТ
            crane[j].KRASH_cntrl_yn[f, (temp_i + 2)] = false; // ТЕЛЕЖКА 1
                                                              //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 3)] = false; // XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX  ТЕЛЕЖКА 1 - ПОДЪЕМ 1 - ТРОСС + КРЮК
            crane[j].KRASH_cntrl_yn[f, (temp_i + 4)] = false; // ТЕЛЕЖКА 1 - ПОДЪЕМ 1 - тросс + крюк + траверса => ВСЕГДА FALSE
            crane[j].KRASH_cntrl_yn[f, (temp_i + 5)] = false; // ТЕЛЕЖКА 1 - ПОДЪЕМ 1 - траверса
            crane[j].KRASH_cntrl_yn[f, (temp_i + 6)] = false; // ТЕЛЕЖКА 1 - ПОДЪЕМ 1 - груз
                                                              //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 7)] = false; // ТЕЛЕЖКА 1 - ПОДЪЕМ 2 - ТРОСС + КРЮК
            crane[j].KRASH_cntrl_yn[f, (temp_i + 8)] = false; // ТЕЛЕЖКА 1 - ПОДЪЕМ 2 - тросс + крюк + траверса => ВСЕГДА FALSE
            crane[j].KRASH_cntrl_yn[f, (temp_i + 9)] = true; // ТЕЛЕЖКА 1 - ПОДЪЕМ 2 - траверса
            crane[j].KRASH_cntrl_yn[f, (temp_i + 10)] = true; // ТЕЛЕЖКА 1 - ПОДЪЕМ 2 - груз
                                                              //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 11)] = false; // тросс + крюк + траверса + 3 подъем тросс + крюк + траверса => ВСЕГДА FALSE
                                                               //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 12)] = false; // ТЕЛЕЖКА 2
                                                               //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 13)] = false; // ТЕЛЕЖКА 2 - ПОДЪЕМ 1 - ТРОСС + КРЮК
            crane[j].KRASH_cntrl_yn[f, (temp_i + 14)] = false; // ТЕЛЕЖКА 2 - ПОДЪЕМ 1 - тросс + крюк + траверса => ВСЕГДА FALSE
            crane[j].KRASH_cntrl_yn[f, (temp_i + 15)] = true; // ТЕЛЕЖКА 2 - ПОДЪЕМ 1 - траверса
            crane[j].KRASH_cntrl_yn[f, (temp_i + 16)] = true; // ТЕЛЕЖКА 2 - ПОДЪЕМ 1 - груз
                                                              //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 17)] = false; // ТЕЛЕЖКА 2 - ПОДЪЕМ 2 - ТРОСС + КРЮК
            crane[j].KRASH_cntrl_yn[f, (temp_i + 18)] = false; // ТЕЛЕЖКА 2 - ПОДЪЕМ 2 - тросс + крюк + траверса => ВСЕГДА FALSE
            crane[j].KRASH_cntrl_yn[f, (temp_i + 19)] = true; // ТЕЛЕЖКА 2 - ПОДЪЕМ 2 - траверса
            crane[j].KRASH_cntrl_yn[f, (temp_i + 20)] = true; // ТЕЛЕЖКА 2 - ПОДЪЕМ 2 - груз
                                                              //
            #endregion
            #region ПОДЪЕМ 1 - тросс + крюк + траверса => НЕТ
            f = 4;
            for (i = 0; i < 812; i++)
            {
                crane[j].KRASH_cntrl_yn[f, i] = false;
            }

            #endregion
            #region ПОДЪЕМ 1 - траверса
            f = 5;
            for (i = 0; i < 812; i++)
            {
                crane[j].KRASH_cntrl_yn[f, i] = true;
            }
            crane[j].KRASH_cntrl_yn[f, (temp_i + 1)] = true; // МОСТ
            crane[j].KRASH_cntrl_yn[f, (temp_i + 2)] = true; // ТЕЛЕЖКА 1
                                                             //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 3)] = false; // ТЕЛЕЖКА 1 - ПОДЪЕМ 1 - ТРОСС + КРЮК
            crane[j].KRASH_cntrl_yn[f, (temp_i + 4)] = false; // ТЕЛЕЖКА 1 - ПОДЪЕМ 1 - тросс + крюк + траверса => ВСЕГДА FALSE
            crane[j].KRASH_cntrl_yn[f, (temp_i + 5)] = false; // XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX  ТЕЛЕЖКА 1 - ПОДЪЕМ 1 - траверса
            crane[j].KRASH_cntrl_yn[f, (temp_i + 6)] = false; // ТЕЛЕЖКА 1 - ПОДЪЕМ 1 - груз
                                                              //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 7)] = true; // ТЕЛЕЖКА 1 - ПОДЪЕМ 2 - ТРОСС + КРЮК
            crane[j].KRASH_cntrl_yn[f, (temp_i + 8)] = false; // ТЕЛЕЖКА 1 - ПОДЪЕМ 2 - тросс + крюк + траверса => ВСЕГДА FALSE
            crane[j].KRASH_cntrl_yn[f, (temp_i + 9)] = true; // ТЕЛЕЖКА 1 - ПОДЪЕМ 2 - траверса
            crane[j].KRASH_cntrl_yn[f, (temp_i + 10)] = true; // ТЕЛЕЖКА 1 - ПОДЪЕМ 2 - груз
                                                              //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 11)] = false; // тросс + крюк + траверса + 3 подъем тросс + крюк + траверса => ВСЕГДА FALSE
                                                               //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 12)] = true; // ТЕЛЕЖКА 2
                                                              //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 13)] = true; // ТЕЛЕЖКА 2 - ПОДЪЕМ 1 - ТРОСС + КРЮК
            crane[j].KRASH_cntrl_yn[f, (temp_i + 14)] = false; // ТЕЛЕЖКА 2 - ПОДЪЕМ 1 - тросс + крюк + траверса => ВСЕГДА FALSE
            crane[j].KRASH_cntrl_yn[f, (temp_i + 15)] = true; // ТЕЛЕЖКА 2 - ПОДЪЕМ 1 - траверса
            crane[j].KRASH_cntrl_yn[f, (temp_i + 16)] = true; // ТЕЛЕЖКА 2 - ПОДЪЕМ 1 - груз
                                                              //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 17)] = true; // ТЕЛЕЖКА 2 - ПОДЪЕМ 2 - ТРОСС + КРЮК
            crane[j].KRASH_cntrl_yn[f, (temp_i + 18)] = false; // ТЕЛЕЖКА 2 - ПОДЪЕМ 2 - тросс + крюк + траверса => ВСЕГДА FALSE
            crane[j].KRASH_cntrl_yn[f, (temp_i + 19)] = true; // ТЕЛЕЖКА 2 - ПОДЪЕМ 2 - траверса
            crane[j].KRASH_cntrl_yn[f, (temp_i + 20)] = true; // ТЕЛЕЖКА 2 - ПОДЪЕМ 2 - груз
                                                              //
            #endregion
            #region ПОДЪЕМ 1 - груз
            f = 6;
            for (i = 0; i < 812; i++)
            {
                crane[j].KRASH_cntrl_yn[f, i] = true;
            }
            crane[j].KRASH_cntrl_yn[f, (temp_i + 1)] = true; // МОСТ
            crane[j].KRASH_cntrl_yn[f, (temp_i + 2)] = true; // ТЕЛЕЖКА 1
                                                             //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 3)] = false; // ТЕЛЕЖКА 1 - ПОДЪЕМ 1 - ТРОСС + КРЮК
            crane[j].KRASH_cntrl_yn[f, (temp_i + 4)] = false; // ТЕЛЕЖКА 1 - ПОДЪЕМ 1 - тросс + крюк + траверса => ВСЕГДА FALSE
            crane[j].KRASH_cntrl_yn[f, (temp_i + 5)] = false; // ТЕЛЕЖКА 1 - ПОДЪЕМ 1 - траверса
            crane[j].KRASH_cntrl_yn[f, (temp_i + 6)] = false; // XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX  ТЕЛЕЖКА 1 - ПОДЪЕМ 1 - груз
                                                              //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 7)] = true; // ТЕЛЕЖКА 1 - ПОДЪЕМ 2 - ТРОСС + КРЮК
            crane[j].KRASH_cntrl_yn[f, (temp_i + 8)] = false; // ТЕЛЕЖКА 1 - ПОДЪЕМ 2 - тросс + крюк + траверса => ВСЕГДА FALSE
            crane[j].KRASH_cntrl_yn[f, (temp_i + 9)] = true; // ТЕЛЕЖКА 1 - ПОДЪЕМ 2 - траверса
            crane[j].KRASH_cntrl_yn[f, (temp_i + 10)] = true; // ТЕЛЕЖКА 1 - ПОДЪЕМ 2 - груз
                                                              //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 11)] = false; // тросс + крюк + траверса + 3 подъем тросс + крюк + траверса => ВСЕГДА FALSE
                                                               //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 12)] = true; // ТЕЛЕЖКА 2
                                                              //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 13)] = true; // ТЕЛЕЖКА 2 - ПОДЪЕМ 1 - ТРОСС + КРЮК
            crane[j].KRASH_cntrl_yn[f, (temp_i + 14)] = false; // ТЕЛЕЖКА 2 - ПОДЪЕМ 1 - тросс + крюк + траверса => ВСЕГДА FALSE
            crane[j].KRASH_cntrl_yn[f, (temp_i + 15)] = true; // ТЕЛЕЖКА 2 - ПОДЪЕМ 1 - траверса
            crane[j].KRASH_cntrl_yn[f, (temp_i + 16)] = true; // ТЕЛЕЖКА 2 - ПОДЪЕМ 1 - груз
                                                              //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 17)] = true; // ТЕЛЕЖКА 2 - ПОДЪЕМ 2 - ТРОСС + КРЮК
            crane[j].KRASH_cntrl_yn[f, (temp_i + 18)] = false; // ТЕЛЕЖКА 2 - ПОДЪЕМ 2 - тросс + крюк + траверса => ВСЕГДА FALSE
            crane[j].KRASH_cntrl_yn[f, (temp_i + 19)] = true; // ТЕЛЕЖКА 2 - ПОДЪЕМ 2 - траверса
            crane[j].KRASH_cntrl_yn[f, (temp_i + 20)] = true; // ТЕЛЕЖКА 2 - ПОДЪЕМ 2 - груз
                                                              //
            #endregion
            //
            //
            #region ПОДЪЕМ 2 - ТРОСС + КРЮК
            f = 7;
            for (i = 0; i < 812; i++)
            {
                crane[j].KRASH_cntrl_yn[f, i] = true;
            }
            crane[j].KRASH_cntrl_yn[f, (temp_i + 1)] = false; // МОСТ
            crane[j].KRASH_cntrl_yn[f, (temp_i + 2)] = false; // ТЕЛЕЖКА 1
                                                              //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 3)] = false; // ТЕЛЕЖКА 1 - ПОДЪЕМ 1 - ТРОСС + КРЮК
            crane[j].KRASH_cntrl_yn[f, (temp_i + 4)] = false; // ТЕЛЕЖКА 1 - ПОДЪЕМ 1 - тросс + крюк + траверса => ВСЕГДА FALSE
            crane[j].KRASH_cntrl_yn[f, (temp_i + 5)] = true; // ТЕЛЕЖКА 1 - ПОДЪЕМ 1 - траверса
            crane[j].KRASH_cntrl_yn[f, (temp_i + 6)] = true; // ТЕЛЕЖКА 1 - ПОДЪЕМ 1 - груз
                                                             //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 7)] = false; // XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX  ТЕЛЕЖКА 1 - ПОДЪЕМ 2 - ТРОСС + КРЮК
            crane[j].KRASH_cntrl_yn[f, (temp_i + 8)] = false; // ТЕЛЕЖКА 1 - ПОДЪЕМ 2 - тросс + крюк + траверса => ВСЕГДА FALSE
            crane[j].KRASH_cntrl_yn[f, (temp_i + 9)] = false; // ТЕЛЕЖКА 1 - ПОДЪЕМ 2 - траверса
            crane[j].KRASH_cntrl_yn[f, (temp_i + 10)] = false; // ТЕЛЕЖКА 1 - ПОДЪЕМ 2 - груз
                                                               //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 11)] = false; // тросс + крюк + траверса + 3 подъем тросс + крюк + траверса => ВСЕГДА FALSE
                                                               //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 12)] = false; // ТЕЛЕЖКА 2
                                                               //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 13)] = false; // ТЕЛЕЖКА 2 - ПОДЪЕМ 1 - ТРОСС + КРЮК
            crane[j].KRASH_cntrl_yn[f, (temp_i + 14)] = false; // ТЕЛЕЖКА 2 - ПОДЪЕМ 1 - тросс + крюк + траверса => ВСЕГДА FALSE
            crane[j].KRASH_cntrl_yn[f, (temp_i + 15)] = true; // ТЕЛЕЖКА 2 - ПОДЪЕМ 1 - траверса
            crane[j].KRASH_cntrl_yn[f, (temp_i + 16)] = true; // ТЕЛЕЖКА 2 - ПОДЪЕМ 1 - груз
                                                              //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 17)] = false; // ТЕЛЕЖКА 2 - ПОДЪЕМ 2 - ТРОСС + КРЮК
            crane[j].KRASH_cntrl_yn[f, (temp_i + 18)] = false; // ТЕЛЕЖКА 2 - ПОДЪЕМ 2 - тросс + крюк + траверса => ВСЕГДА FALSE
            crane[j].KRASH_cntrl_yn[f, (temp_i + 19)] = true; // ТЕЛЕЖКА 2 - ПОДЪЕМ 2 - траверса
            crane[j].KRASH_cntrl_yn[f, (temp_i + 20)] = true; // ТЕЛЕЖКА 2 - ПОДЪЕМ 2 - груз
                                                              //
            #endregion
            #region ПОДЪЕМ 2 - тросс + крюк + траверса => НЕТ
            f = 8;
            for (i = 0; i < 812; i++)
            {
                crane[j].KRASH_cntrl_yn[f, i] = false;
            }

            #endregion
            #region ПОДЪЕМ 2 - траверса
            f = 9;
            for (i = 0; i < 812; i++)
            {
                crane[j].KRASH_cntrl_yn[f, i] = true;
            }
            crane[j].KRASH_cntrl_yn[f, (temp_i + 1)] = true; // МОСТ
            crane[j].KRASH_cntrl_yn[f, (temp_i + 2)] = true; // ТЕЛЕЖКА 1
                                                             //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 3)] = true; // ТЕЛЕЖКА 1 - ПОДЪЕМ 1 - ТРОСС + КРЮК
            crane[j].KRASH_cntrl_yn[f, (temp_i + 4)] = false; // ТЕЛЕЖКА 1 - ПОДЪЕМ 1 - тросс + крюк + траверса => ВСЕГДА FALSE
            crane[j].KRASH_cntrl_yn[f, (temp_i + 5)] = true; // ТЕЛЕЖКА 1 - ПОДЪЕМ 1 - траверса
            crane[j].KRASH_cntrl_yn[f, (temp_i + 6)] = true; // ТЕЛЕЖКА 1 - ПОДЪЕМ 1 - груз
                                                             //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 7)] = false; // ТЕЛЕЖКА 1 - ПОДЪЕМ 2 - ТРОСС + КРЮК
            crane[j].KRASH_cntrl_yn[f, (temp_i + 8)] = false; // ТЕЛЕЖКА 1 - ПОДЪЕМ 2 - тросс + крюк + траверса => ВСЕГДА FALSE
            crane[j].KRASH_cntrl_yn[f, (temp_i + 9)] = false; // XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX  ТЕЛЕЖКА 1 - ПОДЪЕМ 2 - траверса
            crane[j].KRASH_cntrl_yn[f, (temp_i + 10)] = false; // ТЕЛЕЖКА 1 - ПОДЪЕМ 2 - груз
                                                               //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 11)] = false; // тросс + крюк + траверса + 3 подъем тросс + крюк + траверса => ВСЕГДА FALSE
                                                               //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 12)] = true; // ТЕЛЕЖКА 2
                                                              //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 13)] = true; // ТЕЛЕЖКА 2 - ПОДЪЕМ 1 - ТРОСС + КРЮК
            crane[j].KRASH_cntrl_yn[f, (temp_i + 14)] = false; // ТЕЛЕЖКА 2 - ПОДЪЕМ 1 - тросс + крюк + траверса => ВСЕГДА FALSE
            crane[j].KRASH_cntrl_yn[f, (temp_i + 15)] = true; // ТЕЛЕЖКА 2 - ПОДЪЕМ 1 - траверса
            crane[j].KRASH_cntrl_yn[f, (temp_i + 16)] = true; // ТЕЛЕЖКА 2 - ПОДЪЕМ 1 - груз
                                                              //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 17)] = true; // ТЕЛЕЖКА 2 - ПОДЪЕМ 2 - ТРОСС + КРЮК
            crane[j].KRASH_cntrl_yn[f, (temp_i + 18)] = false; // ТЕЛЕЖКА 2 - ПОДЪЕМ 2 - тросс + крюк + траверса => ВСЕГДА FALSE
            crane[j].KRASH_cntrl_yn[f, (temp_i + 19)] = true; // ТЕЛЕЖКА 2 - ПОДЪЕМ 2 - траверса
            crane[j].KRASH_cntrl_yn[f, (temp_i + 20)] = true; // ТЕЛЕЖКА 2 - ПОДЪЕМ 2 - груз
                                                              //
            #endregion
            #region ПОДЪЕМ 2 - груз
            f = 10;
            for (i = 0; i < 812; i++)
            {
                crane[j].KRASH_cntrl_yn[f, i] = true;
            }
            crane[j].KRASH_cntrl_yn[f, (temp_i + 1)] = true; // МОСТ
            crane[j].KRASH_cntrl_yn[f, (temp_i + 2)] = true; // ТЕЛЕЖКА 1
                                                             //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 3)] = true; // ТЕЛЕЖКА 1 - ПОДЪЕМ 1 - ТРОСС + КРЮК
            crane[j].KRASH_cntrl_yn[f, (temp_i + 4)] = false; // ТЕЛЕЖКА 1 - ПОДЪЕМ 1 - тросс + крюк + траверса => ВСЕГДА FALSE
            crane[j].KRASH_cntrl_yn[f, (temp_i + 5)] = true; // ТЕЛЕЖКА 1 - ПОДЪЕМ 1 - траверса
            crane[j].KRASH_cntrl_yn[f, (temp_i + 6)] = true; // ТЕЛЕЖКА 1 - ПОДЪЕМ 1 - груз
                                                             //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 7)] = false; // ТЕЛЕЖКА 1 - ПОДЪЕМ 2 - ТРОСС + КРЮК
            crane[j].KRASH_cntrl_yn[f, (temp_i + 8)] = false; // ТЕЛЕЖКА 1 - ПОДЪЕМ 2 - тросс + крюк + траверса => ВСЕГДА FALSE
            crane[j].KRASH_cntrl_yn[f, (temp_i + 9)] = false; // ТЕЛЕЖКА 1 - ПОДЪЕМ 2 - траверса
            crane[j].KRASH_cntrl_yn[f, (temp_i + 10)] = false; // XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX  ТЕЛЕЖКА 1 - ПОДЪЕМ 2 - груз
                                                               //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 11)] = false; // тросс + крюк + траверса + 3 подъем тросс + крюк + траверса => ВСЕГДА FALSE
                                                               //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 12)] = true; // ТЕЛЕЖКА 2
                                                              //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 13)] = true; // ТЕЛЕЖКА 2 - ПОДЪЕМ 1 - ТРОСС + КРЮК
            crane[j].KRASH_cntrl_yn[f, (temp_i + 14)] = false; // ТЕЛЕЖКА 2 - ПОДЪЕМ 1 - тросс + крюк + траверса => ВСЕГДА FALSE
            crane[j].KRASH_cntrl_yn[f, (temp_i + 15)] = true; // ТЕЛЕЖКА 2 - ПОДЪЕМ 1 - траверса
            crane[j].KRASH_cntrl_yn[f, (temp_i + 16)] = true; // ТЕЛЕЖКА 2 - ПОДЪЕМ 1 - груз
                                                              //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 17)] = true; // ТЕЛЕЖКА 2 - ПОДЪЕМ 2 - ТРОСС + КРЮК
            crane[j].KRASH_cntrl_yn[f, (temp_i + 18)] = false; // ТЕЛЕЖКА 2 - ПОДЪЕМ 2 - тросс + крюк + траверса => ВСЕГДА FALSE
            crane[j].KRASH_cntrl_yn[f, (temp_i + 19)] = true; // ТЕЛЕЖКА 2 - ПОДЪЕМ 2 - траверса
            crane[j].KRASH_cntrl_yn[f, (temp_i + 20)] = true; // ТЕЛЕЖКА 2 - ПОДЪЕМ 2 - груз
                                                              //
            #endregion
            //
            #region тросс + крюк + траверса + 3 подъем тросс + крюк + траверса => НЕТ
            f = 11;
            for (i = 0; i < 812; i++)
            {
                crane[j].KRASH_cntrl_yn[f, i] = false;
            }

            #endregion
            //
            //
            #region ТЕЛЕЖКА 2       
            f = 12;
            for (i = 0; i < 812; i++)
            {
                crane[j].KRASH_cntrl_yn[f, i] = true;
            }
            crane[j].KRASH_cntrl_yn[f, (temp_i + 1)] = false; // МОСТ
            crane[j].KRASH_cntrl_yn[f, (temp_i + 2)] = false; // ТЕЛЕЖКА 1
                                                              //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 3)] = false; // ТЕЛЕЖКА 1 - ПОДЪЕМ 1 - ТРОСС + КРЮК
            crane[j].KRASH_cntrl_yn[f, (temp_i + 4)] = false; // ТЕЛЕЖКА 1 - ПОДЪЕМ 1 - тросс + крюк + траверса => ВСЕГДА FALSE
            crane[j].KRASH_cntrl_yn[f, (temp_i + 5)] = true; // ТЕЛЕЖКА 1 - ПОДЪЕМ 1 - траверса
            crane[j].KRASH_cntrl_yn[f, (temp_i + 6)] = true; // ТЕЛЕЖКА 1 - ПОДЪЕМ 1 - груз
                                                             //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 7)] = false; // ТЕЛЕЖКА 1 - ПОДЪЕМ 2 - ТРОСС + КРЮК
            crane[j].KRASH_cntrl_yn[f, (temp_i + 8)] = false; // ТЕЛЕЖКА 1 - ПОДЪЕМ 2 - тросс + крюк + траверса => ВСЕГДА FALSE
            crane[j].KRASH_cntrl_yn[f, (temp_i + 9)] = true; // ТЕЛЕЖКА 1 - ПОДЪЕМ 2 - траверса
            crane[j].KRASH_cntrl_yn[f, (temp_i + 10)] = true; // ТЕЛЕЖКА 1 - ПОДЪЕМ 2 - груз
                                                              //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 11)] = false; // тросс + крюк + траверса + 3 подъем тросс + крюк + траверса => ВСЕГДА FALSE
                                                               //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 12)] = false; // XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX ТЕЛЕЖКА 2
                                                               //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 13)] = false; // ТЕЛЕЖКА 2 - ПОДЪЕМ 1 - ТРОСС + КРЮК
            crane[j].KRASH_cntrl_yn[f, (temp_i + 14)] = false; // ТЕЛЕЖКА 2 - ПОДЪЕМ 1 - тросс + крюк + траверса => ВСЕГДА FALSE
            crane[j].KRASH_cntrl_yn[f, (temp_i + 15)] = true; // ТЕЛЕЖКА 2 - ПОДЪЕМ 1 - траверса
            crane[j].KRASH_cntrl_yn[f, (temp_i + 16)] = true; // ТЕЛЕЖКА 2 - ПОДЪЕМ 1 - груз
                                                              //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 17)] = false; // ТЕЛЕЖКА 2 - ПОДЪЕМ 2 - ТРОСС + КРЮК
            crane[j].KRASH_cntrl_yn[f, (temp_i + 18)] = false; // ТЕЛЕЖКА 2 - ПОДЪЕМ 2 - тросс + крюк + траверса => ВСЕГДА FALSE
            crane[j].KRASH_cntrl_yn[f, (temp_i + 19)] = true; // ТЕЛЕЖКА 2 - ПОДЪЕМ 2 - траверса
            crane[j].KRASH_cntrl_yn[f, (temp_i + 20)] = true; // ТЕЛЕЖКА 2 - ПОДЪЕМ 2 - груз
                                                              //       
            #endregion
            //
            #region ПОДЪЕМ 3 - ТРОСС + КРЮК
            f = 13;
            for (i = 0; i < 812; i++)
            {
                crane[j].KRASH_cntrl_yn[f, i] = true;
            }
            crane[j].KRASH_cntrl_yn[f, (temp_i + 1)] = false; // МОСТ
            crane[j].KRASH_cntrl_yn[f, (temp_i + 2)] = false; // ТЕЛЕЖКА 1
                                                              //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 3)] = false; // XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX  ТЕЛЕЖКА 1 - ПОДЪЕМ 1 - ТРОСС + КРЮК
            crane[j].KRASH_cntrl_yn[f, (temp_i + 4)] = false; // ТЕЛЕЖКА 1 - ПОДЪЕМ 1 - тросс + крюк + траверса => ВСЕГДА FALSE
            crane[j].KRASH_cntrl_yn[f, (temp_i + 5)] = true; // ТЕЛЕЖКА 1 - ПОДЪЕМ 1 - траверса
            crane[j].KRASH_cntrl_yn[f, (temp_i + 6)] = true; // ТЕЛЕЖКА 1 - ПОДЪЕМ 1 - груз
                                                              //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 7)] = false; // ТЕЛЕЖКА 1 - ПОДЪЕМ 2 - ТРОСС + КРЮК
            crane[j].KRASH_cntrl_yn[f, (temp_i + 8)] = false; // ТЕЛЕЖКА 1 - ПОДЪЕМ 2 - тросс + крюк + траверса => ВСЕГДА FALSE
            crane[j].KRASH_cntrl_yn[f, (temp_i + 9)] = true; // ТЕЛЕЖКА 1 - ПОДЪЕМ 2 - траверса
            crane[j].KRASH_cntrl_yn[f, (temp_i + 10)] = true; // ТЕЛЕЖКА 1 - ПОДЪЕМ 2 - груз
                                                              //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 11)] = false; // тросс + крюк + траверса + 3 подъем тросс + крюк + траверса => ВСЕГДА FALSE
                                                               //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 12)] = false; // ТЕЛЕЖКА 2
                                                               //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 13)] = false; // ТЕЛЕЖКА 2 - ПОДЪЕМ 1 - ТРОСС + КРЮК
            crane[j].KRASH_cntrl_yn[f, (temp_i + 14)] = false; // ТЕЛЕЖКА 2 - ПОДЪЕМ 1 - тросс + крюк + траверса => ВСЕГДА FALSE
            crane[j].KRASH_cntrl_yn[f, (temp_i + 15)] = false; // ТЕЛЕЖКА 2 - ПОДЪЕМ 1 - траверса
            crane[j].KRASH_cntrl_yn[f, (temp_i + 16)] = false; // ТЕЛЕЖКА 2 - ПОДЪЕМ 1 - груз
                                                              //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 17)] = false; // ТЕЛЕЖКА 2 - ПОДЪЕМ 2 - ТРОСС + КРЮК
            crane[j].KRASH_cntrl_yn[f, (temp_i + 18)] = false; // ТЕЛЕЖКА 2 - ПОДЪЕМ 2 - тросс + крюк + траверса => ВСЕГДА FALSE
            crane[j].KRASH_cntrl_yn[f, (temp_i + 19)] = true; // ТЕЛЕЖКА 2 - ПОДЪЕМ 2 - траверса
            crane[j].KRASH_cntrl_yn[f, (temp_i + 20)] = true; // ТЕЛЕЖКА 2 - ПОДЪЕМ 2 - груз
                                                              //
            #endregion
            #region ПОДЪЕМ 3 - тросс + крюк + траверса => НЕТ
            f = 14;
            for (i = 0; i < 812; i++)
            {
                crane[j].KRASH_cntrl_yn[f, i] = false;
            }

            #endregion
            #region ПОДЪЕМ 3 - траверса
            f = 15;
            for (i = 0; i < 812; i++)
            {
                crane[j].KRASH_cntrl_yn[f, i] = true;
            }
            crane[j].KRASH_cntrl_yn[f, (temp_i + 1)] = true; // МОСТ
            crane[j].KRASH_cntrl_yn[f, (temp_i + 2)] = true; // ТЕЛЕЖКА 1
                                                             //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 3)] = false; // ТЕЛЕЖКА 1 - ПОДЪЕМ 1 - ТРОСС + КРЮК
            crane[j].KRASH_cntrl_yn[f, (temp_i + 4)] = false; // ТЕЛЕЖКА 1 - ПОДЪЕМ 1 - тросс + крюк + траверса => ВСЕГДА FALSE
            crane[j].KRASH_cntrl_yn[f, (temp_i + 5)] = true; // XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX  ТЕЛЕЖКА 1 - ПОДЪЕМ 1 - траверса
            crane[j].KRASH_cntrl_yn[f, (temp_i + 6)] = true; // ТЕЛЕЖКА 1 - ПОДЪЕМ 1 - груз
                                                              //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 7)] = true; // ТЕЛЕЖКА 1 - ПОДЪЕМ 2 - ТРОСС + КРЮК
            crane[j].KRASH_cntrl_yn[f, (temp_i + 8)] = false; // ТЕЛЕЖКА 1 - ПОДЪЕМ 2 - тросс + крюк + траверса => ВСЕГДА FALSE
            crane[j].KRASH_cntrl_yn[f, (temp_i + 9)] = true; // ТЕЛЕЖКА 1 - ПОДЪЕМ 2 - траверса
            crane[j].KRASH_cntrl_yn[f, (temp_i + 10)] = true; // ТЕЛЕЖКА 1 - ПОДЪЕМ 2 - груз
                                                              //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 11)] = false; // тросс + крюк + траверса + 3 подъем тросс + крюк + траверса => ВСЕГДА FALSE
                                                               //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 12)] = true; // ТЕЛЕЖКА 2
                                                              //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 13)] = false; // ТЕЛЕЖКА 2 - ПОДЪЕМ 1 - ТРОСС + КРЮК
            crane[j].KRASH_cntrl_yn[f, (temp_i + 14)] = false; // ТЕЛЕЖКА 2 - ПОДЪЕМ 1 - тросс + крюк + траверса => ВСЕГДА FALSE
            crane[j].KRASH_cntrl_yn[f, (temp_i + 15)] = false; // ТЕЛЕЖКА 2 - ПОДЪЕМ 1 - траверса
            crane[j].KRASH_cntrl_yn[f, (temp_i + 16)] = false; // ТЕЛЕЖКА 2 - ПОДЪЕМ 1 - груз
                                                              //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 17)] = true; // ТЕЛЕЖКА 2 - ПОДЪЕМ 2 - ТРОСС + КРЮК
            crane[j].KRASH_cntrl_yn[f, (temp_i + 18)] = false; // ТЕЛЕЖКА 2 - ПОДЪЕМ 2 - тросс + крюк + траверса => ВСЕГДА FALSE
            crane[j].KRASH_cntrl_yn[f, (temp_i + 19)] = true; // ТЕЛЕЖКА 2 - ПОДЪЕМ 2 - траверса
            crane[j].KRASH_cntrl_yn[f, (temp_i + 20)] = true; // ТЕЛЕЖКА 2 - ПОДЪЕМ 2 - груз
                                                              //
            #endregion
            #region ПОДЪЕМ 3 - груз
            f = 16;
            for (i = 0; i < 812; i++)
            {
                crane[j].KRASH_cntrl_yn[f, i] = true;
            }
            crane[j].KRASH_cntrl_yn[f, (temp_i + 1)] = true; // МОСТ
            crane[j].KRASH_cntrl_yn[f, (temp_i + 2)] = true; // ТЕЛЕЖКА 1
                                                             //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 3)] = true; // ТЕЛЕЖКА 1 - ПОДЪЕМ 1 - ТРОСС + КРЮК
            crane[j].KRASH_cntrl_yn[f, (temp_i + 4)] = false; // ТЕЛЕЖКА 1 - ПОДЪЕМ 1 - тросс + крюк + траверса => ВСЕГДА FALSE
            crane[j].KRASH_cntrl_yn[f, (temp_i + 5)] = true; // ТЕЛЕЖКА 1 - ПОДЪЕМ 1 - траверса
            crane[j].KRASH_cntrl_yn[f, (temp_i + 6)] = true; // XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX  ТЕЛЕЖКА 1 - ПОДЪЕМ 1 - груз
                                                              //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 7)] = true; // ТЕЛЕЖКА 1 - ПОДЪЕМ 2 - ТРОСС + КРЮК
            crane[j].KRASH_cntrl_yn[f, (temp_i + 8)] = false; // ТЕЛЕЖКА 1 - ПОДЪЕМ 2 - тросс + крюк + траверса => ВСЕГДА FALSE
            crane[j].KRASH_cntrl_yn[f, (temp_i + 9)] = true; // ТЕЛЕЖКА 1 - ПОДЪЕМ 2 - траверса
            crane[j].KRASH_cntrl_yn[f, (temp_i + 10)] = true; // ТЕЛЕЖКА 1 - ПОДЪЕМ 2 - груз
                                                              //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 11)] = false; // тросс + крюк + траверса + 3 подъем тросс + крюк + траверса => ВСЕГДА FALSE
                                                               //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 12)] = true; // ТЕЛЕЖКА 2
                                                              //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 13)] = false; // ТЕЛЕЖКА 2 - ПОДЪЕМ 1 - ТРОСС + КРЮК
            crane[j].KRASH_cntrl_yn[f, (temp_i + 14)] = false; // ТЕЛЕЖКА 2 - ПОДЪЕМ 1 - тросс + крюк + траверса => ВСЕГДА FALSE
            crane[j].KRASH_cntrl_yn[f, (temp_i + 15)] = false; // ТЕЛЕЖКА 2 - ПОДЪЕМ 1 - траверса
            crane[j].KRASH_cntrl_yn[f, (temp_i + 16)] = false; // ТЕЛЕЖКА 2 - ПОДЪЕМ 1 - груз
                                                              //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 17)] = true; // ТЕЛЕЖКА 2 - ПОДЪЕМ 2 - ТРОСС + КРЮК
            crane[j].KRASH_cntrl_yn[f, (temp_i + 18)] = false; // ТЕЛЕЖКА 2 - ПОДЪЕМ 2 - тросс + крюк + траверса => ВСЕГДА FALSE
            crane[j].KRASH_cntrl_yn[f, (temp_i + 19)] = true; // ТЕЛЕЖКА 2 - ПОДЪЕМ 2 - траверса
            crane[j].KRASH_cntrl_yn[f, (temp_i + 20)] = true; // ТЕЛЕЖКА 2 - ПОДЪЕМ 2 - груз
                                                              //
            #endregion
            //
            //
            #region ПОДЪЕМ 4 - ТРОСС + КРЮК
            f = 17;
            for (i = 0; i < 812; i++)
            {
                crane[j].KRASH_cntrl_yn[f, i] = true;
            }
            crane[j].KRASH_cntrl_yn[f, (temp_i + 1)] = false; // МОСТ
            crane[j].KRASH_cntrl_yn[f, (temp_i + 2)] = false; // ТЕЛЕЖКА 1
                                                              //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 3)] = true; // ТЕЛЕЖКА 1 - ПОДЪЕМ 1 - ТРОСС + КРЮК
            crane[j].KRASH_cntrl_yn[f, (temp_i + 4)] = false; // ТЕЛЕЖКА 1 - ПОДЪЕМ 1 - тросс + крюк + траверса => ВСЕГДА FALSE
            crane[j].KRASH_cntrl_yn[f, (temp_i + 5)] = true; // ТЕЛЕЖКА 1 - ПОДЪЕМ 1 - траверса
            crane[j].KRASH_cntrl_yn[f, (temp_i + 6)] = true; // ТЕЛЕЖКА 1 - ПОДЪЕМ 1 - груз
                                                             //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 7)] = true; // XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX  ТЕЛЕЖКА 1 - ПОДЪЕМ 2 - ТРОСС + КРЮК
            crane[j].KRASH_cntrl_yn[f, (temp_i + 8)] = false; // ТЕЛЕЖКА 1 - ПОДЪЕМ 2 - тросс + крюк + траверса => ВСЕГДА FALSE
            crane[j].KRASH_cntrl_yn[f, (temp_i + 9)] = true; // ТЕЛЕЖКА 1 - ПОДЪЕМ 2 - траверса
            crane[j].KRASH_cntrl_yn[f, (temp_i + 10)] = true; // ТЕЛЕЖКА 1 - ПОДЪЕМ 2 - груз
                                                               //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 11)] = false; // тросс + крюк + траверса + 3 подъем тросс + крюк + траверса => ВСЕГДА FALSE
                                                               //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 12)] = false; // ТЕЛЕЖКА 2
                                                               //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 13)] = false; // ТЕЛЕЖКА 2 - ПОДЪЕМ 1 - ТРОСС + КРЮК
            crane[j].KRASH_cntrl_yn[f, (temp_i + 14)] = false; // ТЕЛЕЖКА 2 - ПОДЪЕМ 1 - тросс + крюк + траверса => ВСЕГДА FALSE
            crane[j].KRASH_cntrl_yn[f, (temp_i + 15)] = true; // ТЕЛЕЖКА 2 - ПОДЪЕМ 1 - траверса
            crane[j].KRASH_cntrl_yn[f, (temp_i + 16)] = true; // ТЕЛЕЖКА 2 - ПОДЪЕМ 1 - груз
                                                              //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 17)] = false; // ТЕЛЕЖКА 2 - ПОДЪЕМ 2 - ТРОСС + КРЮК
            crane[j].KRASH_cntrl_yn[f, (temp_i + 18)] = false; // ТЕЛЕЖКА 2 - ПОДЪЕМ 2 - тросс + крюк + траверса => ВСЕГДА FALSE
            crane[j].KRASH_cntrl_yn[f, (temp_i + 19)] = false; // ТЕЛЕЖКА 2 - ПОДЪЕМ 2 - траверса
            crane[j].KRASH_cntrl_yn[f, (temp_i + 20)] = false; // ТЕЛЕЖКА 2 - ПОДЪЕМ 2 - груз
                                                              //
            #endregion
            #region ПОДЪЕМ 4 - тросс + крюк + траверса => НЕТ
            f = 18;
            for (i = 0; i < 812; i++)
            {
                crane[j].KRASH_cntrl_yn[f, i] = false;
            }

            #endregion
            #region ПОДЪЕМ 4 - траверса
            f = 19;
            for (i = 0; i < 812; i++)
            {
                crane[j].KRASH_cntrl_yn[f, i] = true;
            }
            crane[j].KRASH_cntrl_yn[f, (temp_i + 1)] = true; // МОСТ
            crane[j].KRASH_cntrl_yn[f, (temp_i + 2)] = true; // ТЕЛЕЖКА 1
                                                             //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 3)] = true; // ТЕЛЕЖКА 1 - ПОДЪЕМ 1 - ТРОСС + КРЮК
            crane[j].KRASH_cntrl_yn[f, (temp_i + 4)] = false; // ТЕЛЕЖКА 1 - ПОДЪЕМ 1 - тросс + крюк + траверса => ВСЕГДА FALSE
            crane[j].KRASH_cntrl_yn[f, (temp_i + 5)] = true; // ТЕЛЕЖКА 1 - ПОДЪЕМ 1 - траверса
            crane[j].KRASH_cntrl_yn[f, (temp_i + 6)] = true; // ТЕЛЕЖКА 1 - ПОДЪЕМ 1 - груз
                                                             //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 7)] = true; // ТЕЛЕЖКА 1 - ПОДЪЕМ 2 - ТРОСС + КРЮК
            crane[j].KRASH_cntrl_yn[f, (temp_i + 8)] = false; // ТЕЛЕЖКА 1 - ПОДЪЕМ 2 - тросс + крюк + траверса => ВСЕГДА FALSE
            crane[j].KRASH_cntrl_yn[f, (temp_i + 9)] = true; // XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX  ТЕЛЕЖКА 1 - ПОДЪЕМ 2 - траверса
            crane[j].KRASH_cntrl_yn[f, (temp_i + 10)] = true; // ТЕЛЕЖКА 1 - ПОДЪЕМ 2 - груз
                                                               //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 11)] = false; // тросс + крюк + траверса + 3 подъем тросс + крюк + траверса => ВСЕГДА FALSE
                                                               //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 12)] = true; // ТЕЛЕЖКА 2
                                                              //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 13)] = true; // ТЕЛЕЖКА 2 - ПОДЪЕМ 1 - ТРОСС + КРЮК
            crane[j].KRASH_cntrl_yn[f, (temp_i + 14)] = false; // ТЕЛЕЖКА 2 - ПОДЪЕМ 1 - тросс + крюк + траверса => ВСЕГДА FALSE
            crane[j].KRASH_cntrl_yn[f, (temp_i + 15)] = true; // ТЕЛЕЖКА 2 - ПОДЪЕМ 1 - траверса
            crane[j].KRASH_cntrl_yn[f, (temp_i + 16)] = true; // ТЕЛЕЖКА 2 - ПОДЪЕМ 1 - груз
                                                              //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 17)] = false; // ТЕЛЕЖКА 2 - ПОДЪЕМ 2 - ТРОСС + КРЮК
            crane[j].KRASH_cntrl_yn[f, (temp_i + 18)] = false; // ТЕЛЕЖКА 2 - ПОДЪЕМ 2 - тросс + крюк + траверса => ВСЕГДА FALSE
            crane[j].KRASH_cntrl_yn[f, (temp_i + 19)] = false; // ТЕЛЕЖКА 2 - ПОДЪЕМ 2 - траверса
            crane[j].KRASH_cntrl_yn[f, (temp_i + 20)] = false; // ТЕЛЕЖКА 2 - ПОДЪЕМ 2 - груз
                                                              //
            #endregion
            #region ПОДЪЕМ 4 - груз
            f = 20;
            for (i = 0; i < 812; i++)
            {
                crane[j].KRASH_cntrl_yn[f, i] = true;
            }
            crane[j].KRASH_cntrl_yn[f, (temp_i + 1)] = true; // МОСТ
            crane[j].KRASH_cntrl_yn[f, (temp_i + 2)] = true; // ТЕЛЕЖКА 1
                                                             //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 3)] = true; // ТЕЛЕЖКА 1 - ПОДЪЕМ 1 - ТРОСС + КРЮК
            crane[j].KRASH_cntrl_yn[f, (temp_i + 4)] = false; // ТЕЛЕЖКА 1 - ПОДЪЕМ 1 - тросс + крюк + траверса => ВСЕГДА FALSE
            crane[j].KRASH_cntrl_yn[f, (temp_i + 5)] = true; // ТЕЛЕЖКА 1 - ПОДЪЕМ 1 - траверса
            crane[j].KRASH_cntrl_yn[f, (temp_i + 6)] = true; // ТЕЛЕЖКА 1 - ПОДЪЕМ 1 - груз
                                                             //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 7)] = true; // ТЕЛЕЖКА 1 - ПОДЪЕМ 2 - ТРОСС + КРЮК
            crane[j].KRASH_cntrl_yn[f, (temp_i + 8)] = false; // ТЕЛЕЖКА 1 - ПОДЪЕМ 2 - тросс + крюк + траверса => ВСЕГДА FALSE
            crane[j].KRASH_cntrl_yn[f, (temp_i + 9)] = true; // ТЕЛЕЖКА 1 - ПОДЪЕМ 2 - траверса
            crane[j].KRASH_cntrl_yn[f, (temp_i + 10)] = true; // XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX  ТЕЛЕЖКА 1 - ПОДЪЕМ 2 - груз
                                                               //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 11)] = false; // тросс + крюк + траверса + 3 подъем тросс + крюк + траверса => ВСЕГДА FALSE
                                                               //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 12)] = true; // ТЕЛЕЖКА 2
                                                              //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 13)] = true; // ТЕЛЕЖКА 2 - ПОДЪЕМ 1 - ТРОСС + КРЮК
            crane[j].KRASH_cntrl_yn[f, (temp_i + 14)] = false; // ТЕЛЕЖКА 2 - ПОДЪЕМ 1 - тросс + крюк + траверса => ВСЕГДА FALSE
            crane[j].KRASH_cntrl_yn[f, (temp_i + 15)] = true; // ТЕЛЕЖКА 2 - ПОДЪЕМ 1 - траверса
            crane[j].KRASH_cntrl_yn[f, (temp_i + 16)] = true; // ТЕЛЕЖКА 2 - ПОДЪЕМ 1 - груз
                                                              //
            crane[j].KRASH_cntrl_yn[f, (temp_i + 17)] = false; // ТЕЛЕЖКА 2 - ПОДЪЕМ 2 - ТРОСС + КРЮК
            crane[j].KRASH_cntrl_yn[f, (temp_i + 18)] = false; // ТЕЛЕЖКА 2 - ПОДЪЕМ 2 - тросс + крюк + траверса => ВСЕГДА FALSE
            crane[j].KRASH_cntrl_yn[f, (temp_i + 19)] = false; // ТЕЛЕЖКА 2 - ПОДЪЕМ 2 - траверса
            crane[j].KRASH_cntrl_yn[f, (temp_i + 20)] = false; // ТЕЛЕЖКА 2 - ПОДЪЕМ 2 - груз
                                                              //
            #endregion
            //
            
        }
        #endregion
        // ======================================================================================   
    }
    // Update is called once per frame
    void Update()
    {

        // ------------------------------------------------------------------------------------------------------------------------------------------------------------
        // NNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNN
        // FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
        // --- START READ ---------------------------------------------------------------------------------------------------------------------------------------------
        // ВХОДНЫЕ ДАННЫЕ - ОТ НИКИТЫ Ф.
        // ВМЕСТО НУЛЕВЫХ ЗНАЧЕНИЙ НЕОБХОДИМО ПРИВЯЗАТЬ К АКТУАЛЬНЫМ ПЕРЕМЕННЫМ, ЗНАЧЕНИЕ КОТОРЫХ СООТВЕТСТВУЕТ ОПИСАНИЮ ФУНКЦИОНАЛА
        // ЕСЛИ СТОИТ " = 0; // NO READ " , ТО ДАННАЯ ПЕРЕМЕННАЯ НЕ ТРЕБУЕТ ПРИВЯЗКИ 
        // ------------------------------------------------------------------------------------------------------------------------------------------------------------
        #region ПР00x - ЧТЕНИЕ ВХОДНЫХ ДАННЫХ ДЛЯ ПОСЛЕДУЮЩЕЙ ОБРАБОТКИ
        if (craneScripts.Count != 3)
        {
            // Кранов не 3, смысла выполнять расчеты нет
            return;
        }
        foreach (var craneScript in craneScripts)
        {
            var craneConnectionScript = craneScript.gameObject.GetComponent<CraneConnection>();
            //не удается найти скрипт крана, выходим
           
            if (craneConnectionScript == null)
                return;
            var j = craneScripts.FindIndex(c=>c==craneScript) + 1;

            var status = craneConnectionScript.status;
            var connected = craneConnectionScript.connected;

            #region READ_DATA - КРАН
            // =============================================================================================
            // АКТУАЛЬНЫЕ КООРДИНАТЫ КРАНА - реальные s7 - ПЛК ИЛИ ВИРТУАЛЬНЫЕ С РЕЖИМА ЭМУЛЯЦИИ
            crane[j].BRIDGE_X = connected ? status.dimensions.bridgePosition : craneScript.bridgePosition;
            crane[j].trolley_1_Y = connected ? status.dimensions.mtPosition : craneScript.mtPosition; // ЖЕЛАТЕЛЬНО ИЗМЕНИТЬ НАПРАВЛЕНИЕ СЧЕТА ОТ ТЕКУЩЕГО ВАРИАНТА
            crane[j].trolley_2_Y = connected ? status.dimensions.atPosition : craneScript.atPosition; // ЖЕЛАТЕЛЬНО ИЗМЕНИТЬ НАПРАВЛЕНИЕ СЧЕТА ОТ ТЕКУЩЕГО ВАРИАНТА
                                                                                                      //crane[j].HOIST_1_1_Z = // connected ? status.dimensions.mtMhPosition : craneScript.hoistsMaxHeight - craneScript.mtMhPosition;
                                                                                                      //crane[j].HOIST_1_2_Z = // connected ? status.dimensions.mtAhPosition : craneScript.mtAhPosition; // craneScript.hoistsMaxHeight -
                                                                                                      //crane[j].HOIST_2_1_Z = // connected ? status.dimensions.atMhPosition : craneScript.hoistsMaxHeight - craneScript.atMhPosition;
                                                                                                      // crane[j].HOIST_2_2_Z = // connected ? status.dimensions.atAhPosition : craneScript.hoistsMaxHeight - craneScript.atAhPosition;

            float temp_HOIST_1_1_Z = 0;
            float temp_HOIST_1_2_Z = 0;
            float temp_HOIST_2_1_Z = 0;
            float temp_HOIST_2_2_Z = 0;

            temp_HOIST_1_1_Z = connected ? status.dimensions.mtMhPosition : craneScript.mtMhPosition;
            temp_HOIST_1_2_Z = connected ? status.dimensions.mtAhPosition : craneScript.mtAhPosition; // connected ? status.dimensions.mtAhPosition : craneScript.mtAhPosition; // craneScript.hoistsMaxHeight -
            temp_HOIST_2_1_Z = connected ? status.dimensions.mtAhPosition : craneScript.atMhPosition; // connected ? status.dimensions.atMhPosition : craneScript.hoistsMaxHeight - craneScript.atMhPosition;
            temp_HOIST_2_2_Z = connected ? status.dimensions.mtAhPosition : craneScript.atAhPosition; // connected ? status.dimensions.atAhPosition : craneScript.hoistsMaxHeight - craneScript.atAhPosition;

            if (j == 1)
            {
                crane[j].HOIST_1_1_Z =  temp_HOIST_1_1_Z; //38000f + 4480f + 
                crane[j].HOIST_1_2_Z =  temp_HOIST_1_2_Z; //38000f + 4480f + 
                crane[j].HOIST_2_1_Z =  temp_HOIST_2_1_Z; //38000f + 2410f +
                crane[j].HOIST_2_2_Z =  temp_HOIST_2_2_Z; //38000f + 2410f +
            }
            if (j == 2)
            {
                crane[j].HOIST_1_1_Z =  temp_HOIST_1_1_Z; //26200f + 4764f + 
                crane[j].HOIST_1_2_Z =  temp_HOIST_1_2_Z; //26200f + 4764f + 
                crane[j].HOIST_2_1_Z =  temp_HOIST_2_1_Z; //26200f + 4764f +
                crane[j].HOIST_2_2_Z =  temp_HOIST_2_2_Z; //26200f + 1761f +
            }
            if (j == 3)
            {
                crane[j].HOIST_1_1_Z =  temp_HOIST_1_1_Z; //26200f + 4764f +
                crane[j].HOIST_1_2_Z =  temp_HOIST_1_2_Z;
                crane[j].HOIST_2_1_Z =  temp_HOIST_2_1_Z;
                crane[j].HOIST_2_2_Z =  temp_HOIST_2_2_Z;
            }



            // =============================================================================================

            #region Чего нет, того нет





            #endregion

            // НАСТРОЕЧНЫЕ ПАРАМЕТРЫ КРАНА (ПРЕДЛАГАЮ СДЕЛАТЬ ВОЗМОЖНОСТЬ ВВОДА С ИНТЕРФЕЙСНОГО ЭКРАНА!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! - дизайн картинки с меня)

            // ТРАВЕРСА
            // после Вашей обработки - данные из таблицы траверс и информации из ПЛК
            TraverseStruct? traverse = null;
           

            if (craneConnectionScript.traversesCollection.ContainsKey(status.dimensions.mtMhTraverse))
            {
                traverse = craneConnectionScript.traversesCollection[status.dimensions.mtMhTraverse];
           
            }
            //traversesCollection_ald = craneConnectionScript.traversesCollection;
            



            // ГАБАРИТЫ
            crane[j].static_H11_TR_X = traverse != null ? traverse.Value.length : 0;
            crane[j].static_H11_TR_Y = traverse != null ? traverse.Value.width : 0;
            crane[j].static_H11_TR_Z = traverse != null ? traverse.Value.height : 0;
            // на 1 или 2(true) крюка 
            crane[j].static_H11_TR_twohook_y_n = traverse != null ? traverse.Value.twoHooks : false;
            // есть (true)  или нет траверсы
            crane[j].H11_TR_y_n = traverse != null;

            // ТОЧКА ОТСЧЕТА 
            crane[j].STP_static_H11_TR_X = 0; // NO READ
            crane[j].STP_static_H11_TR_Y = 0; // NO READ
            crane[j].STP_static_H11_TR_Z = 0; // NO READ
                                              // ГАБАРИТЫ ПОДЪЕМ 1-1 - ГРУЗ 
            crane[j].static_H_1_1_LOAD_X = status.dimensions.mtMhWeightDimensions.x;
            crane[j].static_H_1_1_LOAD_Y = status.dimensions.mtMhWeightDimensions.y;
            crane[j].static_H_1_1_LOAD_Z = status.dimensions.mtMhWeightDimensions.z;
            // ТОЧКА ОТСЧЕТА
            crane[j].STP_static_H_1_1_LOAD_X = 0; // NO READ
            crane[j].STP_static_H_1_1_LOAD_Y = 0; // NO READ
            crane[j].STP_static_H_1_1_LOAD_Z = 0; // NO READ
            // =============================================================================================

            
            // ТРАВЕРСА
            // после Вашей обработки - данные из таблицы траверс и информации из ПЛК
            traverse = craneConnectionScript.traversesCollection.ContainsKey(status.dimensions.mtAhTraverse) ? craneConnectionScript.traversesCollection[status.dimensions.mtAhTraverse] : (TraverseStruct?)null;
            // ГАБАРИТЫ
            crane[j].static_H12_TR_X = traverse != null ? traverse.Value.length : 0;
            crane[j].static_H12_TR_Y = traverse != null ? traverse.Value.width : 0;
            crane[j].static_H12_TR_Z = traverse != null ? traverse.Value.height : 0;
            // на 1 или 2(true) крюка
            crane[j].static_H12_TR_twohook_y_n = traverse != null ? traverse.Value.twoHooks : false;
            // есть (true)  или нет траверсы
            crane[j].H12_TR_y_n = traverse != null;
            // ТОЧКА ОТСЧЕТА 
            crane[j].STP_static_H12_TR_X = 0; // NO READ
            crane[j].STP_static_H12_TR_Y = 0; // NO READ
            crane[j].STP_static_H12_TR_Z = 0; // NO READ
                                              // ГАБАРИТЫ ПОДЪЕМ 1-2 - ГРУЗ 
            crane[j].static_H_1_2_LOAD_X = status.dimensions.mtAhWeightDimensions.x;
            crane[j].static_H_1_2_LOAD_Y = status.dimensions.mtAhWeightDimensions.y;
            crane[j].static_H_1_2_LOAD_Z = status.dimensions.mtAhWeightDimensions.z;
            // ТОЧКА ОТСЧЕТА
            crane[j].STP_static_H_1_2_LOAD_X = 0; // NO READ
            crane[j].STP_static_H_1_2_LOAD_Y = 0; // NO READ
            crane[j].STP_static_H_1_2_LOAD_Z = 0; // NO READ
            // =============================================================================================

            
            // ТРАВЕРСА
            // после Вашей обработки - данные из таблицы траверс и информации из ПЛК
            traverse = craneConnectionScript.traversesCollection.ContainsKey(status.dimensions.atMhTraverse) ? craneConnectionScript.traversesCollection[status.dimensions.atMhTraverse] : (TraverseStruct?)null;
            // ГАБАРИТЫ
            crane[j].static_H21_TR_X = traverse != null ? traverse.Value.length : 0;
            crane[j].static_H21_TR_Y = traverse != null ? traverse.Value.width : 0;
            crane[j].static_H21_TR_Z = traverse != null ? traverse.Value.height : 0;
            // на 1 или 2(true) крюка
            crane[j].static_H21_TR_twohook_y_n = traverse != null ? traverse.Value.twoHooks : false;
            // есть (true)  или нет траверсы
            crane[j].H21_TR_y_n = traverse != null;
            // ТОЧКА ОТСЧЕТА
            crane[j].STP_static_H21_TR_X = 0;  // NO READ
            crane[j].STP_static_H21_TR_Y = 0;  // NO READ
            crane[j].STP_static_H21_TR_Z = 0;  // NO READ
                                               // ГАБАРИТЫ ПОДЪЕМ 2-1 - ГРУЗ 
            crane[j].static_H_2_1_LOAD_X = status.dimensions.atMhWeightDimensions.x;
            crane[j].static_H_2_1_LOAD_Y = status.dimensions.atMhWeightDimensions.y;
            crane[j].static_H_2_1_LOAD_Z = status.dimensions.atMhWeightDimensions.z;
            // ТОЧКА ОТСЧЕТА
            crane[j].STP_static_H_2_1_LOAD_X = 0; // NO READ
            crane[j].STP_static_H_2_1_LOAD_Y = 0; // NO READ
            crane[j].STP_static_H_2_1_LOAD_Z = 0; // NO READ
                                                  // =============================================================================================

            
            // ТРАВЕРСА
            // после Вашей обработки - данные из таблицы траверс и информации из ПЛК
            traverse = craneConnectionScript.traversesCollection.ContainsKey(status.dimensions.atAhTraverse) ? craneConnectionScript.traversesCollection[status.dimensions.atAhTraverse] : (TraverseStruct?)null;
            // ГАБАРИТЫ
            crane[j].static_H22_TR_X = traverse != null ? traverse.Value.length : 0;
            crane[j].static_H22_TR_Y = traverse != null ? traverse.Value.width : 0;
            crane[j].static_H22_TR_Z = traverse != null ? traverse.Value.height : 0;
            // на 1 или 2(true) крюка
            crane[j].static_H22_TR_twohook_y_n = traverse != null ? traverse.Value.twoHooks : false;
            // есть (true)  или нет траверсы
            crane[j].H22_TR_y_n = false;
            // ТОЧКА ОТСЧЕТА
            crane[j].STP_static_H22_TR_X = 0;  // NO READ
            crane[j].STP_static_H22_TR_Y = 0;  // NO READ
            crane[j].STP_static_H22_TR_Z = 0;  // NO READ
                                               // ГАБАРИТЫ ПОДЪЕМ 2-2 - ГРУЗ 
            crane[j].static_H_2_2_LOAD_X = status.dimensions.atAhWeightDimensions.x;
            crane[j].static_H_2_2_LOAD_Y = status.dimensions.atAhWeightDimensions.y;
            crane[j].static_H_2_2_LOAD_Z = status.dimensions.atAhWeightDimensions.z;
            // ТОЧКА ОТСЧЕТА
            crane[j].STP_static_H_2_2_LOAD_X = 0;  // NO READ
            crane[j].STP_static_H_2_2_LOAD_Y = 0;  // NO READ
            crane[j].STP_static_H_2_2_LOAD_Z = 0;  // NO READ
            // =============================================================================================


            // =============================================================================================
            // РЕЖИМ РАБОТЫ ПОДЪЕМОВ - необходимо записать число!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            // 0 or default - подъемы независимы
            // 1 - траверс и груза нет
            // 2 - 1 + 2 вместе
            // 4 - 1 + 2 + 3 вместе
            // =============================================================================================
            crane[j].regim_w_hoist = status.statuses.mode;
            // =============================================================================================


            // =============================================================================================
            #region НАСТРОЕЧНЫЕ ПАРАМЕТРЫ ДЕЛЬТЫ ПРЕДУПРЕЖДЕНИЯ И ДЕЛЬТЫ АВАРИИ ЭЛЕМЕНТОВ КРАНА
            // НАСТРОЕЧНЫЕ ПАРАМЕТРЫ КРАНА (ПРЕДЛАГАЮ СДЕЛАТЬ ВОЗМОЖНОСТЬ ВВОДА С ИНТЕРФЕЙСНОГО ЭКРАНА!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!)
            // ДЕЛЬТА ПРЕДУПРЕЖДЕНИЯ
            crane[j].D_UP_WARN = Enumerable.Repeat(Globals.warningDistance, 25).ToArray();
            crane[j].D_DOWN_WARN = Enumerable.Repeat(Globals.warningDistance, 25).ToArray();
            crane[j].D_R_WARN = Enumerable.Repeat(Globals.warningDistance, 25).ToArray();
            crane[j].D_L_WARN = Enumerable.Repeat(Globals.warningDistance, 25).ToArray();
            crane[j].D_F_WARN = Enumerable.Repeat(Globals.warningDistance, 25).ToArray();
            crane[j].D_B_WARN = Enumerable.Repeat(Globals.warningDistance, 25).ToArray();
            // ДЕЛЬТА АВАРИИ
            crane[j].D_UP_ERR = Enumerable.Repeat(Globals.alarmDistance, 25).ToArray();
            crane[j].D_DOWN_ERR = Enumerable.Repeat(Globals.alarmDistance, 25).ToArray();
            crane[j].D_R_ERR = Enumerable.Repeat(Globals.alarmDistance, 25).ToArray();
            crane[j].D_L_ERR = Enumerable.Repeat(Globals.alarmDistance, 25).ToArray();
            crane[j].D_F_ERR = Enumerable.Repeat(Globals.alarmDistance, 25).ToArray();
            crane[j].D_B_ERR = Enumerable.Repeat(Globals.alarmDistance, 25).ToArray();
            #endregion
            // =============================================================================================
            #endregion
        }
        #region READ_DATA - ПРЕПЯТСТВИЯ И КОЛОННЫ - ОБЪЕКТЫ - СТАТИЧЕСКИЕ ОБЪЕКТЫ В ЦЕХЕ / ПРЕПЯТСТВТИЯ
        // НОМЕР условно статического объекта - i

        // 1-20 - КРАН 1 - 320Т
        // 0/21-30 - КРАН 1 - 320Т - РЕЗЕРВ



        // 101-120 - КРАН 2 - 120Т
        // 100/121-150 - КРАН 2 - 120Т - РЕЗЕРВ
        // 151-199 - ТОЛЬКО СТАТИЧНЫЕ ОБЪЕКТЫ - РЕЗЕРВ
        // 201-220 - КРАН 3 - 25Т
        // 200/221-250 - КРАН 3 - 25Т - РЕЗЕРВ
        // 251-299 - ТОЛЬКО СТАТИЧНЫЕ ОБЪЕКТЫ - РЕЗЕРВ

        // READ DATA - 131- 199 - ТОЛЬКО СТАТИЧНЫЕ ОБЪЕКТЫ - СТЕНЫ НЕВИДИМЫЕ
        // READ DATA - 31- 99 - ТОЛЬКО СТАТИЧНЫЕ ОБЪЕКТЫ - КОЛОННЫ И СТЕНЫ
        // READ DATA - 301 - 811 - ТОЛЬКО СТАТИЧНЫЕ ОБЪЕКТЫ - ПРЕПЯТСТВИЯ

        //если не подцеплен скрипт с препятствиями - выходим
        if (obstaclesScript == null)
            return;
        #region ЧТЕНИЕ ПРЕПЯТСТВИЙ
                
        // Берём из массива препятствий только 811 отнять 301 штук, чтобы не выйти на границы диапазона
        var obstacles = obstaclesScript.GetObstaclesObject().Take(811 - 301);

        int _i = 301;
        foreach (var obstacle in obstacles)
        {
            // ГАБАРИТЫ
            Obj_ALL_X[_i] = obstacle.l / 5;
            Obj_ALL_Y[_i] = obstacle.w / 5; 
            Obj_ALL_Z[_i] = obstacle.h / 5; 
            // БАЗОВАЯ ТОЧКА СМЕЩЕНИЕ ОТНОСИТЕЛЬНО НАЧАЛА КООРДИНАТ
            Obj_ALL_ST_P_X[_i] = obstacle.x; 
            // в Unity z - глубина от камеры, а y - высота от пола.
            Obj_ALL_ST_P_Y[_i] = obstacle.z;  //  Globals.zZeroFix320 - 
            Obj_ALL_ST_P_Z[_i] = obstacle.y ; 
            _i++;
        }
        #endregion
        #region ЧТЕНИЕ КОЛОНН              
        //берем 99-31 штук колонн из коллонад (коллоны находятся по признаку наличия скрипта MaterialSwitch)
        var columns = (from collonade in collonades select collonade.GetComponentsInChildren<MaterialSwitch>(false)).SelectMany(c=>c).Take(99-31);

        _i = 31;
        //const float factoryWidth = 47000.0f;
        foreach (var column in columns)
        {
            var cube = column.gameObject;
            // ГАБАРИТЫ
            Obj_ALL_X[_i] = cube.transform.lossyScale.x / Globals.scaleX;
            // в Unity z - глубина от камеры, а y - высота от пола.
            Obj_ALL_Y[_i] = cube.transform.lossyScale.z / Globals.scaleZ;
            Obj_ALL_Z[_i] = cube.transform.lossyScale.y / Globals.scaleY;
            // БАЗОВАЯ ТОЧКА СМЕЩЕНИЕ ОТНОСИТЕЛЬНО НАЧАЛА КООРДИНАТ
            Obj_ALL_ST_P_X[_i] = cube.transform.position.x / Globals.scaleX;
            Obj_ALL_ST_P_Y[_i] = Globals.zZeroFix320 - cube.transform.position.z / Globals.scaleZ;
            Obj_ALL_ST_P_Z[_i] = cube.transform.position.y / Globals.scaleY;
            _i++;
        }
        #endregion
        #endregion
        ///
        ////
        ////
        #endregion
        // ------------------------------------------------------------------------------------------------------------------------------------------------------------
        // --- STOP READ ----------------------------------------------------------------------------------------------------------------------------------------------
        // ВХОДНЫЕ ДАННЫЕ
        // ------------------------------------------------------------------------------------------------------------------------------------------------------------
        // NNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNN
        // FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
        // ------------------------------------------------------------------------------------------------------------------------------------------------------------
        #region ПР001 - ДОБАВЛЕНИЕ ОБЪЕКТОВ НЕВИДИМЫХ - СТЕНЫ

        // СТЕНА НАПРОТИВ - ДАЛЬНЯЯ
        i = 131;
        // ГАБАРИТЫ
        Obj_ALL_X[i] = 174000;
        Obj_ALL_Y[i] = 50;
        Obj_ALL_Z[i] = 75000;
        // БАЗОВАЯ ТОЧКА СМЕЩЕНИЕ ОТНОСИТЕЛЬНО НАЧАЛА КООРДИНАТ
        Obj_ALL_ST_P_X[i] = 0;        
        Obj_ALL_ST_P_Y[i] = -100; 
        Obj_ALL_ST_P_Z[i] = 0;
        // СТЕНА НАПРОТИВ - БЛИЖНЯЯ
        i = 132;
        // ГАБАРИТЫ
        Obj_ALL_X[i] = 174000;
        Obj_ALL_Y[i] = 50;
        Obj_ALL_Z[i] = 37500; ;
        // БАЗОВАЯ ТОЧКА СМЕЩЕНИЕ ОТНОСИТЕЛЬНО НАЧАЛА КООРДИНАТ
        Obj_ALL_ST_P_X[i] = 0;
        Obj_ALL_ST_P_Y[i] = -100;
        Obj_ALL_ST_P_Z[i] = 0;
        // СТЕНА РЯДОМ - ДАЛЬНЯЯ - НИЗКАЯ
        i = 133;
        // ГАБАРИТЫ
        Obj_ALL_X[i] = 174000;
        Obj_ALL_Y[i] = 50;
        Obj_ALL_Z[i] = 37500; ;
        // БАЗОВАЯ ТОЧКА СМЕЩЕНИЕ ОТНОСИТЕЛЬНО НАЧАЛА КООРДИНАТ
        Obj_ALL_ST_P_X[i] = 0;
        Obj_ALL_ST_P_Y[i] = 48000 + 100;
        Obj_ALL_ST_P_Z[i] = 0;
        // СТЕНА НАПРОТИВ - БЛИЖНЯЯ - ВЫСОКАЯ
        i = 134;
        // ГАБАРИТЫ
        Obj_ALL_X[i] = 174000;
        Obj_ALL_Y[i] = 50;
        Obj_ALL_Z[i] = 75000;
        // БАЗОВАЯ ТОЧКА СМЕЩЕНИЕ ОТНОСИТЕЛЬНО НАЧАЛА КООРДИНАТ
        Obj_ALL_ST_P_X[i] = 0;
        Obj_ALL_ST_P_Y[i] = 48000 + 100;
        Obj_ALL_ST_P_Z[i] = 0;

        // СТЕНА ТУПИКА - ЛЕВАЯ
        i = 135;
        // ГАБАРИТЫ
        Obj_ALL_X[i] = 50;
        Obj_ALL_Y[i] = 48000;
        Obj_ALL_Z[i] = 75000;
        // БАЗОВАЯ ТОЧКА СМЕЩЕНИЕ ОТНОСИТЕЛЬНО НАЧАЛА КООРДИНАТ
        Obj_ALL_ST_P_X[i] = -50;
        Obj_ALL_ST_P_Y[i] = 0;
        Obj_ALL_ST_P_Z[i] = 0;
        // СТЕНА ТУПИКА - ПРАВАЯ
        i = 136;
        // ГАБАРИТЫ
        Obj_ALL_X[i] = 50;
        Obj_ALL_Y[i] = 48000;
        Obj_ALL_Z[i] = 75000;
        // БАЗОВАЯ ТОЧКА СМЕЩЕНИЕ ОТНОСИТЕЛЬНО НАЧАЛА КООРДИНАТ
        Obj_ALL_ST_P_X[i] = 174000;
        Obj_ALL_ST_P_Y[i] = 0;
        Obj_ALL_ST_P_Z[i] = 0;

        #endregion
        // ======================================================================================
        #region ПР002 - КОРРЕКТИРОВКА ДЕЛЬТА НА ПРЕДУПРЕЖДЕНИЯ И ОГРАНИЧЕНИЯ
        // =============================================================================================
        // ТАБЛИЦА СООТВЕТСТВИЯ
        // ===============================================================================================================================================================
        // n = 1 КРАН 320Т - 1-20/ 2 КРАН 120Т - (100 + (1-20)) => 101-120/ 3 КРАН 25Т - (200 + (1-20)) => 201-220
        // n =
        // 1 - МОСТ
        // 2 - ТЕЛЕЖКА 1  - ГЛАВНАЯ
        // 3 - ТЕЛЕЖКА 1 - ПОДЪЕМ 1 (1-1) - тросс + крюк
        // 4 - ---------------------------------------------------------------------------- НЕ ИСПОЛЬЗУЕТСЯ ДЛЯ ИТОГОВОГО ПРЕДУПРЕЖДЕНИЯ --- тросс + крюк + траверса
        // 5 - ТЕЛЕЖКА 1 - ПОДЪЕМ 1 (1-1) - ТРАВЕРСА
        // 6 - ТЕЛЕЖКА 1 - ПОДЪЕМ 1 (1-1) - груз
        // 7 - ТЕЛЕЖКА 1 - ПОДЪЕМ 2 (1-2) - тросс + крюк
        // 8 - ---------------------------------------------------------------------------- НЕ ИСПОЛЬЗУЕТСЯ ДЛЯ ИТОГОВОГО ПРЕДУПРЕЖДЕНИЯ --- тросс + крюк + траверса
        // 9 - ТЕЛЕЖКА 1 - ПОДЪЕМ 2 (1-2) - ТРАВЕРСА
        // 10 - ТЕЛЕЖКА 1 - ПОДЪЕМ 2 (1-2) - груз
        // 11 - ---------------------------------------------------------------------------- НЕ ИСПОЛЬЗУЕТСЯ ДЛЯ ИТОГОВОГО ПРЕДУПРЕЖДЕНИЯ --- тросс + крюк + траверса + 3 подъем тросс + крюк + траверса
        // 12 - ТЕЛЕЖКА 2  - ВСПОМОГАТЕЛЬНАЯ
        // 13 - ТЕЛЕЖКА 2 - ПОДЪЕМ 3 (2-1) - тросс + крюк
        // 14 - ---------------------------------------------------------------------------- НЕ ИСПОЛЬЗУЕТСЯ ДЛЯ ИТОГОВОГО ПРЕДУПРЕЖДЕНИЯ --- тросс + крюк + траверса
        // 15 - ТЕЛЕЖКА 2 - ПОДЪЕМ 3 (2-1) - ТРАВЕРСА
        // 16 - ТЕЛЕЖКА 2 - ПОДЪЕМ 3 (2-1) - груз
        // 17 - ТЕЛЕЖКА 2 - ПОДЪЕМ 4 (2-2) - тросс + крюк
        // 18 - ---------------------------------------------------------------------------- НЕ ИСПОЛЬЗУЕТСЯ ДЛЯ ИТОГОВОГО ПРЕДУПРЕЖДЕНИЯ --- тросс + крюк + траверса
        // 19 - ТЕЛЕЖКА 2 - ПОДЪЕМ 4 (2-2) - ТРАВЕРСА
        // 20 - ТЕЛЕЖКА 2 - ПОДЪЕМ 4 (2-2) - груз
        // ===============================================================================================================================================================  
        // ПО УМОЛЧАНИЮ - 
        // - ДЕЛЬТА ОГРАНИЧЕНИЙ НА МОСТ - ТОЛЬКО ДЛЯ ВПРАВО/ВЛЕВО - ПРИНИМАЕМ БАЗОВЫЕ МЕНЮ НАСТРОЙКИ - ОСТАЛЬНЫЕ НУЛЕВЫЕ!!!!!!!!!!!!!!!!!
        // - ДЕЛЬТА ОГРАНИЧЕНИЙ НА ТЕЛЕЖКУ - ВВЕРХ/ВНИЗ = ER0/W0 / ВЛЕВО/ВПРАВО - ER3000/W6000 / ВПЕРЕД/НАЗАД - ER3000/W6000
        // - ДЕЛЬТА ОГРАНИЧЕНИЙ НА КРЮК И ТРОСС - ВВЕРХ/ВНИЗ = ER0/W2000 / ВЛЕВО/ВПРАВО - ER3000/W6000 / ВПЕРЕД/НАЗАД - ER3000/W6000
        // - ДЕЛЬТА ОГРАНИЧЕНИЙ НА ТРАВЕРСУ - ВВЕРХ/ВНИЗ = ER0/W2000 / ВЛЕВО/ВПРАВО - ER3000/W6000 / ВПЕРЕД/НАЗАД - ER3000/W6000
        // - ДЕЛЬТА ОГРАНИЧЕНИЙ НА ГРУЗ - ВВЕРХ/ВНИЗ = ER0/W2000 / ВЛЕВО/ВПРАВО - ER3000/W6000 / ВПЕРЕД/НАЗАД - ER3000/W6000
        //
        #region КРАН320 - КОРРЕКЦИЯ ОТ БАЗОВЫХ - НАСТРОЕЧНЫЕ ПАРАМЕТРЫ ДЕЛЬТЫ ПРЕДУПРЕЖДЕНИЯ И ДЕЛЬТЫ АВАРИИ ЭЛЕМЕНТОВ КРАНА
        j = 1;
        #region КОРРЕКЦИЯ - МОСТ - ДЕЛЬТА - ДЕЛЬТА ТОЛЬКО НА ВЛЕВО/ВПРАВО
        f = 1; // МОСТ
        // НАСТРОЕЧНЫЕ ПАРАМЕТРЫ КРАНА
        // ДЕЛЬТА ПРЕДУПРЕЖДЕНИЯ        
        crane[j].D_UP_WARN[f] = 0;
        crane[j].D_DOWN_WARN[f] = 0;
        //crane[j].D_R_WARN[f] = 0;
        //crane[j].D_L_WARN[f] = 0;
        crane[j].D_F_WARN[f] = 0;
        crane[j].D_B_WARN[f] = 0;
        // ДЕЛЬТА АВАРИИ
        crane[j].D_UP_ERR[f] = 0;
        crane[j].D_DOWN_ERR[f] = 0;
        //crane[j].D_R_ERR[f] = 0;
        //crane[j].D_L_ERR[f] = 0;
        crane[j].D_F_ERR[f] = 0;
        crane[j].D_B_ERR[f] = 0;
        #endregion
        //
        #region КОРРЕКЦИЯ - ТЕЛЕЖКА - 1  - ДЕЛЬТА
        f = 2; // ТЕЛЕЖКА
        // НАСТРОЕЧНЫЕ ПАРАМЕТРЫ КРАНА
        // ДЕЛЬТА ПРЕДУПРЕЖДЕНИЯ        
        crane[j].D_UP_WARN[f] = 0;
        crane[j].D_DOWN_WARN[f] = 0;
        crane[j].D_R_WARN[f] = 6000;
        crane[j].D_L_WARN[f] = 6000;
        crane[j].D_F_WARN[f] = 6000;
        crane[j].D_B_WARN[f] = 6000;
        // ДЕЛЬТА АВАРИИ
        crane[j].D_UP_ERR[f] = 0;
        crane[j].D_DOWN_ERR[f] = 0;
        crane[j].D_R_ERR[f] = 3000;
        crane[j].D_L_ERR[f] = 3000;
        crane[j].D_F_ERR[f] = 3000;
        crane[j].D_B_ERR[f] = 3000;
        #endregion
        //
        #region КОРРЕКЦИЯ - ПОДЪЕМ - 1-1  - ДЕЛЬТА - ТРОСС + КРЮК
        f = 3; // ПОДЪЕМ
        //НАСТРОЕЧНЫЕ ПАРАМЕТРЫ КРАНА
        // ДЕЛЬТА ПРЕДУПРЕЖДЕНИЯ        
        crane[j].D_UP_WARN[f] = 2000;
        crane[j].D_DOWN_WARN[f] = 2000;
        crane[j].D_R_WARN[f] = 6000;
        crane[j].D_L_WARN[f] = 6000;
        crane[j].D_F_WARN[f] = 6000;
        crane[j].D_B_WARN[f] = 6000;
        // ДЕЛЬТА АВАРИИ
        crane[j].D_UP_ERR[f] = 0;
        crane[j].D_DOWN_ERR[f] = 0;
        crane[j].D_R_ERR[f] = 3000;
        crane[j].D_L_ERR[f] = 3000;
        crane[j].D_F_ERR[f] = 3000;
        crane[j].D_B_ERR[f] = 3000;
        #endregion
        #region КОРРЕКЦИЯ - ПОДЪЕМ - 1-1  - ДЕЛЬТА - ТРАВЕРСА
        f = 5; // ПОДЪЕМ
        //НАСТРОЕЧНЫЕ ПАРАМЕТРЫ КРАНА
        // ДЕЛЬТА ПРЕДУПРЕЖДЕНИЯ        
        crane[j].D_UP_WARN[f] = 2000;
        crane[j].D_DOWN_WARN[f] = 2000;
        crane[j].D_R_WARN[f] = 6000;
        crane[j].D_L_WARN[f] = 6000;
        crane[j].D_F_WARN[f] = 6000;
        crane[j].D_B_WARN[f] = 6000;
        // ДЕЛЬТА АВАРИИ
        crane[j].D_UP_ERR[f] = 0;
        crane[j].D_DOWN_ERR[f] = 0;
        crane[j].D_R_ERR[f] = 3000;
        crane[j].D_L_ERR[f] = 3000;
        crane[j].D_F_ERR[f] = 3000;
        crane[j].D_B_ERR[f] = 3000;
        #endregion
        #region КОРРЕКЦИЯ - ПОДЪЕМ - 1-1  - ДЕЛЬТА - груз
        f = 6; // ПОДЪЕМ
        //НАСТРОЕЧНЫЕ ПАРАМЕТРЫ КРАНА
        // ДЕЛЬТА ПРЕДУПРЕЖДЕНИЯ        
        crane[j].D_UP_WARN[f] = 6000;
        crane[j].D_DOWN_WARN[f] = 2000;
        crane[j].D_R_WARN[f] = 6000;
        crane[j].D_L_WARN[f] = 6000;
        crane[j].D_F_WARN[f] = 6000;
        crane[j].D_B_WARN[f] = 6000;
        // ДЕЛЬТА АВАРИИ
        crane[j].D_UP_ERR[f] = 0;
        crane[j].D_DOWN_ERR[f] = 0;
        crane[j].D_R_ERR[f] = 3000;
        crane[j].D_L_ERR[f] = 3000;
        crane[j].D_F_ERR[f] = 3000;
        crane[j].D_B_ERR[f] = 3000;
        #endregion
        //
        #region КОРРЕКЦИЯ - ПОДЪЕМ - 1-2  - ДЕЛЬТА - ТРОСС + КРЮК
        f = 7; // ПОДЪЕМ
        //НАСТРОЕЧНЫЕ ПАРАМЕТРЫ КРАНА
        // ДЕЛЬТА ПРЕДУПРЕЖДЕНИЯ        
        crane[j].D_UP_WARN[f] = 6000;
        crane[j].D_DOWN_WARN[f] = 2000;
        crane[j].D_R_WARN[f] = 6000;
        crane[j].D_L_WARN[f] = 6000;
        crane[j].D_F_WARN[f] = 6000;
        crane[j].D_B_WARN[f] = 6000;
        // ДЕЛЬТА АВАРИИ
        crane[j].D_UP_ERR[f] = 0;
        crane[j].D_DOWN_ERR[f] = 0;
        crane[j].D_R_ERR[f] = 3000;
        crane[j].D_L_ERR[f] = 3000;
        crane[j].D_F_ERR[f] = 3000;
        crane[j].D_B_ERR[f] = 3000;
        #endregion
        #region КОРРЕКЦИЯ - ПОДЪЕМ - 1-2  - ДЕЛЬТА - ТРАВЕРСА
        f = 9; // ПОДЪЕМ
        //НАСТРОЕЧНЫЕ ПАРАМЕТРЫ КРАНА
        // ДЕЛЬТА ПРЕДУПРЕЖДЕНИЯ        
        crane[j].D_UP_WARN[f] = 6000;
        crane[j].D_DOWN_WARN[f] = 2000;
        crane[j].D_R_WARN[f] = 6000;
        crane[j].D_L_WARN[f] = 6000;
        crane[j].D_F_WARN[f] = 6000;
        crane[j].D_B_WARN[f] = 6000;
        // ДЕЛЬТА АВАРИИ
        crane[j].D_UP_ERR[f] = 0;
        crane[j].D_DOWN_ERR[f] = 0;
        crane[j].D_R_ERR[f] = 3000;
        crane[j].D_L_ERR[f] = 3000;
        crane[j].D_F_ERR[f] = 3000;
        crane[j].D_B_ERR[f] = 3000;
        #endregion
        #region КОРРЕКЦИЯ - ПОДЪЕМ - 1-2  - ДЕЛЬТА - груз
        f = 10; // ПОДЪЕМ
        //НАСТРОЕЧНЫЕ ПАРАМЕТРЫ КРАНА
        // ДЕЛЬТА ПРЕДУПРЕЖДЕНИЯ        
        crane[j].D_UP_WARN[f] = 6000;
        crane[j].D_DOWN_WARN[f] = 2000;
        crane[j].D_R_WARN[f] = 6000;
        crane[j].D_L_WARN[f] = 6000;
        crane[j].D_F_WARN[f] = 6000;
        crane[j].D_B_WARN[f] = 6000;
        // ДЕЛЬТА АВАРИИ
        crane[j].D_UP_ERR[f] = 0;
        crane[j].D_DOWN_ERR[f] = 0;
        crane[j].D_R_ERR[f] = 3000;
        crane[j].D_L_ERR[f] = 3000;
        crane[j].D_F_ERR[f] = 3000;
        crane[j].D_B_ERR[f] = 3000;
        #endregion
        //
        #region КОРРЕКЦИЯ - ТЕЛЕЖКА - 2  - ДЕЛЬТА
        f = 12; // ТЕЛЕЖКА
        // НАСТРОЕЧНЫЕ ПАРАМЕТРЫ КРАНА
        // ДЕЛЬТА ПРЕДУПРЕЖДЕНИЯ        
        crane[j].D_UP_WARN[f] = 0;
        crane[j].D_DOWN_WARN[f] = 0;
        crane[j].D_R_WARN[f] = 6000;
        crane[j].D_L_WARN[f] = 6000;
        crane[j].D_F_WARN[f] = 6000;
        crane[j].D_B_WARN[f] = 6000;
        // ДЕЛЬТА АВАРИИ
        crane[j].D_UP_ERR[f] = 0;
        crane[j].D_DOWN_ERR[f] = 0;
        crane[j].D_R_ERR[f] = 3000;
        crane[j].D_L_ERR[f] = 3000;
        crane[j].D_F_ERR[f] = 3000;
        crane[j].D_B_ERR[f] = 3000;
        #endregion
        //
        #region КОРРЕКЦИЯ - ПОДЪЕМ - 2-1  - ДЕЛЬТА - ТРОСС + КРЮК
        f = 13; // ПОДЪЕМ
        // НАСТРОЕЧНЫЕ ПАРАМЕТРЫ КРАНА
        // ДЕЛЬТА ПРЕДУПРЕЖДЕНИЯ        
        crane[j].D_UP_WARN[f] = 2000;
        crane[j].D_DOWN_WARN[f] = 2000;
        crane[j].D_R_WARN[f] = 6000;
        crane[j].D_L_WARN[f] = 6000;
        crane[j].D_F_WARN[f] = 6000;
        crane[j].D_B_WARN[f] = 6000;
        // ДЕЛЬТА АВАРИИ
        crane[j].D_UP_ERR[f] = 0;
        crane[j].D_DOWN_ERR[f] = 0;
        crane[j].D_R_ERR[f] = 3000;
        crane[j].D_L_ERR[f] = 3000;
        crane[j].D_F_ERR[f] = 3000;
        crane[j].D_B_ERR[f] = 3000;
        #endregion
        #region КОРРЕКЦИЯ - ПОДЪЕМ - 2-1  - ДЕЛЬТА  - ТРАВЕРСА
        f = 15; // ПОДЪЕМ
        // НАСТРОЕЧНЫЕ ПАРАМЕТРЫ КРАНА
        // ДЕЛЬТА ПРЕДУПРЕЖДЕНИЯ        
        crane[j].D_UP_WARN[f] = 2000;
        crane[j].D_DOWN_WARN[f] = 2000;
        crane[j].D_R_WARN[f] = 6000;
        crane[j].D_L_WARN[f] = 6000;
        crane[j].D_F_WARN[f] = 6000;
        crane[j].D_B_WARN[f] = 6000;
        // ДЕЛЬТА АВАРИИ
        crane[j].D_UP_ERR[f] = 0;
        crane[j].D_DOWN_ERR[f] = 0;
        crane[j].D_R_ERR[f] = 3000;
        crane[j].D_L_ERR[f] = 3000;
        crane[j].D_F_ERR[f] = 3000;
        crane[j].D_B_ERR[f] = 3000;
        #endregion
        #region КОРРЕКЦИЯ - ПОДЪЕМ - 2-1  - ДЕЛЬТА - груз
        f = 16; // ПОДЪЕМ
        //НАСТРОЕЧНЫЕ ПАРАМЕТРЫ КРАНА
        // ДЕЛЬТА ПРЕДУПРЕЖДЕНИЯ        
        crane[j].D_UP_WARN[f] = 6000;
        crane[j].D_DOWN_WARN[f] = 2000;
        crane[j].D_R_WARN[f] = 6000;
        crane[j].D_L_WARN[f] = 6000;
        crane[j].D_F_WARN[f] = 6000;
        crane[j].D_B_WARN[f] = 6000;
        // ДЕЛЬТА АВАРИИ
        crane[j].D_UP_ERR[f] = 0;
        crane[j].D_DOWN_ERR[f] = 0;
        crane[j].D_R_ERR[f] = 3000;
        crane[j].D_L_ERR[f] = 3000;
        crane[j].D_F_ERR[f] = 3000;
        crane[j].D_B_ERR[f] = 3000;
        #endregion
        //
        #region КОРРЕКЦИЯ - ПОДЪЕМ - 2-2  - ДЕЛЬТА - ТРОСС + КРЮК
        f = 17; // ПОДЪЕМ
        // НАСТРОЕЧНЫЕ ПАРАМЕТРЫ КРАНА
        // ДЕЛЬТА ПРЕДУПРЕЖДЕНИЯ        
        crane[j].D_UP_WARN[f] = 2000;
        crane[j].D_DOWN_WARN[f] = 2000;
        crane[j].D_R_WARN[f] = 6000;
        crane[j].D_L_WARN[f] = 6000;
        crane[j].D_F_WARN[f] = 6000;
        crane[j].D_B_WARN[f] = 6000;
        // ДЕЛЬТА АВАРИИ
        crane[j].D_UP_ERR[f] = 0;
        crane[j].D_DOWN_ERR[f] = 0;
        crane[j].D_R_ERR[f] = 3000;
        crane[j].D_L_ERR[f] = 3000;
        crane[j].D_F_ERR[f] = 3000;
        crane[j].D_B_ERR[f] = 3000;
        #endregion
        #region КОРРЕКЦИЯ - ПОДЪЕМ - 2-2  - ДЕЛЬТА  - ТРАВЕРСА
        f = 19; // ПОДЪЕМ
        // НАСТРОЕЧНЫЕ ПАРАМЕТРЫ КРАНА
        // ДЕЛЬТА ПРЕДУПРЕЖДЕНИЯ        
        crane[j].D_UP_WARN[f] = 2000;
        crane[j].D_DOWN_WARN[f] = 2000;
        crane[j].D_R_WARN[f] = 6000;
        crane[j].D_L_WARN[f] = 6000;
        crane[j].D_F_WARN[f] = 6000;
        crane[j].D_B_WARN[f] = 6000;
        // ДЕЛЬТА АВАРИИ
        crane[j].D_UP_ERR[f] = 0;
        crane[j].D_DOWN_ERR[f] = 0;
        crane[j].D_R_ERR[f] = 3000;
        crane[j].D_L_ERR[f] = 3000;
        crane[j].D_F_ERR[f] = 3000;
        crane[j].D_B_ERR[f] = 3000;
        #endregion
        #region КОРРЕКЦИЯ - ПОДЪЕМ - 2-2  - ДЕЛЬТА - груз
        f = 20; // ПОДЪЕМ
        //НАСТРОЕЧНЫЕ ПАРАМЕТРЫ КРАНА
        // ДЕЛЬТА ПРЕДУПРЕЖДЕНИЯ        
        crane[j].D_UP_WARN[f] = 6000;
        crane[j].D_DOWN_WARN[f] = 2000;
        crane[j].D_R_WARN[f] = 6000;
        crane[j].D_L_WARN[f] = 6000;
        crane[j].D_F_WARN[f] = 6000;
        crane[j].D_B_WARN[f] = 6000;
        // ДЕЛЬТА АВАРИИ
        crane[j].D_UP_ERR[f] = 0;
        crane[j].D_DOWN_ERR[f] = 0;
        crane[j].D_R_ERR[f] = 3000;
        crane[j].D_L_ERR[f] = 3000;
        crane[j].D_F_ERR[f] = 3000;
        crane[j].D_B_ERR[f] = 3000;
        #endregion
        //
        #endregion
        //
        #region КРАН120 - КОРРЕКЦИЯ ОТ БАЗОВЫХ - НАСТРОЕЧНЫЕ ПАРАМЕТРЫ ДЕЛЬТЫ ПРЕДУПРЕЖДЕНИЯ И ДЕЛЬТЫ АВАРИИ ЭЛЕМЕНТОВ КРАНА
        j = 2;
        #region КОРРЕКЦИЯ - МОСТ - ДЕЛЬТА - ДЕЛЬТА ТОЛЬКО НА ВЛЕВО/ВПРАВО
        f = 1; // МОСТ
        // НАСТРОЕЧНЫЕ ПАРАМЕТРЫ КРАНА
        // ДЕЛЬТА ПРЕДУПРЕЖДЕНИЯ        
        crane[j].D_UP_WARN[f] = 0;
        crane[j].D_DOWN_WARN[f] = 0;
        //crane[j].D_R_WARN[f] = 0;
        //crane[j].D_L_WARN[f] = 0;
        crane[j].D_F_WARN[f] = 0;
        crane[j].D_B_WARN[f] = 0;
        // ДЕЛЬТА АВАРИИ
        crane[j].D_UP_ERR[f] = 0;
        crane[j].D_DOWN_ERR[f] = 0;
        //crane[j].D_R_ERR[f] = 0;
        //crane[j].D_L_ERR[f] = 0;
        crane[j].D_F_ERR[f] = 0;
        crane[j].D_B_ERR[f] = 0;
        #endregion
        //
        #region КОРРЕКЦИЯ - ТЕЛЕЖКА - 1  - ДЕЛЬТА
        f = 2; // ТЕЛЕЖКА
        // НАСТРОЕЧНЫЕ ПАРАМЕТРЫ КРАНА
        // ДЕЛЬТА ПРЕДУПРЕЖДЕНИЯ        
        crane[j].D_UP_WARN[f] = 0;
        crane[j].D_DOWN_WARN[f] = 0;
        crane[j].D_R_WARN[f] = 6000;
        crane[j].D_L_WARN[f] = 6000;
        crane[j].D_F_WARN[f] = 6000;
        crane[j].D_B_WARN[f] = 6000;
        // ДЕЛЬТА АВАРИИ
        crane[j].D_UP_ERR[f] = 0;
        crane[j].D_DOWN_ERR[f] = 0;
        crane[j].D_R_ERR[f] = 3000;
        crane[j].D_L_ERR[f] = 3000;
        crane[j].D_F_ERR[f] = 3000;
        crane[j].D_B_ERR[f] = 3000;
        #endregion
        //
        #region КОРРЕКЦИЯ - ПОДЪЕМ - 1-1  - ДЕЛЬТА - ТРОСС + КРЮК
        f = 3; // ПОДЪЕМ
        //НАСТРОЕЧНЫЕ ПАРАМЕТРЫ КРАНА
        // ДЕЛЬТА ПРЕДУПРЕЖДЕНИЯ        
        crane[j].D_UP_WARN[f] = 2000;
        crane[j].D_DOWN_WARN[f] = 2000;
        crane[j].D_R_WARN[f] = 6000;
        crane[j].D_L_WARN[f] = 6000;
        crane[j].D_F_WARN[f] = 6000;
        crane[j].D_B_WARN[f] = 6000;
        // ДЕЛЬТА АВАРИИ
        crane[j].D_UP_ERR[f] = 0;
        crane[j].D_DOWN_ERR[f] = 0;
        crane[j].D_R_ERR[f] = 3000;
        crane[j].D_L_ERR[f] = 3000;
        crane[j].D_F_ERR[f] = 3000;
        crane[j].D_B_ERR[f] = 3000;
        #endregion
        #region КОРРЕКЦИЯ - ПОДЪЕМ - 1-1  - ДЕЛЬТА - ТРАВЕРСА
        f = 5; // ПОДЪЕМ
        //НАСТРОЕЧНЫЕ ПАРАМЕТРЫ КРАНА
        // ДЕЛЬТА ПРЕДУПРЕЖДЕНИЯ        
        crane[j].D_UP_WARN[f] = 2000;
        crane[j].D_DOWN_WARN[f] = 2000;
        crane[j].D_R_WARN[f] = 6000;
        crane[j].D_L_WARN[f] = 6000;
        crane[j].D_F_WARN[f] = 6000;
        crane[j].D_B_WARN[f] = 6000;
        // ДЕЛЬТА АВАРИИ
        crane[j].D_UP_ERR[f] = 0;
        crane[j].D_DOWN_ERR[f] = 0;
        crane[j].D_R_ERR[f] = 3000;
        crane[j].D_L_ERR[f] = 3000;
        crane[j].D_F_ERR[f] = 3000;
        crane[j].D_B_ERR[f] = 3000;
        #endregion
        #region КОРРЕКЦИЯ - ПОДЪЕМ - 1-1  - ДЕЛЬТА - груз
        f = 6; // ПОДЪЕМ
        //НАСТРОЕЧНЫЕ ПАРАМЕТРЫ КРАНА
        // ДЕЛЬТА ПРЕДУПРЕЖДЕНИЯ        
        crane[j].D_UP_WARN[f] = 2000;
        crane[j].D_DOWN_WARN[f] = 2000;
        crane[j].D_R_WARN[f] = 6000;
        crane[j].D_L_WARN[f] = 6000;
        crane[j].D_F_WARN[f] = 6000;
        crane[j].D_B_WARN[f] = 6000;
        // ДЕЛЬТА АВАРИИ
        crane[j].D_UP_ERR[f] = 0;
        crane[j].D_DOWN_ERR[f] = 0;
        crane[j].D_R_ERR[f] = 3000;
        crane[j].D_L_ERR[f] = 3000;
        crane[j].D_F_ERR[f] = 3000;
        crane[j].D_B_ERR[f] = 3000;
        #endregion
        //
        #region КОРРЕКЦИЯ - ПОДЪЕМ - 1-2  - ДЕЛЬТА - ТРОСС + КРЮК
        f = 7; // ПОДЪЕМ
        //НАСТРОЕЧНЫЕ ПАРАМЕТРЫ КРАНА
        // ДЕЛЬТА ПРЕДУПРЕЖДЕНИЯ        
        crane[j].D_UP_WARN[f] = 2000;
        crane[j].D_DOWN_WARN[f] = 2000;
        crane[j].D_R_WARN[f] = 6000;
        crane[j].D_L_WARN[f] = 6000;
        crane[j].D_F_WARN[f] = 6000;
        crane[j].D_B_WARN[f] = 6000;
        // ДЕЛЬТА АВАРИИ
        crane[j].D_UP_ERR[f] = 0;
        crane[j].D_DOWN_ERR[f] = 0;
        crane[j].D_R_ERR[f] = 3000;
        crane[j].D_L_ERR[f] = 3000;
        crane[j].D_F_ERR[f] = 3000;
        crane[j].D_B_ERR[f] = 3000;
        #endregion
        #region КОРРЕКЦИЯ - ПОДЪЕМ - 1-2  - ДЕЛЬТА - ТРАВЕРСА
        f = 9; // ПОДЪЕМ
        //НАСТРОЕЧНЫЕ ПАРАМЕТРЫ КРАНА
        // ДЕЛЬТА ПРЕДУПРЕЖДЕНИЯ        
        crane[j].D_UP_WARN[f] = 2000;
        crane[j].D_DOWN_WARN[f] = 2000;
        crane[j].D_R_WARN[f] = 6000;
        crane[j].D_L_WARN[f] = 6000;
        crane[j].D_F_WARN[f] = 6000;
        crane[j].D_B_WARN[f] = 6000;
        // ДЕЛЬТА АВАРИИ
        crane[j].D_UP_ERR[f] = 0;
        crane[j].D_DOWN_ERR[f] = 0;
        crane[j].D_R_ERR[f] = 3000;
        crane[j].D_L_ERR[f] = 3000;
        crane[j].D_F_ERR[f] = 3000;
        crane[j].D_B_ERR[f] = 3000;
        #endregion
        #region КОРРЕКЦИЯ - ПОДЪЕМ - 1-2  - ДЕЛЬТА - груз
        f = 10; // ПОДЪЕМ
        //НАСТРОЕЧНЫЕ ПАРАМЕТРЫ КРАНА
        // ДЕЛЬТА ПРЕДУПРЕЖДЕНИЯ        
        crane[j].D_UP_WARN[f] = 2000;
        crane[j].D_DOWN_WARN[f] = 2000;
        crane[j].D_R_WARN[f] = 6000;
        crane[j].D_L_WARN[f] = 6000;
        crane[j].D_F_WARN[f] = 6000;
        crane[j].D_B_WARN[f] = 6000;
        // ДЕЛЬТА АВАРИИ
        crane[j].D_UP_ERR[f] = 0;
        crane[j].D_DOWN_ERR[f] = 0;
        crane[j].D_R_ERR[f] = 3000;
        crane[j].D_L_ERR[f] = 3000;
        crane[j].D_F_ERR[f] = 3000;
        crane[j].D_B_ERR[f] = 3000;
        #endregion
        //
        #region КОРРЕКЦИЯ - ТЕЛЕЖКА - 2  - ДЕЛЬТА
        f = 12; // ТЕЛЕЖКА
        // НАСТРОЕЧНЫЕ ПАРАМЕТРЫ КРАНА
        // ДЕЛЬТА ПРЕДУПРЕЖДЕНИЯ        
        crane[j].D_UP_WARN[f] = 0;
        crane[j].D_DOWN_WARN[f] = 0;
        crane[j].D_R_WARN[f] = 6000;
        crane[j].D_L_WARN[f] = 6000;
        crane[j].D_F_WARN[f] = 6000;
        crane[j].D_B_WARN[f] = 6000;
        // ДЕЛЬТА АВАРИИ
        crane[j].D_UP_ERR[f] = 0;
        crane[j].D_DOWN_ERR[f] = 0;
        crane[j].D_R_ERR[f] = 3000;
        crane[j].D_L_ERR[f] = 3000;
        crane[j].D_F_ERR[f] = 3000;
        crane[j].D_B_ERR[f] = 3000;
        #endregion
        //
        #region КОРРЕКЦИЯ - ПОДЪЕМ - 2-1  - ДЕЛЬТА - ТРОСС + КРЮК
        f = 13; // ПОДЪЕМ
        // НАСТРОЕЧНЫЕ ПАРАМЕТРЫ КРАНА
        // ДЕЛЬТА ПРЕДУПРЕЖДЕНИЯ        
        crane[j].D_UP_WARN[f] = 2000;
        crane[j].D_DOWN_WARN[f] = 2000;
        crane[j].D_R_WARN[f] = 6000;
        crane[j].D_L_WARN[f] = 6000;
        crane[j].D_F_WARN[f] = 6000;
        crane[j].D_B_WARN[f] = 6000;
        // ДЕЛЬТА АВАРИИ
        crane[j].D_UP_ERR[f] = 0;
        crane[j].D_DOWN_ERR[f] = 0;
        crane[j].D_R_ERR[f] = 3000;
        crane[j].D_L_ERR[f] = 3000;
        crane[j].D_F_ERR[f] = 3000;
        crane[j].D_B_ERR[f] = 3000;
        #endregion
        #region КОРРЕКЦИЯ - ПОДЪЕМ - 2-1  - ДЕЛЬТА  - ТРАВЕРСА
        f = 15; // ПОДЪЕМ
        // НАСТРОЕЧНЫЕ ПАРАМЕТРЫ КРАНА
        // ДЕЛЬТА ПРЕДУПРЕЖДЕНИЯ        
        crane[j].D_UP_WARN[f] = 2000;
        crane[j].D_DOWN_WARN[f] = 2000;
        crane[j].D_R_WARN[f] = 6000;
        crane[j].D_L_WARN[f] = 6000;
        crane[j].D_F_WARN[f] = 6000;
        crane[j].D_B_WARN[f] = 6000;
        // ДЕЛЬТА АВАРИИ
        crane[j].D_UP_ERR[f] = 0;
        crane[j].D_DOWN_ERR[f] = 0;
        crane[j].D_R_ERR[f] = 3000;
        crane[j].D_L_ERR[f] = 3000;
        crane[j].D_F_ERR[f] = 3000;
        crane[j].D_B_ERR[f] = 3000;
        #endregion
        #region КОРРЕКЦИЯ - ПОДЪЕМ - 2-1  - ДЕЛЬТА - груз
        f = 16; // ПОДЪЕМ
        //НАСТРОЕЧНЫЕ ПАРАМЕТРЫ КРАНА
        // ДЕЛЬТА ПРЕДУПРЕЖДЕНИЯ        
        crane[j].D_UP_WARN[f] = 2000;
        crane[j].D_DOWN_WARN[f] = 2000;
        crane[j].D_R_WARN[f] = 6000;
        crane[j].D_L_WARN[f] = 6000;
        crane[j].D_F_WARN[f] = 6000;
        crane[j].D_B_WARN[f] = 6000;
        // ДЕЛЬТА АВАРИИ
        crane[j].D_UP_ERR[f] = 0;
        crane[j].D_DOWN_ERR[f] = 0;
        crane[j].D_R_ERR[f] = 3000;
        crane[j].D_L_ERR[f] = 3000;
        crane[j].D_F_ERR[f] = 3000;
        crane[j].D_B_ERR[f] = 3000;
        #endregion
        //
        #region КОРРЕКЦИЯ - ПОДЪЕМ - 2-2  - ДЕЛЬТА - ТРОСС + КРЮК
        f = 17; // ПОДЪЕМ
        // НАСТРОЕЧНЫЕ ПАРАМЕТРЫ КРАНА
        // ДЕЛЬТА ПРЕДУПРЕЖДЕНИЯ        
        crane[j].D_UP_WARN[f] = 2000;
        crane[j].D_DOWN_WARN[f] = 2000;
        crane[j].D_R_WARN[f] = 6000;
        crane[j].D_L_WARN[f] = 6000;
        crane[j].D_F_WARN[f] = 6000;
        crane[j].D_B_WARN[f] = 6000;
        // ДЕЛЬТА АВАРИИ
        crane[j].D_UP_ERR[f] = 0;
        crane[j].D_DOWN_ERR[f] = 0;
        crane[j].D_R_ERR[f] = 3000;
        crane[j].D_L_ERR[f] = 3000;
        crane[j].D_F_ERR[f] = 3000;
        crane[j].D_B_ERR[f] = 3000;
        #endregion
        #region КОРРЕКЦИЯ - ПОДЪЕМ - 2-2  - ДЕЛЬТА  - ТРАВЕРСА
        f = 19; // ПОДЪЕМ
        // НАСТРОЕЧНЫЕ ПАРАМЕТРЫ КРАНА
        // ДЕЛЬТА ПРЕДУПРЕЖДЕНИЯ        
        crane[j].D_UP_WARN[f] = 2000;
        crane[j].D_DOWN_WARN[f] = 2000;
        crane[j].D_R_WARN[f] = 6000;
        crane[j].D_L_WARN[f] = 6000;
        crane[j].D_F_WARN[f] = 6000;
        crane[j].D_B_WARN[f] = 6000;
        // ДЕЛЬТА АВАРИИ
        crane[j].D_UP_ERR[f] = 0;
        crane[j].D_DOWN_ERR[f] = 0;
        crane[j].D_R_ERR[f] = 3000;
        crane[j].D_L_ERR[f] = 3000;
        crane[j].D_F_ERR[f] = 3000;
        crane[j].D_B_ERR[f] = 3000;
        #endregion
        #region КОРРЕКЦИЯ - ПОДЪЕМ - 2-2  - ДЕЛЬТА - груз
        f = 20; // ПОДЪЕМ
        //НАСТРОЕЧНЫЕ ПАРАМЕТРЫ КРАНА
        // ДЕЛЬТА ПРЕДУПРЕЖДЕНИЯ        
        crane[j].D_UP_WARN[f] = 2000;
        crane[j].D_DOWN_WARN[f] = 2000;
        crane[j].D_R_WARN[f] = 6000;
        crane[j].D_L_WARN[f] = 6000;
        crane[j].D_F_WARN[f] = 6000;
        crane[j].D_B_WARN[f] = 6000;
        // ДЕЛЬТА АВАРИИ
        crane[j].D_UP_ERR[f] = 0;
        crane[j].D_DOWN_ERR[f] = 0;
        crane[j].D_R_ERR[f] = 3000;
        crane[j].D_L_ERR[f] = 3000;
        crane[j].D_F_ERR[f] = 3000;
        crane[j].D_B_ERR[f] = 3000;
        #endregion
        //
        #endregion
        //
        #region КРАН25 - КОРРЕКЦИЯ ОТ БАЗОВЫХ - НАСТРОЕЧНЫЕ ПАРАМЕТРЫ ДЕЛЬТЫ ПРЕДУПРЕЖДЕНИЯ И ДЕЛЬТЫ АВАРИИ ЭЛЕМЕНТОВ КРАНА
        j = 3;
        #region КОРРЕКЦИЯ - МОСТ - ДЕЛЬТА - ДЕЛЬТА ТОЛЬКО НА ВЛЕВО/ВПРАВО
        f = 1; // МОСТ
        // НАСТРОЕЧНЫЕ ПАРАМЕТРЫ КРАНА
        // ДЕЛЬТА ПРЕДУПРЕЖДЕНИЯ        
        crane[j].D_UP_WARN[f] = 0;
        crane[j].D_DOWN_WARN[f] = 0;
        //crane[j].D_R_WARN[f] = 0;
        //crane[j].D_L_WARN[f] = 0;
        crane[j].D_F_WARN[f] = 0;
        crane[j].D_B_WARN[f] = 0;
        // ДЕЛЬТА АВАРИИ
        crane[j].D_UP_ERR[f] = 0;
        crane[j].D_DOWN_ERR[f] = 0;
        //crane[j].D_R_ERR[f] = 0;
        //crane[j].D_L_ERR[f] = 0;
        crane[j].D_F_ERR[f] = 0;
        crane[j].D_B_ERR[f] = 0;
        #endregion
        //
        #region КОРРЕКЦИЯ - ТЕЛЕЖКА - 1  - ДЕЛЬТА
        f = 2; // ТЕЛЕЖКА
        // НАСТРОЕЧНЫЕ ПАРАМЕТРЫ КРАНА
        // ДЕЛЬТА ПРЕДУПРЕЖДЕНИЯ        
        crane[j].D_UP_WARN[f] = 0;
        crane[j].D_DOWN_WARN[f] = 0;
        crane[j].D_R_WARN[f] = 6000;
        crane[j].D_L_WARN[f] = 6000;
        crane[j].D_F_WARN[f] = 6000;
        crane[j].D_B_WARN[f] = 6000;
        // ДЕЛЬТА АВАРИИ
        crane[j].D_UP_ERR[f] = 0;
        crane[j].D_DOWN_ERR[f] = 0;
        crane[j].D_R_ERR[f] = 3000;
        crane[j].D_L_ERR[f] = 3000;
        crane[j].D_F_ERR[f] = 3000;
        crane[j].D_B_ERR[f] = 3000;
        #endregion
        //
        #region КОРРЕКЦИЯ - ПОДЪЕМ - 1-1  - ДЕЛЬТА - ТРОСС + КРЮК
        f = 3; // ПОДЪЕМ
        //НАСТРОЕЧНЫЕ ПАРАМЕТРЫ КРАНА
        // ДЕЛЬТА ПРЕДУПРЕЖДЕНИЯ        
        crane[j].D_UP_WARN[f] = 2000;
        crane[j].D_DOWN_WARN[f] = 2000;
        crane[j].D_R_WARN[f] = 6000;
        crane[j].D_L_WARN[f] = 6000;
        crane[j].D_F_WARN[f] = 6000;
        crane[j].D_B_WARN[f] = 6000;
        // ДЕЛЬТА АВАРИИ
        crane[j].D_UP_ERR[f] = 0;
        crane[j].D_DOWN_ERR[f] = 0;
        crane[j].D_R_ERR[f] = 3000;
        crane[j].D_L_ERR[f] = 3000;
        crane[j].D_F_ERR[f] = 3000;
        crane[j].D_B_ERR[f] = 3000;
        #endregion
        #region КОРРЕКЦИЯ - ПОДЪЕМ - 1-1  - ДЕЛЬТА - ТРАВЕРСА
        f = 5; // ПОДЪЕМ
        //НАСТРОЕЧНЫЕ ПАРАМЕТРЫ КРАНА
        // ДЕЛЬТА ПРЕДУПРЕЖДЕНИЯ        
        crane[j].D_UP_WARN[f] = 2000;
        crane[j].D_DOWN_WARN[f] = 2000;
        crane[j].D_R_WARN[f] = 6000;
        crane[j].D_L_WARN[f] = 6000;
        crane[j].D_F_WARN[f] = 6000;
        crane[j].D_B_WARN[f] = 6000;
        // ДЕЛЬТА АВАРИИ
        crane[j].D_UP_ERR[f] = 0;
        crane[j].D_DOWN_ERR[f] = 0;
        crane[j].D_R_ERR[f] = 3000;
        crane[j].D_L_ERR[f] = 3000;
        crane[j].D_F_ERR[f] = 3000;
        crane[j].D_B_ERR[f] = 3000;
        #endregion
        #region КОРРЕКЦИЯ - ПОДЪЕМ - 1-1  - ДЕЛЬТА - груз
        f = 6; // ПОДЪЕМ
        //НАСТРОЕЧНЫЕ ПАРАМЕТРЫ КРАНА
        // ДЕЛЬТА ПРЕДУПРЕЖДЕНИЯ        
        crane[j].D_UP_WARN[f] = 2000;
        crane[j].D_DOWN_WARN[f] = 2000;
        crane[j].D_R_WARN[f] = 6000;
        crane[j].D_L_WARN[f] = 6000;
        crane[j].D_F_WARN[f] = 6000;
        crane[j].D_B_WARN[f] = 6000;
        // ДЕЛЬТА АВАРИИ
        crane[j].D_UP_ERR[f] = 0;
        crane[j].D_DOWN_ERR[f] = 0;
        crane[j].D_R_ERR[f] = 3000;
        crane[j].D_L_ERR[f] = 3000;
        crane[j].D_F_ERR[f] = 3000;
        crane[j].D_B_ERR[f] = 3000;
        #endregion
        //
        #region КОРРЕКЦИЯ - ПОДЪЕМ - 1-2  - ДЕЛЬТА - ТРОСС + КРЮК
        f = 7; // ПОДЪЕМ
        //НАСТРОЕЧНЫЕ ПАРАМЕТРЫ КРАНА
        // ДЕЛЬТА ПРЕДУПРЕЖДЕНИЯ        
        crane[j].D_UP_WARN[f] = 2000;
        crane[j].D_DOWN_WARN[f] = 2000;
        crane[j].D_R_WARN[f] = 6000;
        crane[j].D_L_WARN[f] = 6000;
        crane[j].D_F_WARN[f] = 6000;
        crane[j].D_B_WARN[f] = 6000;
        // ДЕЛЬТА АВАРИИ
        crane[j].D_UP_ERR[f] = 0;
        crane[j].D_DOWN_ERR[f] = 0;
        crane[j].D_R_ERR[f] = 3000;
        crane[j].D_L_ERR[f] = 3000;
        crane[j].D_F_ERR[f] = 3000;
        crane[j].D_B_ERR[f] = 3000;
        #endregion
        #region КОРРЕКЦИЯ - ПОДЪЕМ - 1-2  - ДЕЛЬТА - ТРАВЕРСА
        f = 9; // ПОДЪЕМ
        //НАСТРОЕЧНЫЕ ПАРАМЕТРЫ КРАНА
        // ДЕЛЬТА ПРЕДУПРЕЖДЕНИЯ        
        crane[j].D_UP_WARN[f] = 4000;
        crane[j].D_DOWN_WARN[f] = 4000;
        crane[j].D_R_WARN[f] = 6000;
        crane[j].D_L_WARN[f] = 6000;
        crane[j].D_F_WARN[f] = 6000;
        crane[j].D_B_WARN[f] = 6000;
        // ДЕЛЬТА АВАРИИ
        crane[j].D_UP_ERR[f] = 2000;
        crane[j].D_DOWN_ERR[f] = 2000;
        crane[j].D_R_ERR[f] = 3000;
        crane[j].D_L_ERR[f] = 3000;
        crane[j].D_F_ERR[f] = 3000;
        crane[j].D_B_ERR[f] = 3000;
        #endregion
        #region КОРРЕКЦИЯ - ПОДЪЕМ - 1-2  - ДЕЛЬТА - груз
        f = 10; // ПОДЪЕМ
        //НАСТРОЕЧНЫЕ ПАРАМЕТРЫ КРАНА
        // ДЕЛЬТА ПРЕДУПРЕЖДЕНИЯ        
        crane[j].D_UP_WARN[f] = 4000;
        crane[j].D_DOWN_WARN[f] = 4000;
        crane[j].D_R_WARN[f] = 6000;
        crane[j].D_L_WARN[f] = 6000;
        crane[j].D_F_WARN[f] = 6000;
        crane[j].D_B_WARN[f] = 6000;
        // ДЕЛЬТА АВАРИИ
        crane[j].D_UP_ERR[f] = 2000;
        crane[j].D_DOWN_ERR[f] = 2000;
        crane[j].D_R_ERR[f] = 3000;
        crane[j].D_L_ERR[f] = 3000;
        crane[j].D_F_ERR[f] = 3000;
        crane[j].D_B_ERR[f] = 3000;
        #endregion
        //
        #region КОРРЕКЦИЯ - ТЕЛЕЖКА - 2  - ДЕЛЬТА
        f = 12; // ТЕЛЕЖКА
        // НАСТРОЕЧНЫЕ ПАРАМЕТРЫ КРАНА
        // ДЕЛЬТА ПРЕДУПРЕЖДЕНИЯ        
        crane[j].D_UP_WARN[f] = 0;
        crane[j].D_DOWN_WARN[f] = 0;
        crane[j].D_R_WARN[f] = 6000;
        crane[j].D_L_WARN[f] = 6000;
        crane[j].D_F_WARN[f] = 6000;
        crane[j].D_B_WARN[f] = 6000;
        // ДЕЛЬТА АВАРИИ
        crane[j].D_UP_ERR[f] = 0;
        crane[j].D_DOWN_ERR[f] = 0;
        crane[j].D_R_ERR[f] = 3000;
        crane[j].D_L_ERR[f] = 3000;
        crane[j].D_F_ERR[f] = 3000;
        crane[j].D_B_ERR[f] = 3000;
        #endregion
        //
        #region КОРРЕКЦИЯ - ПОДЪЕМ - 2-1  - ДЕЛЬТА - ТРОСС + КРЮК
        f = 13; // ПОДЪЕМ
        // НАСТРОЕЧНЫЕ ПАРАМЕТРЫ КРАНА
        // ДЕЛЬТА ПРЕДУПРЕЖДЕНИЯ        
        crane[j].D_UP_WARN[f] = 2000;
        crane[j].D_DOWN_WARN[f] = 2000;
        crane[j].D_R_WARN[f] = 6000;
        crane[j].D_L_WARN[f] = 6000;
        crane[j].D_F_WARN[f] = 6000;
        crane[j].D_B_WARN[f] = 6000;
        // ДЕЛЬТА АВАРИИ
        crane[j].D_UP_ERR[f] = 0;
        crane[j].D_DOWN_ERR[f] = 0;
        crane[j].D_R_ERR[f] = 3000;
        crane[j].D_L_ERR[f] = 3000;
        crane[j].D_F_ERR[f] = 3000;
        crane[j].D_B_ERR[f] = 3000;
        #endregion
        #region КОРРЕКЦИЯ - ПОДЪЕМ - 2-1  - ДЕЛЬТА  - ТРАВЕРСА
        f = 15; // ПОДЪЕМ
        // НАСТРОЕЧНЫЕ ПАРАМЕТРЫ КРАНА
        // ДЕЛЬТА ПРЕДУПРЕЖДЕНИЯ        
        crane[j].D_UP_WARN[f] = 2000;
        crane[j].D_DOWN_WARN[f] = 2000;
        crane[j].D_R_WARN[f] = 6000;
        crane[j].D_L_WARN[f] = 6000;
        crane[j].D_F_WARN[f] = 6000;
        crane[j].D_B_WARN[f] = 6000;
        // ДЕЛЬТА АВАРИИ
        crane[j].D_UP_ERR[f] = 0;
        crane[j].D_DOWN_ERR[f] = 0;
        crane[j].D_R_ERR[f] = 3000;
        crane[j].D_L_ERR[f] = 3000;
        crane[j].D_F_ERR[f] = 3000;
        crane[j].D_B_ERR[f] = 3000;
        #endregion
        #region КОРРЕКЦИЯ - ПОДЪЕМ - 2-1  - ДЕЛЬТА - груз
        f = 16; // ПОДЪЕМ
        //НАСТРОЕЧНЫЕ ПАРАМЕТРЫ КРАНА
        // ДЕЛЬТА ПРЕДУПРЕЖДЕНИЯ        
        crane[j].D_UP_WARN[f] = 2000;
        crane[j].D_DOWN_WARN[f] = 2000;
        crane[j].D_R_WARN[f] = 6000;
        crane[j].D_L_WARN[f] = 6000;
        crane[j].D_F_WARN[f] = 6000;
        crane[j].D_B_WARN[f] = 6000;
        // ДЕЛЬТА АВАРИИ
        crane[j].D_UP_ERR[f] = 0;
        crane[j].D_DOWN_ERR[f] = 0;
        crane[j].D_R_ERR[f] = 3000;
        crane[j].D_L_ERR[f] = 3000;
        crane[j].D_F_ERR[f] = 3000;
        crane[j].D_B_ERR[f] = 3000;
        #endregion
        //
        #region КОРРЕКЦИЯ - ПОДЪЕМ - 2-2  - ДЕЛЬТА - ТРОСС + КРЮК
        f = 17; // ПОДЪЕМ
        // НАСТРОЕЧНЫЕ ПАРАМЕТРЫ КРАНА
        // ДЕЛЬТА ПРЕДУПРЕЖДЕНИЯ        
        crane[j].D_UP_WARN[f] = 2000;
        crane[j].D_DOWN_WARN[f] = 2000;
        crane[j].D_R_WARN[f] = 6000;
        crane[j].D_L_WARN[f] = 6000;
        crane[j].D_F_WARN[f] = 6000;
        crane[j].D_B_WARN[f] = 6000;
        // ДЕЛЬТА АВАРИИ
        crane[j].D_UP_ERR[f] = 0;
        crane[j].D_DOWN_ERR[f] = 0;
        crane[j].D_R_ERR[f] = 3000;
        crane[j].D_L_ERR[f] = 3000;
        crane[j].D_F_ERR[f] = 3000;
        crane[j].D_B_ERR[f] = 3000;
        #endregion
        #region КОРРЕКЦИЯ - ПОДЪЕМ - 2-2  - ДЕЛЬТА  - ТРАВЕРСА
        f = 19; // ПОДЪЕМ
        // НАСТРОЕЧНЫЕ ПАРАМЕТРЫ КРАНА
        // ДЕЛЬТА ПРЕДУПРЕЖДЕНИЯ        
        crane[j].D_UP_WARN[f] = 2000;
        crane[j].D_DOWN_WARN[f] = 2000;
        crane[j].D_R_WARN[f] = 6000;
        crane[j].D_L_WARN[f] = 6000;
        crane[j].D_F_WARN[f] = 6000;
        crane[j].D_B_WARN[f] = 6000;
        // ДЕЛЬТА АВАРИИ
        crane[j].D_UP_ERR[f] = 0;
        crane[j].D_DOWN_ERR[f] = 0;
        crane[j].D_R_ERR[f] = 3000;
        crane[j].D_L_ERR[f] = 3000;
        crane[j].D_F_ERR[f] = 3000;
        crane[j].D_B_ERR[f] = 3000;
        #endregion
        #region КОРРЕКЦИЯ - ПОДЪЕМ - 2-2  - ДЕЛЬТА - груз
        f = 20; // ПОДЪЕМ
        //НАСТРОЕЧНЫЕ ПАРАМЕТРЫ КРАНА
        // ДЕЛЬТА ПРЕДУПРЕЖДЕНИЯ        
        crane[j].D_UP_WARN[f] = 2000;
        crane[j].D_DOWN_WARN[f] = 2000;
        crane[j].D_R_WARN[f] = 6000;
        crane[j].D_L_WARN[f] = 6000;
        crane[j].D_F_WARN[f] = 6000;
        crane[j].D_B_WARN[f] = 6000;
        // ДЕЛЬТА АВАРИИ
        crane[j].D_UP_ERR[f] = 0;
        crane[j].D_DOWN_ERR[f] = 0;
        crane[j].D_R_ERR[f] = 3000;
        crane[j].D_L_ERR[f] = 3000;
        crane[j].D_F_ERR[f] = 3000;
        crane[j].D_B_ERR[f] = 3000;
        #endregion
        //
        #endregion
        // ======================================================================================     
        #endregion          
        // ======================================================================================
        #region ПР003 - ГАБАРИТЫ МОСТА И ЕГО СОСТАВНЫХ УЗЛОВ
        #region КРАН 320Т - 2020-08-18
        j = 1;
        // =============================================================================================
        // ГАБАРИТЫ МОСТА
        crane[j].static_BR_X = 15000;         // ГАБАРИ ПО ОТБОЙНИКАМ - 15000 ИЗ ЧЕРТЕЖА
        crane[j].static_BR_Y = 48000;         // МЕЖОСЕВОЙ РЕЛЬС НА ОТМЕТКЕ 37830 - ОСИ 4(6) И 5(7) 
        crane[j].static_BR_Z = 5400;          // 5354 - округляем до 5400 - габарит моста без тележек по чертежу
        // ТОЧКА ОТСЧЕТА - смещение относительное плана цеха
        crane[j].STP_static_BR_X = 0;         // принимаем нулевое смещение - далее расчетное по данным от ПЛК крана
        crane[j].STP_static_BR_Y = 0;         // принимаем нулевое смещение
        crane[j].STP_static_BR_Z = 38000;     // отметка установки рельса 37830 + 170 высота рельса => 38000
        // =============================================================================================

        // =============================================================================================
        // ГАБАРИТЫ ТЕЛЕЖКИ 1 - ГЛАВНАЯ 
        crane[j].static_TR_1_X = 13461;             // по чертежу 13461
        crane[j].static_TR_1_Y = 9780;              // по чертежу 8890 для симметрии на оба крана принимаем - 9780
        crane[j].static_TR_1_Z = 3783;              // по чертежу 3783
        // ТОЧКА ОТСЧЕТА - смещение относительно моста - отбойник моста
        crane[j].STP_static_TR_1_X = 771;           // расстояние по чертежу 2999.9854 от отбойника до оголовка по координате Х
                                                    // 2229 от оголовка до  крайней точки тележки по координате Х
                                                    // итого 770.9854 => 771
        crane[j].STP_static_TR_1_Y = 0;             // принимаем нулевое смещение - далее расчетное по данным от ПЛК крана
        crane[j].STP_static_TR_1_Z = 4480;          // по чертежу расстояние между оголовками рельсов моста и тележки - 4480 по оси Z
        // =============================================================================================

        // =============================================================================================

        // ГАБАРИТЫ ТЕЛЕЖКИ 2 - ВСПОМОГАТЕЛЬНАЯ 
        crane[j].static_TR_2_X = 7380;                  // по чертежу 7378.4703 => 7380
        crane[j].static_TR_2_Y = 6131;                  // по чертежу 6131
        crane[j].static_TR_2_Z = 2410;                  // по чертежу 2409,0258 от оголовка рельса => 2410
        // ТОЧКА ОТСЧЕТА - смещение относительно моста - отбойник моста
        crane[j].STP_static_TR_2_X = 3745;              // расстояние по чертежу 4099.9854 от отбойника до оголовка по координате Х
                                                        // 355 от оголовка до  крайней точки тележки по координате Х
                                                        // итого 3744.9854 => 3745
        crane[j].STP_static_TR_2_Y = 0;                 // принимаем нулевое смещение - далее расчетное по данным от ПЛК крана
        crane[j].STP_static_TR_2_Z = 2930;              // по чертежу расстояние между оголовками рельсов моста и тележки - 2930,1747 по оси Z => 2930
        // =============================================================================================

        // =============================================================================================
        // ГАБАРИТЫ ПОДЪЕМ 1-1 - ГЛАВНЫЙ 
        crane[j].static_H_1_1_X = 1500;                // приблизительно по крюу 1492,45 - => 1500
        crane[j].static_H_1_1_Y = 1500;                // приблизительно по крюу 1492,45 - => 1500
        crane[j].static_H_1_1_Z = 0;                   // принимаем нулевое значение - далее расчетное по данным от ПЛК крана
        // ТОЧКА ОТСЧЕТА - смещение относительно тележки 1
        crane[j].STP_static_H_1_1_X = -162;                // по чертежу => -162,2243 = -162
        crane[j].STP_static_H_1_1_Y = 4140;                // 4890,0897 - 750 - => 4140.0897 - 4140
        crane[j].STP_static_H_1_1_Z = 0;                   // принимаем нулевое - т.е. считаем нулевое когда крюк от уровня оголовка рельса


        // =============================================================================================
        // ГАБАРИТЫ ПОДЪЕМ 1-2 - ГЛАВНЫЙ 
        crane[j].static_H_1_2_X = 1500;                 // приблизительно по крюу 1492,45 - => 1500
        crane[j].static_H_1_2_Y = 1500;                 // приблизительно по крюу 1492,45 - => 1500
        crane[j].static_H_1_2_Z = 0;                    // принимаем нулевое значение - далее расчетное по данным от ПЛК крана
        // ТОЧКА ОТСЧЕТА - смещение относительно тележки 1
        crane[j].STP_static_H_1_2_X = 12128;           // по чертежу => 12290 -162,2243 = -162 - 12128
        crane[j].STP_static_H_1_2_Y = 4140;             // 4890,0897 - 750 - => 4140.0897 - 4140
        crane[j].STP_static_H_1_2_Z = 0;                // принимаем нулевое - т.е. считаем нулевое когда крюк от уровня оголовка рельса


        // =============================================================================================
        // ГАБАРИТЫ ПОДЪЕМ 2-1 - ВСПОМОГАТЕЛЬНЫЙ 
        crane[j].static_H_2_1_X = 1500;                 // приблизительно по крюку 1492,45 - => 1500   - от главных подъемов, а так малое расстояние 1166
        crane[j].static_H_2_1_Y = 1500;                 // приблизительно по крюку 1492,45 - => 1500   - от главных подъемов, а так малое расстояние 1166
        crane[j].static_H_2_1_Z = 0;                    // принимаем нулевое значение - далее расчетное по данным от ПЛК крана
        // ТОЧКА ОТСЧЕТА - смещение относительно тележки 2
        crane[j].STP_static_H_2_1_X = 2940;             // 3689,2352 - 750 - 2939,2352 - 2940
        crane[j].STP_static_H_2_1_Y = 1489;             // 2239 - 750 - 1489
        crane[j].STP_static_H_2_1_Z = 0;                // принимаем нулевое - т.е. считаем нулевое когда крюк от уровня оголовка рельса

        // =============================================================================================
        // ГАБАРИТЫ ПОДЪЕМ 2-2 - ВСПОМОГАТЕЛЬНЫЙ - на 320 кране его нет!!!!!!!!!!!!!!!!!!!!
        crane[j].static_H_2_2_X = 0;
        crane[j].static_H_2_2_Y = 0;
        crane[j].static_H_2_2_Z = 0;
        // ТОЧКА ОТСЧЕТА - смещение относительно тележки 2
        crane[j].STP_static_H_2_2_X = 0;
        crane[j].STP_static_H_2_2_Y = 0;
        crane[j].STP_static_H_2_2_Z = 0;

        #endregion
        #region КРАН 120Т - 2020-08-18
        j = 2;
        // =============================================================================================
        // ГАБАРИТЫ МОСТА
        crane[j].static_BR_X = 12000;       // по чертежу по отбойникам
        crane[j].static_BR_Y = 45000;       // по оголовкам рельса - // МЕЖОСЕВОЙ РЕЛЬС НА ОТМЕТКЕ 28830 - ОСИ 4(6) И 5(7)
        crane[j].static_BR_Z = 5710;        // 5710 - 2902,6617 от оголовка рельса до верхней точки / 2800 от оголовка до нижней кромки кабины крана
        // ТОЧКА ОТСЧЕТА - смещение относительное
        crane[j].STP_static_BR_X = 0;       // принимаем нулевое смещение - далее расчетное по данным от ПЛК крана
        crane[j].STP_static_BR_Y = 0;       // принимаем нулевое смещение
        crane[j].STP_static_BR_Z = 26200;   // 29000 - 2800 от оголовка до нижней кромки кабины крана
        // =============================================================================================


        // =============================================================================================
        // ГАБАРИТЫ ТЕЛЕЖКИ 1 - ГЛАВНАЯ 
        crane[j].static_TR_1_X = 11000;         // 
        crane[j].static_TR_1_Y = 6030;          // 6030 по отбойникам тележки
        crane[j].static_TR_1_Z = 5480;          // 2680,9707 от оголовка рельса
        // ТОЧКА ОТСЧЕТА - смещение относительно моста
        crane[j].STP_static_TR_1_X = 500;       //
        crane[j].STP_static_TR_1_Y = 0;         // принимаем нулевое смещение - далее расчетное по данным от ПЛК крана
        crane[j].STP_static_TR_1_Z = 4764;       // по чертежу расстояние между оголовками рельсов моста и тележки 1964 + 2800
        // =============================================================================================

        // =============================================================================================
        // ГАБАРИТЫ ТЕЛЕЖКИ 2 - ВСПОМОГАТЕЛЬНАЯ 
        crane[j].static_TR_2_X = 4560;
        crane[j].static_TR_2_Y = 6000;
        crane[j].static_TR_2_Z = 1725;
        // ТОЧКА ОТСЧЕТА - смещение относительно моста
        crane[j].STP_static_TR_2_X = 3502;
        crane[j].STP_static_TR_2_Y = 0;         // принимаем нулевое смещение - далее расчетное по данным от ПЛК крана
        crane[j].STP_static_TR_2_Z = 1761;
        // =============================================================================================

        // =============================================================================================
        // ГАБАРИТЫ ПОДЪЕМ 1-1 - ГЛАВНЫЙ 
        crane[j].static_H_1_1_X = 700;
        crane[j].static_H_1_1_Y = 700;
        crane[j].static_H_1_1_Z = 0;                 // принимаем нулевое значение - далее расчетное по данным от ПЛК крана
        // ТОЧКА ОТСЧЕТА - смещение относительно тележки 1
        crane[j].STP_static_H_1_1_X = 0;                   // 
        crane[j].STP_static_H_1_1_Y = 2665;                   // 6030/2 - 350
        crane[j].STP_static_H_1_1_Z = 0;                   // принимаем нулевое - т.е. считаем нулевое когда крюк от уровня оголовка рельса


        // =============================================================================================
        // ГАБАРИТЫ ПОДЪЕМ 1-2 - ГЛАВНЫЙ 
        crane[j].static_H_1_2_X = 700;
        crane[j].static_H_1_2_Y = 700;
        crane[j].static_H_1_2_Z = 0;                 // принимаем нулевое значение - далее расчетное по данным от ПЛК крана
        // ТОЧКА ОТСЧЕТА - смещение относительно тележки 1
        crane[j].STP_static_H_1_2_X = 10400;
        crane[j].STP_static_H_1_2_Y = 2665;
        crane[j].STP_static_H_1_2_Z = 0;                   // принимаем нулевое - т.е. считаем нулевое когда крюк от уровня оголовка рельса


        // =============================================================================================
        // ГАБАРИТЫ ПОДЪЕМ 2-1 - ВСПОМОГАТЕЛЬНЫЙ 
        crane[j].static_H_2_1_X = 700;
        crane[j].static_H_2_1_Y = 700;
        crane[j].static_H_2_1_Z = 0;                 // принимаем нулевое значение - далее расчетное по данным от ПЛК крана
        // ТОЧКА ОТСЧЕТА - смещение относительно тележки 2
        crane[j].STP_static_H_2_1_X = 1975;                // 4560/2 - 350
        crane[j].STP_static_H_2_1_Y = 2754;
        crane[j].STP_static_H_2_1_Z = 0;                   // принимаем нулевое - т.е. считаем нулевое когда крюк от уровня оголовка рельса

        // =============================================================================================
        // ГАБАРИТЫ ПОДЪЕМ 2-2 - ВСПОМОГАТЕЛЬНЫЙ 
        crane[j].static_H_2_2_X = 500;
        crane[j].static_H_2_2_Y = 500;
        crane[j].static_H_2_2_Z = 0;                 // принимаем нулевое значение - далее расчетное по данным от ПЛК крана
        // ТОЧКА ОТСЧЕТА - смещение относительно тележки 2
        crane[j].STP_static_H_2_2_X = 1975;                // 4560/2 - 350
        crane[j].STP_static_H_2_2_Y = 4100;
        crane[j].STP_static_H_2_2_Z = 0;                   // принимаем нулевое - т.е. считаем нулевое когда крюк от уровня оголовка рельса

        #endregion
        #region КРАН  25Т - 2020-08-18
        j = 3;
        // =============================================================================================
        // ГАБАРИТЫ МОСТА
        crane[j].static_BR_X = 8497;
        crane[j].static_BR_Y = 45000;       // по оголовкам рельса - // МЕЖОСЕВОЙ РЕЛЬС НА ОТМЕТКЕ 28830 - ОСИ 4(6) И 5(7)
        crane[j].static_BR_Z = 3718;
        // ТОЧКА ОТСЧЕТА - смещение относительное
        crane[j].STP_static_BR_X = 0;       // принимаем нулевое смещение - далее расчетное по данным от ПЛК крана
        crane[j].STP_static_BR_Y = 0;       // принимаем нулевое смещение
        crane[j].STP_static_BR_Z = 29000;
        // =============================================================================================


        // =============================================================================================
        // ГАБАРИТЫ ТЕЛЕЖКИ 1 - ГЛАВНАЯ - так как данных нет принимаем что тележка квадратная
        crane[j].static_TR_1_X = 4145;
        crane[j].static_TR_1_Y = 4145;
        crane[j].static_TR_1_Z = 1840;
        // ТОЧКА ОТСЧЕТА - смещение относительно моста
        crane[j].STP_static_TR_1_X = 1780;
        crane[j].STP_static_TR_1_Y = 0;
        crane[j].STP_static_TR_1_Z = 1670;
        // =============================================================================================

        // =============================================================================================
        // ГАБАРИТЫ ТЕЛЕЖКИ 2 - ВСПОМОГАТЕЛЬНАЯ - нет!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        crane[j].static_TR_2_X = 0;
        crane[j].static_TR_2_Y = 0;
        crane[j].static_TR_2_Z = 0;
        // ТОЧКА ОТСЧЕТА - смещение относительно моста
        crane[j].STP_static_TR_2_X = 0;
        crane[j].STP_static_TR_2_Y = 0;
        crane[j].STP_static_TR_2_Z = 0;
        // =============================================================================================

        // =============================================================================================
        // ГАБАРИТЫ ПОДЪЕМ 1-1 - ГЛАВНЫЙ 
        crane[j].static_H_1_1_X = 500;
        crane[j].static_H_1_1_Y = 500;
        crane[j].static_H_1_1_Z = 0;                 // принимаем нулевое значение - далее расчетное по данным от ПЛК крана
        // ТОЧКА ОТСЧЕТА - смещение относительно тележки 1
        crane[j].STP_static_H_1_1_X = 1825;
        crane[j].STP_static_H_1_1_Y = 0;
        crane[j].STP_static_H_1_1_Z = 0;                   // принимаем нулевое - т.е. считаем нулевое когда крюк от уровня оголовка рельса 


        // =============================================================================================
        // ГАБАРИТЫ ПОДЪЕМ 1-2 - ГЛАВНЫЙ 
        crane[j].static_H_1_2_X = 500;
        crane[j].static_H_1_2_Y = 500;
        crane[j].static_H_1_2_Z = 0;                 // принимаем нулевое значение - далее расчетное по данным от ПЛК крана
        // ТОЧКА ОТСЧЕТА - смещение относительно тележки 1
        crane[j].STP_static_H_1_2_X = 1825;
        crane[j].STP_static_H_1_2_Y = 3895;
        crane[j].STP_static_H_1_2_Z = 0;                   // принимаем нулевое - т.е. считаем нулевое когда крюк от уровня оголовка рельса


        // =============================================================================================
        // ГАБАРИТЫ ПОДЪЕМ 2-1 - ВСПОМОГАТЕЛЬНЫЙ  - нет!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        crane[j].static_H_2_1_X = 0;
        crane[j].static_H_2_1_Y = 0;
        crane[j].static_H_2_1_Z = 0;
        // ТОЧКА ОТСЧЕТА - смещение относительно тележки 2
        crane[j].STP_static_H_2_1_X = 0;
        crane[j].STP_static_H_2_1_Y = 0;
        crane[j].STP_static_H_2_1_Z = 0;

        // =============================================================================================
        // ГАБАРИТЫ ПОДЪЕМ 2-2 - ВСПОМОГАТЕЛЬНЫЙ  - нет!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        crane[j].static_H_2_2_X = 0;
        crane[j].static_H_2_2_Y = 0;
        crane[j].static_H_2_2_Z = 0;
        // ТОЧКА ОТСЧЕТА - смещение относительно тележки 2
        crane[j].STP_static_H_2_2_X = 0;
        crane[j].STP_static_H_2_2_Y = 0;
        crane[j].STP_static_H_2_2_Z = 0;

        #endregion
        j = 0;
        #endregion     
        // ======================================================================================
        #region ПР301 - ФОРМИРОВАНИЕ ЭЛЕМЕНТОВ КРАНА/ОВ
        // ===========================================================
        #region Временные переменные для сравнения 3 чисел
        int temp_compare_1el = 0;
        int temp_compare_2el = 0;
        int temp_compare_3el = 0;
        bool temp_compare_OK_write = false;
        #endregion
        // ===========================================================
        for (j = 1; j <= 3; j++)
        {
            // ===========================================================    
            #region КРАН j => 1- 320t/ 2-120t/ 3-25t - МОСТ
            // XYZ - ШИРИНА/ДЛИНА/ВЫСОТА МОСТА 
            crane[j].Obj_x[1] = crane[j].static_BR_X;
            crane[j].Obj_y[1] = crane[j].static_BR_Y;
            crane[j].Obj_z[1] = crane[j].static_BR_Z;
            // XYZ - STP МОСТА
            crane[j].stp_x[1] = crane[j].BRIDGE_X;
            crane[j].stp_y[1] = crane[j].STP_static_BR_Y;
            crane[j].stp_z[1] = crane[j].STP_static_BR_Z;
            //
            #endregion
            #region КРАН j => 1- 320t/ 2-120t/ 3-25t - ТЕЛЕЖКА 1
            // XYZ - ШИРИНА/ДЛИНА/ВЫСОТА ТЕЛЕЖКИ
            crane[j].Obj_x[2] = crane[j].static_TR_1_X;
            crane[j].Obj_y[2] = crane[j].static_TR_1_Y;
            crane[j].Obj_z[2] = crane[j].static_TR_1_Z;
            // XYZ - STP ТЕЛЕЖКИ
            crane[j].stp_x[2] = crane[j].stp_x[1] + crane[j].STP_static_TR_1_X;
            crane[j].stp_y[2] = crane[j].stp_y[1] + crane[j].STP_static_TR_1_Y + crane[j].trolley_1_Y;
            crane[j].stp_z[2] = crane[j].stp_z[1] + crane[j].STP_static_TR_1_Z;
            //
            #endregion
            #region КРАН j => 1- 320t/ 2-120t/ 3-25t - ТЕЛЕЖКА 2
            // XYZ - ШИРИНА/ДЛИНА/ВЫСОТА ТЕЛЕЖКИ 
            crane[j].Obj_x[12] = crane[j].static_TR_2_X;
            crane[j].Obj_y[12] = crane[j].static_TR_2_Y;
            crane[j].Obj_z[12] = crane[j].static_TR_2_Z;
            // XYZ - STP ТЕЛЕЖКИ
            crane[j].stp_x[12] = crane[j].stp_x[1] + crane[j].STP_static_TR_2_X;
            crane[j].stp_y[12] = crane[j].stp_y[1] + crane[j].STP_static_TR_2_Y + crane[j].trolley_2_Y;
            crane[j].stp_z[12] = crane[j].stp_z[1] + crane[j].STP_static_TR_2_Z;
            //
            #endregion
            #region КРАН j => 1- 320t/ 2-120t/ 3-25t - подъем 1 - 1 - тросс + КРЮК
            // XYZ - ШИРИНА/ДЛИНА/ВЫСОТА подъем - тросс + КРЮК
            crane[j].Obj_x[3] = crane[j].static_H_1_1_X;
            crane[j].Obj_y[3] = crane[j].static_H_1_1_Y;
            crane[j].Obj_z[3] = crane[j].stp_z[2] + crane[j].static_H_1_1_Z - crane[j].HOIST_1_1_Z;
            // XYZ - STP подъем - тросс + КРЮК
            crane[j].stp_x[3] = crane[j].stp_x[2] + crane[j].STP_static_H_1_1_X;
            crane[j].stp_y[3] = crane[j].stp_y[2] + crane[j].STP_static_H_1_1_Y;
            crane[j].stp_z[3] = crane[j].HOIST_1_1_Z;
            #endregion
            #region КРАН j => 1- 320t/ 2-120t/ 3-25t - подъем 1 - 2 - тросс + КРЮК
            // XYZ - ШИРИНА/ДЛИНА/ВЫСОТА подъем - тросс + КРЮК
            crane[j].Obj_x[7] = crane[j].static_H_1_2_X;
            crane[j].Obj_y[7] = crane[j].static_H_1_2_Y;
            crane[j].Obj_z[7] = crane[j].stp_z[2] + crane[j].static_H_1_2_Z - crane[j].HOIST_1_2_Z;
            // XYZ - STP подъем - тросс + КРЮК
            crane[j].stp_x[7] = crane[j].stp_x[2] + crane[j].STP_static_H_1_2_X;
            crane[j].stp_y[7] = crane[j].stp_y[2] + crane[j].STP_static_H_1_2_Y;
            crane[j].stp_z[7] = crane[j].HOIST_1_2_Z;
            #endregion
            #region КРАН j => 1- 320t/ 2-120t/ 3-25t - подъем 2 - 1 - тросс + КРЮК
            // XYZ - ШИРИНА/ДЛИНА/ВЫСОТА подъем - тросс + КРЮК
            crane[j].Obj_x[13] = crane[j].static_H_2_1_X;
            crane[j].Obj_y[13] = crane[j].static_H_2_1_Y;
            crane[j].Obj_z[13] = crane[j].stp_z[12] + crane[j].static_H_2_1_Z - crane[j].HOIST_2_1_Z;
            // XYZ - STP подъем - тросс + КРЮК
            crane[j].stp_x[13] = crane[j].stp_x[12] + crane[j].STP_static_H_2_1_X;
            crane[j].stp_y[13] = crane[j].stp_y[12] + crane[j].STP_static_H_2_1_Y;
            crane[j].stp_z[13] = crane[j].HOIST_2_1_Z;
            #endregion
            #region КРАН j => 1- 320t/ 2-120t/ 3-25t - подъем 2 - 2 - тросс + КРЮК
            // XYZ - ШИРИНА/ДЛИНА/ВЫСОТА подъем - тросс + КРЮК
            crane[j].Obj_x[17] = crane[j].static_H_2_2_X;
            crane[j].Obj_y[17] = crane[j].static_H_2_2_Y;
            crane[j].Obj_z[17] = crane[j].stp_z[12] + crane[j].static_H_2_2_Z - crane[j].HOIST_2_2_Z;
            // XYZ - STP подъем - тросс + КРЮК
            crane[j].stp_x[17] = crane[j].stp_x[12] + crane[j].STP_static_H_2_2_X;
            crane[j].stp_y[17] = crane[j].stp_y[12] + crane[j].STP_static_H_2_2_Y;
            crane[j].stp_z[17] = crane[j].HOIST_2_2_Z;
            #endregion
            // ===========================================================        
            #region ТРАВЕРСЫ
            // 0 or default - подъемы независимы
            // 1 - траверс и груза нет
            // 2 - 1 + 2 вместе
            // 4 - 1 + 2 + 3 вместе
            switch (crane[j].regim_w_hoist)
            {
                case 0: // ok
                    #region КРАН j => 1- 320t/ 2-120t/ 3-25t - подъем 1 - 1 - ТРАВЕРСА независимый подъем
                    if (crane[j].static_H11_TR_X > 0 & crane[j].static_H11_TR_Y > 0 & crane[j].static_H11_TR_Z > 0)
                    {
                        // XYZ - ШИРИНА/ДЛИНА/ВЫСОТА ТРАВЕРСЫ 
                        crane[j].Obj_x[5] = crane[j].static_H11_TR_X;
                        crane[j].Obj_y[5] = crane[j].static_H11_TR_Y;
                        crane[j].Obj_z[5] = crane[j].static_H11_TR_Z;
                        // XYZ - STP ТРАВЕРСЫ
                        crane[j].stp_x[5] = crane[j].stp_x[3] + crane[j].static_H_1_1_X / 2 - crane[j].static_H11_TR_X / 2;
                        crane[j].stp_y[5] = crane[j].stp_y[3] + crane[j].static_H_1_1_Y / 2 - crane[j].static_H11_TR_Y / 2;
                        crane[j].stp_z[5] = crane[j].stp_z[3] - crane[j].static_H11_TR_Z;
                        //
                        // XYZ - STP для привязки груза в зависимости от наличия или отсутствия траверся
                        crane[j].stp_x[4] = crane[j].stp_x[3] + crane[j].static_H_1_1_X / 2;
                        crane[j].stp_y[4] = crane[j].stp_y[3] + crane[j].static_H_1_1_Y / 2;
                        crane[j].stp_z[4] = crane[j].stp_z[3] - crane[j].static_H11_TR_Z;
                        //
                    }
                    else
                    {
                        crane[j].Obj_x[5] = 0;
                        crane[j].Obj_y[5] = 0;
                        crane[j].Obj_z[5] = 0;
                        crane[j].stp_x[5] = 0;
                        crane[j].stp_y[5] = 0;
                        crane[j].stp_z[5] = 0;
                        // XYZ - STP для привязки груза в зависимости от наличия или отсутствия траверся
                        crane[j].stp_x[4] = crane[j].stp_x[3] + crane[j].static_H_1_1_X / 2;
                        crane[j].stp_y[4] = crane[j].stp_y[3] + crane[j].static_H_1_1_Y / 2;
                        crane[j].stp_z[4] = crane[j].stp_z[3];
                        //


                    }
                    #endregion
                    #region КРАН j => 1- 320t/ 2-120t/ 3-25t - подъем 1 - 2 - ТРАВЕРСА независимый подъем
                    if (crane[j].static_H12_TR_X > 0 & crane[j].static_H12_TR_Y > 0 & crane[j].static_H12_TR_Z > 0)
                    {
                        // XYZ - ШИРИНА/ДЛИНА/ВЫСОТА ТРАВЕРСЫ 
                        crane[j].Obj_x[9] = crane[j].static_H12_TR_X;
                        crane[j].Obj_y[9] = crane[j].static_H12_TR_Y;
                        crane[j].Obj_z[9] = crane[j].static_H12_TR_Z;
                        // XYZ - STP ТРАВЕРСЫ
                        crane[j].stp_x[9] = crane[j].stp_x[7] + crane[j].static_H_1_2_X / 2 - crane[j].static_H12_TR_X / 2;
                        crane[j].stp_y[9] = crane[j].stp_y[7] + crane[j].static_H_1_2_Y / 2 - crane[j].static_H12_TR_Y / 2;
                        crane[j].stp_z[9] = crane[j].stp_z[7] - crane[j].static_H12_TR_Z;
                        //
                        // XYZ - STP для привязки груза в зависимости от наличия или отсутствия траверся
                        crane[j].stp_x[8] = crane[j].stp_x[7] + crane[j].static_H_1_2_X / 2;
                        crane[j].stp_y[8] = crane[j].stp_y[7] + crane[j].static_H_1_2_Y / 2;
                        crane[j].stp_z[8] = crane[j].stp_z[7] - crane[j].static_H12_TR_Z;
                        //
                    }
                    else
                    {
                        crane[j].Obj_x[9] = 0;
                        crane[j].Obj_y[9] = 0;
                        crane[j].Obj_z[9] = 0;
                        crane[j].stp_x[9] = 0;
                        crane[j].stp_y[9] = 0;
                        crane[j].stp_z[9] = 0;
                        // XYZ - STP для привязки груза в зависимости от наличия или отсутствия траверся
                        crane[j].stp_x[8] = crane[j].stp_x[7] + crane[j].static_H_1_2_X / 2;
                        crane[j].stp_y[8] = crane[j].stp_y[7] + crane[j].static_H_1_2_Y / 2;
                        crane[j].stp_z[8] = crane[j].stp_z[7];
                        //

                    }
                    #endregion
                    #region КРАН j => 1- 320t/ 2-120t/ 3-25t - подъем 2 - 1 - ТРАВЕРСА независимый подъем
                    if (crane[j].static_H21_TR_X > 0 & crane[j].static_H21_TR_Y > 0 & crane[j].static_H21_TR_Z > 0)
                    {
                        // XYZ - ШИРИНА/ДЛИНА/ВЫСОТА ТРАВЕРСЫ 
                        crane[j].Obj_x[15] = crane[j].static_H21_TR_X;
                        crane[j].Obj_y[15] = crane[j].static_H21_TR_Y;
                        crane[j].Obj_z[15] = crane[j].static_H21_TR_Z;
                        // XYZ - STP ТРАВЕРСЫ
                        crane[j].stp_x[15] = crane[j].stp_x[13] + crane[j].static_H_2_1_X / 2 - crane[j].static_H21_TR_X / 2;
                        crane[j].stp_y[15] = crane[j].stp_y[13] + crane[j].static_H_2_1_Y / 2 - crane[j].static_H21_TR_Y / 2;
                        crane[j].stp_z[15] = crane[j].stp_z[13] - crane[j].static_H21_TR_Z;
                        //
                        // XYZ - STP для привязки груза в зависимости от наличия или отсутствия траверся
                        crane[j].stp_x[14] = crane[j].stp_x[13] + crane[j].static_H_2_1_X / 2;
                        crane[j].stp_y[14] = crane[j].stp_y[13] + crane[j].static_H_2_1_Y / 2;
                        crane[j].stp_z[14] = crane[j].stp_z[13] - crane[j].static_H21_TR_Z;
                        //
                    }
                    else
                    {
                        crane[j].Obj_x[15] = 0;
                        crane[j].Obj_y[15] = 0;
                        crane[j].Obj_z[15] = 0;
                        crane[j].stp_x[15] = 0;
                        crane[j].stp_y[15] = 0;
                        crane[j].stp_z[15] = 0;
                        // XYZ - STP для привязки груза в зависимости от наличия или отсутствия траверся
                        crane[j].stp_x[14] = crane[j].stp_x[13] + crane[j].static_H_2_1_X / 2;
                        crane[j].stp_y[14] = crane[j].stp_y[13] + crane[j].static_H_2_1_Y / 2;
                        crane[j].stp_z[14] = crane[j].stp_z[13];
                        //

                    }
                    #endregion
                    #region КРАН j => 1- 320t/ 2-120t/ 3-25t - подъем 2 - 2 - ТРАВЕРСА независимый подъем
                    if (crane[j].static_H22_TR_X > 0 & crane[j].static_H22_TR_Y > 0 & crane[j].static_H22_TR_Z > 0)
                    {
                        // XYZ - ШИРИНА/ДЛИНА/ВЫСОТА ТРАВЕРСЫ 
                        crane[j].Obj_x[19] = crane[j].static_H22_TR_X;
                        crane[j].Obj_y[19] = crane[j].static_H22_TR_Y;
                        crane[j].Obj_z[19] = crane[j].static_H22_TR_Z;
                        // XYZ - STP ТРАВЕРСЫ
                        crane[j].stp_x[19] = crane[j].stp_x[17] + crane[j].static_H_2_2_X / 2 - crane[j].static_H22_TR_X / 2;
                        crane[j].stp_y[19] = crane[j].stp_y[17] + crane[j].static_H_2_2_Y / 2 - crane[j].static_H22_TR_Y / 2;
                        crane[j].stp_z[19] = crane[j].stp_z[17] - crane[j].static_H22_TR_Z;
                        //
                        // XYZ - STP для привязки груза в зависимости от наличия или отсутствия траверся
                        crane[j].stp_x[18] = crane[j].stp_x[17] + crane[j].static_H_2_2_X / 2;
                        crane[j].stp_y[18] = crane[j].stp_y[17] + crane[j].static_H_2_2_Y / 2;
                        crane[j].stp_z[18] = crane[j].stp_z[17] - crane[j].static_H22_TR_Z;
                        //
                    }
                    else
                    {
                        crane[j].Obj_x[19] = 0;
                        crane[j].Obj_y[19] = 0;
                        crane[j].Obj_z[19] = 0;
                        crane[j].stp_x[19] = 0;
                        crane[j].stp_y[19] = 0;
                        crane[j].stp_z[19] = 0;
                        // XYZ - STP для привязки груза в зависимости от наличия или отсутствия траверся
                        crane[j].stp_x[18] = crane[j].stp_x[17] + crane[j].static_H_2_2_X / 2;
                        crane[j].stp_y[18] = crane[j].stp_y[17] + crane[j].static_H_2_2_Y / 2;
                        crane[j].stp_z[18] = crane[j].stp_z[17];
                        //

                    }
                    #endregion
                    break;
                case 1: // ok
                    #region КРАН j => 1- 320t/ 2-120t/ 3-25t - подъем 1 - 1 - - ТРАВЕРСА = 0

                    crane[j].Obj_x[5] = 0;
                    crane[j].Obj_y[5] = 0;
                    crane[j].Obj_z[5] = 0;
                    crane[j].stp_x[5] = 0;
                    crane[j].stp_y[5] = 0;
                    crane[j].stp_z[5] = 0;

                    // XYZ - STP для привязки груза в зависимости от наличия или отсутствия траверся
                    crane[j].stp_x[4] = crane[j].stp_x[3] + crane[j].static_H_1_1_X / 2;
                    crane[j].stp_y[4] = crane[j].stp_y[3] + crane[j].static_H_1_1_Y / 2;
                    crane[j].stp_z[4] = crane[j].stp_z[3];


                    #endregion
                    #region КРАН j => 1- 320t/ 2-120t/ 3-25t - подъем 1 - 2 - - ТРАВЕРСА = 0

                    crane[j].Obj_x[9] = 0;
                    crane[j].Obj_y[9] = 0;
                    crane[j].Obj_z[9] = 0;
                    crane[j].stp_x[9] = 0;
                    crane[j].stp_y[9] = 0;
                    crane[j].stp_z[9] = 0;
                    // XYZ - STP для привязки груза в зависимости от наличия или отсутствия траверся
                    crane[j].stp_x[8] = crane[j].stp_x[7] + crane[j].static_H_1_2_X / 2;
                    crane[j].stp_y[8] = crane[j].stp_y[7] + crane[j].static_H_1_2_Y / 2;
                    crane[j].stp_z[8] = crane[j].stp_z[7];

                    #endregion
                    #region КРАН j => 1- 320t/ 2-120t/ 3-25t - подъем 2 - 1 - - ТРАВЕРСА = 0

                    crane[j].Obj_x[15] = 0;
                    crane[j].Obj_y[15] = 0;
                    crane[j].Obj_z[15] = 0;
                    crane[j].stp_x[15] = 0;
                    crane[j].stp_y[15] = 0;
                    crane[j].stp_z[15] = 0;
                    // XYZ - STP для привязки груза в зависимости от наличия или отсутствия траверся
                    crane[j].stp_x[14] = crane[j].stp_x[13] + crane[j].static_H_2_1_X / 2;
                    crane[j].stp_y[14] = crane[j].stp_y[13] + crane[j].static_H_2_1_Y / 2;
                    crane[j].stp_z[14] = crane[j].stp_z[13];

                    #endregion
                    #region КРАН j => 1- 320t/ 2-120t/ 3-25t - подъем 2 - 2 - - ТРАВЕРСА = 0

                    crane[j].Obj_x[19] = 0;
                    crane[j].Obj_y[19] = 0;
                    crane[j].Obj_z[19] = 0;
                    crane[j].stp_x[19] = 0;
                    crane[j].stp_y[19] = 0;
                    crane[j].stp_z[19] = 0;
                    // XYZ - STP для привязки груза в зависимости от наличия или отсутствия траверся
                    crane[j].stp_x[18] = crane[j].stp_x[17] + crane[j].static_H_2_2_X / 2;
                    crane[j].stp_y[18] = crane[j].stp_y[17] + crane[j].static_H_2_2_Y / 2;
                    crane[j].stp_z[18] = crane[j].stp_z[17];

                    #endregion
                    break;
                case 2: // ok    
                    #region КРАН j => 1- 320t/ 2-120t/ 3-25t - подъем 1 - 2 - ТРАВЕРСА независимый подъем
                    if (crane[j].static_H12_TR_X > 0 & crane[j].static_H12_TR_Y > 0 & crane[j].static_H12_TR_Z > 0)
                    {
                        // XYZ - ШИРИНА/ДЛИНА/ВЫСОТА ТРАВЕРСЫ 
                        crane[j].Obj_x[9] = crane[j].static_H12_TR_X;
                        crane[j].Obj_y[9] = crane[j].static_H12_TR_Y;
                        crane[j].Obj_z[9] = crane[j].static_H12_TR_Z;
                        // XYZ - STP ТРАВЕРСЫ
                        crane[j].stp_x[9] = crane[j].stp_x[7] + crane[j].static_H_1_2_X / 2 - crane[j].static_H12_TR_X / 2;
                        crane[j].stp_y[9] = crane[j].stp_y[7] + crane[j].static_H_1_2_Y / 2 - crane[j].static_H12_TR_Y / 2;
                        crane[j].stp_z[9] = crane[j].stp_z[7] - crane[j].static_H12_TR_Z;
                        //
                        // XYZ - STP для привязки груза в зависимости от наличия или отсутствия траверся
                        crane[j].stp_x[8] = crane[j].stp_x[7] + crane[j].static_H_1_2_X / 2;
                        crane[j].stp_y[8] = crane[j].stp_y[7] + crane[j].static_H_1_2_Y / 2;
                        crane[j].stp_z[8] = crane[j].stp_z[7] - crane[j].static_H12_TR_Z;
                        //
                    }
                    else
                    {
                        crane[j].Obj_x[9] = 0;
                        crane[j].Obj_y[9] = 0;
                        crane[j].Obj_z[9] = 0;
                        crane[j].stp_x[9] = 0;
                        crane[j].stp_y[9] = 0;
                        crane[j].stp_z[9] = 0;
                        // XYZ - STP для привязки груза в зависимости от наличия или отсутствия траверся
                        crane[j].stp_x[8] = crane[j].stp_x[7] + crane[j].static_H_1_2_X / 2;
                        crane[j].stp_y[8] = crane[j].stp_y[7] + crane[j].static_H_1_2_Y / 2;
                        crane[j].stp_z[8] = crane[j].stp_z[7];
                        //

                    }
                    #endregion
                    #region КРАН j => 1- 320t/ 2-120t/ 3-25t - подъем 1-1/ 1-2 - ТРАВЕРСА на два или один подъем
                    if (crane[j].static_H11_TR_X > 0 & crane[j].static_H11_TR_Y > 0 & crane[j].static_H11_TR_Z > 0)
                    {
                        #region ТРАВЕРСА НА ОДИН ПОДЪЕМ
                        if (!(crane[j].static_H11_TR_twohook_y_n))
                        {
                            #region КРАН j => 1- 320t/ 2-120t/ 3-25t - подъем 1 - 1 - ТРАВЕРСА независимый подъем
                            if (crane[j].static_H11_TR_X > 0 & crane[j].static_H11_TR_Y > 0 & crane[j].static_H11_TR_Z > 0)
                            {
                                // XYZ - ШИРИНА/ДЛИНА/ВЫСОТА ТРАВЕРСЫ 
                                crane[j].Obj_x[5] = crane[j].static_H11_TR_X;
                                crane[j].Obj_y[5] = crane[j].static_H11_TR_Y;
                                crane[j].Obj_z[5] = crane[j].static_H11_TR_Z;
                                // XYZ - STP ТРАВЕРСЫ
                                crane[j].stp_x[5] = crane[j].stp_x[3] + crane[j].static_H_1_1_X / 2 - crane[j].static_H11_TR_X / 2;
                                crane[j].stp_y[5] = crane[j].stp_y[3] + crane[j].static_H_1_1_Y / 2 - crane[j].static_H11_TR_Y / 2;
                                crane[j].stp_z[5] = crane[j].stp_z[3] - crane[j].static_H11_TR_Z;
                                //
                                // XYZ - STP для привязки груза в зависимости от наличия или отсутствия траверся
                                crane[j].stp_x[4] = crane[j].stp_x[3] + crane[j].static_H_1_1_X / 2;
                                crane[j].stp_y[4] = crane[j].stp_y[3] + crane[j].static_H_1_1_Y / 2;
                                crane[j].stp_z[4] = crane[j].stp_z[3] - crane[j].static_H11_TR_Z;
                                //
                            }
                            else
                            {
                                crane[j].Obj_x[5] = 0;
                                crane[j].Obj_y[5] = 0;
                                crane[j].Obj_z[5] = 0;
                                crane[j].stp_x[5] = 0;
                                crane[j].stp_y[5] = 0;
                                crane[j].stp_z[5] = 0;
                                // XYZ - STP для привязки груза в зависимости от наличия или отсутствия траверся
                                crane[j].stp_x[4] = crane[j].stp_x[3] + crane[j].static_H_1_1_X / 2;
                                crane[j].stp_y[4] = crane[j].stp_y[3] + crane[j].static_H_1_1_Y / 2;
                                crane[j].stp_z[4] = crane[j].stp_z[3];
                                //
                            }
                            #endregion
                        }
                        #endregion
                        #region ТРАВЕРСА НА ДВА ПОДЪЕМА
                        if (crane[j].static_H11_TR_twohook_y_n)
                        {
                            // XYZ - ШИРИНА/ДЛИНА/ВЫСОТА ТРАВЕРСЫ 
                            crane[j].Obj_x[5] = crane[j].static_H11_TR_X;
                            crane[j].Obj_y[5] = crane[j].static_H11_TR_Y;
                            crane[j].Obj_z[5] = crane[j].static_H11_TR_Z;
                            // XYZ - STP ТРАВЕРСЫ
                            crane[j].stp_x[5] = ((crane[j].stp_x[3] + (((crane[j].stp_x[3] + crane[j].static_H_1_1_X / 2) - (crane[j].stp_x[7] + crane[j].static_H_1_2_X / 2)) / 2)) - (crane[j].static_H11_TR_X / 2));
                            crane[j].stp_y[5] = crane[j].stp_y[3] + crane[j].static_H_1_1_Y / 2 - crane[j].static_H11_TR_Y / 2;
                            // Z - STP ТРАВЕРСЫ
                            if (crane[j].stp_z[3] >= crane[j].stp_z[7])
                            {
                                crane[j].stp_z[5] = crane[j].stp_z[7] - crane[j].static_H11_TR_Z;
                            }
                            else
                            {
                                crane[j].stp_z[5] = crane[j].stp_z[3] - crane[j].static_H11_TR_Z;
                            }
                            //
                            // XYZ - STP для привязки груза в зависимости от наличия или отсутствия траверсы
                            crane[j].stp_x[4] = crane[j].stp_x[5] + crane[j].static_H11_TR_X / 2;
                            crane[j].stp_y[4] = crane[j].stp_y[5] + crane[j].static_H11_TR_Y / 2;
                            crane[j].stp_z[4] = crane[j].stp_z[5];
                            //
                            crane[j].Obj_x[9] = 0;
                            crane[j].Obj_y[9] = 0;
                            crane[j].Obj_z[9] = 0;
                            crane[j].stp_x[9] = 0;
                            crane[j].stp_y[9] = 0;
                            crane[j].stp_z[9] = 0;
                            // XYZ - STP для привязки груза в зависимости от наличия или отсутствия траверсы
                            crane[j].stp_x[8] = crane[j].stp_x[4];
                            crane[j].stp_y[8] = crane[j].stp_y[4];
                            crane[j].stp_z[8] = crane[j].stp_z[4];
                            // =============================================================================
                        }
                        #endregion
                    }
                    else
                    {
                        crane[j].Obj_x[5] = 0;
                        crane[j].Obj_y[5] = 0;
                        crane[j].Obj_z[5] = 0;
                        crane[j].stp_x[5] = 0;
                        crane[j].stp_y[5] = 0;
                        crane[j].stp_z[5] = 0;
                        // XYZ - STP для привязки груза в зависимости от наличия или отсутствия траверся
                        crane[j].stp_x[4] = crane[j].stp_x[3] + crane[j].static_H_1_1_X / 2;
                        crane[j].stp_y[4] = crane[j].stp_y[3] + crane[j].static_H_1_1_Y / 2;
                        crane[j].stp_z[4] = crane[j].stp_z[3];
                        //
                    }
                    #endregion
                    #region КРАН j => 1- 320t/ 2-120t/ 3-25t - подъем 2 - 1 - ТРАВЕРСА независимый подъем
                    if (crane[j].static_H21_TR_X > 0 & crane[j].static_H21_TR_Y > 0 & crane[j].static_H21_TR_Z > 0)
                    {
                        // XYZ - ШИРИНА/ДЛИНА/ВЫСОТА ТРАВЕРСЫ 
                        crane[j].Obj_x[15] = crane[j].static_H21_TR_X;
                        crane[j].Obj_y[15] = crane[j].static_H21_TR_Y;
                        crane[j].Obj_z[15] = crane[j].static_H21_TR_Z;
                        // XYZ - STP ТРАВЕРСЫ
                        crane[j].stp_x[15] = crane[j].stp_x[13] + crane[j].static_H_2_1_X / 2 - crane[j].static_H21_TR_X / 2;
                        crane[j].stp_y[15] = crane[j].stp_y[13] + crane[j].static_H_2_1_Y / 2 - crane[j].static_H21_TR_Y / 2;
                        crane[j].stp_z[15] = crane[j].stp_z[13] - crane[j].static_H21_TR_Z;
                        //
                        // XYZ - STP для привязки груза в зависимости от наличия или отсутствия траверся
                        crane[j].stp_x[14] = crane[j].stp_x[13] + crane[j].static_H_2_1_X / 2;
                        crane[j].stp_y[14] = crane[j].stp_y[13] + crane[j].static_H_2_1_Y / 2;
                        crane[j].stp_z[14] = crane[j].stp_z[13] - crane[j].static_H21_TR_Z;
                        //
                    }
                    else
                    {
                        crane[j].Obj_x[15] = 0;
                        crane[j].Obj_y[15] = 0;
                        crane[j].Obj_z[15] = 0;
                        crane[j].stp_x[15] = 0;
                        crane[j].stp_y[15] = 0;
                        crane[j].stp_z[15] = 0;
                        // XYZ - STP для привязки груза в зависимости от наличия или отсутствия траверся
                        crane[j].stp_x[14] = crane[j].stp_x[13] + crane[j].static_H_2_1_X / 2;
                        crane[j].stp_y[14] = crane[j].stp_y[13] + crane[j].static_H_2_1_Y / 2;
                        crane[j].stp_z[14] = crane[j].stp_z[13];
                        //

                    }
                    #endregion
                    #region КРАН j => 1- 320t/ 2-120t/ 3-25t - подъем 2 - 2 - ТРАВЕРСА независимый подъем
                    if (crane[j].static_H22_TR_X > 0 & crane[j].static_H22_TR_Y > 0 & crane[j].static_H22_TR_Z > 0)
                    {
                        // XYZ - ШИРИНА/ДЛИНА/ВЫСОТА ТРАВЕРСЫ 
                        crane[j].Obj_x[19] = crane[j].static_H22_TR_X;
                        crane[j].Obj_y[19] = crane[j].static_H22_TR_Y;
                        crane[j].Obj_z[19] = crane[j].static_H22_TR_Z;
                        // XYZ - STP ТРАВЕРСЫ
                        crane[j].stp_x[19] = crane[j].stp_x[17] + crane[j].static_H_2_2_X / 2 - crane[j].static_H22_TR_X / 2;
                        crane[j].stp_y[19] = crane[j].stp_y[17] + crane[j].static_H_2_2_Y / 2 - crane[j].static_H22_TR_Y / 2;
                        crane[j].stp_z[19] = crane[j].stp_z[17] - crane[j].static_H22_TR_Z;
                        //
                        // XYZ - STP для привязки груза в зависимости от наличия или отсутствия траверся
                        crane[j].stp_x[18] = crane[j].stp_x[17] + crane[j].static_H_2_2_X / 2;
                        crane[j].stp_y[18] = crane[j].stp_y[17] + crane[j].static_H_2_2_Y / 2;
                        crane[j].stp_z[18] = crane[j].stp_z[17] - crane[j].static_H22_TR_Z;
                        //
                    }
                    else
                    {
                        crane[j].Obj_x[19] = 0;
                        crane[j].Obj_y[19] = 0;
                        crane[j].Obj_z[19] = 0;
                        crane[j].stp_x[19] = 0;
                        crane[j].stp_y[19] = 0;
                        crane[j].stp_z[19] = 0;
                        // XYZ - STP для привязки груза в зависимости от наличия или отсутствия траверся
                        crane[j].stp_x[18] = crane[j].stp_x[17] + crane[j].static_H_2_2_X / 2;
                        crane[j].stp_y[18] = crane[j].stp_y[17] + crane[j].static_H_2_2_Y / 2;
                        crane[j].stp_z[18] = crane[j].stp_z[17];
                        //

                    }
                    #endregion
                    break;
                case 4: // ok  
                    #region КРАН j => 1- 320t/ 2-120t/ 3-25t - подъем 1-1/ 1-2/ 2-1 - ТРАВЕРСА 1+2+3 ПОДЪЕМЫ

                    #region КРАН j => 1- 320t/ 2-120t/ 3-25t - подъем 1 - 2 - ТРАВЕРСА независимый подъем
                    if (crane[j].static_H12_TR_X > 0 & crane[j].static_H12_TR_Y > 0 & crane[j].static_H12_TR_Z > 0)
                    {
                        // XYZ - ШИРИНА/ДЛИНА/ВЫСОТА ТРАВЕРСЫ 
                        crane[j].Obj_x[9] = crane[j].static_H12_TR_X;
                        crane[j].Obj_y[9] = crane[j].static_H12_TR_Y;
                        crane[j].Obj_z[9] = crane[j].static_H12_TR_Z;
                        // XYZ - STP ТРАВЕРСЫ
                        crane[j].stp_x[9] = crane[j].stp_x[7] + crane[j].static_H_1_2_X / 2 - crane[j].static_H12_TR_X / 2;
                        crane[j].stp_y[9] = crane[j].stp_y[7] + crane[j].static_H_1_2_Y / 2 - crane[j].static_H12_TR_Y / 2;
                        crane[j].stp_z[9] = crane[j].stp_z[7] - crane[j].static_H12_TR_Z;
                        //
                        // XYZ - STP для привязки груза в зависимости от наличия или отсутствия траверся
                        crane[j].stp_x[8] = crane[j].stp_x[7] + crane[j].static_H_1_2_X / 2;
                        crane[j].stp_y[8] = crane[j].stp_y[7] + crane[j].static_H_1_2_Y / 2;
                        crane[j].stp_z[8] = crane[j].stp_z[7] - crane[j].static_H12_TR_Z;
                        //
                    }
                    else
                    {
                        crane[j].Obj_x[9] = 0;
                        crane[j].Obj_y[9] = 0;
                        crane[j].Obj_z[9] = 0;
                        crane[j].stp_x[9] = 0;
                        crane[j].stp_y[9] = 0;
                        crane[j].stp_z[9] = 0;
                        // XYZ - STP для привязки груза в зависимости от наличия или отсутствия траверся
                        crane[j].stp_x[8] = crane[j].stp_x[7] + crane[j].static_H_1_2_X / 2;
                        crane[j].stp_y[8] = crane[j].stp_y[7] + crane[j].static_H_1_2_Y / 2;
                        crane[j].stp_z[8] = crane[j].stp_z[7];
                        //

                    }
                    #endregion
                    #region КРАН j => 1- 320t/ 2-120t/ 3-25t - подъем 1-1/ 1-2 - ТРАВЕРСА на два или один подъем
                    if (crane[j].static_H11_TR_X > 0 & crane[j].static_H11_TR_Y > 0 & crane[j].static_H11_TR_Z > 0)
                    {
                        #region ТРАВЕРСА НА ОДИН ПОДЪЕМ
                        if (!(crane[j].static_H11_TR_twohook_y_n))
                        {
                            #region КРАН j => 1- 320t/ 2-120t/ 3-25t - подъем 1 - 1 - ТРАВЕРСА независимый подъем
                            if (crane[j].static_H11_TR_X > 0 & crane[j].static_H11_TR_Y > 0 & crane[j].static_H11_TR_Z > 0)
                            {
                                // XYZ - ШИРИНА/ДЛИНА/ВЫСОТА ТРАВЕРСЫ 
                                crane[j].Obj_x[5] = crane[j].static_H11_TR_X;
                                crane[j].Obj_y[5] = crane[j].static_H11_TR_Y;
                                crane[j].Obj_z[5] = crane[j].static_H11_TR_Z;
                                // XYZ - STP ТРАВЕРСЫ
                                crane[j].stp_x[5] = crane[j].stp_x[3] + crane[j].static_H_1_1_X / 2 - crane[j].static_H11_TR_X / 2;
                                crane[j].stp_y[5] = crane[j].stp_y[3] + crane[j].static_H_1_1_Y / 2 - crane[j].static_H11_TR_Y / 2;
                                crane[j].stp_z[5] = crane[j].stp_z[3] - crane[j].static_H11_TR_Z;
                                //
                                // XYZ - STP для привязки груза в зависимости от наличия или отсутствия траверся
                                crane[j].stp_x[4] = crane[j].stp_x[3] + crane[j].static_H_1_1_X / 2;
                                crane[j].stp_y[4] = crane[j].stp_y[3] + crane[j].static_H_1_1_Y / 2;
                                crane[j].stp_z[4] = crane[j].stp_z[3] - crane[j].static_H11_TR_Z;
                                //
                            }
                            else
                            {
                                crane[j].Obj_x[5] = 0;
                                crane[j].Obj_y[5] = 0;
                                crane[j].Obj_z[5] = 0;
                                crane[j].stp_x[5] = 0;
                                crane[j].stp_y[5] = 0;
                                crane[j].stp_z[5] = 0;
                                // XYZ - STP для привязки груза в зависимости от наличия или отсутствия траверся
                                crane[j].stp_x[4] = crane[j].stp_x[3] + crane[j].static_H_1_1_X / 2;
                                crane[j].stp_y[4] = crane[j].stp_y[3] + crane[j].static_H_1_1_Y / 2;
                                crane[j].stp_z[4] = crane[j].stp_z[3];
                                //
                            }
                            #endregion
                        }
                        #endregion
                        #region ТРАВЕРСА НА ДВА ПОДЪЕМА
                        if (crane[j].static_H11_TR_twohook_y_n)
                        {
                            // XYZ - ШИРИНА/ДЛИНА/ВЫСОТА ТРАВЕРСЫ 
                            crane[j].Obj_x[5] = crane[j].static_H11_TR_X;
                            crane[j].Obj_y[5] = crane[j].static_H11_TR_Y;
                            crane[j].Obj_z[5] = crane[j].static_H11_TR_Z;
                            // XYZ - STP ТРАВЕРСЫ
                            crane[j].stp_x[5] = ((crane[j].stp_x[3] + (((crane[j].stp_x[3] + crane[j].static_H_1_1_X / 2) - (crane[j].stp_x[7] + crane[j].static_H_1_2_X / 2)) / 2)) - (crane[j].static_H11_TR_X / 2));
                            crane[j].stp_y[5] = crane[j].stp_y[3] + crane[j].static_H_1_1_Y / 2 - crane[j].static_H11_TR_Y / 2;
                            // Z - STP ТРАВЕРСЫ
                            if (crane[j].stp_z[3] >= crane[j].stp_z[7])
                            {
                                crane[j].stp_z[5] = crane[j].stp_z[7] - crane[j].static_H11_TR_Z;
                            }
                            else
                            {
                                crane[j].stp_z[5] = crane[j].stp_z[3] - crane[j].static_H11_TR_Z;
                            }
                            //
                            // XYZ - STP для привязки груза в зависимости от наличия или отсутствия траверсы
                            crane[j].stp_x[4] = crane[j].stp_x[5] + crane[j].static_H11_TR_X / 2;
                            crane[j].stp_y[4] = crane[j].stp_y[5] + crane[j].static_H11_TR_Y / 2;
                            crane[j].stp_z[4] = crane[j].stp_z[5];
                            //
                            crane[j].Obj_x[9] = 0;
                            crane[j].Obj_y[9] = 0;
                            crane[j].Obj_z[9] = 0;
                            crane[j].stp_x[9] = 0;
                            crane[j].stp_y[9] = 0;
                            crane[j].stp_z[9] = 0;
                            // XYZ - STP для привязки груза в зависимости от наличия или отсутствия траверсы
                            crane[j].stp_x[8] = crane[j].stp_x[4];
                            crane[j].stp_y[8] = crane[j].stp_y[4];
                            crane[j].stp_z[8] = crane[j].stp_z[4];
                            // =============================================================================
                        }
                        #endregion
                    }
                    else
                    {
                        crane[j].Obj_x[5] = 0;
                        crane[j].Obj_y[5] = 0;
                        crane[j].Obj_z[5] = 0;
                        crane[j].stp_x[5] = 0;
                        crane[j].stp_y[5] = 0;
                        crane[j].stp_z[5] = 0;
                        // XYZ - STP для привязки груза в зависимости от наличия или отсутствия траверся
                        crane[j].stp_x[4] = crane[j].stp_x[3] + crane[j].static_H_1_1_X / 2;
                        crane[j].stp_y[4] = crane[j].stp_y[3] + crane[j].static_H_1_1_Y / 2;
                        crane[j].stp_z[4] = crane[j].stp_z[3];
                        //
                    }
                    #endregion
                    #region КРАН j => 1- 320t/ 2-120t/ 3-25t - подъем 2 - 1 - ТРАВЕРСА независимый подъем
                    if (crane[j].static_H21_TR_X > 0 & crane[j].static_H21_TR_Y > 0 & crane[j].static_H21_TR_Z > 0)
                    {
                        // XYZ - ШИРИНА/ДЛИНА/ВЫСОТА ТРАВЕРСЫ 
                        crane[j].Obj_x[15] = crane[j].static_H21_TR_X;
                        crane[j].Obj_y[15] = crane[j].static_H21_TR_Y;
                        crane[j].Obj_z[15] = crane[j].static_H21_TR_Z;
                        // XYZ - STP ТРАВЕРСЫ
                        crane[j].stp_x[15] = crane[j].stp_x[13] + crane[j].static_H_2_1_X / 2 - crane[j].static_H21_TR_X / 2;
                        crane[j].stp_y[15] = crane[j].stp_y[13] + crane[j].static_H_2_1_Y / 2 - crane[j].static_H21_TR_Y / 2;
                        crane[j].stp_z[15] = crane[j].stp_z[13] - crane[j].static_H21_TR_Z;
                        //
                        // XYZ - STP для привязки груза в зависимости от наличия или отсутствия траверся
                        crane[j].stp_x[14] = crane[j].stp_x[13] + crane[j].static_H_2_1_X / 2;
                        crane[j].stp_y[14] = crane[j].stp_y[13] + crane[j].static_H_2_1_Y / 2;
                        crane[j].stp_z[14] = crane[j].stp_z[13] - crane[j].static_H21_TR_Z;
                        //
                    }
                    else
                    {
                        crane[j].Obj_x[15] = 0;
                        crane[j].Obj_y[15] = 0;
                        crane[j].Obj_z[15] = 0;
                        crane[j].stp_x[15] = 0;
                        crane[j].stp_y[15] = 0;
                        crane[j].stp_z[15] = 0;
                        // XYZ - STP для привязки груза в зависимости от наличия или отсутствия траверся
                        crane[j].stp_x[14] = crane[j].stp_x[13] + crane[j].static_H_2_1_X / 2;
                        crane[j].stp_y[14] = crane[j].stp_y[13] + crane[j].static_H_2_1_Y / 2;
                        crane[j].stp_z[14] = crane[j].stp_z[13];
                        //

                    }
                    #endregion

                    #region ОПРЕДЕЛЯЕМ ТОЧКУ ПРИВЯЗКИ ГРУЗА для режима 1+2+3
                    // XYZ - STP для привязки груза в зависимости от наличия или отсутствия траверся
                    crane[j].stp_x[11] = crane[j].stp_x[14];
                    if (crane[j].stp_y[14] < crane[j].stp_y[4])
                    {
                        crane[j].stp_y[11] = (crane[j].stp_y[4] - crane[j].stp_y[14]) / 2 + crane[j].stp_y[14];
                    }
                    else
                    {
                        crane[j].stp_y[11] = (crane[j].stp_y[14] - crane[j].stp_y[4]) / 2 + crane[j].stp_y[4];
                    }
                    // Z - STP ГРУЗ 
                    // ==============================================
                    // выбор из трех и привязка к наиболее низкому подъему
                    temp_compare_1el = 0;
                    temp_compare_2el = 0;
                    temp_compare_3el = 0;
                    if (crane[j].stp_z[4] > crane[j].stp_z[8])
                    {
                        temp_compare_1el = temp_compare_1el + 1;
                    }
                    if (crane[j].stp_z[4] > crane[j].stp_z[14])
                    {
                        temp_compare_1el = temp_compare_1el + 1;
                    }
                    if (crane[j].stp_z[8] > crane[j].stp_z[4])
                    {
                        temp_compare_2el = temp_compare_2el + 1;
                    }
                    if (crane[j].stp_z[8] > crane[j].stp_z[14])
                    {
                        temp_compare_2el = temp_compare_2el + 1;
                    }
                    if (crane[j].stp_z[14] > crane[j].stp_z[4])
                    {
                        temp_compare_3el = temp_compare_3el + 1;
                    }
                    if (crane[j].stp_z[14] > crane[j].stp_z[8])
                    {
                        temp_compare_3el = temp_compare_3el + 1;
                    }
                    //
                    temp_compare_OK_write = false;

                    if (temp_compare_1el == 0)
                    {
                        temp_compare_OK_write = true;
                        crane[j].stp_z[11] = crane[j].stp_z[4];
                    }

                    if (!temp_compare_OK_write)
                    {

                        if (temp_compare_2el == 0)
                        {
                            temp_compare_OK_write = true;
                            crane[j].stp_z[11] = crane[j].stp_z[8];
                        }

                    }

                    if (!temp_compare_OK_write)
                    {
                        if (temp_compare_3el == 0)
                        {
                            temp_compare_OK_write = true;
                            crane[j].stp_z[11] = crane[j].stp_z[14];
                        }
                    }
                    // ==============================================
                    #endregion

                    #endregion
                    #region КРАН j => 1- 320t/ 2-120t/ 3-25t - подъем 2 - 2 - ТРАВЕРСА независимый подъем
                    if (crane[j].static_H22_TR_X > 0 & crane[j].static_H22_TR_Y > 0 & crane[j].static_H22_TR_Z > 0)
                    {
                        // XYZ - ШИРИНА/ДЛИНА/ВЫСОТА ТРАВЕРСЫ 
                        crane[j].Obj_x[19] = crane[j].static_H22_TR_X;
                        crane[j].Obj_y[19] = crane[j].static_H22_TR_Y;
                        crane[j].Obj_z[19] = crane[j].static_H22_TR_Z;
                        // XYZ - STP ТРАВЕРСЫ
                        crane[j].stp_x[19] = crane[j].stp_x[17] + crane[j].static_H_2_2_X / 2 - crane[j].static_H22_TR_X / 2;
                        crane[j].stp_y[19] = crane[j].stp_y[17] + crane[j].static_H_2_2_Y / 2 - crane[j].static_H22_TR_Y / 2;
                        crane[j].stp_z[19] = crane[j].stp_z[17] - crane[j].static_H22_TR_Z;
                        //
                        // XYZ - STP для привязки груза в зависимости от наличия или отсутствия траверся
                        crane[j].stp_x[18] = crane[j].stp_x[17] + crane[j].static_H_2_2_X / 2;
                        crane[j].stp_y[18] = crane[j].stp_y[17] + crane[j].static_H_2_2_Y / 2;
                        crane[j].stp_z[18] = crane[j].stp_z[17] - crane[j].static_H22_TR_Z;
                        //
                    }
                    else
                    {
                        crane[j].Obj_x[19] = 0;
                        crane[j].Obj_y[19] = 0;
                        crane[j].Obj_z[19] = 0;
                        crane[j].stp_x[19] = 0;
                        crane[j].stp_y[19] = 0;
                        crane[j].stp_z[19] = 0;
                        // XYZ - STP для привязки груза в зависимости от наличия или отсутствия траверся
                        crane[j].stp_x[18] = crane[j].stp_x[17] + crane[j].static_H_2_2_X / 2;
                        crane[j].stp_y[18] = crane[j].stp_y[17] + crane[j].static_H_2_2_Y / 2;
                        crane[j].stp_z[18] = crane[j].stp_z[17];
                        //

                    }
                    #endregion
                    break;
                default: //копируем из варианта 0
                         // ok
                    #region КРАН j => 1- 320t/ 2-120t/ 3-25t - подъем 1 - 1 - ТРАВЕРСА независимый подъем
                    if (crane[j].static_H11_TR_X > 0 & crane[j].static_H11_TR_Y > 0 & crane[j].static_H11_TR_Z > 0)
                    {
                        // XYZ - ШИРИНА/ДЛИНА/ВЫСОТА ТРАВЕРСЫ 
                        crane[j].Obj_x[5] = crane[j].static_H11_TR_X;
                        crane[j].Obj_y[5] = crane[j].static_H11_TR_Y;
                        crane[j].Obj_z[5] = crane[j].static_H11_TR_Z;
                        // XYZ - STP ТРАВЕРСЫ
                        crane[j].stp_x[5] = crane[j].stp_x[3] + crane[j].static_H_1_1_X / 2 - crane[j].static_H11_TR_X / 2;
                        crane[j].stp_y[5] = crane[j].stp_y[3] + crane[j].static_H_1_1_Y / 2 - crane[j].static_H11_TR_Y / 2;
                        crane[j].stp_z[5] = crane[j].stp_z[3] - crane[j].static_H11_TR_Z;
                        //
                        // XYZ - STP для привязки груза в зависимости от наличия или отсутствия траверся
                        crane[j].stp_x[4] = crane[j].stp_x[3] + crane[j].static_H_1_1_X / 2;
                        crane[j].stp_y[4] = crane[j].stp_y[3] + crane[j].static_H_1_1_Y / 2;
                        crane[j].stp_z[4] = crane[j].stp_z[3] - crane[j].static_H11_TR_Z;
                        //
                    }
                    else
                    {
                        crane[j].Obj_x[5] = 0;
                        crane[j].Obj_y[5] = 0;
                        crane[j].Obj_z[5] = 0;
                        crane[j].stp_x[5] = 0;
                        crane[j].stp_y[5] = 0;
                        crane[j].stp_z[5] = 0;
                        // XYZ - STP для привязки груза в зависимости от наличия или отсутствия траверся
                        crane[j].stp_x[4] = crane[j].stp_x[3] + crane[j].static_H_1_1_X / 2;
                        crane[j].stp_y[4] = crane[j].stp_y[3] + crane[j].static_H_1_1_Y / 2;
                        crane[j].stp_z[4] = crane[j].stp_z[3];
                        //


                    }
                    #endregion
                    #region КРАН j => 1- 320t/ 2-120t/ 3-25t - подъем 1 - 2 - ТРАВЕРСА независимый подъем
                    if (crane[j].static_H12_TR_X > 0 & crane[j].static_H12_TR_Y > 0 & crane[j].static_H12_TR_Z > 0)
                    {
                        // XYZ - ШИРИНА/ДЛИНА/ВЫСОТА ТРАВЕРСЫ 
                        crane[j].Obj_x[9] = crane[j].static_H12_TR_X;
                        crane[j].Obj_y[9] = crane[j].static_H12_TR_Y;
                        crane[j].Obj_z[9] = crane[j].static_H12_TR_Z;
                        // XYZ - STP ТРАВЕРСЫ
                        crane[j].stp_x[9] = crane[j].stp_x[7] + crane[j].static_H_1_2_X / 2 - crane[j].static_H12_TR_X / 2;
                        crane[j].stp_y[9] = crane[j].stp_y[7] + crane[j].static_H_1_2_Y / 2 - crane[j].static_H12_TR_Y / 2;
                        crane[j].stp_z[9] = crane[j].stp_z[7] - crane[j].static_H12_TR_Z;
                        //
                        // XYZ - STP для привязки груза в зависимости от наличия или отсутствия траверся
                        crane[j].stp_x[8] = crane[j].stp_x[7] + crane[j].static_H_1_2_X / 2;
                        crane[j].stp_y[8] = crane[j].stp_y[7] + crane[j].static_H_1_2_Y / 2;
                        crane[j].stp_z[8] = crane[j].stp_z[7] - crane[j].static_H12_TR_Z;
                        //
                    }
                    else
                    {
                        crane[j].Obj_x[9] = 0;
                        crane[j].Obj_y[9] = 0;
                        crane[j].Obj_z[9] = 0;
                        crane[j].stp_x[9] = 0;
                        crane[j].stp_y[9] = 0;
                        crane[j].stp_z[9] = 0;
                        // XYZ - STP для привязки груза в зависимости от наличия или отсутствия траверся
                        crane[j].stp_x[8] = crane[j].stp_x[7] + crane[j].static_H_1_2_X / 2;
                        crane[j].stp_y[8] = crane[j].stp_y[7] + crane[j].static_H_1_2_Y / 2;
                        crane[j].stp_z[8] = crane[j].stp_z[7];
                        //

                    }
                    #endregion
                    #region КРАН j => 1- 320t/ 2-120t/ 3-25t - подъем 2 - 1 - ТРАВЕРСА независимый подъем
                    if (crane[j].static_H21_TR_X > 0 & crane[j].static_H21_TR_Y > 0 & crane[j].static_H21_TR_Z > 0)
                    {
                        // XYZ - ШИРИНА/ДЛИНА/ВЫСОТА ТРАВЕРСЫ 
                        crane[j].Obj_x[15] = crane[j].static_H21_TR_X;
                        crane[j].Obj_y[15] = crane[j].static_H21_TR_Y;
                        crane[j].Obj_z[15] = crane[j].static_H21_TR_Z;
                        // XYZ - STP ТРАВЕРСЫ
                        crane[j].stp_x[15] = crane[j].stp_x[13] + crane[j].static_H_2_1_X / 2 - crane[j].static_H21_TR_X / 2;
                        crane[j].stp_y[15] = crane[j].stp_y[13] + crane[j].static_H_2_1_Y / 2 - crane[j].static_H21_TR_Y / 2;
                        crane[j].stp_z[15] = crane[j].stp_z[13] - crane[j].static_H21_TR_Z;
                        //
                        // XYZ - STP для привязки груза в зависимости от наличия или отсутствия траверся
                        crane[j].stp_x[14] = crane[j].stp_x[13] + crane[j].static_H_2_1_X / 2;
                        crane[j].stp_y[14] = crane[j].stp_y[13] + crane[j].static_H_2_1_Y / 2;
                        crane[j].stp_z[14] = crane[j].stp_z[13] - crane[j].static_H21_TR_Z;
                        //
                    }
                    else
                    {
                        crane[j].Obj_x[15] = 0;
                        crane[j].Obj_y[15] = 0;
                        crane[j].Obj_z[15] = 0;
                        crane[j].stp_x[15] = 0;
                        crane[j].stp_y[15] = 0;
                        crane[j].stp_z[15] = 0;
                        // XYZ - STP для привязки груза в зависимости от наличия или отсутствия траверся
                        crane[j].stp_x[14] = crane[j].stp_x[13] + crane[j].static_H_2_1_X / 2;
                        crane[j].stp_y[14] = crane[j].stp_y[13] + crane[j].static_H_2_1_Y / 2;
                        crane[j].stp_z[14] = crane[j].stp_z[13];
                        //

                    }
                    #endregion
                    #region КРАН j => 1- 320t/ 2-120t/ 3-25t - подъем 2 - 2 - ТРАВЕРСА независимый подъем
                    if (crane[j].static_H22_TR_X > 0 & crane[j].static_H22_TR_Y > 0 & crane[j].static_H22_TR_Z > 0)
                    {
                        // XYZ - ШИРИНА/ДЛИНА/ВЫСОТА ТРАВЕРСЫ 
                        crane[j].Obj_x[19] = crane[j].static_H22_TR_X;
                        crane[j].Obj_y[19] = crane[j].static_H22_TR_Y;
                        crane[j].Obj_z[19] = crane[j].static_H22_TR_Z;
                        // XYZ - STP ТРАВЕРСЫ
                        crane[j].stp_x[19] = crane[j].stp_x[17] + crane[j].static_H_2_2_X / 2 - crane[j].static_H22_TR_X / 2;
                        crane[j].stp_y[19] = crane[j].stp_y[17] + crane[j].static_H_2_2_Y / 2 - crane[j].static_H22_TR_Y / 2;
                        crane[j].stp_z[19] = crane[j].stp_z[17] - crane[j].static_H22_TR_Z;
                        //
                        // XYZ - STP для привязки груза в зависимости от наличия или отсутствия траверся
                        crane[j].stp_x[18] = crane[j].stp_x[17] + crane[j].static_H_2_2_X / 2;
                        crane[j].stp_y[18] = crane[j].stp_y[17] + crane[j].static_H_2_2_Y / 2;
                        crane[j].stp_z[18] = crane[j].stp_z[17] - crane[j].static_H22_TR_Z;
                        //
                    }
                    else
                    {
                        crane[j].Obj_x[19] = 0;
                        crane[j].Obj_y[19] = 0;
                        crane[j].Obj_z[19] = 0;
                        crane[j].stp_x[19] = 0;
                        crane[j].stp_y[19] = 0;
                        crane[j].stp_z[19] = 0;
                        // XYZ - STP для привязки груза в зависимости от наличия или отсутствия траверся
                        crane[j].stp_x[18] = crane[j].stp_x[17] + crane[j].static_H_2_2_X / 2;
                        crane[j].stp_y[18] = crane[j].stp_y[17] + crane[j].static_H_2_2_Y / 2;
                        crane[j].stp_z[18] = crane[j].stp_z[17];
                        //

                    }
                    #endregion
                    break;
            }
            #endregion
            // ===========================================================      
            #region ГРУЗА
            // crane[j].regim_w_hoist
            // 0 or default - подъемы независимы
            // 1 - траверс и груза нет
            // 2 - 1 + 2 вместе
            // 4 - 1 + 2 + 3 вместе
            switch (crane[j].regim_w_hoist)
            {
                // XYZ - ШИРИНА/ДЛИНА/ВЫСОТА ГРУЗ 
                // XYZ - STP ГРУЗ 
                case 0: //OK
                    #region КРАН j => 1- 320t/ 2-120t/ 3-25t - подъем 1 - 1 - ГРУЗ -  подъемы независимы
                    if (crane[j].static_H_1_1_LOAD_X > 0 & crane[j].static_H_1_1_LOAD_Y > 0 & crane[j].static_H_1_1_LOAD_Z > 0)
                    {
                        // XYZ - ШИРИНА/ДЛИНА/ВЫСОТА ГРУЗ 
                        crane[j].Obj_x[6] = crane[j].static_H_1_1_LOAD_X;
                        crane[j].Obj_y[6] = crane[j].static_H_1_1_LOAD_Y;
                        crane[j].Obj_z[6] = crane[j].static_H_1_1_LOAD_Z;
                        // XYZ - STP ГРУЗ 
                        crane[j].stp_x[6] = crane[j].stp_x[4] - crane[j].static_H_1_1_LOAD_X / 2;
                        crane[j].stp_y[6] = crane[j].stp_y[4] - crane[j].static_H_1_1_LOAD_Y / 2;
                        crane[j].stp_z[6] = crane[j].stp_z[4] - crane[j].static_H_1_1_LOAD_Z;
                        //
                    }
                    else
                    {
                        crane[j].Obj_x[6] = 0;
                        crane[j].Obj_y[6] = 0;
                        crane[j].Obj_z[6] = 0;
                        crane[j].stp_x[6] = 0;
                        crane[j].stp_y[6] = 0;
                        crane[j].stp_z[6] = 0;
                    }
                    #endregion
                    #region КРАН j => 1- 320t/ 2-120t/ 3-25t - подъем 1 - 2 - ГРУЗ -  подъемы независимы
                    if (crane[j].static_H_1_2_LOAD_X > 0 & crane[j].static_H_1_2_LOAD_Y > 0 & crane[j].static_H_1_2_LOAD_Z > 0)
                    {
                        // XYZ - ШИРИНА/ДЛИНА/ВЫСОТА ГРУЗ 
                        crane[j].Obj_x[10] = crane[j].static_H_1_2_LOAD_X;
                        crane[j].Obj_y[10] = crane[j].static_H_1_2_LOAD_Y;
                        crane[j].Obj_z[10] = crane[j].static_H_1_2_LOAD_Z;
                        // XYZ - STP ГРУЗ 
                        crane[j].stp_x[10] = crane[j].stp_x[8] - crane[j].static_H_1_2_LOAD_X / 2;
                        crane[j].stp_y[10] = crane[j].stp_y[8] - crane[j].static_H_1_2_LOAD_Y / 2;
                        crane[j].stp_z[10] = crane[j].stp_z[8] - crane[j].static_H_1_2_LOAD_Z;
                        //
                    }
                    else
                    {
                        crane[j].Obj_x[10] = 0;
                        crane[j].Obj_y[10] = 0;
                        crane[j].Obj_z[10] = 0;
                        crane[j].stp_x[10] = 0;
                        crane[j].stp_y[10] = 0;
                        crane[j].stp_z[10] = 0;
                    }
                    #endregion
                    #region КРАН j => 1- 320t/ 2-120t/ 3-25t - подъем 2 - 1 - ГРУЗ -  подъемы независимы
                    if (crane[j].static_H_2_1_LOAD_X > 0 & crane[j].static_H_2_1_LOAD_Y > 0 & crane[j].static_H_2_1_LOAD_Z > 0)
                    {
                        // XYZ - ШИРИНА/ДЛИНА/ВЫСОТА ГРУЗ 
                        crane[j].Obj_x[16] = crane[j].static_H_2_1_LOAD_X;
                        crane[j].Obj_y[16] = crane[j].static_H_2_1_LOAD_Y;
                        crane[j].Obj_z[16] = crane[j].static_H_2_1_LOAD_Z;
                        // XYZ - STP ГРУЗ 
                        crane[j].stp_x[16] = crane[j].stp_x[14] - crane[j].static_H_2_1_LOAD_X / 2;
                        crane[j].stp_y[16] = crane[j].stp_y[14] - crane[j].static_H_2_1_LOAD_Y / 2;
                        crane[j].stp_z[16] = crane[j].stp_z[14] - crane[j].static_H_2_1_LOAD_Z;
                        //
                    }
                    else
                    {
                        crane[j].Obj_x[16] = 0;
                        crane[j].Obj_y[16] = 0;
                        crane[j].Obj_z[16] = 0;
                        crane[j].stp_x[16] = 0;
                        crane[j].stp_y[16] = 0;
                        crane[j].stp_z[16] = 0;
                    }
                    #endregion
                    #region КРАН j => 1- 320t/ 2-120t/ 3-25t - подъем 2 - 2 - ГРУЗ -  подъемы независимы
                    if (crane[j].static_H_2_2_LOAD_X > 0 & crane[j].static_H_2_2_LOAD_Y > 0 & crane[j].static_H_2_2_LOAD_Z > 0)
                    {
                        // XYZ - ШИРИНА/ДЛИНА/ВЫСОТА ГРУЗ 
                        crane[j].Obj_x[20] = crane[j].static_H_2_2_LOAD_X;
                        crane[j].Obj_y[20] = crane[j].static_H_2_2_LOAD_Y;
                        crane[j].Obj_z[20] = crane[j].static_H_2_2_LOAD_Z;
                        // XYZ - STP ГРУЗ 
                        crane[j].stp_x[20] = crane[j].stp_x[18] - crane[j].static_H_2_2_LOAD_X / 2;
                        crane[j].stp_y[20] = crane[j].stp_y[18] - crane[j].static_H_2_2_LOAD_Y / 2;
                        crane[j].stp_z[20] = crane[j].stp_z[18] - crane[j].static_H_2_2_LOAD_Z;
                        //
                    }
                    else
                    {
                        crane[j].Obj_x[20] = 0;
                        crane[j].Obj_y[20] = 0;
                        crane[j].Obj_z[20] = 0;
                        crane[j].stp_x[20] = 0;
                        crane[j].stp_y[20] = 0;
                        crane[j].stp_z[20] = 0;
                    }
                    #endregion
                    break;
                case 1: //OK
                    #region КРАН j => 1- 320t/ 2-120t/ 3-25t - подъем 1 - 1 - ГРУЗ -  => 0 нет

                    crane[j].Obj_x[6] = 0;
                    crane[j].Obj_y[6] = 0;
                    crane[j].Obj_z[6] = 0;
                    crane[j].stp_x[6] = 0;
                    crane[j].stp_y[6] = 0;
                    crane[j].stp_z[6] = 0;

                    #endregion
                    #region КРАН j => 1- 320t/ 2-120t/ 3-25t - подъем 1 - 2 - ГРУЗ -  => 0 нет  

                    crane[j].Obj_x[10] = 0;
                    crane[j].Obj_y[10] = 0;
                    crane[j].Obj_z[10] = 0;
                    crane[j].stp_x[10] = 0;
                    crane[j].stp_y[10] = 0;
                    crane[j].stp_z[10] = 0;

                    #endregion
                    #region КРАН j => 1- 320t/ 2-120t/ 3-25t - подъем 2 - 1 - ГРУЗ -  => 0 нет

                    crane[j].Obj_x[16] = 0;
                    crane[j].Obj_y[16] = 0;
                    crane[j].Obj_z[16] = 0;
                    crane[j].stp_x[16] = 0;
                    crane[j].stp_y[16] = 0;
                    crane[j].stp_z[16] = 0;

                    #endregion
                    #region КРАН j => 1- 320t/ 2-120t/ 3-25t - подъем 2 - 2 - ГРУЗ -  => 0 нет  

                    crane[j].Obj_x[20] = 0;
                    crane[j].Obj_y[20] = 0;
                    crane[j].Obj_z[20] = 0;
                    crane[j].stp_x[20] = 0;
                    crane[j].stp_y[20] = 0;
                    crane[j].stp_z[20] = 0;

                    #endregion
                    break;
                case 2: //OK
                    #region КРАН j => 1- 320t/ 2-120t/ 3-25t - подъем 1-1/ 1-2 - груз 1+2 ПОДЪЕМЫ
                    if (crane[j].static_H_1_1_LOAD_X > 0 & crane[j].static_H_1_1_LOAD_Y > 0 & crane[j].static_H_1_1_LOAD_Z > 0)
                    {
                        // XYZ - ШИРИНА/ДЛИНА/ВЫСОТА ГРУЗ 
                        crane[j].Obj_x[6] = crane[j].static_H_1_1_LOAD_X;
                        crane[j].Obj_y[6] = crane[j].static_H_1_1_LOAD_Y;
                        crane[j].Obj_z[6] = crane[j].static_H_1_1_LOAD_Z;
                        // XYZ - STP ГРУЗ 
                        crane[j].stp_x[6] = crane[j].stp_x[4] - (crane[j].static_H_1_1_LOAD_X / 2);
                        crane[j].stp_y[6] = crane[j].stp_y[4] - (crane[j].static_H_1_1_LOAD_Y / 2);
                        crane[j].stp_y[6] = crane[j].stp_z[4] - crane[j].Obj_z[6];

                        crane[j].Obj_x[10] = 0;
                        crane[j].Obj_y[10] = 0;
                        crane[j].Obj_z[10] = 0;
                        crane[j].stp_x[10] = 0;
                        crane[j].stp_y[10] = 0;
                        crane[j].stp_z[10] = 0;
                    }
                    else
                    {
                        crane[j].Obj_x[6] = 0;
                        crane[j].Obj_y[6] = 0;
                        crane[j].Obj_z[6] = 0;
                        crane[j].stp_x[6] = 0;
                        crane[j].stp_y[6] = 0;
                        crane[j].stp_z[6] = 0;

                        crane[j].Obj_x[10] = 0;
                        crane[j].Obj_y[10] = 0;
                        crane[j].Obj_z[10] = 0;
                        crane[j].stp_x[10] = 0;
                        crane[j].stp_y[10] = 0;
                        crane[j].stp_z[10] = 0;
                    }
                    #endregion
                    #region КРАН j => 1- 320t/ 2-120t/ 3-25t - подъем 2 - 1 - ГРУЗ -  подъемы независимы
                    if (crane[j].static_H_2_1_LOAD_X > 0 & crane[j].static_H_2_1_LOAD_Y > 0 & crane[j].static_H_2_1_LOAD_Z > 0)
                    {
                        // XYZ - ШИРИНА/ДЛИНА/ВЫСОТА ГРУЗ 
                        crane[j].Obj_x[16] = crane[j].static_H_2_1_LOAD_X;
                        crane[j].Obj_y[16] = crane[j].static_H_2_1_LOAD_Y;
                        crane[j].Obj_z[16] = crane[j].static_H_2_1_LOAD_Z;
                        // XYZ - STP ГРУЗ 
                        crane[j].stp_x[16] = crane[j].stp_x[14] - crane[j].static_H_2_1_LOAD_X / 2;
                        crane[j].stp_y[16] = crane[j].stp_y[14] - crane[j].static_H_2_1_LOAD_Y / 2;
                        crane[j].stp_z[16] = crane[j].stp_z[14] - crane[j].static_H_2_1_LOAD_Z;
                        //
                    }
                    else
                    {
                        crane[j].Obj_x[16] = 0;
                        crane[j].Obj_y[16] = 0;
                        crane[j].Obj_z[16] = 0;
                        crane[j].stp_x[16] = 0;
                        crane[j].stp_y[16] = 0;
                        crane[j].stp_z[16] = 0;
                    }
                    #endregion
                    #region КРАН j => 1- 320t/ 2-120t/ 3-25t - подъем 2 - 2 - ГРУЗ -  подъемы независимы
                    if (crane[j].static_H_2_2_LOAD_X > 0 & crane[j].static_H_2_2_LOAD_Y > 0 & crane[j].static_H_2_2_LOAD_Z > 0)
                    {
                        // XYZ - ШИРИНА/ДЛИНА/ВЫСОТА ГРУЗ 
                        crane[j].Obj_x[20] = crane[j].static_H_2_2_LOAD_X;
                        crane[j].Obj_y[20] = crane[j].static_H_2_2_LOAD_Y;
                        crane[j].Obj_z[20] = crane[j].static_H_2_2_LOAD_Z;
                        // XYZ - STP ГРУЗ 
                        crane[j].stp_x[20] = crane[j].stp_x[18] - crane[j].static_H_2_2_LOAD_X / 2;
                        crane[j].stp_y[20] = crane[j].stp_y[18] - crane[j].static_H_2_2_LOAD_Y / 2;
                        crane[j].stp_z[20] = crane[j].stp_z[18] - crane[j].static_H_2_2_LOAD_Z;
                        //
                    }
                    else
                    {
                        crane[j].Obj_x[20] = 0;
                        crane[j].Obj_y[20] = 0;
                        crane[j].Obj_z[20] = 0;
                        crane[j].stp_x[20] = 0;
                        crane[j].stp_y[20] = 0;
                        crane[j].stp_z[20] = 0;
                    }
                    #endregion
                    break;
                case 4: //OK             
                    #region КРАН j => 1- 320t/ 2-120t/ 3-25t - подъем 1-1/ 1-2/ 2-1 - груз 1+2+3 ПОДЪЕМЫ
                    if (crane[j].static_H_1_1_LOAD_X > 0 & crane[j].static_H_1_1_LOAD_Y > 0 & crane[j].static_H_1_1_LOAD_Z > 0)
                    {
                        // XYZ - ШИРИНА/ДЛИНА/ВЫСОТА ГРУЗ 
                        crane[j].Obj_x[6] = crane[j].static_H_1_1_LOAD_X;
                        crane[j].Obj_y[6] = crane[j].static_H_1_1_LOAD_Y;
                        crane[j].Obj_z[6] = crane[j].static_H_1_1_LOAD_Z;
                        // XYZ - STP ГРУЗ 
                        crane[j].stp_x[6] = crane[j].stp_x[11] - (crane[j].static_H_1_1_LOAD_X / 2);
                        crane[j].stp_y[6] = crane[j].stp_y[11] - (crane[j].static_H_1_1_LOAD_Y / 2);
                        crane[j].stp_y[6] = crane[j].stp_z[11] - crane[j].Obj_z[6];

                        crane[j].Obj_x[10] = 0;
                        crane[j].Obj_y[10] = 0;
                        crane[j].Obj_z[10] = 0;
                        crane[j].stp_x[10] = 0;
                        crane[j].stp_y[10] = 0;
                        crane[j].stp_z[10] = 0;

                        crane[j].Obj_x[16] = 0;
                        crane[j].Obj_y[16] = 0;
                        crane[j].Obj_z[16] = 0;
                        crane[j].stp_x[16] = 0;
                        crane[j].stp_y[16] = 0;
                        crane[j].stp_z[16] = 0;
                    }
                    else
                    {
                        crane[j].Obj_x[6] = 0;
                        crane[j].Obj_y[6] = 0;
                        crane[j].Obj_z[6] = 0;
                        crane[j].stp_x[6] = 0;
                        crane[j].stp_y[6] = 0;
                        crane[j].stp_z[6] = 0;

                        crane[j].Obj_x[10] = 0;
                        crane[j].Obj_y[10] = 0;
                        crane[j].Obj_z[10] = 0;
                        crane[j].stp_x[10] = 0;
                        crane[j].stp_y[10] = 0;
                        crane[j].stp_z[10] = 0;

                        crane[j].Obj_x[16] = 0;
                        crane[j].Obj_y[16] = 0;
                        crane[j].Obj_z[16] = 0;
                        crane[j].stp_x[16] = 0;
                        crane[j].stp_y[16] = 0;
                        crane[j].stp_z[16] = 0;
                    }
                    #endregion
                    #region КРАН j => 1- 320t/ 2-120t/ 3-25t - подъем 2 - 2 - ГРУЗ -  подъемы независимы
                    if (crane[j].static_H_2_2_LOAD_X > 0 & crane[j].static_H_2_2_LOAD_Y > 0 & crane[j].static_H_2_2_LOAD_Z > 0)
                    {
                        // XYZ - ШИРИНА/ДЛИНА/ВЫСОТА ГРУЗ 
                        crane[j].Obj_x[20] = crane[j].static_H_2_2_LOAD_X;
                        crane[j].Obj_y[20] = crane[j].static_H_2_2_LOAD_Y;
                        crane[j].Obj_z[20] = crane[j].static_H_2_2_LOAD_Z;
                        // XYZ - STP ГРУЗ 
                        crane[j].stp_x[20] = crane[j].stp_x[18] - crane[j].static_H_2_2_LOAD_X / 2;
                        crane[j].stp_y[20] = crane[j].stp_y[18] - crane[j].static_H_2_2_LOAD_Y / 2;
                        crane[j].stp_z[20] = crane[j].stp_z[18] - crane[j].static_H_2_2_LOAD_Z;
                        //
                    }
                    else
                    {
                        crane[j].Obj_x[20] = 0;
                        crane[j].Obj_y[20] = 0;
                        crane[j].Obj_z[20] = 0;
                        crane[j].stp_x[20] = 0;
                        crane[j].stp_y[20] = 0;
                        crane[j].stp_z[20] = 0;
                    }
                    #endregion
                    break;
                default: //OK - скопирован из  case 0 - "case 0" == "default"
                    #region КРАН j => 1- 320t/ 2-120t/ 3-25t - подъем 1 - 1 - ГРУЗ -  подъемы независимы
                    if (crane[j].static_H_1_1_LOAD_X > 0 & crane[j].static_H_1_1_LOAD_Y > 0 & crane[j].static_H_1_1_LOAD_Z > 0)
                    {
                        // XYZ - ШИРИНА/ДЛИНА/ВЫСОТА ГРУЗ 
                        crane[j].Obj_x[6] = crane[j].static_H_1_1_LOAD_X;
                        crane[j].Obj_y[6] = crane[j].static_H_1_1_LOAD_Y;
                        crane[j].Obj_z[6] = crane[j].static_H_1_1_LOAD_Z;
                        // XYZ - STP ГРУЗ 
                        crane[j].stp_x[6] = crane[j].stp_x[4] - crane[j].static_H_1_1_LOAD_X / 2;
                        crane[j].stp_y[6] = crane[j].stp_y[4] - crane[j].static_H_1_1_LOAD_Y / 2;
                        crane[j].stp_z[6] = crane[j].stp_z[4] - crane[j].static_H_1_1_LOAD_Z;
                        //
                    }
                    else
                    {
                        crane[j].Obj_x[6] = 0;
                        crane[j].Obj_y[6] = 0;
                        crane[j].Obj_z[6] = 0;
                        crane[j].stp_x[6] = 0;
                        crane[j].stp_y[6] = 0;
                        crane[j].stp_z[6] = 0;
                    }
                    #endregion
                    #region КРАН j => 1- 320t/ 2-120t/ 3-25t - подъем 1 - 2 - ГРУЗ -  подъемы независимы
                    if (crane[j].static_H_1_2_LOAD_X > 0 & crane[j].static_H_1_2_LOAD_Y > 0 & crane[j].static_H_1_2_LOAD_Z > 0)
                    {
                        // XYZ - ШИРИНА/ДЛИНА/ВЫСОТА ГРУЗ 
                        crane[j].Obj_x[10] = crane[j].static_H_1_2_LOAD_X;
                        crane[j].Obj_y[10] = crane[j].static_H_1_2_LOAD_Y;
                        crane[j].Obj_z[10] = crane[j].static_H_1_2_LOAD_Z;
                        // XYZ - STP ГРУЗ 
                        crane[j].stp_x[10] = crane[j].stp_x[8] - crane[j].static_H_1_2_LOAD_X / 2;
                        crane[j].stp_y[10] = crane[j].stp_y[8] - crane[j].static_H_1_2_LOAD_Y / 2;
                        crane[j].stp_z[10] = crane[j].stp_z[8] - crane[j].static_H_1_2_LOAD_Z;
                        //
                    }
                    else
                    {
                        crane[j].Obj_x[10] = 0;
                        crane[j].Obj_y[10] = 0;
                        crane[j].Obj_z[10] = 0;
                        crane[j].stp_x[10] = 0;
                        crane[j].stp_y[10] = 0;
                        crane[j].stp_z[10] = 0;
                    }
                    #endregion
                    #region КРАН j => 1- 320t/ 2-120t/ 3-25t - подъем 2 - 1 - ГРУЗ -  подъемы независимы
                    if (crane[j].static_H_2_1_LOAD_X > 0 & crane[j].static_H_2_1_LOAD_Y > 0 & crane[j].static_H_2_1_LOAD_Z > 0)
                    {
                        // XYZ - ШИРИНА/ДЛИНА/ВЫСОТА ГРУЗ 
                        crane[j].Obj_x[16] = crane[j].static_H_2_1_LOAD_X;
                        crane[j].Obj_y[16] = crane[j].static_H_2_1_LOAD_Y;
                        crane[j].Obj_z[16] = crane[j].static_H_2_1_LOAD_Z;
                        // XYZ - STP ГРУЗ 
                        crane[j].stp_x[16] = crane[j].stp_x[14] - crane[j].static_H_2_1_LOAD_X / 2;
                        crane[j].stp_y[16] = crane[j].stp_y[14] - crane[j].static_H_2_1_LOAD_Y / 2;
                        crane[j].stp_z[16] = crane[j].stp_z[14] - crane[j].static_H_2_1_LOAD_Z;
                        //
                    }
                    else
                    {
                        crane[j].Obj_x[16] = 0;
                        crane[j].Obj_y[16] = 0;
                        crane[j].Obj_z[16] = 0;
                        crane[j].stp_x[16] = 0;
                        crane[j].stp_y[16] = 0;
                        crane[j].stp_z[16] = 0;
                    }
                    #endregion
                    #region КРАН j => 1- 320t/ 2-120t/ 3-25t - подъем 2 - 2 - ГРУЗ -  подъемы независимы
                    if (crane[j].static_H_2_2_LOAD_X > 0 & crane[j].static_H_2_2_LOAD_Y > 0 & crane[j].static_H_2_2_LOAD_Z > 0)
                    {
                        // XYZ - ШИРИНА/ДЛИНА/ВЫСОТА ГРУЗ 
                        crane[j].Obj_x[20] = crane[j].static_H_2_2_LOAD_X;
                        crane[j].Obj_y[20] = crane[j].static_H_2_2_LOAD_Y;
                        crane[j].Obj_z[20] = crane[j].static_H_2_2_LOAD_Z;
                        // XYZ - STP ГРУЗ 
                        crane[j].stp_x[20] = crane[j].stp_x[18] - crane[j].static_H_2_2_LOAD_X / 2;
                        crane[j].stp_y[20] = crane[j].stp_y[18] - crane[j].static_H_2_2_LOAD_Y / 2;
                        crane[j].stp_z[20] = crane[j].stp_z[18] - crane[j].static_H_2_2_LOAD_Z;
                        //
                    }
                    else
                    {
                        crane[j].Obj_x[20] = 0;
                        crane[j].Obj_y[20] = 0;
                        crane[j].Obj_z[20] = 0;
                        crane[j].stp_x[20] = 0;
                        crane[j].stp_y[20] = 0;
                        crane[j].stp_z[20] = 0;
                    }
                    #endregion
                    break;
            }
            #endregion
            // ===========================================================    
        }
        // ===========================================================
        #endregion
        // ======================================================================================
        #region ПР302 - ПЕРЕНОС ЭЛЕМЕНТОВ КРАНА В УСЛОВНО СТАТИЧЕСКИЕ ОБЪЕКТЫ
        // ===========================================================
        // 1-20 - КРАН 1 - 320Т
        // 0/21-50 - КРАН 1 - 320Т - РЕЗЕРВ
        // 31-99 - ТОЛЬКО СТАТИЧНЫЕ ОБЪЕКТЫ - КОЛОННЫ И СТЕНЫ
        // 101-120 - КРАН 2 - 120Т
        // 100/121-150 - КРАН 2 - 120Т - РЕЗЕРВ
        // 151-199 - ТОЛЬКО СТАТИЧНЫЕ ОБЪЕКТЫ - РЕЗЕРВ
        // 201-220 - КРАН 3 - 25Т
        // 200/221-250 - КРАН 3 - 25Т - РЕЗЕРВ
        // 251-299 - ТОЛЬКО СТАТИЧНЫЕ ОБЪЕКТЫ - РЕЗЕРВ
        // 301 - 711 - ТОЛЬКО СТАТИЧНЫЕ ОБЪЕКТЫ - ПРЕПЯТСТВИЯ
        // ===========================================================
        // 1-20 - КРАН 1 - 320Т
        // 31-99 - ТОЛЬКО СТАТИЧНЫЕ ОБЪЕКТЫ - КОЛОННЫ И СТЕНЫ
        // 101-120 - КРАН 2 - 120Т
        // 201-220 - КРАН 2 - 120Т
        // 301 - 711 - ТОЛЬКО СТАТИЧНЫЕ ОБЪЕКТЫ - ПРЕПЯТСТВИЯ
        #region КРАН 1 -320 Т
        j = 1;
        for (i = 1; i < 21; i++)
        {
            f = i;

            Obj_ALL_X[i] = crane[j].Obj_x[f];
            Obj_ALL_Y[i] = crane[j].Obj_y[f];
            Obj_ALL_Z[i] = crane[j].Obj_z[f];

            Obj_ALL_ST_P_X[i] = crane[j].stp_x[f];
            Obj_ALL_ST_P_Y[i] = crane[j].stp_y[f];
            Obj_ALL_ST_P_Z[i] = crane[j].stp_z[f];
        }
        #endregion
        #region КРАН 2 -120 Т
        j = 2;
        for (i = 101; i < 121; i++)
        {
            f = i - 100;

            Obj_ALL_X[i] = crane[j].Obj_x[f];
            Obj_ALL_Y[i] = crane[j].Obj_y[f];
            Obj_ALL_Z[i] = crane[j].Obj_z[f];

            Obj_ALL_ST_P_X[i] = crane[j].stp_x[f];
            Obj_ALL_ST_P_Y[i] = crane[j].stp_y[f];
            Obj_ALL_ST_P_Z[i] = crane[j].stp_z[f];
        }
        #endregion
        #region КРАН 3 - 25 Т
        j = 3;
        for (i = 201; i < 221; i++)
        {
            f = i - 200;

            Obj_ALL_X[i] = crane[j].Obj_x[f];
            Obj_ALL_Y[i] = crane[j].Obj_y[f];
            Obj_ALL_Z[i] = crane[j].Obj_z[f];

            Obj_ALL_ST_P_X[i] = crane[j].stp_x[f];
            Obj_ALL_ST_P_Y[i] = crane[j].stp_y[f];
            Obj_ALL_ST_P_Z[i] = crane[j].stp_z[f];
        }
        #endregion
        #endregion
        // ======================================================================================         
        #region ПР303 - КРАНЫ - ИНИЦИАЛИЗАЦИЯ КРАНОВ              
        // ИНИЦИАЛИЗАЦИЯ ВИРТУАЛЬНЫХ СЕГМЕНТОВ КРАНА ПРЕДУПРЕЖДЕНИЯ И АВАРИИ
        for (j = 1; j <= 3; j++)
        {
            // ОБЛАСТИ ПРЕДУПРЕЖДЕНИЙ И АВАРИЙ
            for (f = 1; f < 25; f++)
            {
                // ОБЛАСТИ ПРЕДУПРЕЖДЕНИЙ
                // ОБЛАСТЬ ПРЕДУПРЕЖДЕНИЯ ВВЕРХ
                crane[j].UP_x_WARN[f] = crane[j].Obj_x[f];
                crane[j].UP_y_WARN[f] = crane[j].Obj_y[f];
                crane[j].UP_z_WARN[f] = (crane[j].Obj_z[f] / 2) + crane[j].D_UP_WARN[f];
                crane[j].UP_stp_x_WARN[f] = crane[j].stp_x[f];
                crane[j].UP_stp_y_WARN[f] = crane[j].stp_y[f];
                crane[j].UP_stp_z_WARN[f] = crane[j].stp_z[f] + (crane[j].Obj_z[f] / 2);
                // ОБЛАСТЬ ПРЕДУПРЕЖДЕНИЯ ВНИЗ
                crane[j].DOWN_x_WARN[f] = crane[j].Obj_x[f];
                crane[j].DOWN_y_WARN[f] = crane[j].Obj_y[f];
                crane[j].DOWN_z_WARN[f] = (crane[j].Obj_z[f] / 2) + crane[j].D_DOWN_WARN[f];
                crane[j].DOWN_stp_x_WARN[f] = crane[j].stp_x[f];
                crane[j].DOWN_stp_y_WARN[f] = crane[j].stp_y[f];
                crane[j].DOWN_stp_z_WARN[f] = crane[j].stp_z[f] - crane[j].D_DOWN_WARN[f]; // - crane[j].DOWN_z_WARN[f]; //+ (crane[j].Obj_z[f] / 2) 

                // ОБЛАСТЬ ПРЕДУПРЕЖДЕНИЯ ВПРАВО
                crane[j].R_x_WARN[f] = (crane[j].Obj_x[f] / 2) + crane[j].D_R_WARN[f]; ;
                crane[j].R_y_WARN[f] = crane[j].Obj_y[f];
                crane[j].R_z_WARN[f] = crane[j].Obj_z[f];

                crane[j].R_stp_x_WARN[f] = crane[j].stp_x[f] + (crane[j].Obj_x[f] / 2);
                crane[j].R_stp_y_WARN[f] = crane[j].stp_y[f];
                crane[j].R_stp_z_WARN[f] = crane[j].stp_z[f];

                // ОБЛАСТЬ ПРЕДУПРЕЖДЕНИЯ ВЛЕВО
                crane[j].L_x_WARN[f] = (crane[j].Obj_x[f] / 2) + crane[j].D_L_WARN[f]; ;
                crane[j].L_y_WARN[f] = crane[j].Obj_y[f];
                crane[j].L_z_WARN[f] = crane[j].Obj_z[f];

                crane[j].L_stp_x_WARN[f] = crane[j].stp_x[f]  - crane[j].D_L_WARN[f];  // crane[j].L_x_WARN[f]; // + (crane[j].Obj_x[f] / 2)
                crane[j].L_stp_y_WARN[f] = crane[j].stp_y[f];
                crane[j].L_stp_z_WARN[f] = crane[j].stp_z[f];

                // ОБЛАСТЬ ПРЕДУПРЕЖДЕНИЯ ВПЕРЕД
                crane[j].F_x_WARN[f] = crane[j].Obj_x[f];
                crane[j].F_y_WARN[f] = (crane[j].Obj_y[f] / 2) + crane[j].D_F_WARN[f];
                crane[j].F_z_WARN[f] = crane[j].Obj_z[f];

                crane[j].F_stp_x_WARN[f] = crane[j].stp_x[f];
                crane[j].F_stp_y_WARN[f] = crane[j].stp_y[f] + (crane[j].Obj_y[f] / 2); ;
                crane[j].F_stp_z_WARN[f] = crane[j].stp_z[f];

                // ОБЛАСТЬ ПРЕДУПРЕЖДЕНИЯ НАЗАД
                crane[j].B_x_WARN[f] = crane[j].Obj_x[f];
                crane[j].B_y_WARN[f] = (crane[j].Obj_y[f] / 2) + crane[j].D_B_WARN[f];
                crane[j].B_z_WARN[f] = crane[j].Obj_z[f];

                crane[j].B_stp_x_WARN[f] = crane[j].stp_x[f];
                crane[j].B_stp_y_WARN[f] = crane[j].stp_y[f] - crane[j].D_B_WARN[f]; // - crane[j].B_y_WARN[f]; //+ (crane[j].Obj_y[f] / 2) 
                crane[j].B_stp_z_WARN[f] = crane[j].stp_z[f];

                // ОБЛАСТИ АВАРИЙ

                // ОБЛАСТЬ ПРЕДУПРЕЖДЕНИЯ ВВЕРХ
                crane[j].UP_x_ERR[f] = crane[j].Obj_x[f];
                crane[j].UP_y_ERR[f] = crane[j].Obj_y[f];
                crane[j].UP_z_ERR[f] = (crane[j].Obj_z[f] / 2) + crane[j].D_UP_ERR[f];

                crane[j].UP_stp_x_ERR[f] = crane[j].stp_x[f];
                crane[j].UP_stp_y_ERR[f] = crane[j].stp_y[f];
                crane[j].UP_stp_z_ERR[f] = crane[j].stp_z[f] + (crane[j].Obj_z[f] / 2);

                // ОБЛАСТЬ ПРЕДУПРЕЖДЕНИЯ ВНИЗ
                crane[j].DOWN_x_ERR[f] = crane[j].Obj_x[f];
                crane[j].DOWN_y_ERR[f] = crane[j].Obj_y[f];
                crane[j].DOWN_z_ERR[f] = (crane[j].Obj_z[f] / 2) + crane[j].D_DOWN_ERR[f];

                crane[j].DOWN_stp_x_ERR[f] = crane[j].stp_x[f];
                crane[j].DOWN_stp_y_ERR[f] = crane[j].stp_y[f];
                crane[j].DOWN_stp_z_ERR[f] = crane[j].stp_z[f] - crane[j].D_DOWN_ERR[f]; // crane[j].DOWN_z_ERR[f]; //+ (crane[j].Obj_z[f] / 2) 

                // ОБЛАСТЬ ПРЕДУПРЕЖДЕНИЯ ВПРАВО
                crane[j].R_x_ERR[f] = (crane[j].Obj_x[f] / 2) + crane[j].D_R_ERR[f]; ;
                crane[j].R_y_ERR[f] = crane[j].Obj_y[f];
                crane[j].R_z_ERR[f] = crane[j].Obj_z[f];

                crane[j].R_stp_x_ERR[f] = crane[j].stp_x[f] + (crane[j].Obj_x[f] / 2);
                crane[j].R_stp_y_ERR[f] = crane[j].stp_y[f];
                crane[j].R_stp_z_ERR[f] = crane[j].stp_z[f];

                // ОБЛАСТЬ ПРЕДУПРЕЖДЕНИЯ ВЛЕВО
                crane[j].L_x_ERR[f] = (crane[j].Obj_x[f] / 2) + crane[j].D_L_ERR[f]; ;
                crane[j].L_y_ERR[f] = crane[j].Obj_y[f];
                crane[j].L_z_ERR[f] = crane[j].Obj_z[f];

                crane[j].L_stp_x_ERR[f] = crane[j].stp_x[f] - crane[j].D_L_ERR[f]; // - crane[j].L_x_ERR[f]; // + (crane[j].Obj_x[f] / 2)
                crane[j].L_stp_y_ERR[f] = crane[j].stp_y[f];
                crane[j].L_stp_z_ERR[f] = crane[j].stp_z[f];

                // ОБЛАСТЬ ПРЕДУПРЕЖДЕНИЯ ВПЕРЕД
                crane[j].F_x_ERR[f] = crane[j].Obj_x[f];
                crane[j].F_y_ERR[f] = (crane[j].Obj_y[f] / 2) + crane[j].D_F_ERR[f];
                crane[j].F_z_ERR[f] = crane[j].Obj_z[f];

                crane[j].F_stp_x_ERR[f] = crane[j].stp_x[f];
                crane[j].F_stp_y_ERR[f] = crane[j].stp_y[f] + (crane[j].Obj_y[f] / 2); ;
                crane[j].F_stp_z_ERR[f] = crane[j].stp_z[f];

                // ОБЛАСТЬ ПРЕДУПРЕЖДЕНИЯ НАЗАД
                crane[j].B_x_ERR[f] = crane[j].Obj_x[f];
                crane[j].B_y_ERR[f] = (crane[j].Obj_y[f] / 2) + crane[j].D_B_ERR[f];
                crane[j].B_z_ERR[f] = crane[j].Obj_z[f];

                crane[j].B_stp_x_ERR[f] = crane[j].stp_x[f];
                crane[j].B_stp_y_ERR[f] = crane[j].stp_y[f] - crane[j].D_B_ERR[f]; // - crane[j].B_y_ERR[f]; // + (crane[j].Obj_y[f] / 2) 
                crane[j].B_stp_z_ERR[f] = crane[j].stp_z[f];


            }
        }
        #endregion
        // ======================================================================================
        #region ПР400 - "G-АЛГОРИТМ - part 0" - АНАЛИЗ КАКИЕ ИЗ ЭЛЕМЕНТОВ БУДУТ АНАЛИЗИРОВАТЬСЯ
        // ПРЕДВАРИТЕЛЬНЫЙ АНАЛИЗ, ЧТОБЫ ИСКЛЮЧИТЬ ОБЪЕКТЫ С НУЛЕВЫМИ ЗНАЧЕНИЯМИ ГАБАРИТОВ
        // ===========================================================================================================
        #region  Анализ размеров элементов(ОБЪЕКТОВ) крана

        // Анализ размеров элементов(ОБЪЕКТОВ) крана
        for (j = 1; j <= 3; j++)
        {
            for (f = 0; f < 21; f++)
            {
                crane[j].Obj_Y_N[f] = true;
                if ((crane[j].Obj_x[f] <= 0 || crane[j].Obj_y[f] <= 0 || crane[j].Obj_z[f] <= 0))
                {
                    crane[j].Obj_Y_N[f] = false;
                }

            }
        }
        #endregion
        #region  Анализ размеров элементов(ОБЪЕКТОВ) статических
        // Анализ размеров элементов(ОБЪЕКТОВ) статических

        for (i = 0; i < 812; i++)
        {
            Obj_ALL_Y_N[i] = true;
            if ((Obj_ALL_X[i] <= 0) || (Obj_ALL_Y[i] <= 0) || (Obj_ALL_Z[i] <= 0))
            {
                Obj_ALL_Y_N[i] = false;
            }
        }
        // Объем массива хранения параметров траверс - не анализируется
        for (i = 221; i < 299; i++)
        {
          
                Obj_ALL_Y_N[i] = false;
            
        }

        #endregion
        // ===========================================================================================================
        #endregion
        // ======================================================================================
        #region ПР401 - "G-АЛГОРИТМ - part 1" - АНАЛИЗ ПЕРЕСЕЧЕНИЙ ДИНАМИЧЕСКИХ И СТАТИЧЕСКИХ ЭЛЕМЕНТОВ СИСТЕМЫ
        //
        // ===========================================================================================================
        bool flag_krash_x = false;
        bool flag_krash_y = false;
        bool flag_krash_z = false;
        //
        bool krash_xy = false;
        bool krash_xz = false;
        bool krash_yz = false;
        //
        bool krash = false;
        //
        float ax1 = 0;
        float ax2 = 0;
        //
        float ay1 = 0;
        float ay2 = 0;
        //
        float az1 = 0;
        float az2 = 0;
        //
        float bx1 = 0;
        float bx2 = 0;
        float by1 = 0;
        float by2 = 0;
        float bz1 = 0;
        float bz2 = 0;
        // ==========================================================================================================
        for (i = 1; i < 811; i++)
        {
            Obj_ALL_XYZ_KRASH_warn[i] = false;   // Предупреждения - СБРОС ПЕРЕД ОБРАБОТКОЙ
            Obj_ALL_XYZ_KRASH_err[i] = false;    // Аварии - СБРОС ПЕРЕД ОБРАБОТКОЙ
        }
        // ==========================================================================================================
        for (j = 1; j <= 3; j++)
        {
            for (f = 1; f < 25; f++)
            {
                // Предупреждения - СБРОС ПЕРЕД ОБРАБОТКОЙ
                crane[j].UP_KRASH_WARN[f] = false;
                crane[j].DOWN_KRASH_WARN[f] = false;
                crane[j].R_KRASH_WARN[f] = false;
                crane[j].L_KRASH_WARN[f] = false;
                crane[j].F_KRASH_WARN[f] = false;
                crane[j].B_KRASH_WARN[f] = false;
                // Аварии - СБРОС ПЕРЕД ОБРАБОТКОЙ
                crane[j].UP_KRASH_ERR[f] = false;
                crane[j].DOWN_KRASH_ERR[f] = false;
                crane[j].R_KRASH_ERR[f] = false;
                crane[j].L_KRASH_ERR[f] = false;
                crane[j].F_KRASH_ERR[f] = false;
                crane[j].B_KRASH_ERR[f] = false;

                for (i = 1; i < 811; i++)
                {
                    crane[j].KRASH_UP_WARN[f, i] = false;
                    crane[j].KRASH_UP_WARN[f, i] = false;
                    crane[j].KRASH_DOWN_WARN[f, i] = false;
                    crane[j].KRASH_L_WARN[f, i] = false;
                    crane[j].KRASH_R_WARN[f, i] = false;
                    crane[j].KRASH_F_WARN[f, i] = false;
                    crane[j].KRASH_B_WARN[f, i] = false;

                    crane[j].KRASH_UP_ERR[f, i] = false;
                    crane[j].KRASH_DOWN_ERR[f, i] = false;
                    crane[j].KRASH_L_ERR[f, i] = false;
                    crane[j].KRASH_R_ERR[f, i] = false;
                    crane[j].KRASH_F_ERR[f, i] = false;
                    crane[j].KRASH_B_ERR[f, i] = false;

                }
                    //
                    // ЕСЛИ ЭЛЕМЕНТ(объект) КРАНА ИМЕЕТ ОДНО ИЗ ЗНАЧЕНИЙ ГАБАРИТОВ (XYZ) МЕНЬШЕ ИЛИ РАВНОЕ НУЛЮ, ТО ДЛЯ
                    // ДАННОГО ЭЛЕМЕНТА(объекта) ОБРАБОТКА НЕ ОСУЩЕСТВЛЯЕТСЯ
                    // crane[j].Obj_Y_N[f] = TRUE - ЭЛЕМЕНТ ОБРАБАТЫВАЕТСЯ
                    // crane[j].Obj_Y_N[f] = false - ЭЛЕМЕНТ НЕ ОБРАБАТЫВАЕТСЯ
                    //
                    // ЭТО ДУБЛИРУЮЩАЯ ПРОВЕРКА - 
                    // (!((ax1 == 0) & (ax2 == 0))) & (!((bx1 == 0) & (bx2 == 0)))
                    // (!((ay1 == 0) & (ay2 == 0))) & (!((by1 == 0) & (by2 == 0)))
                    // (!((az1 == 0) & (az2 == 0))) & (!((bz1 == 0) & (bz2 == 0)))
                    // 
                    if (crane[j].Obj_Y_N[f])
                    {

                    // =================================== ПРЕДУПРЕЖДЕНИЯ - WARNING =====================================
                    #region ПРЕДУПРЕЖДЕНИЕ_UP - ВВЕРХ
                    // ==================================================================================================
                    crane[j].UP_KRASH_WARN[f] = false;
                    // "Координата Х"
                    ax1 = crane[j].UP_stp_x_WARN[f];
                    ax2 = crane[j].UP_stp_x_WARN[f] + crane[j].UP_x_WARN[f];
                    // "Координата y"
                    ay1 = crane[j].UP_stp_y_WARN[f];
                    ay2 = crane[j].UP_stp_y_WARN[f] + crane[j].UP_y_WARN[f];
                    // "Координата Z"
                    az1 = crane[j].UP_stp_z_WARN[f];
                    az2 = crane[j].UP_stp_z_WARN[f] + crane[j].UP_z_WARN[f];
                    // ==================================================================================================
                    for (i = 1; i < 811; i++)
                    {
                        if (crane[j].KRASH_cntrl_yn[f, i] & Obj_ALL_Y_N[i])
                        {
                            // "Координата Х"                
                            bx1 = Obj_ALL_ST_P_X[i];
                            bx2 = Obj_ALL_ST_P_X[i] + Obj_ALL_X[i];
                            // "Координата y"
                            by1 = Obj_ALL_ST_P_Y[i];
                            by2 = Obj_ALL_ST_P_Y[i] + Obj_ALL_Y[i];
                            // "Координата Z"
                            bz1 = Obj_ALL_ST_P_Z[i];
                            bz2 = Obj_ALL_ST_P_Z[i] + Obj_ALL_Z[i];
                            //
                            flag_krash_x = false;
                            flag_krash_y = false;
                            flag_krash_z = false;
                            //
                            krash_xy = false;
                            krash_xz = false;
                            krash_yz = false;
                            //
                            krash = false;
                            //     
                            // "Координата Х"    
                            if (((((ax1 >= bx1) & (ax1 <= bx2)) || ((ax2 >= bx1) & (ax2 <= bx2))) || (((bx1 >= ax1) & (bx1 <= ax2)) || ((bx2 >= ax1) & (bx2 <= ax2)))) & (!((ax1 == 0) & (ax2 == 0))) & (!((bx1 == 0) & (bx2 == 0))))
                            {
                                flag_krash_x = true;
                            }
                            // "Координата y"
                            if (((((ay1 >= by1) & (ay1 <= by2)) || ((ay2 >= by1) & (ay2 <= by2))) || (((by1 >= ay1) & (by1 <= ay2)) || ((by2 >= ay1) & (by2 <= ay2)))) & (!((ay1 == 0) & (ay2 == 0))) & (!((by1 == 0) & (by2 == 0))))
                            {
                                flag_krash_y = true;
                            }
                            // "Координата z"       
                            if (((((az1 >= bz1) & (az1 <= bz2)) || ((az2 >= bz1) & (az2 <= bz2))) || (((bz1 >= az1) & (bz1 <= az2)) || ((bz2 >= az1) & (bz2 <= az2)))) & (!((az1 == 0) & (az2 == 0))) & (!((bz1 == 0) & (bz2 == 0))))
                            {
                                flag_krash_z = true;
                            }
                            //
                            krash_xy = flag_krash_x & flag_krash_y;
                            krash_xz = flag_krash_x & flag_krash_z;
                            krash_yz = flag_krash_y & flag_krash_z;
                            //
                            krash = (krash_xy & krash_xz & krash_yz) || (krash_xy & krash_xz) || (krash_xz & krash_yz) || (krash_xy & krash_yz);
                            //
                            // ==================================================================================================
                            crane[j].KRASH_UP_WARN[f, i] = false;

                            if (krash)
                            {
                                crane[j].KRASH_UP_WARN[f, i] = true;

                                crane[j].UP_KRASH_WARN[f] = true;
                                Obj_ALL_XYZ_KRASH_warn[i] = true;

                                // // Write_ERR_WARN_ALD(int TEMP_j int TEMP_f, int TEMP_i, bool alarm, bool FLAG_ERR, bool FLAG_WARN, int zona_krash)
                                // 
                                // Какая зона
                                // 0 -   UP   - зона сверху
                                // 1 -   DOWN - зона снизу
                                // 2 -   F    - зона спереди
                                // 4 -   B    - зона сзади
                                // 8 -   R    - зона справа
                                // 16 -  L    - зона слева
                                //
                                if (!(crane[j].MEM_KRASH_UP_WARN[f, i]))
                                {
                                    // Write_ERR_WARN_ALD(j, f, i, true, false, true, 0, alarmsView);
                                }
                                crane[j].MEM_KRASH_UP_WARN[f, i] = true;
                            }
                            else
                            { 
                                crane[j].MEM_KRASH_UP_WARN[f, i] = false; 
                            }
                            // ==================================================================================================
                        }
                    }
                    #endregion
                    #region ПРЕДУПРЕЖДЕНИЕ_DOWN - ВНИЗ
                    // ==================================================================================================
                    crane[j].DOWN_KRASH_WARN[f] = false;
                    // "Координата Х"
                    ax1 = crane[j].DOWN_stp_x_WARN[f];
                    ax2 = crane[j].DOWN_stp_x_WARN[f] + crane[j].DOWN_x_WARN[f];
                    // "Координата y"
                    ay1 = crane[j].DOWN_stp_y_WARN[f];
                    ay2 = crane[j].DOWN_stp_y_WARN[f] + crane[j].DOWN_y_WARN[f];
                    // "Координата Z"
                    az1 = crane[j].DOWN_stp_z_WARN[f];
                    az2 = crane[j].DOWN_stp_z_WARN[f] + crane[j].DOWN_z_WARN[f];
                    // ==================================================================================================
                    for (i = 1; i < 811; i++)
                    {
                        if (crane[j].KRASH_cntrl_yn[f, i] & Obj_ALL_Y_N[i])
                        {
                            {
                                // "Координата Х"                
                                bx1 = Obj_ALL_ST_P_X[i];
                                bx2 = Obj_ALL_ST_P_X[i] + Obj_ALL_X[i];
                                // "Координата y"
                                by1 = Obj_ALL_ST_P_Y[i];
                                by2 = Obj_ALL_ST_P_Y[i] + Obj_ALL_Y[i];
                                // "Координата Z"
                                bz1 = Obj_ALL_ST_P_Z[i];
                                bz2 = Obj_ALL_ST_P_Z[i] + Obj_ALL_Z[i];
                                //
                                flag_krash_x = false;
                                flag_krash_y = false;
                                flag_krash_z = false;
                                //
                                krash_xy = false;
                                krash_xz = false;
                                krash_yz = false;
                                //
                                krash = false;
                                //     
                                // "Координата Х"    
                                if (((((ax1 >= bx1) & (ax1 <= bx2)) || ((ax2 >= bx1) & (ax2 <= bx2))) || (((bx1 >= ax1) & (bx1 <= ax2)) || ((bx2 >= ax1) & (bx2 <= ax2)))) & (!((ax1 == 0) & (ax2 == 0))) & (!((bx1 == 0) & (bx2 == 0))))
                                {
                                    flag_krash_x = true;
                                }
                                // "Координата y"
                                if (((((ay1 >= by1) & (ay1 <= by2)) || ((ay2 >= by1) & (ay2 <= by2))) || (((by1 >= ay1) & (by1 <= ay2)) || ((by2 >= ay1) & (by2 <= ay2)))) & (!((ay1 == 0) & (ay2 == 0))) & (!((by1 == 0) & (by2 == 0))))
                                {
                                    flag_krash_y = true;
                                }
                                // "Координата z"       
                                if (((((az1 >= bz1) & (az1 <= bz2)) || ((az2 >= bz1) & (az2 <= bz2))) || (((bz1 >= az1) & (bz1 <= az2)) || ((bz2 >= az1) & (bz2 <= az2)))) & (!((az1 == 0) & (az2 == 0))) & (!((bz1 == 0) & (bz2 == 0))))
                                {
                                    flag_krash_z = true;
                                }
                                //
                                krash_xy = flag_krash_x & flag_krash_y;
                                krash_xz = flag_krash_x & flag_krash_z;
                                krash_yz = flag_krash_y & flag_krash_z;
                                //
                                krash = (krash_xy & krash_xz & krash_yz) || (krash_xy & krash_xz) || (krash_xz & krash_yz) || (krash_xy & krash_yz);
                                //
                                // ==================================================================================================
                                crane[j].KRASH_DOWN_WARN[f, i] = false;
                                //
                                if (krash)
                                {
                                    crane[j].KRASH_DOWN_WARN[f, i] = true;

                                    crane[j].DOWN_KRASH_WARN[f] = true;
                                    Obj_ALL_XYZ_KRASH_warn[i] = true;

                                    //Write_ERR_WARN_ALDint TEMP_j int TEMP_f, int TEMP_i, bool alarm, bool FLAG_ERR, bool FLAG_WARN, int zona_krash)
                                    // 
                                    // Какая зона
                                    // 0 -   UP   - зона сверху
                                    // 1 -   DOWN - зона снизу
                                    // 2 -   F    - зона спереди
                                    // 4 -   B    - зона сзади
                                    // 8 -   R    - зона справа
                                    // 16 -  L    - зона слева
                                    //
                                    if (!(crane[j].MEM_KRASH_DOWN_WARN[f, i]))
                                    {
                                       // Write_ERR_WARN_ALD(j, f, i, true, false, true, 1, alarmsView);
                                    }
                                    crane[j].MEM_KRASH_DOWN_WARN[f, i] = true;
                                }
                                else
                                {
                                    crane[j].MEM_KRASH_DOWN_WARN[f, i] = false;
                                }
                                // ==================================================================================================
                            }
                        }
                    }
                    #endregion
                    #region ПРЕДУПРЕЖДЕНИЕ_R - ВПРАВО
                    // ==================================================================================================
                    crane[j].R_KRASH_WARN[f] = false;
                    // "Координата Х"
                    ax1 = crane[j].R_stp_x_WARN[f];
                    ax2 = crane[j].R_stp_x_WARN[f] + crane[j].R_x_WARN[f];
                    // "Координата y"
                    ay1 = crane[j].R_stp_y_WARN[f];
                    ay2 = crane[j].R_stp_y_WARN[f] + crane[j].R_y_WARN[f];
                    // "Координата Z"
                    az1 = crane[j].R_stp_z_WARN[f];
                    az2 = crane[j].R_stp_z_WARN[f] + crane[j].R_z_WARN[f];
                    // ==================================================================================================
                    for (i = 1; i < 811; i++)
                    {
                        if (crane[j].KRASH_cntrl_yn[f, i] & Obj_ALL_Y_N[i])
                        {

                            // "Координата Х"                
                            bx1 = Obj_ALL_ST_P_X[i];
                            bx2 = Obj_ALL_ST_P_X[i] + Obj_ALL_X[i];
                            // "Координата y"
                            by1 = Obj_ALL_ST_P_Y[i];
                            by2 = Obj_ALL_ST_P_Y[i] + Obj_ALL_Y[i];
                            // "Координата Z"
                            bz1 = Obj_ALL_ST_P_Z[i];
                            bz2 = Obj_ALL_ST_P_Z[i] + Obj_ALL_Z[i];
                            //
                            flag_krash_x = false;
                            flag_krash_y = false;
                            flag_krash_z = false;
                            //
                            krash_xy = false;
                            krash_xz = false;
                            krash_yz = false;
                            //
                            krash = false;
                            //     
                            // "Координата Х"    
                            if (((((ax1 >= bx1) & (ax1 <= bx2)) || ((ax2 >= bx1) & (ax2 <= bx2))) || (((bx1 >= ax1) & (bx1 <= ax2)) || ((bx2 >= ax1) & (bx2 <= ax2)))) & (!((ax1 == 0) & (ax2 == 0))) & (!((bx1 == 0) & (bx2 == 0))))
                            {
                                flag_krash_x = true;




                            }
                            // "Координата y"
                            if (((((ay1 >= by1) & (ay1 <= by2)) || ((ay2 >= by1) & (ay2 <= by2))) || (((by1 >= ay1) & (by1 <= ay2)) || ((by2 >= ay1) & (by2 <= ay2)))) & (!((ay1 == 0) & (ay2 == 0))) & (!((by1 == 0) & (by2 == 0))))
                            {
                                flag_krash_y = true;




                            }
                            // "Координата z"       
                            if (((((az1 >= bz1) & (az1 <= bz2)) || ((az2 >= bz1) & (az2 <= bz2))) || (((bz1 >= az1) & (bz1 <= az2)) || ((bz2 >= az1) & (bz2 <= az2)))) & (!((az1 == 0) & (az2 == 0))) & (!((bz1 == 0) & (bz2 == 0))))
                            {
                                flag_krash_z = true;


                            }
                            //
                            krash_xy = flag_krash_x & flag_krash_y;
                            krash_xz = flag_krash_x & flag_krash_z;
                            krash_yz = flag_krash_y & flag_krash_z;
                            //
                            krash = (krash_xy & krash_xz & krash_yz) || (krash_xy & krash_xz) || (krash_xz & krash_yz) || (krash_xy & krash_yz);
                            //
                            // ==================================================================================================
                            crane[j].KRASH_R_WARN[f, i] = false;
                            //
                            if (krash)
                            {
                                crane[j].KRASH_R_WARN[f, i] = true;

                                crane[j].R_KRASH_WARN[f] = true;
                                Obj_ALL_XYZ_KRASH_warn[i] = true;

                               //Write_ERR_WARN_ALD(int TEMP_j int TEMP_f, int TEMP_i, bool alarm, bool FLAG_ERR, bool FLAG_WARN, int zona_krash)
                                // 
                                // Какая зона
                                // 0 -   UP   - зона сверху
                                // 1 -   DOWN - зона снизу
                                // 2 -   F    - зона спереди
                                // 4 -   B    - зона сзади
                                // 8 -   R    - зона справа
                                // 16 -  L    - зона слева
                                //
                                if (!(crane[j].MEM_KRASH_R_WARN[f, i]))
                                {
                                   // Write_ERR_WARN_ALD(j, f, i, true, false, true, 8, alarmsView);
                                }
                                crane[j].MEM_KRASH_R_WARN[f, i] = true;
                            }
                            else
                            {
                                crane[j].MEM_KRASH_R_WARN[f, i] = false;
                            }
                            // ==================================================================================================
                        }
                    }
                    #endregion
                    #region ПРЕДУПРЕЖДЕНИЕ_L - ВЛЕВО
                    // ==================================================================================================
                    crane[j].L_KRASH_WARN[f] = false;
                    // "Координата Х"
                    ax1 = crane[j].L_stp_x_WARN[f];
                    ax2 = crane[j].L_stp_x_WARN[f] + crane[j].L_x_WARN[f];
                    // "Координата y"
                    ay1 = crane[j].L_stp_y_WARN[f];
                    ay2 = crane[j].L_stp_y_WARN[f] + crane[j].L_y_WARN[f];
                    // "Координата Z"
                    az1 = crane[j].L_stp_z_WARN[f];
                    az2 = crane[j].L_stp_z_WARN[f] + crane[j].L_z_WARN[f];
                    // ==================================================================================================
                    for (i = 1; i < 811; i++)
                    {
                        if (crane[j].KRASH_cntrl_yn[f, i] & Obj_ALL_Y_N[i])
                        {

                            // "Координата Х"                
                            bx1 = Obj_ALL_ST_P_X[i];
                            bx2 = Obj_ALL_ST_P_X[i] + Obj_ALL_X[i];
                            // "Координата y"
                            by1 = Obj_ALL_ST_P_Y[i];
                            by2 = Obj_ALL_ST_P_Y[i] + Obj_ALL_Y[i];
                            // "Координата Z"
                            bz1 = Obj_ALL_ST_P_Z[i];
                            bz2 = Obj_ALL_ST_P_Z[i] + Obj_ALL_Z[i];
                            //
                            flag_krash_x = false;
                            flag_krash_y = false;
                            flag_krash_z = false;
                            //
                            krash_xy = false;
                            krash_xz = false;
                            krash_yz = false;
                            //
                            krash = false;
                            //     
                            // "Координата Х"    
                            if (((((ax1 >= bx1) & (ax1 <= bx2)) || ((ax2 >= bx1) & (ax2 <= bx2))) || (((bx1 >= ax1) & (bx1 <= ax2)) || ((bx2 >= ax1) & (bx2 <= ax2)))) & (!((ax1 == 0) & (ax2 == 0))) & (!((bx1 == 0) & (bx2 == 0))))
                            {
                                flag_krash_x = true;
                            }
                            // "Координата y"
                            if (((((ay1 >= by1) & (ay1 <= by2)) || ((ay2 >= by1) & (ay2 <= by2))) || (((by1 >= ay1) & (by1 <= ay2)) || ((by2 >= ay1) & (by2 <= ay2)))) & (!((ay1 == 0) & (ay2 == 0))) & (!((by1 == 0) & (by2 == 0))))
                            {
                                flag_krash_y = true;
                            }
                            // "Координата z"       
                            if (((((az1 >= bz1) & (az1 <= bz2)) || ((az2 >= bz1) & (az2 <= bz2))) || (((bz1 >= az1) & (bz1 <= az2)) || ((bz2 >= az1) & (bz2 <= az2)))) & (!((az1 == 0) & (az2 == 0))) & (!((bz1 == 0) & (bz2 == 0))))
                            {
                                flag_krash_z = true;
                            }
                            //
                            krash_xy = flag_krash_x & flag_krash_y;
                            krash_xz = flag_krash_x & flag_krash_z;
                            krash_yz = flag_krash_y & flag_krash_z;
                            //
                            krash = (krash_xy & krash_xz & krash_yz) || (krash_xy & krash_xz) || (krash_xz & krash_yz) || (krash_xy & krash_yz);
                            //
                            // ==================================================================================================
                            crane[j].KRASH_L_WARN[f, i] = false;
                            //
                            if (krash)
                            {
                                crane[j].KRASH_L_WARN[f, i] = true;

                                crane[j].L_KRASH_WARN[f] = true;
                                Obj_ALL_XYZ_KRASH_warn[i] = true;

                                //Write_ERR_WARN_ALDint TEMP_j int TEMP_f, int TEMP_i, bool alarm, bool FLAG_ERR, bool FLAG_WARN, int zona_krash)
                                // 
                                // Какая зона
                                // 0 -   UP   - зона сверху
                                // 1 -   DOWN - зона снизу
                                // 2 -   F    - зона спереди
                                // 4 -   B    - зона сзади
                                // 8 -   R    - зона справа
                                // 16 -  L    - зона слева
                                //
                                if (!(crane[j].MEM_KRASH_L_WARN[f, i]))
                                {
                                   // Write_ERR_WARN_ALD(j, f, i, true, false, true, 16, alarmsView);
                                }
                                crane[j].MEM_KRASH_L_WARN[f, i] = true;
                            }
                            else
                            {
                                crane[j].MEM_KRASH_L_WARN[f, i] = false;
                            }
                            // ==================================================================================================
                        }
                    }
                    #endregion
                    #region ПРЕДУПРЕЖДЕНИЕ_F - ВПЕРЕД
                    // ==================================================================================================
                    crane[j].F_KRASH_WARN[f] = false;
                    // "Координата Х"
                    ax1 = crane[j].F_stp_x_WARN[f];
                    ax2 = crane[j].F_stp_x_WARN[f] + crane[j].F_x_WARN[f];
                    // "Координата y"
                    ay1 = crane[j].F_stp_y_WARN[f];
                    ay2 = crane[j].F_stp_y_WARN[f] + crane[j].F_y_WARN[f];
                    // "Координата Z"
                    az1 = crane[j].F_stp_z_WARN[f];
                    az2 = crane[j].F_stp_z_WARN[f] + crane[j].F_z_WARN[f];
                    // ==================================================================================================
                    for (i = 1; i < 811; i++)
                    {
                        if (crane[j].KRASH_cntrl_yn[f, i] & Obj_ALL_Y_N[i])
                        {

                            // "Координата Х"                
                            bx1 = Obj_ALL_ST_P_X[i];
                            bx2 = Obj_ALL_ST_P_X[i] + Obj_ALL_X[i];
                            // "Координата y"
                            by1 = Obj_ALL_ST_P_Y[i];
                            by2 = Obj_ALL_ST_P_Y[i] + Obj_ALL_Y[i];
                            // "Координата Z"
                            bz1 = Obj_ALL_ST_P_Z[i];
                            bz2 = Obj_ALL_ST_P_Z[i] + Obj_ALL_Z[i];
                            //
                            flag_krash_x = false;
                            flag_krash_y = false;
                            flag_krash_z = false;
                            //
                            krash_xy = false;
                            krash_xz = false;
                            krash_yz = false;
                            //
                            krash = false;
                            //     
                            // "Координата Х"    
                            if (((((ax1 >= bx1) & (ax1 <= bx2)) || ((ax2 >= bx1) & (ax2 <= bx2))) || (((bx1 >= ax1) & (bx1 <= ax2)) || ((bx2 >= ax1) & (bx2 <= ax2)))) & (!((ax1 == 0) & (ax2 == 0))) & (!((bx1 == 0) & (bx2 == 0))))
                            {
                                flag_krash_x = true;
                            }
                            // "Координата y"
                            if (((((ay1 >= by1) & (ay1 <= by2)) || ((ay2 >= by1) & (ay2 <= by2))) || (((by1 >= ay1) & (by1 <= ay2)) || ((by2 >= ay1) & (by2 <= ay2)))) & (!((ay1 == 0) & (ay2 == 0))) & (!((by1 == 0) & (by2 == 0))))
                            {
                                flag_krash_y = true;
                            }
                            // "Координата z"       
                            if (((((az1 >= bz1) & (az1 <= bz2)) || ((az2 >= bz1) & (az2 <= bz2))) || (((bz1 >= az1) & (bz1 <= az2)) || ((bz2 >= az1) & (bz2 <= az2)))) & (!((az1 == 0) & (az2 == 0))) & (!((bz1 == 0) & (bz2 == 0))))
                            {
                                flag_krash_z = true;
                            }
                            //
                            krash_xy = flag_krash_x & flag_krash_y;
                            krash_xz = flag_krash_x & flag_krash_z;
                            krash_yz = flag_krash_y & flag_krash_z;
                            //
                            krash = (krash_xy & krash_xz & krash_yz) || (krash_xy & krash_xz) || (krash_xz & krash_yz) || (krash_xy & krash_yz);
                            //
                            // ==================================================================================================
                            crane[j].KRASH_F_WARN[f, i] = false;
                            //
                            if (krash)
                            {
                                crane[j].KRASH_F_WARN[f, i] = true;

                                crane[j].F_KRASH_WARN[f] = true;
                                Obj_ALL_XYZ_KRASH_warn[i] = true;

                                //Write_ERR_WARN_ALDint TEMP_j int TEMP_f, int TEMP_i, bool alarm, bool FLAG_ERR, bool FLAG_WARN, int zona_krash)
                                // 
                                // Какая зона
                                // 0 -   UP   - зона сверху
                                // 1 -   DOWN - зона снизу
                                // 2 -   F    - зона спереди
                                // 4 -   B    - зона сзади
                                // 8 -   R    - зона справа
                                // 16 -  L    - зона слева
                                //
                                if (!(crane[j].MEM_KRASH_F_WARN[f, i]))
                                {
                                   // Write_ERR_WARN_ALD(j, f, i, true, false, true, 2, alarmsView);
                                }
                                crane[j].MEM_KRASH_F_WARN[f, i] = true;
                            }
                            else
                            {
                                crane[j].MEM_KRASH_F_WARN[f, i] = false;
                            }
                            // ==================================================================================================
                        }
                    }
                    #endregion
                    #region ПРЕДУПРЕЖДЕНИЕ_B - НАЗАД
                    // ==================================================================================================
                    crane[j].B_KRASH_WARN[f] = false;
                    // "Координата Х"
                    ax1 = crane[j].B_stp_x_WARN[f];
                    ax2 = crane[j].B_stp_x_WARN[f] + crane[j].B_x_WARN[f];
                    // "Координата y"
                    ay1 = crane[j].B_stp_y_WARN[f];
                    ay2 = crane[j].B_stp_y_WARN[f] + crane[j].B_y_WARN[f];
                    // "Координата Z"
                    az1 = crane[j].B_stp_z_WARN[f];
                    az2 = crane[j].B_stp_z_WARN[f] + crane[j].B_z_WARN[f];
                    // ==================================================================================================
                    for (i = 1; i < 811; i++)
                    {
                        if (crane[j].KRASH_cntrl_yn[f, i] & Obj_ALL_Y_N[i])
                        {

                            // "Координата Х"                
                            bx1 = Obj_ALL_ST_P_X[i];
                            bx2 = Obj_ALL_ST_P_X[i] + Obj_ALL_X[i];
                            // "Координата y"
                            by1 = Obj_ALL_ST_P_Y[i];
                            by2 = Obj_ALL_ST_P_Y[i] + Obj_ALL_Y[i];
                            // "Координата Z"
                            bz1 = Obj_ALL_ST_P_Z[i];
                            bz2 = Obj_ALL_ST_P_Z[i] + Obj_ALL_Z[i];
                            //
                            flag_krash_x = false;
                            flag_krash_y = false;
                            flag_krash_z = false;
                            //
                            krash_xy = false;
                            krash_xz = false;
                            krash_yz = false;
                            //
                            krash = false;
                            //     
                            // "Координата Х"    
                            if (((((ax1 >= bx1) & (ax1 <= bx2)) || ((ax2 >= bx1) & (ax2 <= bx2))) || (((bx1 >= ax1) & (bx1 <= ax2)) || ((bx2 >= ax1) & (bx2 <= ax2)))) & (!((ax1 == 0) & (ax2 == 0))) & (!((bx1 == 0) & (bx2 == 0))))
                            {
                                flag_krash_x = true;
                            }
                            // "Координата y"
                            if (((((ay1 >= by1) & (ay1 <= by2)) || ((ay2 >= by1) & (ay2 <= by2))) || (((by1 >= ay1) & (by1 <= ay2)) || ((by2 >= ay1) & (by2 <= ay2)))) & (!((ay1 == 0) & (ay2 == 0))) & (!((by1 == 0) & (by2 == 0))))
                            {
                                flag_krash_y = true;
                            }
                            // "Координата z"       
                            if (((((az1 >= bz1) & (az1 <= bz2)) || ((az2 >= bz1) & (az2 <= bz2))) || (((bz1 >= az1) & (bz1 <= az2)) || ((bz2 >= az1) & (bz2 <= az2)))) & (!((az1 == 0) & (az2 == 0))) & (!((bz1 == 0) & (bz2 == 0))))
                            {
                                flag_krash_z = true;
                            }
                            //
                            krash_xy = flag_krash_x & flag_krash_y;
                            krash_xz = flag_krash_x & flag_krash_z;
                            krash_yz = flag_krash_y & flag_krash_z;
                            //
                            krash = (krash_xy & krash_xz & krash_yz) || (krash_xy & krash_xz) || (krash_xz & krash_yz) || (krash_xy & krash_yz);
                            //
                            // ==================================================================================================
                            crane[j].KRASH_B_WARN[f, i] = false;
                            //
                            if (krash)
                            {
                                crane[j].KRASH_B_WARN[f, i] = true;

                                crane[j].B_KRASH_WARN[f] = true;
                             Obj_ALL_XYZ_KRASH_warn[i] = true;
                                
                                //Write_ERR_WARN_ALDint TEMP_j int TEMP_f, int TEMP_i, bool alarm, bool FLAG_ERR, bool FLAG_WARN, int zona_krash)
                                // 
                                // Какая зона
                                // 0 -   UP   - зона сверху
                                // 1 -   DOWN - зона снизу
                                // 2 -   F    - зона спереди
                                // 4 -   B    - зона сзади
                                // 8 -   R    - зона справа
                                // 16 -  L    - зона слева
                                //
                                if (!(crane[j].MEM_KRASH_B_WARN[f, i]))
                                {
                                   // Write_ERR_WARN_ALD(j, f, i, true, false, true, 4, alarmsView);
                                }
                                crane[j].MEM_KRASH_B_WARN[f, i] = true;
                            }
                            else
                            {
                                crane[j].MEM_KRASH_B_WARN[f, i] = false;
                            }
                            // ==================================================================================================
                        }
                    }
                    #endregion
                    // ==================================================================================================

                    // ======================================= АВАРИИ - ERROR ===========================================   
                    // ==================================================================================================
                    #region АВАРИЯ_UP
                    // ==================================================================================================  
                    crane[j].UP_KRASH_ERR[f] = false;
                    // "Координата Х"
                    ax1 = crane[j].UP_stp_x_ERR[f];
                    ax2 = crane[j].UP_stp_x_ERR[f] + crane[j].UP_x_ERR[f];
                    // "Координата y"
                    ay1 = crane[j].UP_stp_y_ERR[f];
                    ay2 = crane[j].UP_stp_y_ERR[f] + crane[j].UP_y_ERR[f];
                    // "Координата Z"
                    az1 = crane[j].UP_stp_z_ERR[f];
                    az2 = crane[j].UP_stp_z_ERR[f] + crane[j].UP_z_ERR[f];
                    // ==================================================================================================
                    for (i = 1; i < 811; i++)
                    {
                        if (crane[j].KRASH_cntrl_yn[f, i] & Obj_ALL_Y_N[i])
                        {

                            // "Координата Х"                
                            bx1 = Obj_ALL_ST_P_X[i];
                            bx2 = Obj_ALL_ST_P_X[i] + Obj_ALL_X[i];
                            // "Координата y"
                            by1 = Obj_ALL_ST_P_Y[i];
                            by2 = Obj_ALL_ST_P_Y[i] + Obj_ALL_Y[i];
                            // "Координата Z"
                            bz1 = Obj_ALL_ST_P_Z[i];
                            bz2 = Obj_ALL_ST_P_Z[i] + Obj_ALL_Z[i];
                            //
                            flag_krash_x = false;
                            flag_krash_y = false;
                            flag_krash_z = false;
                            //
                            krash_xy = false;
                            krash_xz = false;
                            krash_yz = false;
                            //
                            krash = false;
                            //     
                            // "Координата Х"    
                            if (((((ax1 >= bx1) & (ax1 <= bx2)) || ((ax2 >= bx1) & (ax2 <= bx2))) || (((bx1 >= ax1) & (bx1 <= ax2)) || ((bx2 >= ax1) & (bx2 <= ax2)))) & (!((ax1 == 0) & (ax2 == 0))) & (!((bx1 == 0) & (bx2 == 0))))
                            {
                                flag_krash_x = true;
                            }
                            // "Координата y"
                            if (((((ay1 >= by1) & (ay1 <= by2)) || ((ay2 >= by1) & (ay2 <= by2))) || (((by1 >= ay1) & (by1 <= ay2)) || ((by2 >= ay1) & (by2 <= ay2)))) & (!((ay1 == 0) & (ay2 == 0))) & (!((by1 == 0) & (by2 == 0))))
                            {
                                flag_krash_y = true;
                            }
                            // "Координата z"       
                            if (((((az1 >= bz1) & (az1 <= bz2)) || ((az2 >= bz1) & (az2 <= bz2))) || (((bz1 >= az1) & (bz1 <= az2)) || ((bz2 >= az1) & (bz2 <= az2)))) & (!((az1 == 0) & (az2 == 0))) & (!((bz1 == 0) & (bz2 == 0))))
                            {
                                flag_krash_z = true;
                            }
                            //
                            krash_xy = flag_krash_x & flag_krash_y;
                            krash_xz = flag_krash_x & flag_krash_z;
                            krash_yz = flag_krash_y & flag_krash_z;
                            //
                            krash = (krash_xy & krash_xz & krash_yz) || (krash_xy & krash_xz) || (krash_xz & krash_yz) || (krash_xy & krash_yz);
                            //
                            // ==================================================================================================
                            crane[j].KRASH_UP_ERR[f, i] = false;
                            //
                            if (krash)
                            {
                                crane[j].KRASH_UP_ERR[f, i] = true;

                                crane[j].UP_KRASH_ERR[f] = true;
                                Obj_ALL_XYZ_KRASH_err[i] = true;

                                //Write_ERR_WARN_ALDint TEMP_j int TEMP_f, int TEMP_i, bool alarm, bool FLAG_ERR, bool FLAG_WARN, int zona_krash)
                                // 
                                // Какая зона
                                // 0 -   UP   - зона сверху
                                // 1 -   DOWN - зона снизу
                                // 2 -   F    - зона спереди
                                // 4 -   B    - зона сзади
                                // 8 -   R    - зона справа
                                // 16 -  L    - зона слева
                                //
                                if (!(crane[j].MEM_KRASH_UP_ERR[f, i]))
                                {
                                   // Write_ERR_WARN_ALD(j, f, i, true, true, false, 0, alarmsView);
                                }
                                crane[j].MEM_KRASH_UP_ERR[f, i] = true;
                            }
                            else
                            {
                                crane[j].MEM_KRASH_UP_ERR[f, i] = false;
                            }
                            // ==================================================================================================
                        }
                    }
                    #endregion
                    #region АВАРИЯ_DOWN
                    // ==================================================================================================
                    crane[j].DOWN_KRASH_ERR[f] = false;
                    // "Координата Х"
                    ax1 = crane[j].DOWN_stp_x_ERR[f];
                    ax2 = crane[j].DOWN_stp_x_ERR[f] + crane[j].DOWN_x_ERR[f];
                    // "Координата y"
                    ay1 = crane[j].DOWN_stp_y_ERR[f];
                    ay2 = crane[j].DOWN_stp_y_ERR[f] + crane[j].DOWN_y_ERR[f];
                    // "Координата Z"
                    az1 = crane[j].DOWN_stp_z_ERR[f];
                    az2 = crane[j].DOWN_stp_z_ERR[f] + crane[j].DOWN_z_ERR[f];
                    // ==================================================================================================
                    for (i = 1; i < 811; i++)
                    {
                        if (crane[j].KRASH_cntrl_yn[f, i] & Obj_ALL_Y_N[i])
                        {
                            // "Координата Х"                
                            bx1 = Obj_ALL_ST_P_X[i];
                            bx2 = Obj_ALL_ST_P_X[i] + Obj_ALL_X[i];
                            // "Координата y"
                            by1 = Obj_ALL_ST_P_Y[i];
                            by2 = Obj_ALL_ST_P_Y[i] + Obj_ALL_Y[i];
                            // "Координата Z"
                            bz1 = Obj_ALL_ST_P_Z[i];
                            bz2 = Obj_ALL_ST_P_Z[i] + Obj_ALL_Z[i];
                            //
                            flag_krash_x = false;
                            flag_krash_y = false;
                            flag_krash_z = false;
                            //
                            krash_xy = false;
                            krash_xz = false;
                            krash_yz = false;
                            //
                            krash = false;
                            //     
                            // "Координата Х"    
                            if (((((ax1 >= bx1) & (ax1 <= bx2)) || ((ax2 >= bx1) & (ax2 <= bx2))) || (((bx1 >= ax1) & (bx1 <= ax2)) || ((bx2 >= ax1) & (bx2 <= ax2)))) & (!((ax1 == 0) & (ax2 == 0))) & (!((bx1 == 0) & (bx2 == 0))))
                            {
                                flag_krash_x = true;
                            }
                            // "Координата y"
                            if (((((ay1 >= by1) & (ay1 <= by2)) || ((ay2 >= by1) & (ay2 <= by2))) || (((by1 >= ay1) & (by1 <= ay2)) || ((by2 >= ay1) & (by2 <= ay2)))) & (!((ay1 == 0) & (ay2 == 0))) & (!((by1 == 0) & (by2 == 0))))
                            {
                                flag_krash_y = true;
                            }
                            // "Координата z"       
                            if (((((az1 >= bz1) & (az1 <= bz2)) || ((az2 >= bz1) & (az2 <= bz2))) || (((bz1 >= az1) & (bz1 <= az2)) || ((bz2 >= az1) & (bz2 <= az2)))) & (!((az1 == 0) & (az2 == 0))) & (!((bz1 == 0) & (bz2 == 0))))
                            {
                                flag_krash_z = true;
                            }
                            //
                            krash_xy = flag_krash_x & flag_krash_y;
                            krash_xz = flag_krash_x & flag_krash_z;
                            krash_yz = flag_krash_y & flag_krash_z;
                            //
                            krash = (krash_xy & krash_xz & krash_yz) || (krash_xy & krash_xz) || (krash_xz & krash_yz) || (krash_xy & krash_yz);
                            //
                            // ==================================================================================================
                            crane[j].KRASH_DOWN_ERR[f, i] = false;
                            //
                            if (krash)
                            {
                                crane[j].KRASH_DOWN_ERR[f, i] = true;

                                crane[j].DOWN_KRASH_ERR[f] = true;
                           Obj_ALL_XYZ_KRASH_err[i] = true;


                                //Write_ERR_WARN_ALDint TEMP_j int TEMP_f, int TEMP_i, bool alarm, bool FLAG_ERR, bool FLAG_WARN, int zona_krash)
                                // 
                                // Какая зона
                                // 0 -   UP   - зона сверху
                                // 1 -   DOWN - зона снизу
                                // 2 -   F    - зона спереди
                                // 4 -   B    - зона сзади
                                // 8 -   R    - зона справа
                                // 16 -  L    - зона слева
                                //
                                if (!(crane[j].MEM_KRASH_DOWN_ERR[f, i]))
                                {
                                   // Write_ERR_WARN_ALD(j, f, i, true, true, false, 1, alarmsView);
                                }
                                crane[j].MEM_KRASH_DOWN_ERR[f, i] = true;
                            }
                            else
                            {
                                crane[j].MEM_KRASH_DOWN_ERR[f, i] = false;
                            }
                            // ==================================================================================================
                        }

                    }
                    #endregion
                    #region АВАРИЯ_R
                    // ==================================================================================================
                    crane[j].R_KRASH_ERR[f] = false;
                    // "Координата Х"
                    ax1 = crane[j].R_stp_x_ERR[f];
                    ax2 = crane[j].R_stp_x_ERR[f] + crane[j].R_x_ERR[f];
                    // "Координата y"
                    ay1 = crane[j].R_stp_y_ERR[f];
                    ay2 = crane[j].R_stp_y_ERR[f] + crane[j].R_y_ERR[f];
                    // "Координата Z"
                    az1 = crane[j].R_stp_z_ERR[f];
                    az2 = crane[j].R_stp_z_ERR[f] + crane[j].R_z_ERR[f];
                    // ==================================================================================================
                    for (i = 1; i < 811; i++)
                    {
                        if (crane[j].KRASH_cntrl_yn[f, i] & Obj_ALL_Y_N[i])
                        {

                            // "Координата Х"                
                            bx1 = Obj_ALL_ST_P_X[i];
                            bx2 = Obj_ALL_ST_P_X[i] + Obj_ALL_X[i];
                            // "Координата y"
                            by1 = Obj_ALL_ST_P_Y[i];
                            by2 = Obj_ALL_ST_P_Y[i] + Obj_ALL_Y[i];
                            // "Координата Z"
                            bz1 = Obj_ALL_ST_P_Z[i];
                            bz2 = Obj_ALL_ST_P_Z[i] + Obj_ALL_Z[i];
                            //
                            flag_krash_x = false;
                            flag_krash_y = false;
                            flag_krash_z = false;
                            //
                            krash_xy = false;
                            krash_xz = false;
                            krash_yz = false;
                            //
                            krash = false;
                            //     
                            // "Координата Х"    
                            if (((((ax1 >= bx1) & (ax1 <= bx2)) || ((ax2 >= bx1) & (ax2 <= bx2))) || (((bx1 >= ax1) & (bx1 <= ax2)) || ((bx2 >= ax1) & (bx2 <= ax2)))) & (!((ax1 == 0) & (ax2 == 0))) & (!((bx1 == 0) & (bx2 == 0))))
                            {
                                flag_krash_x = true;
                            }
                            // "Координата y"
                            if (((((ay1 >= by1) & (ay1 <= by2)) || ((ay2 >= by1) & (ay2 <= by2))) || (((by1 >= ay1) & (by1 <= ay2)) || ((by2 >= ay1) & (by2 <= ay2)))) & (!((ay1 == 0) & (ay2 == 0))) & (!((by1 == 0) & (by2 == 0))))
                            {
                                flag_krash_y = true;
                            }
                            // "Координата z"       
                            if (((((az1 >= bz1) & (az1 <= bz2)) || ((az2 >= bz1) & (az2 <= bz2))) || (((bz1 >= az1) & (bz1 <= az2)) || ((bz2 >= az1) & (bz2 <= az2)))) & (!((az1 == 0) & (az2 == 0))) & (!((bz1 == 0) & (bz2 == 0))))
                            {
                                flag_krash_z = true;
                            }
                            //
                            krash_xy = flag_krash_x & flag_krash_y;
                            krash_xz = flag_krash_x & flag_krash_z;
                            krash_yz = flag_krash_y & flag_krash_z;
                            //
                            krash = (krash_xy & krash_xz & krash_yz) || (krash_xy & krash_xz) || (krash_xz & krash_yz) || (krash_xy & krash_yz);
                            //
                            // ==================================================================================================
                            crane[j].KRASH_R_ERR[f, i] = false;
                            //
                            if (krash)
                            {
                                crane[j].KRASH_R_ERR[f, i] = true;

                                crane[j].R_KRASH_ERR[f] = true;
                             Obj_ALL_XYZ_KRASH_err[i] = true;

                                //Write_ERR_WARN_ALDint TEMP_j int TEMP_f, int TEMP_i, bool alarm, bool FLAG_ERR, bool FLAG_WARN, int zona_krash)
                                // 
                                // Какая зона
                                // 0 -   UP   - зона сверху
                                // 1 -   DOWN - зона снизу
                                // 2 -   F    - зона спереди
                                // 4 -   B    - зона сзади
                                // 8 -   R    - зона справа
                                // 16 -  L    - зона слева
                                //
                                if (!(crane[j].MEM_KRASH_R_ERR[f, i]))
                                {
                                   // Write_ERR_WARN_ALD(j, f, i, true, true, false, 8, alarmsView);
                                }
                                crane[j].MEM_KRASH_R_ERR[f, i] = true;
                            }
                            else
                            {
                                crane[j].MEM_KRASH_R_ERR[f, i] = false;
                            }
                            // ==================================================================================================
                        }
                    }
                    #endregion
                    #region АВАРИЯ_L
                    // ==================================================================================================
                    crane[j].L_KRASH_ERR[f] = false;
                    // "Координата Х"
                    ax1 = crane[j].L_stp_x_ERR[f];
                    ax2 = crane[j].L_stp_x_ERR[f] + crane[j].L_x_ERR[f];
                    // "Координата y"
                    ay1 = crane[j].L_stp_y_ERR[f];
                    ay2 = crane[j].L_stp_y_ERR[f] + crane[j].L_y_ERR[f];
                    // "Координата Z"
                    az1 = crane[j].L_stp_z_ERR[f];
                    az2 = crane[j].L_stp_z_ERR[f] + crane[j].L_z_ERR[f];
                    // ==================================================================================================
                    for (i = 1; i < 811; i++)
                    {
                        if (crane[j].KRASH_cntrl_yn[f, i] & Obj_ALL_Y_N[i])
                        {

                            // "Координата Х"                
                            bx1 = Obj_ALL_ST_P_X[i];
                            bx2 = Obj_ALL_ST_P_X[i] + Obj_ALL_X[i];
                            // "Координата y"
                            by1 = Obj_ALL_ST_P_Y[i];
                            by2 = Obj_ALL_ST_P_Y[i] + Obj_ALL_Y[i];
                            // "Координата Z"
                            bz1 = Obj_ALL_ST_P_Z[i];
                            bz2 = Obj_ALL_ST_P_Z[i] + Obj_ALL_Z[i];
                            //
                            flag_krash_x = false;
                            flag_krash_y = false;
                            flag_krash_z = false;
                            //
                            krash_xy = false;
                            krash_xz = false;
                            krash_yz = false;
                            //
                            krash = false;
                            //     
                            // "Координата Х"    
                            if (((((ax1 >= bx1) & (ax1 <= bx2)) || ((ax2 >= bx1) & (ax2 <= bx2))) || (((bx1 >= ax1) & (bx1 <= ax2)) || ((bx2 >= ax1) & (bx2 <= ax2)))) & (!((ax1 == 0) & (ax2 == 0))) & (!((bx1 == 0) & (bx2 == 0))))
                            {
                                flag_krash_x = true;
                            }
                            // "Координата y"
                            if (((((ay1 >= by1) & (ay1 <= by2)) || ((ay2 >= by1) & (ay2 <= by2))) || (((by1 >= ay1) & (by1 <= ay2)) || ((by2 >= ay1) & (by2 <= ay2)))) & (!((ay1 == 0) & (ay2 == 0))) & (!((by1 == 0) & (by2 == 0))))
                            {
                                flag_krash_y = true;
                            }
                            // "Координата z"       
                            if (((((az1 >= bz1) & (az1 <= bz2)) || ((az2 >= bz1) & (az2 <= bz2))) || (((bz1 >= az1) & (bz1 <= az2)) || ((bz2 >= az1) & (bz2 <= az2)))) & (!((az1 == 0) & (az2 == 0))) & (!((bz1 == 0) & (bz2 == 0))))
                            {
                                flag_krash_z = true;
                            }
                            //
                            krash_xy = flag_krash_x & flag_krash_y;
                            krash_xz = flag_krash_x & flag_krash_z;
                            krash_yz = flag_krash_y & flag_krash_z;
                            //
                            krash = (krash_xy & krash_xz & krash_yz) || (krash_xy & krash_xz) || (krash_xz & krash_yz) || (krash_xy & krash_yz);
                            //
                            // ==================================================================================================
                            crane[j].KRASH_L_ERR[f, i] = false;
                            //
                            if (krash)
                            {
                                crane[j].KRASH_L_ERR[f, i] = true;

                                crane[j].L_KRASH_ERR[f] = true;
                                Obj_ALL_XYZ_KRASH_err[i] = true;

                                //Write_ERR_WARN_ALDint TEMP_j int TEMP_f, int TEMP_i, bool alarm, bool FLAG_ERR, bool FLAG_WARN, int zona_krash)
                                // 
                                // Какая зона
                                // 0 -   UP   - зона сверху
                                // 1 -   DOWN - зона снизу
                                // 2 -   F    - зона спереди
                                // 4 -   B    - зона сзади
                                // 8 -   R    - зона справа
                                // 16 -  L    - зона слева
                                //
                                if (!(crane[j].MEM_KRASH_L_ERR[f, i]))
                                {
                                   // Write_ERR_WARN_ALD(j, f, i, true, true, false, 16, alarmsView);
                                }
                                crane[j].MEM_KRASH_L_ERR[f, i] = true;
                            }
                            else
                            {
                                crane[j].MEM_KRASH_L_ERR[f, i] = false;
                            }
                            // ==================================================================================================
                        }
                    }
                    #endregion
                    #region АВАРИЯ_F
                    // ==================================================================================================
                    crane[j].F_KRASH_ERR[f] = false;
                    // "Координата Х"
                    ax1 = crane[j].F_stp_x_ERR[f];
                    ax2 = crane[j].F_stp_x_ERR[f] + crane[j].F_x_ERR[f];
                    // "Координата y"
                    ay1 = crane[j].F_stp_y_ERR[f];
                    ay2 = crane[j].F_stp_y_ERR[f] + crane[j].F_y_ERR[f];
                    // "Координата Z"
                    az1 = crane[j].F_stp_z_ERR[f];
                    az2 = crane[j].F_stp_z_ERR[f] + crane[j].F_z_ERR[f];
                    // ==================================================================================================
                    for (i = 1; i < 811; i++)
                    {
                        if (crane[j].KRASH_cntrl_yn[f, i] & Obj_ALL_Y_N[i])
                        {

                            // "Координата Х"                
                            bx1 = Obj_ALL_ST_P_X[i];
                            bx2 = Obj_ALL_ST_P_X[i] + Obj_ALL_X[i];
                            // "Координата y"
                            by1 = Obj_ALL_ST_P_Y[i];
                            by2 = Obj_ALL_ST_P_Y[i] + Obj_ALL_Y[i];
                            // "Координата Z"
                            bz1 = Obj_ALL_ST_P_Z[i];
                            bz2 = Obj_ALL_ST_P_Z[i] + Obj_ALL_Z[i];
                            //
                            flag_krash_x = false;
                            flag_krash_y = false;
                            flag_krash_z = false;
                            //
                            krash_xy = false;
                            krash_xz = false;
                            krash_yz = false;
                            //
                            krash = false;
                            //     
                            // "Координата Х"    
                            if (((((ax1 >= bx1) & (ax1 <= bx2)) || ((ax2 >= bx1) & (ax2 <= bx2))) || (((bx1 >= ax1) & (bx1 <= ax2)) || ((bx2 >= ax1) & (bx2 <= ax2)))) & (!((ax1 == 0) & (ax2 == 0))) & (!((bx1 == 0) & (bx2 == 0))))
                            {
                                flag_krash_x = true;
                            }
                            // "Координата y"
                            if (((((ay1 >= by1) & (ay1 <= by2)) || ((ay2 >= by1) & (ay2 <= by2))) || (((by1 >= ay1) & (by1 <= ay2)) || ((by2 >= ay1) & (by2 <= ay2)))) & (!((ay1 == 0) & (ay2 == 0))) & (!((by1 == 0) & (by2 == 0))))
                            {
                                flag_krash_y = true;
                            }
                            // "Координата z"       
                            if (((((az1 >= bz1) & (az1 <= bz2)) || ((az2 >= bz1) & (az2 <= bz2))) || (((bz1 >= az1) & (bz1 <= az2)) || ((bz2 >= az1) & (bz2 <= az2)))) & (!((az1 == 0) & (az2 == 0))) & (!((bz1 == 0) & (bz2 == 0))))
                            {
                                flag_krash_z = true;
                            }
                            //
                            krash_xy = flag_krash_x & flag_krash_y;
                            krash_xz = flag_krash_x & flag_krash_z;
                            krash_yz = flag_krash_y & flag_krash_z;
                            //
                            krash = (krash_xy & krash_xz & krash_yz) || (krash_xy & krash_xz) || (krash_xz & krash_yz) || (krash_xy & krash_yz);
                            //
                            // ==================================================================================================
                            crane[j].KRASH_F_ERR[f, i] = false;
                            //
                            if (krash)
                            {
                                crane[j].KRASH_F_ERR[f, i] = true;

                                crane[j].F_KRASH_ERR[f] = true;
                                Obj_ALL_XYZ_KRASH_err[i] = true;

                                //Write_ERR_WARN_ALDint TEMP_j int TEMP_f, int TEMP_i, bool alarm, bool FLAG_ERR, bool FLAG_WARN, int zona_krash)
                                // 
                                // Какая зона
                                // 0 -   UP   - зона сверху
                                // 1 -   DOWN - зона снизу
                                // 2 -   F    - зона спереди
                                // 4 -   B    - зона сзади
                                // 8 -   R    - зона справа
                                // 16 -  L    - зона слева
                                //
                                if (!(crane[j].MEM_KRASH_F_ERR[f, i]))
                                {
                                   // Write_ERR_WARN_ALD(j, f, i, true, true, false, 2, alarmsView);
                                }
                                crane[j].MEM_KRASH_F_ERR[f, i] = true;
                            }
                            else
                            {
                                crane[j].MEM_KRASH_F_ERR[f, i] = false;
                            }
                            // ==================================================================================================
                        }
                    }
                    #endregion
                    #region АВАРИЯ_B
                    // ==================================================================================================
                    crane[j].B_KRASH_ERR[f] = false;
                    // "Координата Х"
                    ax1 = crane[j].B_stp_x_ERR[f];
                    ax2 = crane[j].B_stp_x_ERR[f] + crane[j].B_x_ERR[f];
                    // "Координата y"
                    ay1 = crane[j].B_stp_y_ERR[f];
                    ay2 = crane[j].B_stp_y_ERR[f] + crane[j].B_y_ERR[f];
                    // "Координата Z"
                    az1 = crane[j].B_stp_z_ERR[f];
                    az2 = crane[j].B_stp_z_ERR[f] + crane[j].B_z_ERR[f];
                    // ==================================================================================================
                    for (i = 1; i < 811; i++)
                    {
                        if (crane[j].KRASH_cntrl_yn[f, i] & Obj_ALL_Y_N[i])
                        {
                            // "Координата Х"                
                            bx1 = Obj_ALL_ST_P_X[i];
                            bx2 = Obj_ALL_ST_P_X[i] + Obj_ALL_X[i];
                            // "Координата y"
                            by1 = Obj_ALL_ST_P_Y[i];
                            by2 = Obj_ALL_ST_P_Y[i] + Obj_ALL_Y[i];
                            // "Координата Z"
                            bz1 = Obj_ALL_ST_P_Z[i];
                            bz2 = Obj_ALL_ST_P_Z[i] + Obj_ALL_Z[i];
                            //
                            flag_krash_x = false;
                            flag_krash_y = false;
                            flag_krash_z = false;
                            //
                            krash_xy = false;
                            krash_xz = false;
                            krash_yz = false;
                            //
                            krash = false;
                            //     
                            // "Координата Х"    
                            if (((((ax1 >= bx1) & (ax1 <= bx2)) || ((ax2 >= bx1) & (ax2 <= bx2))) || (((bx1 >= ax1) & (bx1 <= ax2)) || ((bx2 >= ax1) & (bx2 <= ax2)))) & (!((ax1 == 0) & (ax2 == 0))) & (!((bx1 == 0) & (bx2 == 0))))
                            {
                                flag_krash_x = true;
                            }
                            // "Координата y"
                            if (((((ay1 >= by1) & (ay1 <= by2)) || ((ay2 >= by1) & (ay2 <= by2))) || (((by1 >= ay1) & (by1 <= ay2)) || ((by2 >= ay1) & (by2 <= ay2)))) & (!((ay1 == 0) & (ay2 == 0))) & (!((by1 == 0) & (by2 == 0))))
                            {
                                flag_krash_y = true;
                            }
                            // "Координата z"       
                            if (((((az1 >= bz1) & (az1 <= bz2)) || ((az2 >= bz1) & (az2 <= bz2))) || (((bz1 >= az1) & (bz1 <= az2)) || ((bz2 >= az1) & (bz2 <= az2)))) & (!((az1 == 0) & (az2 == 0))) & (!((bz1 == 0) & (bz2 == 0))))
                            {
                                flag_krash_z = true;
                            }
                            //
                            krash_xy = flag_krash_x & flag_krash_y;
                            krash_xz = flag_krash_x & flag_krash_z;
                            krash_yz = flag_krash_y & flag_krash_z;
                            //
                            krash = (krash_xy & krash_xz & krash_yz) || (krash_xy & krash_xz) || (krash_xz & krash_yz) || (krash_xy & krash_yz);
                            //
                            // ==================================================================================================
                            crane[j].KRASH_B_ERR[f, i] = false;
                            //
                            if (krash)
                            {
                                crane[j].KRASH_B_ERR[f, i] = true;

                                crane[j].B_KRASH_ERR[f] = true;
                                Obj_ALL_XYZ_KRASH_err[i] = true;

                                //Write_ERR_WARN_ALDint TEMP_j int TEMP_f, int TEMP_i, bool alarm, bool FLAG_ERR, bool FLAG_WARN, int zona_krash)
                                // 
                                // Какая зона
                                // 0 -   UP   - зона сверху
                                // 1 -   DOWN - зона снизу
                                // 2 -   F    - зона спереди
                                // 4 -   B    - зона сзади
                                // 8 -   R    - зона справа
                                // 16 -  L    - зона слева
                                //
                                if (!(crane[j].MEM_KRASH_B_ERR[f, i]))
                                {
                                   // Write_ERR_WARN_ALD(j, f, i, true, true, false, 4, alarmsView);
                                }
                                crane[j].MEM_KRASH_B_ERR[f, i] = true;
                            }
                            else
                            {
                                crane[j].MEM_KRASH_B_ERR[f, i] = false;
                            }
                            // ==================================================================================================
                        }
                    }
                    #endregion
                    // ==================================================================================================
                }
            }
        }
        // ==========================================================================================================
        #endregion
        // ======================================================================================
        #region ПР402 - "G-АЛГОРИТМ - part 2" - ФОРМИРОВАНИЕ ИТОГОВОГО ПРЕДУПРЕЖДЕНИЯ И АВАРИЙ ДЛЯ ДИНАМИЧЕСКИХ ОБЪЕКТОВ
        for (j = 1; j <= 3; j++)
        {
            #region - ИТОГОВОЕ ПРЕДУПРЕЖДЕНИЕ - ДЛЯ ДАЛЬНЕЙШЕЙ ПЕРЕДАЧИ В ТОМ ЧИСЛЕ И НА ПЛК
            // =======================================================================================================================================
            // РЕЖИМЫ РАБОТЫ ПОДЪЕМОВ
            // =======================================================================================================================================
            // 1 - траверс и груза нет
            // 2 - 1 + 2 вместе
            // 4 - 1 + 2 + 3 вместе
            // =======================================================================================================================================
            // ПРЕДУПРЕЖДЕНИЯ И АВАРИИ
            switch (crane[j].regim_w_hoist)
            {
                // ТАБЛИЦА СООТВЕТСТВИЯ
                // ===============================================================================================================================================================
                // n = 1 КРАН 320Т - 1-20/ 2 КРАН 120Т - (100 + (1-20)) => 101-120/ 3 КРАН 25Т - (200 + (1-20)) => 201-220
                // n =
                // 1 - МОСТ
                // 2 - ТЕЛЕЖКА 1  - ГЛАВНАЯ
                // 3 - ТЕЛЕЖКА 1 - ПОДЪЕМ 1 (1-1) - тросс + крюк
                // 4 - ---------------------------------------------------------------------------- НЕ ИСПОЛЬЗУЕТСЯ ДЛЯ ИТОГОВОГО ПРЕДУПРЕЖДЕНИЯ --- тросс + крюк + траверса
                // 5 - ТЕЛЕЖКА 1 - ПОДЪЕМ 1 (1-1) - ТРАВЕРСА
                // 6 - ТЕЛЕЖКА 1 - ПОДЪЕМ 1 (1-1) - груз
                // 7 - ТЕЛЕЖКА 1 - ПОДЪЕМ 2 (1-2) - тросс + крюк
                // 8 - ---------------------------------------------------------------------------- НЕ ИСПОЛЬЗУЕТСЯ ДЛЯ ИТОГОВОГО ПРЕДУПРЕЖДЕНИЯ --- тросс + крюк + траверса
                // 9 - ТЕЛЕЖКА 1 - ПОДЪЕМ 2 (1-2) - ТРАВЕРСА
                // 10 - ТЕЛЕЖКА 1 - ПОДЪЕМ 2 (1-2) - груз
                // 11 - ---------------------------------------------------------------------------- НЕ ИСПОЛЬЗУЕТСЯ ДЛЯ ИТОГОВОГО ПРЕДУПРЕЖДЕНИЯ --- тросс + крюк + траверса + 3 подъем тросс + крюк + траверса
                // 12 - ТЕЛЕЖКА 2  - ВСПОМОГАТЕЛЬНАЯ
                // 13 - ТЕЛЕЖКА 2 - ПОДЪЕМ 3 (2-1) - тросс + крюк
                // 14 - ---------------------------------------------------------------------------- НЕ ИСПОЛЬЗУЕТСЯ ДЛЯ ИТОГОВОГО ПРЕДУПРЕЖДЕНИЯ --- тросс + крюк + траверса
                // 15 - ТЕЛЕЖКА 2 - ПОДЪЕМ 3 (2-1) - ТРАВЕРСА
                // 16 - ТЕЛЕЖКА 2 - ПОДЪЕМ 3 (2-1) - груз
                // 17 - ТЕЛЕЖКА 2 - ПОДЪЕМ 4 (2-2) - тросс + крюк
                // 18 - ---------------------------------------------------------------------------- НЕ ИСПОЛЬЗУЕТСЯ ДЛЯ ИТОГОВОГО ПРЕДУПРЕЖДЕНИЯ --- тросс + крюк + траверса
                // 19 - ТЕЛЕЖКА 2 - ПОДЪЕМ 4 (2-2) - ТРАВЕРСА
                // 20 - ТЕЛЕЖКА 2 - ПОДЪЕМ 4 (2-2) - груз
                // ===============================================================================================================================================================                  
                case 2: //  1+2 ПОДЪЕМЫ - СОВМЕСТНАЯ РАБОТА
                        // АВАРИИ
                        // АВАРИИ ИТОГОВОЕ ДЛЯ НАПРАВЛЕНИЙ ДВИЖЕНИЯ МЕХАНИЗМОВ                
                    #region ПОДЪЕМ 1-1                
                    // ПОДЪЕМ
                    crane[j].BLOCK_H_1_1_UP_ERR =
                        crane[j].UP_KRASH_ERR[3] ||
                        crane[j].UP_KRASH_ERR[5] ||
                        crane[j].UP_KRASH_ERR[6] ||
                        crane[j].UP_KRASH_ERR[7] ||
                        crane[j].UP_KRASH_ERR[9] ||
                        crane[j].UP_KRASH_ERR[10];
                    // СПУСК
                    crane[j].BLOCK_H_1_1_DOWN_ERR =
                        crane[j].DOWN_KRASH_ERR[3] ||
                        crane[j].DOWN_KRASH_ERR[5] ||
                        crane[j].DOWN_KRASH_ERR[6] ||
                         crane[j].DOWN_KRASH_ERR[7] ||
                        crane[j].DOWN_KRASH_ERR[9] ||
                        crane[j].DOWN_KRASH_ERR[10];
                    #endregion
                    #region ПОДЪЕМ 1-2
                    // ПОДЪЕМ
                    crane[j].BLOCK_H_1_2_UP_ERR =
                        crane[j].UP_KRASH_ERR[3] ||
                        crane[j].UP_KRASH_ERR[5] ||
                        crane[j].UP_KRASH_ERR[6] ||
                        crane[j].UP_KRASH_ERR[7] ||
                        crane[j].UP_KRASH_ERR[9] ||
                        crane[j].UP_KRASH_ERR[10];
                    // СПУСК
                    crane[j].BLOCK_H_1_2_DOWN_ERR =
                        crane[j].DOWN_KRASH_ERR[3] ||
                        crane[j].DOWN_KRASH_ERR[5] ||
                        crane[j].DOWN_KRASH_ERR[6] ||
                        crane[j].DOWN_KRASH_ERR[7] ||
                        crane[j].DOWN_KRASH_ERR[9] ||
                        crane[j].DOWN_KRASH_ERR[10];
                    #endregion
                    #region ПОДЪЕМ 2-1
                    // ПОДЪЕМ
                    crane[j].BLOCK_H_2_1_UP_ERR =
                        crane[j].UP_KRASH_ERR[13] ||
                        crane[j].UP_KRASH_ERR[15] ||
                        crane[j].UP_KRASH_ERR[16];
                    // СПУСК
                    crane[j].BLOCK_H_2_1_DOWN_ERR =
                         crane[j].DOWN_KRASH_ERR[13] ||
                        crane[j].DOWN_KRASH_ERR[15] ||
                        crane[j].DOWN_KRASH_ERR[16];
                    #endregion
                    //
                    #region ТЕЛЕЖКА - 1             
                    // ВПЕРЕД
                    crane[j].BLOCK_TR_1_F_ERR =                       
                       crane[j].F_KRASH_ERR[5] ||
                       crane[j].F_KRASH_ERR[6] ||
                       crane[j].F_KRASH_ERR[9] ||
                       crane[j].F_KRASH_ERR[10] ||

                        crane[j].F_KRASH_ERR[2] ||
                        crane[j].F_KRASH_ERR[3] ||
                        crane[j].F_KRASH_ERR[7];
                    // НАЗАД
                    crane[j].BLOCK_TR_1_B_ERR =
                       crane[j].B_KRASH_ERR[5] ||
                       crane[j].B_KRASH_ERR[6] ||
                       crane[j].B_KRASH_ERR[9] ||
                       crane[j].B_KRASH_ERR[10] ||

                        crane[j].B_KRASH_ERR[2] ||
                        crane[j].B_KRASH_ERR[3] ||
                        crane[j].B_KRASH_ERR[7];
                    #endregion
                    #region ТЕЛЕЖКА - 2            
                    // ВПЕРЕД
                    crane[j].BLOCK_TR_2_F_ERR =
                       crane[j].F_KRASH_ERR[15] ||
                       crane[j].F_KRASH_ERR[16] ||
                       crane[j].F_KRASH_ERR[19] ||
                       crane[j].F_KRASH_ERR[20] ||

                        crane[j].F_KRASH_ERR[12] ||
                        crane[j].F_KRASH_ERR[13] ||
                        crane[j].F_KRASH_ERR[17];
                    // НАЗАД
                    crane[j].BLOCK_TR_2_B_ERR =
                       crane[j].B_KRASH_ERR[15] ||
                       crane[j].B_KRASH_ERR[16] ||
                       crane[j].B_KRASH_ERR[19] ||
                       crane[j].B_KRASH_ERR[20] ||

                        crane[j].B_KRASH_ERR[12] ||
                        crane[j].B_KRASH_ERR[13] ||
                        crane[j].B_KRASH_ERR[17];
                    #endregion
                    //  
                    // ПРЕДУПРЕЖДЕНИЯ
                    // ПРЕДУПРЕЖДЕНИЯ ИТОГОВОЕ ДЛЯ НАПРАВЛЕНИЙ ДВИЖЕНИЯ МЕХАНИЗМОВ
                    // ПРЕДУПРЕЖДЕНИЕ, ТАК ЖЕ ЗАВИСЯТ ОТ НАЛИЧИЯ ФЛАГА АВАРИИ ЭТОГО МЕХАНИЗМА
                    #region ПОДЪЕМ 1-1                
                    // ПОДЪЕМ
                    crane[j].BLOCK_H_1_1_UP_WARN =
                        crane[j].UP_KRASH_WARN[5] ||
                        crane[j].UP_KRASH_WARN[6] ||
                        crane[j].UP_KRASH_WARN[9] ||
                        crane[j].UP_KRASH_WARN[10]
                        ||
                        crane[j].BLOCK_H_1_1_UP_ERR
                        ||
                        crane[j].BLOCK_H_1_2_UP_ERR;
                    // СПУСК
                    crane[j].BLOCK_H_1_1_DOWN_WARN =
                        crane[j].DOWN_KRASH_WARN[5] ||
                        crane[j].DOWN_KRASH_WARN[6] ||
                        crane[j].DOWN_KRASH_WARN[9] ||
                        crane[j].DOWN_KRASH_WARN[10]
                        ||
                        crane[j].BLOCK_H_1_1_DOWN_ERR
                        ||
                        crane[j].BLOCK_H_1_2_DOWN_ERR;
                    #endregion
                    #region ПОДЪЕМ 1-2
                    // ПОДЪЕМ
                    crane[j].BLOCK_H_1_2_UP_WARN =
                        crane[j].UP_KRASH_WARN[5] ||
                        crane[j].UP_KRASH_WARN[6] ||
                        crane[j].UP_KRASH_WARN[9] ||
                        crane[j].UP_KRASH_WARN[10]
                        ||
                        crane[j].BLOCK_H_1_1_UP_ERR
                        ||
                        crane[j].BLOCK_H_1_2_UP_ERR;
                    // СПУСК
                    crane[j].BLOCK_H_1_2_DOWN_WARN =
                        crane[j].DOWN_KRASH_WARN[5] ||
                        crane[j].DOWN_KRASH_WARN[6] ||
                        crane[j].DOWN_KRASH_WARN[9] ||
                        crane[j].DOWN_KRASH_WARN[10]
                         ||
                        crane[j].BLOCK_H_1_1_DOWN_ERR
                         ||
                        crane[j].BLOCK_H_1_2_DOWN_ERR;
                    #endregion
                    #region ПОДЪЕМ 2-1
                    // ПОДЪЕМ
                    crane[j].BLOCK_H_2_1_UP_WARN =
                        crane[j].UP_KRASH_WARN[15] ||
                        crane[j].UP_KRASH_WARN[16]
                        ||
                        crane[j].BLOCK_H_2_1_UP_ERR;
                    // СПУСК
                    crane[j].BLOCK_H_2_1_DOWN_WARN =
                        crane[j].DOWN_KRASH_WARN[15] ||
                        crane[j].DOWN_KRASH_WARN[16]
                        ||
                        crane[j].BLOCK_H_2_1_DOWN_ERR;
                    #endregion
                    //
                    #region ТЕЛЕЖКА - 1             
                    // ВПЕРЕД
                    crane[j].BLOCK_TR_1_F_WARN =
                       crane[j].F_KRASH_WARN[5] ||
                       crane[j].F_KRASH_WARN[6] ||
                       crane[j].F_KRASH_WARN[9] ||
                       crane[j].F_KRASH_WARN[10] ||

                        crane[j].F_KRASH_WARN[2] ||
                        crane[j].F_KRASH_WARN[3] ||
                        crane[j].F_KRASH_WARN[7]
                        ||
                        crane[j].BLOCK_TR_1_F_ERR;
                    // НАЗАД
                    crane[j].BLOCK_TR_1_B_WARN =
                       crane[j].B_KRASH_WARN[5] ||
                       crane[j].B_KRASH_WARN[6] ||
                       crane[j].B_KRASH_WARN[9] ||
                       crane[j].B_KRASH_WARN[10] ||

                        crane[j].B_KRASH_WARN[2] ||
                        crane[j].B_KRASH_WARN[3] ||
                        crane[j].B_KRASH_WARN[7]
                        ||
                        crane[j].BLOCK_TR_1_B_ERR;
                    #endregion
                    #region ТЕЛЕЖКА - 2            
                    // ВПЕРЕД
                    crane[j].BLOCK_TR_2_F_WARN =
                       crane[j].F_KRASH_WARN[15] ||
                       crane[j].F_KRASH_WARN[16] ||
                       crane[j].F_KRASH_WARN[19] ||
                       crane[j].F_KRASH_WARN[20] ||

                        crane[j].F_KRASH_WARN[12] ||
                        crane[j].F_KRASH_WARN[13] ||
                        crane[j].F_KRASH_WARN[17]
                        ||
                        crane[j].BLOCK_TR_2_F_ERR;
                    // НАЗАД
                    crane[j].BLOCK_TR_2_B_WARN =
                       crane[j].B_KRASH_WARN[15] ||
                       crane[j].B_KRASH_WARN[16] ||
                       crane[j].B_KRASH_WARN[19] ||
                       crane[j].B_KRASH_WARN[20] ||

                        crane[j].B_KRASH_WARN[12] ||
                        crane[j].B_KRASH_WARN[13] ||
                        crane[j].B_KRASH_WARN[17]
                        ||
                        crane[j].BLOCK_TR_2_B_ERR;
                    #endregion
                    //                                           
                    break;
                case 4:
                    // АВАРИИ
                    // АВАРИИ ИТОГОВЫЕ ДЛЯ НАПРАВЛЕНИЙ ДВИЖЕНИЯ МЕХАНИЗМОВ        
                    #region ПОДЪЕМ 1-1                
                    // ПОДЪЕМ
                    crane[j].BLOCK_H_1_1_UP_ERR =
                        crane[j].UP_KRASH_ERR[3] ||
                        crane[j].UP_KRASH_ERR[5] ||
                        crane[j].UP_KRASH_ERR[6] ||
                        crane[j].UP_KRASH_ERR[7] ||
                        crane[j].UP_KRASH_ERR[9] ||
                        crane[j].UP_KRASH_ERR[10] ||
                        crane[j].UP_KRASH_ERR[13] ||
                        crane[j].UP_KRASH_ERR[15] ||
                        crane[j].UP_KRASH_ERR[16];
                    // СПУСК
                    crane[j].BLOCK_H_1_1_DOWN_ERR =
                        crane[j].DOWN_KRASH_ERR[3] ||
                        crane[j].DOWN_KRASH_ERR[5] ||
                        crane[j].DOWN_KRASH_ERR[6] ||
                        crane[j].DOWN_KRASH_ERR[7] ||
                        crane[j].DOWN_KRASH_ERR[9] ||
                        crane[j].DOWN_KRASH_ERR[10] ||
                        crane[j].DOWN_KRASH_ERR[13] ||
                        crane[j].DOWN_KRASH_ERR[15] ||
                        crane[j].DOWN_KRASH_ERR[16];
                    #endregion
                    #region ПОДЪЕМ 1-2
                    // ПОДЪЕМ
                    crane[j].BLOCK_H_1_2_UP_ERR =
                        crane[j].UP_KRASH_ERR[3] ||
                        crane[j].UP_KRASH_ERR[5] ||
                        crane[j].UP_KRASH_ERR[6] ||
                         crane[j].UP_KRASH_ERR[7] ||
                        crane[j].UP_KRASH_ERR[9] ||
                        crane[j].UP_KRASH_ERR[10] ||
                        crane[j].UP_KRASH_ERR[13] ||
                        crane[j].UP_KRASH_ERR[15] ||
                        crane[j].UP_KRASH_ERR[16];
                    // СПУСК
                    crane[j].BLOCK_H_1_2_DOWN_ERR =
                        crane[j].DOWN_KRASH_ERR[3] ||
                        crane[j].DOWN_KRASH_ERR[5] ||
                        crane[j].DOWN_KRASH_ERR[6] ||
                         crane[j].DOWN_KRASH_ERR[7] ||
                        crane[j].DOWN_KRASH_ERR[9] ||
                        crane[j].DOWN_KRASH_ERR[10] ||
                        crane[j].DOWN_KRASH_ERR[13] ||
                        crane[j].DOWN_KRASH_ERR[15] ||
                        crane[j].DOWN_KRASH_ERR[16];
                    #endregion
                    #region ПОДЪЕМ 2-1
                    // ПОДЪЕМ
                    crane[j].BLOCK_H_2_1_UP_ERR =
                        crane[j].UP_KRASH_ERR[3] ||
                        crane[j].UP_KRASH_ERR[5] ||
                        crane[j].UP_KRASH_ERR[6] ||
                        crane[j].UP_KRASH_ERR[7] ||
                        crane[j].UP_KRASH_ERR[9] ||
                        crane[j].UP_KRASH_ERR[10] ||
                        crane[j].UP_KRASH_ERR[13] ||
                        crane[j].UP_KRASH_ERR[15] ||
                        crane[j].UP_KRASH_ERR[16];
                    // СПУСК
                    crane[j].BLOCK_H_2_1_DOWN_ERR =
                        crane[j].DOWN_KRASH_ERR[3] ||
                        crane[j].DOWN_KRASH_ERR[5] ||
                        crane[j].DOWN_KRASH_ERR[6] ||
                        crane[j].DOWN_KRASH_ERR[7] ||
                        crane[j].DOWN_KRASH_ERR[9] ||
                        crane[j].DOWN_KRASH_ERR[10] ||
                        crane[j].DOWN_KRASH_ERR[13] ||
                        crane[j].DOWN_KRASH_ERR[15] ||
                        crane[j].DOWN_KRASH_ERR[16];
                    #endregion
                    //
                    #region ТЕЛЕЖКА - 1             
                    // ВПЕРЕД
                    crane[j].BLOCK_TR_1_F_ERR =
                       crane[j].F_KRASH_ERR[5] ||
                       crane[j].F_KRASH_ERR[6] ||
                       crane[j].F_KRASH_ERR[9] ||
                       crane[j].F_KRASH_ERR[10] ||
                       crane[j].F_KRASH_ERR[2] ||
                       crane[j].F_KRASH_ERR[3] ||
                       crane[j].F_KRASH_ERR[7] ||
                       crane[j].F_KRASH_ERR[15] ||
                       crane[j].F_KRASH_ERR[16] ||
                       crane[j].F_KRASH_ERR[19] ||
                       crane[j].F_KRASH_ERR[20] ||
                       crane[j].F_KRASH_ERR[12] ||
                       crane[j].F_KRASH_ERR[13] ||
                       crane[j].F_KRASH_ERR[17];
                    // НАЗАД
                    crane[j].BLOCK_TR_1_B_ERR =
                       crane[j].B_KRASH_ERR[5] ||
                       crane[j].B_KRASH_ERR[6] ||
                       crane[j].B_KRASH_ERR[9] ||
                       crane[j].B_KRASH_ERR[10] ||

                        crane[j].B_KRASH_ERR[2] ||
                        crane[j].B_KRASH_ERR[3] ||
                        crane[j].B_KRASH_ERR[7]
                        ||                        
                        crane[j].B_KRASH_ERR[15] ||
                       crane[j].B_KRASH_ERR[16] ||
                       crane[j].B_KRASH_ERR[19] ||
                       crane[j].B_KRASH_ERR[20] ||

                        crane[j].B_KRASH_ERR[12] ||
                        crane[j].B_KRASH_ERR[13] ||
                        crane[j].B_KRASH_ERR[17];
                    #endregion
                    #region ТЕЛЕЖКА - 2            
                    // ВПЕРЕД
                    crane[j].BLOCK_TR_2_F_ERR =
                       crane[j].F_KRASH_ERR[5] ||
                       crane[j].F_KRASH_ERR[6] ||
                       crane[j].F_KRASH_ERR[9] ||
                       crane[j].F_KRASH_ERR[10] ||

                        crane[j].F_KRASH_ERR[2] ||
                        crane[j].F_KRASH_ERR[3] ||
                        crane[j].F_KRASH_ERR[7]
                        ||                         
                         crane[j].F_KRASH_ERR[15] ||
                       crane[j].F_KRASH_ERR[16] ||
                       crane[j].F_KRASH_ERR[19] ||
                       crane[j].F_KRASH_ERR[20] ||

                        crane[j].F_KRASH_ERR[12] ||
                        crane[j].F_KRASH_ERR[13] ||
                        crane[j].F_KRASH_ERR[17];
                    // НАЗАД
                    crane[j].BLOCK_TR_2_B_ERR =
                       crane[j].B_KRASH_ERR[5] ||
                       crane[j].B_KRASH_ERR[6] ||
                       crane[j].B_KRASH_ERR[9] ||
                       crane[j].B_KRASH_ERR[10] ||

                        crane[j].B_KRASH_ERR[2] ||
                        crane[j].B_KRASH_ERR[3] ||
                        crane[j].B_KRASH_ERR[7]
                        ||
                        crane[j].B_KRASH_ERR[15] ||
                       crane[j].B_KRASH_ERR[16] ||
                       crane[j].B_KRASH_ERR[19] ||
                       crane[j].B_KRASH_ERR[20] ||

                        crane[j].B_KRASH_ERR[12] ||
                        crane[j].B_KRASH_ERR[13] ||
                        crane[j].B_KRASH_ERR[17];
                    #endregion
                    //   
                    // ПРЕДУПРЕЖДЕНИЯ
                    // ПРЕДУПРЕЖДЕНИЯ ИТОГОВОЕ ДЛЯ НАПРАВЛЕНИЙ ДВИЖЕНИЯ МЕХАНИЗМОВ
                    // ПРЕДУПРЕЖДЕНИЕ, ТАК ЖЕ ЗАВИСЯТ ОТ НАЛИЧИЯ ФЛАГА АВАРИИ ЭТОГО МЕХАНИЗМА
                    #region ПОДЪЕМ 1-1                
                    // ПОДЪЕМ
                    crane[j].BLOCK_H_1_1_UP_WARN =
                        crane[j].UP_KRASH_WARN[5] ||
                        crane[j].UP_KRASH_WARN[6] ||
                        crane[j].UP_KRASH_WARN[9] ||
                        crane[j].UP_KRASH_WARN[10] ||
                        crane[j].UP_KRASH_WARN[15] ||
                        crane[j].UP_KRASH_WARN[16]
                        ||
                        crane[j].BLOCK_H_1_1_UP_ERR
                        ||
                        crane[j].BLOCK_H_1_2_UP_ERR
                        ||
                        crane[j].BLOCK_H_2_1_UP_ERR;
                    // СПУСК
                    crane[j].BLOCK_H_1_1_DOWN_WARN =
                        crane[j].DOWN_KRASH_WARN[5] ||
                        crane[j].DOWN_KRASH_WARN[6] ||
                        crane[j].DOWN_KRASH_WARN[9] ||
                        crane[j].DOWN_KRASH_WARN[10] ||
                        crane[j].DOWN_KRASH_WARN[15] ||
                        crane[j].DOWN_KRASH_WARN[16]
                        ||
                        crane[j].BLOCK_H_1_1_DOWN_ERR
                        ||
                        crane[j].BLOCK_H_1_2_DOWN_ERR
                        ||
                        crane[j].BLOCK_H_2_1_DOWN_ERR;
                    #endregion
                    #region ПОДЪЕМ 1-2
                    // ПОДЪЕМ
                    crane[j].BLOCK_H_1_2_UP_WARN =
                        crane[j].UP_KRASH_WARN[5] ||
                        crane[j].UP_KRASH_WARN[6] ||
                        crane[j].UP_KRASH_WARN[9] ||
                        crane[j].UP_KRASH_WARN[10] ||
                        crane[j].UP_KRASH_WARN[15] ||
                        crane[j].UP_KRASH_WARN[16]
                        ||
                        crane[j].BLOCK_H_1_1_UP_ERR
                        ||
                        crane[j].BLOCK_H_1_2_UP_ERR
                        ||
                        crane[j].BLOCK_H_2_1_UP_ERR;
                    // СПУСК
                    crane[j].BLOCK_H_1_2_DOWN_WARN =
                        crane[j].DOWN_KRASH_WARN[5] ||
                        crane[j].DOWN_KRASH_WARN[6] ||
                        crane[j].DOWN_KRASH_WARN[9] ||
                        crane[j].DOWN_KRASH_WARN[10] ||
                        crane[j].DOWN_KRASH_WARN[15] ||
                        crane[j].DOWN_KRASH_WARN[16]
                        ||
                        crane[j].BLOCK_H_1_1_DOWN_ERR
                        ||
                        crane[j].BLOCK_H_1_2_DOWN_ERR
                        ||
                        crane[j].BLOCK_H_2_1_DOWN_ERR;
                    #endregion
                    #region ПОДЪЕМ 2-1
                    // ПОДЪЕМ
                    crane[j].BLOCK_H_2_1_UP_WARN =
                        crane[j].UP_KRASH_WARN[5] ||
                        crane[j].UP_KRASH_WARN[6] ||
                        crane[j].UP_KRASH_WARN[9] ||
                        crane[j].UP_KRASH_WARN[10] ||
                        crane[j].UP_KRASH_WARN[15] ||
                        crane[j].UP_KRASH_WARN[16]
                        ||
                        crane[j].BLOCK_H_1_1_UP_ERR
                        ||
                        crane[j].BLOCK_H_1_2_UP_ERR
                        ||
                        crane[j].BLOCK_H_2_1_UP_ERR;
                    // СПУСК
                    crane[j].BLOCK_H_2_1_DOWN_WARN =
                        crane[j].DOWN_KRASH_WARN[5] ||
                        crane[j].DOWN_KRASH_WARN[6] ||
                        crane[j].DOWN_KRASH_WARN[9] ||
                        crane[j].DOWN_KRASH_WARN[10] ||
                        crane[j].DOWN_KRASH_WARN[15] ||
                        crane[j].DOWN_KRASH_WARN[16]
                        ||
                        crane[j].BLOCK_H_1_1_DOWN_ERR
                        ||
                        crane[j].BLOCK_H_1_2_DOWN_ERR
                        ||
                        crane[j].BLOCK_H_2_1_DOWN_ERR;
                    #endregion
                    //
                    #region ТЕЛЕЖКА - 1             
                    // ВПЕРЕД
                    crane[j].BLOCK_TR_1_F_WARN =
                       crane[j].F_KRASH_WARN[5] ||
                       crane[j].F_KRASH_WARN[6] ||
                       crane[j].F_KRASH_WARN[9] ||
                       crane[j].F_KRASH_WARN[10] ||

                        crane[j].F_KRASH_WARN[2] ||
                        crane[j].F_KRASH_WARN[3] ||
                        crane[j].F_KRASH_WARN[7]
                        ||
                         crane[j].F_KRASH_WARN[15] ||
                       crane[j].F_KRASH_WARN[16] ||
                       crane[j].F_KRASH_WARN[19] ||
                       crane[j].F_KRASH_WARN[20] ||

                        crane[j].F_KRASH_WARN[12] ||
                        crane[j].F_KRASH_WARN[13] ||
                        crane[j].F_KRASH_WARN[17]
                        ||
                        crane[j].BLOCK_TR_1_F_ERR
                        ||
                        crane[j].BLOCK_TR_2_F_ERR;
                    // НАЗАД
                    crane[j].BLOCK_TR_1_B_WARN =
                       crane[j].B_KRASH_WARN[5] ||
                       crane[j].B_KRASH_WARN[6] ||
                       crane[j].B_KRASH_WARN[9] ||
                       crane[j].B_KRASH_WARN[10] ||

                        crane[j].B_KRASH_WARN[2] ||
                        crane[j].B_KRASH_WARN[3] ||
                        crane[j].B_KRASH_WARN[7]
                        ||
                        crane[j].B_KRASH_WARN[15] ||
                       crane[j].B_KRASH_WARN[16] ||
                       crane[j].B_KRASH_WARN[19] ||
                       crane[j].B_KRASH_WARN[20] ||

                        crane[j].B_KRASH_WARN[12] ||
                        crane[j].B_KRASH_WARN[13] ||
                        crane[j].B_KRASH_WARN[17]
                        ||
                        crane[j].BLOCK_TR_1_B_ERR
                        ||
                        crane[j].BLOCK_TR_2_B_ERR;
                    #endregion
                    #region ТЕЛЕЖКА - 2            
                    // ВПЕРЕД
                    crane[j].BLOCK_TR_2_F_WARN =
                       crane[j].F_KRASH_WARN[5] ||
                       crane[j].F_KRASH_WARN[6] ||
                       crane[j].F_KRASH_WARN[9] ||
                       crane[j].F_KRASH_WARN[10] ||

                        crane[j].F_KRASH_WARN[2] ||
                        crane[j].F_KRASH_WARN[3] ||
                        crane[j].F_KRASH_WARN[7]
                        ||
                         crane[j].F_KRASH_WARN[15] ||
                       crane[j].F_KRASH_WARN[16] ||
                       crane[j].F_KRASH_WARN[19] ||
                       crane[j].F_KRASH_WARN[20] ||

                        crane[j].F_KRASH_WARN[12] ||
                        crane[j].F_KRASH_WARN[13] ||
                        crane[j].F_KRASH_WARN[17]
                        ||
                        crane[j].BLOCK_TR_1_F_ERR
                        ||
                        crane[j].BLOCK_TR_2_F_ERR;
                    // НАЗАД
                    crane[j].BLOCK_TR_2_B_WARN =
                       crane[j].B_KRASH_WARN[5] ||
                       crane[j].B_KRASH_WARN[6] ||
                       crane[j].B_KRASH_WARN[9] ||
                       crane[j].B_KRASH_WARN[10] ||

                        crane[j].B_KRASH_WARN[2] ||
                        crane[j].B_KRASH_WARN[3] ||
                        crane[j].B_KRASH_WARN[7]
                        ||
                        crane[j].B_KRASH_WARN[15] ||
                       crane[j].B_KRASH_WARN[16] ||
                       crane[j].B_KRASH_WARN[19] ||
                       crane[j].B_KRASH_WARN[20] ||

                        crane[j].B_KRASH_WARN[12] ||
                        crane[j].B_KRASH_WARN[13] ||
                        crane[j].B_KRASH_WARN[17]
                        ||
                        crane[j].BLOCK_TR_1_B_ERR
                        ||
                        crane[j].BLOCK_TR_2_B_ERR;
                    #endregion
                    //   
                    break;
                default:
                    // АВАРИИ
                    // АВАРИИ ИТОГОВЫЕ ДЛЯ НАПРАВЛЕНИЙ ДВИЖЕНИЯ МЕХАНИЗМОВ       
                    #region ПОДЪЕМ 1-1                
                    // ПОДЪЕМ
                    crane[j].BLOCK_H_1_1_UP_ERR =
                        crane[j].UP_KRASH_ERR[3] ||
                        crane[j].UP_KRASH_ERR[5] ||
                        crane[j].UP_KRASH_ERR[6];
                    // СПУСК
                    crane[j].BLOCK_H_1_1_DOWN_ERR =
                        crane[j].DOWN_KRASH_ERR[3] ||
                        crane[j].DOWN_KRASH_ERR[5] ||
                        crane[j].DOWN_KRASH_ERR[6];
                    #endregion
                    #region ПОДЪЕМ 1-2
                    // ПОДЪЕМ
                    crane[j].BLOCK_H_1_2_UP_ERR =
                        crane[j].UP_KRASH_ERR[7] ||
                        crane[j].UP_KRASH_ERR[9] ||
                        crane[j].UP_KRASH_ERR[10];
                    // СПУСК
                    crane[j].BLOCK_H_1_2_DOWN_ERR =
                        crane[j].DOWN_KRASH_ERR[7] ||
                        crane[j].DOWN_KRASH_ERR[9] ||
                        crane[j].DOWN_KRASH_ERR[10];
                    #endregion
                    #region ПОДЪЕМ 2-1
                    // ПОДЪЕМ
                    crane[j].BLOCK_H_2_1_UP_ERR =
                        crane[j].UP_KRASH_ERR[13] ||
                        crane[j].UP_KRASH_ERR[15] ||
                        crane[j].UP_KRASH_ERR[16];
                    // СПУСК
                    crane[j].BLOCK_H_2_1_DOWN_ERR =
                        crane[j].DOWN_KRASH_ERR[13] ||
                        crane[j].DOWN_KRASH_ERR[15] ||
                        crane[j].DOWN_KRASH_ERR[16];
                    #endregion
                    //
                    #region ТЕЛЕЖКА - 1             
                    // ВПЕРЕД
                    crane[j].BLOCK_TR_1_F_ERR =
                       crane[j].F_KRASH_ERR[5] ||
                       crane[j].F_KRASH_ERR[6] ||
                       crane[j].F_KRASH_ERR[9] ||
                       crane[j].F_KRASH_ERR[10] ||

                        crane[j].F_KRASH_ERR[2] ||
                        crane[j].F_KRASH_ERR[3] ||
                        crane[j].F_KRASH_ERR[7];
                    // НАЗАД
                    crane[j].BLOCK_TR_1_B_ERR =
                       crane[j].B_KRASH_ERR[5] ||
                       crane[j].B_KRASH_ERR[6] ||
                       crane[j].B_KRASH_ERR[9] ||
                       crane[j].B_KRASH_ERR[10] ||

                        crane[j].B_KRASH_ERR[2] ||
                        crane[j].B_KRASH_ERR[3] ||
                        crane[j].B_KRASH_ERR[7];
                    #endregion
                    #region ТЕЛЕЖКА - 2            
                    // ВПЕРЕД
                    crane[j].BLOCK_TR_2_F_ERR =
                       crane[j].F_KRASH_ERR[15] ||
                       crane[j].F_KRASH_ERR[16] ||
                       crane[j].F_KRASH_ERR[19] ||
                       crane[j].F_KRASH_ERR[20] ||

                        crane[j].F_KRASH_ERR[12] ||
                        crane[j].F_KRASH_ERR[13] ||
                        crane[j].F_KRASH_ERR[17];
                    // НАЗАД
                    crane[j].BLOCK_TR_2_B_ERR =
                       crane[j].B_KRASH_ERR[15] ||
                       crane[j].B_KRASH_ERR[16] ||
                       crane[j].B_KRASH_ERR[19] ||
                       crane[j].B_KRASH_ERR[20] ||

                        crane[j].B_KRASH_ERR[12] ||
                        crane[j].B_KRASH_ERR[13] ||
                        crane[j].B_KRASH_ERR[17];
                    #endregion
                    // 
                    // ПРЕДУПРЕЖДЕНИЯ
                    // ПРЕДУПРЕЖДЕНИЯ ИТОГОВОЕ ДЛЯ НАПРАВЛЕНИЙ ДВИЖЕНИЯ МЕХАНИЗМОВ
                    // ПРЕДУПРЕЖДЕНИЕ, ТАК ЖЕ ЗАВИСЯТ ОТ НАЛИЧИЯ ФЛАГА АВАРИИ ЭТОГО МЕХАНИЗМА
                    #region ПОДЪЕМ 1-1                
                    // ПОДЪЕМ
                    crane[j].BLOCK_H_1_1_UP_WARN =
                        crane[j].UP_KRASH_WARN[5] ||
                        crane[j].UP_KRASH_WARN[6]
                        ||
                        crane[j].BLOCK_H_1_1_UP_ERR;
                    // СПУСК
                    crane[j].BLOCK_H_1_1_DOWN_WARN =
                        crane[j].DOWN_KRASH_WARN[5] ||
                        crane[j].DOWN_KRASH_WARN[6]
                        ||
                        crane[j].BLOCK_H_1_1_DOWN_ERR;
                    #endregion
                    #region ПОДЪЕМ 1-2
                    // ПОДЪЕМ
                    crane[j].BLOCK_H_1_2_UP_WARN =
                        crane[j].UP_KRASH_WARN[9] ||
                        crane[j].UP_KRASH_WARN[10]
                        ||
                        crane[j].BLOCK_H_1_2_UP_ERR;
                    // СПУСК
                    crane[j].BLOCK_H_1_2_DOWN_WARN =
                        crane[j].DOWN_KRASH_WARN[9] ||
                        crane[j].DOWN_KRASH_WARN[10]
                         ||
                        crane[j].BLOCK_H_1_2_DOWN_ERR;
                    #endregion
                    #region ПОДЪЕМ 2-1
                    // ПОДЪЕМ
                    crane[j].BLOCK_H_2_1_UP_WARN =
                        crane[j].UP_KRASH_WARN[15] ||
                        crane[j].UP_KRASH_WARN[16]
                        ||
                        crane[j].BLOCK_H_2_1_UP_ERR;
                    // СПУСК
                    crane[j].BLOCK_H_2_1_DOWN_WARN =
                        crane[j].DOWN_KRASH_WARN[15] ||
                        crane[j].DOWN_KRASH_WARN[16]
                        ||
                        crane[j].BLOCK_H_2_1_DOWN_ERR;
                    #endregion
                    //
                    #region ТЕЛЕЖКА - 1             
                    // ВПЕРЕД
                    crane[j].BLOCK_TR_1_F_WARN =
                       crane[j].F_KRASH_WARN[5] ||
                       crane[j].F_KRASH_WARN[6] ||
                       crane[j].F_KRASH_WARN[9] ||
                       crane[j].F_KRASH_WARN[10] ||

                        crane[j].F_KRASH_WARN[2] ||
                        crane[j].F_KRASH_WARN[3] ||
                        crane[j].F_KRASH_WARN[7]
                        ||
                        crane[j].BLOCK_TR_1_F_ERR;
                    // НАЗАД
                    crane[j].BLOCK_TR_1_B_WARN =
                       crane[j].B_KRASH_WARN[5] ||
                       crane[j].B_KRASH_WARN[6] ||
                       crane[j].B_KRASH_WARN[9] ||
                       crane[j].B_KRASH_WARN[10] ||

                        crane[j].B_KRASH_WARN[2] ||
                        crane[j].B_KRASH_WARN[3] ||
                        crane[j].B_KRASH_WARN[7]
                        ||
                        crane[j].BLOCK_TR_1_B_ERR;
                    #endregion
                    #region ТЕЛЕЖКА - 2            
                    // ВПЕРЕД
                    crane[j].BLOCK_TR_2_F_WARN =
                       crane[j].F_KRASH_WARN[15] ||
                       crane[j].F_KRASH_WARN[16] ||
                       crane[j].F_KRASH_WARN[19] ||
                       crane[j].F_KRASH_WARN[20] ||

                        crane[j].F_KRASH_WARN[12] ||
                        crane[j].F_KRASH_WARN[13] ||
                        crane[j].F_KRASH_WARN[17]
                        ||
                        crane[j].BLOCK_TR_2_F_ERR;
                    // НАЗАД
                    crane[j].BLOCK_TR_2_B_WARN =
                       crane[j].B_KRASH_WARN[15] ||
                       crane[j].B_KRASH_WARN[16] ||
                       crane[j].B_KRASH_WARN[19] ||
                       crane[j].B_KRASH_WARN[20] ||

                        crane[j].B_KRASH_WARN[12] ||
                        crane[j].B_KRASH_WARN[13] ||
                        crane[j].B_KRASH_WARN[17]
                        ||
                        crane[j].BLOCK_TR_2_B_ERR;
                    #endregion
                    //   
                    break;
            }
            // =======================================================================================================================================
            // ТАК КАК РАБОТА 4 ПОДЪЕМА И МОСТА НЕ СВЯЗАНА С РЕЖИМОМ РАБОТЫ 1/2/3 ПОДЪЕМОВ - ОБРАБОТКА НЕЗАВИСИМО ОТ РЕЖИМА РАБОТЫ ПОДЪЕМОВ
            // =======================================================================================================================================
            // АВАРИИ
            #region ПОДЪЕМ 2-2
            // ПОДЪЕМ
            crane[j].BLOCK_H_2_2_UP_ERR =
                crane[j].UP_KRASH_ERR[17] ||
                crane[j].UP_KRASH_ERR[19] ||
                crane[j].UP_KRASH_ERR[20];
            // СПУСК
            crane[j].BLOCK_H_2_2_DOWN_ERR =
                 crane[j].DOWN_KRASH_ERR[17] ||
                crane[j].DOWN_KRASH_ERR[19] ||
                crane[j].DOWN_KRASH_ERR[20];
            #endregion
            #region МОСТ           
            // ВПРАВО
            crane[j].BLOCK_BR_R_ERR =
                crane[j].R_KRASH_ERR[1] ||

                crane[j].R_KRASH_ERR[2] ||
                crane[j].R_KRASH_ERR[3] ||

                crane[j].R_KRASH_ERR[5] ||
                crane[j].R_KRASH_ERR[6] ||
                crane[j].R_KRASH_ERR[7] ||

                crane[j].R_KRASH_ERR[9] ||
                crane[j].R_KRASH_ERR[10] ||

                crane[j].R_KRASH_ERR[12] ||
                crane[j].R_KRASH_ERR[13] ||

                crane[j].R_KRASH_ERR[15] ||
                crane[j].R_KRASH_ERR[16] ||
                crane[j].R_KRASH_ERR[17] ||

                crane[j].R_KRASH_ERR[19] ||
                crane[j].R_KRASH_ERR[20];
            // ВЛЕВО
            crane[j].BLOCK_BR_L_ERR =
                 crane[j].L_KRASH_ERR[1] ||

                crane[j].L_KRASH_ERR[2] ||
                crane[j].L_KRASH_ERR[3] ||

                crane[j].L_KRASH_ERR[5] ||
                crane[j].L_KRASH_ERR[6] ||
                crane[j].L_KRASH_ERR[7] ||

                crane[j].L_KRASH_ERR[9] ||
                crane[j].L_KRASH_ERR[10] ||

                crane[j].L_KRASH_ERR[12] ||
                crane[j].L_KRASH_ERR[13] ||

                crane[j].L_KRASH_ERR[15] ||
                crane[j].L_KRASH_ERR[16] ||
                crane[j].L_KRASH_ERR[17] ||

                crane[j].L_KRASH_ERR[19] ||
                crane[j].L_KRASH_ERR[20];
            #endregion
            // =======================================================================================================================================
            // ПРЕДУПРЕЖДЕНИЕ
            // ПРЕДУПРЕЖДЕНИЕ, ТАК ЖЕ ЗАВИСЯТ ОТ НАЛИЧИЯ ФЛАГА АВАРИИ ЭТОГО МЕХАНИЗМА
            #region ПОДЪЕМ 2-2
            // ПОДЪЕМ
            crane[j].BLOCK_H_2_2_UP_WARN =
                crane[j].UP_KRASH_WARN[19] ||
                crane[j].UP_KRASH_WARN[20]
                ||
                crane[j].BLOCK_H_2_2_UP_ERR;
            // СПУСК
            crane[j].BLOCK_H_2_2_DOWN_WARN =
                crane[j].DOWN_KRASH_WARN[19] ||
                crane[j].DOWN_KRASH_WARN[20]
                 ||
                crane[j].BLOCK_H_2_2_DOWN_ERR;
            #endregion
            #region МОСТ           
            // ВПРАВО
            crane[j].BLOCK_BR_R_WARN =
                crane[j].R_KRASH_WARN[1] ||

                crane[j].R_KRASH_WARN[2] ||
                crane[j].R_KRASH_WARN[3] ||

                crane[j].R_KRASH_WARN[5] ||
                crane[j].R_KRASH_WARN[6] ||
                crane[j].R_KRASH_WARN[7] ||

                crane[j].R_KRASH_WARN[9] ||
                crane[j].R_KRASH_WARN[10] ||

                crane[j].R_KRASH_WARN[12] ||
                crane[j].R_KRASH_WARN[13] ||

                crane[j].R_KRASH_WARN[15] ||
                crane[j].R_KRASH_WARN[16] ||
                crane[j].R_KRASH_WARN[17] ||

                crane[j].R_KRASH_WARN[19] ||
                crane[j].R_KRASH_WARN[20]
                ||
                crane[j].BLOCK_BR_R_ERR;
            // ВЛЕВО
            crane[j].BLOCK_BR_L_WARN =
                 crane[j].L_KRASH_WARN[1] ||

                crane[j].L_KRASH_WARN[2] ||
                crane[j].L_KRASH_WARN[3] ||

                crane[j].L_KRASH_WARN[5] ||
                crane[j].L_KRASH_WARN[6] ||
                crane[j].L_KRASH_WARN[7] ||

                crane[j].L_KRASH_WARN[9] ||
                crane[j].L_KRASH_WARN[10] ||

                crane[j].L_KRASH_WARN[12] ||
                crane[j].L_KRASH_WARN[13] ||

                crane[j].L_KRASH_WARN[15] ||
                crane[j].L_KRASH_WARN[16] ||
                crane[j].L_KRASH_WARN[17] ||

                crane[j].L_KRASH_WARN[19] ||
                crane[j].L_KRASH_WARN[20]
                ||
                crane[j].BLOCK_BR_L_ERR;
            #endregion
            // =======================================================================================================================================
            #endregion
        }
        #endregion
        // ======================================================================================
        // ------------------------------------------------------------------------------------------------------------------------------------------------------------
        // NNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNN
        // FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
        // --- START WRITE --------------------------------------------------------------------------------------------------------------------------------------------
        // ВЫХОДНЫЕ ДАННЫЕ
        // ВЫХОДНЫЕ ДАННЫЕ - ДЛЯ НИКИТЫ Ф.
        // ------------------------------------------------------------------------------------------------------------------------------------------------------------
        #region ПР00x - ЗАПИСЬ ВЫХОДНЫХ ДАННЫХ ДЛЯ ПОСЛЕДУЮЩЕЙ ПЕРЕДАЧИ НА ПЛК И ФОРМИРОВАНИЯ ИНДИКАЦИИ
        //bool TEMP_DQ_BOOL = false;
        // ПРЕДЛАГАЮ РАЗМЕСТИТЬ НА ЭКРАННЫХ ФОРМАХ СТРЕЛОЧКИ С ПРИВЯЗКОЙ К ЭЛЕМЕНТАМ МЕХАНИЗМА КРАНА
        // ПЕРЕМЕЩЕНИЕ МОСТА - ВЛЕВО/ВПРАВО
        // НА КАЖДЫЙ ПОДЪЕМ - ВВЕРХ/ВНИЗ
        // НА КАЖДУЮ ТЕЛЕЖКУ - ВПЕРЕД/НАЗАД
        // "ЗЕЛЕНАЯ" - ДВИЖЕНИЕ РАЗРЕШЕНО
        // "ЖЕЛТАЯ" - ДВИЖЕНИЕ ОГРАНИЧЕНО
        // "КРАСНАЯ" - ДВИЖЕНИЕ ЗАПРЕТ
        // ОТКАЗАТЬСЯ ОТ ЦВЕТОВОЙ МАРКИРОВКИ ЭЛЕМЕНТОВ
        // ОСТАВИТЬ ЦВЕТОВУЮ МАРКИРОВКУ УСЛОВНО СТАТИЧЕСКИХ ОБЪЕКТОВ
        #region WRITE DATA - ФЛАГИ НАЛИЧИЯ ПРЕДУПРЕЖДЕНИЙ И АВАРИЙ ДЛЯ УСЛОВНО СТАТИЧЕСКИХ ОБЪЕКТОВ

        // ФЛАГИ СТОЛКНОВЕНИЯ ДЛЯ ВСЕХ СТАТИЧЕСКИХ ОБЪЕКТОВ
        // НОМЕР условно статического объекта - i
        // 1-20 - КРАН 1 - 320Т
        // 0/21-50 - КРАН 1 - 320Т - РЕЗЕРВ
        // 31-99 - ТОЛЬКО СТАТИЧНЫЕ ОБЪЕКТЫ - КОЛОННЫ И СТЕНЫ
        // 101-120 - КРАН 2 - 120Т
        // 100/121-150 - КРАН 2 - 120Т - РЕЗЕРВ
        // 131 - 199 - ДОПОЛНИТЕЛЬНЫЕ ФИКСИРОВАННЫЕ ПРЕПЯТСТВИЯ
        // 151-199 - ТОЛЬКО СТАТИЧНЫЕ ОБЪЕКТЫ - РЕЗЕРВ
        // 201-220 - КРАН 3 - 25Т
        // 200/221-250 - 221-299 - ДЛЯ АРХИВИРОВАНИЯ ИЗМЕНЕНИЙ ПО ПРЕПЯТСТВИЯМ
        // 251-299 - ТОЛЬКО СТАТИЧНЫЕ ОБЪЕКТЫ - РЕЗЕРВ
        // 301 - 811 - ТОЛЬКО СТАТИЧНЫЕ ОБЪЕКТЫ - ПРЕПЯТСТВИЯ

        //забираем список созданных препятствий в цехе
        var obstacleInstances = obstaclesScript.GetObstacleInstances();
        _i = 301;
        //Окрашиваем препятствия в цехе
        foreach (var obstacle in obstacleInstances)
        {
            var ms = obstacle.GetComponentInChildren<MaterialSwitch>();
            ms.warning = Obj_ALL_XYZ_KRASH_warn[_i];
            ms.alarm = Obj_ALL_XYZ_KRASH_err[_i];
            _i++;
        }

        // Окрашиваем колонны
        _i = 31;
        foreach (var column in columns)
        {
            column.warning = Obj_ALL_XYZ_KRASH_warn[_i];
            column.alarm = Obj_ALL_XYZ_KRASH_err[_i];
            _i++;
        }

        #endregion
        #region DATA_WRITE КРАНЫ - ОГРАНИЧЕНИЯ - ПРЕДУПРЕЖДЕНИЯ И АВАРИИ

        j = 1;



        foreach (var craneScript in craneScripts)
        {
            var craneConnectionScript = craneScript.gameObject.GetComponent<CraneConnection>();
            //не удается найти скрипт крана, выходим
            if (craneConnectionScript == null)
                return;
            var j = craneScripts.FindIndex(c => c == craneScript) + 1;
            // =====================================================================================
            // СФОРМИРОВАННЫЕ ОГРАНИЧЕНИЯ - ПРЕДУПРЕЖДЕНИЯ - "ЖЕЛТЫЙ"
            
            ref var disables = ref craneConnectionScript.disables;
            // ПОДЪЕМЫ
            disables.mtMhUpWarning = crane[j].BLOCK_H_1_1_UP_WARN;           
            disables.mtMhDownWarning = crane[j].BLOCK_H_1_1_DOWN_WARN;

            disables.mtAhUpWarning = crane[j].BLOCK_H_1_2_UP_WARN;
            disables.mtAhDownWarning = crane[j].BLOCK_H_1_2_DOWN_WARN;

            disables.atMhUpWarning = crane[j].BLOCK_H_2_1_UP_WARN;
            disables.atMhDownWarning = crane[j].BLOCK_H_2_1_DOWN_WARN;

            disables.atAhUpWarning = crane[j].BLOCK_H_2_2_UP_WARN;
            disables.atAhDownWarning = crane[j].BLOCK_H_2_2_DOWN_WARN;

            // ТЕЛЕЖКИ
            disables.mtForwardWarning = crane[j].BLOCK_TR_1_F_WARN;
            disables.mtBackwardWarning = crane[j].BLOCK_TR_1_B_WARN;

            disables.atForwardWarning = crane[j].BLOCK_TR_2_F_WARN;
            disables.atBackwardWarning = crane[j].BLOCK_TR_2_B_WARN;

            // МОСТ
            disables.bridgeRightWarning = crane[j].BLOCK_BR_R_WARN;
            disables.bridgeLeftWarning = crane[j].BLOCK_BR_L_WARN;

            // СФОРМИРОВАННЫЕ ОГРАНИЧЕНИЯ - АВАРИИ - "КРАСНЫЙ"
            // ПОДЪЕМЫ
            disables.mtMhUpAlarm = crane[j].BLOCK_H_1_1_UP_ERR;
            disables.mtMhDownAlarm = crane[j].BLOCK_H_1_1_DOWN_ERR;

            disables.mtAhUpAlarm = crane[j].BLOCK_H_1_2_UP_ERR;
            disables.mtAhDownAlarm = crane[j].BLOCK_H_1_2_DOWN_ERR;

            disables.atMhUpAlarm = crane[j].BLOCK_H_2_1_UP_ERR;
            disables.atMhDownAlarm = crane[j].BLOCK_H_2_1_DOWN_ERR;

            disables.atAhUpAlarm = crane[j].BLOCK_H_2_2_UP_ERR;
            disables.atAhDownAlarm = crane[j].BLOCK_H_2_2_DOWN_ERR;

            // ТЕЛЕЖКИ
            disables.mtForwardAlarm = crane[j].BLOCK_TR_1_F_ERR;
            disables.mtBackwardAlarm = crane[j].BLOCK_TR_1_B_ERR;

            disables.atForwardAlarm = crane[j].BLOCK_TR_2_F_ERR;
            disables.atBackwardAlarm = crane[j].BLOCK_TR_2_B_ERR;
            // МОСТ
            disables.bridgeRightAlarm = crane[j].BLOCK_BR_R_ERR;
            disables.bridgeLeftAlarm = crane[j].BLOCK_BR_L_ERR;
            // =====================================================================================
            // 0 - зона сверху
            // 1 - зона снизу
            // 2 - зона спереди
            // 4 - зона сзади
            // 8 - зона справа
            // 16 - зона слева


            //1 МОСТ
            //2 ТЕЛЕЖКА 1
            //3 ТЕЛЕЖКА 1 - ПОДЪЕМ 1 
            //4 ТЕЛЕЖКА 1 - ПОДЪЕМ 2 
            //5 ТЕЛЕЖКА 2                                                       
            //6 ТЕЛЕЖКА 2 - ПОДЪЕМ 1
            //7 ТЕЛЕЖКА 2 - ПОДЪЕМ 2 


            // WriteWarning_Alarm_ald(int temp_j, int temp_mech, int Temp_zona_krash, int WARN_OR_ERR, bool temp_M_WARN_ERR, bool temp_WARN_ERR, Alarms logger)
            #region ФОРМИРУЕМ СООБЩЕНИЕ ПРЕДУПРЕЖДЕНИЯ ОБЩЕЕ
            if ((!(Globals.MEM_B_BR_R_WARN[j])) & (crane[j].BLOCK_BR_R_WARN))
            {
                WriteWarning_Alarm_ald(j, 1, 8, 1, alarmsView);
                Globals.MEM_B_BR_R_WARN[j] = true;
            }
            if (!(crane[j].BLOCK_BR_R_WARN))
            {
                Globals.MEM_B_BR_R_WARN[j] = false;
            }
            if ((!(Globals.MEM_B_BR_L_WARN[j])) & (crane[j].BLOCK_BR_L_WARN))
            {
                WriteWarning_Alarm_ald(j, 1, 16, 1, alarmsView);
                Globals.MEM_B_BR_L_WARN[j] = true;
            }
            if (!(crane[j].BLOCK_BR_L_WARN))
            {
                Globals.MEM_B_BR_L_WARN[j] = false;
            }

            if ((!(Globals.MEM_B_TR_1_F_WARN[j])) & (crane[j].BLOCK_TR_1_F_WARN))
            {
                WriteWarning_Alarm_ald(j, 2, 2, 1, alarmsView);
                Globals.MEM_B_TR_1_F_WARN[j] = true;
            }
            if (!(crane[j].BLOCK_TR_1_F_WARN))
            {
                Globals.MEM_B_TR_1_F_WARN[j] = false;
            }
            if ((!(Globals.MEM_B_TR_1_B_WARN[j])) & (crane[j].BLOCK_TR_1_B_WARN))
            {
                WriteWarning_Alarm_ald(j, 2, 4, 1, alarmsView);
                Globals.MEM_B_TR_1_B_WARN[j] = true;
            }
            if (!(crane[j].BLOCK_TR_1_B_WARN))
            {
                Globals.MEM_B_TR_1_B_WARN[j] = false;
            }

            if ((!(Globals.MEM_B_TR_2_F_WARN[j])) & (crane[j].BLOCK_TR_2_F_WARN))
            {
                WriteWarning_Alarm_ald(j, 5, 2, 1, alarmsView);
                Globals.MEM_B_TR_2_F_WARN[j] = true;
            }
            if (!(crane[j].BLOCK_TR_2_F_WARN))
            {
                Globals.MEM_B_TR_2_F_WARN[j] = false;
            }
            if ((!(Globals.MEM_B_TR_2_B_WARN[j])) & (crane[j].BLOCK_TR_2_B_WARN))
            {
                WriteWarning_Alarm_ald(j, 5, 4, 1, alarmsView);
                Globals.MEM_B_TR_2_B_WARN[j] = true;
            }
            if (!(crane[j].BLOCK_TR_2_B_WARN))
            {
                Globals.MEM_B_TR_2_B_WARN[j] = false;
            }

            if ((!(Globals.MEM_B_H_1_1_UP_WARN[j])) & (crane[j].BLOCK_H_1_1_UP_WARN))
            {
                WriteWarning_Alarm_ald(j, 3, 0, 1, alarmsView);
                Globals.MEM_B_H_1_1_UP_WARN[j] = true;
            }
            if (!(crane[j].BLOCK_H_1_1_UP_WARN))
            {
                Globals.MEM_B_H_1_1_UP_WARN[j] = false;
            }
            if ((!(Globals.MEM_B_H_1_1_DOWN_WARN[j])) & (crane[j].BLOCK_H_1_1_DOWN_WARN))
            {
                WriteWarning_Alarm_ald(j, 3, 1, 1, alarmsView);
                Globals.MEM_B_H_1_1_DOWN_WARN[j] = true;
            }
            if (!(crane[j].BLOCK_H_1_1_DOWN_WARN))
            {
                Globals.MEM_B_H_1_1_DOWN_WARN[j] = false;
            }

            if ((!(Globals.MEM_B_H_1_2_UP_WARN[j])) & (crane[j].BLOCK_H_1_2_UP_WARN))
            {
                WriteWarning_Alarm_ald(j, 4, 0, 1, alarmsView);
                Globals.MEM_B_H_1_2_UP_WARN[j] = true;
            }
            if (!(crane[j].BLOCK_H_1_2_UP_WARN))
            {
                Globals.MEM_B_H_1_2_UP_WARN[j] = false;
            }
            if ((!(Globals.MEM_B_H_1_2_DOWN_WARN[j])) & (crane[j].BLOCK_H_1_2_DOWN_WARN))
            {
                WriteWarning_Alarm_ald(j, 4, 1, 1, alarmsView);
                Globals.MEM_B_H_1_2_DOWN_WARN[j] = true;
            }
            if (!(crane[j].BLOCK_H_1_2_DOWN_WARN))
            {
                Globals.MEM_B_H_1_2_DOWN_WARN[j] = false;
            }

            if ((!(Globals.MEM_B_H_2_1_UP_WARN[j])) & (crane[j].BLOCK_H_2_1_UP_WARN))
            {
                WriteWarning_Alarm_ald(j, 6, 0, 1, alarmsView);
                Globals.MEM_B_H_2_1_UP_WARN[j] = true;
            }
            if (!(crane[j].BLOCK_H_2_1_UP_WARN))
            {
                Globals.MEM_B_H_2_1_UP_WARN[j] = false;
            }
            if ((!(Globals.MEM_B_H_2_1_DOWN_WARN[j])) & (crane[j].BLOCK_H_2_1_DOWN_WARN))
            {
                WriteWarning_Alarm_ald(j, 6, 1, 1, alarmsView);
                Globals.MEM_B_H_2_1_DOWN_WARN[j] = true;
            }
            if (!(crane[j].BLOCK_H_2_1_DOWN_WARN))
            {
                Globals.MEM_B_H_2_1_DOWN_WARN[j] = false;
            }

            if ((!(Globals.MEM_B_H_2_2_UP_WARN[j])) & (crane[j].BLOCK_H_2_2_UP_WARN))
            {
                WriteWarning_Alarm_ald(j, 7, 0, 1, alarmsView);
                Globals.MEM_B_H_2_2_UP_WARN[j] = true;
            }
            if (!(crane[j].BLOCK_H_2_2_UP_WARN))
            {
                Globals.MEM_B_H_2_2_UP_WARN[j] = false;
            }
            if ((!(Globals.MEM_B_H_2_2_DOWN_WARN[j])) & (crane[j].BLOCK_H_2_2_DOWN_WARN))
            {
                WriteWarning_Alarm_ald(j, 7, 1, 1, alarmsView);
                Globals.MEM_B_H_2_2_DOWN_WARN[j] = true;
            }
            if (!(crane[j].BLOCK_H_2_2_DOWN_WARN))
            {
                Globals.MEM_B_H_2_2_DOWN_WARN[j] = false;
            }

            #endregion
            #region ФОРМИРУЕМ СООБЩЕНИЕ СТОЛКНОВЕНИЕ ОБЩЕЕ
            if ((!(Globals.MEM_B_BR_R_ERR[j])) & (crane[j].BLOCK_BR_R_ERR))
            {
                WriteWarning_Alarm_ald(j, 1, 8, 2, alarmsView);
                Globals.MEM_B_BR_R_ERR[j] = true;
            }
            if (!(crane[j].BLOCK_BR_R_ERR))
            {
                Globals.MEM_B_BR_R_ERR[j] = false;
            }
            if ((!(Globals.MEM_B_BR_L_ERR[j])) & (crane[j].BLOCK_BR_L_ERR))
            {
                WriteWarning_Alarm_ald(j, 1, 16, 2, alarmsView);
                Globals.MEM_B_BR_L_ERR[j] = true;
            }
            if (!(crane[j].BLOCK_BR_L_ERR))
            {
                Globals.MEM_B_BR_L_ERR[j] = false;
            }

            if ((!(Globals.MEM_B_TR_1_F_ERR[j])) & (crane[j].BLOCK_TR_1_F_ERR))
            {
                WriteWarning_Alarm_ald(j, 2, 2, 2, alarmsView);
                Globals.MEM_B_TR_1_F_ERR[j] = true;
            }
            if (!(crane[j].BLOCK_TR_1_F_ERR))
            {
                Globals.MEM_B_TR_1_F_ERR[j] = false;
            }
            if ((!(Globals.MEM_B_TR_1_B_ERR[j])) & (crane[j].BLOCK_TR_1_B_ERR))
            {
                WriteWarning_Alarm_ald(j, 2, 4, 2, alarmsView);
                Globals.MEM_B_TR_1_B_ERR[j] = true;
            }
            if (!(crane[j].BLOCK_TR_1_B_ERR))
            {
                Globals.MEM_B_TR_1_B_ERR[j] = false;
            }

            if ((!(Globals.MEM_B_TR_2_F_ERR[j])) & (crane[j].BLOCK_TR_2_F_ERR))
            {
                WriteWarning_Alarm_ald(j, 5, 2, 2, alarmsView);
                Globals.MEM_B_TR_2_F_ERR[j] = true;
            }
            if (!(crane[j].BLOCK_TR_2_F_ERR))
            {
                Globals.MEM_B_TR_2_F_ERR[j] = false;
            }
            if ((!(Globals.MEM_B_TR_2_B_ERR[j])) & (crane[j].BLOCK_TR_2_B_ERR))
            {
                WriteWarning_Alarm_ald(j, 5, 4, 2, alarmsView);
                Globals.MEM_B_TR_2_B_ERR[j] = true;
            }
            if (!(crane[j].BLOCK_TR_2_B_ERR))
            {
                Globals.MEM_B_TR_2_B_ERR[j] = false;
            }

            if ((!(Globals.MEM_B_H_1_1_UP_ERR[j])) & (crane[j].BLOCK_H_1_1_UP_ERR))
            {
                WriteWarning_Alarm_ald(j, 3, 0, 2, alarmsView);
                Globals.MEM_B_H_1_1_UP_ERR[j] = true;
            }
            if (!(crane[j].BLOCK_H_1_1_UP_ERR))
            {
                Globals.MEM_B_H_1_1_UP_ERR[j] = false;
            }
            if ((!(Globals.MEM_B_H_1_1_DOWN_ERR[j])) & (crane[j].BLOCK_H_1_1_DOWN_ERR))
            {
                WriteWarning_Alarm_ald(j, 3, 1, 2, alarmsView);
                Globals.MEM_B_H_1_1_DOWN_ERR[j] = true;
            }
            if (!(crane[j].BLOCK_H_1_1_DOWN_ERR))
            {
                Globals.MEM_B_H_1_1_DOWN_ERR[j] = false;
            }

            if ((!(Globals.MEM_B_H_1_2_UP_ERR[j])) & (crane[j].BLOCK_H_1_2_UP_ERR))
            {
                WriteWarning_Alarm_ald(j, 4, 0, 2, alarmsView);
                Globals.MEM_B_H_1_2_UP_ERR[j] = true;
            }
            if (!(crane[j].BLOCK_H_1_2_UP_ERR))
            {
                Globals.MEM_B_H_1_2_UP_ERR[j] = false;
            }
            if ((!(Globals.MEM_B_H_1_2_DOWN_ERR[j])) & (crane[j].BLOCK_H_1_2_DOWN_ERR))
            {
                WriteWarning_Alarm_ald(j, 4, 1, 2, alarmsView);
                Globals.MEM_B_H_1_2_DOWN_ERR[j] = true;
            }
            if (!(crane[j].BLOCK_H_1_2_DOWN_ERR))
            {
                Globals.MEM_B_H_1_2_DOWN_ERR[j] = false;
            }

            if ((!(Globals.MEM_B_H_2_1_UP_ERR[j])) & (crane[j].BLOCK_H_2_1_UP_ERR))
            {
                WriteWarning_Alarm_ald(j, 6, 0, 2, alarmsView);
                Globals.MEM_B_H_2_1_UP_ERR[j] = true;
            }
            if (!(crane[j].BLOCK_H_2_1_UP_ERR))
            {
                Globals.MEM_B_H_2_1_UP_ERR[j] = false;
            }
            if ((!(Globals.MEM_B_H_2_1_DOWN_ERR[j])) & (crane[j].BLOCK_H_2_1_DOWN_ERR))
            {
                WriteWarning_Alarm_ald(j, 6, 1, 2, alarmsView);
                Globals.MEM_B_H_2_1_DOWN_ERR[j] = true;
            }
            if (!(crane[j].BLOCK_H_2_1_DOWN_ERR))
            {
                Globals.MEM_B_H_2_1_DOWN_ERR[j] = false;
            }

            if ((!(Globals.MEM_B_H_2_2_UP_ERR[j])) & (crane[j].BLOCK_H_2_2_UP_ERR))
            {
                WriteWarning_Alarm_ald(j, 7, 0, 2, alarmsView);
                Globals.MEM_B_H_2_2_UP_ERR[j] = true;
            }
            if (!(crane[j].BLOCK_H_2_2_UP_ERR))
            {
                Globals.MEM_B_H_2_2_UP_ERR[j] = false;
            }
            if ((!(Globals.MEM_B_H_2_2_DOWN_ERR[j])) & (crane[j].BLOCK_H_2_2_DOWN_ERR))
            {
                WriteWarning_Alarm_ald(j, 7, 1, 2, alarmsView);
                Globals.MEM_B_H_2_2_DOWN_ERR[j] = true;
            }
            if (!(crane[j].BLOCK_H_2_2_DOWN_ERR))
            {
                Globals.MEM_B_H_2_2_DOWN_ERR[j] = false;
            }

            #endregion
            // **************************************************************************************
           
            // **************************************************************************************
            
            // =====================================================================================






            // **************************************************************************************
            ApplyDisablesToImages(images[craneConnectionScript.craneType], disables, alarmOverlays[j - 1], alarmsView);

            var craneConnected = craneConnectionScript?.connected;
            foreach (var image in images[craneConnectionScript.craneType])
            {
                image.Value.good = (bool)craneConnected;
            }

            //Окрашиваем элементы крана

            if (craneScript.mainTrolley != null)
            {
                var mainTrolley = craneScript.mainTrolley.GetComponent<MaterialSwitch>();
                if (mainTrolley != null)
                {
                    mainTrolley.warning = disables.mtBackwardWarning || disables.mtForwardWarning;
                    mainTrolley.alarm = disables.mtBackwardAlarm || disables.mtForwardAlarm;
                }
            }

            if (craneScript.auxiliaryTrolley != null)
            {
                var auxTrolley = craneScript.auxiliaryTrolley?.GetComponent<MaterialSwitch>();
                if (auxTrolley != null)
                {
                    auxTrolley.warning = disables.atBackwardWarning || disables.atForwardWarning;
                    auxTrolley.alarm = disables.atBackwardAlarm || disables.atForwardAlarm;
                }
            }

            if (craneScript.bridge != null)
            {
                var bridge = craneScript.bridge?.GetComponentsInChildren<MaterialSwitch>();
                if (bridge != null)
                {
                    bridge[0].warning = disables.bridgeLeftWarning || disables.bridgeRightWarning;
                    bridge[0].alarm = disables.bridgeLeftAlarm || disables.bridgeRightAlarm;
                }
            }



            var k = (j - 1) * 100;

            var TEMP_warn = false;
            var TEMP_err = false;

            if (craneScript.mtMainHoist != null)
            {
                f = 3;
                TEMP_warn = crane[j].UP_KRASH_WARN[f] || crane[j].DOWN_KRASH_WARN[f];

                // ||
                //         crane[j].L_KRASH_WARN[f] || crane[j].R_KRASH_WARN[f] ||
                //         crane[j].F_KRASH_WARN[f] || crane[j].B_KRASH_WARN[f];
                TEMP_err = crane[j].UP_KRASH_ERR[f] || crane[j].DOWN_KRASH_ERR[f]; 
                    
                    //||
                    //        crane[j].L_KRASH_ERR[f] || crane[j].R_KRASH_ERR[f] ||
                    //        crane[j].F_KRASH_ERR[f] || crane[j].B_KRASH_ERR[f];

                ApplyDisablesToObject(craneScript.mtMainHoist.GetComponent<MaterialSwitch>(),
                     TEMP_warn,
                     TEMP_err);
                // Obj_ALL_XYZ_KRASH_warn[k + 3], //тросс + крюк
                // Obj_ALL_XYZ_KRASH_err[k + 3]); 

                f = 6;
                TEMP_warn = crane[j].UP_KRASH_WARN[f] || crane[j].DOWN_KRASH_WARN[f];

                //||
                //        crane[j].L_KRASH_WARN[f] || crane[j].R_KRASH_WARN[f] ||
                //        crane[j].F_KRASH_WARN[f] || crane[j].B_KRASH_WARN[f];
                TEMP_err = crane[j].UP_KRASH_ERR[f] || crane[j].DOWN_KRASH_ERR[f]; 
                    
                    //||
                    //        crane[j].L_KRASH_ERR[f] || crane[j].R_KRASH_ERR[f] ||
                    //        crane[j].F_KRASH_ERR[f] || crane[j].B_KRASH_ERR[f];
                ApplyDisablesToWeight(craneScript.mtMainHoist,
                     TEMP_warn,
                     TEMP_err);
                // Obj_ALL_XYZ_KRASH_warn[k + 6],
                // Obj_ALL_XYZ_KRASH_err[k + 6]);     //груз
            }                         
            
            if (craneScript.mtAuxHoist != null)
            {
                f = 7;
                TEMP_warn = crane[j].UP_KRASH_WARN[f] || crane[j].DOWN_KRASH_WARN[f];

                //||
                //        crane[j].L_KRASH_WARN[f] || crane[j].R_KRASH_WARN[f] ||
                //        crane[j].F_KRASH_WARN[f] || crane[j].B_KRASH_WARN[f];
                TEMP_err = crane[j].UP_KRASH_ERR[f] || crane[j].DOWN_KRASH_ERR[f];

                //||
                //        crane[j].L_KRASH_ERR[f] || crane[j].R_KRASH_ERR[f] ||
                //        crane[j].F_KRASH_ERR[f] || crane[j].B_KRASH_ERR[f];

                ApplyDisablesToObject(craneScript.mtAuxHoist.GetComponent<MaterialSwitch>(),
                     TEMP_warn,
                     TEMP_err);
                //Obj_ALL_XYZ_KRASH_warn[k + 7],     //тросс + крюк
                //Obj_ALL_XYZ_KRASH_err[k + 7]);
                f = 10;
                TEMP_warn = crane[j].UP_KRASH_WARN[f] || crane[j].DOWN_KRASH_WARN[f];

                //||
                //        crane[j].L_KRASH_WARN[f] || crane[j].R_KRASH_WARN[f] ||
                //        crane[j].F_KRASH_WARN[f] || crane[j].B_KRASH_WARN[f];
                TEMP_err = crane[j].UP_KRASH_ERR[f] || crane[j].DOWN_KRASH_ERR[f];

                //||
                //        crane[j].L_KRASH_ERR[f] || crane[j].R_KRASH_ERR[f] ||
                //        crane[j].F_KRASH_ERR[f] || crane[j].B_KRASH_ERR[f];
                ApplyDisablesToWeight(craneScript.mtAuxHoist,
                     TEMP_warn,
                     TEMP_err);
                //Obj_ALL_XYZ_KRASH_warn[k + 10],
                //Obj_ALL_XYZ_KRASH_err[k + 10]);    //груз
            }

            if (craneScript.atMainHoist != null)
            {
                f = 13;
                TEMP_warn = crane[j].UP_KRASH_WARN[f] || crane[j].DOWN_KRASH_WARN[f];

                //||
                //        crane[j].L_KRASH_WARN[f] || crane[j].R_KRASH_WARN[f] ||
                //        crane[j].F_KRASH_WARN[f] || crane[j].B_KRASH_WARN[f];
                TEMP_err = crane[j].UP_KRASH_ERR[f] || crane[j].DOWN_KRASH_ERR[f];

                //||
                //        crane[j].L_KRASH_ERR[f] || crane[j].R_KRASH_ERR[f] ||
                //        crane[j].F_KRASH_ERR[f] || crane[j].B_KRASH_ERR[f];

                ApplyDisablesToObject(craneScript.atMainHoist.GetComponent<MaterialSwitch>(),
                      TEMP_warn,
                      TEMP_err);

                //Obj_ALL_XYZ_KRASH_warn[k + 13],    //тросс + крюк
                //  Obj_ALL_XYZ_KRASH_err[k + 13]);
                f = 16;
                TEMP_warn = crane[j].UP_KRASH_WARN[f] || crane[j].DOWN_KRASH_WARN[f];

                //||
                //        crane[j].L_KRASH_WARN[f] || crane[j].R_KRASH_WARN[f] ||
                //        crane[j].F_KRASH_WARN[f] || crane[j].B_KRASH_WARN[f];
                TEMP_err = crane[j].UP_KRASH_ERR[f] || crane[j].DOWN_KRASH_ERR[f];

                //||
                //        crane[j].L_KRASH_ERR[f] || crane[j].R_KRASH_ERR[f] ||
                //        crane[j].F_KRASH_ERR[f] || crane[j].B_KRASH_ERR[f];
                ApplyDisablesToWeight(craneScript.atMainHoist,
                      TEMP_warn,
                      TEMP_err);
                //  Obj_ALL_XYZ_KRASH_warn[k + 16],
                //  Obj_ALL_XYZ_KRASH_err[k + 16]);    //груз
            }

            if (craneScript.atAuxHoist != null)
            {
                f = 17;
                TEMP_warn = crane[j].UP_KRASH_WARN[f] || crane[j].DOWN_KRASH_WARN[f];

                //||
                //        crane[j].L_KRASH_WARN[f] || crane[j].R_KRASH_WARN[f] ||
                //        crane[j].F_KRASH_WARN[f] || crane[j].B_KRASH_WARN[f];
                TEMP_err = crane[j].UP_KRASH_ERR[f] || crane[j].DOWN_KRASH_ERR[f];

                //||
                //        crane[j].L_KRASH_ERR[f] || crane[j].R_KRASH_ERR[f] ||
                //        crane[j].F_KRASH_ERR[f] || crane[j].B_KRASH_ERR[f];

                ApplyDisablesToObject(craneScript.atAuxHoist.GetComponent<MaterialSwitch>(),
                     TEMP_warn,
                     TEMP_err);

                //Obj_ALL_XYZ_KRASH_warn[k + 17],       //тросс + крюк
                //Obj_ALL_XYZ_KRASH_err[k + 17]);
                f = 20;
                TEMP_warn = crane[j].UP_KRASH_WARN[f] || crane[j].DOWN_KRASH_WARN[f];

                //||
                //        crane[j].L_KRASH_WARN[f] || crane[j].R_KRASH_WARN[f] ||
                //        crane[j].F_KRASH_WARN[f] || crane[j].B_KRASH_WARN[f];
                TEMP_err = crane[j].UP_KRASH_ERR[f] || crane[j].DOWN_KRASH_ERR[f];

                //||
                //        crane[j].L_KRASH_ERR[f] || crane[j].R_KRASH_ERR[f] ||
                //        crane[j].F_KRASH_ERR[f] || crane[j].B_KRASH_ERR[f];
                ApplyDisablesToWeight(craneScript.atAuxHoist,
                     TEMP_warn,
                     TEMP_err);

                //Obj_ALL_XYZ_KRASH_warn[k + 20],
                // Obj_ALL_XYZ_KRASH_err[k + 20]);        //груз
            }
            
            
            
            
            
            /*
            if (craneScript.mtMainHoist != null)
            {
                ApplyDisablesToObject(craneScript.mtMainHoist.GetComponent<MaterialSwitch>(),
                   // TEMP_1,
                   // TEMP_2);

                         Obj_ALL_XYZ_KRASH_warn[k + 3], //тросс + крюк
                         Obj_ALL_XYZ_KRASH_err[k + 3]); ;

                var weight = craneScript.mtMainHoist.GetComponent<CargoDrawer>();

               // TEMP_1 = (crane[j].BLOCK_H_1_1_UP_WARN || crane[j].BLOCK_H_1_1_DOWN_WARN);
                // TEMP_2 = (crane[j].BLOCK_H_1_1_UP_ERR || crane[j].BLOCK_H_1_1_DOWN_ERR);

                if (weight != null)
                {
                    var ms = weight.GetComponentInChildren<MaterialSwitch>();
                    if (ms != null)
                    {
                        ms.warning = Obj_ALL_XYZ_KRASH_warn[k + 6]; // груз
                        ms.alarm = Obj_ALL_XYZ_KRASH_err[k + 6];
                    }
                }
            }

            if (craneScript.mtAuxHoist != null)
            {
                ApplyDisablesToObject(craneScript.mtAuxHoist.GetComponent<MaterialSwitch>(),
                    Obj_ALL_XYZ_KRASH_warn[k + 7],
                    Obj_ALL_XYZ_KRASH_err[k + 7]);

                var weight = craneScript.mtAuxHoist.GetComponent<CargoDrawer>();
                if (weight != null)
                {
                    var ms = weight.GetComponentInChildren<MaterialSwitch>();
                    if (ms != null)
                    {
                        ms.warning = Obj_ALL_XYZ_KRASH_warn[k + 10]; // груз
                        ms.alarm = Obj_ALL_XYZ_KRASH_err[k + 10];
                    }
                }
            }

            if (craneScript.atMainHoist != null)
            {
                ApplyDisablesToObject(craneScript.atMainHoist.GetComponent<MaterialSwitch>(),
                    Obj_ALL_XYZ_KRASH_warn[k + 13],
                    Obj_ALL_XYZ_KRASH_err[k + 13]);

                var weight = craneScript.atMainHoist.GetComponent<CargoDrawer>();
                if (weight != null)
                {
                    var ms = weight.GetComponentInChildren<MaterialSwitch>();
                    if (ms != null)
                    {
                        ms.warning = Obj_ALL_XYZ_KRASH_warn[k + 16]; // груз
                        ms.alarm = Obj_ALL_XYZ_KRASH_err[k + 16];
                    }
                }
            }

            if (craneScript.atAuxHoist != null)
            {
                ApplyDisablesToObject(craneScript.atAuxHoist.GetComponent<MaterialSwitch>(),
                    Obj_ALL_XYZ_KRASH_warn[k + 17],
                    Obj_ALL_XYZ_KRASH_err[k + 17]);

                var weight = craneScript.atAuxHoist.GetComponent<CargoDrawer>();
                if (weight != null)
                {
                    var ms = weight.GetComponentInChildren<MaterialSwitch>();
                    if (ms != null)
                    {
                        ms.warning = Obj_ALL_XYZ_KRASH_warn[k + 20]; // груз
                        ms.alarm = Obj_ALL_XYZ_KRASH_err[k + 20];
                    }
                }
            }
            */


            /*
             if (craneScript.mtMainHoist != null)
                ApplyDisablesToObject(craneScript.mtMainHoist.GetComponent<MaterialSwitch>(),
                    disables.mtMhDownWarning || disables.mtMhUpWarning,
                    disables.mtMhDownAlarm || disables.mtMhUpAlarm);

            if (craneScript.mtAuxHoist != null)
                ApplyDisablesToObject(craneScript.mtAuxHoist.GetComponent<MaterialSwitch>(),
                    disables.mtAhDownWarning || disables.mtAhUpWarning,
                    disables.mtAhDownAlarm || disables.mtAhUpAlarm);

            if (craneScript.atMainHoist != null)
                ApplyDisablesToObject(craneScript.atMainHoist.GetComponent<MaterialSwitch>(),
                    disables.atMhDownWarning || disables.atMhUpWarning,
                    disables.atMhDownAlarm || disables.atMhUpAlarm);

            if (craneScript.atAuxHoist != null)
                ApplyDisablesToObject(craneScript.atAuxHoist.GetComponent<MaterialSwitch>(),
                    disables.atAhDownWarning || disables.atAhUpWarning,
                    disables.atAhDownAlarm || disables.atAhUpAlarm);
            */
        }
        #endregion
        #endregion
        // ------------------------------------------------------------------------------------------------------------------------------------------------------------
        // --- STOP WRITE ---------------------------------------------------------------------------------------------------------------------------------------------
        // ВЫХОДНЫЕ ДАННЫЕ
        // ------------------------------------------------------------------------------------------------------------------------------------------------------------
        // NNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNN
        // FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
        // ------------------------------------------------------------------------------------------------------------------------------------------------------------

        // ПРИ ПЕРВОМ СТАРТЕ НИКАКИЕ СООБЩЕНИЯ НЕ ФОРМИРУЮТСЯ
        if (Globals.FLAG_F_START)
        {
            Globals.F_STARTING = true;
        }       
        // ФОРМИРОВАТЕЛЬ СООБЩЕНИЙ ПРЕДУПРЕЖДЕНИЙ И АВАРИЙ
        #region SQL_DB - 1 STEP - ЗАПИСЬ СООБЩЕНИЙ О ПРЕДУПРЕЖДЕНИЙ И АВАРИЙ
        if (!first_start)
        {
            switch (w_e_wr_DBSQL)
            {
                case 1: // 
                    w_e_wr_DBSQL = 2;
                    j = w_e_wr_DBSQL;
                    break;
                case 2: //
                    w_e_wr_DBSQL = 3;
                    j = w_e_wr_DBSQL;
                    break;
                default:
                    w_e_wr_DBSQL = 1;
                    j = 1;
                    break;
            }
            for (f = 1; f < 25; f++)
            {
                for (i = 1; i < 812; i++)
                {

                    // 0 - зона сверху
                    // 1 - зона снизу
                    // 2 - зона спереди
                    // 4 - зона сзади
                    // 8 - зона справа
                    // 16 - зона слева



                    if ((!(Globals.MEM1_UP_WARN[j, f, i])) & (crane[j].KRASH_UP_WARN[f, i]))
                    {
                        Write_ERR_WARN_ALD(j, f, i, true, false, true, 0, alarmsView);
                        Globals.MEM1_UP_WARN[j, f, i] = true;
                    }
                    if (!(crane[j].KRASH_UP_WARN[f, i]))
                    {
                        Globals.MEM1_UP_WARN[j, f, i] = false;
                    }

                    if ((!(Globals.MEM1_DOWN_WARN[j, f, i])) & (crane[j].KRASH_DOWN_WARN[f, i]))
                    {
                        Write_ERR_WARN_ALD(j, f, i, true, false, true, 1, alarmsView);
                        Globals.MEM1_DOWN_WARN[j, f, i] = true;
                    }
                    if (!(crane[j].KRASH_DOWN_WARN[f, i]))
                    {
                        Globals.MEM1_DOWN_WARN[j, f, i] = false;
                    }

                    if ((!(Globals.MEM1_F_WARN[j, f, i])) & (crane[j].KRASH_F_WARN[f, i]))
                    {
                        Write_ERR_WARN_ALD(j, f, i, true, false, true, 2, alarmsView);
                        Globals.MEM1_F_WARN[j, f, i] = true;
                    }
                    if (!(crane[j].KRASH_F_WARN[f, i]))
                    {
                        Globals.MEM1_F_WARN[j, f, i] = false;
                    }

                    if ((!(Globals.MEM1_B_WARN[j, f, i])) & (crane[j].KRASH_B_WARN[f, i]))
                    {
                        Write_ERR_WARN_ALD(j, f, i, true, false, true, 4, alarmsView);
                        Globals.MEM1_B_WARN[j, f, i] = true;
                    }
                    if (!(crane[j].KRASH_B_WARN[f, i]))
                    {
                        Globals.MEM1_B_WARN[j, f, i] = false;
                    }

                    if ((!(Globals.MEM1_R_WARN[j, f, i])) & (crane[j].KRASH_R_WARN[f, i]))
                    {
                        Write_ERR_WARN_ALD(j, f, i, true, false, true, 8, alarmsView);
                        Globals.MEM1_R_WARN[j, f, i] = true;
                    }
                    if (!(crane[j].KRASH_R_WARN[f, i]))
                    {
                        Globals.MEM1_R_WARN[j, f, i] = false;
                    }

                    if ((!(Globals.MEM1_L_WARN[j, f, i])) & (crane[j].KRASH_L_WARN[f, i]))
                    {
                        Write_ERR_WARN_ALD(j, f, i, true, false, true, 16, alarmsView);
                        Globals.MEM1_L_WARN[j, f, i] = true;
                    }
                    if (!(crane[j].KRASH_L_WARN[f, i]))
                    {
                        Globals.MEM1_L_WARN[j, f, i] = false;
                    }
                    // ***************************************************************************
                    if ((!(Globals.MEM1_UP_ERR[j, f, i])) & (crane[j].KRASH_UP_ERR[f, i]))
                    {
                        Write_ERR_WARN_ALD(j, f, i, true, true, false, 0, alarmsView);
                        Globals.MEM1_UP_ERR[j, f, i] = true;
                    }
                    if (!(crane[j].KRASH_UP_ERR[f, i]))
                    {
                        Globals.MEM1_UP_ERR[j, f, i] = false;
                    }

                    if ((!(Globals.MEM1_DOWN_ERR[j, f, i])) & (crane[j].KRASH_DOWN_ERR[f, i]))
                    {
                        Write_ERR_WARN_ALD(j, f, i, true, true, false, 1, alarmsView);
                        Globals.MEM1_DOWN_ERR[j, f, i] = true;
                    }
                    if (!(crane[j].KRASH_DOWN_ERR[f, i]))
                    {
                        Globals.MEM1_DOWN_ERR[j, f, i] = false;
                    }

                    if ((!(Globals.MEM1_F_ERR[j, f, i])) & (crane[j].KRASH_F_ERR[f, i]))
                    {
                        Write_ERR_WARN_ALD(j, f, i, true, true, false, 2, alarmsView);
                        Globals.MEM1_F_ERR[j, f, i] = true;
                    }
                    if (!(crane[j].KRASH_F_ERR[f, i]))
                    {
                        Globals.MEM1_F_ERR[j, f, i] = false;
                    }

                    if ((!(Globals.MEM1_B_ERR[j, f, i])) & (crane[j].KRASH_B_ERR[f, i]))
                    {
                        Write_ERR_WARN_ALD(j, f, i, true, true, false, 4, alarmsView);
                        Globals.MEM1_B_ERR[j, f, i] = true;
                    }
                    if (!(crane[j].KRASH_B_ERR[f, i]))
                    {
                        Globals.MEM1_B_ERR[j, f, i] = false;
                    }

                    if ((!(Globals.MEM1_R_ERR[j, f, i])) & (crane[j].KRASH_R_ERR[f, i]))
                    {
                        Write_ERR_WARN_ALD(j, f, i, true, true, false, 8, alarmsView);
                        Globals.MEM1_R_ERR[j, f, i] = true;
                    }
                    if (!(crane[j].KRASH_R_ERR[f, i]))
                    {
                        Globals.MEM1_R_ERR[j, f, i] = false;
                    }

                    if ((!(Globals.MEM1_L_ERR[j, f, i])) & (crane[j].KRASH_L_ERR[f, i]))
                    {
                        Write_ERR_WARN_ALD(j, f, i, true, true, false, 16, alarmsView);
                        Globals.MEM1_L_ERR[j, f, i] = true;
                    }
                    if (!(crane[j].KRASH_L_ERR[f, i]))
                    {
                        Globals.MEM1_L_ERR[j, f, i] = false;

                    }
                }
            }
        }
        else
        {
            for (j = 1; j <= 3; j++)
            {
                for (f = 1; f < 25; f++)
                {
                    for (i = 1; i < 812; i++)
                    {
                        Globals.MEM1_UP_WARN[j, f, i] = crane[j].KRASH_UP_WARN[f, i];
                        Globals.MEM1_DOWN_WARN[j, f, i] = crane[j].KRASH_DOWN_WARN[f, i];

                        Globals.MEM1_F_WARN[j, f, i] = crane[j].KRASH_F_WARN[f, i];
                        Globals.MEM1_B_WARN[j, f, i] = crane[j].KRASH_B_WARN[f, i];

                        Globals.MEM1_R_WARN[j, f, i] = crane[j].KRASH_R_WARN[f, i];
                        Globals.MEM1_L_WARN[j, f, i] = crane[j].KRASH_L_WARN[f, i];

                        Globals.MEM1_UP_ERR[j, f, i] = crane[j].KRASH_UP_ERR[f, i];
                        Globals.MEM1_DOWN_ERR[j, f, i] = crane[j].KRASH_DOWN_ERR[f, i];

                        Globals.MEM1_F_ERR[j, f, i] = crane[j].KRASH_F_ERR[f, i];
                        Globals.MEM1_B_ERR[j, f, i] = crane[j].KRASH_B_ERR[f, i];

                        Globals.MEM1_R_ERR[j, f, i] = crane[j].KRASH_R_ERR[f, i];
                        Globals.MEM1_L_ERR[j, f, i] = crane[j].KRASH_L_ERR[f, i];

                    }
                }
            }
        }
        #endregion
        // ФОРМИРОВАТЕЛЬ СООБЩЕНИЙ ИЗМЕНЕНИЕ ПРЕПЯТСТВИЙ И ТРАВЕРС
        #region SQL_DB - 2 STEP - ЗАПИСЬ СООБЩЕНИЙ О ПРЕДУПРЕЖДЕНИЙ И АВАРИЙ
        // 301 - 811 - ТОЛЬКО СТАТИЧНЫЕ ОБЪЕКТЫ - ПРЕПЯТСТВИЯ и грузы и траверсы на самом кране
        if (!(Globals.FLAG_F_START))
        {
            #region ПРЕПЯТСТВИЯ
            for (i = 301; i < 812; i++)
            {
                // ========================================================================
                // АНАЛИЗ КООРДИНАТ
                // X - КООРДИНАТА
                if (Globals.MEM_Obj_ALL_X[i] == Obj_ALL_X[i])
                { FLAF_COMMPARE_X = false; }
                else
                { FLAF_COMMPARE_X = true; }
                // Y - КООРДИНАТА
                if (Globals.MEM_Obj_ALL_Y[i] == Obj_ALL_Y[i])
                { FLAF_COMMPARE_Y = false; }
                else
                { FLAF_COMMPARE_Y = true; }
                // Z - КООРДИНАТА
                if (Globals.MEM_Obj_ALL_Z[i] == Obj_ALL_Z[i])
                { FLAF_COMMPARE_Z = false; }
                else
                { FLAF_COMMPARE_Z = true; }
                // ========================================================================
                // АНАЛИЗ БАЗОВОЙ ТОЧКИ ОТСЧЕТА КООРДИНАТ
                // БАЗОВАЯ ТОЧКА X - КООРДИНАТА
                if (Globals.MEM_Obj_ALL_ST_P_X[i] == Obj_ALL_ST_P_X[i])
                { FLAF_COMMPARE_ST_P_X = false; }
                else
                { FLAF_COMMPARE_ST_P_X = true; }
                // БАЗОВАЯ ТОЧКА Y - КООРДИНАТА
                if (Globals.MEM_Obj_ALL_ST_P_Y[i] == Obj_ALL_ST_P_Y[i])
                { FLAF_COMMPARE_ST_P_Y = false; }
                else
                { FLAF_COMMPARE_ST_P_Y = true; }
                // БАЗОВАЯ ТОЧКА Z - КООРДИНАТА
                if (Globals.MEM_Obj_ALL_ST_P_Z[i] == Obj_ALL_ST_P_Z[i])
                { FLAF_COMMPARE_ST_P_Z = false; }
                else
                { FLAF_COMMPARE_ST_P_Z = true; }
                // =========================================================================
                // ИТОГОВОЕ ЗНАЧЕНИЕ
                FLAF_COMMPARE_ALL = (FLAF_COMMPARE_X || FLAF_COMMPARE_Y || FLAF_COMMPARE_Z || FLAF_COMMPARE_ST_P_X || FLAF_COMMPARE_ST_P_Y || FLAF_COMMPARE_ST_P_Z);
                // =========================================================================
                // ЕСЛИ ФИКСИРУЕМ ФЛАГ ИЗМЕНЕНИЯ ПРЕПЯТСТВИЯ - ВЫЗЫВАЕМ ПРОЦЕДУРУ ЗАПИСИ В БАЗУ SQL
                // 
                if (FLAF_COMMPARE_ALL)
                {
                    //  Write_ERR_WARN_ALD_PR_TR(bool TEMP_CHANGE_X, bool TEMP_CHANGE_Y, bool TEMP_CHANGE_Z,
                    // bool TEMP_CHANGE_STP_X, bool TEMP_CHANGE_STP_Y, bool TEMP_CHANGE_STP_Z,
                    // int TEMP_i, float TEMP_x, float TEMP_y, float TEMP_z, float TEMP_st_x, float TEMP_st_y, float TEMP_st_z,
                    // float mem_TEMP_x, float mem_TEMP_y, float mem_TEMP_z, float mem_TEMP_st_x, float mem_TEMP_st_y, float mem_TEMP_st_z, Alarms logger)

                    Write_ERR_WARN_ALD_PR_TR(FLAF_COMMPARE_X, FLAF_COMMPARE_Y, FLAF_COMMPARE_Z,
                        FLAF_COMMPARE_ST_P_X, FLAF_COMMPARE_ST_P_Y, FLAF_COMMPARE_ST_P_Z,
                        i, Obj_ALL_X[i], Obj_ALL_Y[i], Obj_ALL_Z[i], Obj_ALL_ST_P_X[i], Obj_ALL_ST_P_Y[i], Obj_ALL_ST_P_Z[i],
                        Globals.MEM_Obj_ALL_X[i], Globals.MEM_Obj_ALL_Y[i], Globals.MEM_Obj_ALL_Z[i], Globals.MEM_Obj_ALL_ST_P_X[i], Globals.MEM_Obj_ALL_ST_P_Y[i], Globals.MEM_Obj_ALL_ST_P_Z[i], alarmsView);


                }

                Globals.MEM_Obj_ALL_X[i] = Obj_ALL_X[i];
                Globals.MEM_Obj_ALL_Y[i] = Obj_ALL_Y[i];
                Globals.MEM_Obj_ALL_Z[i] = Obj_ALL_Z[i];

                Globals.MEM_Obj_ALL_ST_P_X[i] = Obj_ALL_ST_P_X[i];
                Globals.MEM_Obj_ALL_ST_P_Y[i] = Obj_ALL_ST_P_Y[i];
                Globals.MEM_Obj_ALL_ST_P_Z[i] = Obj_ALL_ST_P_Z[i];
            }
            #endregion
        }
        else
        {
            for (i = 301; i < 812; i++)
            {
            Globals.MEM_Obj_ALL_X[i] = Obj_ALL_X[i];
            Globals.MEM_Obj_ALL_Y[i] = Obj_ALL_Y[i];
            Globals.MEM_Obj_ALL_Z[i] = Obj_ALL_Z[i];

            Globals.MEM_Obj_ALL_ST_P_X[i] = Obj_ALL_ST_P_X[i];
            Globals.MEM_Obj_ALL_ST_P_Y[i] = Obj_ALL_ST_P_Y[i];
            Globals.MEM_Obj_ALL_ST_P_Z[i] = Obj_ALL_ST_P_Z[i];
            }    
        }










        if (!first_start)
        {
            #region ГРУЗА и ТРАВЕРСЫ на кране
            for (NEW_i = 1; NEW_i < 25; NEW_i ++)
            {
                #region  // КАКОЙ ЭЛЕМЕНТ = i
                //1 МОСТ
                //2 ТЕЛЕЖКА 1
                //3 ТЕЛЕЖКА 1 - ПОДЪЕМ 1 - ТРОСС + КРЮК
                //4 ТЕЛЕЖКА 1 - ПОДЪЕМ 1 - тросс + крюк + траверса => ВСЕГДА FALSE
                //5 ТЕЛЕЖКА 1 - ПОДЪЕМ 1 - траверса
                //6 ТЕЛЕЖКА 1 - ПОДЪЕМ 1 - груз
                //7 ТЕЛЕЖКА 1 - ПОДЪЕМ 2 - ТРОСС + КРЮК
                //8 ТЕЛЕЖКА 1 - ПОДЪЕМ 2 - тросс + крюк + траверса => ВСЕГДА FALSE
                //9 ТЕЛЕЖКА 1 - ПОДЪЕМ 2 - траверса
                //10 ТЕЛЕЖКА 1 - ПОДЪЕМ 2 - груз
                //11 тросс + крюк + траверса + 3 подъем тросс + крюк + траверса => ВСЕГДА FALSE
                //12 ТЕЛЕЖКА 2                                                       
                //13 ТЕЛЕЖКА 2 - ПОДЪЕМ 1 - ТРОСС + КРЮК
                //14 ТЕЛЕЖКА 2 - ПОДЪЕМ 1 - тросс + крюк + траверса => ВСЕГДА FALSE
                //15 ТЕЛЕЖКА 2 - ПОДЪЕМ 1 - траверса
                //16 ТЕЛЕЖКА 2 - ПОДЪЕМ 1 - груз
                //17 ТЕЛЕЖКА 2 - ПОДЪЕМ 2 - ТРОСС + КРЮК
                //18 ТЕЛЕЖКА 2 - ПОДЪЕМ 2 - тросс + крюк + траверса => ВСЕГДА FALSE
                //19 ТЕЛЕЖКА 2 - ПОДЪЕМ 2 - траверса
                //20 ТЕЛЕЖКА 2 - ПОДЪЕМ 2 - груз

                var TEMP_CRANEs_X_EL_X = 0; // КАКОЙ КРАН И ЧТО ПИШЕМ 11 - КРАН 320 ГРУЗ/ 12 - КРАН 320 ТРАВЕРСА

                switch (NEW_i)
                {
                    case 1: i = 5; TEMP_CRANEs_X_EL_X = 12; break;   // ТРАВЕРСА НА ПОДЪЕМЕ 1-1 - КРАН 320Т
                    case 2: i = 6; TEMP_CRANEs_X_EL_X = 11; break;   // ГРУЗ НА ПОДЪЕМЕ 1-1 - КРАН 320Т
                    case 3: i = 9; TEMP_CRANEs_X_EL_X = 12; break;   // ТРАВЕРСА НА ПОДЪЕМЕ 1-2 - КРАН 320Т
                    case 4: i = 10; TEMP_CRANEs_X_EL_X = 11; break;  // ГРУЗ НА ПОДЪЕМЕ 1-2 - КРАН 320Т
                    case 5: i = 15; TEMP_CRANEs_X_EL_X = 12; break;  // ТРАВЕРСА НА ПОДЪЕМЕ 2-1 - КРАН 320Т
                    case 6: i = 16; TEMP_CRANEs_X_EL_X = 11; break;  // ГРУЗ НА ПОДЪЕМЕ 2-1 - КРАН 320Т
                    case 7: i = 19; TEMP_CRANEs_X_EL_X = 12; break;  // ТРАВЕРСА НА ПОДЪЕМЕ 2-2 - КРАН 320Т
                    case 8: i = 20; TEMP_CRANEs_X_EL_X = 11; break;  // ГРУЗ НА ПОДЪЕМЕ 2-2 - КРАН 320Т
                    
                    case 9: i = 105; TEMP_CRANEs_X_EL_X = 22; break;   // ТРАВЕРСА НА ПОДЪЕМЕ 1-1 - КРАН 120Т
                    case 10: i = 106; TEMP_CRANEs_X_EL_X = 21; break;   // ГРУЗ НА ПОДЪЕМЕ 1-1 - КРАН 120Т
                    case 11: i = 109; TEMP_CRANEs_X_EL_X = 22; break;   // ТРАВЕРСА НА ПОДЪЕМЕ 1-2 - КРАН 120Т
                    case 12: i = 110; TEMP_CRANEs_X_EL_X = 21; break;  // ГРУЗ НА ПОДЪЕМЕ 1-2 - КРАН 120Т
                    case 13: i = 115; TEMP_CRANEs_X_EL_X = 22; break;  // ТРАВЕРСА НА ПОДЪЕМЕ 2-1 - КРАН 120Т
                    case 14: i = 116; TEMP_CRANEs_X_EL_X = 21; break;  // ГРУЗ НА ПОДЪЕМЕ 2-1 - КРАН 120Т
                    case 15: i = 119; TEMP_CRANEs_X_EL_X = 22; break;  // ТРАВЕРСА НА ПОДЪЕМЕ 2-2 - КРАН 120Т
                    case 16: i = 120; TEMP_CRANEs_X_EL_X = 21; break;  // ГРУЗ НА ПОДЪЕМЕ 2-2 - КРАН 120Т

                    case 17: i = 205; TEMP_CRANEs_X_EL_X = 32; break;   // ТРАВЕРСА НА ПОДЪЕМЕ 1-1 - КРАН 25Т
                    case 18: i = 206; TEMP_CRANEs_X_EL_X = 31; break;   // ГРУЗ НА ПОДЪЕМЕ 1-1 - КРАН 25Т
                    case 19: i = 209; TEMP_CRANEs_X_EL_X = 32; break;   // ТРАВЕРСА НА ПОДЪЕМЕ 1-2 - КРАН 25Т
                    case 20: i = 210; TEMP_CRANEs_X_EL_X = 31; break;  // ГРУЗ НА ПОДЪЕМЕ 1-2 - КРАН 25Т
                    case 21: i = 215; TEMP_CRANEs_X_EL_X = 32; break;  // ТРАВЕРСА НА ПОДЪЕМЕ 2-1 - КРАН 25Т
                    case 22: i = 216; TEMP_CRANEs_X_EL_X = 31; break;  // ГРУЗ НА ПОДЪЕМЕ 2-1 - КРАН 25Т
                    case 23: i = 219; TEMP_CRANEs_X_EL_X = 32; break;  // ТРАВЕРСА НА ПОДЪЕМЕ 2-2 - КРАН 25Т
                    case 24: i = 220; TEMP_CRANEs_X_EL_X = 31; break;  // ГРУЗ НА ПОДЪЕМЕ 2-2 - КРАН 25Т
                   
                    default: i = 300; TEMP_CRANEs_X_EL_X = 0; break;
                }
                #endregion
                // ========================================================================
                // АНАЛИЗ КООРДИНАТ
                // X - КООРДИНАТА
                if (Globals.MEM_Obj_ALL_X[i] == Obj_ALL_X[i])
                { FLAF_COMMPARE_X = false; }
                else
                { FLAF_COMMPARE_X = true; }
                // Y - КООРДИНАТА
                if (Globals.MEM_Obj_ALL_Y[i] == Obj_ALL_Y[i])
                { FLAF_COMMPARE_Y = false; }
                else
                { FLAF_COMMPARE_Y = true; }
                // Z - КООРДИНАТА
                if (Globals.MEM_Obj_ALL_Z[i] == Obj_ALL_Z[i])
                { FLAF_COMMPARE_Z = false; }
                else
                { FLAF_COMMPARE_Z = true; }
                // ========================================================================
                // АНАЛИЗ БАЗОВОЙ ТОЧКИ ОТСЧЕТА КООРДИНАТ
                // БАЗОВАЯ ТОЧКА X - КООРДИНАТА
                //if (Globals.MEM_Obj_ALL_ST_P_X[i] == Obj_ALL_ST_P_X[i])
                //{ FLAF_COMMPARE_ST_P_X = false; }
                //else
                //{ FLAF_COMMPARE_ST_P_X = true; }
                // БАЗОВАЯ ТОЧКА Y - КООРДИНАТА
                //if (Globals.MEM_Obj_ALL_ST_P_Y[i] == Obj_ALL_ST_P_Y[i])
                //{ FLAF_COMMPARE_ST_P_Y = false; }
                //else
                //{ FLAF_COMMPARE_ST_P_Y = true; }
                // БАЗОВАЯ ТОЧКА Z - КООРДИНАТА
                //if (Globals.MEM_Obj_ALL_ST_P_Z[i] == Obj_ALL_ST_P_Z[i])
                //{ FLAF_COMMPARE_ST_P_Z = false; }
                //else
                //{ FLAF_COMMPARE_ST_P_Z = true; }
                // =========================================================================
                // ИТОГОВОЕ ЗНАЧЕНИЕ
                FLAF_COMMPARE_ALL = (FLAF_COMMPARE_X || FLAF_COMMPARE_Y || FLAF_COMMPARE_Z || FLAF_COMMPARE_ST_P_X || FLAF_COMMPARE_ST_P_Y || FLAF_COMMPARE_ST_P_Z);
                // =========================================================================
                // ЕСЛИ ФИКСИРУЕМ ФЛАГ ИЗМЕНЕНИЯ ПРЕПЯТСТВИЯ - ВЫЗЫВАЕМ ПРОЦЕДУРУ ЗАПИСИ В БАЗУ SQL
                // 
                if (FLAF_COMMPARE_ALL)
                {

                    Write_ERR_WARN_ALD_HOIST_TR_CRANEs(FLAF_COMMPARE_X, FLAF_COMMPARE_Y, FLAF_COMMPARE_Z,
                        FLAF_COMMPARE_ST_P_X, FLAF_COMMPARE_ST_P_Y, FLAF_COMMPARE_ST_P_Z,
                        i, Obj_ALL_X[i], Obj_ALL_Y[i], Obj_ALL_Z[i], Obj_ALL_ST_P_X[i], Obj_ALL_ST_P_Y[i], Obj_ALL_ST_P_Z[i],
                        Globals.MEM_Obj_ALL_X[i], Globals.MEM_Obj_ALL_Y[i], Globals.MEM_Obj_ALL_Z[i], Globals.MEM_Obj_ALL_ST_P_X[i], Globals.MEM_Obj_ALL_ST_P_Y[i], Globals.MEM_Obj_ALL_ST_P_Z[i], alarmsView);


                }

                Globals.MEM_Obj_ALL_X[i] = Obj_ALL_X[i];
                Globals.MEM_Obj_ALL_Y[i] = Obj_ALL_Y[i];
                Globals.MEM_Obj_ALL_Z[i] = Obj_ALL_Z[i];

                Globals.MEM_Obj_ALL_ST_P_X[i] = Obj_ALL_ST_P_X[i];
                Globals.MEM_Obj_ALL_ST_P_Y[i] = Obj_ALL_ST_P_Y[i];
                Globals.MEM_Obj_ALL_ST_P_Z[i] = Obj_ALL_ST_P_Z[i];
            }
            #endregion
        }
        else
        {            
           for (NEW_i = 1; NEW_i < 25; NEW_i++)
                    {


                switch (NEW_i)
                {
                    case 1: i = 5;  break;   // ТРАВЕРСА НА ПОДЪЕМЕ 1-1 - КРАН 320Т
                    case 2: i = 6;  break;   // ГРУЗ НА ПОДЪЕМЕ 1-1 - КРАН 320Т
                    case 3: i = 9;  break;   // ТРАВЕРСА НА ПОДЪЕМЕ 1-2 - КРАН 320Т
                    case 4: i = 10;  break;  // ГРУЗ НА ПОДЪЕМЕ 1-2 - КРАН 320Т
                    case 5: i = 15;  break;  // ТРАВЕРСА НА ПОДЪЕМЕ 2-1 - КРАН 320Т
                    case 6: i = 16;  break;  // ГРУЗ НА ПОДЪЕМЕ 2-1 - КРАН 320Т
                    case 7: i = 19;  break;  // ТРАВЕРСА НА ПОДЪЕМЕ 2-2 - КРАН 320Т
                    case 8: i = 20;  break;  // ГРУЗ НА ПОДЪЕМЕ 2-2 - КРАН 320Т

                    case 9: i = 105;  break;   // ТРАВЕРСА НА ПОДЪЕМЕ 1-1 - КРАН 120Т
                    case 10: i = 106;  break;   // ГРУЗ НА ПОДЪЕМЕ 1-1 - КРАН 120Т
                    case 11: i = 109;  break;   // ТРАВЕРСА НА ПОДЪЕМЕ 1-2 - КРАН 120Т
                    case 12: i = 110; break;  // ГРУЗ НА ПОДЪЕМЕ 1-2 - КРАН 120Т
                    case 13: i = 115;  break;  // ТРАВЕРСА НА ПОДЪЕМЕ 2-1 - КРАН 120Т
                    case 14: i = 116;  break;  // ГРУЗ НА ПОДЪЕМЕ 2-1 - КРАН 120Т
                    case 15: i = 119;  break;  // ТРАВЕРСА НА ПОДЪЕМЕ 2-2 - КРАН 120Т
                    case 16: i = 120;  break;  // ГРУЗ НА ПОДЪЕМЕ 2-2 - КРАН 120Т

                    case 17: i = 205;  break;   // ТРАВЕРСА НА ПОДЪЕМЕ 1-1 - КРАН 25Т
                    case 18: i = 206;  break;   // ГРУЗ НА ПОДЪЕМЕ 1-1 - КРАН 25Т
                    case 19: i = 209;  break;   // ТРАВЕРСА НА ПОДЪЕМЕ 1-2 - КРАН 25Т
                    case 20: i = 210;  break;  // ГРУЗ НА ПОДЪЕМЕ 1-2 - КРАН 25Т
                    case 21: i = 215;  break;  // ТРАВЕРСА НА ПОДЪЕМЕ 2-1 - КРАН 25Т
                    case 22: i = 216;  break;  // ГРУЗ НА ПОДЪЕМЕ 2-1 - КРАН 25Т
                    case 23: i = 219;  break;  // ТРАВЕРСА НА ПОДЪЕМЕ 2-2 - КРАН 25Т
                    case 24: i = 220;  break;  // ГРУЗ НА ПОДЪЕМЕ 2-2 - КРАН 25Т

                    default: i = 300;  break;
                }



                Globals.MEM_Obj_ALL_X[i] = Obj_ALL_X[i];
                        Globals.MEM_Obj_ALL_Y[i] = Obj_ALL_Y[i];
                        Globals.MEM_Obj_ALL_Z[i] = Obj_ALL_Z[i];

                        Globals.MEM_Obj_ALL_ST_P_X[i] = Obj_ALL_ST_P_X[i];
                        Globals.MEM_Obj_ALL_ST_P_Y[i] = Obj_ALL_ST_P_Y[i];
                        Globals.MEM_Obj_ALL_ST_P_Z[i] = Obj_ALL_ST_P_Z[i];

                    }
        }
        #endregion
        // ФОРМИРОВАТЕЛЬ СООБЩЕНИЙ ИЗМЕНЕНИЕ ТРАВЕРС
        #region SQL_DB - 3 STEP - ЗАПИСЬ СООБЩЕНИЙ О ТРАВЕРСАХ В МАССИВЕ ДАННЫХ
        // 221-299 - ДЛЯ АРХИВИРОВАНИЯ ИЗМЕНЕНИЙ ПО ПРЕПЯТСТВИЯМ - c 221 по 271 - только 50 траверс
        #region ЗАПИСЫВАЕМ ДАННЫЕ ПО ТРАВЕРСАМ
        var TEMP_count_tr = razmer_arr_tr;
        var TEMP_count_tr_i = 0;

        //TraverseStruct[] TEMP_ARRAY_TR = TR_ald;
        
        // ПЕРЕВОРАЧИВАЕМ МАССИВ
        //for (int COUNER_I = 0; COUNER_I < TEMP_count_tr; COUNER_I++)
        //{
        //    var counter_j = (TEMP_count_tr - 1 - COUNER_I);
        //    TEMP_ARRAY_TR[counter_j] = TR_ald[COUNER_I];
        //}
                    
            if ((!(TEMP_count_tr > 49)) & (!(TEMP_count_tr <= 0)))
            {
                TEMP_count_tr_i = 220 + TEMP_count_tr;
                for (i = 221; i < 270; i++)
                {
                    if ((i - 221) <= (TEMP_count_tr-1))
                        {
                        var iNEW_100 = i - 221;                       
                        Obj_ALL_X[i] = TR_ald[iNEW_100].length;
                        Obj_ALL_Y[i] = TR_ald[iNEW_100].width;
                        Obj_ALL_Z[i] = TR_ald[iNEW_100].height;

                        Globals.traversa_ID[i - 220] = TR_ald[iNEW_100].id;
                        Globals.traversa_TWO_HOOK[i - 220] = TR_ald[iNEW_100].twoHooks;
                        Globals.traversa_name[i - 220] = TR_ald[iNEW_100].name ;
                }
                    else
                    {
                        Obj_ALL_X[i] = 0f;
                        Obj_ALL_Y[i] = 0f;
                        Obj_ALL_Z[i] = 0f;

                        Globals.traversa_ID[i - 220] = 0;
                        Globals.traversa_TWO_HOOK[i - 220] = false;
                        Globals.traversa_name[i - 220] = "";
                }
                }

            }
            #endregion
            if (!first_start_tr)
            {
                #region МАССИВ ТРАВЕРС
                for (i = 221; i < 270; i++)
                {
                    // ========================================================================
                    // АНАЛИЗ КООРДИНАТ
                    // X - КООРДИНАТА
                    if (Globals.MEM_Obj_ALL_X[i] == Obj_ALL_X[i])
                    { FLAF_COMMPARE_X = false; }
                    else
                    { FLAF_COMMPARE_X = true; }
                    // Y - КООРДИНАТА
                    if (Globals.MEM_Obj_ALL_Y[i] == Obj_ALL_Y[i])
                    { FLAF_COMMPARE_Y = false; }
                    else
                    { FLAF_COMMPARE_Y = true; }
                    // Z - КООРДИНАТА
                    if (Globals.MEM_Obj_ALL_Z[i] == Obj_ALL_Z[i])
                    { FLAF_COMMPARE_Z = false; }
                    else
                    { FLAF_COMMPARE_Z = true; }
                    // ========================================================================
                    // АНАЛИЗ НОМЕРА ID ТРАВЕРСЫ
                    if (Globals.MEM_traversa_ID[i - 220] == Globals.traversa_ID[i - 220])
                    { FLAF_COMMPARE_TR_id = false; }
                    else
                    { FLAF_COMMPARE_TR_id = true; }
                    // АНАЛИЗ НА ДВА ИЛИ ОДИН КРЮК ТРАВЕРСЫ
                    if ((Globals.MEM_traversa_TWO_HOOK[i - 220] & Globals.traversa_TWO_HOOK[i - 220]) || ((!(Globals.MEM_traversa_TWO_HOOK[i - 220])) & (!(Globals.traversa_TWO_HOOK[i - 220]))))
                    { FLAF_COMMPARE_TR_TWO_HOOK = false; }
                    else
                    { FLAF_COMMPARE_TR_TWO_HOOK = true; }
                    // АНАЛИЗ ИМЕНИ ТРАВЕРСЫ
                    if (Globals.MEM_traversa_name[i - 220] == Globals.traversa_name[i - 220])
                    { FLAF_COMMPARE_TR_NAME = false; }
                    else
                    { FLAF_COMMPARE_TR_NAME = true; }
                    // =========================================================================
                    // ИТОГОВОЕ ЗНАЧЕНИЕ
                    FLAF_COMMPARE_ALL = (FLAF_COMMPARE_X || FLAF_COMMPARE_Y || FLAF_COMMPARE_Z || FLAF_COMMPARE_TR_id || FLAF_COMMPARE_TR_TWO_HOOK || FLAF_COMMPARE_TR_NAME);
                    // =========================================================================
                    // ЕСЛИ ФИКСИРУЕМ ФЛАГ ИЗМЕНЕНИЯ ПРЕПЯТСТВИЯ - ВЫЗЫВАЕМ ПРОЦЕДУРУ ЗАПИСИ В БАЗУ SQL
                    // 
                    if (FLAF_COMMPARE_ALL)
                    {
                        //Write_CHANGE_traversy(P_CHANGE_X, bool TEMP_CHANGE_Y, bool TEMP_CHANGE_Z,
                        // bool TEMP_CHANGE_ID, bool TEMP_CHANGE_TWO_HOOK, bool TEMP_CHANGE_NAME,
                        // int TEMP_i, float TEMP_x, float TEMP_y, float TEMP_z, int TEMP_ID, bool TEMP_TWO_HOOK, string TEMP_NAME,
                        // float mem_TEMP_x, float mem_TEMP_y, float mem_TEMP_z, int mem_TEMP_ID, bool mem_TEMP_TWO_HOOK, string mem_TEMP_NAME, Alarms logger)


                        Write_CHANGE_traversy(FLAF_COMMPARE_X, FLAF_COMMPARE_Y, FLAF_COMMPARE_Z,
                            FLAF_COMMPARE_TR_id, FLAF_COMMPARE_TR_TWO_HOOK, FLAF_COMMPARE_TR_NAME,
                            i, Obj_ALL_X[i], Obj_ALL_Y[i], Obj_ALL_Z[i], Globals.traversa_ID[i - 220], Globals.traversa_TWO_HOOK[i - 220], Globals.traversa_name[i - 220],
                            Globals.MEM_Obj_ALL_X[i], Globals.MEM_Obj_ALL_Y[i], Globals.MEM_Obj_ALL_Z[i],
                            Globals.MEM_traversa_ID[i - 220], Globals.MEM_traversa_TWO_HOOK[i - 220], Globals.MEM_traversa_name[i - 220], alarmsView);


                    }

                    Globals.MEM_Obj_ALL_X[i] = Obj_ALL_X[i];
                    Globals.MEM_Obj_ALL_Y[i] = Obj_ALL_Y[i];
                    Globals.MEM_Obj_ALL_Z[i] = Obj_ALL_Z[i];

                    Globals.MEM_Obj_ALL_ST_P_X[i] = Obj_ALL_ST_P_X[i];
                    Globals.MEM_Obj_ALL_ST_P_Y[i] = Obj_ALL_ST_P_Y[i];
                    Globals.MEM_Obj_ALL_ST_P_Z[i] = Obj_ALL_ST_P_Z[i];

                    Globals.MEM_traversa_ID[i - 220] = Globals.traversa_ID[i - 220];
                    Globals.MEM_traversa_TWO_HOOK[i - 220] = Globals.traversa_TWO_HOOK[i - 220];
                    Globals.MEM_traversa_name[i - 220] = Globals.traversa_name[i - 220];




                }
                #endregion
            }
            else
            {
                for (i = 221; i <= 270; i++)
                {

                    Globals.MEM_Obj_ALL_X[i] = Obj_ALL_X[i];
                    Globals.MEM_Obj_ALL_Y[i] = Obj_ALL_Y[i];
                    Globals.MEM_Obj_ALL_Z[i] = Obj_ALL_Z[i];

                    Globals.MEM_traversa_ID[i - 220] = Globals.traversa_ID[i - 220];
                    Globals.MEM_traversa_TWO_HOOK[i - 220] = Globals.traversa_TWO_HOOK[i - 220];
                    Globals.MEM_traversa_name[i - 220] = Globals.traversa_name[i - 220];
                }

            }
        //}

        #endregion








        // ФОРМИРОВАТЕЛЬ СООБЩЕНИЙ ПО СИГНАЛАМ ОТ КРАНА
        #region SQL_DB - 4 STEP - ЗАПИСЬ СООБЩЕНИЙ ПО СИГНАЛАМ ОТ КРАНА













        #endregion





        // СНАЧАЛА ДОЛЖНА ПРОЙТИ ИНИЦИАЛИЗАЦИЯ ПРИ ПЕРВОМ СТАРТЕ, А ТОЛЬКО ПОТОМ  БУДУТ ФОРМИРОВТЬСЯ СООБЩЕНИЯ
        // кроме сообщений о сетевых коммуникаций
        // Необходимо предусмотреть что в случае потери связи ПЛК воспринимали как запрет движения!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        if (Globals.FLAG_F_START & Globals.FLAG_F_START_1MAIL)
        {
            var temp_string_MAIL_FIRST_START = "ВНИМАНИЕ!!! - Запуск системы координатной защиты";
            alarmsView.Alarm(temp_string_MAIL_FIRST_START, 0);
            Globals.FLAG_F_START_1MAIL = false;
        }

        if (first_start & Globals.FLAG_F_START_2MAIL)
        {
            var temp_string_MAIL_FIRST_START_reset = "ВНИМАНИЕ!!! - Перезапуск системы координатной защиты - после редактирования препятствий ";
            alarmsView.Alarm(temp_string_MAIL_FIRST_START_reset, 0);
            Globals.FLAG_F_START_2MAIL = false;
        }
        //
        if (!(flag_first_start_tr))
        {
            first_start_tr = false;
            Globals.FLAG_F_START = false;
        }
        if (!(Globals.FLAG_F_START))
        {
            first_start = false;
        }

     }


    private void ApplyDisablesToWeight(GameObject hoist, bool warning, bool alarm)
    {
        var cargoDrawerScript = hoist.GetComponent<CargoDrawer>();
        var weight = cargoDrawerScript?.weight;
        if (weight != null)
        {
            var ms = weight.GetComponentInChildren<MaterialSwitch>();
            if (ms != null)
            {
                ms.warning = warning; // груз
                ms.alarm = alarm;
            }
        }
    }
    private static void ApplyDisablesToObject(MaterialSwitch obj, bool warning, bool alarm)
    {
        if (obj != null)
        {
            obj.warning = warning;
            obj.alarm = alarm;
        }
    }
    private static void ApplyDisablesToImages(Dictionary<ColliderTargetAndType, ImageIndicator> images, CraneDisables disables, AlarmOverlay overlay, Alarms logger)
    {

        WriteWarning(images[ColliderTargetAndType.mtMhUpWarning], disables.mtMhUpWarning, logger);
        WriteAlarm(images[ColliderTargetAndType.mtMhUpWarning], disables.mtMhUpAlarm, logger);
        WriteWarning(images[ColliderTargetAndType.mtMhDownWarning], disables.mtMhDownWarning, logger);
        WriteAlarm(images[ColliderTargetAndType.mtMhDownWarning], disables.mtMhDownAlarm, logger);

        WriteWarning(images[ColliderTargetAndType.mtAhUpWarning], disables.mtAhUpWarning, logger);
        WriteAlarm(images[ColliderTargetAndType.mtAhUpWarning], disables.mtAhUpAlarm, logger);
        WriteWarning(images[ColliderTargetAndType.mtAhDownWarning], disables.mtAhDownWarning, logger);
        WriteAlarm(images[ColliderTargetAndType.mtAhDownWarning], disables.mtAhDownAlarm, logger);

        WriteWarning(images[ColliderTargetAndType.atMhUpWarning], disables.atMhUpWarning, logger);        
        WriteAlarm(images[ColliderTargetAndType.atMhUpWarning], disables.atMhUpAlarm, logger);
        WriteWarning(images[ColliderTargetAndType.atMhDownWarning], disables.atMhDownWarning, logger);
        WriteAlarm(images[ColliderTargetAndType.atMhDownWarning], disables.atMhDownAlarm, logger);

        WriteWarning(images[ColliderTargetAndType.atAhUpWarning], disables.atAhUpWarning, logger);
        WriteAlarm(images[ColliderTargetAndType.atAhUpWarning], disables.atAhUpAlarm, logger);
        WriteWarning(images[ColliderTargetAndType.atAhDownWarning], disables.atAhDownWarning, logger);
        WriteAlarm(images[ColliderTargetAndType.atAhDownWarning], disables.atAhDownAlarm, logger);

        WriteWarning(images[ColliderTargetAndType.mainTrolleyForwardWarning], disables.mtForwardWarning, logger);
        WriteAlarm(images[ColliderTargetAndType.mainTrolleyForwardWarning], disables.mtForwardAlarm, logger);
        WriteWarning(images[ColliderTargetAndType.mainTrolleyBackwardWarning], disables.mtBackwardWarning, logger);
        WriteAlarm(images[ColliderTargetAndType.mainTrolleyBackwardWarning], disables.mtBackwardAlarm, logger);

        WriteWarning(images[ColliderTargetAndType.auxTrolleyForwardWarning], disables.atForwardWarning, logger);
        WriteAlarm(images[ColliderTargetAndType.auxTrolleyForwardWarning], disables.atForwardAlarm, logger);
        WriteWarning(images[ColliderTargetAndType.auxTrolleyBackwardWarning], disables.atBackwardWarning, logger);
        WriteAlarm(images[ColliderTargetAndType.auxTrolleyBackwardWarning], disables.atBackwardAlarm, logger);

        WriteWarning(images[ColliderTargetAndType.bridgeLeftWarning], disables.bridgeLeftWarning, logger);
        WriteAlarm(images[ColliderTargetAndType.bridgeLeftWarning], disables.bridgeLeftAlarm, logger);
        WriteWarning(images[ColliderTargetAndType.bridgeRightWarning], disables.bridgeRightWarning, logger);
        WriteAlarm(images[ColliderTargetAndType.bridgeRightWarning], disables.bridgeRightAlarm, logger);

        var alarms = from image in images where image.Value.alarm select image.Value.objectName;
        if (alarms.Count() > 0)
        {
            overlay.gameObject.SetActive(true);
            overlay.alarmText = alarms.Last();
        }
        else
        {
            overlay.gameObject.SetActive(false);
        }
    }
    private static void WriteWarning(ImageIndicator image, bool warning, Alarms logger)
    {
       // if(!image.warning && warning)
        //{
        //   // logger.Alarm($"Предупреждение о приближении: {image.objectName}");
        //}
        image.warning = warning;
    }
    private static void WriteAlarm(ImageIndicator image, bool alarm, Alarms logger)
    {
        //if (!image.alarm && alarm)
        //{
           // logger.Alarm($"Опасность столкновения: {image.objectName}");
        //}
        image.alarm = alarm;
    }
    private static void Write_ERR_WARN_ALD(int TEMP_j, int TEMP_f, int TEMP_i, bool alarm, bool FLAG_ERR, bool FLAG_WARN, int zona_krash, Alarms logger)
    {
       
        var temp_string_crane = "";
        var temp_string_crane_el = "";
        var temp_string_crane_el_OBJ = "";
        var temp_string_crane_zona = "";

        // ==========================================================================================================================================
        #region  // Какой кран
        switch (TEMP_j)
        {   case 1: // 
                temp_string_crane = "Кран 320Т ";
                break;
            case 2: //
                temp_string_crane = "Кран 120Т ";
                break;
            case 3: // 
                temp_string_crane = "Кран 25Т ";
                break;
        default:
                temp_string_crane = "Программная ошибка ";
                break;
        }
        #endregion
        // ==========================================================================================================================================
        // ==========================================================================================================================================
        #region  // Какой элемент крана
        //1 МОСТ
        //2 ТЕЛЕЖКА 1
        //3 ТЕЛЕЖКА 1 - ПОДЪЕМ 1 - ТРОСС + КРЮК
        //4 ТЕЛЕЖКА 1 - ПОДЪЕМ 1 - тросс + крюк + траверса => ВСЕГДА FALSE
        //5 ТЕЛЕЖКА 1 - ПОДЪЕМ 1 - траверса
        //6 ТЕЛЕЖКА 1 - ПОДЪЕМ 1 - груз
        //7 ТЕЛЕЖКА 1 - ПОДЪЕМ 2 - ТРОСС + КРЮК
        //8 ТЕЛЕЖКА 1 - ПОДЪЕМ 2 - тросс + крюк + траверса => ВСЕГДА FALSE
        //9 ТЕЛЕЖКА 1 - ПОДЪЕМ 2 - траверса
        //10 ТЕЛЕЖКА 1 - ПОДЪЕМ 2 - груз
        //11 тросс + крюк + траверса + 3 подъем тросс + крюк + траверса => ВСЕГДА FALSE
        //12 ТЕЛЕЖКА 2                                                       
        //13 ТЕЛЕЖКА 2 - ПОДЪЕМ 1 - ТРОСС + КРЮК
        //14 ТЕЛЕЖКА 2 - ПОДЪЕМ 1 - тросс + крюк + траверса => ВСЕГДА FALSE
        //15 ТЕЛЕЖКА 2 - ПОДЪЕМ 1 - траверса
        //16 ТЕЛЕЖКА 2 - ПОДЪЕМ 1 - груз
        //17 ТЕЛЕЖКА 2 - ПОДЪЕМ 2 - ТРОСС + КРЮК
        //18 ТЕЛЕЖКА 2 - ПОДЪЕМ 2 - тросс + крюк + траверса => ВСЕГДА FALSE
        //19 ТЕЛЕЖКА 2 - ПОДЪЕМ 2 - траверса
        //20 ТЕЛЕЖКА 2 - ПОДЪЕМ 2 - груз
        switch (TEMP_f)
        {
            case 1: temp_string_crane_el = "МОСТ ";              break;
            case 2: temp_string_crane_el = "ТЕЛЕЖКА 1 "; break;

            case 3: temp_string_crane_el = "ПОДЪЕМ 1 - тросс + крюк "; break;
            case 4: temp_string_crane_el = "ПОДЪЕМ 1 - тросс + крюк + траверса => ВСЕГДА FALSE "; break;
            case 5: temp_string_crane_el = "ПОДЪЕМ 1 - траверса "; break;
            case 6: temp_string_crane_el = "ПОДЪЕМ 1 - груз "; break;

            case 7: temp_string_crane_el = "ПОДЪЕМ 2 - тросс + крюк "; break;
            case 8: temp_string_crane_el = "ПОДЪЕМ 2 - тросс + крюк + траверса => ВСЕГДА FALSE "; break;
            case 9: temp_string_crane_el = "ПОДЪЕМ 2 - траверса "; break;
            case 10: temp_string_crane_el = "ПОДЪЕМ 2 - груз "; break;

            case 11: temp_string_crane_el = "тросс + крюк + траверса + 3 подъем тросс + крюк + траверса => ВСЕГДА FALSE "; break;

            case 12: temp_string_crane_el = "ТЕЛЕЖКА 2 "; break;

            case 13: temp_string_crane_el = "ПОДЪЕМ 3 - тросс + крюк "; break;
            case 14: temp_string_crane_el = "ПОДЪЕМ 3 - тросс + крюк + траверса => ВСЕГДА FALSE "; break;
            case 15: temp_string_crane_el = "ПОДЪЕМ 3 - траверса "; break;
            case 16: temp_string_crane_el = "ПОДЪЕМ 3 - груз "; break;

            case 17: temp_string_crane_el = "ПОДЪЕМ 4 - тросс + крюк "; break;
            case 18: temp_string_crane_el = "ПОДЪЕМ 4 - тросс + крюк + траверса => ВСЕГДА FALSE "; break;
            case 19: temp_string_crane_el = "ПОДЪЕМ 4 - траверса "; break;
            case 20: temp_string_crane_el = "ПОДЪЕМ 4 - груз "; break;
            default: temp_string_crane_el = "Программная ошибка - ST1 ";                break;
        }
        #endregion
        // ==========================================================================================================================================
        // ==========================================================================================================================================
        #region  // Какой элемент препятствия
        // НОМЕР условно статического объекта - i
        // 1-20 - КРАН 1 - 320Т
        // 0/21-50 - КРАН 1 - 320Т - РЕЗЕРВ
        // 31-99 - ТОЛЬКО СТАТИЧНЫЕ ОБЪЕКТЫ - КОЛОННЫ И СТЕНЫ
        // 101-120 - КРАН 2 - 120Т
        // 100/121-150 - КРАН 2 - 120Т - РЕЗЕРВ
        // 151-199 - ТОЛЬКО СТАТИЧНЫЕ ОБЪЕКТЫ - РЕЗЕРВ
        // 201-220 - КРАН 3 - 25Т
        // 200/221-250 - КРАН 3 - 25Т - РЕЗЕРВ
        // 251-299 - ТОЛЬКО СТАТИЧНЫЕ ОБЪЕКТЫ - РЕЗЕРВ
        // 301 - 811 - ТОЛЬКО СТАТИЧНЫЕ ОБЪЕКТЫ - ПРЕПЯТСТВИЯ
        var new_temp_I = TEMP_i;
        if (TEMP_i >= 31 & TEMP_i <= 99)
        {
            new_temp_I = 31; // колонны
        }

        if (TEMP_i >= 301 & TEMP_i <= 811)
        {
            new_temp_I = 301; // препятствия
        }
        switch (new_temp_I)
        {
            #region КРАН 1 - 320Т
            case 1: temp_string_crane_el_OBJ = "Кран 320Т - МОСТ "; break;
            case 2: temp_string_crane_el_OBJ = "Кран 320Т - ТЕЛЕЖКА 1 "; break;

            case 3: temp_string_crane_el_OBJ = "Кран 320Т - ПОДЪЕМ 1 - тросс + крюк "; break;
            case 4: temp_string_crane_el_OBJ = "Кран 320Т - ПОДЪЕМ 1 - тросс + крюк + траверса => ВСЕГДА FALSE "; break;
            case 5: temp_string_crane_el_OBJ = "Кран 320Т - ПОДЪЕМ 1 - траверса "; break;
            case 6: temp_string_crane_el_OBJ = "Кран 320Т - ПОДЪЕМ 1 - груз "; break;

            case 7: temp_string_crane_el_OBJ = "Кран 320Т - ПОДЪЕМ 2 - тросс + крюк "; break;
            case 8: temp_string_crane_el_OBJ = "Кран 320Т - ПОДЪЕМ 2 - тросс + крюк + траверса => ВСЕГДА FALSE "; break;
            case 9: temp_string_crane_el_OBJ = "Кран 320Т - ПОДЪЕМ 2 - траверса "; break;
            case 10: temp_string_crane_el_OBJ = "Кран 320Т - ПОДЪЕМ 2 - груз "; break;

            case 11: temp_string_crane_el_OBJ = "Кран 320Т - тросс + крюк + траверса + 3 подъем тросс + крюк + траверса => ВСЕГДА FALSE "; break;

            case 12: temp_string_crane_el_OBJ = "Кран 320Т - ТЕЛЕЖКА 2 "; break;

            case 13: temp_string_crane_el_OBJ = "Кран 320Т - ПОДЪЕМ 3 - тросс + крюк "; break;
            case 14: temp_string_crane_el_OBJ = "Кран 320Т - ПОДЪЕМ 3 - тросс + крюк + траверса => ВСЕГДА FALSE "; break;
            case 15: temp_string_crane_el_OBJ = "Кран 320Т - ПОДЪЕМ 3 - траверса "; break;
            case 16: temp_string_crane_el_OBJ = "Кран 320Т - ПОДЪЕМ 3 - груз "; break;

            case 17: temp_string_crane_el_OBJ = "Кран 320Т - ПОДЪЕМ 4 - тросс + крюк "; break;
            case 18: temp_string_crane_el_OBJ = "Кран 320Т - ПОДЪЕМ 4 - тросс + крюк + траверса => ВСЕГДА FALSE "; break;
            case 19: temp_string_crane_el_OBJ = "Кран 320Т - ПОДЪЕМ 4 - траверса "; break;
            case 20: temp_string_crane_el_OBJ = "Кран 320Т - ПОДЪЕМ 4 - груз "; break;
            #endregion

            case 31: temp_string_crane_el_OBJ = @"колонной № " + TEMP_i; break;

            #region КРАН 2 - 120Т
            case 101: temp_string_crane_el_OBJ = "Кран 120Т - МОСТ "; break;
            case 102: temp_string_crane_el_OBJ = "Кран 120Т - ТЕЛЕЖКА 1 "; break;

            case 103: temp_string_crane_el_OBJ = "Кран 120Т - ПОДЪЕМ 1 - тросс + крюк "; break;
            case 104: temp_string_crane_el_OBJ = "Кран 120Т - ПОДЪЕМ 1 - тросс + крюк + траверса => ВСЕГДА FALSE "; break;
            case 105: temp_string_crane_el_OBJ = "Кран 120Т - ПОДЪЕМ 1 - траверса "; break;
            case 106: temp_string_crane_el_OBJ = "Кран 120Т - ПОДЪЕМ 1 - груз "; break;

            case 107: temp_string_crane_el_OBJ = "Кран 120Т - ПОДЪЕМ 2 - тросс + крюк "; break;
            case 108: temp_string_crane_el_OBJ = "Кран 120Т - ПОДЪЕМ 2 - тросс + крюк + траверса => ВСЕГДА FALSE "; break;
            case 109: temp_string_crane_el_OBJ = "Кран 120Т - ПОДЪЕМ 2 - траверса "; break;
            case 110: temp_string_crane_el_OBJ = "Кран 120Т - ПОДЪЕМ 2 - груз "; break;

            case 111: temp_string_crane_el_OBJ = "Кран 120Т - тросс + крюк + траверса + 3 подъем тросс + крюк + траверса => ВСЕГДА FALSE "; break;

            case 112: temp_string_crane_el_OBJ = "Кран 120Т - ТЕЛЕЖКА 2 "; break;

            case 113: temp_string_crane_el_OBJ = "Кран 120Т - ПОДЪЕМ 3 - тросс + крюк "; break;
            case 114: temp_string_crane_el_OBJ = "Кран 120Т - ПОДЪЕМ 3 - тросс + крюк + траверса => ВСЕГДА FALSE "; break;
            case 115: temp_string_crane_el_OBJ = "Кран 120Т - ПОДЪЕМ 3 - траверса "; break;
            case 116: temp_string_crane_el_OBJ = "Кран 120Т - ПОДЪЕМ 3 - груз "; break;

            case 117: temp_string_crane_el_OBJ = "Кран 120Т - ПОДЪЕМ 4 - тросс + крюк "; break;
            case 118: temp_string_crane_el_OBJ = "Кран 120Т - ПОДЪЕМ 4 - тросс + крюк + траверса => ВСЕГДА FALSE "; break;
            case 119: temp_string_crane_el_OBJ = "Кран 120Т - ПОДЪЕМ 4 - траверса "; break;
            case 120: temp_string_crane_el_OBJ = "Кран 120Т - ПОДЪЕМ 4 - груз "; break;
            #endregion

            // СТЕНА НАПРОТИВ - ДАЛЬНЯЯ    i = 131;       
            // СТЕНА НАПРОТИВ - БЛИЖНЯЯ        i = 132;       
            // СТЕНА РЯДОМ - ДАЛЬНЯЯ - НИЗКАЯ        i = 133;       
            // СТЕНА НАПРОТИВ - БЛИЖНЯЯ - ВЫСОКАЯ        i = 134;  
            // СТЕНА ТУПИКА - ЛЕВАЯ        i = 135;       
            // СТЕНА ТУПИКА - ПРАВАЯ        i = 136;
            case 131: temp_string_crane_el_OBJ = "Стена с колоннами напротив дальняя высокая"; break;
            case 132: temp_string_crane_el_OBJ = "Стена с колоннами напротив ближняя низкая "; break;
            case 133: temp_string_crane_el_OBJ = "Стена с колоннами рядом дальняя низкая "; break;
            case 134: temp_string_crane_el_OBJ = "Стена с колоннами рядом ближняя высокая "; break;
            case 135: temp_string_crane_el_OBJ = "Стена тупика левая "; break;
            case 136: temp_string_crane_el_OBJ = "Стена тупика правая "; break;

            #region КРАН 3 - 25Т
            case 201: temp_string_crane_el_OBJ = "Кран 25Т - МОСТ "; break;
            case 202: temp_string_crane_el_OBJ = "Кран 25Т - ТЕЛЕЖКА 1 "; break;

            case 203: temp_string_crane_el_OBJ = "Кран 25Т - ПОДЪЕМ 1 - тросс + крюк "; break;
            case 204: temp_string_crane_el_OBJ = "Кран 25Т - ПОДЪЕМ 1 - тросс + крюк + траверса => ВСЕГДА FALSE "; break;
            case 205: temp_string_crane_el_OBJ = "Кран 25Т - ПОДЪЕМ 1 - траверса "; break;
            case 206: temp_string_crane_el_OBJ = "Кран 25Т - ПОДЪЕМ 1 - груз "; break;

            case 207: temp_string_crane_el_OBJ = "Кран 25Т - ПОДЪЕМ 2 - тросс + крюк "; break;
            case 208: temp_string_crane_el_OBJ = "Кран 25Т - ПОДЪЕМ 2 - тросс + крюк + траверса => ВСЕГДА FALSE "; break;
            case 209: temp_string_crane_el_OBJ = "Кран 25Т - ПОДЪЕМ 2 - траверса "; break;
            case 210: temp_string_crane_el_OBJ = "Кран 25Т - ПОДЪЕМ 2 - груз "; break;

            case 211: temp_string_crane_el_OBJ = "Кран 25Т - тросс + крюк + траверса + 3 подъем тросс + крюк + траверса => ВСЕГДА FALSE "; break;

            case 212: temp_string_crane_el_OBJ = "Кран 25Т - ТЕЛЕЖКА 2 "; break;

            case 213: temp_string_crane_el_OBJ = "Кран 25Т - ПОДЪЕМ 3 - тросс + крюк "; break;
            case 214: temp_string_crane_el_OBJ = "Кран 25Т - ПОДЪЕМ 3 - тросс + крюк + траверса => ВСЕГДА FALSE "; break;
            case 215: temp_string_crane_el_OBJ = "Кран 25Т - ПОДЪЕМ 3 - траверса "; break;
            case 216: temp_string_crane_el_OBJ = "Кран 25Т - ПОДЪЕМ 3 - груз "; break;

            case 217: temp_string_crane_el_OBJ = "Кран 25Т - ПОДЪЕМ 4 - тросс + крюк "; break;
            case 218: temp_string_crane_el_OBJ = "Кран 25Т - ПОДЪЕМ 4 - тросс + крюк + траверса => ВСЕГДА FALSE "; break;
            case 219: temp_string_crane_el_OBJ = "Кран 25Т - ПОДЪЕМ 4 - траверса "; break;
            case 220: temp_string_crane_el_OBJ = "Кран 25Т - ПОДЪЕМ 4 - груз "; break;
            #endregion

            case 301: temp_string_crane_el_OBJ = @"препятствие № " + TEMP_i; break;

            default: temp_string_crane_el_OBJ = "- Программная ошибка - ST2 "; break;
        }
        #endregion
        // ==========================================================================================================================================
        // ==========================================================================================================================================
        #region  // Какая зона
       // 0 - зона сверху
       // 1 - зона снизу
       // 2 - зона спереди
       // 4 - зона сзади
       // 8 - зона справа
       // 16 - зона слева
        switch (zona_krash)
        {
            case 0: temp_string_crane_zona = " сверху "; break;
            case 1: temp_string_crane_zona = " снизу "; break;
            case 2: temp_string_crane_zona = " спереди "; break;
            case 4: temp_string_crane_zona = " сзади "; break;
            case 8: temp_string_crane_zona = " справа "; break;
            case 16: temp_string_crane_zona = " слева "; break;
            default: temp_string_crane_zona = " - Программная ошибка - ST3 "; break;
        }
        #endregion
        // ==========================================================================================================================================
       
        if (FLAG_WARN)
        {
        if (TEMP_j == 1)
        {
                logger.Alarm($"{temp_string_crane}, {temp_string_crane_zona} от {temp_string_crane_el} сближение с: {temp_string_crane_el_OBJ} / -[{TEMP_j},{TEMP_f},{TEMP_i}]",1);
               //alarmsView_320.Alarm($"{temp_string_crane}, {temp_string_crane_zona} от {temp_string_crane_el} сближение с: {temp_string_crane_el_OBJ} / -[{TEMP_j},{TEMP_f},{TEMP_i}]",1);
            }

            if (TEMP_j == 2)
        {
                logger.Alarm($"{temp_string_crane}, {temp_string_crane_zona} от {temp_string_crane_el} сближение с: {temp_string_crane_el_OBJ} / -[{TEMP_j},{TEMP_f},{TEMP_i}]",2);
               // alarmsView_120.Alarm($"{temp_string_crane}, {temp_string_crane_zona} от {temp_string_crane_el} сближение с: {temp_string_crane_el_OBJ} / -[{TEMP_j},{TEMP_f},{TEMP_i}]",2);
            }

            if (TEMP_j == 3)
        {
                logger.Alarm($"{temp_string_crane}, {temp_string_crane_zona} от {temp_string_crane_el} сближение с: {temp_string_crane_el_OBJ} / -[{TEMP_j},{TEMP_f},{TEMP_i}]",3);
              // alarmsView_25.Alarm($"{temp_string_crane}, {temp_string_crane_zona} от {temp_string_crane_el} сближение с: {temp_string_crane_el_OBJ} / -[{TEMP_j},{TEMP_f},{TEMP_i}]",3);
            }
        }
        // ==========================================================================================================================================
        // ==========================================================================================================================================
        if (FLAG_ERR)
        {
            if (TEMP_j == 1)
            {
                logger.Alarm($"{temp_string_crane}, {temp_string_crane_zona} от {temp_string_crane_el} ОПАСНОСТЬ!!! столкновения с: {temp_string_crane_el_OBJ} / -[{TEMP_j},{TEMP_f},{TEMP_i}]");
               // alarmsView_320.Alarm($"{temp_string_crane}, {temp_string_crane_zona} от {temp_string_crane_el} ОПАСНОСТЬ!!! столкновения с: {temp_string_crane_el_OBJ} / -[{TEMP_j},{TEMP_f},{TEMP_i}]");
            }

            if (TEMP_j == 2)
            {
                logger.Alarm($"{temp_string_crane}, {temp_string_crane_zona} от {temp_string_crane_el} ОПАСНОСТЬ!!! столкновения с: {temp_string_crane_el_OBJ} / -[{TEMP_j},{TEMP_f},{TEMP_i}]");
               // alarmsView_120.Alarm($"{temp_string_crane}, {temp_string_crane_zona} от {temp_string_crane_el} ОПАСНОСТЬ!!! столкновения с: {temp_string_crane_el_OBJ} / -[{TEMP_j},{TEMP_f},{TEMP_i}]");
            }

            if (TEMP_j == 3)
            {
                logger.Alarm($"{temp_string_crane}, {temp_string_crane_zona} от {temp_string_crane_el} ОПАСНОСТЬ!!! столкновения с: {temp_string_crane_el_OBJ} / -[{TEMP_j},{TEMP_f},{TEMP_i}]");
               //alarmsView_25.Alarm($"{temp_string_crane}, {temp_string_crane_zona} от {temp_string_crane_el} ОПАСНОСТЬ!!! столкновения с: {temp_string_crane_el_OBJ} / -[{TEMP_j},{TEMP_f},{TEMP_i}]");
            }
        }
        // ==========================================================================================================================================
    }
    private static void Write_ERR_WARN_ALD_PR_TR(bool TEMP_CHANGE_X, bool TEMP_CHANGE_Y, bool TEMP_CHANGE_Z,
        bool TEMP_CHANGE_STP_X, bool TEMP_CHANGE_STP_Y, bool TEMP_CHANGE_STP_Z, 
        int TEMP_i, float TEMP_x, float TEMP_y, float TEMP_z, float TEMP_st_x, float TEMP_st_y, float TEMP_st_z,
        float mem_TEMP_x, float mem_TEMP_y, float mem_TEMP_z, float mem_TEMP_st_x, float mem_TEMP_st_y, float mem_TEMP_st_z, Alarms logger)
{
        var temp_string_stp1 = $"Изменение габаритов препятствия     № {TEMP_i}:";
        var temp_string_stp2 = $"Изменение базовой точки препятствия № {TEMP_i}:";

        var temp_string_X = $"X({mem_TEMP_x})=>({TEMP_x})/ ";
        var temp_string_Y = $"Y({mem_TEMP_y})=>({TEMP_y})/ ";
        var temp_string_Z = $"Z({mem_TEMP_z})=>({TEMP_z}); ";

        var temp_string_stp_X = $"stpX({mem_TEMP_st_x})=>({TEMP_st_x})/ ";
        var temp_string_stp_Y = $"stpY({mem_TEMP_st_y})=>({TEMP_st_y})/ ";
        var temp_string_stp_Z = $"stpZ({mem_TEMP_st_z})=>({TEMP_st_z}); ";

        var temp_string1 = "";
        var temp_string2 = "";
        var temp_string3 = "";

        var temp_string_TEMP1 = $"{ temp_string_X}{temp_string_Y}{temp_string_Z}";
        var temp_string_TEMP2 = $"{ temp_string_stp_X}{temp_string_stp_Y}{temp_string_stp_Z}";
        var temp_string_TEMP = "";


        temp_string3 = $"{temp_string_stp1}{temp_string_TEMP1}";
        logger.Alarm(temp_string3, 11);
        temp_string3 = $"{temp_string_stp2}{temp_string_TEMP2}";
        logger.Alarm(temp_string3, 11);

        // ==========================================================================================================================================
        /*
        // ==========================================================================================================================================
        #region  // КАКОЙ ЭЛЕМЕНТ ПРЕПЯТСТВИЯ
        if (TEMP_CHANGE_X || TEMP_CHANGE_Y || TEMP_CHANGE_Z)
        {            temp_string1 = temp_string_stp1;        }
        if (TEMP_CHANGE_STP_X || TEMP_CHANGE_STP_Y || TEMP_CHANGE_STP_Z)    
        {            temp_string2 = temp_string_stp2;        }
        if (TEMP_CHANGE_X)
        { temp_string_TEMP1 = temp_string_X; }
        if (TEMP_CHANGE_Y)
        { temp_string_TEMP = $"{temp_string_TEMP1}{temp_string_Y}";
          temp_string_TEMP1 = temp_string_TEMP;        }
        if (TEMP_CHANGE_Z)
        {   temp_string_TEMP = $"{temp_string_TEMP1}{temp_string_Z}";
            temp_string_TEMP1 = temp_string_TEMP;      }

        temp_string_TEMP = "";

        if (TEMP_CHANGE_STP_X)
        { temp_string_TEMP2 = temp_string_stp_X; }
        if (TEMP_CHANGE_STP_Y)
        {
            temp_string_TEMP = $"{temp_string_TEMP2}{temp_string_stp_Y}";
            temp_string_TEMP2 = temp_string_TEMP;
        }
        if (TEMP_CHANGE_STP_Z)
        {
            temp_string_TEMP = $"{temp_string_TEMP2}{temp_string_stp_Z}";
            temp_string_TEMP2 = temp_string_TEMP;
        }    
        #endregion
        */
    }
    private static void WriteWarning_Alarm_ald(int temp_j, int temp_mech, int Temp_zona_krash, int WARN_OR_ERR, Alarms logger)
    {
        
        var string_temp1 = "";
        var string_temp2 = "";
        var string_temp3 = "";
        var string_temp4 = "";

        var string_temp5 = "";

        #region  // Какой кран
        switch (temp_j)
        {
            case 1: // 
                string_temp1 = "Кран 320Т ";
                break;
            case 2: //
                string_temp1 = "Кран 120Т ";
                break;
            case 3: // 
                string_temp1 = "Кран  25Т ";
                break;
            default:
                string_temp1 = "Программная ошибка ";
                break;
        }
        #endregion
        // ==========================================================================================================================================
        #region  // Какой элемент крана
        //1 МОСТ
        //2 ТЕЛЕЖКА 1
        //3 ТЕЛЕЖКА 1 - ПОДЪЕМ 1 
        //4 ТЕЛЕЖКА 1 - ПОДЪЕМ 2 
        //5 ТЕЛЕЖКА 2                                                       
        //6 ТЕЛЕЖКА 2 - ПОДЪЕМ 1
        //7 ТЕЛЕЖКА 2 - ПОДЪЕМ 2 
        switch (temp_mech)
        {
            case 1: string_temp2 = "МОСТ "; break;
            case 2: string_temp2 = "ТЕЛЕЖКА 1 "; break;
            case 3: string_temp2 = "ПОДЪЕМ 1 "; break;            
            case 4: string_temp2 = "ПОДЪЕМ 2 "; break;
            case 5: string_temp2 = "ТЕЛЕЖКА 2 "; break;
            case 6: string_temp2 = "ПОДЪЕМ 3 "; break;
            case 7: string_temp2 = "ПОДЪЕМ 4 "; break;
            default: string_temp2 = "Программная ошибка - ST1 "; break;
        }
        #endregion
        // ==========================================================================================================================================       
        #region  // Какая зона
        // 0 - зона сверху
        // 1 - зона снизу
        // 2 - зона спереди
        // 4 - зона сзади
        // 8 - зона справа
        // 16 - зона слева
        switch (Temp_zona_krash)
        {
            case 0: string_temp3 = " сверху "; break;
            case 1: string_temp3 = " снизу "; break;
            case 2: string_temp3 = " спереди "; break;
            case 4: string_temp3 = " сзади "; break;
            case 8: string_temp3 = " справа "; break;
            case 16: string_temp3 = " слева "; break;
            default: string_temp3 = " - Программная ошибка - ST3 "; break;
        }
        #endregion
        // ==========================================================================================================================================
        #region  // ФОРМИУЕМ СООБЩЕНИЕ В БАЗУ ДАННЫХ
        switch (WARN_OR_ERR)
        {
            case 1: // WARNING
                string_temp4 = "ПРЕДУПРЕЖДЕНИЕ ";
                string_temp5 = ($"{string_temp4}{string_temp1}{string_temp2}{string_temp3}");

               
                    logger.Alarm(string_temp5, 0);
               

                break;
            case 2: // ERROR
                string_temp4 = "СТОЛКНОВЕНИЕ ";
                string_temp5 = ($"{string_temp4}{string_temp1}{string_temp2}{string_temp3}");

                
                    logger.Alarm(string_temp5, 0);
               


                break;
           
            default:
                string_temp4 = "Программная ошибка ";
                break;
        }
        #endregion
        

        
    }
    private static void Write_ERR_WARN_ALD_HOIST_TR_CRANEs(bool TEMP_CHANGE_X, bool TEMP_CHANGE_Y, bool TEMP_CHANGE_Z,
        bool TEMP_CHANGE_STP_X, bool TEMP_CHANGE_STP_Y, bool TEMP_CHANGE_STP_Z,
        int TEMP_i, float TEMP_x, float TEMP_y, float TEMP_z, float TEMP_st_x, float TEMP_st_y, float TEMP_st_z,
        float mem_TEMP_x, float mem_TEMP_y, float mem_TEMP_z, float mem_TEMP_st_x, float mem_TEMP_st_y, float mem_TEMP_st_z, Alarms logger)
    {
        var temp_string_stp10 = "";

        switch (TEMP_i)
        {
            case 5: temp_string_stp10 = "Кран 320Т подъем 1-1 траверса"; break;   	// ТРАВЕРСА НА ПОДЪЕМЕ 1-1 - КРАН 320Т
            case 6: temp_string_stp10 = "Кран 320Т подъем 1-1     груз"; break;       	// ГРУЗ НА ПОДЪЕМЕ 1-1 - КРАН 320Т
            case 9: temp_string_stp10 =  "Кран 320Т подъем 1-2 траверса"; break;   	// ТРАВЕРСА НА ПОДЪЕМЕ 1-2 - КРАН 320Т
            case 10: temp_string_stp10 = "Кран 320Т подъем 1-2     груз"; break;  		// ГРУЗ НА ПОДЪЕМЕ 1-2 - КРАН 320Т
            case 15: temp_string_stp10 = "Кран 320Т подъем 2-1 траверса"; break;  	// ТРАВЕРСА НА ПОДЪЕМЕ 2-1 - КРАН 320Т
            case 16: temp_string_stp10 = "Кран 320Т подъем 2-1     груз"; break;  		// ГРУЗ НА ПОДЪЕМЕ 2-1 - КРАН 320Т
            case 19: temp_string_stp10 = "Кран 320Т подъем 2-2 траверса"; break;  	// ТРАВЕРСА НА ПОДЪЕМЕ 2-2 - КРАН 320Т
            case 20: temp_string_stp10 = "Кран 320Т подъем 2-2     груз"; break;  		// ГРУЗ НА ПОДЪЕМЕ 2-2 - КРАН 320Т

            case 105: temp_string_stp10 = "Кран 120Т подъем 1-1 траверса"; break;   	// ТРАВЕРСА НА ПОДЪЕМЕ 1-1 - КРАН 120Т
            case 106: temp_string_stp10 = "Кран 120Т подъем 1-1     груз"; break;       	// ГРУЗ НА ПОДЪЕМЕ 1-1 - КРАН 120Т
            case 109: temp_string_stp10 = "Кран 120Т подъем 1-2 траверса"; break;   	// ТРАВЕРСА НА ПОДЪЕМЕ 1-2 - КРАН 120Т
            case 110: temp_string_stp10 = "Кран 120Т подъем 1-2     груз"; break;  		// ГРУЗ НА ПОДЪЕМЕ 1-2 - КРАН 120Т
            case 115: temp_string_stp10 = "Кран 120Т подъем 2-1 траверса"; break;  	// ТРАВЕРСА НА ПОДЪЕМЕ 2-1 - КРАН 120Т
            case 116: temp_string_stp10 = "Кран 120Т подъем 2-1     груз"; break;  		// ГРУЗ НА ПОДЪЕМЕ 2-1 - КРАН 120Т
            case 119: temp_string_stp10 = "Кран 120Т подъем 2-2 траверса"; break;  	// ТРАВЕРСА НА ПОДЪЕМЕ 2-2 - КРАН 120Т
            case 120: temp_string_stp10 = "Кран 120Т подъем 2-2     груз"; break;  		// ГРУЗ НА ПОДЪЕМЕ 2-2 - КРАН 120Т

            case 205: temp_string_stp10 = "Кран 25Т подъем 1-1 траверса"; break;   	// ТРАВЕРСА НА ПОДЪЕМЕ 1-1 - КРАН 25Т 
            case 206: temp_string_stp10 = "Кран 25Т подъем 1-1     груз"; break;       	// ГРУЗ НА ПОДЪЕМЕ 1-1 - КРАН 25Т 
            case 209: temp_string_stp10 = "Кран 25Т подъем 1-2 траверса"; break;   	// ТРАВЕРСА НА ПОДЪЕМЕ 1-2 - КРАН 25Т 
            case 210: temp_string_stp10 = "Кран 25Т подъем 1-2     груз"; break;  		// ГРУЗ НА ПОДЪЕМЕ 1-2 - КРАН 25Т 
            case 215: temp_string_stp10 = "Кран 25Т подъем 2-1 траверса"; break;  	// ТРАВЕРСА НА ПОДЪЕМЕ 2-1 - КРАН 25Т 
            case 216: temp_string_stp10 = "Кран 25Т подъем 2-1     груз"; break;  		// ГРУЗ НА ПОДЪЕМЕ 2-1 - КРАН 25Т 
            case 219: temp_string_stp10 = "Кран 25Т подъем 2-2 траверса"; break;  	// ТРАВЕРСА НА ПОДЪЕМЕ 2-2 - КРАН 25Т 
            case 220: temp_string_stp10 = "Кран 25Т подъем 2-2     груз"; break;  		// ГРУЗ НА ПОДЪЕМЕ 2-2 - КРАН 25Т 

            default: temp_string_stp10 = ""; break;
        }

        var temp_string_stp1 = $"{temp_string_stp10} изменение габаритов     /№ {TEMP_i}:";
        var temp_string_stp2 = $"{temp_string_stp10} изменение базовой точки /№ {TEMP_i}:";

        var temp_string_X = $"X({mem_TEMP_x})=>({TEMP_x})/ ";
        var temp_string_Y = $"Y({mem_TEMP_y})=>({TEMP_y})/ ";
        var temp_string_Z = $"Z({mem_TEMP_z})=>({TEMP_z}); ";

        var temp_string_stp_X = $"stpX({mem_TEMP_st_x})=>({TEMP_st_x})/ ";
        var temp_string_stp_Y = $"stpY({mem_TEMP_st_y})=>({TEMP_st_y})/ ";
        var temp_string_stp_Z = $"stpZ({mem_TEMP_st_z})=>({TEMP_st_z}); ";

        var temp_string1 = $"{temp_string_X}{temp_string_Y}{temp_string_Z}";
        var temp_string2 = $"{temp_string_stp_X}{temp_string_stp_Y}{temp_string_stp_Z}";
        var temp_string3 = "";

        var temp_string_TEMP1 = "";
        var temp_string_TEMP2 = "";
        var temp_string_TEMP = "";

        // ==========================================================================================================================================
        temp_string_TEMP1 = $"{temp_string_stp1}{temp_string1}";
        logger.Alarm(temp_string_TEMP1, 21);
        temp_string_TEMP2 = $"{temp_string_stp2}{temp_string2}";
        logger.Alarm(temp_string_TEMP2, 21);
        // ==========================================================================================================================================

    }

    private static void Write_CHANGE_traversy(bool TEMP_CHANGE_X, bool TEMP_CHANGE_Y, bool TEMP_CHANGE_Z,
        bool TEMP_CHANGE_ID, bool TEMP_CHANGE_TWO_HOOK, bool TEMP_CHANGE_NAME,
        int TEMP_i, float TEMP_x, float TEMP_y, float TEMP_z, int TEMP_ID, bool TEMP_TWO_HOOK, string TEMP_NAME,
        float mem_TEMP_x, float mem_TEMP_y, float mem_TEMP_z, int mem_TEMP_ID, bool mem_TEMP_TWO_HOOK, string mem_TEMP_NAME, Alarms logger)
    {
        var temp_string_stp10 = "ТРАВЕРСЫ";

       /* switch (TEMP_i)
        {
            case 5: temp_string_stp10 = "Кран 320Т подъем 1-1 траверса"; break;   	// ТРАВЕРСА НА ПОДЪЕМЕ 1-1 - КРАН 320Т
            case 6: temp_string_stp10 = "Кран 320Т подъем 1-1 груз"; break;       	// ГРУЗ НА ПОДЪЕМЕ 1-1 - КРАН 320Т
            case 9: temp_string_stp10 = "Кран 320Т подъем 1-2 траверса"; break;   	// ТРАВЕРСА НА ПОДЪЕМЕ 1-2 - КРАН 320Т
            case 10: temp_string_stp10 = "Кран 320Т подъем 1-2 груз"; break;  		// ГРУЗ НА ПОДЪЕМЕ 1-2 - КРАН 320Т
            case 15: temp_string_stp10 = "Кран 320Т подъем 2-1 траверса"; break;  	// ТРАВЕРСА НА ПОДЪЕМЕ 2-1 - КРАН 320Т
            case 16: temp_string_stp10 = "Кран 320Т подъем 2-1 груз"; break;  		// ГРУЗ НА ПОДЪЕМЕ 2-1 - КРАН 320Т
            case 19: temp_string_stp10 = "Кран 320Т подъем 2-2 траверса"; break;  	// ТРАВЕРСА НА ПОДЪЕМЕ 2-2 - КРАН 320Т
            case 20: temp_string_stp10 = "Кран 320Т подъем 2-2 груз"; break;  		// ГРУЗ НА ПОДЪЕМЕ 2-2 - КРАН 320Т

            case 105: temp_string_stp10 = "Кран 120Т подъем 1-1 траверса"; break;   	// ТРАВЕРСА НА ПОДЪЕМЕ 1-1 - КРАН 120Т
            case 106: temp_string_stp10 = "Кран 120Т подъем 1-1 груз"; break;       	// ГРУЗ НА ПОДЪЕМЕ 1-1 - КРАН 120Т
            case 109: temp_string_stp10 = "Кран 120Т подъем 1-2 траверса"; break;   	// ТРАВЕРСА НА ПОДЪЕМЕ 1-2 - КРАН 120Т
            case 110: temp_string_stp10 = "Кран 120Т подъем 1-2 груз"; break;  		// ГРУЗ НА ПОДЪЕМЕ 1-2 - КРАН 120Т
            case 115: temp_string_stp10 = "Кран 120Т подъем 2-1 траверса"; break;  	// ТРАВЕРСА НА ПОДЪЕМЕ 2-1 - КРАН 120Т
            case 116: temp_string_stp10 = "Кран 120Т подъем 2-1 груз"; break;  		// ГРУЗ НА ПОДЪЕМЕ 2-1 - КРАН 120Т
            case 119: temp_string_stp10 = "Кран 120Т подъем 2-2 траверса"; break;  	// ТРАВЕРСА НА ПОДЪЕМЕ 2-2 - КРАН 120Т
            case 120: temp_string_stp10 = "Кран 120Т подъем 2-2 груз"; break;  		// ГРУЗ НА ПОДЪЕМЕ 2-2 - КРАН 120Т

            case 205: temp_string_stp10 = "Кран 25Т подъем 1-1 траверса"; break;   	// ТРАВЕРСА НА ПОДЪЕМЕ 1-1 - КРАН 25Т 
            case 206: temp_string_stp10 = "Кран 25Т подъем 1-1 груз"; break;       	// ГРУЗ НА ПОДЪЕМЕ 1-1 - КРАН 25Т 
            case 209: temp_string_stp10 = "Кран 25Т подъем 1-2 траверса"; break;   	// ТРАВЕРСА НА ПОДЪЕМЕ 1-2 - КРАН 25Т 
            case 210: temp_string_stp10 = "Кран 25Т подъем 1-2 груз"; break;  		// ГРУЗ НА ПОДЪЕМЕ 1-2 - КРАН 25Т 
            case 215: temp_string_stp10 = "Кран 25Т подъем 2-1 траверса"; break;  	// ТРАВЕРСА НА ПОДЪЕМЕ 2-1 - КРАН 25Т 
            case 216: temp_string_stp10 = "Кран 25Т подъем 2-1 груз"; break;  		// ГРУЗ НА ПОДЪЕМЕ 2-1 - КРАН 25Т 
            case 219: temp_string_stp10 = "Кран 25Т подъем 2-2 траверса"; break;  	// ТРАВЕРСА НА ПОДЪЕМЕ 2-2 - КРАН 25Т 
            case 220: temp_string_stp10 = "Кран 25Т подъем 2-2 груз"; break;  		// ГРУЗ НА ПОДЪЕМЕ 2-2 - КРАН 25Т 

            default: temp_string_stp10 = ""; break;
        }
        */
        var temp_string_stp1 = $"{temp_string_stp10} изменение габаритов  /№ {TEMP_i}:";
        var temp_string_stp2 = $"{temp_string_stp10} изменение параметров /№ {TEMP_i}:";

        var temp_string_X = $"X({mem_TEMP_x})=>({TEMP_x})/ ";
        var temp_string_Y = $"Y({mem_TEMP_y})=>({TEMP_y})/ ";
        var temp_string_Z = $"Z({mem_TEMP_z})=>({TEMP_z}); ";

        var temp_string_ID = $"ID({mem_TEMP_ID})=>({TEMP_ID})/ ";
        var temp_string_TWO_HOOK = $"TwoHook({mem_TEMP_TWO_HOOK})=>({TEMP_TWO_HOOK})/ ";
        var temp_string_NAME = $"Name({mem_TEMP_NAME})=>({TEMP_NAME}); ";

        var temp_string1 = "";
        var temp_string2 = "";
        var temp_string3 = "";

        var temp_string_TEMP1 = $"{temp_string_X}{temp_string_Y}{temp_string_Z}";
        var temp_string_TEMP2 = $"{temp_string_ID}{temp_string_TWO_HOOK}{temp_string_NAME}";
        var temp_string_TEMP = "";

       
        temp_string3 = $"{temp_string_stp2}{temp_string_TEMP2}";
        logger.Alarm(temp_string3, 31);

        temp_string3 = $"{temp_string_stp1}{temp_string_TEMP1}";
        logger.Alarm(temp_string3, 31);
        /*
        // ==========================================================================================================================================
        #region  // КАКОЙ ЭЛЕМЕНТ координаты
        if (TEMP_CHANGE_X || TEMP_CHANGE_Y || TEMP_CHANGE_Z)
        { temp_string1 = temp_string_X; }
        if (TEMP_CHANGE_ID || TEMP_CHANGE_TWO_HOOK || TEMP_CHANGE_NAME)
        { temp_string2 = temp_string_ID; }

        if (TEMP_CHANGE_X)
        { temp_string_TEMP1 = temp_string_X; }
        if (TEMP_CHANGE_Y)
        {
            temp_string_TEMP = $"{temp_string_TEMP1}{temp_string_Y}";
            temp_string_TEMP1 = temp_string_TEMP;
        }
        if (TEMP_CHANGE_Z)
        {
            temp_string_TEMP = $"{temp_string_TEMP1}{temp_string_Z}";
            temp_string_TEMP1 = temp_string_TEMP;
        }

        temp_string_TEMP = "";

        if (TEMP_CHANGE_ID)
        { temp_string_TEMP2 = temp_string_ID; }
        if (TEMP_CHANGE_TWO_HOOK)
        {
            temp_string_TEMP = $"{temp_string_TEMP2}{temp_string_TWO_HOOK}";
            temp_string_TEMP2 = temp_string_TEMP;
        }
        if (TEMP_CHANGE_NAME)
        {
            temp_string_TEMP = $"{temp_string_TEMP2}{temp_string_NAME}";
            temp_string_TEMP2 = temp_string_TEMP;
        }
        #endregion
        */

        // ==========================================================================================================================================

    }

}