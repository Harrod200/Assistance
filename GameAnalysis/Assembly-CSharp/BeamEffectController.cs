using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;
using UnityEngine;

// Token: 0x0200001F RID: 31
public class BeamEffectController : MonoBehaviour
{
	// Token: 0x17000007 RID: 7
	// (get) Token: 0x060000CA RID: 202 RVA: 0x00006CEA File Offset: 0x00004EEA
	public Vector3 StartPoint
	{
		get
		{
			return this.lineRenderer.GetPosition(0);
		}
	}

	// Token: 0x17000008 RID: 8
	// (get) Token: 0x060000CB RID: 203 RVA: 0x00006CF8 File Offset: 0x00004EF8
	public Vector3 EndPoint
	{
		get
		{
			return this.lineRenderer.GetPosition(1);
		}
	}

	// Token: 0x060000CC RID: 204 RVA: 0x00006D08 File Offset: 0x00004F08
	private void Awake()
	{
		this.lineRenderer = base.GetComponentInChildren<LineRenderer>();
		this.lineRenderer.enabled = base.isActiveAndEnabled;
		this.lineRenderer.useWorldSpace = true;
		this.lineRenderer.widthMultiplier = this.beamWidth;
		if (this.lineRenderer.material != null)
		{
			this.targetColorUniforms.Add(new ValueTuple<Material, Color>(this.lineRenderer.material, this.lineRenderer.material.color));
			this.initialTextureScale = this.lineRenderer.material.GetTextureScale(BeamEffectController.mainTexUniform);
		}
		if (this._hitParticleSystem)
		{
			this.hitParticleSystems = this._hitParticleSystem.GetComponentsInChildren<ParticleSystem>();
			foreach (ParticleSystem particleSystem in this.hitParticleSystems)
			{
				this.targetParticleColors.Add(new ValueTuple<ParticleSystem, Color>(particleSystem, particleSystem.main.startColor.color));
				ParticleSystemRenderer component = particleSystem.GetComponent<ParticleSystemRenderer>();
				if (component.material != null)
				{
					this.targetColorUniforms.Add(new ValueTuple<Material, Color>(component.material, component.material.color));
					if (component.material.HasProperty(BeamEffectController.emissionUniform))
					{
						this.targetEmissionUniforms.Add(new ValueTuple<Material, Color>(component.material, component.material.GetColor(BeamEffectController.emissionUniform)));
					}
				}
				if (component.trailMaterial != null)
				{
					this.trailMaterial = new Material(component.trailMaterial);
					component.trailMaterial = this.trailMaterial;
					this.targetColorUniforms.Add(new ValueTuple<Material, Color>(this.trailMaterial, component.trailMaterial.color));
					if (component.trailMaterial.HasProperty(BeamEffectController.emissionUniform))
					{
						this.targetEmissionUniforms.Add(new ValueTuple<Material, Color>(this.trailMaterial, component.trailMaterial.GetColor(BeamEffectController.emissionUniform)));
					}
				}
			}
		}
		if (this.startLight != null)
		{
			this.startLightInitialRange = this.startLight.range;
		}
		if (this.endLight != null)
		{
			this.endLightInitialRange = this.endLight.range;
		}
		this.ApplyColorToMaterials();
	}

	// Token: 0x060000CD RID: 205 RVA: 0x00006F48 File Offset: 0x00005148
	private void OnEnable()
	{
		this.lineRenderer.enabled = true;
		if (this._hitParticleSystem != null)
		{
			this._hitParticleSystem.Play();
		}
		if (this.fireParticleSystem != null)
		{
			this.fireParticleSystem.Play();
		}
		if (BeamEffectController.isLaserPointLightEnabled)
		{
			float num = Mathf.Max(new float[]
			{
				base.transform.lossyScale.x,
				base.transform.lossyScale.y,
				base.transform.lossyScale.z
			});
			if (this.startLight != null)
			{
				this.startLight.enabled = true;
				this.startLight.range = this.startLightInitialRange * num;
			}
			if (this.endLight != null)
			{
				this.endLight.enabled = true;
				this.endLight.range *= this.endLightInitialRange * num;
			}
		}
		if (this.lineRenderer.material != null)
		{
			float num2 = Mathf.Max(new float[]
			{
				base.transform.lossyScale.x,
				base.transform.lossyScale.y,
				base.transform.lossyScale.z
			});
			this.lineRenderer.material.SetTextureScale(BeamEffectController.mainTexUniform, this.initialTextureScale / num2);
		}
	}

	// Token: 0x060000CE RID: 206 RVA: 0x000070BC File Offset: 0x000052BC
	private void OnDisable()
	{
		this.lineRenderer.enabled = false;
		if (this._hitParticleSystem != null)
		{
			this._hitParticleSystem.Stop();
		}
		if (this.fireParticleSystem != null)
		{
			this.fireParticleSystem.Stop();
		}
		if (BeamEffectController.isLaserPointLightEnabled)
		{
			if (this.startLight != null)
			{
				this.startLight.enabled = false;
			}
			if (this.endLight != null)
			{
				this.endLight.enabled = false;
			}
		}
	}

	// Token: 0x060000CF RID: 207 RVA: 0x00007142 File Offset: 0x00005342
	private void OnDestroy()
	{
		if (this.trailMaterial != null)
		{
			global::UnityEngine.Object.Destroy(this.trailMaterial);
		}
	}

	// Token: 0x060000D0 RID: 208 RVA: 0x00007160 File Offset: 0x00005360
	public void SetBeamPoints(Vector3 start, Vector3 end)
	{
		Vector3[] array = new Vector3[] { start, end };
		this.lineRenderer.SetPositions(array);
		Vector3.Distance(start, end);
		ParticleSystem[] array2 = this.hitParticleSystems;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].transform.position = end;
		}
		if (this.fireParticleSystem != null)
		{
			this.fireParticleSystem.transform.position = end;
		}
		if (this.startLight != null)
		{
			this.startLight.transform.position = start;
		}
		if (this.endLight != null)
		{
			this.endLight.transform.position = end;
		}
	}

	// Token: 0x060000D1 RID: 209 RVA: 0x00007218 File Offset: 0x00005418
	private void OnWillRenderObject()
	{
		Vector3 position = Camera.current.transform.position;
		float num = Mathf.Max(new float[]
		{
			base.transform.lossyScale.x,
			base.transform.lossyScale.y,
			base.transform.lossyScale.z
		});
		if (TIUtilities.IsInCombatMode)
		{
			float num2 = Vector3.Distance(position, this.lineRenderer.GetPosition(0)) / num * BeamEffectController.beamDistanceScaling;
			this.lineRenderer.startWidth = num * Mathf.Max(this.beamWidth, num2);
			num2 = Vector3.Distance(position, this.lineRenderer.GetPosition(1)) / num * BeamEffectController.beamDistanceScaling;
			this.lineRenderer.endWidth = num * Mathf.Max(this.beamWidth, num2);
			return;
		}
		float num3 = this.beamWidth * this.lineRenderer.GetPosition(0).magnitude * BeamEffectController.beamDistanceScaling;
		float num4 = this.beamWidth * this.lineRenderer.GetPosition(1).magnitude * BeamEffectController.beamDistanceScaling;
		this.lineRenderer.startWidth = num3;
		this.lineRenderer.endWidth = num4;
		float num5 = num4 / num / 3f;
		if (this.hitParticleMaxScale >= 0f)
		{
			num5 = Mathf.Min(num5, this.hitParticleMaxScale);
		}
		this._hitParticleSystem.transform.localScale = Vector3.one * num5;
	}

	// Token: 0x060000D2 RID: 210 RVA: 0x00007390 File Offset: 0x00005590
	private void ApplyColorToMaterials()
	{
		foreach (ValueTuple<Material, Color> valueTuple in this.targetColorUniforms)
		{
			Material item = valueTuple.Item1;
			Color item2 = valueTuple.Item2;
			item.color = this.GetUpdatedColor(item2, this.beamColor);
		}
		foreach (ValueTuple<Material, Color> valueTuple2 in this.targetEmissionUniforms)
		{
			Material item3 = valueTuple2.Item1;
			Color item4 = valueTuple2.Item2;
			item3.SetColor(BeamEffectController.emissionUniform, this.GetUpdatedColor(item4, this.beamColor));
		}
		foreach (ValueTuple<ParticleSystem, Color> valueTuple3 in this.targetParticleColors)
		{
			ParticleSystem item5 = valueTuple3.Item1;
			ParticleSystem.MainModule main = item5.main;
			main.startColor = new Color(this.beamColor.r, this.beamColor.g, this.beamColor.b, main.startColor.color.a);
		}
		if (this.startLight != null)
		{
			this.startLight.color = this.beamColor;
		}
		if (this.endLight != null)
		{
			this.endLight.color = this.beamColor;
		}
	}

	// Token: 0x060000D3 RID: 211 RVA: 0x00007530 File Offset: 0x00005730
	private Color GetUpdatedColor(Color orig, Color dest)
	{
		float num = (orig.r + orig.g + orig.b) / 3f;
		float num2 = (dest.r + dest.g + dest.b) / 3f;
		float num3 = num * this.intensity;
		Color color = new Color(dest.r / num2, dest.g / num2, dest.b / num2);
		return new Color(color.r * num3, color.g * num3, color.b * num3, dest.a * orig.a);
	}

	// Token: 0x040000AC RID: 172
	private static float beamDistanceScaling = 0.005f;

	// Token: 0x040000AD RID: 173
	public static bool isLaserPointLightEnabled = false;

	// Token: 0x040000AE RID: 174
	private static int emissionUniform = Shader.PropertyToID("_EmissionColor");

	// Token: 0x040000AF RID: 175
	private static int mainTexUniform = Shader.PropertyToID("_MainTex");

	// Token: 0x040000B0 RID: 176
	[SerializeField]
	private Color beamColor = Color.red;

	// Token: 0x040000B1 RID: 177
	[SerializeField]
	private float intensity = 1f;

	// Token: 0x040000B2 RID: 178
	[SerializeField]
	private float beamWidth = 1.5f;

	// Token: 0x040000B3 RID: 179
	[SerializeField]
	private ParticleSystem _hitParticleSystem;

	// Token: 0x040000B4 RID: 180
	private ParticleSystem[] hitParticleSystems = new ParticleSystem[0];

	// Token: 0x040000B5 RID: 181
	[SerializeField]
	private ParticleSystem fireParticleSystem;

	// Token: 0x040000B6 RID: 182
	[SerializeField]
	private Light startLight;

	// Token: 0x040000B7 RID: 183
	[SerializeField]
	private Light endLight;

	// Token: 0x040000B8 RID: 184
	private float startLightInitialRange = 1f;

	// Token: 0x040000B9 RID: 185
	private float endLightInitialRange = 1f;

	// Token: 0x040000BA RID: 186
	private Vector2 initialTextureScale = Vector2.one;

	// Token: 0x040000BB RID: 187
	private LineRenderer lineRenderer;

	// Token: 0x040000BC RID: 188
	private List<ValueTuple<Material, Color>> targetColorUniforms = new List<ValueTuple<Material, Color>>();

	// Token: 0x040000BD RID: 189
	private List<ValueTuple<Material, Color>> targetEmissionUniforms = new List<ValueTuple<Material, Color>>();

	// Token: 0x040000BE RID: 190
	private List<ValueTuple<ParticleSystem, Color>> targetParticleColors = new List<ValueTuple<ParticleSystem, Color>>();

	// Token: 0x040000BF RID: 191
	private Material trailMaterial;

	// Token: 0x040000C0 RID: 192
	public float hitParticleMaxScale = -1f;
}
