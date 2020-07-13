using System;
using PhiClient.Legacy;
using Verse;

namespace PhiClient
{
	// Token: 0x0200000B RID: 11
	[Serializable]
	public class ErrorPacket : Packet
	{
		// Token: 0x06000011 RID: 17 RVA: 0x0000220C File Offset: 0x0000040C
		public override void Apply(User user, RealmData realmData)
		{
			Dialog_Confirm window = new Dialog_Confirm(this.error, delegate()
			{
			}, false, null, true);
			Find.WindowStack.Add(window);
		}

		// Token: 0x04000017 RID: 23
		public string error;
	}
}
