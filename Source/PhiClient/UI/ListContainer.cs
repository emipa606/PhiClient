using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace PhiClient.UI;

[StaticConstructorOnStartup]
internal class ListContainer : Displayable
{
    public const float SPACE = 10f;

    public static readonly Texture2D alternateBackground =
        SolidColorMaterials.NewSolidColorTexture(new Color(1f, 1f, 1f, 0.04f));

    private readonly List<Displayable> children;

    private readonly ListDirection direction;

    private readonly ListFlow flow;

    public bool drawAlternateBackground;

    public float spaceBetween;

    public ListContainer(List<Displayable> children, ListFlow flow = ListFlow.COLUMN,
        ListDirection direction = ListDirection.NORMAL)
    {
        this.children = children;
        this.flow = flow;
        this.direction = direction;
    }

    public ListContainer() : this(new List<Displayable>())
    {
    }

    public ListContainer(ListFlow flow = ListFlow.COLUMN, ListDirection direction = ListDirection.NORMAL) : this(
        new List<Displayable>(), flow, direction)
    {
    }

    public void Add(Displayable display)
    {
        children.Add(display);
    }

    public override float CalcHeight(float width)
    {
        if (IsFluidHeight())
        {
            return -1f;
        }

        return children.Sum(c => c.CalcHeight(width)) + ((children.Count - 1) * spaceBetween);
    }

    public override float CalcWidth(float height)
    {
        if (IsFluidWidth())
        {
            return -1f;
        }

        return children.Sum(c => c.CalcWidth(height));
    }

    public override void Draw(Rect inRect)
    {
        GUI.BeginGroup(inRect);
        if (flow == ListFlow.COLUMN)
        {
            DrawColumn(inRect);
        }
        else
        {
            DrawRow(inRect);
        }

        GUI.EndGroup();
    }

    private void DrawRow(Rect inRect)
    {
        var height = inRect.height;
        var countFluidElements = 0;
        var num = !IsFluidWidth()
            ? inRect.width
            : children.Sum(delegate(Displayable c)
            {
                var num6 = c.CalcWidth(height);
                if (num6 != -1f)
                {
                    return num6;
                }

                countFluidElements++;
                return 0f;
            });
        var num2 = inRect.width - num;
        num2 -= (children.Count - 1) * spaceBetween;
        num2 /= countFluidElements;
        var num3 = 0f;
        if (direction == ListDirection.OPPOSITE)
        {
            num3 += inRect.width;
        }

        var num4 = 0;
        foreach (var displayable in children)
        {
            var num5 = displayable.CalcWidth(height);
            if (num5 == -1f)
            {
                num5 = num2;
            }

            if (direction == ListDirection.OPPOSITE)
            {
                num3 -= num5 + spaceBetween;
            }

            var rect = new Rect(num3, 0f, num5, height);
            GUI.BeginGroup(rect);
            rect.x = 0f;
            if (drawAlternateBackground && num4 % 2 == 1)
            {
                GUI.DrawTexture(rect, alternateBackground);
            }

            displayable.Draw(rect);
            GUI.EndGroup();
            if (direction == ListDirection.NORMAL)
            {
                num3 += num5 + spaceBetween;
            }

            num4++;
        }
    }

    private void DrawColumn(Rect inRect)
    {
        var width = inRect.width;
        var countFluidElements = 0;
        var num = !IsFluidWidth()
            ? inRect.height
            : children.Sum(delegate(Displayable c)
            {
                var num6 = c.CalcHeight(width);
                if (num6 != -1f)
                {
                    return num6;
                }

                countFluidElements++;
                return 0f;
            });
        var num2 = inRect.height - num;
        num2 -= (children.Count - 1) * spaceBetween;
        num2 /= countFluidElements;
        var num3 = 0f;
        if (direction == ListDirection.OPPOSITE)
        {
            num3 += inRect.height;
        }

        var num4 = 0;
        foreach (var displayable in children)
        {
            var num5 = displayable.CalcHeight(width);
            if (num5 == -1f)
            {
                num5 = num2;
            }

            if (direction == ListDirection.OPPOSITE)
            {
                num3 -= num5 + spaceBetween;
            }

            var rect = new Rect(0f, num3, width, num5);
            GUI.BeginGroup(rect);
            rect.y = 0f;
            if (drawAlternateBackground && num4 % 2 == 1)
            {
                GUI.DrawTexture(rect, alternateBackground);
            }

            displayable.Draw(rect);
            GUI.EndGroup();
            if (direction == ListDirection.NORMAL)
            {
                num3 += num5 + spaceBetween;
            }

            num4++;
        }
    }

    private int CountFluidHeight()
    {
        return children.Count(c => c.IsFluidHeight());
    }

    private int CountFluidWidth()
    {
        return children.Count(c => c.IsFluidWidth());
    }

    public override bool IsFluidHeight()
    {
        return flow != ListFlow.COLUMN || children.Any(c => c.IsFluidHeight());
    }

    public override bool IsFluidWidth()
    {
        return flow != ListFlow.ROW || children.Any(c => c.IsFluidWidth());
    }
}