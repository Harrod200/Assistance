using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200017F RID: 383
public class TICouncilorAppearanceTemplate : TIDataTemplate
{
	// Token: 0x170000B4 RID: 180
	// (get) Token: 0x06000599 RID: 1433 RVA: 0x00019EC0 File Offset: 0x000180C0
	public List<TICouncilorTypeTemplate> allowedJobs
	{
		get
		{
			List<TICouncilorTypeTemplate> list = new List<TICouncilorTypeTemplate>();
			if (this.allowedJobNames == null || this.allowedJobNames.Count == 0 || this.allowedJobNames[0].ToLowerInvariant() == "all")
			{
				list.AddRange(TemplateManager.IterateByClass<TICouncilorTypeTemplate>(true));
			}
			else
			{
				foreach (string text in this.allowedJobNames)
				{
					if (!string.IsNullOrEmpty(text))
					{
						TICouncilorTypeTemplate ticouncilorTypeTemplate = TemplateManager.Find<TICouncilorTypeTemplate>(text, false);
						if (ticouncilorTypeTemplate != null && text != "all")
						{
							list.Add(ticouncilorTypeTemplate);
						}
						else
						{
							Log.Error("Bad allowed job entry " + text + " in councilor appearance template " + base.dataName, Array.Empty<object>());
						}
					}
				}
			}
			return list;
		}
	}

	// Token: 0x0600059A RID: 1434 RVA: 0x00019F9C File Offset: 0x0001819C
	public string idleVideo(TICouncilorState councilor)
	{
		if (!councilor.useOldPortrait)
		{
			return this.idleVideoYoung;
		}
		return this.idleVideoOld;
	}

	// Token: 0x0600059B RID: 1435 RVA: 0x00019FB3 File Offset: 0x000181B3
	public string portrait(TICouncilorState councilor)
	{
		if (!councilor.useOldPortrait)
		{
			return this.portraitYoung;
		}
		return this.portraitOld;
	}

	// Token: 0x0600059C RID: 1436 RVA: 0x00019FCA File Offset: 0x000181CA
	public string icon(TICouncilorState councilor)
	{
		if (!councilor.useOldPortrait)
		{
			return this.iconYoung;
		}
		return this.iconOld;
	}

	// Token: 0x0600059D RID: 1437 RVA: 0x00019FE4 File Offset: 0x000181E4
	public bool ValidForCharacter(TICouncilorState councilorState, int gameYear, bool requireJobAlignment, bool requireAncestryAlignment, bool requireNotDuplicated)
	{
		if (!this.enable)
		{
			return false;
		}
		if (this.specific_person)
		{
			return false;
		}
		if (councilorState.isAlien && !this.allowedAncestries.Contains(CouncilorAncestry.Alien))
		{
			return false;
		}
		if (!councilorState.isAlien && this.allowedAncestries.Contains(CouncilorAncestry.Alien))
		{
			return false;
		}
		if (!this.allowedGenders.Contains(councilorState.gender))
		{
			return false;
		}
		if (requireJobAlignment && !this.allowedJobs.Contains(councilorState.typeTemplate))
		{
			return false;
		}
		if (requireAncestryAlignment && !this.allowedAncestries.Contains(councilorState.ancestry))
		{
			return false;
		}
		if (gameYear > 0)
		{
			int? num = this.year;
			if ((gameYear < num.GetValueOrDefault()) & (num != null))
			{
				return false;
			}
		}
		if (requireNotDuplicated && TICouncilorAppearanceTemplate.AppearanceTemplatesInUse().Contains(base.dataName))
		{
			return false;
		}
		if (this.regionalHeadwear)
		{
			switch (councilorState.homeRegion.template.asi_RegionalHeadwear)
			{
			case RegionalHeadwear.None:
				return false;
			case RegionalHeadwear.Female:
				if (councilorState.gender != CouncilorGender.Female)
				{
					return false;
				}
				break;
			case RegionalHeadwear.Male:
				if (councilorState.gender != CouncilorGender.Male)
				{
					return false;
				}
				break;
			}
		}
		return true;
	}

	// Token: 0x0600059E RID: 1438 RVA: 0x0001A0FA File Offset: 0x000182FA
	public static List<string> AppearanceTemplatesInUse()
	{
		return TIGlobalValuesState.GlobalValues.councilorAppearanceTemplatesInUse;
	}

	// Token: 0x0400055C RID: 1372
	public bool enable;

	// Token: 0x0400055D RID: 1373
	public bool specific_person;

	// Token: 0x0400055E RID: 1374
	public string idleVideoYoung;

	// Token: 0x0400055F RID: 1375
	public string idleVideoOld;

	// Token: 0x04000560 RID: 1376
	public string portraitYoung;

	// Token: 0x04000561 RID: 1377
	public string portraitOld;

	// Token: 0x04000562 RID: 1378
	public string iconYoung;

	// Token: 0x04000563 RID: 1379
	public string iconOld;

	// Token: 0x04000564 RID: 1380
	public bool regionalHeadwear;

	// Token: 0x04000565 RID: 1381
	public int? year;

	// Token: 0x04000566 RID: 1382
	public List<CouncilorGender> allowedGenders = new List<CouncilorGender>();

	// Token: 0x04000567 RID: 1383
	public List<CouncilorAncestry> allowedAncestries = new List<CouncilorAncestry>();

	// Token: 0x04000568 RID: 1384
	public List<string> allowedJobNames = new List<string>();

	// Token: 0x04000569 RID: 1385
	public static int ageCutPoint = 55;
}
