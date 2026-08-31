using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020002A0 RID: 672
public class TIArmyTemplate : TIDataTemplate
{
	// Token: 0x06000938 RID: 2360 RVA: 0x0002CA0C File Offset: 0x0002AC0C
	public override TIGameState CreateGameState()
	{
		TIGameState tigameState = base.CreateGameState();
		if (tigameState == null)
		{
			tigameState = GameStateManager.CreateNewGameState<TIArmyState>();
		}
		return tigameState;
	}

	// Token: 0x1700012B RID: 299
	// (get) Token: 0x06000939 RID: 2361 RVA: 0x0002CA30 File Offset: 0x0002AC30
	public TIRegionState startRegion
	{
		get
		{
			TIRegionState tiregionState = GameStateManager.FindByTemplate<TIRegionState>(this.startRegionStr, false);
			if (tiregionState != null)
			{
				return tiregionState;
			}
			Log.Error("Bad start region string " + this.startRegionStr + " for " + base.dataName, Array.Empty<object>());
			return null;
		}
	}

	// Token: 0x1700012C RID: 300
	// (get) Token: 0x0600093A RID: 2362 RVA: 0x0002CA7C File Offset: 0x0002AC7C
	public TIRegionState homeRegion
	{
		get
		{
			TIRegionState tiregionState = GameStateManager.FindByTemplate<TIRegionState>(this.homeRegionStr, false);
			if (tiregionState != null)
			{
				return tiregionState;
			}
			Log.Error("Bad start region string " + this.homeRegionStr + " for " + base.dataName, Array.Empty<object>());
			return null;
		}
	}

	// Token: 0x0400075C RID: 1884
	public string startRegionStr;

	// Token: 0x0400075D RID: 1885
	public string homeRegionStr;

	// Token: 0x0400075E RID: 1886
	public DeploymentType deploymentType;

	// Token: 0x0400075F RID: 1887
	public ArmyType armyType;

	// Token: 0x04000760 RID: 1888
	public float startingStrength;
}
