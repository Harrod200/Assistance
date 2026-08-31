using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000028 RID: 40
public class TIConeLayoutState
{
	// Token: 0x060000FC RID: 252 RVA: 0x00008828 File Offset: 0x00006A28
	public TIConeLayoutState(in Vector3d position, in Quaternion rotation, in int width = 2, in int height = 2, in int depth = 0)
	{
		this._worldPosition = position;
		this._worldRotation = rotation;
		this._width = width;
		this._height = height;
		if (depth > 0)
		{
			this._depth = depth;
			return;
		}
		this._depth = int.MaxValue;
	}

	// Token: 0x060000FD RID: 253 RVA: 0x0000888A File Offset: 0x00006A8A
	public void SetOrientation(in Vector3d position, in Quaternion rotation)
	{
		this._worldPosition = position;
		this._worldRotation = rotation;
	}

	// Token: 0x060000FE RID: 254 RVA: 0x000088A4 File Offset: 0x00006AA4
	public bool TryAddItem(object item, out Vector3d position)
	{
		if (this._itemList.Contains(item))
		{
			this.GetPositionForIndex(this._itemList.IndexOf(item), out position);
			return true;
		}
		int num = this._width * this._height;
		if (this._itemList.Count / num < this._depth)
		{
			int count;
			if (!this.GetAvailableIndex(out count))
			{
				count = this._itemList.Count;
				this._itemList.Add(item);
			}
			this._itemList[count] = item;
			this.GetPositionForIndex(count, out position);
			return true;
		}
		position = Vector3d.zero;
		return false;
	}

	// Token: 0x060000FF RID: 255 RVA: 0x0000893C File Offset: 0x00006B3C
	public void RemoveItem(object item)
	{
		bool flag = false;
		int num = 0;
		foreach (object obj in this._itemList)
		{
			if (item == obj)
			{
				flag = true;
				break;
			}
			num++;
		}
		if (flag)
		{
			this._itemList[num] = null;
		}
	}

	// Token: 0x06000100 RID: 256 RVA: 0x000089A8 File Offset: 0x00006BA8
	private bool GetAvailableIndex(out int index)
	{
		index = 0;
		using (List<object>.Enumerator enumerator = this._itemList.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current == null)
				{
					return true;
				}
				index++;
			}
		}
		index = -1;
		return false;
	}

	// Token: 0x06000101 RID: 257 RVA: 0x00008A08 File Offset: 0x00006C08
	private void GetPositionForIndex(int index, out Vector3d position)
	{
		int num = this._width * this._height;
		float num2 = (float)(index % num % this._width);
		int num3 = index % num / this._width;
		int num4 = index / num + 1;
		float num5 = num2 / (float)this._width;
		float num6 = (float)num3 / (float)this._height;
		Vector3 vector = this._worldRotation * Vector3.forward;
		Vector3 vector2 = this._worldRotation * Vector3.up;
		Vector3 vector3 = this._worldRotation * Vector3.right;
		vector = Quaternion.AngleAxis(-45f, vector2) * vector;
		vector = Quaternion.AngleAxis(90f * num5, vector2) * vector;
		vector = Quaternion.AngleAxis(-45f, vector3) * vector;
		vector = Quaternion.AngleAxis(90f * num6, vector3) * vector;
		position = new Vector3d(vector.x * 900f * (float)num4, vector.y * 900f * (float)num4, vector.z * 900f * (float)num4);
		position += new Vector3d(vector.x * 1500f, vector.y * 1500f, vector.z * 1500f);
		position += this._worldPosition;
	}

	// Token: 0x040000FF RID: 255
	private const float WidthRange = 90f;

	// Token: 0x04000100 RID: 256
	private const float HeightRange = 90f;

	// Token: 0x04000101 RID: 257
	private const float PivotPadding = 1500f;

	// Token: 0x04000102 RID: 258
	private const float IndexPadding = 900f;

	// Token: 0x04000103 RID: 259
	[SerializeField]
	private int _width;

	// Token: 0x04000104 RID: 260
	[SerializeField]
	private int _height;

	// Token: 0x04000105 RID: 261
	[SerializeField]
	private int _depth;

	// Token: 0x04000106 RID: 262
	[SerializeField]
	private Vector3d _worldPosition;

	// Token: 0x04000107 RID: 263
	[SerializeField]
	private Quaternion _worldRotation;

	// Token: 0x04000108 RID: 264
	[SerializeField]
	private List<object> _itemList = new List<object>();
}
