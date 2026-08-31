using System;
using System.Collections.Generic;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000567 RID: 1383
	[Serializable]
	public class TIRegionOutline
	{
		// Token: 0x060024B9 RID: 9401 RVA: 0x000C5170 File Offset: 0x000C3370
		public Mesh ToMesh()
		{
			List<int> list = new List<int>();
			List<Vector2> list2 = new List<Vector2>();
			foreach (CurvedPolygon curvedPolygon in this.poly2DList)
			{
				List<int> list3 = new Triangulator(curvedPolygon.data).Triangulate();
				int count = list2.Count;
				for (int i = 0; i < list3.Count; i++)
				{
					List<int> list4 = list3;
					int j = i;
					list4[j] += count;
				}
				list.AddRange(list3);
				CurvedPolyPoint[] data = curvedPolygon.data;
				for (int j = 0; j < data.Length; j++)
				{
					Vector2 vector = (Vector2)data[j];
					list2.Add(vector);
				}
			}
			Vector3[] array = new Vector3[list2.Count];
			for (int k = 0; k < array.Length; k++)
			{
				array[k] = new Vector3(list2[k].x, -list2[k].y, 0f);
			}
			Mesh mesh = new Mesh();
			mesh.vertices = array;
			mesh.SetTriangles(list.ToArray(), 0);
			mesh.RecalculateNormals();
			mesh.RecalculateBounds();
			return mesh;
		}

		// Token: 0x060024BA RID: 9402 RVA: 0x000C52CC File Offset: 0x000C34CC
		public Mesh ToMesh2()
		{
			CombineInstance[] array = new CombineInstance[this.poly2DList.Count];
			int num = 0;
			Mesh mesh;
			foreach (CurvedPolygon curvedPolygon in this.poly2DList)
			{
				List<int> list = new Triangulator(curvedPolygon.data).Triangulate();
				Vector3[] array2 = new Vector3[curvedPolygon.data.Length];
				for (int i = 0; i < array2.Length; i++)
				{
					array2[i] = new Vector3(curvedPolygon.data[i].x, -curvedPolygon.data[i].y);
				}
				mesh = new Mesh();
				mesh.vertices = array2;
				mesh.SetTriangles(list.ToArray(), 0);
				mesh.RecalculateBounds();
				array[num].mesh = mesh;
				num++;
			}
			mesh = new Mesh();
			mesh.name = this.regionName;
			mesh.CombineMeshes(array, true, false);
			mesh.RecalculateNormals();
			mesh.RecalculateBounds();
			mesh.uv = this.CalculateLinearUVForMesh(mesh);
			return mesh;
		}

		// Token: 0x060024BB RID: 9403 RVA: 0x000C5404 File Offset: 0x000C3604
		public Mesh ToBorder(float width)
		{
			int num = 0;
			foreach (CurvedPolygon curvedPolygon in this.poly2DList)
			{
				num += curvedPolygon.data.Length;
			}
			CombineInstance[] array = new CombineInstance[num];
			int num2 = 0;
			foreach (CurvedPolygon curvedPolygon2 in this.poly2DList)
			{
				for (int i = 0; i < curvedPolygon2.data.Length; i++)
				{
					int num3 = curvedPolygon2.data.Length;
					int num4 = i;
					int num5 = (num4 + 1) % num3;
					int num6 = (num5 + 1) % num3;
					int num7 = (num6 + 1) % num3;
					array[num2++].mesh = this.ToQuad((Vector2)curvedPolygon2.data[num4], (Vector2)curvedPolygon2.data[num5], (Vector2)curvedPolygon2.data[num6], (Vector2)curvedPolygon2.data[num7], width);
				}
			}
			Mesh mesh = new Mesh();
			mesh.name = this.regionName;
			mesh.CombineMeshes(array, true, false);
			mesh.RecalculateBounds();
			return mesh;
		}

		// Token: 0x060024BC RID: 9404 RVA: 0x000C5574 File Offset: 0x000C3774
		public Mesh ToQuad(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float inset)
		{
			Mesh mesh = new Mesh();
			p0.y *= -1f;
			p1.y *= -1f;
			p2.y *= -1f;
			p3.y *= -1f;
			Vector2 vector = ((p0 - p1).normalized + (p2 - p1).normalized).normalized;
			if (Vector3.Cross(p1 - p0, vector).z > 0f)
			{
				vector *= -1f;
			}
			vector = inset * vector + p1;
			Vector2 vector2 = ((p1 - p2).normalized + (p3 - p2).normalized).normalized;
			if (Vector3.Cross(p2 - p1, vector2).z > 0f)
			{
				vector2 *= -1f;
			}
			vector2 = inset * vector2 + p2;
			mesh.vertices = new Vector3[] { p1, vector, vector2, p2 };
			mesh.triangles = new int[] { 0, 2, 1, 0, 3, 2 };
			mesh.uv = new Vector2[]
			{
				new Vector2(0f, 0f),
				new Vector2(1f, 0f),
				new Vector2(1f, 1f),
				new Vector2(0f, 1f)
			};
			mesh.normals = new Vector3[]
			{
				new Vector3(0f, 0f, 1f),
				new Vector3(0f, 0f, 1f),
				new Vector3(0f, 0f, 1f),
				new Vector3(0f, 0f, 1f)
			};
			return mesh;
		}

		// Token: 0x060024BD RID: 9405 RVA: 0x000C57D0 File Offset: 0x000C39D0
		public Vector2[] CalculateClampedUVForMesh(Mesh msh)
		{
			Vector2[] array = new Vector2[msh.vertexCount];
			Vector2 vector = new Vector2(msh.bounds.min.x, msh.bounds.max.y);
			Vector2 vector2 = new Vector2(msh.bounds.max.x, msh.bounds.max.y);
			Vector2 vector3 = new Vector2(msh.bounds.min.x, msh.bounds.min.y);
			Vector2 vector4 = new Vector2(msh.bounds.max.x, msh.bounds.min.y);
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			float num5 = 1E+10f;
			float num6 = 1E+10f;
			float num7 = 1E+10f;
			float num8 = 1E+10f;
			int num9 = 0;
			foreach (Vector3 vector5 in msh.vertices)
			{
				if ((vector - vector5).magnitude < num5)
				{
					num5 = (vector - vector5).magnitude;
					num = num9;
				}
				if ((vector2 - vector5).magnitude < num6)
				{
					num6 = (vector2 - vector5).magnitude;
					num2 = num9;
				}
				if ((vector3 - vector5).magnitude < num7)
				{
					num7 = (vector3 - vector5).magnitude;
					num3 = num9;
				}
				if ((vector4 - vector5).magnitude < num8)
				{
					num8 = (vector4 - vector5).magnitude;
					num4 = num9;
				}
				num9++;
			}
			float[] array2 = this.ComputeArcLengths(msh.vertices, num3, num);
			float num10 = array2[array2.Length - 1];
			for (int j = 0; j < array2.Length; j++)
			{
				int num11 = (j + num3) % msh.vertexCount;
				array[num11] = new Vector2(0f, array2[j] / num10);
			}
			array2 = this.ComputeArcLengths(msh.vertices, num, num2);
			num10 = array2[array2.Length - 1];
			for (int k = 0; k < array2.Length; k++)
			{
				int num11 = (k + num) % msh.vertexCount;
				array[num11] = new Vector2(array2[k] / num10, 1f);
			}
			array2 = this.ComputeArcLengths(msh.vertices, num2, num4);
			num10 = array2[array2.Length - 1];
			for (int l = 0; l < array2.Length; l++)
			{
				int num11 = (l + num2) % msh.vertexCount;
				array[num11] = new Vector2(1f, (num10 - array2[l]) / num10);
			}
			array2 = this.ComputeArcLengths(msh.vertices, num4, num3);
			num10 = array2[array2.Length - 1];
			for (int m = 0; m < array2.Length; m++)
			{
				int num11 = (m + num4) % msh.vertexCount;
				array[num11] = new Vector2((num10 - array2[m]) / num10, 0f);
			}
			return array;
		}

		// Token: 0x060024BE RID: 9406 RVA: 0x000C5B44 File Offset: 0x000C3D44
		public float[] ComputeArcLengths(Vector3[] verts, int startIdx, int endIdx)
		{
			float num = 0f;
			int num2 = (verts.Length + endIdx - startIdx) % verts.Length;
			float[] array = new float[num2 + 1];
			for (int i = 1; i <= num2; i++)
			{
				int num3 = (i + startIdx) % verts.Length;
				num += (verts[num3] - verts[(num3 + 1) % verts.Length]).magnitude;
				array[i] = num;
			}
			return array;
		}

		// Token: 0x060024BF RID: 9407 RVA: 0x000C5BB0 File Offset: 0x000C3DB0
		public Vector2[] CalculateLinearUVForMesh(Mesh msh)
		{
			Vector2[] array = new Vector2[msh.vertexCount];
			float x = msh.bounds.min.x;
			float num = msh.bounds.max.x - x;
			float y = msh.bounds.min.y;
			float num2 = msh.bounds.max.y - y;
			int num3 = 0;
			foreach (Vector3 vector in msh.vertices)
			{
				float num4 = (vector.x - x) / num;
				float num5 = (vector.y - y) / num2;
				array[num3] = new Vector2(num4, num5);
				num3++;
			}
			return array;
		}

		// Token: 0x04001B9C RID: 7068
		public string name;

		// Token: 0x04001B9D RID: 7069
		public string regionName = "New Region";

		// Token: 0x04001B9E RID: 7070
		public string nationTag;

		// Token: 0x04001B9F RID: 7071
		public List<CurvedPolygon> poly2DList;

		// Token: 0x04001BA0 RID: 7072
		public List<Vector3List> regionShapes;

		// Token: 0x04001BA1 RID: 7073
		public List<Vector3Array> regionSurfacePoints;

		// Token: 0x04001BA2 RID: 7074
		public List<LabelPosition> labelPositions;
	}
}
