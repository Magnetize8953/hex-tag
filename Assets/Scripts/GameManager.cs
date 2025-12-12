using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int hexPassDelay = 5;
    [SerializeField] private float _hexPassDelayCountdown;
    public float HexPassDelayCountdown
    {
        get => this._hexPassDelayCountdown;
        set => this._hexPassDelayCountdown = hexPassDelay; // kinda dumb and confusing outside this line, but shorter than a whole method
    }

    public List<GameObject> Players { get; private set; }
    private GameObject _hexedPlayer;
    public GameObject HexedPlayer
    {
        get => this._hexedPlayer;
        set => this._hexedPlayer = value;
    }

    private ChunkSpawn chunkSpawner;
    public List<GameObject> WorldChunks { get; private set; }
    public Vector3 WorldLowerBound { get; private set; }
    public Vector3 WorldUpperBound { get; private set; }

    private void Awake()
    {
        /* default countdown */
        this._hexPassDelayCountdown = 0;

        /* create list of players */
        this.Players = GameObject.FindGameObjectsWithTag("Player").ToList<GameObject>();

        /* assign hex */
        // TODO: fix
        int selectedPlayer = Random.Range(0, this.Players.Count);
        GameObject hexedPlayer = this.Players[selectedPlayer];
        hexedPlayer.GetComponent<HexManager>().Hexed = true;

        /* chunk information */
        this.chunkSpawner = this.GetComponent<ChunkSpawn>();
        float bounds = this.chunkSpawner.GridSize * 5;
        this.WorldLowerBound = new Vector3(-bounds, 0, -bounds);
        this.WorldUpperBound = new Vector3(bounds, 0, bounds);

        Debug.Log(this.WorldLowerBound + " bounds " + this.WorldUpperBound);
    }

    private void Update()
    {
        if (this._hexPassDelayCountdown > 0)
        {
            this._hexPassDelayCountdown -= Time.deltaTime;
        }
    }

}
