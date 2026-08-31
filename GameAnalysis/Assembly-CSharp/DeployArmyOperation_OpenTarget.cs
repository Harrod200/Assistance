using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PavonisInteractive.TerraInvicta;

// Token: 0x020002FE RID: 766
public class DeployArmyOperation_OpenTarget : DeployArmyOperation
{
	// Token: 0x06000BE5 RID: 3045 RVA: 0x0003F56C File Offset: 0x0003D76C
	public DeployArmyOperation_OpenTarget(bool allowJournies_ = false)
		: base(false)
	{
		base.JourneyMode = allowJournies_;
	}

	// Token: 0x06000BE6 RID: 3046 RVA: 0x0003F57C File Offset: 0x0003D77C
	public static List<TIGameState> GetPossibleTargets(TIGameState actorState, bool allowJournies)
	{
		if (!actorState.isArmyState || actorState.ref_army.atSea)
		{
			return new List<TIGameState>();
		}
		TIArmyState ref_army = actorState.ref_army;
		if (allowJournies)
		{
			return ref_army.ReachableRegions.Cast<TIGameState>().ToList<TIGameState>();
		}
		return TIArmyState.OneStepValidDestinationRegions(ref_army, ref_army.currentRegion, ref_army.IsMoving).Cast<TIGameState>().ToList<TIGameState>();
	}

	// Token: 0x06000BE7 RID: 3047 RVA: 0x0003F5DB File Offset: 0x0003D7DB
	public override List<TIGameState> GetPossibleTargets(TIGameState actorState, TIGameState defaultTarget = null)
	{
		return DeployArmyOperation_OpenTarget.GetPossibleTargets(actorState, base.JourneyMode);
	}

	// Token: 0x06000BE8 RID: 3048 RVA: 0x0003F5EC File Offset: 0x0003D7EC
	public override string GetDescription(TIGameState actorState = null, TIGameState target = null)
	{
		OperationCanvasController singleton = OperationCanvasController.Singleton;
		bool flag;
		if (singleton == null)
		{
			flag = false;
		}
		else
		{
			List<TIArmyState> armyGroup = singleton.armyGroup;
			int? num = ((armyGroup != null) ? new int?(armyGroup.Count) : null);
			int num2 = 1;
			flag = (num.GetValueOrDefault() > num2) & (num != null);
		}
		if (flag)
		{
			return Loc.T(new StringBuilder(base.GetType().Name).Append(".description").Append(".group").ToString());
		}
		return Loc.T(new StringBuilder(base.GetType().Name).Append(".description").ToString());
	}
}
