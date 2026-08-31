using System;
using ModelShark;
using PavonisInteractive.TerraInvicta.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020008D5 RID: 2261
	public class MoonsListItemController : MonoBehaviour
	{
		// Token: 0x0600569C RID: 22172 RVA: 0x0027A0FC File Offset: 0x002782FC
		public void SetListItem(TISpaceBodyState bodyState)
		{
			this.moon = bodyState;
			this.moonName.SetText(bodyState.displayName);
			this.moonImage.sprite = this.moon.icon;
			if (bodyState.habSites.Length != 0)
			{
				this.moonDescription.SetText(bodyState.habSites[0].miningProfile.displayName);
				this.sitesText.SetText(Loc.T("UI.Space.Sites"), (float)bodyState.surfaceBases.Count, (float)bodyState.habSites.Length);
			}
			else
			{
				this.moonDescription.SetText(Loc.T("UI.Space.NoHabs"));
				this.sitesText.enabled = false;
			}
			this.tip.SetDelegate("BodyText", () => this.moon.SummaryTooltip(GameControl.control.activePlayer));
		}

		// Token: 0x0600569D RID: 22173 RVA: 0x0027A1C6 File Offset: 0x002783C6
		public void OnMoonButtonPressed()
		{
			SoundEffectController.PlaySelectSound(this.moon);
			TIUtilities.GotoGameState(this.moon, true, true, true, true, false, -1f);
		}

		// Token: 0x04003DBD RID: 15805
		private TISpaceBodyState moon;

		// Token: 0x04003DBE RID: 15806
		public TMP_Text moonName;

		// Token: 0x04003DBF RID: 15807
		public Image moonImage;

		// Token: 0x04003DC0 RID: 15808
		public TMP_Text moonDescription;

		// Token: 0x04003DC1 RID: 15809
		public TMP_Text sitesText;

		// Token: 0x04003DC2 RID: 15810
		public TooltipTrigger tip;
	}
}
