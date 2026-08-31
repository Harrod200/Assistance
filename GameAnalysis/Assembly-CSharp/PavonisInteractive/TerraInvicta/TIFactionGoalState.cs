using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta.Tasks;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000728 RID: 1832
	public abstract class TIFactionGoalState : TIGameState
	{
		// Token: 0x17000681 RID: 1665
		// (get) Token: 0x06002D46 RID: 11590 RVA: 0x000F9A40 File Offset: 0x000F7C40
		public float Age_days
		{
			get
			{
				return (float)(TITimeState.Now() - this.assignedDate).TotalDays;
			}
		}

		// Token: 0x17000682 RID: 1666
		// (get) Token: 0x06002D47 RID: 11591 RVA: 0x000F9A66 File Offset: 0x000F7C66
		public float Age_months
		{
			get
			{
				return this.Age_days / 30.436874f;
			}
		}

		// Token: 0x17000683 RID: 1667
		// (get) Token: 0x06002D48 RID: 11592 RVA: 0x000F9A74 File Offset: 0x000F7C74
		public float Age_years
		{
			get
			{
				return this.Age_days / 365.2422f;
			}
		}

		// Token: 0x17000684 RID: 1668
		// (get) Token: 0x06002D49 RID: 11593 RVA: 0x000F9A82 File Offset: 0x000F7C82
		// (set) Token: 0x06002D4A RID: 11594 RVA: 0x000F9AA4 File Offset: 0x000F7CA4
		public virtual TIObjectiveTemplate objective
		{
			get
			{
				if (this._objective == null)
				{
					this._objective = TemplateManager.Find<TIObjectiveTemplate>(this.objectiveTemplateName, false);
				}
				return this._objective;
			}
			set
			{
				this._objective = value;
				if (this._objective != null)
				{
					this.objectiveTemplateName = this._objective.dataName;
				}
			}
		}

		// Token: 0x17000685 RID: 1669
		// (get) Token: 0x06002D4B RID: 11595 RVA: 0x000F9AC6 File Offset: 0x000F7CC6
		public bool objectiveGoal
		{
			get
			{
				return this.objective != null;
			}
		}

		// Token: 0x06002D4C RID: 11596
		public abstract GoalType GetGoalType();

		// Token: 0x06002D4D RID: 11597
		public abstract void RemoveState();

		// Token: 0x17000686 RID: 1670
		// (get) Token: 0x06002D4E RID: 11598 RVA: 0x000F9AD1 File Offset: 0x000F7CD1
		// (set) Token: 0x06002D4F RID: 11599 RVA: 0x000F9AD9 File Offset: 0x000F7CD9
		public int importance { get; private set; }

		// Token: 0x06002D50 RID: 11600
		public abstract TIGameState actor();

		// Token: 0x06002D51 RID: 11601
		public abstract TIGameState target();

		// Token: 0x06002D52 RID: 11602
		public abstract TIGameState location();

		// Token: 0x06002D53 RID: 11603
		public abstract bool ValidNewGoal();

		// Token: 0x06002D54 RID: 11604 RVA: 0x000F9AE2 File Offset: 0x000F7CE2
		public virtual bool IsDuplicate(TIFactionGoalState testGoal, TIGameState testTarget = null)
		{
			if (testGoal.GetType() != base.GetType())
			{
				return false;
			}
			if (testTarget == null)
			{
				testTarget = testGoal.target();
			}
			return this.target() == testTarget;
		}

		// Token: 0x06002D55 RID: 11605
		public abstract bool InProgress();

		// Token: 0x06002D56 RID: 11606
		public abstract bool ShouldDiscardGoal();

		// Token: 0x06002D57 RID: 11607 RVA: 0x000F9B16 File Offset: 0x000F7D16
		public virtual bool ShouldPauseGoal()
		{
			return false;
		}

		// Token: 0x06002D58 RID: 11608
		public abstract bool GoalFulfilled();

		// Token: 0x17000687 RID: 1671
		// (get) Token: 0x06002D59 RID: 11609 RVA: 0x000F9B19 File Offset: 0x000F7D19
		public bool skipGoal
		{
			get
			{
				return this.ShouldDiscardGoal() || this.GoalFulfilled();
			}
		}

		// Token: 0x06002D5A RID: 11610
		public abstract TIGameState goalProduct();

		// Token: 0x06002D5B RID: 11611
		public abstract List<TIFactionGoalState> BuildSubsequentGoals();

		// Token: 0x17000688 RID: 1672
		// (get) Token: 0x06002D5C RID: 11612
		public abstract List<GoalType> incompatibleGoals { get; }

		// Token: 0x06002D5D RID: 11613
		public abstract void ChangeTarget(TIGameState newTarget);

		// Token: 0x17000689 RID: 1673
		// (get) Token: 0x06002D5E RID: 11614 RVA: 0x000F9B2B File Offset: 0x000F7D2B
		public virtual bool PoliciesAsFactionActor
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700068A RID: 1674
		// (get) Token: 0x06002D5F RID: 11615 RVA: 0x000F9B2E File Offset: 0x000F7D2E
		public virtual List<PolicyType> policiesAsNation { get; }

		// Token: 0x1700068B RID: 1675
		// (get) Token: 0x06002D60 RID: 11616 RVA: 0x000F9B36 File Offset: 0x000F7D36
		public virtual List<PolicyType> factionLevelPoliciesAsNation { get; }

		// Token: 0x1700068C RID: 1676
		// (get) Token: 0x06002D61 RID: 11617 RVA: 0x000F9B3E File Offset: 0x000F7D3E
		public virtual List<PolicyType> policiesAtTarget { get; }

		// Token: 0x1700068D RID: 1677
		// (get) Token: 0x06002D62 RID: 11618 RVA: 0x000F9B46 File Offset: 0x000F7D46
		public virtual List<PolicyType> factionLevelPoliciesAtTarget { get; }

		// Token: 0x1700068E RID: 1678
		// (get) Token: 0x06002D63 RID: 11619 RVA: 0x000F9B4E File Offset: 0x000F7D4E
		public virtual Dictionary<string, float> missionPayoffMultipliersAgainstTarget { get; }

		// Token: 0x06002D64 RID: 11620 RVA: 0x000F9B56 File Offset: 0x000F7D56
		public virtual TIDataTemplate SavingForTemplate(TIFactionState faction, out bool alreadyOrdered, out TIHabModuleState shipyard)
		{
			alreadyOrdered = false;
			shipyard = null;
			return null;
		}

		// Token: 0x1700068F RID: 1679
		// (get) Token: 0x06002D65 RID: 11621 RVA: 0x000F9B5F File Offset: 0x000F7D5F
		public virtual bool isFleetGoal
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000690 RID: 1680
		// (get) Token: 0x06002D66 RID: 11622 RVA: 0x000F9B62 File Offset: 0x000F7D62
		public virtual FactionGoal_Fleet ref_fleetGoal
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000691 RID: 1681
		// (get) Token: 0x06002D67 RID: 11623 RVA: 0x000F9B65 File Offset: 0x000F7D65
		public virtual bool GrantMissionControlIndulgence
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06002D68 RID: 11624 RVA: 0x000F9B68 File Offset: 0x000F7D68
		public TIFactionGoalState()
		{
		}

		// Token: 0x06002D69 RID: 11625 RVA: 0x000F9B70 File Offset: 0x000F7D70
		public virtual void OnGoalAssigned()
		{
		}

		// Token: 0x06002D6A RID: 11626 RVA: 0x000F9B72 File Offset: 0x000F7D72
		public virtual void DailyGoalMaintenance()
		{
		}

		// Token: 0x06002D6B RID: 11627 RVA: 0x000F9B74 File Offset: 0x000F7D74
		public virtual void OnGoalRemoved()
		{
		}

		// Token: 0x06002D6C RID: 11628 RVA: 0x000F9B76 File Offset: 0x000F7D76
		public virtual void OnGoalDiscarded()
		{
		}

		// Token: 0x06002D6D RID: 11629 RVA: 0x000F9B78 File Offset: 0x000F7D78
		public virtual void OnGoalComplete()
		{
			this.faction.RemoveGoal(this);
			List<GoalType> list = this.subsequentGoals;
			if (list != null && list.Count > 0)
			{
				IEnumerable<TIFactionGoalState> enumerable = this.BuildSubsequentGoals();
				foreach (TIFactionGoalState tifactionGoalState in (enumerable ?? Enumerable.Empty<TIFactionGoalState>()))
				{
					if (tifactionGoalState != null)
					{
						this.faction.AddGoal(tifactionGoalState, HandleDuplicateGoalRule.ResetImportance, null);
					}
				}
			}
		}

		// Token: 0x06002D6E RID: 11630 RVA: 0x000F9C04 File Offset: 0x000F7E04
		public void SetImportance(int importance_)
		{
			int importance = this.importance;
			this.importance = Mathf.Clamp(importance_, 0, 20);
			if (importance != this.importance)
			{
				this.faction.factionGoals[this.GetGoalType()] = (from x in this.faction.factionGoals[this.GetGoalType()]
					orderby x.importance descending, x.assignedDate
					select x).ToList<TIFactionGoalState>();
				if (this.isFleetGoal)
				{
					List<ShipConstructionQueueItem> list = new List<ShipConstructionQueueItem>();
					foreach (List<ShipConstructionQueueItem> list2 in this.faction.nShipyardQueues.Values)
					{
						foreach (ShipConstructionQueueItem shipConstructionQueueItem in list2)
						{
							if (shipConstructionQueueItem.AIFactionGoal == this)
							{
								list.Add(shipConstructionQueueItem);
							}
						}
					}
					foreach (ShipConstructionQueueItem shipConstructionQueueItem2 in list)
					{
						AIDailyFactionPlanner.AdjustShipyardQueue(this.faction, shipConstructionQueueItem2);
					}
				}
			}
		}

		// Token: 0x06002D6F RID: 11631 RVA: 0x000F9D94 File Offset: 0x000F7F94
		public void ChangeImportance(int delta, int min = 1, int max = 20)
		{
			if (this.importance + delta < min)
			{
				this.SetImportance(min);
				return;
			}
			if (this.importance + delta > max)
			{
				this.SetImportance(max);
				return;
			}
			this.SetImportance(this.importance + delta);
		}

		// Token: 0x06002D70 RID: 11632 RVA: 0x000F9DCA File Offset: 0x000F7FCA
		public float FractionalImportance(float minValue = 0f)
		{
			return Mathf.Clamp((float)this.importance / 20f, minValue, 1f);
		}

		// Token: 0x06002D71 RID: 11633 RVA: 0x000F9DE4 File Offset: 0x000F7FE4
		public override void SetDisplayName(string name)
		{
			this.displayName = this.GetGoalType().ToString();
		}

		// Token: 0x17000692 RID: 1682
		// (get) Token: 0x06002D72 RID: 11634 RVA: 0x000F9E0C File Offset: 0x000F800C
		public string description
		{
			get
			{
				string[] array = new string[10];
				array[0] = this.GetGoalType().ToString();
				array[1] = " Imp: ";
				array[2] = this.importance.ToString();
				array[3] = " Act: ";
				int num = 4;
				TIGameState tigameState = this.actor();
				array[num] = ((tigameState != null) ? tigameState.GetDisplayName(GameControl.control.activePlayer) : null) ?? "none";
				array[5] = " Tar: ";
				int num2 = 6;
				TIGameState tigameState2 = this.target();
				array[num2] = ((tigameState2 != null) ? tigameState2.GetDisplayName(GameControl.control.activePlayer) : null) ?? "none";
				array[7] = ((this.target() != null && this.target().isHabSiteState) ? (", " + this.target().ref_habSite.ref_naturalSpaceObject.displayName) : "");
				array[8] = " InProgress: ";
				array[9] = this.InProgress().ToString();
				return string.Concat(array);
			}
		}

		// Token: 0x06002D73 RID: 11635 RVA: 0x000F9F13 File Offset: 0x000F8113
		public float GetMissionPayoffMultiplier(TIMissionTemplate mission, float defaultMultiplier = 1f)
		{
			if (this.missionPayoffMultipliersAgainstTarget.ContainsKey(mission.dataName))
			{
				return this.missionPayoffMultipliersAgainstTarget[mission.dataName] * (float)this.importance;
			}
			return defaultMultiplier;
		}

		// Token: 0x06002D74 RID: 11636 RVA: 0x000F9F43 File Offset: 0x000F8143
		public virtual bool FoundHabGoal()
		{
			return false;
		}

		// Token: 0x06002D75 RID: 11637 RVA: 0x000F9F46 File Offset: 0x000F8146
		public virtual bool BuildHabGoal()
		{
			return false;
		}

		// Token: 0x06002D76 RID: 11638 RVA: 0x000F9F49 File Offset: 0x000F8149
		public virtual bool NationMissionModifyingGoal()
		{
			return false;
		}

		// Token: 0x06002D77 RID: 11639 RVA: 0x000F9F4C File Offset: 0x000F814C
		public virtual bool FactionMissionModifyingGoal()
		{
			return false;
		}

		// Token: 0x06002D78 RID: 11640 RVA: 0x000F9F4F File Offset: 0x000F814F
		public virtual bool PoliciesAsNationGoal()
		{
			return false;
		}

		// Token: 0x06002D79 RID: 11641 RVA: 0x000F9F52 File Offset: 0x000F8152
		public virtual bool PoliciesAtTargetNationGoal()
		{
			return false;
		}

		// Token: 0x06002D7A RID: 11642 RVA: 0x000F9F55 File Offset: 0x000F8155
		public virtual bool NationPrioritiesGoal()
		{
			return false;
		}

		// Token: 0x040021D5 RID: 8661
		public const int forceDiscardGoalImportance = 0;

		// Token: 0x040021D6 RID: 8662
		public const int minimalGoalImportance = 1;

		// Token: 0x040021D7 RID: 8663
		public const int lowGoalImportance = 5;

		// Token: 0x040021D8 RID: 8664
		public const int regularGoalImportance = 10;

		// Token: 0x040021D9 RID: 8665
		public const int highGoalImportance = 15;

		// Token: 0x040021DA RID: 8666
		public const int maxGoalImportance = 20;

		// Token: 0x040021DB RID: 8667
		public const float IdealFleetSuperiorityFactor = 1.4f;

		// Token: 0x040021DC RID: 8668
		public TIFactionState faction;

		// Token: 0x040021DD RID: 8669
		public TIDateTime assignedDate;

		// Token: 0x040021DE RID: 8670
		[SerializeField]
		private string objectiveTemplateName;

		// Token: 0x040021DF RID: 8671
		private TIObjectiveTemplate _objective;

		// Token: 0x040021E1 RID: 8673
		public List<GoalType> subsequentGoals;

		// Token: 0x040021E7 RID: 8679
		public static readonly List<GoalType> FoundStationGoals = new List<GoalType>
		{
			GoalType.FoundStation,
			GoalType.FoundPlatform,
			GoalType.FoundMaxStation,
			GoalType.FoundSurveillanceStation
		};

		// Token: 0x040021E8 RID: 8680
		public static readonly List<GoalType> FoundHabGoals = new List<GoalType>
		{
			GoalType.FoundPlatform,
			GoalType.FoundBase,
			GoalType.FoundMaxStation,
			GoalType.FoundSurveillanceStation
		};

		// Token: 0x040021E9 RID: 8681
		public static readonly List<GoalType> BuildHabGoals = new List<GoalType>
		{
			GoalType.BuildFullStation,
			GoalType.BuildFullBase,
			GoalType.BuildMiningBase,
			GoalType.BuildRefuellingStation,
			GoalType.BuildSpecialtyBase,
			GoalType.BuildSpecialtyStation
		};

		// Token: 0x040021EA RID: 8682
		public static readonly List<GoalType> CaptureNationGoals = new List<GoalType>
		{
			GoalType.CaptureNationClean,
			GoalType.CaptureNationDirty
		};

		// Token: 0x040021EB RID: 8683
		public static readonly List<GoalType> NationMissionModifyingGoals = new List<GoalType>
		{
			GoalType.CaptureNationClean,
			GoalType.CaptureNationDirty,
			GoalType.ExpandNation,
			GoalType.DevelopNation,
			GoalType.NeutralizeNation,
			GoalType.PillageNation,
			GoalType.WarOnFaction,
			GoalType.TruceWithFaction,
			GoalType.NonAggressionPact,
			GoalType.SpaceifyNation,
			GoalType.MilitarizeNation
		};

		// Token: 0x040021EC RID: 8684
		public static readonly List<GoalType> FactionMissionModifyingGoals = new List<GoalType>
		{
			GoalType.WarOnFaction,
			GoalType.TruceWithFaction,
			GoalType.NonAggressionPact,
			GoalType.CaptureHab
		};

		// Token: 0x040021ED RID: 8685
		public static readonly List<GoalType> FactionOnFactionGoals = new List<GoalType>
		{
			GoalType.WarOnFaction,
			GoalType.TruceWithFaction,
			GoalType.NonAggressionPact
		};

		// Token: 0x040021EE RID: 8686
		public static readonly List<GoalType> NationPriorityModifyingGoals = new List<GoalType>
		{
			GoalType.CaptureNationClean,
			GoalType.CaptureNationDirty,
			GoalType.NeutralizeNation,
			GoalType.PillageNation,
			GoalType.DevelopNation,
			GoalType.ExpandNation,
			GoalType.SpaceifyNation,
			GoalType.MilitarizeNation
		};

		// Token: 0x040021EF RID: 8687
		public static readonly List<GoalType> NationManagementGoals = new List<GoalType>
		{
			GoalType.PillageNation,
			GoalType.DevelopNation,
			GoalType.ExpandNation,
			GoalType.SpaceifyNation,
			GoalType.MilitarizeNation
		};

		// Token: 0x040021F0 RID: 8688
		public static readonly List<GoalType> BenevolentNationManagementGoals = new List<GoalType>
		{
			GoalType.DevelopNation,
			GoalType.ExpandNation,
			GoalType.SpaceifyNation,
			GoalType.MilitarizeNation
		};

		// Token: 0x040021F1 RID: 8689
		public static readonly List<GoalType> UnificationAllowedManagementGoals = new List<GoalType>
		{
			GoalType.DevelopNation,
			GoalType.ExpandNation,
			GoalType.SpaceifyNation,
			GoalType.MilitarizeNation
		};

		// Token: 0x040021F2 RID: 8690
		public static readonly List<GoalType> OffensiveFleetGoals = new List<GoalType>
		{
			GoalType.AttackWithFleet,
			GoalType.CaptureHab
		};
	}
}
