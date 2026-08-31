using System;
using System.Text;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200017C RID: 380
public struct Formation
{
	// Token: 0x170000B1 RID: 177
	// (get) Token: 0x06000568 RID: 1384 RVA: 0x000180A4 File Offset: 0x000162A4
	public TIFormationTemplate pattern
	{
		get
		{
			return TemplateManager.Find<TIFormationTemplate>(this.patternDataName, false);
		}
	}

	// Token: 0x06000569 RID: 1385 RVA: 0x000180B2 File Offset: 0x000162B2
	public Formation(Formation formation)
	{
		this.patternDataName = formation.patternDataName;
		this.spacing = formation.spacing;
		this.concentration = formation.concentration;
		this.focus = formation.focus;
	}

	// Token: 0x0600056A RID: 1386 RVA: 0x000180E4 File Offset: 0x000162E4
	public Formation(string patternDataName, FormationFocus focus, FormationSpacing spacing, FormationConcentration concentration)
	{
		this.patternDataName = patternDataName;
		this.spacing = spacing;
		this.concentration = concentration;
		this.focus = focus;
	}

	// Token: 0x0600056B RID: 1387 RVA: 0x00018103 File Offset: 0x00016303
	public static string spacingName(FormationSpacing spacing)
	{
		return Loc.T(new StringBuilder("TIFormationTemplate.Spacing.").Append(spacing.ToString()).ToString());
	}

	// Token: 0x0600056C RID: 1388 RVA: 0x0001812B File Offset: 0x0001632B
	public static string concentrationName(FormationConcentration concentration)
	{
		return Loc.T(new StringBuilder("TIFormationTemplate.Concentration.").Append(concentration.ToString()).ToString());
	}

	// Token: 0x0600056D RID: 1389 RVA: 0x00018153 File Offset: 0x00016353
	public static string focusName(FormationFocus focus)
	{
		return Loc.T(new StringBuilder("TIFormationTemplate.Focus.").Append(focus.ToString()).ToString());
	}

	// Token: 0x0600056E RID: 1390 RVA: 0x0001817B File Offset: 0x0001637B
	public static string patternName(string patternDataName)
	{
		return TemplateManager.Find<TIFormationTemplate>(patternDataName, false).displayName;
	}

	// Token: 0x0600056F RID: 1391 RVA: 0x00018189 File Offset: 0x00016389
	public static string concentrationDescription(FormationConcentration concentration)
	{
		return Loc.T(new StringBuilder("TIFormationTemplate.Concentration.Desc.").Append(concentration.ToString()).ToString());
	}

	// Token: 0x06000570 RID: 1392 RVA: 0x000181B1 File Offset: 0x000163B1
	public static string focusDescription(FormationFocus focus)
	{
		return Loc.T(new StringBuilder("TIFormationTemplate.Focus.Desc.").Append(focus.ToString()).ToString());
	}

	// Token: 0x06000571 RID: 1393 RVA: 0x000181D9 File Offset: 0x000163D9
	public static string patternDescription(string patternDataname)
	{
		return Loc.T(new StringBuilder("TIFormationTemplate.Desc.").Append(patternDataname.ToString()).ToString());
	}

	// Token: 0x170000B2 RID: 178
	// (get) Token: 0x06000572 RID: 1394 RVA: 0x000181FC File Offset: 0x000163FC
	public string displayName
	{
		get
		{
			return Loc.T("TIFormationTemplate.displayName", new object[]
			{
				Formation.spacingName(this.spacing),
				Formation.patternName(this.patternDataName),
				Formation.focusName(this.focus),
				Formation.concentrationName(this.concentration)
			});
		}
	}

	// Token: 0x170000B3 RID: 179
	// (get) Token: 0x06000573 RID: 1395 RVA: 0x00018251 File Offset: 0x00016451
	public string description
	{
		get
		{
			return Loc.T("TIFormationTemplate.description", new object[]
			{
				Formation.patternDescription(this.patternDataName),
				Formation.focusDescription(this.focus),
				Formation.concentrationDescription(this.concentration)
			});
		}
	}

	// Token: 0x04000555 RID: 1365
	public string patternDataName;

	// Token: 0x04000556 RID: 1366
	public FormationSpacing spacing;

	// Token: 0x04000557 RID: 1367
	public FormationConcentration concentration;

	// Token: 0x04000558 RID: 1368
	public FormationFocus focus;
}
