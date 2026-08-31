using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x020003F2 RID: 1010
public class TISpaceFleetTemplate : TISpaceAssetTemplate
{
	// Token: 0x170002AD RID: 685
	// (get) Token: 0x060013EE RID: 5102 RVA: 0x0005D942 File Offset: 0x0005BB42
	public TIFactionTemplate factionTemplate
	{
		get
		{
			return TemplateManager.Find<TIFactionTemplate>(this.factionName, false);
		}
	}

	// Token: 0x170002AE RID: 686
	// (get) Token: 0x060013EF RID: 5103 RVA: 0x0005D950 File Offset: 0x0005BB50
	public Formation defaultFormation
	{
		get
		{
			if (TemplateManager.Find<TIFormationTemplate>(this.formationName, false) == null)
			{
				this.formationName = "Convoy";
			}
			return new Formation(this.formationName, this.formationFocus, FormationSpacing.Loose, this.formationConcentration);
		}
	}

	// Token: 0x060013F0 RID: 5104 RVA: 0x0005D984 File Offset: 0x0005BB84
	public override TIGameState CreateGameState()
	{
		this.objectType = SpaceObjectType.Fleet;
		this.modelResource = "ships/FleetContainer";
		this.modelScale = 1000f;
		TIGameState tigameState = base.CreateGameState();
		if (tigameState == null)
		{
			tigameState = GameStateManager.CreateNewGameState<TISpaceFleetState>();
		}
		return tigameState;
	}

	// Token: 0x060013F1 RID: 5105 RVA: 0x0005D9C5 File Offset: 0x0005BBC5
	public TISpaceFleetTemplate(string dataNameToSet)
	{
		base.dataName = dataNameToSet;
	}

	// Token: 0x170002AF RID: 687
	// (get) Token: 0x060013F2 RID: 5106 RVA: 0x0005D9D4 File Offset: 0x0005BBD4
	public List<TISpaceFleetTemplate.ShipFleetDefinition> filteredShipsInFleet
	{
		get
		{
			return this.shipsInFleet.Where<TISpaceFleetTemplate.ShipFleetDefinition>((TISpaceFleetTemplate.ShipFleetDefinition x) => !string.IsNullOrEmpty(x.shipTemplateName)).ToList<TISpaceFleetTemplate.ShipFleetDefinition>();
		}
	}

	// Token: 0x040011FB RID: 4603
	public List<TISpaceFleetTemplate.ShipFleetDefinition> shipsInFleet;

	// Token: 0x040011FC RID: 4604
	public string factionName;

	// Token: 0x040011FD RID: 4605
	public FormationSpacing formationSpacing;

	// Token: 0x040011FE RID: 4606
	public string formationName;

	// Token: 0x040011FF RID: 4607
	public FormationConcentration formationConcentration;

	// Token: 0x04001200 RID: 4608
	public FormationFocus formationFocus;

	// Token: 0x02000BEA RID: 3050
	public struct ShipFleetDefinition
	{
		// Token: 0x06006AA7 RID: 27303 RVA: 0x00303BCC File Offset: 0x00301DCC
		public ShipFleetDefinition(string shipTemplateName)
		{
			this.shipTemplateName = shipTemplateName;
		}

		// Token: 0x04004C77 RID: 19575
		public string shipTemplateName;
	}
}
