using System;
using System.Collections.Generic;
using System.Text;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Tasks;

// Token: 0x020002C1 RID: 705
public class TransferRegionsOption : TIPolicyOptionWithConfirm
{
	// Token: 0x06000A14 RID: 2580 RVA: 0x000311B1 File Offset: 0x0002F3B1
	public override PolicyType GetPolicyType()
	{
		return PolicyType.TransferRegionsOption;
	}

	// Token: 0x17000150 RID: 336
	// (get) Token: 0x06000A15 RID: 2581 RVA: 0x000311B5 File Offset: 0x0002F3B5
	public override bool EnactAgainstRelatedState
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000151 RID: 337
	// (get) Token: 0x06000A16 RID: 2582 RVA: 0x000311B8 File Offset: 0x0002F3B8
	public override string PromptName
	{
		get
		{
			return "PromptRespondToTransferRegionCall";
		}
	}

	// Token: 0x06000A17 RID: 2583 RVA: 0x000311BF File Offset: 0x0002F3BF
	public override bool WeakensNation()
	{
		return false;
	}

	// Token: 0x06000A18 RID: 2584 RVA: 0x000311C2 File Offset: 0x0002F3C2
	public override bool ImprovesRelations()
	{
		return true;
	}

	// Token: 0x06000A19 RID: 2585 RVA: 0x000311C5 File Offset: 0x0002F3C5
	public override float AIAgreeChance(TINationState proposingNation, TIGameState targetedRegion)
	{
		return StratPolicyResponseSelector.ChanceSurrenderRegion(proposingNation, targetedRegion.ref_region);
	}

	// Token: 0x06000A1A RID: 2586 RVA: 0x000311D4 File Offset: 0x0002F3D4
	public override string GetResponsePrompt(TINationState policyNation, TINationState respondingNation, TIGameState policyTarget)
	{
		return Loc.T(new StringBuilder(base.GetType().Name).Append(".responsePrompt").ToString(), new object[]
		{
			policyNation.displayNameWithArticleCapitalized,
			policyTarget.displayName,
			policyTarget.ref_nation.displayName
		});
	}

	// Token: 0x06000A1B RID: 2587 RVA: 0x0003122B File Offset: 0x0002F42B
	public override void PromptPolicyResponse(TINationState enactingNation, TIGameState policyTarget)
	{
		if (enactingNation.executiveFaction != null && policyTarget.ref_faction == enactingNation.executiveFaction)
		{
			this.EnactPolicy(enactingNation, policyTarget);
			return;
		}
		TIPromptQueueState.AddPromptStatic(policyTarget.ref_nation, enactingNation, policyTarget, this.PromptName, 0);
	}

	// Token: 0x06000A1C RID: 2588 RVA: 0x0003126B File Offset: 0x0002F46B
	public override bool Allowed(TINationState nationState)
	{
		return nationState.ExecutivePowerConsolidated && this.GetPossibleTargets(nationState).Count > 0;
	}

	// Token: 0x06000A1D RID: 2589 RVA: 0x00031286 File Offset: 0x0002F486
	public override int Importance(TINationState policyNation, TIGameState target)
	{
		return 1;
	}

	// Token: 0x06000A1E RID: 2590 RVA: 0x00031289 File Offset: 0x0002F489
	public override string GetConfirmPrompt(TINationState enactingNation, TIGameState target)
	{
		return Loc.T(new StringBuilder(base.dataName).Append(".confirmText").ToString(), new object[]
		{
			target.displayName,
			target.ref_nation.displayName
		});
	}

	// Token: 0x06000A1F RID: 2591 RVA: 0x000312C8 File Offset: 0x0002F4C8
	public override IList<TIGameState> GetPossibleTargets(TINationState actingNation)
	{
		List<TIGameState> list = new List<TIGameState>();
		foreach (TIRegionState tiregionState in actingNation.ExternalClaims())
		{
			if (actingNation.CanImproveRelationsYet(tiregionState.nation) && tiregionState.nation.ExecutivePowerConsolidated && tiregionState != tiregionState.nation.capital && !tiregionState.nation.atWar && !actingNation.ClaimWillBeHostile(tiregionState, false) && (!actingNation.rivals.Contains(tiregionState.nation) || actingNation.CanEndRivalry(tiregionState.nation)))
			{
				if (actingNation.alienNation)
				{
					if (TIEffectsState.CheckForAnyEffectInContext(Context.CanTransferTerritoryToAliens, tiregionState.nation.executiveFaction))
					{
						list.Add(tiregionState);
					}
				}
				else
				{
					list.Add(tiregionState);
				}
			}
		}
		return list;
	}

	// Token: 0x06000A20 RID: 2592 RVA: 0x000313B8 File Offset: 0x0002F5B8
	public override void OnPassage(TINationState enactingNation, TIGameState policyTarget)
	{
		TINationState nation = policyTarget.ref_region.nation;
		nation.TransferRegionsControlTo(new List<TIRegionState> { policyTarget.ref_region }, enactingNation, false, true, false, true, false);
		if (enactingNation.IsRivalWith(nation) && enactingNation.CanEndRivalry(nation))
		{
			enactingNation.EndRivalry(enactingNation.executiveFaction, nation);
		}
		else
		{
			enactingNation.SetImproveRelationsCooldown(enactingNation.executiveFaction, nation, TemplateManager.global.improveRelationsCooldown_FormAlliance_d);
		}
		if (enactingNation.alienNation)
		{
			TINotificationQueueState.LogAlienNationGrows(enactingNation, policyTarget);
		}
	}

	// Token: 0x06000A21 RID: 2593 RVA: 0x00031435 File Offset: 0x0002F635
	public override void EnactPolicy(TINationState enactingNation, TIGameState policyTarget)
	{
		TINotificationQueueState.LogPolicyAdopted(this, enactingNation, policyTarget, policyTarget.ref_nation, this.Importance(enactingNation, policyTarget), "", "");
		this.OnPassage(enactingNation, policyTarget);
	}
}
