using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x020001BE RID: 446
public class TIMissionCondition_ContactableTarget : TIMissionCondition
{
	// Token: 0x170000ED RID: 237
	// (get) Token: 0x0600065B RID: 1627 RVA: 0x0001CDD5 File Offset: 0x0001AFD5
	public override List<string> feedback
	{
		get
		{
			return new List<string> { "TIMissionCondition_ContactableTarget", "TIMissionCondition_ContactableTarget2", "TIMissionCondition_ContactableTarget3" };
		}
	}

	// Token: 0x0600065C RID: 1628 RVA: 0x0001CE00 File Offset: 0x0001B000
	public override string CanTarget(TICouncilorState councilor, TIGameState possibleTarget)
	{
		if (councilor.isAlien)
		{
			if (possibleTarget.ref_faction.CanContactAlien)
			{
				return "_Pass";
			}
			return this.feedback[0];
		}
		else if (possibleTarget.ref_councilor.isAlien)
		{
			if (councilor.faction.CanContactAlien)
			{
				return "_Pass";
			}
			return this.feedback[0];
		}
		else
		{
			if (councilor.faction == possibleTarget.ref_faction)
			{
				return this.feedback[1];
			}
			if (possibleTarget.ref_faction.ignoreContacts.Contains(councilor.faction))
			{
				return this.feedback[2];
			}
			return "_Pass";
		}
	}
}
