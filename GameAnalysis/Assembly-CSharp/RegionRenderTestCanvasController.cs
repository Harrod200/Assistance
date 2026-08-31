using System;
using PavonisInteractive.TerraInvicta;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200000B RID: 11
public class RegionRenderTestCanvasController : MonoBehaviour
{
	// Token: 0x06000045 RID: 69 RVA: 0x00003810 File Offset: 0x00001A10
	private void Start()
	{
		this.liftSlider.value = 0f;
	}

	// Token: 0x06000046 RID: 70 RVA: 0x00003824 File Offset: 0x00001A24
	private void Update()
	{
		if (this.Time1 != null)
		{
			this.Time2.text = "Time2 = " + RegionController.timeSegmentingCurves.ToString();
		}
		if (this.Time2 != null)
		{
			this.Time3.text = "Time3 = " + RegionController.timePolyConvert.ToString();
		}
		if (this.Time3 != null)
		{
			this.Time1.text = "Time1 = " + RegionController.timeAddInteriorPoints.ToString();
		}
		if (this.Time4 != null)
		{
			this.Time4.text = "Time4 = " + RegionController.timeTriangulate.ToString();
		}
		if (this.Time5 != null)
		{
			this.Time5.text = "Time5 = " + RegionController.timeDisplayOutline.ToString();
		}
		if (this.Time6 != null)
		{
			this.Time6.text = "Time6 = " + RegionController.timeDisplayMesh.ToString();
		}
	}

	// Token: 0x06000047 RID: 71 RVA: 0x00003940 File Offset: 0x00001B40
	public void LineWidthSliderChanged(float newValue)
	{
		float num = this.minLineWidth + (this.maxLineWidth - this.minLineWidth) * newValue;
		this.outliner.SetOutlineWidths(num);
	}

	// Token: 0x06000048 RID: 72 RVA: 0x00003970 File Offset: 0x00001B70
	public void NumSegmentsSliderChanged(float newValue)
	{
		float num = newValue * 0.5f;
		this.outliner.SetLiftValue(num, "");
		this.liftValue.text = num.ToString("N4");
	}

	// Token: 0x04000036 RID: 54
	public Slider lineWidthSlider;

	// Token: 0x04000037 RID: 55
	public float minLineWidth;

	// Token: 0x04000038 RID: 56
	public float maxLineWidth;

	// Token: 0x04000039 RID: 57
	public MapController outliner;

	// Token: 0x0400003A RID: 58
	public Slider liftSlider;

	// Token: 0x0400003B RID: 59
	public Text liftValue;

	// Token: 0x0400003C RID: 60
	public Text Time1;

	// Token: 0x0400003D RID: 61
	public Text Time2;

	// Token: 0x0400003E RID: 62
	public Text Time3;

	// Token: 0x0400003F RID: 63
	public Text Time4;

	// Token: 0x04000040 RID: 64
	public Text Time5;

	// Token: 0x04000041 RID: 65
	public Text Time6;
}
