using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMovement : MonoBehaviour
{


    public GameObject character;
    public Camera characterCamera;


    public float click_towards_speed;
    public bool use_click_towards = false;
    private Vector2 targetPosition;

    
    private Rigidbody2D _rigidbody;
    public float rotationSpeed = 720f;
    public float joystick_speed = 4.0f;
    private Vector2 _movementInput = Vector2.zero;
    private Vector2 _smoothedMovementInput;
    private Vector2 _movemenetInputSmoothVelocity;
    private float _smoothDampResponseTime = 0.01f;

    //herman - jag la till en dash :)
    private bool canDash = true;
    

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
        if (canDash)
        {
            SetPlayerVelocity();
            RotateInDirectionOfInput();
        }
    }

    private void SetPlayerVelocity()
    {
        _smoothedMovementInput = Vector2.SmoothDamp(
                    _smoothedMovementInput,
                    _movementInput,
                    ref _movemenetInputSmoothVelocity,
                    _smoothDampResponseTime
                    );
        Vector2 velocity_vector2 = _smoothedMovementInput * joystick_speed;
        _rigidbody.velocity = velocity_vector2;
    }

    private void RotateInDirectionOfInput()
    {
        if (_movementInput != Vector2.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(character.transform.forward, _smoothedMovementInput);
            Quaternion rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            _rigidbody.MoveRotation(rotation);
        } 
    }

    private void OnMove(InputValue inputValue)
    {
        _movementInput = inputValue.Get<Vector2>();


    }

    // herman
    public void Dash()
    {
        if (canDash)
        {
            StartCoroutine(PerformDash());
        }
    }

    private IEnumerator PerformDash()
    {
        canDash = false;
        _rigidbody.velocity = _smoothedMovementInput.normalized * 10f;
        yield return new WaitForSeconds(0.3f);
        canDash = true;
        _rigidbody.velocity = Vector2.zero;
    }
}