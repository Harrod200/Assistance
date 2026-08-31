using System;
using ModelShark;
using PavonisInteractive.TerraInvicta.Audio;
using PavonisInteractive.TerraInvicta.Systems.Camera;
using PavonisInteractive.TerraInvicta.Systems.SolarSystem;
using TMPro;
using Unity.Entities;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200059F RID: 1439
	public class SectorDataPanelController : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
	{
		// Token: 0x060026B1 RID: 9905 RVA: 0x000D28DA File Offset: 0x000D0ADA
		public void Awake()
		{
			this.cameraManager = World.Active.GetExistingManager<CameraManager>();
			base.gameObject.SetActive(true);
			this.sectorFactionName.enabled = false;
		}

		// Token: 0x060026B2 RID: 9906 RVA: 0x000D2904 File Offset: 0x000D0B04
		public void Initialize(TISectorState sector, HabModelController modelController)
		{
			this.sector = sector;
			this.habModelController = modelController;
			this.spaceObjectSelection = World.Active.GetExistingManager<SpaceObjectSelection>();
			GameControl.eventManager.AddListener<SectorAssignedToFaction>(new EventManager.EventDelegate<SectorAssignedToFaction>(this.SetSectorData), null, sector.hab, true, false);
			GameControl.eventManager.AddListener<HabEntersCombat>(new EventManager.EventDelegate<HabEntersCombat>(this.DisableData), null, sector.hab, true, false);
			GameControl.eventManager.AddListener<HabDefendInterestsUpdated>(new EventManager.EventDelegate<HabDefendInterestsUpdated>(this.OnHabDefendInterestsUpdated), null, sector.hab, true, false);
		}

		// Token: 0x060026B3 RID: 9907 RVA: 0x000D298C File Offset: 0x000D0B8C
		private void DisableData(HabEntersCombat e)
		{
			this.TurnOffSectorData();
		}

		// Token: 0x060026B4 RID: 9908 RVA: 0x000D2994 File Offset: 0x000D0B94
		private void SetSectorData(SectorAssignedToFaction e)
		{
			if (e.sector == this.sector)
			{
				this.SetSectorData();
			}
		}

		// Token: 0x060026B5 RID: 9909 RVA: 0x000D29B0 File Offset: 0x000D0BB0
		public void SetSectorData()
		{
			if (this.sector.faction == null || this.sector.sectorNum != 0)
			{
				this.primaryCanvas.enabled = false;
				return;
			}
			this.factionIcon.sprite = this.sector.faction.factionIcon128;
			if (this.sector.sectorNum == 0)
			{
				this.habName.SetText(this.sector.hab.GetDisplayName(GameControl.control.activePlayer));
				this.sectorFactionName.SetText(this.sector.faction.displayNameCapitalized);
				if (this.sector.hab.coreDefended)
				{
					this.defendedIcon.SetActive(true);
					this.tooltip.SetDelegate("BodyText", () => Loc.T("UI.Habs.DefendedTip", new object[]
					{
						this.sector.faction.displayNameCapitalizedWithColor,
						this.sector.hab.coreDefendExpiration.ToCustomDateString()
					}));
					this.tooltip.enabled = true;
				}
				else
				{
					this.defendedIcon.SetActive(false);
					this.tooltip.enabled = false;
				}
			}
			else
			{
				this.sectorFactionName.SetText(this.sector.faction.displayNameCapitalized);
			}
			this.sectorFactionName.enabled = false;
			this.primaryCanvas.enabled = true;
			if (this.habName != null)
			{
				this.habName.enabled = false;
			}
		}

		// Token: 0x060026B6 RID: 9910 RVA: 0x000D2B04 File Offset: 0x000D0D04
		public void TurnOffSectorData()
		{
			this.primaryCanvas.enabled = false;
		}

		// Token: 0x060026B7 RID: 9911 RVA: 0x000D2B12 File Offset: 0x000D0D12
		public void TurnOnSectorData()
		{
			this.SetSectorData();
		}

		// Token: 0x060026B8 RID: 9912 RVA: 0x000D2B1C File Offset: 0x000D0D1C
		public void OnClicked()
		{
			this.spaceObjectSelection.BlockThisFrame = true;
			SoundEffectController.PlaySelectSound(this.sector.hab);
			if (GeneralControlsController.UIPlayerInTargetingMode)
			{
				if (GeneralControlsController.UITargetingMode.TargetedGameStates().Contains(typeof(TIHabState)))
				{
					GameControl.eventManager.TriggerEvent(new HabSelectedEvent(this.sector.hab), null, new object[] { this.sector.hab });
					return;
				}
				if (GeneralControlsController.UITargetingMode.TargetedGameStates().Contains(typeof(TISectorState)))
				{
					GameControl.eventManager.TriggerEvent(new SectorSelectedEvent(this.sector), null, new object[] { this.sector });
					return;
				}
			}
			else
			{
				GameControl.eventManager.TriggerEvent(new HabSelectedEvent(this.sector.hab), null, new object[] { this.sector.hab });
				TIUtilities.GotoSelectedStateUI(this.sector.hab, true);
			}
		}

		// Token: 0x060026B9 RID: 9913 RVA: 0x000D2C1C File Offset: 0x000D0E1C
		public void OnPointerEnter(PointerEventData eventData)
		{
			if (this.habModelController != null)
			{
				this.habModelController.mouseOverHabUIIcon = true;
				if (this.habName != null)
				{
					this.habName.enabled = true;
				}
				this.sectorFactionName.enabled = true;
				if (this.sector.sectorNum == 0)
				{
					this.habName.SetText(this.sector.hab.GetDisplayName(GameControl.control.activePlayer));
				}
			}
		}

		// Token: 0x060026BA RID: 9914 RVA: 0x000D2C9C File Offset: 0x000D0E9C
		public void OnPointerExit(PointerEventData eventData)
		{
			if (this.habModelController != null)
			{
				this.habModelController.mouseOverHabUIIcon = false;
				if (this.habName != null)
				{
					this.habName.enabled = false;
				}
				this.sectorFactionName.enabled = false;
			}
		}

		// Token: 0x060026BB RID: 9915 RVA: 0x000D2CE9 File Offset: 0x000D0EE9
		public void OnHabDefendInterestsUpdated(HabDefendInterestsUpdated e)
		{
			if (this.sector.sectorNum == 0)
			{
				if (this.sector.hab.coreDefended)
				{
					this.defendedIcon.SetActive(true);
					return;
				}
				this.defendedIcon.SetActive(false);
			}
		}

		// Token: 0x060026BC RID: 9916 RVA: 0x000D2D23 File Offset: 0x000D0F23
		public void Update()
		{
			if (this.primaryCanvas.enabled)
			{
				base.transform.rotation = this.cameraManager.BillboardRotation;
			}
		}

		// Token: 0x060026BD RID: 9917 RVA: 0x000D2D48 File Offset: 0x000D0F48
		public void OnDestroy()
		{
			GameControl.eventManager.RemoveListener<SectorAssignedToFaction>(new EventManager.EventDelegate<SectorAssignedToFaction>(this.SetSectorData), null);
		}

		// Token: 0x04001CC3 RID: 7363
		public Canvas primaryCanvas;

		// Token: 0x04001CC4 RID: 7364
		public Image factionIcon;

		// Token: 0x04001CC5 RID: 7365
		public Button primaryButton;

		// Token: 0x04001CC6 RID: 7366
		public TMP_Text sectorFactionName;

		// Token: 0x04001CC7 RID: 7367
		public TMP_Text habName;

		// Token: 0x04001CC8 RID: 7368
		private CameraManager cameraManager;

		// Token: 0x04001CC9 RID: 7369
		private TISectorState sector;

		// Token: 0x04001CCA RID: 7370
		private HabModelController habModelController;

		// Token: 0x04001CCB RID: 7371
		private SpaceObjectSelection spaceObjectSelection;

		// Token: 0x04001CCC RID: 7372
		public GameObject defendedIcon;

		// Token: 0x04001CCD RID: 7373
		public TooltipTrigger tooltip;
	}
}
