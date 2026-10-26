using UnityEngine;

public class court : MonoBehaviour
{
    [Header("Walls")]
    public float wallThickness = 0.4f;
    public Color topColor = new Color(0.62f, 0.08f, 0.4f);
    public Color rightColor = new Color(0.72f, 0.28f, 0f);
    public Color bottomColor = new Color(0.62f, 0.08f, 0.4f);
    public Color leftColor = new Color(0.72f, 0.28f, 0f);
    public Color leftPaddleColor = new Color(0.05f, 0.5f, 0.72f);
    public Color rightPaddleColor = new Color(0.05f, 0.5f, 0.72f);

    float appliedThickness = -1f;
    Color appliedTop;
    Color appliedRight;
    Color appliedBottom;
    Color appliedLeft;
    Color appliedLeftPaddle;
    Color appliedRightPaddle;
    float appliedHalfW;
    float appliedHalfH;
    bool applied;

    void Awake()
    {
        Physics2D.gravity = Vector2.zero;
        Physics2D.bounceThreshold = 0f;
        ApplyWalls();
    }

    void LateUpdate()
    {
        Camera cam = Camera.main;
        if (cam == null)
            return;

        float halfH = cam.orthographicSize;
        float halfW = halfH * cam.aspect;
        if (!applied || ColorsChanged() || wallThickness != appliedThickness
            || Mathf.Abs(halfW - appliedHalfW) > 0.001f
            || Mathf.Abs(halfH - appliedHalfH) > 0.001f)
            ApplyWalls();
    }

    void OnValidate()
    {
        if (wallThickness < 0.05f)
            wallThickness = 0.05f;
        ApplyWalls();
    }

    public void FitToCamera()
    {
        ApplyWalls();
    }

    public void ApplyWalls()
    {
        Camera cam = Camera.main;
        if (cam == null)
            return;

        float halfH = cam.orthographicSize;
        float halfW = halfH * cam.aspect;
        float thickness = Mathf.Clamp(wallThickness, 0.05f, halfH * 0.75f);

        FitWall("WallTop", new Vector2(0f, halfH - thickness * 0.5f), new Vector2(halfW * 2f, thickness), topColor);
        FitWall("WallBottom", new Vector2(0f, -halfH + thickness * 0.5f), new Vector2(halfW * 2f, thickness), bottomColor);
        FitWall("WallLeft", new Vector2(-halfW + thickness * 0.5f, 0f), new Vector2(thickness, halfH * 2f - thickness * 2f), leftColor);
        FitWall("WallRight", new Vector2(halfW - thickness * 0.5f, 0f), new Vector2(thickness, halfH * 2f - thickness * 2f), rightColor);

        GameObject background = GameObject.Find("bkgrd_0");
        if (background != null)
        {
            SpriteRenderer sprite = background.GetComponent<SpriteRenderer>();
            if (sprite != null && sprite.sprite != null)
            {
                Vector2 spriteSize = sprite.sprite.bounds.size;
                background.transform.position = Vector3.zero;
                background.transform.localScale = new Vector3(
                    (halfW * 2.04f) / spriteSize.x,
                    (halfH * 2.04f) / spriteSize.y,
                    1f);
                sprite.sortingOrder = -20;
            }
        }

        appliedThickness = wallThickness;
        appliedTop = topColor;
        appliedRight = rightColor;
        appliedBottom = bottomColor;
        appliedLeft = leftColor;
        appliedLeftPaddle = leftPaddleColor;
        appliedRightPaddle = rightPaddleColor;
        appliedHalfW = halfW;
        appliedHalfH = halfH;
        applied = true;

        Physics2D.SyncTransforms();

        paddleit[] paddles = FindObjectsByType<paddleit>(FindObjectsSortMode.None);
        for (int i = 0; i < paddles.Length; i++)
        {
            SpriteRenderer sprite = paddles[i].GetComponent<SpriteRenderer>();
            if (sprite != null)
                sprite.color = paddles[i].side < 0 ? leftPaddleColor : rightPaddleColor;
            paddles[i].PlaceInsideCourt();
        }
    }

    bool ColorsChanged()
    {
        return topColor != appliedTop || rightColor != appliedRight
            || bottomColor != appliedBottom || leftColor != appliedLeft
            || leftPaddleColor != appliedLeftPaddle
            || rightPaddleColor != appliedRightPaddle;
    }

    void FitWall(string wallName, Vector2 center, Vector2 size, Color color)
    {
        GameObject wall = GameObject.Find(wallName);
        if (wall == null)
            return;

        wall.transform.position = new Vector3(center.x, center.y, 0f);

        SpriteRenderer sprite = wall.GetComponent<SpriteRenderer>();
        Vector2 native = Vector2.one;
        if (sprite != null && sprite.sprite != null)
            native = sprite.sprite.bounds.size;

        wall.transform.localScale = new Vector3(size.x / native.x, size.y / native.y, 1f);

        if (sprite != null)
        {
            sprite.drawMode = SpriteDrawMode.Simple;
            sprite.color = color;
            sprite.sortingOrder = 2;
            sprite.enabled = true;
        }

        BoxCollider2D box = wall.GetComponent<BoxCollider2D>();
        if (box != null)
        {
            box.size = native;
            box.offset = sprite != null && sprite.sprite != null ? (Vector2)sprite.sprite.bounds.center : Vector2.zero;
        }
    }
}
