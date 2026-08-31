using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000757 RID: 1879
	public abstract class FactionGoal_FleetCouncilorGoal : FactionGoal_Fleet
	{
		// Token: 0x17000742 RID: 1858
		// (get) Token: 0x060030EB RID: 12523 RVA: 0x001082CB File Offset: 0x001064CB
		public override bool FleetCouncilorGoal
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000743 RID: 1859
		// (get) Token: 0x060030EC RID: 12524 RVA: 0x001082CE File Offset: 0x001064CE
		// (set) Token: 0x060030ED RID: 12525 RVA: 0x001082D6 File Offset: 0x001064D6
		public TIGameState councilorDestination { get; protected set; }

		// Token: 0x17000744 RID: 1860
		// (get) Token: 0x060030EE RID: 12526 RVA: 0x001082DF File Offset: 0x001064DF
		public virtual bool WantsAdditionalCouncilors
		{
			get
			{
				return this.GetUltimateMissionOptions().Any<TIMissionTemplate>();
			}
		}

		// Token: 0x060030EF RID: 12527 RVA: 0x001082EC File Offset: 0x001064EC
		public virtual bool ShouldUnassignCouncilor(TICouncilorState councilor)
		{
			if (councilor == null || !councilor.active || councilor.archived)
			{
				return true;
			}
			IEnumerable<TIMissionTemplate> possibleMissionList = councilor.GetPossibleMissionList(false, false, true, null, false);
			IEnumerable<TIMissionTemplate> ultimateMissionOptions = this.GetUltimateMissionOptions();
			return !possibleMissionList.Intersect<TIMissionTemplate>(ultimateMissionOptions).Any<TIMissionTemplate>();
		}

		// Token: 0x060030F0 RID: 12528 RVA: 0x00108338 File Offset: 0x00106538
		public virtual TIGameState WhereShouldThisCouncilorBe(TICouncilorState councilor)
		{
			TIGameState tigameState = this.target();
			if (tigameState.ref_region != null)
			{
				return tigameState.ref_region;
			}
			if (tigameState.ref_fleet != null)
			{
				return tigameState.ref_fleet;
			}
			if (tigameState.ref_hab != null)
			{
				return tigameState.ref_hab;
			}
			return tigameState;
		}

		// Token: 0x060030F1 RID: 12529 RVA: 0x0010838C File Offset: 0x0010658C
		public virtual IEnumerable<TIMissionTemplate> GetUltimateMissionOptions()
		{
			return Enumerable.Empty<TIMissionTemplate>();
		}

		// Token: 0x060030F2 RID: 12530 RVA: 0x00108393 File Offset: 0x00106593
		public virtual TIGameState GetMissionTarget(TIMissionTemplate mission)
		{
			return this.target();
		}

		// Token: 0x060030F3 RID: 12531 RVA: 0x0010839C File Offset: 0x0010659C
		[return: TupleElementNames(new string[] { "Mission", "Target" })]
		public virtual IEnumerable<ValueTuple<TIMissionTemplate, TIGameState>> GetMissionOptions(TICouncilorState councilor)
		{
			IEnumerable<ValueTuple<TIMissionTemplate, TIGameState>> enumerable = Enumerable.Empty<ValueTuple<TIMissionTemplate, TIGameState>>();
			TIGameState tigameState = this.WhereShouldThisCouncilorBe(councilor);
			if (councilor.location == tigameState || (councilor.OnEarth && tigameState.ref_region != null))
			{
				enumerable = (from x in this.GetUltimateMissionOptions()
					select new ValueTuple<TIMissionTemplate, TIGameState>(x, this.GetMissionTarget(x))).Where<ValueTuple<TIMissionTemplate, TIGameState>>(delegate([TupleElementNames(new string[] { "Mission", "Target" })] ValueTuple<TIMissionTemplate, TIGameState> x)
				{
					List<string> list = x.Item1.target.ValidateSingleTarget(x.Item1, councilor, x.Item2);
					return x.Item1.target.ValidTarget(list);
				});
			}
			else if (councilor.OnEarth)
			{
				IList<TIGameState> validTargets = TIFactionState.orbitMission.GetValidTargets(councilor);
				if (validTargets.Contains(tigameState))
				{
					enumerable = enumerable.Append(new ValueTuple<TIMissionTemplate, TIGameState>(TIFactionState.orbitMission, tigameState));
				}
				else if (base.assignedFleet != null && validTargets.Contains(base.assignedFleet))
				{
					enumerable = enumerable.Append(new ValueTuple<TIMissionTemplate, TIGameState>(TIFactionState.orbitMission, base.assignedFleet));
				}
			}
			return enumerable;
		}

		// Token: 0x0400226F RID: 8815
		public List<TICouncilorState> assignedCouncilors = new List<TICouncilorState>();
	}
}
