using System;
using PavonisInteractive.TerraInvicta;
using UnityEngine;

// Token: 0x02000203 RID: 515
public class TIMissionModifier_ResourceSpent : TIMissionModifier
{
	// Token: 0x06000702 RID: 1794 RVA: 0x00021EE8 File Offset: 0x000200E8
	public override float GetModifier(TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0f, FactionResource resource = FactionResource.None)
	{
		this.heldResource = resource;
		float num = 0f;
		if (resourcesSpent > 0f)
		{
			if (resourcesSpent < 1f)
			{
				num = resourcesSpent;
			}
			else if (resource == FactionResource.Money)
			{
				num = 1f + Mathf.Log(resourcesSpent / TemplateManager.global.missionMoneyMultiplier) / 0.6931472f;
			}
			else
			{
				num = 1f + Mathf.Log(resourcesSpent) / 0.6931472f;
			}
		}
		return num;
	}

	// Token: 0x170000F8 RID: 248
	// (get) Token: 0x06000703 RID: 1795 RVA: 0x00021F50 File Offset: 0x00020150
	public override string displayName
	{
		get
		{
			return TIUtilities.GetResourceString(this.heldResource);
		}
	}

	// Token: 0x04000625 RID: 1573
	private FactionResource heldResource;
}
