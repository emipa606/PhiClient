using System;
using System.Runtime.Serialization;

namespace PhiClient
{
	// Token: 0x0200001A RID: 26
	[Serializable]
	public class ReceivePawnPacket : Packet
	{
		// Token: 0x0600003E RID: 62 RVA: 0x0000208E File Offset: 0x0000028E
		public override void Apply(User user, RealmData realmData)
		{
		}

		// Token: 0x0600003F RID: 63 RVA: 0x0000392F File Offset: 0x00001B2F
		[OnSerializing]
		internal void OnSerializingCallback(StreamingContext c)
		{
			this.userFromId = this.userFrom.id;
		}

		// Token: 0x06000040 RID: 64 RVA: 0x00003944 File Offset: 0x00001B44
		[OnDeserialized]
		internal void OnDeserializedCallback(StreamingContext c)
		{
			RealmContext realmContext = (RealmContext)c.Context;
			this.userFrom = ID.Find<User>(realmContext.realmData.users, this.userFromId);
		}

		// Token: 0x04000056 RID: 86
		[NonSerialized]
		public User userFrom;

		// Token: 0x04000057 RID: 87
		private int userFromId;

		// Token: 0x04000058 RID: 88
		public RealmPawn realmPawn;
	}
}
