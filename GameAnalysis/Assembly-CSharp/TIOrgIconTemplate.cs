using System;
using System.Collections.Generic;

// Token: 0x02000294 RID: 660
public class TIOrgIconTemplate : TIDataTemplate
{
	// Token: 0x0600090E RID: 2318 RVA: 0x0002ABAC File Offset: 0x00028DAC
	public override bool IsValid(out string error)
	{
		if (this.primaryOrgType == OrgType.Any)
		{
			error = "Primary org type not set for " + base.dataName;
			return false;
		}
		error = string.Empty;
		return true;
	}

	// Token: 0x0600090F RID: 2319 RVA: 0x0002ABD4 File Offset: 0x00028DD4
	public bool ValidIconForOrg(string orgName, OrgType orgType, int tier)
	{
		return tier >= this.minTier && tier <= this.maxTier && (string.IsNullOrEmpty(this.firstLetters) || orgName.StartsWith(this.firstLetters)) && (this.primaryOrgType == orgType || this.allowedOrgTypes.Contains(orgType));
	}

	// Token: 0x04000671 RID: 1649
	public string path;

	// Token: 0x04000672 RID: 1650
	public string firstLetters;

	// Token: 0x04000673 RID: 1651
	public OrgType primaryOrgType;

	// Token: 0x04000674 RID: 1652
	public List<OrgType> allowedOrgTypes = new List<OrgType>();

	// Token: 0x04000675 RID: 1653
	public int minTier;

	// Token: 0x04000676 RID: 1654
	public int maxTier;
}
