using System;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000D4 RID: 212
public class TIGlobalCondition_bWaveOfFearEligible : TIGlobalCondition
{
	// Token: 0x060003B8 RID: 952 RVA: 0x0001357A File Offset: 0x0001177A
	public override string GetDescriptionPath()
	{
		return base.GetDescriptionPathWithValue();
	}

	// Token: 0x060003B9 RID: 953 RVA: 0x00013584 File Offset: 0x00011784
	public override bool PassesCondition(TIGameState state)
	{
		bool flag;
		if (!GameStateManager.AlienFaction().councilors.Any<TICouncilorState>((TICouncilorState x) => x.OnEarth) && !GameStateManager.AlienNation().extant)
		{
			if (!GameStateManager.AlienFaction().habs.Any<TIHabState>((TIHabState x) => x.ref_naturalSpaceObject.inEarthSystem))
			{
				if (!GameStateManager.AlienFaction().fleets.Any<TISpaceFleetState>(delegate(TISpaceFleetState x)
				{
					TINaturalSpaceObjectState ref_naturalSpaceObject = x.ref_naturalSpaceObject;
					return ref_naturalSpaceObject != null && ref_naturalSpaceObject.inEarthSystem;
				}))
				{
					flag = GameStateManager.AlienFaction().fleets.Any<TISpaceFleetState>(delegate(TISpaceFleetState x)
					{
						if (x.transferAssigned)
						{
							TISpaceGameState destination = x.trajectory.destination;
							bool? flag3;
							if (destination == null)
							{
								flag3 = null;
							}
							else
							{
								TINaturalSpaceObjectState ref_naturalSpaceObject2 = destination.ref_naturalSpaceObject;
								flag3 = ((ref_naturalSpaceObject2 != null) ? new bool?(ref_naturalSpaceObject2.inEarthSystem) : null);
							}
							bool? flag4 = flag3;
							return flag4.GetValueOrDefault();
						}
						return false;
					});
					goto IL_00D3;
				}
			}
		}
		flag = true;
		IL_00D3:
		bool flag2 = flag;
		return TICondition.PassesComparison(this.sign, flag2, TIUtilities.GetBoolValue(this.strValue));
	}
}
