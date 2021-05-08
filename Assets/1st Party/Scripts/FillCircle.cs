using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FillCircle : MonoBehaviour
{

    private PlayerMovement playerMovement;
    private Image meter;
    float test;

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = GameObject.Find("Player").GetComponent<PlayerMovement>();
        test = playerMovement.hackMeter;
        meter = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        meter.fillAmount = playerMovement.hackMeter * 2;
    }
}
