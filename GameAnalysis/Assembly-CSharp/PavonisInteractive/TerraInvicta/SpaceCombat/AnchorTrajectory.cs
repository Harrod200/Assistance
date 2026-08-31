using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.SpaceCombat
{
	// Token: 0x020009F1 RID: 2545
	public class AnchorTrajectory : BasicWaypoint, IPreviousTrajectory, ITrajectory, IPathDetail, IWaypoint
	{
		// Token: 0x0600605A RID: 24666 RVA: 0x002D5BB4 File Offset: 0x002D3DB4
		public bool IsInBurn(TIDateTime time)
		{
			return this._nextTrajectory.IsInBurn(time);
		}

		// Token: 0x0600605B RID: 24667 RVA: 0x002D5BC2 File Offset: 0x002D3DC2
		public bool IsAcceleratingRight(TIDateTime time)
		{
			return this._nextTrajectory.IsAcceleratingRight(time);
		}

		// Token: 0x0600605C RID: 24668 RVA: 0x002D5BD0 File Offset: 0x002D3DD0
		public bool IsAcceleratingLeft(TIDateTime time)
		{
			return this._nextTrajectory.IsAcceleratingLeft(time);
		}

		// Token: 0x0600605D RID: 24669 RVA: 0x002D5BDE File Offset: 0x002D3DDE
		public bool IsAcceleratingUp(TIDateTime time)
		{
			return this._nextTrajectory.IsAcceleratingUp(time);
		}

		// Token: 0x0600605E RID: 24670 RVA: 0x002D5BEC File Offset: 0x002D3DEC
		public bool IsAcceleratingDown(TIDateTime time)
		{
			return this._nextTrajectory.IsAcceleratingDown(time);
		}

		// Token: 0x0600605F RID: 24671 RVA: 0x002D5BFA File Offset: 0x002D3DFA
		public bool IsAcceleratingRollRight(TIDateTime time)
		{
			return this._nextTrajectory.IsAcceleratingRollRight(time);
		}

		// Token: 0x06006060 RID: 24672 RVA: 0x002D5C08 File Offset: 0x002D3E08
		public bool IsAcceleratingRollLeft(TIDateTime time)
		{
			return this._nextTrajectory.IsAcceleratingRollLeft(time);
		}

		// Token: 0x06006061 RID: 24673 RVA: 0x002D5C16 File Offset: 0x002D3E16
		public Vector3 PositionAt(TIDateTime time)
		{
			return this._nextTrajectory.PositionAt(time);
		}

		// Token: 0x06006062 RID: 24674 RVA: 0x002D5C24 File Offset: 0x002D3E24
		public Vector3 VelocityAt(TIDateTime time)
		{
			return this._nextTrajectory.VelocityAt(time);
		}

		// Token: 0x06006063 RID: 24675 RVA: 0x002D5C32 File Offset: 0x002D3E32
		public Vector3 AccelerationAt(TIDateTime time)
		{
			return this._nextTrajectory.AccelerationAt(time);
		}

		// Token: 0x06006064 RID: 24676 RVA: 0x002D5C40 File Offset: 0x002D3E40
		public float AngularVelocityAt_Rad(TIDateTime time)
		{
			return this._nextTrajectory.AngularVelocityAt_Rad(time);
		}

		// Token: 0x06006065 RID: 24677 RVA: 0x002D5C4E File Offset: 0x002D3E4E
		public Vector3 HeadingAt(TIDateTime time)
		{
			return this._nextTrajectory.HeadingAt(time);
		}

		// Token: 0x06006066 RID: 24678 RVA: 0x002D5C5C File Offset: 0x002D3E5C
		public Quaternion RotationAt(TIDateTime time)
		{
			return this._nextTrajectory.RotationAt(time);
		}

		// Token: 0x06006067 RID: 24679 RVA: 0x002D5C6A File Offset: 0x002D3E6A
		public ITrajectory TrajectoryAt(TIDateTime time)
		{
			return this._nextTrajectory.TrajectoryAt(time);
		}

		// Token: 0x06006068 RID: 24680 RVA: 0x002D5C78 File Offset: 0x002D3E78
		public AnchorTrajectory(IWaypoint waypoint)
			: base(waypoint)
		{
		}

		// Token: 0x06006069 RID: 24681 RVA: 0x002D5C81 File Offset: 0x002D3E81
		public void UpdatePathNodes(TIDateTime timingStart, Camera cam, Vector3 shipPosition)
		{
			this._nextTrajectory.UpdatePathNodes(timingStart, cam, shipPosition);
		}

		// Token: 0x0600606A RID: 24682 RVA: 0x002D5C91 File Offset: 0x002D3E91
		public void SetNextTrajectory(ITrajectory nextTrajectory)
		{
			this._nextTrajectory = nextTrajectory;
		}

		// Token: 0x0600606B RID: 24683 RVA: 0x002D5C9A File Offset: 0x002D3E9A
		[return: TupleElementNames(new string[] { "time", "isBurn" })]
		public List<ValueTuple<TIDateTime, bool>> GetBurnTimings()
		{
			return this._nextTrajectory.GetBurnTimings();
		}

		// Token: 0x04004419 RID: 17433
		private ITrajectory _nextTrajectory;
	}
}
