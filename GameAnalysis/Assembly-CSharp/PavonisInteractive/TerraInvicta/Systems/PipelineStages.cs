using System;
using Unity.Entities;
using UnityEngine.PlayerLoop;

namespace PavonisInteractive.TerraInvicta.Systems
{
	// Token: 0x0200099A RID: 2458
	public static class PipelineStages
	{
		// Token: 0x0200133C RID: 4924
		public class InputHandleStage
		{
		}

		// Token: 0x0200133D RID: 4925
		[UpdateAfter(typeof(PipelineStages.InputHandleStage))]
		public class InputProcessStage
		{
		}

		// Token: 0x0200133E RID: 4926
		[UpdateAfter(typeof(PipelineStages.InputProcessStage))]
		public class SimulationStage
		{
		}

		// Token: 0x0200133F RID: 4927
		[UpdateAfter(typeof(PipelineStages.SimulationStage))]
		public class RenderStage
		{
		}

		// Token: 0x02001340 RID: 4928
		[UpdateAfter(typeof(PipelineStages.RenderStage))]
		[UpdateBefore(typeof(Update))]
		public class FinalizationStage
		{
		}

		// Token: 0x02001341 RID: 4929
		[UpdateAfter(typeof(PostLateUpdate))]
		public class EndFrameStage
		{
		}
	}
}
