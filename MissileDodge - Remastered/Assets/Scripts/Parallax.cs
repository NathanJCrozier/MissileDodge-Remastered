using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    private float length;
    private float start_position;
    [SerializeField] GameObject game_camera;
    [SerializeField] private float parallax_effect;

    // Start is called before the first frame update
    void Start()
    {
        start_position = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        float temp = (game_camera.transform.position.x * (1 - parallax_effect));

        if (temp > start_position + length)
        {
            start_position += length;
        }
        else if (temp < start_position - length) start_position -= length;

        float distance = (game_camera.transform.position.x * parallax_effect);

        transform.position = new Vector3(start_position + distance, transform.position.y, transform.position.z);
    }
}
