using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Tasks;
using UnityEngine;

// Token: 0x020002C5 RID: 709
public class EndWarOption : TIPolicyOptionWithConfirm
{
	// Token: 0x06000A40 RID: 2624 RVA: 0x00031696 File Offset: 0x0002F896
	public override PolicyType GetPolicyType()
	{
		return PolicyType.EndWarOption;
	}

	// Token: 0x06000A41 RID: 2625 RVA: 0x00031699 File Offset: 0x0002F899
	public override bool ImprovesRelations()
	{
		return true;
	}

	// Token: 0x17000157 RID: 343
	// (get) Token: 0x06000A42 RID: 2626 RVA: 0x0003169C File Offset: 0x0002F89C
	public override string PromptName
	{
		get
		{
			return "PromptRespondToEndWarCall";
		}
	}

	// Token: 0x17000158 RID: 344
	// (get) Token: 0x06000A43 RID: 2627 RVA: 0x000316A3 File Offset: 0x0002F8A3
	public override bool EnactAgainstRelatedState
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06000A44 RID: 2628 RVA: 0x000316A8 File Offset: 0x0002F8A8
	public override float AIAgreeChance_Prospective(TINationState proposingNation, TIGameState respondingState)
	{
		float num = base.AIAgreeChance_Prospective(proposingNation, respondingState);
		int num2 = 90;
		TIDateTime timeoutDate = TITimeState.Now();
		timeoutDate.AddDays((float)(-(float)num2));
		int num3 = (from x in respondingState.ref_war.GetPeaceOffers(proposingNation)
			where x >= timeoutDate
			select x).Count<TIDateTime>();
		float num4 = Mathf.Pow(0.1f, (float)num3);
		return num * num4;
	}

	// Token: 0x06000A45 RID: 2629 RVA: 0x00031711 File Offset: 0x0002F911
	public override void OnConfirm(TINationState enactingNation, TIGameState policyTarget)
	{
		(policyTarget as TIWarState).LogPeaceOffer(enactingNation);
		base.OnConfirm(enactingNation, policyTarget);
	}

	// Token: 0x06000A46 RID: 2630 RVA: 0x00031728 File Offset: 0x0002F928
	public override void EnactPolicy(TINationState enactingNation, TIGameState policyTarget)
	{
		TINationState tinationState = policyTarget.ref_war.EnemyWarLeader(enactingNation, false);
		TINotificationQueueState.LogPolicyAdopted(this, enactingNation, tinationState, policyTarget, this.Importance(enactingNation, policyTarget), "", "");
		this.OnPassage(enactingNation, policyTarget);
	}

	// Token: 0x06000A47 RID: 2631 RVA: 0x00031766 File Offset: 0x0002F966
	public override bool Allowed(TINationState nation)
	{
		return nation.atWar;
	}

	// Token: 0x06000A48 RID: 2632 RVA: 0x0003176E File Offset: 0x0002F96E
	public override IList<TIGameState> GetPossibleTargets(TINationState policyTarget)
	{
		return policyTarget.currentWarStates.ConvertAll<TIGameState>((TIWarState x) => x);
	}

	// Token: 0x06000A49 RID: 2633 RVA: 0x0003179A File Offset: 0x0002F99A
	public override void OnPassage(TINationState enactingNation, TIGameState policyTarget)
	{
		enactingNation.WhitePeace(enactingNation.executiveFaction, policyTarget.ref_war, true);
	}

	// Token: 0x06000A4A RID: 2634 RVA: 0x000317AF File Offset: 0x0002F9AF
	public override void PromptPolicyResponse(TINationState enactingNation, TIGameState policyTarget)
	{
		TIPromptQueueState.AddPromptStatic(policyTarget.ref_war.EnemyWarLeader(enactingNation, false), enactingNation, policyTarget, this.PromptName, 0);
	}

	// Token: 0x06000A4B RID: 2635 RVA: 0x000317CC File Offset: 0x0002F9CC
	public override string GetConfirmPrompt(TINationState enactingNation, TIGameState target)
	{
		TIWarState ref_war = target.ref_war;
		string text = TIUtilities.ConstructTextList(new List<TINationState>(ref_war.EnemyAlliance(enactingNation)).ConvertAll<TIGameState>((TINationState x) => x), false, false);
		float num = enactingNation.CohesionLossFromWhitePeace(ref_war);
		StringBuilder stringBuilder = new StringBuilder();
		if (ref_war.AllianceWarLeader(enactingNation) == enactingNation)
		{
			stringBuilder.AppendLine(Loc.T(new StringBuilder(base.GetType().Name).Append(".confirmTextLeader").ToString(), new object[] { text, ref_war.displayName }));
		}
		else
		{
			stringBuilder.AppendLine(Loc.T(new StringBuilder(base.GetType().Name).Append(".confirmTextSeparatePeace").ToString(), new object[] { text, ref_war.displayName }));
		}
		if (num != 0f)
		{
			stringBuilder.AppendLine().AppendLine(Loc.T(new StringBuilder(base.GetType().Name).Append(".cohesionLoss").ToString(), new object[]
			{
				enactingNation.displayNameWithArticleCapitalized,
				(-num).ToString("N2")
			}));
		}
		return stringBuilder.ToString();
	}

	// Token: 0x06000A4C RID: 2636 RVA: 0x00031914 File Offset: 0x0002FB14
	public override string GetResponsePrompt(TINationState policyNation, TINationState respondingNation, TIGameState policyTarget)
	{
		TIWarState ref_war = policyTarget.ref_war;
		string text = TIUtilities.ConstructTextList(new List<TINationState>(ref_war.EnemyAlliance(policyNation)).ConvertAll<TIGameState>((TINationState x) => x), false, false);
		float num = respondingNation.CohesionLossFromWhitePeace(ref_war);
		StringBuilder stringBuilder = new StringBuilder();
		if (ref_war.AllianceWarLeader(policyNation) == policyNation)
		{
			stringBuilder.AppendLine(Loc.T(new StringBuilder(base.GetType().Name).Append(".responsePromptLeader").ToString(), new object[] { policyNation.displayNameWithArticleCapitalized, text, ref_war.displayName }));
		}
		else
		{
			stringBuilder.AppendLine(Loc.T(new StringBuilder(base.GetType().Name).Append(".responsePromptSeparatePeace").ToString(), new object[] { policyNation.displayNameWithArticleCapitalized, text, ref_war.displayName }));
		}
		if (num != 0f)
		{
			stringBuilder.AppendLine().AppendLine(Loc.T(new StringBuilder(base.GetType().Name).Append(".cohesionLoss").ToString(), new object[]
			{
				respondingNation.displayNameWithArticleCapitalized,
				(-num).ToString("N2")
			}));
		}
		return stringBuilder.ToString();
	}

	// Token: 0x06000A4D RID: 2637 RVA: 0x00031A6D File Offset: 0x0002FC6D
	public override int Importance(TINationState policyNation, TIGameState target)
	{
		return 2;
	}

	// Token: 0x06000A4E RID: 2638 RVA: 0x00031A70 File Offset: 0x0002FC70
	public override float AIAgreeChance(TINationState proposingNation, TIGameState war)
	{
		return StratPolicyResponseSelector.ChanceEndWar(proposingNation, war.ref_war);
	}
}
