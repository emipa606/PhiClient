using System.Collections.Generic;
using PhiClient.UI;
using UnityEngine;
using Verse;

namespace PhiClient
{
    // Token: 0x02000008 RID: 8
    internal class UserSendColonistWindow : Window
    {
        // Token: 0x04000024 RID: 36
        private readonly User user;

        // Token: 0x04000025 RID: 37
        private Vector2 scrollPosition = Vector2.zero;

        // Token: 0x06000054 RID: 84 RVA: 0x00003A89 File Offset: 0x00001C89
        public UserSendColonistWindow(User user)
        {
            this.user = user;
            doCloseX = true;
            closeOnClickedOutside = true;
        }

        // Token: 0x06000055 RID: 85 RVA: 0x00003AB4 File Offset: 0x00001CB4
        public override void DoWindowContents(Rect inRect)
        {
            var listContainer = new ListContainer {spaceBetween = 10f};
            listContainer.Add(new TextWidget(
                "ATTENTION: This feature is highly experimental. Only use it if you're playing with a save you could potentially corrupt."));
            var listContainer2 = new ListContainer {drawAlternateBackground = true};
            listContainer.Add(new ScrollContainer(listContainer2, scrollPosition,
                delegate(Vector2 s) { scrollPosition = s; }));
            using (IEnumerator<Pawn> enumerator = Find.CurrentMap.mapPawns.FreeColonists.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    var freeColonist = enumerator.Current;
                    var listContainer3 = new ListContainer(ListFlow.ROW) {spaceBetween = 10f};
                    listContainer2.Add(new HeightContainer(listContainer3, 40f));
                    listContainer3.Add(new ButtonWidget(freeColonist?.Label,
                        delegate { OnColonistClick(freeColonist); },
                        false));
                }
            }

            listContainer.Draw(inRect);
        }

        // Token: 0x06000056 RID: 86 RVA: 0x00003BB8 File Offset: 0x00001DB8
        public void OnColonistClick(Pawn pawn)
        {
            PhiClient.instance.SendPawn(user, pawn, TransactionType.Colonist);
        }
    }
}