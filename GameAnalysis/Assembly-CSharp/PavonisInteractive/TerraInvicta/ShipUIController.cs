using System;
using PavonisInteractive.TerraInvicta.Audio;
using PavonisInteractive.TerraInvicta.SpaceCombat;
using PavonisInteractive.TerraInvicta.SpaceCombat.UI;
using PavonisInteractive.TerraInvicta.Systems.SolarSystem;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020008D0 RID: 2256
	public class ShipUIController : SpaceCombatAssetUIController
	{
		// Token: 0x17000F18 RID: 3864
		// (get) Token: 0x06005673 RID: 22131 RVA: 0x00278522 File Offset: 0x00276722
		public TISpaceShipState ship
		{
			get
			{
				return this.shipVisController.shipState;
			}
		}

		// Token: 0x06005674 RID: 22132 RVA: 0x00278530 File Offset: 0x00276730
		public void Initialize(ShipVisController shipVisController)
		{
			if (shipVisController.UIVisualizationOnly)
			{
				base.gameObject.SetActive(false);
				return;
			}
			this.shipVisController = shipVisController;
			this.modelController = shipVisController.ModelController;
			Vector3 mouseColliderDimensions = this.modelController.GetMouseColliderDimensions(this.ship.hull);
			this.mouseCollider.radius = Mathf.Max(mouseColliderDimensions.x, 10f);
			this.mouseCollider.height = mouseColliderDimensions.y;
			this.mouseCollider.center = new Vector3(0f, 0f, mouseColliderDimensions.z);
			this.UIcanvas.enabled = false;
			this.UpdateUIScale();
			GameControl.eventManager.RemoveListener<UIScaleSettingChange>(new EventManager.EventDelegate<UIScaleSettingChange>(this.OnUIScaleChanged), null);
			GameControl.eventManager.AddListener<UIScaleSettingChange>(new EventManager.EventDelegate<UIScaleSettingChange>(this.OnUIScaleChanged), null, null, true, false);
			this.groupNumber.enabled = false;
			this.SetShipDamageImages();
			this.shipName.SetText(this.ship.displayName);
			if (this.ship.hull.noShipyardBuild)
			{
				if (this.ship.isAlien)
				{
					this.shipClass.SetText(Loc.T("UI.Precombat.AlienExofighter"));
				}
				else
				{
					this.shipClass.SetText(Loc.T("UI.Precombat.Exofighter"));
				}
			}
			else
			{
				this.shipClass.SetText(this.ship.template.className);
			}
			this.activePlayerShip = GameControl.control.activePlayer == this.ship.faction;
			this.mainCamera = GameControl.control.mainCamera;
			if (this.weaponRangeCone != null)
			{
				this.weaponRangeCone.transform.rotation = this.modelController.transform.rotation;
			}
			if (this.ship.isAlien)
			{
				this.radiatorImage.enabled = false;
				this.driveImage.enabled = false;
			}
			Loc.SwapFonts(base.gameObject);
		}

		// Token: 0x06005675 RID: 22133 RVA: 0x0027872C File Offset: 0x0027692C
		public override void InitializeForCombat(CombatantController combatShipController, CombatantListItemController listItemController)
		{
			this.combatShipController = combatShipController.ref_shipController;
			base.combatantListItemController = listItemController;
			if (this.weaponRangeCone != null)
			{
				this.weaponRangeCone.transform.rotation = this.modelController.transform.rotation;
			}
			this.groupNumber.enabled = false;
			this.RemoveListeners();
			GameControl.eventManager.AddListener<CombatShipGroupChange>(new EventManager.EventDelegate<CombatShipGroupChange>(this.OnShipGroupChange), null, this.ship, true, false);
			GameControl.eventManager.AddListener<CombatEnds>(new EventManager.EventDelegate<CombatEnds>(this.RemoveCombatListeners), null, null, true, false);
			GameControl.eventManager.RemoveListener<UIScaleSettingChange>(new EventManager.EventDelegate<UIScaleSettingChange>(this.OnUIScaleChanged), null);
			GameControl.eventManager.AddListener<UIScaleSettingChange>(new EventManager.EventDelegate<UIScaleSettingChange>(this.OnUIScaleChanged), null, null, true, false);
		}

		// Token: 0x06005676 RID: 22134 RVA: 0x002787F6 File Offset: 0x002769F6
		public void DisableWeaponRangeVisualizations()
		{
			if (this.weaponRangeCone != null)
			{
				this.weaponRangeCone.SetActive(false);
			}
			if (this.weaponRangeSphere != null)
			{
				this.weaponRangeSphere.SetActive(false);
			}
		}

		// Token: 0x06005677 RID: 22135 RVA: 0x0027882C File Offset: 0x00276A2C
		public bool IsShipDestroyed()
		{
			return this.combatShipController.isDestroyed;
		}

		// Token: 0x06005678 RID: 22136 RVA: 0x0027883C File Offset: 0x00276A3C
		public void SetShipDamageImages()
		{
			CombatantListItemController.SetNoseImage(this.ship, this.noseImage);
			CombatantListItemController.SetMidImage(this.ship, this.lateralImage);
			CombatantListItemController.SetTailImage(this.ship, this.tailImage);
			if (!this.ship.isAlien)
			{
				CombatantListItemController.SetRadiatorImage(this.ship, this.radiatorImage);
				CombatantListItemController.SetDriveImage(this.ship, this.driveImage);
			}
		}

		// Token: 0x06005679 RID: 22137 RVA: 0x002788AB File Offset: 0x00276AAB
		private void ShipDamaged(ShipSystemDamageChange e)
		{
			this.SetShipDamageImages();
		}

		// Token: 0x0600567A RID: 22138 RVA: 0x002788B3 File Offset: 0x00276AB3
		private void ShipDamaged(ShipPartDamageChange e)
		{
			this.SetShipDamageImages();
		}

		// Token: 0x0600567B RID: 22139 RVA: 0x002788BB File Offset: 0x00276ABB
		private void RemoveCombatListeners(CombatEnds e)
		{
			this.RemoveListeners();
		}

		// Token: 0x0600567C RID: 22140 RVA: 0x002788C3 File Offset: 0x00276AC3
		private void OnUIScaleChanged(UIScaleSettingChange e)
		{
			this.UpdateUIScale();
		}

		// Token: 0x0600567D RID: 22141 RVA: 0x002788CB File Offset: 0x00276ACB
		private void UpdateUIScale()
		{
			this.canvasScaler.referenceResolution = new Vector2(1920f, (float)TemplateManager.global.uiScaleValues[TIPlayerProfileManager.uiScaleSetting]);
		}

		// Token: 0x0600567E RID: 22142 RVA: 0x002788F3 File Offset: 0x00276AF3
		public void TurnOffRangeVisuals()
		{
			if (this.weaponRangeCone != null)
			{
				this.weaponRangeCone.SetActive(false);
			}
			if (this.weaponRangeSphere != null)
			{
				this.weaponRangeSphere.SetActive(false);
			}
		}

		// Token: 0x0600567F RID: 22143 RVA: 0x00278929 File Offset: 0x00276B29
		public void OnShipGroupChange(CombatShipGroupChange e)
		{
			this.OnShipGroupChange();
		}

		// Token: 0x06005680 RID: 22144 RVA: 0x00278934 File Offset: 0x00276B34
		public void OnShipGroupChange()
		{
			if (this.combatShipController.controlGroups.Count > 0)
			{
				this.groupNumber.SetText(this.combatShipController.GetGroupMembershipString());
				this.groupNumber.enabled = true;
				return;
			}
			this.groupNumber.enabled = false;
		}

		// Token: 0x06005681 RID: 22145 RVA: 0x00278984 File Offset: 0x00276B84
		private void OnMouseEnter()
		{
			if (!TIStandaloneInputModule.current.IsPointerOverUIGameObject() && !this.shipVisController.UIVisualizationOnly)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/NEW_UI_SFX/trig_SFX_HoverSpaceShipModel", false, false);
				if (TIGlobalValuesState.isSpaceCombatEnabled)
				{
					if (this.combatShipController.destructionTriggered)
					{
						this.modelController.StopSelectionAnimation();
						GameControl.eventManager.RemoveListener<ShipSystemDamageChange>(new EventManager.EventDelegate<ShipSystemDamageChange>(this.ShipDamaged), null);
						GameControl.eventManager.RemoveListener<ShipPartDamageChange>(new EventManager.EventDelegate<ShipPartDamageChange>(this.ShipDamaged), null);
						this.UIcanvas.enabled = false;
						this.groupNumber.enabled = false;
						if (base.combatantListItemController != null && base.combatantListItemController.frameImage != null && base.combatantListItemController.button != null)
						{
							base.combatantListItemController.frameImage.color = base.combatantListItemController.button.colors.normalColor;
						}
						return;
					}
					GameControl.eventManager.AddListener<ShipSystemDamageChange>(new EventManager.EventDelegate<ShipSystemDamageChange>(this.ShipDamaged), null, this.ship, true, false);
					GameControl.eventManager.AddListener<ShipPartDamageChange>(new EventManager.EventDelegate<ShipPartDamageChange>(this.ShipDamaged), null, this.ship, true, false);
					base.combatantListItemController.frameImage.color = base.combatantListItemController.button.colors.highlightedColor;
					this.combatShipController._waypointNavigationController.ToggleHeightLines(true);
					if (TIInputManager.inTargetingMode)
					{
						TIInputManager.SetCursor(TIInputManager.targetCursorValid, true);
					}
				}
				if (!this.maintainAnimation || !this.modelController.selectionAnimating)
				{
					this.modelController.StartSelectionAnimation();
				}
				this.shipName.SetText(this.ship.displayName);
				this.shipClass.SetText(this.ship.template.className);
				this.SetShipDamageImages();
				this.UIcanvas.enabled = true;
			}
		}

		// Token: 0x06005682 RID: 22146 RVA: 0x00278B70 File Offset: 0x00276D70
		private void OnMouseExit()
		{
			if (!this.shipVisController.UIVisualizationOnly)
			{
				if (!this.maintainAnimation)
				{
					this.modelController.StopSelectionAnimation();
				}
				this.UIcanvas.enabled = false;
				if (TIGlobalValuesState.isSpaceCombatEnabled)
				{
					if (base.combatantListItemController.frameImage != null)
					{
						base.combatantListItemController.frameImage.color = base.combatantListItemController.button.colors.normalColor;
					}
					GameControl.eventManager.RemoveListener<ShipSystemDamageChange>(new EventManager.EventDelegate<ShipSystemDamageChange>(this.ShipDamaged), null);
					GameControl.eventManager.RemoveListener<ShipPartDamageChange>(new EventManager.EventDelegate<ShipPartDamageChange>(this.ShipDamaged), null);
					this.combatShipController._waypointNavigationController.ToggleHeightLines(false);
					if (TIInputManager.inTargetingMode)
					{
						TIInputManager.SetCursor(TIInputManager.targetCursor, true);
					}
				}
			}
		}

		// Token: 0x06005683 RID: 22147 RVA: 0x00278C42 File Offset: 0x00276E42
		private void OnMouseDown()
		{
			if (TIStandaloneInputModule.current.IsPointerOverAltWaypointSelectionUI())
			{
				this._selectingAltWaypoint = true;
				return;
			}
			this._selectingAltWaypoint = false;
		}

		// Token: 0x06005684 RID: 22148 RVA: 0x00278C60 File Offset: 0x00276E60
		private void OnMouseUpAsButton()
		{
			if (TIStandaloneInputModule.current.IsPointerOverUIGameObject())
			{
				return;
			}
			if (!TIGlobalValuesState.isSpaceCombatEnabled && !GameControl.control.skirmishMode)
			{
				if (!EventSystem.current.IsPointerOverGameObject())
				{
					if (GeneralControlsController.CurrentlyTargetingStateType(typeof(TISpaceFleetState)))
					{
						if (GeneralControlsController.UITargetingMode.GetPossibleTargets.Contains(this.ship.fleet))
						{
							AudioManager.PlayOneShot(this.activePlayerShip ? "event:/SFX/UI_SFX/trig_SFX_MyFleetSelect" : (this.ship.faction.IsAlienFaction ? "event:/SFX/UI_SFX/trig_SFX_AlienFleetSelect" : "event:/SFX/UI_SFX/trig_SFX_OtherHumanFleetSelect"), false, false);
							TIUtilities.GotoGameState(this.ship.fleet, false, this.ship.fleet.faction != GameControl.control.activePlayer, true, true, false, -1f);
							return;
						}
						AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
						return;
					}
					else if (GeneralControlsController.CurrentlyTargetingStateType(typeof(TISpaceShipState)))
					{
						if (GeneralControlsController.UITargetingMode.GetPossibleTargets.Contains(this.ship))
						{
							AudioManager.PlayOneShot(this.activePlayerShip ? "event:/SFX/UI_SFX/trig_SFX_MyFleetSelect" : (this.ship.faction.IsAlienFaction ? "event:/SFX/UI_SFX/trig_SFX_AlienFleetSelect" : "event:/SFX/UI_SFX/trig_SFX_OtherHumanFleetSelect"), false, false);
							TIUtilities.GotoGameState(this.ship, false, this.ship.fleet.faction != GameControl.control.activePlayer, true, true, false, -1f);
							return;
						}
						AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
						return;
					}
					else
					{
						AudioManager.PlayOneShot(this.activePlayerShip ? "event:/SFX/UI_SFX/trig_SFX_MyFleetSelect" : (this.ship.faction.IsAlienFaction ? "event:/SFX/UI_SFX/trig_SFX_AlienFleetSelect" : "event:/SFX/UI_SFX/trig_SFX_OtherHumanFleetSelect"), false, false);
						TIUtilities.GotoGameState(this.ship.fleet, true, true, true, true, true, -1f);
						SpaceObjectSelection.BlockSelectionFrame();
					}
				}
				return;
			}
			if (this._selectingAltWaypoint)
			{
				return;
			}
			SpaceCombatManager spaceCombat = GameControl.spaceCombat;
			if (spaceCombat.IsInFormationSelectionMode)
			{
				GameControl.eventManager.TriggerEvent(new ShipSelectedDuringFormationSetting(this.ship), null, Array.Empty<object>());
				return;
			}
			AudioManager.PlayOneShot(this.activePlayerShip ? "event:/SFX/UI_SFX/trig_SFX_MyFleetSelect" : (this.ship.faction.IsAlienFaction ? "event:/SFX/UI_SFX/trig_SFX_AlienFleetSelect" : "event:/SFX/UI_SFX/trig_SFX_OtherHumanFleetSelect"), false, false);
			if (!GeneralControlsController.UIPlayerInTargetingMode)
			{
				this._doubleClickCount++;
				if (this._doubleClickCount == 1)
				{
					this._lastClickTime = Time.time;
				}
				if (this._doubleClickCount > 1 && Time.time - this._lastClickTime <= this._doubleClickWindow)
				{
					this._doubleClickCount = 0;
					this._lastClickTime = 0f;
					spaceCombat.combatCamera.LookAtCombatant(this.combatShipController);
				}
				else
				{
					this._doubleClickCount = 1;
					this._lastClickTime = Time.time;
				}
			}
			GameControl.eventManager.TriggerEvent(new CombatTargetedableStateSelected(this.ship, false, false), null, Array.Empty<object>());
		}

		// Token: 0x06005685 RID: 22149 RVA: 0x00278F40 File Offset: 0x00277140
		private void RemoveListeners()
		{
			GameControl.eventManager.RemoveListener<CombatEnds>(new EventManager.EventDelegate<CombatEnds>(this.RemoveCombatListeners), null);
			GameControl.eventManager.RemoveListener<CombatShipGroupChange>(new EventManager.EventDelegate<CombatShipGroupChange>(this.OnShipGroupChange), null);
			GameControl.eventManager.RemoveListener<ShipSystemDamageChange>(new EventManager.EventDelegate<ShipSystemDamageChange>(this.ShipDamaged), null);
			GameControl.eventManager.RemoveListener<ShipPartDamageChange>(new EventManager.EventDelegate<ShipPartDamageChange>(this.ShipDamaged), null);
			GameControl.eventManager.RemoveListener<UIScaleSettingChange>(new EventManager.EventDelegate<UIScaleSettingChange>(this.OnUIScaleChanged), null);
		}

		// Token: 0x06005686 RID: 22150 RVA: 0x00278FC0 File Offset: 0x002771C0
		private void OnDisable()
		{
			this.UIcanvas.enabled = false;
			this.groupNumber.enabled = false;
		}

		// Token: 0x06005687 RID: 22151 RVA: 0x00278FDA File Offset: 0x002771DA
		private void OnDestroy()
		{
			this.RemoveListeners();
		}

		// Token: 0x06005688 RID: 22152 RVA: 0x00278FE4 File Offset: 0x002771E4
		private void LateUpdate()
		{
			if (this.UIcanvas.enabled || this.groupNumber.enabled)
			{
				Vector2 vector = Vector2.zero;
				if (TIGlobalValuesState.isSpaceCombatEnabled)
				{
					vector = RectTransformUtility.WorldToScreenPoint(this.mainCamera, this.combatShipController.position);
					float num = Vector3.Distance(this.mainCamera.transform.position, this.combatShipController.position);
					if (this.groupNumber.enabled)
					{
						float num2 = vector.x - 40f + 40f / Mathf.Max(1f, num);
						float num3 = vector.y + 25f + 25f / Mathf.Max(1f, num);
						this.groupNumber.transform.position = new Vector2(num2, num3);
						if (Vector3.Dot(this.mainCamera.transform.forward, (this.combatShipController.position - this.mainCamera.transform.position).normalized) < 0f)
						{
							this.groupNumber.color = Color.clear;
						}
						else
						{
							this.groupNumber.color = TIUtilities.UITextColor;
						}
					}
				}
				else if (this.shipVisController != null)
				{
					vector = RectTransformUtility.WorldToScreenPoint(this.mainCamera, this.shipVisController.transform.position);
				}
				if (this.UIcanvas.enabled)
				{
					this.shipName.transform.position = new Vector2(vector.x, vector.y + this.shipNameYOffset * this.UIcanvas.scaleFactor);
					this.shipClass.transform.position = new Vector2(vector.x, vector.y + this.shipClassYOffset * this.UIcanvas.scaleFactor);
					this.shipImagePanel.transform.position = new Vector2(vector.x, vector.y + this.shipImageYOffset * this.UIcanvas.scaleFactor);
				}
			}
		}

		// Token: 0x04003D7E RID: 15742
		private CombatShipController combatShipController;

		// Token: 0x04003D7F RID: 15743
		public CapsuleCollider mouseCollider;

		// Token: 0x04003D80 RID: 15744
		private ShipVisController shipVisController;

		// Token: 0x04003D81 RID: 15745
		private ShipModelController modelController;

		// Token: 0x04003D82 RID: 15746
		public Canvas UIcanvas;

		// Token: 0x04003D83 RID: 15747
		public CanvasScaler canvasScaler;

		// Token: 0x04003D84 RID: 15748
		public TMP_Text groupNumber;

		// Token: 0x04003D85 RID: 15749
		public TMP_Text shipName;

		// Token: 0x04003D86 RID: 15750
		public TMP_Text shipClass;

		// Token: 0x04003D87 RID: 15751
		public GameObject shipImagePanel;

		// Token: 0x04003D88 RID: 15752
		public Image noseImage;

		// Token: 0x04003D89 RID: 15753
		public Image lateralImage;

		// Token: 0x04003D8A RID: 15754
		public Image tailImage;

		// Token: 0x04003D8B RID: 15755
		public Image driveImage;

		// Token: 0x04003D8C RID: 15756
		public Image radiatorImage;

		// Token: 0x04003D8D RID: 15757
		public GameObject weaponRangeSphere;

		// Token: 0x04003D8E RID: 15758
		public GameObject weaponRangeCone;

		// Token: 0x04003D8F RID: 15759
		public Material sphere_Material;

		// Token: 0x04003D90 RID: 15760
		public Material cone_Material;

		// Token: 0x04003D91 RID: 15761
		private bool activePlayerShip;

		// Token: 0x04003D92 RID: 15762
		private Camera mainCamera;

		// Token: 0x04003D93 RID: 15763
		[Header("UI Placement")]
		[SerializeField]
		private float shipNameYOffset = -40f;

		// Token: 0x04003D94 RID: 15764
		[SerializeField]
		private float shipClassYOffset = -85f;

		// Token: 0x04003D95 RID: 15765
		[SerializeField]
		private float shipImageYOffset = -65f;

		// Token: 0x04003D96 RID: 15766
		[SerializeField]
		private float formationModeShipNameYOffset = 30f;

		// Token: 0x04003D97 RID: 15767
		[SerializeField]
		private float formationModeClassNameYOffset = -30f;

		// Token: 0x04003D98 RID: 15768
		private const float shipGroupXOffset = 20f;

		// Token: 0x04003D99 RID: 15769
		private const float shipGroupYOffset = 20f;

		// Token: 0x04003D9A RID: 15770
		private bool _selectingAltWaypoint;

		// Token: 0x04003D9B RID: 15771
		private int _doubleClickCount;

		// Token: 0x04003D9C RID: 15772
		private float _lastClickTime;

		// Token: 0x04003D9D RID: 15773
		private float _doubleClickWindow = 0.5f;
	}
}
