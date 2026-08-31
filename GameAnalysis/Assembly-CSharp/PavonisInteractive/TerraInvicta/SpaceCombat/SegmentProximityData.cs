using System;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.SpaceCombat
{
	// Token: 0x020009F0 RID: 2544
	public class SegmentProximityData
	{
		// Token: 0x170010A2 RID: 4258
		// (get) Token: 0x06006053 RID: 24659 RVA: 0x002D5B4B File Offset: 0x002D3D4B
		public int WaypointID { get; }

		// Token: 0x170010A3 RID: 4259
		// (get) Token: 0x06006054 RID: 24660 RVA: 0x002D5B53 File Offset: 0x002D3D53
		public float DistanceToSegment { get; }

		// Token: 0x170010A4 RID: 4260
		// (get) Token: 0x06006055 RID: 24661 RVA: 0x002D5B5B File Offset: 0x002D3D5B
		public float FullSegmentDistance { get; }

		// Token: 0x170010A5 RID: 4261
		// (get) Token: 0x06006056 RID: 24662 RVA: 0x002D5B63 File Offset: 0x002D3D63
		public Vector3 PointOnSegment { get; }

		// Token: 0x06006058 RID: 24664 RVA: 0x002D5B77 File Offset: 0x002D3D77
		private SegmentProximityData()
			: this(-1, float.PositiveInfinity, float.PositiveInfinity, Vector3.positiveInfinity)
		{
		}

		// Token: 0x06006059 RID: 24665 RVA: 0x002D5B8F File Offset: 0x002D3D8F
		public SegmentProximityData(int waypointId, float distanceToSegment, float fullSegmentDistance, Vector3 pointOnSegment)
		{
			this.DistanceToSegment = distanceToSegment;
			this.WaypointID = waypointId;
			this.FullSegmentDistance = fullSegmentDistance;
			this.PointOnSegment = pointOnSegment;
		}

		// Token: 0x04004414 RID: 17428
		public static SegmentProximityData DefaultData = new SegmentProximityData();
	}
}
