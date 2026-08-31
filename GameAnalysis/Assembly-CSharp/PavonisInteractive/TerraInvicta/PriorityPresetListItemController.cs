using System;
using ModelShark;
using PavonisInteractive.TerraInvicta.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200089E RID: 2206
	public class PriorityPresetListItemController : MonoBehaviour
	{
		// Token: 0x17000EF2 RID: 3826
		// (get) Token: 0x06005339 RID: 21305 RVA: 0x00250C52 File Offset: 0x0024EE52
		// (set) Token: 0x0600533A RID: 21306 RVA: 0x00250C5A File Offset: 0x0024EE5A
		public PriorityType priority { get; private set; }

		// Token: 0x0600533B RID: 21307 RVA: 0x00250C64 File Offset: 0x0024EE64
		public void Init(NationInfoController controller, PriorityType priority)
		{
			this.controller = controller;
			this.priority = priority;
			this.tooltip.SetDelegate("BodyText", () => NationInfoController.GenericPriorityTipStr(priority));
		}

		// Token: 0x0600533C RID: 21308 RVA: 0x00250CB0 File Offset: 0x0024EEB0
		public void UpdateListItem(int proposedValue, int totalWeights)
		{
			string text = TIUtilities.GetPriorityString(this.priority, true);
			if (!TIGlobalValuesState.CanAnyHumanNationUsePriority(this.priority))
			{
				text = TIUtilities.RedLine(text);
			}
			this.priorityName.SetText(text);
			this.setting.sprite = NationInfoController.weightSprite[proposedValue];
			this.percentageDetail.SetText(((float)proposedValue / (float)totalWeights).ToPercent("P0"));
		}

		// Token: 0x0600533D RID: 21309 RVA: 0x00250D16 File Offset: 0x0024EF16
		public void OnLeftClickPreset()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			this.controller.ChangePresetValue(this.priority, 1);
		}

		// Token: 0x0600533E RID: 21310 RVA: 0x00250D36 File Offset: 0x0024EF36
		public void OnRightClickPreset()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleBack", false, false);
			this.controller.ChangePresetValue(this.priority, -1);
		}

		// Token: 0x040038B3 RID: 14515
		private NationInfoController controller;

		// Token: 0x040038B5 RID: 14517
		public TMP_Text priorityName;

		// Token: 0x040038B6 RID: 14518
		public Image setting;

		// Token: 0x040038B7 RID: 14519
		public TooltipTrigger tooltip;

		// Token: 0x040038B8 RID: 14520
		public TMP_Text percentageDetail;
	}
}
