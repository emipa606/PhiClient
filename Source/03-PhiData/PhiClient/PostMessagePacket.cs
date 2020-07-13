using System;

namespace PhiClient
{
	// Token: 0x02000011 RID: 17
	[Serializable]
	public class PostMessagePacket : Packet
	{
		// Token: 0x0600001D RID: 29 RVA: 0x000023BF File Offset: 0x000005BF
		public override void Apply(User user, RealmData realmData)
		{
			realmData.ServerPostMessage(user, this.message);
		}

		// Token: 0x0400001D RID: 29
		public string message;
	}
}
