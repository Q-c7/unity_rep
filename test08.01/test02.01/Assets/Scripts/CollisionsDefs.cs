using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CRANEs
{
    // 2020_08_24
    // ФОРМИРОВАНИЕ ФЛАГА ПРЕДУПРЕЖДЕНИЯ И АВАРИИ
    public bool[,] KRASH_UP_WARN = new bool[ 25, 812];
    public bool[,] KRASH_DOWN_WARN = new bool[ 25, 812];
    public bool[,] KRASH_L_WARN = new bool[ 25, 812];
    public bool[,] KRASH_R_WARN = new bool[ 25, 812];
    public bool[,] KRASH_F_WARN = new bool[ 25, 812];
    public bool[,] KRASH_B_WARN = new bool[ 25, 812];

    public bool[,] KRASH_UP_ERR = new bool[ 25, 812];
    public bool[,] KRASH_DOWN_ERR = new bool[ 25, 812];
    public bool[,] KRASH_L_ERR = new bool[ 25, 812];
    public bool[,] KRASH_R_ERR = new bool[ 25, 812];
    public bool[,] KRASH_F_ERR = new bool[ 25, 812];
    public bool[,] KRASH_B_ERR = new bool[ 25, 812];

    // ФОРМИРОВАНИЕ ПАМЯТИ ФЛАГА ПРЕДУПРЕЖДЕНИЯ И АВАРИИ
    public bool[,] MEM_KRASH_UP_WARN = new bool[25, 812];
    public bool[,] MEM_KRASH_DOWN_WARN = new bool[25, 812];
    public bool[,] MEM_KRASH_L_WARN = new bool[25, 812];
    public bool[,] MEM_KRASH_R_WARN = new bool[25, 812];
    public bool[,] MEM_KRASH_F_WARN = new bool[25, 812];
    public bool[,] MEM_KRASH_B_WARN = new bool[25, 812];

    public bool[,] MEM_KRASH_UP_ERR = new bool[25, 812];
    public bool[,] MEM_KRASH_DOWN_ERR = new bool[25, 812];
    public bool[,] MEM_KRASH_L_ERR = new bool[25, 812];
    public bool[,] MEM_KRASH_R_ERR = new bool[25, 812];
    public bool[,] MEM_KRASH_F_ERR = new bool[25, 812];
    public bool[,] MEM_KRASH_B_ERR = new bool[25, 812];

    // ФОРМИРОВАНИЕ ПАМЯТИ ДЛЯ ПРОБЫ ФЛАГА ПРЕДУПРЕЖДЕНИЯ И АВАРИИ
    public bool[,] MEM1_KRASH_UP_WARN = new bool[25, 812];
    public bool[,] MEM1_KRASH_DOWN_WARN = new bool[25, 812];
    public bool[,] MEM1_KRASH_L_WARN = new bool[25, 812];
    public bool[,] MEM1_KRASH_R_WARN = new bool[25, 812];
    public bool[,] MEM1_KRASH_F_WARN = new bool[25, 812];
    public bool[,] MEM1_KRASH_B_WARN = new bool[25, 812];

    public bool[,] MEM1_KRASH_UP_ERR = new bool[25, 812];
    public bool[,] MEM1_KRASH_DOWN_ERR = new bool[25, 812];
    public bool[,] MEM1_KRASH_L_ERR = new bool[25, 812];
    public bool[,] MEM1_KRASH_R_ERR = new bool[25, 812];
    public bool[,] MEM1_KRASH_F_ERR = new bool[25, 812];
    public bool[,] MEM1_KRASH_B_ERR = new bool[25, 812];

    // АКТУАЛЬНЫЕ КООРДИНАТЫ КРАНА ОТ ПЛК
    public float BRIDGE_X = 0;
    public float trolley_1_Y = 0;
    public float trolley_2_Y = 0;
    public float HOIST_1_1_Z = 0;
    public float HOIST_1_2_Z = 0;
    public float HOIST_2_1_Z = 0;
    public float HOIST_2_2_Z = 0;

    // =====================================================================================
    // СФОРМИРОВАННЫЕ ОГРАНИЧЕНИЯ - ПРЕДУПРЕЖДЕНИЯ - "ЖЕЛТЫЙ"
    // ПОДЪЕМЫ
    public bool BLOCK_H_1_1_UP_WARN = false;
    public bool BLOCK_H_1_1_DOWN_WARN = false;
    public bool BLOCK_H_1_2_UP_WARN = false;
    public bool BLOCK_H_1_2_DOWN_WARN = false;
    public bool BLOCK_H_2_1_UP_WARN = false;
    public bool BLOCK_H_2_1_DOWN_WARN = false;
    public bool BLOCK_H_2_2_UP_WARN = false;
    public bool BLOCK_H_2_2_DOWN_WARN = false;
    // ТЕЛЕЖКИ
    public bool BLOCK_TR_1_F_WARN = false;
    public bool BLOCK_TR_1_B_WARN = false;
    public bool BLOCK_TR_2_F_WARN = false;
    public bool BLOCK_TR_2_B_WARN = false;
    // МОСТ
    public bool BLOCK_BR_R_WARN = false;
    public bool BLOCK_BR_L_WARN = false;

    // СФОРМИРОВАННЫЕ ОГРАНИЧЕНИЯ - АВАРИИ - "КРАСНЫЙ"
    // ПОДЪЕМЫ
    public bool BLOCK_H_1_1_UP_ERR = false;
    public bool BLOCK_H_1_1_DOWN_ERR = false;
    public bool BLOCK_H_1_2_UP_ERR = false;
    public bool BLOCK_H_1_2_DOWN_ERR = false;
    public bool BLOCK_H_2_1_UP_ERR = false;
    public bool BLOCK_H_2_1_DOWN_ERR = false;
    public bool BLOCK_H_2_2_UP_ERR = false;
    public bool BLOCK_H_2_2_DOWN_ERR = false;
    // ТЕЛЕЖКИ
    public bool BLOCK_TR_1_F_ERR = false;
    public bool BLOCK_TR_1_B_ERR = false;
    public bool BLOCK_TR_2_F_ERR = false;
    public bool BLOCK_TR_2_B_ERR = false;
    // МОСТ
    public bool BLOCK_BR_R_ERR = false;
    public bool BLOCK_BR_L_ERR = false;
    // =====================================================================================





    // ПОСТОЯННЫЕ ВЕЛИЧИНЫ
    // ГАБАРИТЫ МОСТА
    public float static_BR_X = 0;
    public float static_BR_Y = 0;
    public float static_BR_Z = 0;

    public float STP_static_BR_X = 0;
    public float STP_static_BR_Y = 0;
    public float STP_static_BR_Z = 0;

    // ГАБАРИТЫ ТЕЛЕЖКИ 1 - ГЛАВНАЯ 
    public float static_TR_1_X = 0;
    public float static_TR_1_Y = 0;
    public float static_TR_1_Z = 0;

    public float STP_static_TR_1_X = 0;
    public float STP_static_TR_1_Y = 0;
    public float STP_static_TR_1_Z = 0;

    // ГАБАРИТЫ ТЕЛЕЖКИ 2 - ВСПОМОГАТЕЛЬНАЯ 
    public float static_TR_2_X = 0;
    public float static_TR_2_Y = 0;
    public float static_TR_2_Z = 0;

    public float STP_static_TR_2_X = 0;
    public float STP_static_TR_2_Y = 0;
    public float STP_static_TR_2_Z = 0;

    // ГАБАРИТЫ ПОДЪЕМ 1-1 - ГЛАВНЫЙ 
    public float static_H_1_1_X = 0;
    public float static_H_1_1_Y = 0;
    public float static_H_1_1_Z = 0;

    public float STP_static_H_1_1_X = 0;
    public float STP_static_H_1_1_Y = 0;
    public float STP_static_H_1_1_Z = 0;
    // ТРАВЕРСА
    public float static_H11_TR_X = 0;
    public float static_H11_TR_Y = 0;
    public float static_H11_TR_Z = 0;

    public bool static_H11_TR_twohook_y_n = false;
    public bool H11_TR_y_n = false;

    public float STP_static_H11_TR_X = 0;
    public float STP_static_H11_TR_Y = 0;
    public float STP_static_H11_TR_Z = 0;

    // ГАБАРИТЫ ПОДЪЕМ 1-1 - ГРУЗ 
    public float static_H_1_1_LOAD_X = 0;
    public float static_H_1_1_LOAD_Y = 0;
    public float static_H_1_1_LOAD_Z = 0;

    public float STP_static_H_1_1_LOAD_X = 0;
    public float STP_static_H_1_1_LOAD_Y = 0;
    public float STP_static_H_1_1_LOAD_Z = 0;


    // ГАБАРИТЫ ПОДЪЕМ 1-2 - ГЛАВНЫЙ 
    public float static_H_1_2_X = 0;
    public float static_H_1_2_Y = 0;
    public float static_H_1_2_Z = 0;

    public float STP_static_H_1_2_X = 0;
    public float STP_static_H_1_2_Y = 0;
    public float STP_static_H_1_2_Z = 0;

    // ТРАВЕРСА
    public float static_H12_TR_X = 0;
    public float static_H12_TR_Y = 0;
    public float static_H12_TR_Z = 0;

    public bool static_H12_TR_twohook_y_n = false;
    public bool H12_TR_y_n = false;

    public float STP_static_H12_TR_X = 0;
    public float STP_static_H12_TR_Y = 0;
    public float STP_static_H12_TR_Z = 0;

    // ГАБАРИТЫ ПОДЪЕМ 1-2 - ГРУЗ 
    public float static_H_1_2_LOAD_X = 0;
    public float static_H_1_2_LOAD_Y = 0;
    public float static_H_1_2_LOAD_Z = 0;

    public float STP_static_H_1_2_LOAD_X = 0;
    public float STP_static_H_1_2_LOAD_Y = 0;
    public float STP_static_H_1_2_LOAD_Z = 0;


    // ГАБАРИТЫ ПОДЪЕМ 2-1 - ВСПОМОГАТЕЛЬНЫЙ 
    public float static_H_2_1_X = 0;
    public float static_H_2_1_Y = 0;
    public float static_H_2_1_Z = 0;

    public float STP_static_H_2_1_X = 0;
    public float STP_static_H_2_1_Y = 0;
    public float STP_static_H_2_1_Z = 0;

    // ТРАВЕРСА
    public float static_H21_TR_X = 0;
    public float static_H21_TR_Y = 0;
    public float static_H21_TR_Z = 0;

    public bool static_H21_TR_twohook_y_n = false;
    public bool H21_TR_y_n = false;

    public float STP_static_H21_TR_X = 0;
    public float STP_static_H21_TR_Y = 0;
    public float STP_static_H21_TR_Z = 0;

    // ГАБАРИТЫ ПОДЪЕМ 2-1 - ГРУЗ 
    public float static_H_2_1_LOAD_X = 0;
    public float static_H_2_1_LOAD_Y = 0;
    public float static_H_2_1_LOAD_Z = 0;

    public float STP_static_H_2_1_LOAD_X = 0;
    public float STP_static_H_2_1_LOAD_Y = 0;
    public float STP_static_H_2_1_LOAD_Z = 0;

    // ГАБАРИТЫ ПОДЪЕМ 2-2 - ВСПОМОГАТЕЛЬНЫЙ 
    public float static_H_2_2_X = 0;
    public float static_H_2_2_Y = 0;
    public float static_H_2_2_Z = 0;

    public float STP_static_H_2_2_X = 0;
    public float STP_static_H_2_2_Y = 0;
    public float STP_static_H_2_2_Z = 0;

    // ТРАВЕРСА
    public float static_H22_TR_X = 0;
    public float static_H22_TR_Y = 0;
    public float static_H22_TR_Z = 0;

    public bool static_H22_TR_twohook_y_n = false;
    public bool H22_TR_y_n = false;

    public float STP_static_H22_TR_X = 0;
    public float STP_static_H22_TR_Y = 0;
    public float STP_static_H22_TR_Z = 0;

    // ГАБАРИТЫ ПОДЪЕМ 2-2 - ГРУЗ 
    public float static_H_2_2_LOAD_X = 0;
    public float static_H_2_2_LOAD_Y = 0;
    public float static_H_2_2_LOAD_Z = 0;

    public float STP_static_H_2_2_LOAD_X = 0;
    public float STP_static_H_2_2_LOAD_Y = 0;
    public float STP_static_H_2_2_LOAD_Z = 0;


    // РЕЖИМ РАБОТЫ ПОДЪЕМОВ

    public int regim_w_hoist = 0;


    // 2020_08_08
    // ФОРМИРУЕТСЯ НА ОСНОВЕ АНАЛИЗА ДАННЫХ ГАБАРИТОВ ОБЪЕКТА, ЕСЛИ ОДИН ИЗ ГАБАРИТОВ МЕНЬШЕ ИЛИ РАВЕН НУЛЮ
    // ДИНАМИЧЕСКИЙ ОБЪЕКТ НЕ ПРИНМАЕТСЯ ДЛЯ АНАЛИЗА
    public bool[] Obj_Y_N = new bool[25];



    // разрешение для обработки
    public bool[,] KRASH_cntrl_yn = new bool[25, 812];
    // СТАТУС
    public bool CRANEs_WARN = false;
    public bool CRANEs_ERR = false;
    //
    public bool[] KRASH_WARN = new bool[25];
    public bool[] KRASH_ERR = new bool[25];
    //
    public bool[] UP_KRASH_WARN = new bool[25];
    public bool[] DOWN_KRASH_WARN = new bool[25];
    public bool[] F_KRASH_WARN = new bool[25];
    public bool[] B_KRASH_WARN = new bool[25];
    public bool[] R_KRASH_WARN = new bool[25];
    public bool[] L_KRASH_WARN = new bool[25];
    //
    public bool[] UP_KRASH_ERR = new bool[25];
    public bool[] DOWN_KRASH_ERR = new bool[25];
    public bool[] F_KRASH_ERR = new bool[25];
    public bool[] B_KRASH_ERR = new bool[25];
    public bool[] R_KRASH_ERR = new bool[25];
    public bool[] L_KRASH_ERR = new bool[25];
    //ГАБАРИТЫ
    public float[] Obj_x = new float[25];
    public float[] Obj_y = new float[25];
    public float[] Obj_z = new float[25];
    // БАЗОВАЯ ТОЧКА - "STP"
    public float[] stp_x = new float[25];
    public float[] stp_y = new float[25];
    public float[] stp_z = new float[25];
    // ДЕЛЬТА ПРЕДУПРЕЖДЕНИЯ
    public float[] D_UP_WARN = new float[25];
    public float[] D_DOWN_WARN = new float[25];
    public float[] D_R_WARN = new float[25];
    public float[] D_L_WARN = new float[25];
    public float[] D_F_WARN = new float[25];
    public float[] D_B_WARN = new float[25];
    // ДЕЛЬТА АВАРИИ
    public float[] D_UP_ERR = new float[25];
    public float[] D_DOWN_ERR = new float[25];
    public float[] D_R_ERR = new float[25];
    public float[] D_L_ERR = new float[25];
    public float[] D_F_ERR = new float[25];
    public float[] D_B_ERR = new float[25];
    // ОБЛАСТИ ПРЕДУПРЕЖДЕНИЙ
    // ГАБАРИТЫ - ВВЕРХ
    public float[] UP_x_WARN = new float[25];
    public float[] UP_y_WARN = new float[25];
    public float[] UP_z_WARN = new float[25];
    // БАЗОВАЯ ТОЧКА - "STP" - ВВЕРХ
    public float[] UP_stp_x_WARN = new float[25];
    public float[] UP_stp_y_WARN = new float[25];
    public float[] UP_stp_z_WARN = new float[25];
    // ГАБАРИТЫ - ВНИЗ
    public float[] DOWN_x_WARN = new float[25];
    public float[] DOWN_y_WARN = new float[25];
    public float[] DOWN_z_WARN = new float[25];
    // БАЗОВАЯ ТОЧКА - "STP" - ВНИЗ
    public float[] DOWN_stp_x_WARN = new float[25];
    public float[] DOWN_stp_y_WARN = new float[25];
    public float[] DOWN_stp_z_WARN = new float[25];
    // ГАБАРИТЫ - ВПЕРЕД
    public float[] F_x_WARN = new float[25];
    public float[] F_y_WARN = new float[25];
    public float[] F_z_WARN = new float[25];
    // БАЗОВАЯ ТОЧКА - "STP" - ВПЕРЕД
    public float[] F_stp_x_WARN = new float[25];
    public float[] F_stp_y_WARN = new float[25];
    public float[] F_stp_z_WARN = new float[25];
    // ГАБАРИТЫ - НАЗАД
    public float[] B_x_WARN = new float[25];
    public float[] B_y_WARN = new float[25];
    public float[] B_z_WARN = new float[25];
    // БАЗОВАЯ ТОЧКА - "STP" - НАЗАД
    public float[] B_stp_x_WARN = new float[25];
    public float[] B_stp_y_WARN = new float[25];
    public float[] B_stp_z_WARN = new float[25];
    // ГАБАРИТЫ - ВПРАВО
    public float[] R_x_WARN = new float[25];
    public float[] R_y_WARN = new float[25];
    public float[] R_z_WARN = new float[25];
    // БАЗОВАЯ ТОЧКА - "STP" - ВПРАВО
    public float[] R_stp_x_WARN = new float[25];
    public float[] R_stp_y_WARN = new float[25];
    public float[] R_stp_z_WARN = new float[25];
    // ГАБАРИТЫ - ВЛЕВО
    public float[] L_x_WARN = new float[25];
    public float[] L_y_WARN = new float[25];
    public float[] L_z_WARN = new float[25];
    // БАЗОВАЯ ТОЧКА - "STP" - ВЛЕВО
    public float[] L_stp_x_WARN = new float[25];
    public float[] L_stp_y_WARN = new float[25];
    public float[] L_stp_z_WARN = new float[25];
    // ОБЛАСТИ АВАРИЙ
    // ГАБАРИТЫ - ВВЕРХ
    public float[] UP_x_ERR = new float[25];
    public float[] UP_y_ERR = new float[25];
    public float[] UP_z_ERR = new float[25];
    // БАЗОВАЯ ТОЧКА - "STP" - ВВЕРХ
    public float[] UP_stp_x_ERR = new float[25];
    public float[] UP_stp_y_ERR = new float[25];
    public float[] UP_stp_z_ERR = new float[25];
    // ГАБАРИТЫ - ВНИЗ
    public float[] DOWN_x_ERR = new float[25];
    public float[] DOWN_y_ERR = new float[25];
    public float[] DOWN_z_ERR = new float[25];
    // БАЗОВАЯ ТОЧКА - "STP" - ВНИЗ
    public float[] DOWN_stp_x_ERR = new float[25];
    public float[] DOWN_stp_y_ERR = new float[25];
    public float[] DOWN_stp_z_ERR = new float[25];
    // ГАБАРИТЫ - ВПЕРЕД
    public float[] F_x_ERR = new float[25];
    public float[] F_y_ERR = new float[25];
    public float[] F_z_ERR = new float[25];
    // БАЗОВАЯ ТОЧКА - "STP" - ВПЕРЕД
    public float[] F_stp_x_ERR = new float[25];
    public float[] F_stp_y_ERR = new float[25];
    public float[] F_stp_z_ERR = new float[25];
    // ГАБАРИТЫ - НАЗАД
    public float[] B_x_ERR = new float[25];
    public float[] B_y_ERR = new float[25];
    public float[] B_z_ERR = new float[25];
    // БАЗОВАЯ ТОЧКА - "STP" - НАЗАД
    public float[] B_stp_x_ERR = new float[25];
    public float[] B_stp_y_ERR = new float[25];
    public float[] B_stp_z_ERR = new float[25];
    // ГАБАРИТЫ - ВПРАВО
    public float[] R_x_ERR = new float[25];
    public float[] R_y_ERR = new float[25];
    public float[] R_z_ERR = new float[25];
    // БАЗОВАЯ ТОЧКА - "STP" - ВПРАВО
    public float[] R_stp_x_ERR = new float[25];
    public float[] R_stp_y_ERR = new float[25];
    public float[] R_stp_z_ERR = new float[25];
    // ГАБАРИТЫ - ВЛЕВО
    public float[] L_x_ERR = new float[25];
    public float[] L_y_ERR = new float[25];
    public float[] L_z_ERR = new float[25];
    // БАЗОВАЯ ТОЧКА - "STP" - ВЛЕВО
    public float[] L_stp_x_ERR = new float[25];
    public float[] L_stp_y_ERR = new float[25];
    public float[] L_stp_z_ERR = new float[25];
    //
    public int INDEX_1 = 0;
    // 
}
