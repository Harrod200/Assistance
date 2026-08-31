using System;
using UnityEngine;
using Zenject;

namespace PavonisInteractive.TerraInvicta.Settings
{
	// Token: 0x02000962 RID: 2402
	[CreateAssetMenu(fileName = "GameSettings", menuName = "Installers/GameSettings")]
	public class GameSettings : ScriptableObjectInstaller<GameSettings>
	{
		// Token: 0x06005B93 RID: 23443 RVA: 0x002BF221 File Offset: 0x002BD421
		public override void InstallBindings()
		{
			base.Container.BindInstance<CameraConfig>(this.cameraConfig);
		}

		// Token: 0x04004198 RID: 16792
		public CameraConfig cameraConfig;
	}
}
