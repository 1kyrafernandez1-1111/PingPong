using UnityEngine;
using UnityEngine.UI;

public class scoreboard : MonoBehaviour
{
    public int leftScore;
    public int rightScore;

    Text leftLabel;
    Text rightLabel;

    void Awake()
    {
        Build();
        Refresh();
    }

    public void PointForLeft()
    {
        leftScore++;
        Refresh();
    }

    public void PointForRight()
    {
        rightScore++;
        Refresh();
    }

    void Refresh()
    {
        if (leftLabel != null)
            leftLabel.text = leftScore.ToString();
        if (rightLabel != null)
            rightLabel.text = rightScore.ToString();
    }

    void Build()
    {
        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        GameObject canvasObject = new GameObject("ScoreCanvas");
        canvasObject.transform.SetParent(transform, false);
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 50;
        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);

        Color scoreRed = new Color(0.96f, 0.12f, 0.12f);
        leftLabel = MakeLabel(canvasObject.transform, font, new Vector2(0.22f, 0.9f), scoreRed);
        rightLabel = MakeLabel(canvasObject.transform, font, new Vector2(0.78f, 0.9f), scoreRed);
    }

    static Text MakeLabel(Transform parent, Font font, Vector2 anchor, Color color)
    {
        GameObject labelObject = new GameObject("Score");
        labelObject.transform.SetParent(parent, false);
        RectTransform rect = labelObject.AddComponent<RectTransform>();
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(280f, 160f);

        Text label = labelObject.AddComponent<Text>();
        label.font = font;
        label.fontSize = 90;
        label.alignment = TextAnchor.MiddleCenter;
        label.color = color;
        label.horizontalOverflow = HorizontalWrapMode.Overflow;
        label.verticalOverflow = VerticalWrapMode.Overflow;

        Outline outline = labelObject.AddComponent<Outline>();
        outline.effectColor = new Color(0f, 0f, 0f, 0.9f);
        outline.effectDistance = new Vector2(3f, -3f);
        return label;
    }
}
