using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialScript : MonoBehaviour
{

    public GameObject gameCanvas;
    private PauseMenu pauseMenu;
    private bool[] flags = new bool[5];
    public GameObject textBox;
    public TMP_Text promptText;
    private int activePromptIndex = -1;
    public GameObject eString;
    public GameObject hackString;
    private string[] messages =
        {"Agent, your mission is to reach the heart of Synaum HQ. You are to do so by any means necessary. Use your rifle and suppressed tranquilizer to complete the mission.",
        "A... mind hacking device? Agent, it's imperative that you complete the mission. Use this technology against them at your discretion.",
        "A locked door... You must find a way through. Maybe there is a keycard around here...",
        "It seems the device also serves as an authentication key for the door...",
        "Once you are through that door, the communications signal will break and you'll be on your own. Good luck, Agent."};

    // Start is called before the first frame update
    void Start()
    {
        pauseMenu = gameCanvas.GetComponent<PauseMenu>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Escape))
        {
            if (activePromptIndex != -1)
            {
                if (activePromptIndex == 1)
                {
                    eString.SetActive(true);
                    hackString.SetActive(true);
                }
                StopAllCoroutines();
                promptText.gameObject.SetActive(false);
                textBox.SetActive(false);
                pauseMenu.inTutorial = false;
                pauseMenu.IsPaused(false);
            }
            if (activePromptIndex == 1 && flags[2])
            {
                StartCoroutine("TextCoroutine", 3);
            } else
            {
                activePromptIndex = -1;
            }
        }
    }

    void Trigger(int flag)
    {
        switch (flag)
        {
            case 2:
                if (!flags[1] && !flags[2])
                {
                    StartCoroutine("TextCoroutine", flag);
                }
                break;
            case 4:
                if (flags[1] && !flags[4])
                {
                    StartCoroutine("TextCoroutine", flag);
                }
                break;
            default:
                if (!flags[flag])
                {
                    StartCoroutine("TextCoroutine", flag);
                }
                break;
        }
    }

    IEnumerator TextCoroutine(int flag)
    {
        promptText.gameObject.SetActive(true);
        textBox.SetActive(true);
        flags[flag] = true;
        pauseMenu.inTutorial = true;
        pauseMenu.IsPaused(true);
        activePromptIndex = flag;
        promptText.text = "";
        foreach (char letter in messages[flag].ToCharArray())
        {
            promptText.text += letter;
            yield return new WaitForSeconds(.02f);
        }
    }
}
