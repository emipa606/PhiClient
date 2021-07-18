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
        // Token: 0x04000019 RID: 25
        private const float TITLE_HEIGHT = 45f;

        // Token: 0x0400001A RID: 26
        private const float ROW_HEIGHT = 30f;

        // Token: 0x0400001B RID: 27
        private const float CONTROLS_WIDTH = 300f;

        // Token: 0x0400001D RID: 29
        private readonly Dictionary<List<Thing>, int> chosenThings = new Dictionary<List<Thing>, int>();

        // Token: 0x0400001C RID: 28
        private readonly List<List<Thing>> inventory = new List<List<Thing>>();

        // Token: 0x0400001E RID: 30
        private readonly User user;

        // Token: 0x04000021 RID: 33
        private List<List<Thing>> filteredInventory;

        // Token: 0x04000020 RID: 32
        private string filterTerm = "";

        // Token: 0x0400001F RID: 31
        private Vector2 scrollPosition = Vector2.zero;

        // Token: 0x06000044 RID: 68 RVA: 0x0000330C File Offset: 0x0000150C
        public UserGiveWindow(User user)
        {
            this.user = user;
            closeOnClickedOutside = true;
            doCloseX = true;
        }

        // Token: 0x17000003 RID: 3
        // (get) Token: 0x06000043 RID: 67 RVA: 0x000032F9 File Offset: 0x000014F9
        public override Vector2 InitialSize => new Vector2(1100f, Screen.height);

        // Token: 0x06000045 RID: 69 RVA: 0x00003360 File Offset: 0x00001560
        public void CountItems()
        {
            inventory.Clear();
            foreach (var zone2 in Find.CurrentMap.zoneManager.AllZones.FindAll(zone => zone is Zone_Stockpile))
            {
                foreach (var thing in ((Zone_Stockpile) zone2).AllContainedThings)
                {
                    if (thing.def.category != ThingCategory.Item || thing.def.IsCorpse)
                    {
                        continue;
                    }

                    var stackable = false;
                    foreach (var list in inventory)
                    {
                        if (!list[0].CanStackWith(thing))
                        {
                            continue;
                        }

                        list.Add(thing);
                        stackable = true;
                    }

                    if (stackable)
                    {
                        continue;
                    }

                    var list2 = new List<Thing> {thing};
                    inventory.Add(list2);
                }
            }

            FilterInventory();
        }

        // Token: 0x06000046 RID: 70 RVA: 0x000034C4 File Offset: 0x000016C4
        public void FilterInventory()
        {
            filteredInventory = (from e in inventory
                where ContainsStringIgnoreCase(e[0].Label, filterTerm)
                select e).ToList();
            scrollPosition = Vector2.zero;
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
            CountItems();
        }

        // Token: 0x06000049 RID: 73 RVA: 0x00003504 File Offset: 0x00001704
        public override void DoWindowContents(Rect inRect)
        {
            var listContainer = new ListContainer {spaceBetween = 10f};
            listContainer.Add(new TextWidget("Ship to " + user.name, GameFont.Medium, TextAnchor.MiddleCenter));
            listContainer.Add(new Container(new TextFieldWidget(filterTerm, delegate(string s)
            {
                filterTerm = s;
                FilterInventory();
            }), 150f, 30f));
            var listContainer2 = new ListContainer {drawAlternateBackground = true};
            listContainer.Add(new ScrollContainer(listContainer2, scrollPosition,
                delegate(Vector2 s) { scrollPosition = s; }));
            using (var enumerator = filteredInventory.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    var item = enumerator.Current;
                    if (item == null)
                    {
                        continue;
                    }

                    var thing = item[0];
                    var num = item.Sum(e => e.stackCount);
                    chosenThings.TryGetValue(item, out var num2);
                    var listContainer3 = new ListContainer(ListFlow.ROW) {spaceBetween = 10f};
                    listContainer2.Add(new HeightContainer(listContainer3, 30f));
                    listContainer3.Add(new Container(new ThingIconWidget(thing), 30f, 30f));
                    listContainer3.Add(new TextWidget(thing.LabelCapNoCount, GameFont.Small, TextAnchor.MiddleLeft));
                    listContainer3.Add(new TextWidget(num.ToString(), GameFont.Small, TextAnchor.MiddleRight));
                    var listContainer4 = new ListContainer(ListFlow.ROW);
                    listContainer3.Add(new WidthContainer(listContainer4, 300f));
                    listContainer4.Add(new ButtonWidget("-100", delegate { ChangeChosenCount(item, -100); }));
                    listContainer4.Add(new ButtonWidget("-10", delegate { ChangeChosenCount(item, -10); }));
                    listContainer4.Add(new ButtonWidget("-1", delegate { ChangeChosenCount(item, -1); }));
                    listContainer4.Add(new TextFieldWidget(num2.ToString(),
                        delegate(string str) { ChangeChosenCount(item, str); }));
                    listContainer4.Add(new ButtonWidget("+1", delegate { ChangeChosenCount(item, 1); }));
                    listContainer4.Add(new ButtonWidget("+10", delegate { ChangeChosenCount(item, 10); }));
                    listContainer4.Add(new ButtonWidget("+100", delegate { ChangeChosenCount(item, 100); }));
                }
            }

            listContainer.Add(new HeightContainer(new ButtonWidget("Send", OnSendClick), 30f));
            listContainer.Draw(inRect);
        }

        // Token: 0x0600004A RID: 74 RVA: 0x000037DC File Offset: 0x000019DC
        public void ChangeChosenCount(List<Thing> things, string count)
        {
            chosenThings.TryGetValue(things, out var num);
            if (count.Length == 0)
            {
                ChangeChosenCount(things, -num);
                return;
            }

            if (int.TryParse(count, out var num2))
            {
                ChangeChosenCount(things, num2 - num);
            }
        }

        // Token: 0x0600004B RID: 75 RVA: 0x00003824 File Offset: 0x00001A24
        public void ChangeChosenCount(List<Thing> things, int count)
        {
            if (chosenThings.ContainsKey(things))
            {
                var dictionary = chosenThings;
                dictionary[things] += count;
            }
            else
            {
                chosenThings.Add(things, count);
            }

            if (chosenThings[things] > 0)
            {
                chosenThings[things] = Math.Min(chosenThings[things], things.Sum(t => t.stackCount));
                return;
            }

            chosenThings.Remove(things);
        }

        // Token: 0x0600004C RID: 76 RVA: 0x000038C6 File Offset: 0x00001AC6
        public void OnSendClick()
        {
            if (PhiClient.instance.SendThings(user, chosenThings))
            {
                Close();
            }
        }
    }
}