using System.Collections.Generic;
using PhiClient.UI;
using UnityEngine;
using Verse;

namespace PhiClient;

internal class UserSendColonistWindow : Window
{
    private readonly User user;

    private Vector2 scrollPosition = Vector2.zero;

    public UserSendColonistWindow(User user)
    {
        this.user = user;
        doCloseX = true;
        closeOnClickedOutside = true;
    }

    public override void DoWindowContents(Rect inRect)
    {
        var listContainer = new ListContainer { spaceBetween = 10f };
        listContainer.Add(new TextWidget(
            "ATTENTION: This feature is highly experimental. Only use it if you're playing with a save you could potentially corrupt."));
        var listContainer2 = new ListContainer { drawAlternateBackground = true };
        listContainer.Add(new ScrollContainer(listContainer2, scrollPosition,
            delegate(Vector2 s) { scrollPosition = s; }));
        using (IEnumerator<Pawn> enumerator = Find.CurrentMap.mapPawns.FreeColonists.GetEnumerator())
        {
            while (enumerator.MoveNext())
            {
                var freeColonist = enumerator.Current;
                var listContainer3 = new ListContainer(ListFlow.ROW) { spaceBetween = 10f };
                listContainer2.Add(new HeightContainer(listContainer3, 40f));
                listContainer3.Add(new ButtonWidget(freeColonist?.Label,
                    delegate { OnColonistClick(freeColonist); },
                    false));
            }
        }

        listContainer.Draw(inRect);
    }

    public void OnColonistClick(Pawn pawn)
    {
        PhiClient.instance.SendPawn(user, pawn, TransactionType.Colonist);
    }
}