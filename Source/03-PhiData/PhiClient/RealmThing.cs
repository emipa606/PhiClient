using System;

namespace PhiClient
{
	// Token: 0x02000017 RID: 23
	[Serializable]
	public class RealmThing
	{
		// Token: 0x0400004A RID: 74
		public int senderThingId;

		// Token: 0x0400004B RID: 75
		public string thingDefName;

		// Token: 0x0400004C RID: 76
		public string stuffDefName;

		// Token: 0x0400004D RID: 77
		public int compQuality;

		// Token: 0x0400004E RID: 78
		public int stackCount;

		// Token: 0x0400004F RID: 79
		public int hitPoints;

		// Token: 0x04000050 RID: 80
		public RealmThing innerThing;
	}
}
