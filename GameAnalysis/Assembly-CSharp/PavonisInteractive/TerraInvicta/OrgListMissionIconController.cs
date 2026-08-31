using System;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000846 RID: 2118
	public class OrgListMissionIconController : MonoBehaviour
	{
		// Token: 0x06004CFA RID: 19706 RVA: 0x0020B47C File Offset: 0x0020967C
		public void SetListItem(TIMissionTemplate mission)
		{
			GameControl.assetLoader.LoadAssetForImageAssignment(mission.missionIconImagePath_Off, this.icon);
		}

		// Token: 0x04002F46 RID: 12102
		public Image icon;
	}
}
