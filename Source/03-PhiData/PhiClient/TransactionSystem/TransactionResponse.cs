using System;

namespace PhiClient.TransactionSystem
{
	// Token: 0x0200002A RID: 42
	public enum TransactionResponse
	{
		// Token: 0x0400008D RID: 141
		WAITING,
		// Token: 0x0400008E RID: 142
		ACCEPTED,
		// Token: 0x0400008F RID: 143
		DECLINED,
		// Token: 0x04000090 RID: 144
		INTERRUPTED,
		// Token: 0x04000091 RID: 145
		TOOFAST
	}
}
