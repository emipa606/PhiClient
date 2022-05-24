using System;
using System.Collections.Generic;
using System.Linq;
using PhiClient.UI;
using RimWorld;
using UnityEngine;
using Verse;

namespace PhiClient;

internal class UserGiveWindow : Window
{
    private const float TITLE_HEIGHT = 45f;

    private const float ROW_HEIGHT = 30f;

    private const float CONTROLS_WIDTH = 300f;

    private readonly Dictionary<List<Thing>, int> chosenThings = new Dictionary<List<Thing>, int>();

    private readonly List<List<Thing>> inventory = new List<List<Thing>>();

    private readonly User user;

    private List<List<Thing>> filteredInventory;

    private string filterTerm = "";

    private Vector2 scrollPosition = Vector2.zero;

    public UserGiveWindow(User user)
    {
        this.user = user;
        closeOnClickedOutside = true;
        doCloseX = true;
    }

    public override Vector2 InitialSize => new Vector2(1100f, Screen.height);

    public void CountItems()
    {
        inventory.Clear();
        foreach (var zone2 in Find.CurrentMap.zoneManager.AllZones.FindAll(zone => zone is Zone_Stockpile))
        {
            foreach (var thing in ((Zone_Stockpile)zone2).AllContainedThings)
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

                var list2 = new List<Thing> { thing };
                inventory.Add(list2);
            }
        }

        FilterInventory();
    }

    public void FilterInventory()
    {
        filteredInventory = (from e in inventory
            where ContainsStringIgnoreCase(e[0].Label, filterTerm)
            select e).ToList();
        scrollPosition = Vector2.zero;
    }

    private bool ContainsStringIgnoreCase(string hay, string needle)
    {
        return hay.IndexOf(needle, StringComparison.OrdinalIgnoreCase) >= 0;
    }

    public override void PreOpen()
    {
        base.PreOpen();
        CountItems();
    }

    public override void DoWindowContents(Rect inRect)
    {
        var listContainer = new ListContainer { spaceBetween = 10f };
        listContainer.Add(new TextWidget($"Ship to {user.name}", GameFont.Medium, TextAnchor.MiddleCenter));
        listContainer.Add(new Container(new TextFieldWidget(filterTerm, delegate(string s)
        {
            filterTerm = s;
            FilterInventory();
        }), 150f, 30f));
        var listContainer2 = new ListContainer { drawAlternateBackground = true };
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
                var listContainer3 = new ListContainer(ListFlow.ROW) { spaceBetween = 10f };
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

    public void ChangeChosenCount(List<Thing> things, int count)
    {
        if (chosenThings.ContainsKey(things))
        {
            chosenThings[things] += count;
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

    public void OnSendClick()
    {
        if (PhiClient.instance.SendThings(user, chosenThings))
        {
            Close();
        }
    }
}