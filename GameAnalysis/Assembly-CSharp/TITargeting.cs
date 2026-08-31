using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200027B RID: 635
public abstract class TITargeting
{
	// Token: 0x17000105 RID: 261
	// (get) Token: 0x0600085B RID: 2139 RVA: 0x00027084 File Offset: 0x00025284
	// (set) Token: 0x0600085C RID: 2140 RVA: 0x0002708C File Offset: 0x0002528C
	public bool activated { get; protected set; }

	// Token: 0x0600085D RID: 2141
	public abstract List<Type> TargetedGameStates();

	// Token: 0x17000106 RID: 262
	// (get) Token: 0x0600085E RID: 2142 RVA: 0x00027095 File Offset: 0x00025295
	public IList<TIGameState> GetPossibleTargets
	{
		get
		{
			return this.possibleTargets;
		}
	}

	// Token: 0x17000107 RID: 263
	// (get) Token: 0x0600085F RID: 2143 RVA: 0x0002709D File Offset: 0x0002529D
	public virtual bool forceMap
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06000860 RID: 2144 RVA: 0x000270A0 File Offset: 0x000252A0
	private void CycleToTarget(TIGameState target)
	{
		TIUtilities.LookAtGameState(target);
		TIUtilities.GotoGameState(target, false, true, true, true, false, -1f);
	}

	// Token: 0x06000861 RID: 2145 RVA: 0x000270B8 File Offset: 0x000252B8
	public void CycleTargetForward()
	{
		if (this.possibleTargets == null || this.possibleTargets.Count == 0)
		{
			return;
		}
		TIGameState tigameState;
		if (this.currentTarget == null || this.currentTarget == this.possibleTargets.Last<TIGameState>())
		{
			tigameState = this.possibleTargets.First<TIGameState>();
		}
		else
		{
			int num = this.possibleTargets.IndexOf(this.currentTarget);
			tigameState = this.possibleTargets[num + 1];
		}
		this.CycleToTarget(tigameState);
	}

	// Token: 0x06000862 RID: 2146 RVA: 0x00027138 File Offset: 0x00025338
	public void CycleTargetBackward()
	{
		if (this.possibleTargets == null || this.possibleTargets.Count == 0)
		{
			return;
		}
		TIGameState tigameState;
		if (this.currentTarget == null || this.currentTarget == this.possibleTargets.First<TIGameState>())
		{
			tigameState = this.possibleTargets.Last<TIGameState>();
		}
		else
		{
			int num = this.possibleTargets.IndexOf(this.currentTarget);
			tigameState = this.possibleTargets[num - 1];
		}
		this.CycleToTarget(tigameState);
	}

	// Token: 0x0400063A RID: 1594
	protected TIGameState currentTarget;

	// Token: 0x0400063C RID: 1596
	protected IList<TIGameState> possibleTargets;
}
