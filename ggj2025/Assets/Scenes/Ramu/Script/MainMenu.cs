using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Name of the scene to load
    [SerializeField]
    private string gameSceneName = "MainGame";

    // Method to start the game
    public void LoadGameScene()
    {
        // Log for debugging
        Debug.Log("Loading game scene: " + gameSceneName);

        // Load the specified scene
        SceneManager.LoadScene(gameSceneName);
    }
    public void QuitGame(){
        Debug.Log("Game is exiting...");
       // Quit the application
        Application.Quit();
    }
}
