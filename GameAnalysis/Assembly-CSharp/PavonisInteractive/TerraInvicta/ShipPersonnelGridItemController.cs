using System;
using System.Text;
using ModelShark;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000859 RID: 2137
	internal class ShipPersonnelGridItemController : MonoBehaviour
	{
		// Token: 0x06004E63 RID: 20067 RVA: 0x0021BA14 File Offset: 0x00219C14
		public void UpdateGridItem(TIOfficerState officer, bool dead = false)
		{
			GameControl.assetLoader.LoadAssetForImageAssignment(officer.template.GetIconPath(officer.rank), this.icon);
			this.tooltip.SetDelegate("BodyText", () => this.OfficerTip(officer));
			this.kiaIcon.enabled = dead;
		}

		// Token: 0x06004E64 RID: 20068 RVA: 0x0021BA88 File Offset: 0x00219C88
		public void UpdateGridItem(CouncilorView councilorView)
		{
			GameControl.assetLoader.LoadAssetForImageAssignment(councilorView.mapIconResourcePathCurrent, this.icon);
			this.tooltip.SetDelegate("BodyText", () => this.CouncilorTip(councilorView));
			this.kiaIcon.enabled = false;
		}

		// Token: 0x06004E65 RID: 20069 RVA: 0x0021BAEC File Offset: 0x00219CEC
		private string OfficerTip(TIOfficerState officer)
		{
			if (this.kiaIcon.enabled)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine(officer.DisplayNameAndJob);
				stringBuilder.AppendLine(Loc.T("TIOfficerTemplate.KilledInAction"));
				stringBuilder.AppendLine(officer.template.description);
				stringBuilder.Append(officer.template.EffectsAtRankString(officer.rank));
				return stringBuilder.ToString();
			}
			return officer.FullDescription;
		}

		// Token: 0x06004E66 RID: 20070 RVA: 0x0021BB5F File Offset: 0x00219D5F
		private string CouncilorTip(CouncilorView councilorView)
		{
			return councilorView.councilor.displayName;
		}

		// Token: 0x040031FD RID: 12797
		public Image icon;

		// Token: 0x040031FE RID: 12798
		public Image kiaIcon;

		// Token: 0x040031FF RID: 12799
		public TooltipTrigger tooltip;
	}
}
