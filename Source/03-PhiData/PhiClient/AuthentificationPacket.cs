using System;

namespace PhiClient
{
	// Token: 0x02000006 RID: 6
	[Serializable]
	public class AuthentificationPacket : Packet
	{
		// Token: 0x06000004 RID: 4 RVA: 0x0000208E File Offset: 0x0000028E
		public override void Apply(User user, RealmData realmData)
		{
		}

		// Token: 0x0400000B RID: 11
		public string name;

		// Token: 0x0400000C RID: 12
		public int? id;

		// Token: 0x0400000D RID: 13
		public string hashedKey;

		// Token: 0x0400000E RID: 14
		public string version;
	}
}
