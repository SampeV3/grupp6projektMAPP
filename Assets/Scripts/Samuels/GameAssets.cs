using UnityEngine;

public class GameAssets : MonoBehaviour
{
    private static GameAssets _i;

    public static GameAssets instance
    {
        get
        {
            if (_i == null) _i = (Instantiate(Resources.Load("GameAssets")) as GameObject).GetComponent<GameAssets>();
            return _i;
        }
    }

    public Sprite Wepon_1;
    public Sprite Wepon_2;
    public Sprite HealthCharge;
    public Sprite HealthBattery;
    public Sprite PowerUp_1;
}

