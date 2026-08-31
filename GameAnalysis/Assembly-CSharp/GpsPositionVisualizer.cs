using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000425 RID: 1061
public class GpsPositionVisualizer : MonoBehaviour
{
	// Token: 0x0600161B RID: 5659 RVA: 0x000706DE File Offset: 0x0006E8DE
	public static void ShowPoint(float longitude, float latitude)
	{
		GpsPositionVisualizer.ShowPoint(longitude, latitude, Quaternion.identity);
	}

	// Token: 0x0600161C RID: 5660 RVA: 0x000706EC File Offset: 0x0006E8EC
	public static void ShowPoint(float longitude, float latitude, Quaternion parentRotation)
	{
		GpsPositionVisualizer.ShowPoint(longitude, latitude, parentRotation, 1f);
	}

	// Token: 0x0600161D RID: 5661 RVA: 0x000706FB File Offset: 0x0006E8FB
	public static void ShowPoint(float longitude, float latitude, Quaternion parentRotation, float parentBodyRadius)
	{
		GpsPositionVisualizer.ShowPoint(longitude, latitude, parentRotation, parentBodyRadius, Vector3.zero);
	}

	// Token: 0x0600161E RID: 5662 RVA: 0x0007070B File Offset: 0x0006E90B
	public static void ShowPoint(float longitude, float latitude, Quaternion parentRotation, float parentBodyRadius, Vector3 parentBodyWorldPosition)
	{
		GpsPositionVisualizer.Longitude_POI = longitude;
		GpsPositionVisualizer.Latitude_POI = latitude;
		GpsPositionVisualizer.ParentRadius = parentBodyRadius;
		GpsPositionVisualizer.ParenRotation = parentRotation;
		GpsPositionVisualizer.ParentCenter = parentBodyWorldPosition;
		GpsPositionVisualizer.s_instance.CalculatePoints();
	}

	// Token: 0x0600161F RID: 5663 RVA: 0x00070738 File Offset: 0x0006E938
	private void CalculatePoints()
	{
		this._gridPositions.Clear();
		if (!this.ShowParentAsWireframe)
		{
			for (int i = 0; i <= this.NumberOfLongitudePoints; i++)
			{
				for (int j = 0; j <= this.NumberOfLatitudePoints; j++)
				{
					float num = (float)i / (float)this.NumberOfLongitudePoints * 360f - 180f;
					float num2 = (float)j / (float)this.NumberOfLatitudePoints * 180f - 90f;
					this._gridPositions.Add(GpsPositionVisualizer.ParenRotation * Quaternion.AngleAxis(num, -Vector3.up) * Quaternion.AngleAxis(num2, -Vector3.right) * Vector3.forward * GpsPositionVisualizer.ParentRadius + GpsPositionVisualizer.ParentCenter);
				}
			}
		}
	}

	// Token: 0x06001620 RID: 5664 RVA: 0x0007080F File Offset: 0x0006EA0F
	private void OnValidate()
	{
		GpsPositionVisualizer.s_instance = this;
		this.CalculatePoints();
	}

	// Token: 0x06001621 RID: 5665 RVA: 0x00070820 File Offset: 0x0006EA20
	private void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		if (this.ShowParentAsWireframe)
		{
			Gizmos.DrawWireSphere(GpsPositionVisualizer.ParentCenter, GpsPositionVisualizer.ParentRadius);
		}
		else
		{
			foreach (Vector3 vector in this._gridPositions)
			{
				Gizmos.DrawSphere(vector, this.SizeOfGridPoint);
			}
		}
		Vector3 vector2 = GpsPositionVisualizer.ParenRotation * Quaternion.AngleAxis(GpsPositionVisualizer.Longitude_POI, -Vector3.up) * Quaternion.AngleAxis(GpsPositionVisualizer.Latitude_POI, -Vector3.right) * Vector3.forward * GpsPositionVisualizer.ParentRadius + GpsPositionVisualizer.ParentCenter;
		Gizmos.color = Color.white;
		Gizmos.DrawSphere(vector2, this.SizeOfPoi);
	}

	// Token: 0x04001428 RID: 5160
	private static float Longitude_POI = 0f;

	// Token: 0x04001429 RID: 5161
	private static float Latitude_POI = 0f;

	// Token: 0x0400142A RID: 5162
	private static float ParentRadius = 2f;

	// Token: 0x0400142B RID: 5163
	private static Vector3 ParentCenter = Vector3.zero;

	// Token: 0x0400142C RID: 5164
	private static Quaternion ParenRotation = Quaternion.identity;

	// Token: 0x0400142D RID: 5165
	private static GpsPositionVisualizer s_instance;

	// Token: 0x0400142E RID: 5166
	public bool ShowParentAsWireframe;

	// Token: 0x0400142F RID: 5167
	public float SizeOfPoi = 0.1f;

	// Token: 0x04001430 RID: 5168
	public float SizeOfGridPoint = 0.05f;

	// Token: 0x04001431 RID: 5169
	[Range(1f, 500f)]
	public int NumberOfLongitudePoints = 15;

	// Token: 0x04001432 RID: 5170
	[Range(1f, 500f)]
	public int NumberOfLatitudePoints = 15;

	// Token: 0x04001433 RID: 5171
	private List<Vector3> _gridPositions = new List<Vector3>();
}
