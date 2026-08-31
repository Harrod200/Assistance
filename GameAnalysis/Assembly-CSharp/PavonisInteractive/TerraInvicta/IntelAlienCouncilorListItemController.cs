using System;
using PavonisInteractive.TerraInvicta.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000874 RID: 2164
	public class IntelAlienCouncilorListItemController : MonoBehaviour
	{
		// Token: 0x06005105 RID: 20741 RVA: 0x00236CE8 File Offset: 0x00234EE8
		public void UpdateListItem(CouncilorView councilor)
		{
			this.councilor = councilor;
			if (councilor.portraitPath != null)
			{
				GameControl.assetLoader.LoadAssetForImageAssignment(councilor.portraitPath, this.councilorImage);
				this.councilorImage.enabled = true;
			}
			else
			{
				this.councilorImage.enabled = false;
			}
			this.councilorName.SetText(councilor.displayNameMemory);
			this.councilorLocation.SetText(councilor.associatedLocationString);
			base.gameObject.SetActive(true);
		}

		// Token: 0x06005106 RID: 20742 RVA: 0x00236D68 File Offset: 0x00234F68
		public void OnClick()
		{
			if (GameControl.control.activePlayer.HasIntelOnCouncilorBasicData(this.councilor.councilor))
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Acknowledge", false, false);
				TIUtilities.GotoGameState(this.councilor, true, false, true, true);
				return;
			}
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
		}

		// Token: 0x040034D5 RID: 13525
		public Image councilorImage;

		// Token: 0x040034D6 RID: 13526
		public TMP_Text councilorName;

		// Token: 0x040034D7 RID: 13527
		public TMP_Text councilorLocation;

		// Token: 0x040034D8 RID: 13528
		private CouncilorView councilor;
	}
}
