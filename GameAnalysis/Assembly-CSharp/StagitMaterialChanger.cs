using System;
using UnityEngine;

// Token: 0x0200000D RID: 13
public class StagitMaterialChanger : MonoBehaviour
{
	// Token: 0x0600004F RID: 79 RVA: 0x00004A8F File Offset: 0x00002C8F
	private void Start()
	{
	}

	// Token: 0x06000050 RID: 80 RVA: 0x00004A91 File Offset: 0x00002C91
	private void OnGUI()
	{
	}

	// Token: 0x06000051 RID: 81 RVA: 0x00004A93 File Offset: 0x00002C93
	private void OnDrawGizmos()
	{
		if (this.SetMaterial)
		{
			this.SetMaterial = false;
			Debug.Log("Setting Material");
			this.changeMaterials();
		}
	}

	// Token: 0x06000052 RID: 82 RVA: 0x00004AB4 File Offset: 0x00002CB4
	private void changeMaterials()
	{
		Material[] sharedMaterials = base.GetComponent<Renderer>().sharedMaterials;
		for (int i = 0; i < sharedMaterials.Length; i++)
		{
			sharedMaterials[i].name = sharedMaterials[i].name.Replace(" (Instance)", "");
			string text = sharedMaterials[i].name.Replace("earth", "");
			int num = int.Parse(text.Substring(0, 1));
			int num2 = int.Parse(text.Substring(2, 1));
			int num3 = num2 - 5;
			int num4 = num - 4;
			if (num2 == 1)
			{
				num3 = -2;
			}
			if (num2 == 3)
			{
				num3 = -4;
			}
			if (num2 == 4)
			{
				num3 = -5;
			}
			if (num2 == 5)
			{
				num3 = -6;
			}
			if (num2 == 6)
			{
				num3 = -7;
			}
			if (num2 == 7)
			{
				num3 = 0;
			}
			if (num2 == 8)
			{
				num3 = -1;
			}
			if (num == 1)
			{
				num4 = -3;
			}
			if (num == 2)
			{
				num4 = -2;
			}
			if (num == 3)
			{
				num4 = -1;
			}
			if (num == 4)
			{
				num4 = 0;
			}
			sharedMaterials[i].SetFloat("_LightScale", this.light_scale);
			sharedMaterials[i].SetFloat("_Brightness", this.main_brightness);
			sharedMaterials[i].SetFloat("_Shininess", this.reflection_shine);
			sharedMaterials[i].SetFloat("_NormalStrength", this.normal_strength);
			sharedMaterials[i].SetColor("_ReflectionColor", this.reflection_color);
			sharedMaterials[i].SetTextureScale("_SpecGlossMap", new Vector2(8f, 4f));
			sharedMaterials[i].SetTextureOffset("_SpecGlossMap", new Vector2((float)num3, (float)num4));
			sharedMaterials[i].SetTextureScale("_Normals", new Vector2(8f, 4f));
			sharedMaterials[i].SetTextureOffset("_Normals", new Vector2((float)num3, (float)num4));
			sharedMaterials[i].SetTextureScale("_Lights", new Vector2(8f, 4f));
			sharedMaterials[i].SetTextureOffset("_Lights", new Vector2((float)num3, (float)num4));
			sharedMaterials[i].SetColor("_AtmosNear", this.AtmosNearColor);
			sharedMaterials[i].SetColor("_AtmosFar", this.AtmosFarColor);
			sharedMaterials[i].SetFloat("_AtmosFalloff", this.AtmosFallOff);
		}
	}

	// Token: 0x04000045 RID: 69
	public float normal_strength = 0.5f;

	// Token: 0x04000046 RID: 70
	public float main_brightness = 0.5f;

	// Token: 0x04000047 RID: 71
	public float light_scale = 0.5f;

	// Token: 0x04000048 RID: 72
	public float reflection_shine = 0.22f;

	// Token: 0x04000049 RID: 73
	public Color32 reflection_color = Color.white;

	// Token: 0x0400004A RID: 74
	public Color32 AtmosNearColor = new Color(0.168f, 0.737f, 1f);

	// Token: 0x0400004B RID: 75
	public Color32 AtmosFarColor = new Color(0.455f, 0.518f, 0.985f);

	// Token: 0x0400004C RID: 76
	public float AtmosFallOff = 3f;

	// Token: 0x0400004D RID: 77
	public bool SetMaterial;
}
