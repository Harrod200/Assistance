using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000805 RID: 2053
	public class FactionToggleListItemController : MonoBehaviour
	{
		// Token: 0x06004A64 RID: 19044 RVA: 0x001F31C8 File Offset: 0x001F13C8
		public void Init(TIFactionTemplate template, TIFactionTemplate selectedPlayerFaction, StartMenuController controller)
		{
			this.controller = controller;
			this.faction = template;
			this.factionNameText.SetText(this.faction.capitalizedFactionNameCurrent);
			this.factionToggle.interactable = !this.faction.isAlien && this.faction.dataName != "SubmitCouncil" && this.faction != selectedPlayerFaction;
			this.factionToggle.SetIsOnWithoutNotify(true);
		}

		// Token: 0x06004A65 RID: 19045 RVA: 0x001F3244 File Offset: 0x001F1444
		public void UpdateItem(TIFactionTemplate currentSelectedFaction)
		{
			if (this.faction == currentSelectedFaction)
			{
				this.factionToggle.isOn = true;
				this.factionToggle.interactable = false;
				return;
			}
			if (!this.faction.isAlien && this.faction.dataName != "SubmitCouncil")
			{
				this.factionToggle.interactable = true;
			}
		}

		// Token: 0x06004A66 RID: 19046 RVA: 0x001F32A4 File Offset: 0x001F14A4
		public void UpdateForDefaultFactions(List<TIFactionTemplate> factionsInScenario)
		{
			List<string> list = new List<string>();
			foreach (TIFactionTemplate tifactionTemplate in factionsInScenario)
			{
				list.Add(tifactionTemplate.dataName);
			}
			this.factionToggle.SetIsOnWithoutNotify(list.Contains(this.faction.dataName));
		}

		// Token: 0x06004A67 RID: 19047 RVA: 0x001F331C File Offset: 0x001F151C
		public void OnUpdateToggle()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			this.controller.ValidateFactionRequirements();
		}

		// Token: 0x04002B47 RID: 11079
		public TMP_Text factionNameText;

		// Token: 0x04002B48 RID: 11080
		public Toggle factionToggle;

		// Token: 0x04002B49 RID: 11081
		public TIFactionTemplate faction;

		// Token: 0x04002B4A RID: 11082
		private StartMenuController controller;
	}
}
