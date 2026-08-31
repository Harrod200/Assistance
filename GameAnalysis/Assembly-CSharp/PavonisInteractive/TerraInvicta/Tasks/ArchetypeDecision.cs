using System;
using System.Collections.Generic;
using System.Linq;

namespace PavonisInteractive.TerraInvicta.Tasks
{
	// Token: 0x02000939 RID: 2361
	public class ArchetypeDecision : HabSchematicDecision
	{
		// Token: 0x17000F4D RID: 3917
		// (get) Token: 0x06005A6F RID: 23151 RVA: 0x002B1B07 File Offset: 0x002AFD07
		public static IEnumerable<ArchetypeDecision.HabModuleArchetype> Archetypes
		{
			get
			{
				return (ArchetypeDecision.HabModuleArchetype[])Enum.GetValues(typeof(ArchetypeDecision.HabModuleArchetype));
			}
		}

		// Token: 0x17000F4E RID: 3918
		// (get) Token: 0x06005A70 RID: 23152 RVA: 0x002B1B1D File Offset: 0x002AFD1D
		// (set) Token: 0x06005A71 RID: 23153 RVA: 0x002B1B25 File Offset: 0x002AFD25
		public ArchetypeDecision.HabModuleArchetype Archetype { get; private set; }

		// Token: 0x17000F4F RID: 3919
		// (get) Token: 0x06005A72 RID: 23154 RVA: 0x002B1B2E File Offset: 0x002AFD2E
		// (set) Token: 0x06005A73 RID: 23155 RVA: 0x002B1B36 File Offset: 0x002AFD36
		public bool ReturnAllMatches { get; private set; }

		// Token: 0x06005A74 RID: 23156 RVA: 0x002B1B3F File Offset: 0x002AFD3F
		public ArchetypeDecision(ArchetypeDecision.HabModuleArchetype archetype, bool returnAllMatches = false)
		{
			this.Archetype = archetype;
			this.ReturnAllMatches = returnAllMatches;
		}

		// Token: 0x06005A75 RID: 23157 RVA: 0x002B1B55 File Offset: 0x002AFD55
		public ArchetypeDecision()
		{
		}

		// Token: 0x06005A76 RID: 23158 RVA: 0x002B1B60 File Offset: 0x002AFD60
		public override IEnumerable<TIHabModuleTemplate> Decide(TIFactionState faction, TIGameState location, HabSchematicOrder order)
		{
			List<TIHabModuleTemplate> list = (from x in ArchetypeDecision.GetTemplatesWithinArchetype(this.Archetype)
				where x.EverAllowedForFaction(faction)
				where x.AllowedLocation(location, location.isHabState ? location.ref_hab : null)
				where HabSchematicDecision.IsValidModule(faction, location, x, order)
				select x).ToList<TIHabModuleTemplate>();
			if (this.ReturnAllMatches || list.Count == 0)
			{
				return list;
			}
			return HabSchematicDecision.Nothing.Append(list.First<TIHabModuleTemplate>());
		}

		// Token: 0x06005A77 RID: 23159 RVA: 0x002B1BF0 File Offset: 0x002AFDF0
		public static IEnumerable<TIHabModuleTemplate> GetTemplatesWithinArchetype(ArchetypeDecision.HabModuleArchetype archetype)
		{
			List<TIHabModuleTemplate> list;
			if (ArchetypeDecision.templatesByArchetype.TryGetValue(archetype, out list))
			{
				return list;
			}
			IOrderedEnumerable<TIHabModuleTemplate> orderedEnumerable = TemplateManager.HabModuleTemplates.OrderBy<TIHabModuleTemplate, float>((TIHabModuleTemplate x) => TIUtilities.RandomFloatValue());
			Func<TIHabModuleTemplate, float> func = (TIHabModuleTemplate x) => 0f;
			FactionResource archetypeResource = FactionResource.None;
			switch (archetype)
			{
			case ArchetypeDecision.HabModuleArchetype.Core:
				list = orderedEnumerable.Where<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.coreModule).ToList<TIHabModuleTemplate>();
				func = (TIHabModuleTemplate x) => (float)x.tier;
				break;
			case ArchetypeDecision.HabModuleArchetype.Power:
				list = orderedEnumerable.Where<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.powerSource).ToList<TIHabModuleTemplate>();
				func = (TIHabModuleTemplate x) => (float)x.power;
				break;
			case ArchetypeDecision.HabModuleArchetype.Research:
				archetypeResource = FactionResource.Research;
				break;
			case ArchetypeDecision.HabModuleArchetype.ResearchCategory:
				list = orderedEnumerable.Where<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.techBonuses.Sum<TechBonus>((TechBonus x) => x.bonus) > 0f).ToList<TIHabModuleTemplate>();
				func = (TIHabModuleTemplate x) => x.techBonuses.Sum<TechBonus>((TechBonus x) => x.bonus);
				break;
			case ArchetypeDecision.HabModuleArchetype.Projects:
				archetypeResource = FactionResource.Projects;
				break;
			case ArchetypeDecision.HabModuleArchetype.MissionControl:
				archetypeResource = FactionResource.MissionControl;
				break;
			case ArchetypeDecision.HabModuleArchetype.Influence:
				archetypeResource = FactionResource.Influence;
				break;
			case ArchetypeDecision.HabModuleArchetype.Money:
				archetypeResource = FactionResource.Money;
				break;
			case ArchetypeDecision.HabModuleArchetype.Operations:
				archetypeResource = FactionResource.Operations;
				break;
			case ArchetypeDecision.HabModuleArchetype.Antimatter:
				archetypeResource = FactionResource.Antimatter;
				break;
			case ArchetypeDecision.HabModuleArchetype.Mining:
				list = orderedEnumerable.Where<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.mine).ToList<TIHabModuleTemplate>();
				func = (TIHabModuleTemplate x) => x.miningModifier;
				break;
			case ArchetypeDecision.HabModuleArchetype.Farming:
				list = orderedEnumerable.Where<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.IsFarm).ToList<TIHabModuleTemplate>();
				func = (TIHabModuleTemplate x) => x.FarmValue;
				break;
			case ArchetypeDecision.HabModuleArchetype.Shipbuilding:
				list = orderedEnumerable.Where<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.allowsShipConstruction).ToList<TIHabModuleTemplate>();
				func = (TIHabModuleTemplate x) => (float)x.tier;
				break;
			case ArchetypeDecision.HabModuleArchetype.Defense:
				list = orderedEnumerable.Where<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.spaceCombatModule).ToList<TIHabModuleTemplate>();
				func = (TIHabModuleTemplate x) => x.specialRulesValue;
				break;
			case ArchetypeDecision.HabModuleArchetype.Marines:
				list = orderedEnumerable.Where<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.specialRules.Contains(HabModuleSpecialRule.DropTroops)).ToList<TIHabModuleTemplate>();
				func = (TIHabModuleTemplate x) => (float)x.crew;
				break;
			case ArchetypeDecision.HabModuleArchetype.Construction:
				list = orderedEnumerable.Where<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.EnablesLocalFounding).ToList<TIHabModuleTemplate>();
				func = (TIHabModuleTemplate x) => 1f / x.buildTime_Days;
				break;
			case ArchetypeDecision.HabModuleArchetype.LEO:
				list = orderedEnumerable.Where<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.HasLEOBonus()).ToList<TIHabModuleTemplate>();
				func = (TIHabModuleTemplate x) => (float)x.tier;
				break;
			default:
				return Enumerable.Empty<TIHabModuleTemplate>();
			}
			if (archetypeResource != FactionResource.None)
			{
				list = orderedEnumerable.Where<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.DailyResourceIncome(archetypeResource, null, null) > 0f).ToList<TIHabModuleTemplate>();
				func = (TIHabModuleTemplate x) => x.DailyResourceIncome(archetypeResource, null, null);
			}
			if (archetypeResource == FactionResource.Research)
			{
				list = list.Union<TIHabModuleTemplate>(ArchetypeDecision.GetTemplatesWithinArchetype(ArchetypeDecision.HabModuleArchetype.Projects)).Union<TIHabModuleTemplate>(ArchetypeDecision.GetTemplatesWithinArchetype(ArchetypeDecision.HabModuleArchetype.ResearchCategory)).ToList<TIHabModuleTemplate>();
			}
			list = list.OrderByDescending<TIHabModuleTemplate, float>(func).ToList<TIHabModuleTemplate>();
			ArchetypeDecision.templatesByArchetype[archetype] = list;
			return list;
		}

		// Token: 0x06005A78 RID: 23160 RVA: 0x002B2098 File Offset: 0x002B0298
		public static void ClearTemplates()
		{
			ArchetypeDecision.templatesByArchetype.Clear();
		}

		// Token: 0x17000F50 RID: 3920
		// (get) Token: 0x06005A79 RID: 23161 RVA: 0x002B20A4 File Offset: 0x002B02A4
		public static TIHabModuleTemplate HumanOutpostCore
		{
			get
			{
				return ArchetypeDecision.GetTemplatesWithinArchetype(ArchetypeDecision.HabModuleArchetype.Core).FirstOrDefault<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.habType == HabType.Base && !x.alienModule && !x.automated && x.tier == 1);
			}
		}

		// Token: 0x17000F51 RID: 3921
		// (get) Token: 0x06005A7A RID: 23162 RVA: 0x002B20D0 File Offset: 0x002B02D0
		public static TIHabModuleTemplate HumanOutpostMine
		{
			get
			{
				return ArchetypeDecision.GetTemplatesWithinArchetype(ArchetypeDecision.HabModuleArchetype.Mining).FirstOrDefault<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.habType == HabType.Base && !x.alienModule && !x.automated && x.tier == 1);
			}
		}

		// Token: 0x04004149 RID: 16713
		private static Dictionary<ArchetypeDecision.HabModuleArchetype, List<TIHabModuleTemplate>> templatesByArchetype = new Dictionary<ArchetypeDecision.HabModuleArchetype, List<TIHabModuleTemplate>>();

		// Token: 0x020012B9 RID: 4793
		public enum HabModuleArchetype
		{
			// Token: 0x04006CC3 RID: 27843
			None,
			// Token: 0x04006CC4 RID: 27844
			Core,
			// Token: 0x04006CC5 RID: 27845
			Power,
			// Token: 0x04006CC6 RID: 27846
			Research,
			// Token: 0x04006CC7 RID: 27847
			ResearchCategory,
			// Token: 0x04006CC8 RID: 27848
			Projects,
			// Token: 0x04006CC9 RID: 27849
			MissionControl,
			// Token: 0x04006CCA RID: 27850
			Influence,
			// Token: 0x04006CCB RID: 27851
			Money,
			// Token: 0x04006CCC RID: 27852
			Operations,
			// Token: 0x04006CCD RID: 27853
			Antimatter,
			// Token: 0x04006CCE RID: 27854
			Mining,
			// Token: 0x04006CCF RID: 27855
			Farming,
			// Token: 0x04006CD0 RID: 27856
			Shipbuilding,
			// Token: 0x04006CD1 RID: 27857
			Defense,
			// Token: 0x04006CD2 RID: 27858
			Marines,
			// Token: 0x04006CD3 RID: 27859
			Construction,
			// Token: 0x04006CD4 RID: 27860
			LEO
		}
	}
}
