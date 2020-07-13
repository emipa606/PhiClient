using System;
using UnityEngine;
using Verse;

namespace PhiClient.UI
{
	// Token: 0x02000015 RID: 21
	public class TextWidget : Displayable
	{
		// Token: 0x06000085 RID: 133 RVA: 0x00004440 File Offset: 0x00002640
		public TextWidget(string text, GameFont font = GameFont.Small, TextAnchor anchor = TextAnchor.UpperLeft)
		{
			this.text = text;
			this.font = font;
			this.anchor = anchor;
		}

		// Token: 0x06000086 RID: 134 RVA: 0x0000445D File Offset: 0x0000265D
		public override float CalcHeight(float width)
		{
			this.SetStyle();
			float result = Text.CalcHeight(this.text, width);
			this.ClearStyle();
			return result;
		}

		// Token: 0x06000087 RID: 135 RVA: 0x00004477 File Offset: 0x00002677
		public override void Draw(Rect inRect)
		{
			this.SetStyle();
			Widgets.Label(inRect, this.text);
			this.ClearStyle();
		}

		// Token: 0x06000088 RID: 136 RVA: 0x00004491 File Offset: 0x00002691
		private void SetStyle()
		{
			Text.Anchor = this.anchor;
			Text.Font = this.font;
		}

		// Token: 0x06000089 RID: 137 RVA: 0x000044A9 File Offset: 0x000026A9
		private void ClearStyle()
		{
			Text.Anchor = 0;
			Text.Font = GameFont.Small;
		}

		// Token: 0x0600008A RID: 138 RVA: 0x00003C7F File Offset: 0x00001E7F
		public override bool IsFluidHeight()
		{
			return false;
		}

		// Token: 0x04000049 RID: 73
		private string text;

		// Token: 0x0400004A RID: 74
		private GameFont font;

		// Token: 0x0400004B RID: 75
		private TextAnchor anchor;
	}
}
