using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Fills the hack circle
/// </summary>
public class FillCircle : MonoBehaviour
{

    private PlayerMovement playerMovement;
    private Image meter;

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = GameObject.Find("Player").GetComponent<PlayerMovement>();
        meter = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        meter.fillAmount = playerMovement.hackMeter * 2;
    }
}
