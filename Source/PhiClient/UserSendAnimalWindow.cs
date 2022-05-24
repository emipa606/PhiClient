using System.Linq;
using PhiClient.UI;
using RimWorld;
using UnityEngine;
using Verse;

namespace PhiClient;

internal class UserSendAnimalWindow : Window
{
    private readonly User user;

    private Vector2 scrollPosition = Vector2.zero;

    public UserSendAnimalWindow(User user)
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
        using (var enumerator = (from p in Find.CurrentMap.mapPawns.AllPawns
                   where p.RaceProps.Animal && p.Faction == Faction.OfPlayer
                   select p).GetEnumerator())
        {
            while (enumerator.MoveNext())
            {
                var item = enumerator.Current;
                var listContainer3 = new ListContainer(ListFlow.ROW) { spaceBetween = 10f };
                listContainer2.Add(new HeightContainer(listContainer3, 40f));
                if (item != null)
                {
                    listContainer3.Add(new ButtonWidget(item.Label, delegate { OnAnimalClick(item); }, false));
                }
            }
        }

        listContainer.Draw(inRect);
    }

    public void OnAnimalClick(Pawn pawn)
    {
        PhiClient.instance.SendPawn(user, pawn, TransactionType.Animal);
    }
}