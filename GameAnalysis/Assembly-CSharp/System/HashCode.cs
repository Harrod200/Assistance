using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace System
{
	// Token: 0x020004EC RID: 1260
	public struct HashCode
	{
		// Token: 0x06001E17 RID: 7703 RVA: 0x0009D951 File Offset: 0x0009BB51
		private static uint GenerateGlobalSeed()
		{
			return (uint)new Random().Next(int.MinValue, int.MaxValue);
		}

		// Token: 0x06001E18 RID: 7704 RVA: 0x0009D968 File Offset: 0x0009BB68
		public static int Combine<T1>(T1 value1)
		{
			uint num = (uint)((value1 == null) ? value1.GetHashCode() : 0);
			return (int)HashCode.MixFinal(HashCode.QueueRound(HashCode.MixEmptyState() + 4U, num));
		}

		// Token: 0x06001E19 RID: 7705 RVA: 0x0009D9A0 File Offset: 0x0009BBA0
		public static int Combine<T1, T2>(T1 value1, T2 value2)
		{
			uint num = (uint)((value1 == null) ? value1.GetHashCode() : 0);
			uint num2 = (uint)((value2 == null) ? value2.GetHashCode() : 0);
			return (int)HashCode.MixFinal(HashCode.QueueRound(HashCode.QueueRound(HashCode.MixEmptyState() + 8U, num), num2));
		}

		// Token: 0x06001E1A RID: 7706 RVA: 0x0009D9F8 File Offset: 0x0009BBF8
		public static int Combine<T1, T2, T3>(T1 value1, T2 value2, T3 value3)
		{
			uint num = (uint)((value1 == null) ? value1.GetHashCode() : 0);
			uint num2 = (uint)((value2 == null) ? value2.GetHashCode() : 0);
			uint num3 = (uint)((value3 == null) ? value3.GetHashCode() : 0);
			return (int)HashCode.MixFinal(HashCode.QueueRound(HashCode.QueueRound(HashCode.QueueRound(HashCode.MixEmptyState() + 12U, num), num2), num3));
		}

		// Token: 0x06001E1B RID: 7707 RVA: 0x0009DA70 File Offset: 0x0009BC70
		public static int Combine<T1, T2, T3, T4>(T1 value1, T2 value2, T3 value3, T4 value4)
		{
			uint num = (uint)((value1 == null) ? 0 : value1.GetHashCode());
			uint num2 = (uint)((value2 == null) ? 0 : value2.GetHashCode());
			uint num3 = (uint)((value3 == null) ? 0 : value3.GetHashCode());
			uint num4 = (uint)((value4 == null) ? 0 : value4.GetHashCode());
			uint num5;
			uint num6;
			uint num7;
			uint num8;
			HashCode.Initialize(out num5, out num6, out num7, out num8);
			num5 = HashCode.Round(num5, num);
			num6 = HashCode.Round(num6, num2);
			num7 = HashCode.Round(num7, num3);
			num8 = HashCode.Round(num8, num4);
			return (int)HashCode.MixFinal(HashCode.MixState(num5, num6, num7, num8) + 16U);
		}

		// Token: 0x06001E1C RID: 7708 RVA: 0x0009DB2C File Offset: 0x0009BD2C
		public static int Combine<T1, T2, T3, T4, T5>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5)
		{
			uint num = (uint)((value1 == null) ? 0 : value1.GetHashCode());
			uint num2 = (uint)((value2 == null) ? 0 : value2.GetHashCode());
			uint num3 = (uint)((value3 == null) ? 0 : value3.GetHashCode());
			uint num4 = (uint)((value4 == null) ? 0 : value4.GetHashCode());
			uint num5 = (uint)((value5 == null) ? 0 : value5.GetHashCode());
			uint num6;
			uint num7;
			uint num8;
			uint num9;
			HashCode.Initialize(out num6, out num7, out num8, out num9);
			num6 = HashCode.Round(num6, num);
			num7 = HashCode.Round(num7, num2);
			num8 = HashCode.Round(num8, num3);
			num9 = HashCode.Round(num9, num4);
			return (int)HashCode.MixFinal(HashCode.QueueRound(HashCode.MixState(num6, num7, num8, num9) + 20U, num5));
		}

		// Token: 0x06001E1D RID: 7709 RVA: 0x0009DC0C File Offset: 0x0009BE0C
		public static int Combine<T1, T2, T3, T4, T5, T6>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6)
		{
			uint num = (uint)((value1 == null) ? 0 : value1.GetHashCode());
			uint num2 = (uint)((value2 == null) ? 0 : value2.GetHashCode());
			uint num3 = (uint)((value3 == null) ? 0 : value3.GetHashCode());
			uint num4 = (uint)((value4 == null) ? 0 : value4.GetHashCode());
			uint num5 = (uint)((value5 == null) ? 0 : value5.GetHashCode());
			uint num6 = (uint)((value6 == null) ? 0 : value6.GetHashCode());
			uint num7;
			uint num8;
			uint num9;
			uint num10;
			HashCode.Initialize(out num7, out num8, out num9, out num10);
			num7 = HashCode.Round(num7, num);
			num8 = HashCode.Round(num8, num2);
			num9 = HashCode.Round(num9, num3);
			num10 = HashCode.Round(num10, num4);
			return (int)HashCode.MixFinal(HashCode.QueueRound(HashCode.QueueRound(HashCode.MixState(num7, num8, num9, num10) + 24U, num5), num6));
		}

		// Token: 0x06001E1E RID: 7710 RVA: 0x0009DD0C File Offset: 0x0009BF0C
		public static int Combine<T1, T2, T3, T4, T5, T6, T7>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7)
		{
			uint num = (uint)((value1 == null) ? 0 : value1.GetHashCode());
			uint num2 = (uint)((value2 == null) ? 0 : value2.GetHashCode());
			uint num3 = (uint)((value3 == null) ? 0 : value3.GetHashCode());
			uint num4 = (uint)((value4 == null) ? 0 : value4.GetHashCode());
			uint num5 = (uint)((value5 == null) ? 0 : value5.GetHashCode());
			uint num6 = (uint)((value6 == null) ? 0 : value6.GetHashCode());
			uint num7 = (uint)((value7 == null) ? 0 : value7.GetHashCode());
			uint num8;
			uint num9;
			uint num10;
			uint num11;
			HashCode.Initialize(out num8, out num9, out num10, out num11);
			num8 = HashCode.Round(num8, num);
			num9 = HashCode.Round(num9, num2);
			num10 = HashCode.Round(num10, num3);
			num11 = HashCode.Round(num11, num4);
			return (int)HashCode.MixFinal(HashCode.QueueRound(HashCode.QueueRound(HashCode.QueueRound(HashCode.MixState(num8, num9, num10, num11) + 28U, num5), num6), num7));
		}

		// Token: 0x06001E1F RID: 7711 RVA: 0x0009DE30 File Offset: 0x0009C030
		public static int Combine<T1, T2, T3, T4, T5, T6, T7, T8>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8)
		{
			uint num = (uint)((value1 == null) ? 0 : value1.GetHashCode());
			uint num2 = (uint)((value2 == null) ? 0 : value2.GetHashCode());
			uint num3 = (uint)((value3 == null) ? 0 : value3.GetHashCode());
			uint num4 = (uint)((value4 == null) ? 0 : value4.GetHashCode());
			uint num5 = (uint)((value5 == null) ? 0 : value5.GetHashCode());
			uint num6 = (uint)((value6 == null) ? 0 : value6.GetHashCode());
			uint num7 = (uint)((value7 == null) ? 0 : value7.GetHashCode());
			uint num8 = (uint)((value8 == null) ? 0 : value8.GetHashCode());
			uint num9;
			uint num10;
			uint num11;
			uint num12;
			HashCode.Initialize(out num9, out num10, out num11, out num12);
			num9 = HashCode.Round(num9, num);
			num10 = HashCode.Round(num10, num2);
			num11 = HashCode.Round(num11, num3);
			num12 = HashCode.Round(num12, num4);
			num9 = HashCode.Round(num9, num5);
			num10 = HashCode.Round(num10, num6);
			num11 = HashCode.Round(num11, num7);
			num12 = HashCode.Round(num12, num8);
			return (int)HashCode.MixFinal(HashCode.MixState(num9, num10, num11, num12) + 32U);
		}

		// Token: 0x06001E20 RID: 7712 RVA: 0x0009DF83 File Offset: 0x0009C183
		private static uint Rol(uint value, int count)
		{
			return (value << count) | (value >> 32 - count);
		}

		// Token: 0x06001E21 RID: 7713 RVA: 0x0009DF95 File Offset: 0x0009C195
		private static void Initialize(out uint v1, out uint v2, out uint v3, out uint v4)
		{
			v1 = HashCode.s_seed + 2654435761U + 2246822519U;
			v2 = HashCode.s_seed + 2246822519U;
			v3 = HashCode.s_seed;
			v4 = HashCode.s_seed - 2654435761U;
		}

		// Token: 0x06001E22 RID: 7714 RVA: 0x0009DFCB File Offset: 0x0009C1CB
		private static uint Round(uint hash, uint input)
		{
			hash += input * 2246822519U;
			hash = HashCode.Rol(hash, 13);
			hash *= 2654435761U;
			return hash;
		}

		// Token: 0x06001E23 RID: 7715 RVA: 0x0009DFEC File Offset: 0x0009C1EC
		private static uint QueueRound(uint hash, uint queuedValue)
		{
			hash += queuedValue * 3266489917U;
			return HashCode.Rol(hash, 17) * 668265263U;
		}

		// Token: 0x06001E24 RID: 7716 RVA: 0x0009E007 File Offset: 0x0009C207
		private static uint MixState(uint v1, uint v2, uint v3, uint v4)
		{
			return HashCode.Rol(v1, 1) + HashCode.Rol(v2, 7) + HashCode.Rol(v3, 12) + HashCode.Rol(v4, 18);
		}

		// Token: 0x06001E25 RID: 7717 RVA: 0x0009E02A File Offset: 0x0009C22A
		private static uint MixEmptyState()
		{
			return HashCode.s_seed + 374761393U;
		}

		// Token: 0x06001E26 RID: 7718 RVA: 0x0009E037 File Offset: 0x0009C237
		private static uint MixFinal(uint hash)
		{
			hash ^= hash >> 15;
			hash *= 2246822519U;
			hash ^= hash >> 13;
			hash *= 3266489917U;
			hash ^= hash >> 16;
			return hash;
		}

		// Token: 0x06001E27 RID: 7719 RVA: 0x0009E064 File Offset: 0x0009C264
		public void Add<T>(T value)
		{
			this.Add((value == null) ? 0 : value.GetHashCode());
		}

		// Token: 0x06001E28 RID: 7720 RVA: 0x0009E084 File Offset: 0x0009C284
		public void Add<T>(T value, IEqualityComparer<T> comparer)
		{
			this.Add((comparer != null) ? comparer.GetHashCode(value) : ((value == null) ? 0 : value.GetHashCode()));
		}

		// Token: 0x06001E29 RID: 7721 RVA: 0x0009E0B0 File Offset: 0x0009C2B0
		private void Add(int value)
		{
			uint length = this._length;
			this._length = length + 1U;
			uint num = length;
			uint num2 = num % 4U;
			if (num2 == 0U)
			{
				this._queue1 = (uint)value;
				return;
			}
			if (num2 == 1U)
			{
				this._queue2 = (uint)value;
				return;
			}
			if (num2 == 2U)
			{
				this._queue3 = (uint)value;
				return;
			}
			if (num == 3U)
			{
				HashCode.Initialize(out this._v1, out this._v2, out this._v3, out this._v4);
			}
			this._v1 = HashCode.Round(this._v1, this._queue1);
			this._v2 = HashCode.Round(this._v2, this._queue2);
			this._v3 = HashCode.Round(this._v3, this._queue3);
			this._v4 = HashCode.Round(this._v4, (uint)value);
		}

		// Token: 0x06001E2A RID: 7722 RVA: 0x0009E170 File Offset: 0x0009C370
		public int ToHashCode()
		{
			uint length = this._length;
			uint num = length % 4U;
			uint num2 = ((length < 4U) ? HashCode.MixEmptyState() : HashCode.MixState(this._v1, this._v2, this._v3, this._v4));
			num2 += length * 4U;
			if (num > 0U)
			{
				num2 = HashCode.QueueRound(num2, this._queue1);
				if (num > 1U)
				{
					num2 = HashCode.QueueRound(num2, this._queue2);
					if (num > 2U)
					{
						num2 = HashCode.QueueRound(num2, this._queue3);
					}
				}
			}
			return (int)HashCode.MixFinal(num2);
		}

		// Token: 0x06001E2B RID: 7723 RVA: 0x0009E1F2 File Offset: 0x0009C3F2
		[Obsolete("HashCode is a mutable struct and should not be compared with other HashCodes. Use ToHashCode to retrieve the computed hash code.", true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override int GetHashCode()
		{
			throw new NotSupportedException("GetHashCode Not Suported");
		}

		// Token: 0x06001E2C RID: 7724 RVA: 0x0009E1FE File Offset: 0x0009C3FE
		[Obsolete("HashCode is a mutable struct and should not be compared with other HashCodes.", true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override bool Equals(object obj)
		{
			throw new NotSupportedException("Equals Not Supported");
		}

		// Token: 0x040017DC RID: 6108
		private static readonly uint s_seed = HashCode.GenerateGlobalSeed();

		// Token: 0x040017DD RID: 6109
		private const uint Prime1 = 2654435761U;

		// Token: 0x040017DE RID: 6110
		private const uint Prime2 = 2246822519U;

		// Token: 0x040017DF RID: 6111
		private const uint Prime3 = 3266489917U;

		// Token: 0x040017E0 RID: 6112
		private const uint Prime4 = 668265263U;

		// Token: 0x040017E1 RID: 6113
		private const uint Prime5 = 374761393U;

		// Token: 0x040017E2 RID: 6114
		private uint _v1;

		// Token: 0x040017E3 RID: 6115
		private uint _v2;

		// Token: 0x040017E4 RID: 6116
		private uint _v3;

		// Token: 0x040017E5 RID: 6117
		private uint _v4;

		// Token: 0x040017E6 RID: 6118
		private uint _queue1;

		// Token: 0x040017E7 RID: 6119
		private uint _queue2;

		// Token: 0x040017E8 RID: 6120
		private uint _queue3;

		// Token: 0x040017E9 RID: 6121
		private uint _length;
	}
}
