using System;
using System.Collections.Generic;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000789 RID: 1929
	public struct Prompt : IEquatable<Prompt>
	{
		// Token: 0x17000AD9 RID: 2777
		// (get) Token: 0x06003D67 RID: 15719 RVA: 0x00181DB0 File Offset: 0x0017FFB0
		// (set) Token: 0x06003D68 RID: 15720 RVA: 0x00181DB8 File Offset: 0x0017FFB8
		public TIGameState actingState { readonly get; private set; }

		// Token: 0x17000ADA RID: 2778
		// (get) Token: 0x06003D69 RID: 15721 RVA: 0x00181DC1 File Offset: 0x0017FFC1
		// (set) Token: 0x06003D6A RID: 15722 RVA: 0x00181DC9 File Offset: 0x0017FFC9
		public TIGameState promptingGameState { readonly get; private set; }

		// Token: 0x17000ADB RID: 2779
		// (get) Token: 0x06003D6B RID: 15723 RVA: 0x00181DD2 File Offset: 0x0017FFD2
		// (set) Token: 0x06003D6C RID: 15724 RVA: 0x00181DDA File Offset: 0x0017FFDA
		public TIGameState relatedGameState { readonly get; private set; }

		// Token: 0x17000ADC RID: 2780
		// (get) Token: 0x06003D6D RID: 15725 RVA: 0x00181DE3 File Offset: 0x0017FFE3
		// (set) Token: 0x06003D6E RID: 15726 RVA: 0x00181DEB File Offset: 0x0017FFEB
		public string name { readonly get; private set; }

		// Token: 0x17000ADD RID: 2781
		// (get) Token: 0x06003D6F RID: 15727 RVA: 0x00181DF4 File Offset: 0x0017FFF4
		// (set) Token: 0x06003D70 RID: 15728 RVA: 0x00181DFC File Offset: 0x0017FFFC
		public int value { readonly get; private set; }

		// Token: 0x06003D71 RID: 15729 RVA: 0x00181E05 File Offset: 0x00180005
		public Prompt(TIGameState actingState, TIGameState promptingGameState, TIGameState relatedGameState, string name, int value)
		{
			this.actingState = actingState;
			this.promptingGameState = promptingGameState;
			this.relatedGameState = relatedGameState;
			this.name = name;
			this.value = value;
		}

		// Token: 0x06003D72 RID: 15730 RVA: 0x00181E2C File Offset: 0x0018002C
		public override string ToString()
		{
			string[] array = new string[9];
			array[0] = this.name;
			array[1] = ": ";
			int num = 2;
			TIGameState actingState = this.actingState;
			array[num] = ((actingState != null) ? actingState.displayName : null) ?? "NoAS";
			array[3] = " ";
			int num2 = 4;
			TIGameState promptingGameState = this.promptingGameState;
			array[num2] = ((promptingGameState != null) ? promptingGameState.displayName : null) ?? "NoPGS";
			array[5] = " ";
			int num3 = 6;
			TIGameState relatedGameState = this.relatedGameState;
			array[num3] = ((relatedGameState != null) ? relatedGameState.displayName : null) ?? "NoRGS";
			array[7] = " ";
			array[8] = this.value.ToString();
			return string.Concat(array);
		}

		// Token: 0x06003D73 RID: 15731 RVA: 0x00181EDC File Offset: 0x001800DC
		public bool Equals(Prompt other)
		{
			return EqualityComparer<TIGameState>.Default.Equals(this.actingState, other.actingState) && string.Equals(this.name, other.name) && EqualityComparer<TIGameState>.Default.Equals(this.promptingGameState, other.promptingGameState) && EqualityComparer<TIGameState>.Default.Equals(this.relatedGameState, other.relatedGameState) && this.value == other.value;
		}

		// Token: 0x06003D74 RID: 15732 RVA: 0x00181F5C File Offset: 0x0018015C
		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (obj is Prompt)
			{
				Prompt prompt = (Prompt)obj;
				return this.Equals(prompt);
			}
			return false;
		}

		// Token: 0x06003D75 RID: 15733 RVA: 0x00181F86 File Offset: 0x00180186
		public override int GetHashCode()
		{
			return (EqualityComparer<TIGameState>.Default.GetHashCode(this.actingState) * 397) ^ ((this.name != null) ? this.name.GetHashCode() : 0);
		}

		// Token: 0x06003D76 RID: 15734 RVA: 0x00181FB5 File Offset: 0x001801B5
		public static bool operator ==(Prompt left, Prompt right)
		{
			return left.Equals(right);
		}

		// Token: 0x06003D77 RID: 15735 RVA: 0x00181FBF File Offset: 0x001801BF
		public static bool operator !=(Prompt left, Prompt right)
		{
			return !left.Equals(right);
		}
	}
}
