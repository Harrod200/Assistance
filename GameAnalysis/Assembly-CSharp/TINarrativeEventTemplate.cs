using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PavonisInteractive.TerraInvicta;

// Token: 0x020002A8 RID: 680
public class TINarrativeEventTemplate : TIDataTemplate
{
	// Token: 0x06000950 RID: 2384 RVA: 0x0002DC21 File Offset: 0x0002BE21
	public string summary(TIGameState actingState, TIGameState target, TIGameState secondaryTarget)
	{
		return this.NarrativeEventStringReplacement(new StringBuilder(Loc.T(new StringBuilder("TINarrativeEventTemplate.").Append(base.dataName).Append(".summary").ToString())), actingState, target, secondaryTarget, true).ToString();
	}

	// Token: 0x06000951 RID: 2385 RVA: 0x0002DC60 File Offset: 0x0002BE60
	public string query(TIFactionState faction, TIGameState target, TIGameState secondaryTarget)
	{
		return this.NarrativeEventStringReplacement(new StringBuilder(this.summary(faction, target, secondaryTarget)).AppendLine().AppendLine().Append(Loc.T(new StringBuilder("TINarrativeEventTemplate.").Append(base.dataName).Append(".query").ToString())), faction, target, secondaryTarget, false).ToString();
	}

	// Token: 0x06000952 RID: 2386 RVA: 0x0002DCC4 File Offset: 0x0002BEC4
	public string optionButtonText(TIGameState actor, TIGameState target, TIGameState secondaryTarget, int idx)
	{
		return this.NarrativeEventStringReplacement(new StringBuilder(Loc.T(new StringBuilder("TINarrativeEventTemplate.").Append(base.dataName).Append(".option").Append(idx.ToString())
			.ToString())), actor, target, secondaryTarget, false).ToString();
	}

	// Token: 0x06000953 RID: 2387 RVA: 0x0002DD1C File Offset: 0x0002BF1C
	public string optionButtonDetail(TIFactionState faction, TIGameState target, TIGameState secondaryTarget, int idx, Dictionary<TIGameState, TIGameState> allTargets)
	{
		return this.NarrativeEventStringReplacement(this.eventOptions[idx].OptionDetail(faction, target, secondaryTarget, base.dataName, idx, allTargets, this), faction, target, secondaryTarget, false).ToString();
	}

	// Token: 0x06000954 RID: 2388 RVA: 0x0002DD5C File Offset: 0x0002BF5C
	public string optionSummary(TIGameState actingState, TIGameState target, TIGameState secondaryTarget, int idx)
	{
		return this.NarrativeEventStringReplacement(new StringBuilder(Loc.T(new StringBuilder("TINarrativeEventTemplate.").Append(base.dataName).Append(".optionResult").Append(idx.ToString())
			.ToString())), actingState, target, secondaryTarget, false).ToString();
	}

	// Token: 0x06000955 RID: 2389 RVA: 0x0002DDB4 File Offset: 0x0002BFB4
	public string outcomeSummary(TIGameState actingState, TIGameState target, TIGameState secondaryTarget, int optionIdx, int outcomeIdx)
	{
		return this.NarrativeEventStringReplacement(new StringBuilder(Loc.T(new StringBuilder("TINarrativeEventTemplate.").Append(base.dataName).Append(".option").Append(optionIdx.ToString())
			.Append(".outcome")
			.Append(outcomeIdx.ToString())
			.Append(".Summary")
			.ToString())), actingState, target, secondaryTarget, false).ToString();
	}

	// Token: 0x06000956 RID: 2390 RVA: 0x0002DE2C File Offset: 0x0002C02C
	public string outcomeDetail(TIGameState actingState, TIGameState target, TIGameState secondaryTarget, int optionIdx, int outcomeIdx)
	{
		return this.NarrativeEventStringReplacement(new StringBuilder(Loc.T(new StringBuilder("TINarrativeEventTemplate.").Append(base.dataName).Append(".option").Append(optionIdx.ToString())
			.Append(".outcome")
			.Append(outcomeIdx.ToString())
			.Append(".Detail")
			.ToString())), actingState, target, secondaryTarget, false).ToString();
	}

	// Token: 0x06000957 RID: 2391 RVA: 0x0002DEA4 File Offset: 0x0002C0A4
	public override bool IsValid(out string error)
	{
		if (this.eventOptions[0].outcomes.Any<NarrativeEventOutcome>((NarrativeEventOutcome x) => x.GetRawCosts().anyDebit))
		{
			if (this.eventOptions[0].outcomes.Any<NarrativeEventOutcome>((NarrativeEventOutcome x) => x.GetRawCosts().anyDebit) && (this.targetConditions == null || this.targetConditions.Count == 0))
			{
				error = "Option zero in " + base.dataName + " has positive resource costs or target conditions. Could potentially lock up the game by presenting no options";
				return false;
			}
		}
		return base.IsValid(out error);
	}

	// Token: 0x06000958 RID: 2392 RVA: 0x0002DF54 File Offset: 0x0002C154
	public bool ActorCanAffordAnyOption(TIFactionState faction, TIGameState target, TIGameState secondary)
	{
		if (faction != null)
		{
			IEnumerable<NarrativeEventOption> enumerable = this.eventOptions;
			Func<NarrativeEventOption, IEnumerable<NarrativeEventOutcome>> <>9__0;
			Func<NarrativeEventOption, IEnumerable<NarrativeEventOutcome>> func;
			if ((func = <>9__0) == null)
			{
				func = (<>9__0 = (NarrativeEventOption x) => x.possibleOutcomes(faction, target, secondary));
			}
			foreach (NarrativeEventOutcome narrativeEventOutcome in enumerable.SelectMany<NarrativeEventOption, NarrativeEventOutcome>(func))
			{
				TIResourcesCost costs = narrativeEventOutcome.GetCosts(null);
				if (costs == null || !costs.anyDebit)
				{
					return true;
				}
				if (costs.CanAfford(faction, 1f, null, float.PositiveInfinity))
				{
					return true;
				}
			}
			return false;
		}
		return true;
	}

	// Token: 0x17000135 RID: 309
	// (get) Token: 0x06000959 RID: 2393 RVA: 0x0002E02C File Offset: 0x0002C22C
	public bool ShouldCacheEventData
	{
		get
		{
			return this.targetType == NarrativeEventTargetType.priorActor || this.targetType == NarrativeEventTargetType.priorSecondary || this.targetType == NarrativeEventTargetType.priorTarget || this.secondaryStateType == EffectSecondaryStateType.PriorEvent_Actor || this.secondaryStateType == EffectSecondaryStateType.PriorEvent_SecondaryTarget || this.secondaryStateType == EffectSecondaryStateType.PriorEvent_Target;
		}
	}

	// Token: 0x0600095A RID: 2394 RVA: 0x0002E06C File Offset: 0x0002C26C
	private StringBuilder NarrativeEventStringReplacement(StringBuilder baseString, TIGameState actingState, TIGameState target, TIGameState secondaryTarget, bool isSummary = false)
	{
		StringBuilder stringBuilder = new StringBuilder(baseString.ToString());
		stringBuilder.Replace("{eventDisplayName}", this.displayName);
		stringBuilder.Replace("{actorName}", actingState.displayName);
		StringBuilder stringBuilder2 = stringBuilder;
		string text = "{actorNameWithArticle}";
		string text2;
		if (!(actingState is TINationState))
		{
			TIFactionState ref_faction = actingState.ref_faction;
			text2 = ((ref_faction != null) ? ref_faction.displayNameWithColor : null);
		}
		else
		{
			text2 = actingState.ref_nation.displayNameWithArticle;
		}
		stringBuilder2.Replace(text, text2);
		StringBuilder stringBuilder3 = stringBuilder;
		string text3 = "{actorNameWithArticleCap}";
		string text4;
		if (!(actingState is TINationState))
		{
			TIFactionState ref_faction2 = actingState.ref_faction;
			text4 = ((ref_faction2 != null) ? ref_faction2.displayNameCapitalizedWithColor : null);
		}
		else
		{
			text4 = actingState.ref_nation.displayNameWithArticleCapitalized;
		}
		stringBuilder3.Replace(text3, text4);
		StringBuilder stringBuilder4 = stringBuilder;
		string text5 = "{actingFactionAdjective}";
		TIFactionState ref_faction3 = actingState.ref_faction;
		stringBuilder4.Replace(text5, (ref_faction3 != null) ? ref_faction3.adjectiveWithColor : null);
		StringBuilder stringBuilder5 = stringBuilder;
		string text6 = "{targetFactionName}";
		TIFactionState ref_faction4 = target.ref_faction;
		stringBuilder5.Replace(text6, (ref_faction4 != null) ? ref_faction4.displayNameWithColor : null);
		StringBuilder stringBuilder6 = stringBuilder;
		string text7 = "{targetFactionNameCap}";
		TIFactionState ref_faction5 = target.ref_faction;
		stringBuilder6.Replace(text7, (ref_faction5 != null) ? ref_faction5.displayNameCapitalizedWithColor : null);
		StringBuilder stringBuilder7 = stringBuilder;
		string text8 = "{targetFactionAdjective}";
		TIFactionState ref_faction6 = target.ref_faction;
		stringBuilder7.Replace(text8, (ref_faction6 != null) ? ref_faction6.adjectiveWithColor : null);
		StringBuilder stringBuilder8 = stringBuilder;
		string text9 = "{targetNationName}";
		TINationState ref_nation = target.ref_nation;
		stringBuilder8.Replace(text9, (ref_nation != null) ? ref_nation.displayName : null);
		StringBuilder stringBuilder9 = stringBuilder;
		string text10 = "{targetNationNameWithArticle}";
		TINationState ref_nation2 = target.ref_nation;
		stringBuilder9.Replace(text10, (ref_nation2 != null) ? ref_nation2.displayNameWithArticle : null);
		StringBuilder stringBuilder10 = stringBuilder;
		string text11 = "{targetNationNameWithArticleCap}";
		TINationState ref_nation3 = target.ref_nation;
		stringBuilder10.Replace(text11, (ref_nation3 != null) ? ref_nation3.displayNameWithArticleCapitalized : null);
		StringBuilder stringBuilder11 = stringBuilder;
		string text12 = "{targetNationNameWithPrep}";
		TINationState ref_nation4 = target.ref_nation;
		stringBuilder11.Replace(text12, (ref_nation4 != null) ? ref_nation4.displayNameWithArticleAndPlacePrep : null);
		StringBuilder stringBuilder12 = stringBuilder;
		string text13 = "{targetNationNameWithArticleAndPlacePrep}";
		TINationState ref_nation5 = target.ref_nation;
		stringBuilder12.Replace(text13, (ref_nation5 != null) ? ref_nation5.displayNameWithArticleAndPlacePrep : null);
		StringBuilder stringBuilder13 = stringBuilder;
		string text14 = "{targetNationAdjective}";
		TINationState ref_nation6 = target.ref_nation;
		stringBuilder13.Replace(text14, (ref_nation6 != null) ? ref_nation6.nationalAdjective : null);
		StringBuilder stringBuilder14 = stringBuilder;
		string text15 = "{targetRegionName}";
		TIRegionState ref_region = target.ref_region;
		stringBuilder14.Replace(text15, (ref_region != null) ? ref_region.displayName : null);
		StringBuilder stringBuilder15 = stringBuilder;
		string text16 = "{targetRegionNameSentIn}";
		TIRegionState ref_region2 = target.ref_region;
		stringBuilder15.Replace(text16, (ref_region2 != null) ? ref_region2.displayNameSentIn : null);
		StringBuilder stringBuilder16 = stringBuilder;
		string text17 = "{targetRegionNameSentOf}";
		TIRegionState ref_region3 = target.ref_region;
		stringBuilder16.Replace(text17, (ref_region3 != null) ? ref_region3.displayNameSentOf : null);
		StringBuilder stringBuilder17 = stringBuilder;
		string text18 = "{targetRegionBoostFacilityName}";
		TIRegionState ref_region4 = target.ref_region;
		string text19;
		if (ref_region4 == null)
		{
			text19 = null;
		}
		else
		{
			TIRegionSpaceFacilityState regionSpaceFacility = ref_region4.GetRegionSpaceFacility(SpaceFacilityType.launchFacility);
			text19 = ((regionSpaceFacility != null) ? regionSpaceFacility.displayName : null);
		}
		stringBuilder17.Replace(text18, text19);
		StringBuilder stringBuilder18 = stringBuilder;
		string text20 = "{secondaryFactionName}";
		string text21;
		if (secondaryTarget == null)
		{
			text21 = null;
		}
		else
		{
			TIFactionState ref_faction7 = secondaryTarget.ref_faction;
			text21 = ((ref_faction7 != null) ? ref_faction7.displayNameWithColor : null);
		}
		stringBuilder18.Replace(text20, text21);
		StringBuilder stringBuilder19 = stringBuilder;
		string text22 = "{secondaryFactionNameWithArticleCap}";
		string text23;
		if (secondaryTarget == null)
		{
			text23 = null;
		}
		else
		{
			TIFactionState ref_faction8 = secondaryTarget.ref_faction;
			text23 = ((ref_faction8 != null) ? ref_faction8.displayNameCapitalizedWithColor : null);
		}
		stringBuilder19.Replace(text22, text23);
		StringBuilder stringBuilder20 = stringBuilder;
		string text24 = "{secondaryTargetNationName}";
		string text25;
		if (secondaryTarget == null)
		{
			text25 = null;
		}
		else
		{
			TINationState ref_nation7 = secondaryTarget.ref_nation;
			text25 = ((ref_nation7 != null) ? ref_nation7.displayName : null);
		}
		stringBuilder20.Replace(text24, text25);
		StringBuilder stringBuilder21 = stringBuilder;
		string text26 = "{secondaryTargetNationNameWithArticle}";
		string text27;
		if (secondaryTarget == null)
		{
			text27 = null;
		}
		else
		{
			TINationState ref_nation8 = secondaryTarget.ref_nation;
			text27 = ((ref_nation8 != null) ? ref_nation8.displayNameWithArticle : null);
		}
		stringBuilder21.Replace(text26, text27);
		StringBuilder stringBuilder22 = stringBuilder;
		string text28 = "{secondaryTargetNationNameWithArticleCap}";
		string text29;
		if (secondaryTarget == null)
		{
			text29 = null;
		}
		else
		{
			TINationState ref_nation9 = secondaryTarget.ref_nation;
			text29 = ((ref_nation9 != null) ? ref_nation9.displayNameWithArticleCapitalized : null);
		}
		stringBuilder22.Replace(text28, text29);
		StringBuilder stringBuilder23 = stringBuilder;
		string text30 = "{secondaryTargetNationNameWithArticleAndPlacePrep}";
		string text31;
		if (secondaryTarget == null)
		{
			text31 = null;
		}
		else
		{
			TINationState ref_nation10 = secondaryTarget.ref_nation;
			text31 = ((ref_nation10 != null) ? ref_nation10.displayNameWithArticleAndPlacePrep : null);
		}
		stringBuilder23.Replace(text30, text31);
		StringBuilder stringBuilder24 = stringBuilder;
		string text32 = "{secondaryTargetNationAdjective}";
		string text33;
		if (secondaryTarget == null)
		{
			text33 = null;
		}
		else
		{
			TINationState ref_nation11 = secondaryTarget.ref_nation;
			text33 = ((ref_nation11 != null) ? ref_nation11.nationalAdjective : null);
		}
		stringBuilder24.Replace(text32, text33);
		StringBuilder stringBuilder25 = stringBuilder;
		string text34 = "{secondaryTargetRegionName}";
		string text35;
		if (secondaryTarget == null)
		{
			text35 = null;
		}
		else
		{
			TIRegionState ref_region5 = secondaryTarget.ref_region;
			text35 = ((ref_region5 != null) ? ref_region5.displayName : null);
		}
		stringBuilder25.Replace(text34, text35);
		StringBuilder stringBuilder26 = stringBuilder;
		string text36 = "{targetHabName}";
		TIHabState ref_hab = target.ref_hab;
		stringBuilder26.Replace(text36, (ref_hab != null) ? ref_hab.displayName : null);
		StringBuilder stringBuilder27 = stringBuilder;
		string text37 = "{secondaryTargetHabName}";
		string text38;
		if (secondaryTarget == null)
		{
			text38 = null;
		}
		else
		{
			TIHabState ref_hab2 = secondaryTarget.ref_hab;
			text38 = ((ref_hab2 != null) ? ref_hab2.displayName : null);
		}
		stringBuilder27.Replace(text37, text38);
		StringBuilder stringBuilder28 = stringBuilder;
		string text39 = "{targetHabSiteName}";
		TIHabSiteState ref_habSite = target.ref_habSite;
		stringBuilder28.Replace(text39, (ref_habSite != null) ? ref_habSite.displayName : null);
		StringBuilder stringBuilder29 = stringBuilder;
		string text40 = "{targetNaturalSpaceObjectName}";
		TINaturalSpaceObjectState ref_naturalSpaceObject = target.ref_naturalSpaceObject;
		stringBuilder29.Replace(text40, (ref_naturalSpaceObject != null) ? ref_naturalSpaceObject.displayName : null);
		StringBuilder stringBuilder30 = stringBuilder;
		string text41 = "{secondaryTargetNaturalSpaceObjectName}";
		string text42;
		if (secondaryTarget == null)
		{
			text42 = null;
		}
		else
		{
			TINaturalSpaceObjectState ref_naturalSpaceObject2 = secondaryTarget.ref_naturalSpaceObject;
			text42 = ((ref_naturalSpaceObject2 != null) ? ref_naturalSpaceObject2.displayName : null);
		}
		stringBuilder30.Replace(text41, text42);
		StringBuilder stringBuilder31 = stringBuilder;
		string text43 = "{targetSunOrbitingRelatedObjectName}";
		TINaturalSpaceObjectState ref_naturalSpaceObject3 = target.ref_naturalSpaceObject;
		string text44;
		if (ref_naturalSpaceObject3 == null)
		{
			text44 = null;
		}
		else
		{
			TISpaceObjectState getSunOrbitingRelatedObject = ref_naturalSpaceObject3.GetSunOrbitingRelatedObject;
			text44 = ((getSunOrbitingRelatedObject != null) ? getSunOrbitingRelatedObject.displayName : null);
		}
		stringBuilder31.Replace(text43, text44);
		StringBuilder stringBuilder32 = stringBuilder;
		string text45 = "{targetOrbitName}";
		TIOrbitState ref_orbit = target.ref_orbit;
		stringBuilder32.Replace(text45, (ref_orbit != null) ? ref_orbit.displayName : null);
		StringBuilder stringBuilder33 = stringBuilder;
		string text46 = "{targetArmyName}";
		TIArmyState ref_army = target.ref_army;
		stringBuilder33.Replace(text46, (ref_army != null) ? ref_army.displayName : null);
		StringBuilder stringBuilder34 = stringBuilder;
		string text47 = "{targetArmyNameWithArticle}";
		TIArmyState ref_army2 = target.ref_army;
		stringBuilder34.Replace(text47, (ref_army2 != null) ? ref_army2.displayNameWithArticle : null);
		StringBuilder stringBuilder35 = stringBuilder;
		string text48 = "{targetArmyNameWithArticleCap}";
		TIArmyState ref_army3 = target.ref_army;
		stringBuilder35.Replace(text48, (ref_army3 != null) ? ref_army3.displayNameWithArticleCapitalized : null);
		StringBuilder stringBuilder36 = stringBuilder;
		string text49 = "{targetCouncilorFullName}";
		TICouncilorState ref_councilor = target.ref_councilor;
		stringBuilder36.Replace(text49, (ref_councilor != null) ? ref_councilor.displayName : null);
		StringBuilder stringBuilder37 = stringBuilder;
		string text50 = "{targetCouncilorPersonalName}";
		TICouncilorState ref_councilor2 = target.ref_councilor;
		stringBuilder37.Replace(text50, (ref_councilor2 != null) ? ref_councilor2.personalName : null);
		StringBuilder stringBuilder38 = stringBuilder;
		string text51 = "{targetCouncilorFamilyName}";
		TICouncilorState ref_councilor3 = target.ref_councilor;
		stringBuilder38.Replace(text51, (ref_councilor3 != null) ? ref_councilor3.familyName : null);
		StringBuilder stringBuilder39 = stringBuilder;
		string text52 = "{targetCouncilorSubjectivePronoun}";
		TICouncilorState ref_councilor4 = target.ref_councilor;
		stringBuilder39.Replace(text52, (ref_councilor4 != null) ? ref_councilor4.subjectivePronoun(false) : null);
		StringBuilder stringBuilder40 = stringBuilder;
		string text53 = "{targetCouncilorSubjectivePronounCap}";
		TICouncilorState ref_councilor5 = target.ref_councilor;
		stringBuilder40.Replace(text53, Utilities.Capitalize((ref_councilor5 != null) ? ref_councilor5.subjectivePronoun(true) : null));
		StringBuilder stringBuilder41 = stringBuilder;
		string text54 = "{targetCouncilorObjectivePronoun}";
		TICouncilorState ref_councilor6 = target.ref_councilor;
		stringBuilder41.Replace(text54, (ref_councilor6 != null) ? ref_councilor6.objectivePronoun(false) : null);
		StringBuilder stringBuilder42 = stringBuilder;
		string text55 = "{targetCouncilorObjectivePronounCap}";
		TICouncilorState ref_councilor7 = target.ref_councilor;
		stringBuilder42.Replace(text55, Utilities.Capitalize((ref_councilor7 != null) ? ref_councilor7.objectivePronoun(true) : null));
		StringBuilder stringBuilder43 = stringBuilder;
		string text56 = "{targetCouncilorPossessivePronoun}";
		TICouncilorState ref_councilor8 = target.ref_councilor;
		stringBuilder43.Replace(text56, (ref_councilor8 != null) ? ref_councilor8.possessivePronoun(false) : null);
		StringBuilder stringBuilder44 = stringBuilder;
		string text57 = "{targetCouncilorPossessivePronounCap}";
		TICouncilorState ref_councilor9 = target.ref_councilor;
		stringBuilder44.Replace(text57, Utilities.Capitalize((ref_councilor9 != null) ? ref_councilor9.possessivePronoun(true) : null));
		StringBuilder stringBuilder45 = stringBuilder;
		string text58 = "{targetCouncilorHomeNationWithArticle}";
		TICouncilorState ref_councilor10 = target.ref_councilor;
		stringBuilder45.Replace(text58, (ref_councilor10 != null) ? ref_councilor10.homeNation.displayNameWithArticle : null);
		StringBuilder stringBuilder46 = stringBuilder;
		string text59 = "{targetCouncilorHomeNationWithArticleCap}";
		TICouncilorState ref_councilor11 = target.ref_councilor;
		stringBuilder46.Replace(text59, (ref_councilor11 != null) ? ref_councilor11.homeNation.displayNameWithArticleCapitalized : null);
		StringBuilder stringBuilder47 = stringBuilder;
		string text60 = "{targetCouncilorHomeRegionName}";
		TICouncilorState ref_councilor12 = target.ref_councilor;
		stringBuilder47.Replace(text60, (ref_councilor12 != null) ? ref_councilor12.homeRegion.displayName : null);
		StringBuilder stringBuilder48 = stringBuilder;
		string text61 = "{secondaryTargetCouncilorFullName}";
		string text62;
		if (secondaryTarget == null)
		{
			text62 = null;
		}
		else
		{
			TICouncilorState ref_councilor13 = secondaryTarget.ref_councilor;
			text62 = ((ref_councilor13 != null) ? ref_councilor13.displayName : null);
		}
		stringBuilder48.Replace(text61, text62);
		StringBuilder stringBuilder49 = stringBuilder;
		string text63 = "{secondaryTargetCouncilorPersonalName}";
		string text64;
		if (secondaryTarget == null)
		{
			text64 = null;
		}
		else
		{
			TICouncilorState ref_councilor14 = secondaryTarget.ref_councilor;
			text64 = ((ref_councilor14 != null) ? ref_councilor14.personalName : null);
		}
		stringBuilder49.Replace(text63, text64);
		StringBuilder stringBuilder50 = stringBuilder;
		string text65 = "{secondaryTargetCouncilorFamilyName}";
		string text66;
		if (secondaryTarget == null)
		{
			text66 = null;
		}
		else
		{
			TICouncilorState ref_councilor15 = secondaryTarget.ref_councilor;
			text66 = ((ref_councilor15 != null) ? ref_councilor15.familyName : null);
		}
		stringBuilder50.Replace(text65, text66);
		StringBuilder stringBuilder51 = stringBuilder;
		string text67 = "{secondaryTargetCouncilorPossessivePronoun}";
		string text68;
		if (secondaryTarget == null)
		{
			text68 = null;
		}
		else
		{
			TICouncilorState ref_councilor16 = secondaryTarget.ref_councilor;
			text68 = ((ref_councilor16 != null) ? ref_councilor16.possessivePronoun(false) : null);
		}
		stringBuilder51.Replace(text67, text68);
		StringBuilder stringBuilder52 = stringBuilder;
		string text69 = "{secondaryTargetCouncilorHomeNationWithArticle}";
		string text70;
		if (secondaryTarget == null)
		{
			text70 = null;
		}
		else
		{
			TICouncilorState ref_councilor17 = secondaryTarget.ref_councilor;
			text70 = ((ref_councilor17 != null) ? ref_councilor17.homeNation.displayNameWithArticle : null);
		}
		stringBuilder52.Replace(text69, text70);
		StringBuilder stringBuilder53 = stringBuilder;
		string text71 = "{secondaryTargetCouncilorHomeNationWithArticleCap}";
		string text72;
		if (secondaryTarget == null)
		{
			text72 = null;
		}
		else
		{
			TICouncilorState ref_councilor18 = secondaryTarget.ref_councilor;
			text72 = ((ref_councilor18 != null) ? ref_councilor18.homeNation.displayNameWithArticleCapitalized : null);
		}
		stringBuilder53.Replace(text71, text72);
		StringBuilder stringBuilder54 = stringBuilder;
		string text73 = "{secondaryTargetCouncilorHomeRegionName}";
		string text74;
		if (secondaryTarget == null)
		{
			text74 = null;
		}
		else
		{
			TICouncilorState ref_councilor19 = secondaryTarget.ref_councilor;
			text74 = ((ref_councilor19 != null) ? ref_councilor19.homeRegion.displayName : null);
		}
		stringBuilder54.Replace(text73, text74);
		StringBuilder stringBuilder55 = stringBuilder;
		string text75 = "{targetSpaceShipName}";
		TISpaceShipState ref_ship = target.ref_ship;
		stringBuilder55.Replace(text75, (ref_ship != null) ? ref_ship.displayName : null);
		StringBuilder stringBuilder56 = stringBuilder;
		string text76 = "{secondaryTargetSpaceShipName}";
		string text77;
		if (secondaryTarget == null)
		{
			text77 = null;
		}
		else
		{
			TISpaceShipState ref_ship2 = secondaryTarget.ref_ship;
			text77 = ((ref_ship2 != null) ? ref_ship2.displayName : null);
		}
		stringBuilder56.Replace(text76, text77);
		StringBuilder stringBuilder57 = stringBuilder;
		string text78 = "{targetSpaceFleetName}";
		TISpaceFleetState ref_fleet = target.ref_fleet;
		stringBuilder57.Replace(text78, (ref_fleet != null) ? ref_fleet.GetDisplayName(GameControl.control.activePlayer) : null);
		StringBuilder stringBuilder58 = stringBuilder;
		string text79 = "{secondaryTargetSpaceFleetName}";
		string text80;
		if (secondaryTarget == null)
		{
			text80 = null;
		}
		else
		{
			TISpaceFleetState ref_fleet2 = secondaryTarget.ref_fleet;
			text80 = ((ref_fleet2 != null) ? ref_fleet2.GetDisplayName(GameControl.control.activePlayer) : null);
		}
		stringBuilder58.Replace(text79, text80);
		StringBuilder stringBuilder59 = stringBuilder;
		string text81 = "{targetOfficerFullName}";
		TIOfficerState ref_officer = target.ref_officer;
		stringBuilder59.Replace(text81, (ref_officer != null) ? ref_officer.officerName : null);
		StringBuilder stringBuilder60 = stringBuilder;
		string text82 = "{targetOfficerFullNameWithRank}";
		TIOfficerState ref_officer2 = target.ref_officer;
		stringBuilder60.Replace(text82, (ref_officer2 != null) ? ref_officer2.displayName : null);
		StringBuilder stringBuilder61 = stringBuilder;
		string text83 = "{targetOfficerFullNameWithRankAndJob}";
		TIOfficerState ref_officer3 = target.ref_officer;
		stringBuilder61.Replace(text83, (ref_officer3 != null) ? ref_officer3.DisplayNameAndJob : null);
		StringBuilder stringBuilder62 = stringBuilder;
		string text84 = "{targetOfficerRank}";
		TIOfficerState ref_officer4 = target.ref_officer;
		stringBuilder62.Replace(text84, (ref_officer4 != null) ? ref_officer4.template.GetRankString(target.ref_officer.maxRank) : null);
		StringBuilder stringBuilder63 = stringBuilder;
		string text85 = "{targetOfficerNextRank}";
		TIOfficerState ref_officer5 = target.ref_officer;
		stringBuilder63.Replace(text85, (ref_officer5 != null) ? ref_officer5.template.GetRankString(target.ref_officer.maxRank + 1) : null);
		StringBuilder stringBuilder64 = stringBuilder;
		string text86 = "{targetOfficerJob}";
		TIOfficerState ref_officer6 = target.ref_officer;
		stringBuilder64.Replace(text86, (ref_officer6 != null) ? ref_officer6.template.displayName : null);
		StringBuilder stringBuilder65 = stringBuilder;
		string text87 = "{secondaryTargetOfficerFullName}";
		string text88;
		if (secondaryTarget == null)
		{
			text88 = null;
		}
		else
		{
			TIOfficerState ref_officer7 = secondaryTarget.ref_officer;
			text88 = ((ref_officer7 != null) ? ref_officer7.officerName : null);
		}
		stringBuilder65.Replace(text87, text88);
		StringBuilder stringBuilder66 = stringBuilder;
		string text89 = "{secondaryTargetOfficerFullNameWithRank}";
		string text90;
		if (secondaryTarget == null)
		{
			text90 = null;
		}
		else
		{
			TIOfficerState ref_officer8 = secondaryTarget.ref_officer;
			text90 = ((ref_officer8 != null) ? ref_officer8.displayName : null);
		}
		stringBuilder66.Replace(text89, text90);
		StringBuilder stringBuilder67 = stringBuilder;
		string text91 = "{secondaryTargetOfficerFullNameWithRankAndJob}";
		string text92;
		if (secondaryTarget == null)
		{
			text92 = null;
		}
		else
		{
			TIOfficerState ref_officer9 = secondaryTarget.ref_officer;
			text92 = ((ref_officer9 != null) ? ref_officer9.DisplayNameAndJob : null);
		}
		stringBuilder67.Replace(text91, text92);
		StringBuilder stringBuilder68 = stringBuilder;
		string text93 = "{secondaryTargetOfficerRank}";
		string text94;
		if (secondaryTarget == null)
		{
			text94 = null;
		}
		else
		{
			TIOfficerState ref_officer10 = secondaryTarget.ref_officer;
			text94 = ((ref_officer10 != null) ? ref_officer10.template.GetRankString(target.ref_officer.maxRank) : null);
		}
		stringBuilder68.Replace(text93, text94);
		StringBuilder stringBuilder69 = stringBuilder;
		string text95 = "{secondaryTargetOfficerNextRank}";
		string text96;
		if (secondaryTarget == null)
		{
			text96 = null;
		}
		else
		{
			TIOfficerState ref_officer11 = secondaryTarget.ref_officer;
			text96 = ((ref_officer11 != null) ? ref_officer11.template.GetRankString(target.ref_officer.maxRank + 1) : null);
		}
		stringBuilder69.Replace(text95, text96);
		StringBuilder stringBuilder70 = stringBuilder;
		string text97 = "{secondaryTargetOfficerJob}";
		string text98;
		if (secondaryTarget == null)
		{
			text98 = null;
		}
		else
		{
			TIOfficerState ref_officer12 = secondaryTarget.ref_officer;
			text98 = ((ref_officer12 != null) ? ref_officer12.template.displayName : null);
		}
		stringBuilder70.Replace(text97, text98);
		if (!isSummary)
		{
			stringBuilder.Replace("{summary}", this.summary(actingState, target, secondaryTarget));
		}
		return stringBuilder;
	}

	// Token: 0x0600095B RID: 2395 RVA: 0x0002EA7B File Offset: 0x0002CC7B
	public bool ReportOutcome(NarrativeEventOption selectedOption, NarrativeEventOutcome outcome, TIFactionState faction, TIGameState targetState, TIGameState secondaryState)
	{
		return outcome.forceAlert || (selectedOption.possibleOutcomes(faction, targetState, secondaryState).Count > 1 && outcome.ReportOutcome(targetState));
	}

	// Token: 0x040007B2 RID: 1970
	public string illustrationResource;

	// Token: 0x040007B3 RID: 1971
	public string soundResource;

	// Token: 0x040007B4 RID: 1972
	public bool requiresAliens;

	// Token: 0x040007B5 RID: 1973
	public int? year;

	// Token: 0x040007B6 RID: 1974
	public int? endYear;

	// Token: 0x040007B7 RID: 1975
	public int? earliestMonth;

	// Token: 0x040007B8 RID: 1976
	public int? latestMonth;

	// Token: 0x040007B9 RID: 1977
	public string reqTechDataName;

	// Token: 0x040007BA RID: 1978
	public bool reqEventUnlock;

	// Token: 0x040007BB RID: 1979
	public PublicityType logPublicity;

	// Token: 0x040007BC RID: 1980
	public PublicityType alertPublicity;

	// Token: 0x040007BD RID: 1981
	public RepeatableStatus repeatable;

	// Token: 0x040007BE RID: 1982
	public int numOptions;

	// Token: 0x040007BF RID: 1983
	public bool forceEvent;

	// Token: 0x040007C0 RID: 1984
	public float baseWeight;

	// Token: 0x040007C1 RID: 1985
	public float monthlyWeightDelta;

	// Token: 0x040007C2 RID: 1986
	public float altMonthlyWeightDelta;

	// Token: 0x040007C3 RID: 1987
	public float weightDeltaWhenTriggered;

	// Token: 0x040007C4 RID: 1988
	public NarrativeEventWeightModifier altBaseWeight;

	// Token: 0x040007C5 RID: 1989
	public int global_cooldown_months;

	// Token: 0x040007C6 RID: 1990
	public int target_cooldown_months;

	// Token: 0x040007C7 RID: 1991
	public NarrativeEventTargetType targetType;

	// Token: 0x040007C8 RID: 1992
	public bool hitAllQualifyingTargets;

	// Token: 0x040007C9 RID: 1993
	public bool firstTargetNotificationOnly;

	// Token: 0x040007CA RID: 1994
	public List<string> possibleTargetDataNames = new List<string>();

	// Token: 0x040007CB RID: 1995
	public List<TICondition> targetConditions;

	// Token: 0x040007CC RID: 1996
	public List<NarrativeEventWeightModifier> targetWeightModifiers = new List<NarrativeEventWeightModifier>();

	// Token: 0x040007CD RID: 1997
	public EffectSecondaryStateType secondaryStateType;

	// Token: 0x040007CE RID: 1998
	public List<TICondition> secondaryStateConditions;

	// Token: 0x040007CF RID: 1999
	public bool sameSecondaryForAllTargets;

	// Token: 0x040007D0 RID: 2000
	public List<string> possibleSecondaryStateDataNames = new List<string>();

	// Token: 0x040007D1 RID: 2001
	public List<NarrativeEventWeightModifier> secondaryWeightModifiers = new List<NarrativeEventWeightModifier>();

	// Token: 0x040007D2 RID: 2002
	public List<NarrativeEventOption> eventOptions = new List<NarrativeEventOption>();
}
