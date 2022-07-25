using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private BoxCollider2D box_colldr;
    private Rigidbody2D rb2D;
    private CapsuleCollider2D cap_colldr;
    private float distance_to_ground;
    [SerializeField] private Animator animator;
    [SerializeField] private KeyCode move_left;
    [SerializeField] private KeyCode move_right;
    [SerializeField] private KeyCode jump;
    [SerializeField] private KeyCode down;
    [SerializeField] private float movement_speed = 10f;
    [SerializeField] private float jump_height;
    [SerializeField] private bool is_grounded;
    //Used to prevent autojump by holding the jump button.
    [SerializeField] private bool jump_on_cooldown;
    private enum Intent
    {
        Idle,
        Left,
        Right,
        Jump,
        Crouch,
        Slam
    }

    private Intent x_intent;
    private Intent y_intent;

    private void Start()
    {
        rb2D = transform.GetComponent<Rigidbody2D>();
        box_colldr = transform.GetComponent<BoxCollider2D>();
        cap_colldr = transform.GetComponent<CapsuleCollider2D>();
        distance_to_ground = box_colldr.bounds.extents.y + 0.1f;
    }


    // Update is called once per frame
    void Update()
    {

        //Set|Check the player's grounded status once per frame.
        is_grounded = IsGrounded();

        if (Input.GetKey(move_left)) { x_intent = Intent.Left; }

        else if (Input.GetKey(move_right)) { x_intent = Intent.Right; }

        else { ReturnToIdleX(); }

        if (Input.GetKey(jump) && is_grounded && !jump_on_cooldown) { y_intent = Intent.Jump; }

        else if (!Input.GetKey(jump) && jump_on_cooldown)
        {
            jump_on_cooldown = false;
            y_intent = Intent.Idle;
        }

        else if (Input.GetKey(down) && is_grounded) { Crouch(); }

        else { ReturnToIdleY(); }
    }


    private void FixedUpdate()
    {
        if (x_intent == Intent.Left) { MoveLeft(); }

        else if (x_intent == Intent.Right) { MoveRight(); }

        else if (x_intent == Intent.Idle) { HaltHorizontalMomentum(); }

        if (y_intent == Intent.Jump && IsGrounded()) { Jump(); }
    }
    bool IsGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up, distance_to_ground + 0.1f); //Send out a raycast below the player's feet.

        if (hit.collider != null)
        {
            //If it hits something, we must be standing on a surface and therefore we are grounded.
            if (animator.GetBool("is_jumping"))
            {
                animator.SetBool("is_jumping", false);

            }

            return true;
        }
        else
        {
            return false;
        }
    }

    void Crouch()
    {
        animator.SetBool("is_crouching", true);
        cap_colldr.offset = new Vector2(0, -0.35f);
        cap_colldr.size = new Vector2(1.3f, 2.1f);
    }

    void Jump()
    {
        animator.SetBool("is_jumping", true);
        rb2D.velocity = new Vector2(rb2D.velocity.x, jump_height); //Move the player up
        jump_on_cooldown = true;
    }

    void MoveRight()
    {
        if (gameObject.transform.localScale.x < 0)
        {
            this.gameObject.transform.localScale = new Vector3(1, 1, 1); //Flip the sprite if it isn't facing the correct direction.
        }
        animator.SetBool("is_running", true);
        rb2D.velocity = new Vector2(+movement_speed, rb2D.velocity.y); //Move the player to the right.
    }

    void MoveLeft()
    {
        if (gameObject.transform.localScale.x > 0) //Flip the sprite if it isn't facing the correct direction.
        {
            this.gameObject.transform.localScale = new Vector3(-1, 1, 1);
        }
        animator.SetBool("is_running", true);
        rb2D.velocity = new Vector2(-movement_speed, rb2D.velocity.y); //Move the player to the left.
    }

    void HaltHorizontalMomentum()
    {
        rb2D.velocity = new Vector2(0, rb2D.velocity.y); //If the player isn't touching a movement key, cancel out their horizontal momentum for crisper movement.

    }

    void ReturnToIdleY()
    {
        animator.SetBool("is_crouching", false);
        y_intent = Intent.Idle;
        cap_colldr.offset = new Vector2(0, 0);
        cap_colldr.size = new Vector2(1.3f, 2.8f);
    }

    void ReturnToIdleX()
    {
        animator.SetBool("is_running", false);
        x_intent = Intent.Idle;
    }
}
