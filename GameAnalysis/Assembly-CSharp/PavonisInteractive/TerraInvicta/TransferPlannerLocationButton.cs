using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000889 RID: 2185
	public class TransferPlannerLocationButton : MonoBehaviour, IPointerClickHandler, IEventSystemHandler, IPointerEnterHandler, IPointerExitHandler
	{
		// Token: 0x17000EDB RID: 3803
		// (get) Token: 0x060051B4 RID: 20916 RVA: 0x0023EDAE File Offset: 0x0023CFAE
		public bool isDestination
		{
			get
			{
				return !this.isOrigin;
			}
		}

		// Token: 0x17000EDC RID: 3804
		// (get) Token: 0x060051B5 RID: 20917 RVA: 0x0023EDB9 File Offset: 0x0023CFB9
		public TransferPlanner TransferPlanner
		{
			get
			{
				return base.GetComponentInParent<TransferPlanner>();
			}
		}

		// Token: 0x17000EDD RID: 3805
		// (get) Token: 0x060051B6 RID: 20918 RVA: 0x0023EDC1 File Offset: 0x0023CFC1
		// (set) Token: 0x060051B7 RID: 20919 RVA: 0x0023EDCC File Offset: 0x0023CFCC
		public ITransferTarget SelectedLocation
		{
			get
			{
				return this.selectedLocation;
			}
			set
			{
				if (value == this.selectedLocation)
				{
					return;
				}
				this.selectedLocation = value;
				if (this.isOrigin && this.SelectionIsFleet)
				{
					this.TransferPlanner.accelerationInputField.text = (this.SelectedFleet.cruiseAcceleration_gs * 1000f).ToString("N1");
					this.TransferPlanner.dvInputField.text = this.SelectedFleet.currentDeltaV_kps.ToString("N1");
				}
				this.TransferPlanner.UpdateThrustProfile();
				if (this.SelectedGameState.ref_orbit != null)
				{
					string text = null;
					if (this.SelectionIsFleet)
					{
						text = "UI.Intel.FleetInOrbit";
					}
					else if (this.SelectionIsHab)
					{
						text = "UI.Intel.StationInOrbit";
					}
					if (text != null)
					{
						this.text.text = Loc.T(text, new object[]
						{
							this.SelectedGameState.GetDisplayName(GameControl.control.activePlayer),
							this.SelectedGameState.ref_orbit.displayName
						});
						return;
					}
					this.text.text = this.SelectedGameState.GetDisplayName(GameControl.control.activePlayer);
					return;
				}
				else
				{
					if (this.SelectionIsHab)
					{
						this.text.text = Loc.T("UI.Intel.BaseOnSpaceBody", new object[]
						{
							this.SelectedGameState.GetDisplayName(GameControl.control.activePlayer),
							this.SelectedGameState.ref_naturalSpaceObject.displayName
						});
						return;
					}
					this.text.text = Loc.T(this.SelectedGameState.GetDisplayName(GameControl.control.activePlayer));
					return;
				}
			}
		}

		// Token: 0x17000EDE RID: 3806
		// (get) Token: 0x060051B8 RID: 20920 RVA: 0x0023EF6C File Offset: 0x0023D16C
		public TIGameState SelectedGameState
		{
			get
			{
				return this.selectedLocation as TIGameState;
			}
		}

		// Token: 0x17000EDF RID: 3807
		// (get) Token: 0x060051B9 RID: 20921 RVA: 0x0023EF79 File Offset: 0x0023D179
		public TISpaceFleetState SelectedFleet
		{
			get
			{
				return this.selectedLocation as TISpaceFleetState;
			}
		}

		// Token: 0x17000EE0 RID: 3808
		// (get) Token: 0x060051BA RID: 20922 RVA: 0x0023EF86 File Offset: 0x0023D186
		public bool SelectionIsFleet
		{
			get
			{
				return this.SelectedFleet != null;
			}
		}

		// Token: 0x17000EE1 RID: 3809
		// (get) Token: 0x060051BB RID: 20923 RVA: 0x0023EF94 File Offset: 0x0023D194
		public TIHabState SelectedHab
		{
			get
			{
				return this.selectedLocation as TIHabState;
			}
		}

		// Token: 0x17000EE2 RID: 3810
		// (get) Token: 0x060051BC RID: 20924 RVA: 0x0023EFA1 File Offset: 0x0023D1A1
		public bool SelectionIsHab
		{
			get
			{
				return this.SelectedHab != null;
			}
		}

		// Token: 0x17000EE3 RID: 3811
		// (get) Token: 0x060051BD RID: 20925 RVA: 0x0023EFAF File Offset: 0x0023D1AF
		public TIOrbitState SelectedOrbit
		{
			get
			{
				return this.selectedLocation as TIOrbitState;
			}
		}

		// Token: 0x17000EE4 RID: 3812
		// (get) Token: 0x060051BE RID: 20926 RVA: 0x0023EFBC File Offset: 0x0023D1BC
		public bool SelectionIsOrbit
		{
			get
			{
				return this.SelectedOrbit != null;
			}
		}

		// Token: 0x17000EE5 RID: 3813
		// (get) Token: 0x060051BF RID: 20927 RVA: 0x0023EFCA File Offset: 0x0023D1CA
		// (set) Token: 0x060051C0 RID: 20928 RVA: 0x0023EFD1 File Offset: 0x0023D1D1
		public static TransferPlannerLocationButton SelectedLocationButton { get; private set; }

		// Token: 0x17000EE6 RID: 3814
		// (get) Token: 0x060051C1 RID: 20929 RVA: 0x0023EFD9 File Offset: 0x0023D1D9
		public bool IsSelected
		{
			get
			{
				return TransferPlannerLocationButton.SelectedLocationButton == this;
			}
		}

		// Token: 0x060051C2 RID: 20930 RVA: 0x0023EFE6 File Offset: 0x0023D1E6
		private void Start()
		{
			this.UpdateText();
		}

		// Token: 0x060051C3 RID: 20931 RVA: 0x0023EFF0 File Offset: 0x0023D1F0
		private void Update()
		{
			if (this.isHovered)
			{
				this.BackgroundImage.color = this.HoveredColor;
				return;
			}
			if (this.IsSelected)
			{
				this.BackgroundImage.color = this.SelectedColor;
				return;
			}
			this.BackgroundImage.color = this.RestColor;
		}

		// Token: 0x060051C4 RID: 20932 RVA: 0x0023F044 File Offset: 0x0023D244
		public void Select()
		{
			this.targetSelectionTool.SetTargetsToAllOrbitsAndSpaceAssets();
			this.targetSelectionTool.onTargetSelected = new TargetSelectionTool.OnTargetSelected(this.OnLocationSelected);
			Color lightOrange = Colors.LightOrange;
			lightOrange.a = 0.3f;
			this.targetSelectionTool.highlightImage.Blink(0.7f, 2, lightOrange);
			this.UpdateText();
			TransferPlannerLocationButton.SelectedLocationButton = this;
			this.targetSelectionTool.UpdateLabels();
		}

		// Token: 0x060051C5 RID: 20933 RVA: 0x0023F0B4 File Offset: 0x0023D2B4
		private void UpdateText()
		{
			if (this.SelectedLocation == null)
			{
				if (this.isOrigin)
				{
					this.text.text = Loc.T("UI.Intel.SelectOrigin");
				}
				else
				{
					this.text.text = Loc.T("UI.Intel.SelectDestination");
				}
				if (TransferPlannerLocationButton.SelectedLocationButton != null)
				{
					if (this.isOrigin)
					{
						this.text.text = Loc.T("UI.Intel.NoOrigin");
						return;
					}
					this.text.text = Loc.T("UI.Intel.NoDestination");
				}
			}
		}

		// Token: 0x060051C6 RID: 20934 RVA: 0x0023F140 File Offset: 0x0023D340
		public void OnLocationSelected(TIGameState gameState)
		{
			ITransferTarget transferTarget;
			if (this.isOrigin)
			{
				transferTarget = this.TransferPlanner.destinationButton.selectedLocation;
			}
			else
			{
				transferTarget = this.TransferPlanner.originButton.selectedLocation;
			}
			if (gameState == ((transferTarget != null) ? transferTarget.selfState() : null))
			{
				return;
			}
			this.SelectedLocation = gameState as ITransferTarget;
		}

		// Token: 0x060051C7 RID: 20935 RVA: 0x0023F19A File Offset: 0x0023D39A
		public void OnPointerClick(PointerEventData eventData)
		{
			this.Select();
		}

		// Token: 0x060051C8 RID: 20936 RVA: 0x0023F1A2 File Offset: 0x0023D3A2
		public void OnPointerEnter(PointerEventData eventData)
		{
			this.isHovered = true;
		}

		// Token: 0x060051C9 RID: 20937 RVA: 0x0023F1AB File Offset: 0x0023D3AB
		public void OnPointerExit(PointerEventData eventData)
		{
			this.isHovered = false;
		}

		// Token: 0x0400365B RID: 13915
		private bool isHovered;

		// Token: 0x0400365C RID: 13916
		public bool isOrigin;

		// Token: 0x0400365D RID: 13917
		public TargetSelectionTool targetSelectionTool;

		// Token: 0x0400365E RID: 13918
		public TMP_Text text;

		// Token: 0x0400365F RID: 13919
		public Image BackgroundImage;

		// Token: 0x04003660 RID: 13920
		public Color RestColor;

		// Token: 0x04003661 RID: 13921
		public Color HoveredColor;

		// Token: 0x04003662 RID: 13922
		public Color SelectedColor;

		// Token: 0x04003663 RID: 13923
		private ITransferTarget selectedLocation;
	}
}
