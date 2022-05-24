using UnityEngine;
using Verse;

namespace PhiClient.UI;

internal class Container : Displayable
{
    private readonly Displayable child;

    private readonly float height;

    private readonly float width;

    public Container(Displayable child, float width, float height)
    {
        this.child = child;
        this.width = width;
        this.height = height;
    }

    public override float CalcHeight(float width)
    {
        return height;
    }

    public override float CalcWidth(float height)
    {
        return width;
    }

    public override void Draw(Rect inRect)
    {
        if (!IsFluidHeight())
        {
            inRect = inRect.TopPartPixels(height);
        }

        if (!IsFluidWidth())
        {
            inRect = inRect.LeftPartPixels(width);
        }

        child.Draw(inRect);
    }

    public override bool IsFluidHeight()
    {
        return height == -1f;
    }

    public override bool IsFluidWidth()
    {
        return width == -1f;
    }
}