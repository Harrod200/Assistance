using System;
using UnityEngine;

namespace Pixelplacement
{
	// Token: 0x0200051C RID: 1308
	[Serializable]
	public class SplineFollower
	{
		// Token: 0x17000480 RID: 1152
		// (get) Token: 0x06002049 RID: 8265 RVA: 0x000A7E69 File Offset: 0x000A6069
		public bool WasMoved
		{
			get
			{
				if (this.percentage != this._previousPercentage || this.faceDirection != this._previousFaceDirection)
				{
					this._previousPercentage = this.percentage;
					this._previousFaceDirection = this.faceDirection;
					return true;
				}
				return false;
			}
		}

		// Token: 0x0600204A RID: 8266 RVA: 0x000A7EA4 File Offset: 0x000A60A4
		public void UpdateOrientation(Spline spline)
		{
			if (this.target == null)
			{
				return;
			}
			if (!spline.loop)
			{
				this.percentage = Mathf.Clamp01(this.percentage);
			}
			if (this.faceDirection)
			{
				if (spline.direction == SplineDirection.Forward)
				{
					this.target.LookAt(this.target.position + spline.GetDirection(this.percentage, true));
				}
				else
				{
					this.target.LookAt(this.target.position - spline.GetDirection(this.percentage, true));
				}
			}
			this.target.position = spline.GetPosition(this.percentage, true);
		}

		// Token: 0x04001900 RID: 6400
		public Transform target;

		// Token: 0x04001901 RID: 6401
		public float percentage = -1f;

		// Token: 0x04001902 RID: 6402
		public bool faceDirection;

		// Token: 0x04001903 RID: 6403
		private float _previousPercentage;

		// Token: 0x04001904 RID: 6404
		private bool _previousFaceDirection;

		// Token: 0x04001905 RID: 6405
		private bool _detached;
	}
}
