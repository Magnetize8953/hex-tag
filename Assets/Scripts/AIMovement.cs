using UnityEngine;
using System;

public class AIMovement : MonoBehaviour
{

    private System.Random random = new System.Random(); // because seeding is weird with UnityEngine.Random
    private GameManager gameManager;
    private HexManager hexManager;
    private CharacterController controller;

    private float speed;
    private float radiusOfSatisfaction;
    public Vector3 velocity;
    private float gravityYPos = 0.580005f; // this is approx the y height of players after gravity is applied; a better way probably exists

    private Transform targetTransform;
    private Vector3 randomMapLocation;
    private bool getNewRandLocation = true;

    [SerializeField] private float gravity = -9.81f;

    private void Start()
    {
        this.speed = 5.0f;
        this.radiusOfSatisfaction = 1.0f;
        this.controller = GetComponent<CharacterController>();
        this.hexManager = GetComponent<HexManager>();
        this.gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        GetRandomPlayerTarget();
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
        Vector3 towardsTarget = targetPosition - this.transform.position;

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
        this.transform.rotation = Quaternion.Lerp(this.transform.rotation, targetRotation, 0.1f);

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

    public void GetRandomPlayerTarget()
    {
        this.targetTransform = this.gameManager.Players[random.Next(this.gameManager.Players.Count)].transform;
        while (this.targetTransform == null || this.targetTransform.transform == this.transform)
            this.targetTransform = this.gameManager.Players[random.Next(this.gameManager.Players.Count)].transform;
    }
}
