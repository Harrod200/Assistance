using System;
using System.Linq;
using System.Text;
using ModelShark;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000433 RID: 1075
internal class TransferOfficerListItemController : MonoBehaviour
{
	// Token: 0x17000333 RID: 819
	// (get) Token: 0x06001643 RID: 5699 RVA: 0x000719BA File Offset: 0x0006FBBA
	// (set) Token: 0x06001644 RID: 5700 RVA: 0x000719C2 File Offset: 0x0006FBC2
	public TIOfficerState officer { get; private set; }

	// Token: 0x06001645 RID: 5701 RVA: 0x000719CC File Offset: 0x0006FBCC
	private string OfficerTip(TIOfficerState officer, int rankToShow, OfficerCarrierState otherAsset)
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (otherAsset != null && otherAsset.GetState().isSpaceShipState && !officer.CanTransferOfficer(officer.OfficerCarrier, otherAsset, false, false, 0))
		{
			StringBuilder stringBuilder2 = stringBuilder;
			TIOfficerTemplate template = officer.template;
			TISpaceShipState ship = officer.ship;
			stringBuilder2.AppendLine(template.FullDescriptionAtRank(rankToShow, (ship != null) ? ship.hull : null, true, (from x in officer.OfficerAllowedForShipFail(otherAsset.GetState().ref_ship, false, 0)
				select x.requirement).ToList<OfficerRequirementType>()));
		}
		else
		{
			StringBuilder stringBuilder3 = stringBuilder;
			TIOfficerTemplate template2 = officer.template;
			TISpaceShipState ship2 = officer.ship;
			stringBuilder3.AppendLine(template2.FullDescriptionAtRank(rankToShow, (ship2 != null) ? ship2.hull : null, true, null));
		}
		return stringBuilder.ToString();
	}

	// Token: 0x06001646 RID: 5702 RVA: 0x00071A98 File Offset: 0x0006FC98
	public void SetListItem(TIOfficerState officer, OperationCanvasController controller, bool giver, bool initialHide, OfficerCarrierState asset)
	{
		this.officer = officer;
		this.controller = controller;
		this.giver = giver;
		GameControl.assetLoader.LoadAssetForImageAssignment(officer.GetIconPath(), this.officerIcon);
		this.officerName.SetText(officer.displayName);
		int rankToShow = officer.rank;
		if (initialHide)
		{
			base.gameObject.SetActive(false);
			this.officerDescription.SetText(new StringBuilder(TIOfficerState.RankStarsInline(rankToShow)).Append(officer.template.displayName));
		}
		else
		{
			base.gameObject.SetActive(true);
		}
		this.officerEffects.SetDelegate("BodyText", () => this.OfficerTip(officer, rankToShow, giver ? controller.selectedOfficerReceiver : controller.selectedOfficerGiver));
		this.officerDescription.SetText(new StringBuilder(TIOfficerState.RankStarsInline(rankToShow)).Append(officer.template.displayName));
		this.Colorize(Color.white);
	}

	// Token: 0x06001647 RID: 5703 RVA: 0x00071BD2 File Offset: 0x0006FDD2
	public void OnOfficerButtonPressed()
	{
		AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
		this.controller.ProposeOfficerTransfer(this.officer, this.giver);
	}

	// Token: 0x06001648 RID: 5704 RVA: 0x00071BF7 File Offset: 0x0006FDF7
	public void Colorize(Color color)
	{
		this.buttonBackground.color = color;
	}

	// Token: 0x040014A7 RID: 5287
	public Image officerIcon;

	// Token: 0x040014A8 RID: 5288
	public TMP_Text officerName;

	// Token: 0x040014A9 RID: 5289
	public TMP_Text officerDescription;

	// Token: 0x040014AA RID: 5290
	public TooltipTrigger officerEffects;

	// Token: 0x040014AB RID: 5291
	public Button transferOfficerButton;

	// Token: 0x040014AC RID: 5292
	public Image buttonBackground;

	// Token: 0x040014AE RID: 5294
	private bool giver;

	// Token: 0x040014AF RID: 5295
	private OperationCanvasController controller;
}
