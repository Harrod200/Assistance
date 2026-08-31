using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200000A RID: 10
public class RegionMeshBorderEffect : MonoBehaviour
{
	// Token: 0x17000006 RID: 6
	// (get) Token: 0x06000040 RID: 64 RVA: 0x000035A9 File Offset: 0x000017A9
	public Mesh BorderMesh
	{
		get
		{
			return this.m_borderMesh;
		}
	}

	// Token: 0x06000041 RID: 65 RVA: 0x000035B1 File Offset: 0x000017B1
	private void Start()
	{
		if (!RegionEffectRenderer.s_Instance)
		{
			Debug.LogWarning("No region effect renderer instance - region won't be drawn properly.");
			return;
		}
		RegionEffectRenderer.s_Instance.AddRegion(this);
		this.InitBorderMesh();
	}

	// Token: 0x06000042 RID: 66 RVA: 0x000035DC File Offset: 0x000017DC
	private void InitBorderMesh()
	{
		Mesh sharedMesh = base.GetComponent<MeshFilter>().sharedMesh;
		List<EdgeHelpers.Edge> list = EdgeHelpers.GetEdges(sharedMesh.triangles, sharedMesh.vertices).FindBoundary().SortEdges();
		List<Vector3> list2 = new List<Vector3>();
		List<int> list3 = new List<int>();
		EdgeHelpers.Edge edge = list[list.Count - 1];
		for (int i = 0; i < list.Count; i++)
		{
			Vector3 vector = Vector3.Normalize(edge.normal + list[i].normal);
			list2.Add(sharedMesh.vertices[list[i].v1]);
			list2.Add(sharedMesh.vertices[list[i].v1] - vector * 0.025f);
		}
		int num = 0;
		while (num + 3 < list2.Count)
		{
			list3.Add(num);
			list3.Add(num + 1);
			list3.Add(num + 2);
			list3.Add(num + 1);
			list3.Add(num + 3);
			list3.Add(num + 2);
			num += 2;
		}
		list3.Add(num);
		list3.Add(num + 1);
		list3.Add(0);
		list3.Add(num + 1);
		list3.Add(1);
		list3.Add(0);
		Mesh mesh = new Mesh();
		mesh.vertices = list2.ToArray();
		mesh.SetIndices(list3.ToArray(), MeshTopology.Triangles, 0);
		this.m_borderMesh = mesh;
	}

	// Token: 0x06000043 RID: 67 RVA: 0x00003760 File Offset: 0x00001960
	private void InitLineMesh()
	{
		Mesh sharedMesh = base.GetComponent<MeshFilter>().sharedMesh;
		List<EdgeHelpers.Edge> list = EdgeHelpers.GetEdges(sharedMesh.triangles).FindBoundary().SortEdges();
		List<Vector3> list2 = new List<Vector3>();
		List<int> list3 = new List<int>();
		for (int i = 0; i < list.Count; i++)
		{
			list2.Add(sharedMesh.vertices[list[i].v1]);
			list3.Add(i);
		}
		list3.Add(0);
		Mesh mesh = new Mesh();
		mesh.vertices = list2.ToArray();
		mesh.SetIndices(list3.ToArray(), MeshTopology.LineStrip, 0);
		this.m_borderMesh = mesh;
	}

	// Token: 0x04000035 RID: 53
	private Mesh m_borderMesh;
}
