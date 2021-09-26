using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Handles the dialog in the first level
/// </summary>
public class TutorialScript : MonoBehaviour
{

    public PauseMenu pauseMenu;
    private bool[] flags = new bool[6];
    public GameObject textBox;
    public TMP_Text promptText;
    public GameObject controls;
    private int activePromptIndex = -1;
    public GameObject eString;
    public GameObject hackString;
    private string[] messages =
        {"Agent, your mission is to reach the heart of Synaum HQ. You are to do so by any means necessary.",
        "A... mind hacking device? Agent, it's imperative that you complete the mission. Use this technology against them at your discretion.",
        "A locked door... You must find a way through. Maybe there is a keycard around here...",
        "It seems the device also serves as an authentication key for the door...",
        "Once you are through that door, the communications signal will break and you'll be on your own. Good luck, Agent.",
        "Use your rifle and suppressed tranquilizer to complete the mission. Remember, your tranquilizer isn't effective if the enemy has spotted you."};

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
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
                StartCoroutine(TextCoroutine(3));
            } else if (activePromptIndex == 0)
            {
                StartCoroutine(TextCoroutine(5));
            } else
            {
                activePromptIndex = -1;
            }

        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (PauseMenu.GameIsPaused) //Pausing occurs on LateUpdate, so this looks at the value before the pause occurs
            {
                controls.SetActive(true);
            } else
            {
                controls.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Triggers dialog flag if proper conditions are met
    /// </summary>
    /// <param name="flag">Dialog flag to be triggered</param>
    void Trigger(int flag)
    {
        switch (flag)
        {
            case 2:
                if (!flags[1] && !flags[2])
                {
                    StartCoroutine(TextCoroutine(flag));
                }
                break;
            case 4:
                if (flags[1] && !flags[4])
                {
                    StartCoroutine(TextCoroutine(flag));
                }
                break;
            default:
                if (!flags[flag])
                {
                    StartCoroutine(TextCoroutine(flag));
                }
                break;
        }
    }

    /// <summary>
    /// Slowly displays the dialogue text
    /// </summary>
    /// <param name="flag">Corresponding dialogue to display</param>
    /// <returns></returns>
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
