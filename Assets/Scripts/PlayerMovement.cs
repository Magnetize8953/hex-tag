using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    private GameObject gameManager;
    private CharacterController controller;
    private InputAction moveAction;
    private InputAction jumpAction;
    private Vector2 move;
    private Vector3 velocity;
    private bool grounded;

    [SerializeField] private InputActionAsset action;

    [SerializeField] private float speed = 5f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private bool _hexed;
    public bool Hexed
    {
        get => this._hexed;
        set => this._hexed = value;
    }

    private ParticleSystem hexParticles;
    private bool isSpawning = false;

    private void Awake()
    {

        this.gameManager = GameObject.Find("GameManager");

        // map necessary components and inputs
        this.controller = GetComponent<CharacterController>();
        this.moveAction = this.action.FindAction("Move");
        this.jumpAction = this.action.FindAction("Jump");

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

        this.hexParticles = GetComponent<ParticleSystem>();
        if (this._hexed)
        {
            hexParticles.Play();
            isSpawning = true;
        }
        else
        {
            hexParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            isSpawning = false;
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
        }
    }

    private void Update()
    {
        this.grounded = this.controller.isGrounded;
        if (this.grounded && this.velocity.y < 0f) {
            this.velocity.y = -2f;
        }

        Vector3 trueMove = new Vector3(this.move.x, 0, this.move.y);
        trueMove = transform.TransformDirection(trueMove);//converting from local to world space (credit to ChatGPT to enable look-driven turning)

        trueMove.Normalize(); // normalize the vector to bring its magnitude to 1

        this.controller.Move(trueMove * this.speed * Time.deltaTime);

        this.velocity.y += this.gravity * Time.deltaTime;
        this.controller.Move(this.velocity * Time.deltaTime);

        if (this._hexed && !isSpawning) {
            hexParticles.Play();
            isSpawning = true;
        } else if (!this._hexed && isSpawning) {
            hexParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            isSpawning = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collided with " + other.gameObject.ToString());
        if (other.gameObject.tag == "AI" && this._hexed && this.gameManager.GetComponent<GameManager>().HexPassDelayCountdown <= 0)
        {
            other.gameObject.GetComponent<AIMovement>().Hexed = true;
            this.gameManager.GetComponent<GameManager>().HexPassDelayCountdown = 5;
            this._hexed = false;
            Debug.Log("Hex transferred");
        }
    }
}
