using System;
using UnityEngine;

// Token: 0x02000409 RID: 1033
public class MeshRange : MonoBehaviour
{
	// Token: 0x0600152E RID: 5422 RVA: 0x00067294 File Offset: 0x00065494
	private void Start()
	{
	}

	// Token: 0x0600152F RID: 5423 RVA: 0x00067298 File Offset: 0x00065498
	private void Update()
	{
		if (Input.GetKeyUp(KeyCode.Alpha2))
		{
			Mesh mesh = base.GetComponent<MeshFilter>().mesh;
			this.meshVerts = mesh.vertices;
			foreach (Vector3 vector in this.meshVerts)
			{
				this.vertPos = base.transform.TransformPoint(vector);
				Debug.Log(this.vertPos);
				float num = Vector3.Distance(this.vertPos, base.transform.position);
				Debug.Log(num);
				if (num > this.range)
				{
					Vector3.MoveTowards(this.vertPos, this.referencePoint.transform.position, num);
					vector.Set(this.vertPos.x, this.vertPos.y, this.vertPos.z);
				}
			}
			mesh.vertices = this.meshVerts;
			mesh.RecalculateBounds();
			mesh.RecalculateNormals();
		}
	}

	// Token: 0x0400129E RID: 4766
	public GameObject referencePoint;

	// Token: 0x0400129F RID: 4767
	public float range = 20f;

	// Token: 0x040012A0 RID: 4768
	public Vector3[] meshVerts;

	// Token: 0x040012A1 RID: 4769
	public Vector3 vertPos;
}
