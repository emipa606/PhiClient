using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace PhiClient.UI;

public class TabsContainer : Displayable
{
    private const float TAB_HEIGHT = 45f;

    private readonly List<TabEntry> tabs = new List<TabEntry>();

    private int selectedTab;

    public TabsContainer(int selectedTab, Action onChange)
    {
        this.selectedTab = selectedTab;
    }

    public void AddTab(string label, Displayable displayable)
    {
        var index = tabs.Count;
        var tab = new TabRecord(label, delegate { selectedTab = index; }, selectedTab == index);
        tabs.Add(new TabEntry
        {
            tab = tab,
            displayable = displayable
        });
    }

    public override void Draw(Rect inRect)
    {
        TabDrawer.DrawTabs(inRect.TopPartPixels(45f), (from o in tabs
            select o.tab).ToList());
        var inRect2 = inRect.BottomPartPixels(inRect.height - 45f);
        tabs[selectedTab].displayable.Draw(inRect2);
    }

    public override float CalcHeight(float width)
    {
        return 45f;
    }

    public override bool IsFluidHeight()
    {
        return false;
    }
}