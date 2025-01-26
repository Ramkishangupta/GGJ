using UnityEngine;
using UnityEngine.SceneManagement; // For exiting the application
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
	public GameObject pauseMenuUI; // Reference to the pause menu panel
	private bool isPaused = false; // Tracks the pause state

	private void Update()
	{
		// Toggle pause menu on Escape key
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (isPaused)
			{
				ResumeGame();
			}
			else
			{
				PauseGame();
			}
		}
	}

	public void PauseGame()
	{
		pauseMenuUI.SetActive(true); // Show the pause menu
		Time.timeScale = 0f; // Freeze game time
		isPaused = true;
	}

	public void ResumeGame()
	{
		pauseMenuUI.SetActive(false); // Hide the pause menu
		Time.timeScale = 1f; // Resume game time
		isPaused = false;
	}

	public void ExitGame()
	{
		Debug.Log("Exiting the game...");
		Application.Quit(); // Quit the application (works only in a build)
	}
}
