using UnityEngine;

public class AIMovement : MonoBehaviour
{

    private GameObject gameManager;
    private HexManager hexManager;
    private CharacterController controller;
    private float speed;
    private float radiusOfSatisfaction;
    public Vector3 velocity;
    private Vector3 randomMapLocation;
    private bool getNewRandLocation = true;

    [SerializeField] private Transform myTransform;
    [SerializeField] private Transform targetTransform;
    [SerializeField] private GameObject map;
    [SerializeField] private float gravity = -9.81f;

    private void Awake()
    {
        this.speed = 5.0f;
        this.radiusOfSatisfaction = 1.0f;
        this.controller = GetComponent<CharacterController>();
        this.hexManager = GetComponent<HexManager>();
        this.gameManager = GameObject.Find("GameManager");
    }

    private void Update()
    {
        if (this.hexManager.Frozen)
            return;

        if (this.hexManager.Hexed)
        {
            RunKinematicArrive(new Vector3(this.targetTransform.position.x, this.gravityYPos, this.targetTransform.position.z));
        }
        else
        {
            // get a random point on the map
            if (this.getNewRandLocation)
            {
                this.randomMapLocation = new Vector3(
                    UnityEngine.Random.Range(this.gameManager.WorldLowerBound.x, this.gameManager.WorldUpperBound.x),
                    this.gravityYPos,
                    UnityEngine.Random.Range(this.gameManager.WorldLowerBound.z, this.gameManager.WorldUpperBound.z)
                );
                this.getNewRandLocation = false;
                Debug.Log(this.name + " ai going to: " + this.randomMapLocation);
            }

            RunKinematicArrive(this.randomMapLocation);
        }

    }

    private void RunKinematicArrive(Vector3 targetPosition)
    {

        // vector from player to target
        Vector3 towardsTarget = targetPosition - this.myTransform.position;

        // return if we am within the radius of satisfaction of the target
        if (towardsTarget.magnitude <= this.radiusOfSatisfaction)
        {
            this.getNewRandLocation = true;
            return;
        }

        // normalise because all we care about is direction
        towardsTarget = towardsTarget.normalized;

        // smoothly rotate to face target
        Quaternion targetRotation = Quaternion.LookRotation(towardsTarget);
        this.myTransform.rotation = Quaternion.Lerp(this.myTransform.rotation, targetRotation, 0.1f);

        // move forward in the direction we are facing
        this.controller.Move(towardsTarget * this.speed * Time.deltaTime);

        // gravity
        if (this.controller.isGrounded && this.velocity.y < 0f)
        {
            this.velocity.y = -2f;
        }

        this.velocity.y += this.gravity * Time.deltaTime;
        this.controller.Move(this.velocity * Time.deltaTime);

    }
}
