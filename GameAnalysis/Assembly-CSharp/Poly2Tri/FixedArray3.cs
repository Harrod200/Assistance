using System;
using System.Collections;
using System.Collections.Generic;

namespace Poly2Tri
{
	// Token: 0x020004E5 RID: 1253
	public struct FixedArray3<T> : IEnumerable<T>, IEnumerable where T : class
	{
		// Token: 0x1700042C RID: 1068
		public T this[int index]
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

		// Token: 0x06001D52 RID: 7506 RVA: 0x0009B4F8 File Offset: 0x000996F8
		public bool Contains(T value)
		{
			for (int i = 0; i < 3; i++)
			{
				if (this[i] != null && this[i].Equals(value))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06001D53 RID: 7507 RVA: 0x0009B53C File Offset: 0x0009973C
		public int IndexOf(T value)
		{
			for (int i = 0; i < 3; i++)
			{
				if (this[i] != null && this[i].Equals(value))
				{
					return i;
				}
			}
			return -1;
		}

		// Token: 0x06001D54 RID: 7508 RVA: 0x0009B580 File Offset: 0x00099780
		public void Clear()
		{
			this._0 = (this._1 = (this._2 = default(T)));
		}

		// Token: 0x06001D55 RID: 7509 RVA: 0x0009B5B0 File Offset: 0x000997B0
		public void Clear(T value)
		{
			for (int i = 0; i < 3; i++)
			{
				if (this[i] != null && this[i].Equals(value))
				{
					this[i] = default(T);
				}
			}
		}

		// Token: 0x06001D56 RID: 7510 RVA: 0x0009B600 File Offset: 0x00099800
		private IEnumerable<T> Enumerate()
		{
			int num;
			for (int i = 0; i < 3; i = num)
			{
				yield return this[i];
				num = i + 1;
			}
			yield break;
		}

		// Token: 0x06001D57 RID: 7511 RVA: 0x0009B615 File Offset: 0x00099815
		public IEnumerator<T> GetEnumerator()
		{
			return this.Enumerate().GetEnumerator();
		}

		// Token: 0x06001D58 RID: 7512 RVA: 0x0009B622 File Offset: 0x00099822
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		// Token: 0x040017C5 RID: 6085
		public T _0;

		// Token: 0x040017C6 RID: 6086
		public T _1;

		// Token: 0x040017C7 RID: 6087
		public T _2;
	}
}
