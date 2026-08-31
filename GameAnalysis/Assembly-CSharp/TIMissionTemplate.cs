using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000291 RID: 657
public class TIMissionTemplate : TIDataTemplate
{
	// Token: 0x1700010A RID: 266
	// (get) Token: 0x060008F3 RID: 2291 RVA: 0x00029E70 File Offset: 0x00028070
	public CouncilorAttribute primaryAttackerStat
	{
		get
		{
			if (!this._primaryAttackerStatSet)
			{
				foreach (TIMissionModifier timissionModifier in this.resolutionMethod.attackingModifiers)
				{
					if (timissionModifier.GetType() == typeof(TIMissionModifier_CouncilorAttackStat))
					{
						this._primaryAttackerStat = (timissionModifier as TIMissionModifier_CouncilorAttackStat).attackerAttribute;
						break;
					}
				}
				this._primaryAttackerStatSet = true;
			}
			return this._primaryAttackerStat;
		}
	}

	// Token: 0x060008F4 RID: 2292 RVA: 0x00029F00 File Offset: 0x00028100
	public CouncilorAttribute primaryDefenderStat()
	{
		if (!this._primaryDefenderStatSet)
		{
			foreach (TIMissionModifier timissionModifier in this.resolutionMethod.defendingModifiers)
			{
				if (timissionModifier.GetType() == typeof(TIMissionModifier_CouncilorDefendStat))
				{
					this._primaryDefenderStat = (timissionModifier as TIMissionModifier_CouncilorDefendStat).defenderAttribute;
					break;
				}
			}
			this._primaryDefenderStatSet = true;
		}
		return this._primaryDefenderStat;
	}

	// Token: 0x1700010B RID: 267
	// (get) Token: 0x060008F5 RID: 2293 RVA: 0x00029F90 File Offset: 0x00028190
	public FactionResource primaryResource
	{
		get
		{
			if (!this.hasCost)
			{
				return FactionResource.None;
			}
			return this.cost.resourceType;
		}
	}

	// Token: 0x1700010C RID: 268
	// (get) Token: 0x060008F6 RID: 2294 RVA: 0x00029FA7 File Offset: 0x000281A7
	public bool ContestedMission
	{
		get
		{
			return this.resolutionMethod.GetType() == typeof(TIMissionResolution_Contested);
		}
	}

	// Token: 0x1700010D RID: 269
	// (get) Token: 0x060008F7 RID: 2295 RVA: 0x00029FC3 File Offset: 0x000281C3
	public bool hasCost
	{
		get
		{
			return this.cost != null;
		}
	}

	// Token: 0x1700010E RID: 270
	// (get) Token: 0x060008F8 RID: 2296 RVA: 0x00029FCE File Offset: 0x000281CE
	public bool UsesSlider
	{
		get
		{
			return this.hasCost && this.cost.GetType() == typeof(TIMissionCost_Bonus);
		}
	}

	// Token: 0x1700010F RID: 271
	// (get) Token: 0x060008F9 RID: 2297 RVA: 0x00029FF4 File Offset: 0x000281F4
	public bool IsVictoryMission
	{
		get
		{
			return this.targetEffects.Concat<TIMissionEffect>(this.councilorEffects).Any<TIMissionEffect>((TIMissionEffect x) => x is TIMissionEffect_Win);
		}
	}

	// Token: 0x17000110 RID: 272
	// (get) Token: 0x060008FA RID: 2298 RVA: 0x0002A02C File Offset: 0x0002822C
	public string description
	{
		get
		{
			return new StringBuilder(Loc.T(new StringBuilder(base.GetType().Name).Append(".description.").Append(base.dataName).ToString())).Append(this.keyValues).ToString();
		}
	}

	// Token: 0x17000111 RID: 273
	// (get) Token: 0x060008FB RID: 2299 RVA: 0x0002A080 File Offset: 0x00028280
	public string descriptionWithTiming
	{
		get
		{
			return new StringBuilder(this.description).AppendLine().AppendLine().AppendLine(Loc.T(new StringBuilder("TIMission.Timing.").Append(this.resolutionOrder.ToString()).ToString()))
				.ToString();
		}
	}

	// Token: 0x060008FC RID: 2300 RVA: 0x0002A0D0 File Offset: 0x000282D0
	public string CriticalSuccessText(params string[] args)
	{
		return Loc.T(new StringBuilder(base.GetType().Name).Append(".CriticalSuccess.").Append(base.dataName).ToString(), args);
	}

	// Token: 0x060008FD RID: 2301 RVA: 0x0002A110 File Offset: 0x00028310
	public string SuccessText(params string[] args)
	{
		return Loc.T(new StringBuilder(base.GetType().Name).Append(".Success.").Append(base.dataName).ToString(), args);
	}

	// Token: 0x060008FE RID: 2302 RVA: 0x0002A150 File Offset: 0x00028350
	public string FailureText(params string[] args)
	{
		return Loc.T(new StringBuilder(base.GetType().Name).Append(".Failure.").Append(base.dataName).ToString(), args);
	}

	// Token: 0x060008FF RID: 2303 RVA: 0x0002A190 File Offset: 0x00028390
	public string CriticalFailureText(params string[] args)
	{
		return Loc.T(new StringBuilder(base.GetType().Name).Append(".CriticalFailure.").Append(base.dataName).ToString(), args);
	}

	// Token: 0x17000112 RID: 274
	// (get) Token: 0x06000900 RID: 2304 RVA: 0x0002A1CF File Offset: 0x000283CF
	public string missionIconImagePath_On
	{
		get
		{
			return new StringBuilder(this.missionIconImagePath).Append("_on").ToString();
		}
	}

	// Token: 0x17000113 RID: 275
	// (get) Token: 0x06000901 RID: 2305 RVA: 0x0002A1EB File Offset: 0x000283EB
	public string missionIconImagePath_Off
	{
		get
		{
			return new StringBuilder(this.missionIconImagePath).Append("_off").ToString();
		}
	}

	// Token: 0x17000114 RID: 276
	// (get) Token: 0x06000902 RID: 2306 RVA: 0x0002A207 File Offset: 0x00028407
	public string iconAnimationController
	{
		get
		{
			return new StringBuilder(base.dataName).Append("/").Append(base.dataName).Append("_Animator")
				.ToString();
		}
	}

	// Token: 0x17000115 RID: 277
	// (get) Token: 0x06000903 RID: 2307 RVA: 0x0002A238 File Offset: 0x00028438
	public string pendingAnimation
	{
		get
		{
			return new StringBuilder(base.dataName).Append("/").Append(base.dataName).Append("_SS_P")
				.ToString();
		}
	}

	// Token: 0x17000116 RID: 278
	// (get) Token: 0x06000904 RID: 2308 RVA: 0x0002A269 File Offset: 0x00028469
	public string resolvingAnimation
	{
		get
		{
			return new StringBuilder(base.dataName).Append("/").Append(base.dataName).Append("_SS_R")
				.ToString();
		}
	}

	// Token: 0x06000905 RID: 2309 RVA: 0x0002A29C File Offset: 0x0002849C
	public string GetCompletedIllustrationResource(TIGameState missionTarget, TIControlPoint targetControlPoint)
	{
		List<string> list = this.completedIllustrationResource;
		if (list == null || list.Count <= 0)
		{
			return string.Empty;
		}
		string text = this.completedIllustrationResource[0];
		if (text != null)
		{
			if (text == "special_ControlPoint")
			{
				return ((targetControlPoint != null) ? targetControlPoint.GetIllustrationPath() : null) ?? string.Empty;
			}
			if (text == "special_AlienAssetDestroyed")
			{
				return missionTarget.ref_regionAlienAsset.GetDestroyedIllustrationPath();
			}
		}
		if (this.completedIllustrationResource.Count >= 3 && !string.IsNullOrEmpty(this.completedIllustrationResource[2]))
		{
			TIFactionState ref_faction = missionTarget.ref_faction;
			if (ref_faction != null && ref_faction.IsAlienFaction)
			{
				return this.completedIllustrationResource[2];
			}
		}
		if (this.completedIllustrationResource.Count >= 2 && !string.IsNullOrEmpty(this.completedIllustrationResource[1]) && missionTarget.inSpace)
		{
			return this.completedIllustrationResource[1];
		}
		return this.completedIllustrationResource[0];
	}

	// Token: 0x17000117 RID: 279
	// (get) Token: 0x06000906 RID: 2310 RVA: 0x0002A39C File Offset: 0x0002859C
	public string keyValues
	{
		get
		{
			StringBuilder stringBuilder = new StringBuilder();
			CouncilorAttribute primaryAttackerStat = this.primaryAttackerStat;
			if (primaryAttackerStat != CouncilorAttribute.None)
			{
				stringBuilder.Append(" ").Append(Loc.T("UI.Councilor.MissionKeyStat", new object[] { TIUtilities.InlineAttributeStr(primaryAttackerStat) }));
			}
			FactionResource primaryResource = this.primaryResource;
			if (this.primaryResource != FactionResource.None)
			{
				if (this.cost is TIMissionCost_FlatOnEarth)
				{
					stringBuilder.Append(" ").Append(Loc.T("UI.Councilor.MissionResource_FlatOnEarth", new object[]
					{
						TIUtilities.FormatSmallNumber(this.cost.value, 7, 0, true, false),
						TIUtilities.InlineResourceStr(primaryResource)
					}));
				}
				else if (this.cost is TIMissionCost_Flat)
				{
					stringBuilder.Append(" ").Append(Loc.T("UI.Councilor.MissionResource_Flat", new object[]
					{
						TIUtilities.FormatSmallNumber(this.cost.value, 7, 0, true, false),
						TIUtilities.InlineResourceStr(primaryResource)
					}));
				}
				else
				{
					stringBuilder.Append(" ").Append(Loc.T("UI.Councilor.MissionResource", new object[] { TIUtilities.InlineResourceStr(primaryResource) }));
				}
			}
			return stringBuilder.ToString();
		}
	}

	// Token: 0x17000118 RID: 280
	// (get) Token: 0x06000907 RID: 2311 RVA: 0x0002A4C8 File Offset: 0x000286C8
	public string multiLineDescriptionWithModifiers
	{
		get
		{
			StringBuilder stringBuilder = new StringBuilder(TIUtilities.HighlightLine(this.displayName)).AppendLine().AppendLine().Append(this.descriptionWithTiming)
				.AppendLine();
			if (this.ContestedMission)
			{
				if (this.resolutionMethod.attackingModifiers.Count > 0)
				{
					stringBuilder.AppendLine(Loc.T("TIMission_AttackerModifiers"));
					foreach (TIMissionModifier timissionModifier in this.resolutionMethod.attackingModifiers)
					{
						TIMissionModifier_HideInCodex timissionModifier_HideInCodex = timissionModifier as TIMissionModifier_HideInCodex;
						if (timissionModifier_HideInCodex != null)
						{
							if (timissionModifier_HideInCodex.ShowCondition(GameControl.control.activePlayer))
							{
								stringBuilder.AppendLine(timissionModifier.displayName);
							}
						}
						else if (!(timissionModifier is TIMissionModifier_ResourceSpent))
						{
							stringBuilder.AppendLine(timissionModifier.displayName);
						}
					}
					stringBuilder.AppendLine();
				}
				if (this.resolutionMethod.defendingModifiers.Count > 0)
				{
					stringBuilder.AppendLine(Loc.T("TIMission_DefenderModifiers"));
					foreach (TIMissionModifier timissionModifier2 in this.resolutionMethod.defendingModifiers)
					{
						TIMissionModifier_HideInCodex timissionModifier_HideInCodex2 = timissionModifier2 as TIMissionModifier_HideInCodex;
						if (timissionModifier_HideInCodex2 != null)
						{
							if (timissionModifier_HideInCodex2.ShowCondition(GameControl.control.activePlayer))
							{
								stringBuilder.AppendLine(timissionModifier2.displayName);
							}
						}
						else
						{
							stringBuilder.AppendLine(timissionModifier2.displayName);
						}
					}
				}
			}
			return stringBuilder.ToString();
		}
	}

	// Token: 0x06000908 RID: 2312 RVA: 0x0002A66C File Offset: 0x0002886C
	public TIMissionTemplate(string name)
		: base(name)
	{
	}

	// Token: 0x06000909 RID: 2313 RVA: 0x0002A6E8 File Offset: 0x000288E8
	public override TIGameState CreateGameState()
	{
		TIGameState tigameState = base.CreateGameState();
		if (tigameState == null)
		{
			tigameState = GameStateManager.CreateNewGameState<TIMissionState>();
		}
		return tigameState;
	}

	// Token: 0x0600090A RID: 2314 RVA: 0x0002A70C File Offset: 0x0002890C
	public IList<TIGameState> GetValidTargets(TICouncilorState councilor)
	{
		return this.target.GetValidTargets(this, councilor);
	}

	// Token: 0x0600090B RID: 2315 RVA: 0x0002A71C File Offset: 0x0002891C
	public bool CanAfford(TIFactionState faction, TICouncilorState councilor = null)
	{
		if (this.hasCost)
		{
			TIMissionCost_Flat timissionCost_Flat = this.cost as TIMissionCost_Flat;
			if (timissionCost_Flat != null)
			{
				if (councilor != null)
				{
					TIMissionState activeMission = councilor.activeMission;
					if (activeMission != null && TIMissionPhaseState.InMissionPhase() && activeMission.missionTemplate.cost != null && activeMission.missionTemplate.cost.resourceType == timissionCost_Flat.resourceType)
					{
						float resources = activeMission.resources;
						return faction.GetCurrentResourceAmount(timissionCost_Flat.resourceType) + resources >= this.cost.value;
					}
				}
				return faction.GetCurrentResourceAmount(timissionCost_Flat.resourceType) >= this.cost.value;
			}
		}
		return true;
	}

	// Token: 0x0600090C RID: 2316 RVA: 0x0002A7D0 File Offset: 0x000289D0
	public static string MissionTargetingList(TIFactionState faction, TIGameState target)
	{
		List<MissionOption> list = new List<MissionOption>();
		foreach (TICouncilorState ticouncilorState in faction.activeCouncilors)
		{
			list.AddRange(ticouncilorState.MissionOptionsForTarget(target));
		}
		list = (from x in list
			orderby x.mission.sortOrder, x.baseChance descending
			select x).ToList<MissionOption>();
		StringBuilder stringBuilder = new StringBuilder();
		foreach (MissionOption missionOption in list)
		{
			stringBuilder.Append(missionOption.councilor.displayName).Append(" ").Append(missionOption.mission.displayName)
				.Append(" ")
				.Append(missionOption.baseChance.ToPercent("P0"))
				.AppendLine();
		}
		return stringBuilder.ToString();
	}

	// Token: 0x0600090D RID: 2317 RVA: 0x0002A918 File Offset: 0x00028B18
	public string MissionDetailText()
	{
		StringBuilder stringBuilder = new StringBuilder();
		List<TIMissionModifier> list = new List<TIMissionModifier>(this.resolutionMethod.attackingModifiers);
		if (list.Count > 0)
		{
			foreach (TIMissionModifier timissionModifier in this.resolutionMethod.attackingModifiers)
			{
				TIMissionModifier_HideInCodex timissionModifier_HideInCodex = timissionModifier as TIMissionModifier_HideInCodex;
				if (timissionModifier_HideInCodex != null && !timissionModifier_HideInCodex.ShowCondition(GameControl.control.activePlayer))
				{
					list.Remove(timissionModifier);
				}
			}
		}
		List<TIMissionModifier> list2 = new List<TIMissionModifier>(this.resolutionMethod.defendingModifiers);
		if (list2.Count > 0)
		{
			foreach (TIMissionModifier timissionModifier2 in this.resolutionMethod.defendingModifiers)
			{
				TIMissionModifier_HideInCodex timissionModifier_HideInCodex2 = timissionModifier2 as TIMissionModifier_HideInCodex;
				if (timissionModifier_HideInCodex2 != null && !timissionModifier_HideInCodex2.ShowCondition(GameControl.control.activePlayer))
				{
					list2.Remove(timissionModifier2);
				}
			}
		}
		for (int i = 0; i < Math.Max(list.Count, list2.Count); i++)
		{
			if (i == 0 && ((list != null && list[i] != null) || (list2 != null && list2[i] != null)))
			{
				stringBuilder.AppendLine(Loc.T("TIMissionModifier.CodexHeader"));
			}
			if (i < ((list != null) ? list.Count : 0) && list[i] != null)
			{
				if (list[i] is TIMissionModifier_ResourceSpent)
				{
					stringBuilder.Append(Loc.T("TIMissionModifier.Spend", new object[] { TIUtilities.GetResourceString(this.cost.resourceType) }));
				}
				else
				{
					stringBuilder.Append(list[i].displayName);
				}
			}
			if (i < ((list2 != null) ? list2.Count : 0) && list2[i] != null)
			{
				TIMissionModifier_FlatModifier timissionModifier_FlatModifier = list2[i] as TIMissionModifier_FlatModifier;
				if (timissionModifier_FlatModifier != null)
				{
					stringBuilder.Append("<line-height=0.01%>\n<align=\"right\">").Append(Loc.T("TIMissionModifier.Flat", new object[]
					{
						list2[i].displayName,
						timissionModifier_FlatModifier.flatModifier
					})).Append("</align></line-height>");
				}
				else
				{
					stringBuilder.Append("<line-height=0.01%>\n<align=\"right\">").Append(list2[i].displayName).Append("</align></line-height>");
				}
			}
			stringBuilder.AppendLine();
		}
		return stringBuilder.ToString();
	}

	// Token: 0x04000643 RID: 1603
	public bool baseMission;

	// Token: 0x04000644 RID: 1604
	public float[] noise = new float[6];

	// Token: 0x04000645 RID: 1605
	public float[] hate = new float[6];

	// Token: 0x04000646 RID: 1606
	public int XPonSuccess;

	// Token: 0x04000647 RID: 1607
	public int resolutionOrder;

	// Token: 0x04000648 RID: 1608
	public MissionMovementRule movementRule;

	// Token: 0x04000649 RID: 1609
	public TIMissionResolution resolutionMethod;

	// Token: 0x0400064A RID: 1610
	public List<Context> attackerContexts = new List<Context>();

	// Token: 0x0400064B RID: 1611
	public List<Context> defenderContexts = new List<Context>();

	// Token: 0x0400064C RID: 1612
	public List<TIMissionEffect> targetEffects = new List<TIMissionEffect>();

	// Token: 0x0400064D RID: 1613
	public List<TIMissionEffect> councilorEffects = new List<TIMissionEffect>();

	// Token: 0x0400064E RID: 1614
	public IMissionTarget target;

	// Token: 0x0400064F RID: 1615
	public List<TIMissionCondition> conditions = new List<TIMissionCondition>();

	// Token: 0x04000650 RID: 1616
	public TIMissionCost cost;

	// Token: 0x04000651 RID: 1617
	public int sortOrder;

	// Token: 0x04000652 RID: 1618
	public MissionContext missionContext;

	// Token: 0x04000653 RID: 1619
	public bool persistentEffect;

	// Token: 0x04000654 RID: 1620
	public float utilityScore;

	// Token: 0x04000655 RID: 1621
	public bool AIDoubleUpAllowed;

	// Token: 0x04000656 RID: 1622
	public int maximumTargetOptionCount = int.MaxValue;

	// Token: 0x04000657 RID: 1623
	public bool specialPost;

	// Token: 0x04000658 RID: 1624
	public bool permanentAssignment;

	// Token: 0x04000659 RID: 1625
	public bool debugForced;

	// Token: 0x0400065A RID: 1626
	public string knowledgeProject;

	// Token: 0x0400065B RID: 1627
	public string successSFX;

	// Token: 0x0400065C RID: 1628
	public string successSFXAlienSpecial;

	// Token: 0x0400065D RID: 1629
	public bool UIalertEnemyOnFail;

	// Token: 0x0400065E RID: 1630
	public bool allowedForAutoDefense;

	// Token: 0x0400065F RID: 1631
	private CouncilorAttribute _primaryAttackerStat;

	// Token: 0x04000660 RID: 1632
	private bool _primaryAttackerStatSet;

	// Token: 0x04000661 RID: 1633
	private CouncilorAttribute _primaryDefenderStat;

	// Token: 0x04000662 RID: 1634
	private bool _primaryDefenderStatSet;

	// Token: 0x04000663 RID: 1635
	public string missionIconImagePath;

	// Token: 0x04000664 RID: 1636
	public List<string> completedIllustrationResource = new List<string>();

	// Token: 0x04000665 RID: 1637
	public Type targetingMethodType;
}
