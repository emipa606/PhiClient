using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using PhiClient.TransactionSystem;
using RimWorld;
using Verse;

namespace PhiClient
{
	// Token: 0x02000013 RID: 19
	[Serializable]
	public class RealmData
	{
		// Token: 0x14000001 RID: 1
		// (add) Token: 0x0600001F RID: 31 RVA: 0x000023D0 File Offset: 0x000005D0
		// (remove) Token: 0x06000020 RID: 32 RVA: 0x00002408 File Offset: 0x00000608
		[field:NonSerialized]
		public event RealmData.PacketHandler PacketToClient;

		// Token: 0x14000002 RID: 2
		// (add) Token: 0x06000021 RID: 33 RVA: 0x00002440 File Offset: 0x00000640
		// (remove) Token: 0x06000022 RID: 34 RVA: 0x00002478 File Offset: 0x00000678
		[field: NonSerialized]
		public event RealmData.LogHandler Log;

		// Token: 0x14000003 RID: 3
		// (add) Token: 0x06000023 RID: 35 RVA: 0x000024B0 File Offset: 0x000006B0
		// (remove) Token: 0x06000024 RID: 36 RVA: 0x000024E8 File Offset: 0x000006E8
		public event Action<Packet> PacketToServer;

		// Token: 0x06000025 RID: 37 RVA: 0x0000251D File Offset: 0x0000071D
		public void AddUser(User user)
		{
			this.users.Add(user);
		}

		// Token: 0x06000026 RID: 38 RVA: 0x0000252B File Offset: 0x0000072B
		public void AddChatMessage(ChatMessage message)
		{
			this.chat.Add(message);
		}

		// Token: 0x06000027 RID: 39 RVA: 0x0000253C File Offset: 0x0000073C
		public bool CanStartTransaction(User sender, User receiver)
		{
			return !this.transactions.Exists((Transaction t) => !t.IsFinished() && t.sender == sender && t.receiver == receiver);
		}

		// Token: 0x06000028 RID: 40 RVA: 0x00002578 File Offset: 0x00000778
		public Transaction TryFindTransaction(int transactionId, int transactionSenderId)
		{
			return this.transactions.FindLast((Transaction t) => t != null && t.sender != null && t.getID() == transactionId && t.sender.getID() == transactionSenderId);
		}

		// Token: 0x06000029 RID: 41 RVA: 0x000025B0 File Offset: 0x000007B0
		public void EmitLog(LogLevel level, string message)
		{
			this.Log(level, message);
		}

		// Token: 0x0600002A RID: 42 RVA: 0x000025C0 File Offset: 0x000007C0
		public Transaction FindTransaction(int transactionId, int transactionSenderId)
		{
			Transaction transaction = this.TryFindTransaction(transactionId, transactionSenderId);
			if (transaction == null)
			{
				throw new Exception(string.Concat(new object[]
				{
					"Couldn't find Transaction ",
					transactionId,
					" from sender ",
					transactionSenderId
				}));
			}
			return transaction;
		}

		// Token: 0x0600002B RID: 43 RVA: 0x0000260D File Offset: 0x0000080D
		public void NotifyPacketToServer(Packet packet)
		{
			this.PacketToServer(packet);
		}

		// Token: 0x0600002C RID: 44 RVA: 0x0000261B File Offset: 0x0000081B
		public void NotifyPacket(User user, Packet packet)
		{
			this.PacketToClient(user, packet);
		}

		// Token: 0x0600002D RID: 45 RVA: 0x0000262C File Offset: 0x0000082C
		public void BroadcastPacket(Packet packet)
		{
			foreach (User user in this.users)
			{
				this.NotifyPacket(user, packet);
			}
		}

		// Token: 0x0600002E RID: 46 RVA: 0x00002680 File Offset: 0x00000880
		public void BroadcastPacketExcept(Packet packet, User excludedUser)
		{
			foreach (User user in this.users)
			{
				if (user != excludedUser)
				{
					this.NotifyPacket(user, packet);
				}
			}
		}

		// Token: 0x0600002F RID: 47 RVA: 0x000026D8 File Offset: 0x000008D8
		public User ServerAddUser(string name, int id)
		{
			User user = new User
			{
				id = id,
				name = name,
				connected = true,
				inGame = false
			};
			this.AddUser(user);
			this.EmitLog(LogLevel.INFO, string.Format("Created user {0} ({1})", name, id));
			return user;
		}

		// Token: 0x06000030 RID: 48 RVA: 0x00002728 File Offset: 0x00000928
		public void ServerPostMessage(User user, string message)
		{
			if (message.Length >= 1)
			{
				string text = TextHelper.StripRichText(message, new string[]
				{
					"size"
				});
				text = TextHelper.Clamp(text, 1, 250, '-');
				ChatMessage message2 = new ChatMessage
				{
					user = user,
					message = text
				};
				this.AddChatMessage(message2);
				this.EmitLog(LogLevel.INFO, string.Format("{0}: {1}", user.name, message));
				this.BroadcastPacket(new ChatMessagePacket
				{
					message = message2
				});
			}
		}

		// Token: 0x06000031 RID: 49 RVA: 0x000027A8 File Offset: 0x000009A8
		public RealmThing ToRealmThing(Thing thing)
		{
			string stuffDefName = (thing.Stuff != null) ? thing.Stuff.defName : "";
			int compQuality = -1;
			if (thing.TryGetComp<CompQuality>() != null)
			{
				compQuality = (int)thing.TryGetComp<CompQuality>().Quality;
			}
			RealmThing innerThing = null;
			if (thing is MinifiedThing)
			{
				MinifiedThing minifiedThing = (MinifiedThing)thing;
				innerThing = this.ToRealmThing(minifiedThing.InnerThing);
			}
			return new RealmThing
			{
				thingDefName = thing.def.defName,
				stackCount = thing.stackCount,
				stuffDefName = stuffDefName,
				compQuality = compQuality,
				hitPoints = thing.HitPoints,
				innerThing = innerThing
			};
		}

		// Token: 0x06000032 RID: 50 RVA: 0x00002848 File Offset: 0x00000A48
		public Thing FromRealmThing(RealmThing realmThing)
		{
			ThingDef def2 = DefDatabase<ThingDef>.AllDefs.First((ThingDef def) => def.defName == realmThing.thingDefName);
			ThingDef stuff = null;
			if (realmThing.stuffDefName != "")
			{
				stuff = DefDatabase<ThingDef>.AllDefs.First((ThingDef def) => def.defName == realmThing.stuffDefName);
			}
			Thing thing = ThingMaker.MakeThing(def2, stuff);
			thing.stackCount = realmThing.stackCount;
			CompQuality compQuality = thing.TryGetComp<CompQuality>();
			if (compQuality != null && realmThing.compQuality != -1)
			{
				compQuality.SetQuality((QualityCategory)realmThing.compQuality, ArtGenerationContext.Outsider);
			}
			thing.HitPoints = realmThing.hitPoints;
			if (thing is MinifiedThing)
			{
				((MinifiedThing)thing).InnerThing = this.FromRealmThing(realmThing.innerThing);
			}
			return thing;
		}

		// Token: 0x06000033 RID: 51 RVA: 0x00002924 File Offset: 0x00000B24
		[OnSerializing]
		internal void OnSerializingCallback(StreamingContext c)
		{
			int index = Math.Max(0, this.chat.Count - 30);
			int count = Math.Min(this.chat.Count, 30);
			this.serializeChat = this.chat.GetRange(index, count);
			this.serializeTransactions = new List<Transaction>();
		}

		// Token: 0x06000034 RID: 52 RVA: 0x00002977 File Offset: 0x00000B77
		[OnDeserialized]
		internal void OnDeserializedCallback(StreamingContext c)
		{
			this.chat = this.serializeChat;
			this.transactions = this.serializeTransactions;
		}

		// Token: 0x04000020 RID: 32
		public const string VERSION = "0.14.1";

		// Token: 0x04000021 RID: 33
		public const int CHAT_MESSAGES_TO_SEND = 30;

		// Token: 0x04000022 RID: 34
		public const int CHAT_MESSAGE_MAX_LENGTH = 250;

		// Token: 0x04000023 RID: 35
		public List<User> users = new List<User>();

		// Token: 0x04000024 RID: 36
		[NonSerialized]
		public List<ChatMessage> chat = new List<ChatMessage>();

		// Token: 0x04000025 RID: 37
		private List<ChatMessage> serializeChat;

		// Token: 0x04000026 RID: 38
		[NonSerialized]
		public List<Transaction> transactions = new List<Transaction>();

		// Token: 0x04000027 RID: 39
		private List<Transaction> serializeTransactions;

		// Token: 0x04000028 RID: 40
		public int lastUserGivenId;

		// Token: 0x0200002F RID: 47
		// (Invoke) Token: 0x06000084 RID: 132
		public delegate void PacketHandler(User user, Packet packet);

		// Token: 0x02000030 RID: 48
		// (Invoke) Token: 0x06000088 RID: 136
		public delegate void LogHandler(LogLevel level, string message);
	}
}
