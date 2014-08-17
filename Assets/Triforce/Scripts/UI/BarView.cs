using UnityEngine;
using System.Collections;
using Vectrosity;

public class BarView : GameController {

    public string property;
    public string bgProperty;
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

    int BackgroundValue {
        get {
            return (int)player.GetProperty(bgProperty);
        }
    }
    int Value {
        get {
            return (int)player.GetProperty(property);
        }
    }

    // Use this for initialization
    void Start () {
        DrawBackground();
    }

    // Update is called once per frame
    void Update () {
        DrawBar();
    }

    void DrawBackground () {
        float y = Screen.height - yOffset;
        Vector2 start = new Vector2(xOffset, y);
        float x = (xOffset + BackgroundValue)*multiplier + 2 * margin;
        Vector2 end = new Vector2(x, y);
        bgLine = VectorLine.SetLine(bgColor, new Vector2[] { start, end });
        bgLine.lineWidth = bgThickness;
        bgLine.sortingOrder = 1;
        bgLine.Draw();
    }

    void DrawBar () {

        if (hpLine == null) {
            InitializeHPBar();
        }

        if (previousHP.HasValue && previousHP == Value) {
            return;
        }

        float x = (xOffset + Value)*multiplier + margin;
        hpLine.points2[1] = new Vector2(x, Screen.height - yOffset);
        hpLine.Draw();
        previousHP = Value;
    }

    void InitializeHPBar () {
        float y = Screen.height - yOffset;
        Vector2 start = new Vector2(xOffset + margin, y);
        float x = (xOffset + Value)*multiplier + margin;
        Vector2 end = new Vector2(x, y);
        hpLine = VectorLine.SetLine(hpColor, new Vector2[] { start, end });
        hpLine.lineWidth = bgThickness - (2 * margin);
        hpLine.sortingOrder = 2;
        hpLine.Draw();
    }
}
