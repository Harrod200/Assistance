using System;
using System.Linq;
using System.Text;
using PavonisInteractive.TerraInvicta;
using UnityEngine;

// Token: 0x02000183 RID: 387
public class TIFactionIdeologyTemplate : TIDataTemplate
{
	// Token: 0x170000BE RID: 190
	// (get) Token: 0x060005B2 RID: 1458 RVA: 0x0001A7AE File Offset: 0x000189AE
	public bool human
	{
		get
		{
			return !this.alien;
		}
	}

	// Token: 0x170000BF RID: 191
	// (get) Token: 0x060005B3 RID: 1459 RVA: 0x0001A7B9 File Offset: 0x000189B9
	public bool proAlien
	{
		get
		{
			return this.ideologyCoordinates.x < 0f;
		}
	}

	// Token: 0x170000C0 RID: 192
	// (get) Token: 0x060005B4 RID: 1460 RVA: 0x0001A7CD File Offset: 0x000189CD
	public bool antiAlien
	{
		get
		{
			return this.ideologyCoordinates.x > 0f;
		}
	}

	// Token: 0x170000C1 RID: 193
	// (get) Token: 0x060005B5 RID: 1461 RVA: 0x0001A7E1 File Offset: 0x000189E1
	public bool idealistic
	{
		get
		{
			return this.ideologyCoordinates.y > 0f;
		}
	}

	// Token: 0x170000C2 RID: 194
	// (get) Token: 0x060005B6 RID: 1462 RVA: 0x0001A7F5 File Offset: 0x000189F5
	public bool cynical
	{
		get
		{
			return this.ideologyCoordinates.y < 0f;
		}
	}

	// Token: 0x170000C3 RID: 195
	// (get) Token: 0x060005B7 RID: 1463 RVA: 0x0001A809 File Offset: 0x00018A09
	public bool fanatic
	{
		get
		{
			return Mathf.Abs(this.ideologyCoordinates.x) >= 2f;
		}
	}

	// Token: 0x170000C4 RID: 196
	// (get) Token: 0x060005B8 RID: 1464 RVA: 0x0001A825 File Offset: 0x00018A25
	public string ideologyStr
	{
		get
		{
			return Loc.T(new StringBuilder("TIFactionIdeologyTemplate.").Append(base.dataName).ToString());
		}
	}

	// Token: 0x170000C5 RID: 197
	// (get) Token: 0x060005B9 RID: 1465 RVA: 0x0001A846 File Offset: 0x00018A46
	public string ideologyStrGeneric
	{
		get
		{
			return Loc.T(new StringBuilder("TIFactionIdeologyTemplate.").Append(base.dataName).Append("Generic").ToString());
		}
	}

	// Token: 0x170000C6 RID: 198
	// (get) Token: 0x060005BA RID: 1466 RVA: 0x0001A871 File Offset: 0x00018A71
	public string ideologyStrPublicOpinion
	{
		get
		{
			return Loc.T(new StringBuilder("TIFactionIdeologyTemplate.").Append(base.dataName).Append(".public").ToString());
		}
	}

	// Token: 0x060005BB RID: 1467 RVA: 0x0001A89C File Offset: 0x00018A9C
	public static TIFactionIdeologyTemplate GetIdeologyTemplate(FactionIdeology ideology)
	{
		return TIGlobalValuesState.GlobalValues.ideologyTemplateLookup[ideology];
	}

	// Token: 0x060005BC RID: 1468 RVA: 0x0001A8B0 File Offset: 0x00018AB0
	public static TIFactionState GetFactionByIdeology(FactionIdeology ideology)
	{
		return GameStateManager.AllFactions().FirstOrDefault<TIFactionState>((TIFactionState x) => x.ideology.ideology == ideology);
	}

	// Token: 0x060005BD RID: 1469 RVA: 0x0001A8E0 File Offset: 0x00018AE0
	public static TIFactionState GetFactionByIdeologyTemplate(TIFactionIdeologyTemplate template)
	{
		return GameStateManager.AllFactions().FirstOrDefault<TIFactionState>((TIFactionState x) => x.ideology == template);
	}

	// Token: 0x040005A7 RID: 1447
	public bool alien;

	// Token: 0x040005A8 RID: 1448
	public bool undecided;

	// Token: 0x040005A9 RID: 1449
	public int sortOrder;

	// Token: 0x040005AA RID: 1450
	public int willProxy;

	// Token: 0x040005AB RID: 1451
	public int willAppease;

	// Token: 0x040005AC RID: 1452
	public int initialReactionGroup;

	// Token: 0x040005AD RID: 1453
	public FactionIdeology ideology;

	// Token: 0x040005AE RID: 1454
	public Vector3 ideologyCoordinates;
}
