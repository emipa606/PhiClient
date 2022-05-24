using System;
using PhiClient.Legacy;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace PhiClient.TransactionSystem;

[Serializable]
public class PawnTransaction : Transaction
{
    [NonSerialized] public Pawn pawn;

    public RealmPawn realmPawn;

    public TransactionType transaction;

    public PawnTransaction(int id, User sender, User receiver, Pawn pawn, RealmPawn realmPawn,
        TransactionType transaction) : base(id, sender, receiver)
    {
        this.pawn = pawn;
        this.realmPawn = realmPawn;
        this.transaction = transaction;
    }

    public override void OnStartReceiver(RealmData realmData)
    {
        if (!receiver.preferences.receiveColonists)
        {
            realmData.NotifyPacketToServer(new ConfirmServerTransactionPacket
            {
                transaction = this,
                response = TransactionResponse.DECLINED
            });
            return;
        }

        var window = new Dialog_GeneralChoice(new DialogChoiceConfig
        {
            text =
                $"{sender.name} wants to send you a {Enum.GetName(typeof(TransactionType), transaction)?.ToLower()}",
            buttonAText = "Accept",
            buttonAAction = delegate
            {
                realmData.NotifyPacketToServer(new ConfirmServerTransactionPacket
                {
                    transaction = this,
                    response = TransactionResponse.ACCEPTED
                });
            },
            buttonBText = "Refuse",
            buttonBAction = delegate
            {
                realmData.NotifyPacketToServer(new ConfirmServerTransactionPacket
                {
                    transaction = this,
                    response = TransactionResponse.DECLINED
                });
            }
        });
        Find.WindowStack.Add(window);
    }

    public override void OnEndReceiver(RealmData realmData)
    {
        if (!receiver.preferences.receiveItems)
        {
            state = TransactionResponse.DECLINED;
        }

        if (state == TransactionResponse.ACCEPTED)
        {
            var singleContainedThing = realmPawn.FromRealmPawn(realmData);
            var intVec = DropCellFinder.RandomDropSpot(Find.CurrentMap);
            DropPodUtility.MakeDropPodAt(intVec, Find.CurrentMap, new ActiveDropPodInfo
            {
                SingleContainedThing = singleContainedThing,
                openDelay = 110,
                leaveSlag = false
            });
            Find.LetterStack.ReceiveLetter(
                $"{Enum.GetName(typeof(TransactionType), transaction)} pod",
                $"A {Enum.GetName(typeof(TransactionType), transaction)?.ToLower()} was sent to you by {sender.name}",
                LetterDefOf.PositiveEvent, new GlobalTargetInfo(intVec, Find.CurrentMap));
            return;
        }

        if (state == TransactionResponse.INTERRUPTED)
        {
            Messages.Message("Unexpected interruption during item transaction with " + sender.name,
                MessageTypeDefOf.RejectInput);
            return;
        }

        var unused = state;
    }

    public override void OnEndSender(RealmData realmData)
    {
        if (!receiver.preferences.receiveItems)
        {
            state = TransactionResponse.DECLINED;
        }

        if (state == TransactionResponse.ACCEPTED)
        {
            pawn.DeSpawn();
            Find.WorldPawns.PassToWorld(pawn, PawnDiscardDecideMode.KeepForever);
            Messages.Message(receiver.name + " accepted your items", MessageTypeDefOf.NeutralEvent);
            return;
        }

        if (state == TransactionResponse.DECLINED)
        {
            Messages.Message(receiver.name + " declined your items", MessageTypeDefOf.RejectInput);
            return;
        }

        if (state == TransactionResponse.INTERRUPTED)
        {
            Messages.Message("Unexpected interruption during item transaction with " + receiver.name,
                MessageTypeDefOf.RejectInput);
            return;
        }

        if (state == TransactionResponse.TOOFAST)
        {
            Messages.Message(
                "Transaction with " + receiver.name +
                " was declined by the server. Are you sending colonists too quickly?",
                MessageTypeDefOf.RejectInput);
        }
    }
}