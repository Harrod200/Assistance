using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta.Ship;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020005AA RID: 1450
	public class DamageLayer : MonoBehaviour
	{
		// Token: 0x06002765 RID: 10085 RVA: 0x000D7A8B File Offset: 0x000D5C8B
		private void Start()
		{
			ShipModelController component = base.GetComponent<ShipModelController>();
			this.refShipState = ((component != null) ? component.GetRefShipState() : null);
		}

		// Token: 0x06002766 RID: 10086 RVA: 0x000D7AA8 File Offset: 0x000D5CA8
		private void OnEnable()
		{
			this.SyncDamageVisualizations();
			foreach (Renderer renderer in this._shipRenderers)
			{
				Material[] materials = renderer.materials;
				this._originalMaterials[renderer] = materials;
				List<Material> list = new List<Material>();
				list.AddRange(materials);
				Material material = new Material(this._shipDamageMaterial);
				this._damageMaterials.Add(material);
				list.Add(material);
				renderer.materials = list.ToArray();
			}
		}

		// Token: 0x06002767 RID: 10087 RVA: 0x000D7B28 File Offset: 0x000D5D28
		private void OnDisable()
		{
			foreach (Renderer renderer in this._shipRenderers)
			{
				renderer.materials = this._originalMaterials[renderer];
			}
			for (int j = this._damageMaterials.Count - 1; j >= 0; j--)
			{
				global::UnityEngine.Object.Destroy(this._damageMaterials[j]);
			}
			this._damageMaterials.Clear();
		}

		// Token: 0x06002768 RID: 10088 RVA: 0x000D7B94 File Offset: 0x000D5D94
		private void LateUpdate()
		{
			Vector4[] array = new Vector4[8];
			for (int i = 0; i < 8; i++)
			{
				if (i < this._damagePoints.Count)
				{
					array[i] = this._damagePoints[i];
				}
				else
				{
					array[i] = Vector4.zero;
				}
			}
			foreach (Material material in this._damageMaterials)
			{
				material.SetInt(DamageLayer.s_uDamagePointArrayLength, this._damagePoints.Count);
				material.SetVectorArray(DamageLayer.s_uDamagePointArray, array);
			}
			if (this._clearDamageOnUpdate)
			{
				this.ClearDamagePoints();
			}
		}

		// Token: 0x06002769 RID: 10089 RVA: 0x000D7C50 File Offset: 0x000D5E50
		public void AddDamagePoint(Vector3 hitPosition, float radius, DamageType damageType)
		{
			if (!TIGameState.Valid(this.refShipState))
			{
				return;
			}
			this.refShipState.SyncDamageVisuals();
			radius = Mathf.Clamp(radius * ((float)(this._damagePoints.Count + 1) * 1.02f), 1f, 50f);
			float num = Mathf.Min(0.99f, (float)damageType / 2f);
			float packedFloat = DamageLayer.GetPackedFloat(Mathf.Min(50.1f, radius / 50.1f), num);
			this._damagePoints.Add(new Vector4(hitPosition.x, hitPosition.y, hitPosition.z, packedFloat));
			if (this._damagePoints.Count > 8)
			{
				this._damagePoints = this._damagePoints.OrderByDescending<Vector4, float>((Vector4 o) => o.w).ToList<Vector4>();
				this._damagePoints.RemoveAt(this._damagePoints.Count - 1);
			}
		}

		// Token: 0x0600276A RID: 10090 RVA: 0x000D7D48 File Offset: 0x000D5F48
		public static Vector4 AddDamagePointInternal(Vector3 hitPosition, float radius, DamageType damageType)
		{
			float num = Mathf.Min(0.99f, (float)damageType / 2f);
			float packedFloat = DamageLayer.GetPackedFloat(Mathf.Min(50.1f, radius / 50.1f), num);
			return new Vector4(hitPosition.x, hitPosition.y, hitPosition.z, packedFloat);
		}

		// Token: 0x0600276B RID: 10091 RVA: 0x000D7D98 File Offset: 0x000D5F98
		public List<Vector4> GetDamagePoints()
		{
			return this._damagePoints;
		}

		// Token: 0x0600276C RID: 10092 RVA: 0x000D7DA0 File Offset: 0x000D5FA0
		public void LoadDamagePoints(List<Vector4> damagePoints)
		{
			this._damagePoints = damagePoints;
		}

		// Token: 0x0600276D RID: 10093 RVA: 0x000D7DA9 File Offset: 0x000D5FA9
		public void SyncDamageVisualizations()
		{
			if (this.refShipState != null)
			{
				this._damagePoints = this.refShipState.damagePoints;
			}
		}

		// Token: 0x0600276E RID: 10094 RVA: 0x000D7DCA File Offset: 0x000D5FCA
		public void ClearDamagePoints()
		{
			this._damagePoints.Clear();
		}

		// Token: 0x0600276F RID: 10095 RVA: 0x000D7DD8 File Offset: 0x000D5FD8
		private static float GetPackedFloat(float a, float b)
		{
			uint num = (uint)(a * 65535f);
			uint num2 = (uint)(b * 65535f);
			return (num << 16) | (num2 & 65535U);
		}

		// Token: 0x06002770 RID: 10096 RVA: 0x000D7E04 File Offset: 0x000D6004
		private static ValueTuple<float, float> GetUnpackedFloat(float f)
		{
			uint num = (uint)f;
			float num2 = (num >> 16) / 65535f;
			float num3 = (num & 65535U) / 65535f;
			return new ValueTuple<float, float>(num2, num3);
		}

		// Token: 0x04001D4F RID: 7503
		private static readonly int s_uDamagePointArray = Shader.PropertyToID("_DamagePointArray");

		// Token: 0x04001D50 RID: 7504
		private static readonly int s_uDamagePointArrayLength = Shader.PropertyToID("_DamagePointArrayLength");

		// Token: 0x04001D51 RID: 7505
		[SerializeField]
		private Material _shipDamageMaterial;

		// Token: 0x04001D52 RID: 7506
		[SerializeField]
		private Renderer[] _shipRenderers;

		// Token: 0x04001D53 RID: 7507
		[SerializeField]
		private bool _clearDamageOnUpdate;

		// Token: 0x04001D54 RID: 7508
		private Dictionary<Renderer, Material[]> _originalMaterials = new Dictionary<Renderer, Material[]>();

		// Token: 0x04001D55 RID: 7509
		private List<Material> _damageMaterials = new List<Material>();

		// Token: 0x04001D56 RID: 7510
		private List<Vector4> _damagePoints = new List<Vector4>();

		// Token: 0x04001D57 RID: 7511
		private TISpaceShipState refShipState;
	}
}
