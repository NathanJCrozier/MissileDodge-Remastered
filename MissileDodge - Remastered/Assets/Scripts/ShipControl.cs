using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipControl : MonoBehaviour
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
    private bool is_frozen = false;
    [SerializeField] private Animator animator;
    [SerializeField] private KeyCode accelerate_key;
    [SerializeField] private KeyCode pitch_left_key;
    [SerializeField] private KeyCode pitch_right_key;
    [SerializeField] private KeyCode brake_key; 
    [SerializeField] private float speed = 10f; //How fast the engine can go
    [SerializeField] private float rotation_speed = 10f; //How much the player should be able to rotate
    [SerializeField] private float usable_rotation_speed; //How much the player can actually rotate
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

        if (temperature <= -40f)
        {
            Freeze();
        }
        //If we have touched/gone below the ground the player dies.
        if (this.transform.position.y <= ground_position)
        {
            DestroyShip();
        }

        if (this.transform.position.y >= max_height)
        {
            temperature -= 0.025f;
        }
        else if (this.transform.position.y <= max_height && temperature < 22)
        {
            temperature += 0.01f;
        }

        //The player's ability to rotate will diminish somewhat as their speed increases.
        usable_rotation_speed = rotation_speed / ((rb2D.velocity.magnitude / 50) + 1);

        if (Input.GetKey(accelerate_key)) { s_intent = Intent.ACCELERATE; }

        else if (Input.GetKey(brake_key)) { s_intent = Intent.BRAKE; }

        else {s_intent = Intent.IDLE;}


        if (Input.GetKey(pitch_right_key)) { r_intent = Intent.PITCHRIGHT; }

        else if (Input.GetKey(pitch_left_key)) { r_intent = Intent.PITCHLEFT; }

        else { r_intent = Intent.IDLE; }

    }
    //FixedUpdate is called 50 times per second
    private void FixedUpdate()
    {

        if (is_frozen)
        {
            engine_trail.emitting = false;
            lwing_trail.emitting = false;
            rwing_trail.emitting = false;
            return;
        }

        //Apply lift forces to the ship based on its angle
        ApplyLift(CheckRotation());

        //If the plane's speed is fast enough, we can show additional particle effects.
        if (rb2D.velocity.magnitude > 10)
        {
            lwing_trail.emitting = true;
            rwing_trail.emitting = true;
        }
        else
        {
            lwing_trail.emitting = false;
            rwing_trail.emitting = false;
            lbrake_trail.emitting = false;
            rbrake_trail.emitting = false;
        }

        if (s_intent == Intent.ACCELERATE)
        {
            Accelerate();
        }

        else if (s_intent == Intent.BRAKE)
        {
            Brake();
        }
        else if (s_intent == Intent.IDLE)
        {
            animator.SetBool("is_accelerating", false);
            animator.SetBool("is_braking", false);
            lbrake_trail.emitting = false;
            rbrake_trail.emitting = false;
            engine_trail.emitting = false;
        }

        if (r_intent == Intent.PITCHLEFT)
        {
            PitchLeft();
        }

        else if (r_intent == Intent.PITCHRIGHT)
        {
            PitchRight();
        }

        else if (r_intent == Intent.IDLE)
        {
            StabilizePitch(); 
        }
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

    void PitchLeft()
    {
        rb2D.angularVelocity = usable_rotation_speed;
    }

    void PitchRight()
    {
        rb2D.angularVelocity = -usable_rotation_speed;
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

    void ApplyLift(float angle)
    {
        //Flattened out
        if ((angle >= 0 && angle <= 20) || (angle >= 160 && angle <= 200) || (angle >= 340 && angle <= 360))
        {
            Vector2 velocity = rb2D.velocity;
            velocity.y = velocity.y * 0.95f;
            rb2D.velocity = velocity;
            rb2D.AddRelativeForce(Vector2.right * ((speed * 0.25f) * glide_boost));
            rb2D.AddRelativeForce(Vector2.up * ((speed * 0.05f) * glide_boost));
            glide_boost = glide_boost * 0.99f;
        }
        //Pointed up
        else if ((angle >= 20 && angle <= 160))
        {
            rb2D.velocity = rb2D.velocity * 0.99f;
            rb2D.AddRelativeForce(Vector2.right * (1.5f * glide_boost));
            glide_boost = glide_boost * 0.95f;
        }
        //Pointed down
        else if ((angle >= 200 && angle <= 340))
        {
            rb2D.AddRelativeForce(Vector2.right * (speed * 0.5f));
            if (glide_boost < 7f)
            {
                glide_boost += (speed * 0.05f);
            }
            
        }
    }

    void Brake()
    {
        animator.SetBool("is_accelerating", false);
        animator.SetBool("is_braking", true);
        if (rb2D.velocity.magnitude > 20)
        {
            lbrake_trail.emitting = true;
            rbrake_trail.emitting = true;
        }
        engine_trail.emitting = false;
        rb2D.velocity = rb2D.velocity * 0.90f;
    }

    void Freeze()
    {
        is_frozen = true;
        animator.SetBool("is_frozen", true);
    }

    void DestroyShip()
    {
        Destroy(this.gameObject);
    }
}
