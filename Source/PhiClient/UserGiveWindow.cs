using System;
using System.Collections.Generic;
using System.Linq;
using PhiClient.UI;
using RimWorld;
using UnityEngine;
using Verse;

namespace PhiClient
{
	// Token: 0x02000006 RID: 6
	internal class UserGiveWindow : Window
	{
		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000043 RID: 67 RVA: 0x000032F9 File Offset: 0x000014F9
		public override Vector2 InitialSize
		{
			get
			{
				return new Vector2(1100f, (float)Screen.height);
			}
		}

		// Token: 0x06000044 RID: 68 RVA: 0x0000330C File Offset: 0x0000150C
		public UserGiveWindow(User user)
		{
			this.user = user;
			this.closeOnClickedOutside = true;
			this.doCloseX = true;
		}

		// Token: 0x06000045 RID: 69 RVA: 0x00003360 File Offset: 0x00001560
		public void CountItems()
		{
			this.inventory.Clear();
			foreach (Zone zone2 in Find.CurrentMap.zoneManager.AllZones.FindAll((Zone zone) => zone is Zone_Stockpile))
			{
				foreach (Thing thing in ((Zone_Stockpile)zone2).AllContainedThings)
				{
					if (thing.def.category == ThingCategory.Item && !thing.def.IsCorpse)
					{
						bool flag = false;
						foreach (List<Thing> list in this.inventory)
						{
							if (list[0].CanStackWith(thing))
							{
								list.Add(thing);
								flag = true;
							}
						}
						if (!flag)
						{
							List<Thing> list2 = new List<Thing>();
							list2.Add(thing);
							this.inventory.Add(list2);
						}
					}
				}
			}
			this.FilterInventory();
		}

		// Token: 0x06000046 RID: 70 RVA: 0x000034C4 File Offset: 0x000016C4
		public void FilterInventory()
		{
			this.filteredInventory = (from e in this.inventory
			where this.ContainsStringIgnoreCase(e[0].Label, this.filterTerm)
			select e).ToList<List<Thing>>();
			this.scrollPosition = Vector2.zero;
		}

		// Token: 0x06000047 RID: 71 RVA: 0x00003059 File Offset: 0x00001259
		private bool ContainsStringIgnoreCase(string hay, string needle)
		{
			return hay.IndexOf(needle, StringComparison.OrdinalIgnoreCase) >= 0;
		}

		// Token: 0x06000048 RID: 72 RVA: 0x000034F3 File Offset: 0x000016F3
		public override void PreOpen()
		{
			base.PreOpen();
			this.CountItems();
		}

		// Token: 0x06000049 RID: 73 RVA: 0x00003504 File Offset: 0x00001704
		public override void DoWindowContents(Rect inRect)
		{
			ListContainer listContainer = new ListContainer();
			listContainer.spaceBetween = 10f;
			listContainer.Add(new TextWidget("Ship to " + this.user.name, GameFont.Medium, TextAnchor.MiddleCenter));
			listContainer.Add(new Container(new TextFieldWidget(this.filterTerm, delegate(string s)
			{
				this.filterTerm = s;
				this.FilterInventory();
			}), 150f, 30f));
			ListContainer listContainer2 = new ListContainer();
			listContainer2.drawAlternateBackground = true;
			listContainer.Add(new ScrollContainer(listContainer2, this.scrollPosition, delegate(Vector2 s)
			{
				this.scrollPosition = s;
			}));
			using (List<List<Thing>>.Enumerator enumerator = this.filteredInventory.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					List<Thing> item = enumerator.Current;
					Thing thing = item[0];
					int num = item.Sum((Thing e) => e.stackCount);
					int num2 = 0;
					this.chosenThings.TryGetValue(item, out num2);
					ListContainer listContainer3 = new ListContainer(ListFlow.ROW, ListDirection.NORMAL);
					listContainer3.spaceBetween = 10f;
					listContainer2.Add(new HeightContainer(listContainer3, 30f));
					listContainer3.Add(new Container(new ThingIconWidget(thing), 30f, 30f));
					listContainer3.Add(new TextWidget(thing.LabelCapNoCount, GameFont.Small, TextAnchor.MiddleLeft));
					listContainer3.Add(new TextWidget(num.ToString(), GameFont.Small, TextAnchor.MiddleRight));
					ListContainer listContainer4 = new ListContainer(ListFlow.ROW, ListDirection.NORMAL);
					listContainer3.Add(new WidthContainer(listContainer4, 300f));
					listContainer4.Add(new ButtonWidget("-100", delegate()
					{
						this.ChangeChosenCount(item, -100);
					}, true));
					listContainer4.Add(new ButtonWidget("-10", delegate()
					{
						this.ChangeChosenCount(item, -10);
					}, true));
					listContainer4.Add(new ButtonWidget("-1", delegate()
					{
						this.ChangeChosenCount(item, -1);
					}, true));
					listContainer4.Add(new TextFieldWidget(num2.ToString(), delegate(string str)
					{
						this.ChangeChosenCount(item, str);
					}));
					listContainer4.Add(new ButtonWidget("+1", delegate()
					{
						this.ChangeChosenCount(item, 1);
					}, true));
					listContainer4.Add(new ButtonWidget("+10", delegate()
					{
						this.ChangeChosenCount(item, 10);
					}, true));
					listContainer4.Add(new ButtonWidget("+100", delegate()
					{
						this.ChangeChosenCount(item, 100);
					}, true));
				}
			}
			listContainer.Add(new HeightContainer(new ButtonWidget("Send", new Action(this.OnSendClick), true), 30f));
			listContainer.Draw(inRect);
		}

		// Token: 0x0600004A RID: 74 RVA: 0x000037DC File Offset: 0x000019DC
		public void ChangeChosenCount(List<Thing> things, string count)
		{
			int num = 0;
			this.chosenThings.TryGetValue(things, out num);
			if (count.Length == 0)
			{
				this.ChangeChosenCount(things, -num);
				return;
			}
			int num2 = 0;
			if (int.TryParse(count, out num2))
			{
				this.ChangeChosenCount(things, num2 - num);
			}
		}

		// Token: 0x0600004B RID: 75 RVA: 0x00003824 File Offset: 0x00001A24
		public void ChangeChosenCount(List<Thing> things, int count)
		{
			if (this.chosenThings.ContainsKey(things))
			{
				Dictionary<List<Thing>, int> dictionary = this.chosenThings;
				dictionary[things] += count;
			}
			else
			{
				this.chosenThings.Add(things, count);
			}
			if (this.chosenThings[things] > 0)
			{
				this.chosenThings[things] = Math.Min(this.chosenThings[things], things.Sum((Thing t) => t.stackCount));
				return;
			}
			this.chosenThings.Remove(things);
		}

		// Token: 0x0600004C RID: 76 RVA: 0x000038C6 File Offset: 0x00001AC6
		public void OnSendClick()
		{
			if (PhiClient.instance.SendThings(this.user, this.chosenThings))
			{
				this.Close(true);
			}
		}

		// Token: 0x04000019 RID: 25
		private const float TITLE_HEIGHT = 45f;

		// Token: 0x0400001A RID: 26
		private const float ROW_HEIGHT = 30f;

		// Token: 0x0400001B RID: 27
		private const float CONTROLS_WIDTH = 300f;

		// Token: 0x0400001C RID: 28
		private List<List<Thing>> inventory = new List<List<Thing>>();

		// Token: 0x0400001D RID: 29
		private Dictionary<List<Thing>, int> chosenThings = new Dictionary<List<Thing>, int>();

		// Token: 0x0400001E RID: 30
		private User user;

		// Token: 0x0400001F RID: 31
		private Vector2 scrollPosition = Vector2.zero;

		// Token: 0x04000020 RID: 32
		private string filterTerm = "";

		// Token: 0x04000021 RID: 33
		private List<List<Thing>> filteredInventory;
	}
}
