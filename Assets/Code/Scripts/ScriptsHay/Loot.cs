using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu]
public class Loot : ScriptableObject
{
    public Sprite lootSprite;
    public string lootName;
    public int dropChance;
    public GameObject effectScript;

    public Loot(string lootName, int dropChance, GameObject effectScript)
    {
        this.lootName = lootName;
        this.dropChance = dropChance;
        this.effectScript = effectScript;
    }
}
