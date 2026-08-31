using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000346 RID: 838
public class LaunchAllProbeOperation : LaunchMultipleProbesOperation
{
	// Token: 0x06000E84 RID: 3716 RVA: 0x00048C43 File Offset: 0x00046E43
	public override int SortOrder()
	{
		return 0;
	}

	// Token: 0x06000E85 RID: 3717 RVA: 0x00048C48 File Offset: 0x00046E48
	public override List<TIGameState> GetPossibleTargets(TIGameState actorState, TIGameState defaultTarget = null)
	{
		return (from x in (from x in GameStateManager.AllSpaceBodies().ToList<TISpaceBodyState>()
				where actorState.ref_faction.CanProspectWithProbe(x, false)
				select x).ToList<TISpaceBodyState>()
			select x.ref_gameState).ToList<TIGameState>();
	}
}
