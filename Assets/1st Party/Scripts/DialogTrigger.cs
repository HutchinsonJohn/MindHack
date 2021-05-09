using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
