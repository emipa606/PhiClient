using System;
using UnityEngine;
using Verse;

namespace PhiClient.UI;

internal class TextFieldWidget : Displayable
{
    public Action<string> onChange;

    public string text = "";

    public TextFieldWidget(string text, Action<string> onChange)
    {
        this.text = text;
        this.onChange = onChange;
    }

    public override void Draw(Rect inRect)
    {
        var text = Widgets.TextField(inRect, this.text);
        if (text != this.text)
        {
            onChange(text);
        }
    }
}