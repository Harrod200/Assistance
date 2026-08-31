using System;
using Zenject;

namespace PavonisInteractive.TerraInvicta.Systems.Bootstrap
{
	// Token: 0x020009BD RID: 2493
	public class StartScreenInstaller : MonoInstaller<StartScreenInstaller>
	{
		// Token: 0x17001029 RID: 4137
		// (get) Token: 0x06005DE3 RID: 24035 RVA: 0x002CACC9 File Offset: 0x002C8EC9
		// (set) Token: 0x06005DE4 RID: 24036 RVA: 0x002CACD0 File Offset: 0x002C8ED0
		public static DiContainer container { get; private set; }

		// Token: 0x06005DE5 RID: 24037 RVA: 0x002CACD8 File Offset: 0x002C8ED8
		public override void InstallBindings()
		{
			Log.Debug("StartScreen Install Bindings", Array.Empty<object>());
			StartScreenInstaller.container = base.Container;
			base.Container.BindInterfacesAndSelfTo<StartScreenBootstrap>().AsSingle();
			base.Container.BindExecutionOrder<StartScreenBootstrap>(-1000);
			base.Container.Bind<SceneManager>().AsSingle().NonLazy();
		}
	}
}
