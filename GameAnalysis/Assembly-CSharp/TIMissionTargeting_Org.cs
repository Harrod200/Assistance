using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200028B RID: 651
public class TIMissionTargeting_Org : TIMissionTargeting
{
	// Token: 0x060008CA RID: 2250 RVA: 0x0002929C File Offset: 0x0002749C
	public override List<Type> TargetedGameStates()
	{
		return new List<Type> { typeof(TIOrgState) };
	}

	// Token: 0x060008CB RID: 2251 RVA: 0x000292B4 File Offset: 0x000274B4
	public override void Activate()
	{
		if (!base.activated)
		{
			base.SetActivation(this);
			this.possibleTargets = base.missionTemplate.target.GetValidTargets(base.missionTemplate, this.councilor);
			base.SetDefaultTarget();
			EventManager eventManager = GameControl.eventManager;
			TICouncilorState councilor = this.councilor;
			TIMissionTemplate missionTemplate = base.missionTemplate;
			IList<TIGameState> possibleTargets = this.possibleTargets;
			TIGameState currentTarget = this.currentTarget;
			eventManager.TriggerEvent(new TargetOrgs(councilor, missionTemplate, possibleTargets, (currentTarget != null) ? currentTarget.ref_org : null), null, Array.Empty<object>());
			GameControl.eventManager.AddListener<OrgSelectedEvent>(new EventManager.EventDelegate<OrgSelectedEvent>(this.OrgSelectedForTargeting), null, null, true, false);
		}
	}

	// Token: 0x060008CC RID: 2252 RVA: 0x0002934E File Offset: 0x0002754E
	public override void Shutdown()
	{
		if (base.activated)
		{
			base.SetShutdown();
			GameControl.eventManager.TriggerEvent(new DeTargetOrgs(), null, Array.Empty<object>());
			GameControl.eventManager.RemoveListener<OrgSelectedEvent>(new EventManager.EventDelegate<OrgSelectedEvent>(this.OrgSelectedForTargeting), null);
		}
	}

	// Token: 0x060008CD RID: 2253 RVA: 0x0002938A File Offset: 0x0002758A
	public void OrgSelectedForTargeting(OrgSelectedEvent e)
	{
		base.SetTarget(e.org);
	}
}
