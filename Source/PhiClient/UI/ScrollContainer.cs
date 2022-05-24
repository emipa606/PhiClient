using System;
using UnityEngine;
using Verse;

namespace PhiClient.UI;

internal class ScrollContainer : Displayable
{
    private const float SCROLL_BAR_SIZE = 16f;

    private readonly Displayable child;

    private readonly Action<Vector2> onScroll;

    private Vector2 scrollPosition = Vector2.zero;

    public ScrollContainer(Displayable child, Vector2 scrollPosition, Action<Vector2> onScroll)
    {
        this.scrollPosition = scrollPosition;
        this.child = child;
        this.onScroll = onScroll;
    }

    public override void Draw(Rect inRect)
    {
        var rect = inRect.LeftPartPixels(inRect.width - 16f);
        var width = rect.width;
        var num = child.CalcHeight(rect.width);
        if (num == -1f)
        {
            num = rect.height;
        }

        var rect2 = new Rect(0f, 0f, width, num);
        Widgets.BeginScrollView(inRect, ref scrollPosition, rect2);
        onScroll(scrollPosition);
        child.Draw(rect2);
        Widgets.EndScrollView();
    }
}