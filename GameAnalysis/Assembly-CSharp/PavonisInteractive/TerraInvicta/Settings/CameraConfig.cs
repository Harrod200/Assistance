using System;

namespace PavonisInteractive.TerraInvicta.Settings
{
	// Token: 0x02000963 RID: 2403
	[Serializable]
	public class CameraConfig
	{
		// Token: 0x17000F7A RID: 3962
		// (get) Token: 0x06005B95 RID: 23445 RVA: 0x002BF23D File Offset: 0x002BD43D
		public double DragRateNormal
		{
			get
			{
				return TemplateManager.global.strategyCamera_DragRateNormal;
			}
		}

		// Token: 0x17000F7B RID: 3963
		// (get) Token: 0x06005B96 RID: 23446 RVA: 0x002BF249 File Offset: 0x002BD449
		public double DragRateSlow
		{
			get
			{
				return TemplateManager.global.strategyCamera_DragRateSlow;
			}
		}

		// Token: 0x17000F7C RID: 3964
		// (get) Token: 0x06005B97 RID: 23447 RVA: 0x002BF255 File Offset: 0x002BD455
		public double ZoomRateNormal
		{
			get
			{
				return TemplateManager.global.strategyCamera_ZoomRateNormal;
			}
		}

		// Token: 0x17000F7D RID: 3965
		// (get) Token: 0x06005B98 RID: 23448 RVA: 0x002BF261 File Offset: 0x002BD461
		public double ZoomRateSlow
		{
			get
			{
				return TemplateManager.global.strategyCamera_ZoomRateSlow;
			}
		}

		// Token: 0x17000F7E RID: 3966
		// (get) Token: 0x06005B99 RID: 23449 RVA: 0x002BF26D File Offset: 0x002BD46D
		public double ZoomRateLongDistanceMultiplier
		{
			get
			{
				return TemplateManager.global.strategyCamera_ZoomLongDistanceMultiplier;
			}
		}

		// Token: 0x17000F7F RID: 3967
		// (get) Token: 0x06005B9A RID: 23450 RVA: 0x002BF279 File Offset: 0x002BD479
		public double ZoomRateMediumDistanceMultiplier
		{
			get
			{
				return TemplateManager.global.strategyCamera_ZoomMediumDistanceMultiplier;
			}
		}

		// Token: 0x17000F80 RID: 3968
		// (get) Token: 0x06005B9B RID: 23451 RVA: 0x002BF285 File Offset: 0x002BD485
		public double ZoomRateLongDistanceThreshold
		{
			get
			{
				return TemplateManager.global.strategyCamera_ZoomLongDistanceThreshold;
			}
		}

		// Token: 0x17000F81 RID: 3969
		// (get) Token: 0x06005B9C RID: 23452 RVA: 0x002BF291 File Offset: 0x002BD491
		public double ZoomRateMediumDistanceThreshold
		{
			get
			{
				return TemplateManager.global.strategyCamera_ZoomMediumDistanceThreshold;
			}
		}

		// Token: 0x17000F82 RID: 3970
		// (get) Token: 0x06005B9D RID: 23453 RVA: 0x002BF29D File Offset: 0x002BD49D
		public double ZoomRateShortDistanceThreshold
		{
			get
			{
				return TemplateManager.global.strategyCamera_ZoomShortDistanceThreshold;
			}
		}

		// Token: 0x06005B9E RID: 23454 RVA: 0x002BF2A9 File Offset: 0x002BD4A9
		public double ZoomLimit(bool Earth)
		{
			if (!Earth)
			{
				return TemplateManager.global.strategyCamera_ZoomLimit;
			}
			return TemplateManager.global.strategyCamera_ZoomLimitEarth;
		}

		// Token: 0x17000F83 RID: 3971
		// (get) Token: 0x06005B9F RID: 23455 RVA: 0x002BF2C3 File Offset: 0x002BD4C3
		public double MaxZoomStep
		{
			get
			{
				return TemplateManager.global.strategyCamera_MaxZoomStep;
			}
		}

		// Token: 0x17000F84 RID: 3972
		// (get) Token: 0x06005BA0 RID: 23456 RVA: 0x002BF2CF File Offset: 0x002BD4CF
		public double MinDistanceFromCamera
		{
			get
			{
				return TemplateManager.global.strategyCamera_MinDistanceFromCamera;
			}
		}

		// Token: 0x17000F85 RID: 3973
		// (get) Token: 0x06005BA1 RID: 23457 RVA: 0x002BF2DB File Offset: 0x002BD4DB
		public double LogScaleDistanceFromCamera
		{
			get
			{
				return TemplateManager.global.strategyCamera_LogScaleDistanceFromCamera;
			}
		}
	}
}
