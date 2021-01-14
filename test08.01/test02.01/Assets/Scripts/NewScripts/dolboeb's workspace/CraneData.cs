using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CraneData
{
    public float bridgePos, mtPos, atPos, mtMhPos, mtAhPos, atMhPos, atAhPos = 0f;
    public int mtMhTrav, mtAhTrav, atMhTrav, atAhTrav = 0;

    public static CraneData InitData (ref Crane CraneScript)
    {
        CraneData NewBlock = new CraneData();

        NewBlock.bridgePos = CraneScript.bridgePosition;
        NewBlock.mtPos = CraneScript.mtPosition;
        NewBlock.atPos = CraneScript.atPosition;
        NewBlock.mtMhPos = CraneScript.mtMhPosition;
        NewBlock.mtAhPos = CraneScript.mtAhPosition;
        NewBlock.atMhPos = CraneScript.atMhPosition;
        NewBlock.atAhPos = CraneScript.atAhPosition;

        NewBlock.mtMhTrav = CraneScript.mtMhTraverse;
        NewBlock.mtAhTrav = CraneScript.mtAhTraverse;
        NewBlock.atMhTrav = CraneScript.atMhTraverse;
        NewBlock.atAhTrav = CraneScript.atAhTraverse;

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