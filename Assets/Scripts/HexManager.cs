using UnityEngine;

public class HexManager : MonoBehaviour
{

    private ParticleSystem hexParticles;
    private GameObject gameManager;

    [SerializeField] private string _collisionTag;
    public string CollisionTag { get; }
    [SerializeField] private bool _hexed;
    public bool Hexed
    {
        get => this._hexed;
        set => this._hexed = value;
    }
    private bool isSpawning = false;


    private bool _frozen = false;
    public bool Frozen { get => this._frozen; set => this._frozen = value; }
    [SerializeField] private int _spinTimeMin = 3;
    public int SpinTimeMin { get => this._spinTimeMin; }
    [SerializeField] private int _spinTimeMax = 7;
    public int SpinTimeMax { get => this._spinTimeMax; }
    private int _spinTime = 1;
    public int SpinTime { set => this._spinTime = value; }
    private Vector3 _savedPosition = Vector3.zero;
    public Vector3 SavedPosition { set => this._savedPosition = value; }

    void Awake()
    {
        this.gameManager = GameObject.Find("GameManager");
        this.hexParticles = GetComponent<ParticleSystem>();

        if (this._hexed)
        {
            this.hexParticles.Play();
            this.isSpawning = true;
        }
        else
        {
            this.hexParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            this.isSpawning = false;
        }
    }

    void Update()
    {
        if (this._hexed && !this.isSpawning)
        {
            this.hexParticles.Play();
            this.isSpawning = true;
        }
        else if (!this._hexed && this.isSpawning)
        {
            this.hexParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            this.isSpawning = false;
        }

        // TODO: make actually based on an amount of time. right now it is just a weird thing????
        if (this._frozen)
        {
            // freeze
            this.transform.position = this._savedPosition;

            // spin
            Quaternion sideTarget = Quaternion.Euler(0, (this.transform.rotation.y + 1) + (this._spinTime * 150), 0);
            this.transform.rotation = Quaternion.Lerp(this.transform.rotation, sideTarget, Time.deltaTime * 5f);

            if (Quaternion.Angle(this.transform.rotation, sideTarget) < 5)
            {
                this._frozen = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collided with " + other.gameObject.ToString());
        if (other.gameObject.tag == "Player" && this._hexed && this.gameManager.GetComponent<GameManager>().HexPassDelayCountdown <= 0)
        {
            other.gameObject.GetComponent<HexManager>().Hexed = true;
            other.gameObject.GetComponent<HexManager>().Frozen = true;
            other.gameObject.GetComponent<HexManager>().SpinTime = Random.Range(
                other.gameObject.GetComponent<HexManager>().SpinTimeMin,
                other.gameObject.GetComponent<HexManager>().SpinTimeMax + 1
            );
            other.gameObject.GetComponent<HexManager>().SavedPosition = other.gameObject.transform.position;
            if (other.GetComponent<AIMovement>() != null)
                other.gameObject.GetComponent<AIMovement>().GetRandomPlayerTarget();
            this.gameManager.GetComponent<GameManager>().HexPassDelayCountdown = 5;
            this._hexed = false;
            Debug.Log("Hex transferred");
        }
    }
}
