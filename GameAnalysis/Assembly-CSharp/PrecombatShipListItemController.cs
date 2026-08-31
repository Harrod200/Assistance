using System;
using ModelShark;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.SpaceCombat.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000436 RID: 1078
public class PrecombatShipListItemController : MonoBehaviour
{
	// Token: 0x17000335 RID: 821
	// (get) Token: 0x06001664 RID: 5732 RVA: 0x00072306 File Offset: 0x00070506
	// (set) Token: 0x06001665 RID: 5733 RVA: 0x0007230E File Offset: 0x0007050E
	public TIDataClass gameState { get; private set; }

	// Token: 0x06001666 RID: 5734 RVA: 0x00072318 File Offset: 0x00070518
	public void SetListItem(TIDataClass item)
	{
		this.gameState = item;
		TISpaceShipState ship = item as TISpaceShipState;
		if (ship != null)
		{
			this.shipName.SetText(ship.NameWithDamageIcons());
			CombatantListItemController.SetNoseImage(ship, this.nose);
			CombatantListItemController.SetMidImage(ship, this.hull);
			CombatantListItemController.SetTailImage(ship, this.tail);
			this.nose.enabled = true;
			this.tail.enabled = true;
			this.hull.enabled = true;
			this.shipName.enabled = true;
			if (ship.isAlien)
			{
				this.radiators.enabled = false;
				this.drive.enabled = false;
			}
			else
			{
				CombatantListItemController.SetRadiatorImage(ship, this.radiators);
				CombatantListItemController.SetDriveImage(ship, this.drive);
				this.radiators.enabled = true;
				this.drive.enabled = true;
			}
			this.tooltip.enabled = true;
			this.tooltip.SetDelegate("BodyText", () => ship.template.quickSummary(ship.isAlien && !GameControl.control.activePlayer.finishedProjectNames.Contains("Project_TheirWarships"), ship, false, false, false));
			return;
		}
		TIHabState hab = item as TIHabState;
		if (hab != null)
		{
			this.tooltip.enabled = true;
			this.tooltip.SetDelegate("BodyText", () => hab.GetLocalizedHabModuleList());
			if (hab != null)
			{
				this.shipName.SetText(hab.GetDisplayName(GameControl.control.activePlayer));
				this.shipName.enabled = true;
				this.radiators.enabled = true;
				this.drive.enabled = false;
				this.nose.enabled = false;
				this.tail.enabled = false;
				this.hull.enabled = false;
				CombatantListItemController.SetHabImage(hab, this.radiators);
				return;
			}
		}
		else
		{
			TISpaceShipTemplate fighter = item as TISpaceShipTemplate;
			if (fighter != null)
			{
				CombatantListItemController.SetNoseImage(fighter, this.nose);
				CombatantListItemController.SetMidImage(fighter, this.hull);
				CombatantListItemController.SetTailImage(fighter, this.tail);
				this.shipName.SetText(Loc.T("UI.Precombat.SquadronName2", new object[] { fighter.displayName }));
				this.nose.enabled = true;
				this.tail.enabled = true;
				this.hull.enabled = true;
				this.radiators.enabled = false;
				this.drive.enabled = false;
				this.shipName.enabled = true;
				this.tooltip.enabled = true;
				this.tooltip.SetDelegate("BodyText", () => fighter.quickSummary(true, null, false, true, false));
			}
		}
	}

	// Token: 0x06001667 RID: 5735 RVA: 0x00072608 File Offset: 0x00070808
	public void SetListItemShading(bool shade)
	{
		if (this.nose.enabled)
		{
			this.nose.color = (shade ? new Color(1f, 1f, 1f, 0.25f) : Color.white);
		}
		if (this.hull.enabled)
		{
			this.hull.color = (shade ? new Color(1f, 1f, 1f, 0.25f) : Color.white);
		}
		if (this.tail.enabled)
		{
			this.tail.color = (shade ? new Color(1f, 1f, 1f, 0.25f) : Color.white);
		}
		if (this.drive.enabled)
		{
			this.drive.color = (shade ? new Color(1f, 1f, 1f, 0.25f) : Color.white);
		}
		if (this.radiators.enabled)
		{
			this.radiators.color = (shade ? new Color(1f, 1f, 1f, 0.25f) : Color.white);
		}
		if (this.shipName.enabled)
		{
			this.shipName.color = (shade ? TIUtilities.UITextColorTransluscent : TIUtilities.UITextColor);
		}
	}

	// Token: 0x040014C7 RID: 5319
	public Image nose;

	// Token: 0x040014C8 RID: 5320
	public Image hull;

	// Token: 0x040014C9 RID: 5321
	public Image tail;

	// Token: 0x040014CA RID: 5322
	public Image radiators;

	// Token: 0x040014CB RID: 5323
	public Image drive;

	// Token: 0x040014CC RID: 5324
	public TMP_Text shipName;

	// Token: 0x040014CD RID: 5325
	public TooltipTrigger tooltip;
}
