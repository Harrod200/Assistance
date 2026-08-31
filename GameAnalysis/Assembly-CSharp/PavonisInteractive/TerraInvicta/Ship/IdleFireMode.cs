using System;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.Ship
{
	// Token: 0x0200097C RID: 2428
	public class IdleFireMode : IFireMode
	{
		// Token: 0x17000FCE RID: 4046
		// (get) Token: 0x06005C5E RID: 23646 RVA: 0x002C0CB1 File Offset: 0x002BEEB1
		// (set) Token: 0x06005C5F RID: 23647 RVA: 0x002C0CB9 File Offset: 0x002BEEB9
		public IWeapon weapon { get; private set; }

		// Token: 0x17000FCF RID: 4047
		// (get) Token: 0x06005C60 RID: 23648 RVA: 0x002C0CC2 File Offset: 0x002BEEC2
		public FireMode mode
		{
			get
			{
				return FireMode.Idle;
			}
		}

		// Token: 0x17000FD0 RID: 4048
		// (get) Token: 0x06005C61 RID: 23649 RVA: 0x002C0CC5 File Offset: 0x002BEEC5
		public string displayName
		{
			get
			{
				return Loc.T("UI.SpaceCombat.Idle");
			}
		}

		// Token: 0x17000FD1 RID: 4049
		// (get) Token: 0x06005C62 RID: 23650 RVA: 0x002C0CD1 File Offset: 0x002BEED1
		public string description
		{
			get
			{
				return Loc.T("UI.SpaceCombat.Idle.description");
			}
		}

		// Token: 0x17000FD2 RID: 4050
		// (get) Token: 0x06005C63 RID: 23651 RVA: 0x002C0CDD File Offset: 0x002BEEDD
		public string iconPath
		{
			get
			{
				return "ui_spacecombat/BUT_mode_idle";
			}
		}

		// Token: 0x06005C64 RID: 23652 RVA: 0x002C0CE4 File Offset: 0x002BEEE4
		public IdleFireMode(IWeapon weapon)
		{
			this.weapon = weapon;
		}

		// Token: 0x06005C65 RID: 23653 RVA: 0x002C0CF3 File Offset: 0x002BEEF3
		public IDamageable AcquireTarget(DateTime currentTime, out Vector3 targetLocation, out float distance_km)
		{
			targetLocation = Vector3.zero;
			distance_km = float.MaxValue;
			return null;
		}
	}
}
