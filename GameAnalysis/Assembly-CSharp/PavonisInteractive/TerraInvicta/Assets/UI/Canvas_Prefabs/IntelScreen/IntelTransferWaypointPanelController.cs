using System;
using TMPro;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta.Assets.UI.Canvas_Prefabs.IntelScreen
{
	// Token: 0x020009C3 RID: 2499
	internal class IntelTransferWaypointPanelController
	{
		// Token: 0x06005E21 RID: 24097 RVA: 0x002CC130 File Offset: 0x002CA330
		public void SetListItem(IPatchedTransferSegment segment)
		{
			this.spaceObjectIcon.sprite = segment.barycenter.icon;
			this.spaceObjectName.SetText(segment.barycenter.displayName);
			this.transferSegmentTrajectoryType.SetText("TODO");
			this.DVBurn.SetText(Loc.T("UI.Fleets.SingleDV", new object[] { TIUtilities.FormatBigOrSmallNumber(segment.DV_mps * 1000.0, 1, 7, 0, false, false) }));
			this.duration.SetText((segment.endTime - segment.startTime).ToString());
		}

		// Token: 0x04004344 RID: 17220
		public Image spaceObjectIcon;

		// Token: 0x04004345 RID: 17221
		public TMP_Text spaceObjectName;

		// Token: 0x04004346 RID: 17222
		public TMP_Text transferSegmentTrajectoryType;

		// Token: 0x04004347 RID: 17223
		public TMP_Text DVBurn;

		// Token: 0x04004348 RID: 17224
		public TMP_Text duration;
	}
}
