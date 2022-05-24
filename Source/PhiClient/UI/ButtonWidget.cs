using System;
using UnityEngine;
using Verse;

namespace PhiClient.UI;

internal class ButtonWidget : Displayable
{
    public Action clickAction;

    public bool drawBackground;

    public string label;

    public ButtonWidget(string label, Action clickAction, bool drawBackground = true)
    {
        this.label = label;
        this.drawBackground = drawBackground;
        this.clickAction = clickAction;
    }

    public override void Draw(Rect inRect)
    {
        if (Widgets.ButtonText(inRect, label, drawBackground, false))
        {
            clickAction();
        }
    }

    public override bool IsFluidHeight()
    {
        return drawBackground;
    }

    public override float CalcHeight(float width)
    {
        return Text.CalcHeight(label, width);
    }
}