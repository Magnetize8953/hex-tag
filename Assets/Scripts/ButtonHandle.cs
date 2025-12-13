using UnityEngine;
using UnityEditor;

public class ButtonHandle : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GoToLevel() {
        UnityEngine.SceneManagement.SceneManager.LoadScene("gridTestingScene");
    }

    public void QuitGame() {
        if (Application.isEditor){
            // Application.Quit() does not work in the editor so
            // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
            EditorApplication.isPlaying = false;
        }
        else {
            Application.Quit();
        }

    }

    public void GoToMenu() {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    public void GoToCredits() {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Credits");
    }

    public void GoToHowToPlay() {
        UnityEngine.SceneManagement.SceneManager.LoadScene("HowToPlay");
    }
}
