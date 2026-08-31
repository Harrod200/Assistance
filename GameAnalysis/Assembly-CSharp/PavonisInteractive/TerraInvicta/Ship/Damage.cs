using System;

namespace PavonisInteractive.TerraInvicta.Ship
{
	// Token: 0x02000969 RID: 2409
	public struct Damage : IEquatable<Damage>
	{
		// Token: 0x17000F92 RID: 3986
		// (get) Token: 0x06005BCB RID: 23499 RVA: 0x002BF984 File Offset: 0x002BDB84
		// (set) Token: 0x06005BCC RID: 23500 RVA: 0x002BF98C File Offset: 0x002BDB8C
		public TIFactionState applyingFaction { readonly get; private set; }

		// Token: 0x17000F93 RID: 3987
		// (get) Token: 0x06005BCD RID: 23501 RVA: 0x002BF995 File Offset: 0x002BDB95
		// (set) Token: 0x06005BCE RID: 23502 RVA: 0x002BF99D File Offset: 0x002BDB9D
		public TIShipWeaponTemplate weapon { readonly get; private set; }

		// Token: 0x17000F94 RID: 3988
		// (get) Token: 0x06005BCF RID: 23503 RVA: 0x002BF9A6 File Offset: 0x002BDBA6
		// (set) Token: 0x06005BD0 RID: 23504 RVA: 0x002BF9AE File Offset: 0x002BDBAE
		public float range_km { readonly get; private set; }

		// Token: 0x17000F95 RID: 3989
		// (get) Token: 0x06005BD1 RID: 23505 RVA: 0x002BF9B7 File Offset: 0x002BDBB7
		// (set) Token: 0x06005BD2 RID: 23506 RVA: 0x002BF9BF File Offset: 0x002BDBBF
		public DamageType type { readonly get; private set; }

		// Token: 0x17000F96 RID: 3990
		// (get) Token: 0x06005BD3 RID: 23507 RVA: 0x002BF9C8 File Offset: 0x002BDBC8
		// (set) Token: 0x06005BD4 RID: 23508 RVA: 0x002BF9D0 File Offset: 0x002BDBD0
		public float amount { readonly get; private set; }

		// Token: 0x17000F97 RID: 3991
		// (get) Token: 0x06005BD5 RID: 23509 RVA: 0x002BF9D9 File Offset: 0x002BDBD9
		// (set) Token: 0x06005BD6 RID: 23510 RVA: 0x002BF9E1 File Offset: 0x002BDBE1
		public float chippingAmount { readonly get; private set; }

		// Token: 0x17000F98 RID: 3992
		// (get) Token: 0x06005BD7 RID: 23511 RVA: 0x002BF9EA File Offset: 0x002BDBEA
		// (set) Token: 0x06005BD8 RID: 23512 RVA: 0x002BF9F2 File Offset: 0x002BDBF2
		public int shreddingAmount { readonly get; private set; }

		// Token: 0x06005BD9 RID: 23513 RVA: 0x002BF9FB File Offset: 0x002BDBFB
		private float RandomizedDamageAmount(float damageValue)
		{
			return damageValue * 0.8f + damageValue * TIUtilities.RandomRange(0f, 0.4f);
		}

		// Token: 0x06005BDA RID: 23514 RVA: 0x002BFA18 File Offset: 0x002BDC18
		public Damage(TIShipWeaponTemplate weapon, float range_km, DamageType type, float amount, float chippingAmount, int shreddingAmount, TIFactionState applyingFaction)
		{
			this.weapon = weapon;
			this.range_km = range_km;
			this.type = type;
			this.amount = amount;
			this.chippingAmount = chippingAmount;
			this.shreddingAmount = shreddingAmount;
			this.applyingFaction = applyingFaction;
			this.amount = this.RandomizedDamageAmount(this.amount);
			this.chippingAmount = this.RandomizedDamageAmount(this.chippingAmount);
		}

		// Token: 0x06005BDB RID: 23515 RVA: 0x002BFA7E File Offset: 0x002BDC7E
		public override bool Equals(object other)
		{
			return other != null && other is Damage && this.Equals((Damage)other);
		}

		// Token: 0x06005BDC RID: 23516 RVA: 0x002BFA9B File Offset: 0x002BDC9B
		public override int GetHashCode()
		{
			return HashCode.Combine<DamageType, float>(this.type, this.amount);
		}

		// Token: 0x06005BDD RID: 23517 RVA: 0x002BFAB0 File Offset: 0x002BDCB0
		public override string ToString()
		{
			return string.Format("{0}({1})", this.type.ToString(), this.amount);
		}

		// Token: 0x06005BDE RID: 23518 RVA: 0x002BFAE6 File Offset: 0x002BDCE6
		public bool Equals(Damage other)
		{
			return this.amount == other.amount && this.type == other.type;
		}

		// Token: 0x06005BDF RID: 23519 RVA: 0x002BFB08 File Offset: 0x002BDD08
		public static bool operator ==(Damage lhs, Damage rhs)
		{
			return lhs.Equals(rhs);
		}

		// Token: 0x06005BE0 RID: 23520 RVA: 0x002BFB12 File Offset: 0x002BDD12
		public static bool operator !=(Damage lhs, Damage rhs)
		{
			return !lhs.Equals(rhs);
		}

		// Token: 0x06005BE1 RID: 23521 RVA: 0x002BFB1F File Offset: 0x002BDD1F
		public static bool operator <(Damage lhs, float rhs)
		{
			return lhs.amount < rhs;
		}

		// Token: 0x06005BE2 RID: 23522 RVA: 0x002BFB2B File Offset: 0x002BDD2B
		public static bool operator >(Damage lhs, float rhs)
		{
			return lhs.amount > rhs;
		}

		// Token: 0x06005BE3 RID: 23523 RVA: 0x002BFB37 File Offset: 0x002BDD37
		public static bool operator <=(Damage lhs, float rhs)
		{
			return lhs.amount <= rhs;
		}

		// Token: 0x06005BE4 RID: 23524 RVA: 0x002BFB46 File Offset: 0x002BDD46
		public static bool operator >=(Damage lhs, float rhs)
		{
			return lhs.amount >= rhs;
		}

		// Token: 0x040041A7 RID: 16807
		public const float randomizer = 0.2f;

		// Token: 0x040041A8 RID: 16808
		public static readonly Damage None;
	}
}
