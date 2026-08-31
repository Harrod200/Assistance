using System;
using PavonisInteractive.TerraInvicta;
using UnityEngine;

// Token: 0x02000378 RID: 888
public class L5Position : LagrangePosition
{
	// Token: 0x0600100B RID: 4107 RVA: 0x000531CF File Offset: 0x000513CF
	public override LagrangeValue GetLagrangePointNumber()
	{
		return LagrangeValue.L5;
	}

	// Token: 0x0600100C RID: 4108 RVA: 0x000531D2 File Offset: 0x000513D2
	public override Vector3d GetPosition(Vector3d position, Vector3d barycenterPos, double m1, double m2)
	{
		return position;
	}

	// Token: 0x0600100D RID: 4109 RVA: 0x000531D8 File Offset: 0x000513D8
	public override CartesianState GetCartesianState(TISpaceObjectState relatedObject, TIDateTime dateTime = null)
	{
		if (Error.IsNull<TISpaceObjectState>(relatedObject))
		{
			return CartesianState.zero;
		}
		CartesianState cartesianState = relatedObject.ToLocalCartesianStateAtTime(dateTime);
		if (relatedObject.barycenter != null)
		{
			Vector3d vector3d = cartesianState.positionDisplay;
			Vector3d vector3d2 = cartesianState.velocityDisplay;
			Vector3d normalized = Vector3d.Cross(vector3d, vector3d2).normalized;
			Quaterniond quaterniond = Quaterniond.AngleAxis(-60.0, normalized);
			vector3d = quaterniond * vector3d;
			vector3d2 = quaterniond * vector3d2;
			vector3d = relatedObject.barycenter.SpatialRotation * vector3d;
			vector3d2 = relatedObject.barycenter.SpatialRotation * vector3d2;
			cartesianState = relatedObject.barycenter.ToGlobalCartesianStateAtTime(dateTime) + new CartesianState(vector3d.xzy, vector3d2.xzy);
		}
		return cartesianState;
	}

	// Token: 0x0600100E RID: 4110 RVA: 0x00053298 File Offset: 0x00051498
	public override Vector3d GetPosition(TISpaceObjectState relatedObject, TIDateTime dateTime = null, bool display = true)
	{
		CartesianState cartesianState = this.GetCartesianState(relatedObject, dateTime);
		if (display)
		{
			return cartesianState.positionDisplay;
		}
		return cartesianState.position;
	}
}
