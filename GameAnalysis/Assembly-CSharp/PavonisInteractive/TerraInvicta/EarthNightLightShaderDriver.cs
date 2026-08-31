using System;
using System.Collections;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000568 RID: 1384
	public class EarthNightLightShaderDriver : MonoBehaviour
	{
		// Token: 0x060024C1 RID: 9409 RVA: 0x000C5C8C File Offset: 0x000C3E8C
		private void Awake()
		{
			this.earthMeshRenderer = base.GetComponent<MeshRenderer>();
			this._materialBlock = new MaterialPropertyBlock();
			this.earthMeshRenderer.SetPropertyBlock(this._materialBlock);
		}

		// Token: 0x060024C2 RID: 9410 RVA: 0x000C5CB6 File Offset: 0x000C3EB6
		private void Start()
		{
			this.Initialize();
		}

		// Token: 0x060024C3 RID: 9411 RVA: 0x000C5CC0 File Offset: 0x000C3EC0
		private void Update()
		{
			if (this.showEarthLights != TIPlayerProfileManager.showEarthLights)
			{
				this.showEarthLights = TIPlayerProfileManager.showEarthLights;
				this.UpdateRegionLightValues();
			}
			if (!this.showEarthLights && this.geoScapeActive)
			{
				return;
			}
			if (this.earthMeshRenderer == null)
			{
				return;
			}
			Vector3 normalized = (this.SunTransform.position - this.EarthTransform.position).normalized;
			this._materialBlock.SetVector("_SunNormal", normalized);
			this.earthMeshRenderer.SetPropertyBlock(this._materialBlock);
		}

		// Token: 0x060024C4 RID: 9412 RVA: 0x000C5D56 File Offset: 0x000C3F56
		private void OnDisable()
		{
			this.UpdateShowEarthLights();
		}

		// Token: 0x060024C5 RID: 9413 RVA: 0x000C5D5E File Offset: 0x000C3F5E
		public void OnRegionNuked(RegionNuked e)
		{
			this.HandleNukeRegion(e.region);
		}

		// Token: 0x060024C6 RID: 9414 RVA: 0x000C5D6C File Offset: 0x000C3F6C
		public void OnRegionGDPLightsRecalculation(RegionGDPLightsRecalculation e)
		{
			this.UpdateSpecificRegionLightValues(e.region, true);
		}

		// Token: 0x060024C7 RID: 9415 RVA: 0x000C5D7B File Offset: 0x000C3F7B
		public void OnCombatStarts(CombatStarts e)
		{
			this.UpdateShowEarthLights();
		}

		// Token: 0x060024C8 RID: 9416 RVA: 0x000C5D83 File Offset: 0x000C3F83
		public void OnCombatEnds(CombatEnds e)
		{
			this.UpdateShowEarthLights();
		}

		// Token: 0x060024C9 RID: 9417 RVA: 0x000C5D8B File Offset: 0x000C3F8B
		public void OnMapChanged(MapActivationChangedEvent e)
		{
			this.geoScapeActive = e.active;
			this.UpdateShowEarthLights();
		}

		// Token: 0x060024CA RID: 9418 RVA: 0x000C5D9F File Offset: 0x000C3F9F
		public void RegionChangedOwner(RegionControlChanged e)
		{
			this.UpdateSpecificRegionLightValues(e.region, true);
		}

		// Token: 0x060024CB RID: 9419 RVA: 0x000C5DB0 File Offset: 0x000C3FB0
		public void Initialize()
		{
			if (Shader.GetGlobalFloat("_BlendSpace") != this.BlendSpace)
			{
				Shader.SetGlobalFloat("_BlendSpace", this.BlendSpace);
			}
			this.showEarthLights = TIPlayerProfileManager.showEarthLights;
			this.spaceCombatEnabled = TIGlobalValuesState.isSpaceCombatEnabled;
			this.geoScapeActive = true;
			this.SunTransform = GameStateManager.Sol().gameObjectLink.transform;
			this.EarthTransform = GameStateManager.Earth().gameObjectLink.transform;
			this.mapRegionTemplates = TemplateManager.GetAllTemplates<TIMapRegionTemplate>(true);
			int num = this.mapRegionTemplates.Length;
			this.gdpList = new float[num + 1];
			this.gdpAnimationList = new float[num + 1];
			this.gdpDisplayList = new float[num + 1];
			this.alienControlList = new float[num + 1];
			for (int i = 0; i <= num; i++)
			{
				this.gdpList[i] = 0f;
				this.gdpAnimationList[i] = 1f;
				this.gdpDisplayList[i] = this.gdpList[i] * this.gdpAnimationList[i];
				this.alienControlList[i] = 0f;
			}
			int count = this.mapRegionTemplates.Where<TIMapRegionTemplate>((TIMapRegionTemplate x) => x.oilId != 0).ToList<TIMapRegionTemplate>().Count;
			this.oilList = new float[count + 1];
			this.oilAnimationList = new float[count + 1];
			this.oilDisplayList = new float[count + 1];
			for (int j = 0; j <= count; j++)
			{
				this.oilList[j] = 0f;
				this.oilAnimationList[j] = 1f;
				this.oilDisplayList[j] = this.oilList[j] * this.oilAnimationList[j];
			}
			this.UpdateRegionLightValues();
			GameControl.eventManager.AddListener<RegionNuked>(new EventManager.EventDelegate<RegionNuked>(this.OnRegionNuked), null, null, true, false);
			GameControl.eventManager.AddListener<RegionGDPLightsRecalculation>(new EventManager.EventDelegate<RegionGDPLightsRecalculation>(this.OnRegionGDPLightsRecalculation), null, null, true, false);
			GameControl.eventManager.AddListener<CombatStarts>(new EventManager.EventDelegate<CombatStarts>(this.OnCombatStarts), null, null, true, false);
			GameControl.eventManager.AddListener<CombatEnds>(new EventManager.EventDelegate<CombatEnds>(this.OnCombatEnds), null, null, true, false);
			GameControl.eventManager.AddListener<MapActivationChangedEvent>(new EventManager.EventDelegate<MapActivationChangedEvent>(this.OnMapChanged), null, null, false, false);
			GameControl.eventManager.AddListener<RegionControlChanged>(new EventManager.EventDelegate<RegionControlChanged>(this.RegionChangedOwner), null, null, true, false);
		}

		// Token: 0x060024CC RID: 9420 RVA: 0x000C6000 File Offset: 0x000C4200
		public void UpdateRegionLightValues()
		{
			this.UpdateShowEarthLights();
			if (!this.showEarthLights)
			{
				return;
			}
			if (this.earthMeshRenderer == null)
			{
				return;
			}
			if (this.mapRegionTemplates != null)
			{
				for (int i = 1; i <= this.mapRegionTemplates.Length; i++)
				{
					TIRegionState tiregionState = GameStateManager.MapRegionLookup(this.mapRegionTemplates[i - 1].dataName);
					this.UpdateSpecificRegionLightValues(tiregionState, false);
				}
				this.UpdateShader();
			}
		}

		// Token: 0x060024CD RID: 9421 RVA: 0x000C606C File Offset: 0x000C426C
		private void UpdateSpecificRegionLightValues(TIRegionState currentRegion, bool sendToShader = true)
		{
			if (currentRegion != null)
			{
				float num = Mathf.Log10((float)currentRegion.regionalPerCapitaGDP);
				float num2 = Mathf.Pow(10f, 0.398f + 1.234f * num + -0.244f * (num * num));
				float num3 = (1f - num2 / 150f + 0.1f) * 4f - 1.2f;
				int visualId = currentRegion.mapRegionTemplate.visualId;
				this.gdpList[visualId] = num3;
				this.gdpDisplayList[visualId] = this.gdpList[visualId] * this.gdpAnimationList[visualId];
				this.alienControlList[visualId] = (currentRegion.nation.alienNation ? 1f : 0f);
				int oilId = currentRegion.mapRegionTemplate.oilId;
				if (oilId != 0)
				{
					this.oilList[oilId] = (currentRegion.oilRegion ? 1f : 0f);
					this.oilDisplayList[oilId] = this.oilList[oilId] * this.oilAnimationList[oilId];
				}
				if (sendToShader)
				{
					this.UpdateShader();
				}
			}
		}

		// Token: 0x060024CE RID: 9422 RVA: 0x000C6178 File Offset: 0x000C4378
		private void UpdateShader()
		{
			this._materialBlock.SetFloatArray("_RegionGDP", this.gdpDisplayList);
			this._materialBlock.SetFloatArray("_RegionAlienControl", this.alienControlList);
			this._materialBlock.SetFloatArray("_RegionOil", this.oilDisplayList);
			this.earthMeshRenderer.SetPropertyBlock(this._materialBlock);
		}

		// Token: 0x060024CF RID: 9423 RVA: 0x000C61D8 File Offset: 0x000C43D8
		private void UpdateShowEarthLights()
		{
			this.showEarthLights = TIPlayerProfileManager.showEarthLights;
			this.spaceCombatEnabled = GameControl.spaceCombat != null && TIGlobalValuesState.isSpaceCombatEnabled;
			int num = (this.spaceCombatEnabled ? 0 : (this.showEarthLights ? 1 : (this.geoScapeActive ? 0 : 1)));
			if (Shader.GetGlobalInt("_ShowEarthLights") != num)
			{
				Shader.SetGlobalInt("_ShowEarthLights", num);
			}
		}

		// Token: 0x060024D0 RID: 9424 RVA: 0x000C6246 File Offset: 0x000C4446
		public void HandleNukeRegion(TIRegionState currentRegion)
		{
			base.StartCoroutine(this.NukeRegion(currentRegion));
		}

		// Token: 0x060024D1 RID: 9425 RVA: 0x000C6256 File Offset: 0x000C4456
		private IEnumerator NukeRegion(TIRegionState currentRegion)
		{
			int visualID = currentRegion.mapRegionTemplate.visualId;
			int oilID = currentRegion.mapRegionTemplate.oilId;
			float nukeAnimationLength = this.nukeAnimation.keys[this.nukeAnimation.keys.Length - 1].time;
			float count = 0f;
			while (count < nukeAnimationLength)
			{
				this.<NukeRegion>g__UpdateRegionAnimations|33_0(currentRegion, visualID, oilID, count);
				count += Time.deltaTime;
				yield return null;
			}
			this.<NukeRegion>g__UpdateRegionAnimations|33_0(currentRegion, visualID, oilID, nukeAnimationLength);
			yield break;
		}

		// Token: 0x060024D3 RID: 9427 RVA: 0x000C628D File Offset: 0x000C448D
		[CompilerGenerated]
		private void <NukeRegion>g__UpdateRegionAnimations|33_0(TIRegionState currentRegion, int visualID, int oilID, float count)
		{
			this.gdpAnimationList[visualID] = this.nukeAnimation.Evaluate(count);
			this.oilAnimationList[oilID] = this.nukeAnimation.Evaluate(count);
			this.UpdateSpecificRegionLightValues(currentRegion, true);
		}

		// Token: 0x04001BA3 RID: 7075
		[SerializeField]
		[Range(0.01f, 0.99f)]
		private float BlendSpace = 0.25f;

		// Token: 0x04001BA4 RID: 7076
		[SerializeField]
		private AnimationCurve nukeAnimation;

		// Token: 0x04001BA5 RID: 7077
		private MeshRenderer earthMeshRenderer;

		// Token: 0x04001BA6 RID: 7078
		private Transform SunTransform;

		// Token: 0x04001BA7 RID: 7079
		private Transform EarthTransform;

		// Token: 0x04001BA8 RID: 7080
		private TIMapRegionTemplate[] mapRegionTemplates;

		// Token: 0x04001BA9 RID: 7081
		private float[] gdpList;

		// Token: 0x04001BAA RID: 7082
		private float[] gdpAnimationList;

		// Token: 0x04001BAB RID: 7083
		private float[] gdpDisplayList;

		// Token: 0x04001BAC RID: 7084
		private float[] alienControlList;

		// Token: 0x04001BAD RID: 7085
		private float[] oilList;

		// Token: 0x04001BAE RID: 7086
		private float[] oilAnimationList;

		// Token: 0x04001BAF RID: 7087
		private float[] oilDisplayList;

		// Token: 0x04001BB0 RID: 7088
		private MaterialPropertyBlock _materialBlock;

		// Token: 0x04001BB1 RID: 7089
		private bool showEarthLights = true;

		// Token: 0x04001BB2 RID: 7090
		private bool spaceCombatEnabled;

		// Token: 0x04001BB3 RID: 7091
		private bool geoScapeActive = true;
	}
}
