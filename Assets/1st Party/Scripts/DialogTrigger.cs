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
        if (other.gameObject.CompareTag("Player"))
        {
            tutorialScript.SendMessage("Trigger", triggerFlag);
        }
    }
}
