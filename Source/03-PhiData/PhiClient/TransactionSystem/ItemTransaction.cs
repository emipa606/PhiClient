using System;
using System.Collections.Generic;
using System.Linq;
using PhiClient.Legacy;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace PhiClient.TransactionSystem
{
	// Token: 0x02000026 RID: 38
	[Serializable]
	public class ItemTransaction : Transaction
	{
		// Token: 0x06000066 RID: 102 RVA: 0x000040BB File Offset: 0x000022BB
		public ItemTransaction(int id, User sender, User receiver, Dictionary<List<Thing>, int> things, List<KeyValuePair<RealmThing, int>> realmThings) : base(id, sender, receiver)
		{
			this.things = things;
			this.realmThings = realmThings;
		}

		// Token: 0x06000067 RID: 103 RVA: 0x000040D8 File Offset: 0x000022D8
		public override void OnStartReceiver(RealmData realmData)
		{
			if (!this.receiver.preferences.receiveItems)
			{
				realmData.NotifyPacketToServer(new ConfirmServerTransactionPacket
				{
					transaction = this,
					response = TransactionResponse.DECLINED
				});
				return;
			}
			List<KeyValuePair<Thing, int>> source = this.realmThings.Select(delegate(KeyValuePair<RealmThing, int> r)
			{
				RealmData realmData2 = realmData;
				KeyValuePair<RealmThing, int> keyValuePair = r;
				Thing key = realmData2.FromRealmThing(keyValuePair.Key);
				keyValuePair = r;
				return new KeyValuePair<Thing, int>(key, keyValuePair.Value);
			}).ToList<KeyValuePair<Thing, int>>();
			string str = string.Join("\n", source.Select(delegate(KeyValuePair<Thing, int> t)
			{
				KeyValuePair<Thing, int> keyValuePair = t;
				string str2 = keyValuePair.Value.ToString();
				string str3 = "x ";
				keyValuePair = t;
				return str2 + str3 + keyValuePair.Key.LabelCapNoCount;
			}).ToArray<string>());
			Dialog_GeneralChoice window = new Dialog_GeneralChoice(new DialogChoiceConfig
			{
				text = this.sender.name + " wants to ship you:\n" + str,
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

		// Token: 0x06000068 RID: 104 RVA: 0x000041E8 File Offset: 0x000023E8
		public override void OnEndReceiver(RealmData realmData)
		{
			if (!this.receiver.preferences.receiveItems)
			{
				this.state = TransactionResponse.DECLINED;
			}
			if (this.state == TransactionResponse.ACCEPTED)
			{
				List<Thing> list = new List<Thing>();
				foreach (KeyValuePair<RealmThing, int> keyValuePair in this.realmThings)
				{
					RealmThing key = keyValuePair.Key;
					Thing thing = realmData.FromRealmThing(key);
					for (int i = keyValuePair.Value; i > 0; i -= key.stackCount)
					{
						key.stackCount = Math.Min(i, thing.def.stackLimit);
						list.Add(realmData.FromRealmThing(key));
					}
				}
				IntVec3 intVec = DropCellFinder.RandomDropSpot(Find.CurrentMap);
				DropPodUtility.DropThingsNear(intVec, Find.CurrentMap, list, 110, false, false, true);
				Find.LetterStack.ReceiveLetter("Ship pod", "A pod was sent from " + this.sender.name + " containing items", LetterDefOf.PositiveEvent, new GlobalTargetInfo(intVec, Find.CurrentMap, false), null, null);
				return;
			}
			if (this.state == TransactionResponse.INTERRUPTED)
			{
				Messages.Message("Unexpected interruption during item transaction with " + this.sender.name, MessageTypeDefOf.RejectInput, true);
				return;
			}
			TransactionResponse state = this.state;
		}

		// Token: 0x06000069 RID: 105 RVA: 0x00004348 File Offset: 0x00002548
		public override void OnEndSender(RealmData realmData)
		{
			if (!this.receiver.preferences.receiveItems)
			{
				this.state = TransactionResponse.DECLINED;
			}
			if (this.state == TransactionResponse.ACCEPTED)
			{
				foreach (KeyValuePair<List<Thing>, int> keyValuePair in this.things)
				{
					int num = keyValuePair.Value;
					foreach (Thing thing in keyValuePair.Key)
					{
						if (!thing.Destroyed)
						{
							int num2 = Math.Min(num, thing.stackCount);
							if (num2 == thing.stackCount)
							{
								thing.Destroy(DestroyMode.Vanish);
							}
							else
							{
								thing.stackCount -= num2;
							}
							num -= num2;
						}
					}
					if (num > 0)
					{
						Log.Warning(string.Concat(new object[]
						{
							"Trying to destroy ",
							keyValuePair.Key[0].LabelShort,
							" but couldn't destroy the ",
							num,
							" remaining"
						}), false);
					}
				}
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
				Messages.Message("Transaction with " + this.receiver.name + " was declined by the server. Are you sending items too quickly?", MessageTypeDefOf.RejectInput, true);
			}
		}

		// Token: 0x04000082 RID: 130
		[NonSerialized]
		public Dictionary<List<Thing>, int> things;

		// Token: 0x04000083 RID: 131
		public List<KeyValuePair<RealmThing, int>> realmThings;
	}
}
