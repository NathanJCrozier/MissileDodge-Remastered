using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileSpawnerBehavior : MonoBehaviour
{

    [SerializeField] private GameObject to_be_spawned;
    [SerializeField] private int spawn_rate;
    private float current_time;
    private int counter;

    // Start is called before the first frame update
    void Start()
    {
        current_time = Time.time;
        counter = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (counter == spawn_rate * 50)
        {
            SpawnObject(to_be_spawned);
            counter = 0;
        }

        counter++;
        
    }

    void SpawnObject(GameObject to_be_spawned)
    {
        Instantiate(to_be_spawned, (this.transform.position), this.transform.rotation);
    }
}
