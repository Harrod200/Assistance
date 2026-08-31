using System;

namespace ModelShark
{
	// Token: 0x020004C0 RID: 1216
	public class TooltipOverflow
	{
		// Token: 0x170003C0 RID: 960
		// (get) Token: 0x06001B6A RID: 7018 RVA: 0x00093DBA File Offset: 0x00091FBA
		public bool IsAny
		{
			get
			{
				return this.BottomLeftCorner || this.TopLeftCorner || this.TopRightCorner || this.BottomRightCorner;
			}
		}

		// Token: 0x170003C1 RID: 961
		// (get) Token: 0x06001B6B RID: 7019 RVA: 0x00093DDC File Offset: 0x00091FDC
		public bool TopEdge
		{
			get
			{
				return this.TopLeftCorner && this.TopRightCorner;
			}
		}

		// Token: 0x170003C2 RID: 962
		// (get) Token: 0x06001B6C RID: 7020 RVA: 0x00093DEE File Offset: 0x00091FEE
		public bool RightEdge
		{
			get
			{
				return this.TopRightCorner && this.BottomRightCorner;
			}
		}

		// Token: 0x170003C3 RID: 963
		// (get) Token: 0x06001B6D RID: 7021 RVA: 0x00093E00 File Offset: 0x00092000
		public bool LeftEdge
		{
			get
			{
				return this.TopLeftCorner && this.BottomLeftCorner;
			}
		}

		// Token: 0x170003C4 RID: 964
		// (get) Token: 0x06001B6E RID: 7022 RVA: 0x00093E12 File Offset: 0x00092012
		public bool BottomEdge
		{
			get
			{
				return this.BottomLeftCorner && this.BottomRightCorner;
			}
		}

		// Token: 0x170003C5 RID: 965
		// (get) Token: 0x06001B6F RID: 7023 RVA: 0x00093E24 File Offset: 0x00092024
		// (set) Token: 0x06001B70 RID: 7024 RVA: 0x00093E2C File Offset: 0x0009202C
		public bool TopRightCorner { get; set; }

		// Token: 0x170003C6 RID: 966
		// (get) Token: 0x06001B71 RID: 7025 RVA: 0x00093E35 File Offset: 0x00092035
		// (set) Token: 0x06001B72 RID: 7026 RVA: 0x00093E3D File Offset: 0x0009203D
		public bool TopLeftCorner { get; set; }

		// Token: 0x170003C7 RID: 967
		// (get) Token: 0x06001B73 RID: 7027 RVA: 0x00093E46 File Offset: 0x00092046
		// (set) Token: 0x06001B74 RID: 7028 RVA: 0x00093E4E File Offset: 0x0009204E
		public bool BottomRightCorner { get; set; }

		// Token: 0x170003C8 RID: 968
		// (get) Token: 0x06001B75 RID: 7029 RVA: 0x00093E57 File Offset: 0x00092057
		// (set) Token: 0x06001B76 RID: 7030 RVA: 0x00093E5F File Offset: 0x0009205F
		public bool BottomLeftCorner { get; set; }

		// Token: 0x06001B77 RID: 7031 RVA: 0x00093E68 File Offset: 0x00092068
		public TipPosition SuggestNewPosition(TipPosition fromPosition)
		{
			bool flag = fromPosition == TipPosition.MouseBottomLeftCorner || fromPosition == TipPosition.MouseTopLeftCorner || fromPosition == TipPosition.MouseBottomRightCorner || fromPosition == TipPosition.MouseTopRightCorner || fromPosition == TipPosition.MouseTopMiddle || fromPosition == TipPosition.MouseLeftMiddle || fromPosition == TipPosition.MouseRightMiddle || fromPosition == TipPosition.MouseBottomMiddle;
			switch (fromPosition)
			{
			case TipPosition.TopRightCorner:
			case TipPosition.MouseTopRightCorner:
				if (this.TopEdge && this.RightEdge)
				{
					if (!flag)
					{
						return TipPosition.BottomLeftCorner;
					}
					return TipPosition.MouseBottomLeftCorner;
				}
				else if (this.TopEdge)
				{
					if (!flag)
					{
						return TipPosition.BottomRightCorner;
					}
					return TipPosition.MouseBottomRightCorner;
				}
				else if (this.RightEdge)
				{
					if (!flag)
					{
						return TipPosition.TopLeftCorner;
					}
					return TipPosition.MouseTopLeftCorner;
				}
				break;
			case TipPosition.BottomRightCorner:
			case TipPosition.MouseBottomRightCorner:
				if (this.BottomEdge && this.RightEdge)
				{
					if (!flag)
					{
						return TipPosition.TopLeftCorner;
					}
					return TipPosition.MouseTopLeftCorner;
				}
				else if (this.BottomEdge)
				{
					if (!flag)
					{
						return TipPosition.TopRightCorner;
					}
					return TipPosition.MouseTopRightCorner;
				}
				else if (this.RightEdge)
				{
					if (!flag)
					{
						return TipPosition.BottomLeftCorner;
					}
					return TipPosition.MouseBottomLeftCorner;
				}
				break;
			case TipPosition.TopLeftCorner:
			case TipPosition.MouseTopLeftCorner:
				if (this.TopEdge && this.LeftEdge)
				{
					if (!flag)
					{
						return TipPosition.BottomRightCorner;
					}
					return TipPosition.MouseBottomRightCorner;
				}
				else if (this.TopEdge)
				{
					if (!flag)
					{
						return TipPosition.BottomLeftCorner;
					}
					return TipPosition.MouseBottomLeftCorner;
				}
				else if (this.LeftEdge)
				{
					if (!flag)
					{
						return TipPosition.TopRightCorner;
					}
					return TipPosition.MouseTopRightCorner;
				}
				break;
			case TipPosition.BottomLeftCorner:
			case TipPosition.MouseBottomLeftCorner:
				if (this.BottomEdge && this.LeftEdge)
				{
					if (!flag)
					{
						return TipPosition.TopRightCorner;
					}
					return TipPosition.MouseTopRightCorner;
				}
				else if (this.BottomEdge)
				{
					if (!flag)
					{
						return TipPosition.TopLeftCorner;
					}
					return TipPosition.MouseTopLeftCorner;
				}
				else if (this.LeftEdge)
				{
					if (!flag)
					{
						return TipPosition.BottomRightCorner;
					}
					return TipPosition.MouseBottomRightCorner;
				}
				break;
			case TipPosition.TopMiddle:
			case TipPosition.MouseTopMiddle:
				if (this.TopEdge && this.RightEdge)
				{
					if (!flag)
					{
						return TipPosition.BottomLeftCorner;
					}
					return TipPosition.MouseBottomLeftCorner;
				}
				else if (this.TopEdge && this.LeftEdge)
				{
					if (!flag)
					{
						return TipPosition.BottomRightCorner;
					}
					return TipPosition.MouseBottomRightCorner;
				}
				else if (this.TopEdge)
				{
					if (!flag)
					{
						return TipPosition.BottomMiddle;
					}
					return TipPosition.MouseBottomMiddle;
				}
				else if (this.RightEdge)
				{
					if (!flag)
					{
						return TipPosition.LeftMiddle;
					}
					return TipPosition.MouseLeftMiddle;
				}
				else if (this.LeftEdge)
				{
					if (!flag)
					{
						return TipPosition.RightMiddle;
					}
					return TipPosition.MouseRightMiddle;
				}
				break;
			case TipPosition.BottomMiddle:
			case TipPosition.MouseBottomMiddle:
				if (this.BottomEdge && this.RightEdge)
				{
					if (!flag)
					{
						return TipPosition.TopLeftCorner;
					}
					return TipPosition.MouseTopLeftCorner;
				}
				else if (this.BottomEdge && this.LeftEdge)
				{
					if (!flag)
					{
						return TipPosition.TopRightCorner;
					}
					return TipPosition.MouseTopRightCorner;
				}
				else if (this.BottomEdge)
				{
					if (!flag)
					{
						return TipPosition.TopMiddle;
					}
					return TipPosition.MouseTopMiddle;
				}
				else if (this.RightEdge)
				{
					if (!flag)
					{
						return TipPosition.LeftMiddle;
					}
					return TipPosition.MouseLeftMiddle;
				}
				else if (this.LeftEdge)
				{
					if (!flag)
					{
						return TipPosition.RightMiddle;
					}
					return TipPosition.MouseRightMiddle;
				}
				break;
			case TipPosition.RightMiddle:
				if (this.RightEdge)
				{
					if (!flag)
					{
						return TipPosition.LeftMiddle;
					}
					return TipPosition.MouseLeftMiddle;
				}
				break;
			case TipPosition.LeftMiddle:
				if (this.LeftEdge)
				{
					if (!flag)
					{
						return TipPosition.RightMiddle;
					}
					return TipPosition.MouseRightMiddle;
				}
				break;
			case TipPosition.MouseRightMiddle:
				if (this.TopEdge && this.RightEdge)
				{
					if (!flag)
					{
						return TipPosition.BottomLeftCorner;
					}
					return TipPosition.MouseBottomLeftCorner;
				}
				else if (this.BottomEdge && this.RightEdge)
				{
					if (!flag)
					{
						return TipPosition.TopLeftCorner;
					}
					return TipPosition.MouseTopLeftCorner;
				}
				else if (this.TopEdge)
				{
					if (!flag)
					{
						return TipPosition.BottomMiddle;
					}
					return TipPosition.MouseBottomMiddle;
				}
				else if (this.BottomEdge)
				{
					if (!flag)
					{
						return TipPosition.TopMiddle;
					}
					return TipPosition.MouseTopMiddle;
				}
				else if (this.RightEdge)
				{
					if (!flag)
					{
						return TipPosition.LeftMiddle;
					}
					return TipPosition.MouseLeftMiddle;
				}
				break;
			case TipPosition.MouseLeftMiddle:
				if (this.TopEdge && this.LeftEdge)
				{
					if (!flag)
					{
						return TipPosition.BottomRightCorner;
					}
					return TipPosition.MouseBottomRightCorner;
				}
				else if (this.BottomEdge && this.LeftEdge)
				{
					if (!flag)
					{
						return TipPosition.TopRightCorner;
					}
					return TipPosition.MouseTopRightCorner;
				}
				else if (this.TopEdge)
				{
					if (!flag)
					{
						return TipPosition.BottomMiddle;
					}
					return TipPosition.MouseBottomMiddle;
				}
				else if (this.BottomEdge)
				{
					if (!flag)
					{
						return TipPosition.TopMiddle;
					}
					return TipPosition.MouseTopMiddle;
				}
				else if (this.LeftEdge)
				{
					if (!flag)
					{
						return TipPosition.RightMiddle;
					}
					return TipPosition.MouseRightMiddle;
				}
				break;
			}
			return fromPosition;
		}
	}
}
