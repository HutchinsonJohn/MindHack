using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogTrigger : MonoBehaviour
{

    public int triggerFlag;
    public GameObject tutorialCanvas;
    private TutorialScript tutorialScript;

    private void Start()
    {
        tutorialScript = tutorialCanvas.GetComponent<TutorialScript>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            tutorialScript.SendMessage("Trigger", triggerFlag);
        }
    }
}
