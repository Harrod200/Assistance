using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200011E RID: 286
public class TIHabSiteCondition_bIcequakes : TIHabSiteCondition
{
	// Token: 0x06000476 RID: 1142 RVA: 0x00015240 File Offset: 0x00013440
	public override bool PassesCondition(TIGameState state)
	{
		bool flag = state.ref_habSite.miningProfile == TIUtilities.GetTemplateValue<TIMiningProfileTemplate>("IcyPlanetoidMine") || state.ref_habSite.miningProfile == TIUtilities.GetTemplateValue<TIMiningProfileTemplate>("GanymedeanMine");
		TISpaceObjectState getSunOrbitingRelatedObject = state.ref_naturalSpaceObject.GetSunOrbitingRelatedObject;
		bool flag2 = false;
		if (getSunOrbitingRelatedObject != null)
		{
			TISpaceBodyTemplate tispaceBodyTemplate = TemplateManager.Find<TISpaceBodyTemplate>(getSunOrbitingRelatedObject.templateName, false);
			if (tispaceBodyTemplate != null && tispaceBodyTemplate.atmosphereSurfaceDensity_kgpm3 > 0.0)
			{
				flag2 = true;
			}
		}
		return state.ref_hab != null && TICondition.PassesComparison(this.sign, flag && flag2, TIUtilities.GetBoolValue(this.strValue));
	}
}
