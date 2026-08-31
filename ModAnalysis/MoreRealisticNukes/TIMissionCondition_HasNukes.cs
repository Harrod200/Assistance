using System;
using PavonisInteractive.TerraInvicta;

namespace MoreRealisticNukes
{
	// Token: 0x02000006 RID: 6
	public class TIMissionCondition_HasNukes : TIMissionCondition
	{
		// Token: 0x06000016 RID: 22 RVA: 0x0000297C File Offset: 0x00000B7C
		public override string CanTarget(TICouncilorState councilor, TIGameState possibleTarget)
		{
			string text;
			if (possibleTarget == null || !possibleTarget.isNationState)
			{
				text = base.GetType().Name;
			}
			else
			{
				TINationState ref_nation = possibleTarget.ref_nation;
				text = ((ref_nation != null && ref_nation.numNuclearWeapons > 0) ? "_Pass" : base.GetType().Name);
			}
			return text;
		}
	}
}
