using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000288 RID: 648
public class TIMissionTargeting_Hab : TIMissionTargeting
{
	// Token: 0x060008B5 RID: 2229 RVA: 0x00028AD9 File Offset: 0x00026CD9
	public override List<Type> TargetedGameStates()
	{
		return new List<Type> { typeof(TIHabState) };
	}

	// Token: 0x060008B6 RID: 2230 RVA: 0x00028AF0 File Offset: 0x00026CF0
	public override void Activate()
	{
		if (!base.activated)
		{
			base.SetActivation(this);
			this.possibleTargets = base.missionTemplate.target.GetValidTargets(base.missionTemplate, this.councilor);
			base.SetDefaultTarget();
			GameControl.eventManager.AddListener<HabSelectedEvent>(new EventManager.EventDelegate<HabSelectedEvent>(this.HabSelectedForTargeting), null, null, true, false);
		}
	}

	// Token: 0x060008B7 RID: 2231 RVA: 0x00028B4E File Offset: 0x00026D4E
	public override void Shutdown()
	{
		if (base.activated)
		{
			base.SetShutdown();
			GameControl.eventManager.RemoveListener<HabSelectedEvent>(new EventManager.EventDelegate<HabSelectedEvent>(this.HabSelectedForTargeting), null);
		}
	}

	// Token: 0x060008B8 RID: 2232 RVA: 0x00028B75 File Offset: 0x00026D75
	public override TIGameState GetDefaultTarget()
	{
		if (this.councilor.ref_hab != null)
		{
			return this.councilor.ref_hab;
		}
		return base.GetDefaultTarget();
	}

	// Token: 0x060008B9 RID: 2233 RVA: 0x00028B9C File Offset: 0x00026D9C
	private void HabSelectedForTargeting(HabSelectedEvent e)
	{
		base.SetTarget(e.hab);
	}
}
