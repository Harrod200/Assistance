using System;
using PavonisInteractive.TerraInvicta.TIVirtualFleetState;
using TMPro;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000888 RID: 2184
	public class TransferPlanner : MonoBehaviour
	{
		// Token: 0x17000ED3 RID: 3795
		// (get) Token: 0x060051A5 RID: 20901 RVA: 0x0023E9DD File Offset: 0x0023CBDD
		public ITransferTarget Origin
		{
			get
			{
				return this.originButton.SelectedLocation;
			}
		}

		// Token: 0x17000ED4 RID: 3796
		// (get) Token: 0x060051A6 RID: 20902 RVA: 0x0023E9EA File Offset: 0x0023CBEA
		public ITransferTarget Destination
		{
			get
			{
				return this.destinationButton.SelectedLocation;
			}
		}

		// Token: 0x17000ED5 RID: 3797
		// (get) Token: 0x060051A7 RID: 20903 RVA: 0x0023E9F7 File Offset: 0x0023CBF7
		public bool CanParseAcceleration
		{
			get
			{
				return this.accelerationInputField.text.CanParseAsFloat();
			}
		}

		// Token: 0x17000ED6 RID: 3798
		// (get) Token: 0x060051A8 RID: 20904 RVA: 0x0023EA09 File Offset: 0x0023CC09
		public float Acceleration
		{
			get
			{
				return Mathf.Max(float.Parse(this.accelerationInputField.text), 0.0001f);
			}
		}

		// Token: 0x17000ED7 RID: 3799
		// (get) Token: 0x060051A9 RID: 20905 RVA: 0x0023EA25 File Offset: 0x0023CC25
		public bool CanParseDV
		{
			get
			{
				return this.dvInputField.text.CanParseAsFloat();
			}
		}

		// Token: 0x17000ED8 RID: 3800
		// (get) Token: 0x060051AA RID: 20906 RVA: 0x0023EA37 File Offset: 0x0023CC37
		public float DV
		{
			get
			{
				return float.Parse(this.dvInputField.text);
			}
		}

		// Token: 0x17000ED9 RID: 3801
		// (get) Token: 0x060051AB RID: 20907 RVA: 0x0023EA49 File Offset: 0x0023CC49
		public bool CanParseDeparture
		{
			get
			{
				return this.departureInputField.text.CanParseAsFloat();
			}
		}

		// Token: 0x17000EDA RID: 3802
		// (get) Token: 0x060051AC RID: 20908 RVA: 0x0023EA5B File Offset: 0x0023CC5B
		public float minDaysToDeparture
		{
			get
			{
				return float.Parse(this.departureInputField.text);
			}
		}

		// Token: 0x060051AD RID: 20909 RVA: 0x0023EA70 File Offset: 0x0023CC70
		private void Awake()
		{
			this.originButton.Select();
			this.targetSelectionTool.Filter = GameStateManager.Earth();
			this.InitializeLocalization();
			this.targetSelectionTool.Open();
			this.thrustProfileTool.Open();
			IntelScreenController componentInParent = base.GetComponentInParent<IntelScreenController>();
			componentInParent.OnExit = (IntelScreenController.OnExitCallback)Delegate.Combine(componentInParent.OnExit, new IntelScreenController.OnExitCallback(delegate
			{
				if (this.OnNextClose != null)
				{
					Action onNextClose = this.OnNextClose;
					this.OnNextClose = null;
					if (this.tabbedPaneController.IsSelected)
					{
						onNextClose();
					}
				}
			}));
		}

		// Token: 0x060051AE RID: 20910 RVA: 0x0023EADC File Offset: 0x0023CCDC
		private void Update()
		{
			Action<TMP_InputField> action = delegate(TMP_InputField inputField)
			{
				if (inputField.text.Length > 8)
				{
					inputField.text = inputField.text.Substring(0, 8);
				}
			};
			action(this.accelerationInputField);
			action(this.dvInputField);
			action(this.departureInputField);
		}

		// Token: 0x060051AF RID: 20911 RVA: 0x0023EB2C File Offset: 0x0023CD2C
		public void UpdateThrustProfile()
		{
			if (this.Origin == null || this.Destination == null || !TIGameState.Valid(this.Origin.selfState()) || !TIGameState.Valid(this.Destination.selfState()) || !this.CanParseAcceleration || !this.CanParseDV)
			{
				return;
			}
			float num = this.Acceleration * 0.00980665f;
			float num2 = this.DV * 1000f;
			TIFactionState ref_faction = GameControl.control.activePlayer.ref_faction;
			TISpaceAssetState tispaceAssetState = this.Origin as TISpaceAssetState;
			TIVirtualSpaceFleet tivirtualSpaceFleet;
			if (tispaceAssetState != null)
			{
				tivirtualSpaceFleet = new TIVirtualSpaceFleet(tispaceAssetState, num, num2, ref_faction);
			}
			else
			{
				TIOrbitState tiorbitState = this.Origin as TIOrbitState;
				if (tiorbitState == null)
				{
					return;
				}
				tivirtualSpaceFleet = new TIVirtualSpaceFleet(tiorbitState, num, num2, ref_faction, null, 0.0);
			}
			this.thrustProfileTool.Target = this.Destination as TIGameState;
			this.thrustProfileTool.Actor = tivirtualSpaceFleet;
			this.thrustProfileTool.ComputeCandidateTrajectories();
		}

		// Token: 0x060051B0 RID: 20912 RVA: 0x0023EC1C File Offset: 0x0023CE1C
		public void OnHideTab()
		{
			this.OnNextClose = null;
		}

		// Token: 0x060051B1 RID: 20913 RVA: 0x0023EC28 File Offset: 0x0023CE28
		private void InitializeLocalization()
		{
			this.headerLabel.text = Loc.T("UI.Intel.TransferPlannerHeader");
			this.originLabel.text = Loc.T("UI.Intel.Origin");
			this.destinationLabel.text = Loc.T("UI.Intel.Destination");
			this.accelerationLabel.text = Loc.T("UI.Intel.Acceleration");
			this.accelerationUnits.text = Loc.T("UI.Intel.Milligees");
			this.dvLabel.text = Loc.T("UI.Intel.DV");
			this.dvUnits.text = Loc.T("UI.Intel.KilometersPerSecond");
			this.departureLabel.text = Loc.T("UI.Intel.EarliestDeparture");
			this.departureUnits.text = Loc.T("UI.Intel.Days");
			this.thrustProfileHeaderLabel.text = Loc.T("UI.Intel.ThrustProfileHeader");
			this.targetSelectionTool.GetHeaderString = delegate(TargetSelectionTool targetSelectionTool_)
			{
				string displayName = targetSelectionTool_.Filter.ref_naturalSpaceObject.displayName;
				if (TransferPlannerLocationButton.SelectedLocationButton.isOrigin)
				{
					return Loc.T("UI.Intel.SelectOriginAround", new object[] { displayName });
				}
				return Loc.T("UI.Intel.SelectDestinationAround", new object[] { displayName });
			};
			this.thrustProfileTool.GetHeaderString = (ThrustProfileTool thrustProfileTool_) => Loc.T("UI.Intel.ThrustProfileHeader");
			this.thrustProfileTool.DisplayFailureReport(Loc.T("UI.Intel.MissingTrjectoryInfo"));
		}

		// Token: 0x04003645 RID: 13893
		public TabbedPaneController tabbedPaneController;

		// Token: 0x04003646 RID: 13894
		public TargetSelectionTool targetSelectionTool;

		// Token: 0x04003647 RID: 13895
		public ThrustProfileTool thrustProfileTool;

		// Token: 0x04003648 RID: 13896
		public TransferPlannerLocationButton originButton;

		// Token: 0x04003649 RID: 13897
		public TransferPlannerLocationButton destinationButton;

		// Token: 0x0400364A RID: 13898
		public TMP_InputField accelerationInputField;

		// Token: 0x0400364B RID: 13899
		public TMP_InputField dvInputField;

		// Token: 0x0400364C RID: 13900
		public TMP_InputField departureInputField;

		// Token: 0x0400364D RID: 13901
		public TMP_Text headerLabel;

		// Token: 0x0400364E RID: 13902
		public TMP_Text originLabel;

		// Token: 0x0400364F RID: 13903
		public TMP_Text destinationLabel;

		// Token: 0x04003650 RID: 13904
		public TMP_Text accelerationLabel;

		// Token: 0x04003651 RID: 13905
		public TMP_Text accelerationUnits;

		// Token: 0x04003652 RID: 13906
		public TMP_Text dvLabel;

		// Token: 0x04003653 RID: 13907
		public TMP_Text dvUnits;

		// Token: 0x04003654 RID: 13908
		public TMP_Text departureLabel;

		// Token: 0x04003655 RID: 13909
		public TMP_Text departureUnits;

		// Token: 0x04003656 RID: 13910
		public TMP_Text thrustProfileHeaderLabel;

		// Token: 0x04003657 RID: 13911
		public GameObject[] transferWaypointPanels;

		// Token: 0x04003658 RID: 13912
		public GameObject[] transferWaypointArrows;

		// Token: 0x04003659 RID: 13913
		public Action OnNextClose;

		// Token: 0x0400365A RID: 13914
		private const int maxInputLength = 8;
	}
}
