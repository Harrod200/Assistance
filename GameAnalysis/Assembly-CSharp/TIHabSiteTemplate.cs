using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200036C RID: 876
public class TIHabSiteTemplate : TIDataTemplate
{
	// Token: 0x06000FD4 RID: 4052 RVA: 0x000523D4 File Offset: 0x000505D4
	public override TIGameState CreateGameState()
	{
		TIGameState tigameState = base.CreateGameState();
		if (tigameState == null)
		{
			tigameState = GameStateManager.CreateNewGameState<TIHabSiteState>();
		}
		return tigameState;
	}

	// Token: 0x170001D1 RID: 465
	// (get) Token: 0x06000FD5 RID: 4053 RVA: 0x000523F8 File Offset: 0x000505F8
	public TIMiningProfileTemplate miningProfile
	{
		get
		{
			return TemplateManager.Find<TIMiningProfileTemplate>(this.miningProfileName, false);
		}
	}

	// Token: 0x170001D2 RID: 466
	// (get) Token: 0x06000FD6 RID: 4054 RVA: 0x00052406 File Offset: 0x00050606
	public TISpaceBodyState parentBody
	{
		get
		{
			return GameStateManager.FindByTemplate<TISpaceBodyState>(this.parentBodyName, false);
		}
	}

	// Token: 0x04001012 RID: 4114
	public string parentBodyName;

	// Token: 0x04001013 RID: 4115
	public int x;

	// Token: 0x04001014 RID: 4116
	public int y;

	// Token: 0x04001015 RID: 4117
	public float? latitude;

	// Token: 0x04001016 RID: 4118
	public float? longitude;

	// Token: 0x04001017 RID: 4119
	public string miningProfileName;

	// Token: 0x04001018 RID: 4120
	public string backgroundPath;
}
