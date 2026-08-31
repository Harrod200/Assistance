using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x020001A5 RID: 421
public class TIMissionCondition_SpaceAssetNotInTransfer : TIMissionCondition
{
	// Token: 0x170000E8 RID: 232
	// (get) Token: 0x06000624 RID: 1572 RVA: 0x0001C459 File Offset: 0x0001A659
	public override List<string> feedback
	{
		get
		{
			return new List<string> { "TIMissionCondition_SpaceAssetNotInTransfer", "TIMissionCondition_SpaceAssetNotInTransfer2" };
		}
	}

	// Token: 0x06000625 RID: 1573 RVA: 0x0001C478 File Offset: 0x0001A678
	public override string CanTarget(TICouncilorState councilor, TIGameState possibleTarget)
	{
		if (possibleTarget.isSpaceShipState)
		{
			TISpaceFleetState fleet = possibleTarget.ref_ship.fleet;
			if (fleet != null && !fleet.transferAssigned)
			{
				return "_Pass";
			}
			return "TIMissionCondition_SpaceAssetNotInTransfer2";
		}
		else
		{
			if (possibleTarget.isHabState)
			{
				return "_Pass";
			}
			return base.GetType().Name;
		}
	}
}
