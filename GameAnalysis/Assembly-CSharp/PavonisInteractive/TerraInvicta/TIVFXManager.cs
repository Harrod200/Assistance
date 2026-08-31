using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta.SpaceCombat;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020006F3 RID: 1779
	public class TIVFXManager : MonoBehaviour
	{
		// Token: 0x170005C7 RID: 1479
		// (get) Token: 0x060029E1 RID: 10721 RVA: 0x000E2DA2 File Offset: 0x000E0FA2
		public static TIVFXManager Instance
		{
			get
			{
				return TIVFXManager._instance;
			}
		}

		// Token: 0x060029E2 RID: 10722 RVA: 0x000E2DA9 File Offset: 0x000E0FA9
		private void Awake()
		{
			if (TIVFXManager._instance != null && TIVFXManager._instance != this)
			{
				global::UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			TIVFXManager._instance = this;
			global::UnityEngine.Object.DontDestroyOnLoad(this);
		}

		// Token: 0x060029E3 RID: 10723 RVA: 0x000E2DE0 File Offset: 0x000E0FE0
		public void CreateCombatVFXBuffer(List<CombatShipController> shipsInCombat)
		{
			float num = 1f;
			if (shipsInCombat.Count > TIPlayerProfileManager.maxShipsInCombat)
			{
				num = (float)(TIPlayerProfileManager.maxShipsInCombat / shipsInCombat.Count);
			}
			int poolCount = this.GetPoolCount(TemplateManager.global.pathAlienThrusterVFX);
			int poolCount2 = this.GetPoolCount(TemplateManager.global.pathHumanThrusterBasicVFX);
			int poolCount3 = this.GetPoolCount(TemplateManager.global.pathHumanThrusterAdvancedVFX);
			int poolCount4 = this.GetPoolCount(TemplateManager.global.pathFallbackMuzzleFlashVFX);
			int poolCount5 = this.GetPoolCount(TemplateManager.global.pathFallbackLaserVFX);
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			Dictionary<string, int> dictionary2 = new Dictionary<string, int>();
			foreach (CombatShipController combatShipController in shipsInCombat)
			{
				foreach (ModuleDataEntry moduleDataEntry in combatShipController.ShipState.hullWeapons)
				{
					if (moduleDataEntry.weaponTemplate.ref_missileWeapon != null)
					{
						int num2 = (int)((float)moduleDataEntry.weaponTemplate.ref_missileWeapon.FullAmmoCount_Max(combatShipController.ShipState.template) * num);
						string text = ((moduleDataEntry.weaponTemplate.ref_projectileWeapon.ammoMass_kg < 10f) ? "spaceCombat/TinyExplosion" : "spaceCombat/SmallExplosion");
						if (!dictionary.ContainsKey(moduleDataEntry.weaponTemplate.ref_missileWeapon.impactVisualFXResource))
						{
							dictionary.Add(moduleDataEntry.weaponTemplate.ref_missileWeapon.impactVisualFXResource, num2);
						}
						else
						{
							Dictionary<string, int> dictionary3 = dictionary;
							string text2 = moduleDataEntry.weaponTemplate.ref_missileWeapon.impactVisualFXResource;
							dictionary3[text2] += num2;
						}
						if (!dictionary2.ContainsKey(text))
						{
							dictionary2.Add(text, num2);
						}
						else
						{
							Dictionary<string, int> dictionary3 = dictionary2;
							string text2 = text;
							dictionary3[text2] += num2;
						}
					}
				}
			}
			foreach (KeyValuePair<string, int> keyValuePair in dictionary)
			{
				int poolCount6 = this.GetPoolCount(keyValuePair.Key);
				if (poolCount6 < keyValuePair.Value)
				{
					for (int i = 0; i < keyValuePair.Value - poolCount6; i++)
					{
						GameObject gameObject = GameControl.assetLoader.InstantiatePrefab(keyValuePair.Key, TIVFXManager.Instance.transform);
						gameObject.SetActive(false);
						this.AddObjectToPool(keyValuePair.Key, gameObject);
					}
				}
			}
			foreach (KeyValuePair<string, int> keyValuePair2 in dictionary2)
			{
				int poolCount7 = this.GetPoolCount(keyValuePair2.Key);
				if (poolCount7 < keyValuePair2.Value)
				{
					for (int j = 0; j < keyValuePair2.Value - poolCount7; j++)
					{
						GameObject gameObject2 = GameControl.assetLoader.InstantiatePrefab(keyValuePair2.Key, TIVFXManager.Instance.transform);
						gameObject2.SetActive(false);
						this.AddObjectToPool(keyValuePair2.Key, gameObject2);
					}
				}
			}
			for (int k = 0; k < 180 - poolCount; k++)
			{
				GameObject gameObject3 = GameControl.assetLoader.InstantiatePrefab(TemplateManager.global.pathAlienThrusterVFX, TIVFXManager.Instance.transform);
				this.AddObjectToPool(TemplateManager.global.pathAlienThrusterVFX, gameObject3);
			}
			for (int l = 0; l < 180 - poolCount2; l++)
			{
				GameObject gameObject4 = GameControl.assetLoader.InstantiatePrefab(TemplateManager.global.pathHumanThrusterBasicVFX, TIVFXManager.Instance.transform);
				this.AddObjectToPool(TemplateManager.global.pathHumanThrusterBasicVFX, gameObject4);
			}
			for (int m = 0; m < 180 - poolCount3; m++)
			{
				GameObject gameObject5 = GameControl.assetLoader.InstantiatePrefab(TemplateManager.global.pathHumanThrusterAdvancedVFX, TIVFXManager.Instance.transform);
				this.AddObjectToPool(TemplateManager.global.pathHumanThrusterAdvancedVFX, gameObject5);
			}
			for (int n = 0; n < 96 - poolCount4; n++)
			{
				GameObject gameObject6 = GameControl.assetLoader.InstantiatePrefab(TemplateManager.global.pathFallbackMuzzleFlashVFX, TIVFXManager.Instance.transform);
				this.AddObjectToPool(TemplateManager.global.pathFallbackMuzzleFlashVFX, gameObject6);
			}
			for (int num3 = 0; num3 < 30 - poolCount5; num3++)
			{
				GameObject gameObject7 = GameControl.assetLoader.InstantiatePrefab(TemplateManager.global.pathFallbackLaserVFX, TIVFXManager.Instance.transform);
				this.AddObjectToPool(TemplateManager.global.pathFallbackLaserVFX, gameObject7);
			}
		}

		// Token: 0x060029E4 RID: 10724 RVA: 0x000E32C4 File Offset: 0x000E14C4
		public void CachePrefabs()
		{
			this.AlienGlowGO = GameControl.assetLoader.LoadAsset<GameObject>("vfx/Alien Glow");
			this.BigExplosionGO = GameControl.assetLoader.LoadAsset<GameObject>("vfx/BigExplosion");
			this.RocketTrailGO = GameControl.assetLoader.LoadAsset<GameObject>("vfx/RocketTrail_MarkerPrefab");
			this.TinyFlamesGO = GameControl.assetLoader.LoadAsset<GameObject>("vfx/TinyFlames");
			this.ArtilleryFlashesGO = GameControl.assetLoader.LoadAsset<GameObject>("vfx/Artillery Flashes");
			this.NukeLaunchGO = GameControl.assetLoader.LoadAsset<GameObject>("vfx/Nuke Launch");
			this.NukeStrikeGO = GameControl.assetLoader.LoadAsset<GameObject>("vfx/Nuke Strike");
			this.LinearFlamesGO = GameControl.assetLoader.LoadAsset<GameObject>("vfx/LinearFlames");
			this.AlienMobileLightsGO = GameControl.assetLoader.LoadAsset<GameObject>("vfx/Alien Mobile Lights");
			this.ReentryFlamesGO = GameControl.assetLoader.LoadAsset<GameObject>("vfx/Reentry Flames");
		}

		// Token: 0x060029E5 RID: 10725 RVA: 0x000E33A4 File Offset: 0x000E15A4
		private void AddObjectToPool(string poolKey, GameObject objectToAdd)
		{
			if (TIVFXManager.Instance.freeVFXPrefabs.ContainsKey(poolKey))
			{
				TIVFXManager.Instance.freeVFXPrefabs[poolKey].Add(objectToAdd.GetInstanceID(), objectToAdd);
				return;
			}
			TIVFXManager.Instance.freeVFXPrefabs.Add(poolKey, new Dictionary<int, GameObject> { 
			{
				objectToAdd.GetInstanceID(),
				objectToAdd
			} });
		}

		// Token: 0x060029E6 RID: 10726 RVA: 0x000E3402 File Offset: 0x000E1602
		private int GetPoolCount(string poolKey)
		{
			if (this.freeVFXPrefabs.ContainsKey(poolKey))
			{
				return this.freeVFXPrefabs[poolKey].Count;
			}
			return 0;
		}

		// Token: 0x060029E7 RID: 10727 RVA: 0x000E3428 File Offset: 0x000E1628
		public static GameObject GetVFX(string name, Transform parent = null)
		{
			if (!TIVFXManager.Instance.init)
			{
				TIVFXManager.Instance.CachePrefabs();
				TIVFXManager.Instance.init = true;
			}
			if (TIVFXManager.Instance.freeVFXPrefabs.ContainsKey(name) && TIVFXManager.Instance.freeVFXPrefabs[name].Count > 0)
			{
				GameObject value = TIVFXManager.Instance.freeVFXPrefabs[name].ElementAt<KeyValuePair<int, GameObject>>(0).Value;
				TIVFXManager.Instance.freeVFXPrefabs[name].Remove(value.GetInstanceID());
				if (parent != null)
				{
					value.transform.SetParent(parent);
				}
				return value;
			}
			GameObject gameObject = GameControl.assetLoader.InstantiatePrefab(name, parent);
			gameObject.transform.localPosition = Vector3.zero;
			return gameObject;
		}

		// Token: 0x060029E8 RID: 10728 RVA: 0x000E34EE File Offset: 0x000E16EE
		public static void ReturnVFX(string name, GameObject obj)
		{
			if (TIVFXManager.Instance == null)
			{
				return;
			}
			obj.SetActive(false);
			obj.transform.SetParent(TIVFXManager.Instance.transform, false);
			TIVFXManager.Instance.AddObjectToPool(name, obj);
		}

		// Token: 0x0400203F RID: 8255
		public GameObject AlienGlowGO;

		// Token: 0x04002040 RID: 8256
		public GameObject BigExplosionGO;

		// Token: 0x04002041 RID: 8257
		public GameObject RocketTrailGO;

		// Token: 0x04002042 RID: 8258
		public GameObject TinyFlamesGO;

		// Token: 0x04002043 RID: 8259
		public GameObject ArtilleryFlashesGO;

		// Token: 0x04002044 RID: 8260
		public GameObject NukeLaunchGO;

		// Token: 0x04002045 RID: 8261
		public GameObject NukeStrikeGO;

		// Token: 0x04002046 RID: 8262
		public GameObject LinearFlamesGO;

		// Token: 0x04002047 RID: 8263
		public GameObject AlienMobileLightsGO;

		// Token: 0x04002048 RID: 8264
		public GameObject ReentryFlamesGO;

		// Token: 0x04002049 RID: 8265
		private Dictionary<string, Dictionary<int, GameObject>> freeVFXPrefabs = new Dictionary<string, Dictionary<int, GameObject>>();

		// Token: 0x0400204A RID: 8266
		private bool init;

		// Token: 0x0400204B RID: 8267
		private static TIVFXManager _instance;

		// Token: 0x02000D19 RID: 3353
		public static class EarthMarkerVFX
		{
			// Token: 0x0400507C RID: 20604
			public const string AlienGlow = "vfx/Alien Glow";

			// Token: 0x0400507D RID: 20605
			public const string BigExplosion = "vfx/BigExplosion";

			// Token: 0x0400507E RID: 20606
			public const string RocketTrail = "vfx/RocketTrail_MarkerPrefab";

			// Token: 0x0400507F RID: 20607
			public const string TinyFlames = "vfx/TinyFlames";

			// Token: 0x04005080 RID: 20608
			public const string ArtilleryFlashes = "vfx/Artillery Flashes";

			// Token: 0x04005081 RID: 20609
			public const string NukeLaunch = "vfx/Nuke Launch";

			// Token: 0x04005082 RID: 20610
			public const string NukeStrike = "vfx/Nuke Strike";

			// Token: 0x04005083 RID: 20611
			public const string LinearFlames = "vfx/LinearFlames";

			// Token: 0x04005084 RID: 20612
			public const string AlienMobileLights = "vfx/Alien Mobile Lights";

			// Token: 0x04005085 RID: 20613
			public const string ReentryFlames = "vfx/Reentry Flames";
		}
	}
}
