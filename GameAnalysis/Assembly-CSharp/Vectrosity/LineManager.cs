using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vectrosity
{
	// Token: 0x02000499 RID: 1177
	[AddComponentMenu("Vectrosity/LineManager")]
	public class LineManager : MonoBehaviour
	{
		// Token: 0x06001939 RID: 6457 RVA: 0x000819F8 File Offset: 0x0007FBF8
		private void Awake()
		{
			this.Initialize();
		}

		// Token: 0x0600193A RID: 6458 RVA: 0x00081A00 File Offset: 0x0007FC00
		private void Initialize()
		{
			LineManager.lines = new List<VectorLine>();
			LineManager.transforms = new List<Transform>();
			LineManager.lineCount = 0;
			base.enabled = false;
		}

		// Token: 0x0600193B RID: 6459 RVA: 0x00081A24 File Offset: 0x0007FC24
		public void AddLine(VectorLine vectorLine, Transform thisTransform, float time)
		{
			if (time > 0f)
			{
				base.StartCoroutine(this.DisableLine(vectorLine, time, false));
			}
			for (int i = 0; i < LineManager.lineCount; i++)
			{
				if (vectorLine == LineManager.lines[i])
				{
					return;
				}
			}
			LineManager.lines.Add(vectorLine);
			LineManager.transforms.Add(thisTransform);
			if (++LineManager.lineCount == 1)
			{
				base.enabled = true;
			}
		}

		// Token: 0x0600193C RID: 6460 RVA: 0x00081A95 File Offset: 0x0007FC95
		public void DisableLine(VectorLine vectorLine, float time)
		{
			base.StartCoroutine(this.DisableLine(vectorLine, time, false));
		}

		// Token: 0x0600193D RID: 6461 RVA: 0x00081AA7 File Offset: 0x0007FCA7
		private IEnumerator DisableLine(VectorLine vectorLine, float time, bool remove)
		{
			yield return new WaitForSeconds(time);
			if (remove)
			{
				this.RemoveLine(vectorLine);
			}
			else
			{
				this.RemoveLine(vectorLine);
				VectorLine.Destroy(ref vectorLine);
			}
			vectorLine = null;
			yield break;
		}

		// Token: 0x0600193E RID: 6462 RVA: 0x00081ACC File Offset: 0x0007FCCC
		private void LateUpdate()
		{
			if (!VectorLine.camTransformExists)
			{
				return;
			}
			for (int i = 0; i < LineManager.lineCount; i++)
			{
				if (LineManager.lines[i].rectTransform != null)
				{
					LineManager.lines[i].Draw3D();
				}
				else
				{
					this.RemoveLine(i--);
				}
			}
			if (VectorLine.CameraHasMoved())
			{
				VectorManager.DrawArrayLines();
			}
			VectorLine.UpdateCameraInfo();
			VectorManager.DrawArrayLines2();
		}

		// Token: 0x0600193F RID: 6463 RVA: 0x00081B3C File Offset: 0x0007FD3C
		private void RemoveLine(int i)
		{
			LineManager.lines.RemoveAt(i);
			LineManager.transforms.RemoveAt(i);
			LineManager.lineCount--;
			this.DisableIfUnused();
		}

		// Token: 0x06001940 RID: 6464 RVA: 0x00081B68 File Offset: 0x0007FD68
		public void RemoveLine(VectorLine vectorLine)
		{
			for (int i = 0; i < LineManager.lineCount; i++)
			{
				if (vectorLine == LineManager.lines[i])
				{
					this.RemoveLine(i);
					return;
				}
			}
		}

		// Token: 0x06001941 RID: 6465 RVA: 0x00081B9B File Offset: 0x0007FD9B
		public void DisableIfUnused()
		{
			if (!this.destroyed && LineManager.lineCount == 0 && VectorManager.arrayCount == 0 && VectorManager.arrayCount2 == 0)
			{
				base.enabled = false;
			}
		}

		// Token: 0x06001942 RID: 6466 RVA: 0x00081BC1 File Offset: 0x0007FDC1
		public void EnableIfUsed()
		{
			if (VectorManager.arrayCount == 1 || VectorManager.arrayCount2 == 1)
			{
				base.enabled = true;
			}
		}

		// Token: 0x06001943 RID: 6467 RVA: 0x00081BDA File Offset: 0x0007FDDA
		public void StartCheckDistance()
		{
			base.InvokeRepeating("CheckDistance", 0.01f, VectorManager.distanceCheckFrequency);
		}

		// Token: 0x06001944 RID: 6468 RVA: 0x00081BF1 File Offset: 0x0007FDF1
		private void CheckDistance()
		{
			VectorManager.CheckDistance();
		}

		// Token: 0x06001945 RID: 6469 RVA: 0x00081BF8 File Offset: 0x0007FDF8
		private void OnDestroy()
		{
			this.destroyed = true;
		}

		// Token: 0x0400163D RID: 5693
		private static List<VectorLine> lines;

		// Token: 0x0400163E RID: 5694
		private static List<Transform> transforms;

		// Token: 0x0400163F RID: 5695
		private static int lineCount;

		// Token: 0x04001640 RID: 5696
		private bool destroyed;
	}
}
