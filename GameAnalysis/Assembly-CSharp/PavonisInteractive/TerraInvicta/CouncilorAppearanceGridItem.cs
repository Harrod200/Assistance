using System;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000832 RID: 2098
	public class CouncilorAppearanceGridItem : MonoBehaviour
	{
		// Token: 0x06004C0A RID: 19466 RVA: 0x001FF5FC File Offset: 0x001FD7FC
		public void SetListItem(CouncilGridController controller, TICouncilorAppearanceTemplate template, bool old)
		{
			this.controller = controller;
			this.template = template;
			this.old = old;
			string text = (old ? template.portraitOld : template.portraitYoung);
			if (text == string.Empty)
			{
				Log.Info(template.dataName, Array.Empty<object>());
			}
			GameControl.assetLoader.LoadAssetForImageAssignment(text, this.councilorImage);
		}

		// Token: 0x06004C0B RID: 19467 RVA: 0x001FF65E File Offset: 0x001FD85E
		public void OnCouncilorImageSelected()
		{
			this.controller.OnNewAppearanceSelected(this.template);
		}

		// Token: 0x04002D72 RID: 11634
		private CouncilGridController controller;

		// Token: 0x04002D73 RID: 11635
		[HideInInspector]
		public TICouncilorAppearanceTemplate template;

		// Token: 0x04002D74 RID: 11636
		[HideInInspector]
		public bool old;

		// Token: 0x04002D75 RID: 11637
		public Image councilorImage;
	}
}
