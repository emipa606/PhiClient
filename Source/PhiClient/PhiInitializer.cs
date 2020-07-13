using System;
using HugsLib;

namespace PhiClient
{
	// Token: 0x02000003 RID: 3
	public class PhiInitializer : ModBase
	{
		// Token: 0x0600001E RID: 30 RVA: 0x0000281C File Offset: 0x00000A1C
		public PhiInitializer()
		{
			new PhiClient().TryConnect();
		}

		// Token: 0x17000001 RID: 1
		// (get) Token: 0x0600001F RID: 31 RVA: 0x0000282E File Offset: 0x00000A2E
		public override string ModIdentifier
		{
			get
			{
				return "Phi";
			}
		}

		// Token: 0x06000020 RID: 32 RVA: 0x00002835 File Offset: 0x00000A35
		public override void Update()
		{
			PhiClient.instance.OnUpdate();
		}
	}
}
