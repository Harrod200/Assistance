using System;
using System.Linq;
using UnityEngine;

// Token: 0x0200040A RID: 1034
public class NoseWeaponCone : MonoBehaviour
{
	// Token: 0x06001531 RID: 5425 RVA: 0x000673AD File Offset: 0x000655AD
	private void Start()
	{
	}

	// Token: 0x06001532 RID: 5426 RVA: 0x000673B0 File Offset: 0x000655B0
	public void CreateCone(float angle)
	{
		this.numCapRows = Mathf.Clamp(this.numCapRows, 0, int.MaxValue);
		this.openingAngle = angle;
		if (this.openingAngle > 0f && this.openingAngle < 180f)
		{
			this.radiusTop = 0f;
			this.radiusBottom = this.length * Mathf.Tan(this.openingAngle * 0.017453292f / 2f);
		}
		string text = string.Concat(new string[]
		{
			"RangeCone",
			this.numVertices.ToString(),
			"v",
			this.radiusTop.ToString(),
			"t",
			this.radiusBottom.ToString(),
			"b",
			this.length.ToString(),
			"l",
			this.length.ToString(),
			this.outside ? "o" : "",
			this.inside ? "i" : ""
		});
		Mesh mesh = new Mesh();
		mesh = new Mesh();
		mesh.name = text;
		int num = (this.outside ? 1 : 0) + (this.inside ? 1 : 0);
		int num2 = ((this.outside && this.inside) ? (2 * this.numVertices) : 0);
		Vector3[] array = new Vector3[2 * num * this.numVertices];
		Vector3[] array2 = new Vector3[2 * num * this.numVertices];
		Vector2[] array3 = new Vector2[2 * num * this.numVertices];
		this.CreateSideVerticesNormalsUVs(ref array, ref array2, ref array3, num2);
		if (this.numCapRows > 0)
		{
			Vector3[] array4 = new Vector3[2 * num * this.numVertices * this.numCapRows];
			Vector3[] array5 = new Vector3[2 * num * this.numVertices * this.numCapRows];
			Vector2[] array6 = new Vector2[2 * num * this.numVertices * this.numCapRows];
			array = this.ConcatArrays<Vector3>(new Vector3[][] { array, array4 });
			array2 = this.ConcatArrays<Vector3>(new Vector3[][] { array2, array5 });
			array3 = this.ConcatArrays<Vector2>(new Vector2[][] { array3, array6 });
		}
		mesh.vertices = array;
		mesh.normals = array2;
		mesh.uv = array3;
		int[] array7;
		this.CreateSideTris(out array7, num, num2);
		if (this.numCapRows > 0)
		{
			array7 = this.ConcatArrays<int>(new int[][] { array7 });
		}
		mesh.triangles = array7;
		base.GetComponent<MeshFilter>().mesh = mesh;
	}

	// Token: 0x06001533 RID: 5427 RVA: 0x00067648 File Offset: 0x00065848
	private void CreateSideVerticesNormalsUVs(ref Vector3[] vertices, ref Vector3[] normals, ref Vector2[] uvs, int offset)
	{
		float num = Mathf.Atan((this.radiusBottom - this.radiusTop) / this.length);
		float num2 = Mathf.Sin(num);
		float num3 = Mathf.Cos(num);
		Vector3 vector = Quaternion.Euler(num * 57.29578f / (float)this.numCapRows * (float)(this.numCapRows - 1), num * 57.29578f / (float)this.numCapRows * (float)(this.numCapRows - 1), 0f) * (Vector3.forward * this.length);
		for (int i = 0; i < this.numVertices; i++)
		{
			float num4 = 6.2831855f * (float)i / (float)this.numVertices;
			float num5 = Mathf.Sin(num4);
			float num6 = Mathf.Cos(num4);
			float num7 = 6.2831855f * ((float)i + 0.5f) / (float)this.numVertices;
			float num8 = Mathf.Sin(num7);
			float num9 = Mathf.Cos(num7);
			vertices[i] = new Vector3(this.radiusTop * num6, this.radiusTop * num5, 0f);
			vertices[i + this.numVertices] = new Vector3(this.radiusBottom * num6, this.radiusBottom * num5, vector.z);
			if (this.radiusTop == 0f)
			{
				normals[i] = new Vector3(num9 * num3, num8 * num3, -num2);
			}
			else
			{
				normals[i] = new Vector3(num6 * num3, num5 * num3, -num2);
			}
			if (this.radiusBottom == 0f)
			{
				normals[i + this.numVertices] = new Vector3(num9 * num3, num8 * num3, -num2);
			}
			else
			{
				normals[i + this.numVertices] = new Vector3(num6 * num3, num5 * num3, -num2);
			}
			uvs[i] = new Vector2(1f * (float)i / (float)this.numVertices, 1f);
			uvs[i + this.numVertices] = new Vector2(1f * (float)i / (float)this.numVertices, 0f);
			if (this.outside && this.inside)
			{
				vertices[i + 2 * this.numVertices] = vertices[i];
				vertices[i + 3 * this.numVertices] = vertices[i + this.numVertices];
				uvs[i + 2 * this.numVertices] = uvs[i];
				uvs[i + 3 * this.numVertices] = uvs[i + this.numVertices];
			}
			if (this.inside)
			{
				normals[i + offset] = -normals[i];
				normals[i + this.numVertices + offset] = -normals[i + this.numVertices];
			}
		}
	}

	// Token: 0x06001534 RID: 5428 RVA: 0x00067934 File Offset: 0x00065B34
	private void CreateSideTris(out int[] tris, int multiplier, int offset)
	{
		int num = 0;
		if (this.radiusTop == 0f)
		{
			tris = new int[this.numVertices * 3 * multiplier];
			if (this.outside)
			{
				for (int i = 0; i < this.numVertices; i++)
				{
					tris[num++] = i + this.numVertices;
					tris[num++] = i;
					if (i == this.numVertices - 1)
					{
						tris[num++] = this.numVertices;
					}
					else
					{
						tris[num++] = i + 1 + this.numVertices;
					}
				}
			}
			if (this.inside)
			{
				for (int i = offset; i < this.numVertices + offset; i++)
				{
					tris[num++] = i;
					tris[num++] = i + this.numVertices;
					if (i == this.numVertices - 1 + offset)
					{
						tris[num++] = this.numVertices + offset;
					}
					else
					{
						tris[num++] = i + 1 + this.numVertices;
					}
				}
				return;
			}
		}
		else if (this.radiusBottom == 0f)
		{
			tris = new int[this.numVertices * 3 * multiplier];
			if (this.outside)
			{
				for (int i = 0; i < this.numVertices; i++)
				{
					tris[num++] = i;
					if (i == this.numVertices - 1)
					{
						tris[num++] = 0;
					}
					else
					{
						tris[num++] = i + 1;
					}
					tris[num++] = i + this.numVertices;
				}
			}
			if (this.inside)
			{
				for (int i = offset; i < this.numVertices + offset; i++)
				{
					if (i == this.numVertices - 1 + offset)
					{
						tris[num++] = offset;
					}
					else
					{
						tris[num++] = i + 1;
					}
					tris[num++] = i;
					tris[num++] = i + this.numVertices;
				}
				return;
			}
		}
		else
		{
			tris = new int[this.numVertices * 6 * multiplier];
			if (this.outside)
			{
				for (int i = 0; i < this.numVertices; i++)
				{
					int num2 = i + 1;
					if (num2 == this.numVertices)
					{
						num2 = 0;
					}
					tris[num++] = i;
					tris[num++] = num2;
					tris[num++] = i + this.numVertices;
					tris[num++] = num2 + this.numVertices;
					tris[num++] = i + this.numVertices;
					tris[num++] = num2;
				}
			}
			if (this.inside)
			{
				for (int i = offset; i < this.numVertices + offset; i++)
				{
					int num3 = i + 1;
					if (num3 == this.numVertices + offset)
					{
						num3 = offset;
					}
					tris[num++] = num3;
					tris[num++] = i;
					tris[num++] = i + this.numVertices;
					tris[num++] = i + this.numVertices;
					tris[num++] = num3 + this.numVertices;
					tris[num++] = num3;
				}
			}
		}
	}

	// Token: 0x06001535 RID: 5429 RVA: 0x00067BF8 File Offset: 0x00065DF8
	private void CreateCapVerticesNormalsUVs(ref Vector3[] vertices, ref Vector3[] normals, ref Vector2[] uvs, int offset)
	{
		float num = Mathf.Atan((this.radiusBottom - this.radiusTop) / this.length) * 57.29578f;
		for (int i = 0; i < this.numCapRows; i++)
		{
			Vector3 vector = Quaternion.Euler(num / (float)this.numCapRows * (float)i, num / (float)this.numCapRows * (float)i, 0f) * (Vector3.forward * (this.length * 0.97f));
			Vector3 vector2 = Quaternion.Euler(num / (float)this.numCapRows * (float)(i + 1), num / (float)this.numCapRows * (float)(i + 1), 0f) * (Vector3.forward * (this.length * 0.97f));
			for (int j = 0; j < this.numVertices; j++)
			{
				int num2 = j + this.numVertices * i;
				int num3 = j + this.numVertices + this.numVertices * i;
				vertices[num2] = this.RotateRadiansOnAxis(vector, Vector3.forward, 360f / (float)this.numVertices * (float)j * 0.017453292f);
				vertices[num3] = this.RotateRadiansOnAxis(vector2, Vector3.forward, 360f / (float)this.numVertices * (float)j * 0.017453292f);
				if (num2 < this.numVertices)
				{
					normals[num2] = Vector3.forward;
				}
				else
				{
					Vector3 vector3 = Quaternion.Euler(num / (float)this.numCapRows * (float)(i - 1), num / (float)this.numCapRows * (float)(i - 1), 0f) * (Vector3.forward * (this.length * 0.97f));
					normals[num2] = Vector3.Cross((this.RotateRadiansOnAxis(vector3, Vector3.forward, 360f / (float)this.numVertices * (float)j * 0.017453292f) - vertices[num2]).normalized, (this.RotateRadiansOnAxis(vector, Vector3.forward, 360f / (float)this.numVertices * (float)(j - 1) * 0.017453292f) - vertices[num2]).normalized);
				}
				normals[num3] = Vector3.Cross((vertices[num2] - vertices[num3]).normalized, this.RotateRadiansOnAxis(vector2, Vector3.forward, 360f / (float)this.numVertices * (float)j + 0.017453292f) - vertices[num3]).normalized;
				uvs[num2] = Vector2.one;
				uvs[num3] = Vector2.one;
				if (this.outside && this.inside)
				{
					vertices[num2 + offset] = vertices[num2];
					vertices[num3 + offset] = vertices[num3];
					uvs[num2 + offset] = uvs[num2];
					uvs[num3 + offset] = uvs[num3];
					normals[num2 + offset] = -normals[num2];
					normals[num3 + this.numVertices + offset] = -normals[num3];
				}
				else if (this.inside)
				{
					normals[num2] = -normals[num2];
					normals[num3] = -normals[num3];
				}
			}
		}
	}

	// Token: 0x06001536 RID: 5430 RVA: 0x00067F98 File Offset: 0x00066198
	private void CreateCapTris(out int[] tris, int multiplier, int offset)
	{
		int num = 0;
		tris = new int[this.numVertices * this.numCapRows * 6 * multiplier];
		if (this.outside)
		{
			for (int i = 0; i < this.numCapRows; i++)
			{
				for (int j = 0; j < this.numVertices; j++)
				{
					tris[num++] = j + this.numVertices * i;
					tris[num++] = j + this.numVertices + this.numVertices * i;
					if (j == this.numVertices - 1)
					{
						tris[num++] = this.numVertices + this.numVertices * i;
					}
					else
					{
						tris[num++] = j + 1 + this.numVertices + this.numVertices * i;
					}
					if (j == this.numVertices - 1)
					{
						tris[num++] = j + this.numVertices * i;
						tris[num++] = j + 1 + this.numVertices * i;
						tris[num++] = this.numVertices * i;
					}
					else
					{
						tris[num++] = j + 1 + this.numVertices + this.numVertices * i;
						tris[num++] = j + 1 + this.numVertices * i;
						tris[num++] = j + this.numVertices * i;
					}
				}
			}
		}
		if (this.inside)
		{
			for (int k = 0; k < this.numCapRows; k++)
			{
				for (int j = 0; j < this.numVertices; j++)
				{
					tris[num++] = j + this.numVertices * k + offset;
					if (j == this.numVertices - 1)
					{
						tris[num++] = this.numVertices + this.numVertices * k + offset;
					}
					else
					{
						tris[num++] = j + 1 + this.numVertices + this.numVertices * k + offset;
					}
					tris[num++] = j + this.numVertices + this.numVertices * k + offset;
					if (j == this.numVertices - 1)
					{
						tris[num++] = j + this.numVertices * k + offset;
						tris[num++] = this.numVertices * k + offset;
						tris[num++] = j + 1 + this.numVertices * k + offset;
					}
					else
					{
						tris[num++] = j + 1 + this.numVertices + this.numVertices * k + offset;
						tris[num++] = j + this.numVertices * k + offset;
						tris[num++] = j + 1 + this.numVertices * k + offset;
					}
				}
			}
		}
	}

	// Token: 0x06001537 RID: 5431 RVA: 0x00068224 File Offset: 0x00066424
	private T[] ConcatArrays<T>(params T[][] list)
	{
		T[] array = new T[list.Sum<T[]>((T[] a) => a.Length)];
		int num = 0;
		for (int i = 0; i < list.Length; i++)
		{
			list[i].CopyTo(array, num);
			num += list[i].Length;
		}
		return array;
	}

	// Token: 0x06001538 RID: 5432 RVA: 0x00068280 File Offset: 0x00066480
	private Vector3 RotateRadiansOnAxis(Vector3 v, Vector3 axis, float radians)
	{
		return v * Mathf.Cos(radians) + Vector3.Dot(v, axis) * axis * (1f - Mathf.Cos(radians)) + Vector3.Cross(axis, v) * Mathf.Sin(radians);
	}

	// Token: 0x040012A2 RID: 4770
	public int numCapRows = 4;

	// Token: 0x040012A3 RID: 4771
	public int numVertices = 40;

	// Token: 0x040012A4 RID: 4772
	public float radiusTop = 1f;

	// Token: 0x040012A5 RID: 4773
	public float radiusBottom;

	// Token: 0x040012A6 RID: 4774
	public float length = 1f;

	// Token: 0x040012A7 RID: 4775
	public float openingAngle = 45f;

	// Token: 0x040012A8 RID: 4776
	public bool outside = true;

	// Token: 0x040012A9 RID: 4777
	public bool inside = true;

	// Token: 0x040012AA RID: 4778
	public bool addCollider;
}
