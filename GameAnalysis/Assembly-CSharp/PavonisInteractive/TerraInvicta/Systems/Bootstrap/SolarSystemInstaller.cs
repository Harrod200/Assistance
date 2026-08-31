using System;
using Zenject;

namespace PavonisInteractive.TerraInvicta.Systems.Bootstrap
{
	// Token: 0x020009BB RID: 2491
	public class SolarSystemInstaller : MonoInstaller<SolarSystemInstaller>
	{
		// Token: 0x06005DDC RID: 24028 RVA: 0x002CAB0C File Offset: 0x002C8D0C
		public override void InstallBindings()
		{
			SolarSystemInstaller.container = base.Container;
			base.Container.BindInterfacesAndSelfTo<SolarSystemBootstrap>().AsSingle();
			base.Container.BindExecutionOrder<SolarSystemBootstrap>(-1000);
			base.Container.BindInterfacesAndSelfTo<EntityHelper>().AsSingle().NonLazy();
			base.Container.BindExecutionOrder<EntityHelper>(-1001);
			base.Container.Bind<SceneManager>().AsSingle().NonLazy();
			base.Container.Bind<AssetLoader>().AsSingle();
		}

		// Token: 0x04004320 RID: 17184
		public static DiContainer container;
	}
}
