using System;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// End of level load trigger
/// </summary>
public class LoadTrigger : MonoBehaviour
{

    public int levelToLoad;

    private PlayerMovement player;

    private void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerMovement>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
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
