using UnityEngine;
using Verse;

namespace PhiClient.UI
{
    // Token: 0x02000016 RID: 22
    internal class ThingIconWidget : Displayable
    {
        // Token: 0x0400004C RID: 76
        public Thing thing;

        // Token: 0x0600008B RID: 139 RVA: 0x000044B7 File Offset: 0x000026B7
        public ThingIconWidget(Thing thing)
        {
            this.thing = thing;
        }

        // Token: 0x0600008C RID: 140 RVA: 0x000044C6 File Offset: 0x000026C6
        public override void Draw(Rect inRect)
        {
            Widgets.ThingIcon(inRect, thing);
        }
    }
}