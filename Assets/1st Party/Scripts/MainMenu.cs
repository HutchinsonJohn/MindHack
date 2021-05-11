﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{

    public Button loadGameButton;
    public TMP_Text loadGameText;

    private void Start()
    {
        Application.targetFrameRate = 60;
        if (PlayerPrefs.GetInt("CurrentLevel") == 0)
        {
            loadGameButton.interactable = false;
            loadGameText.alpha = .25f;
        }
        BGM.Instance.Stop();
    }

    

    public void NewGame()
    {
        PlayerPrefs.SetInt("KilledEnemies", 0);
        PlayerPrefs.SetInt("SleptEnemies", 0);
        PlayerPrefs.SetInt("HackedEnemies", 0);
        PlayerPrefs.SetInt("TimesUndetected", 0);
        PlayerPrefs.SetInt("CurrentLevel", 1);
        PlayerPrefs.Save();

        SceneManager.LoadScene(1);
    }

    public void LoadGame()
    {
        SceneManager.LoadScene(PlayerPrefs.GetInt("CurrentLevel"));
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
