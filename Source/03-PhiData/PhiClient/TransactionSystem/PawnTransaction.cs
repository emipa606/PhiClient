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
		// Token: 0x0600005A RID: 90 RVA: 0x00003BE9 File Offset: 0x00001DE9
		public PawnTransaction(int id, User sender, User receiver, Pawn pawn, RealmPawn realmPawn, TransactionType transaction) : base(id, sender, receiver)
		{
			this.pawn = pawn;
			this.realmPawn = realmPawn;
			this.transaction = transaction;
		}

		// Token: 0x0600005B RID: 91 RVA: 0x00003C0C File Offset: 0x00001E0C
		public override void OnStartReceiver(RealmData realmData)
		{
			if (!this.receiver.preferences.receiveColonists)
			{
				realmData.NotifyPacketToServer(new ConfirmServerTransactionPacket
				{
					transaction = this,
					response = TransactionResponse.DECLINED
				});
				return;
			}
			Dialog_GeneralChoice window = new Dialog_GeneralChoice(new DialogChoiceConfig
			{
				text = string.Format("{0} wants to send you a {1}", this.sender.name, Enum.GetName(typeof(TransactionType), this.transaction).ToLower()),
				buttonAText = "Accept",
				buttonAAction = delegate()
				{
					realmData.NotifyPacketToServer(new ConfirmServerTransactionPacket
					{
						transaction = this,
						response = TransactionResponse.ACCEPTED
					});
				},
				buttonBText = "Refuse",
				buttonBAction = delegate()
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
			if (!this.receiver.preferences.receiveItems)
			{
				this.state = TransactionResponse.DECLINED;
			}
			if (this.state == TransactionResponse.ACCEPTED)
			{
				Pawn singleContainedThing = this.realmPawn.FromRealmPawn(realmData);
				IntVec3 intVec = DropCellFinder.RandomDropSpot(Find.CurrentMap);
				DropPodUtility.MakeDropPodAt(intVec, Find.CurrentMap, new ActiveDropPodInfo
				{
					SingleContainedThing = singleContainedThing,
					openDelay = 110,
					leaveSlag = false
				});
				Find.LetterStack.ReceiveLetter(string.Format("{0} pod", Enum.GetName(typeof(TransactionType), this.transaction)), string.Format("A {0} was sent to you by {1}", Enum.GetName(typeof(TransactionType), this.transaction).ToLower(), this.sender.name), LetterDefOf.PositiveEvent, new GlobalTargetInfo(intVec, Find.CurrentMap, false), null, null);
				return;
			}
			if (this.state == TransactionResponse.INTERRUPTED)
			{
				Messages.Message("Unexpected interruption during item transaction with " + this.sender.name, MessageTypeDefOf.RejectInput, true);
				return;
			}
			TransactionResponse state = this.state;
		}

		// Token: 0x0600005D RID: 93 RVA: 0x00003E08 File Offset: 0x00002008
		public override void OnEndSender(RealmData realmData)
		{
			if (!this.receiver.preferences.receiveItems)
			{
				this.state = TransactionResponse.DECLINED;
			}
			if (this.state == TransactionResponse.ACCEPTED)
			{
				this.pawn.DeSpawn(DestroyMode.Vanish);
				Find.WorldPawns.PassToWorld(this.pawn, PawnDiscardDecideMode.KeepForever);
				Messages.Message(this.receiver.name + " accepted your items", MessageTypeDefOf.NeutralEvent, true);
				return;
			}
			if (this.state == TransactionResponse.DECLINED)
			{
				Messages.Message(this.receiver.name + " declined your items", MessageTypeDefOf.RejectInput, true);
				return;
			}
			if (this.state == TransactionResponse.INTERRUPTED)
			{
				Messages.Message("Unexpected interruption during item transaction with " + this.receiver.name, MessageTypeDefOf.RejectInput, true);
				return;
			}
			if (this.state == TransactionResponse.TOOFAST)
			{
				Messages.Message("Transaction with " + this.receiver.name + " was declined by the server. Are you sending colonists too quickly?", MessageTypeDefOf.RejectInput, true);
			}
		}

		// Token: 0x04000076 RID: 118
		[NonSerialized]
		public Pawn pawn;

		// Token: 0x04000077 RID: 119
		public RealmPawn realmPawn;

		// Token: 0x04000078 RID: 120
		public TransactionType transaction;
	}
}
