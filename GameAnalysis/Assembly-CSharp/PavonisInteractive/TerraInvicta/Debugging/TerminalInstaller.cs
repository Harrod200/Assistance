using System;
using UnityEngine;
using Zenject;

namespace PavonisInteractive.TerraInvicta.Debugging
{
	// Token: 0x02000915 RID: 2325
	public class TerminalInstaller : Installer<TerminalInstaller>
	{
		// Token: 0x060058E3 RID: 22755 RVA: 0x0028C1C4 File Offset: 0x0028A3C4
		public override void InstallBindings()
		{
			base.Container.BindInterfacesAndSelfTo<Terminal>().AsSingle().NonLazy();
			GameObject gameObject = base.Container.InstantiatePrefabResource("Debugging/Terminal");
			base.Container.BindInstance<GameObject>(gameObject).WhenInjectedInto<Terminal>();
			base.Container.BindInterfacesAndSelfTo<TerminalButtonListener>().AsSingle().NonLazy();
			base.Container.Bind<TerminalController>().AsSingle();
			base.Container.Bind<TerminalResourceCommands>().AsSingle().NonLazy();
			base.Container.Bind<TerminalMusicCommands>().AsSingle().NonLazy();
			base.Container.Bind<TerminalTechProjectCommands>().AsSingle().NonLazy();
			base.Container.Bind<TerminalNarrativeEventsCommands>().AsSingle().NonLazy();
			base.Container.Bind<TerminalObjectiveCommands>().AsSingle().NonLazy();
			base.Container.Bind<TerminalCouncilorCommands>().AsSingle().NonLazy();
			base.Container.Bind<TerminalNationCommands>().AsSingle().NonLazy();
			base.Container.Bind<TerminalFleetCommands>().AsSingle().NonLazy();
			base.Container.Bind<TerminalAutopilotCommands>().AsSingle().NonLazy();
			base.Container.Bind<TerminalHistoricalDataCommands>().AsSingle().NonLazy();
		}
	}
}
