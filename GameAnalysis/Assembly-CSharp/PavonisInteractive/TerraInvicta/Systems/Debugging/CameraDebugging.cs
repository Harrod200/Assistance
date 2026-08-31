using System;
using PavonisInteractive.TerraInvicta.Systems.GameTime;
using Unity.Entities;
using Zenject;

namespace PavonisInteractive.TerraInvicta.Systems.Debugging
{
	// Token: 0x020009AF RID: 2479
	[UpdateInGroup(typeof(PipelineStages.EndFrameStage))]
	public class CameraDebugging : ComponentSystem
	{
		// Token: 0x06005D77 RID: 23927 RVA: 0x002C8854 File Offset: 0x002C6A54
		protected override void OnUpdate()
		{
		}

		// Token: 0x040042DC RID: 17116
		[global::Zenject.Inject]
		private TestSettings.DebugSettings debugSettings;

		// Token: 0x040042DD RID: 17117
		[Unity.Entities.Inject]
		private GameTimeManager gameTimeManager;

		// Token: 0x040042DE RID: 17118
		[Unity.Entities.Inject]
		private EntityManager entityManager;
	}
}
