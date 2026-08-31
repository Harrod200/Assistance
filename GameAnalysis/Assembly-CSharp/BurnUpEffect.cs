using System;
using System.Collections.Generic;
using ModestTree;
using UnityEngine;

// Token: 0x02000012 RID: 18
public class BurnUpEffect : AbstractEffectController
{
	// Token: 0x06000077 RID: 119 RVA: 0x0000544E File Offset: 0x0000364E
	private void Awake()
	{
		this.InitTargets();
	}

	// Token: 0x06000078 RID: 120 RVA: 0x00005458 File Offset: 0x00003658
	private void InitTargets()
	{
		foreach (GameObject gameObject in this.m_targetObjects)
		{
			if (this.m_applyToDescendants)
			{
				this.m_targets.AddRange(gameObject.GetComponentsInChildren<MeshRenderer>());
				this.m_targets.AddRange(gameObject.GetComponentsInChildren<SkinnedMeshRenderer>());
			}
			else
			{
				this.m_targets.Add(gameObject.GetComponent<MeshRenderer>());
				if (this.m_targets.IsEmpty<Renderer>())
				{
					this.m_targets.Add(gameObject.GetComponent<SkinnedMeshRenderer>());
				}
			}
		}
	}

	// Token: 0x06000079 RID: 121 RVA: 0x000054DC File Offset: 0x000036DC
	private void DisableTargetRenderers()
	{
		for (int i = this.m_targets.Count - 1; i >= 0; i--)
		{
			this.m_targets[i].enabled = false;
		}
	}

	// Token: 0x0600007A RID: 122 RVA: 0x00005514 File Offset: 0x00003714
	private void DestroyTargets()
	{
		for (int i = this.m_targets.Count - 1; i >= 0; i--)
		{
			global::UnityEngine.Object.Destroy(this.m_targets[i].gameObject);
			this.m_targets.RemoveAt(i);
		}
		this.m_targets.Clear();
	}

	// Token: 0x0600007B RID: 123 RVA: 0x00005568 File Offset: 0x00003768
	private void ApplyMaterialToTargets()
	{
		foreach (Renderer renderer in this.m_targets)
		{
			if (renderer.enabled)
			{
				Material[] materials = renderer.materials;
				foreach (Material material in materials)
				{
					if (!(material.shader.name != "Standard"))
					{
						material.shader = this.m_burnUpMaterial.shader;
						material.SetTexture(BurnUpEffect.u_mask, this.m_burnUpMaterial.GetTexture(BurnUpEffect.u_mask));
						material.SetTextureScale(BurnUpEffect.u_mask, this.m_burnUpMaterial.GetTextureScale(BurnUpEffect.u_mask));
						material.SetTextureOffset(BurnUpEffect.u_mask, this.m_burnUpMaterial.GetTextureOffset(BurnUpEffect.u_mask));
						material.SetColor(BurnUpEffect.u_burnColor, this.m_burnUpMaterial.GetColor(BurnUpEffect.u_burnColor));
						this.m_burnUpMaterials.Add(material);
					}
				}
				renderer.materials = materials;
			}
		}
	}

	// Token: 0x0600007C RID: 124 RVA: 0x000056A0 File Offset: 0x000038A0
	public override void CleanUp()
	{
		foreach (Renderer renderer in this.m_targets)
		{
			renderer.enabled = false;
		}
		foreach (Material material in this.m_burnUpMaterials)
		{
			global::UnityEngine.Object.Destroy(material);
		}
		this.m_burnUpMaterials.Clear();
	}

	// Token: 0x0600007D RID: 125 RVA: 0x0000573C File Offset: 0x0000393C
	protected override void OnPlay()
	{
		this.ApplyMaterialToTargets();
	}

	// Token: 0x0600007E RID: 126 RVA: 0x00005744 File Offset: 0x00003944
	protected override void OnUpdate(float deltaTime)
	{
		this.m_time += deltaTime;
		foreach (Material material in this.m_burnUpMaterials)
		{
			material.SetFloat(BurnUpEffect.u_progress, this.m_time / this.m_duration);
		}
		if (this.m_time > this.m_duration)
		{
			base.EffectCompleted();
			if (this.m_destroyTargetsOnComplete)
			{
				this.DestroyTargets();
				return;
			}
			this.DisableTargetRenderers();
		}
	}

	// Token: 0x0600007F RID: 127 RVA: 0x000057E0 File Offset: 0x000039E0
	protected override void OnStop()
	{
	}

	// Token: 0x06000080 RID: 128 RVA: 0x000057E2 File Offset: 0x000039E2
	protected override void OnPause()
	{
	}

	// Token: 0x06000081 RID: 129 RVA: 0x000057E4 File Offset: 0x000039E4
	protected override void OnUnPause()
	{
	}

	// Token: 0x06000082 RID: 130 RVA: 0x000057E6 File Offset: 0x000039E6
	public void SetTargetObjects(GameObject[] targets, bool applyToDescendants = false)
	{
		this.m_applyToDescendants = applyToDescendants;
		this.m_targetObjects = targets;
		this.m_targets.Clear();
		this.InitTargets();
	}

	// Token: 0x04000066 RID: 102
	private static int u_mask = Shader.PropertyToID("_Mask");

	// Token: 0x04000067 RID: 103
	private static int u_maskST = Shader.PropertyToID("_Mask_ST");

	// Token: 0x04000068 RID: 104
	private static int u_burnColor = Shader.PropertyToID("_BurnColor");

	// Token: 0x04000069 RID: 105
	private static int u_progress = Shader.PropertyToID("_Progress");

	// Token: 0x0400006A RID: 106
	public Material m_burnUpMaterial;

	// Token: 0x0400006B RID: 107
	public float m_duration = 1f;

	// Token: 0x0400006C RID: 108
	public bool m_destroyTargetsOnComplete;

	// Token: 0x0400006D RID: 109
	[SerializeField]
	private bool m_applyToDescendants;

	// Token: 0x0400006E RID: 110
	[SerializeField]
	private GameObject[] m_targetObjects = new GameObject[0];

	// Token: 0x0400006F RID: 111
	private List<Renderer> m_targets = new List<Renderer>();

	// Token: 0x04000070 RID: 112
	private List<Material> m_burnUpMaterials = new List<Material>();

	// Token: 0x04000071 RID: 113
	private float m_time;
}
