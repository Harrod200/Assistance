using System;
using PavonisInteractive.TerraInvicta;
using UnityEngine;

// Token: 0x02000377 RID: 887
public class L4Position : LagrangePosition
{
	// Token: 0x06001006 RID: 4102 RVA: 0x000530A9 File Offset: 0x000512A9
	public override LagrangeValue GetLagrangePointNumber()
	{
		return LagrangeValue.L4;
	}

	// Token: 0x06001007 RID: 4103 RVA: 0x000530AC File Offset: 0x000512AC
	public override Vector3d GetPosition(Vector3d position, Vector3d barycenterPos, double m1, double m2)
	{
		return position;
	}

	// Token: 0x06001008 RID: 4104 RVA: 0x000530B0 File Offset: 0x000512B0
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
			Quaterniond quaterniond = Quaterniond.AngleAxis(60.0, normalized);
			vector3d = quaterniond * vector3d;
			vector3d2 = quaterniond * vector3d2;
			vector3d = relatedObject.barycenter.SpatialRotation * vector3d;
			vector3d2 = relatedObject.barycenter.SpatialRotation * vector3d2;
			double y = vector3d.y;
			vector3d.y = vector3d.z;
			vector3d.z = y;
			double y2 = vector3d2.y;
			vector3d2.y = vector3d2.z;
			vector3d2.z = y2;
			cartesianState = relatedObject.barycenter.ToGlobalCartesianStateAtTime(dateTime) + new CartesianState(vector3d, vector3d2);
		}
		return cartesianState;
	}

	// Token: 0x06001009 RID: 4105 RVA: 0x000531A0 File Offset: 0x000513A0
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
