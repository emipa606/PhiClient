using System;

namespace PhiClient.UI
{
	// Token: 0x0200000D RID: 13
	internal class HeightContainer : Container
	{
		// Token: 0x0600006C RID: 108 RVA: 0x00003D20 File Offset: 0x00001F20
		public HeightContainer(Displayable child, float height) : base(child, -1f, height)
		{
		}

		// Token: 0x0600006D RID: 109 RVA: 0x00003C7F File Offset: 0x00001E7F
		public override bool IsFluidHeight()
		{
			return false;
		}
	}
}
