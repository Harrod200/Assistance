using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020005A0 RID: 1440
	public class SolarSysModelController : MonoBehaviour
	{
		// Token: 0x17000574 RID: 1396
		// (get) Token: 0x060026C0 RID: 9920 RVA: 0x000D2DA6 File Offset: 0x000D0FA6
		// (set) Token: 0x060026C1 RID: 9921 RVA: 0x000D2DAE File Offset: 0x000D0FAE
		public SpaceObjectController container { get; protected set; }

		// Token: 0x060026C2 RID: 9922 RVA: 0x000D2DB8 File Offset: 0x000D0FB8
		protected void SetShadowBehavior()
		{
			SpaceObjectType objectType = this.container.spaceObjectState.objectType;
			if (objectType - SpaceObjectType.Star <= 6 || objectType == SpaceObjectType.LagrangePoint)
			{
				foreach (Renderer renderer in base.GetComponentsInChildren<Renderer>())
				{
					renderer.receiveShadows = false;
					renderer.shadowCastingMode = ShadowCastingMode.On;
				}
				return;
			}
			foreach (Renderer renderer2 in base.GetComponentsInChildren<Renderer>())
			{
				renderer2.receiveShadows = true;
				renderer2.shadowCastingMode = ShadowCastingMode.On;
			}
		}

		// Token: 0x060026C3 RID: 9923 RVA: 0x000D2E2C File Offset: 0x000D102C
		public virtual void InitializeModel(SpaceObjectController container)
		{
			this.container = container;
			foreach (object obj in base.transform)
			{
				((Transform)obj).SetLayer(LayerMask.NameToLayer("Solar System"), true);
			}
			RotateCloudsSolarSystemScene componentInChildren = base.GetComponentInChildren<RotateCloudsSolarSystemScene>();
			if (componentInChildren != null)
			{
				componentInChildren.InitAlbedoControl();
			}
			this.SetShadowBehavior();
		}
	}
}
