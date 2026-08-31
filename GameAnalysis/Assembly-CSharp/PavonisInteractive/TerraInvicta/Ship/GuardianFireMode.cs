using System;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.Ship
{
	// Token: 0x0200096E RID: 2414
	public class GuardianFireMode : IFireMode
	{
		// Token: 0x17000FA4 RID: 4004
		// (get) Token: 0x06005C02 RID: 23554 RVA: 0x002C049E File Offset: 0x002BE69E
		// (set) Token: 0x06005C03 RID: 23555 RVA: 0x002C04A6 File Offset: 0x002BE6A6
		public IWeapon weapon { get; private set; }

		// Token: 0x17000FA5 RID: 4005
		// (get) Token: 0x06005C04 RID: 23556 RVA: 0x002C04AF File Offset: 0x002BE6AF
		public FireMode mode
		{
			get
			{
				return FireMode.Guardian;
			}
		}

		// Token: 0x17000FA6 RID: 4006
		// (get) Token: 0x06005C05 RID: 23557 RVA: 0x002C04B2 File Offset: 0x002BE6B2
		public string displayName
		{
			get
			{
				if (!this.weaponTemplate.CanOnlyDefensivelyTargetMissiles())
				{
					return Loc.T("UI.SpaceCombat.Guardian");
				}
				return Loc.T("UI.SpaceCombat.MissileGuardian");
			}
		}

		// Token: 0x17000FA7 RID: 4007
		// (get) Token: 0x06005C06 RID: 23558 RVA: 0x002C04D6 File Offset: 0x002BE6D6
		public string description
		{
			get
			{
				if (!this.weaponTemplate.CanOnlyDefensivelyTargetMissiles())
				{
					return Loc.T("UI.SpaceCombat.Guardian.description");
				}
				return Loc.T("UI.SpaceCombat.MissileGuardian.description");
			}
		}

		// Token: 0x17000FA8 RID: 4008
		// (get) Token: 0x06005C07 RID: 23559 RVA: 0x002C04FA File Offset: 0x002BE6FA
		public string iconPath
		{
			get
			{
				if (!this.weaponTemplate.CanOnlyDefensivelyTargetMissiles())
				{
					return "ui_spacecombat/BUT_mode_guardian";
				}
				return "ui_spacecombat/BUT_mode_missileguardian";
			}
		}

		// Token: 0x06005C08 RID: 23560 RVA: 0x002C0514 File Offset: 0x002BE714
		public GuardianFireMode(IWeapon weapon, bool isAI)
		{
			this.weapon = weapon;
			this.weaponAsset = weapon as Weapon;
			this.weaponTemplate = this.weaponAsset.weaponTemplate;
			this.offense = new OffenseFireMode(weapon);
			this.defense = new DefenseFireMode(weapon);
			this.SetAIManagement(isAI);
		}

		// Token: 0x06005C09 RID: 23561 RVA: 0x002C056A File Offset: 0x002BE76A
		public void SetAIManagement(bool setting)
		{
			this.IsAI = setting;
		}

		// Token: 0x06005C0A RID: 23562 RVA: 0x002C0574 File Offset: 0x002BE774
		public IDamageable AcquireTarget(DateTime currentTime, out Vector3 targetPosition, out float distanceToTarget_km)
		{
			this.cachedTarget = null;
			IDamageable damageable = this.defense.AcquireTarget(currentTime, out targetPosition, out distanceToTarget_km);
			if (this.IsAI && damageable != null && this.weaponTemplate.noseWeapon && distanceToTarget_km > 300f && damageable.damageableType == IDamageableType.BallisticProjectile && damageable.ref_projectile.originWeapon.warheadMass_kg < 100f)
			{
				this.cachedTarget = damageable;
				this.cachedDistanceToTarget_km = distanceToTarget_km;
				this.cachedTargetPosition = targetPosition;
				damageable = null;
			}
			if (damageable == null)
			{
				damageable = this.offense.AcquireTarget(currentTime, out targetPosition, out distanceToTarget_km);
			}
			if (this.IsAI && damageable == null)
			{
				damageable = this.cachedTarget;
				distanceToTarget_km = this.cachedDistanceToTarget_km;
				targetPosition = this.cachedTargetPosition;
			}
			return damageable;
		}

		// Token: 0x040041C6 RID: 16838
		private readonly Weapon weaponAsset;

		// Token: 0x040041C7 RID: 16839
		private readonly TIShipWeaponTemplate weaponTemplate;

		// Token: 0x040041C8 RID: 16840
		private bool IsAI;

		// Token: 0x040041C9 RID: 16841
		private IDamageable cachedTarget;

		// Token: 0x040041CA RID: 16842
		private float cachedDistanceToTarget_km;

		// Token: 0x040041CB RID: 16843
		private Vector3 cachedTargetPosition;

		// Token: 0x040041CC RID: 16844
		private readonly OffenseFireMode offense;

		// Token: 0x040041CD RID: 16845
		private readonly DefenseFireMode defense;
	}
}
