using UnityEngine;
using System.Collections;
using Vectrosity;

public class ManaView : GameController {

    public float xOffset;
    public float yOffset;
    public float bgThickness;
    public float multiplier;
    public float margin;

    public Color bgColor;
    public Color hpColor;

    VectorLine bgLine;
    VectorLine hpLine;
    int? previousHP;

    // Use this for initialization
    void Start () {
        DrawMaxBar();
    }

    // Update is called once per frame
    void Update () {
    }

    void DrawHitPoints () {
    }

    void DrawMaxBar () {
        float y = Screen.height - yOffset;
        Vector2 start = new Vector2(xOffset, y);
        float x = xOffset + player.MaxHitPoints + 2 * margin;
        Vector2 end = new Vector2(x*multiplier, y);
        bgLine = VectorLine.SetLine(bgColor, new Vector2[] { start, end });
        bgLine.lineWidth = bgThickness;
        bgLine.sortingOrder = 1;
        bgLine.Draw();
    }

    void DrawHPBar () {

        if (hpLine == null) {
            InitializeHPBar();
        }

        if (previousHP.HasValue && previousHP == player.HitPoints) {
            return;
        }

        float x = xOffset + margin + player.HitPoints;
        hpLine.points2[1] = new Vector2(x*multiplier, Screen.height - yOffset);
        hpLine.Draw();
        previousHP = player.HitPoints;
    }

    void InitializeHPBar () {
        float y = Screen.height - yOffset;
        Vector2 start = new Vector2(xOffset + margin, y);
        float x = xOffset + margin + player.HitPoints;
        Vector2 end = new Vector2(x*multiplier, y);
        hpLine = VectorLine.SetLine(hpColor, new Vector2[] { start, end });
        hpLine.lineWidth = bgThickness - (2 * margin);
        hpLine.sortingOrder = 2;
        hpLine.Draw();
    }
}
