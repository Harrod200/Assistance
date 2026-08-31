using System;
using System.Text;
using PavonisInteractive.TerraInvicta;
using UnityEngine;

// Token: 0x0200036F RID: 879
public class TIMiningProfileTemplate : TIDataTemplate
{
	// Token: 0x170001D7 RID: 471
	// (get) Token: 0x06000FE0 RID: 4064 RVA: 0x00052741 File Offset: 0x00050941
	public string description
	{
		get
		{
			return Loc.T(new StringBuilder(base.GetType().Name).Append(".description.").Append(base.dataName).ToString());
		}
	}

	// Token: 0x06000FE1 RID: 4065 RVA: 0x00052774 File Offset: 0x00050974
	public bool ZeroInBaseRange(FactionResource resource)
	{
		switch (resource)
		{
		case FactionResource.Water:
			return this.water_min <= 0f && this.water_mean - this.water_width / 2f <= 0f;
		case FactionResource.Volatiles:
			return this.volatiles_min <= 0f && this.volatiles_mean - this.volatiles_width / 2f <= 0f;
		case FactionResource.Metals:
			return this.metals_min <= 0f && this.metals_mean - this.metals_width / 2f <= 0f;
		case FactionResource.NobleMetals:
			return this.nobles_min <= 0f && this.nobles_mean - this.nobles_width / 2f <= 0f;
		case FactionResource.Fissiles:
			return this.fissiles_min <= 0f && this.fissiles_mean - this.fissiles_width / 2f <= 0f;
		default:
			return true;
		}
	}

	// Token: 0x06000FE2 RID: 4066 RVA: 0x00052884 File Offset: 0x00050A84
	public static SiteProfileRating GetSiteProfileRating(float mean, float width, float min, float globalCap, bool prospected)
	{
		if (mean <= 0f && (prospected || (min <= 0f && width <= Mathf.Abs(mean))))
		{
			return SiteProfileRating.empty;
		}
		if (min <= 0f && !prospected)
		{
			return SiteProfileRating.possible;
		}
		if (mean <= globalCap * 0.333f)
		{
			return SiteProfileRating.low;
		}
		if (mean <= globalCap * 0.667f)
		{
			return SiteProfileRating.medium;
		}
		if (mean <= globalCap * 0.95f)
		{
			return SiteProfileRating.high;
		}
		return SiteProfileRating.max;
	}

	// Token: 0x06000FE3 RID: 4067 RVA: 0x000528E4 File Offset: 0x00050AE4
	public string GetProfileRatingIconPath(FactionResource resource, bool inline, bool prospected)
	{
		SiteProfileRating siteProfileRating = SiteProfileRating.empty;
		switch (resource)
		{
		case FactionResource.Water:
			siteProfileRating = TIMiningProfileTemplate.GetSiteProfileRating(this.water_mean, this.water_width, this.water_min, TIGlobalValuesState.GlobalValues.maxGlobalExpectedHabSiteProduction_day[resource], prospected);
			break;
		case FactionResource.Volatiles:
			siteProfileRating = TIMiningProfileTemplate.GetSiteProfileRating(this.volatiles_mean, this.volatiles_width, this.volatiles_min, TIGlobalValuesState.GlobalValues.maxGlobalExpectedHabSiteProduction_day[resource], prospected);
			break;
		case FactionResource.Metals:
			siteProfileRating = TIMiningProfileTemplate.GetSiteProfileRating(this.metals_mean, this.metals_width, this.metals_min, TIGlobalValuesState.GlobalValues.maxGlobalExpectedHabSiteProduction_day[resource], prospected);
			break;
		case FactionResource.NobleMetals:
			siteProfileRating = TIMiningProfileTemplate.GetSiteProfileRating(this.nobles_mean, this.nobles_width, this.nobles_min, TIGlobalValuesState.GlobalValues.maxGlobalExpectedHabSiteProduction_day[resource], prospected);
			break;
		case FactionResource.Fissiles:
			siteProfileRating = TIMiningProfileTemplate.GetSiteProfileRating(this.fissiles_mean, this.fissiles_width, this.fissiles_min, TIGlobalValuesState.GlobalValues.maxGlobalExpectedHabSiteProduction_day[resource], prospected);
			break;
		}
		switch (siteProfileRating)
		{
		case SiteProfileRating.empty:
			if (!inline)
			{
				return TemplateManager.global.pathResNoneIcon;
			}
			return TemplateManager.global.zeroResourcesInlineSpritePath;
		case SiteProfileRating.possible:
			if (!inline)
			{
				return TemplateManager.global.pathResPossibleIcon;
			}
			return TemplateManager.global.unknownResourcesInlineSpritePath;
		case SiteProfileRating.low:
			if (!inline)
			{
				return TemplateManager.global.pathResLowIcon;
			}
			return TemplateManager.global.level1ResourcesInlineSpritePath;
		case SiteProfileRating.medium:
			if (!inline)
			{
				return TemplateManager.global.pathResMedIcon;
			}
			return TemplateManager.global.level2ResourcesInlineSpritePath;
		case SiteProfileRating.high:
			if (!inline)
			{
				return TemplateManager.global.pathResHighIcon;
			}
			return TemplateManager.global.level3ResourcesInlineSpritePath;
		case SiteProfileRating.max:
			if (!inline)
			{
				return TemplateManager.global.pathResMaxIcon;
			}
			return TemplateManager.global.level4ResourcesInlineSpritePath;
		default:
			return string.Empty;
		}
	}

	// Token: 0x04001020 RID: 4128
	public bool modifyBySize;

	// Token: 0x04001021 RID: 4129
	public int modelValue;

	// Token: 0x04001022 RID: 4130
	public float water_mean;

	// Token: 0x04001023 RID: 4131
	public float water_width;

	// Token: 0x04001024 RID: 4132
	public float water_min;

	// Token: 0x04001025 RID: 4133
	public float water_jump;

	// Token: 0x04001026 RID: 4134
	public float volatiles_mean;

	// Token: 0x04001027 RID: 4135
	public float volatiles_width;

	// Token: 0x04001028 RID: 4136
	public float volatiles_min;

	// Token: 0x04001029 RID: 4137
	public float volatiles_jump;

	// Token: 0x0400102A RID: 4138
	public float metals_mean;

	// Token: 0x0400102B RID: 4139
	public float metals_width;

	// Token: 0x0400102C RID: 4140
	public float metals_min;

	// Token: 0x0400102D RID: 4141
	public float metals_jump;

	// Token: 0x0400102E RID: 4142
	public float nobles_mean;

	// Token: 0x0400102F RID: 4143
	public float nobles_width;

	// Token: 0x04001030 RID: 4144
	public float nobles_min;

	// Token: 0x04001031 RID: 4145
	public float nobles_jump;

	// Token: 0x04001032 RID: 4146
	public float fissiles_mean;

	// Token: 0x04001033 RID: 4147
	public float fissiles_width;

	// Token: 0x04001034 RID: 4148
	public float fissiles_min;

	// Token: 0x04001035 RID: 4149
	public float fissiles_jump;
}
