using System;
using System.Collections.Generic;
using System.Text;
using ModelShark;
using PavonisInteractive.TerraInvicta.Audio;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000867 RID: 2151
	public class HabCouncilorGridItemController : MonoBehaviour
	{
		// Token: 0x06004FBA RID: 20410 RVA: 0x00226E70 File Offset: 0x00225070
		public void UpdateGridItem(TIFactionState viewingFaction, TICouncilorState councilor)
		{
			this.heldCouncilor = councilor;
			this.heldFaction = viewingFaction;
			CouncilorView viewofCouncilor = viewingFaction.GetViewofCouncilor(councilor);
			TIFactionState factionCurrent = viewofCouncilor.factionCurrent;
			this.councilorIconBackground.color = ((factionCurrent == null) ? Color.clear : factionCurrent.template.color);
			GameControl.assetLoader.LoadAssetForImageAssignment(viewofCouncilor.mapIconResourcePathMemory, this.councilorIcon);
			this.councilorTooltip.SetText("BodyText", viewofCouncilor.displayNameMemory);
			this.councilorIconBackground.gameObject.SetActive(true);
		}

		// Token: 0x06004FBB RID: 20411 RVA: 0x00226F00 File Offset: 0x00225100
		public void UpdateGridItem(List<TIOfficerState> officers)
		{
			this.councilorIconBackground.gameObject.SetActive(false);
			GameControl.assetLoader.LoadAssetForImageAssignment(officers.MaxBy<TIOfficerState, int>((TIOfficerState x) => x.rank).GetIconPath(), this.councilorIcon);
			this.councilorTooltip.SetDelegate("BodyText", delegate
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (TIOfficerState tiofficerState in officers)
				{
					stringBuilder.AppendLine(tiofficerState.DisplayNameAndJob);
				}
				return stringBuilder.ToString();
			});
		}

		// Token: 0x06004FBC RID: 20412 RVA: 0x00226F88 File Offset: 0x00225188
		public void onClickHabCouncilorGridItem()
		{
			if (this.heldCouncilor != null)
			{
				if (this.heldCouncilor.faction == this.heldFaction)
				{
					SoundEffectController.PlaySelectSound(this.heldCouncilor);
				}
				TIUtilities.GotoGameState(this.heldCouncilor, true, true, true);
			}
		}

		// Token: 0x0400331C RID: 13084
		private TICouncilorState heldCouncilor;

		// Token: 0x0400331D RID: 13085
		private TIFactionState heldFaction;

		// Token: 0x0400331E RID: 13086
		public Image councilorIcon;

		// Token: 0x0400331F RID: 13087
		public TooltipTrigger councilorTooltip;

		// Token: 0x04003320 RID: 13088
		public Image councilorIconBackground;
	}
}
