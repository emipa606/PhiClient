using System;
using System.Collections.Generic;
using System.Linq;
using PhiClient.UI;
using RimWorld;
using UnityEngine;
using Verse;

namespace PhiClient
{
	// Token: 0x02000007 RID: 7
	internal class UserSendAnimalWindow : Window
	{
		// Token: 0x06000050 RID: 80 RVA: 0x00003919 File Offset: 0x00001B19
		public UserSendAnimalWindow(User user)
		{
			this.user = user;
			this.doCloseX = true;
			this.closeOnClickedOutside = true;
		}

		// Token: 0x06000051 RID: 81 RVA: 0x00003944 File Offset: 0x00001B44
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
			using (IEnumerator<Pawn> enumerator = (from p in Find.CurrentMap.mapPawns.AllPawns
			where p.RaceProps.Animal && p.Faction == Faction.OfPlayer
			select p).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Pawn item = enumerator.Current;
					ListContainer listContainer3 = new ListContainer(ListFlow.ROW, ListDirection.NORMAL);
					listContainer3.spaceBetween = 10f;
					listContainer2.Add(new HeightContainer(listContainer3, 40f));
					listContainer3.Add(new ButtonWidget(item.Label, delegate()
					{
						this.OnAnimalClick(item);
					}, false));
				}
			}
			listContainer.Draw(inRect);
		}

		// Token: 0x06000052 RID: 82 RVA: 0x00003A6C File Offset: 0x00001C6C
		public void OnAnimalClick(Pawn pawn)
		{
			PhiClient.instance.SendPawn(this.user, pawn, TransactionType.Animal);
		}

		// Token: 0x04000022 RID: 34
		private User user;

		// Token: 0x04000023 RID: 35
		private Vector2 scrollPosition = Vector2.zero;
	}
}
