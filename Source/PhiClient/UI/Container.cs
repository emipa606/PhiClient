using UnityEngine;
using Verse;

namespace PhiClient.UI
{
    // Token: 0x0200000B RID: 11
    internal class Container : Displayable
    {
        // Token: 0x0400002F RID: 47
        private readonly Displayable child;

        // Token: 0x0400002E RID: 46
        private readonly float height;

        // Token: 0x0400002D RID: 45
        private readonly float width;

        // Token: 0x06000060 RID: 96 RVA: 0x00003C89 File Offset: 0x00001E89
        public Container(Displayable child, float width, float height)
        {
            this.child = child;
            this.width = width;
            this.height = height;
        }

        // Token: 0x06000061 RID: 97 RVA: 0x00003CA6 File Offset: 0x00001EA6
        public override float CalcHeight(float width)
        {
            return height;
        }

        // Token: 0x06000062 RID: 98 RVA: 0x00003CAE File Offset: 0x00001EAE
        public override float CalcWidth(float height)
        {
            return width;
        }

        // Token: 0x06000063 RID: 99 RVA: 0x00003CB6 File Offset: 0x00001EB6
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

        // Token: 0x06000064 RID: 100 RVA: 0x00003CF0 File Offset: 0x00001EF0
        public override bool IsFluidHeight()
        {
            return height == -1f;
        }

        // Token: 0x06000065 RID: 101 RVA: 0x00003CFF File Offset: 0x00001EFF
        public override bool IsFluidWidth()
        {
            return width == -1f;
        }
    }
}