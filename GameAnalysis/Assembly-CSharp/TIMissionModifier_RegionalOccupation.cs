using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;
using UnityEngine;

// Token: 0x0200022B RID: 555
public class TIMissionModifier_RegionalOccupation : TIMissionModifier
{
	// Token: 0x0600075B RID: 1883 RVA: 0x00023280 File Offset: 0x00021480
	public override float GetModifier(TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0f, FactionResource resource = FactionResource.None)
	{
		float num = 0f;
		TIRegionState ref_region = target.ref_region;
		if (ref_region != null)
		{
			TINationState tinationState;
			List<TINationState> list;
			num += ref_region.GetHighestWarAllianceOccupationValue(out tinationState, out list) * 10f;
		}
		return Mathf.Max(num, 0f);
	}
}
