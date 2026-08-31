using System;
using System.Collections.Generic;
using System.Text;
using PavonisInteractive.TerraInvicta;

// Token: 0x020002B4 RID: 692
public abstract class TIPolicyOption : TIDataTemplate, IPolicyOption
{
	// Token: 0x06000995 RID: 2453
	public abstract PolicyType GetPolicyType();

	// Token: 0x06000996 RID: 2454 RVA: 0x000302F9 File Offset: 0x0002E4F9
	public string GetDisplayName()
	{
		return Loc.T(new StringBuilder(base.dataName).Append(".displayName").ToString());
	}

	// Token: 0x06000997 RID: 2455 RVA: 0x0003031A File Offset: 0x0002E51A
	public virtual string GetDescription()
	{
		return Loc.T(new StringBuilder(base.dataName).Append(".description").ToString());
	}

	// Token: 0x06000998 RID: 2456 RVA: 0x0003033B File Offset: 0x0002E53B
	public string GetTargetSelectionHeaderText()
	{
		return Loc.T(new StringBuilder(base.dataName).Append(".targetHeader").ToString());
	}

	// Token: 0x06000999 RID: 2457 RVA: 0x0003035C File Offset: 0x0002E55C
	public string templateName()
	{
		return base.dataName;
	}

	// Token: 0x0600099A RID: 2458 RVA: 0x00030364 File Offset: 0x0002E564
	public virtual string GetResponsePrompt(TINationState policyNation, TINationState respondingNation, TIGameState policyTarget)
	{
		return Loc.T(new StringBuilder(base.GetType().Name).Append(".responsePrompt").ToString(), new object[]
		{
			policyNation.displayNameWithArticleCapitalized,
			policyTarget.ref_nation.displayNameWithArticle
		});
	}

	// Token: 0x0600099B RID: 2459 RVA: 0x000303B2 File Offset: 0x0002E5B2
	public virtual string GetConfirmPrompt(TINationState enactingNation, TIGameState target)
	{
		return Loc.T(new StringBuilder(base.dataName).Append(".confirmText").ToString(), new object[] { target.displayName });
	}

	// Token: 0x0600099C RID: 2460
	public abstract IList<TIGameState> GetPossibleTargets(TINationState policyNation);

	// Token: 0x0600099D RID: 2461
	public abstract void OnPassage(TINationState enactingNation, TIGameState policyTarget);

	// Token: 0x0600099E RID: 2462 RVA: 0x000303E2 File Offset: 0x0002E5E2
	public virtual bool Allowed(TINationState nationState)
	{
		return true;
	}

	// Token: 0x0600099F RID: 2463 RVA: 0x000303E5 File Offset: 0x0002E5E5
	public virtual bool RequiresTargets()
	{
		return true;
	}

	// Token: 0x060009A0 RID: 2464 RVA: 0x000303E8 File Offset: 0x0002E5E8
	public virtual bool RequiresTargetConfirm()
	{
		return false;
	}

	// Token: 0x060009A1 RID: 2465 RVA: 0x000303EB File Offset: 0x0002E5EB
	public virtual bool DegradesRelations()
	{
		return false;
	}

	// Token: 0x060009A2 RID: 2466 RVA: 0x000303EE File Offset: 0x0002E5EE
	public virtual bool ImprovesRelations()
	{
		return false;
	}

	// Token: 0x060009A3 RID: 2467 RVA: 0x000303F1 File Offset: 0x0002E5F1
	public virtual bool WeakensNation()
	{
		return false;
	}

	// Token: 0x060009A4 RID: 2468 RVA: 0x000303F4 File Offset: 0x0002E5F4
	public virtual bool HasTooltip()
	{
		return false;
	}

	// Token: 0x060009A5 RID: 2469 RVA: 0x000303F7 File Offset: 0x0002E5F7
	public virtual string GetTooltipString()
	{
		return string.Empty;
	}

	// Token: 0x060009A6 RID: 2470 RVA: 0x000303FE File Offset: 0x0002E5FE
	public virtual bool HandledAtFactionLevel()
	{
		return false;
	}

	// Token: 0x17000147 RID: 327
	// (get) Token: 0x060009A7 RID: 2471 RVA: 0x00030401 File Offset: 0x0002E601
	public virtual bool EnactAgainstRelatedState
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000148 RID: 328
	// (get) Token: 0x060009A8 RID: 2472 RVA: 0x00030404 File Offset: 0x0002E604
	public virtual bool TargetsMyFederation
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000149 RID: 329
	// (get) Token: 0x060009A9 RID: 2473 RVA: 0x00030407 File Offset: 0x0002E607
	public virtual RelationChange relationChange
	{
		get
		{
			return RelationChange.None;
		}
	}

	// Token: 0x060009AA RID: 2474 RVA: 0x0003040A File Offset: 0x0002E60A
	public TIPolicyOption()
	{
		base.dataName = base.GetType().ToString();
		this._displayName = this.GetDisplayName();
		TemplateManager.Add(this, typeof(TIPolicyOption), false);
	}

	// Token: 0x060009AB RID: 2475 RVA: 0x00030440 File Offset: 0x0002E640
	public virtual void OnConfirm(TINationState enactingNation, TIGameState policyTarget)
	{
		this.EnactPolicy(enactingNation, policyTarget);
	}

	// Token: 0x060009AC RID: 2476 RVA: 0x0003044A File Offset: 0x0002E64A
	public virtual void EnactPolicy(TINationState enactingNation, TIGameState policyTarget)
	{
		this.OnPassage(enactingNation, policyTarget);
		TINotificationQueueState.LogPolicyAdopted(this, enactingNation, policyTarget, null, this.Importance(enactingNation, policyTarget), "", "");
	}

	// Token: 0x060009AD RID: 2477 RVA: 0x0003046F File Offset: 0x0002E66F
	public virtual void DeclinePolicy(TINationState enactingNation, TIGameState policyTarget)
	{
		TINotificationQueueState.LogPolicyDeclined(this, enactingNation, policyTarget as TINationState);
		if (this.ImprovesRelations())
		{
			policyTarget.ref_nation.DeclineImproveRelations(enactingNation);
		}
	}

	// Token: 0x060009AE RID: 2478 RVA: 0x00030492 File Offset: 0x0002E692
	public virtual int Importance(TINationState policyNation, TIGameState target)
	{
		return 1;
	}
}
