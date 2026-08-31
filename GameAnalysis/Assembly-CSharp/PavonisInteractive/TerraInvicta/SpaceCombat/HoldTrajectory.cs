using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Shapes;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.SpaceCombat
{
	// Token: 0x020009F4 RID: 2548
	public class HoldTrajectory : BasicWaypoint, IPreviousTrajectory, ITrajectory, IPathDetail, IWaypoint
	{
		// Token: 0x06006084 RID: 24708 RVA: 0x002D6708 File Offset: 0x002D4908
		protected HoldTrajectory(IWaypoint previousWaypoint)
		{
			this._previousWaypoint = previousWaypoint;
		}

		// Token: 0x06006085 RID: 24709 RVA: 0x002D675C File Offset: 0x002D495C
		protected HoldTrajectory(IPreviousTrajectory previousWaypoint)
		{
			previousWaypoint.SetNextTrajectory(this);
			this._previousWaypoint = previousWaypoint;
		}

		// Token: 0x06006086 RID: 24710 RVA: 0x002D67B4 File Offset: 0x002D49B4
		public HoldTrajectory(IPreviousTrajectory start, IWaypoint end)
			: this(start)
		{
			base.AlphaBlendValue = end.AlphaBlendValue;
			this._alphaRange.x = start.AlphaBlendValue;
			this._alphaRange.y = base.AlphaBlendValue;
			this.SetData(end);
			base.Velocity = start.Velocity;
			base.Rotation = start.Rotation;
			base.Position = this.PositionAt(base.Timing);
			base.AlphaBlendValue = end.AlphaBlendValue;
			this._alphaRange.x = start.AlphaBlendValue;
			this._alphaRange.y = base.AlphaBlendValue;
			end.Position = base.Position;
			this._totalDuration_s = (float)(base.Timing - start.Timing).TotalSeconds;
			this._mainCamera = GameControl.spaceCombat.mainCamera;
			this.InitializePathList();
		}

		// Token: 0x06006087 RID: 24711 RVA: 0x002D6898 File Offset: 0x002D4A98
		protected void InitializePathList()
		{
			float num = (float)(base.Timing - this._previousWaypoint.Timing).TotalSeconds;
			if (base.AlphaBlendValue != 1f || (this._previousWaypoint.Velocity == base.Velocity && this._previousWaypoint.Rotation == base.Rotation))
			{
				this._renderNodesCount = 2;
			}
			else
			{
				this._renderNodesCount = Mathf.Max((int)((float)this._renderNodesCount * (num / 60f)), 0);
			}
			this._pathRenderNodes = new List<Vector3>(this._renderNodesCount);
			for (int i = this._renderNodesCount; i >= 0; i--)
			{
				float num2 = num * ((float)i / (float)this._renderNodesCount);
				this._pathRenderNodes.Add(this.PositionAt(num2));
			}
		}

		// Token: 0x06006088 RID: 24712 RVA: 0x002D6968 File Offset: 0x002D4B68
		public void UpdatePathNodes(TIDateTime currentTime, Camera cam, Vector3 shipPosition)
		{
			if (GameControl.spaceCombat.IsInFormationSelectionMode)
			{
				return;
			}
			ITrajectory nextTrajectory = this._nextTrajectory;
			if (nextTrajectory != null)
			{
				nextTrajectory.UpdatePathNodes(currentTime, cam, shipPosition);
			}
			if (this._pathRenderNodes.Count <= 1)
			{
				return;
			}
			int num = 1;
			int count = this._pathRenderNodes.Count;
			float num2 = (float)(base.Timing - currentTime).TotalSeconds;
			Vector3 vector = Vector3.zero;
			Vector3 vector2 = Vector3.zero;
			Color color = this._pathLineColor;
			bool flag = Utilities.CompareColor32(this._pathLineColor, WaypointNavigationController.waypointGreenLine);
			foreach (Vector3 vector3 in this._pathRenderNodes)
			{
				if (num == 1)
				{
					vector = vector3;
				}
				float num3 = this._totalDuration_s * ((float)num / (float)count);
				Vector3 vector4 = ((num < count) ? this._pathRenderNodes[num] : this._previousWaypoint.Position);
				if (num2 > num3)
				{
					float num4 = this._totalDuration_s * ((float)num / (float)count);
					if (num2 < num4)
					{
						if (shipPosition != Vector3.zero)
						{
							vector2 = shipPosition;
							if (num == 1)
							{
								vector = shipPosition;
							}
						}
					}
					else
					{
						color.a = this._alphaRange.x + (this._alphaRange.y - this._alphaRange.x) * (1f - (float)num / (float)count);
						vector2 = vector4;
						if (!flag)
						{
							GameControl.spaceCombat.AddPath(vector3, vector4, LineEndCap.None, color);
						}
						num++;
					}
				}
				else if (shipPosition != Vector3.zero)
				{
					if (!(shipPosition != Vector3.zero))
					{
						break;
					}
					if (flag)
					{
						vector2 = shipPosition;
						break;
					}
					if (num > 1)
					{
						GameControl.spaceCombat.SetPathEndPointToShipPosition(shipPosition);
						break;
					}
					break;
				}
			}
			if (flag)
			{
				GameControl.spaceCombat.AddPath(vector, vector2, LineEndCap.None, color);
			}
		}

		// Token: 0x06006089 RID: 24713 RVA: 0x002D6B58 File Offset: 0x002D4D58
		protected float ElapsedTimeInSeconds(TIDateTime time)
		{
			return (float)(time - this._previousWaypoint.Timing).TotalSeconds;
		}

		// Token: 0x0600608A RID: 24714 RVA: 0x002D6B80 File Offset: 0x002D4D80
		public bool InCounterBurn(TIDateTime time)
		{
			return (time - this._previousWaypoint.Timing).TotalSeconds > (base.Timing - this._previousWaypoint.Timing).TotalSeconds * 0.5;
		}

		// Token: 0x0600608B RID: 24715 RVA: 0x002D6BD0 File Offset: 0x002D4DD0
		public virtual bool IsInBurn(TIDateTime time)
		{
			return time >= base.Timing && this._nextTrajectory != null && this._nextTrajectory.IsInBurn(time);
		}

		// Token: 0x0600608C RID: 24716 RVA: 0x002D6BF6 File Offset: 0x002D4DF6
		public virtual bool IsAcceleratingRight(TIDateTime time)
		{
			return time >= base.Timing && this._nextTrajectory != null && this._nextTrajectory.IsAcceleratingRight(time);
		}

		// Token: 0x0600608D RID: 24717 RVA: 0x002D6C1C File Offset: 0x002D4E1C
		public virtual bool IsAcceleratingLeft(TIDateTime time)
		{
			return time >= base.Timing && this._nextTrajectory != null && this._nextTrajectory.IsAcceleratingLeft(time);
		}

		// Token: 0x0600608E RID: 24718 RVA: 0x002D6C42 File Offset: 0x002D4E42
		public virtual bool IsAcceleratingUp(TIDateTime time)
		{
			return time >= base.Timing && this._nextTrajectory != null && this._nextTrajectory.IsAcceleratingUp(time);
		}

		// Token: 0x0600608F RID: 24719 RVA: 0x002D6C68 File Offset: 0x002D4E68
		public virtual bool IsAcceleratingDown(TIDateTime time)
		{
			return time >= base.Timing && this._nextTrajectory != null && this._nextTrajectory.IsAcceleratingDown(time);
		}

		// Token: 0x06006090 RID: 24720 RVA: 0x002D6C8E File Offset: 0x002D4E8E
		public virtual bool IsAcceleratingRollRight(TIDateTime time)
		{
			return time >= base.Timing && this._nextTrajectory != null && this._nextTrajectory.IsAcceleratingRollRight(time);
		}

		// Token: 0x06006091 RID: 24721 RVA: 0x002D6CB4 File Offset: 0x002D4EB4
		public virtual bool IsAcceleratingRollLeft(TIDateTime time)
		{
			return time >= base.Timing && this._nextTrajectory != null && this._nextTrajectory.IsAcceleratingRollLeft(time);
		}

		// Token: 0x06006092 RID: 24722 RVA: 0x002D6CDA File Offset: 0x002D4EDA
		public virtual Vector3 PositionAt(TIDateTime time)
		{
			if (time < base.Timing)
			{
				return this.PositionAt(this.ElapsedTimeInSeconds(time));
			}
			ITrajectory nextTrajectory = this._nextTrajectory;
			if (nextTrajectory == null)
			{
				return this.PositionAt(this.ElapsedTimeInSeconds(time));
			}
			return nextTrajectory.PositionAt(time);
		}

		// Token: 0x06006093 RID: 24723 RVA: 0x002D6D16 File Offset: 0x002D4F16
		public virtual Vector3 VelocityAt(TIDateTime time)
		{
			if (time < base.Timing)
			{
				return this.VelocityAt(this.ElapsedTimeInSeconds(time));
			}
			ITrajectory nextTrajectory = this._nextTrajectory;
			if (nextTrajectory == null)
			{
				return this.VelocityAt(this.ElapsedTimeInSeconds(time));
			}
			return nextTrajectory.VelocityAt(time);
		}

		// Token: 0x06006094 RID: 24724 RVA: 0x002D6D52 File Offset: 0x002D4F52
		public virtual Vector3 AccelerationAt(TIDateTime time)
		{
			if (time < base.Timing)
			{
				return this.AccelerationAt(this.ElapsedTimeInSeconds(time));
			}
			ITrajectory nextTrajectory = this._nextTrajectory;
			if (nextTrajectory == null)
			{
				return this.AccelerationAt(this.ElapsedTimeInSeconds(time));
			}
			return nextTrajectory.AccelerationAt(time);
		}

		// Token: 0x06006095 RID: 24725 RVA: 0x002D6D8E File Offset: 0x002D4F8E
		public virtual float AngularVelocityAt_Rad(TIDateTime time)
		{
			if (time < base.Timing)
			{
				return this.AngularVelocityAt(this.ElapsedTimeInSeconds(time));
			}
			ITrajectory nextTrajectory = this._nextTrajectory;
			if (nextTrajectory == null)
			{
				return this.AngularVelocityAt(this.ElapsedTimeInSeconds(time));
			}
			return nextTrajectory.AngularVelocityAt_Rad(time);
		}

		// Token: 0x06006096 RID: 24726 RVA: 0x002D6DCA File Offset: 0x002D4FCA
		public virtual Vector3 HeadingAt(TIDateTime time)
		{
			if (time < base.Timing)
			{
				return this.HeadingAt(this.ElapsedTimeInSeconds(time));
			}
			ITrajectory nextTrajectory = this._nextTrajectory;
			if (nextTrajectory == null)
			{
				return this.HeadingAt(this.ElapsedTimeInSeconds(time));
			}
			return nextTrajectory.HeadingAt(time);
		}

		// Token: 0x06006097 RID: 24727 RVA: 0x002D6E06 File Offset: 0x002D5006
		public virtual Quaternion RotationAt(TIDateTime time)
		{
			if (time < base.Timing)
			{
				return this.RotationAt(this.ElapsedTimeInSeconds(time));
			}
			ITrajectory nextTrajectory = this._nextTrajectory;
			if (nextTrajectory == null)
			{
				return this.RotationAt(this.ElapsedTimeInSeconds(time));
			}
			return nextTrajectory.RotationAt(time);
		}

		// Token: 0x06006098 RID: 24728 RVA: 0x002D6E44 File Offset: 0x002D5044
		public virtual ITrajectory TrajectoryAt(TIDateTime time)
		{
			ITrajectory trajectory;
			if (!(time < base.Timing))
			{
				ITrajectory nextTrajectory = this._nextTrajectory;
				if ((trajectory = ((nextTrajectory != null) ? nextTrajectory.TrajectoryAt(time) : null)) == null)
				{
					return this;
				}
			}
			else
			{
				trajectory = this;
			}
			return trajectory;
		}

		// Token: 0x06006099 RID: 24729 RVA: 0x002D6E7B File Offset: 0x002D507B
		protected virtual Vector3 PositionAt(float elapsedTime)
		{
			return PhysicsHelpers.PositionFromVelocityAndTime(this._previousWaypoint.Position, this._previousWaypoint.Velocity, elapsedTime);
		}

		// Token: 0x0600609A RID: 24730 RVA: 0x002D6E99 File Offset: 0x002D5099
		protected virtual Vector3 VelocityAt(float elapsedTime)
		{
			return this._previousWaypoint.Velocity;
		}

		// Token: 0x0600609B RID: 24731 RVA: 0x002D6EA6 File Offset: 0x002D50A6
		protected virtual Vector3 AccelerationAt(float elapsedTime)
		{
			return Vector3.zero;
		}

		// Token: 0x0600609C RID: 24732 RVA: 0x002D6EAD File Offset: 0x002D50AD
		protected virtual float AngularVelocityAt(float elapsedTime)
		{
			return 0f;
		}

		// Token: 0x0600609D RID: 24733 RVA: 0x002D6EB4 File Offset: 0x002D50B4
		protected virtual Vector3 HeadingAt(float heading)
		{
			return this._previousWaypoint.Heading;
		}

		// Token: 0x0600609E RID: 24734 RVA: 0x002D6EC1 File Offset: 0x002D50C1
		protected virtual Quaternion RotationAt(float elapsedTime)
		{
			return this._previousWaypoint.Rotation;
		}

		// Token: 0x0600609F RID: 24735 RVA: 0x002D6ECE File Offset: 0x002D50CE
		public void SetNextTrajectory(ITrajectory nextTrajectory)
		{
			this._nextTrajectory = nextTrajectory;
		}

		// Token: 0x060060A0 RID: 24736 RVA: 0x002D6ED8 File Offset: 0x002D50D8
		[return: TupleElementNames(new string[] { "time", "isBurn" })]
		public virtual List<ValueTuple<TIDateTime, bool>> GetBurnTimings()
		{
			if (this._nextTrajectory == null)
			{
				return new List<ValueTuple<TIDateTime, bool>>
				{
					new ValueTuple<TIDateTime, bool>(this._previousWaypoint.Timing, false),
					new ValueTuple<TIDateTime, bool>(base.Timing, false)
				};
			}
			List<ValueTuple<TIDateTime, bool>> burnTimings = this._nextTrajectory.GetBurnTimings();
			burnTimings.Insert(0, new ValueTuple<TIDateTime, bool>(this._previousWaypoint.Timing, false));
			return burnTimings;
		}

		// Token: 0x04004420 RID: 17440
		private int _renderNodesCount = 16;

		// Token: 0x04004421 RID: 17441
		protected float _totalDuration_s;

		// Token: 0x04004422 RID: 17442
		protected List<Vector3> _pathRenderNodes;

		// Token: 0x04004423 RID: 17443
		protected Color32 _pathLineColor = new Color(0f, 0.5f, 0f, 1f);

		// Token: 0x04004424 RID: 17444
		protected Vector2 _alphaRange = Vector2.one;

		// Token: 0x04004425 RID: 17445
		protected readonly IWaypoint _previousWaypoint;

		// Token: 0x04004426 RID: 17446
		protected ITrajectory _nextTrajectory;

		// Token: 0x04004427 RID: 17447
		protected Camera _mainCamera;
	}
}
