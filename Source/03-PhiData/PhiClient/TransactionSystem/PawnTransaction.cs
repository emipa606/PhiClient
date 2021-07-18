using System;
using PhiClient.Legacy;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace PhiClient.TransactionSystem
{
    // Token: 0x02000023 RID: 35
    [Serializable]
    public class PawnTransaction : Transaction
    {
        // Token: 0x04000076 RID: 118
        [NonSerialized] public Pawn pawn;

        // Token: 0x04000077 RID: 119
        public RealmPawn realmPawn;

        // Token: 0x04000078 RID: 120
        public TransactionType transaction;

        // Token: 0x0600005A RID: 90 RVA: 0x00003BE9 File Offset: 0x00001DE9
        public PawnTransaction(int id, User sender, User receiver, Pawn pawn, RealmPawn realmPawn,
            TransactionType transaction) : base(id, sender, receiver)
        {
            this.pawn = pawn;
            this.realmPawn = realmPawn;
            this.transaction = transaction;
        }

        // Token: 0x0600005B RID: 91 RVA: 0x00003C0C File Offset: 0x00001E0C
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

        // Token: 0x0600005C RID: 92 RVA: 0x00003CE8 File Offset: 0x00001EE8
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

        // Token: 0x0600005D RID: 93 RVA: 0x00003E08 File Offset: 0x00002008
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
}