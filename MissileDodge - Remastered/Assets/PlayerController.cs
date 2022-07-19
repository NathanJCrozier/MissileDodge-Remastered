using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb2D;
    public KeyCode move_left;
    public KeyCode move_right;
    public KeyCode jump;
    public float movement_speed = 10f;
    public float jump_height;

    private void Start()
    {
        rb2D = transform.GetComponent<Rigidbody2D>();
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(move_left))
        {
            rb2D.velocity = new Vector2(-movement_speed, rb2D.velocity.y);
        }

        else if (Input.GetKey(move_right))
        {
            rb2D.velocity = new Vector2(+movement_speed, rb2D.velocity.y);
        }

        else
        {
            rb2D.velocity = new Vector2(0, rb2D.velocity.y);
        }

        if (Input.GetKey(jump) && IsGrounded())
        {
            rb2D.velocity = new Vector2(rb2D.velocity.x, jump_height);
        }
    }
    //TODO Implement function to check if player is touching the ground and use it to limit our jumping.
    bool IsGrounded()
    {
        return true;
    }
}
