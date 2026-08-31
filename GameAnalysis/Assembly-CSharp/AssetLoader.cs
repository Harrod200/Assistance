using System;
using System.Collections.Generic;
using AssetBundles;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000029 RID: 41
public class AssetLoader
{
	// Token: 0x06000102 RID: 258 RVA: 0x00008B72 File Offset: 0x00006D72
	public void Initialize()
	{
	}

	// Token: 0x06000103 RID: 259 RVA: 0x00008B74 File Offset: 0x00006D74
	public T LoadAsset<T>(string asset) where T : global::UnityEngine.Object
	{
		return AssetBundleManager.LoadAsset<T>(asset);
	}

	// Token: 0x06000104 RID: 260 RVA: 0x00008B7C File Offset: 0x00006D7C
	public T[] LoadAll<T>(string[] assetArray) where T : global::UnityEngine.Object
	{
		T[] array = new T[assetArray.Length];
		int num = 0;
		foreach (string text in assetArray)
		{
			array[num++] = this.LoadAsset<T>(text);
		}
		return array;
	}

	// Token: 0x06000105 RID: 261 RVA: 0x00008BC0 File Offset: 0x00006DC0
	public GameObject InstantiatePrefab(string asset)
	{
		GameObject gameObject = this.LoadAsset<GameObject>(asset);
		if (gameObject == null)
		{
			return null;
		}
		return global::UnityEngine.Object.Instantiate<GameObject>(gameObject, Vector3.zero, Quaternion.identity);
	}

	// Token: 0x06000106 RID: 262 RVA: 0x00008BF0 File Offset: 0x00006DF0
	public GameObject InstantiatePrefab(string asset, Transform parent)
	{
		GameObject gameObject = this.LoadAsset<GameObject>(asset);
		if (gameObject == null)
		{
			return null;
		}
		return global::UnityEngine.Object.Instantiate<GameObject>(gameObject, Vector3.zero, Quaternion.identity, parent);
	}

	// Token: 0x06000107 RID: 263 RVA: 0x00008C24 File Offset: 0x00006E24
	public void LoadAssetForImageAssignment(string asset, Image imageToAssign)
	{
		Sprite sprite;
		if (this._cachedAssets.TryGetValue(asset, out sprite))
		{
			imageToAssign.sprite = sprite;
			return;
		}
		sprite = this.LoadAsset<Sprite>(asset);
		this._cachedAssets[asset] = sprite;
		imageToAssign.sprite = sprite;
	}

	// Token: 0x06000108 RID: 264 RVA: 0x00008C65 File Offset: 0x00006E65
	public Texture2D LoadAssetForTexture2DAssignment(string asset)
	{
		return this.LoadAsset<Texture2D>(asset);
	}

	// Token: 0x06000109 RID: 265 RVA: 0x00008C6E File Offset: 0x00006E6E
	public Sprite LoadAssetForSpriteAssignment(string asset)
	{
		return this.LoadAsset<Sprite>(asset);
	}

	// Token: 0x04000109 RID: 265
	private Dictionary<string, Sprite> _cachedAssets = new Dictionary<string, Sprite>();
}
