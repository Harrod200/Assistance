using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x020001BC RID: 444
public class TIMissionCondition_HumanFactionsPermitControl : TIMissionCondition
{
	// Token: 0x170000EC RID: 236
	// (get) Token: 0x06000656 RID: 1622 RVA: 0x0001CC2A File Offset: 0x0001AE2A
	public override List<string> feedback
	{
		get
		{
			return new List<string> { "TIMissionCondition_HumanFactionsPermitControl", "TIMissionCondition_HumanFactionsPermitControl2" };
		}
	}

	// Token: 0x06000657 RID: 1623 RVA: 0x0001CC48 File Offset: 0x0001AE48
	public override string CanTarget(TICouncilorState councilor, TIGameState possibleTarget)
	{
		if (GameStateManager.AlienFaction().councilors.None<TICouncilorState>((TICouncilorState x) => x.OnOrAroundEarth) && !TIEffectsState.CheckForAnyEffectInContext(Context.ManyAliensOnEarth, GameStateManager.AlienFaction()))
		{
			return this.feedback[1];
		}
		TINationState ref_nation = possibleTarget.ref_nation;
		using (List<TIControlPoint>.Enumerator enumerator = ref_nation.controlPoints.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (!enumerator.Current.owned)
				{
					return this.feedback[0];
				}
			}
		}
		if (councilor.faction != ref_nation.executiveFaction)
		{
			return this.feedback[0];
		}
		foreach (TIFactionState tifactionState in ref_nation.FactionsWithControlPoint)
		{
			float num = TIEffectsState.SumEffectsModifiers(Context.CanTransferTerritoryToAliens, tifactionState, 0f, null);
			if ((float)ref_nation.CountFactionControlPoints(tifactionState, true, false, true) > num)
			{
				return this.feedback[0];
			}
		}
		return "_Pass";
	}
}
