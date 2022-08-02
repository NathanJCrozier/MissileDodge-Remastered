using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class SpeedReadout : MonoBehaviour
{
    ShipControl ship_control_script;
    TMP_Text speed;

    // Start is called before the first frame update
    void Awake()
    {
        ship_control_script = GameObject.Find("Player").GetComponent<ShipControl>();
        speed = this.GetComponents<TextMeshProUGUI>()[0];
    }

    // Update is called once per frame
    void Update()
    {
        speed.text = "Speed: " + ((int)(ship_control_script.currrent_speed)).ToString() + "kt";
    }
}
