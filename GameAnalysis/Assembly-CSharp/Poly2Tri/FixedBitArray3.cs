using System;
using System.Collections;
using System.Collections.Generic;

namespace Poly2Tri
{
	// Token: 0x020004E6 RID: 1254
	public struct FixedBitArray3 : IEnumerable<bool>, IEnumerable
	{
		// Token: 0x1700042D RID: 1069
		public bool this[int index]
		{
			get
			{
				switch (index)
				{
				case 0:
					return this._0;
				case 1:
					return this._1;
				case 2:
					return this._2;
				default:
					throw new IndexOutOfRangeException();
				}
			}
			set
			{
				switch (index)
				{
				case 0:
					this._0 = value;
					return;
				case 1:
					this._1 = value;
					return;
				case 2:
					this._2 = value;
					return;
				default:
					throw new IndexOutOfRangeException();
				}
			}
		}

		// Token: 0x06001D5B RID: 7515 RVA: 0x0009B690 File Offset: 0x00099890
		public bool Contains(bool value)
		{
			for (int i = 0; i < 3; i++)
			{
				if (this[i] == value)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06001D5C RID: 7516 RVA: 0x0009B6B8 File Offset: 0x000998B8
		public int IndexOf(bool value)
		{
			for (int i = 0; i < 3; i++)
			{
				if (this[i] == value)
				{
					return i;
				}
			}
			return -1;
		}

		// Token: 0x06001D5D RID: 7517 RVA: 0x0009B6E0 File Offset: 0x000998E0
		public void Clear()
		{
			this._0 = (this._1 = (this._2 = false));
		}

		// Token: 0x06001D5E RID: 7518 RVA: 0x0009B708 File Offset: 0x00099908
		public void Clear(bool value)
		{
			for (int i = 0; i < 3; i++)
			{
				if (this[i] == value)
				{
					this[i] = false;
				}
			}
		}

		// Token: 0x06001D5F RID: 7519 RVA: 0x0009B733 File Offset: 0x00099933
		private IEnumerable<bool> Enumerate()
		{
			int num;
			for (int i = 0; i < 3; i = num)
			{
				yield return this[i];
				num = i + 1;
			}
			yield break;
		}

		// Token: 0x06001D60 RID: 7520 RVA: 0x0009B748 File Offset: 0x00099948
		public IEnumerator<bool> GetEnumerator()
		{
			return this.Enumerate().GetEnumerator();
		}

		// Token: 0x06001D61 RID: 7521 RVA: 0x0009B755 File Offset: 0x00099955
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		// Token: 0x040017C8 RID: 6088
		public bool _0;

		// Token: 0x040017C9 RID: 6089
		public bool _1;

		// Token: 0x040017CA RID: 6090
		public bool _2;
	}
}
