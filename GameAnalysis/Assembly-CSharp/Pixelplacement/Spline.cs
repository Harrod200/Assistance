using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pixelplacement
{
	// Token: 0x02000519 RID: 1305
	[ExecuteInEditMode]
	public class Spline : MonoBehaviour
	{
		// Token: 0x14000003 RID: 3
		// (add) Token: 0x06002023 RID: 8227 RVA: 0x000A6F84 File Offset: 0x000A5184
		// (remove) Token: 0x06002024 RID: 8228 RVA: 0x000A6FBC File Offset: 0x000A51BC
		public event Action OnSplineChanged;

		// Token: 0x17000478 RID: 1144
		// (get) Token: 0x06002025 RID: 8229 RVA: 0x000A6FF1 File Offset: 0x000A51F1
		// (set) Token: 0x06002026 RID: 8230 RVA: 0x000A6FF9 File Offset: 0x000A51F9
		public float Length { get; private set; }

		// Token: 0x17000479 RID: 1145
		// (get) Token: 0x06002027 RID: 8231 RVA: 0x000A7004 File Offset: 0x000A5204
		public SplineAnchor[] Anchors
		{
			get
			{
				if (this.loop != this._wasLooping)
				{
					this._previousAnchorCount = -1;
					this._wasLooping = this.loop;
				}
				if (!this.loop)
				{
					if (base.transform.childCount != this._previousAnchorCount || base.transform.childCount == 0)
					{
						this._anchors = base.GetComponentsInChildren<SplineAnchor>();
						this._previousAnchorCount = base.transform.childCount;
					}
					return this._anchors;
				}
				if (base.transform.childCount != this._previousAnchorCount || base.transform.childCount == 0)
				{
					this._anchors = base.GetComponentsInChildren<SplineAnchor>();
					Array.Resize<SplineAnchor>(ref this._anchors, this._anchors.Length + 1);
					this._anchors[this._anchors.Length - 1] = this._anchors[0];
					this._previousAnchorCount = base.transform.childCount;
				}
				return this._anchors;
			}
		}

		// Token: 0x1700047A RID: 1146
		// (get) Token: 0x06002028 RID: 8232 RVA: 0x000A70EF File Offset: 0x000A52EF
		public Color SecondaryColor
		{
			get
			{
				return Color.Lerp(this.color, Color.black, 0.2f);
			}
		}

		// Token: 0x06002029 RID: 8233 RVA: 0x000A7106 File Offset: 0x000A5306
		private void Reset()
		{
			if (this.Anchors.Length < 2)
			{
				this.AddAnchors(2 - this.Anchors.Length);
			}
		}

		// Token: 0x0600202A RID: 8234 RVA: 0x000A7124 File Offset: 0x000A5324
		private void Update()
		{
			if (this.followers != null && this.followers.Length != 0 && this.Anchors.Length >= 2)
			{
				bool flag = false;
				if (this._anchorsChanged || this._previousChildCount != base.transform.childCount || this.direction != this._previousDirection || this.loop != this._previousLoopChoice)
				{
					this._previousChildCount = base.transform.childCount;
					this._previousLoopChoice = this.loop;
					this._previousDirection = this.direction;
					this._anchorsChanged = false;
					flag = true;
				}
				for (int i = 0; i < this.followers.Length; i++)
				{
					if (this.followers[i].WasMoved || flag)
					{
						this.followers[i].UpdateOrientation(this);
					}
				}
			}
			bool flag2 = false;
			if (this.Anchors.Length > 1)
			{
				for (int j = 0; j < this.Anchors.Length; j++)
				{
					if (this.Anchors[j].Changed)
					{
						flag2 = true;
						this.Anchors[j].Changed = false;
						this._anchorsChanged = true;
					}
					if (!this.loop)
					{
						if (j == 0)
						{
							this.Anchors[j].SetTangentStatus(false, true);
						}
						else if (j == this.Anchors.Length - 1)
						{
							this.Anchors[j].SetTangentStatus(true, false);
						}
						else
						{
							this.Anchors[j].SetTangentStatus(true, true);
						}
					}
					else
					{
						this.Anchors[j].SetTangentStatus(true, true);
					}
				}
			}
			if (this._previousLength != this.Anchors.Length || flag2)
			{
				this.HangleLengthChange();
				this._previousLength = this.Anchors.Length;
			}
		}

		// Token: 0x0600202B RID: 8235 RVA: 0x000A72C7 File Offset: 0x000A54C7
		private void HangleLengthChange()
		{
			this._lengthDirty = true;
			Action onSplineChanged = this.OnSplineChanged;
			if (onSplineChanged == null)
			{
				return;
			}
			onSplineChanged();
		}

		// Token: 0x0600202C RID: 8236 RVA: 0x000A72E0 File Offset: 0x000A54E0
		private float Reparam(float percent)
		{
			if (this._lengthDirty)
			{
				this.CalculateLength();
			}
			for (int i = 0; i < this._splineReparams.Count; i++)
			{
				float num = this._splineReparams[i].length / this.Length;
				if (num == percent)
				{
					return this._splineReparams[i].percentage;
				}
				if (num > percent)
				{
					float num2 = this._splineReparams[i - 1].length / this.Length;
					float num3 = num - num2;
					float num4 = (percent - num2) / num3;
					return Mathf.Lerp(this._splineReparams[i - 1].percentage, this._splineReparams[i].percentage, num4);
				}
			}
			return 0f;
		}

		// Token: 0x0600202D RID: 8237 RVA: 0x000A73A4 File Offset: 0x000A55A4
		public void CalculateLength()
		{
			int num = (this.Anchors.Length - 1) * this._slicesPerCurve;
			this.Length = 0f;
			this._splineReparams.Clear();
			this._splineReparams.Add(new Spline.SplineReparam(0f, 0f));
			for (int i = 1; i < num + 1; i++)
			{
				float num2 = (float)i / (float)num;
				float num3 = (float)(i - 1) / (float)num;
				Vector3 position = this.GetPosition(num3, false);
				Vector3 position2 = this.GetPosition(num2, false);
				float num4 = Vector3.Distance(position, position2);
				this.Length += num4;
				this._splineReparams.Add(new Spline.SplineReparam(this.Length, num2));
			}
			this._lengthDirty = false;
		}

		// Token: 0x0600202E RID: 8238 RVA: 0x000A7457 File Offset: 0x000A5657
		public Vector3 Up(float percentage, bool normalized = true)
		{
			return Quaternion.LookRotation(this.GetDirection(percentage, normalized)) * Vector3.up;
		}

		// Token: 0x0600202F RID: 8239 RVA: 0x000A7470 File Offset: 0x000A5670
		public Vector3 Right(float percentage, bool normalized = true)
		{
			return Quaternion.LookRotation(this.GetDirection(percentage, normalized)) * Vector3.right;
		}

		// Token: 0x06002030 RID: 8240 RVA: 0x000A7489 File Offset: 0x000A5689
		public Vector3 Forward(float percentage, bool normalized = true)
		{
			return this.GetDirection(percentage, normalized);
		}

		// Token: 0x06002031 RID: 8241 RVA: 0x000A7494 File Offset: 0x000A5694
		public Vector3 GetDirection(float percentage, bool normalized = true)
		{
			if (normalized)
			{
				percentage = this.Reparam(percentage);
			}
			CurveDetail curve = this.GetCurve(percentage);
			if (curve.currentCurve < 0)
			{
				return Vector3.zero;
			}
			SplineAnchor splineAnchor = this.Anchors[curve.currentCurve];
			SplineAnchor splineAnchor2 = this.Anchors[curve.currentCurve + 1];
			return BezierCurves.GetFirstDerivative(splineAnchor.Anchor.position, splineAnchor2.Anchor.position, splineAnchor.OutTangent.position, splineAnchor2.InTangent.position, curve.currentCurvePercentage).normalized;
		}

		// Token: 0x06002032 RID: 8242 RVA: 0x000A7524 File Offset: 0x000A5724
		public Vector3 GetPosition(float percentage, bool normalized = true)
		{
			if (normalized)
			{
				percentage = this.Reparam(percentage);
			}
			CurveDetail curve = this.GetCurve(percentage);
			if (curve.currentCurve < 0)
			{
				return Vector3.zero;
			}
			SplineAnchor splineAnchor = this.Anchors[curve.currentCurve];
			SplineAnchor splineAnchor2 = this.Anchors[curve.currentCurve + 1];
			return BezierCurves.GetPoint(splineAnchor.Anchor.position, splineAnchor2.Anchor.position, splineAnchor.OutTangent.position, splineAnchor2.InTangent.position, curve.currentCurvePercentage, true, 100);
		}

		// Token: 0x06002033 RID: 8243 RVA: 0x000A75AC File Offset: 0x000A57AC
		public Vector3 GetPosition(float percentage, Vector3 relativeOffset, bool normalized = true)
		{
			if (normalized)
			{
				percentage = this.Reparam(percentage);
			}
			Vector3 position = this.GetPosition(percentage, true);
			Quaternion quaternion = Quaternion.LookRotation(this.GetDirection(percentage, true));
			Vector3 vector = quaternion * Vector3.up;
			Vector3 vector2 = quaternion * Vector3.right;
			Vector3 vector3 = quaternion * Vector3.forward;
			return position + vector2 * relativeOffset.x + vector * relativeOffset.y + vector3 * relativeOffset.z;
		}

		// Token: 0x06002034 RID: 8244 RVA: 0x000A7630 File Offset: 0x000A5830
		public float ClosestPoint(Vector3 point, int divisions = 100)
		{
			if (divisions <= 0)
			{
				divisions = 1;
			}
			float num = float.MaxValue;
			Vector3 zero = Vector3.zero;
			Vector3 zero2 = Vector3.zero;
			float num2 = 0f;
			for (float num3 = 0f; num3 < (float)(divisions + 1); num3 += 1f)
			{
				float num4 = num3 / (float)divisions;
				float sqrMagnitude = (this.GetPosition(num4, true) - point).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					num = sqrMagnitude;
					num2 = num4;
				}
			}
			return num2;
		}

		// Token: 0x06002035 RID: 8245 RVA: 0x000A76B0 File Offset: 0x000A58B0
		public GameObject[] AddAnchors(int count)
		{
			GameObject gameObject = Resources.Load("Anchor") as GameObject;
			GameObject[] array = new GameObject[count];
			for (int i = 0; i < count; i++)
			{
				Transform transform = null;
				Transform transform2 = null;
				if (this.Anchors.Length == 1)
				{
					transform = base.transform;
					transform2 = this.Anchors[0].transform;
				}
				else if (this.Anchors.Length > 1)
				{
					transform = this.Anchors[this.Anchors.Length - 2].transform;
					transform2 = this.Anchors[this.Anchors.Length - 1].transform;
				}
				GameObject gameObject2 = global::UnityEngine.Object.Instantiate<GameObject>(gameObject);
				gameObject2.name = gameObject2.name.Replace("(Clone)", "");
				SplineAnchor component = gameObject2.GetComponent<SplineAnchor>();
				component.tangentMode = this.defaultTangentMode;
				gameObject2.transform.parent = base.transform;
				gameObject2.transform.rotation = Quaternion.LookRotation(base.transform.forward);
				component.InTangent.Translate(Vector3.up * 0.5f);
				component.OutTangent.Translate(Vector3.up * -0.5f);
				if (transform != null && transform2 != null)
				{
					Vector3 vector = (transform2.position - transform.position).normalized;
					if (vector == Vector3.zero)
					{
						vector = base.transform.forward;
					}
					gameObject2.transform.position = transform2.transform.position + vector * 1.5f;
				}
				else
				{
					gameObject2.transform.localPosition = Vector3.zero;
				}
				array[i] = gameObject2;
			}
			return array;
		}

		// Token: 0x06002036 RID: 8246 RVA: 0x000A7870 File Offset: 0x000A5A70
		public CurveDetail GetCurve(float percentage)
		{
			if (this.loop)
			{
				percentage = Mathf.Repeat(percentage, 1f);
			}
			else
			{
				percentage = Mathf.Clamp01(percentage);
			}
			if (this.Anchors.Length == 2)
			{
				if (this.direction == SplineDirection.Backwards)
				{
					percentage = 1f - percentage;
				}
				return new CurveDetail(0, percentage);
			}
			this._curveCount = this.Anchors.Length - 1;
			this._currentCurve = (float)this._curveCount * percentage;
			if ((int)this._currentCurve == this._curveCount)
			{
				this._currentCurve = (float)(this._curveCount - 1);
				this._curvePercentage = 1f;
			}
			else
			{
				this._curvePercentage = this._currentCurve - (float)((int)this._currentCurve);
			}
			this._currentCurve = (float)((int)this._currentCurve);
			this._operatingCurve = (int)this._currentCurve;
			if (this.direction == SplineDirection.Backwards)
			{
				this._curvePercentage = 1f - this._curvePercentage;
				this._operatingCurve = this._curveCount - 1 - this._operatingCurve;
			}
			return new CurveDetail(this._operatingCurve, this._curvePercentage);
		}

		// Token: 0x040018D8 RID: 6360
		public Color color = Color.yellow;

		// Token: 0x040018D9 RID: 6361
		[Range(0f, 1f)]
		public float toolScale = 0.1f;

		// Token: 0x040018DA RID: 6362
		public TangentMode defaultTangentMode;

		// Token: 0x040018DB RID: 6363
		public SplineDirection direction;

		// Token: 0x040018DC RID: 6364
		public bool loop;

		// Token: 0x040018DD RID: 6365
		public SplineFollower[] followers;

		// Token: 0x040018DE RID: 6366
		private SplineAnchor[] _anchors;

		// Token: 0x040018DF RID: 6367
		private int _curveCount;

		// Token: 0x040018E0 RID: 6368
		private int _previousAnchorCount;

		// Token: 0x040018E1 RID: 6369
		private int _previousChildCount;

		// Token: 0x040018E2 RID: 6370
		private bool _wasLooping;

		// Token: 0x040018E3 RID: 6371
		private bool _previousLoopChoice;

		// Token: 0x040018E4 RID: 6372
		private bool _anchorsChanged;

		// Token: 0x040018E5 RID: 6373
		private SplineDirection _previousDirection;

		// Token: 0x040018E6 RID: 6374
		private float _curvePercentage;

		// Token: 0x040018E7 RID: 6375
		private int _operatingCurve;

		// Token: 0x040018E8 RID: 6376
		private float _currentCurve;

		// Token: 0x040018E9 RID: 6377
		private int _previousLength;

		// Token: 0x040018EA RID: 6378
		private int _slicesPerCurve = 10;

		// Token: 0x040018EB RID: 6379
		private List<Spline.SplineReparam> _splineReparams = new List<Spline.SplineReparam>();

		// Token: 0x040018EC RID: 6380
		private bool _lengthDirty = true;

		// Token: 0x02000C8A RID: 3210
		private class SplineReparam
		{
			// Token: 0x06006D1A RID: 27930 RVA: 0x0030A58D File Offset: 0x0030878D
			public SplineReparam(float length, float percentage)
			{
				this.length = length;
				this.percentage = percentage;
			}

			// Token: 0x04004EDF RID: 20191
			public float length;

			// Token: 0x04004EE0 RID: 20192
			public float percentage;
		}
	}
}
