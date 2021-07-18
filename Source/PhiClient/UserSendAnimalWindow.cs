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
        // Token: 0x04000022 RID: 34
        private readonly User user;

        // Token: 0x04000023 RID: 35
        private Vector2 scrollPosition = Vector2.zero;

        // Token: 0x06000050 RID: 80 RVA: 0x00003919 File Offset: 0x00001B19
        public UserSendAnimalWindow(User user)
        {
            this.user = user;
            doCloseX = true;
            closeOnClickedOutside = true;
        }

        // Token: 0x06000051 RID: 81 RVA: 0x00003944 File Offset: 0x00001B44
        public override void DoWindowContents(Rect inRect)
        {
            var listContainer = new ListContainer {spaceBetween = 10f};
            listContainer.Add(new TextWidget(
                "ATTENTION: This feature is highly experimental. Only use it if you're playing with a save you could potentially corrupt."));
            var listContainer2 = new ListContainer {drawAlternateBackground = true};
            listContainer.Add(new ScrollContainer(listContainer2, scrollPosition,
                delegate(Vector2 s) { scrollPosition = s; }));
            using (var enumerator = (from p in Find.CurrentMap.mapPawns.AllPawns
                where p.RaceProps.Animal && p.Faction == Faction.OfPlayer
                select p).GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    var item = enumerator.Current;
                    var listContainer3 = new ListContainer(ListFlow.ROW) {spaceBetween = 10f};
                    listContainer2.Add(new HeightContainer(listContainer3, 40f));
                    if (item != null)
                    {
                        listContainer3.Add(new ButtonWidget(item.Label, delegate { OnAnimalClick(item); }, false));
                    }
                }
            }

            listContainer.Draw(inRect);
        }

        // Token: 0x06000052 RID: 82 RVA: 0x00003A6C File Offset: 0x00001C6C
        public void OnAnimalClick(Pawn pawn)
        {
            PhiClient.instance.SendPawn(user, pawn, TransactionType.Animal);
        }
    }
}