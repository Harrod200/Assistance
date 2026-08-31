using System;
using System.Collections.Generic;
using System.Linq;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020007AC RID: 1964
	public class TISectorState : TIGameState
	{
		// Token: 0x17000C2D RID: 3117
		// (get) Token: 0x060041B5 RID: 16821 RVA: 0x001A76FE File Offset: 0x001A58FE
		// (set) Token: 0x060041B6 RID: 16822 RVA: 0x001A7706 File Offset: 0x001A5906
		public TIFactionState faction { get; private set; }

		// Token: 0x17000C2E RID: 3118
		// (get) Token: 0x060041B7 RID: 16823 RVA: 0x001A770F File Offset: 0x001A590F
		public override TIFactionState ref_faction
		{
			get
			{
				return this.faction;
			}
		}

		// Token: 0x17000C2F RID: 3119
		// (get) Token: 0x060041B8 RID: 16824 RVA: 0x001A7717 File Offset: 0x001A5917
		public override TIHabState ref_hab
		{
			get
			{
				return this.hab;
			}
		}

		// Token: 0x17000C30 RID: 3120
		// (get) Token: 0x060041B9 RID: 16825 RVA: 0x001A771F File Offset: 0x001A591F
		public override TIHabSiteState ref_habSite
		{
			get
			{
				return this.hab.habSite;
			}
		}

		// Token: 0x17000C31 RID: 3121
		// (get) Token: 0x060041BA RID: 16826 RVA: 0x001A772C File Offset: 0x001A592C
		public override TIOrbitState ref_orbit
		{
			get
			{
				return this.hab.ref_orbit;
			}
		}

		// Token: 0x17000C32 RID: 3122
		// (get) Token: 0x060041BB RID: 16827 RVA: 0x001A7739 File Offset: 0x001A5939
		public override TISpaceBodyState ref_spaceBody
		{
			get
			{
				return this.hab.ref_spaceBody;
			}
		}

		// Token: 0x17000C33 RID: 3123
		// (get) Token: 0x060041BC RID: 16828 RVA: 0x001A7746 File Offset: 0x001A5946
		public override TISpaceObjectState ref_spaceObject
		{
			get
			{
				if (!this.hab.IsBase)
				{
					return this.hab;
				}
				return this.hab.ref_spaceBody;
			}
		}

		// Token: 0x17000C34 RID: 3124
		// (get) Token: 0x060041BD RID: 16829 RVA: 0x001A7767 File Offset: 0x001A5967
		public override TINaturalSpaceObjectState ref_naturalSpaceObject
		{
			get
			{
				if (!this.hab.IsBase)
				{
					return this.hab.barycenter;
				}
				return this.hab.ref_spaceBody;
			}
		}

		// Token: 0x17000C35 RID: 3125
		// (get) Token: 0x060041BE RID: 16830 RVA: 0x001A778D File Offset: 0x001A598D
		public override TISpaceAssetState ref_spaceAsset
		{
			get
			{
				return this.hab;
			}
		}

		// Token: 0x17000C36 RID: 3126
		// (get) Token: 0x060041BF RID: 16831 RVA: 0x001A7795 File Offset: 0x001A5995
		public override bool hasMapObject
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000C37 RID: 3127
		// (get) Token: 0x060041C0 RID: 16832 RVA: 0x001A7798 File Offset: 0x001A5998
		public override bool inSpace
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000C38 RID: 3128
		// (get) Token: 0x060041C1 RID: 16833 RVA: 0x001A779B File Offset: 0x001A599B
		public bool active
		{
			get
			{
				return this.faction != null;
			}
		}

		// Token: 0x17000C39 RID: 3129
		// (get) Token: 0x060041C2 RID: 16834 RVA: 0x001A77A9 File Offset: 0x001A59A9
		public bool coreSector
		{
			get
			{
				return this.sectorNum == 0;
			}
		}

		// Token: 0x17000C3A RID: 3130
		// (get) Token: 0x060041C3 RID: 16835 RVA: 0x001A77B4 File Offset: 0x001A59B4
		public string shortSectorString
		{
			get
			{
				return Loc.T("UI.Habs.Sector", new object[] { TISectorState.sectorDisplayNum(this.sectorNum, this.hab.habType) });
			}
		}

		// Token: 0x17000C3B RID: 3131
		// (get) Token: 0x060041C4 RID: 16836 RVA: 0x001A77E4 File Offset: 0x001A59E4
		public string iconResource
		{
			get
			{
				return this.faction.template.habSectorIcon;
			}
		}

		// Token: 0x17000C3C RID: 3132
		// (get) Token: 0x060041C5 RID: 16837 RVA: 0x001A77F6 File Offset: 0x001A59F6
		public int numFunctionalModules
		{
			get
			{
				return this.habModules.Count<TIHabModuleState>((TIHabModuleState x) => x.completed && !x.destroyed);
			}
		}

		// Token: 0x060041C6 RID: 16838 RVA: 0x001A7822 File Offset: 0x001A5A22
		public List<TIHabModuleState> AllModules()
		{
			return this.habModules.Where<TIHabModuleState>((TIHabModuleState x) => !x.empty).ToList<TIHabModuleState>();
		}

		// Token: 0x060041C7 RID: 16839 RVA: 0x001A7853 File Offset: 0x001A5A53
		public List<TIHabModuleState> CompletedModules()
		{
			return this.habModules.Where<TIHabModuleState>((TIHabModuleState x) => x.completed).ToList<TIHabModuleState>();
		}

		// Token: 0x060041C8 RID: 16840 RVA: 0x001A7884 File Offset: 0x001A5A84
		public List<TIHabModuleState> OkayModules()
		{
			return this.habModules.Where<TIHabModuleState>((TIHabModuleState x) => x.okay).ToList<TIHabModuleState>();
		}

		// Token: 0x060041C9 RID: 16841 RVA: 0x001A78B5 File Offset: 0x001A5AB5
		public List<TIHabModuleState> FunctionalModules()
		{
			return this.habModules.Where<TIHabModuleState>((TIHabModuleState x) => x.functional).ToList<TIHabModuleState>();
		}

		// Token: 0x060041CA RID: 16842 RVA: 0x001A78E6 File Offset: 0x001A5AE6
		public List<TIHabModuleState> ActiveModules()
		{
			return this.habModules.Where<TIHabModuleState>((TIHabModuleState x) => x.active).ToList<TIHabModuleState>();
		}

		// Token: 0x060041CB RID: 16843 RVA: 0x001A7917 File Offset: 0x001A5B17
		public List<TIHabModuleState> UnpoweredModules()
		{
			return (from x in this.OkayModules()
				where !x.powered
				select x).ToList<TIHabModuleState>();
		}

		// Token: 0x060041CC RID: 16844 RVA: 0x001A7948 File Offset: 0x001A5B48
		public List<TIHabModuleState> ActiveCombatModules()
		{
			return (from x in this.ActiveModules()
				where x.moduleTemplate.spaceCombatModule
				select x).ToList<TIHabModuleState>();
		}

		// Token: 0x060041CD RID: 16845 RVA: 0x001A797C File Offset: 0x001A5B7C
		public static int sectorDisplayNum(int sectorNum, HabType habType)
		{
			if (habType == HabType.Station)
			{
				switch (sectorNum)
				{
				case 0:
					return 1;
				case 1:
					return 4;
				case 2:
					return 3;
				case 3:
					return 5;
				case 4:
					return 2;
				}
			}
			else
			{
				switch (sectorNum)
				{
				case 0:
					return 1;
				case 1:
					return 3;
				case 2:
					return 2;
				case 3:
					return 5;
				case 4:
					return 4;
				}
			}
			return -1;
		}

		// Token: 0x060041CE RID: 16846 RVA: 0x001A79DC File Offset: 0x001A5BDC
		public void SetDisplayName()
		{
			this.displayName = Loc.T("UI.Habs.HabNameAndSector", new object[]
			{
				this.hab.displayName,
				TISectorState.sectorDisplayNum(this.sectorNum, this.hab.habType)
			});
		}

		// Token: 0x060041CF RID: 16847 RVA: 0x001A7A2C File Offset: 0x001A5C2C
		public void SetFaction(TIFactionState newFaction)
		{
			if (this.faction != null && this.faction.AISavingTarget.active)
			{
				TIGameState location = this.faction.AISavingTarget.location;
				TIGameState tigameState;
				if (location == null)
				{
					tigameState = null;
				}
				else
				{
					TIHabModuleState ref_habModule = location.ref_habModule;
					tigameState = ((ref_habModule != null) ? ref_habModule.sector : null);
				}
				if (tigameState == this)
				{
					this.faction.AIClearSavingTarget("Hab sector changed hands");
				}
			}
			TIFactionState faction = this.faction;
			if (newFaction != faction)
			{
				this.faction = newFaction;
				if (this.sectorNum == 0)
				{
					TIHabState tihabState = this.hab;
					if (tihabState != null)
					{
						tihabState.SetFaction(this.faction);
					}
				}
				if (faction != null)
				{
					faction.habSectors.Remove(this);
				}
				if (newFaction != null)
				{
					newFaction.habSectors.Add(this);
				}
				if (this.habModules != null && faction != null)
				{
					foreach (TIHabModuleState tihabModuleState in this.AllModules())
					{
						TIHabModuleTemplate moduleTemplate = tihabModuleState.moduleTemplate;
						if (moduleTemplate != null && moduleTemplate.allowsShipConstruction)
						{
							faction.RemoveShipyardFromFaction(tihabModuleState, false);
							if (tihabModuleState.completed)
							{
								newFaction.AddShipyardToFaction(tihabModuleState, false);
							}
						}
					}
				}
				if (faction != null)
				{
					faction.CheckforHabProjectUnlock();
				}
				if (newFaction != null)
				{
					newFaction.CheckforHabProjectUnlock();
				}
				if (this.sectorNum == 0)
				{
					TIHabState tihabState2 = this.hab;
					if (tihabState2 != null)
					{
						tihabState2.ResetIcon();
					}
				}
				TIHabState tihabState3 = this.hab;
				if (tihabState3 != null)
				{
					tihabState3.UpdateCurrentAnnualNetResourceIncomes(false);
				}
				if (newFaction != null)
				{
					newFaction.SetResourceIncomeDataDirty();
				}
				if (faction != null)
				{
					faction.SetResourceIncomeDataDirty();
				}
				EventManager eventManager = GameControl.eventManager;
				GameEvent gameEvent = new SectorAssignedToFaction(this);
				string text = null;
				object[] array = new object[5];
				array[0] = this;
				array[1] = this.hab;
				array[2] = faction;
				array[3] = newFaction;
				int num = 4;
				TIHabState tihabState4 = this.hab;
				array[num] = ((tihabState4 != null) ? tihabState4.ref_habSite : null);
				eventManager.TriggerEvent(gameEvent, text, array.Where<object>((object x) => x != null).ToArray<object>());
			}
		}

		// Token: 0x060041D0 RID: 16848 RVA: 0x001A7C28 File Offset: 0x001A5E28
		public bool ValidModuleForSlot(TIHabModuleTemplate module, int slot)
		{
			if (this.habModules[slot].moduleTemplate == module)
			{
				return false;
			}
			if (this.habModules[slot].decommissioning)
			{
				return false;
			}
			if (this.hab.staticHab && (!this.habModules[slot].destroyed || this.habModules[slot].priorModuleTemplate != module))
			{
				return false;
			}
			if (module.coreModule)
			{
				return this.coreSector && slot == 0 && module.tier <= this.hab.maxTier;
			}
			if (module.mine)
			{
				return this.hab.IsBase && this.hab.tier >= module.tier && this.coreSector && slot == 1;
			}
			return (!this.hab.OnlyUpgradeAllowed(module) || module.OnFutureOrPastUpgradePath(this.habModules[slot].moduleTemplate)) && (module.tier <= this.hab.tier && (!this.coreSector || (this.hab.IsBase && slot >= 2) || (this.hab.IsStation && slot >= 1)));
		}

		// Token: 0x060041D1 RID: 16849 RVA: 0x001A7D64 File Offset: 0x001A5F64
		public bool HasAnyModules()
		{
			using (List<TIHabModuleState>.Enumerator enumerator = this.habModules.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (!enumerator.Current.empty)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x060041D2 RID: 16850 RVA: 0x001A7DC0 File Offset: 0x001A5FC0
		public bool HasAnyFunctionalModules(bool skipCoreModule = false)
		{
			foreach (TIHabModuleState tihabModuleState in this.habModules)
			{
				if (!tihabModuleState.empty && !tihabModuleState.destroyed && (!skipCoreModule || !tihabModuleState.moduleTemplate.coreModule))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060041D3 RID: 16851 RVA: 0x001A7E34 File Offset: 0x001A6034
		public bool HasAnyOuterRingModules()
		{
			return this.sectorNum > 0 && (this.habModules[0].hasModule || this.habModules[1].hasModule || this.habModules[3].hasModule);
		}

		// Token: 0x060041D4 RID: 16852 RVA: 0x001A7E85 File Offset: 0x001A6085
		public bool HasAnyWingModules()
		{
			return this.sectorNum > 0 && (this.habModules[1].hasModule || this.habModules[3].hasModule);
		}

		// Token: 0x060041D5 RID: 16853 RVA: 0x001A7EB8 File Offset: 0x001A60B8
		public static TIHabModuleState UpdateModuleConnectorMap(TIHabState hab, TIHabModuleState m)
		{
			m.C0 = (m.N1 = (m.S1 = (m.E1 = (m.W1 = (m.N2 = (m.S2 = (m.E2 = (m.W2 = true))))))));
			if (m.hasModule)
			{
				m.C0 = false;
				if (m.moduleTemplate.tier >= 2)
				{
					m.N1 = (m.S1 = (m.E1 = (m.W1 = false)));
				}
				if (m.moduleTemplate.tier >= 3)
				{
					m.N2 = (m.S2 = (m.E2 = (m.W2 = false)));
				}
			}
			if (m.empty || m.moduleTemplate.tier < 3)
			{
				bool hasModule = m.hasModule;
				if (hab.IsStation)
				{
					bool flag = hab.tier >= 3 && hab.sectors[1].HasAnyModules();
					bool flag2 = hab.tier >= 2 && hab.sectors[2].HasAnyModules();
					bool flag3 = hab.tier >= 3 && hab.sectors[3].HasAnyModules();
					bool flag4 = hab.tier >= 2 && hab.sectors[4].HasAnyModules();
					switch (m.sectorNum)
					{
					case 0:
						switch (m.slot)
						{
						case 0:
							m.N1 &= hab.sectors[0].habModules[1].hasModule || flag;
							m.N2 &= hab.sectors[0].habModules[1].hasModule || flag;
							m.E1 &= hab.sectors[0].habModules[2].hasModule || flag2;
							m.E2 &= hab.sectors[0].habModules[2].hasModule || flag2;
							m.S1 &= hab.sectors[0].habModules[3].hasModule || flag3;
							m.S2 &= hab.sectors[0].habModules[3].hasModule || flag3;
							m.W1 &= hab.sectors[0].habModules[4].hasModule || flag4;
							m.W2 &= hab.sectors[0].habModules[4].hasModule || flag4;
							break;
						case 1:
						{
							m.C0 = m.C0 && flag;
							m.N1 = m.N1 && flag;
							m.N2 = m.N2 && flag;
							m.S1 &= hasModule || flag;
							m.S2 &= hasModule || flag;
							bool e = m.E1;
							m.E1 = false;
							bool e2 = m.E2;
							m.E2 = false;
							bool w = m.W1;
							m.W1 = false;
							bool w2 = m.W2;
							m.W2 = false;
							break;
						}
						case 2:
						{
							m.C0 = m.C0 && flag2;
							bool n = m.N1;
							m.N1 = false;
							bool n2 = m.N2;
							m.N2 = false;
							bool s = m.S1;
							m.S1 = false;
							bool s2 = m.S2;
							m.S2 = false;
							m.E1 = m.E1 && flag2;
							m.E2 = m.E2 && flag2;
							m.W1 &= hasModule || flag2;
							m.W2 &= hasModule || flag2;
							break;
						}
						case 3:
						{
							m.C0 = m.C0 && flag3;
							m.N1 &= hasModule || flag3;
							m.N2 &= hasModule || flag3;
							m.S1 = m.S1 && flag3;
							m.S2 = m.S2 && flag3;
							bool e3 = m.E1;
							m.E1 = false;
							bool e4 = m.E2;
							m.E2 = false;
							bool w3 = m.W1;
							m.W1 = false;
							bool w4 = m.W2;
							m.W2 = false;
							break;
						}
						case 4:
						{
							m.C0 = m.C0 && flag4;
							bool n3 = m.N1;
							m.N1 = false;
							bool n4 = m.N2;
							m.N2 = false;
							bool s3 = m.S1;
							m.S1 = false;
							bool s4 = m.S2;
							m.S2 = false;
							m.E1 &= hasModule || flag4;
							m.E2 &= hasModule || flag4;
							m.W1 = m.W1 && flag4;
							m.W2 = m.W2 && flag4;
							break;
						}
						}
						break;
					case 1:
					{
						bool flag5 = hab.sectors[1].HasAnyOuterRingModules();
						bool flag6 = hab.sectors[1].HasAnyWingModules();
						switch (m.slot)
						{
						case 0:
						{
							m.C0 = m.C0 && flag6;
							bool n5 = m.N1;
							m.N1 = false;
							bool n6 = m.N2;
							m.N2 = false;
							m.S1 = m.S1 && flag5;
							m.S2 = m.S2 && flag5;
							m.E1 &= hab.sectors[1].habModules[1].hasModule;
							m.E2 &= hab.sectors[1].habModules[1].hasModule;
							m.W1 &= hab.sectors[1].habModules[3].hasModule;
							m.W2 &= hab.sectors[1].habModules[3].hasModule;
							break;
						}
						case 1:
						{
							bool c = m.C0;
							m.C0 = false;
							bool n7 = m.N1;
							m.N1 = false;
							bool n8 = m.N2;
							m.N2 = false;
							bool s5 = m.S1;
							m.S1 = false;
							bool s6 = m.S2;
							m.S2 = false;
							m.E1 &= hasModule && hab.sectors[2].habModules[3].hasModule;
							m.E2 &= hasModule && hab.sectors[2].habModules[3].hasModule;
							m.W1 = m.W1 && hasModule;
							m.W2 = m.W2 && hasModule;
							break;
						}
						case 2:
						{
							m.C0 = m.C0 && flag5;
							m.N1 = m.N1 && flag5;
							m.N2 = m.N2 && flag5;
							m.S1 = m.S1 && flag;
							m.S2 = m.S2 && flag;
							bool e5 = m.E1;
							m.E1 = false;
							bool e6 = m.E2;
							m.E2 = false;
							bool w5 = m.W1;
							m.W1 = false;
							bool w6 = m.W2;
							m.W2 = false;
							break;
						}
						case 3:
						{
							bool c2 = m.C0;
							m.C0 = false;
							bool n9 = m.N1;
							m.N1 = false;
							bool n10 = m.N2;
							m.N2 = false;
							bool s7 = m.S1;
							m.S1 = false;
							bool s8 = m.S2;
							m.S2 = false;
							m.W1 &= hasModule && hab.sectors[4].habModules[1].hasModule;
							m.W2 &= hasModule && hab.sectors[4].habModules[1].hasModule;
							m.E1 = m.E1 && hasModule;
							m.E2 = m.E2 && hasModule;
							break;
						}
						}
						break;
					}
					case 2:
					{
						bool flag7 = hab.sectors[2].HasAnyOuterRingModules();
						bool flag8 = hab.sectors[2].HasAnyWingModules();
						switch (m.slot)
						{
						case 0:
						{
							m.C0 = m.C0 && flag8;
							m.N1 &= hab.sectors[2].habModules[3].hasModule;
							m.N2 &= hab.sectors[2].habModules[3].hasModule;
							m.S1 &= hab.sectors[2].habModules[1].hasModule;
							m.S2 &= hab.sectors[2].habModules[1].hasModule;
							bool e7 = m.E1;
							m.E1 = false;
							bool e8 = m.E2;
							m.E2 = false;
							m.W1 = m.W1 && flag7;
							m.W2 = m.W2 && flag7;
							break;
						}
						case 1:
						{
							bool c3 = m.C0;
							m.C0 = false;
							m.N1 = m.N1 && hasModule;
							m.N2 = m.N2 && hasModule;
							m.S1 &= hasModule && hab.sectors[3].habModules[3].hasModule;
							m.S2 &= hasModule && hab.sectors[3].habModules[3].hasModule;
							bool e9 = m.E1;
							m.E1 = false;
							bool e10 = m.E2;
							m.E2 = false;
							bool w7 = m.W1;
							m.W1 = false;
							bool w8 = m.W2;
							m.W2 = false;
							break;
						}
						case 2:
						{
							m.C0 = m.C0 && flag7;
							bool n11 = m.N1;
							m.N1 = false;
							bool n12 = m.N2;
							m.N2 = false;
							bool s9 = m.S1;
							m.S1 = false;
							bool s10 = m.S2;
							m.S2 = false;
							m.E1 = m.E1 && flag7;
							m.E2 = m.E2 && flag7;
							m.W1 = m.W1 && flag2;
							m.W2 = m.W2 && flag2;
							break;
						}
						case 3:
						{
							bool c4 = m.C0;
							m.C0 = false;
							m.S1 = m.S1 && hasModule;
							m.S2 = m.S2 && hasModule;
							m.N1 &= hasModule && hab.sectors[1].habModules[1].hasModule;
							m.N2 &= hasModule && hab.sectors[1].habModules[1].hasModule;
							bool e11 = m.E1;
							m.E1 = false;
							bool e12 = m.E2;
							m.E2 = false;
							bool w9 = m.W1;
							m.W1 = false;
							bool w10 = m.W2;
							m.W2 = false;
							break;
						}
						}
						break;
					}
					case 3:
					{
						bool flag9 = hab.sectors[3].HasAnyOuterRingModules();
						bool flag10 = hab.sectors[3].HasAnyWingModules();
						switch (m.slot)
						{
						case 0:
						{
							m.C0 = m.C0 && flag10;
							bool s11 = m.S1;
							m.S1 = false;
							bool s12 = m.S2;
							m.S2 = false;
							m.N1 = m.N1 && flag9;
							m.N2 = m.N2 && flag9;
							m.W1 &= hab.sectors[3].habModules[1].hasModule;
							m.W2 &= hab.sectors[3].habModules[1].hasModule;
							m.E1 &= hab.sectors[3].habModules[3].hasModule;
							m.E2 &= hab.sectors[3].habModules[3].hasModule;
							break;
						}
						case 1:
						{
							bool c5 = m.C0;
							m.C0 = false;
							bool n13 = m.N1;
							m.N1 = false;
							bool n14 = m.N2;
							m.N2 = false;
							bool s13 = m.S1;
							m.S1 = false;
							bool s14 = m.S2;
							m.S2 = false;
							m.W1 &= hasModule && hab.sectors[4].habModules[3].hasModule;
							m.W2 &= hasModule && hab.sectors[4].habModules[3].hasModule;
							m.E1 = m.E1 && hasModule;
							m.E2 = m.E2 && hasModule;
							break;
						}
						case 2:
						{
							m.C0 = m.C0 && flag9;
							m.S1 = m.S1 && flag9;
							m.S2 = m.S2 && flag9;
							m.N1 = m.N1 && flag3;
							m.N2 = m.N2 && flag3;
							bool e13 = m.E1;
							m.E1 = false;
							bool e14 = m.E2;
							m.E2 = false;
							bool w11 = m.W1;
							m.W1 = false;
							bool w12 = m.W2;
							m.W2 = false;
							break;
						}
						case 3:
						{
							bool c6 = m.C0;
							m.C0 = false;
							bool n15 = m.N1;
							m.N1 = false;
							bool n16 = m.N2;
							m.N2 = false;
							bool s15 = m.S1;
							m.S1 = false;
							bool s16 = m.S2;
							m.S2 = false;
							m.E1 &= hasModule && hab.sectors[2].habModules[1].hasModule;
							m.E2 &= hasModule && hab.sectors[2].habModules[1].hasModule;
							m.W1 = m.W1 && hasModule;
							m.W2 = m.W2 && hasModule;
							break;
						}
						}
						break;
					}
					case 4:
					{
						bool flag11 = hab.sectors[4].HasAnyOuterRingModules();
						bool flag12 = hab.sectors[4].HasAnyWingModules();
						switch (m.slot)
						{
						case 0:
						{
							m.C0 = m.C0 && flag12;
							m.N1 &= hab.sectors[4].habModules[1].hasModule;
							m.N2 &= hab.sectors[4].habModules[1].hasModule;
							m.S1 &= hab.sectors[4].habModules[3].hasModule;
							m.S2 &= hab.sectors[4].habModules[3].hasModule;
							m.E1 = m.E1 && flag11;
							m.E2 = m.E2 && flag11;
							bool w13 = m.W1;
							m.W1 = false;
							bool w14 = m.W2;
							m.W2 = false;
							break;
						}
						case 1:
						{
							bool c7 = m.C0;
							m.C0 = false;
							m.S1 = m.S1 && hasModule;
							m.S2 = m.S2 && hasModule;
							m.N1 &= hasModule && hab.sectors[1].habModules[3].hasModule;
							m.N2 &= hasModule && hab.sectors[1].habModules[3].hasModule;
							bool e15 = m.E1;
							m.E1 = false;
							bool e16 = m.E2;
							m.E2 = false;
							bool w15 = m.W1;
							m.W1 = false;
							bool w16 = m.W2;
							m.W2 = false;
							break;
						}
						case 2:
						{
							m.C0 = m.C0 && flag11;
							bool n17 = m.N1;
							m.N1 = false;
							bool n18 = m.N2;
							m.N2 = false;
							bool s17 = m.S1;
							m.S1 = false;
							bool s18 = m.S2;
							m.S2 = false;
							m.W1 = m.W1 && flag11;
							m.W2 = m.W2 && flag11;
							m.E1 = m.E1 && flag4;
							m.E2 = m.E2 && flag4;
							break;
						}
						case 3:
						{
							bool c8 = m.C0;
							m.C0 = false;
							m.N1 = m.N1 && hasModule;
							m.N2 = m.N2 && hasModule;
							m.S1 &= hasModule && hab.sectors[3].habModules[1].hasModule;
							m.S2 &= hasModule && hab.sectors[3].habModules[1].hasModule;
							bool e17 = m.E1;
							m.E1 = false;
							bool e18 = m.E2;
							m.E2 = false;
							bool w17 = m.W1;
							m.W1 = false;
							bool w18 = m.W2;
							m.W2 = false;
							break;
						}
						}
						break;
					}
					}
				}
				else if (hab.IsBase)
				{
					bool flag13 = hab.tier >= 2 && hab.sectors[1].HasAnyModules();
					bool flag14 = hab.tier >= 2 && hab.sectors[2].HasAnyModules();
					bool flag15 = hab.tier >= 3 && hab.sectors[3].HasAnyModules();
					bool flag16 = hab.tier >= 3 && hab.sectors[4].HasAnyModules();
					switch (m.sectorNum)
					{
					case 0:
						switch (m.slot)
						{
						case 0:
						{
							bool c9 = m.C0;
							m.C0 = false;
							m.N1 &= hab.sectors[0].habModules[1].hasModule;
							m.N2 &= hab.sectors[0].habModules[1].hasModule;
							m.S1 &= hab.sectors[0].habModules[3].hasModule;
							m.S2 &= hab.sectors[0].habModules[3].hasModule;
							m.E1 &= hab.sectors[0].habModules[2].hasModule || flag13;
							m.E2 &= hab.sectors[0].habModules[2].hasModule || flag13;
							m.W1 &= hab.sectors[0].habModules[4].hasModule || flag14;
							m.W2 &= hab.sectors[0].habModules[4].hasModule || flag14;
							break;
						}
						case 1:
						{
							bool c10 = m.C0;
							m.C0 = false;
							bool n19 = m.N1;
							m.N1 = false;
							bool n20 = m.N2;
							m.N2 = false;
							bool e19 = m.E1;
							m.E1 = false;
							bool e20 = m.E2;
							m.E2 = false;
							bool w19 = m.W1;
							m.W1 = false;
							bool w20 = m.W2;
							m.W2 = false;
							bool s19 = m.S1;
							m.S1 = false;
							bool s20 = m.S2;
							m.S2 = false;
							break;
						}
						case 2:
						{
							m.C0 = m.C0 && flag13;
							bool n21 = m.N1;
							m.N1 = false;
							bool n22 = m.N2;
							m.N2 = false;
							m.E1 = m.E1 && flag13;
							m.E2 = m.E2 && flag13;
							m.W1 &= hasModule || flag13;
							m.W2 &= hasModule || flag13;
							bool s21 = m.S1;
							m.S1 = false;
							bool s22 = m.S2;
							m.S2 = false;
							break;
						}
						case 3:
						{
							m.C0 &= flag15 || flag16;
							m.N1 &= hasModule || flag15 || flag16;
							m.N2 &= hasModule || flag15 || flag16;
							bool e21 = m.E1;
							m.E1 = false;
							bool e22 = m.E2;
							m.E2 = false;
							bool w21 = m.W1;
							m.W1 = false;
							bool w22 = m.W2;
							m.W2 = false;
							m.S1 &= flag15 || flag16;
							m.S2 &= flag15 || flag16;
							break;
						}
						case 4:
						{
							m.C0 = m.C0 && flag14;
							bool n23 = m.N1;
							m.N1 = false;
							bool n24 = m.N2;
							m.N2 = false;
							m.W1 = m.W1 && flag14;
							m.W2 = m.W2 && flag14;
							m.E1 &= hasModule || flag14;
							m.E2 &= hasModule || flag14;
							bool s23 = m.S1;
							m.S1 = false;
							bool s24 = m.S2;
							m.S2 = false;
							break;
						}
						}
						break;
					case 1:
					{
						bool flag17 = hab.sectors[1].HasAnyOuterRingModules();
						bool flag18 = hab.sectors[1].HasAnyWingModules();
						switch (m.slot)
						{
						case 0:
						{
							m.C0 = m.C0 && flag18;
							m.N1 &= hab.sectors[1].habModules[3].hasModule;
							m.N2 &= hab.sectors[1].habModules[3].hasModule;
							m.S1 &= hab.sectors[1].habModules[1].hasModule;
							m.S2 &= hab.sectors[1].habModules[1].hasModule;
							bool e23 = m.E1;
							m.E1 = false;
							bool e24 = m.E2;
							m.E2 = false;
							m.W1 = m.W1 && flag17;
							m.W2 = m.W2 && flag17;
							break;
						}
						case 1:
						{
							bool c11 = m.C0;
							m.C0 = false;
							m.N1 = m.N1 && hasModule;
							m.N2 = m.N2 && hasModule;
							bool e25 = m.E1;
							m.E1 = false;
							bool e26 = m.E2;
							m.E2 = false;
							bool w23 = m.W1;
							m.W1 = false;
							bool w24 = m.W2;
							m.W2 = false;
							m.S1 &= hasModule && hab.sectors[3].habModules[2].hasModule;
							m.S2 &= hasModule && hab.sectors[3].habModules[2].hasModule;
							break;
						}
						case 2:
						{
							m.C0 = m.C0 && flag17;
							bool n25 = m.N1;
							m.N1 = false;
							bool n26 = m.N2;
							m.N2 = false;
							bool s25 = m.S1;
							m.S1 = false;
							bool s26 = m.S2;
							m.S2 = false;
							m.E1 = m.E1 && flag17;
							m.E2 = m.E2 && flag17;
							m.W1 &= hasModule || flag17;
							m.W2 &= hasModule || flag17;
							break;
						}
						case 3:
						{
							m.S1 = m.S1 && hasModule;
							m.S2 = m.S2 && hasModule;
							bool n27 = m.N1;
							m.N1 = false;
							bool n28 = m.N2;
							m.N2 = false;
							bool e27 = m.E1;
							m.E1 = false;
							bool e28 = m.E2;
							m.E2 = false;
							bool w25 = m.W1;
							m.W1 = false;
							bool w26 = m.W2;
							m.W2 = false;
							break;
						}
						}
						break;
					}
					case 2:
					{
						bool flag19 = hab.sectors[2].HasAnyOuterRingModules();
						bool flag20 = hab.sectors[2].HasAnyWingModules();
						switch (m.slot)
						{
						case 0:
						{
							m.C0 = m.C0 && flag20;
							m.N1 &= hab.sectors[2].habModules[1].hasModule;
							m.N2 &= hab.sectors[2].habModules[1].hasModule;
							m.S1 &= hab.sectors[2].habModules[3].hasModule;
							m.S2 &= hab.sectors[2].habModules[3].hasModule;
							bool w27 = m.W1;
							m.W1 = false;
							bool w28 = m.W2;
							m.W2 = false;
							m.E1 = m.E1 && flag19;
							m.E2 = m.E2 && flag19;
							break;
						}
						case 1:
						{
							bool c12 = m.C0;
							m.C0 = false;
							m.S1 = m.S1 && hasModule;
							m.S2 = m.S2 && hasModule;
							bool e29 = m.E1;
							m.E1 = false;
							bool e30 = m.E2;
							m.E2 = false;
							bool w29 = m.W1;
							m.W1 = false;
							bool w30 = m.W2;
							m.W2 = false;
							bool n29 = m.N1;
							m.N1 = false;
							bool n30 = m.N2;
							m.N2 = false;
							break;
						}
						case 2:
						{
							m.C0 = m.C0 && flag19;
							bool n31 = m.N1;
							m.N1 = false;
							bool n32 = m.N2;
							m.N2 = false;
							bool s27 = m.S1;
							m.S1 = false;
							bool s28 = m.S2;
							m.S2 = false;
							m.E1 &= hasModule || flag19;
							m.E2 &= hasModule || flag19;
							m.W1 = m.W1 && flag19;
							m.W2 = m.W2 && flag19;
							break;
						}
						case 3:
						{
							bool c13 = m.C0;
							m.C0 = false;
							m.S1 &= hasModule && hab.sectors[4].habModules[2].hasModule;
							m.S2 &= hasModule && hab.sectors[4].habModules[2].hasModule;
							m.N1 = m.N1 && hasModule;
							m.N2 = m.N2 && hasModule;
							bool e31 = m.E1;
							m.E1 = false;
							bool e32 = m.E2;
							m.E2 = false;
							bool w31 = m.W1;
							m.W1 = false;
							bool w32 = m.W2;
							m.W2 = false;
							break;
						}
						}
						break;
					}
					case 3:
					{
						bool flag21 = hab.sectors[3].HasAnyOuterRingModules();
						bool flag22 = hab.sectors[3].HasAnyWingModules();
						switch (m.slot)
						{
						case 0:
						{
							m.C0 = m.C0 && flag22;
							bool s29 = m.S1;
							m.S1 = false;
							bool s30 = m.S2;
							m.S2 = false;
							m.N1 = m.N1 && flag21;
							m.N2 = m.N2 && flag21;
							m.E1 &= hab.sectors[3].habModules[3].hasModule;
							m.E2 &= hab.sectors[3].habModules[3].hasModule;
							m.W1 &= hab.sectors[3].habModules[1].hasModule;
							m.W2 &= hab.sectors[3].habModules[1].hasModule;
							break;
						}
						case 1:
						{
							bool c14 = m.C0;
							m.C0 = false;
							bool n33 = m.N1;
							m.N1 = false;
							bool n34 = m.N2;
							m.N2 = false;
							bool s31 = m.S1;
							m.S1 = false;
							bool s32 = m.S2;
							m.S2 = false;
							bool w33 = m.W1;
							m.W1 = false;
							bool w34 = m.W2;
							m.W2 = false;
							m.E1 = m.E1 && hasModule;
							m.E2 = m.E2 && hasModule;
							break;
						}
						case 2:
						{
							m.C0 = m.C0 && flag21;
							bool n35 = m.N1;
							m.N1 = false;
							bool n36 = m.N2;
							m.N2 = false;
							m.S1 = m.S1 && flag21;
							m.S2 = m.S2 && flag21;
							m.E1 &= hasModule && hab.sectors[1].habModules[1].hasModule;
							m.E2 &= hasModule && hab.sectors[1].habModules[1].hasModule;
							m.W1 &= hasModule || flag21;
							m.W2 &= hasModule || flag21;
							break;
						}
						case 3:
						{
							bool c15 = m.C0;
							m.C0 = false;
							bool n37 = m.N1;
							m.N1 = false;
							bool n38 = m.N2;
							m.N2 = false;
							bool s33 = m.S1;
							m.S1 = false;
							bool s34 = m.S2;
							m.S2 = false;
							bool e33 = m.E1;
							m.E1 = false;
							bool e34 = m.E2;
							m.E2 = false;
							m.W1 = m.W1 && hasModule;
							m.W2 = m.W2 && hasModule;
							break;
						}
						}
						break;
					}
					case 4:
					{
						bool flag23 = hab.sectors[4].HasAnyOuterRingModules();
						bool flag24 = hab.sectors[4].HasAnyWingModules();
						switch (m.slot)
						{
						case 0:
						{
							m.C0 = m.C0 && flag24;
							m.N1 = m.N1 && flag23;
							m.N2 = m.N2 && flag23;
							bool s35 = m.S1;
							m.S1 = false;
							bool s36 = m.S2;
							m.S2 = false;
							m.E1 &= hab.sectors[4].habModules[3].hasModule;
							m.E2 &= hab.sectors[4].habModules[3].hasModule;
							m.W1 &= hab.sectors[4].habModules[1].hasModule;
							m.W2 &= hab.sectors[4].habModules[1].hasModule;
							break;
						}
						case 1:
						{
							bool c16 = m.C0;
							m.C0 = false;
							bool n39 = m.N1;
							m.N1 = false;
							bool n40 = m.N2;
							m.N2 = false;
							bool s37 = m.S1;
							m.S1 = false;
							bool s38 = m.S2;
							m.S2 = false;
							bool w35 = m.W1;
							m.W1 = false;
							bool w36 = m.W2;
							m.W2 = false;
							m.E1 = m.E1 && hasModule;
							m.E2 = m.E2 && hasModule;
							break;
						}
						case 2:
						{
							m.C0 = m.C0 && flag23;
							bool n41 = m.N1;
							m.N1 = false;
							bool n42 = m.N2;
							m.N2 = false;
							m.S1 = m.S1 && flag23;
							m.S2 = m.S2 && flag23;
							m.E1 = m.E1 && flag16;
							m.E2 = m.E2 && flag16;
							m.W1 &= hasModule && hab.sectors[2].habModules[3].hasModule;
							m.W2 &= hasModule && hab.sectors[2].habModules[3].hasModule;
							break;
						}
						case 3:
						{
							bool c17 = m.C0;
							m.C0 = false;
							bool n43 = m.N1;
							m.N1 = false;
							bool n44 = m.N2;
							m.N2 = false;
							bool s39 = m.S1;
							m.S1 = false;
							bool s40 = m.S2;
							m.S2 = false;
							bool e35 = m.E1;
							m.E1 = false;
							bool e36 = m.E2;
							m.E2 = false;
							m.W1 = m.W1 && hasModule;
							m.W2 = m.W2 && hasModule;
							break;
						}
						}
						break;
					}
					}
				}
			}
			return m;
		}

		// Token: 0x17000C3D RID: 3133
		// (get) Token: 0x060041D6 RID: 16854 RVA: 0x001AA1D4 File Offset: 0x001A83D4
		public int SectorPowerGeneration
		{
			get
			{
				int num = 0;
				for (int i = 0; i < this.habModules.Count; i++)
				{
					TIHabModuleState tihabModuleState = this.habModules[i];
					if (tihabModuleState.active && tihabModuleState.ModulePower() > 0)
					{
						num += tihabModuleState.ModulePower();
					}
				}
				return num;
			}
		}

		// Token: 0x060041D7 RID: 16855 RVA: 0x001AA224 File Offset: 0x001A8424
		public int SectorNetPowerValue(bool includeUnderConstruction, bool includeDeactivated)
		{
			int num = 0;
			for (int i = 0; i < this.habModules.Count; i++)
			{
				TIHabModuleState tihabModuleState = this.habModules[i];
				if (!tihabModuleState.decommissioning && (tihabModuleState.active || (includeUnderConstruction && tihabModuleState.underConstruction) || (includeDeactivated && !tihabModuleState.empty && !tihabModuleState.powered && !tihabModuleState.underConstruction)))
				{
					num += tihabModuleState.ModulePower();
				}
			}
			return num;
		}

		// Token: 0x060041D8 RID: 16856 RVA: 0x001AA298 File Offset: 0x001A8498
		public bool HasIncome(FactionResource resourceType)
		{
			for (int i = 0; i < this.habModules.Count; i++)
			{
				TIHabModuleState tihabModuleState = this.habModules[i];
				if (tihabModuleState.active && (tihabModuleState.moduleTemplate.MonthlyResourceIncome(resourceType, this.hab, this.faction) != 0f || tihabModuleState.moduleTemplate.MonthlySupportCost(resourceType, true, this.faction, this.hab) != 0f))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060041D9 RID: 16857 RVA: 0x001AA312 File Offset: 0x001A8512
		public float GetNetDailyIncomeForDisplay(FactionResource resource)
		{
			if (resource == FactionResource.Projects || resource == FactionResource.MissionControl)
			{
				return this.GetNetYearlyIncomeForDisplay(resource);
			}
			return this.GetNetYearlyIncomeForDisplay(resource) / 365.2422f;
		}

		// Token: 0x060041DA RID: 16858 RVA: 0x001AA331 File Offset: 0x001A8531
		public float GetNetMonthlyIncomeForDisplay(FactionResource resource)
		{
			if (resource == FactionResource.Projects || resource == FactionResource.MissionControl)
			{
				return this.GetNetYearlyIncomeForDisplay(resource);
			}
			return this.GetNetYearlyIncomeForDisplay(resource) / 12f;
		}

		// Token: 0x060041DB RID: 16859 RVA: 0x001AA350 File Offset: 0x001A8550
		private float GetNetYearlyIncomeForDisplay(FactionResource resourceType)
		{
			float num = 0f;
			for (int i = 0; i < this.habModules.Count; i++)
			{
				TIHabModuleState tihabModuleState = this.habModules[i];
				if (tihabModuleState.active)
				{
					num += tihabModuleState.moduleTemplate.YearlyResourceIncome(resourceType, this.hab, this.faction);
					switch (resourceType)
					{
					case FactionResource.Money:
						if (num > 0f)
						{
							num *= this.hab.AdministrationAdviserMultiplier;
						}
						break;
					case FactionResource.Research:
						num *= this.hab.ScienceAdviserMultiplier;
						break;
					case FactionResource.Water:
					case FactionResource.Volatiles:
					case FactionResource.Metals:
					case FactionResource.NobleMetals:
					case FactionResource.Fissiles:
						num *= this.hab.AdministrationAdviserMultiplier;
						break;
					}
					num -= tihabModuleState.moduleTemplate.MonthlySupportCost(resourceType, true, this.faction, this.hab) * 12f;
				}
			}
			return num;
		}

		// Token: 0x060041DC RID: 16860 RVA: 0x001AA448 File Offset: 0x001A8648
		public bool AllowsResupply_Display(bool includeInactives)
		{
			if (!includeInactives)
			{
				return this.ActiveModules().Any<TIHabModuleState>((TIHabModuleState x) => x.moduleTemplate.allowsResupply);
			}
			return this.FunctionalModules().Any<TIHabModuleState>((TIHabModuleState x) => x.moduleTemplate.allowsResupply);
		}

		// Token: 0x060041DD RID: 16861 RVA: 0x001AA4B0 File Offset: 0x001A86B0
		public bool AllowsShipConstruction_Display(bool includeInactives)
		{
			if (!includeInactives)
			{
				return this.ActiveModules().Any<TIHabModuleState>((TIHabModuleState x) => x.moduleTemplate.allowsShipConstruction);
			}
			return this.FunctionalModules().Any<TIHabModuleState>((TIHabModuleState x) => x.moduleTemplate.allowsShipConstruction);
		}

		// Token: 0x060041DE RID: 16862 RVA: 0x001AA518 File Offset: 0x001A8718
		public float SectorCombatValue_Display(bool includeInactives)
		{
			if (!includeInactives)
			{
				return this.ActiveModules().Sum<TIHabModuleState>((TIHabModuleState x) => x.SpaceCombatValue());
			}
			return this.FunctionalModules().Sum<TIHabModuleState>((TIHabModuleState x) => x.SpaceCombatValue());
		}

		// Token: 0x060041DF RID: 16863 RVA: 0x001AA580 File Offset: 0x001A8780
		public float GetNetScienceBonus_Display(bool includeInactives, TechCategory category)
		{
			if (!includeInactives)
			{
				return (from x in this.ActiveModules()
					select x.moduleTemplate).Sum<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.GetTechBonusByCategory(category));
			}
			return (from x in this.FunctionalModules()
				select x.moduleTemplate).Sum<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.GetTechBonusByCategory(category));
		}

		// Token: 0x060041E0 RID: 16864 RVA: 0x001AA614 File Offset: 0x001A8814
		public float GetModuleConstructionTimeModifier_Display(bool includeInactives = false)
		{
			float num = 1f;
			foreach (TIHabModuleState tihabModuleState in (includeInactives ? this.FunctionalModules() : this.ActiveModules()))
			{
				num *= tihabModuleState.moduleTemplate.moduleConstructionSpeedModifier;
			}
			return num;
		}

		// Token: 0x17000C3E RID: 3134
		// (get) Token: 0x060041E1 RID: 16865 RVA: 0x001AA680 File Offset: 0x001A8880
		public int controlPointCapacityValue
		{
			get
			{
				return this.ActiveModules().Sum<TIHabModuleState>((TIHabModuleState x) => x.controlPointCapacity);
			}
		}

		// Token: 0x040027B1 RID: 10161
		public int sectorNum;

		// Token: 0x040027B3 RID: 10163
		public TIHabState hab;

		// Token: 0x040027B4 RID: 10164
		public List<TIHabModuleState> habModules;

		// Token: 0x040027B5 RID: 10165
		public int slots;
	}
}
