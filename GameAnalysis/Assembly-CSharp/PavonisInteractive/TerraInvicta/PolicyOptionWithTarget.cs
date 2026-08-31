using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200076C RID: 1900
	public class PolicyOptionWithTarget
	{
		// Token: 0x170009F4 RID: 2548
		// (get) Token: 0x060039AD RID: 14765 RVA: 0x001553A5 File Offset: 0x001535A5
		public TIPolicyOption policy
		{
			get
			{
				return PolicyManager.policies[this.policyType] as TIPolicyOption;
			}
		}

		// Token: 0x060039AE RID: 14766 RVA: 0x001553BC File Offset: 0x001535BC
		public PolicyOptionWithTarget(TINationState actingNation, TIPolicyOption policy, TIGameState target)
		{
			this.actingNation = actingNation;
			this.policyType = policy.GetPolicyType();
			this.target = target;
		}

		// Token: 0x060039AF RID: 14767 RVA: 0x001553DE File Offset: 0x001535DE
		public PolicyOptionWithTarget(TINationState actingNation, PolicyType policyType, TIGameState target)
		{
			this.actingNation = actingNation;
			this.policyType = policyType;
			this.target = target;
		}

		// Token: 0x170009F5 RID: 2549
		// (get) Token: 0x060039B0 RID: 14768 RVA: 0x001553FC File Offset: 0x001535FC
		public bool CausesGuaranteedOneWayNationExpansion
		{
			get
			{
				if (this.actingNation.alienNation && this.policyType == PolicyType.TransferRegionsOption)
				{
					TIGameState tigameState = this.target;
					bool? flag;
					if (tigameState == null)
					{
						flag = null;
					}
					else
					{
						TINationState ref_nation = tigameState.ref_nation;
						flag = ((ref_nation != null) ? new bool?(ref_nation.executiveFaction.IsAlienProxy) : null);
					}
					bool? flag2 = flag;
					if (flag2.GetValueOrDefault())
					{
						return true;
					}
				}
				return false;
			}
		}

		// Token: 0x060039B1 RID: 14769 RVA: 0x00155465 File Offset: 0x00153665
		public bool AllowAIToUseWithoutGoal()
		{
			return this.SuperNationRelease() || this.CausesGuaranteedOneWayNationExpansion;
		}

		// Token: 0x060039B2 RID: 14770 RVA: 0x0015547C File Offset: 0x0015367C
		public bool SuperNationRelease()
		{
			return this.policyType == PolicyType.PeacefulBreakupOption && !this.target.ref_nation.extant && this.target.ref_nation.claims.Count > this.actingNation.claims.Count;
		}

		// Token: 0x170009F6 RID: 2550
		// (get) Token: 0x060039B3 RID: 14771 RVA: 0x001554CE File Offset: 0x001536CE
		public int GoallessImportance
		{
			get
			{
				if (!this.AllowAIToUseWithoutGoal())
				{
					return 10;
				}
				return 20;
			}
		}

		// Token: 0x04002568 RID: 9576
		public TINationState actingNation;

		// Token: 0x04002569 RID: 9577
		public PolicyType policyType;

		// Token: 0x0400256A RID: 9578
		public TIGameState target;
	}
}
