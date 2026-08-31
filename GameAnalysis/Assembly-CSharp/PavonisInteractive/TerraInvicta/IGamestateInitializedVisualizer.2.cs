using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200055E RID: 1374
	public interface IGamestateInitializedVisualizer<T> : IGamestateInitializedVisualizer where T : TIGameState
	{
		// Token: 0x06002456 RID: 9302
		void Initialize(T state);
	}
}
