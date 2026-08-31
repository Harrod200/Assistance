using System;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020007D5 RID: 2005
	public struct GameStateID : IEquatable<GameStateID>, IEquatable<GameStateID?>, IEquatable<int>, IComparable<GameStateID>
	{
		// Token: 0x06004803 RID: 18435 RVA: 0x001DCD4C File Offset: 0x001DAF4C
		public GameStateID(int value)
		{
			this.value = value;
		}

		// Token: 0x06004804 RID: 18436 RVA: 0x001DCD55 File Offset: 0x001DAF55
		public override int GetHashCode()
		{
			return this.value.GetHashCode();
		}

		// Token: 0x06004805 RID: 18437 RVA: 0x001DCD62 File Offset: 0x001DAF62
		public TIGameState GetState()
		{
			return GameStateManager.FindGameState<TIGameState>(this, true);
		}

		// Token: 0x06004806 RID: 18438 RVA: 0x001DCD70 File Offset: 0x001DAF70
		public T GetState<T>(bool allowChild = false) where T : TIGameState
		{
			if (this.value == 0)
			{
				throw new InvalidOperationException("Cannot get GameState for ID 0. Use TryGetState instead.");
			}
			T t = GameStateManager.FindGameState<T>(this, allowChild);
			if (t == null)
			{
				throw new GameStateNotFound(this.value, typeof(T));
			}
			return t;
		}

		// Token: 0x06004807 RID: 18439 RVA: 0x001DCDC5 File Offset: 0x001DAFC5
		public bool TryGetState<T>(out T state, bool allowChild = false) where T : TIGameState
		{
			state = GameStateManager.FindGameState<T>(this, allowChild);
			return state != null;
		}

		// Token: 0x06004808 RID: 18440 RVA: 0x001DCDEA File Offset: 0x001DAFEA
		public bool Equals(GameStateID other)
		{
			return this.value.Equals(other.value);
		}

		// Token: 0x06004809 RID: 18441 RVA: 0x001DCDFD File Offset: 0x001DAFFD
		public bool Equals(GameStateID? other)
		{
			return other != null && this.value.Equals(other.Value.value);
		}

		// Token: 0x0600480A RID: 18442 RVA: 0x001DCE21 File Offset: 0x001DB021
		public bool Equals(int otherValue)
		{
			return this.value == otherValue;
		}

		// Token: 0x0600480B RID: 18443 RVA: 0x001DCE2C File Offset: 0x001DB02C
		public override bool Equals(object obj)
		{
			return false;
		}

		// Token: 0x0600480C RID: 18444 RVA: 0x001DCE2F File Offset: 0x001DB02F
		public int CompareTo(GameStateID other)
		{
			return this.value.CompareTo(other.value);
		}

		// Token: 0x0600480D RID: 18445 RVA: 0x001DCE42 File Offset: 0x001DB042
		public override string ToString()
		{
			return this.value.ToString();
		}

		// Token: 0x0600480E RID: 18446 RVA: 0x001DCE4F File Offset: 0x001DB04F
		public static bool operator ==(GameStateID lhs, GameStateID rhs)
		{
			return lhs.Equals(rhs);
		}

		// Token: 0x0600480F RID: 18447 RVA: 0x001DCE59 File Offset: 0x001DB059
		public static bool operator ==(GameStateID lhs, GameStateID? rhs)
		{
			return lhs.Equals(rhs);
		}

		// Token: 0x06004810 RID: 18448 RVA: 0x001DCE63 File Offset: 0x001DB063
		public static bool operator ==(GameStateID lhs, int rhs)
		{
			return lhs.value == rhs;
		}

		// Token: 0x06004811 RID: 18449 RVA: 0x001DCE6E File Offset: 0x001DB06E
		public static bool operator !=(GameStateID lhs, GameStateID rhs)
		{
			return !lhs.Equals(rhs);
		}

		// Token: 0x06004812 RID: 18450 RVA: 0x001DCE7B File Offset: 0x001DB07B
		public static bool operator !=(GameStateID lhs, GameStateID? rhs)
		{
			return !lhs.Equals(rhs);
		}

		// Token: 0x06004813 RID: 18451 RVA: 0x001DCE88 File Offset: 0x001DB088
		public static bool operator !=(GameStateID lhs, int rhs)
		{
			return lhs.value != rhs;
		}

		// Token: 0x06004814 RID: 18452 RVA: 0x001DCE96 File Offset: 0x001DB096
		public static bool operator <(GameStateID lhs, GameStateID rhs)
		{
			return lhs.value < rhs.value;
		}

		// Token: 0x06004815 RID: 18453 RVA: 0x001DCEA6 File Offset: 0x001DB0A6
		public static bool operator >(GameStateID lhs, GameStateID rhs)
		{
			return lhs.value > rhs.value;
		}

		// Token: 0x06004816 RID: 18454 RVA: 0x001DCEB6 File Offset: 0x001DB0B6
		public static int operator +(GameStateID lhs, int rhs)
		{
			return lhs.value + rhs;
		}

		// Token: 0x06004817 RID: 18455 RVA: 0x001DCEC0 File Offset: 0x001DB0C0
		public static GameStateID operator ++(GameStateID id)
		{
			return new GameStateID(id.value + 1);
		}

		// Token: 0x06004818 RID: 18456 RVA: 0x001DCECF File Offset: 0x001DB0CF
		public static explicit operator int(GameStateID id)
		{
			return id.value;
		}

		// Token: 0x06004819 RID: 18457 RVA: 0x001DCED7 File Offset: 0x001DB0D7
		public static implicit operator GameStateID(int newValue)
		{
			return new GameStateID(newValue);
		}

		// Token: 0x040029AD RID: 10669
		[SerializeField]
		private int value;
	}
}
