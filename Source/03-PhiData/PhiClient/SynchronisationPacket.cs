using System;
using System.Runtime.Serialization;

namespace PhiClient
{
	// Token: 0x0200001C RID: 28
	[Serializable]
	public class SynchronisationPacket : Packet
	{
		// Token: 0x06000046 RID: 70 RVA: 0x0000208E File Offset: 0x0000028E
		public override void Apply(User user, RealmData realmData)
		{
		}

		// Token: 0x06000047 RID: 71 RVA: 0x000039EA File Offset: 0x00001BEA
		[OnSerializing]
		internal void OnSerializingCallback(StreamingContext c)
		{
			this.userId = this.user.id;
		}

		// Token: 0x06000048 RID: 72 RVA: 0x000039FD File Offset: 0x00001BFD
		[OnDeserialized]
		internal void OnDeserializedCallback(StreamingContext c)
		{
			this.user = ID.Find<User>(this.realmData.users, this.userId);
		}

		// Token: 0x0400005C RID: 92
		public RealmData realmData;

		// Token: 0x0400005D RID: 93
		[NonSerialized]
		public User user;

		// Token: 0x0400005E RID: 94
		private int userId;
	}
}
