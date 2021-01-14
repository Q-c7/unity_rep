using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AllCranesData
{
    private const int N = 7;
    private const int M = 4;

    public List<float> PosArray = new List<float>(new float[3 * N]);
    public List<int> TravArray = new List<int>(new int[3 * M]);

    /* foreach (float posi in PosArray)
        posi = 0f;
    foreach (int trav in TravArray)
        trav = 0;  */

    public static void GetFloats (ref Crane CraneScript, ref List<float> PosArr, int pos)
    {
        PosArr[pos] = CraneScript.bridgePosition;
        PosArr[pos + 1] = CraneScript.mtPosition;
        PosArr[pos + 2] = CraneScript.atPosition;
        PosArr[pos + 3] = CraneScript.mtMhPosition;
        PosArr[pos + 4] = CraneScript.mtAhPosition;
        PosArr[pos + 5] = CraneScript.atMhPosition;
        PosArr[pos + 6] = CraneScript.atAhPosition;
    }

    public static void GetInts (ref Crane CraneScript, ref List<int> TravArr, int pos)
    {
        TravArr[pos] = CraneScript.mtMhTraverse;
        TravArr[pos + 1] = CraneScript.mtAhTraverse;
        TravArr[pos + 2] = CraneScript.atMhTraverse;
        TravArr[pos + 3] = CraneScript.atAhTraverse;
    }


    public static AllCranesData InitData (Crane CraneScript1, Crane CraneScript2, Crane CraneScript3) //mne poxui 4to eto kostyl
    {
        AllCranesData NewBlock = new AllCranesData();

        GetFloats(ref CraneScript1, ref NewBlock.PosArray, 0);
        GetFloats(ref CraneScript2, ref NewBlock.PosArray, N);
        GetFloats(ref CraneScript3, ref NewBlock.PosArray, 2 * N);

        GetInts(ref CraneScript1, ref NewBlock.TravArray, 0);
        GetInts(ref CraneScript2, ref NewBlock.TravArray, M);
        GetInts(ref CraneScript3, ref NewBlock.TravArray, 2 * M);

        return NewBlock;
    }
}

/*
[System.Serializable]
public class CraneDictElem
{
    public CraneDictElem(Crane CraneScript)
    {
        float bridgePos = CraneScript.bridgePosition;
        float mtPos = CraneScript.mtPosition;
        float atPos = CraneScript.atPosition;
        float mtMhPos = CraneScript.mtMhPosition;
        float mtAhPos = CraneScript.mtAhPosition;
        float atMhPos = CraneScript.atMhPosition;
        float atAhPos = CraneScript.atAhPosition;

        float mtMhTrav = (float) CraneScript.mtMhTraverse;
        float mtAhTrav = (float) CraneScript.mtAhTraverse;
        float atMhTrav = (float) CraneScript.atMhTraverse;
        float atAhTrav = (float) CraneScript.atAhTraverse;
    }

}*/