using System;
using System.Linq;
using System.Text;
using ModelShark;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000432 RID: 1074
internal class TransferOfficerAssetListItemController : MonoBehaviour
{
	// Token: 0x17000332 RID: 818
	// (get) Token: 0x0600163C RID: 5692 RVA: 0x0007166B File Offset: 0x0006F86B
	// (set) Token: 0x0600163D RID: 5693 RVA: 0x00071673 File Offset: 0x0006F873
	public OfficerCarrierState state { get; private set; }

	// Token: 0x0600163E RID: 5694 RVA: 0x0007167C File Offset: 0x0006F87C
	private string OfficerTip(OfficerCarrierState state)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (TIOfficerState tiofficerState in from x in state.GetOfficers()
			orderby x.template.sortOrder
			select x)
		{
			stringBuilder.AppendLine(tiofficerState.DisplayNameAndJob);
		}
		return stringBuilder.ToString();
	}

	// Token: 0x0600163F RID: 5695 RVA: 0x00071700 File Offset: 0x0006F900
	public void SetListItem(OfficerCarrierState state, bool giver, OperationCanvasController controller)
	{
		if (this.selectedAssetButtonDefaultSprite == null)
		{
			this.selectedAssetButtonDefaultSprite = this.selectAssetButton.image.sprite;
		}
		this.giver = giver;
		this.state = state;
		this.controller = controller;
		if (state.GetState().isHabState)
		{
			TIHabState ref_hab = state.GetState().ref_hab;
			this.assetName.SetText(ref_hab.displayName);
			this.assetDescription.SetText(ref_hab.description);
			this.fleetIcon.gameObject.SetActive(false);
		}
		else
		{
			TISpaceShipState ship = state.GetState().ref_ship;
			this.assetName.SetText(ship.NameWithDamageIcons());
			this.assetDescription.SetText(ship.template.fullClassName);
			this.fleetIcon.SetGridItem_Alt(ship, () => ship.template.quickSummary(false, ship, false, true, true), true);
			this.fleetIcon.gameObject.SetActive(true);
		}
		int count = state.GetOfficers().Count;
		for (int i = 0; i < 12; i++)
		{
			if (count > i)
			{
				GameControl.assetLoader.LoadAssetForImageAssignment(state.GetOfficers()[i].GetIconPath(), this.officerIcons[i]);
				this.officerIcons[i].gameObject.SetActive(true);
			}
			else
			{
				this.officerIcons[i].gameObject.SetActive(false);
			}
		}
		this.officerListTip.SetDelegate("BodyText", () => this.OfficerTip(state));
		this.HighlightButtonAfterSelection();
	}

	// Token: 0x06001640 RID: 5696 RVA: 0x000718D0 File Offset: 0x0006FAD0
	public void OnShipClicked()
	{
		if (this.giver)
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			this.controller.SetSelectedGiver(this.state);
			return;
		}
		AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleBack", false, false);
		this.controller.SetSelectedReciever(this.state);
	}

	// Token: 0x06001641 RID: 5697 RVA: 0x00071920 File Offset: 0x0006FB20
	public void HighlightButtonAfterSelection()
	{
		if (this.giver)
		{
			this.selectAssetButton.image.sprite = ((this.state == this.controller.selectedOfficerGiver) ? this.selectAssetButton.spriteState.highlightedSprite : this.selectedAssetButtonDefaultSprite);
			return;
		}
		this.selectAssetButton.image.sprite = ((this.state == this.controller.selectedOfficerReceiver) ? this.selectAssetButton.spriteState.highlightedSprite : this.selectedAssetButtonDefaultSprite);
	}

	// Token: 0x0400149D RID: 5277
	public TMP_Text assetName;

	// Token: 0x0400149E RID: 5278
	public TMP_Text assetDescription;

	// Token: 0x0400149F RID: 5279
	public TooltipTrigger officerListTip;

	// Token: 0x040014A0 RID: 5280
	public Image[] officerIcons;

	// Token: 0x040014A1 RID: 5281
	public Button selectAssetButton;

	// Token: 0x040014A2 RID: 5282
	private Sprite selectedAssetButtonDefaultSprite;

	// Token: 0x040014A4 RID: 5284
	private OperationCanvasController controller;

	// Token: 0x040014A5 RID: 5285
	private bool giver;

	// Token: 0x040014A6 RID: 5286
	public FleetShipGridItemController fleetIcon;
}
