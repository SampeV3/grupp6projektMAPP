using UnityEngine;

public class BulletID : MonoBehaviour
{
    public GameObject KillerGameObject;
    private void Awake()
    {
        transform.parent = RoomContentGenerator.getItemParent();
    }
}
