using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    private GameObject gameManager;
    private HexManager hexManager;
    private CharacterController controller;
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction sprintAction;
    private Vector2 move;
    private Vector3 velocity;
    private bool grounded;
    private bool canDoubleJump = true;

    [SerializeField] private InputActionAsset action;

    [SerializeField] private float speed = 5f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpHeight = 2f;

    private void Awake()
    {

        this.gameManager = GameObject.Find("GameManager");
        this.hexManager = GetComponent<HexManager>();

        // map necessary components and inputs
        this.controller = GetComponent<CharacterController>();
        this.moveAction = this.action.FindAction("Move");
        this.jumpAction = this.action.FindAction("Jump");
        this.sprintAction = this.action.FindAction("Sprint");

        if (this.moveAction != null) {
            this.moveAction.started += OnMove;
            this.moveAction.performed += OnMove;
            this.moveAction.canceled += OnMove;
        }

        if (this.jumpAction != null) {
            this.jumpAction.started += OnJump;
            this.jumpAction.performed += OnJump;
            this.jumpAction.canceled += OnJump;
        }
    }

    private void OnMove(InputAction.CallbackContext context) 
    {
        if (context.started || context.performed) {
            this.move = context.ReadValue<Vector2>();
        } else if (context.canceled) {
            // stay still if not inputting movement
            this.move = Vector2.zero;
        }
    }

    private void OnJump(InputAction.CallbackContext context) 
    {
        if (context.started && this.grounded) {
            this.velocity.y = Mathf.Sqrt(this.jumpHeight * -2f * this.gravity);
        } else if (context.started && !this.grounded && this.canDoubleJump) {
            // swapped -2f for -1f
            // not sure if this is the best way to do it, but it works
            this.velocity.y = Mathf.Sqrt(this.jumpHeight * -1f * this.gravity);
            this.canDoubleJump = false;
        }
    }

    private void Update()
    {
        this.grounded = this.controller.isGrounded;
        if (this.grounded && this.velocity.y < 0f) {
            this.velocity.y = -2f;
        }

        // separate if to allow for double jumping while still going up
        if (this.grounded)
            this.canDoubleJump = true;

        // there's gotta be a better way to do this
        if (this.hexManager.Frozen)
            return;

        Vector3 trueMove = new Vector3(this.move.x, 0, this.move.y);
        trueMove = transform.TransformDirection(trueMove);//converting from local to world space (credit to ChatGPT to enable look-driven turning)

        trueMove.Normalize(); // normalize the vector to bring its magnitude to 1

        // currently just sprinting, including in air
        // look into doing a single-shot air dash in addition
        if (this.sprintAction.IsPressed())
            trueMove *= 2;

        this.controller.Move(trueMove * this.speed * Time.deltaTime);

        this.velocity.y += this.gravity * Time.deltaTime;
        this.controller.Move(this.velocity * Time.deltaTime);
    }
}
