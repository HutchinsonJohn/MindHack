using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class WinScreen : MonoBehaviour
{
    public GameObject hackObject;
    public GameObject killObject;
    public GameObject sleepObject;
    public GameObject untouchedObject;
    public GameObject undetectedObject;
    public GameObject scoreObject;

    public TMP_Text hacks;
    public TMP_Text kills;
    public TMP_Text sleeps;
    public TMP_Text untouched;
    public TMP_Text undetected;
    public TMP_Text score;

    private int hackNum;
    private int killNum;
    private int sleepNum;
    private int untouchedNum;
    private int undetectedNum;
    private int scoreNum;

    private int totalEnemies = 17;

    // Start is called before the first frame update
    void Start()
    {
        hackNum = PlayerPrefs.GetInt("HackedEnemies");
        killNum = PlayerPrefs.GetInt("KilledEnemies");
        sleepNum = PlayerPrefs.GetInt("SleptEnemies");
        untouchedNum = totalEnemies - hackNum - killNum - sleepNum;
        undetectedNum = PlayerPrefs.GetInt("TimesUndetected");
        scoreNum = hackNum * 10 + killNum * 100 + sleepNum * 1000 + untouchedNum * 5000 + undetectedNum * 10000;
        Invoke(nameof(Hacks), .5f);
        Invoke(nameof(Kills), 1f);
        Invoke(nameof(Sleeps), 1.5f);
        Invoke(nameof(Untouched), 2f);
        Invoke(nameof(Undetected), 2.5f);
        Invoke(nameof(Score), 3f);
        PlayerPrefs.SetInt("KilledEnemies", 0);
        PlayerPrefs.SetInt("SleptEnemies", 0);
        PlayerPrefs.SetInt("HackedEnemies", 0);
        PlayerPrefs.SetInt("TimesUndetected", 0);
        PlayerPrefs.SetInt("CurrentLevel", 1);
        PlayerPrefs.Save();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Hacks()
    {
        hackObject.SetActive(true);
        hacks.text = "" + hackNum;
    }

    void Kills()
    {
        killObject.SetActive(true);
        kills.text = "" + killNum;
    }

    void Sleeps()
    {
        sleepObject.SetActive(true);
        sleeps.text = "" + sleepNum;
    }

    void Untouched()
    {
        untouchedObject.SetActive(true);
        untouched.text = "" + untouchedNum;
    }

    void Undetected()
    {
        undetectedObject.SetActive(true);
        undetected.text = "" + undetectedNum;
    }

    void Score()
    {
        scoreObject.SetActive(true);
        score.text = "" + scoreNum;
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
