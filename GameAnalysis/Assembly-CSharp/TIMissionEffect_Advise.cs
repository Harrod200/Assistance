using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020001EF RID: 495
public class TIMissionEffect_Advise : TIMissionEffect
{
	// Token: 0x060006CF RID: 1743 RVA: 0x00020FB4 File Offset: 0x0001F1B4
	public override string ApplyEffect(TIMissionState mission, TIGameState target, TIMissionOutcome outcome = TIMissionOutcome.Success)
	{
		TICouncilorState councilor = mission.councilor;
		if (target.isNationState)
		{
			float adviserScienceBonus = target.ref_nation.adviserScienceBonus;
			float adviserCommandBonus = target.ref_nation.adviserCommandBonus;
			float adviserAdministrationBonus = target.ref_nation.adviserAdministrationBonus;
			target.ref_nation.AddAdvisingCouncilor(councilor);
			return Loc.T("TIMissionEffect_Advise.Special1", new object[]
			{
				(target.ref_nation.adviserScienceBonus - adviserScienceBonus).ToPercent("P0"),
				(target.ref_nation.adviserAdministrationBonus - adviserAdministrationBonus).ToPercent("P0"),
				(target.ref_nation.adviserCommandBonus - adviserCommandBonus).ToString("N2")
			});
		}
		if (target.isHabState)
		{
			TIHabState ref_hab = target.ref_hab;
			float advisingAttribute = ref_hab.GetAdvisingAttribute(CouncilorAttribute.Science);
			float advisingAttribute2 = ref_hab.GetAdvisingAttribute(CouncilorAttribute.Command);
			float advisingAttribute3 = ref_hab.GetAdvisingAttribute(CouncilorAttribute.Administration);
			ref_hab.AddAdvisingCouncilor(councilor);
			return Loc.T("TIMissionEffect_Advise.Special2", new object[]
			{
				(ref_hab.GetAdvisingAttribute(CouncilorAttribute.Science) - advisingAttribute).ToPercent("P0"),
				(ref_hab.GetAdvisingAttribute(CouncilorAttribute.Administration) - advisingAttribute3).ToPercent("P0"),
				(ref_hab.GetAdvisingAttribute(CouncilorAttribute.Command) - advisingAttribute2).ToPercent("P0")
			});
		}
		return Loc.T("TIMissionEffect_Advise.Special3", new object[]
		{
			councilor.AdvisingBonus(CouncilorAttribute.Command).ToPercent("P0"),
			councilor.AdvisingBonus(CouncilorAttribute.Science).ToPercent("P0")
		});
	}
}
