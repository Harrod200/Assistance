using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000266 RID: 614
public interface IMissionTarget<T> : IMissionTarget where T : TIGameState
{
	// Token: 0x060007EF RID: 2031
	IList<T> GetValidTargets(TIMissionTemplate mission, TICouncilorState councilor);

	// Token: 0x060007F0 RID: 2032
	IEnumerable<T> GetAllPotentialTargets(TIFactionState faction);
}
