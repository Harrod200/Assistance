using System;
using System.Collections.Generic;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.TIVirtualFleetState
{
	// Token: 0x02000960 RID: 2400
	public interface IMobileAsset : ITransferTarget
	{
		// Token: 0x17000F6A RID: 3946
		// (get) Token: 0x06005B4A RID: 23370
		TIOrbitState ref_orbit { get; }

		// Token: 0x17000F6B RID: 3947
		// (get) Token: 0x06005B4B RID: 23371
		float cruiseAcceleration_mps2 { get; }

		// Token: 0x17000F6C RID: 3948
		// (get) Token: 0x06005B4C RID: 23372
		float currentDeltaV_mps { get; }

		// Token: 0x06005B4D RID: 23373
		TINaturalSpaceObjectState FindCommonBarycenter(TIGameState secondSpaceObject);

		// Token: 0x06005B4E RID: 23374
		void SetAccelerationPhaseStatus(bool inPhase, bool forceRotation = false, bool forceStop = false);

		// Token: 0x06005B4F RID: 23375
		void SetDecelerationPhaseStatus(bool inPhase, bool forceRotation = false, bool forceStop = false);

		// Token: 0x06005B50 RID: 23376
		double meanAnomaly_Rad(TIDateTime time);

		// Token: 0x17000F6D RID: 3949
		// (get) Token: 0x06005B51 RID: 23377
		TIDateTime epoch_DateTime { get; }

		// Token: 0x17000F6E RID: 3950
		// (get) Token: 0x06005B52 RID: 23378
		TIFactionState faction { get; }

		// Token: 0x06005B53 RID: 23379
		Vector3d GetGlobalPositionAtTime(TIDateTime time);

		// Token: 0x06005B54 RID: 23380
		CartesianState ToGlobalCartesianStateAtTime(TIDateTime time);

		// Token: 0x17000F6F RID: 3951
		// (get) Token: 0x06005B55 RID: 23381
		// (set) Token: 0x06005B56 RID: 23382
		FleetTrajectoryData fleetTrajectoryData { get; set; }

		// Token: 0x17000F70 RID: 3952
		// (get) Token: 0x06005B57 RID: 23383
		List<TISpaceShipState> ships { get; }

		// Token: 0x17000F71 RID: 3953
		// (get) Token: 0x06005B58 RID: 23384
		bool transferAssigned { get; }
	}
}
