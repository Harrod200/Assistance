using System;
using System.Collections.Generic;
using System.Linq;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000743 RID: 1859
	public abstract class FactionGoal_BuildHab : TIFactionGoalState
	{
		// Token: 0x17000715 RID: 1813
		// (get) Token: 0x06002F76 RID: 12150 RVA: 0x00103C0E File Offset: 0x00101E0E
		public override TIHabState ref_hab
		{
			get
			{
				return this.hab;
			}
		}

		// Token: 0x17000716 RID: 1814
		// (get) Token: 0x06002F77 RID: 12151 RVA: 0x00103C16 File Offset: 0x00101E16
		// (set) Token: 0x06002F78 RID: 12152 RVA: 0x00103C1E File Offset: 0x00101E1E
		public TIHabState hab { get; protected set; }

		// Token: 0x06002F79 RID: 12153 RVA: 0x00103C27 File Offset: 0x00101E27
		public override TIGameState actor()
		{
			return this.faction;
		}

		// Token: 0x06002F7A RID: 12154 RVA: 0x00103C2F File Offset: 0x00101E2F
		public override TIGameState target()
		{
			return this.hab;
		}

		// Token: 0x06002F7B RID: 12155 RVA: 0x00103C37 File Offset: 0x00101E37
		public override TIGameState location()
		{
			return this.hab.location;
		}

		// Token: 0x06002F7C RID: 12156 RVA: 0x00103C44 File Offset: 0x00101E44
		public override TIGameState goalProduct()
		{
			return this.hab;
		}

		// Token: 0x06002F7D RID: 12157 RVA: 0x00103C4C File Offset: 0x00101E4C
		public override bool InProgress()
		{
			return this.hab != null;
		}

		// Token: 0x06002F7E RID: 12158 RVA: 0x00103C5A File Offset: 0x00101E5A
		public override bool BuildHabGoal()
		{
			return true;
		}

		// Token: 0x17000717 RID: 1815
		// (get) Token: 0x06002F7F RID: 12159 RVA: 0x00103C5D File Offset: 0x00101E5D
		public override bool GrantMissionControlIndulgence
		{
			get
			{
				return base.objectiveGoal;
			}
		}

		// Token: 0x06002F80 RID: 12160 RVA: 0x00103C65 File Offset: 0x00101E65
		public override void ChangeTarget(TIGameState newTarget)
		{
			this.hab = ((newTarget != null) ? newTarget.ref_hab : null);
		}

		// Token: 0x06002F81 RID: 12161 RVA: 0x00103C79 File Offset: 0x00101E79
		public override List<TIFactionGoalState> BuildSubsequentGoals()
		{
			return null;
		}

		// Token: 0x06002F82 RID: 12162 RVA: 0x00103C7C File Offset: 0x00101E7C
		public virtual List<TIHabModuleTemplate> RequiredModules()
		{
			return new List<TIHabModuleTemplate>();
		}

		// Token: 0x06002F83 RID: 12163 RVA: 0x00103C83 File Offset: 0x00101E83
		public virtual List<TIHabModuleTemplate> allowedModules()
		{
			return new List<TIHabModuleTemplate>();
		}

		// Token: 0x17000718 RID: 1816
		// (get) Token: 0x06002F84 RID: 12164 RVA: 0x00103C8C File Offset: 0x00101E8C
		protected List<TIHabModuleTemplate> specialModules
		{
			get
			{
				List<TIHabModuleTemplate> list = new List<TIHabModuleTemplate>();
				foreach (string text in this.specialtyModuleDataNames)
				{
					TIHabModuleTemplate tihabModuleTemplate = TemplateManager.Find<TIHabModuleTemplate>(text, false);
					if (tihabModuleTemplate != null)
					{
						list.Add(tihabModuleTemplate);
					}
				}
				return list;
			}
		}

		// Token: 0x06002F85 RID: 12165 RVA: 0x00103CF0 File Offset: 0x00101EF0
		public override TIDataTemplate SavingForTemplate(TIFactionState faction, out bool alreadyOrdered, out TIHabModuleState shipyard)
		{
			FactionGoal_BuildHab.<>c__DisplayClass21_0 CS$<>8__locals1 = new FactionGoal_BuildHab.<>c__DisplayClass21_0();
			CS$<>8__locals1.faction = faction;
			alreadyOrdered = false;
			shipyard = null;
			if (!(this.hab != null))
			{
				return null;
			}
			List<TIHabModuleState> list = this.hab.AvailableSlots();
			List<TIHabModuleTemplate> upgradeableModuleTemplates = (from x in this.hab.OkayModules()
				where x.CanUpgrade(CS$<>8__locals1.faction)
				select x.moduleTemplate).Distinct<TIHabModuleTemplate>().ToList<TIHabModuleTemplate>();
			if (list.Count<TIHabModuleState>() <= 1)
			{
				return null;
			}
			List<TIHabModuleTemplate> existingHabModuleTemplates = (from x in this.hab.AllModules()
				select x.moduleTemplate).ToList<TIHabModuleTemplate>();
			List<TIHabModuleTemplate> allowedHabModules = this.hab.AllowedModules(CS$<>8__locals1.faction).Where<TIHabModuleTemplate>(new Func<TIHabModuleTemplate, bool>(CS$<>8__locals1.<SavingForTemplate>g__ShouldSaveFor|1)).ToList<TIHabModuleTemplate>();
			if (list.Count<TIHabModuleState>() == 0)
			{
				allowedHabModules = allowedHabModules.Where<TIHabModuleTemplate>((TIHabModuleTemplate x) => upgradeableModuleTemplates.Contains(x.UpgradesFrom)).ToList<TIHabModuleTemplate>();
			}
			IEnumerable<TIHabModuleTemplate> enumerable = from x in this.RequiredModules()
				where !existingHabModuleTemplates.Contains(x) && allowedHabModules.Contains(x)
				select x;
			TIHabModuleTemplate tihabModuleTemplate;
			if (enumerable == null)
			{
				tihabModuleTemplate = null;
			}
			else
			{
				tihabModuleTemplate = enumerable.MaxBy<TIHabModuleTemplate, int>((TIHabModuleTemplate x) => x.tier);
			}
			TIHabModuleTemplate tihabModuleTemplate2 = tihabModuleTemplate;
			if (tihabModuleTemplate2 == null && this.specialtyModuleDataNames != null)
			{
				List<TIHabModuleTemplate> specialModules = this.specialModules;
				TIHabModuleTemplate tihabModuleTemplate3;
				if (specialModules == null)
				{
					tihabModuleTemplate3 = null;
				}
				else
				{
					IEnumerable<TIHabModuleTemplate> enumerable2 = from x in specialModules.Where<TIHabModuleTemplate>(new Func<TIHabModuleTemplate, bool>(CS$<>8__locals1.<SavingForTemplate>g__ShouldSaveFor|1))
						where !existingHabModuleTemplates.Contains(x) && allowedHabModules.Contains(x)
						select x;
					if (enumerable2 == null)
					{
						tihabModuleTemplate3 = null;
					}
					else
					{
						tihabModuleTemplate3 = enumerable2.MaxBy<TIHabModuleTemplate, int>((TIHabModuleTemplate x) => x.tier);
					}
				}
				tihabModuleTemplate2 = tihabModuleTemplate3;
			}
			if (tihabModuleTemplate2 == null)
			{
				tihabModuleTemplate2 = (from x in this.allowedModules().Where<TIHabModuleTemplate>(new Func<TIHabModuleTemplate, bool>(CS$<>8__locals1.<SavingForTemplate>g__ShouldSaveFor|1))
					where allowedHabModules.Contains(x) && x.coreModule
					select x).FirstOrDefault<TIHabModuleTemplate>();
			}
			if (tihabModuleTemplate2 != null && (tihabModuleTemplate2.spaceCombatModule || tihabModuleTemplate2.SpecialRules.Contains(HabModuleSpecialRule.DropTroops)))
			{
				return null;
			}
			return tihabModuleTemplate2;
		}

		// Token: 0x06002F86 RID: 12166 RVA: 0x00103F17 File Offset: 0x00102117
		public override bool ValidNewGoal()
		{
			return !this.ShouldDiscardGoal() && !this.GoalFulfilled();
		}

		// Token: 0x06002F87 RID: 12167 RVA: 0x00103F2C File Offset: 0x0010212C
		public override bool ShouldDiscardGoal()
		{
			return base.importance <= 0 || this.hab == null || this.hab.archived || this.hab.faction != this.faction;
		}

		// Token: 0x06002F88 RID: 12168 RVA: 0x00103F6C File Offset: 0x0010216C
		public override bool GoalFulfilled()
		{
			if (this.hab != null && this.hab.numActiveSectors == 5 && this.hab.numCompletedModules == TIHabState.maxModules(3))
			{
				return this.hab.CompletedModules().TrueForAll((TIHabModuleState x) => x.tier == 3);
			}
			return false;
		}

		// Token: 0x17000719 RID: 1817
		// (get) Token: 0x06002F89 RID: 12169 RVA: 0x00103FD9 File Offset: 0x001021D9
		public override List<GoalType> incompatibleGoals
		{
			get
			{
				return new List<GoalType>();
			}
		}

		// Token: 0x04002243 RID: 8771
		public List<string> specialtyModuleDataNames;
	}
}
