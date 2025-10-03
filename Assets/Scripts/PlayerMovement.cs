using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float gravity = -9.81f;
    public float jumpHeight = 2f;

    private CharacterController controller;

    private Vector2 move;
    private Vector3 velocity;
    private bool grounded;

    //INPUT CONTROLS
    public InputActionAsset _action;

    public InputActionAsset action {
        get => _action;
        set => _action = value;
    }

    protected InputAction clickAction {get; set; }

    InputAction moveAction;

    InputAction jumpAction;


    void Awake()
    {
        //map necessary components and inputs
        controller = GetComponent<CharacterController>();
        moveAction = action.FindAction("Move");
        jumpAction = action.FindAction("Jump");

        if (moveAction != null) {
            moveAction.started += OnMove;
            moveAction.performed += OnMove;
            moveAction.canceled += OnMove;
        }

        if (jumpAction != null) {
            jumpAction.started += OnJump;
            jumpAction.performed += OnJump;
            jumpAction.canceled += OnJump;
        }
    }

    public void OnMove(InputAction.CallbackContext context) 
    {
        if (context.started || context.performed) {
            move = context.ReadValue<Vector2>();
        } else if (context.canceled) {
            // stay still if not inputting movement
            move = Vector2.zero;
        }
    }

    public void OnJump(InputAction.CallbackContext context) 
    {
        if (context.started && grounded) {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        grounded = controller.isGrounded;
        if (grounded && velocity.y < 0f) {
            velocity.y = -2f;
        }

        Vector3 trueMove = new Vector3(move.x, 0, move.y);
        trueMove.Normalize(); //normalize the vector to bring its magnitude to 1
        
        controller.Move(trueMove * speed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
