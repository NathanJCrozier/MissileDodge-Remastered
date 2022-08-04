using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipMainMenu : MonoBehaviour
{

    private EdgeCollider2D colldr;
    private Rigidbody2D rb2D;
    private TrailRenderer engine_trail;
    private TrailRenderer lwing_trail;
    private TrailRenderer rwing_trail;
    private TrailRenderer lbrake_trail;
    private TrailRenderer rbrake_trail;
    private float glide_boost; //The amount of boost accumulated by diving
    public float temperature = 22;
    public float altitude = 0;
    public float currrent_speed = 0;
    public float curr_angle = 0;
    private Animator animator;
    [SerializeField] private float speed = 10f; //How fast the engine can go
    [SerializeField] private float ground_position; //The position of the ground in the level (the jet cannot fly below this point)
    [SerializeField] private float max_height; //The position of the maximum height in the level (the jet cannot fly above this point)
    [SerializeField] private Intent s_intent; //The player's speed intent
    [SerializeField] private Intent r_intent; //The player's rotation intent

    private enum Intent
    {
        ACCELERATE,
        PITCHLEFT,
        PITCHRIGHT,
        IDLE,
        BRAKE,
    }
    // Start is called before the first frame update
    void Start()
    {
        colldr = this.gameObject.GetComponent<EdgeCollider2D>();
        rb2D = this.gameObject.GetComponent<Rigidbody2D>();
        animator = this.gameObject.GetComponent<Animator>();
        engine_trail = this.gameObject.GetComponentsInChildren<TrailRenderer>()[0];
        lwing_trail = this.gameObject.GetComponentsInChildren<TrailRenderer>()[1];
        rwing_trail = this.gameObject.GetComponentsInChildren<TrailRenderer>()[2];
        lbrake_trail = this.gameObject.GetComponentsInChildren<TrailRenderer>()[3];
        rbrake_trail = this.gameObject.GetComponentsInChildren<TrailRenderer>()[4];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        Accelerate();
    }

    void Accelerate()
    {
        animator.SetBool("is_accelerating", true);
        animator.SetBool("is_braking", false);
        lbrake_trail.emitting = false;
        rbrake_trail.emitting = false;
        rb2D.AddRelativeForce(Vector2.right * speed);
        engine_trail.emitting = true;
    }
    void StabilizePitch()
    {
        rb2D.angularVelocity = rb2D.angularVelocity / 2;
    }

    float CheckRotation()
    {
        float ship_angle;

        ship_angle = rb2D.rotation % 360; //Gives the angle within 0-360 degrees

        if (ship_angle < 0)
        {
            ship_angle = ship_angle + 360; //Ensures the angle is never negative.
        }

        return ship_angle;
    }

    void ApplyLift()
    {
        //Flattened out
        if ((curr_angle >= 0 && curr_angle <= 20) || (curr_angle >= 160 && curr_angle <= 200) || (curr_angle >= 340 && curr_angle <= 360))
        {
            Vector2 velocity = rb2D.velocity;
            velocity.y = velocity.y * 0.95f;
            rb2D.velocity = velocity;
            rb2D.AddRelativeForce(Vector2.right * ((speed * 0.25f) * glide_boost));
            rb2D.AddRelativeForce(Vector2.up * ((speed * 0.05f) * glide_boost));
            glide_boost = glide_boost * 0.99f;
        }
        //Pointed up
        else if ((curr_angle >= 20 && curr_angle <= 160))
        {
            rb2D.velocity = rb2D.velocity * 0.99f;
            rb2D.AddRelativeForce(Vector2.right * (1.5f * glide_boost));
            glide_boost = glide_boost * 0.95f;
        }
        //Pointed down
        else if ((curr_angle >= 200 && curr_angle <= 340))
        {
            rb2D.AddRelativeForce(Vector2.right * (speed * 0.5f));
            if (glide_boost < 7f)
            {
                glide_boost += (speed * 0.05f);
            }

        }
    }

}
