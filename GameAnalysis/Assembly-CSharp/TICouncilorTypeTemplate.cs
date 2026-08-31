using System;
using System.Collections.Generic;
using System.Text;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000181 RID: 385
public class TICouncilorTypeTemplate : TIDataTemplate
{
	// Token: 0x170000B5 RID: 181
	// (get) Token: 0x060005A3 RID: 1443 RVA: 0x0001A19D File Offset: 0x0001839D
	public string description
	{
		get
		{
			return Loc.T(new StringBuilder("TICouncilorTypeTemplate.description.").Append(base.dataName).ToString());
		}
	}

	// Token: 0x170000B6 RID: 182
	// (get) Token: 0x060005A4 RID: 1444 RVA: 0x0001A1BE File Offset: 0x000183BE
	private TITechTemplate unlockingTech
	{
		get
		{
			return TemplateManager.Find<TITechTemplate>(this.unlockingTechName, false);
		}
	}

	// Token: 0x170000B7 RID: 183
	// (get) Token: 0x060005A5 RID: 1445 RVA: 0x0001A1CC File Offset: 0x000183CC
	public bool unlocked
	{
		get
		{
			return string.IsNullOrEmpty(this.unlockingTechName) || GameStateManager.GlobalResearch().IsTechFinished(this.unlockingTech);
		}
	}

	// Token: 0x170000B8 RID: 184
	// (get) Token: 0x060005A6 RID: 1446 RVA: 0x0001A1F0 File Offset: 0x000183F0
	public List<TIMissionTemplate> missions
	{
		get
		{
			if (this._missions == null)
			{
				this._missions = new List<TIMissionTemplate>();
				foreach (string text in this.missionNames)
				{
					if (!string.IsNullOrEmpty(text))
					{
						TIMissionTemplate timissionTemplate = TemplateManager.Find<TIMissionTemplate>(text, false);
						if (timissionTemplate == null)
						{
							Log.Error("Bad json: " + text, Array.Empty<object>());
						}
						if (!this._missions.Contains(timissionTemplate))
						{
							this._missions.Add(timissionTemplate);
						}
					}
				}
			}
			return this._missions;
		}
	}

	// Token: 0x04000587 RID: 1415
	public string iconStr;

	// Token: 0x04000588 RID: 1416
	public float weight;

	// Token: 0x04000589 RID: 1417
	public string unlockingTechName;

	// Token: 0x0400058A RID: 1418
	public int basePersuasion;

	// Token: 0x0400058B RID: 1419
	public int baseEspionage;

	// Token: 0x0400058C RID: 1420
	public int baseCommand;

	// Token: 0x0400058D RID: 1421
	public int baseInvestigation;

	// Token: 0x0400058E RID: 1422
	public int baseScience;

	// Token: 0x0400058F RID: 1423
	public int baseAdministration;

	// Token: 0x04000590 RID: 1424
	public int baseSecurity;

	// Token: 0x04000591 RID: 1425
	public int baseLoyalty;

	// Token: 0x04000592 RID: 1426
	public int randPersuasion;

	// Token: 0x04000593 RID: 1427
	public int randEspionage;

	// Token: 0x04000594 RID: 1428
	public int randCommand;

	// Token: 0x04000595 RID: 1429
	public int randInvestigation;

	// Token: 0x04000596 RID: 1430
	public int randScience;

	// Token: 0x04000597 RID: 1431
	public int randAdministration;

	// Token: 0x04000598 RID: 1432
	public int randSecurity;

	// Token: 0x04000599 RID: 1433
	public int randLoyalty;

	// Token: 0x0400059A RID: 1434
	public List<FactionIdeology> affinities = new List<FactionIdeology>();

	// Token: 0x0400059B RID: 1435
	public List<FactionIdeology> antiAffinities = new List<FactionIdeology>();

	// Token: 0x0400059C RID: 1436
	public string[] missionNames;

	// Token: 0x0400059D RID: 1437
	public List<TIMissionTemplate> _missions;

	// Token: 0x0400059E RID: 1438
	public CouncilorAttribute[] keyStat;
}
