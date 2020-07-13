using System;
using UnityEngine;
using Verse;

namespace PhiClient.UI
{
	// Token: 0x02000011 RID: 17
	internal class ScrollContainer : Displayable
	{
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
			Rect rect = GenUI.LeftPartPixels(inRect, inRect.width - 16f);
			float width = rect.width;
			float num = this.child.CalcHeight(rect.width);
			if (num == -1f)
			{
				num = rect.height;
			}
			Rect rect2 = new Rect(0f, 0f, width, num);
			Widgets.BeginScrollView(inRect, ref this.scrollPosition, rect2, true);
			this.onScroll(this.scrollPosition);
			this.child.Draw(rect2);
			Widgets.EndScrollView();
		}

		// Token: 0x0400003E RID: 62
		private const float SCROLL_BAR_SIZE = 16f;

		// Token: 0x0400003F RID: 63
		private Displayable child;

		// Token: 0x04000040 RID: 64
		private Action<Vector2> onScroll;

		// Token: 0x04000041 RID: 65
		private Vector2 scrollPosition = Vector2.zero;
	}
}
