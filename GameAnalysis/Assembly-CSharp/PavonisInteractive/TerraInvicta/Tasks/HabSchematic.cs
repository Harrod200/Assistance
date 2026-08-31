using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.Tasks
{
	// Token: 0x0200093D RID: 2365
	public class HabSchematic
	{
		// Token: 0x17000F52 RID: 3922
		// (get) Token: 0x06005A85 RID: 23173 RVA: 0x002B2651 File Offset: 0x002B0851
		// (set) Token: 0x06005A86 RID: 23174 RVA: 0x002B2659 File Offset: 0x002B0859
		public TIHabSchematicTemplate Template { get; private set; }

		// Token: 0x06005A87 RID: 23175 RVA: 0x002B2662 File Offset: 0x002B0862
		public HabSchematic(TIHabSchematicTemplate template, HabPreferences preferences, params HabSchematicDecision[] decisions)
		{
			this.Template = template;
			this.Decisions = decisions.ToList<HabSchematicDecision>();
			this.Preferences = preferences.Copy();
		}

		// Token: 0x06005A88 RID: 23176 RVA: 0x002B2694 File Offset: 0x002B0894
		public HabSchematic(params HabSchematicDecision[] decisions)
		{
			this.Decisions = decisions.ToList<HabSchematicDecision>();
		}

		// Token: 0x06005A89 RID: 23177 RVA: 0x002B26B3 File Offset: 0x002B08B3
		public HabSchematic(IEnumerable<HabSchematicDecision> decisions, TIHabSchematicTemplate template = null, HabPreferences preferences = null)
		{
			this.Template = template;
			this.Decisions = decisions.ToList<HabSchematicDecision>();
			if (preferences != null)
			{
				this.Preferences = preferences.Copy();
			}
		}

		// Token: 0x06005A8A RID: 23178 RVA: 0x002B26E8 File Offset: 0x002B08E8
		public HabSchematic(IEnumerable<HabSchematicDecision> decisions, TIHabSchematicTemplate template = null)
		{
			this.Template = template;
			this.Decisions = decisions.ToList<HabSchematicDecision>();
		}

		// Token: 0x06005A8B RID: 23179 RVA: 0x002B270E File Offset: 0x002B090E
		public HabSchematic()
		{
			this.Decisions = new List<HabSchematicDecision>();
		}

		// Token: 0x06005A8C RID: 23180 RVA: 0x002B272C File Offset: 0x002B092C
		public HabSchematicOrder GetOrder(TIFactionState faction, TIGameState location, bool useImagination = false, bool useExistingModules = true, IEnumerable<TIHabModuleTemplate> forcedModules = null)
		{
			HabSchematic.<>c__DisplayClass11_0 CS$<>8__locals1 = new HabSchematic.<>c__DisplayClass11_0();
			CS$<>8__locals1.useExistingModules = useExistingModules;
			CS$<>8__locals1.useImagination = useImagination;
			CS$<>8__locals1.location = location;
			CS$<>8__locals1.faction = faction;
			if (!CS$<>8__locals1.location.isHabState)
			{
				CS$<>8__locals1.useImagination = true;
				CS$<>8__locals1.useExistingModules = false;
			}
			CS$<>8__locals1.order = new HabSchematicOrder(this.Preferences.Copy(), null);
			List<TIHabModuleTemplate> list = new List<TIHabModuleTemplate>();
			if (CS$<>8__locals1.location.isHabState)
			{
				list = (from x in CS$<>8__locals1.location.ref_hab.OkayModules()
					select x.moduleTemplate into x
					where CS$<>8__locals1.useExistingModules || (!CS$<>8__locals1.useImagination && x.coreModule)
					select x).ToList<TIHabModuleTemplate>();
				CS$<>8__locals1.order.AddRange(list.Where<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.coreModule));
				CS$<>8__locals1.order.AddRange(list.Where<TIHabModuleTemplate>((TIHabModuleTemplate x) => !x.coreModule));
			}
			CS$<>8__locals1.GetAvailableSlotCount = delegate
			{
				int num5;
				if (CS$<>8__locals1.location.isHabState && !CS$<>8__locals1.useImagination)
				{
					num5 = CS$<>8__locals1.location.ref_hab.sectors.Where<TISectorState>((TISectorState x) => x.active).Sum<TISectorState>((TISectorState x) => x.slots);
				}
				else
				{
					TIHabModuleTemplate tihabModuleTemplate5 = CS$<>8__locals1.order.FirstOrDefault<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.coreModule);
					if (tihabModuleTemplate5 == null)
					{
						return 0;
					}
					num5 = tihabModuleTemplate5.slotsProvided + 1;
				}
				if (CS$<>8__locals1.location.ref_habSite != null)
				{
					if (CS$<>8__locals1.order.None<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.mine))
					{
						num5--;
					}
				}
				return num5 - CS$<>8__locals1.order.Count;
			};
			CS$<>8__locals1.GetExcessPower = delegate
			{
				IEnumerable<TIHabModuleTemplate> order = CS$<>8__locals1.order;
				Func<TIHabModuleTemplate, int> func7;
				if ((func7 = CS$<>8__locals1.<>9__16) == null)
				{
					func7 = (CS$<>8__locals1.<>9__16 = delegate(TIHabModuleTemplate moduleTemplate)
					{
						int num6 = moduleTemplate.ProspectivePower(CS$<>8__locals1.location, CS$<>8__locals1.faction);
						if (moduleTemplate.powerSource & CS$<>8__locals1.useImagination)
						{
							return Mathf.Max(num6, 10);
						}
						return num6;
					});
				}
				return order.Sum<TIHabModuleTemplate>(func7);
			};
			HabSchematicDecision habSchematicDecision = new CoreDecision();
			HabSchematicDecision habSchematicDecision2 = new ArchetypeDecision(ArchetypeDecision.HabModuleArchetype.Mining, false);
			HabSchematicDecision habSchematicDecision3 = new ConstructionDecision();
			IEnumerable<HabSchematicDecision> enumerable = Enumerable.Empty<HabSchematicDecision>();
			if (forcedModules != null)
			{
				enumerable = enumerable.Concat<HabSchematicDecision>(forcedModules.Select<TIHabModuleTemplate, NotADecision>((TIHabModuleTemplate x) => new NotADecision(x, true)));
			}
			if (CS$<>8__locals1.useImagination)
			{
				enumerable = enumerable.Append(habSchematicDecision);
			}
			enumerable = enumerable.Append(habSchematicDecision2).Append(habSchematicDecision3);
			int num = enumerable.Count<HabSchematicDecision>() * 4;
			int num2 = 0;
			bool flag = false;
			if (CS$<>8__locals1.location.ref_habSite != null)
			{
				flag = CS$<>8__locals1.location.ref_spaceBody.habSites.Count<TIHabSiteState>() > 4 || CS$<>8__locals1.location.ref_system.habsInSystem.Any<TIHabState>((TIHabState x) => CS$<>8__locals1.faction.permanentAlly(x.faction));
				if (flag)
				{
					TIHabState ref_hab = CS$<>8__locals1.location.ref_hab;
					bool flag2;
					if (ref_hab == null)
					{
						flag2 = false;
					}
					else
					{
						flag2 = ref_hab.OkayModules().Any<TIHabModuleState>((TIHabModuleState x) => x.moduleTemplate.CombatTroops());
					}
					if (!flag2)
					{
						ArchetypeDecision archetypeDecision = new ArchetypeDecision(ArchetypeDecision.HabModuleArchetype.Marines, false);
						enumerable = enumerable.Append(archetypeDecision);
					}
				}
			}
			int num3 = 0;
			if (CS$<>8__locals1.useExistingModules)
			{
				num3 = CS$<>8__locals1.location.ref_hab.OkayModules().Count<TIHabModuleState>((TIHabModuleState x) => !x.moduleTemplate.coreModule && !x.PowerProvider() && !x.moduleTemplate.IsFarm && !x.moduleTemplate.mine);
			}
			if (CS$<>8__locals1.location.ref_hab != null)
			{
				if (CS$<>8__locals1.location.ref_hab.OkayModules().Any<TIHabModuleState>((TIHabModuleState x) => x.moduleTemplate.constructionModule && x.tier == 1))
				{
					num3--;
				}
			}
			List<HabSchematicDecision> list2 = this.Decisions.ToList<HabSchematicDecision>();
			int num4 = 0;
			while (num4 < num3 && list2.Count > 0)
			{
				list2.RemoveAt(0);
				num4++;
			}
			if (flag)
			{
				HabSchematicDecision habSchematicDecision4 = list2.SelectSansNulls<HabSchematicDecision, ArchetypeDecision>((HabSchematicDecision x) => x as ArchetypeDecision).FirstOrDefault<ArchetypeDecision>((ArchetypeDecision x) => x.Archetype == ArchetypeDecision.HabModuleArchetype.Marines);
				if (habSchematicDecision4 == null)
				{
					habSchematicDecision4 = (from x in list2.SelectSansNulls<HabSchematicDecision, ScoreDecision>((HabSchematicDecision x) => x as ScoreDecision)
						where x.Decisions.Count<HabSchematicDecision>() == 1
						select x).Where<ScoreDecision>(delegate(ScoreDecision x)
					{
						ArchetypeDecision archetypeDecision2 = x.Decisions.First<HabSchematicDecision>() as ArchetypeDecision;
						return archetypeDecision2 != null && archetypeDecision2.Archetype == ArchetypeDecision.HabModuleArchetype.Marines;
					}).FirstOrDefault<ScoreDecision>();
				}
				if (habSchematicDecision4 != null)
				{
					list2.Remove(habSchematicDecision4);
				}
			}
			enumerable = enumerable.Concat<HabSchematicDecision>(list2);
			enumerable = enumerable.Concat<HabSchematicDecision>(Enumerable.Repeat<WildCardDecision>(new WildCardDecision(), 20));
			enumerable = enumerable.SelectMany<HabSchematicDecision, HabSchematicDecision>((HabSchematicDecision x) => Enumerable.Empty<HabSchematicDecision>().Append(x).Append(new FarmDecision()));
			enumerable = enumerable.SelectMany<HabSchematicDecision, HabSchematicDecision>((HabSchematicDecision x) => Enumerable.Empty<HabSchematicDecision>().Append(x).Append(new PowerDecision()));
			using (IEnumerator<HabSchematicDecision> enumerator = enumerable.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					HabSchematicDecision habSchematicDecision5 = enumerator.Current;
					foreach (TIHabModuleTemplate tihabModuleTemplate in habSchematicDecision5.Decide(CS$<>8__locals1.faction, CS$<>8__locals1.location, CS$<>8__locals1.order))
					{
						CS$<>8__locals1.order.Add(tihabModuleTemplate);
					}
					if (++num2 >= num && CS$<>8__locals1.GetAvailableSlotCount() <= 0 && CS$<>8__locals1.GetExcessPower() >= 0)
					{
						HabSchematic.<>c__DisplayClass11_1 CS$<>8__locals2 = new HabSchematic.<>c__DisplayClass11_1();
						CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
						HabSchematic.<>c__DisplayClass11_1 CS$<>8__locals3 = CS$<>8__locals2;
						IEnumerable<TIHabModuleTemplate> enumerable2 = CS$<>8__locals2.CS$<>8__locals1.order.Where<TIHabModuleTemplate>((TIHabModuleTemplate x) => !x.coreModule);
						Func<TIHabModuleTemplate, int> func;
						if ((func = CS$<>8__locals2.CS$<>8__locals1.<>9__24) == null)
						{
							func = (CS$<>8__locals2.CS$<>8__locals1.<>9__24 = (TIHabModuleTemplate x) => Mathf.Abs(x.ProspectivePower(CS$<>8__locals2.CS$<>8__locals1.location, CS$<>8__locals2.CS$<>8__locals1.faction)));
						}
						CS$<>8__locals3.removableModules = (from x in enumerable2.OrderBy<TIHabModuleTemplate, int>(func)
							orderby x.mine, !x.powerSource, x.IsFarm
							select x).ToList<TIHabModuleTemplate>();
						foreach (TIHabModuleTemplate tihabModuleTemplate2 in list)
						{
							CS$<>8__locals2.removableModules.Remove(tihabModuleTemplate2);
						}
						HabSchematic.<>c__DisplayClass11_1 CS$<>8__locals4 = CS$<>8__locals2;
						Func<TIHabModuleTemplate, bool> func2;
						if ((func2 = CS$<>8__locals2.CS$<>8__locals1.<>9__28) == null)
						{
							func2 = (CS$<>8__locals2.CS$<>8__locals1.<>9__28 = (TIHabModuleTemplate x) => x.powerSource && CS$<>8__locals2.CS$<>8__locals1.GetExcessPower() - x.ProspectivePower(CS$<>8__locals2.CS$<>8__locals1.location, CS$<>8__locals2.CS$<>8__locals1.faction) < 0);
						}
						CS$<>8__locals4.IsRequiredForPower = func2;
						HabSchematic.<>c__DisplayClass11_1 CS$<>8__locals5 = CS$<>8__locals2;
						Func<TIHabModuleTemplate, bool> func3;
						if ((func3 = CS$<>8__locals2.CS$<>8__locals1.<>9__29) == null)
						{
							func3 = (CS$<>8__locals2.CS$<>8__locals1.<>9__29 = delegate(TIHabModuleTemplate x)
							{
								if (x.EnablesLocalFounding)
								{
									return CS$<>8__locals2.CS$<>8__locals1.order.Count<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.EnablesLocalFounding) == 1;
								}
								return false;
							});
						}
						CS$<>8__locals5.IsRequiredForLocalFounding = func3;
						Func<bool> func4 = delegate
						{
							int num7 = CS$<>8__locals2.CS$<>8__locals1.GetAvailableSlotCount();
							if (num7 < 0)
							{
								return true;
							}
							int num8 = CS$<>8__locals2.CS$<>8__locals1.GetExcessPower();
							if (num8 < 0)
							{
								return true;
							}
							TIHabModuleTemplate tihabModuleTemplate6 = CS$<>8__locals2.removableModules.FirstOrDefault<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.powerSource);
							if (tihabModuleTemplate6 != null)
							{
								float num9 = (float)tihabModuleTemplate6.ProspectivePower(CS$<>8__locals2.CS$<>8__locals1.location, CS$<>8__locals2.CS$<>8__locals1.faction);
								if ((num7 == 0 && num9 < (float)num8) || num9 * 2f < (float)num8)
								{
									return true;
								}
							}
							return false;
						};
						while (func4())
						{
							IEnumerable<TIHabModuleTemplate> removableModules = CS$<>8__locals2.removableModules;
							Func<TIHabModuleTemplate, bool> func5;
							if ((func5 = CS$<>8__locals2.<>9__33) == null)
							{
								func5 = (CS$<>8__locals2.<>9__33 = (TIHabModuleTemplate x) => !CS$<>8__locals2.IsRequiredForPower(x));
							}
							IOrderedEnumerable<TIHabModuleTemplate> orderedEnumerable = from x in removableModules.Where<TIHabModuleTemplate>(func5)
								orderby x.mine
								select x;
							Func<TIHabModuleTemplate, bool> func6;
							if ((func6 = CS$<>8__locals2.<>9__35) == null)
							{
								func6 = (CS$<>8__locals2.<>9__35 = (TIHabModuleTemplate x) => CS$<>8__locals2.IsRequiredForLocalFounding(x));
							}
							TIHabModuleTemplate tihabModuleTemplate3 = orderedEnumerable.ThenBy<TIHabModuleTemplate, bool>(func6).FirstOrDefault<TIHabModuleTemplate>();
							if (tihabModuleTemplate3 == null)
							{
								return new HabSchematicOrder(null, null);
							}
							CS$<>8__locals2.CS$<>8__locals1.order.Remove(tihabModuleTemplate3);
							CS$<>8__locals2.removableModules.Remove(tihabModuleTemplate3);
						}
						break;
					}
				}
				goto IL_07F8;
			}
			IL_0799:
			IEnumerable<TIHabModuleTemplate> enumerable3 = new PackingDecision().Decide(CS$<>8__locals1.faction, CS$<>8__locals1.location, CS$<>8__locals1.order);
			if (!enumerable3.Any<TIHabModuleTemplate>())
			{
				goto IL_0806;
			}
			foreach (TIHabModuleTemplate tihabModuleTemplate4 in enumerable3)
			{
				CS$<>8__locals1.order.Add(tihabModuleTemplate4);
			}
			IL_07F8:
			if (CS$<>8__locals1.GetAvailableSlotCount() > 0)
			{
				goto IL_0799;
			}
			IL_0806:
			return CS$<>8__locals1.order;
		}

		// Token: 0x06005A8D RID: 23181 RVA: 0x002B2FAC File Offset: 0x002B11AC
		public static HabSchematic SelectHabSchematic(TIFactionState faction, TIGameState location, out HabSchematicOrder order, Func<FactionResource, float> GetMonthlyIncome = null)
		{
			List<HabSchematic> list = new List<HabSchematic>();
			IEnumerable<HabSchematic> enumerable = faction.HabSchematics.Except<HabSchematic>(list);
			Func<HabSchematic, int> <>9__6;
			for (int i = 0; i < 2; i++)
			{
				IEnumerable<HabSchematic> enumerable2 = enumerable;
				Func<HabSchematic, int> func;
				if ((func = <>9__6) == null)
				{
					func = (<>9__6 = (HabSchematic schematic) => (from hab in faction.habs
						where hab.HabSchematic != null && hab.HabSchematicAssignedDate != null
						orderby hab.HabSchematicAssignedDate
						select hab).ToList<TIHabState>().Take<TIHabState>(5).Count<TIHabState>((TIHabState hab) => hab.HabSchematic == schematic));
				}
				HabSchematic habSchematic = enumerable2.MaxBy<HabSchematic, int>(func);
				if (habSchematic != null)
				{
					list.Add(habSchematic);
				}
			}
			list.AddRange(enumerable.Take_Random<HabSchematic>(1));
			HabSchematic habSchematic2 = faction.HabSchematics.MaxBy<HabSchematic, int>((HabSchematic x) => x.Template.DecisionArchetypes.Count<ArchetypeDecision.HabModuleArchetype>((ArchetypeDecision.HabModuleArchetype x) => x == ArchetypeDecision.HabModuleArchetype.Shipbuilding));
			if (!list.Contains(habSchematic2))
			{
				list.Add(habSchematic2);
			}
			Dictionary<HabSchematic, HabSchematicOrder> dictionary = list.ToDictionary<HabSchematic, HabSchematic, HabSchematicOrder>((HabSchematic x) => x, (HabSchematic x) => x.GetOrder(faction, location, true, false, null));
			HabSchematic key = dictionary.ToDictionary<KeyValuePair<HabSchematic, HabSchematicOrder>, HabSchematic, float>((KeyValuePair<HabSchematic, HabSchematicOrder> x) => x.Key, (KeyValuePair<HabSchematic, HabSchematicOrder> x) => x.Value.Score(faction, location, GetMonthlyIncome, false, true)).MaxBy<KeyValuePair<HabSchematic, float>, float>((KeyValuePair<HabSchematic, float> x) => x.Value).Key;
			order = dictionary[key];
			return key;
		}

		// Token: 0x06005A8E RID: 23182 RVA: 0x002B3120 File Offset: 0x002B1320
		public static HabSchematic SelectHabSchematic(TIFactionState faction, TIGameState location, Func<FactionResource, float> GetMonthlyIncome = null)
		{
			HabSchematicOrder habSchematicOrder;
			return HabSchematic.SelectHabSchematic(faction, location, out habSchematicOrder, GetMonthlyIncome);
		}

		// Token: 0x06005A8F RID: 23183 RVA: 0x002B3138 File Offset: 0x002B1338
		public static HabSchematicOrder GetOrderWithoutHabSchematic(TIFactionState faction, TIGameState location, Func<FactionResource, float> GetMonthlyIncome = null)
		{
			HabSchematicOrder habSchematicOrder;
			HabSchematic.SelectHabSchematic(faction, location, out habSchematicOrder, GetMonthlyIncome);
			return habSchematicOrder;
		}

		// Token: 0x04004150 RID: 16720
		public List<HabSchematicDecision> Decisions;

		// Token: 0x04004151 RID: 16721
		public HabPreferences Preferences = new HabPreferences();
	}
}
