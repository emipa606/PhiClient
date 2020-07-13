using System;
using UnityEngine;
using Verse;

namespace PhiClient.UI
{
	// Token: 0x02000009 RID: 9
	internal class ButtonWidget : Displayable
	{
		// Token: 0x06000058 RID: 88 RVA: 0x00003BD5 File Offset: 0x00001DD5
		public ButtonWidget(string label, Action clickAction, bool drawBackground = true)
		{
			this.label = label;
			this.drawBackground = drawBackground;
			this.clickAction = clickAction;
		}

		// Token: 0x06000059 RID: 89 RVA: 0x00003BF2 File Offset: 0x00001DF2
		public override void Draw(Rect inRect)
		{
			if (Widgets.ButtonText(inRect, this.label, this.drawBackground, false, true))
			{
				this.clickAction();
			}
		}

		// Token: 0x0600005A RID: 90 RVA: 0x00003C15 File Offset: 0x00001E15
		public override bool IsFluidHeight()
		{
			return this.drawBackground;
		}

		// Token: 0x0600005B RID: 91 RVA: 0x00003C1D File Offset: 0x00001E1D
		public override float CalcHeight(float width)
		{
			return Text.CalcHeight(this.label, width);
		}

		// Token: 0x04000026 RID: 38
		public string label;

		// Token: 0x04000027 RID: 39
		public bool drawBackground;

		// Token: 0x04000028 RID: 40
		public Action clickAction;
	}
}
