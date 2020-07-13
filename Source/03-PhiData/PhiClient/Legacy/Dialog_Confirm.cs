using System;
using UnityEngine;
using Verse;

namespace PhiClient.Legacy
{
	// Token: 0x0200002B RID: 43
	public class Dialog_Confirm : Window
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000076 RID: 118 RVA: 0x000046B0 File Offset: 0x000028B0
		public override Vector2 InitialSize
		{
			get
			{
				float num = 300f;
				if (this.title != null)
				{
					num += 40f;
				}
				return new Vector2(500f, num);
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000077 RID: 119 RVA: 0x000046DE File Offset: 0x000028DE
		private float TimeUntilInteractive
		{
			get
			{
				return this.interactionDelay - (Time.realtimeSinceStartup - this.createRealTime);
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000078 RID: 120 RVA: 0x000046F3 File Offset: 0x000028F3
		private bool InteractionDelayExpired
		{
			get
			{
				return this.TimeUntilInteractive <= 0f;
			}
		}

		// Token: 0x06000079 RID: 121 RVA: 0x00004708 File Offset: 0x00002908
		public Dialog_Confirm(string text, Action confirmedAction, bool destructive = false, string title = null, bool showGoBack = true)
		{
			this.text = text;
			this.confirmedAction = confirmedAction;
			this.destructiveAction = destructive;
			this.title = title;
			this.showGoBack = showGoBack;
			this.confirmLabel = Translator.Translate("Confirm");
			this.goBackLabel = Translator.Translate("GoBack");
			this.forcePause = true;
			this.absorbInputAroundWindow = true;
			this.createRealTime = Time.realtimeSinceStartup;
		}

		// Token: 0x0600007A RID: 122 RVA: 0x0000477C File Offset: 0x0000297C
		public override void DoWindowContents(Rect inRect)
		{
			float num = inRect.y;
			if (!this.title.NullOrEmpty())
			{
				Text.Font = GameFont.Medium;
				Widgets.Label(new Rect(0f, num, inRect.width, 40f), this.title);
				num += 40f;
			}
			Text.Font = GameFont.Small;
			Rect rect = new Rect(0f, num, inRect.width, inRect.height - 45f - num);
			Rect rect2 = new Rect(0f, 0f, inRect.width - 16f, this.scrollViewHeight);
			Widgets.BeginScrollView(rect, ref this.scrollPos, rect2, true);
			Widgets.Label(new Rect(0f, 0f, rect2.width, this.scrollViewHeight), this.text);
			if (Event.current.type == EventType.Layout)
			{
				this.scrollViewHeight = Text.CalcHeight(this.text, rect2.width);
			}
			Widgets.EndScrollView();
			if (this.destructiveAction)
			{
				GUI.color = new Color(1f, 0.3f, 0.35f);
			}
			string text = (!this.InteractionDelayExpired) ? (this.confirmLabel + "(" + Mathf.Ceil(this.TimeUntilInteractive).ToString("F0") + ")") : this.confirmLabel;
			if (Widgets.ButtonText(new Rect(inRect.width / 2f + 20f, inRect.height - 35f, inRect.width / 2f - 20f, 35f), text, true, false, true) && this.InteractionDelayExpired)
			{
				this.confirmedAction();
				this.Close(true);
			}
			GUI.color = Color.white;
			if (this.showGoBack && Widgets.ButtonText(new Rect(0f, inRect.height - 35f, inRect.width / 2f - 20f, 35f), this.goBackLabel, true, false, true))
			{
				this.Close(true);
			}
		}

		// Token: 0x04000092 RID: 146
		private const float TitleHeight = 40f;

		// Token: 0x04000093 RID: 147
		private string text;

		// Token: 0x04000094 RID: 148
		private Action confirmedAction;

		// Token: 0x04000095 RID: 149
		private bool destructiveAction;

		// Token: 0x04000096 RID: 150
		private string title;

		// Token: 0x04000097 RID: 151
		public string confirmLabel;

		// Token: 0x04000098 RID: 152
		public string goBackLabel;

		// Token: 0x04000099 RID: 153
		public bool showGoBack;

		// Token: 0x0400009A RID: 154
		public float interactionDelay;

		// Token: 0x0400009B RID: 155
		private Vector2 scrollPos;

		// Token: 0x0400009C RID: 156
		private float scrollViewHeight;

		// Token: 0x0400009D RID: 157
		private float createRealTime;
	}
}
