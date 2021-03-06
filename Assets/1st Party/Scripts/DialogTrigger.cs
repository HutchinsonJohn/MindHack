using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles dialog triggers for tutorial level
/// </summary>
public class DialogTrigger : MonoBehaviour
{

    public int triggerFlag;
    public TutorialScript tutorialScript;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            tutorialScript.SendMessage("Trigger", triggerFlag);
        }
    }
}
