using System;

// Token: 0x02000186 RID: 390
public struct AIValues
{
	// Token: 0x060005DD RID: 1501 RVA: 0x0001B031 File Offset: 0x00019231
	public float AIValueFromString(string variableName)
	{
		if (!string.IsNullOrEmpty(variableName))
		{
			return (float)base.GetType().GetField(variableName).GetValue(this);
		}
		return 1f;
	}

	// Token: 0x170000E0 RID: 224
	// (get) Token: 0x060005DE RID: 1502 RVA: 0x0001B06C File Offset: 0x0001926C
	public static AIValues Zero
	{
		get
		{
			return new AIValues
			{
				proAlien = 0f,
				antiAlien = 0f,
				protectHumanLife = 0f,
				protectAlienLife = 0f,
				dirtyTricks = 0f,
				riskAversion = 0f,
				protectCouncilors = 0f,
				wantEarthWarCapability = 0f,
				wantSpaceFacilities = 0f,
				wantSpaceWarCapability = 0f,
				wantPopularity = 0f,
				gatherMoney = 0f,
				gatherInfluence = 0f,
				gatherOps = 0f,
				gatherScience = 0f,
				energyTechs = 0f,
				materialsTechs = 0f,
				informationTechs = 0f,
				lifeTechs = 0f,
				militaryTechs = 0f,
				socialTechs = 0f,
				spaceTechs = 0f,
				preserveLife = 0f
			};
		}
	}

	// Token: 0x060005DF RID: 1503 RVA: 0x0001B198 File Offset: 0x00019398
	public static AIValues operator *(AIValues a, AIValues b)
	{
		AIValues aivalues = a;
		aivalues.proAlien *= b.proAlien;
		aivalues.antiAlien *= b.antiAlien;
		aivalues.protectHumanLife *= b.protectHumanLife;
		aivalues.protectAlienLife *= b.protectAlienLife;
		aivalues.dirtyTricks *= b.dirtyTricks;
		aivalues.riskAversion *= b.riskAversion;
		aivalues.protectCouncilors *= b.protectCouncilors;
		aivalues.wantEarthWarCapability *= b.wantEarthWarCapability;
		aivalues.wantSpaceFacilities *= b.wantSpaceFacilities;
		aivalues.wantSpaceWarCapability *= b.wantSpaceWarCapability;
		aivalues.wantPopularity *= b.wantPopularity;
		aivalues.gatherMoney *= b.gatherMoney;
		aivalues.gatherInfluence *= b.gatherInfluence;
		aivalues.gatherOps *= b.gatherOps;
		aivalues.gatherScience *= b.gatherScience;
		aivalues.energyTechs *= b.energyTechs;
		aivalues.materialsTechs *= b.materialsTechs;
		aivalues.informationTechs *= b.informationTechs;
		aivalues.lifeTechs *= b.lifeTechs;
		aivalues.militaryTechs *= b.militaryTechs;
		aivalues.socialTechs *= b.socialTechs;
		aivalues.spaceTechs *= b.spaceTechs;
		aivalues.preserveLife *= b.preserveLife;
		return aivalues;
	}

	// Token: 0x060005E0 RID: 1504 RVA: 0x0001B330 File Offset: 0x00019530
	public static AIValues operator +(AIValues a, AIValues b)
	{
		AIValues aivalues = a;
		aivalues.proAlien += b.proAlien;
		aivalues.antiAlien += b.antiAlien;
		aivalues.protectHumanLife += b.protectHumanLife;
		aivalues.protectAlienLife += b.protectAlienLife;
		aivalues.dirtyTricks += b.dirtyTricks;
		aivalues.riskAversion += b.riskAversion;
		aivalues.protectCouncilors += b.protectCouncilors;
		aivalues.wantEarthWarCapability += b.wantEarthWarCapability;
		aivalues.wantSpaceFacilities += b.wantSpaceFacilities;
		aivalues.wantSpaceWarCapability += b.wantSpaceWarCapability;
		aivalues.wantPopularity += b.wantPopularity;
		aivalues.gatherMoney += b.gatherMoney;
		aivalues.gatherInfluence += b.gatherInfluence;
		aivalues.gatherOps += b.gatherOps;
		aivalues.gatherScience += b.gatherScience;
		aivalues.energyTechs += b.energyTechs;
		aivalues.materialsTechs += b.materialsTechs;
		aivalues.informationTechs += b.informationTechs;
		aivalues.lifeTechs += b.lifeTechs;
		aivalues.militaryTechs += b.militaryTechs;
		aivalues.socialTechs += b.socialTechs;
		aivalues.spaceTechs += b.spaceTechs;
		aivalues.preserveLife += b.preserveLife;
		return aivalues;
	}

	// Token: 0x040005FA RID: 1530
	public float proAlien;

	// Token: 0x040005FB RID: 1531
	public float antiAlien;

	// Token: 0x040005FC RID: 1532
	public float protectHumanLife;

	// Token: 0x040005FD RID: 1533
	public float protectAlienLife;

	// Token: 0x040005FE RID: 1534
	public float dirtyTricks;

	// Token: 0x040005FF RID: 1535
	public float riskAversion;

	// Token: 0x04000600 RID: 1536
	public float protectCouncilors;

	// Token: 0x04000601 RID: 1537
	public float wantEarthWarCapability;

	// Token: 0x04000602 RID: 1538
	public float wantSpaceFacilities;

	// Token: 0x04000603 RID: 1539
	public float wantSpaceWarCapability;

	// Token: 0x04000604 RID: 1540
	public float wantPopularity;

	// Token: 0x04000605 RID: 1541
	public float gatherMoney;

	// Token: 0x04000606 RID: 1542
	public float gatherInfluence;

	// Token: 0x04000607 RID: 1543
	public float gatherOps;

	// Token: 0x04000608 RID: 1544
	public float gatherScience;

	// Token: 0x04000609 RID: 1545
	public float energyTechs;

	// Token: 0x0400060A RID: 1546
	public float materialsTechs;

	// Token: 0x0400060B RID: 1547
	public float informationTechs;

	// Token: 0x0400060C RID: 1548
	public float lifeTechs;

	// Token: 0x0400060D RID: 1549
	public float militaryTechs;

	// Token: 0x0400060E RID: 1550
	public float socialTechs;

	// Token: 0x0400060F RID: 1551
	public float spaceTechs;

	// Token: 0x04000610 RID: 1552
	public float preserveLife;

	// Token: 0x04000611 RID: 1553
	public float fleetSmalls;

	// Token: 0x04000612 RID: 1554
	public float fleetMediums;
}
