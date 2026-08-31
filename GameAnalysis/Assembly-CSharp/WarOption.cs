using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PavonisInteractive.TerraInvicta;

// Token: 0x020002B5 RID: 693
public class WarOption : TIPolicyOption
{
	// Token: 0x060009AF RID: 2479 RVA: 0x00030495 File Offset: 0x0002E695
	public override PolicyType GetPolicyType()
	{
		return PolicyType.WarOption;
	}

	// Token: 0x060009B0 RID: 2480 RVA: 0x00030498 File Offset: 0x0002E698
	public override bool DegradesRelations()
	{
		return true;
	}

	// Token: 0x060009B1 RID: 2481 RVA: 0x0003049B File Offset: 0x0002E69B
	public override bool Allowed(TINationState nationState)
	{
		return nationState.WarCapable && this.GetPossibleTargets(nationState).Count > 0;
	}

	// Token: 0x060009B2 RID: 2482 RVA: 0x000304B8 File Offset: 0x0002E6B8
	public override IList<TIGameState> GetPossibleTargets(TINationState policyNation)
	{
		List<TIGameState> list = new List<TIGameState>(policyNation.ValidNewWarTargets().ConvertAll<TIGameState>((TINationState x) => x));
		foreach (TIWarState tiwarState in GameStateManager.GlobalValues().interstateWars)
		{
			if (policyNation.CanJoinExistingWarAsAttacker(tiwarState))
			{
				list.Add(tiwarState);
			}
			else if (policyNation.CanJoinExistingWarAsDefender(tiwarState))
			{
				list.Add(tiwarState);
			}
		}
		return list;
	}

	// Token: 0x060009B3 RID: 2483 RVA: 0x0003055C File Offset: 0x0002E75C
	public override void OnPassage(TINationState enactingNation, TIGameState policyTarget)
	{
		if (policyTarget.isNationState)
		{
			enactingNation.DeclareFullWar(enactingNation.executiveFaction, policyTarget.ref_nation);
			if (enactingNation.executiveFaction != null && enactingNation.executiveFaction.isActivePlayer)
			{
				enactingNation.executiveFaction.UnlockAchievement("declareWar");
				return;
			}
		}
		else if (policyTarget.isWarState)
		{
			TIWarState ref_war = policyTarget.ref_war;
			if (enactingNation.CanJoinExistingWarAsAttacker(ref_war))
			{
				enactingNation.JoinWar(enactingNation.executiveFaction, ref_war.attacker, ref_war);
				return;
			}
			if (enactingNation.CanJoinExistingWarAsDefender(ref_war))
			{
				enactingNation.JoinWar(enactingNation.executiveFaction, ref_war.defender, ref_war);
			}
		}
	}

	// Token: 0x060009B4 RID: 2484 RVA: 0x000305F8 File Offset: 0x0002E7F8
	public override void EnactPolicy(TINationState enactingNation, TIGameState policyTarget)
	{
		this.OnPassage(enactingNation, policyTarget);
		GameControl.eventManager.TriggerEvent(new NationRelationsChange(enactingNation), null, new object[] { enactingNation, policyTarget }.ToArray<object>());
		TINotificationQueueState.LogPolicyAdopted(this, enactingNation, policyTarget, null, this.Importance(enactingNation, policyTarget), "", "");
	}

	// Token: 0x060009B5 RID: 2485 RVA: 0x0003064C File Offset: 0x0002E84C
	public override string GetConfirmPrompt(TINationState enactingNation, TIGameState target)
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (target.isNationState)
		{
			TINationState ref_nation = target.ref_nation;
			List<TIGameState> list = new List<TIGameState>();
			list.Add(ref_nation);
			list.AddRange(ref_nation.WarCapableAllies);
			string text = TIUtilities.ConstructTextList(list, false, false);
			stringBuilder.Append(Loc.T(new StringBuilder(base.GetType().Name).Append(".confirmText").ToString(), new object[] { text }));
			List<TINationState> list2 = enactingNation.ProspectiveOffensiveAlliance(ref_nation, false);
			if (list2.Count > 0)
			{
				string text2 = TIUtilities.ConstructTextList(list2.ConvertAll<TIGameState>((TINationState x) => x), false, false);
				list2.Add(enactingNation);
				TINationState tinationState = list2.OrderByDescending<TINationState, float>((TINationState x) => x.militaryStrength).First<TINationState>();
				stringBuilder.Append(Loc.T("WarOption.confirmTextAlliance", new object[] { text2, tinationState.displayNameWithArticle }));
			}
			float num = enactingNation.CohesionLossFromDeclaringWar(ref_nation);
			if (num > 0f)
			{
				stringBuilder.Append(Loc.T("WarOption.cohesionEffect", new object[] { num.ToString("N2") }));
			}
		}
		else
		{
			TIWarState ref_war = target.ref_war;
			TINationState tinationState2;
			if (ref_war.attackingAlliance.SelectMany<TINationState, TINationState>((TINationState x) => x.allies).Contains(enactingNation))
			{
				tinationState2 = ref_war.attacker;
			}
			else
			{
				tinationState2 = ref_war.defender;
			}
			stringBuilder.Append(Loc.T("WarOption.joinWarConfirmText", new object[] { tinationState2.displayNameWithArticle, ref_war.displayNameWithArticle }));
		}
		return stringBuilder.ToString();
	}

	// Token: 0x060009B6 RID: 2486 RVA: 0x0003081F File Offset: 0x0002EA1F
	public override int Importance(TINationState policyNation, TIGameState target)
	{
		return 2;
	}
}
