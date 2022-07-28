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
    [SerializeField] private Animator animator;
    [SerializeField] private KeyCode accelerate_key;
    [SerializeField] private KeyCode pitch_left_key;
    [SerializeField] private KeyCode pitch_right_key;
    [SerializeField] private KeyCode brake_key;
    [SerializeField] private float speed = 10f;
    [SerializeField] private float rotation_speed = 10f;
    [SerializeField] private float usable_rotation_speed;

    [SerializeField] private Intent s_intent;
    [SerializeField] private Intent r_intent;

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

        usable_rotation_speed = rotation_speed / ((rb2D.velocity.magnitude / 50) + 1);

        if (Input.GetKey(accelerate_key)) { s_intent = Intent.ACCELERATE; }

        else if (Input.GetKey(brake_key)) { s_intent = Intent.BRAKE; }

        else {s_intent = Intent.IDLE;}


        if (Input.GetKey(pitch_right_key)) { r_intent = Intent.PITCHRIGHT; }

        else if (Input.GetKey(pitch_left_key)) { r_intent = Intent.PITCHLEFT; }

        else { r_intent = Intent.IDLE; }

    }

    private void FixedUpdate()
    {
        ApplyLift(CheckRotation());

        if (rb2D.velocity.magnitude > 20)
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

        ship_angle = rb2D.rotation % 360;

        if (ship_angle < 0)
        {
            ship_angle = ship_angle + 360;
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
            rb2D.AddRelativeForce(Vector2.right * 10f);
        }
        //Pointed up
        else if ((angle >= 20 && angle <= 160))
        {
            rb2D.velocity = rb2D.velocity * 0.99f;
        }
        //Pointed down
        else if ((angle >= 200 && angle <= 340))
        {
            rb2D.AddRelativeForce(Vector2.right * 50f);
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
}
