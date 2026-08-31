using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000006 RID: 6
public static class EdgeHelpers
{
	// Token: 0x0600000D RID: 13 RVA: 0x000023D4 File Offset: 0x000005D4
	private static Vector3 CalculateNormal(Vector3 v1, Vector3 v2)
	{
		Vector3 vector = Vector3.Normalize(v2 - v1);
		Vector3 vector2 = Vector3.Normalize(v2 + v1);
		return Vector3.Cross(vector, vector2);
	}

	// Token: 0x0600000E RID: 14 RVA: 0x00002400 File Offset: 0x00000600
	public static List<EdgeHelpers.Edge> GetEdges(int[] indices, Vector3[] vertices)
	{
		List<EdgeHelpers.Edge> list = new List<EdgeHelpers.Edge>();
		for (int i = 0; i < indices.Length; i += 3)
		{
			int num = indices[i];
			int num2 = indices[i + 1];
			int num3 = indices[i + 2];
			Vector3 vector = vertices[num];
			Vector3 vector2 = vertices[num2];
			Vector3 vector3 = vertices[num3];
			list.Add(new EdgeHelpers.Edge(num, num2, i, EdgeHelpers.CalculateNormal(vector, vector2)));
			list.Add(new EdgeHelpers.Edge(num2, num3, i, EdgeHelpers.CalculateNormal(vector2, vector3)));
			list.Add(new EdgeHelpers.Edge(num3, num, i, EdgeHelpers.CalculateNormal(vector3, vector)));
		}
		return list;
	}

	// Token: 0x0600000F RID: 15 RVA: 0x00002498 File Offset: 0x00000698
	public static List<EdgeHelpers.Edge> GetEdges(int[] indices)
	{
		List<EdgeHelpers.Edge> list = new List<EdgeHelpers.Edge>();
		for (int i = 0; i < indices.Length; i += 3)
		{
			int num = indices[i];
			int num2 = indices[i + 1];
			int num3 = indices[i + 2];
			list.Add(new EdgeHelpers.Edge(num, num2, i, Vector3.zero));
			list.Add(new EdgeHelpers.Edge(num2, num3, i, Vector3.zero));
			list.Add(new EdgeHelpers.Edge(num3, num, i, Vector3.zero));
		}
		return list;
	}

	// Token: 0x06000010 RID: 16 RVA: 0x00002508 File Offset: 0x00000708
	public static List<EdgeHelpers.Edge> FindBoundary(this List<EdgeHelpers.Edge> aEdges)
	{
		List<EdgeHelpers.Edge> list = new List<EdgeHelpers.Edge>(aEdges);
		for (int i = list.Count - 1; i > 0; i--)
		{
			for (int j = i - 1; j >= 0; j--)
			{
				if (list[i].v1 == list[j].v2 && list[i].v2 == list[j].v1)
				{
					list.RemoveAt(i);
					list.RemoveAt(j);
					i--;
					break;
				}
			}
		}
		return list;
	}

	// Token: 0x06000011 RID: 17 RVA: 0x00002588 File Offset: 0x00000788
	public static List<EdgeHelpers.Edge> SortEdges(this List<EdgeHelpers.Edge> aEdges)
	{
		List<EdgeHelpers.Edge> list = new List<EdgeHelpers.Edge>(aEdges);
		for (int i = 0; i < list.Count - 2; i++)
		{
			EdgeHelpers.Edge edge = list[i];
			int j = i + 1;
			while (j < list.Count)
			{
				EdgeHelpers.Edge edge2 = list[j];
				if (edge.v2 == edge2.v1)
				{
					if (j != i + 1)
					{
						list[j] = list[i + 1];
						list[i + 1] = edge2;
						break;
					}
					break;
				}
				else
				{
					j++;
				}
			}
		}
		return list;
	}

	// Token: 0x02000AAF RID: 2735
	public struct Edge
	{
		// Token: 0x060065C6 RID: 26054 RVA: 0x002FE088 File Offset: 0x002FC288
		public Edge(int aV1, int aV2, int aIndex, Vector3 normal)
		{
			this.v1 = aV1;
			this.v2 = aV2;
			this.triangleIndex = aIndex;
			this.normal = normal;
		}

		// Token: 0x04004822 RID: 18466
		public int v1;

		// Token: 0x04004823 RID: 18467
		public int v2;

		// Token: 0x04004824 RID: 18468
		public int triangleIndex;

		// Token: 0x04004825 RID: 18469
		public Vector3 normal;
	}
}
