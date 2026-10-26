using UnityEngine;
using UnityEngine.InputSystem;

public class paddleit : MonoBehaviour
{
    public Key upKey = Key.W;
    public Key downKey = Key.S;
    public float speed = 9f;

    // -1 is the left paddle, 1 is the right paddle.
    public int side = -1;

    // Room behind the paddle so a ball can pass and hit the wall.
    public float ballClearance = 1.35f;
    public AudioClip tapClip;

    Rigidbody2D body;
    AudioSource tapSource;
    float minY;
    float maxY;

    void Awake()
    {
        tapSource = GetComponent<AudioSource>();
        if (tapSource == null)
            tapSource = gameObject.AddComponent<AudioSource>();
        tapSource.playOnAwake = false;
        tapSource.spatialBlend = 0f;
    }

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        PlaceInsideCourt();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (tapClip == null || tapSource == null)
            return;
        if (collision.collider.GetComponent<eyeball>() == null)
            return;
        tapSource.PlayOneShot(tapClip);
    }

    public void PlaceInsideCourt()
    {
        if (body == null)
            body = GetComponent<Rigidbody2D>();

        Camera cam = Camera.main;
        if (cam == null || body == null)
            return;
        float halfScreen = cam.orthographicSize;
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        float halfPaddleY = sprite.bounds.extents.y;
        float halfPaddleX = sprite.bounds.extents.x;

        float topLimit = halfScreen - halfPaddleY;
        float bottomLimit = -halfScreen + halfPaddleY;

        Collider2D topWall = Wall("WallTop");
        Collider2D bottomWall = Wall("WallBottom");
        if (topWall != null)
            topLimit = Mathf.Min(topLimit, topWall.bounds.min.y - halfPaddleY);
        if (bottomWall != null)
            bottomLimit = Mathf.Max(bottomLimit, bottomWall.bounds.max.y + halfPaddleY);

        maxY = topLimit;
        minY = bottomLimit;

        float x = body.position.x;
        Collider2D sideWall = Wall(side < 0 ? "WallLeft" : "WallRight");
        if (sideWall != null)
        {
            if (side < 0)
                x = sideWall.bounds.max.x + halfPaddleX + ballClearance;
            else
                x = sideWall.bounds.min.x - halfPaddleX - ballClearance;
        }

        float y = Mathf.Clamp(body.position.y, minY, maxY);
        body.position = new Vector2(x, y);
    }

    void FixedUpdate()
    {
        float input = 0f;
        Keyboard keyboard = Keyboard.current;
        if (keyboard != null)
        {
            if (keyboard[upKey].isPressed)
                input += 1f;
            if (keyboard[downKey].isPressed)
                input -= 1f;
        }

        Vector2 pos = body.position;
        pos.y += input * speed * Time.fixedDeltaTime;
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        body.MovePosition(pos);
    }

    static Collider2D Wall(string wallName)
    {
        GameObject wall = GameObject.Find(wallName);
        if (wall == null)
            return null;
        return wall.GetComponent<Collider2D>();
    }
}
