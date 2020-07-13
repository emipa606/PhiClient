using System;
using UnityEngine;
using Verse;

namespace PhiClient.UI
{
	// Token: 0x0200000A RID: 10
	internal class CheckboxLabeledWidget : Displayable
	{
		// Token: 0x0600005C RID: 92 RVA: 0x00003C2B File Offset: 0x00001E2B
		public CheckboxLabeledWidget(string label, bool checkedOn, Action<bool> onChange)
		{
			this.label = label;
			this.checkedOn = checkedOn;
			this.onChange = onChange;
		}

		// Token: 0x0600005D RID: 93 RVA: 0x00003C48 File Offset: 0x00001E48
		public override void Draw(Rect inRect)
		{
			bool flag = this.checkedOn;
			Widgets.CheckboxLabeled(inRect, this.label, ref this.checkedOn, false, null, null, false);
			if (flag != this.checkedOn)
			{
				this.onChange(this.checkedOn);
			}
		}

		// Token: 0x0600005E RID: 94 RVA: 0x00003C7F File Offset: 0x00001E7F
		public override bool IsFluidHeight()
		{
			return false;
		}

		// Token: 0x0600005F RID: 95 RVA: 0x00003C82 File Offset: 0x00001E82
		public override float CalcHeight(float width)
		{
			return 40f;
		}

		// Token: 0x04000029 RID: 41
		private const float CHECKBOX_SIZE = 40f;

		// Token: 0x0400002A RID: 42
		private string label;

		// Token: 0x0400002B RID: 43
		private bool checkedOn;

		// Token: 0x0400002C RID: 44
		private Action<bool> onChange;
	}
}
