using System;
using Com.ForbiddenByte.OSA.Core;
using Com.ForbiddenByte.OSA.CustomParams;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020008E3 RID: 2275
	public class LedgerListItemViewsHolder : BaseItemViewsHolder
	{
		// Token: 0x0600579C RID: 22428 RVA: 0x002845F4 File Offset: 0x002827F4
		public override void CollectViews()
		{
			base.CollectViews();
			this.ledgerListItem = this.root.GetComponent<LedgerListItemController>();
		}

		// Token: 0x0600579D RID: 22429 RVA: 0x00284610 File Offset: 0x00282810
		public void UpdateFromModel(LedgerListItemModel model, BaseParamsWithPrefab parameters)
		{
			TIFactionState tifactionState = model.ledgerListItemData.associatedState as TIFactionState;
			if (tifactionState != null)
			{
				if (tifactionState == GameControl.control.activePlayer)
				{
					this.ledgerListItem.SetListItem(model.ledgerListItemData, tifactionState, model.ledgerListItemData.which);
					return;
				}
				this.ledgerListItem.SetListItem(model.ledgerListItemData, tifactionState, 3);
				return;
			}
			else
			{
				TICouncilorState ticouncilorState = model.ledgerListItemData.associatedState as TICouncilorState;
				if (ticouncilorState != null)
				{
					this.ledgerListItem.SetListItem(model.ledgerListItemData, ticouncilorState);
					return;
				}
				TITraitTemplate titraitTemplate = model.ledgerListItemData.associatedTemplate as TITraitTemplate;
				if (titraitTemplate != null)
				{
					this.ledgerListItem.SetListItem(model.ledgerListItemData, titraitTemplate, model.ledgerListItemData.parentGameState.ref_councilor);
					return;
				}
				TIOrgState tiorgState = model.ledgerListItemData.associatedState as TIOrgState;
				if (tiorgState != null)
				{
					this.ledgerListItem.SetListItem(model.ledgerListItemData, tiorgState);
					return;
				}
				TINationState tinationState = model.ledgerListItemData.associatedState as TINationState;
				if (tinationState != null)
				{
					this.ledgerListItem.SetListItem(model.ledgerListItemData, tinationState, GameControl.control.activePlayer);
					return;
				}
				TIHabState tihabState = model.ledgerListItemData.associatedState as TIHabState;
				if (tihabState != null)
				{
					this.ledgerListItem.SetListItem(model.ledgerListItemData, tihabState);
					return;
				}
				TIHabModuleState tihabModuleState = model.ledgerListItemData.associatedState as TIHabModuleState;
				if (tihabModuleState != null)
				{
					this.ledgerListItem.SetListItem(model.ledgerListItemData, tihabModuleState);
					return;
				}
				TISpaceFleetState tispaceFleetState = model.ledgerListItemData.associatedState as TISpaceFleetState;
				if (tispaceFleetState != null)
				{
					this.ledgerListItem.SetListItem(model.ledgerListItemData, tispaceFleetState);
					return;
				}
				TISpaceShipState tispaceShipState = model.ledgerListItemData.associatedState as TISpaceShipState;
				if (tispaceShipState != null)
				{
					this.ledgerListItem.SetListItem(model.ledgerListItemData, tispaceShipState);
				}
				return;
			}
		}

		// Token: 0x04003F4B RID: 16203
		public LedgerListItemController ledgerListItem;
	}
}
