using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000180 RID: 384
public class TICouncilorTemplate : TIDataTemplate
{
	// Token: 0x060005A1 RID: 1441 RVA: 0x0001A138 File Offset: 0x00018338
	public override TIGameState CreateGameState()
	{
		if (this.randomized)
		{
			return null;
		}
		TIGameState tigameState = base.CreateGameState();
		if (tigameState == null)
		{
			tigameState = GameStateManager.CreateNewGameState<TICouncilorState>();
		}
		return tigameState;
	}

	// Token: 0x0400056A RID: 1386
	public string personalName;

	// Token: 0x0400056B RID: 1387
	public string familyName;

	// Token: 0x0400056C RID: 1388
	public string type;

	// Token: 0x0400056D RID: 1389
	public bool debugOnly;

	// Token: 0x0400056E RID: 1390
	public string debugStartingCouncil;

	// Token: 0x0400056F RID: 1391
	public string debugStartingNation;

	// Token: 0x04000570 RID: 1392
	public int? yearBorn = new int?(1970);

	// Token: 0x04000571 RID: 1393
	public int? monthBorn = new int?(1);

	// Token: 0x04000572 RID: 1394
	public int? dayBorn = new int?(1);

	// Token: 0x04000573 RID: 1395
	public string regionBorn;

	// Token: 0x04000574 RID: 1396
	public string mapRegionBorn;

	// Token: 0x04000575 RID: 1397
	public string strGender;

	// Token: 0x04000576 RID: 1398
	public string strAncestry;

	// Token: 0x04000577 RID: 1399
	public string appearanceTemplateName;

	// Token: 0x04000578 RID: 1400
	public string voiceTemplateName;

	// Token: 0x04000579 RID: 1401
	public int? persuasion;

	// Token: 0x0400057A RID: 1402
	public int? espionage;

	// Token: 0x0400057B RID: 1403
	public int? command;

	// Token: 0x0400057C RID: 1404
	public int? investigation;

	// Token: 0x0400057D RID: 1405
	public int? science;

	// Token: 0x0400057E RID: 1406
	public int? administration;

	// Token: 0x0400057F RID: 1407
	public int? security;

	// Token: 0x04000580 RID: 1408
	public int? loyalty;

	// Token: 0x04000581 RID: 1409
	public bool randomizeTraits = true;

	// Token: 0x04000582 RID: 1410
	public bool allowRandomOnlyTraits;

	// Token: 0x04000583 RID: 1411
	public string[] traits;

	// Token: 0x04000584 RID: 1412
	public bool randomized;

	// Token: 0x04000585 RID: 1413
	public bool alien;

	// Token: 0x04000586 RID: 1414
	public FactionIdeology[] allowedIdeologies;
}
