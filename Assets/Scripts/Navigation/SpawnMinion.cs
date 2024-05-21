using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SpawnMinion : MonoBehaviour
{
    [SerializeField] private PrefabPlacer prefabPlacer;
    public Dictionary<string, GameObject> minionDictionary = new Dictionary<string, GameObject>();

    /// <summary>
    ///  Create minion with provided name indexed from minionDictionary.
    /// </summary>
    /// <param name="minionName"></param>
    /// <param name="placementPosition"></param>
    public void CreateMinionAt(string minionName, Vector3 placementPosition)
    {
        GameObject minionPrefab = minionDictionary[minionName];
        GameObject placedMinion = prefabPlacer.CreateObject(minionPrefab, placementPosition);
        try
        {
            if ((RoomContentGenerator.itemParent) != null) {
                placedMinion.transform.SetParent(RoomContentGenerator.itemParent, false);
            }

        } catch (IOException e)
        {
            print(e);
        } finally
        {
            print("Spawned " + minionName + "( " + placedMinion.name + ") at placement position" + placementPosition);
        }
    }


}
