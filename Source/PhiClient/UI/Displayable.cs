using UnityEngine;

namespace PhiClient.UI
{
    // Token: 0x0200000C RID: 12
    public abstract class Displayable
    {
        // Token: 0x04000030 RID: 48
        public const float FLUID = -1f;

        // Token: 0x06000066 RID: 102
        public abstract void Draw(Rect inRect);

        // Token: 0x06000067 RID: 103 RVA: 0x00003D0E File Offset: 0x00001F0E
        public virtual bool IsFluidWidth()
        {
            return true;
        }

        // Token: 0x06000068 RID: 104 RVA: 0x00003D0E File Offset: 0x00001F0E
        public virtual bool IsFluidHeight()
        {
            return true;
        }

        // Token: 0x06000069 RID: 105 RVA: 0x00003D11 File Offset: 0x00001F11
        public virtual float CalcWidth(float height)
        {
            return -1f;
        }

        // Token: 0x0600006A RID: 106 RVA: 0x00003D11 File Offset: 0x00001F11
        public virtual float CalcHeight(float width)
        {
            return -1f;
        }
    }
}