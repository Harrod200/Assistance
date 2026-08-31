using System;
using System.Collections.Generic;
using System.IO;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Modding;
using UnityEngine;

namespace AssetBundles
{
	// Token: 0x020004ED RID: 1261
	public class AssetBundleManager
	{
		// Token: 0x17000446 RID: 1094
		// (get) Token: 0x06001E2E RID: 7726 RVA: 0x0009E216 File Offset: 0x0009C416
		public static bool SimulateAssetBundleInEditor
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06001E2F RID: 7727 RVA: 0x0009E21C File Offset: 0x0009C41C
		public static void Initialize()
		{
			if (AssetBundleManager.SimulateAssetBundleInEditor)
			{
				return;
			}
			if (AssetBundleManager.loadedBundles != null)
			{
				return;
			}
			AssetBundleManager.bundlePath = Path.Combine(Application.streamingAssetsPath, "AssetBundles");
			AssetBundleManager.manifest = AssetBundle.LoadFromFile(Path.Combine(AssetBundleManager.bundlePath, "AssetBundles")).LoadAsset<AssetBundleManifest>("AssetBundleManifest");
			string[] allAssetBundles = AssetBundleManager.manifest.GetAllAssetBundles();
			if (AssetBundleManager.loadedBundles == null)
			{
				AssetBundleManager.loadedBundles = new Dictionary<string, AssetBundle>(allAssetBundles.Length);
			}
			for (int i = 0; i < allAssetBundles.Length; i++)
			{
				AssetBundle assetBundle = AssetBundle.LoadFromFile(Path.Combine(AssetBundleManager.bundlePath, allAssetBundles[i]));
				AssetBundleManager.loadedBundles.Add(allAssetBundles[i], assetBundle);
			}
			if (ModManager.dlcDirectories.Count > 0 && ModManager.dlcAssetbundleManifestFiles.Count > 0)
			{
				List<AssetBundle> list = new List<AssetBundle>();
				for (int j = 0; j < ModManager.dlcAssetbundleManifestFiles.Count; j++)
				{
					list.Add(AssetBundle.LoadFromFile(ModManager.dlcAssetbundles[j]));
				}
				for (int k = 0; k < list.Count; k++)
				{
					if (AssetBundleManager.loadedBundles.ContainsKey(list[k].name))
					{
						AssetBundleManager.loadedBundles.Remove(list[k].name);
					}
					AssetBundleManager.loadedBundles.Add(list[k].name, list[k]);
				}
			}
			if (TIPlayerProfileManager.useMods && ModManager.ModDirectories.Count > 0 && ModManager.ModAssetBundleManifestFiles.Count > 0)
			{
				new List<AssetBundleManifest>();
				List<AssetBundle> list2 = new List<AssetBundle>();
				for (int l = 0; l < ModManager.ModAssetBundleManifestFiles.Count; l++)
				{
					list2.Add(AssetBundle.LoadFromFile(ModManager.ModAssetBundles[l]));
				}
				for (int m = 0; m < list2.Count; m++)
				{
					AssetBundleManager.loadedBundles.Add(list2[m].name, list2[m]);
				}
			}
		}

		// Token: 0x06001E30 RID: 7728 RVA: 0x0009E418 File Offset: 0x0009C618
		public static T LoadAsset<T>(string assetPath) where T : global::UnityEngine.Object
		{
			if (!assetPath.Contains("/"))
			{
				Log.Error("Invalid asset path \"" + assetPath + "\" expected BUNDLE/ASSET format", Array.Empty<object>());
				return default(T);
			}
			string[] array = assetPath.Split(new char[] { '/' });
			string text = array[0].ToLowerInvariant();
			string text2 = array[1];
			if (AssetBundleManager.loadedBundles.ContainsKey(text))
			{
				T t = AssetBundleManager.loadedBundles[text].LoadAsset<T>(text2);
				if (t == null)
				{
					Debug.LogWarning("No asset found for " + assetPath);
				}
				return t;
			}
			Debug.LogError("Bundle not found! bundle name: " + text);
			return default(T);
		}

		// Token: 0x06001E31 RID: 7729 RVA: 0x0009E4C9 File Offset: 0x0009C6C9
		public static void UnloadAssetBundle(string assetBundleName)
		{
			if (AssetBundleManager.SimulateAssetBundleInEditor)
			{
				return;
			}
			AssetBundleManager.loadedBundles[assetBundleName].Unload(false);
			AssetBundleManager.loadedBundles.Remove(assetBundleName);
		}

		// Token: 0x06001E32 RID: 7730 RVA: 0x0009E4F0 File Offset: 0x0009C6F0
		public static bool AreDLCBundlesLoaded(int index)
		{
			return index == 1 && GameControl.DLCValidated && (Application.isEditor || AssetBundleManager.loadedBundles.ContainsKey("ships_prm"));
		}

		// Token: 0x040017EA RID: 6122
		private static string bundlePath = "";

		// Token: 0x040017EB RID: 6123
		private static AssetBundleManifest manifest;

		// Token: 0x040017EC RID: 6124
		private static Dictionary<string, AssetBundle> loadedBundles;

		// Token: 0x040017ED RID: 6125
		private static int simulateAssetBundleInEditor = -1;

		// Token: 0x040017EE RID: 6126
		private const string simulateAssetBundles = "SimulateAssetBundles";

		// Token: 0x040017EF RID: 6127
		private const string DLCAShipBundle = "ships_prm";

		// Token: 0x040017F0 RID: 6128
		public const string keySimulateDLCDarkSkiesInEditor = "SimulateDLCInstalled_DarkSkies";
	}
}
