using System;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Systems.GameTime;
using Unity.Entities;
using UnityEngine;

// Token: 0x02000379 RID: 889
public abstract class OrbitPosition : TINavigablePosition
{
	// Token: 0x06001010 RID: 4112 RVA: 0x000532C7 File Offset: 0x000514C7
	protected OrbitPosition()
	{
		if (Application.isPlaying)
		{
			this.gameTime = World.Active.GetExistingManager<GameTimeManager>();
		}
	}

	// Token: 0x06001011 RID: 4113 RVA: 0x000532E8 File Offset: 0x000514E8
	public override Vector3d GetPosition(TISpaceObjectState relatedObject, TIDateTime dateTime = null, bool display = true)
	{
		if (dateTime == null)
		{
			dateTime = this.gameTime.currentTime;
		}
		this.center = relatedObject.barycenter.ToGlobalCartesianStateAtTime(dateTime);
		this.centerpos = this.center.position;
		this.state = relatedObject.ToGlobalCartesianStateAtTime(dateTime);
		this.pos = this.state.position;
		this.relpos = this.pos - this.centerpos;
		this.vel = this.state.velocity;
		double num = 1.0 / this.relpos.magnitude;
		if (display)
		{
			return (1.0 - num) * this.center.positionDisplay + num * this.state.positionDisplay;
		}
		return this.center.position + num * (this.state.position - this.center.position);
	}

	// Token: 0x0400104A RID: 4170
	protected CartesianState center;

	// Token: 0x0400104B RID: 4171
	protected CartesianState state;

	// Token: 0x0400104C RID: 4172
	protected Vector3d centerpos;

	// Token: 0x0400104D RID: 4173
	protected Vector3d relpos;

	// Token: 0x0400104E RID: 4174
	protected Vector3d pos;

	// Token: 0x0400104F RID: 4175
	protected Vector3d vel;

	// Token: 0x04001050 RID: 4176
	private readonly GameTimeManager gameTime;
}
