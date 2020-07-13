using System;
using System.Collections.Generic;
using PhiClient.UI;
using UnityEngine;
using Verse;

namespace PhiClient
{
	// Token: 0x02000008 RID: 8
	internal class UserSendColonistWindow : Window
	{
		// Token: 0x06000054 RID: 84 RVA: 0x00003A89 File Offset: 0x00001C89
		public UserSendColonistWindow(User user)
		{
			this.user = user;
			this.doCloseX = true;
			this.closeOnClickedOutside = true;
		}

		// Token: 0x06000055 RID: 85 RVA: 0x00003AB4 File Offset: 0x00001CB4
		public override void DoWindowContents(Rect inRect)
		{
			ListContainer listContainer = new ListContainer();
			listContainer.spaceBetween = 10f;
			listContainer.Add(new TextWidget("ATTENTION: This feature is highly experimental. Only use it if you're playing with a save you could potentially corrupt.", GameFont.Small, 0));
			ListContainer listContainer2 = new ListContainer();
			listContainer2.drawAlternateBackground = true;
			listContainer.Add(new ScrollContainer(listContainer2, this.scrollPosition, delegate(Vector2 s)
			{
				this.scrollPosition = s;
			}));
			using (IEnumerator<Pawn> enumerator = Find.CurrentMap.mapPawns.FreeColonists.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Pawn freeColonist = enumerator.Current;
					ListContainer listContainer3 = new ListContainer(ListFlow.ROW, ListDirection.NORMAL);
					listContainer3.spaceBetween = 10f;
					listContainer2.Add(new HeightContainer(listContainer3, 40f));
					listContainer3.Add(new ButtonWidget(freeColonist.Label, delegate()
					{
						this.OnColonistClick(freeColonist);
					}, false));
				}
			}
			listContainer.Draw(inRect);
		}

		// Token: 0x06000056 RID: 86 RVA: 0x00003BB8 File Offset: 0x00001DB8
		public void OnColonistClick(Pawn pawn)
		{
			PhiClient.instance.SendPawn(this.user, pawn, TransactionType.Colonist);
		}

		// Token: 0x04000024 RID: 36
		private User user;

		// Token: 0x04000025 RID: 37
		private Vector2 scrollPosition = Vector2.zero;
	}
}
