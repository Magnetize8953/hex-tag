using UnityEngine;

public class AIMovement : MonoBehaviour
{

    private CharacterController controller;
    private float speed;
    private float radiusOfSatisfaction;
    private Vector3 velocity;
    private Vector3 randomMapLocation;

    [SerializeField] private Transform myTransform;
    [SerializeField] private Transform targetTransform;
    [SerializeField] private GameObject map;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private bool _hexed;
    public bool Hexed
    {
        get => this._hexed;
        set => this._hexed = value;
    }

    private void Awake()
    {
        this.speed = 5.0f;
        this.radiusOfSatisfaction = 1.0f;
        this.controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (this._hexed)
        {
            RunKinematicArrive(this.targetTransform.position);
        }
        else
        {
            // get the edges of a single plane
            // default plane size is 10, so moving from the middle, +/- 5 is needed to get to edges
            Vector3 mapBottomLeft = this.map.transform.TransformPoint(new Vector3(-5, 0, 5));
            Vector3 mapTopRight = this.map.transform.TransformPoint(new Vector3(5, 0, -5));

            // get a random point on the map
            if (this.randomMapLocation == Vector3.zero)
            {
                this.randomMapLocation = new Vector3(Random.Range(mapBottomLeft.x, mapTopRight.x), 1, Random.Range(mapBottomLeft.z, mapTopRight.z));
                Debug.Log(this.randomMapLocation);
            }

            RunKinematicArrive(this.randomMapLocation);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collided with " + other.gameObject.ToString());
        if (other.gameObject.tag == "Player" && this._hexed)
        {
            other.gameObject.GetComponent<PlayerMovement>().Hexed = true;
            this._hexed = false;
        }
    }

    private void RunKinematicArrive(Vector3 targetPosition)
    {

        // vector from player to target
        Vector3 towardsTarget = targetPosition - this.myTransform.position;

        // return if we am within the radius of satisfaction of the target
        if (towardsTarget.magnitude <= this.radiusOfSatisfaction)
            return;

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
