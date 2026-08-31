using System;

// Token: 0x020003F7 RID: 1015
public static class ShipRoleExtensions
{
	// Token: 0x060014D5 RID: 5333 RVA: 0x0006615D File Offset: 0x0006435D
	public static bool IsCombatantRole(this ShipRole role)
	{
		return role > ShipRole.CouncilorTransport;
	}

	// Token: 0x060014D6 RID: 5334 RVA: 0x00066168 File Offset: 0x00064368
	public static float GetExpectedCombatRange_km(this ShipRole role)
	{
		switch (role)
		{
		case ShipRole.LS_Penetrator:
		case ShipRole.MS_Strike:
		case ShipRole.SS_Interceptor:
			return 200f;
		case ShipRole.LL_Intruder:
		case ShipRole.LL_Bomber:
		case ShipRole.ML_Standoff:
		case ShipRole.SL_Defender:
			return 800f;
		}
		return 500f;
	}
}
