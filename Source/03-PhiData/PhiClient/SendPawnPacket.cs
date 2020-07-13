using System;
using System.Runtime.Serialization;

namespace PhiClient
{
	// Token: 0x0200001B RID: 27
	[Serializable]
	public class SendPawnPacket : Packet
	{
		// Token: 0x06000042 RID: 66 RVA: 0x0000397A File Offset: 0x00001B7A
		public override void Apply(User user, RealmData realmData)
		{
			realmData.NotifyPacket(this.userTo, new ReceivePawnPacket
			{
				userFrom = user,
				realmPawn = this.realmPawn
			});
		}

		// Token: 0x06000043 RID: 67 RVA: 0x000039A0 File Offset: 0x00001BA0
		[OnSerializing]
		internal void OnSerializingCallback(StreamingContext c)
		{
			this.userToId = this.userTo.id;
		}

		// Token: 0x06000044 RID: 68 RVA: 0x000039B4 File Offset: 0x00001BB4
		[OnDeserialized]
		internal void OnDeserializedCallback(StreamingContext c)
		{
			RealmContext realmContext = (RealmContext)c.Context;
			this.userTo = ID.Find<User>(realmContext.realmData.users, this.userToId);
		}

		// Token: 0x04000059 RID: 89
		public RealmPawn realmPawn;

		// Token: 0x0400005A RID: 90
		[NonSerialized]
		public User userTo;

		// Token: 0x0400005B RID: 91
		private int userToId;
	}
}
