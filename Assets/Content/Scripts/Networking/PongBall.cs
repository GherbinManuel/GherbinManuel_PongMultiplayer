using Mirror;
using UnityEngine;

public class PongBall : NetworkBehaviour
{

    public float speed = 1500;
    public Rigidbody2D rigidbody2d;

    private Vector3 SetMovementDirection
    {
        set
        {
            Vector3 vel = speed * Time.deltaTime * value;

#if UNITY_6000_0_OR_NEWER
            rigidbody2d.linearVelocity = vel;
#else
            rigidbody2d.velocity = vel;
#endif
        }
    }
    public override void OnStartServer()
    {
        base.OnStartServer();

        speed = 1500;

        rigidbody2d.simulated = true;

        SetMovementDirection = Vector3.right;
    }

    [ServerCallback]
    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.CompareTag("Goal"))
        {
            ((PongNetworkManager)PongNetworkManager.singleton).OnGoal();
            return;
        }

        if (collision.gameObject.CompareTag("PlayerBounce"))
        {
            Bounce(collision);
            return;
        }

        Debug.LogWarning($"Hit a worng layer {collision.gameObject}");
    }

    float HitFactor(Vector2 ballPos, Vector2 racketPos, float racketHeight)
    {
        // ascii art:
        // ||  1 <- at the top of the racket
        // ||
        // ||  0 <- at the middle of the racket
        // ||
        // || -1 <- at the bottom of the racket
        return (ballPos.y - racketPos.y) / racketHeight;
    }

    private void Bounce(Collision2D collision)
    {
            // Calculate y direction via hit Factor
            float y = HitFactor(transform.position,
                                collision.transform.position,
                                collision.collider.bounds.size.y);

            // Calculate x direction via opposite collision
            float x = collision.relativeVelocity.x > 0 ? 1 : -1;

            // Calculate direction, make length=1 via .normalized
            Vector2 dir = new Vector2(x, y).normalized;

        // Set Velocity with dir * speed

        SetMovementDirection = dir;
        
    }
}
