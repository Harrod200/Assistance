using System;
using System.Collections.Generic;

// Token: 0x020002B1 RID: 689
public static class PolicyManager
{
	// Token: 0x06000983 RID: 2435 RVA: 0x00030080 File Offset: 0x0002E280
	public static void Initialize()
	{
		PolicyManager.policies.Clear();
		PolicyManager.policies.Add(PolicyType.ProposeAllianceOption, new ProposeAllianceOption());
		PolicyManager.policies.Add(PolicyType.EndAllianceOption, new EndAllianceOption());
		PolicyManager.policies.Add(PolicyType.InitiateRivalryOption, new InitiateRivalryOption());
		PolicyManager.policies.Add(PolicyType.EndRivalryOption, new EndRivalryOption());
		PolicyManager.policies.Add(PolicyType.WarOption, new WarOption());
		PolicyManager.policies.Add(PolicyType.EndWarOption, new EndWarOption());
		PolicyManager.policies.Add(PolicyType.JoinFederationOption, new JoinFederationOption());
		PolicyManager.policies.Add(PolicyType.LeaveFederationOption, new LeaveFederationOption());
		PolicyManager.policies.Add(PolicyType.UnificationOption, new UnificationOption());
		PolicyManager.policies.Add(PolicyType.PeacefulBreakupOption, new PeacefulBreakupOption());
		PolicyManager.policies.Add(PolicyType.TransferRegionsOption, new TransferRegionsOption());
		PolicyManager.policies.Add(PolicyType.DisbandArmyOption, new DisbandArmyOption());
		PolicyManager.policies.Add(PolicyType.DisarmNuclearWeaponsOption, new DisarmNuclearWeaponsOption());
		PolicyManager.policies.Add(PolicyType.DeclareIndependenceOption, new DeclareIndependenceOption());
		PolicyManager.policies.Add(PolicyType.EmployNuclearWeaponsOption, new EmployNuclearWeaponsOption());
		PolicyManager.policies.Add(PolicyType.CancelOption, new CancelOption());
	}

	// Token: 0x0400083D RID: 2109
	public static Dictionary<PolicyType, IPolicyOption> policies = new Dictionary<PolicyType, IPolicyOption>();

	// Token: 0x0400083E RID: 2110
	public static readonly List<PolicyType> RegularPolicyNames_SetPolicy = new List<PolicyType>
	{
		PolicyType.WarOption,
		PolicyType.EndWarOption,
		PolicyType.JoinFederationOption,
		PolicyType.UnificationOption,
		PolicyType.LeaveFederationOption
	};

	// Token: 0x0400083F RID: 2111
	public static readonly List<PolicyType> ImproveRelationsPolicyNames_SetPolicy = new List<PolicyType>
	{
		PolicyType.EndWarOption,
		PolicyType.JoinFederationOption,
		PolicyType.UnificationOption,
		PolicyType.TransferRegionsOption
	};

	// Token: 0x04000840 RID: 2112
	public static readonly List<PolicyType> DegradeRelationsPolicyNames_SetPolicy = new List<PolicyType>
	{
		PolicyType.WarOption,
		PolicyType.LeaveFederationOption,
		PolicyType.DeclareIndependenceOption
	};

	// Token: 0x04000841 RID: 2113
	public static readonly List<PolicyType> WeakenNationPolicyNames_SetPolicy = new List<PolicyType>
	{
		PolicyType.TransferRegionsOption,
		PolicyType.DisbandArmyOption,
		PolicyType.DisarmNuclearWeaponsOption,
		PolicyType.PeacefulBreakupOption
	};

	// Token: 0x04000842 RID: 2114
	public static readonly List<PolicyType> StabilizeNationPolicyNames_SetPolicy = new List<PolicyType>
	{
		PolicyType.WarOption,
		PolicyType.TransferRegionsOption,
		PolicyType.PeacefulBreakupOption,
		PolicyType.JoinFederationOption,
		PolicyType.UnificationOption,
		PolicyType.EndWarOption
	};

	// Token: 0x04000843 RID: 2115
	public static readonly List<PolicyType> ImproveRelationsPolicyNames_Faction = new List<PolicyType>
	{
		PolicyType.ProposeAllianceOption,
		PolicyType.EndRivalryOption
	};

	// Token: 0x04000844 RID: 2116
	public static readonly List<PolicyType> DegradeRelationsPolicyNames_Faction = new List<PolicyType>
	{
		PolicyType.EndAllianceOption,
		PolicyType.InitiateRivalryOption
	};

	// Token: 0x04000845 RID: 2117
	public static readonly List<PolicyType> AllPolicyNames_Faction = new List<PolicyType>
	{
		PolicyType.ProposeAllianceOption,
		PolicyType.EndAllianceOption,
		PolicyType.EndRivalryOption,
		PolicyType.InitiateRivalryOption
	};

	// Token: 0x04000846 RID: 2118
	public static readonly List<PolicyType> NormalizeRelationsPolicyNames_Faction = new List<PolicyType>
	{
		PolicyType.EndAllianceOption,
		PolicyType.EndRivalryOption
	};
}
