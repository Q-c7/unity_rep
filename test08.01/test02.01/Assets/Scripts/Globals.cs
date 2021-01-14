using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Globals
{
    public static bool showAlarmWindow;
    public static bool showStatusWindow;
    public static bool showConnectionStatusWindow;
    public static bool showFpsWindow;
    public static float scaleX = 0.0001f;
    public static float scaleY = 0.0001f;
    public static float scaleZ = 0.0001f;
    public static float defaultWeightLength;
    public static float defaultWeightWidth;
    public static float defaultWeightHeight;
    public static float defaultWeightMass;
    public static float warningDistance;
    public static float alarmDistance;
    public static float minWeight = 100.0f;
    public static float crane320_x;
    public static float crane320_y;
    public static float crane320_z;
    public static float crane120_x;
    public static float crane120_y;
    public static float crane120_z;
    public static float crane25_x;
    public static float crane25_y;
    public static float crane25_z;
    public static float zZeroFix320 = 48000.0f;
    public static float zZeroFix = 45000.0f;
    public static float hoistsMaxHeight = 29000.0f;
    public static float hoistsMaxHeight320 = 34830.0f;











    // Flag первого запуска проекта
    public static bool FLAG_F_START = true;
    public static bool F_STARTING = false;

    public static bool FLAG_F_START_1MAIL = false;
    public static bool FLAG_F_START_2MAIL = false;
    // МАССИВ ДАННЫХ ДЛЯ РЕГИСТРАЦИЙ ИЗМЕНЕНИЙ В SQL
    // ГАБАРИТЫ
    public static float[] MEM_Obj_ALL_X = new float[812];
    public static float[] MEM_Obj_ALL_Y = new float[812];
    public static float[] MEM_Obj_ALL_Z = new float[812];
    // БАЗОВАЯ ТОЧКА
    public static float[] MEM_Obj_ALL_ST_P_X = new float[812];
    public static float[] MEM_Obj_ALL_ST_P_Y = new float[812];
    public static float[] MEM_Obj_ALL_ST_P_Z = new float[812];

    // Для траверс - номер в массиве
    public static int[] MEM_traversa_ID = new int[70];
    public static bool[] MEM_traversa_TWO_HOOK = new bool[70];
    public static string[] MEM_traversa_name = new string[70];
    public static int[] traversa_ID = new int[70];
    public static bool[] traversa_TWO_HOOK = new bool[70];
    public static string[] traversa_name = new string[70];

    public static bool[,,] MEM1_DOWN_WARN = new bool [5, 25, 812];
    public static bool[,,] MEM1_UP_WARN = new bool[5, 25, 812];
    public static bool[,,] MEM1_L_WARN = new bool[5, 25, 812];
    public static bool[,,] MEM1_R_WARN = new bool[5, 25, 812];
    public static bool[,,] MEM1_B_WARN = new bool[5, 25, 812];
    public static bool[,,] MEM1_F_WARN = new bool[5, 25, 812];

    public static bool[,,] MEM1_DOWN_ERR = new bool[5, 25, 812];
    public static bool[,,] MEM1_UP_ERR = new bool[5, 25, 812];
    public static bool[,,] MEM1_L_ERR = new bool[5, 25, 812];
    public static bool[,,] MEM1_R_ERR = new bool[5, 25, 812];
    public static bool[,,] MEM1_B_ERR = new bool[5, 25, 812];
    public static bool[,,] MEM1_F_ERR = new bool[5, 25, 812];

    // =====================================================================================
    // память состояния СФОРМИРОВАННЫх ОГРАНИЧЕНИй - ПРЕДУПРЕЖДЕНИЯ - "ЖЕЛТЫЙ"
    // ПОДЪЕМЫ
    public static bool[] MEM_B_H_1_1_UP_WARN = new bool[5];
    public static bool[] MEM_B_H_1_1_DOWN_WARN = new bool[5];
    public static bool[] MEM_B_H_1_2_UP_WARN = new bool[5];
    public static bool[] MEM_B_H_1_2_DOWN_WARN = new bool[5];
    public static bool[] MEM_B_H_2_1_UP_WARN = new bool[5];
    public static bool[] MEM_B_H_2_1_DOWN_WARN = new bool[5];
    public static bool[] MEM_B_H_2_2_UP_WARN = new bool[5];
    public static bool[] MEM_B_H_2_2_DOWN_WARN = new bool[5];
    // ТЕЛЕЖКИ
    public static bool[] MEM_B_TR_1_F_WARN = new bool[5];
    public static bool[] MEM_B_TR_1_B_WARN = new bool[5];
    public static bool[] MEM_B_TR_2_F_WARN = new bool[5];
    public static bool[] MEM_B_TR_2_B_WARN = new bool[5];
    // МОСТ
    public static bool[] MEM_B_BR_R_WARN = new bool[5];
    public static bool[] MEM_B_BR_L_WARN = new bool[5];

    // СФОРМИРОВАННЫЕ ОГРАНИЧЕНИЯ - АВАРИИ - "КРАСНЫЙ"
    // ПОДЪЕМЫ
    public static bool[] MEM_B_H_1_1_UP_ERR = new bool[5];
    public static bool[] MEM_B_H_1_1_DOWN_ERR = new bool[5];
    public static bool[] MEM_B_H_1_2_UP_ERR = new bool[5];
    public static bool[] MEM_B_H_1_2_DOWN_ERR = new bool[5];
    public static bool[] MEM_B_H_2_1_UP_ERR = new bool[5];
    public static bool[] MEM_B_H_2_1_DOWN_ERR = new bool[5];
    public static bool[] MEM_B_H_2_2_UP_ERR = new bool[5];
    public static bool[] MEM_B_H_2_2_DOWN_ERR = new bool[5];
    // ТЕЛЕЖКИ
    public static bool[] MEM_B_TR_1_F_ERR = new bool[5];
    public static bool[] MEM_B_TR_1_B_ERR = new bool[5];
    public static bool[] MEM_B_TR_2_F_ERR = new bool[5];
    public static bool[] MEM_B_TR_2_B_ERR = new bool[5];
    // МОСТ
    public static bool[] MEM_B_BR_R_ERR = new bool[5];
    public static bool[] MEM_B_BR_L_ERR = new bool[5];
    // =====================================================================================

    // Группа сообщений состояние крана
    // =====================================================================================
    #region КРАН 320Т/120T/25T
    public struct status_craneS
    {
        public bool MOVE_BR_R;
        public bool MOVE_BR_L;

        public bool MOVE_TR_1_F;
        public bool MOVE_TR_1_B;

        public bool MOVE_H_1_1_UP;
        public bool MOVE_H_1_1_DOWN;

        public bool MOVE_H_1_2_UP;
        public bool MOVE_H_1_2_DOWN;

        public bool MOVE_TR_2_F;
        public bool MOVE_TR_2_B;

        public bool MOVE_H_2_1_UP;
        public bool MOVE_H_2_1_DOWN;

        public bool MOVE_H_2_2_UP;
        public bool MOVE_H_2_2_DOWN;
    }
    // МАССИВ АКТУАЛЬНЫХ ДАННЫХ
    public static status_craneS[] CRANEs_maiL = new status_craneS[5];
    // МАССИВ ПАМЯТИ ДАННЫХ - ДЛЯ ОПРЕДЕЛЕНИЯ ФРОНТА ПОЯВЛЕНЯ СИГНАЛА
    public static status_craneS[] MEM_CRANEs_maiL = new status_craneS[5];
    #endregion












    #region КРАН 120Т

    #endregion
    #region КРАН 25Т

    #endregion







    // =====================================================================================








}
