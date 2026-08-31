using System;
using System.Threading.Tasks;
using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace PavonisInteractive.TerraInvicta.Systems.Bootstrap
{
	// Token: 0x020009BC RID: 2492
	public class StartScreenBootstrap : IInitializable
	{
		// Token: 0x06005DDE RID: 24030 RVA: 0x002CAB98 File Offset: 0x002C8D98
		public void Initialize()
		{
			foreach (ScriptBehaviourManager scriptBehaviourManager in World.Active.BehaviourManagers)
			{
				ComponentSystemBase componentSystemBase = scriptBehaviourManager as ComponentSystemBase;
				if (componentSystemBase != null)
				{
					componentSystemBase.Enabled = false;
				}
			}
		}

		// Token: 0x06005DDF RID: 24031 RVA: 0x002CABF4 File Offset: 0x002C8DF4
		public async void LoadSolarSystemScene(string savefile)
		{
			if (!string.IsNullOrEmpty(savefile))
			{
				AsyncOperation asyncOperation = this.sceneLoader.LoadSceneAsync("SolarSystemScene", LoadSceneMode.Single, delegate(DiContainer container)
				{
					container.BindInstance<string>(savefile).WhenInjectedInto<SolarSystemBootstrap>();
				});
				await this.HandleLoadingScreen(asyncOperation);
				return;
			}
			throw new Exception("No savefile provided to load gamestates from");
		}

		// Token: 0x06005DE0 RID: 24032 RVA: 0x002CAC38 File Offset: 0x002C8E38
		public async void LoadSolarSystemScene(string[] templates)
		{
			if (templates.IsNotNullOrEmpty())
			{
				AsyncOperation asyncOperation = this.sceneLoader.LoadSceneAsync("SolarSystemScene", LoadSceneMode.Additive, delegate(DiContainer container)
				{
					container.BindInstance<string[]>(templates).WhenInjectedInto<SolarSystemBootstrap>();
				});
				await this.HandleLoadingScreen(asyncOperation);
				return;
			}
			throw new Exception("No templates provided to load gamestates from");
		}

		// Token: 0x06005DE1 RID: 24033 RVA: 0x002CAC7C File Offset: 0x002C8E7C
		private async Task HandleLoadingScreen(AsyncOperation sceneLoad)
		{
			while (!sceneLoad.isDone)
			{
				await Task.Delay(100);
			}
		}

		// Token: 0x04004321 RID: 17185
		[global::Zenject.Inject]
		private ZenjectSceneLoader sceneLoader;
	}
}
