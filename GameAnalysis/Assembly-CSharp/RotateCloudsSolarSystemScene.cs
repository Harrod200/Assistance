using System;
using System.Collections;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Systems.GameTime;
using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x0200000E RID: 14
public class RotateCloudsSolarSystemScene : MonoBehaviour
{
	// Token: 0x06000054 RID: 84 RVA: 0x00004D60 File Offset: 0x00002F60
	private void Awake()
	{
		if (SceneManager.GetActiveScene().name == "StartScreenScene")
		{
			global::UnityEngine.Object.Destroy(this);
			return;
		}
		this.gameTime = World.Active.GetExistingManager<GameTimeManager>();
		this.baseAlbedoColor = base.gameObject.GetComponent<MeshRenderer>().material.GetColor("_Color");
		base.gameObject.GetComponent<MeshRenderer>().material.SetColor("_Color", new Color(this.baseAlbedoColor.r, this.baseAlbedoColor.g, this.baseAlbedoColor.b, 0.4f));
		if (TIGlobalValuesState.isSpaceCombatEnabled)
		{
			this.speedY = -0.01f;
			return;
		}
		this.speedY = -0.12f;
	}

	// Token: 0x06000055 RID: 85 RVA: 0x00004E20 File Offset: 0x00003020
	public void InitAlbedoControl()
	{
		GameControl.eventManager.AddListener<MapActivationChangedEvent>(new EventManager.EventDelegate<MapActivationChangedEvent>(this.OnMapActivationChanged), null, null, true, false);
		GameControl.eventManager.AddListener<EarthParticulateThresholdChanges>(new EventManager.EventDelegate<EarthParticulateThresholdChanges>(this.OnCloudThresholdChange), null, null, true, false);
		this.SetMaterial((GameStateManager.GlobalValues().stratosphericAerosols_ppm >= 0.01f) ? 1 : 0);
	}

	// Token: 0x06000056 RID: 86 RVA: 0x00004E7C File Offset: 0x0000307C
	private void OnMapActivationChanged(MapActivationChangedEvent e)
	{
		if (!base.gameObject.activeInHierarchy)
		{
			base.gameObject.GetComponent<MeshRenderer>().material.SetColor("_Color", new Color(this.baseAlbedoColor.r, this.baseAlbedoColor.g, this.baseAlbedoColor.b, 0.4f));
			return;
		}
		if (e.active)
		{
			base.StartCoroutine(this.LightenClouds());
			return;
		}
		base.StartCoroutine(this.DarkenClouds());
	}

	// Token: 0x06000057 RID: 87 RVA: 0x00004F00 File Offset: 0x00003100
	public void OnCloudThresholdChange(EarthParticulateThresholdChanges e)
	{
		this.SetMaterial(e.particulates);
		if (GameControl.control.viewMgr.currentView == ViewType.PoliticalMap)
		{
			base.StartCoroutine(this.LightenClouds());
			return;
		}
		if (base.gameObject.activeInHierarchy)
		{
			base.StartCoroutine(this.DarkenClouds());
		}
	}

	// Token: 0x06000058 RID: 88 RVA: 0x00004F54 File Offset: 0x00003154
	private void SetMaterial(int idx)
	{
		Material material;
		if (idx == 0)
		{
			material = GameControl.assetLoader.LoadAsset<Material>("planets/MAT_Planet_Earth_Clouds");
			base.gameObject.GetComponent<MeshRenderer>().material = material;
			return;
		}
		if (idx != 1)
		{
			return;
		}
		material = GameControl.assetLoader.LoadAsset<Material>("planets/MAT_Planet_Earth_NuclearClouds");
		base.gameObject.GetComponent<MeshRenderer>().material = material;
	}

	// Token: 0x06000059 RID: 89 RVA: 0x00004FAC File Offset: 0x000031AC
	private IEnumerator DarkenClouds()
	{
		for (float i = 0f; i <= 1f; i += Time.deltaTime)
		{
			float num = 0.04f + this.transparencyDelta * i;
			base.gameObject.GetComponent<MeshRenderer>().material.SetColor("_Color", new Color(this.baseAlbedoColor.r, this.baseAlbedoColor.g, this.baseAlbedoColor.b, num));
			yield return null;
		}
		yield break;
	}

	// Token: 0x0600005A RID: 90 RVA: 0x00004FBB File Offset: 0x000031BB
	private IEnumerator LightenClouds()
	{
		for (float i = 0f; i <= 1f; i += Time.deltaTime)
		{
			float num = 0.4f - this.transparencyDelta * i;
			base.gameObject.GetComponent<MeshRenderer>().material.SetColor("_Color", new Color(this.baseAlbedoColor.r, this.baseAlbedoColor.g, this.baseAlbedoColor.b, num));
			yield return null;
		}
		yield break;
	}

	// Token: 0x0600005B RID: 91 RVA: 0x00004FCC File Offset: 0x000031CC
	private void LateUpdate()
	{
		if (!this.gameTime.Paused)
		{
			base.transform.Rotate(new Vector3(0f, this.speedY * Time.deltaTime * (float)this.gameTime.currentSpeedIndex, 0f));
		}
	}

	// Token: 0x0600005C RID: 92 RVA: 0x00005019 File Offset: 0x00003219
	private void OnDestroy()
	{
		if (base.gameObject.layer == LayerMask.NameToLayer("Solar System"))
		{
			GameControl.eventManager.RemoveListener<MapActivationChangedEvent>(new EventManager.EventDelegate<MapActivationChangedEvent>(this.OnMapActivationChanged), null);
		}
	}

	// Token: 0x0400004E RID: 78
	private float speedY;

	// Token: 0x0400004F RID: 79
	private const float activatedTransparency = 0.04f;

	// Token: 0x04000050 RID: 80
	private const float distantTransparency = 0.4f;

	// Token: 0x04000051 RID: 81
	private readonly float transparencyDelta = 0.36f;

	// Token: 0x04000052 RID: 82
	private GameTimeManager gameTime;

	// Token: 0x04000053 RID: 83
	private Color baseAlbedoColor;
}
