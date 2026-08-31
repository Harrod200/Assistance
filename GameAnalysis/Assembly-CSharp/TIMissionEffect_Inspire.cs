using System;
using PavonisInteractive.TerraInvicta;
using UnityEngine;

// Token: 0x020001E6 RID: 486
public class TIMissionEffect_Inspire : TIMissionEffect
{
	// Token: 0x060006BD RID: 1725 RVA: 0x00020584 File Offset: 0x0001E784
	public override string ApplyEffect(TIMissionState mission, TIGameState target, TIMissionOutcome outcome = TIMissionOutcome.Success)
	{
		TICouncilorState councilor = mission.councilor;
		TICouncilorState ref_councilor = target.ref_councilor;
		CouncilorView viewofCouncilor = councilor.faction.GetViewofCouncilor(ref_councilor);
		float attribute = viewofCouncilor.GetAttribute(CouncilorAttribute.ApparentLoyalty);
		bool flag = councilor.ref_faction != target.ref_faction;
		int num;
		int num2;
		switch (outcome)
		{
		case TIMissionOutcome.CriticalFailure:
			num = -3;
			num2 = -1;
			goto IL_007D;
		case TIMissionOutcome.Failure:
			num = -1;
			num2 = 1;
			goto IL_007D;
		case TIMissionOutcome.Success:
			num = 1;
			num2 = 3;
			goto IL_007D;
		case TIMissionOutcome.CriticalSuccess:
			num = 2;
			num2 = 4;
			goto IL_007D;
		}
		return string.Empty;
		IL_007D:
		int attribute2 = ref_councilor.GetAttribute(CouncilorAttribute.Loyalty, true, true, true, false, false, true);
		int num3 = TIUtilities.RandomRange(num, num2);
		if (flag)
		{
			num3 *= -1;
		}
		else if (attribute2 + num3 <= 0)
		{
			num3 = -attribute2 + 1;
		}
		ref_councilor.ModifyAttribute(CouncilorAttribute.Loyalty, num3);
		int num4 = (num + num2) / 2;
		if (flag)
		{
			num4 *= -1;
		}
		ref_councilor.ModifyAttribute(CouncilorAttribute.ApparentLoyalty, num4);
		float num5 = viewofCouncilor.GetAttribute(CouncilorAttribute.ApparentLoyalty) - attribute;
		if (!flag && ref_councilor.turned && TIUtilities.RandomRange(0f, 15f) < (float)ref_councilor.GetAttribute(CouncilorAttribute.Loyalty, true, true, true, false, false, false))
		{
			ref_councilor.UnTurnCouncilor(false, true);
		}
		if (councilor.faction.HasIntelOnCouncilorSecrets(ref_councilor))
		{
			if (num3 > 0)
			{
				return Loc.T("TIMissionTemplate.Inspire.PositiveEffect", new object[]
				{
					Loc.T("UI.Global.Loyalty"),
					Mathf.Min(ref_councilor.GetAttribute(CouncilorAttribute.Loyalty, true, true, true, false, false, false) - attribute2, num3).ToString()
				});
			}
			if (num3 < 0)
			{
				return Loc.T("TIMissionTemplate.Inspire.NegativeEffect", new object[]
				{
					Loc.T("UI.Global.Loyalty"),
					(-num3).ToString()
				});
			}
			return string.Empty;
		}
		else
		{
			if (num5 > 0f)
			{
				return Loc.T("TIMissionTemplate.Inspire.PositiveEffect", new object[]
				{
					Loc.T("UI.Global.ApparentLoyalty"),
					num4.ToString()
				});
			}
			if (num5 < 0f)
			{
				return Loc.T("TIMissionTemplate.Inspire.NegativeEffect", new object[]
				{
					Loc.T("UI.Global.ApparentLoyalty"),
					num4.ToString()
				});
			}
			return Loc.T("TIMissionTemplate.Inspire.NoVisibleEffect");
		}
	}
}
