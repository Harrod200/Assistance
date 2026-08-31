using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000345 RID: 837
public class LaunchGroupProbeOperation : LaunchMultipleProbesOperation
{
	// Token: 0x06000E82 RID: 3714 RVA: 0x00048BBC File Offset: 0x00046DBC
	public override List<TIGameState> GetPossibleTargets(TIGameState actorState, TIGameState defaultTarget = null)
	{
		if (((defaultTarget != null) ? defaultTarget.ref_spaceBody : null) != null)
		{
			return (from x in (from x in TINaturalSpaceObjectState.GetFilteredSolarSystemGroupObjects(defaultTarget.ref_spaceBody, true)
					where x.isSpaceBodyState && actorState.ref_faction.CanProspectWithProbe(x.ref_spaceBody, false)
					select x).ToList<TINaturalSpaceObjectState>()
				select x.ref_gameState).ToList<TIGameState>();
		}
		return new List<TIGameState>();
	}
}
