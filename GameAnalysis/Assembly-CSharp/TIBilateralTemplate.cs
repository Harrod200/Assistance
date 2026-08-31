using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x020002A1 RID: 673
public class TIBilateralTemplate : TIDataTemplate
{
	// Token: 0x1700012D RID: 301
	// (get) Token: 0x0600093C RID: 2364 RVA: 0x0002CACF File Offset: 0x0002ACCF
	public TIProjectTemplate projectUnlock
	{
		get
		{
			return TemplateManager.Find<TIProjectTemplate>(this.projectUnlockName, false);
		}
	}

	// Token: 0x1700012E RID: 302
	// (get) Token: 0x0600093D RID: 2365 RVA: 0x0002CADD File Offset: 0x0002ACDD
	public TINationState nationState1
	{
		get
		{
			if (this._nationState1 == null)
			{
				this._nationState1 = GameStateManager.FindByTemplate<TINationState>(this.nation1, false);
			}
			return this._nationState1;
		}
	}

	// Token: 0x1700012F RID: 303
	// (get) Token: 0x0600093E RID: 2366 RVA: 0x0002CB05 File Offset: 0x0002AD05
	public TINationState nationState2
	{
		get
		{
			if (this._nationState2 == null)
			{
				this._nationState2 = GameStateManager.FindByTemplate<TINationState>(this.nation2, false);
			}
			return this._nationState2;
		}
	}

	// Token: 0x17000130 RID: 304
	// (get) Token: 0x0600093F RID: 2367 RVA: 0x0002CB30 File Offset: 0x0002AD30
	public TIRegionState regionState1
	{
		get
		{
			if (this._regionState1 == null)
			{
				this._regionState1 = GameStateManager.MapRegionLookup(this.region1);
				if (this._regionState1 == null)
				{
					this._regionState1 = GameStateManager.FindByTemplate<TIRegionState>(this.region1, false);
				}
			}
			return this._regionState1;
		}
	}

	// Token: 0x17000131 RID: 305
	// (get) Token: 0x06000940 RID: 2368 RVA: 0x0002CB84 File Offset: 0x0002AD84
	public TIRegionState regionState2
	{
		get
		{
			if (this._regionState2 == null)
			{
				this._regionState2 = GameStateManager.MapRegionLookup(this.region2);
				if (this._regionState2 == null)
				{
					this._regionState2 = GameStateManager.FindByTemplate<TIRegionState>(this.region2, false);
				}
			}
			return this._regionState2;
		}
	}

	// Token: 0x06000941 RID: 2369 RVA: 0x0002CBD8 File Offset: 0x0002ADD8
	public TIGameState CheckToCreateGameState()
	{
		BilateralRelationType bilateralRelationType = this.relationType;
		if (bilateralRelationType != BilateralRelationType.Federation)
		{
			if (bilateralRelationType == BilateralRelationType.War)
			{
				foreach (TIWarState tiwarState in GameStateManager.IterateByClass<TIWarState>(false))
				{
					TINationState attacker = tiwarState.attacker;
					if (((attacker != null) ? attacker.templateName : null) == this.nation1)
					{
						TINationState defender = tiwarState.defender;
						if (((defender != null) ? defender.templateName : null) == this.nation2)
						{
							return null;
						}
					}
				}
				GameStateManager.CreateNewGameState<TIWarState>();
			}
		}
		else
		{
			using (IEnumerator<TIFederationState> enumerator2 = GameStateManager.IterateByClass<TIFederationState>(false).GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					if (enumerator2.Current.federationName == this.federation)
					{
						return null;
					}
				}
			}
			GameStateManager.CreateNewGameState<TIFederationState>().federationName = this.federation;
		}
		return null;
	}

	// Token: 0x06000942 RID: 2370 RVA: 0x0002CCE0 File Offset: 0x0002AEE0
	public bool BilateralIsInScenario()
	{
		if (!this._currentScenarioSet)
		{
			switch (this.relationType)
			{
			case BilateralRelationType.Federation:
				this._inCurrentScenario = this.nationState1 != null && !string.IsNullOrEmpty(this.federation);
				break;
			case BilateralRelationType.Alliance:
			case BilateralRelationType.Rivalry:
			case BilateralRelationType.War:
			case BilateralRelationType.Breakaway:
				this._inCurrentScenario = this.nationState1 != null && this.nationState2 != null;
				break;
			case BilateralRelationType.PhysicalAdjacency:
				this._inCurrentScenario = this.regionState1 != null && this.regionState2 != null;
				break;
			case BilateralRelationType.Claim:
				this._inCurrentScenario = this.nationState1 != null && this.regionState1 != null;
				break;
			}
			this._currentScenarioSet = true;
		}
		return this._inCurrentScenario;
	}

	// Token: 0x06000943 RID: 2371 RVA: 0x0002CDCC File Offset: 0x0002AFCC
	public bool BilateralIsInScenario_FromTemplates(List<TINationTemplate> nationsInScenario, List<string> completedProjects, bool includingGatedByTech)
	{
		if (this._currentScenarioSet)
		{
			return this._inCurrentScenario;
		}
		bool flag = includingGatedByTech || string.IsNullOrEmpty(this.projectUnlockName) || completedProjects.Contains(this.projectUnlockName);
		IEnumerable<string> enumerable = nationsInScenario.Select<TINationTemplate, string>((TINationTemplate x) => x.dataName);
		if (this.nation1 != string.Empty && enumerable.Contains(this.nation1))
		{
			return flag;
		}
		return this.nation2 != string.Empty && enumerable.Contains(this.nation2) && flag;
	}

	// Token: 0x06000944 RID: 2372 RVA: 0x0002CE73 File Offset: 0x0002B073
	public bool BilateralIsActive()
	{
		if (this.BilateralCanBeActive)
		{
			TIProjectTemplate projectUnlock = this.projectUnlock;
			return projectUnlock == null || projectUnlock.SomeoneHasDoneIt();
		}
		return false;
	}

	// Token: 0x17000132 RID: 306
	// (get) Token: 0x06000945 RID: 2373 RVA: 0x0002CE90 File Offset: 0x0002B090
	public bool BilateralCanBeActive
	{
		get
		{
			return this.BilateralIsInScenario();
		}
	}

	// Token: 0x04000761 RID: 1889
	public BilateralRelationType relationType;

	// Token: 0x04000762 RID: 1890
	public string federation;

	// Token: 0x04000763 RID: 1891
	public string nation1;

	// Token: 0x04000764 RID: 1892
	public string nation2;

	// Token: 0x04000765 RID: 1893
	public string region1;

	// Token: 0x04000766 RID: 1894
	public string region2;

	// Token: 0x04000767 RID: 1895
	public string projectUnlockName;

	// Token: 0x04000768 RID: 1896
	public bool capitalClaim;

	// Token: 0x04000769 RID: 1897
	public bool initialOwner;

	// Token: 0x0400076A RID: 1898
	public bool initialColony;

	// Token: 0x0400076B RID: 1899
	public bool friendlyOnly;

	// Token: 0x0400076C RID: 1900
	public bool hostileClaim;

	// Token: 0x0400076D RID: 1901
	private bool _currentScenarioSet;

	// Token: 0x0400076E RID: 1902
	private bool _inCurrentScenario;

	// Token: 0x0400076F RID: 1903
	private TINationState _nationState1;

	// Token: 0x04000770 RID: 1904
	private TINationState _nationState2;

	// Token: 0x04000771 RID: 1905
	private TIRegionState _regionState1;

	// Token: 0x04000772 RID: 1906
	private TIRegionState _regionState2;
}
