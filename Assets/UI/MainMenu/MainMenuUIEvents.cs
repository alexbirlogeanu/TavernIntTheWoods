using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUIEvents : MonoBehaviour {

	// Use this for initialization
	void Start () {
        //make sure that the game is not paused
        Time.timeScale = 1.0f;
	}
	
    public void OnStartNewGame()
    {
        SceneManager.LoadScene("Scenes/onlylevel");
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }

    public void OnQuitGame()
    {
        Application.Quit();
    }
}
