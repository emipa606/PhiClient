using System;
using UnityEngine;
using Verse;

namespace PhiClient.UI;

internal class CheckboxLabeledWidget : Displayable
{
    private const float CHECKBOX_SIZE = 40f;

    private readonly string label;

    private readonly Action<bool> onChange;

    private bool checkedOn;

    public CheckboxLabeledWidget(string label, bool checkedOn, Action<bool> onChange)
    {
        this.label = label;
        this.checkedOn = checkedOn;
        this.onChange = onChange;
    }

    public override void Draw(Rect inRect)
    {
        var preCheck = checkedOn;
        Widgets.CheckboxLabeled(inRect, label, ref checkedOn);
        if (preCheck != checkedOn)
        {
            onChange(checkedOn);
        }
    }

    public override bool IsFluidHeight()
    {
        return false;
    }

    public override float CalcHeight(float width)
    {
        return 40f;
    }
}