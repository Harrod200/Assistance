using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200044A RID: 1098
public class UILineRenderer : Graphic
{
	// Token: 0x1700033D RID: 829
	// (get) Token: 0x06001709 RID: 5897 RVA: 0x00076FF9 File Offset: 0x000751F9
	public override Texture mainTexture
	{
		get
		{
			if (!(this.m_Texture == null))
			{
				return this.m_Texture;
			}
			return Graphic.s_WhiteTexture;
		}
	}

	// Token: 0x1700033E RID: 830
	// (get) Token: 0x0600170A RID: 5898 RVA: 0x00077015 File Offset: 0x00075215
	// (set) Token: 0x0600170B RID: 5899 RVA: 0x0007701D File Offset: 0x0007521D
	public Texture texture
	{
		get
		{
			return this.m_Texture;
		}
		set
		{
			if (this.m_Texture == value)
			{
				return;
			}
			this.m_Texture = value;
			this.SetVerticesDirty();
			this.SetMaterialDirty();
		}
	}

	// Token: 0x1700033F RID: 831
	// (get) Token: 0x0600170C RID: 5900 RVA: 0x00077041 File Offset: 0x00075241
	// (set) Token: 0x0600170D RID: 5901 RVA: 0x00077049 File Offset: 0x00075249
	public Rect uvRect
	{
		get
		{
			return this.m_UVRect;
		}
		set
		{
			if (this.m_UVRect == value)
			{
				return;
			}
			this.m_UVRect = value;
			this.SetVerticesDirty();
		}
	}

	// Token: 0x0600170E RID: 5902 RVA: 0x00077068 File Offset: 0x00075268
	protected new void OnPopulateMesh(Mesh toFill)
	{
		if (this.Points == null || this.Points.Length < 2)
		{
			this.Points = new Vector2[]
			{
				new Vector2(0f, 0f),
				new Vector2(1f, 1f)
			};
		}
		int num = 24;
		float num2 = base.rectTransform.rect.width;
		float num3 = base.rectTransform.rect.height;
		float num4 = -base.rectTransform.pivot.x * base.rectTransform.rect.width;
		float num5 = -base.rectTransform.pivot.y * base.rectTransform.rect.height;
		if (!this.relativeSize)
		{
			num2 = 1f;
			num3 = 1f;
		}
		List<Vector2> list = new List<Vector2>();
		list.Add(this.Points[0]);
		Vector2 vector = this.Points[0] + (this.Points[1] - this.Points[0]).normalized * (float)num;
		list.Add(vector);
		for (int i = 1; i < this.Points.Length - 1; i++)
		{
			list.Add(this.Points[i]);
		}
		vector = this.Points[this.Points.Length - 1] - (this.Points[this.Points.Length - 1] - this.Points[this.Points.Length - 2]).normalized * (float)num;
		list.Add(vector);
		list.Add(this.Points[this.Points.Length - 1]);
		Vector2[] array = list.ToArray();
		if (this.UseMargins)
		{
			num2 -= this.Margin.x;
			num3 -= this.Margin.y;
			num4 += this.Margin.x / 2f;
			num5 += this.Margin.y / 2f;
		}
		toFill.Clear();
		VertexHelper vertexHelper = new VertexHelper(toFill);
		Vector2 vector2 = Vector2.zero;
		Vector2 vector3 = Vector2.zero;
		for (int j = 1; j < array.Length; j++)
		{
			Vector2 vector4 = array[j - 1];
			Vector2 vector5 = array[j];
			vector4 = new Vector2(vector4.x * num2 + num4, vector4.y * num3 + num5);
			vector5 = new Vector2(vector5.x * num2 + num4, vector5.y * num3 + num5);
			float num6 = Mathf.Atan2(vector5.y - vector4.y, vector5.x - vector4.x) * 180f / 3.1415927f;
			Vector2 vector6 = vector4 + new Vector2(0f, -this.LineThickness / 2f);
			Vector2 vector7 = vector4 + new Vector2(0f, this.LineThickness / 2f);
			Vector2 vector8 = vector5 + new Vector2(0f, this.LineThickness / 2f);
			Vector2 vector9 = vector5 + new Vector2(0f, -this.LineThickness / 2f);
			vector6 = this.RotatePointAroundPivot(vector6, vector4, new Vector3(0f, 0f, num6));
			vector7 = this.RotatePointAroundPivot(vector7, vector4, new Vector3(0f, 0f, num6));
			vector8 = this.RotatePointAroundPivot(vector8, vector5, new Vector3(0f, 0f, num6));
			vector9 = this.RotatePointAroundPivot(vector9, vector5, new Vector3(0f, 0f, num6));
			Vector2 zero = Vector2.zero;
			Vector2 vector10 = new Vector2(0f, 1f);
			Vector2 vector11 = new Vector2(0.5f, 0f);
			Vector2 vector12 = new Vector2(0.5f, 1f);
			Vector2 vector13 = new Vector2(1f, 0f);
			Vector2 vector14 = new Vector2(1f, 1f);
			Vector2[] array2 = new Vector2[] { vector11, vector12, vector12, vector11 };
			if (j > 1)
			{
				this.SetVbo(vertexHelper, new Vector2[] { vector2, vector3, vector6, vector7 }, array2);
			}
			if (j == 1)
			{
				array2 = new Vector2[] { zero, vector10, vector12, vector11 };
			}
			else if (j == array.Length - 1)
			{
				array2 = new Vector2[] { vector11, vector12, vector14, vector13 };
			}
			vertexHelper.AddUIVertexQuad(this.SetVbo(vertexHelper, new Vector2[] { vector6, vector7, vector8, vector9 }, array2));
			vertexHelper.FillMesh(toFill);
			vector2 = vector8;
			vector3 = vector9;
		}
	}

	// Token: 0x0600170F RID: 5903 RVA: 0x00077618 File Offset: 0x00075818
	protected UIVertex[] SetVbo(VertexHelper vbo, Vector2[] vertices, Vector2[] uvs)
	{
		UIVertex[] array = new UIVertex[4];
		for (int i = 0; i < vertices.Length; i++)
		{
			UIVertex simpleVert = UIVertex.simpleVert;
			simpleVert.color = this.color;
			simpleVert.position = vertices[i];
			simpleVert.uv0 = uvs[i];
			array[i] = simpleVert;
		}
		return array;
	}

	// Token: 0x06001710 RID: 5904 RVA: 0x00077684 File Offset: 0x00075884
	public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
	{
		Vector3 vector = point - pivot;
		vector = Quaternion.Euler(angles) * vector;
		point = vector + pivot;
		return point;
	}

	// Token: 0x04001586 RID: 5510
	[SerializeField]
	private Texture m_Texture;

	// Token: 0x04001587 RID: 5511
	[SerializeField]
	private Rect m_UVRect = new Rect(0f, 0f, 1f, 1f);

	// Token: 0x04001588 RID: 5512
	public float LineThickness = 2f;

	// Token: 0x04001589 RID: 5513
	public bool UseMargins;

	// Token: 0x0400158A RID: 5514
	public Vector2 Margin;

	// Token: 0x0400158B RID: 5515
	public Vector2[] Points;

	// Token: 0x0400158C RID: 5516
	public bool relativeSize;
}
