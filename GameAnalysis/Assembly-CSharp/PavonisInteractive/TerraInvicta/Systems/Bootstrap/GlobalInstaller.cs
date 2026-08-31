using System;
using System.Reflection;
using AssetBundles;
using PavonisInteractive.TerraInvicta.Debugging;
using PavonisInteractive.TerraInvicta.Entities;
using PavonisInteractive.TerraInvicta.Installers;
using PavonisInteractive.TerraInvicta.Systems.GameTime;
using UnityEngine;
using Zenject;

namespace PavonisInteractive.TerraInvicta.Systems.Bootstrap
{
	// Token: 0x020009B7 RID: 2487
	public class GlobalInstaller : MonoInstaller<GlobalInstaller>
	{
		// Token: 0x06005DC6 RID: 24006 RVA: 0x002CA1A8 File Offset: 0x002C83A8
		public static void InjectGameControlBinding<T>(T component, string propertyName) where T : class
		{
			FieldInfo field = typeof(GameControl).GetField(propertyName, BindingFlags.Instance | BindingFlags.NonPublic);
			if (field != null)
			{
				field.SetValue(GameControl.control, component);
				return;
			}
			throw new Exception("No field named " + propertyName);
		}

		// Token: 0x06005DC7 RID: 24007 RVA: 0x002CA1F4 File Offset: 0x002C83F4
		public override void InstallBindings()
		{
			GlobalInstaller.container = base.Container;
			Application.logMessageReceived += this.HandleException;
			Log.Initialize();
			Log.Info("TerraInvicta v" + Application.version, Array.Empty<object>());
			Log.Info("Initializing global bindings", Array.Empty<object>());
			this.playerPrefab = (GameObject)Resources.Load("Prefabs/PlayerPrefab");
			base.Container.BindFactory<TIFactionState, Council, Council.Factory>();
			base.Container.BindFactory<TICouncilorState, Councilor, Councilor.Factory>();
			base.Container.BindFactory<TIPlayerState, Player, Player.Factory>().FromSubContainerResolve().ByNewPrefab<PlayerInstaller>(this.playerPrefab);
			this.gameControl = GameObject.Find("Game Controller");
			if (this.gameControl == null)
			{
				throw new Exception("Game Controller not found");
			}
			base.Container.ShouldCheckForInstallWarning = false;
			if (Application.isPlaying)
			{
				global::UnityEngine.Object.DontDestroyOnLoad(this.gameControl);
			}
			base.Container.Bind<GameControl>().FromInstance(this.gameControl.GetComponent<GameControl>());
			base.Container.Bind<ViewControl>().FromInstance(this.gameControl.GetComponent<ViewControl>());
			base.Container.Bind<EventManager>().FromInstance(this.gameControl.GetComponent<EventManager>());
			base.Container.Bind<IEventManager>().FromInstance(this.gameControl.GetComponent<EventManager>());
			base.Container.Bind<SolarSystemControl>().FromInstance(this.gameControl.GetComponent<SolarSystemControl>());
			base.Container.Bind<SpaceCombatManager>().FromInstance(this.gameControl.GetComponent<SpaceCombatManager>());
			base.Container.Bind<PlayerManager>().FromInstance(this.gameControl.GetComponent<PlayerManager>());
			base.Container.BindInstance<TemplateManager>(TemplateManager.self);
			base.Container.Resolve<TemplateManager>().Initialize(Application.streamingAssetsPath + "/Templates");
			base.Container.Bind<LocalizationManager>().AsSingle().NonLazy();
			if (string.IsNullOrEmpty(Loc.CurrentLanguage))
			{
				Debug.Log("DefaultFallbackLanguage");
				Loc.SetLanguage("en");
			}
			Log.Time("<color=#00cc00>LoadTime:</color> AssetBundleManager Initialize", new Action(AssetBundleManager.Initialize), true, true);
			TIPlayerProfileManager.Init();
			base.Container.Bind<NamelistManager>().AsSingle().NonLazy();
			GameControl.control = base.Container.Resolve<GameControl>();
			base.Container.InjectGameObject(this.gameControl);
			Log.Info("GameControl Injected", Array.Empty<object>());
			Installer<TerminalInstaller>.Install(base.Container);
			Log.Info("Developer Terminal Injected", Array.Empty<object>());
			if (Application.isPlaying)
			{
				base.Container.Resolve<ViewControl>().StartSession();
			}
		}

		// Token: 0x06005DC8 RID: 24008 RVA: 0x002CA498 File Offset: 0x002C8698
		private void HandleException(string message, string stackTrace, LogType type)
		{
			if (type == LogType.Exception && !GameControl.handlingException)
			{
				Debug.Log("Handling Exception");
				if (!Application.isEditor)
				{
					try
					{
						GameControl.handlingException = true;
						Debug.Log("Game speed at time of crash: " + GameTimeManager.Singleton.currentSpeed.ToString());
						(GameControl.canvasStack.OptionsScreen as OptionsScreenController).ShowExceptionDialog(message, stackTrace);
						GameTimeManager.Singleton.PauseAndBlock();
						GameControl.eventManager.ClearAllEvents();
						TIInputManager.acceptingInput = false;
						return;
					}
					catch (Exception ex)
					{
						Debug.LogError("Exception in Exception Handling: " + ex.StackTrace);
						GameControl.Stop();
					}
				}
				GameControl.Stop();
				return;
			}
		}

		// Token: 0x04004316 RID: 17174
		public static DiContainer container;

		// Token: 0x04004317 RID: 17175
		[SerializeField]
		private GameObject playerPrefab;

		// Token: 0x04004318 RID: 17176
		private GameObject gameControl;
	}
}
