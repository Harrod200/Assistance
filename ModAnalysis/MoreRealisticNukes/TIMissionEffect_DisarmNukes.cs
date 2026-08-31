using System;
using PavonisInteractive.TerraInvicta;

namespace MoreRealisticNukes
{
	// Token: 0x02000007 RID: 7
	public class TIMissionEffect_DisarmNukes : TIMissionEffect
	{
		// Token: 0x06000018 RID: 24 RVA: 0x000029E8 File Offset: 0x00000BE8
		public override string ApplyEffect(TIMissionState mission, TIGameState target, TIMissionOutcome outcome)
		{
			string text;
			if (target == null || !target.isNationState || target.ref_nation == null)
			{
				text = string.Empty;
			}
			else
			{
				if (outcome == 5)
				{
					target.ref_nation.ChangeNumNuclearWeapons(-2);
				}
				else if (outcome == 4)
				{
					target.ref_nation.ChangeNumNuclearWeapons(-1);
				}
				text = string.Empty;
			}
			return text;
		}
	}
}
