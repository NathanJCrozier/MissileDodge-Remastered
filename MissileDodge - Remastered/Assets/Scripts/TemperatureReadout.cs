using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class TemperatureReadout : MonoBehaviour
{
    ShipControl ship_control_script;
    TMP_Text temperature;

    // Start is called before the first frame update
    void Awake()
    {
        ship_control_script = GameObject.Find("Player").GetComponent<ShipControl>();
        temperature = this.GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        temperature.text = "Temperature: " + ((int)(ship_control_script.temperature)).ToString() + "°C";
    }
}
