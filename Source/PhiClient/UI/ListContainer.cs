using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace PhiClient.UI
{
	// Token: 0x0200000E RID: 14
	[StaticConstructorOnStartup]
	internal class ListContainer : Displayable
	{
		// Token: 0x0600006E RID: 110 RVA: 0x00003D2F File Offset: 0x00001F2F
		public ListContainer(List<Displayable> children, ListFlow flow = ListFlow.COLUMN, ListDirection direction = ListDirection.NORMAL)
		{
			this.children = children;
			this.flow = flow;
			this.direction = direction;
		}

		// Token: 0x0600006F RID: 111 RVA: 0x00003D4C File Offset: 0x00001F4C
		public ListContainer() : this(new List<Displayable>(), ListFlow.COLUMN, ListDirection.NORMAL)
		{
		}

		// Token: 0x06000070 RID: 112 RVA: 0x00003D5B File Offset: 0x00001F5B
		public ListContainer(ListFlow flow = ListFlow.COLUMN, ListDirection direction = ListDirection.NORMAL) : this(new List<Displayable>(), flow, direction)
		{
		}

		// Token: 0x06000071 RID: 113 RVA: 0x00003D6A File Offset: 0x00001F6A
		public void Add(Displayable display)
		{
			this.children.Add(display);
		}

		// Token: 0x06000072 RID: 114 RVA: 0x00003D78 File Offset: 0x00001F78
		public override float CalcHeight(float width)
		{
			if (this.IsFluidHeight())
			{
				return -1f;
			}
			return this.children.Sum((Displayable c) => c.CalcHeight(width)) + (float)(this.children.Count - 1) * this.spaceBetween;
		}

		// Token: 0x06000073 RID: 115 RVA: 0x00003DD0 File Offset: 0x00001FD0
		public override float CalcWidth(float height)
		{
			if (this.IsFluidWidth())
			{
				return -1f;
			}
			return this.children.Sum((Displayable c) => c.CalcWidth(height));
		}

		// Token: 0x06000074 RID: 116 RVA: 0x00003E0F File Offset: 0x0000200F
		public override void Draw(Rect inRect)
		{
			GUI.BeginGroup(inRect);
			if (this.flow == ListFlow.COLUMN)
			{
				this.DrawColumn(inRect);
			}
			else
			{
				this.DrawRow(inRect);
			}
			GUI.EndGroup();
		}

		// Token: 0x06000075 RID: 117 RVA: 0x00003E38 File Offset: 0x00002038
		private void DrawRow(Rect inRect)
		{
			float height = inRect.height;
			int countFluidElements = 0;
			float num = (!this.IsFluidWidth()) ? inRect.width : this.children.Sum(delegate(Displayable c)
			{
				float num6 = c.CalcWidth(height);
				if (num6 != -1f)
				{
					return num6;
				}
				countFluidElements++;
				return 0f;
			});
			float num2 = inRect.width - num;
			num2 -= (float)(this.children.Count - 1) * this.spaceBetween;
			num2 /= (float)countFluidElements;
			float num3 = 0f;
			if (this.direction == ListDirection.OPPOSITE)
			{
				num3 += inRect.width;
			}
			int num4 = 0;
			foreach (Displayable displayable in this.children)
			{
				float num5 = displayable.CalcWidth(height);
				if (num5 == -1f)
				{
					num5 = num2;
				}
				if (this.direction == ListDirection.OPPOSITE)
				{
					num3 -= num5 + this.spaceBetween;
				}
				Rect rect = new Rect(num3, 0f, num5, height);
				GUI.BeginGroup(rect);
				rect.x = 0f;
				if (this.drawAlternateBackground && num4 % 2 == 1)
				{
					GUI.DrawTexture(rect, ListContainer.alternateBackground);
				}
				displayable.Draw(rect);
				GUI.EndGroup();
				if (this.direction == ListDirection.NORMAL)
				{
					num3 += num5 + this.spaceBetween;
				}
				num4++;
			}
		}

		// Token: 0x06000076 RID: 118 RVA: 0x00003FB4 File Offset: 0x000021B4
		private void DrawColumn(Rect inRect)
		{
			float width = inRect.width;
			int countFluidElements = 0;
			float num = (!this.IsFluidWidth()) ? inRect.height : this.children.Sum(delegate(Displayable c)
			{
				float num6 = c.CalcHeight(width);
				if (num6 != -1f)
				{
					return num6;
				}
				countFluidElements++;
				return 0f;
			});
			float num2 = inRect.height - num;
			num2 -= (float)(this.children.Count - 1) * this.spaceBetween;
			num2 /= (float)countFluidElements;
			float num3 = 0f;
			if (this.direction == ListDirection.OPPOSITE)
			{
				num3 += inRect.height;
			}
			int num4 = 0;
			foreach (Displayable displayable in this.children)
			{
				float num5 = displayable.CalcHeight(width);
				if (num5 == -1f)
				{
					num5 = num2;
				}
				if (this.direction == ListDirection.OPPOSITE)
				{
					num3 -= num5 + this.spaceBetween;
				}
				Rect rect = new Rect(0f, num3, width, num5);
				GUI.BeginGroup(rect);
				rect.y = 0f;
				if (this.drawAlternateBackground && num4 % 2 == 1)
				{
					GUI.DrawTexture(rect, ListContainer.alternateBackground);
				}
				displayable.Draw(rect);
				GUI.EndGroup();
				if (this.direction == ListDirection.NORMAL)
				{
					num3 += num5 + this.spaceBetween;
				}
				num4++;
			}
		}

		// Token: 0x06000077 RID: 119 RVA: 0x00004130 File Offset: 0x00002330
		private int CountFluidHeight()
		{
			return this.children.Count((Displayable c) => c.IsFluidHeight());
		}

		// Token: 0x06000078 RID: 120 RVA: 0x0000415C File Offset: 0x0000235C
		private int CountFluidWidth()
		{
			return this.children.Count((Displayable c) => c.IsFluidWidth());
		}

		// Token: 0x06000079 RID: 121 RVA: 0x00004188 File Offset: 0x00002388
		public override bool IsFluidHeight()
		{
			if (this.flow == ListFlow.COLUMN)
			{
				return this.children.Any((Displayable c) => c.IsFluidHeight());
			}
			return true;
		}

		// Token: 0x0600007A RID: 122 RVA: 0x000041BF File Offset: 0x000023BF
		public override bool IsFluidWidth()
		{
			if (this.flow == ListFlow.ROW)
			{
				return this.children.Any((Displayable c) => c.IsFluidWidth());
			}
			return true;
		}

		// Token: 0x04000031 RID: 49
		public static readonly Texture2D alternateBackground = SolidColorMaterials.NewSolidColorTexture(new Color(1f, 1f, 1f, 0.04f));

		// Token: 0x04000032 RID: 50
		public const float SPACE = 10f;

		// Token: 0x04000033 RID: 51
		private List<Displayable> children;

		// Token: 0x04000034 RID: 52
		private ListFlow flow;

		// Token: 0x04000035 RID: 53
		private ListDirection direction;

		// Token: 0x04000036 RID: 54
		public float spaceBetween;

		// Token: 0x04000037 RID: 55
		public bool drawAlternateBackground;
	}
}
