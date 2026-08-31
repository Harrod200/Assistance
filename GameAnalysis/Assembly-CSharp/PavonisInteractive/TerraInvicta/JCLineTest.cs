using System;
using Shapes;
using UnityEngine;
using UnityEngine.Rendering;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020007FE RID: 2046
	public class JCLineTest : MonoBehaviour
	{
		// Token: 0x06004A3C RID: 19004 RVA: 0x001F252D File Offset: 0x001F072D
		private void Start()
		{
		}

		// Token: 0x06004A3D RID: 19005 RVA: 0x001F252F File Offset: 0x001F072F
		private void Update()
		{
		}

		// Token: 0x06004A3E RID: 19006 RVA: 0x001F2531 File Offset: 0x001F0731
		public virtual void DrawShapes(Camera cam)
		{
		}

		// Token: 0x06004A3F RID: 19007 RVA: 0x001F2534 File Offset: 0x001F0734
		private void OnCameraPreRender(Camera cam)
		{
			CameraType cameraType = cam.cameraType;
			if (cameraType == CameraType.Preview || cameraType == CameraType.Reflection)
			{
				return;
			}
			if (this.useCullingMasks && (cam.cullingMask & (1 << base.gameObject.layer)) == 0)
			{
				return;
			}
			this.DrawShapesTest(cam);
		}

		// Token: 0x06004A40 RID: 19008 RVA: 0x001F257C File Offset: 0x001F077C
		public void DrawShapesTest(Camera cam)
		{
			using (Draw.Command(cam, CameraEvent.BeforeImageEffects))
			{
				Draw.BlendMode = ShapesBlendMode.Opaque;
				Draw.Thickness = 1f;
				Draw.LineGeometry = LineGeometry.Billboard;
				Draw.ThicknessSpace = ThicknessSpace.Pixels;
				Draw.Color = Color.green;
				global::UnityEngine.Random.InitState(this.seed);
				new PolylinePath();
				for (int i = 0; i < this.lineCount; i++)
				{
					Draw.Line(Vector3.zero, new Vector3(global::UnityEngine.Random.Range(-2f, 2f), global::UnityEngine.Random.Range(-2f, 2f), global::UnityEngine.Random.Range(-2f, 2f)));
				}
			}
		}

		// Token: 0x06004A41 RID: 19009 RVA: 0x001F2634 File Offset: 0x001F0834
		private void DrawLineTest()
		{
		}

		// Token: 0x06004A42 RID: 19010 RVA: 0x001F2636 File Offset: 0x001F0836
		public virtual void OnEnable()
		{
			Camera.onPreRender = (Camera.CameraCallback)Delegate.Combine(Camera.onPreRender, new Camera.CameraCallback(this.OnCameraPreRender));
		}

		// Token: 0x06004A43 RID: 19011 RVA: 0x001F2658 File Offset: 0x001F0858
		public virtual void OnDisable()
		{
			Camera.onPreRender = (Camera.CameraCallback)Delegate.Remove(Camera.onPreRender, new Camera.CameraCallback(this.OnCameraPreRender));
		}

		// Token: 0x04002B15 RID: 11029
		public bool useCullingMasks = true;

		// Token: 0x04002B16 RID: 11030
		[Header("Settings")]
		public int seed;

		// Token: 0x04002B17 RID: 11031
		[Range(1f, 4000f)]
		public int lineCount = 1000;

		// Token: 0x04002B18 RID: 11032
		[Range(1f, 4000f)]
		public int polyLineCount = 180;

		// Token: 0x04002B19 RID: 11033
		public bool use3D;
	}
}
