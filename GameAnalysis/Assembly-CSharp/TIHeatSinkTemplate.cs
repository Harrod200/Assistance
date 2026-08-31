using System;
using System.Collections.Generic;
using System.Text;
using PavonisInteractive.TerraInvicta;

// Token: 0x020003D3 RID: 979
public class TIHeatSinkTemplate : TIShipModuleTemplate
{
	// Token: 0x1700023F RID: 575
	// (get) Token: 0x060012C4 RID: 4804 RVA: 0x000596BB File Offset: 0x000578BB
	public override List<ShipModuleSlotType> allowedSlots
	{
		get
		{
			return new List<ShipModuleSlotType> { ShipModuleSlotType.Utility };
		}
	}

	// Token: 0x17000240 RID: 576
	// (get) Token: 0x060012C5 RID: 4805 RVA: 0x000596C9 File Offset: 0x000578C9
	public override TIHeatSinkTemplate ref_heatSink
	{
		get
		{
			return this;
		}
	}

	// Token: 0x17000241 RID: 577
	// (get) Token: 0x060012C6 RID: 4806 RVA: 0x000596CC File Offset: 0x000578CC
	public override bool isHeatSink
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060012C7 RID: 4807 RVA: 0x000596D0 File Offset: 0x000578D0
	public override string GetDescriptionData(TISpaceShipState ship, TISpaceShipTemplate shipTemplate = null, bool prospective = false, ShipModuleSlotType slot = ShipModuleSlotType.None, bool splitFireModes = false)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine(this.GetLocalizedMass());
		if (this.crew > 0)
		{
			stringBuilder.AppendLine(base.GetLocalizedCrew());
		}
		stringBuilder.AppendLine(this.GetLocalizedCapacity());
		stringBuilder.AppendLine(this.GetLocalizedCost());
		return stringBuilder.ToString();
	}

	// Token: 0x060012C8 RID: 4808 RVA: 0x00059726 File Offset: 0x00057926
	public string GetLocalizedCapacity()
	{
		return Loc.T("TIHeatSinkTemplate.Capacity", new object[] { this.heatCapacity_GJ.ToString("N0") });
	}

	// Token: 0x060012C9 RID: 4809 RVA: 0x0005974B File Offset: 0x0005794B
	public override float AIScoringValueForResearch()
	{
		return this.heatCapacity_GJ * 2f / this.mass_tons;
	}

	// Token: 0x04001106 RID: 4358
	public float heatCapacity_GJ;
}
