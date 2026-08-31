using System;
using PavonisInteractive.TerraInvicta.Entities;
using PavonisInteractive.TerraInvicta.Tasks;
using Zenject;

namespace PavonisInteractive.TerraInvicta.Installers
{
	// Token: 0x0200095F RID: 2399
	internal sealed class PlayerInstaller : MonoInstaller<PlayerInstaller>
	{
		// Token: 0x06005B47 RID: 23367 RVA: 0x002BE920 File Offset: 0x002BCB20
		[Inject]
		public void Construct(TIPlayerState playerState, Council.Factory councilFactory)
		{
			this.playerState = playerState;
			this.councilFactory = councilFactory;
		}

		// Token: 0x06005B48 RID: 23368 RVA: 0x002BE930 File Offset: 0x002BCB30
		public override void InstallBindings()
		{
			base.Container.ShouldCheckForInstallWarning = false;
			TIFactionState faction = this.playerState.faction;
			Player component = base.GetComponent<Player>();
			base.Container.BindInterfacesAndSelfTo<Player>().FromInstance(component).AsSingle();
			base.Container.BindInstance<TIPlayerState>(this.playerState).AsSingle();
			base.Container.BindInstance<TIFactionState>(faction).AsSingle();
			base.Container.BindInstance<Council>(this.councilFactory.Create(faction));
			base.Container.Bind<IProjectSelectionStrategy>().To<StratProjectSelector>().AsSingle();
			base.Container.Bind<ITechSelectionStrategy>().To<StratTechSelector>().AsSingle();
			base.Container.Bind<ICalledAllyResponseSelectionStrategy>().To<StratCalledAllyResponseSelector>().AsSingle();
			base.Container.Bind<IPolicyResponseSelectionStrategy>().To<StratPolicyResponseSelector>().AsSingle();
			bool isAI = this.playerState.isAI;
		}

		// Token: 0x0400418E RID: 16782
		private TIPlayerState playerState;

		// Token: 0x0400418F RID: 16783
		private Council.Factory councilFactory;
	}
}
