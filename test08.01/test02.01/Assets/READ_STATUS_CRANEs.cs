using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class READ_STATUS_CRANEs : MonoBehaviour
{
    /**
    * Скрипты кранов
    * должны идти в том-же порядке, что и элементы массива crane
    */
    public List<Crane> craneScripts;
    public List<AlarmOverlay> alarmOverlays;
    public Alarms alarmsView_ST;

    // Start is called before the first frame update
    void Start()
    {
       

    }
    // Update is called once per frame
    void Update()
    {
        // ******************************************************************************************************************************************
        #region ГРУППОВЫЕ ТЕКСТОВЫЕ СООБЩЕНИЯ ДЛЯ ФОРМИРОВАНИЯ ИТОГОВОГО

        var temp_STRING = "";
        var temp_STRING_TIP_CRANES = "";

        var temp_STRING_START = " СТАРТ ";
        var temp_STRING_STOP = " СТОП ";
                
        var temp_STRING_MOVE_BR_R = " движения моста вправо ";
        var temp_STRING_MOVE_BR_L = " движения моста влево ";

        var temp_STRING_MOVE_TR_1_F = " движения главной тележки вперед ";
        var temp_STRING_MOVE_TR_1_B = " движения главной тележки назад ";

        var temp_STRING_MOVE_H_1_1_UP = " подъема главного подъема 1-1 ";
        var temp_STRING_MOVE_H_1_1_DOWN = " спуска главного подъема 1-1 ";

        var temp_STRING_MOVE_H_1_2_UP = " подъема главного подъема 1-2 ";
        var temp_STRING_MOVE_H_1_2_DOWN = " спуска главного подъема 1-2 ";

        var temp_STRING_MOVE_TR_2_F = " движения вспомогательной тележки вперед ";
        var temp_STRING_MOVE_TR_2_B = " движения вспомогательной тележки назад  ";

        var temp_STRING_MOVE_H_2_1_UP = " подъема главного подъема 2-1 ";
        var temp_STRING_MOVE_H_2_1_DOWN = " спуска главного подъема 2-1 ";

        var temp_STRING_MOVE_H_2_2_UP = " подъема главного подъема 2-2 ";
        var temp_STRING_MOVE_H_2_2_DOWN = " спуска главного подъема 2-2 ";
        #endregion
        // ******************************************************************************************************************************************
        #region ЧИТАЕМ ДАННЫЕ ДЛЯ ДАЛЬНЕЙШЕЙ ОБРАБОТКИ !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        // СМОТРИ СКРИПТ CraneConnection.cs
        // там осуществляется запись состояния по кранам
        #endregion
        // ******************************************************************************************************************************************
        #region ФОРМИРУЕМ СООБЩЕНИЯ В БАЗУ ДАННЫХ SQL - СОСТОЯНИЯ ОСНОВНЫХ МЕХАНИЗМОВ КРАНА Т.Е. В КАКИХ НАПРАВЛЕНИЯХ ДВИГАЮТСЯ
        // ******************************************************************************************************************************************
        var COUNTER_cranes = 0;
        // AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
        // ==========================================================================================================================================
        #region КРАН - 320Т
        // ==========================================================================================================================================
        COUNTER_cranes = 1;// 1 - 320Т / 2 - 120Т / 3 - 25Т -  ТАКЖЕ ОПРЕДЕЛЯЕТ ГРУППУ СООБЩЕНИЙ => alarmsView_ST.Alarm(temp_STRING, COUNTER_cranes);
        // ==========================================================================================================================================
        #region  // Какой кран
        switch (COUNTER_cranes)
        {
            case 1: // 
                temp_STRING_TIP_CRANES = "Кран 320Т: ";
                break;
            case 2: //
                temp_STRING_TIP_CRANES = "Кран 120Т: ";
                break;
            case 3: // 
                temp_STRING_TIP_CRANES = "Кран  25Т: ";
                break;
            default:
                temp_STRING_TIP_CRANES = "Программная ошибка ";
                break;
        }
        #endregion
        // ==========================================================================================================================================
        #region  МОСТ - ВЛЕВО => MOVE_BR_L
        // =========================================================================================================
        if (!Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_BR_L & Globals.CRANEs_maiL[COUNTER_cranes].MOVE_BR_L)
        {
            temp_STRING = $"{temp_STRING_TIP_CRANES}{temp_STRING_START}{temp_STRING_MOVE_BR_L}";
            alarmsView_ST.Alarm(temp_STRING, COUNTER_cranes);
        }
        if (Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_BR_L & !Globals.CRANEs_maiL[COUNTER_cranes].MOVE_BR_L)
        {
            temp_STRING = $"{temp_STRING_TIP_CRANES}{temp_STRING_STOP}{temp_STRING_MOVE_BR_L}";
            alarmsView_ST.Alarm(temp_STRING, COUNTER_cranes);
        }
        Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_BR_L = Globals.CRANEs_maiL[COUNTER_cranes].MOVE_BR_L;
        // =========================================================================================================
        #endregion
        // ==========================================================================================================================================
        #region  МОСТ - ВПРАВО => MOVE_BR_R
        // =========================================================================================================
        if (!Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_BR_R & Globals.CRANEs_maiL[COUNTER_cranes].MOVE_BR_R)
        {
            temp_STRING = $"{temp_STRING_TIP_CRANES}{temp_STRING_START}{temp_STRING_MOVE_BR_R}";
            alarmsView_ST.Alarm(temp_STRING, COUNTER_cranes);
        }
        if (Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_BR_R & !Globals.CRANEs_maiL[COUNTER_cranes].MOVE_BR_R)
        {
            temp_STRING = $"{temp_STRING_TIP_CRANES}{temp_STRING_STOP}{temp_STRING_MOVE_BR_R}";
            alarmsView_ST.Alarm(temp_STRING, COUNTER_cranes);
        }
        Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_BR_R = Globals.CRANEs_maiL[COUNTER_cranes].MOVE_BR_R;
        // =========================================================================================================
        #endregion
        // ==========================================================================================================================================
        #region  ТЕЛЕЖКА 1 - ВПЕРЕД => MOVE_TR_1_F
        // =========================================================================================================
        if (!Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_TR_1_F & Globals.CRANEs_maiL[COUNTER_cranes].MOVE_TR_1_F)
        {
            temp_STRING = $"{temp_STRING_TIP_CRANES}{temp_STRING_START}{temp_STRING_MOVE_TR_1_F}";
            alarmsView_ST.Alarm(temp_STRING, COUNTER_cranes);
        }
        if (Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_TR_1_F & !Globals.CRANEs_maiL[COUNTER_cranes].MOVE_TR_1_F)
        {
            temp_STRING = $"{temp_STRING_TIP_CRANES}{temp_STRING_STOP}{temp_STRING_MOVE_TR_1_F}";
            alarmsView_ST.Alarm(temp_STRING, COUNTER_cranes);
        }
        Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_TR_1_F = Globals.CRANEs_maiL[COUNTER_cranes].MOVE_TR_1_F;
        // =========================================================================================================
        #endregion
        // ==========================================================================================================================================
        #region  ТЕЛЕЖКА 1 - НАЗАД => MOVE_TR_1_B
        // =========================================================================================================
        if (!Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_TR_1_B & Globals.CRANEs_maiL[COUNTER_cranes].MOVE_TR_1_B)
        {
            temp_STRING = $"{temp_STRING_TIP_CRANES}{temp_STRING_START}{temp_STRING_MOVE_TR_1_B}";
            alarmsView_ST.Alarm(temp_STRING, COUNTER_cranes);
        }
        if (Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_TR_1_B & !Globals.CRANEs_maiL[COUNTER_cranes].MOVE_TR_1_B)
        {
            temp_STRING = $"{temp_STRING_TIP_CRANES}{temp_STRING_STOP}{temp_STRING_MOVE_TR_1_B}";
            alarmsView_ST.Alarm(temp_STRING, COUNTER_cranes);
        }
        Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_TR_1_B = Globals.CRANEs_maiL[COUNTER_cranes].MOVE_TR_1_B;
        // =========================================================================================================
        #endregion       
        // ==========================================================================================================================================
        #region  ПОДЪЕМ 1-1 - ПОДЪЕМ => MOVE_H_1_1_UP
        // =========================================================================================================
        if (!Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_H_1_1_UP & Globals.CRANEs_maiL[COUNTER_cranes].MOVE_H_1_1_UP)
        {
            temp_STRING = $"{temp_STRING_TIP_CRANES}{temp_STRING_START}{temp_STRING_MOVE_H_1_1_UP}";
            alarmsView_ST.Alarm(temp_STRING, COUNTER_cranes);
        }
        if (Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_H_1_1_UP & !Globals.CRANEs_maiL[COUNTER_cranes].MOVE_H_1_1_UP)
        {
            temp_STRING = $"{temp_STRING_TIP_CRANES}{temp_STRING_STOP}{temp_STRING_MOVE_H_1_1_UP}";
            alarmsView_ST.Alarm(temp_STRING, COUNTER_cranes);
        }
        Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_H_1_1_UP = Globals.CRANEs_maiL[COUNTER_cranes].MOVE_H_1_1_UP;
        // =========================================================================================================
        #endregion
        // ==========================================================================================================================================
        #region  ПОДЪЕМ 1-1 - СРУСК  => MOVE_H_1_1_DOWN
        // =========================================================================================================
        if (!Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_H_1_1_DOWN & Globals.CRANEs_maiL[COUNTER_cranes].MOVE_H_1_1_DOWN)
        {
            temp_STRING = $"{temp_STRING_TIP_CRANES}{temp_STRING_START}{temp_STRING_MOVE_H_1_1_DOWN}";
            alarmsView_ST.Alarm(temp_STRING, COUNTER_cranes);
        }
        if (Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_H_1_1_DOWN & !Globals.CRANEs_maiL[COUNTER_cranes].MOVE_H_1_1_DOWN)
        {
            temp_STRING = $"{temp_STRING_TIP_CRANES}{temp_STRING_STOP}{temp_STRING_MOVE_H_1_1_DOWN}";
            alarmsView_ST.Alarm(temp_STRING, COUNTER_cranes);
        }
        Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_H_1_1_DOWN = Globals.CRANEs_maiL[COUNTER_cranes].MOVE_H_1_1_DOWN;
        // =========================================================================================================
        #endregion       
        // ==========================================================================================================================================
        #region  ПОДЪЕМ 1-2 - ПОДЪЕМ => MOVE_H_1_2_UP
        // =========================================================================================================
        if (!Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_H_1_2_UP & Globals.CRANEs_maiL[COUNTER_cranes].MOVE_H_1_2_UP)
        {
            temp_STRING = $"{temp_STRING_TIP_CRANES}{temp_STRING_START}{temp_STRING_MOVE_H_1_2_UP}";
            alarmsView_ST.Alarm(temp_STRING, COUNTER_cranes);
        }
        if (Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_H_1_2_UP & !Globals.CRANEs_maiL[COUNTER_cranes].MOVE_H_1_2_UP)
        {
            temp_STRING = $"{temp_STRING_TIP_CRANES}{temp_STRING_STOP}{temp_STRING_MOVE_H_1_2_UP}";
            alarmsView_ST.Alarm(temp_STRING, COUNTER_cranes);
        }
        Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_H_1_2_UP = Globals.CRANEs_maiL[COUNTER_cranes].MOVE_H_1_2_UP;
        // =========================================================================================================
        #endregion
        // ==========================================================================================================================================
        #region  ПОДЪЕМ 1-2 - СРУСК  => MOVE_H_1_2_DOWN
        // =========================================================================================================
        if (!Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_H_1_2_DOWN & Globals.CRANEs_maiL[COUNTER_cranes].MOVE_H_1_2_DOWN)
        {
            temp_STRING = $"{temp_STRING_TIP_CRANES}{temp_STRING_START}{temp_STRING_MOVE_H_1_2_DOWN}";
            alarmsView_ST.Alarm(temp_STRING, COUNTER_cranes);
        }
        if (Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_H_1_2_DOWN & !Globals.CRANEs_maiL[COUNTER_cranes].MOVE_H_1_2_DOWN)
        {
            temp_STRING = $"{temp_STRING_TIP_CRANES}{temp_STRING_STOP}{temp_STRING_MOVE_H_1_2_DOWN}";
            alarmsView_ST.Alarm(temp_STRING, COUNTER_cranes);
        }
        Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_H_1_2_DOWN = Globals.CRANEs_maiL[COUNTER_cranes].MOVE_H_1_2_DOWN;
        // =========================================================================================================
        #endregion
        // ==========================================================================================================================================
        #region  ТЕЛЕЖКА 2 - ВПЕРЕД => MOVE_TR_2_F
        // =========================================================================================================
        if (!Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_TR_2_F & Globals.CRANEs_maiL[COUNTER_cranes].MOVE_TR_2_F)
        {
            temp_STRING = $"{temp_STRING_TIP_CRANES}{temp_STRING_START}{temp_STRING_MOVE_TR_2_F}";
            alarmsView_ST.Alarm(temp_STRING, COUNTER_cranes);
        }
        if (Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_TR_2_F & !Globals.CRANEs_maiL[COUNTER_cranes].MOVE_TR_2_F)
        {
            temp_STRING = $"{temp_STRING_TIP_CRANES}{temp_STRING_STOP}{temp_STRING_MOVE_TR_2_F}";
            alarmsView_ST.Alarm(temp_STRING, COUNTER_cranes);
        }
        Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_TR_2_F = Globals.CRANEs_maiL[COUNTER_cranes].MOVE_TR_2_F;
        // =========================================================================================================
        #endregion
        // ==========================================================================================================================================
        #region  ТЕЛЕЖКА 2 - НАЗАД => MOVE_TR_2_B
        // =========================================================================================================
        if (!Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_TR_2_B & Globals.CRANEs_maiL[COUNTER_cranes].MOVE_TR_2_B)
        {
            temp_STRING = $"{temp_STRING_TIP_CRANES}{temp_STRING_START}{temp_STRING_MOVE_TR_2_B}";
            alarmsView_ST.Alarm(temp_STRING, COUNTER_cranes);
        }
        if (Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_TR_2_B & !Globals.CRANEs_maiL[COUNTER_cranes].MOVE_TR_2_B)
        {
            temp_STRING = $"{temp_STRING_TIP_CRANES}{temp_STRING_STOP}{temp_STRING_MOVE_TR_2_B}";
            alarmsView_ST.Alarm(temp_STRING, COUNTER_cranes);
        }
        Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_TR_2_B = Globals.CRANEs_maiL[COUNTER_cranes].MOVE_TR_2_B;
        // =========================================================================================================
        #endregion       
        // ==========================================================================================================================================
        #region  ПОДЪЕМ 2-1 - ПОДЪЕМ => MOVE_H_2_1_UP
        // =========================================================================================================
        if (!Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_H_2_1_UP & Globals.CRANEs_maiL[COUNTER_cranes].MOVE_H_2_1_UP)
        {
            temp_STRING = $"{temp_STRING_TIP_CRANES}{temp_STRING_START}{temp_STRING_MOVE_H_2_1_UP}";
            alarmsView_ST.Alarm(temp_STRING, COUNTER_cranes);
        }
        if (Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_H_2_1_UP & !Globals.CRANEs_maiL[COUNTER_cranes].MOVE_H_2_1_UP)
        {
            temp_STRING = $"{temp_STRING_TIP_CRANES}{temp_STRING_STOP}{temp_STRING_MOVE_H_2_1_UP}";
            alarmsView_ST.Alarm(temp_STRING, COUNTER_cranes);
        }
        Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_H_2_1_UP = Globals.CRANEs_maiL[COUNTER_cranes].MOVE_H_2_1_UP;
        // =========================================================================================================
        #endregion
        // ==========================================================================================================================================
        #region  ПОДЪЕМ 2-1 - СРУСК  => MOVE_H_2_1_DOWN
        // =========================================================================================================
        if (!Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_H_2_1_DOWN & Globals.CRANEs_maiL[COUNTER_cranes].MOVE_H_2_1_DOWN)
        {
            temp_STRING = $"{temp_STRING_TIP_CRANES}{temp_STRING_START}{temp_STRING_MOVE_H_2_1_DOWN}";
            alarmsView_ST.Alarm(temp_STRING, COUNTER_cranes);
        }
        if (Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_H_2_1_DOWN & !Globals.CRANEs_maiL[COUNTER_cranes].MOVE_H_2_1_DOWN)
        {
            temp_STRING = $"{temp_STRING_TIP_CRANES}{temp_STRING_STOP}{temp_STRING_MOVE_H_2_1_DOWN}";
            alarmsView_ST.Alarm(temp_STRING, COUNTER_cranes);
        }
        Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_H_2_1_DOWN = Globals.CRANEs_maiL[COUNTER_cranes].MOVE_H_2_1_DOWN;
        // =========================================================================================================
        #endregion       
        // ==========================================================================================================================================
        #region  ПОДЪЕМ 2-2 - ПОДЪЕМ => MOVE_H_2_2_UP
        // =========================================================================================================
        if (!Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_H_2_2_UP & Globals.CRANEs_maiL[COUNTER_cranes].MOVE_H_2_2_UP)
        {
            temp_STRING = $"{temp_STRING_TIP_CRANES}{temp_STRING_START}{temp_STRING_MOVE_H_2_2_UP}";
            alarmsView_ST.Alarm(temp_STRING, COUNTER_cranes);
        }
        if (Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_H_2_2_UP & !Globals.CRANEs_maiL[COUNTER_cranes].MOVE_H_2_2_UP)
        {
            temp_STRING = $"{temp_STRING_TIP_CRANES}{temp_STRING_STOP}{temp_STRING_MOVE_H_2_2_UP}";
            alarmsView_ST.Alarm(temp_STRING, COUNTER_cranes);
        }
        Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_H_2_2_UP = Globals.CRANEs_maiL[COUNTER_cranes].MOVE_H_2_2_UP;
        // =========================================================================================================
        #endregion
        // ==========================================================================================================================================
        #region  ПОДЪЕМ 2-2 - СРУСК  => MOVE_H_2_2_DOWN
        // =========================================================================================================
        if (!Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_H_2_2_DOWN & Globals.CRANEs_maiL[COUNTER_cranes].MOVE_H_2_2_DOWN)
        {
            temp_STRING = $"{temp_STRING_TIP_CRANES}{temp_STRING_START}{temp_STRING_MOVE_H_2_2_DOWN}";
            alarmsView_ST.Alarm(temp_STRING, COUNTER_cranes);
        }
        if (Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_H_2_2_DOWN & !Globals.CRANEs_maiL[COUNTER_cranes].MOVE_H_2_2_DOWN)
        {
            temp_STRING = $"{temp_STRING_TIP_CRANES}{temp_STRING_STOP}{temp_STRING_MOVE_H_2_2_DOWN}";
            alarmsView_ST.Alarm(temp_STRING, COUNTER_cranes);
        }
        Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_H_2_2_DOWN = Globals.CRANEs_maiL[COUNTER_cranes].MOVE_H_2_2_DOWN;
        // =========================================================================================================
        #endregion
        // ==========================================================================================================================================
        #endregion
        // ==========================================================================================================================================
        // AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
        // ==========================================================================================================================================
        #region КРАН - 120Т
        // ==========================================================================================================================================
        COUNTER_cranes = 2;// 1 - 320Т / 2 - 120Т / 3 - 25Т -  ТАКЖЕ ОПРЕДЕЛЯЕТ ГРУППУ СООБЩЕНИЙ => alarmsView_ST.Alarm(temp_STRING, COUNTER_cranes);
        // ==========================================================================================================================================
        #region  // Какой кран
        switch (COUNTER_cranes)
        {
            case 1: // 
                temp_STRING_TIP_CRANES = "Кран 320Т: ";
                break;
            case 2: //
                temp_STRING_TIP_CRANES = "Кран 120Т: ";
                break;
            case 3: // 
                temp_STRING_TIP_CRANES = "Кран  25Т: ";
                break;
            default:
                temp_STRING_TIP_CRANES = "Программная ошибка ";
                break;
        }
        #endregion
        // ==========================================================================================================================================
        #region  МОСТ - ВЛЕВО => MOVE_BR_L
        // =========================================================================================================
        if (!Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_BR_L & Globals.CRANEs_maiL[COUNTER_cranes].MOVE_BR_L)
        {
            temp_STRING = $"{temp_STRING_TIP_CRANES}{temp_STRING_START}{temp_STRING_MOVE_BR_L}";
            alarmsView_ST.Alarm(temp_STRING, COUNTER_cranes);
        }
        if (Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_BR_L & !Globals.CRANEs_maiL[COUNTER_cranes].MOVE_BR_L)
        {
            temp_STRING = $"{temp_STRING_TIP_CRANES}{temp_STRING_STOP}{temp_STRING_MOVE_BR_L}";
            alarmsView_ST.Alarm(temp_STRING, COUNTER_cranes);
        }
        Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_BR_L = Globals.CRANEs_maiL[COUNTER_cranes].MOVE_BR_L;
        // =========================================================================================================
        #endregion
        // ==========================================================================================================================================
        #region  МОСТ - ВПРАВО => MOVE_BR_R
        // =========================================================================================================
        if (!Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_BR_R & Globals.CRANEs_maiL[COUNTER_cranes].MOVE_BR_R)
        {
            temp_STRING = $"{temp_STRING_TIP_CRANES}{temp_STRING_START}{temp_STRING_MOVE_BR_R}";
            alarmsView_ST.Alarm(temp_STRING, COUNTER_cranes);
        }
        if (Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_BR_R & !Globals.CRANEs_maiL[COUNTER_cranes].MOVE_BR_R)
        {
            temp_STRING = $"{temp_STRING_TIP_CRANES}{temp_STRING_STOP}{temp_STRING_MOVE_BR_R}";
            alarmsView_ST.Alarm(temp_STRING, COUNTER_cranes);
        }
        Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_BR_R = Globals.CRANEs_maiL[COUNTER_cranes].MOVE_BR_R;
        // =========================================================================================================
        #endregion
        // ==========================================================================================================================================
        #region  ТЕЛЕЖКА 1 - ВПЕРЕД => MOVE_TR_1_F
        // =========================================================================================================
        if (!Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_TR_1_F & Globals.CRANEs_maiL[COUNTER_cranes].MOVE_TR_1_F)
        {
            temp_STRING = $"{temp_STRING_TIP_CRANES}{temp_STRING_START}{temp_STRING_MOVE_TR_1_F}";
            alarmsView_ST.Alarm(temp_STRING, COUNTER_cranes);
        }
        if (Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_TR_1_F & !Globals.CRANEs_maiL[COUNTER_cranes].MOVE_TR_1_F)
        {
            temp_STRING = $"{temp_STRING_TIP_CRANES}{temp_STRING_STOP}{temp_STRING_MOVE_TR_1_F}";
            alarmsView_ST.Alarm(temp_STRING, COUNTER_cranes);
        }
        Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_TR_1_F = Globals.CRANEs_maiL[COUNTER_cranes].MOVE_TR_1_F;
        // =========================================================================================================
        #endregion
        // ==========================================================================================================================================
        #region  ТЕЛЕЖКА 1 - НАЗАД => MOVE_TR_1_B
        // =========================================================================================================
        if (!Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_TR_1_B & Globals.CRANEs_maiL[COUNTER_cranes].MOVE_TR_1_B)
        {
            temp_STRING = $"{temp_STRING_TIP_CRANES}{temp_STRING_START}{temp_STRING_MOVE_TR_1_B}";
            alarmsView_ST.Alarm(temp_STRING, COUNTER_cranes);
        }
        if (Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_TR_1_B & !Globals.CRANEs_maiL[COUNTER_cranes].MOVE_TR_1_B)
        {
            temp_STRING = $"{temp_STRING_TIP_CRANES}{temp_STRING_STOP}{temp_STRING_MOVE_TR_1_B}";
            alarmsView_ST.Alarm(temp_STRING, COUNTER_cranes);
        }
        Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_TR_1_B = Globals.CRANEs_maiL[COUNTER_cranes].MOVE_TR_1_B;
        // =========================================================================================================
        #endregion       
        // ==========================================================================================================================================
        #region  ПОДЪЕМ 1-1 - ПОДЪЕМ => MOVE_H_1_1_UP
        // =========================================================================================================
        if (!Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_H_1_1_UP & Globals.CRANEs_maiL[COUNTER_cranes].MOVE_H_1_1_UP)
        {
            temp_STRING = $"{temp_STRING_TIP_CRANES}{temp_STRING_START}{temp_STRING_MOVE_H_1_1_UP}";
            alarmsView_ST.Alarm(temp_STRING, COUNTER_cranes);
        }
        if (Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_H_1_1_UP & !Globals.CRANEs_maiL[COUNTER_cranes].MOVE_H_1_1_UP)
        {
            temp_STRING = $"{temp_STRING_TIP_CRANES}{temp_STRING_STOP}{temp_STRING_MOVE_H_1_1_UP}";
            alarmsView_ST.Alarm(temp_STRING, COUNTER_cranes);
        }
        Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_H_1_1_UP = Globals.CRANEs_maiL[COUNTER_cranes].MOVE_H_1_1_UP;
        // =========================================================================================================
        #endregion
        // ==========================================================================================================================================
        #region  ПОДЪЕМ 1-1 - СРУСК  => MOVE_H_1_1_DOWN
        // =========================================================================================================
        if (!Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_H_1_1_DOWN & Globals.CRANEs_maiL[COUNTER_cranes].MOVE_H_1_1_DOWN)
        {
            temp_STRING = $"{temp_STRING_TIP_CRANES}{temp_STRING_START}{temp_STRING_MOVE_H_1_1_DOWN}";
            alarmsView_ST.Alarm(temp_STRING, COUNTER_cranes);
        }
        if (Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_H_1_1_DOWN & !Globals.CRANEs_maiL[COUNTER_cranes].MOVE_H_1_1_DOWN)
        {
            temp_STRING = $"{temp_STRING_TIP_CRANES}{temp_STRING_STOP}{temp_STRING_MOVE_H_1_1_DOWN}";
            alarmsView_ST.Alarm(temp_STRING, COUNTER_cranes);
        }
        Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_H_1_1_DOWN = Globals.CRANEs_maiL[COUNTER_cranes].MOVE_H_1_1_DOWN;
        // =========================================================================================================
        #endregion       
        // ==========================================================================================================================================
        #region  ПОДЪЕМ 1-2 - ПОДЪЕМ => MOVE_H_1_2_UP
        // =========================================================================================================
        if (!Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_H_1_2_UP & Globals.CRANEs_maiL[COUNTER_cranes].MOVE_H_1_2_UP)
        {
            temp_STRING = $"{temp_STRING_TIP_CRANES}{temp_STRING_START}{temp_STRING_MOVE_H_1_2_UP}";
            alarmsView_ST.Alarm(temp_STRING, COUNTER_cranes);
        }
        if (Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_H_1_2_UP & !Globals.CRANEs_maiL[COUNTER_cranes].MOVE_H_1_2_UP)
        {
            temp_STRING = $"{temp_STRING_TIP_CRANES}{temp_STRING_STOP}{temp_STRING_MOVE_H_1_2_UP}";
            alarmsView_ST.Alarm(temp_STRING, COUNTER_cranes);
        }
        Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_H_1_2_UP = Globals.CRANEs_maiL[COUNTER_cranes].MOVE_H_1_2_UP;
        // =========================================================================================================
        #endregion
        // ==========================================================================================================================================
        #region  ПОДЪЕМ 1-2 - СРУСК  => MOVE_H_1_2_DOWN
        // =========================================================================================================
        if (!Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_H_1_2_DOWN & Globals.CRANEs_maiL[COUNTER_cranes].MOVE_H_1_2_DOWN)
        {
            temp_STRING = $"{temp_STRING_TIP_CRANES}{temp_STRING_START}{temp_STRING_MOVE_H_1_2_DOWN}";
            alarmsView_ST.Alarm(temp_STRING, COUNTER_cranes);
        }
        if (Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_H_1_2_DOWN & !Globals.CRANEs_maiL[COUNTER_cranes].MOVE_H_1_2_DOWN)
        {
            temp_STRING = $"{temp_STRING_TIP_CRANES}{temp_STRING_STOP}{temp_STRING_MOVE_H_1_2_DOWN}";
            alarmsView_ST.Alarm(temp_STRING, COUNTER_cranes);
        }
        Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_H_1_2_DOWN = Globals.CRANEs_maiL[COUNTER_cranes].MOVE_H_1_2_DOWN;
        // =========================================================================================================
        #endregion
        // ==========================================================================================================================================
        #region  ТЕЛЕЖКА 2 - ВПЕРЕД => MOVE_TR_2_F
        // =========================================================================================================
        if (!Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_TR_2_F & Globals.CRANEs_maiL[COUNTER_cranes].MOVE_TR_2_F)
        {
            temp_STRING = $"{temp_STRING_TIP_CRANES}{temp_STRING_START}{temp_STRING_MOVE_TR_2_F}";
            alarmsView_ST.Alarm(temp_STRING, COUNTER_cranes);
        }
        if (Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_TR_2_F & !Globals.CRANEs_maiL[COUNTER_cranes].MOVE_TR_2_F)
        {
            temp_STRING = $"{temp_STRING_TIP_CRANES}{temp_STRING_STOP}{temp_STRING_MOVE_TR_2_F}";
            alarmsView_ST.Alarm(temp_STRING, COUNTER_cranes);
        }
        Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_TR_2_F = Globals.CRANEs_maiL[COUNTER_cranes].MOVE_TR_2_F;
        // =========================================================================================================
        #endregion
        // ==========================================================================================================================================
        #region  ТЕЛЕЖКА 2 - НАЗАД => MOVE_TR_2_B
        // =========================================================================================================
        if (!Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_TR_2_B & Globals.CRANEs_maiL[COUNTER_cranes].MOVE_TR_2_B)
        {
            temp_STRING = $"{temp_STRING_TIP_CRANES}{temp_STRING_START}{temp_STRING_MOVE_TR_2_B}";
            alarmsView_ST.Alarm(temp_STRING, COUNTER_cranes);
        }
        if (Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_TR_2_B & !Globals.CRANEs_maiL[COUNTER_cranes].MOVE_TR_2_B)
        {
            temp_STRING = $"{temp_STRING_TIP_CRANES}{temp_STRING_STOP}{temp_STRING_MOVE_TR_2_B}";
            alarmsView_ST.Alarm(temp_STRING, COUNTER_cranes);
        }
        Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_TR_2_B = Globals.CRANEs_maiL[COUNTER_cranes].MOVE_TR_2_B;
        // =========================================================================================================
        #endregion       
        // ==========================================================================================================================================
        #region  ПОДЪЕМ 2-1 - ПОДЪЕМ => MOVE_H_2_1_UP
        // =========================================================================================================
        if (!Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_H_2_1_UP & Globals.CRANEs_maiL[COUNTER_cranes].MOVE_H_2_1_UP)
        {
            temp_STRING = $"{temp_STRING_TIP_CRANES}{temp_STRING_START}{temp_STRING_MOVE_H_2_1_UP}";
            alarmsView_ST.Alarm(temp_STRING, COUNTER_cranes);
        }
        if (Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_H_2_1_UP & !Globals.CRANEs_maiL[COUNTER_cranes].MOVE_H_2_1_UP)
        {
            temp_STRING = $"{temp_STRING_TIP_CRANES}{temp_STRING_STOP}{temp_STRING_MOVE_H_2_1_UP}";
            alarmsView_ST.Alarm(temp_STRING, COUNTER_cranes);
        }
        Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_H_2_1_UP = Globals.CRANEs_maiL[COUNTER_cranes].MOVE_H_2_1_UP;
        // =========================================================================================================
        #endregion
        // ==========================================================================================================================================
        #region  ПОДЪЕМ 2-1 - СРУСК  => MOVE_H_2_1_DOWN
        // =========================================================================================================
        if (!Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_H_2_1_DOWN & Globals.CRANEs_maiL[COUNTER_cranes].MOVE_H_2_1_DOWN)
        {
            temp_STRING = $"{temp_STRING_TIP_CRANES}{temp_STRING_START}{temp_STRING_MOVE_H_2_1_DOWN}";
            alarmsView_ST.Alarm(temp_STRING, COUNTER_cranes);
        }
        if (Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_H_2_1_DOWN & !Globals.CRANEs_maiL[COUNTER_cranes].MOVE_H_2_1_DOWN)
        {
            temp_STRING = $"{temp_STRING_TIP_CRANES}{temp_STRING_STOP}{temp_STRING_MOVE_H_2_1_DOWN}";
            alarmsView_ST.Alarm(temp_STRING, COUNTER_cranes);
        }
        Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_H_2_1_DOWN = Globals.CRANEs_maiL[COUNTER_cranes].MOVE_H_2_1_DOWN;
        // =========================================================================================================
        #endregion       
        // ==========================================================================================================================================
        #region  ПОДЪЕМ 2-2 - ПОДЪЕМ => MOVE_H_2_2_UP
        // =========================================================================================================
        if (!Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_H_2_2_UP & Globals.CRANEs_maiL[COUNTER_cranes].MOVE_H_2_2_UP)
        {
            temp_STRING = $"{temp_STRING_TIP_CRANES}{temp_STRING_START}{temp_STRING_MOVE_H_2_2_UP}";
            alarmsView_ST.Alarm(temp_STRING, COUNTER_cranes);
        }
        if (Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_H_2_2_UP & !Globals.CRANEs_maiL[COUNTER_cranes].MOVE_H_2_2_UP)
        {
            temp_STRING = $"{temp_STRING_TIP_CRANES}{temp_STRING_STOP}{temp_STRING_MOVE_H_2_2_UP}";
            alarmsView_ST.Alarm(temp_STRING, COUNTER_cranes);
        }
        Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_H_2_2_UP = Globals.CRANEs_maiL[COUNTER_cranes].MOVE_H_2_2_UP;
        // =========================================================================================================
        #endregion
        // ==========================================================================================================================================
        #region  ПОДЪЕМ 2-2 - СРУСК  => MOVE_H_2_2_DOWN
        // =========================================================================================================
        if (!Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_H_2_2_DOWN & Globals.CRANEs_maiL[COUNTER_cranes].MOVE_H_2_2_DOWN)
        {
            temp_STRING = $"{temp_STRING_TIP_CRANES}{temp_STRING_START}{temp_STRING_MOVE_H_2_2_DOWN}";
            alarmsView_ST.Alarm(temp_STRING, COUNTER_cranes);
        }
        if (Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_H_2_2_DOWN & !Globals.CRANEs_maiL[COUNTER_cranes].MOVE_H_2_2_DOWN)
        {
            temp_STRING = $"{temp_STRING_TIP_CRANES}{temp_STRING_STOP}{temp_STRING_MOVE_H_2_2_DOWN}";
            alarmsView_ST.Alarm(temp_STRING, COUNTER_cranes);
        }
        Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_H_2_2_DOWN = Globals.CRANEs_maiL[COUNTER_cranes].MOVE_H_2_2_DOWN;
        // =========================================================================================================
        #endregion
        // ==========================================================================================================================================
        #endregion
        // ==========================================================================================================================================       
        // AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
        // ==========================================================================================================================================
        #region КРАН - 25Т
        // ==========================================================================================================================================
        COUNTER_cranes = 3;// 1 - 320Т / 2 - 120Т / 3 - 25Т -  ТАКЖЕ ОПРЕДЕЛЯЕТ ГРУППУ СООБЩЕНИЙ => alarmsView_ST.Alarm(temp_STRING, COUNTER_cranes);
        // ==========================================================================================================================================
        #region  // Какой кран
        switch (COUNTER_cranes)
        {
            case 1: // 
                temp_STRING_TIP_CRANES = "Кран 320Т: ";
                break;
            case 2: //
                temp_STRING_TIP_CRANES = "Кран 120Т: ";
                break;
            case 3: // 
                temp_STRING_TIP_CRANES = "Кран  25Т: ";
                break;
            default:
                temp_STRING_TIP_CRANES = "Программная ошибка ";
                break;
        }
        #endregion
        // ==========================================================================================================================================
        #region  МОСТ - ВЛЕВО => MOVE_BR_L
        // =========================================================================================================
        if (!Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_BR_L & Globals.CRANEs_maiL[COUNTER_cranes].MOVE_BR_L)
        {
            temp_STRING = $"{temp_STRING_TIP_CRANES}{temp_STRING_START}{temp_STRING_MOVE_BR_L}";
            alarmsView_ST.Alarm(temp_STRING, COUNTER_cranes);
        }
        if (Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_BR_L & !Globals.CRANEs_maiL[COUNTER_cranes].MOVE_BR_L)
        {
            temp_STRING = $"{temp_STRING_TIP_CRANES}{temp_STRING_STOP}{temp_STRING_MOVE_BR_L}";
            alarmsView_ST.Alarm(temp_STRING, COUNTER_cranes);
        }
        Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_BR_L = Globals.CRANEs_maiL[COUNTER_cranes].MOVE_BR_L;
        // =========================================================================================================
        #endregion
        // ==========================================================================================================================================
        #region  МОСТ - ВПРАВО => MOVE_BR_R
        // =========================================================================================================
        if (!Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_BR_R & Globals.CRANEs_maiL[COUNTER_cranes].MOVE_BR_R)
        {
            temp_STRING = $"{temp_STRING_TIP_CRANES}{temp_STRING_START}{temp_STRING_MOVE_BR_R}";
            alarmsView_ST.Alarm(temp_STRING, COUNTER_cranes);
        }
        if (Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_BR_R & !Globals.CRANEs_maiL[COUNTER_cranes].MOVE_BR_R)
        {
            temp_STRING = $"{temp_STRING_TIP_CRANES}{temp_STRING_STOP}{temp_STRING_MOVE_BR_R}";
            alarmsView_ST.Alarm(temp_STRING, COUNTER_cranes);
        }
        Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_BR_R = Globals.CRANEs_maiL[COUNTER_cranes].MOVE_BR_R;
        // =========================================================================================================
        #endregion
        // ==========================================================================================================================================
        #region  ТЕЛЕЖКА 1 - ВПЕРЕД => MOVE_TR_1_F
        // =========================================================================================================
        if (!Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_TR_1_F & Globals.CRANEs_maiL[COUNTER_cranes].MOVE_TR_1_F)
        {
            temp_STRING = $"{temp_STRING_TIP_CRANES}{temp_STRING_START}{temp_STRING_MOVE_TR_1_F}";
            alarmsView_ST.Alarm(temp_STRING, COUNTER_cranes);
        }
        if (Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_TR_1_F & !Globals.CRANEs_maiL[COUNTER_cranes].MOVE_TR_1_F)
        {
            temp_STRING = $"{temp_STRING_TIP_CRANES}{temp_STRING_STOP}{temp_STRING_MOVE_TR_1_F}";
            alarmsView_ST.Alarm(temp_STRING, COUNTER_cranes);
        }
        Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_TR_1_F = Globals.CRANEs_maiL[COUNTER_cranes].MOVE_TR_1_F;
        // =========================================================================================================
        #endregion
        // ==========================================================================================================================================
        #region  ТЕЛЕЖКА 1 - НАЗАД => MOVE_TR_1_B
        // =========================================================================================================
        if (!Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_TR_1_B & Globals.CRANEs_maiL[COUNTER_cranes].MOVE_TR_1_B)
        {
            temp_STRING = $"{temp_STRING_TIP_CRANES}{temp_STRING_START}{temp_STRING_MOVE_TR_1_B}";
            alarmsView_ST.Alarm(temp_STRING, COUNTER_cranes);
        }
        if (Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_TR_1_B & !Globals.CRANEs_maiL[COUNTER_cranes].MOVE_TR_1_B)
        {
            temp_STRING = $"{temp_STRING_TIP_CRANES}{temp_STRING_STOP}{temp_STRING_MOVE_TR_1_B}";
            alarmsView_ST.Alarm(temp_STRING, COUNTER_cranes);
        }
        Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_TR_1_B = Globals.CRANEs_maiL[COUNTER_cranes].MOVE_TR_1_B;
        // =========================================================================================================
        #endregion       
        // ==========================================================================================================================================
        #region  ПОДЪЕМ 1-1 - ПОДЪЕМ => MOVE_H_1_1_UP
        // =========================================================================================================
        if (!Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_H_1_1_UP & Globals.CRANEs_maiL[COUNTER_cranes].MOVE_H_1_1_UP)
        {
            temp_STRING = $"{temp_STRING_TIP_CRANES}{temp_STRING_START}{temp_STRING_MOVE_H_1_1_UP}";
            alarmsView_ST.Alarm(temp_STRING, COUNTER_cranes);
        }
        if (Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_H_1_1_UP & !Globals.CRANEs_maiL[COUNTER_cranes].MOVE_H_1_1_UP)
        {
            temp_STRING = $"{temp_STRING_TIP_CRANES}{temp_STRING_STOP}{temp_STRING_MOVE_H_1_1_UP}";
            alarmsView_ST.Alarm(temp_STRING, COUNTER_cranes);
        }
        Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_H_1_1_UP = Globals.CRANEs_maiL[COUNTER_cranes].MOVE_H_1_1_UP;
        // =========================================================================================================
        #endregion
        // ==========================================================================================================================================
        #region  ПОДЪЕМ 1-1 - СРУСК  => MOVE_H_1_1_DOWN
        // =========================================================================================================
        if (!Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_H_1_1_DOWN & Globals.CRANEs_maiL[COUNTER_cranes].MOVE_H_1_1_DOWN)
        {
            temp_STRING = $"{temp_STRING_TIP_CRANES}{temp_STRING_START}{temp_STRING_MOVE_H_1_1_DOWN}";
            alarmsView_ST.Alarm(temp_STRING, COUNTER_cranes);
        }
        if (Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_H_1_1_DOWN & !Globals.CRANEs_maiL[COUNTER_cranes].MOVE_H_1_1_DOWN)
        {
            temp_STRING = $"{temp_STRING_TIP_CRANES}{temp_STRING_STOP}{temp_STRING_MOVE_H_1_1_DOWN}";
            alarmsView_ST.Alarm(temp_STRING, COUNTER_cranes);
        }
        Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_H_1_1_DOWN = Globals.CRANEs_maiL[COUNTER_cranes].MOVE_H_1_1_DOWN;
        // =========================================================================================================
        #endregion       
        // ==========================================================================================================================================
        #region  ПОДЪЕМ 1-2 - ПОДЪЕМ => MOVE_H_1_2_UP
        // =========================================================================================================
        if (!Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_H_1_2_UP & Globals.CRANEs_maiL[COUNTER_cranes].MOVE_H_1_2_UP)
        {
            temp_STRING = $"{temp_STRING_TIP_CRANES}{temp_STRING_START}{temp_STRING_MOVE_H_1_2_UP}";
            alarmsView_ST.Alarm(temp_STRING, COUNTER_cranes);
        }
        if (Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_H_1_2_UP & !Globals.CRANEs_maiL[COUNTER_cranes].MOVE_H_1_2_UP)
        {
            temp_STRING = $"{temp_STRING_TIP_CRANES}{temp_STRING_STOP}{temp_STRING_MOVE_H_1_2_UP}";
            alarmsView_ST.Alarm(temp_STRING, COUNTER_cranes);
        }
        Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_H_1_2_UP = Globals.CRANEs_maiL[COUNTER_cranes].MOVE_H_1_2_UP;
        // =========================================================================================================
        #endregion
        // ==========================================================================================================================================
        #region  ПОДЪЕМ 1-2 - СРУСК  => MOVE_H_1_2_DOWN
        // =========================================================================================================
        if (!Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_H_1_2_DOWN & Globals.CRANEs_maiL[COUNTER_cranes].MOVE_H_1_2_DOWN)
        {
            temp_STRING = $"{temp_STRING_TIP_CRANES}{temp_STRING_START}{temp_STRING_MOVE_H_1_2_DOWN}";
            alarmsView_ST.Alarm(temp_STRING, COUNTER_cranes);
        }
        if (Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_H_1_2_DOWN & !Globals.CRANEs_maiL[COUNTER_cranes].MOVE_H_1_2_DOWN)
        {
            temp_STRING = $"{temp_STRING_TIP_CRANES}{temp_STRING_STOP}{temp_STRING_MOVE_H_1_2_DOWN}";
            alarmsView_ST.Alarm(temp_STRING, COUNTER_cranes);
        }
        Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_H_1_2_DOWN = Globals.CRANEs_maiL[COUNTER_cranes].MOVE_H_1_2_DOWN;
        // =========================================================================================================
        #endregion
        // ==========================================================================================================================================
        #region  ТЕЛЕЖКА 2 - ВПЕРЕД => MOVE_TR_2_F
        // =========================================================================================================
        if (!Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_TR_2_F & Globals.CRANEs_maiL[COUNTER_cranes].MOVE_TR_2_F)
        {
            temp_STRING = $"{temp_STRING_TIP_CRANES}{temp_STRING_START}{temp_STRING_MOVE_TR_2_F}";
            alarmsView_ST.Alarm(temp_STRING, COUNTER_cranes);
        }
        if (Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_TR_2_F & !Globals.CRANEs_maiL[COUNTER_cranes].MOVE_TR_2_F)
        {
            temp_STRING = $"{temp_STRING_TIP_CRANES}{temp_STRING_STOP}{temp_STRING_MOVE_TR_2_F}";
            alarmsView_ST.Alarm(temp_STRING, COUNTER_cranes);
        }
        Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_TR_2_F = Globals.CRANEs_maiL[COUNTER_cranes].MOVE_TR_2_F;
        // =========================================================================================================
        #endregion
        // ==========================================================================================================================================
        #region  ТЕЛЕЖКА 2 - НАЗАД => MOVE_TR_2_B
        // =========================================================================================================
        if (!Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_TR_2_B & Globals.CRANEs_maiL[COUNTER_cranes].MOVE_TR_2_B)
        {
            temp_STRING = $"{temp_STRING_TIP_CRANES}{temp_STRING_START}{temp_STRING_MOVE_TR_2_B}";
            alarmsView_ST.Alarm(temp_STRING, COUNTER_cranes);
        }
        if (Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_TR_2_B & !Globals.CRANEs_maiL[COUNTER_cranes].MOVE_TR_2_B)
        {
            temp_STRING = $"{temp_STRING_TIP_CRANES}{temp_STRING_STOP}{temp_STRING_MOVE_TR_2_B}";
            alarmsView_ST.Alarm(temp_STRING, COUNTER_cranes);
        }
        Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_TR_2_B = Globals.CRANEs_maiL[COUNTER_cranes].MOVE_TR_2_B;
        // =========================================================================================================
        #endregion       
        // ==========================================================================================================================================
        #region  ПОДЪЕМ 2-1 - ПОДЪЕМ => MOVE_H_2_1_UP
        // =========================================================================================================
        if (!Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_H_2_1_UP & Globals.CRANEs_maiL[COUNTER_cranes].MOVE_H_2_1_UP)
        {
            temp_STRING = $"{temp_STRING_TIP_CRANES}{temp_STRING_START}{temp_STRING_MOVE_H_2_1_UP}";
            alarmsView_ST.Alarm(temp_STRING, COUNTER_cranes);
        }
        if (Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_H_2_1_UP & !Globals.CRANEs_maiL[COUNTER_cranes].MOVE_H_2_1_UP)
        {
            temp_STRING = $"{temp_STRING_TIP_CRANES}{temp_STRING_STOP}{temp_STRING_MOVE_H_2_1_UP}";
            alarmsView_ST.Alarm(temp_STRING, COUNTER_cranes);
        }
        Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_H_2_1_UP = Globals.CRANEs_maiL[COUNTER_cranes].MOVE_H_2_1_UP;
        // =========================================================================================================
        #endregion
        // ==========================================================================================================================================
        #region  ПОДЪЕМ 2-1 - СРУСК  => MOVE_H_2_1_DOWN
        // =========================================================================================================
        if (!Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_H_2_1_DOWN & Globals.CRANEs_maiL[COUNTER_cranes].MOVE_H_2_1_DOWN)
        {
            temp_STRING = $"{temp_STRING_TIP_CRANES}{temp_STRING_START}{temp_STRING_MOVE_H_2_1_DOWN}";
            alarmsView_ST.Alarm(temp_STRING, COUNTER_cranes);
        }
        if (Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_H_2_1_DOWN & !Globals.CRANEs_maiL[COUNTER_cranes].MOVE_H_2_1_DOWN)
        {
            temp_STRING = $"{temp_STRING_TIP_CRANES}{temp_STRING_STOP}{temp_STRING_MOVE_H_2_1_DOWN}";
            alarmsView_ST.Alarm(temp_STRING, COUNTER_cranes);
        }
        Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_H_2_1_DOWN = Globals.CRANEs_maiL[COUNTER_cranes].MOVE_H_2_1_DOWN;
        // =========================================================================================================
        #endregion       
        // ==========================================================================================================================================
        #region  ПОДЪЕМ 2-2 - ПОДЪЕМ => MOVE_H_2_2_UP
        // =========================================================================================================
        if (!Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_H_2_2_UP & Globals.CRANEs_maiL[COUNTER_cranes].MOVE_H_2_2_UP)
        {
            temp_STRING = $"{temp_STRING_TIP_CRANES}{temp_STRING_START}{temp_STRING_MOVE_H_2_2_UP}";
            alarmsView_ST.Alarm(temp_STRING, COUNTER_cranes);
        }
        if (Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_H_2_2_UP & !Globals.CRANEs_maiL[COUNTER_cranes].MOVE_H_2_2_UP)
        {
            temp_STRING = $"{temp_STRING_TIP_CRANES}{temp_STRING_STOP}{temp_STRING_MOVE_H_2_2_UP}";
            alarmsView_ST.Alarm(temp_STRING, COUNTER_cranes);
        }
        Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_H_2_2_UP = Globals.CRANEs_maiL[COUNTER_cranes].MOVE_H_2_2_UP;
        // =========================================================================================================
        #endregion
        // ==========================================================================================================================================
        #region  ПОДЪЕМ 2-2 - СРУСК  => MOVE_H_2_2_DOWN
        // =========================================================================================================
        if (!Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_H_2_2_DOWN & Globals.CRANEs_maiL[COUNTER_cranes].MOVE_H_2_2_DOWN)
        {
            temp_STRING = $"{temp_STRING_TIP_CRANES}{temp_STRING_START}{temp_STRING_MOVE_H_2_2_DOWN}";
            alarmsView_ST.Alarm(temp_STRING, COUNTER_cranes);
        }
        if (Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_H_2_2_DOWN & !Globals.CRANEs_maiL[COUNTER_cranes].MOVE_H_2_2_DOWN)
        {
            temp_STRING = $"{temp_STRING_TIP_CRANES}{temp_STRING_STOP}{temp_STRING_MOVE_H_2_2_DOWN}";
            alarmsView_ST.Alarm(temp_STRING, COUNTER_cranes);
        }
        Globals.MEM_CRANEs_maiL[COUNTER_cranes].MOVE_H_2_2_DOWN = Globals.CRANEs_maiL[COUNTER_cranes].MOVE_H_2_2_DOWN;
        // =========================================================================================================
        #endregion
        // ==========================================================================================================================================
        #endregion
        // ==========================================================================================================================================
        // AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
        // ******************************************************************************************************************************************
        #endregion 
        // ******************************************************************************************************************************************

        #region XXXXXXXXXXXXXXX

        #endregion
        


    }
}
