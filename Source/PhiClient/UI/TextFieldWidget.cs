using System;
using UnityEngine;
using Verse;

namespace PhiClient.UI
{
	// Token: 0x02000014 RID: 20
	internal class TextFieldWidget : Displayable
	{
		// Token: 0x06000083 RID: 131 RVA: 0x000043E9 File Offset: 0x000025E9
		public TextFieldWidget(string text, Action<string> onChange)
		{
			this.text = text;
			this.onChange = onChange;
		}

		// Token: 0x06000084 RID: 132 RVA: 0x0000440C File Offset: 0x0000260C
		public override void Draw(Rect inRect)
		{
			string text = Widgets.TextField(inRect, this.text);
			if (text != this.text)
			{
				this.onChange(text);
			}
		}

		// Token: 0x04000047 RID: 71
		public string text = "";

		// Token: 0x04000048 RID: 72
		public Action<string> onChange;
	}
}
