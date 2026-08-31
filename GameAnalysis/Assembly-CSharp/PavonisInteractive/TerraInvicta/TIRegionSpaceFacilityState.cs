using System;
using System.Collections.Generic;
using System.Linq;
using FullSerializer;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000773 RID: 1907
	public abstract class TIRegionSpaceFacilityState : TIRegionEntityState
	{
		// Token: 0x17000A0D RID: 2573
		// (get) Token: 0x060039FA RID: 14842 RVA: 0x00156413 File Offset: 0x00154613
		// (set) Token: 0x060039FB RID: 14843 RVA: 0x0015641B File Offset: 0x0015461B
		public SpaceFacilityType spaceFacilityType { get; private set; }

		// Token: 0x17000A0E RID: 2574
		// (get) Token: 0x060039FC RID: 14844 RVA: 0x00156424 File Offset: 0x00154624
		public override bool isRegionSpaceFacility
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000A0F RID: 2575
		// (get) Token: 0x060039FD RID: 14845 RVA: 0x00156427 File Offset: 0x00154627
		public override TIFactionState ref_faction
		{
			get
			{
				return base.region.nation.TotalOwningFaction;
			}
		}

		// Token: 0x17000A10 RID: 2576
		// (get) Token: 0x060039FE RID: 14846 RVA: 0x00156439 File Offset: 0x00154639
		public override List<TIFactionState> ref_factions
		{
			get
			{
				return base.region.nation.FactionsWithControlPoint;
			}
		}

		// Token: 0x17000A11 RID: 2577
		// (get) Token: 0x060039FF RID: 14847 RVA: 0x0015644B File Offset: 0x0015464B
		public override TIRegionSpaceFacilityState ref_regionSpaceFacility
		{
			get
			{
				return this;
			}
		}

		// Token: 0x06003A00 RID: 14848
		public abstract float GetAIValuation();

		// Token: 0x17000A12 RID: 2578
		// (get) Token: 0x06003A01 RID: 14849 RVA: 0x0015644E File Offset: 0x0015464E
		public TIRegionTemplate template
		{
			get
			{
				return this.GetMyTemplate<TIRegionTemplate>();
			}
		}

		// Token: 0x06003A02 RID: 14850 RVA: 0x00156458 File Offset: 0x00154658
		public void InitWithRegionState(SpaceFacilityType facilityType, TIRegionState regionState)
		{
			if (!this.gameStateSubjectCreated)
			{
				TIRegionTemplate template = regionState.template;
				this.templateName = template.dataName;
				base.region = regionState;
				this.spaceFacilityType = facilityType;
				this.gameStateSubjectCreated = true;
				this.displayName = this.GetDisplayName(null);
			}
		}

		// Token: 0x06003A03 RID: 14851
		public abstract int GetSize();

		// Token: 0x06003A04 RID: 14852 RVA: 0x001564A2 File Offset: 0x001546A2
		public bool UnderArmyAssault()
		{
			return base.region.armies.Any<TIArmyState>((TIArmyState x) => x.CurrentOperations().Any<OperationData>() && x.CurrentOperations()[0].target == this && x.CurrentOperations()[0].operation is AssaultSpaceFacilityOperation);
		}

		// Token: 0x04002575 RID: 9589
		[fsIgnore]
		public FacilityMarkerController FacilityMarkerController;
	}
}
