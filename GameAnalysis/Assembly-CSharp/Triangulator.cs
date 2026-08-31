using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;
using UnityEngine;

// Token: 0x0200015B RID: 347
public class Triangulator
{
	// Token: 0x0600054E RID: 1358 RVA: 0x00017668 File Offset: 0x00015868
	public Triangulator(CurvedPolyPoint[] points)
	{
		this.m_points = new List<CurvedPolyPoint>(points);
	}

	// Token: 0x0600054F RID: 1359 RVA: 0x00017688 File Offset: 0x00015888
	public List<int> Triangulate()
	{
		List<int> list = new List<int>();
		int count = this.m_points.Count;
		if (count < 3)
		{
			return list;
		}
		int[] array = new int[count];
		if (this.Area() > 0f)
		{
			for (int i = 0; i < count; i++)
			{
				array[i] = i;
			}
		}
		else
		{
			for (int j = 0; j < count; j++)
			{
				array[j] = count - 1 - j;
			}
		}
		int k = count;
		int num = 2 * k;
		int num2 = 0;
		int num3 = k - 1;
		while (k > 2)
		{
			if (num-- <= 0)
			{
				return list;
			}
			int num4 = num3;
			if (k <= num4)
			{
				num4 = 0;
			}
			num3 = num4 + 1;
			if (k <= num3)
			{
				num3 = 0;
			}
			int num5 = num3 + 1;
			if (k <= num5)
			{
				num5 = 0;
			}
			if (this.Snip(num4, num3, num5, k, array))
			{
				int num6 = array[num4];
				int num7 = array[num3];
				int num8 = array[num5];
				list.Add(num6);
				list.Add(num7);
				list.Add(num8);
				num2++;
				int num9 = num3;
				for (int l = num3 + 1; l < k; l++)
				{
					array[num9] = array[l];
					num9++;
				}
				k--;
				num = 2 * k;
			}
		}
		return list;
	}

	// Token: 0x06000550 RID: 1360 RVA: 0x000177B4 File Offset: 0x000159B4
	private float Area()
	{
		int count = this.m_points.Count;
		float num = 0f;
		int num2 = count - 1;
		int i = 0;
		while (i < count)
		{
			Vector2 vector = (Vector2)this.m_points[num2];
			Vector2 vector2 = (Vector2)this.m_points[i];
			num += vector.x * vector2.y - vector2.x * vector.y;
			num2 = i++;
		}
		return num * 0.5f;
	}

	// Token: 0x06000551 RID: 1361 RVA: 0x00017834 File Offset: 0x00015A34
	private bool Snip(int u, int v, int w, int n, int[] V)
	{
		Vector2 vector = (Vector2)this.m_points[V[u]];
		Vector2 vector2 = (Vector2)this.m_points[V[v]];
		Vector2 vector3 = (Vector2)this.m_points[V[w]];
		if (Mathf.Epsilon > (vector2.x - vector.x) * (vector3.y - vector.y) - (vector2.y - vector.y) * (vector3.x - vector.x))
		{
			return false;
		}
		for (int i = 0; i < n; i++)
		{
			if (i != u && i != v && i != w)
			{
				Vector2 vector4 = (Vector2)this.m_points[V[i]];
				if (this.InsideTriangle(vector, vector2, vector3, vector4))
				{
					return false;
				}
			}
		}
		return true;
	}

	// Token: 0x06000552 RID: 1362 RVA: 0x00017900 File Offset: 0x00015B00
	private bool InsideTriangle(Vector2 A, Vector2 B, Vector2 C, Vector2 P)
	{
		float num = C.x - B.x;
		float num2 = C.y - B.y;
		float num3 = A.x - C.x;
		float num4 = A.y - C.y;
		float num5 = B.x - A.x;
		float num6 = B.y - A.y;
		float num7 = P.x - A.x;
		float num8 = P.y - A.y;
		float num9 = P.x - B.x;
		float num10 = P.y - B.y;
		float num11 = P.x - C.x;
		float num12 = P.y - C.y;
		float num13 = num * num10 - num2 * num9;
		float num14 = num5 * num8 - num6 * num7;
		float num15 = num3 * num12 - num4 * num11;
		return num13 >= 0f && num15 >= 0f && num14 >= 0f;
	}

	// Token: 0x0400027A RID: 634
	private List<CurvedPolyPoint> m_points = new List<CurvedPolyPoint>();
}
