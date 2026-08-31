using System;
using PavonisInteractive.TerraInvicta;
using UnityEngine;

// Token: 0x02000176 RID: 374
public static class SimulatedCombatExtensions
{
	// Token: 0x06000563 RID: 1379 RVA: 0x00017F3E File Offset: 0x0001613E
	public static Vector3 GetSurfaceNormal(this ArmorFacing armorFacing)
	{
		switch (armorFacing)
		{
		case ArmorFacing.Right:
			return Vector3.right;
		case ArmorFacing.Left:
			return Vector3.left;
		case ArmorFacing.Tail:
			return Vector3.back;
		}
		return Vector3.forward;
	}

	// Token: 0x06000564 RID: 1380 RVA: 0x00017F78 File Offset: 0x00016178
	public static float GenerateAngleOfIncidence_deg(this ArmorFacing armorFacing)
	{
		float num = TIUtilities.RandomFloatValue();
		num = 0.99f;
		switch (armorFacing)
		{
		case ArmorFacing.Right:
			num = Mathf.Pow(num, 0.5f);
			goto IL_0060;
		case ArmorFacing.Left:
			num = Mathf.Pow(num, 0.5f);
			goto IL_0060;
		case ArmorFacing.Tail:
			num = Mathf.Pow(num, 1f);
			goto IL_0060;
		}
		num = Mathf.Pow(num, 2f);
		IL_0060:
		return (float)((TIUtilities.RandomFloatValue() < 0.5f) ? 1 : (-1)) * num * 30f;
	}
}
