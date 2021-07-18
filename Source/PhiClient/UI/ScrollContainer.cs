using System;
using UnityEngine;
using Verse;

namespace PhiClient.UI
{
    // Token: 0x02000011 RID: 17
    internal class ScrollContainer : Displayable
    {
        // Token: 0x0400003E RID: 62
        private const float SCROLL_BAR_SIZE = 16f;

        // Token: 0x0400003F RID: 63
        private readonly Displayable child;

        // Token: 0x04000040 RID: 64
        private readonly Action<Vector2> onScroll;

        // Token: 0x04000041 RID: 65
        private Vector2 scrollPosition = Vector2.zero;

        // Token: 0x0600007C RID: 124 RVA: 0x0000421A File Offset: 0x0000241A
        public ScrollContainer(Displayable child, Vector2 scrollPosition, Action<Vector2> onScroll)
        {
            this.scrollPosition = scrollPosition;
            this.child = child;
            this.onScroll = onScroll;
        }

        // Token: 0x0600007D RID: 125 RVA: 0x00004244 File Offset: 0x00002444
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
}