using System;
using UnityEngine.EventSystems;

namespace PavonisInteractive.TerraInvicta.UI
{
	// Token: 0x0200091F RID: 2335
	public class CouncilorOrgsDragDestination : DragDestination
	{
		// Token: 0x06005937 RID: 22839 RVA: 0x0028F412 File Offset: 0x0028D612
		public override void SetControllerBase(CanvasControllerBase canvasControllerBase)
		{
			this._councilorController = (CouncilGridController)canvasControllerBase;
		}

		// Token: 0x06005938 RID: 22840 RVA: 0x0028F420 File Offset: 0x0028D620
		public void SetCouncilor(TICouncilorState councilor, CouncilGridController gridController)
		{
			this.councilor = councilor;
			this._councilorController = gridController;
		}

		// Token: 0x06005939 RID: 22841 RVA: 0x0028F430 File Offset: 0x0028D630
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
				this._councilorController.StartOrgPurchase(currentItem.GetComponent<OrgItemView>().GetOrg());
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
					if (this._councilorController.tempFactionOrgs.Contains(org))
					{
						this._councilorController.tempFactionOrgs.Remove(org);
					}
					if (this._councilorController.tempMarketPoolOrgs.Contains(org))
					{
						this._councilorController.tempMarketPoolOrgs.Remove(org);
					}
				}
				this._councilorController.tempFactionCouncilorOrgs.Add(org, this.councilor);
				this._councilorController.UpdateOrgManagementUI();
				DragManager.DestroyCurrentItem();
			}
			currentItem.gameObject.SetActive(false);
		}

		// Token: 0x0600593A RID: 22842 RVA: 0x0028F53C File Offset: 0x0028D73C
		protected override bool CanDropItemHere()
		{
			if (!base.gameObject.activeSelf || DragManager.currentDragItemType != this.dragItemType)
			{
				return false;
			}
			if (this.organizer && this._councilorController.orgManagementCanvas.enabled)
			{
				TIOrgState org = DragManager.currentItem.GetComponent<OrganizerOrgListItem>().org;
				return (!this._councilorController.tempFactionCouncilorOrgs.ContainsKey(org) || this._councilorController.tempFactionCouncilorOrgs[org] != this.councilor) && (org.ref_councilor == null || !org.ref_councilor.OrgProvidingActiveMission(org)) && org.CouncilorCanAcquire(this.councilor);
			}
			if (!this._councilorController.councilGridCanvas.enabled || !this._councilorController.councilorSingleCanvas.enabled)
			{
				return false;
			}
			DragItem currentItem = DragManager.currentItem;
			if (currentItem == null)
			{
				return true;
			}
			OrgItemView component = currentItem.GetComponent<OrgItemView>();
			OrgItemView.OrgStatus? orgStatus = ((component != null) ? new OrgItemView.OrgStatus?(component.status) : null);
			OrgItemView.OrgStatus orgStatus2 = OrgItemView.OrgStatus.ASSIGNED;
			return !((orgStatus.GetValueOrDefault() == orgStatus2) & (orgStatus != null));
		}

		// Token: 0x0400407D RID: 16509
		private CouncilGridController _councilorController;

		// Token: 0x0400407E RID: 16510
		private TICouncilorState councilor;

		// Token: 0x0400407F RID: 16511
		public bool organizer;
	}
}
