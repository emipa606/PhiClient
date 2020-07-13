using System;

namespace PhiClient
{
	// Token: 0x0200000A RID: 10
	[Serializable]
	public class ChatMessagePacket : Packet
	{
		// Token: 0x0600000F RID: 15 RVA: 0x000021FE File Offset: 0x000003FE
		public override void Apply(User user, RealmData realmData)
		{
			realmData.AddChatMessage(this.message);
		}

		// Token: 0x04000016 RID: 22
		public ChatMessage message;
	}
}
