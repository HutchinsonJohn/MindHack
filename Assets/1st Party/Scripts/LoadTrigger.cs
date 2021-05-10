using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class LoadTrigger : MonoBehaviour
{

    public int levelToLoad;

    public PlayerMovement player;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            PlayerPrefs.SetInt("KilledEnemies", PlayerPrefs.GetInt("KilledEnemies") + player.killedEnemies);
            PlayerPrefs.SetInt("SleptEnemies", PlayerPrefs.GetInt("SleptEnemies") + player.sleptEnemies);
            PlayerPrefs.SetInt("HackedEnemies", PlayerPrefs.GetInt("HackedEnemies") + player.hackedEnemies);
            if (!player.wasDetected)
            {
                PlayerPrefs.SetInt("TimesUndetected", PlayerPrefs.GetInt("TimesUndetected") + 1);
            }
            PlayerPrefs.SetInt("RifleEquipped", Convert.ToInt32(player.rifleEquipped));
            PlayerPrefs.SetInt("CurrentLevel", levelToLoad);
            PlayerPrefs.Save();

            SceneManager.LoadScene(levelToLoad);
        }
    }

}
