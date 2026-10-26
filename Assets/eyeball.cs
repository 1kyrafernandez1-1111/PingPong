using UnityEngine;

public class eyeball : MonoBehaviour
{
    public Vector2 launchVelocity = new Vector2(7.5f, 3.5f);

    Rigidbody2D body;
    float cruiseSpeed;
    float nextScoreTime;

    void Awake()
    {
        // No gravity anywhere in this game. A bounce threshold of 0
        // means even a slow hit still rebounds.
        Physics2D.gravity = Vector2.zero;
        Physics2D.bounceThreshold = 0f;

        body = GetComponent<Rigidbody2D>();
        body.gravityScale = 0f;
        body.linearDamping = 0f;
        body.angularDamping = 0f;
        body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        body.sleepMode = RigidbodySleepMode2D.NeverSleep;
    }

    void Start()
    {
        cruiseSpeed = launchVelocity.magnitude;
        body.linearVelocity = launchVelocity;
    }

    void FixedUpdate()
    {
        Vector2 velocity = body.linearVelocity;
        float speed = velocity.magnitude;
        if (speed < 0.01f)
        {
            body.linearVelocity = launchVelocity;
            return;
        }

        // Bounciness does the rebounding. This only undoes tiny solver
        // drift so a ball never stalls, and caps speed after a hard paddle hit.
        if (speed < cruiseSpeed * 0.92f)
            body.linearVelocity = velocity / speed * cruiseSpeed;
        else if (speed > cruiseSpeed * 1.45f)
            body.linearVelocity = velocity / speed * (cruiseSpeed * 1.25f);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (Time.time < nextScoreTime)
            return;

        scoreboard board = FindAnyObjectByType<scoreboard>();
        if (board == null)
            return;

        if (collision.gameObject.name == "WallLeft")
        {
            board.PointForRight();
            nextScoreTime = Time.time + 0.45f;
        }
        else if (collision.gameObject.name == "WallRight")
        {
            board.PointForLeft();
            nextScoreTime = Time.time + 0.45f;
        }
    }
}
