using UnityEngine;

public class PlayerBorder : MonoBehaviour
{
    public float rightEdge = 8.39f; // Right edge position where the sphere resets
    public float leftEdge = -8.39f;
    public float topEdge = 4.39f; // Top edge position where the sphere bounces back
    public float bottomEdge = -4.39f; // Bottom edge position where the sphere bounces
    private Vector2 initialPosition = new Vector2(-3f, 0f); // Always start at x = -3
    private Rigidbody2D rb;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Get the Rigidbody2D
        transform.position = initialPosition; // Ensure the sphere starts at the left side
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x <= leftEdge && rb.linearVelocity.x < 0)
        {
            ReverseHorizontalDirection();
        }

        if (transform.position.x >= rightEdge && rb.linearVelocity.x > 0)
        {
            ResetSpherePosition();
        }

        if (transform.position.y <= bottomEdge && rb.linearVelocity.y < 0)
        {
            ReverseVerticalDirection();
        }

        if (transform.position.y >= topEdge && rb.linearVelocity.y > 0)
        {
            ReverseVerticalDirection();
        }
    }

    private void ReverseHorizontalDirection()
    {
        rb.linearVelocity = new Vector2(-rb.linearVelocity.x, rb.linearVelocity.y);
    }

    private void ReverseVerticalDirection()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, -rb.linearVelocity.y);
    }

    public void ResetSpherePosition()
    {
        transform.position = initialPosition;
        rb.linearVelocity = Vector2.zero;
    }
}
