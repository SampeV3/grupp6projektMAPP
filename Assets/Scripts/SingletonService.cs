using UnityEngine;

public class SingletonClass : Singleton<SingletonClass>
{
    // (Optional) Prevent non-singleton constructor use.
    protected SingletonClass() { }
    public delegate void XPAddedAction(int xp_added); //metod signatur för subscribers till eventet
    public static event XPAddedAction OnXPAdded;

    // Then add whatever code to the class you need as you normally would.
    public static void AwardXP(int xp_amount)
    {
        OnXPAdded(xp_amount);
    }

    public static void OnEnemyKilled()
    {
        

    }

}
