using System;
using System.Collections.Generic;
using System.Text;
using PavonisInteractive.TerraInvicta.Audio;
using PavonisInteractive.TerraInvicta.Components;
using PavonisInteractive.TerraInvicta.Systems;
using PavonisInteractive.TerraInvicta.Systems.SolarSystem;
using PavonisInteractive.TerraInvicta.TIVirtualFleetState;
using TMPro;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020008B7 RID: 2231
	public class ThrustProfileTool : MonoBehaviour
	{
		// Token: 0x17000F0B RID: 3851
		// (get) Token: 0x06005514 RID: 21780 RVA: 0x0026A55B File Offset: 0x0026875B
		public double LowestDVFound_kps
		{
			get
			{
				return this.lowestDVFound_kps;
			}
		}

		// Token: 0x17000F0C RID: 3852
		// (get) Token: 0x06005515 RID: 21781 RVA: 0x0026A563 File Offset: 0x00268763
		public Trajectory CurrentTrajectory
		{
			get
			{
				return this.currentTrajectory;
			}
		}

		// Token: 0x17000F0D RID: 3853
		// (get) Token: 0x06005516 RID: 21782 RVA: 0x0026A56B File Offset: 0x0026876B
		public bool CanReachTarget
		{
			get
			{
				return this.transferResult != null && this.transferResult.Result == TransferResult.Outcome.Success && this.candidateTrajectories != null && this.candidateTrajectories.Length != 0;
			}
		}

		// Token: 0x17000F0E RID: 3854
		// (get) Token: 0x06005517 RID: 21783 RVA: 0x0026A596 File Offset: 0x00268796
		// (set) Token: 0x06005518 RID: 21784 RVA: 0x0026A59E File Offset: 0x0026879E
		public IMobileAsset Actor
		{
			get
			{
				return this.actor;
			}
			set
			{
				if (this.actor == value)
				{
					return;
				}
				this.actor = value;
			}
		}

		// Token: 0x17000F0F RID: 3855
		// (get) Token: 0x06005519 RID: 21785 RVA: 0x0026A5B1 File Offset: 0x002687B1
		private TISpaceFleetState Fleet
		{
			get
			{
				return this.Actor as TISpaceFleetState;
			}
		}

		// Token: 0x17000F10 RID: 3856
		// (get) Token: 0x0600551A RID: 21786 RVA: 0x0026A5BE File Offset: 0x002687BE
		private bool isActorAFleet
		{
			get
			{
				return this.Fleet != null;
			}
		}

		// Token: 0x17000F11 RID: 3857
		// (get) Token: 0x0600551B RID: 21787 RVA: 0x0026A5CC File Offset: 0x002687CC
		// (set) Token: 0x0600551C RID: 21788 RVA: 0x0026A5D4 File Offset: 0x002687D4
		public TIGameState Target
		{
			get
			{
				return this.target;
			}
			set
			{
				if (this.target == value)
				{
					return;
				}
				this.target = value;
				this.ComputeCandidateTrajectories();
			}
		}

		// Token: 0x0600551D RID: 21789 RVA: 0x0026A5F2 File Offset: 0x002687F2
		public void UpdateTargetWithoutComputingNewTrajectories(TIGameState newTarget)
		{
			this.target = newTarget;
		}

		// Token: 0x0600551E RID: 21790 RVA: 0x0026A5FC File Offset: 0x002687FC
		private void Awake()
		{
			this.thrustPanelSortByHeaderText.SetText(Loc.T("UI.Operations.SortByHeader"));
			this.thrustPanelSortByLaunchDateText.SetText(Loc.T("UI.Operations.SortByLaunchDate"));
			this.thrustPanelSortByArrivalDateText.SetText(Loc.T("UI.Operations.SortByArrivalDate"));
			this.thrustPanelSortByDeltaVCostText.SetText(Loc.T("UI.Operations.SortByDeltaVCost"));
			this.thrustPanelSpecialManeuverText.SetText(Loc.T("UI.Operations.SpecialOrdersHeader"));
			this.thrustProfilePanelAeroCaptureText.SetText(Loc.T("UI.Operations.Aerocapture"));
			this.thrustProfilePanelFlyByText.SetText(Loc.T("UI.Operations.FlyBy"));
			this.thurstProfileRepairandResupplyText.SetText(Loc.T("UI.Operations.ROA"));
			this.fleetFollowWarning.SetText(new StringBuilder(TIGlobalConfig.globalConfig.warningInlineSpritePath).Append(TIUtilities.RedLine(Loc.T("UI.Operations.CantFollowTargetFleet"))).Append(TIGlobalConfig.globalConfig.warningInlineSpritePath));
			this.thrustPanelAllowAerocaptureToggle.gameObject.SetActive(false);
			this.thrustPanelFlyByToggle.gameObject.SetActive(false);
			this.thrustProfilePanelAeroCaptureText.gameObject.SetActive(false);
			this.thrustProfilePanelFlyByText.gameObject.SetActive(false);
			this.thrustPanelSpecialManeuverText.gameObject.SetActive(false);
			this.DVSlider.gameObject.SetActive(false);
			this.interceptionWarningObject.SetActive(false);
		}

		// Token: 0x0600551F RID: 21791 RVA: 0x0026A760 File Offset: 0x00268960
		public void UpdateUI()
		{
			if (this.transferComputationError)
			{
				this.DisplayFailureReport("An error occured while computing transfers.");
				return;
			}
			if (this.transferResult == null && !this.isChangeTrajectory && (this.candidateTrajectories == null || this.candidateTrajectories.Length == 0))
			{
				return;
			}
			if (this.transferResult != null && this.transferResult.Result != TransferResult.Outcome.Success)
			{
				double num;
				if (this.transferResult.TryGetMinimumAccelerationNeeded(out num, (double)this.Actor.cruiseAcceleration_mps2))
				{
					double num2 = Mathd.Round(num * 10000.0 / 9.806650161743164 + 0.5) / 10.0;
					this.DisplayFailureReport(Loc.T("UI.TransferResult.Fail_InsufficientAcceleration", new object[] { num2 }));
				}
				else if (this.transferResult.Result == TransferResult.Outcome.Fail_AttemptedFleetInterceptThatWouldCauseTargetingLoop)
				{
					string text = "UI.TransferResult.Fail_Loop";
					object[] array = new object[1];
					int num3 = 0;
					TISpaceFleetState tispaceFleetState = this.actor as TISpaceFleetState;
					array[num3] = ((tispaceFleetState != null) ? tispaceFleetState.GetDisplayName(GameControl.control.activePlayer) : null) ?? "";
					this.DisplayFailureReport(Loc.T(text, array));
				}
				else
				{
					this.DisplayFailureReport(this.transferResult.ToString());
				}
			}
			else if (base.gameObject.activeSelf && this.Actor != null && this.Target != null && this.Target.exists && this.candidateTrajectories != null)
			{
				this.availDV = (double)this.Actor.currentDeltaV_mps / 1000.0;
				this.availDVstr = this.availDV.ToString(TIUtilities.DecimalPlaces(this.availDV, 7, 0));
				TIInputManager.blockSelectionRaycasts = true;
				float num4 = this.Actor.cruiseAcceleration_mps2 / 9.80665f;
				this.accelerationText.SetText(Loc.T("UI.Operations.Acceleration", new object[] { num4.ToString(TIUtilities.DecimalPlaces((double)num4, 7, 0)) }));
				this.thrustProfileSelectionPanel.SetActive(true);
				this.thrustProfileImpossiblePanel.SetActive(false);
				this.currentTrajectory = this.candidateTrajectories[0];
				if (this.candidateTrajectories.Length > 1)
				{
					this.DVSlider.enabled = false;
					this.DVSlider.gameObject.SetActive(false);
					this.DVSlider.minValue = 0f;
					this.DVSlider.maxValue = (float)(this.candidateTrajectories.Length - 1);
					this.DVSlider.wholeNumbers = true;
					this.DVSlider.gameObject.SetActive(true);
					this.DVSlider.enabled = true;
					this.DVSlider.value = 0f;
				}
				else
				{
					this.DVSlider.gameObject.SetActive(false);
					this.DVSlider.enabled = false;
				}
				this.UpdateDVDetails();
			}
			if (this.GetHeaderString != null)
			{
				this.thrustProfilePanelHeader.text = this.GetHeaderString(this);
			}
		}

		// Token: 0x06005520 RID: 21792 RVA: 0x0026AA54 File Offset: 0x00268C54
		public void UpdateDVDetails()
		{
			if (this.Fleet != null && this.Fleet.controller.orbitTrailLink != null)
			{
				global::UnityEngine.Object.Destroy(this.Fleet.controller.orbitTrailLink);
			}
			double dv_kps = this.currentTrajectory.DV_kps;
			string text = dv_kps.ToString(TIUtilities.DecimalPlaces(dv_kps, 7, 0));
			if (dv_kps >= this.availDV * 0.8999999761581421)
			{
				text = TIUtilities.RedLine(text);
			}
			else if (dv_kps >= this.availDV * 0.5)
			{
				text = TIUtilities.YellowLine(text);
			}
			this.DVText.SetText(Loc.T("UI.Operations.DV", new object[] { text, this.availDVstr }));
			this.tripDurationText.SetText(Loc.T("UI.Operations.FlightTime", new object[] { ThrustProfileTool.DigestibleTimeStr(this.currentTrajectory.duration) }));
			int num = Mathd.FloorToInt(this.currentTrajectory.launchTime.DifferenceInSeconds(this.currentTrajectory.assignedTime));
			if (num < 60)
			{
				num = 0;
			}
			this.loiterDurationText.SetText(Loc.T("UI.Operations.Loiter", new object[] { ThrustProfileTool.DigestibleTimeStr(new TimeSpan(0, 0, num)) }));
			TIDateTime launchTime = this.currentTrajectory.launchTime;
			this.launchDateText.SetText(Loc.T("UI.Operations.LaunchDate", new object[] { launchTime.ToCustomTimeDateString() }));
			TIDateTime arrivalTime = this.currentTrajectory.arrivalTime;
			this.arrivalDateText.SetText(Loc.T("UI.Operations.ArrivalDate", new object[] { arrivalTime.ToCustomTimeDateString() }));
			if (this.isActorAFleet)
			{
				FleetTransferPlan fleetTransferPlan;
				fleetTransferPlan.fleet = this.Fleet;
				fleetTransferPlan.planningOnly = true;
				fleetTransferPlan.StartPoint = this.Actor.GetGlobalPositionAtTime(launchTime);
				fleetTransferPlan.TotalSeconds = this.currentTrajectory.duration_s;
				fleetTransferPlan.StartTime = launchTime.ExportTime();
				fleetTransferPlan.EndTime = this.currentTrajectory.arrivalTime.ExportTime();
				fleetTransferPlan.EndPoint = this.currentTrajectory.DestinationPositionAtTime(new TIDateTime(fleetTransferPlan.EndTime), this.Fleet.faction);
				fleetTransferPlan.commonBarycenter = this.currentTrajectory.commonBarycenter;
				fleetTransferPlan.TransferSegments = new List<Orbit>();
				if (this.Fleet.controller.orbitTrailLink != null)
				{
					global::UnityEngine.Object.Destroy(this.Fleet.controller.orbitTrailLink);
				}
				fleetTransferPlan.TransferSegments.Add(new Orbit
				{
					Barycenter = this.currentTrajectory.commonBarycenter.gameObjectLink.GetComponent<TIGameObjectEntity>().Entity
				}.Fill(true).FillTransferOrbit(this.currentTrajectory.fleet, this.currentTrajectory, out this.Fleet.controller.orbitTrailLink));
				this.Fleet.gameObjectLink.GetOrAdd<TransferPlanComponent>().Value = fleetTransferPlan;
			}
			if (this.isActorAFleet && this.target.isSpaceFleetState && !this.target.ref_faction.permanentAlly(this.actor.faction))
			{
				if (this.currentTrajectory.NeedsPincerToCatchTargetFleet())
				{
					this.interceptionWarning.SetText(TIUtilities.RedLine(Loc.T("UI.Operations.EnemyFleetCanEscape")));
				}
				else
				{
					this.interceptionWarning.SetText(TIUtilities.GreenLine(Loc.T("UI.Operations.EnemyFleetCantEscape")));
				}
				this.interceptionWarningObject.SetActive(true);
			}
			else
			{
				this.interceptionWarningObject.SetActive(false);
			}
			this.fleetFollowWarningObject.SetActive(this.isActorAFleet && this.target.isSpaceFleetState && this.target.ref_fleet.transferAssigned && this.target.ref_fleet.trajectory.arrivalTime > arrivalTime && this.target.ref_fleet.trajectory.fleetCruiseAcceleration_mps2 > (double)this.actor.cruiseAcceleration_mps2 * 2.0);
			if (this.isActorAFleet && this.target.isHabState && this.target.ref_factions.Contains(this.actor.faction) && this.target.ref_hab.AllowsResupply(this.actor.faction, false, false))
			{
				this.thrustPanelRepairandResupplyGameObject.SetActive(true);
			}
			else
			{
				this.thrustPanelRepairandResupplyGameObject.SetActive(false);
			}
			World.Active.GetExistingManager<OrbitTrailRendering>().TriggerForceTransferUpdate(this.Fleet);
		}

		// Token: 0x17000F12 RID: 3858
		// (get) Token: 0x06005521 RID: 21793 RVA: 0x0026AEDF File Offset: 0x002690DF
		// (set) Token: 0x06005522 RID: 21794 RVA: 0x0026AEE7 File Offset: 0x002690E7
		public TransferResult transferResult { get; private set; }

		// Token: 0x06005523 RID: 21795 RVA: 0x0026AEF0 File Offset: 0x002690F0
		public void ComputeCandidateTrajectories()
		{
			this.GenerateCandidateTrajectories();
			this.SortTrajectories();
		}

		// Token: 0x06005524 RID: 21796 RVA: 0x0026AF00 File Offset: 0x00269100
		private void GenerateCandidateTrajectories()
		{
			if (this.Actor == null || this.Target == null || this.Actor.cruiseAcceleration_mps2 <= 0f)
			{
				this.candidateTrajectories = null;
				return;
			}
			this.transferResult = null;
			try
			{
				this.transferResult = MasterTransferPlanner.RequestTrajectories(this.Actor, this.Target, 64, delegate(Trajectory[] t)
				{
					this.transferComputationError = false;
					this.candidateTrajectories = t;
				}, out this.lowestDVFound_kps, false, false, 1.0);
			}
			catch (Exception ex)
			{
				this.transferComputationError = true;
				string text = "ThrustProfileTool - Exception while requesting trajectories : ";
				string message = ex.Message;
				IEnumerable<string> enumerable = ex.StackTrace.SplitLines();
				Log.Error(text + message + ((enumerable != null) ? enumerable.ToString() : null), Array.Empty<object>());
				this.candidateTrajectories = null;
			}
		}

		// Token: 0x06005525 RID: 21797 RVA: 0x0026AFD0 File Offset: 0x002691D0
		private void SortTrajectories()
		{
			if (this.candidateTrajectories != null && this.candidateTrajectories.Length > 1)
			{
				if (this.thrustPanelSortByLaunchDateToggle.isOn)
				{
					this.SortByStartDate();
				}
				else if (this.thrustPanelSortByArrivalDateToggle.isOn)
				{
					this.SortByArrival();
				}
				else if (this.thrustPanelSortByDeltaVCostToggle.isOn)
				{
					this.SortByDeltaV();
				}
			}
			this.UpdateUI();
			if (this.onCandidateTrajectoriesComputed != null)
			{
				this.onCandidateTrajectoriesComputed();
			}
		}

		// Token: 0x06005526 RID: 21798 RVA: 0x0026B046 File Offset: 0x00269246
		public void DisplayFailureReport(string errorReport)
		{
			this.thrustProfileSelectionPanel.SetActive(false);
			this.thrustProfileImpossiblePanel.SetActive(true);
			this.thrustProfileImpossibleReportText.text = errorReport;
		}

		// Token: 0x06005527 RID: 21799 RVA: 0x0026B06C File Offset: 0x0026926C
		public void Open(IMobileAsset mobileSpaceAsset, TIGameState target, Trajectory[] precomputedTrajectories = null)
		{
			this.Actor = mobileSpaceAsset;
			if (precomputedTrajectories != null && precomputedTrajectories.Length >= 1)
			{
				this.UpdateTargetWithoutComputingNewTrajectories(target);
				this.candidateTrajectories = precomputedTrajectories;
				this.transferComputationError = false;
				this.transferResult = new TransferResult(TransferResult.Outcome.Success, 0.0, 0.0);
			}
			else
			{
				this.Target = target;
				this.GenerateCandidateTrajectories();
			}
			this.SortTrajectories();
			base.gameObject.SetActive(true);
			this.UpdateUI();
		}

		// Token: 0x06005528 RID: 21800 RVA: 0x0026B0E3 File Offset: 0x002692E3
		public void Open()
		{
			base.gameObject.SetActive(true);
			this.UpdateUI();
		}

		// Token: 0x06005529 RID: 21801 RVA: 0x0026B0F8 File Offset: 0x002692F8
		public void Close()
		{
			base.gameObject.SetActive(false);
			if (this.Fleet != null && !this.Fleet.deleted)
			{
				if (this.Fleet.controller != null && this.Fleet.controller.orbitTrailLink != null)
				{
					global::UnityEngine.Object.Destroy(this.Fleet.controller.orbitTrailLink);
				}
				if (this.Fleet.gameObjectLink != null)
				{
					if (!this.Fleet.transferAssigned)
					{
						this.Fleet.gameObjectLink.Remove<TransferPlanComponent>(true);
					}
					else
					{
						TransferPlanComponent component = this.Fleet.gameObjectLink.GetComponent<TransferPlanComponent>();
						if (this.isChangeTrajectory)
						{
							if (component != null)
							{
								component.Value.planningOnly = false;
							}
							this.Fleet.trajectory.DeTargetFleet();
							this.Fleet.LaunchFleet(false);
						}
						else if (this.Fleet.trajectory.involuntary && component != null)
						{
							this.Fleet.gameObjectLink.GetComponent<TransferPlanComponent>().Value.planningOnly = false;
						}
					}
				}
			}
			this.isChangeTrajectory = false;
		}

		// Token: 0x0600552A RID: 21802 RVA: 0x0026B234 File Offset: 0x00269434
		public void OnDVSliderChangedValue()
		{
			this.currentTrajectory = this.candidateTrajectories[(int)this.DVSlider.value];
			this.UpdateDVDetails();
		}

		// Token: 0x0600552B RID: 21803 RVA: 0x0026B255 File Offset: 0x00269455
		public void SortByStartDate()
		{
			Array.Sort<Trajectory>(this.candidateTrajectories, delegate(Trajectory a, Trajectory b)
			{
				int num = a.launchTime.CompareTo(b.launchTime);
				if (num == 0)
				{
					num = a.arrivalTime.CompareTo(b.arrivalTime);
				}
				return num;
			});
		}

		// Token: 0x0600552C RID: 21804 RVA: 0x0026B281 File Offset: 0x00269481
		public void SortByArrival()
		{
			Array.Sort<Trajectory>(this.candidateTrajectories, (Trajectory a, Trajectory b) => a.arrivalTime.CompareTo(b.arrivalTime));
		}

		// Token: 0x0600552D RID: 21805 RVA: 0x0026B2AD File Offset: 0x002694AD
		public void SortByDeltaV()
		{
			Array.Sort<Trajectory>(this.candidateTrajectories, delegate(Trajectory a, Trajectory b)
			{
				int num = a.DV_mps.CompareTo(b.DV_mps);
				if (num == 0)
				{
					num = a.duration_s.CompareTo(b.duration_s);
				}
				return num;
			});
		}

		// Token: 0x0600552E RID: 21806 RVA: 0x0026B2D9 File Offset: 0x002694D9
		public void OnSortByStartDateSelected(bool value)
		{
			if (value)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			}
			this.SortByStartDate();
			this.UpdateUI();
		}

		// Token: 0x0600552F RID: 21807 RVA: 0x0026B2F6 File Offset: 0x002694F6
		public void OnSortByArrivalDateSelected(bool value)
		{
			if (value)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			}
			this.SortByArrival();
			this.UpdateUI();
		}

		// Token: 0x06005530 RID: 21808 RVA: 0x0026B313 File Offset: 0x00269513
		public void OnSortByDeltaVSelected(bool value)
		{
			if (value)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			}
			this.SortByDeltaV();
			this.UpdateUI();
		}

		// Token: 0x06005531 RID: 21809 RVA: 0x0026B330 File Offset: 0x00269530
		public void OnAerocaptureSelected()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			this.ComputeCandidateTrajectories();
		}

		// Token: 0x06005532 RID: 21810 RVA: 0x0026B344 File Offset: 0x00269544
		public void OnFlyBySelected()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			this.ComputeCandidateTrajectories();
		}

		// Token: 0x06005533 RID: 21811 RVA: 0x0026B358 File Offset: 0x00269558
		public void OnRepairAndResupplySelected()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			bool isOn = this.thrustPanelRepairandResupplyToggle.isOn;
		}

		// Token: 0x06005534 RID: 21812 RVA: 0x0026B374 File Offset: 0x00269574
		public static string DigestibleTimeStr(TimeSpan duration)
		{
			if (duration.TotalDays < 1.0)
			{
				return Loc.T("UI.Operations.Duration_hours", new object[] { duration.TotalHours.ToString(TIUtilities.DecimalPlaces(duration.TotalHours, 7, 0)) });
			}
			if (duration.TotalDays < 7.0)
			{
				return Loc.T("UI.Operations.Duration_days", new object[] { duration.TotalDays.ToString("N2") });
			}
			return Loc.T("UI.Operations.Duration_weeks", new object[] { (duration.TotalDays / 7.0).ToString("N2") });
		}

		// Token: 0x04003B45 RID: 15173
		public GameObject thrustProfileSelectionPanel;

		// Token: 0x04003B46 RID: 15174
		public GameObject thrustProfileImpossiblePanel;

		// Token: 0x04003B47 RID: 15175
		public TMP_Text thrustProfilePanelHeader;

		// Token: 0x04003B48 RID: 15176
		public TMP_Text thrustProfileImpossibleReportText;

		// Token: 0x04003B49 RID: 15177
		public Slider DVSlider;

		// Token: 0x04003B4A RID: 15178
		public TMP_Text accelerationText;

		// Token: 0x04003B4B RID: 15179
		public TMP_Text DVText;

		// Token: 0x04003B4C RID: 15180
		public TMP_Text tripDurationText;

		// Token: 0x04003B4D RID: 15181
		public TMP_Text loiterDurationText;

		// Token: 0x04003B4E RID: 15182
		public TMP_Text launchDateText;

		// Token: 0x04003B4F RID: 15183
		public TMP_Text arrivalDateText;

		// Token: 0x04003B50 RID: 15184
		public GameObject interceptionWarningObject;

		// Token: 0x04003B51 RID: 15185
		public TMP_Text interceptionWarning;

		// Token: 0x04003B52 RID: 15186
		public GameObject fleetFollowWarningObject;

		// Token: 0x04003B53 RID: 15187
		public TMP_Text fleetFollowWarning;

		// Token: 0x04003B54 RID: 15188
		public Toggle thrustPanelSortByLaunchDateToggle;

		// Token: 0x04003B55 RID: 15189
		public Toggle thrustPanelSortByArrivalDateToggle;

		// Token: 0x04003B56 RID: 15190
		public Toggle thrustPanelSortByDeltaVCostToggle;

		// Token: 0x04003B57 RID: 15191
		public Toggle thrustPanelAllowAerocaptureToggle;

		// Token: 0x04003B58 RID: 15192
		public Toggle thrustPanelFlyByToggle;

		// Token: 0x04003B59 RID: 15193
		public GameObject thrustPanelRepairandResupplyGameObject;

		// Token: 0x04003B5A RID: 15194
		public Toggle thrustPanelRepairandResupplyToggle;

		// Token: 0x04003B5B RID: 15195
		public TMP_Text thrustPanelSortByHeaderText;

		// Token: 0x04003B5C RID: 15196
		public TMP_Text thrustPanelSortByLaunchDateText;

		// Token: 0x04003B5D RID: 15197
		public TMP_Text thrustPanelSortByArrivalDateText;

		// Token: 0x04003B5E RID: 15198
		public TMP_Text thrustPanelSortByDeltaVCostText;

		// Token: 0x04003B5F RID: 15199
		public TMP_Text thrustPanelSpecialManeuverText;

		// Token: 0x04003B60 RID: 15200
		public TMP_Text thrustProfilePanelAeroCaptureText;

		// Token: 0x04003B61 RID: 15201
		public TMP_Text thrustProfilePanelFlyByText;

		// Token: 0x04003B62 RID: 15202
		public TMP_Text thurstProfileRepairandResupplyText;

		// Token: 0x04003B63 RID: 15203
		private Trajectory currentTrajectory;

		// Token: 0x04003B64 RID: 15204
		private Trajectory[] candidateTrajectories;

		// Token: 0x04003B65 RID: 15205
		private double availDV;

		// Token: 0x04003B66 RID: 15206
		private string availDVstr;

		// Token: 0x04003B67 RID: 15207
		private double lowestDVFound_kps;

		// Token: 0x04003B68 RID: 15208
		public bool isChangeTrajectory;

		// Token: 0x04003B69 RID: 15209
		private IMobileAsset actor;

		// Token: 0x04003B6A RID: 15210
		private TIGameState target;

		// Token: 0x04003B6B RID: 15211
		public Func<ThrustProfileTool, string> GetHeaderString;

		// Token: 0x04003B6C RID: 15212
		public ThrustProfileTool.OnCandidateTrajectoriesComputed onCandidateTrajectoriesComputed;

		// Token: 0x04003B6D RID: 15213
		public bool transferComputationError;

		// Token: 0x02001183 RID: 4483
		// (Invoke) Token: 0x060087D2 RID: 34770
		public delegate void OnCandidateTrajectoriesComputed();
	}
}
