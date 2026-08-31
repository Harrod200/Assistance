using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.Ship
{
	// Token: 0x02000967 RID: 2407
	public class BracketFireMode : IFireMode
	{
		// Token: 0x17000F8C RID: 3980
		// (get) Token: 0x06005BB1 RID: 23473 RVA: 0x002BF461 File Offset: 0x002BD661
		public FireMode mode
		{
			get
			{
				return FireMode.Bracket;
			}
		}

		// Token: 0x17000F8D RID: 3981
		// (get) Token: 0x06005BB2 RID: 23474 RVA: 0x002BF464 File Offset: 0x002BD664
		public string displayName
		{
			get
			{
				return Loc.T("UI.SpaceCombat.Bracket");
			}
		}

		// Token: 0x17000F8E RID: 3982
		// (get) Token: 0x06005BB3 RID: 23475 RVA: 0x002BF470 File Offset: 0x002BD670
		public string description
		{
			get
			{
				return Loc.T("UI.SpaceCombat.Bracket.description");
			}
		}

		// Token: 0x17000F8F RID: 3983
		// (get) Token: 0x06005BB4 RID: 23476 RVA: 0x002BF47C File Offset: 0x002BD67C
		public string iconPath
		{
			get
			{
				return "ui_spacecombat/BUT_mode_bracketing";
			}
		}

		// Token: 0x17000F90 RID: 3984
		// (get) Token: 0x06005BB5 RID: 23477 RVA: 0x002BF483 File Offset: 0x002BD683
		// (set) Token: 0x06005BB6 RID: 23478 RVA: 0x002BF48B File Offset: 0x002BD68B
		public IWeapon weapon { get; private set; }

		// Token: 0x06005BB7 RID: 23479 RVA: 0x002BF494 File Offset: 0x002BD694
		public BracketFireMode(IWeapon weapon)
		{
			this.weapon = weapon;
			this.offense = new OffenseFireMode(weapon);
		}

		// Token: 0x06005BB8 RID: 23480 RVA: 0x002BF4B0 File Offset: 0x002BD6B0
		public IDamageable AcquireTarget(DateTime currentTime, out Vector3 targetPosition, out float distanceToTarget_km)
		{
			IDamageable damageable = this.offense.AcquireTarget(currentTime, out targetPosition, out distanceToTarget_km);
			if (this.priorTarget != damageable || damageable == null || damageable.damageableType != IDamageableType.Ship || this.shotCycler >= 6)
			{
				this.shotCycler = 0;
			}
			else
			{
				Vector3 vector = BracketFireMode.<AcquireTarget>g__GetOrthogonalVector|16_0(damageable.velocityVector);
				float magnitude = damageable.velocityVector.magnitude;
				float num = 30f;
				switch (this.shotCycler)
				{
				case 1:
					targetPosition += damageable.velocityVector * num;
					break;
				case 2:
					targetPosition += num * magnitude * vector;
					break;
				case 3:
					targetPosition += Quaternion.AngleAxis(90f, damageable.velocityVector) * (num * magnitude * vector);
					break;
				case 4:
					targetPosition += Quaternion.AngleAxis(180f, damageable.velocityVector) * (num * magnitude * vector);
					break;
				case 5:
					targetPosition += Quaternion.AngleAxis(270f, damageable.velocityVector) * (num * magnitude * vector);
					break;
				}
				this.shotCycler++;
			}
			this.priorTarget = damageable;
			return damageable;
		}

		// Token: 0x06005BB9 RID: 23481 RVA: 0x002BF62C File Offset: 0x002BD82C
		[CompilerGenerated]
		internal static Vector3 <AcquireTarget>g__GetOrthogonalVector|16_0(Vector3 v)
		{
			Vector3 vector = new Vector3(-v.y, v.x, 0f);
			if (vector == Vector3.zero)
			{
				vector = new Vector3(0f, -v.z, v.y);
			}
			return vector.normalized;
		}

		// Token: 0x040041A0 RID: 16800
		private readonly OffenseFireMode offense;

		// Token: 0x040041A1 RID: 16801
		private IDamageable priorTarget;

		// Token: 0x040041A2 RID: 16802
		private int shotCycler;
	}
}
