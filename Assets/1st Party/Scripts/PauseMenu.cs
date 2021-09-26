using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles pausing behaviors
/// </summary>
public class PauseMenu : MonoBehaviour
{

    public static bool GameIsPaused = false;

    public GameObject pauseMenuUI;
    public GameObject controlsMenuUI;
    public PlayerMovement player;

    public bool inTutorial = false;

    // Update is called once per frame
    void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !inTutorial && !player.isDying)
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    /// <summary>
    /// Sets GameIsPaused to isPaused
    /// </summary>
    /// <param name="isPaused">Whether the game is paused</param>
    public void IsPaused(bool isPaused)
    {
        GameIsPaused = isPaused;
    }

    /// <summary>
    /// Closes pauseMenu and sets timeScale to 1
    /// </summary>
    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        controlsMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    /// <summary>
    /// Opens pauseMenu and sets timeScale to 0
    /// </summary>
    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    /// <summary>
    /// Reloads current level
    /// </summary>
    public void Retry()
    {
        SceneManager.LoadScene(PlayerPrefs.GetInt("CurrentLevel"));
    }

    /// <summary>
    /// Quits the application
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }
}
