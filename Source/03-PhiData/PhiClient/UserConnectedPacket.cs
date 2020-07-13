using System;
using System.Runtime.Serialization;

namespace PhiClient
{
	// Token: 0x02000021 RID: 33
	[Serializable]
	public class UserConnectedPacket : Packet
	{
		// Token: 0x06000055 RID: 85 RVA: 0x00003B7D File Offset: 0x00001D7D
		public override void Apply(User cUser, RealmData realmData)
		{
			this.user.connected = this.connected;
		}

		// Token: 0x06000056 RID: 86 RVA: 0x00003B90 File Offset: 0x00001D90
		[OnSerializing]
		internal void OnSerializingCallback(StreamingContext c)
		{
			this.userId = this.user.id;
		}

		// Token: 0x06000057 RID: 87 RVA: 0x00003BA4 File Offset: 0x00001DA4
		[OnDeserialized]
		internal void OnDeserializedCallback(StreamingContext c)
		{
			RealmContext realmContext = (RealmContext)c.Context;
			this.user = ID.Find<User>(realmContext.realmData.users, this.userId);
		}

		// Token: 0x04000070 RID: 112
		[NonSerialized]
		public User user;

		// Token: 0x04000071 RID: 113
		public int userId;

		// Token: 0x04000072 RID: 114
		public bool connected;
	}
}
