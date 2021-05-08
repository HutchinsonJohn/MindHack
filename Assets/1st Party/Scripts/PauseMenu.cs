using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{

    public static bool GameIsPaused = false;

    public GameObject pauseMenuUI;

    public bool inTutorial = false;

    // Update is called once per frame
    void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !inTutorial)
        {
            if (GameIsPaused)
            {
                Resume();
            } else
            {
                Pause();
            }
        }
    }

    public void IsPaused(bool isPaused)
    {
        GameIsPaused = isPaused;
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
