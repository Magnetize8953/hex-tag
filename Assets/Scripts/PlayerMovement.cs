using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;

    private CharacterController controller;

    private Vector2 move;

    //INPUT CONTROLS
    public InputActionAsset _action;

    public InputActionAsset action {
        get => _action;
        set => _action = value;
    }

    protected InputAction clickAction {get; set; }

    InputAction moveAction;


    void Awake()
    {
        //map necessary components and inputs
        controller = GetComponent<CharacterController>();
        moveAction = action.FindAction("Move");

        if (moveAction != null) {
            moveAction.started += OnMove;
            moveAction.performed += OnMove;
            moveAction.canceled += OnMove;
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 trueMove = new Vector3(move.x, 0, move.y);
        trueMove.Normalize(); //normalize the vector to bring its magnitude to 1
        
        controller.Move(trueMove * speed * Time.deltaTime);
    }
}
