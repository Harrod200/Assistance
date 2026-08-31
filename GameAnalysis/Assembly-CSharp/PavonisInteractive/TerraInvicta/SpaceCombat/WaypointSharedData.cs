using System;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.SpaceCombat
{
	// Token: 0x020009FB RID: 2555
	public class WaypointSharedData
	{
		// Token: 0x170010C5 RID: 4293
		// (get) Token: 0x060061AE RID: 25006 RVA: 0x002DE9A7 File Offset: 0x002DCBA7
		public float WaypointTimeDelta { get; }

		// Token: 0x170010C6 RID: 4294
		// (get) Token: 0x060061AF RID: 25007 RVA: 0x002DE9AF File Offset: 0x002DCBAF
		// (set) Token: 0x060061B0 RID: 25008 RVA: 0x002DE9B7 File Offset: 0x002DCBB7
		public float LinearAcceleration { get; private set; }

		// Token: 0x170010C7 RID: 4295
		// (get) Token: 0x060061B1 RID: 25009 RVA: 0x002DE9C0 File Offset: 0x002DCBC0
		// (set) Token: 0x060061B2 RID: 25010 RVA: 0x002DE9C8 File Offset: 0x002DCBC8
		public float CruiseAcceleration { get; private set; }

		// Token: 0x170010C8 RID: 4296
		// (get) Token: 0x060061B3 RID: 25011 RVA: 0x002DE9D1 File Offset: 0x002DCBD1
		// (set) Token: 0x060061B4 RID: 25012 RVA: 0x002DE9D9 File Offset: 0x002DCBD9
		public float MaxAngularVelocity { get; private set; }

		// Token: 0x170010C9 RID: 4297
		// (get) Token: 0x060061B5 RID: 25013 RVA: 0x002DE9E2 File Offset: 0x002DCBE2
		// (set) Token: 0x060061B6 RID: 25014 RVA: 0x002DE9EA File Offset: 0x002DCBEA
		public float AngularAccelerationRads { get; private set; }

		// Token: 0x170010CA RID: 4298
		// (get) Token: 0x060061B7 RID: 25015 RVA: 0x002DE9F3 File Offset: 0x002DCBF3
		public Transform WaypointPrefab { get; }

		// Token: 0x060061B8 RID: 25016 RVA: 0x002DE9FB File Offset: 0x002DCBFB
		public WaypointSharedData(Transform waypointPrefab, float waypointTimeDelta, float linearAcceleration, float cruiseAcceleration, float angularAccelerationRads, float maxAngularVelocity)
		{
			this.WaypointPrefab = waypointPrefab;
			this.WaypointTimeDelta = waypointTimeDelta;
			this.LinearAcceleration = linearAcceleration;
			this.CruiseAcceleration = cruiseAcceleration;
			this.AngularAccelerationRads = angularAccelerationRads;
			this.MaxAngularVelocity = maxAngularVelocity;
		}

		// Token: 0x060061B9 RID: 25017 RVA: 0x002DEA30 File Offset: 0x002DCC30
		public void UpdatePropulsionValues(float linearAcceleration, float cruiseAcceleration, float angularAccelerationRads, float maxAngularVelocity)
		{
			this.LinearAcceleration = linearAcceleration;
			this.CruiseAcceleration = this.CruiseAcceleration;
			this.AngularAccelerationRads = angularAccelerationRads;
			this.MaxAngularVelocity = maxAngularVelocity;
		}
	}
}
