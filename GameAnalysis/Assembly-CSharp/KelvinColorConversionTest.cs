using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000025 RID: 37
public class KelvinColorConversionTest : MonoBehaviour
{
	// Token: 0x060000EE RID: 238 RVA: 0x000081A8 File Offset: 0x000063A8
	private void Update()
	{
		float value = this._slider.value;
		Color color = this.ConvertKelvinToRGB((double)value);
		this._UI.color = color;
	}

	// Token: 0x060000EF RID: 239 RVA: 0x000081D8 File Offset: 0x000063D8
	private Color ConvertKelvinToRGB(double kelvin)
	{
		double num = kelvin / 100.0;
		Color black = Color.black;
		if (num <= 66.0)
		{
			black.r = 1f;
		}
		else
		{
			double num2 = num - 60.0;
			num2 = 329.698727446 * Math.Pow(num2, -0.1332047592);
			if (num2 < 0.0)
			{
				num2 = 0.0;
			}
			if (num2 > 255.0)
			{
				num2 = 255.0;
			}
			black.r = (float)num2 / 255f;
		}
		double num3;
		if (num <= 66.0)
		{
			num3 = num;
			num3 = 99.4708025861 * Math.Log(num3) - 161.1195681661;
			if (num3 < 0.0)
			{
				num3 = 0.0;
			}
			if (num3 > 255.0)
			{
				num3 = 255.0;
			}
		}
		else
		{
			num3 = num - 60.0;
			num3 = 288.1221695283 * Math.Pow(num3, -0.0755148492);
			if (num3 < 0.0)
			{
				num3 = 0.0;
			}
			if (num3 > 255.0)
			{
				num3 = 255.0;
			}
		}
		black.g = (float)num3 / 255f;
		double num4;
		if (num >= 66.0)
		{
			num4 = 255.0;
		}
		else if (num <= 19.0)
		{
			num4 = 0.0;
		}
		else
		{
			num4 = num - 10.0;
			num4 = 138.5177312231 * Math.Log(num4) - 305.0447927307;
			if (num4 < 0.0)
			{
				num4 = 0.0;
			}
			if (num4 > 255.0)
			{
				num4 = 255.0;
			}
		}
		black.b = (float)num4 / 255f;
		return black;
	}

	// Token: 0x040000E7 RID: 231
	public Image _UI;

	// Token: 0x040000E8 RID: 232
	public Slider _slider;
}
