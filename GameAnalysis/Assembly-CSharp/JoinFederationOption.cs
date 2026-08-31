using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Tasks;

// Token: 0x020002B7 RID: 695
public class JoinFederationOption : TIPolicyOptionWithConfirm
{
	// Token: 0x060009C0 RID: 2496 RVA: 0x0003088F File Offset: 0x0002EA8F
	public override PolicyType GetPolicyType()
	{
		return PolicyType.JoinFederationOption;
	}

	// Token: 0x1700014B RID: 331
	// (get) Token: 0x060009C1 RID: 2497 RVA: 0x00030892 File Offset: 0x0002EA92
	public override string PromptName
	{
		get
		{
			return "PromptRespondToJoinFederationCall";
		}
	}

	// Token: 0x060009C2 RID: 2498 RVA: 0x00030899 File Offset: 0x0002EA99
	public override bool ImprovesRelations()
	{
		return true;
	}

	// Token: 0x060009C3 RID: 2499 RVA: 0x0003089C File Offset: 0x0002EA9C
	public override bool Allowed(TINationState nationState)
	{
		return nationState.extant && (!nationState.inFederation || nationState.federation.leadNation == nationState) && !nationState.breakaway && this.GetPossibleTargets(nationState).Count > 0;
	}

	// Token: 0x060009C4 RID: 2500 RVA: 0x000308DC File Offset: 0x0002EADC
	public override IList<TIGameState> GetPossibleTargets(TINationState policyNation)
	{
		if (!policyNation.inFederation)
		{
			List<TIGameState> list = (from x in GameStateManager.IterateByClass<TIFederationState>(false)
				where x.CanAddNation(policyNation)
				select x).ToList<TIFederationState>().ConvertAll<TIGameState>((TIFederationState x) => x);
			foreach (TINationState tinationState in from x in GameStateManager.AllExtantNations()
				where !x.inFederation
				select x)
			{
				if (policyNation.CanFormFederation(tinationState))
				{
					list.Add(tinationState);
				}
			}
			return list;
		}
		if (policyNation.federation.leadNation == policyNation)
		{
			return (from x in GameStateManager.AllExtantNations()
				where policyNation.federation.CanAddNation(x)
				select x).ToList<TINationState>().ConvertAll<TIGameState>((TINationState x) => x);
		}
		return new List<TIGameState>();
	}

	// Token: 0x060009C5 RID: 2501 RVA: 0x00030A1C File Offset: 0x0002EC1C
	public override void OnPassage(TINationState enactingNation, TIGameState policyTarget)
	{
		TINationState ref_nation = policyTarget.ref_nation;
		if (enactingNation.inFederation)
		{
			enactingNation.federation.AddNation(enactingNation.executiveFaction, ref_nation, false);
			return;
		}
		if (ref_nation.inFederation)
		{
			ref_nation.federation.AddNation(enactingNation.executiveFaction, enactingNation, false);
			return;
		}
		enactingNation.FormFederation(ref_nation);
	}

	// Token: 0x060009C6 RID: 2502 RVA: 0x00030A70 File Offset: 0x0002EC70
	public override string GetResponsePrompt(TINationState policyNation, TINationState respondingNation, TIGameState policyTarget)
	{
		string text;
		string text2;
		if (policyNation.inFederation)
		{
			text = policyNation.federation.displayNameWithArticleCapitalized;
			text2 = policyTarget.ref_nation.displayNameWithArticle;
		}
		else if (policyTarget.ref_nation.inFederation)
		{
			text = policyNation.displayNameWithArticleCapitalized;
			text2 = policyTarget.ref_nation.federation.displayNameWithArticle;
		}
		else
		{
			text = policyNation.displayNameWithArticleCapitalized;
			text2 = policyTarget.ref_nation.displayNameWithArticle;
		}
		return Loc.T(new StringBuilder(base.GetType().Name).Append(".responsePrompt").ToString(), new object[] { text, text2 });
	}

	// Token: 0x060009C7 RID: 2503 RVA: 0x00030B0B File Offset: 0x0002ED0B
	public override float AIAgreeChance(TINationState proposingNation, TIGameState respondingPolity)
	{
		return StratPolicyResponseSelector.ChanceFederation(proposingNation, respondingPolity.ref_nation);
	}
}
