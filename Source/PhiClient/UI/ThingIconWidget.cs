using UnityEngine;
using Verse;

namespace PhiClient.UI;

internal class ThingIconWidget : Displayable
{
    public Thing thing;

    public ThingIconWidget(Thing thing)
    {
        this.thing = thing;
    }

    public override void Draw(Rect inRect)
    {
        Widgets.ThingIcon(inRect, thing);
    }
}