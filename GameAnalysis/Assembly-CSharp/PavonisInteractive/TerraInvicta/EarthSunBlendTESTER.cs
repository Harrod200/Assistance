using System;
using System.Collections;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000569 RID: 1385
	public class EarthSunBlendTESTER : MonoBehaviour
	{
		// Token: 0x060024D4 RID: 9428 RVA: 0x000C62C1 File Offset: 0x000C44C1
		private void Awake()
		{
			Shader.SetGlobalInt("_ShowEarthLights", 1);
			this.earthMeshRenderer = base.GetComponent<MeshRenderer>();
			this._materialBlock = new MaterialPropertyBlock();
		}

		// Token: 0x060024D5 RID: 9429 RVA: 0x000C62E8 File Offset: 0x000C44E8
		private void Start()
		{
			int num = this.numMapRegions;
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
			this.oilList = new float[this.numOilRegions + 1];
			this.oilAnimationList = new float[this.numOilRegions + 1];
			this.oilDisplayList = new float[this.numOilRegions + 1];
			for (int j = 0; j <= this.numOilRegions; j++)
			{
				this.oilList[j] = 0f;
				this.oilAnimationList[j] = 1f;
				this.oilDisplayList[j] = this.oilList[j] * this.oilAnimationList[j];
			}
			this.UpdateRegionGDPValues();
			this.UpdateRegionAlienControlValues(false);
			this.UpdateRegionOilValues();
		}

		// Token: 0x060024D6 RID: 9430 RVA: 0x000C6410 File Offset: 0x000C4610
		private void Update()
		{
			Vector3 normalized = (this.SunTransform.position - this.EarthTransform.position).normalized;
			this._materialBlock.SetVector("_SunNormal", normalized);
			if (this.earthMeshRenderer != null)
			{
				this.earthMeshRenderer.SetPropertyBlock(this._materialBlock);
			}
		}

		// Token: 0x060024D7 RID: 9431 RVA: 0x000C6478 File Offset: 0x000C4678
		public void UpdateRegionGDPValues()
		{
			Log.Warn("UPDATE REGION GDP VALUES", Array.Empty<object>());
			for (int i = 1; i < this.numMapRegions + 1; i++)
			{
				this.gdpList[i] = (this.uniformGDP ? this.normalizedGDP : global::UnityEngine.Random.Range(0f, this.normalizedGDP));
				this.gdpDisplayList[i] = this.gdpList[i] * this.gdpAnimationList[i];
			}
			this.UpdateShader();
		}

		// Token: 0x060024D8 RID: 9432 RVA: 0x000C64F0 File Offset: 0x000C46F0
		public void UpdateRegionAlienControlValues(bool alienControl)
		{
			Log.Warn("UPDATE REGION GDP VALUES", Array.Empty<object>());
			for (int i = 1; i < this.numMapRegions + 1; i++)
			{
				this.alienControlList[i] = (alienControl ? 1f : 0f);
			}
			this.UpdateShader();
		}

		// Token: 0x060024D9 RID: 9433 RVA: 0x000C653C File Offset: 0x000C473C
		public void UpdateRegionOilValues()
		{
			Log.Warn("UPDATE Oil VALUES", Array.Empty<object>());
			for (int i = 1; i < this.numOilRegions + 1; i++)
			{
				this.oilList[i] = (this.uniformOil ? this.normalizedOil : global::UnityEngine.Random.Range(0f, this.normalizedOil));
				this.oilDisplayList[i] = this.oilList[i] * this.oilAnimationList[i];
			}
			this.UpdateShader();
		}

		// Token: 0x060024DA RID: 9434 RVA: 0x000C65B4 File Offset: 0x000C47B4
		public void UpdateSpecificGDP()
		{
			this.gdpList[this.targetVisualID] = this.targetNormalizedGDP;
			this.gdpDisplayList[this.targetVisualID] = this.gdpList[this.targetVisualID] * this.gdpAnimationList[this.targetVisualID];
			this.UpdateShader();
		}

		// Token: 0x060024DB RID: 9435 RVA: 0x000C6602 File Offset: 0x000C4802
		public void UpdateSpecificAlienControl()
		{
			this.alienControlList[this.targetVisualID] = this.targetAlienControl;
			this.UpdateShader();
		}

		// Token: 0x060024DC RID: 9436 RVA: 0x000C6620 File Offset: 0x000C4820
		public void UpdateSpecificOil()
		{
			this.oilList[this.targetOilID] = this.targetOil;
			this.oilDisplayList[this.targetOilID] = this.oilList[this.targetOilID] * this.oilAnimationList[this.targetOilID];
			this.UpdateShader();
		}

		// Token: 0x060024DD RID: 9437 RVA: 0x000C6670 File Offset: 0x000C4870
		private void UpdateShader()
		{
			this._materialBlock.SetFloatArray("_RegionGDP", this.gdpDisplayList);
			this._materialBlock.SetFloatArray("_RegionAlienControl", this.alienControlList);
			this._materialBlock.SetFloatArray("_RegionOil", this.oilDisplayList);
			if (this.earthMeshRenderer != null)
			{
				this.earthMeshRenderer.SetPropertyBlock(this._materialBlock);
			}
		}

		// Token: 0x060024DE RID: 9438 RVA: 0x000C66E0 File Offset: 0x000C48E0
		private void UpdateSpecificRegionLightValues(int visualID, int oilID, bool sendToShader = true)
		{
			this.gdpList[visualID] = this.normalizedGDP;
			this.gdpDisplayList[visualID] = this.gdpList[visualID] * this.gdpAnimationList[visualID];
			this.oilList[oilID] = this.targetOil;
			this.oilDisplayList[oilID] = this.oilList[oilID] * this.oilAnimationList[oilID];
			if (sendToShader)
			{
				this._materialBlock.SetFloatArray("_RegionGDP", this.gdpDisplayList);
				this._materialBlock.SetFloatArray("_RegionAlienControl", this.alienControlList);
				this._materialBlock.SetFloatArray("_RegionOil", this.oilDisplayList);
				this.earthMeshRenderer.SetPropertyBlock(this._materialBlock);
			}
		}

		// Token: 0x060024DF RID: 9439 RVA: 0x000C6791 File Offset: 0x000C4991
		public void HandleNukeRegion()
		{
			Debug.Log("HandleNukeRegion");
			base.StartCoroutine(this.NukeRegion(this.targetVisualID, this.targetOilID));
		}

		// Token: 0x060024E0 RID: 9440 RVA: 0x000C67B6 File Offset: 0x000C49B6
		private IEnumerator NukeRegion(int visualID, int oilID)
		{
			float nukeAnimationLength = this.nukeAnimation.keys[this.nukeAnimation.keys.Length - 1].time;
			float count = 0f;
			Debug.Log(string.Format("NukeRegion {0}", nukeAnimationLength));
			while (count < nukeAnimationLength)
			{
				Debug.Log(string.Format("Nuke animation: {0}, {1}", count, this.nukeAnimation.Evaluate(count)));
				this.gdpAnimationList[visualID] = this.nukeAnimation.Evaluate(count);
				this.oilAnimationList[oilID] = this.nukeAnimation.Evaluate(count);
				this.UpdateSpecificRegionLightValues(visualID, oilID, true);
				count += Time.deltaTime;
				yield return null;
			}
			this.gdpAnimationList[visualID] = this.nukeAnimation.Evaluate(nukeAnimationLength);
			this.oilAnimationList[oilID] = this.nukeAnimation.Evaluate(nukeAnimationLength);
			this.UpdateSpecificRegionLightValues(visualID, oilID, true);
			yield break;
		}

		// Token: 0x04001BB4 RID: 7092
		private MeshRenderer earthMeshRenderer;

		// Token: 0x04001BB5 RID: 7093
		public Transform SunTransform;

		// Token: 0x04001BB6 RID: 7094
		public Transform EarthTransform;

		// Token: 0x04001BB7 RID: 7095
		[Header("Region GDP Testing")]
		public bool uniformGDP = true;

		// Token: 0x04001BB8 RID: 7096
		private int numMapRegions = 363;

		// Token: 0x04001BB9 RID: 7097
		private float[] gdpList;

		// Token: 0x04001BBA RID: 7098
		private float[] gdpAnimationList;

		// Token: 0x04001BBB RID: 7099
		private float[] gdpDisplayList;

		// Token: 0x04001BBC RID: 7100
		private float[] alienControlList;

		// Token: 0x04001BBD RID: 7101
		[Range(0f, 5f)]
		public float normalizedGDP = 3f;

		// Token: 0x04001BBE RID: 7102
		public int targetVisualID;

		// Token: 0x04001BBF RID: 7103
		[Range(0f, 5f)]
		public float targetNormalizedGDP = 3f;

		// Token: 0x04001BC0 RID: 7104
		public float targetAlienControl;

		// Token: 0x04001BC1 RID: 7105
		[Header("Oil Light Testing")]
		public bool uniformOil = true;

		// Token: 0x04001BC2 RID: 7106
		private int numOilRegions = 37;

		// Token: 0x04001BC3 RID: 7107
		private float[] oilList;

		// Token: 0x04001BC4 RID: 7108
		private float[] oilAnimationList;

		// Token: 0x04001BC5 RID: 7109
		private float[] oilDisplayList;

		// Token: 0x04001BC6 RID: 7110
		[Range(0f, 50f)]
		public float normalizedOil = 3f;

		// Token: 0x04001BC7 RID: 7111
		public int targetOilID;

		// Token: 0x04001BC8 RID: 7112
		[Range(0f, 5f)]
		public float targetOil;

		// Token: 0x04001BC9 RID: 7113
		[Header("Nuke Animation")]
		[SerializeField]
		private AnimationCurve nukeAnimation;

		// Token: 0x04001BCA RID: 7114
		private MaterialPropertyBlock _materialBlock;
	}
}
