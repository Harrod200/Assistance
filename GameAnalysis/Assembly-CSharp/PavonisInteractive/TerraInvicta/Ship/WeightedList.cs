using System;
using System.Collections.Generic;

namespace PavonisInteractive.TerraInvicta.Ship
{
	// Token: 0x02000985 RID: 2437
	public class WeightedList<T>
	{
		// Token: 0x17000FEA RID: 4074
		// (get) Token: 0x06005CB7 RID: 23735 RVA: 0x002C2D97 File Offset: 0x002C0F97
		// (set) Token: 0x06005CB8 RID: 23736 RVA: 0x002C2DA4 File Offset: 0x002C0FA4
		public int Count
		{
			get
			{
				return this.entries.Count;
			}
			private set
			{
			}
		}

		// Token: 0x06005CB9 RID: 23737 RVA: 0x002C2DA6 File Offset: 0x002C0FA6
		public WeightedList()
		{
			this.entries = new List<WeightedList<T>.Entry<T>>();
			this.random = new Random();
		}

		// Token: 0x06005CBA RID: 23738 RVA: 0x002C2DC4 File Offset: 0x002C0FC4
		public void Add(T item, int weight)
		{
			WeightedList<T>.Entry<T> entry = new WeightedList<T>.Entry<T>(item, weight);
			this.entries.Add(entry);
			checked
			{
				this.weight += unchecked((long)weight);
			}
		}

		// Token: 0x06005CBB RID: 23739 RVA: 0x002C2DF8 File Offset: 0x002C0FF8
		public void Remove(T item)
		{
			foreach (WeightedList<T>.Entry<T> entry in this.entries)
			{
				T item2 = entry.item;
				if (item2.Equals(item))
				{
					this.entries.Remove(entry);
					this.weight -= (long)entry.weight;
					break;
				}
			}
		}

		// Token: 0x06005CBC RID: 23740 RVA: 0x002C2E80 File Offset: 0x002C1080
		public T Random()
		{
			double num = this.random.NextDouble() * (double)this.weight;
			foreach (WeightedList<T>.Entry<T> entry in this.entries)
			{
				if (num < (double)entry.weight)
				{
					return entry.item;
				}
			}
			return this.entries[this.random.Next(this.entries.Count)].item;
		}

		// Token: 0x04004201 RID: 16897
		private IList<WeightedList<T>.Entry<T>> entries;

		// Token: 0x04004202 RID: 16898
		private long weight;

		// Token: 0x04004203 RID: 16899
		private Random random;

		// Token: 0x0200133A RID: 4922
		private struct Entry<U>
		{
			// Token: 0x060090A6 RID: 37030 RVA: 0x003452FF File Offset: 0x003434FF
			public Entry(U item, int weight)
			{
				this.item = item;
				this.weight = weight;
			}

			// Token: 0x04006F75 RID: 28533
			public U item;

			// Token: 0x04006F76 RID: 28534
			public int weight;
		}
	}
}
