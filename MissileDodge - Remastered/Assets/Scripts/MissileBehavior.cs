using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileBehavior : MonoBehaviour
{
    public Animator animator;

    private Rigidbody2D rb2D;

    private GameObject target;

    [SerializeField] private float speed;

    [SerializeField] private float rotation_speed;

    [SerializeField] private float spawn_time;

    [SerializeField] private int time_limit = 30;

    private bool out_of_fuel = false;

    private Vector3 false_position;
    




    // Start is called before the first frame update
    void Start()
    {
        spawn_time = Time.time;
        rb2D = GetComponent<Rigidbody2D>();
        target = GameObject.Find("Player");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //If missiles have been out of fuel for over a second tell the animator to start their death animation.
        if (Time.time - spawn_time > time_limit + 1)
        {
            animator.SetBool("is_done_for", true);
        }
        //If missiles have been out of fuel for over 2 seconds, destroy them.
        if (Time.time - spawn_time > time_limit + 2)
        {
            MissileTimeout();
        }
        //If missiles have surpassed their time to live, set them to out of fuel.
        else if (Time.time - spawn_time > time_limit && !out_of_fuel)
        {
            
            out_of_fuel = true;
            false_position = target.transform.position;
        }
        //If a missile is out of fuel, tell the animator to start spinning it and make it stop tracking the player.
        else if (out_of_fuel)
        {
            animator.SetBool("is_spinning", true);
            SpinOut(false_position);
        }
        //If a missile still has fuel, proceed to chase the player.
        else
        {
            FollowTarget(target.transform);
        }

        

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            MissileTimeout();
        }
        
    }
    //Chase the specified target by rotating in the direction of the negative cross product.
    private void FollowTarget(Transform target)
    {
        Vector2 direction = (Vector2)target.position - rb2D.position;

        direction.Normalize();

        float rotate_amount = Vector3.Cross(direction, transform.right).z;

        rb2D.angularVelocity = -rotate_amount * rotation_speed;

        rb2D.velocity = transform.right * speed;
    }
    //Start slowing down and trailing off of the player
    private void SpinOut(Vector3 old_position)
    {

        speed = speed / 1.1f;
        
        Vector2 direction = (Vector2)old_position- rb2D.position;

        direction.Normalize();

        float rotate_amount = Vector3.Cross(direction, transform.right).z;

        rb2D.angularVelocity = -rotate_amount * rotation_speed;

        rb2D.velocity = transform.right * speed;
    }

    //Get rid of this missile
    private void MissileTimeout()
    {
        Destroy(this.gameObject);
    }
}
