using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileBehavior : MonoBehaviour
{

    private Rigidbody2D rb2D;

    public Transform target;

    public float speed;

    public float rotation_speed;


    // Start is called before the first frame update
    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 direction = (Vector2)target.position - rb2D.position;

        direction.Normalize();

        float rotate_amount = Vector3.Cross(direction, transform.right).z;

        rb2D.angularVelocity = -rotate_amount * rotation_speed;

        rb2D.velocity = transform.right * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Destroy(target.gameObject);
            Destroy(this.gameObject);
        }
        
    }
}
