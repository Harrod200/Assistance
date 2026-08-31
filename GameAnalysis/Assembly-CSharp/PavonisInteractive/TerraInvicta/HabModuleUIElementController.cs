using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta.Audio;
using PavonisInteractive.TerraInvicta.SpaceCombat.UI;
using PavonisInteractive.TerraInvicta.Systems.SolarSystem;
using TMPro;
using Unity.Entities;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200059A RID: 1434
	public class HabModuleUIElementController : SpaceCombatAssetUIController
	{
		// Token: 0x17000571 RID: 1393
		// (get) Token: 0x0600266C RID: 9836 RVA: 0x000D02FA File Offset: 0x000CE4FA
		private TIHabState hab
		{
			get
			{
				return this.moduleController.hab;
			}
		}

		// Token: 0x0600266D RID: 9837 RVA: 0x000D0308 File Offset: 0x000CE508
		public void SetController(HabModuleController moduleController)
		{
			this.moduleController = moduleController;
			base.gameObject.SetActive(false);
			this.mainCamera = Camera.main;
			this.spaceObjectSelection = World.Active.GetExistingManager<SpaceObjectSelection>();
			this.canvas.enabled = false;
			Mesh sharedMesh = base.transform.parent.GetComponent<MeshFilter>().sharedMesh;
			if (sharedMesh != null)
			{
				Vector3 center = sharedMesh.bounds.center;
				this.collider.size = new Vector3(Mathf.Max(80f, sharedMesh.bounds.size.x), Mathf.Max(80f, sharedMesh.bounds.size.y), Mathf.Max(80f, sharedMesh.bounds.size.z));
				this.collider.center = center;
			}
		}

		// Token: 0x0600266E RID: 9838 RVA: 0x000D03F3 File Offset: 0x000CE5F3
		public void Initialize(TIHabModuleState module)
		{
			this.module = module;
		}

		// Token: 0x0600266F RID: 9839 RVA: 0x000D03FC File Offset: 0x000CE5FC
		public override void InitializeForCombat(CombatantController combatantController, CombatantListItemController combatantListItemController)
		{
			base.combatantListItemController = combatantListItemController;
		}

		// Token: 0x06002670 RID: 9840 RVA: 0x000D0405 File Offset: 0x000CE605
		private void OnEnable()
		{
			TMP_Text tmp_Text = this.moduleDisplayName;
			TIHabModuleState tihabModuleState = this.module;
			tmp_Text.SetText(((tihabModuleState != null) ? tihabModuleState.displayName : null) ?? "No module yet!");
		}

		// Token: 0x06002671 RID: 9841 RVA: 0x000D042D File Offset: 0x000CE62D
		private void OnDisable()
		{
			this.canvas.enabled = false;
		}

		// Token: 0x06002672 RID: 9842 RVA: 0x000D043C File Offset: 0x000CE63C
		private void OnMouseEnter()
		{
			if (this.moduleController.fullVisualization && !this.module.empty && !this.moduleController.habModelController.mouseOverHabUIIcon)
			{
				TIStandaloneInputModule current = TIStandaloneInputModule.current;
				if ((current == null || !current.IsPointerOverUIGameObject()) && (!TIGlobalValuesState.isSpaceCombatEnabled || this.module.isCombatModule))
				{
					this.canvas.enabled = true;
					List<MeshRenderer> renderers = this.moduleController.renderers;
					if (renderers != null)
					{
						renderers.ForEach(delegate(MeshRenderer x)
						{
							this.moduleController.SetHighlightColor(x);
						});
					}
					this.moduleController.highlighted = true;
					AudioManager.PlayOneShot("event:/SFX/UI_SFX/NEW_UI_SFX/trig_SFX_HoverIndividualSpaceHabitatModule", false, false);
				}
			}
		}

		// Token: 0x06002673 RID: 9843 RVA: 0x000D04EC File Offset: 0x000CE6EC
		private void OnMouseOver()
		{
			if (this.moduleController.fullVisualization && this.moduleController.highlighted && this.moduleController.habModelController.mouseOverHabUIIcon)
			{
				this.OnMouseExit();
				return;
			}
			if (this.moduleController.fullVisualization && !this.moduleController.highlighted)
			{
				this.OnMouseEnter();
			}
		}

		// Token: 0x06002674 RID: 9844 RVA: 0x000D054C File Offset: 0x000CE74C
		private void OnMouseExit()
		{
			if (this.moduleController.fullVisualization)
			{
				List<MeshRenderer> renderers = this.moduleController.renderers;
				if (renderers != null)
				{
					renderers.ForEach(delegate(MeshRenderer x)
					{
						this.moduleController.SetNormalColor(x);
					});
				}
				this.canvas.enabled = false;
				this.moduleController.highlighted = false;
			}
		}

		// Token: 0x06002675 RID: 9845 RVA: 0x000D05A0 File Offset: 0x000CE7A0
		private void OnMouseUp()
		{
			if (!this.module.deleted && !this.hab.deleted && this.moduleController.fullVisualization && this.moduleController.highlighted && !this.module.empty && !this.moduleController.habModelController.mouseOverHabUIIcon && !TIStandaloneInputModule.current.IsPointerOverUIGameObject() && !EventSystem.current.IsPointerOverGameObject())
			{
				if (!TIGlobalValuesState.isSpaceCombatEnabled)
				{
					this.spaceObjectSelection.BlockThisFrame = true;
					SoundEffectController.PlaySelectSound(this.hab.sectors[this.moduleController.sector]);
					GameControl.eventManager.TriggerEvent(new HabModuleSelected(this.module), null, Array.Empty<object>());
					if (!GeneralControlsController.UIPlayerInTargetingMode && (GeneralControlsController.UIOtherSelectedState == this.hab || GeneralControlsController.UIOtherSelectedState == this.hab.sectors[this.moduleController.sector] || GeneralControlsController.UIOtherSelectedState == this.module))
					{
						GameControl.eventManager.TriggerEvent(new HabDetailRequested(this.hab, true), null, Array.Empty<object>());
						return;
					}
					TIUtilities.GotoSelectedStateUI(this.hab, true);
					if (GeneralControlsController.UIPlayerInTargetingMode && GeneralControlsController.CurrentlyTargetingStateType(typeof(TIHabState)))
					{
						GameControl.eventManager.TriggerEvent(new HabSelectedEvent(this.hab), null, Array.Empty<object>());
						return;
					}
				}
				else if (this.module.isCombatModule)
				{
					AudioManager.PlayOneShot(this.module.IsAlien() ? "event:/SFX/UI_SFX/trig_SFX_AlienFleetSelect" : "event:/SFX/UI_SFX/trig_SFX_OtherHumanFleetSelect", false, false);
					GameControl.eventManager.TriggerEvent(new CombatTargetedableStateSelected(this.module, false, false), null, Array.Empty<object>());
				}
			}
		}

		// Token: 0x06002676 RID: 9846 RVA: 0x000D077C File Offset: 0x000CE97C
		private void Update()
		{
			if (this.canvas.enabled)
			{
				Vector2 vector = RectTransformUtility.WorldToScreenPoint(this.mainCamera, this.moduleController.transform.position);
				this.moduleDisplayName.transform.position = new Vector2(vector.x, vector.y - 45f);
			}
		}

		// Token: 0x04001C8F RID: 7311
		public Canvas canvas;

		// Token: 0x04001C90 RID: 7312
		public TMP_Text moduleDisplayName;

		// Token: 0x04001C91 RID: 7313
		private TIHabModuleState module;

		// Token: 0x04001C92 RID: 7314
		private Camera mainCamera;

		// Token: 0x04001C93 RID: 7315
		private HabModuleController moduleController;

		// Token: 0x04001C94 RID: 7316
		private SpaceObjectSelection spaceObjectSelection;

		// Token: 0x04001C95 RID: 7317
		public BoxCollider collider;
	}
}
