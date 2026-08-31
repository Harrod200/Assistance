using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000302 RID: 770
public class DeployArmyOperation_TargetHome : DeployArmyOperation
{
	// Token: 0x06000C0B RID: 3083 RVA: 0x0003FB2C File Offset: 0x0003DD2C
	public override int SortOrder()
	{
		return 1;
	}

	// Token: 0x17000181 RID: 385
	// (get) Token: 0x06000C0C RID: 3084 RVA: 0x0003FB2F File Offset: 0x0003DD2F
	public override bool isConvenienceOperation
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06000C0D RID: 3085 RVA: 0x0003FB34 File Offset: 0x0003DD34
	public override bool OpVisibleToActor(TIGameState actorState, TIGameState targetState = null)
	{
		return base.OpVisibleToActor(actorState, targetState) && actorState.ref_army.currentRegion != actorState.ref_army.homeRegion && actorState.ref_army.CanGetTo(actorState.ref_army.homeRegion, null, null, null);
	}

	// Token: 0x06000C0E RID: 3086 RVA: 0x0003FB84 File Offset: 0x0003DD84
	public override bool ActorCanPerformOperation(TIGameState actorState, TIGameState target)
	{
		return actorState.ref_army.currentRegion != actorState.ref_army.homeRegion && actorState.ref_army.CanGetTo(actorState.ref_army.homeRegion, null, null, null) && base.ActorCanPerformOperation(actorState, target);
	}

	// Token: 0x06000C0F RID: 3087 RVA: 0x0003FBD3 File Offset: 0x0003DDD3
	public static List<TIGameState> GetPossibleTargets(TIGameState actorState, bool allowJournies)
	{
		if (!actorState.ref_army.CanGetTo(actorState.ref_army.homeRegion, null, null, null))
		{
			return new List<TIGameState>();
		}
		return new List<TIGameState>(1) { actorState.ref_army.homeRegion };
	}

	// Token: 0x06000C10 RID: 3088 RVA: 0x0003FC0D File Offset: 0x0003DE0D
	public override List<TIGameState> GetPossibleTargets(TIGameState actorState, TIGameState defaultTarget = null)
	{
		if (!actorState.ref_army.CanGetTo(actorState.ref_army.homeRegion, null, null, null))
		{
			return new List<TIGameState>();
		}
		return new List<TIGameState>(1) { actorState.ref_army.homeRegion };
	}

	// Token: 0x06000C11 RID: 3089 RVA: 0x0003FC47 File Offset: 0x0003DE47
	public DeployArmyOperation_TargetHome()
		: base(false)
	{
	}
}
