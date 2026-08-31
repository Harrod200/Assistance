using System;
using PavonisInteractive.TerraInvicta.Systems.Camera;
using Unity.Entities;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200055A RID: 1370
	public abstract class SingleMarkerController : MonoBehaviour, IMarkerControl
	{
		// Token: 0x1700050D RID: 1293
		// (get) Token: 0x06002432 RID: 9266 RVA: 0x000C07A9 File Offset: 0x000BE9A9
		// (set) Token: 0x06002433 RID: 9267 RVA: 0x000C07B1 File Offset: 0x000BE9B1
		private protected TIFactionState activePlayer { protected get; private set; }

		// Token: 0x1700050E RID: 1294
		// (get) Token: 0x06002434 RID: 9268 RVA: 0x000C07BA File Offset: 0x000BE9BA
		// (set) Token: 0x06002435 RID: 9269 RVA: 0x000C07C2 File Offset: 0x000BE9C2
		private protected RegionController regionController { protected get; private set; }

		// Token: 0x1700050F RID: 1295
		// (get) Token: 0x06002436 RID: 9270 RVA: 0x000C07CB File Offset: 0x000BE9CB
		// (set) Token: 0x06002437 RID: 9271 RVA: 0x000C07D3 File Offset: 0x000BE9D3
		private protected TIRegionState region { protected get; private set; }

		// Token: 0x17000510 RID: 1296
		// (get) Token: 0x06002438 RID: 9272 RVA: 0x000C07DC File Offset: 0x000BE9DC
		// (set) Token: 0x06002439 RID: 9273 RVA: 0x000C07E4 File Offset: 0x000BE9E4
		private protected MarkerContainerController container { protected get; private set; }

		// Token: 0x17000511 RID: 1297
		// (get) Token: 0x0600243A RID: 9274 RVA: 0x000C07ED File Offset: 0x000BE9ED
		// (set) Token: 0x0600243B RID: 9275 RVA: 0x000C07F5 File Offset: 0x000BE9F5
		private protected CameraManager cameraManager { protected get; private set; }

		// Token: 0x17000512 RID: 1298
		// (get) Token: 0x0600243C RID: 9276 RVA: 0x000C07FE File Offset: 0x000BE9FE
		protected TINationState nation
		{
			get
			{
				return this.region.nation;
			}
		}

		// Token: 0x17000513 RID: 1299
		// (get) Token: 0x0600243D RID: 9277 RVA: 0x000C080B File Offset: 0x000BEA0B
		protected TIGameState globalCurrentTarget
		{
			get
			{
				return GeneralControlsController.UITargetedState;
			}
		}

		// Token: 0x0600243E RID: 9278 RVA: 0x000C0812 File Offset: 0x000BEA12
		public virtual void InitializeWithRegion(RegionController regionController, MarkerContainerController container)
		{
			this.regionController = regionController;
			this.region = regionController.region;
			this.container = container;
			this.cameraManager = World.Active.GetExistingManager<CameraManager>();
			this.SetActivePlayer(true);
		}

		// Token: 0x0600243F RID: 9279
		public abstract void UpdateMarker();

		// Token: 0x06002440 RID: 9280 RVA: 0x000C0845 File Offset: 0x000BEA45
		public void SetActivePlayer(bool startup)
		{
			this.activePlayer = GameControl.control.activePlayer;
			if (!startup)
			{
				this.UpdateMarker();
			}
		}
	}
}
