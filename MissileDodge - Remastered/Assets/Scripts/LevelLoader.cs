using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class LevelLoader : MonoBehaviour
{

    public Animator transition;
    public float transition_time = 0.5f;

    // Update is called once per frame
    void Update()
    {

    }

    public void LoadLevel(string level_name)
    {
        StartCoroutine(Initiate_Level_Load(level_name));
    }


    IEnumerator Initiate_Level_Load(string level_name)
    {
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transition_time);

        SceneManager.LoadScene(level_name);
    }

}
