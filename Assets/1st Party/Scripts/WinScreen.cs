using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Calculates and displays the user's score
/// </summary>
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
        // TODO: Save the high score somewhere and display it
        PlayerPrefs.SetInt("KilledEnemies", 0);
        PlayerPrefs.SetInt("SleptEnemies", 0);
        PlayerPrefs.SetInt("HackedEnemies", 0);
        PlayerPrefs.SetInt("TimesUndetected", 0);
        PlayerPrefs.SetInt("CurrentLevel", 0);
        PlayerPrefs.Save();
    }

    private void Hacks()
    {
        hackObject.SetActive(true);
        hacks.text = hackNum.ToString();
    }

    private void Kills()
    {
        killObject.SetActive(true);
        kills.text = killNum.ToString();
    }

    private void Sleeps()
    {
        sleepObject.SetActive(true);
        sleeps.text = sleepNum.ToString();
    }

    private void Untouched()
    {
        untouchedObject.SetActive(true);
        untouched.text = untouchedNum.ToString();
    }

    private void Undetected()
    {
        undetectedObject.SetActive(true);
        undetected.text = undetectedNum.ToString();
    }

    private void Score()
    {
        scoreObject.SetActive(true);
        score.text = scoreNum.ToString();
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
