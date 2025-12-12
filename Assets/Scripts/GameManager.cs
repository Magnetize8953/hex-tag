using System.Collections.Generic;
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

        /* assign hex */
        GameObject userPlayer = GameObject.FindGameObjectWithTag("Player");
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("AI");

        int selectedPlayer = Random.Range(0, enemies.Length + 1);
        if (selectedPlayer == enemies.Length)
            userPlayer.GetComponent<HexManager>().Hexed = true;
        else
            enemies[selectedPlayer].GetComponent<HexManager>().Hexed = true;

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
