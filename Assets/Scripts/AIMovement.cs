using UnityEngine;
using System;

public class AIMovement : MonoBehaviour
{

    private System.Random random = new System.Random(Guid.NewGuid().GetHashCode()); // because seeding is weird with UnityEngine.Random
    private GameManager gameManager;
    private HexManager hexManager;
    private CharacterController controller;

    private float speed;
    private float radiusOfSatisfaction;
    public Vector3 velocity;
    private float gravityYPos = 0.580005f; // this is approx the y height of players after gravity is applied; a better way probably exists

    private Transform _targetTransform;
    public Transform TargetTransform { get => this._targetTransform;  }
    private Vector3 randomMapLocation;
    private bool getNewRandLocation = true;
    private float locationCooldown = 10;

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

        // this is the jankiest thing ever
        // basically. destroying things that other things depend on is not a great idea
        // and thus. this.
        try
        {
            var _ = this._targetTransform.position;
        }
        catch (MissingReferenceException)
        {
            // one player remaining
            if (this.gameManager.Players.Count == 1)
                return;

            Debug.Log(this.name + " - changing player target due to destruction");
            GetRandomPlayerTarget();
        }

        if (this.hexManager.Frozen)
            return;

        if (this.hexManager.Hexed)
        {
            RunKinematicArrive(new Vector3(this._targetTransform.position.x, this.gravityYPos, this._targetTransform.position.z));
        }
        else
        {
            // get a random point on the map
            if (this.getNewRandLocation || this.locationCooldown <= 0)
            {
                this.randomMapLocation = new Vector3(
                    UnityEngine.Random.Range(this.gameManager.WorldLowerBound.x, this.gameManager.WorldUpperBound.x),
                    this.gravityYPos,
                    UnityEngine.Random.Range(this.gameManager.WorldLowerBound.z, this.gameManager.WorldUpperBound.z)
                );
                this.getNewRandLocation = false;
                this.locationCooldown = 10;
                // Debug.Log(this.name + " ai going to: " + this.randomMapLocation);
            }

            RunKinematicArrive(this.randomMapLocation);
        }

        this.locationCooldown -= Time.deltaTime;

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

        #region obstacle avoidance
        bool avoid = false;
        float obstacleDistance = 1f;
        int layerMask = 1 << 10;
        Vector3 obstacleCoords = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        Vector3 feetRayCoords = new Vector3(this.transform.position.x, 0.25f, this.transform.position.z);
        Ray ray = new Ray(feetRayCoords, towardsTarget.normalized);
        RaycastHit hit;

        Debug.DrawRay(feetRayCoords, towardsTarget.normalized * obstacleDistance, Color.red);
        Debug.DrawRay(feetRayCoords, Quaternion.Euler(0f, 45, 0f) * towardsTarget.normalized * obstacleDistance, Color.blue); // left
        Debug.DrawRay(feetRayCoords, Quaternion.Euler(0f, -45, 0f) * towardsTarget.normalized * obstacleDistance, Color.green); // right

        // if there is something in the way to the front
        if (Physics.Raycast(ray, out hit, obstacleDistance, layerMask))
            obstacleCoords = hit.point;
        ray = new Ray(feetRayCoords, Quaternion.Euler(0f, -45f, 0f) * towardsTarget.normalized);
        // or to the left
        if (Physics.Raycast(ray, out hit, obstacleDistance, layerMask))
            obstacleCoords = hit.point;
        // or to the right
        ray = new Ray(feetRayCoords, Quaternion.Euler(0f, 45f, 0f) * towardsTarget.normalized);
        if (Physics.Raycast(ray, out hit, obstacleDistance))
            obstacleCoords = hit.point;

        if (obstacleCoords != new Vector3(float.MaxValue, float.MaxValue, float.MaxValue))
        {
            avoid = true;

            // get what would be our right vector, but proportionate to the direction we're traveling
            Vector3 proportionalRight = Vector3.Cross(this.transform.up, towardsTarget).normalized;

            // take the dot product of that and a vector from the NPC's location to where the collision occured
            float dot = Vector3.Dot(proportionalRight, (obstacleCoords - this.transform.position).normalized);

            if (dot >= 0)
            { // if obstacle is to the right (or in front), nudge to the left
                // Debug.Log(this.name + " nudging to the left");
                towardsTarget = Quaternion.AngleAxis(-45f, Vector3.up) * towardsTarget;
            }
            else
            { // otherwise, if its to the left, nudge to the right
                // Debug.Log(this.name + " nudgling to the right");
                towardsTarget = Quaternion.AngleAxis(45f, Vector3.up) * towardsTarget;
            }

            // repeat our prior process of normalizing
            towardsTarget.Normalize();
            towardsTarget *= this.speed;
            this.velocity = towardsTarget;
        }
        #endregion

        // smoothly rotate to face target
        Quaternion targetRotation = Quaternion.LookRotation(towardsTarget);
        this.transform.rotation = Quaternion.Lerp(this.transform.rotation, targetRotation, 0.1f);

        // move forward in the direction we are facing
        if (!avoid)
            this.controller.Move(towardsTarget * this.speed * Time.deltaTime);

        // gravity
        if (this.controller.isGrounded && this.velocity.y < 0f)
            this.velocity.y = -2f;

        this.velocity.y += this.gravity * Time.deltaTime;
        if (avoid)
            this.controller.Move(this.velocity * Time.deltaTime);

    }

    public void GetRandomPlayerTarget()
    {
        // one player remaining
        if (this.gameManager.Players.Count == 1)
            return;

        try
        {
            this._targetTransform = this.gameManager.Players[random.Next(this.gameManager.Players.Count)].transform;
            while (this._targetTransform == null || this._targetTransform.transform == this.transform)
                this._targetTransform = this.gameManager.Players[random.Next(this.gameManager.Players.Count)].transform;
        }
        catch (MissingReferenceException)
        { // this exception should only be happening in frame perfect instances
            Debug.Log(this.name + " - changing player target due to destruction");
            GetRandomPlayerTarget();
        }
        catch (StackOverflowException)
        { // also a frame perfect instance. should have been handled by that first condition, but slips past sometimes
            Debug.Log(this.name + " - last player remaining");
            return;
        }
    }
}
