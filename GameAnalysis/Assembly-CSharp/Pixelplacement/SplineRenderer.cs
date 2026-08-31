using System;
using UnityEngine;

namespace Pixelplacement
{
	// Token: 0x0200051E RID: 1310
	[ExecuteInEditMode]
	[RequireComponent(typeof(LineRenderer))]
	[RequireComponent(typeof(Spline))]
	public class SplineRenderer : MonoBehaviour
	{
		// Token: 0x0600204D RID: 8269 RVA: 0x000A7F70 File Offset: 0x000A6170
		private void Reset()
		{
			this._lineRenderer = base.GetComponent<LineRenderer>();
			this._initialized = false;
			this._lineRenderer.startWidth = 0.03f;
			this._lineRenderer.endWidth = 0.03f;
			this._lineRenderer.startColor = Color.white;
			this._lineRenderer.endColor = Color.yellow;
			this._lineRenderer.material = Resources.Load("SplineRenderer") as Material;
		}

		// Token: 0x0600204E RID: 8270 RVA: 0x000A7FEC File Offset: 0x000A61EC
		private void Update()
		{
			if (!this._initialized)
			{
				this._lineRenderer = base.GetComponent<LineRenderer>();
				this._spline = base.GetComponent<Spline>();
				this.ConfigureLineRenderer();
				this.UpdateLineRenderer();
				this._initialized = true;
			}
			if (this.segmentsPerCurve != this._previousSegmentsPerCurve || this._previousAnchorsLength != this._spline.Anchors.Length)
			{
				this.ConfigureLineRenderer();
				this.UpdateLineRenderer();
			}
			if (this._spline.Anchors.Length <= 1)
			{
				this._lineRenderer.positionCount = 0;
				return;
			}
			foreach (SplineAnchor splineAnchor in this._spline.Anchors)
			{
				if (splineAnchor.RenderingChange)
				{
					splineAnchor.RenderingChange = false;
					this.UpdateLineRenderer();
				}
			}
			if (this.startPercentage != this._previousStart || this.endPercentage != this._previousEnd)
			{
				this.UpdateLineRenderer();
				this._previousStart = this.startPercentage;
				this._previousEnd = this.endPercentage;
			}
		}

		// Token: 0x0600204F RID: 8271 RVA: 0x000A80E8 File Offset: 0x000A62E8
		private void UpdateLineRenderer()
		{
			if (this._spline.Anchors.Length < 2)
			{
				return;
			}
			for (int i = 0; i < this._vertexCount; i++)
			{
				float num = (float)i / (float)(this._vertexCount - 1);
				float num2 = Mathf.Lerp(this.startPercentage, this.endPercentage, num);
				this._lineRenderer.SetPosition(i, this._spline.GetPosition(num2, false));
			}
		}

		// Token: 0x06002050 RID: 8272 RVA: 0x000A8150 File Offset: 0x000A6350
		private void ConfigureLineRenderer()
		{
			this.segmentsPerCurve = Mathf.Max(0, this.segmentsPerCurve);
			this._vertexCount = this.segmentsPerCurve * (this._spline.Anchors.Length - 1) + 2;
			if (Mathf.Sign((float)this._vertexCount) == 1f)
			{
				this._lineRenderer.positionCount = this._vertexCount;
			}
			this._previousSegmentsPerCurve = this.segmentsPerCurve;
			this._previousAnchorsLength = this._spline.Anchors.Length;
		}

		// Token: 0x04001906 RID: 6406
		public int segmentsPerCurve = 25;

		// Token: 0x04001907 RID: 6407
		[Range(0f, 1f)]
		public float startPercentage;

		// Token: 0x04001908 RID: 6408
		[Range(0f, 1f)]
		public float endPercentage = 1f;

		// Token: 0x04001909 RID: 6409
		private LineRenderer _lineRenderer;

		// Token: 0x0400190A RID: 6410
		private Spline _spline;

		// Token: 0x0400190B RID: 6411
		private bool _initialized;

		// Token: 0x0400190C RID: 6412
		private int _previousAnchorsLength;

		// Token: 0x0400190D RID: 6413
		private int _previousSegmentsPerCurve;

		// Token: 0x0400190E RID: 6414
		private int _vertexCount;

		// Token: 0x0400190F RID: 6415
		private float _previousStart;

		// Token: 0x04001910 RID: 6416
		private float _previousEnd;
	}
}
