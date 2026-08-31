using System;
using System.Threading.Tasks;
using PavonisInteractive.TerraInvicta.Systems.GameTime;
using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace PavonisInteractive.TerraInvicta.Systems.Bootstrap
{
	// Token: 0x020009B9 RID: 2489
	public class SceneManager
	{
		// Token: 0x17001026 RID: 4134
		// (get) Token: 0x06005DCE RID: 24014 RVA: 0x002CA55C File Offset: 0x002C875C
		public string activeSceneName
		{
			get
			{
				return SceneManager.GetActiveScene().name;
			}
		}

		// Token: 0x17001027 RID: 4135
		// (get) Token: 0x06005DCF RID: 24015 RVA: 0x002CA576 File Offset: 0x002C8776
		public bool onStartScreen
		{
			get
			{
				return this.activeSceneName == "StartScreenScene";
			}
		}

		// Token: 0x17001028 RID: 4136
		// (get) Token: 0x06005DD0 RID: 24016 RVA: 0x002CA588 File Offset: 0x002C8788
		public bool onSolarSystem
		{
			get
			{
				return this.activeSceneName == "SolarSystemScene";
			}
		}

		// Token: 0x06005DD1 RID: 24017 RVA: 0x002CA59A File Offset: 0x002C879A
		public SceneManager()
		{
			SceneManager.self = this;
		}

		// Token: 0x06005DD2 RID: 24018 RVA: 0x002CA5A8 File Offset: 0x002C87A8
		public async void LoadScene(string name)
		{
			World.Active.GetExistingManager<GameTimeManager>().UnBlock();
			AsyncOperation asyncOperation = this.sceneLoader.LoadSceneAsync(name, LoadSceneMode.Single);
			await this.HandleLoadingScreen(asyncOperation);
		}

		// Token: 0x06005DD3 RID: 24019 RVA: 0x002CA5EC File Offset: 0x002C87EC
		public async void LoadScene(string name, Action<DiContainer> inject)
		{
			World.Active.GetExistingManager<GameTimeManager>().UnBlock();
			AsyncOperation asyncOperation = this.sceneLoader.LoadSceneAsync(name, LoadSceneMode.Single, inject);
			await this.HandleLoadingScreen(asyncOperation);
		}

		// Token: 0x06005DD4 RID: 24020 RVA: 0x002CA638 File Offset: 0x002C8838
		private async Task HandleLoadingScreen(AsyncOperation sceneLoad)
		{
			while (!sceneLoad.isDone)
			{
				await Task.Delay(100);
			}
		}

		// Token: 0x04004319 RID: 17177
		public static SceneManager self;

		// Token: 0x0400431A RID: 17178
		[global::Zenject.Inject]
		private ZenjectSceneLoader sceneLoader;
	}
}
