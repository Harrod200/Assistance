using System;
using ModelShark;
using PavonisInteractive.TerraInvicta.Audio;
using PavonisInteractive.TerraInvicta.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000840 RID: 2112
	public class OrganizerOrgListItem : DragItem
	{
		// Token: 0x06004C6C RID: 19564 RVA: 0x00204634 File Offset: 0x00202834
		public void SetListItem(TIOrgState orgState, OrganizerOrgListItem.OrgStatus status, CouncilGridController controller, OrganizerCouncilorListItem parentContainer)
		{
			base.gameObject.SetActive(true);
			this.gridController = controller;
			this.parentDragContainer = parentContainer;
			this.org = orgState;
			this.orgStatus = status;
			this.orgIcon.sprite = orgState.icon;
			this.orgName.SetText(orgState.displayName);
			this.orgDescription.SetText(orgState.descriptionTruncated());
			this.orgTooltip.SetDelegate("BodyText", () => orgState.description(false, GameControl.control.activePlayer, false, true));
			this.orgTier.SetText(orgState.tierStarsInline);
			if (status == OrganizerOrgListItem.OrgStatus.AVAILABLE)
			{
				Behaviour behaviour = this.newRibbon;
				TIFactionState factionOrbit = orgState.factionOrbit;
				behaviour.enabled = factionOrbit != null && factionOrbit.newAvailableOrgs.Contains(orgState);
			}
		}

		// Token: 0x06004C6D RID: 19565 RVA: 0x00204721 File Offset: 0x00202921
		public override void OnBeginDrag(PointerEventData eventData)
		{
			this.gridController.UpdateDraggableOrgAreas(this.org);
			base.OnBeginDrag(eventData);
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
		}

		// Token: 0x06004C6E RID: 19566 RVA: 0x00204747 File Offset: 0x00202947
		public override void OnEndDrag(PointerEventData eventData)
		{
			this.gridController.ResetDraggableOrgAreas();
			base.OnEndDrag(eventData);
		}

		// Token: 0x04002E58 RID: 11864
		public Image orgIcon;

		// Token: 0x04002E59 RID: 11865
		public TMP_Text orgName;

		// Token: 0x04002E5A RID: 11866
		public TMP_Text orgDescription;

		// Token: 0x04002E5B RID: 11867
		public TMP_Text orgTier;

		// Token: 0x04002E5C RID: 11868
		public TooltipTrigger orgTooltip;

		// Token: 0x04002E5D RID: 11869
		public OrganizerOrgListItem.OrgStatus orgStatus;

		// Token: 0x04002E5E RID: 11870
		public TIOrgState org;

		// Token: 0x04002E5F RID: 11871
		public Image newRibbon;

		// Token: 0x04002E60 RID: 11872
		public OrganizerCouncilorListItem parentDragContainer;

		// Token: 0x04002E61 RID: 11873
		public CouncilGridController gridController;

		// Token: 0x0200104E RID: 4174
		public enum OrgStatus
		{
			// Token: 0x04006244 RID: 25156
			ASSIGNED,
			// Token: 0x04006245 RID: 25157
			UNASSIGNED,
			// Token: 0x04006246 RID: 25158
			AVAILABLE
		}
	}
}
