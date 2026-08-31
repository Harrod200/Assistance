using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000023 RID: 35
public class CombatFormationVisualTestScript : MonoBehaviour
{
	// Token: 0x060000E7 RID: 231 RVA: 0x00007C22 File Offset: 0x00005E22
	private void Awake()
	{
		this._render = true;
	}

	// Token: 0x060000E8 RID: 232 RVA: 0x00007C2C File Offset: 0x00005E2C
	private void OnDrawGizmos()
	{
		if (!this._render)
		{
			return;
		}
		this.points.Clear();
		Quaternion quaternion = base.transform.rotation;
		float num = this.MaxDepthDegrees / (float)(this.ShipCount_PerRow.Length + 1);
		for (int i = 0; i < this.ShipCount_PerRow.Length; i++)
		{
			quaternion = Quaternion.AngleAxis(num, -base.transform.right) * quaternion;
			for (int j = 0; j < this.ShipCount_PerRow[i]; j++)
			{
				if (this.ShipCount_PerRow[i] > 0)
				{
					Vector3 vector = Quaternion.AngleAxis(this.MaxRotationDegrees / (float)this.ShipCount_PerRow[i] * (float)j + 1f, base.transform.forward) * quaternion * Vector3.forward + base.transform.position;
					Vector3 normalized = (vector - base.transform.position).normalized;
					vector += normalized.x * this.ShipSpacing.x * base.transform.right;
					vector += normalized.y * this.ShipSpacing.y * base.transform.up;
					vector += normalized.z * this.ShipSpacing.z * base.transform.forward;
					this.points.Add(vector);
				}
			}
		}
		foreach (Vector3 vector2 in this.points)
		{
			Gizmos.color = this.DebugColor;
			Gizmos.DrawSphere(vector2, this.PointSize);
		}
	}

	// Token: 0x040000D6 RID: 214
	public int ShipCountTotal;

	// Token: 0x040000D7 RID: 215
	public int[] ShipCount_PerRow;

	// Token: 0x040000D8 RID: 216
	public float MaxDepthDegrees = 180f;

	// Token: 0x040000D9 RID: 217
	public float MaxRotationDegrees = 360f;

	// Token: 0x040000DA RID: 218
	public Vector3 ShipSpacing = Vector3.zero;

	// Token: 0x040000DB RID: 219
	public float PointSize = 0.1f;

	// Token: 0x040000DC RID: 220
	public Color DebugColor = Color.green;

	// Token: 0x040000DD RID: 221
	private List<Vector3> points = new List<Vector3>();

	// Token: 0x040000DE RID: 222
	private bool _render;
}
