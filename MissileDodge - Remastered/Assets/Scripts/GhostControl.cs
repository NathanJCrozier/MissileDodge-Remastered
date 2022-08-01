using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GhostControl : MonoBehaviour
{

    LevelLoader level_loader_script;

    // Start is called before the first frame update
    void Start()
    {
        level_loader_script = GameObject.Find("LevelLoader").GetComponent<LevelLoader>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKey)
        {
            level_loader_script.LoadLevel("Main Level");
        }
    }
}
