using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public Animator animator;
    private Collider2D colldr;
    private Rigidbody2D rb2D;
    private float distance_to_ground;
    public KeyCode move_left;
    public KeyCode move_right;
    public KeyCode jump;
    public float movement_speed = 10f;
    public float jump_height;
    public bool is_grounded;
    //Used to prevent autojump by holding the jump button.
    public bool jump_on_cooldown;
    private enum Intent
    {
        Idle,
        Left,
        Right,
        Jump,
        Slam
    }

    private Intent x_intent;
    private Intent y_intent;

    private void Start()
    {
        rb2D = transform.GetComponent<Rigidbody2D>();
        colldr = transform.GetComponent<Collider2D>();
        distance_to_ground = colldr.bounds.extents.y + 0.1f;
    }


    // Update is called once per frame
    void Update()
    {

        //Set/Check the player's grounded status once per frame.
        is_grounded = IsGrounded();

        if (Input.GetKey(move_left))
        {
            animator.SetBool("is_running", true);
            x_intent = Intent.Left;
        }

        else if (Input.GetKey(move_right))
        {
            animator.SetBool("is_running", true);
            x_intent = Intent.Right;
        }

        else
        {
            animator.SetBool("is_running", false);
            x_intent = Intent.Idle;
        }
        //If the player presses the jump button, is grounded, and the jump button isn't being held
        if (Input.GetKey(jump) && IsGrounded() && !jump_on_cooldown)
        {
            y_intent = Intent.Jump;

        }
        //If the jump button is let go, remove the jump cooldown.
        else if (!Input.GetKey(jump) && jump_on_cooldown)
        {
            jump_on_cooldown = false;
            y_intent = Intent.Idle;
        }
        else
        {
            y_intent = Intent.Idle;
        }
    }


    private void FixedUpdate()
    {
        if (x_intent == Intent.Left)
        {
            if (gameObject.transform.localScale.x > 0) //Flip the sprite if it isn't facing the correct direction.
            {
                this.gameObject.transform.localScale = new Vector3(-1, 1, 1); 
            }
            rb2D.velocity = new Vector2(-movement_speed, rb2D.velocity.y); //Move the player to the left.
        }

        else if (x_intent == Intent.Right)
        {
            if (gameObject.transform.localScale.x < 0)
            {
                this.gameObject.transform.localScale = new Vector3(1, 1, 1); //Flip the sprite if it isn't facing the correct direction.
            }
            rb2D.velocity = new Vector2(+movement_speed, rb2D.velocity.y); //Move the player to the right.
        }

        else if (x_intent == Intent.Idle)
        {
            rb2D.velocity = new Vector2(0, rb2D.velocity.y); //If the player isn't touching a movement key, cancel out their horizontal momentum for crisper movement.
        }

        if (y_intent == Intent.Jump && IsGrounded())
        {
            rb2D.velocity = new Vector2(rb2D.velocity.x, jump_height); //Move the player up
            jump_on_cooldown = true;
        }
    }
    //TODO Implement function to check if player is touching the ground and use it to limit our jumping.
    bool IsGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up, distance_to_ground + 0.1f); //Send out a raycast below the player's feet.

        return (hit.collider != null); //If it hits something, we must be standing on a surface and therefore we are grounded.
        
    }
}
