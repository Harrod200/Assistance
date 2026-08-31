using System;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.SpaceCombat
{
	// Token: 0x020009E6 RID: 2534
	public class BasicWaypoint : IWaypoint
	{
		// Token: 0x1700108E RID: 4238
		// (get) Token: 0x06006006 RID: 24582 RVA: 0x002D595C File Offset: 0x002D3B5C
		// (set) Token: 0x06006007 RID: 24583 RVA: 0x002D5964 File Offset: 0x002D3B64
		public float AlphaBlendValue { get; set; } = 1f;

		// Token: 0x1700108F RID: 4239
		// (get) Token: 0x06006008 RID: 24584 RVA: 0x002D596D File Offset: 0x002D3B6D
		public BasicWaypoint.WaypointOrientation PreviousOrientation
		{
			get
			{
				return this._previousOrientation;
			}
		}

		// Token: 0x17001090 RID: 4240
		// (get) Token: 0x06006009 RID: 24585 RVA: 0x002D5975 File Offset: 0x002D3B75
		// (set) Token: 0x0600600A RID: 24586 RVA: 0x002D597D File Offset: 0x002D3B7D
		public Vector3 Position { get; set; }

		// Token: 0x17001091 RID: 4241
		// (get) Token: 0x0600600B RID: 24587 RVA: 0x002D5986 File Offset: 0x002D3B86
		// (set) Token: 0x0600600C RID: 24588 RVA: 0x002D598E File Offset: 0x002D3B8E
		public Vector3 Velocity { get; set; }

		// Token: 0x17001092 RID: 4242
		// (get) Token: 0x0600600D RID: 24589 RVA: 0x002D5997 File Offset: 0x002D3B97
		// (set) Token: 0x0600600E RID: 24590 RVA: 0x002D599F File Offset: 0x002D3B9F
		public Quaternion Rotation { get; set; }

		// Token: 0x17001093 RID: 4243
		// (get) Token: 0x0600600F RID: 24591 RVA: 0x002D59A8 File Offset: 0x002D3BA8
		// (set) Token: 0x06006010 RID: 24592 RVA: 0x002D59BA File Offset: 0x002D3BBA
		public Vector3 Heading
		{
			get
			{
				return this.Rotation * Vector3.forward;
			}
			set
			{
				this.Rotation = Quaternion.FromToRotation(Vector3.forward, value);
			}
		}

		// Token: 0x06006011 RID: 24593 RVA: 0x002D59D0 File Offset: 0x002D3BD0
		public void SetHeading(Vector3 headingDirection, bool preserveRoll)
		{
			if (preserveRoll)
			{
				Vector3 eulerAngles = this.Rotation.eulerAngles;
				this.Rotation = Quaternion.FromToRotation(Vector3.forward, headingDirection);
				this.Rotation = Quaternion.Euler(this.Rotation.eulerAngles.x, this.Rotation.eulerAngles.y, eulerAngles.z);
				return;
			}
			this.Rotation = Quaternion.FromToRotation(Vector3.forward, headingDirection);
		}

		// Token: 0x17001094 RID: 4244
		// (get) Token: 0x06006012 RID: 24594 RVA: 0x002D5A49 File Offset: 0x002D3C49
		// (set) Token: 0x06006013 RID: 24595 RVA: 0x002D5A51 File Offset: 0x002D3C51
		public TIDateTime Timing { get; set; }

		// Token: 0x06006014 RID: 24596 RVA: 0x002D5A5A File Offset: 0x002D3C5A
		protected BasicWaypoint()
		{
			this.SetData(Vector3.zero, Vector3.zero, Quaternion.identity, new TIDateTime(), 1f);
		}

		// Token: 0x06006015 RID: 24597 RVA: 0x002D5A8C File Offset: 0x002D3C8C
		protected BasicWaypoint(IWaypoint waypoint)
		{
			this.SetData(waypoint.Position, waypoint.Velocity, waypoint.Rotation, new TIDateTime(waypoint.Timing), waypoint.AlphaBlendValue);
		}

		// Token: 0x06006016 RID: 24598 RVA: 0x002D5AC8 File Offset: 0x002D3CC8
		public virtual void SetData(IWaypoint waypoint)
		{
			this.SetData(waypoint.Position, waypoint.Velocity, waypoint.Rotation, new TIDateTime(waypoint.Timing), waypoint.AlphaBlendValue);
		}

		// Token: 0x06006017 RID: 24599 RVA: 0x002D5AF3 File Offset: 0x002D3CF3
		protected void SetData(Vector3 position, Vector3 velocity, Quaternion rotation, TIDateTime timing, float alphaBlendValue)
		{
			this.AlphaBlendValue = alphaBlendValue;
			this.Position = position;
			this.Velocity = velocity;
			this.Rotation = rotation;
			this.Timing = timing;
		}

		// Token: 0x0400440D RID: 17421
		protected BasicWaypoint.WaypointOrientation _previousOrientation;

		// Token: 0x02001386 RID: 4998
		public struct WaypointOrientation
		{
			// Token: 0x040071B9 RID: 29113
			public Vector3 Position;

			// Token: 0x040071BA RID: 29114
			public Quaternion Rotation;

			// Token: 0x040071BB RID: 29115
			public Vector3 Velocity;

			// Token: 0x040071BC RID: 29116
			public TIDateTime Timing;
		}
	}
}
