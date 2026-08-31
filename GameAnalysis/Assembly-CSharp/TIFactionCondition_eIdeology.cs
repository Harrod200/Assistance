using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000092 RID: 146
public class TIFactionCondition_eIdeology : TIFactionCondition
{
	// Token: 0x060002FA RID: 762 RVA: 0x00011C63 File Offset: 0x0000FE63
	public override string GetDescriptionPath()
	{
		return base.GetDescriptionPathWithSign();
	}

	// Token: 0x1700004F RID: 79
	// (get) Token: 0x060002FB RID: 763 RVA: 0x00011C6C File Offset: 0x0000FE6C
	public override List<string> descriptionParams
	{
		get
		{
			TIFactionState factionByIdeology = TIFactionIdeologyTemplate.GetFactionByIdeology(this.strValue.ToEnum(FactionIdeology.None));
			if (factionByIdeology != null)
			{
				return new List<string>(1) { factionByIdeology.GetDisplayName(GameControl.control.activePlayer) };
			}
			FactionIdeology ideology = this.strValue.ToEnum(FactionIdeology.None);
			List<string> list = new List<string>(1);
			TIFactionIdeologyTemplate tifactionIdeologyTemplate = TemplateManager.IterateByClass<TIFactionIdeologyTemplate>(true).FirstOrDefault<TIFactionIdeologyTemplate>((TIFactionIdeologyTemplate x) => x.ideology == ideology);
			list.Add(((tifactionIdeologyTemplate != null) ? tifactionIdeologyTemplate.ideologyStrGeneric : null) ?? "ERROR");
			return list;
		}
	}

	// Token: 0x060002FC RID: 764 RVA: 0x00011D01 File Offset: 0x0000FF01
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_faction != null && TICondition.PassesComparison(this.sign, (int)this.strValue.ToEnum(FactionIdeology.None), (int)state.ref_faction.ideology.ideology);
	}
}
