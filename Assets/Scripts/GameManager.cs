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
    private List<Vector3> _worldGraph;
    public List<Vector3> WorldGraph { get => this._worldGraph; }

    private void Awake()
    {
        /* default countdown */
        this._hexPassDelayCountdown = 0;

        /* assign hex */
        GameObject userPlayer = GameObject.FindGameObjectWithTag("Player");
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("AI");

        int selectedPlayer = Random.Range(0, enemies.Length + 1);
        if (selectedPlayer == enemies.Length)
        {
            userPlayer.GetComponent<HexManager>().Hexed = true;
        }
        else
        {
            enemies[selectedPlayer].GetComponent<HexManager>().Hexed = true;
        }

        /* fill out world graph */
    }

    private void Update()
    {
        if (this._hexPassDelayCountdown > 0)
        {
            this._hexPassDelayCountdown -= Time.deltaTime;
        }
    }
}
