using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Items : MonoBehaviour
{
    public enum ItemType
    {
        Wepon_1,
        Wepon_2, 
        HealthCharge,
        HealthBattery,
        PowerUp_1

    }

    public static int GetCost(ItemType itemType)
    {
        switch (itemType)
        {
            default:
            case ItemType.Wepon_1:          return 200;
            case ItemType.Wepon_2:          return 500;
            case ItemType.HealthCharge:     return 20;
            case ItemType.HealthBattery:    return 100;
            case ItemType.PowerUp_1:        return 70;
        }
    }

    public static Sprite GetSprite(ItemType itemType)
    {
        switch(itemType)
        {
            default: 
            case ItemType.Wepon_1: return GameAssets.instance.Wepon_1;
            case ItemType.Wepon_2: return GameAssets.instance.Wepon_2;
            case ItemType.HealthBattery: return GameAssets.instance.HealthBattery;
            case ItemType.HealthCharge: return GameAssets.instance.HealthCharge;
            case ItemType.PowerUp_1: return GameAssets.instance.PowerUp_1;

        }
    }


}
