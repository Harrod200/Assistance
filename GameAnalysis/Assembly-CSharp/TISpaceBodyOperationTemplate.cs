using System;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000340 RID: 832
public abstract class TISpaceBodyOperationTemplate : TIOperationTemplate
{
	// Token: 0x06000E50 RID: 3664 RVA: 0x00047D40 File Offset: 0x00045F40
	public override bool IsBlockingOperation()
	{
		return false;
	}

	// Token: 0x06000E51 RID: 3665 RVA: 0x00047D44 File Offset: 0x00045F44
	public override bool OperationConfirmed(TIGameState actor, TIGameState target, TIDateTime opCompleteDate)
	{
		if (actor.isFactionState)
		{
			string factionOperationCompleteName = actor.ref_faction.factionOperationCompleteName;
			TITimeEvent.CreateNewTimeEvent(opCompleteDate, actor.ref_faction, target, this, factionOperationCompleteName, true, false, TITimeQueueRepeatType.None, 1, true, false);
			GameControl.eventManager.TriggerEvent(new StartFactionOperation(actor, this, target), null, (from x in new object[] { actor, target }.Distinct<object>()
				where x != null
				select x).ToArray<object>());
			return true;
		}
		return false;
	}
}
