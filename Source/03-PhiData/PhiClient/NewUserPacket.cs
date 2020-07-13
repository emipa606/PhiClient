using System;

namespace PhiClient
{
	// Token: 0x0200000F RID: 15
	[Serializable]
	public class NewUserPacket : Packet
	{
		// Token: 0x06000017 RID: 23 RVA: 0x00002310 File Offset: 0x00000510
		public override void Apply(User user, RealmData realmData)
		{
			realmData.AddUser(this.user);
		}

		// Token: 0x0400001C RID: 28
		public User user;
	}
}
