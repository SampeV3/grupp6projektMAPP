using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMovement : MonoBehaviour
{


    public GameObject character;
    public Camera characterCamera;

    public float joystick_speed = 4.0f;

    public float click_towards_speed;
    public bool use_click_towards = false;


    private Vector2 targetPosition;
    private Vector2 _movementInput = Vector2.zero;
    private Rigidbody2D _rigidbody;

    void Start()
    {

        targetPosition = new Vector2(0.0f, 0.0f);
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!use_click_towards)
        {
            return;
        }
        if (Input.GetMouseButtonDown(0))
        {
            targetPosition = Input.mousePosition;
            targetPosition = characterCamera.ScreenToWorldPoint(new Vector3(targetPosition.x, targetPosition.y, 0.0f));
        }
        this.transform.position = Vector2.MoveTowards(character.transform.position, targetPosition, click_towards_speed * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (use_click_towards) { return; }
        Vector2 velocity_vector2 = _movementInput * joystick_speed;
        _rigidbody.velocity = velocity_vector2;
    }

    private void OnMove(InputValue inputValue)
    {
        _movementInput = inputValue.Get<Vector2>();


    }


}