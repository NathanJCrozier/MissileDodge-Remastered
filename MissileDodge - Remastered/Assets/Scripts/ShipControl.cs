using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShipControl : MonoBehaviour
{
    private EdgeCollider2D colldr;
    private Rigidbody2D rb2D;
    LevelLoader level_loader_script;
    private TrailRenderer engine_trail;
    private TrailRenderer lwing_trail;
    private TrailRenderer rwing_trail;
    private TrailRenderer lbrake_trail;
    private TrailRenderer rbrake_trail;
    private float glide_boost; //The amount of boost accumulated by diving
    private int frames_spent_upside_down = 0;
    public float temperature = 22;
    public float altitude = 0;
    public float currrent_speed = 0;
    public float curr_angle = 0;
    private bool is_frozen = false;
    private Animator animator;
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

    Gamepad game_pad;
    private bool gamepad_enabled = true;

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
        level_loader_script = GameObject.Find("LevelLoader").GetComponent<LevelLoader>();
        rb2D = this.gameObject.GetComponent<Rigidbody2D>();
        animator = this.gameObject.GetComponent<Animator>();
        engine_trail = this.gameObject.GetComponentsInChildren<TrailRenderer>()[0];
        lwing_trail = this.gameObject.GetComponentsInChildren<TrailRenderer>()[1];
        rwing_trail = this.gameObject.GetComponentsInChildren<TrailRenderer>()[2];
        lbrake_trail = this.gameObject.GetComponentsInChildren<TrailRenderer>()[3];
        rbrake_trail = this.gameObject.GetComponentsInChildren<TrailRenderer>()[4];
        game_pad = Gamepad.current;
    }

    // Update is called once per frame
    void Update()
    {

        //The player's ability to rotate will diminish somewhat as their speed increases.
        usable_rotation_speed = rotation_speed / ((rb2D.velocity.magnitude / 50) + 1);

        if (gamepad_enabled && game_pad != null)
        {
            CheckGamepadInput();
        }

        else
        {
            CheckKeyboardInput();
        }
        

    }
    //FixedUpdate is called 50 times per second
    private void FixedUpdate()
    {

        curr_angle = CheckRotation();

        MaintainUpright();

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
            temperature -= 0.05f;
        }
        else if (this.transform.position.y <= max_height && temperature < 22)
        {
            temperature += 0.025f;
        }

        altitude = (this.transform.position.y - ground_position) * 50;

        currrent_speed = (rb2D.velocity.magnitude * 100);

        if (is_frozen)
        {
            engine_trail.emitting = false;
            lwing_trail.emitting = false;
            rwing_trail.emitting = false;
            this.rb2D.gravityScale = 1.5f;
            return;
        }

        //Apply lift forces to the ship based on its angle
        ApplyLift();

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
            FindObjectOfType<AudioManager>().Play("Accelerate");
            Accelerate();
        }

        else if (s_intent == Intent.BRAKE)
        {
            FindObjectOfType<AudioManager>().Play("Brake");
            Brake();
        }
        else if (s_intent == Intent.IDLE)
        {
            FindObjectOfType<AudioManager>().Stop("Accelerate");
            FindObjectOfType<AudioManager>().Stop("Brake");
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

    void CheckGamepadInput()
    {
        if (game_pad.rightTrigger.isPressed) { s_intent = Intent.ACCELERATE; }

        else if (game_pad.leftTrigger.isPressed) { s_intent = Intent.BRAKE; }

        else { s_intent = Intent.IDLE; }


        if (game_pad.dpad.right.isPressed) { r_intent = Intent.PITCHRIGHT; }

        else if (game_pad.dpad.left.isPressed) { r_intent = Intent.PITCHLEFT; }

        else { r_intent = Intent.IDLE; }
    }

    void CheckKeyboardInput()
    {
        if (Input.GetKey(accelerate_key)) { s_intent = Intent.ACCELERATE; }

        else if (Input.GetKey(brake_key)) { s_intent = Intent.BRAKE; }

        else { s_intent = Intent.IDLE; }


        if (Input.GetKey(pitch_right_key)) { r_intent = Intent.PITCHRIGHT; }

        else if (Input.GetKey(pitch_left_key)) { r_intent = Intent.PITCHLEFT; }

        else { r_intent = Intent.IDLE; }
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

    void MaintainUpright()
    {
        if (rb2D.velocity.magnitude > 3)
        {
            //If speed is high enough the plane has too much momentum to flip over
            if (frames_spent_upside_down > 0)
            {
                frames_spent_upside_down--;
            }
            
            return;
        }



        if (curr_angle >= 160 && curr_angle <= 200)
        {

            if (gameObject.transform.localScale.y > 0) //Flip the sprite if it isn't facing the correct direction.
            {

                frames_spent_upside_down++;

                if (frames_spent_upside_down > 30)
                {
                    Roll();
                    FlipSprite();
                    frames_spent_upside_down = 0;
                }

            }

        }

        if ((curr_angle >= 0 && curr_angle <= 20) || (curr_angle >= 340 && curr_angle <= 360))
        {

            if (gameObject.transform.localScale.y < 0) //Flip the sprite if it isn't facing the correct direction.
            {
                frames_spent_upside_down++;

                if (frames_spent_upside_down > 30)
                {
                    Roll();
                    FlipSprite();
                    frames_spent_upside_down = 0;
                }
            }

        }
    }

    void FlipSprite()
    {
        this.gameObject.transform.localScale = Vector3.Scale(this.gameObject.transform.localScale, new Vector3(1, -1, 1));
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
        FindObjectOfType<AudioManager>().Play("Freeze");
        is_frozen = true;
        animator.SetBool("is_frozen", true);
    }

    void Roll()
    {
        animator.SetTrigger("Roll");
    }

    void DestroyShip()
    {
        FindObjectOfType<AudioManager>().Play("Crash");
        level_loader_script.LoadLevel("GameOver");
        Destroy(gameObject);
    }
}
