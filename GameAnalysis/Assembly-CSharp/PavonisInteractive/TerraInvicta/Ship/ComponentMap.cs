using System;

namespace PavonisInteractive.TerraInvicta.Ship
{
	// Token: 0x02000968 RID: 2408
	public class ComponentMap : IEquatable<ComponentMap>, ICloneable
	{
		// Token: 0x17000F91 RID: 3985
		// (get) Token: 0x06005BBA RID: 23482 RVA: 0x002BF67F File Offset: 0x002BD87F
		public static ComponentMap single
		{
			get
			{
				return new ComponentMap(1, 1, 1UL);
			}
		}

		// Token: 0x06005BBB RID: 23483 RVA: 0x002BF68A File Offset: 0x002BD88A
		private static ulong ToUInt64(string map)
		{
			map = map.Replace(" ", string.Empty).Replace("_", string.Empty);
			return Convert.ToUInt64(map, 2);
		}

		// Token: 0x06005BBC RID: 23484 RVA: 0x002BF6B4 File Offset: 0x002BD8B4
		public ComponentMap(int height, int width, ulong map)
		{
			this.height = height;
			this.width = width;
			this.map = map;
			this.size = Utilities.CountBits(map);
		}

		// Token: 0x06005BBD RID: 23485 RVA: 0x002BF6DD File Offset: 0x002BD8DD
		public ComponentMap(int height, int width, string map)
			: this(height, width, ComponentMap.ToUInt64(map))
		{
		}

		// Token: 0x06005BBE RID: 23486 RVA: 0x002BF6ED File Offset: 0x002BD8ED
		public ComponentMap(ComponentMap other)
		{
			this.height = other.height;
			this.width = other.width;
			this.size = other.size;
			this.map = other.map;
		}

		// Token: 0x06005BBF RID: 23487 RVA: 0x002BF728 File Offset: 0x002BD928
		public bool CanAttach(ComponentMap item)
		{
			return !Error.IsNull<ComponentMap>(item) && item.height <= this.height && item.width <= this.width && item.size <= this.size && (item.size == 1 || this.FindMatch(item));
		}

		// Token: 0x06005BC0 RID: 23488 RVA: 0x002BF77D File Offset: 0x002BD97D
		public bool CanAttach(ComponentMap item, int heightOffset, int widthOffset)
		{
			return !Error.IsNull<ComponentMap>(item) && this.CheckAttach(item, heightOffset, widthOffset);
		}

		// Token: 0x06005BC1 RID: 23489 RVA: 0x002BF792 File Offset: 0x002BD992
		public bool Attach(ComponentMap item, int heightOffset, int widthOffset)
		{
			if (this.CanAttach(item, heightOffset, widthOffset))
			{
				this.map ^= item.BuildMask(this.width, heightOffset, widthOffset);
				this.size -= item.size;
				return true;
			}
			return false;
		}

		// Token: 0x06005BC2 RID: 23490 RVA: 0x002BF7D0 File Offset: 0x002BD9D0
		public bool Detach(ComponentMap item, int heightOffset, int widthOffset)
		{
			if (this.CheckDetach(item, heightOffset, widthOffset))
			{
				this.map |= item.BuildMask(this.width, heightOffset, widthOffset);
				this.size += item.size;
				return true;
			}
			return false;
		}

		// Token: 0x06005BC3 RID: 23491 RVA: 0x002BF80E File Offset: 0x002BDA0E
		public override string ToString()
		{
			return string.Format("({0},{1}) : {2}", this.height, this.width, this.map);
		}

		// Token: 0x06005BC4 RID: 23492 RVA: 0x002BF83B File Offset: 0x002BDA3B
		public object Clone()
		{
			return new ComponentMap(this);
		}

		// Token: 0x06005BC5 RID: 23493 RVA: 0x002BF843 File Offset: 0x002BDA43
		public bool Equals(ComponentMap other)
		{
			return other.map == this.map && other.height == this.height && other.width == this.width;
		}

		// Token: 0x06005BC6 RID: 23494 RVA: 0x002BF874 File Offset: 0x002BDA74
		private ulong BuildMask(int width, int heightOffset, int widthOffset)
		{
			ulong num = 0UL;
			for (int i = 0; i < this.height; i++)
			{
				num |= this.Row(i) << (i + heightOffset) * width + widthOffset;
			}
			return num;
		}

		// Token: 0x06005BC7 RID: 23495 RVA: 0x002BF8AC File Offset: 0x002BDAAC
		private bool FindMatch(ComponentMap item)
		{
			for (int i = 0; i <= this.height - item.height; i++)
			{
				for (int j = 0; j <= this.width - item.width; j++)
				{
					if (this.CheckAttach(item, i, j))
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06005BC8 RID: 23496 RVA: 0x002BF8F8 File Offset: 0x002BDAF8
		private bool CheckDetach(ComponentMap item, int heightOffset, int widthOffset)
		{
			ulong num = item.BuildMask(this.width, heightOffset, widthOffset);
			return (this.map & num) == 0UL;
		}

		// Token: 0x06005BC9 RID: 23497 RVA: 0x002BF920 File Offset: 0x002BDB20
		private bool CheckAttach(ComponentMap item, int heightOffset, int widthOffset)
		{
			ulong num = item.BuildMask(this.width, heightOffset, widthOffset);
			return (this.map & num) == num;
		}

		// Token: 0x06005BCA RID: 23498 RVA: 0x002BF948 File Offset: 0x002BDB48
		private ulong Row(int i)
		{
			ulong num = (ulong)((ulong)((long)((1 << this.width) - 1)) << i * this.width);
			return (this.map & num) >> i * this.width;
		}

		// Token: 0x040041A3 RID: 16803
		private ulong map;

		// Token: 0x040041A4 RID: 16804
		private int height;

		// Token: 0x040041A5 RID: 16805
		private int width;

		// Token: 0x040041A6 RID: 16806
		private int size;
	}
}
