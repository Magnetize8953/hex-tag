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
    private bool updateHex = false;

    private ChunkSpawn chunkSpawner;
    public List<GameObject> WorldChunks { get; private set; }
    public Vector3 WorldLowerBound { get; private set; }
    public Vector3 WorldUpperBound { get; private set; }

    [SerializeField] private float hexRoundDuration = 60;
    [SerializeField] private float _hexRoundCountdown;
    public float HexRoundCountdown
    {
        get => this._hexRoundCountdown;
        set => this._hexRoundCountdown = hexRoundDuration; // same as above
    }

    private void Awake()
    {
        /* default countdown */
        this._hexPassDelayCountdown = 0;

        /* set up hex */
        AssignHex();

        /* chunk information */
        this.chunkSpawner = this.GetComponent<ChunkSpawn>();
        float bounds = this.chunkSpawner.GridSize * 5;
        this.WorldLowerBound = new Vector3(-bounds, 0, -bounds);
        this.WorldUpperBound = new Vector3(bounds, 0, bounds);

        /* game timer */
        this._hexRoundCountdown = this.hexRoundDuration;
    }

    private void Update()
    {
        if (updateHex)
            AssignHex();

        if (this._hexPassDelayCountdown > 0)
            this._hexPassDelayCountdown -= Time.deltaTime;

        if (this._hexRoundCountdown > 0)
        {
            this._hexRoundCountdown -= Time.deltaTime;
        }
        else
        {
            if (this._hexedPlayer != GameObject.FindGameObjectWithTag("Player") && this.Players.Count - 1 != 1)
            {
                Debug.Log(this._hexedPlayer.name + " is about to be destroyed...");
                Destroy(this._hexedPlayer);
                this.updateHex = true;
            }
            else if (this._hexedPlayer != GameObject.FindGameObjectWithTag("Player") && this.Players.Count - 1 == 1)
            {
                Destroy(this._hexedPlayer);
                Debug.Log("player wins!!!");
                Destroy(GameObject.FindGameObjectWithTag("Player"));
                UnityEngine.SceneManagement.SceneManager.LoadScene("WinScreen");
                return;
            }
            else
            {
                Debug.Log("player loses!!!");
                Destroy(GameObject.FindGameObjectWithTag("Player"));
                UnityEngine.SceneManagement.SceneManager.LoadScene("LoseScreen");
                return;
            }
        }
    }

    private void AssignHex()
    {
        // create list of players
        this.Players = GameObject.FindGameObjectsWithTag("Player").ToList<GameObject>();
        this.Players.AddRange(GameObject.FindGameObjectsWithTag("AI").ToList<GameObject>());

        // assign hex
        int selectedPlayer = Random.Range(0, this.Players.Count);
        GameObject hexedPlayer = this.Players[selectedPlayer];
        hexedPlayer.GetComponent<HexManager>().Hexed = true;
        this._hexedPlayer = hexedPlayer;
        Debug.Log("new hex: " + this._hexedPlayer.name);

        // reset countdown
        this._hexRoundCountdown = this.hexRoundDuration;

        // updating the hex assignment this way because the list of gameobjects with a given tag doesn't update until next update loop
        // this allows the assignhex() call to be delayed by one update tick
        this.updateHex = false;
    }

}
