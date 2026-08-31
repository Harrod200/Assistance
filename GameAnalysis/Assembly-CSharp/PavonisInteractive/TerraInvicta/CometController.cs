using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta.Systems.Camera;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200058F RID: 1423
	public class CometController : MonoBehaviour
	{
		// Token: 0x1700053C RID: 1340
		// (get) Token: 0x060025D2 RID: 9682 RVA: 0x000CD798 File Offset: 0x000CB998
		// (set) Token: 0x060025D3 RID: 9683 RVA: 0x000CD7A0 File Offset: 0x000CB9A0
		public SpaceObjectController SpaceObjectController { get; private set; }

		// Token: 0x1700053D RID: 1341
		// (get) Token: 0x060025D4 RID: 9684 RVA: 0x000CD7A9 File Offset: 0x000CB9A9
		public TISpaceBodyState Comet
		{
			get
			{
				if (!this.IsInOverrideRenderMode)
				{
					return this.SpaceObjectController.spaceObjectState.ref_spaceBody;
				}
				return this.OverrideComet;
			}
		}

		// Token: 0x1700053E RID: 1342
		// (get) Token: 0x060025D5 RID: 9685 RVA: 0x000CD7CA File Offset: 0x000CB9CA
		public IEnumerable<CometParticleController> ParticleControllers
		{
			get
			{
				return Enumerable.Empty<CometParticleController>().Append(this.ComaController).Append(this.DustTailController)
					.Append(this.GasTailController);
			}
		}

		// Token: 0x1700053F RID: 1343
		// (get) Token: 0x060025D6 RID: 9686 RVA: 0x000CD7F2 File Offset: 0x000CB9F2
		public static float FrostLine_AU
		{
			get
			{
				return 3f;
			}
		}

		// Token: 0x17000540 RID: 1344
		// (get) Token: 0x060025D7 RID: 9687 RVA: 0x000CD7FC File Offset: 0x000CB9FC
		public bool IsCometOutgassing
		{
			get
			{
				return this.Comet.GetGlobalPosition().magnitude / 149597870700.0 <= (double)CometController.FrostLine_AU;
			}
		}

		// Token: 0x17000541 RID: 1345
		// (get) Token: 0x060025D8 RID: 9688 RVA: 0x000CD831 File Offset: 0x000CBA31
		// (set) Token: 0x060025D9 RID: 9689 RVA: 0x000CD839 File Offset: 0x000CBA39
		public float VolatileFraction { get; private set; }

		// Token: 0x17000542 RID: 1346
		// (get) Token: 0x060025DA RID: 9690 RVA: 0x000CD842 File Offset: 0x000CBA42
		// (set) Token: 0x060025DB RID: 9691 RVA: 0x000CD84A File Offset: 0x000CBA4A
		public float VolatileWaterFraction { get; private set; }

		// Token: 0x17000543 RID: 1347
		// (get) Token: 0x060025DC RID: 9692 RVA: 0x000CD853 File Offset: 0x000CBA53
		public float Productivity
		{
			get
			{
				return this.VolatileFraction / 0.5f;
			}
		}

		// Token: 0x17000544 RID: 1348
		// (get) Token: 0x060025DD RID: 9693 RVA: 0x000CD864 File Offset: 0x000CBA64
		public float DistanceBasedProductivity
		{
			get
			{
				if (!this.IsCometOutgassing)
				{
					return 0f;
				}
				float num = (float)(this.Comet.GetGlobalPosition().magnitude / 149597870700.0);
				float num2 = 0.14f;
				float num3 = (num - num2) / (CometController.FrostLine_AU - num2);
				float num4 = Mathf.Pow(1f - num3, 2f);
				return this.Productivity * num4;
			}
		}

		// Token: 0x060025DE RID: 9694 RVA: 0x000CD8CC File Offset: 0x000CBACC
		public void InitiateOverrideRenderMode(TISpaceBodyState overrideComet, Camera overrideCamera, bool drawingToRenderTexture)
		{
			this.OverrideComet = overrideComet;
			this.OverrideCamera = overrideCamera;
			foreach (CometParticleController cometParticleController in this.ParticleControllers)
			{
				cometParticleController.InitiateOverrideRenderMode(drawingToRenderTexture);
			}
		}

		// Token: 0x17000545 RID: 1349
		// (get) Token: 0x060025DF RID: 9695 RVA: 0x000CD928 File Offset: 0x000CBB28
		// (set) Token: 0x060025E0 RID: 9696 RVA: 0x000CD930 File Offset: 0x000CBB30
		public TISpaceBodyState OverrideComet { get; private set; }

		// Token: 0x17000546 RID: 1350
		// (get) Token: 0x060025E1 RID: 9697 RVA: 0x000CD939 File Offset: 0x000CBB39
		// (set) Token: 0x060025E2 RID: 9698 RVA: 0x000CD941 File Offset: 0x000CBB41
		public Camera OverrideCamera { get; private set; }

		// Token: 0x17000547 RID: 1351
		// (get) Token: 0x060025E3 RID: 9699 RVA: 0x000CD94A File Offset: 0x000CBB4A
		public bool IsInOverrideRenderMode
		{
			get
			{
				return this.OverrideComet != null;
			}
		}

		// Token: 0x17000548 RID: 1352
		// (get) Token: 0x060025E4 RID: 9700 RVA: 0x000CD958 File Offset: 0x000CBB58
		public float OverrideSizeFactor
		{
			get
			{
				return (float)((double)(base.transform.lossyScale.x * this.Comet.modelScale) / this.Comet.meanRadius_m);
			}
		}

		// Token: 0x17000549 RID: 1353
		// (get) Token: 0x060025E5 RID: 9701 RVA: 0x000CD984 File Offset: 0x000CBB84
		public bool DoNotDisplay
		{
			get
			{
				return CameraManager.Singleton == null || !GameControl.loadcycle100 || TITimeState.Now() == null || GameControl.control.skirmishMode || (CameraManager.Singleton.LOD == CameraManagerLOD.Surface && CameraManager.Singleton.SelectedState != this.Comet) || !this.IsCometOutgassing;
			}
		}

		// Token: 0x060025E6 RID: 9702 RVA: 0x000CD9E8 File Offset: 0x000CBBE8
		private void Start()
		{
			this.SpaceObjectController = base.GetComponentInParent<SpaceObjectController>();
			float num = this.Comet.habSites.Sum<TIHabSiteState>((TIHabSiteState x) => x.volatiles_day);
			float num2 = this.Comet.habSites.Sum<TIHabSiteState>((TIHabSiteState x) => x.water_day);
			float num3 = this.Comet.habSites.Sum<TIHabSiteState>((TIHabSiteState x) => x.metals_day + x.nobles_day + x.fissiles_day);
			float num4 = num + num2 + num3;
			this.VolatileFraction = (num + num2) / (num + num2 + num3 + num4);
			this.VolatileWaterFraction = num2 / (num + num2);
			if (this.IsInOverrideRenderMode)
			{
				this.DustTailController.gameObject.SetActive(false);
				this.GasTailController.gameObject.SetActive(false);
			}
		}

		// Token: 0x04001C46 RID: 7238
		public CometComaController ComaController;

		// Token: 0x04001C47 RID: 7239
		public CometDustTailController DustTailController;

		// Token: 0x04001C48 RID: 7240
		public CometGasTailController GasTailController;
	}
}
