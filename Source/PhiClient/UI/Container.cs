using System;
using UnityEngine;
using Verse;

namespace PhiClient.UI
{
	// Token: 0x0200000B RID: 11
	internal class Container : Displayable
	{
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
			return this.height;
		}

		// Token: 0x06000062 RID: 98 RVA: 0x00003CAE File Offset: 0x00001EAE
		public override float CalcWidth(float height)
		{
			return this.width;
		}

		// Token: 0x06000063 RID: 99 RVA: 0x00003CB6 File Offset: 0x00001EB6
		public override void Draw(Rect inRect)
		{
			if (!this.IsFluidHeight())
			{
				inRect = GenUI.TopPartPixels(inRect, this.height);
			}
			if (!this.IsFluidWidth())
			{
				inRect = GenUI.LeftPartPixels(inRect, this.width);
			}
			this.child.Draw(inRect);
		}

		// Token: 0x06000064 RID: 100 RVA: 0x00003CF0 File Offset: 0x00001EF0
		public override bool IsFluidHeight()
		{
			return this.height == -1f;
		}

		// Token: 0x06000065 RID: 101 RVA: 0x00003CFF File Offset: 0x00001EFF
		public override bool IsFluidWidth()
		{
			return this.width == -1f;
		}

		// Token: 0x0400002D RID: 45
		private float width;

		// Token: 0x0400002E RID: 46
		private float height;

		// Token: 0x0400002F RID: 47
		private Displayable child;
	}
}
