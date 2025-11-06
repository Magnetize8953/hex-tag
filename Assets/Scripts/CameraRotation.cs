using UnityEngine;
using UnityEngine.InputSystem;

public class CameraRotation : MonoBehaviour
{

    private InputAction xLookAction;
    private InputAction yLookAction;
        
    public float sensitivityX = 0.001f;
    public float sensitivityY = 0.001f;

    [SerializeField] private PlayerMovement player; //holder for the parent player object

    [SerializeField] private InputActionAsset action;

    private float rotationY = 0f;

    private void Awake() 
    {
        this.xLookAction = this.action.FindAction("Look");
        this.yLookAction = this.action.FindAction("Look");

        if (this.xLookAction != null) {
            this.xLookAction.started += OnTurn;
            this.xLookAction.performed += OnTurn;
            this.xLookAction.canceled += OnTurn;
        }
        if (this.yLookAction != null) {
            this.yLookAction.started += OnLook;
            this.yLookAction.performed += OnLook;
            this.yLookAction.canceled += OnLook;
        }
        
    }

    private void OnTurn(InputAction.CallbackContext context)
    {   
        //Grab the horizontal position of the mouse and tune it for sensitivity
        float mouseX = this.xLookAction.ReadValue<Vector2>().x * sensitivityX; 
        //Rotate the parent player representation to change the direction it's facing
        this.player.transform.Rotate(Vector3.up * mouseX, Space.World); 
    }
    
    private void OnLook(InputAction.CallbackContext context)
    {
        float mouseY = -this.yLookAction.ReadValue<Vector2>().y * sensitivityY;
        rotationY += mouseY;
        rotationY = Mathf.Clamp(rotationY, -90f, 90f);
        this.transform.localEulerAngles = new Vector3(rotationY, 0f, 0f);
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
