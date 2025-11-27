using UnityEngine;

public class HexManager : MonoBehaviour
{

    private ParticleSystem hexParticles;
    private GameObject gameManager;

    [SerializeField] private string collisionTag;
    [SerializeField] private bool _hexed;
    public bool Hexed
    {
        get => this._hexed;
        set => this._hexed = value;
    }
    private bool isSpawning = false;

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
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collided with " + other.gameObject.ToString());
        if (other.gameObject.tag == this.collisionTag && this._hexed && this.gameManager.GetComponent<GameManager>().HexPassDelayCountdown <= 0)
        {
            other.gameObject.GetComponent<HexManager>().Hexed = true;
            this.gameManager.GetComponent<GameManager>().HexPassDelayCountdown = 5;
            this._hexed = false;
            Debug.Log("Hex transferred");
        }
    }
}
