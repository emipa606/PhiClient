using System;
using Verse;

namespace PhiClient
{
	// Token: 0x02000005 RID: 5
	[Serializable]
	public class AuthentificationErrorPacket : Packet
	{
		// Token: 0x06000002 RID: 2 RVA: 0x0000206E File Offset: 0x0000026E
		public override void Apply(User user, RealmData realmData)
		{
			Log.Warning("Couldn't authenticate:" + this.error, false);
		}

		// Token: 0x0400000A RID: 10
		public string error;
	}
}
