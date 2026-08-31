using System;
using System.Collections.Generic;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020005B1 RID: 1457
	public interface IOrbitalTransfer
	{
		// Token: 0x06002783 RID: 10115
		TIDateTime GetTransferStartTime();

		// Token: 0x06002784 RID: 10116
		TIDateTime GetTransferEndTime();

		// Token: 0x06002785 RID: 10117
		OrbitalElementsState GetPostTransferOrbitalElements();

		// Token: 0x06002786 RID: 10118
		double GetDeltaVToExecute();

		// Token: 0x06002787 RID: 10119
		double GetDesiredOrbitAltitude();

		// Token: 0x06002788 RID: 10120
		TINaturalSpaceObjectState GetBarycenter();

		// Token: 0x06002789 RID: 10121
		CartesianState ToCartesianStateAtTime(TIDateTime time);

		// Token: 0x0600278A RID: 10122
		List<Vector3d> GetOrbitTrailForTransfer(int numSegments = 90);

		// Token: 0x0600278B RID: 10123
		List<Vector3d> GetOrbitTrailDuringTransfer(TIDateTime currentTime, int numSegments = 90);
	}
}
