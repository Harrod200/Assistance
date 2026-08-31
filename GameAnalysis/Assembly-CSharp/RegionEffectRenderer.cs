using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Audio;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000009 RID: 9
public class RegionEffectRenderer : MonoBehaviour
{
	// Token: 0x06000034 RID: 52 RVA: 0x00003124 File Offset: 0x00001324
	private void Awake()
	{
		RegionEffectRenderer.s_Instance = this;
		this.m_pCamera = base.GetComponent<Camera>();
		this.m_regionCommandBuffer = new CommandBuffer
		{
			name = "RegionEffectRenderer - Border Mask Pass"
		};
	}

	// Token: 0x06000035 RID: 53 RVA: 0x00003150 File Offset: 0x00001350
	public void Initialize()
	{
		if (!this.initialized)
		{
			if (this.m_BorderMaterial == null)
			{
				Shader shader = Shader.Find("Hidden/TerraInvicta/RegionBorderShader");
				this.m_BorderMaterial = new Material(shader);
			}
			GameObject gameObject = ((GameControl.control.viewMgr != null) ? GameControl.control.viewMgr.earthObject : null);
			StagitMaterialChanger stagitMaterialChanger = ((gameObject != null) ? gameObject.GetComponentInChildren<StagitMaterialChanger>(true) : null);
			this.earthObject = ((stagitMaterialChanger != null) ? stagitMaterialChanger.gameObject : null);
			if (this.earthObject != null)
			{
				GameObject gameObject2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				gameObject2.name = "BorderEffectRendererClearSphere";
				gameObject2.hideFlags = HideFlags.HideAndDontSave;
				this.m_zMesh = gameObject2.GetComponent<MeshFilter>();
				if (this.m_zMesh != null)
				{
					this.m_zMesh.transform.localScale = new Vector3(80f, 80f, 80f);
					this.m_zMesh.transform.SetParent(this.earthObject.transform, false);
					global::UnityEngine.Object.Destroy(gameObject2.GetComponent<MeshRenderer>());
					global::UnityEngine.Object.Destroy(gameObject2.GetComponent<SphereCollider>());
					this.initialized = true;
					this.ignoreFirst = false;
					if (!this.listeningForEarthSwap)
					{
						GameControl.eventManager.AddListener<ForceUpdateSpaceBodyModelFinished>(new EventManager.EventDelegate<ForceUpdateSpaceBodyModelFinished>(this.OnForceUpdateSpaceBodyModelFinished), null, null, true, false);
						this.listeningForEarthSwap = true;
					}
				}
			}
		}
	}

	// Token: 0x06000036 RID: 54 RVA: 0x000032B3 File Offset: 0x000014B3
	public void OnForceUpdateSpaceBodyModelFinished(ForceUpdateSpaceBodyModelFinished e)
	{
		if (!e.spaceBody.isEarth)
		{
			return;
		}
		this.initialized = false;
		this.Initialize();
	}

	// Token: 0x06000037 RID: 55 RVA: 0x000032D0 File Offset: 0x000014D0
	private void OnDestroy()
	{
		this.ReleaseRenderTextures();
		if (this.listeningForEarthSwap)
		{
			GameControl.eventManager.RemoveListener<ForceUpdateSpaceBodyModelFinished>(new EventManager.EventDelegate<ForceUpdateSpaceBodyModelFinished>(this.OnForceUpdateSpaceBodyModelFinished), null);
		}
	}

	// Token: 0x06000038 RID: 56 RVA: 0x000032F7 File Offset: 0x000014F7
	private void OnEnable()
	{
		this.InitRenderTextures();
		this.m_pCamera.AddCommandBuffer(CameraEvent.BeforeForwardOpaque, this.m_regionCommandBuffer);
		if (this.ignoreFirst)
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/NEW_UI_SFX/trig_SFX_ActivateEarthMap", false, false);
		}
		else
		{
			this.ignoreFirst = true;
		}
		this.Initialize();
	}

	// Token: 0x06000039 RID: 57 RVA: 0x00003335 File Offset: 0x00001535
	private void OnDisable()
	{
		this.ReleaseRenderTextures();
		this.m_pCamera.RemoveCommandBuffer(CameraEvent.BeforeForwardOpaque, this.m_regionCommandBuffer);
		if (this.ignoreFirst)
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/NEW_UI_SFX/trig_SFX_DeActivateEarthMap", false, false);
		}
	}

	// Token: 0x0600003A RID: 58 RVA: 0x00003364 File Offset: 0x00001564
	private void OnPreRender()
	{
		if (this.earthObject)
		{
			this.m_regionCommandBuffer.Clear();
			this.m_regionCommandBuffer.SetRenderTarget(this.m_borderLineBufferRT);
			this.m_regionCommandBuffer.ClearRenderTarget(true, true, Color.black);
			this.m_regionCommandBuffer.SetViewProjectionMatrices(this.m_pCamera.worldToCameraMatrix, this.m_pCamera.projectionMatrix);
			this.m_regionCommandBuffer.DrawMesh(this.m_zMesh.mesh, this.m_zMesh.transform.localToWorldMatrix, this.m_BorderMaterial, 0, 1);
			foreach (RegionMeshBorderEffect regionMeshBorderEffect in this.m_regionList)
			{
				this.m_regionCommandBuffer.DrawMesh(regionMeshBorderEffect.BorderMesh, regionMeshBorderEffect.transform.localToWorldMatrix, this.m_BorderMaterial, 0, 0);
			}
			this.m_regionCommandBuffer.Blit(this.m_borderLineBufferRT, this.m_borderMaskRT);
			BlurUtility.BlurRenderTexture(ref this.m_regionCommandBuffer, this.m_borderMaskRT, 1);
			this.m_regionCommandBuffer.SetGlobalTexture(RegionEffectRenderer.s_borderTextureUniform, this.m_borderMaskRT);
		}
	}

	// Token: 0x0600003B RID: 59 RVA: 0x000034B0 File Offset: 0x000016B0
	private void InitRenderTextures()
	{
		this.m_borderMaskRT = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.R8)
		{
			name = "Region Blurred Border Mask (R8)",
			filterMode = FilterMode.Bilinear
		};
		this.m_borderLineBufferRT = new RenderTexture(Screen.width, Screen.height, 16, RenderTextureFormat.R8)
		{
			name = "Region Border Line Buffer (R8)",
			filterMode = FilterMode.Bilinear
		};
	}

	// Token: 0x0600003C RID: 60 RVA: 0x00003514 File Offset: 0x00001714
	private void ReleaseRenderTextures()
	{
		if (this.m_borderMaskRT != null)
		{
			this.m_borderMaskRT.Release();
			global::UnityEngine.Object.Destroy(this.m_borderMaskRT);
			this.m_borderMaskRT = null;
		}
		if (this.m_borderLineBufferRT != null)
		{
			this.m_borderLineBufferRT.Release();
			global::UnityEngine.Object.Destroy(this.m_borderLineBufferRT);
			this.m_borderLineBufferRT = null;
		}
	}

	// Token: 0x0600003D RID: 61 RVA: 0x00003577 File Offset: 0x00001777
	public void AddRegion(RegionMeshBorderEffect region)
	{
		this.m_regionList.Add(region);
	}

	// Token: 0x04000027 RID: 39
	private static int s_borderTextureUniform = Shader.PropertyToID("_RegionBorderMask");

	// Token: 0x04000028 RID: 40
	public static RegionEffectRenderer s_Instance;

	// Token: 0x04000029 RID: 41
	private Camera m_pCamera;

	// Token: 0x0400002A RID: 42
	private CommandBuffer m_regionCommandBuffer;

	// Token: 0x0400002B RID: 43
	private RenderTexture m_borderMaskRT;

	// Token: 0x0400002C RID: 44
	private RenderTexture m_borderLineBufferRT;

	// Token: 0x0400002D RID: 45
	private Material m_BorderMaterial;

	// Token: 0x0400002E RID: 46
	private MeshFilter m_zMesh;

	// Token: 0x0400002F RID: 47
	private List<RegionMeshBorderEffect> m_regionList = new List<RegionMeshBorderEffect>();

	// Token: 0x04000030 RID: 48
	private const int MASK_DOWNSCALE_PASSES = 1;

	// Token: 0x04000031 RID: 49
	private GameObject earthObject;

	// Token: 0x04000032 RID: 50
	private bool initialized;

	// Token: 0x04000033 RID: 51
	private bool ignoreFirst;

	// Token: 0x04000034 RID: 52
	private bool listeningForEarthSwap;
}
