using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000027 RID: 39
public class TIConeLayoutGroup : MonoBehaviour
{
	// Token: 0x060000F7 RID: 247 RVA: 0x0000852C File Offset: 0x0000672C
	public void AddItem(object item, out Vector3 position)
	{
		if (this._itemList.Contains(item))
		{
			this.GetPositionForIndex(this._itemList.IndexOf(item), out position);
			return;
		}
		int num = this._width * this._height;
		if (this._itemList.Count / num >= this._depth)
		{
			this.GetPositionForIndex(this._itemList.IndexOf(item), out position);
			return;
		}
		int count;
		if (!this.GetAvailableIndex(out count))
		{
			count = this._itemList.Count;
			this._itemList.Add(item);
		}
		this.GetPositionForIndex(count, out position);
	}

	// Token: 0x060000F8 RID: 248 RVA: 0x000085BC File Offset: 0x000067BC
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

	// Token: 0x060000F9 RID: 249 RVA: 0x00008628 File Offset: 0x00006828
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

	// Token: 0x060000FA RID: 250 RVA: 0x00008688 File Offset: 0x00006888
	private void GetPositionForIndex(int index, out Vector3 position)
	{
		int num = this._width * this._height;
		float num2 = (float)(index % num % this._width);
		int num3 = index % num / this._width;
		int num4 = index / num + 1;
		float num5 = num2 / (float)this._width;
		float num6 = (float)num3 / (float)this._height;
		Vector3 vector = base.transform.forward;
		vector = Quaternion.AngleAxis(-this.WidthRange * 0.5f, base.transform.up) * base.transform.forward;
		vector = Quaternion.AngleAxis(this.WidthRange * num5, base.transform.up) * vector;
		vector = Quaternion.AngleAxis(-this.HeightRange * 0.5f, base.transform.right) * vector;
		vector = Quaternion.AngleAxis(this.HeightRange * num6, base.transform.right) * vector;
		position = new Vector3(vector.x * this.IndexPadding * (float)num4, vector.y * this.IndexPadding * (float)num4, vector.z * this.IndexPadding * (float)num4);
		position += new Vector3(vector.x * this.PivotPadding, vector.y * this.PivotPadding, vector.z * this.PivotPadding);
		position += base.transform.position;
	}

	// Token: 0x040000F4 RID: 244
	[Header("Cone Properties")]
	[SerializeField]
	[Tooltip("")]
	[Range(0f, 360f)]
	private float WidthRange;

	// Token: 0x040000F5 RID: 245
	[SerializeField]
	[Tooltip("")]
	[Range(0f, 360f)]
	private float HeightRange;

	// Token: 0x040000F6 RID: 246
	[SerializeField]
	[Tooltip("Padding between the pivot and first index. Value is in Unity meters.")]
	[Range(0.0001f, 10f)]
	private float PivotPadding;

	// Token: 0x040000F7 RID: 247
	[SerializeField]
	[Tooltip("Padding between the individual index placements. Value is in Unity meters.")]
	private float IndexPadding;

	// Token: 0x040000F8 RID: 248
	[Header("Elements")]
	[SerializeField]
	[Tooltip("The number of elements wide per depth.")]
	private int Width;

	// Token: 0x040000F9 RID: 249
	[SerializeField]
	[Tooltip("The number of elements high per depth.")]
	private int Height;

	// Token: 0x040000FA RID: 250
	[SerializeField]
	[Tooltip("The number of rows deep before overflowing. Set to zero to ignore this limit.")]
	private int Depth;

	// Token: 0x040000FB RID: 251
	private int _width;

	// Token: 0x040000FC RID: 252
	private int _height;

	// Token: 0x040000FD RID: 253
	private int _depth;

	// Token: 0x040000FE RID: 254
	private List<object> _itemList = new List<object>();
}
