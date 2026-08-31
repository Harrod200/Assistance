using System;
using UnityEngine.EventSystems;

namespace PavonisInteractive.TerraInvicta.UI
{
	// Token: 0x0200091E RID: 2334
	public class CouncilOrgsDragDestination : DragDestination
	{
		// Token: 0x06005933 RID: 22835 RVA: 0x0028F26F File Offset: 0x0028D46F
		public override void SetControllerBase(CanvasControllerBase canvasControllerBase)
		{
			this.councilorController = (CouncilGridController)canvasControllerBase;
		}

		// Token: 0x06005934 RID: 22836 RVA: 0x0028F280 File Offset: 0x0028D480
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
				this.councilorController.StartMoveToCouncilOrgs(currentItem.GetComponent<OrgItemView>().GetOrg());
			}
			else
			{
				TIOrgState org = DragManager.currentItem.GetComponent<OrganizerOrgListItem>().org;
				if (this.councilorController.tempFactionCouncilorOrgs.ContainsKey(org))
				{
					this.councilorController.tempFactionCouncilorOrgs.Remove(org);
				}
				else
				{
					this.councilorController.tempMarketPoolOrgs.Remove(org);
				}
				this.councilorController.tempFactionOrgs.Add(org);
				this.councilorController.UpdateOrgManagementUI();
				DragManager.DestroyCurrentItem();
			}
			currentItem.gameObject.SetActive(false);
		}

		// Token: 0x06005935 RID: 22837 RVA: 0x0028F348 File Offset: 0x0028D548
		protected override bool CanDropItemHere()
		{
			if (!base.gameObject.activeSelf || DragManager.currentDragItemType != this.dragItemType)
			{
				return false;
			}
			if (this.organizer && this.councilorController.orgManagementCanvas.enabled)
			{
				TIOrgState org = DragManager.currentItem.GetComponent<OrganizerOrgListItem>().org;
				return DragManager.currentItem.GetComponent<OrganizerOrgListItem>().orgStatus != OrganizerOrgListItem.OrgStatus.UNASSIGNED && (org.ref_councilor == null || !org.ref_councilor.OrgProvidingActiveMission(org));
			}
			return this.councilorController.councilGridCanvas.enabled && this.councilorController.councilorSingleCanvas.enabled && DragManager.currentItem.GetComponent<OrgItemView>().status != OrgItemView.OrgStatus.UNASSIGNED;
		}

		// Token: 0x0400407B RID: 16507
		private CouncilGridController councilorController;

		// Token: 0x0400407C RID: 16508
		public bool organizer;
	}
}
