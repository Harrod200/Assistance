using System;
using PavonisInteractive.TerraInvicta.Components;
using Unity.Entities;

namespace PavonisInteractive.TerraInvicta.Systems.Habs
{
	// Token: 0x020009A9 RID: 2473
	[UpdateInGroup(typeof(PipelineStages.SimulationStage))]
	public class HabBuilding : StrategyLayerComponentSystem, IHabBuilder
	{
		// Token: 0x06005D33 RID: 23859 RVA: 0x002C7808 File Offset: 0x002C5A08
		public override void Initialize()
		{
			base.Initialize();
			this.oldNow = default(DateTime);
		}

		// Token: 0x06005D34 RID: 23860 RVA: 0x002C781C File Offset: 0x002C5A1C
		protected override void OnUpdate()
		{
			DateTime dateTime = TITimeState.SystemNow();
			if (dateTime.Day != this.oldNow.Day || this.oldNow == default(DateTime))
			{
				for (int i = 0; i < this.habBuilds.Length; i++)
				{
					if (!(this.habBuilds.Hab[i] == null))
					{
						TIHabState hab = this.habBuilds.Hab[i].hab;
						if (!(hab == null) && hab.sectors != null)
						{
							this.habBuilds.Hab[i].notBuilding = true;
							for (int j = 0; j < hab.sectors.Count; j++)
							{
								TISectorState tisectorState = hab.sectors[j];
								for (int k = 0; k < tisectorState.habModules.Count; k++)
								{
									TIHabModuleState tihabModuleState = tisectorState.habModules[k];
									if (!tihabModuleState.constructionCompleted)
									{
										if (tihabModuleState.completionDate > dateTime)
										{
											this.habBuilds.Hab[i].notBuilding = false;
										}
										else
										{
											hab.CompleteModuleConstruction(tihabModuleState);
										}
									}
									else if (tihabModuleState.decommissioning)
									{
										if (tihabModuleState.decommissionDate > dateTime)
										{
											this.habBuilds.Hab[i].notBuilding = false;
										}
										else
										{
											hab.CompleteDecommissionModule(tihabModuleState, true);
										}
									}
								}
							}
						}
					}
				}
			}
			this.oldNow = dateTime;
		}

		// Token: 0x06005D35 RID: 23861 RVA: 0x002C79B8 File Offset: 0x002C5BB8
		public void BuildHab(TIHabState habState)
		{
			int i = 0;
			while (i < this.habs.Length)
			{
				if (this.habs.Hab[i].hab == habState)
				{
					if (this.habs.Hab[i].notBuilding)
					{
						this.habs.GameObject[i].GetOrAdd<HabBuildComponent>();
						return;
					}
					break;
				}
				else
				{
					i++;
				}
			}
		}

		// Token: 0x040042B9 RID: 17081
		[Inject]
		private HabBuilding.HabGroup habs;

		// Token: 0x040042BA RID: 17082
		[Inject]
		private HabBuilding.HabBuildGroup habBuilds;

		// Token: 0x040042BB RID: 17083
		private DateTime oldNow;

		// Token: 0x02001354 RID: 4948
		public struct HabGroup
		{
			// Token: 0x04006FC9 RID: 28617
			public readonly int Length;

			// Token: 0x04006FCA RID: 28618
			public GameObjectArray GameObject;

			// Token: 0x04006FCB RID: 28619
			public ComponentArray<HabComponent> Hab;
		}

		// Token: 0x02001355 RID: 4949
		public struct HabBuildGroup
		{
			// Token: 0x04006FCC RID: 28620
			public readonly int Length;

			// Token: 0x04006FCD RID: 28621
			public GameObjectArray GameObject;

			// Token: 0x04006FCE RID: 28622
			public ComponentArray<HabComponent> Hab;

			// Token: 0x04006FCF RID: 28623
			public ComponentArray<HabBuildComponent> HabBuild;
		}
	}
}
