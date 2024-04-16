using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]

public class GameData
{
    public int deathCount = 0;
    public Vector3 playerPosition = Vector3.zero;
    public GameData()
    {
        this.deathCount = 0;
    }

    public override string ToString()
    {
        return string.Format("Deathcount {0}", deathCount);
    }

}
