using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020001A6 RID: 422
public class TIMissionCondition_EnemySpaceAsset : TIMissionCondition
{
	// Token: 0x06000627 RID: 1575 RVA: 0x0001C4D8 File Offset: 0x0001A6D8
	public override string CanTarget(TICouncilorState councilor, TIGameState possibleTarget)
	{
		if (possibleTarget.isSpaceShipState && !possibleTarget.ref_ship.faction.permanentAlly(councilor.faction))
		{
			return "_Pass";
		}
		if (possibleTarget.isHabState && !possibleTarget.ref_hab.faction.permanentAlly(councilor.faction))
		{
			return "_Pass";
		}
		return base.GetType().Name;
	}
}
