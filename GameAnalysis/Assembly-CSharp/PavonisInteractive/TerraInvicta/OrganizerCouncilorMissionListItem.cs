using System;
using ModelShark;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200083D RID: 2109
	public class OrganizerCouncilorMissionListItem : MonoBehaviour
	{
		// Token: 0x06004C65 RID: 19557 RVA: 0x00204578 File Offset: 0x00202778
		public void SetListItem(TIMissionTemplate missionTemplate)
		{
			GameControl.assetLoader.LoadAssetForImageAssignment(missionTemplate.missionIconImagePath_Off, this.missionIcon);
			this.missionTooltip.SetDelegate("BodyText", () => missionTemplate.multiLineDescriptionWithModifiers);
		}

		// Token: 0x04002E54 RID: 11860
		public Image missionIcon;

		// Token: 0x04002E55 RID: 11861
		public TooltipTrigger missionTooltip;
	}
}
