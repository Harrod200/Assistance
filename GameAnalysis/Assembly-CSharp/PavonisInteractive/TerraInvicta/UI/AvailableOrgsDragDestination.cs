using System;
using UnityEngine.EventSystems;

namespace PavonisInteractive.TerraInvicta.UI
{
	// Token: 0x0200091D RID: 2333
	public class AvailableOrgsDragDestination : DragDestination
	{
		// Token: 0x0600592F RID: 22831 RVA: 0x0028F0AC File Offset: 0x0028D2AC
		public override void SetControllerBase(CanvasControllerBase canvasControllerBase)
		{
			this._councilorController = (CouncilGridController)canvasControllerBase;
		}

		// Token: 0x06005930 RID: 22832 RVA: 0x0028F0BC File Offset: 0x0028D2BC
		public override void OnDrop(PointerEventData eventData)
		{
			if (!base.gameObject.activeInHierarchy || !DragManager.canDropCurrentItem)
			{
				return;
			}
			DragItem currentItem = DragManager.currentItem;
			if (currentItem == null)
			{
				return;
			}
			if (!this.organizer)
			{
				this._councilorController.StartSellOrg(currentItem.GetComponent<OrgItemView>().GetOrg(), false);
			}
			else
			{
				TIOrgState org = DragManager.currentItem.GetComponent<OrganizerOrgListItem>().org;
				if (this._councilorController.tempFactionCouncilorOrgs.ContainsKey(org))
				{
					this._councilorController.tempFactionCouncilorOrgs.Remove(org);
				}
				else
				{
					this._councilorController.tempFactionOrgs.Remove(org);
				}
				this._councilorController.tempMarketPoolOrgs.Add(org);
				this._councilorController.UpdateOrgManagementUI();
				DragManager.DestroyCurrentItem();
			}
			currentItem.gameObject.SetActive(false);
		}

		// Token: 0x06005931 RID: 22833 RVA: 0x0028F188 File Offset: 0x0028D388
		protected override bool CanDropItemHere()
		{
			if (!base.gameObject.activeSelf || DragManager.currentDragItemType != this.dragItemType)
			{
				return false;
			}
			if (this.organizer && this._councilorController.orgManagementCanvas.enabled)
			{
				OrganizerOrgListItem component = DragManager.currentItem.GetComponent<OrganizerOrgListItem>();
				if (component != null)
				{
					TIOrgState org = component.org;
					return component.orgStatus != OrganizerOrgListItem.OrgStatus.AVAILABLE && (org.ref_councilor == null || !org.ref_councilor.OrgProvidingActiveMission(org)) && org.AllowedOnFactionMarket(GameControl.control.activePlayer);
				}
			}
			else if (this._councilorController.councilGridCanvas.enabled && this._councilorController.councilorSingleCanvas.enabled)
			{
				OrgItemView component2 = DragManager.currentItem.GetComponent<OrgItemView>();
				if (component2 != null)
				{
					return component2.status != OrgItemView.OrgStatus.AVAILABLE;
				}
			}
			return false;
		}

		// Token: 0x04004079 RID: 16505
		private CouncilGridController _councilorController;

		// Token: 0x0400407A RID: 16506
		public bool organizer;
	}
}
