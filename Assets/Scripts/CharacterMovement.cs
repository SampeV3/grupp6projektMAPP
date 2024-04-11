using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public GameObject character;
    public Camera characterCamera;
    public float speed;
    private Vector2 targetPosition;
    void Start()
    {

        targetPosition = new Vector2(0.0f, 0.0f);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            targetPosition = Input.mousePosition;
            targetPosition = characterCamera.ScreenToWorldPoint(new Vector3(targetPosition.x, targetPosition.y, 0.0f));
        }
        this.transform.position = Vector2.MoveTowards(character.transform.position, targetPosition, speed * Time.deltaTime);


    }
}