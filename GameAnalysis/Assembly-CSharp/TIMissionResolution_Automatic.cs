using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000261 RID: 609
public class TIMissionResolution_Automatic : TIMissionResolution
{
	// Token: 0x17000103 RID: 259
	// (get) Token: 0x060007DA RID: 2010 RVA: 0x00024C8E File Offset: 0x00022E8E
	public override bool automaticSuccess
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060007DB RID: 2011 RVA: 0x00024C91 File Offset: 0x00022E91
	public override float GetSuccessChance(TIMissionTemplate mission, TICouncilorState councilor = null, TIGameState target = null, float resourcesSpent = 0f, bool reValidateTarget = false)
	{
		return 1f;
	}

	// Token: 0x060007DC RID: 2012 RVA: 0x00024C98 File Offset: 0x00022E98
	public override TIMissionResult GetMissionOutcome(TIMissionTemplate mission, TICouncilorState councilor = null, TIGameState target = null, float resourcesSpent = 0f)
	{
		return new TIMissionResult
		{
			roll = 0f,
			outcome = TIMissionOutcome.Success
		};
	}
}
