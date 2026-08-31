using System;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Systems.GameTime;
using Unity.Entities;
using UnityEngine;

// Token: 0x02000373 RID: 883
public abstract class LagrangePosition : TINavigablePosition
{
	// Token: 0x06000FF4 RID: 4084
	public abstract LagrangeValue GetLagrangePointNumber();

	// Token: 0x06000FF5 RID: 4085 RVA: 0x00052C30 File Offset: 0x00050E30
	public virtual CartesianState GetCartesianState(TISpaceObjectState relatedObject, TIDateTime dateTime = null)
	{
		TIDateTime tidateTime = new TIDateTime(dateTime);
		Vector3d position = this.GetPosition(relatedObject, tidateTime, false);
		tidateTime.AddSeconds(-0.5);
		Vector3d position2 = this.GetPosition(relatedObject, tidateTime, false);
		tidateTime.AddSeconds(1.0);
		Vector3d vector3d = this.GetPosition(relatedObject, tidateTime, false) - position2;
		return new CartesianState(position, vector3d);
	}

	// Token: 0x06000FF6 RID: 4086 RVA: 0x00052C8B File Offset: 0x00050E8B
	protected LagrangePosition()
	{
		if (Application.isPlaying)
		{
			this.gameTime = World.Active.GetExistingManager<GameTimeManager>();
		}
	}

	// Token: 0x06000FF7 RID: 4087 RVA: 0x00052CAC File Offset: 0x00050EAC
	public void GetStates(TISpaceObjectState relatedObject, TIDateTime dateTime = null)
	{
		if (Error.IsNull<TISpaceObjectState>(relatedObject))
		{
			return;
		}
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
		this.M1 = relatedObject.barycenter.mass_kg;
		this.M2 = relatedObject.mass_kg;
	}

	// Token: 0x06000FF8 RID: 4088 RVA: 0x00052D5E File Offset: 0x00050F5E
	public double GetHillRadius()
	{
		return this.relpos.magnitude * Mathd.Pow(this.M2 / (3.0 * this.M1), 0.3333333333333333);
	}

	// Token: 0x06000FF9 RID: 4089 RVA: 0x00052D91 File Offset: 0x00050F91
	protected double HillRadius(double d, double m1, double m2)
	{
		return d * Mathd.Pow(m2 / (3.0 * m1), 0.3333333333333333);
	}

	// Token: 0x04001041 RID: 4161
	protected CartesianState center;

	// Token: 0x04001042 RID: 4162
	protected CartesianState state;

	// Token: 0x04001043 RID: 4163
	protected Vector3d centerpos;

	// Token: 0x04001044 RID: 4164
	protected Vector3d relpos;

	// Token: 0x04001045 RID: 4165
	protected Vector3d pos;

	// Token: 0x04001046 RID: 4166
	protected Vector3d vel;

	// Token: 0x04001047 RID: 4167
	protected double M1;

	// Token: 0x04001048 RID: 4168
	protected double M2;

	// Token: 0x04001049 RID: 4169
	private GameTimeManager gameTime;
}
