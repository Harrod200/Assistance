using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Tasks;

// Token: 0x02000187 RID: 391
public class TIHabSchematicTemplate : TIDataTemplate
{
	// Token: 0x170000E1 RID: 225
	// (get) Token: 0x060005E1 RID: 1505 RVA: 0x0001B4C7 File Offset: 0x000196C7
	public IEnumerable<ArchetypeDecision.HabModuleArchetype> DecisionArchetypes
	{
		get
		{
			return this.decisions.Select<string, ArchetypeDecision.HabModuleArchetype>((string x) => ArchetypeDecision.Archetypes.FirstOrDefault<ArchetypeDecision.HabModuleArchetype>((ArchetypeDecision.HabModuleArchetype y) => x == y.ToString()));
		}
	}

	// Token: 0x170000E2 RID: 226
	// (get) Token: 0x060005E2 RID: 1506 RVA: 0x0001B4F4 File Offset: 0x000196F4
	public HabSchematic HabSchematic
	{
		get
		{
			return new HabSchematic(this.DecisionArchetypes.Select<ArchetypeDecision.HabModuleArchetype, HabSchematicDecision>(delegate(ArchetypeDecision.HabModuleArchetype x)
			{
				if (x == ArchetypeDecision.HabModuleArchetype.None)
				{
					return new WildCardDecision();
				}
				return new ScoreDecision(x);
			}), this, this.preferences.Normalized().Scaled(this.relativeValue));
		}
	}

	// Token: 0x060005E3 RID: 1507 RVA: 0x0001B547 File Offset: 0x00019747
	public bool AvailableToFaction(TIFactionState faction)
	{
		return this.factionDataName == null || this.factionDataName == "" || faction.template.dataName == this.factionDataName;
	}

	// Token: 0x04000613 RID: 1555
	public List<string> decisions;

	// Token: 0x04000614 RID: 1556
	public HabPreferences preferences;

	// Token: 0x04000615 RID: 1557
	public string factionDataName;

	// Token: 0x04000616 RID: 1558
	public float relativeValue = 1f;
}
