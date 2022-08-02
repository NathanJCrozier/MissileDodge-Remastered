using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class AltitudeReadout : MonoBehaviour
{
    ShipControl ship_control_script;
    TMP_Text altitude;

    // Start is called before the first frame update
    void Awake()
    {
        ship_control_script = GameObject.Find("Player").GetComponent<ShipControl>();
        altitude = this.GetComponents<TextMeshProUGUI>()[0];
    }

    // Update is called once per frame
    void Update()
    {
        altitude.text = "Altitude: " + ((int)(ship_control_script.altitude)).ToString() + "m";
    }
}
