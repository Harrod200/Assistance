using System;
using System.Collections.Generic;
using System.Linq;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200066E RID: 1646
	public class CouncilorMapItemSelected : GameEvent
	{
		// Token: 0x06002895 RID: 10389 RVA: 0x000DA6C0 File Offset: 0x000D88C0
		public CouncilorMapItemSelected(TICouncilorState councilor)
		{
			this.councilor = councilor;
		}

		// Token: 0x06002896 RID: 10390 RVA: 0x000DA6D0 File Offset: 0x000D88D0
		public static object[] MakeSourceObjects(TICouncilorState target)
		{
			List<object> list = new List<object> { target.ref_region };
			if (TIMissionPhaseState.InMissionPhase())
			{
				List<object> list2 = list;
				TIGameState preMissionPhaseLocation = target.preMissionPhaseLocation;
				list2.Add((preMissionPhaseLocation != null) ? preMissionPhaseLocation.ref_region : null);
				if (GeneralControlsController.UITargetedState != null)
				{
					TICouncilorState ticouncilorState = GeneralControlsController.UITargetedState as TICouncilorState;
					if (ticouncilorState != null)
					{
						list.Add(ticouncilorState.ref_region);
						List<object> list3 = list;
						TIGameState preMissionPhaseLocation2 = ticouncilorState.preMissionPhaseLocation;
						list3.Add((preMissionPhaseLocation2 != null) ? preMissionPhaseLocation2.ref_region : null);
					}
				}
			}
			return (from x in list.Distinct<object>()
				where x != null
				select x).ToArray<object>();
		}

		// Token: 0x04001ECC RID: 7884
		public TICouncilorState councilor;
	}
}
