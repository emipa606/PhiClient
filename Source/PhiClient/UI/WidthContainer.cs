using System;

namespace PhiClient.UI
{
	// Token: 0x02000017 RID: 23
	internal class WidthContainer : Container
	{
		// Token: 0x0600008D RID: 141 RVA: 0x000044D9 File Offset: 0x000026D9
		public WidthContainer(Displayable child, float width) : base(child, width, -1f)
		{
		}

		// Token: 0x0600008E RID: 142 RVA: 0x00003C7F File Offset: 0x00001E7F
		public override bool IsFluidWidth()
		{
			return false;
		}
	}
}
