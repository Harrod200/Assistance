using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x020002EE RID: 750
public abstract class TIOperationTargeting_Orbit : TIOperationTargeting
{
	// Token: 0x06000B4B RID: 2891 RVA: 0x0003D7BF File Offset: 0x0003B9BF
	public override List<Type> TargetedGameStates()
	{
		return new List<Type> { typeof(TIOrbitState) };
	}

	// Token: 0x06000B4C RID: 2892 RVA: 0x0003D7D6 File Offset: 0x0003B9D6
	public override void Init(IOperation operationType, TIGameState actorState, TIGameState defaultTarget)
	{
		this.operationType = operationType;
		this.actorState = actorState;
		this.barycenter = ((defaultTarget != null) ? defaultTarget.ref_naturalSpaceObject : null) ?? actorState.ref_naturalSpaceObject;
		this.possibleTargets = operationType.GetPossibleTargets(actorState, defaultTarget);
	}

	// Token: 0x06000B4D RID: 2893 RVA: 0x0003D810 File Offset: 0x0003BA10
	public void OrbitSelectedForTargeting(OrbitSelectedEvent e)
	{
		if (this.possibleTargets.Contains(e.orbit))
		{
			this.currentTarget = e.orbit;
			GameControl.eventManager.TriggerEvent(new OperationTargettedEvent(this.currentTarget, this.actorState), null, Array.Empty<object>());
		}
	}

	// Token: 0x06000B4E RID: 2894 RVA: 0x0003D860 File Offset: 0x0003BA60
	public override void Activate(TIGameState forceTarget = null)
	{
		if (!base.activated)
		{
			base.SetActivation(this);
			base.SetDefaultTarget(forceTarget ?? this.GetDefaultTarget());
			GameControl.eventManager.TriggerEvent(new TargetOrbits(this.actorState, this.barycenter), null, Array.Empty<object>());
			GameControl.eventManager.AddListener<OrbitSelectedEvent>(new EventManager.EventDelegate<OrbitSelectedEvent>(this.OrbitSelectedForTargeting), null, null, true, false);
		}
	}

	// Token: 0x06000B4F RID: 2895 RVA: 0x0003D8C8 File Offset: 0x0003BAC8
	public override void Shutdown()
	{
		if (base.activated)
		{
			base.SetShutdown();
			GameControl.eventManager.TriggerEvent(new DeTargetOrbits(this.actorState.ref_faction), null, Array.Empty<object>());
			GameControl.eventManager.RemoveListener<OrbitSelectedEvent>(new EventManager.EventDelegate<OrbitSelectedEvent>(this.OrbitSelectedForTargeting), null);
		}
	}

	// Token: 0x06000B50 RID: 2896 RVA: 0x0003D91A File Offset: 0x0003BB1A
	public override TIGameState GetDefaultTarget()
	{
		return this.possibleTargets[0];
	}

	// Token: 0x04000E8D RID: 3725
	private TISpaceObjectState barycenter;
}
