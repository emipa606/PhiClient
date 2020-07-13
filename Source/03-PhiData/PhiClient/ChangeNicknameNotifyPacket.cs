using System;
using System.Runtime.Serialization;

namespace PhiClient
{
	// Token: 0x02000007 RID: 7
	[Serializable]
	public class ChangeNicknameNotifyPacket : Packet
	{
		// Token: 0x06000006 RID: 6 RVA: 0x00002090 File Offset: 0x00000290
		public override void Apply(User user, RealmData realmData)
		{
			this.user.name = this.name;
		}

		// Token: 0x06000007 RID: 7 RVA: 0x000020A3 File Offset: 0x000002A3
		[OnSerializing]
		internal void OnSerializingCallback(StreamingContext c)
		{
			this.userId = this.user.id;
		}

		// Token: 0x06000008 RID: 8 RVA: 0x000020B8 File Offset: 0x000002B8
		[OnDeserialized]
		internal void OnDeserializedCallback(StreamingContext c)
		{
			RealmContext realmContext = (RealmContext)c.Context;
			this.user = ID.Find<User>(realmContext.realmData.users, this.userId);
		}

		// Token: 0x0400000F RID: 15
		[NonSerialized]
		public User user;

		// Token: 0x04000010 RID: 16
		public int userId;

		// Token: 0x04000011 RID: 17
		public string name;
	}
}
