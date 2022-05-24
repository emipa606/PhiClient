using UnityEngine;
using Verse;

namespace PhiClient.UI;

public class TextWidget : Displayable
{
    private readonly TextAnchor anchor;

    private readonly GameFont font;

    private readonly string text;

    public TextWidget(string text, GameFont font = GameFont.Small, TextAnchor anchor = TextAnchor.UpperLeft)
    {
        this.text = text;
        this.font = font;
        this.anchor = anchor;
    }

    public override float CalcHeight(float width)
    {
        SetStyle();
        var result = Text.CalcHeight(text, width);
        ClearStyle();
        return result;
    }

    public override void Draw(Rect inRect)
    {
        SetStyle();
        Widgets.Label(inRect, text);
        ClearStyle();
    }

    private void SetStyle()
    {
        Text.Anchor = anchor;
        Text.Font = font;
    }

    private void ClearStyle()
    {
        Text.Anchor = 0;
        Text.Font = GameFont.Small;
    }

    public override bool IsFluidHeight()
    {
        return false;
    }
}