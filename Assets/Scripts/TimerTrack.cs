using UnityEngine;
using TMPro;

public class TimerTrack : MonoBehaviour
{

    [SerializeField] private GameManager gm;

    TextMeshProUGUI tmp;
    
    void Awake() 
    {
        tmp = GetComponent<TextMeshProUGUI>();
    }
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        tmp.text = "Time to Hexplosion: " + gm.HexRoundCountdown + "!";
    }
}
