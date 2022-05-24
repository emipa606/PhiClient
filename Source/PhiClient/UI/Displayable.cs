using UnityEngine;

namespace PhiClient.UI;

public abstract class Displayable
{
    public const float FLUID = -1f;

    public abstract void Draw(Rect inRect);

    public virtual bool IsFluidWidth()
    {
        return true;
    }

    public virtual bool IsFluidHeight()
    {
        return true;
    }

    public virtual float CalcWidth(float height)
    {
        return -1f;
    }

    public virtual float CalcHeight(float width)
    {
        return -1f;
    }
}