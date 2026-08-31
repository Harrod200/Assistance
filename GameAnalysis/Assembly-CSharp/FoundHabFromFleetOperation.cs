using System;
using System.Collections.Generic;
using System.Text;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200030F RID: 783
public abstract class FoundHabFromFleetOperation : TISpaceFleetOperationTemplate_Special
{
	// Token: 0x06000CA6 RID: 3238 RVA: 0x0004127F File Offset: 0x0003F47F
	public override OperationTiming GetOperationTiming()
	{
		return OperationTiming.InstantExecution;
	}

	// Token: 0x06000CA7 RID: 3239 RVA: 0x00041282 File Offset: 0x0003F482
	public override bool IsBlockingOperation()
	{
		return true;
	}

	// Token: 0x06000CA8 RID: 3240
	public abstract TIHabModuleTemplate CoreModule(bool alien);

	// Token: 0x06000CA9 RID: 3241
	public abstract List<string> AdditionalModules(bool alien);

	// Token: 0x06000CAA RID: 3242 RVA: 0x00041285 File Offset: 0x0003F485
	public override float GetDuration_days(TIGameState actorState, TIGameState target, Trajectory trajectory = null)
	{
		return -1f;
	}

	// Token: 0x06000CAB RID: 3243 RVA: 0x0004128C File Offset: 0x0003F48C
	public override bool UpdatePropulsionOnComplete()
	{
		return false;
	}

	// Token: 0x06000CAC RID: 3244 RVA: 0x0004128F File Offset: 0x0003F48F
	public override bool MustAcceptCombat()
	{
		return true;
	}

	// Token: 0x06000CAD RID: 3245 RVA: 0x00041292 File Offset: 0x0003F492
	public override bool CancelUponCombat()
	{
		return true;
	}

	// Token: 0x06000CAE RID: 3246 RVA: 0x00041295 File Offset: 0x0003F495
	public override bool OpVisibleToActor(TIGameState actorState, TIGameState targetState = null)
	{
		return actorState.ref_fleet.ShipsWithSpecialModuleRule(this.RequiredCapability()).Count > 0;
	}

	// Token: 0x06000CAF RID: 3247 RVA: 0x000412B0 File Offset: 0x0003F4B0
	public override string GetDescription(TIGameState actorState = null, TIGameState target = null)
	{
		TIFactionState tifactionState = ((actorState != null) ? actorState.ref_faction : null);
		return new StringBuilder(Loc.T(new StringBuilder(base.GetType().Name).Append(".description").ToString(), new object[]
		{
			-this.CoreModule(tifactionState != null && tifactionState.IsAlienFaction).missionControl,
			TemplateManager.global.missionControlInlineSpritePath
		})).ToString();
	}

	// Token: 0x06000CB0 RID: 3248 RVA: 0x0004132B File Offset: 0x0003F52B
	public virtual int GetTier()
	{
		return 1;
	}
}
