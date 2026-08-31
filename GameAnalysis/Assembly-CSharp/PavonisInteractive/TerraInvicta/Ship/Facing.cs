using System;

namespace PavonisInteractive.TerraInvicta.Ship
{
	// Token: 0x0200096C RID: 2412
	public struct Facing : IEquatable<Facing>
	{
		// Token: 0x17000F9E RID: 3998
		// (get) Token: 0x06005BF0 RID: 23536 RVA: 0x002C01A4 File Offset: 0x002BE3A4
		// (set) Token: 0x06005BF1 RID: 23537 RVA: 0x002C01AC File Offset: 0x002BE3AC
		public float facingAngle { readonly get; private set; }

		// Token: 0x17000F9F RID: 3999
		// (get) Token: 0x06005BF2 RID: 23538 RVA: 0x002C01B5 File Offset: 0x002BE3B5
		// (set) Token: 0x06005BF3 RID: 23539 RVA: 0x002C01BD File Offset: 0x002BE3BD
		public ArmorFacing armorFacing { readonly get; private set; }

		// Token: 0x06005BF4 RID: 23540 RVA: 0x002C01C6 File Offset: 0x002BE3C6
		public Facing(float facingAngle, ArmorFacing armorFacing)
		{
			this = new Facing(facingAngle, 180f, armorFacing);
		}

		// Token: 0x06005BF5 RID: 23541 RVA: 0x002C01D5 File Offset: 0x002BE3D5
		public Facing(float facingAngle, float bounds, ArmorFacing armorFacing)
		{
			this = new Facing(facingAngle, bounds, bounds, armorFacing);
		}

		// Token: 0x06005BF6 RID: 23542 RVA: 0x002C01E1 File Offset: 0x002BE3E1
		public Facing(float facingAngle, float minBound, float maxBound, ArmorFacing armorFacing)
		{
			this.facingAngle = facingAngle;
			this.armorFacing = armorFacing;
			this.minAngle = facingAngle - minBound;
			this.maxAngle = facingAngle + maxBound;
		}

		// Token: 0x06005BF7 RID: 23543 RVA: 0x002C0204 File Offset: 0x002BE404
		public bool Equals(Facing other)
		{
			return this.facingAngle == other.facingAngle && this.minAngle == other.minAngle && this.maxAngle == other.maxAngle;
		}

		// Token: 0x06005BF8 RID: 23544 RVA: 0x002C0233 File Offset: 0x002BE433
		public override bool Equals(object obj)
		{
			return obj != null && !(obj.GetType() != typeof(Facing)) && this.Equals((Facing)obj);
		}

		// Token: 0x06005BF9 RID: 23545 RVA: 0x002C025F File Offset: 0x002BE45F
		public override int GetHashCode()
		{
			return HashCode.Combine<float, float, float>(this.facingAngle, this.minAngle, this.maxAngle);
		}

		// Token: 0x06005BFA RID: 23546 RVA: 0x002C0278 File Offset: 0x002BE478
		public override string ToString()
		{
			return string.Format("{0} < {1} > {2}", this.minAngle, this.facingAngle, this.maxAngle);
		}

		// Token: 0x06005BFB RID: 23547 RVA: 0x002C02A8 File Offset: 0x002BE4A8
		public bool Contains(float angle)
		{
			if (this.maxAngle > 180f)
			{
				return (angle >= this.minAngle && angle <= this.facingAngle) || angle + 360f < this.maxAngle;
			}
			if (this.minAngle <= -180f)
			{
				return (angle < this.maxAngle && angle >= this.facingAngle) || angle - 360f >= this.minAngle;
			}
			return angle >= this.minAngle && angle < this.maxAngle;
		}

		// Token: 0x040041C2 RID: 16834
		private float minAngle;

		// Token: 0x040041C3 RID: 16835
		private float maxAngle;
	}
}
