using System;
using System.Text;
using PavonisInteractive.TerraInvicta;

// Token: 0x020001F7 RID: 503
public abstract class TIMissionModifier : IMissionModifier
{
	// Token: 0x060006E2 RID: 1762
	public abstract float GetModifier(TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0f, FactionResource resource = FactionResource.None);

	// Token: 0x060006E3 RID: 1763 RVA: 0x00021B3C File Offset: 0x0001FD3C
	public static float CouncilCollectiveDefense(TIFactionState councilState, CouncilorAttribute attribute)
	{
		if (councilState == null)
		{
			return 0f;
		}
		return councilState.GetAggregateStat(attribute, false, null);
	}

	// Token: 0x170000EE RID: 238
	// (get) Token: 0x060006E4 RID: 1764 RVA: 0x00021B50 File Offset: 0x0001FD50
	public virtual string displayName
	{
		get
		{
			return Loc.T(new StringBuilder(base.GetType().Name).Append(".displayName").ToString());
		}
	}

	// Token: 0x060006E5 RID: 1765 RVA: 0x00021B76 File Offset: 0x0001FD76
	protected static TINationState ObjectToNation(TIFactionState viewingFaction, TIGameState state)
	{
		if (state.isCouncilorState && state.ref_councilor.OnEarth)
		{
			TIGameState tigameState = TIMissionPhaseState.CouncilorLastKnownLocation(viewingFaction, state.ref_councilor);
			return ((tigameState != null) ? tigameState.ref_nation : null) ?? null;
		}
		return state.ref_nation;
	}
}
