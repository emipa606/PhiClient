using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace PhiClient.UI
{
    // Token: 0x02000013 RID: 19
    public class TabsContainer : Displayable
    {
        // Token: 0x04000044 RID: 68
        private const float TAB_HEIGHT = 45f;

        // Token: 0x04000046 RID: 70
        private int selectedTab;

        // Token: 0x04000045 RID: 69
        private readonly List<TabEntry> tabs = new List<TabEntry>();

        // Token: 0x0600007E RID: 126 RVA: 0x000042D3 File Offset: 0x000024D3
        public TabsContainer(int selectedTab, Action onChange)
        {
            this.selectedTab = selectedTab;
        }

        // Token: 0x0600007F RID: 127 RVA: 0x000042F0 File Offset: 0x000024F0
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

        // Token: 0x06000080 RID: 128 RVA: 0x00004360 File Offset: 0x00002560
        public override void Draw(Rect inRect)
        {
            TabDrawer.DrawTabs(inRect.TopPartPixels(45f), (from o in tabs
                select o.tab).ToList());
            var inRect2 = inRect.BottomPartPixels(inRect.height - 45f);
            tabs[selectedTab].displayable.Draw(inRect2);
        }

        // Token: 0x06000081 RID: 129 RVA: 0x000043E2 File Offset: 0x000025E2
        public override float CalcHeight(float width)
        {
            return 45f;
        }

        // Token: 0x06000082 RID: 130 RVA: 0x00003C7F File Offset: 0x00001E7F
        public override bool IsFluidHeight()
        {
            return false;
        }
    }
}