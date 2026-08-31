using System;
using PavonisInteractive.TerraInvicta.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta.SpaceCombat.UI
{
	// Token: 0x02000A06 RID: 2566
	public class EnemyShipListItemController : CombatantListItemController
	{
		// Token: 0x060062F3 RID: 25331 RVA: 0x002E9835 File Offset: 0x002E7A35
		public override void Init(SpaceCombatCanvasController masterController, CombatantController combatantController, int position)
		{
			GameControl.eventManager.RemoveListener<CombatSecond>(new EventManager.EventDelegate<CombatSecond>(this.UpdateTargetDistance), null);
			base.Init(masterController, combatantController, position);
			this.primaryTargetImage.enabled = false;
			this.distanceToTargetTxt.enabled = false;
			base.UpdateListItem();
		}

		// Token: 0x060062F4 RID: 25332 RVA: 0x002E9878 File Offset: 0x002E7A78
		public void OnListItemClicked()
		{
			AudioManager.PlayOneShot(base.combatantController.GetCombatantState().IsAlien() ? "event:/SFX/UI_SFX/trig_SFX_AlienFleetSelect" : "event:/SFX/UI_SFX/trig_SFX_OtherHumanFleetSelect", false, false);
			GameControl.eventManager.TriggerEvent(new CombatTargetedableStateSelected(base.combatantController.GetCombatantState(), false, false), null, Array.Empty<object>());
		}

		// Token: 0x060062F5 RID: 25333 RVA: 0x002E98CC File Offset: 0x002E7ACC
		public override void OnDoubleClick()
		{
			if (!GeneralControlsController.UIPlayerInTargetingMode)
			{
				this.spaceCombat.combatCamera.LookAtCombatant(base.combatantController);
			}
		}

		// Token: 0x060062F6 RID: 25334 RVA: 0x002E98EC File Offset: 0x002E7AEC
		public void OnPrimaryTargetSelected()
		{
			if (this.masterController.selectedFriendlyShip != null && this.masterController.selectedFriendlyShip.primaryTarget != null)
			{
				this.primaryTargetImage.enabled = this.masterController.selectedFriendlyShip.primaryTarget == base.combatantController;
				if (this.masterController.selectedFriendlyShip.primaryTarget == base.combatantController)
				{
					this.targetingCombatant = this.masterController.selectedFriendlyShip;
				}
			}
		}

		// Token: 0x060062F7 RID: 25335 RVA: 0x002E9978 File Offset: 0x002E7B78
		public void OnPlayerShipSelected()
		{
			if (!this._addedCombatSecondListener)
			{
				GameControl.eventManager.AddListener<CombatSecond>(new EventManager.EventDelegate<CombatSecond>(this.UpdateTargetDistance), null, null, true, false);
				this._addedCombatSecondListener = true;
			}
			this.ShowTargetDistance();
		}

		// Token: 0x060062F8 RID: 25336 RVA: 0x002E99A9 File Offset: 0x002E7BA9
		public void OnShipSelectionCleared()
		{
			this.distanceToTargetTxt.enabled = false;
			GameControl.eventManager.RemoveListener<CombatSecond>(new EventManager.EventDelegate<CombatSecond>(this.UpdateTargetDistance), null);
			this._addedCombatSecondListener = false;
		}

		// Token: 0x060062F9 RID: 25337 RVA: 0x002E99D8 File Offset: 0x002E7BD8
		private void UpdateTargetDistance(CombatSecond e)
		{
			if (this.masterController.selectedFriendlyShip == null || base.combatantController == null)
			{
				return;
			}
			this.distanceToTargetTxt.SetText(Loc.T("UI.Space.Distkm", new object[] { SpaceCombatManager.scale_to_km(Vector3.Distance(base.combatantController.position, this.masterController.selectedFriendlyShip.position)).ToString("F0") }));
		}

		// Token: 0x060062FA RID: 25338 RVA: 0x002E9A58 File Offset: 0x002E7C58
		private void ShowTargetDistance()
		{
			this.distanceToTargetTxt.enabled = true;
			this.distanceToTargetTxt.SetText(Loc.T("UI.Space.Distkm", new object[] { SpaceCombatManager.scale_to_km(Vector3.Distance(base.combatantController.position, this.masterController.selectedFriendlyShip.position)).ToString("F0") }));
		}

		// Token: 0x060062FB RID: 25339 RVA: 0x002E9AC1 File Offset: 0x002E7CC1
		public void ClearPrimaryTarget()
		{
			if (this.primaryTargetImage != null)
			{
				this.primaryTargetImage.enabled = false;
			}
			this.targetingCombatant = null;
		}

		// Token: 0x060062FC RID: 25340 RVA: 0x002E9AE4 File Offset: 0x002E7CE4
		public override void OnDisable()
		{
			base.OnDisable();
			if (this._addedCombatSecondListener)
			{
				GameControl.eventManager.RemoveListener<CombatSecond>(new EventManager.EventDelegate<CombatSecond>(this.UpdateTargetDistance), null);
				this._addedCombatSecondListener = false;
			}
		}

		// Token: 0x060062FD RID: 25341 RVA: 0x002E9B12 File Offset: 0x002E7D12
		public override void OnDestroy()
		{
			base.OnDestroy();
			if (this._addedCombatSecondListener)
			{
				GameControl.eventManager.RemoveListener<CombatSecond>(new EventManager.EventDelegate<CombatSecond>(this.UpdateTargetDistance), null);
				this._addedCombatSecondListener = false;
			}
		}

		// Token: 0x040045CD RID: 17869
		public Image primaryTargetImage;

		// Token: 0x040045CE RID: 17870
		public TMP_Text distanceToTargetTxt;

		// Token: 0x040045CF RID: 17871
		private CombatantController targetingCombatant;

		// Token: 0x040045D0 RID: 17872
		public const float ySize = 108f;

		// Token: 0x040045D1 RID: 17873
		private bool _addedCombatSecondListener;
	}
}
