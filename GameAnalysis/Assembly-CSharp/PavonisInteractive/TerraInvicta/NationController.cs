using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200055F RID: 1375
	public class NationController : MonoBehaviour
	{
		// Token: 0x06002457 RID: 9303 RVA: 0x000C0C84 File Offset: 0x000BEE84
		public void Initialize(TIGameState gamestate, MapController mapVis)
		{
			if (SceneManager.GetActiveScene().name == "RegionVisualizerTestScene")
			{
				if (!this.is3D)
				{
					GameObject gameObject = global::UnityEngine.Object.Instantiate<GameObject>(base.gameObject);
					gameObject.GetComponent<NationController>().is3D = true;
					gameObject.GetComponent<NationController>().Initialize(gamestate, mapVis);
				}
			}
			else
			{
				this.is3D = true;
			}
			this.nationState = gamestate as TINationState;
			base.gameObject.name = this.nationState.templateName;
			this.mapVisualizer = mapVis;
			base.transform.position = Vector3.zero;
			using (List<TIRegionState>.Enumerator enumerator = this.nationState.regions.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIRegionState regionState = enumerator.Current;
					Log.Time("<color=#00cc00>LoadTime:</color> MapController InitializeRegion " + regionState.displayName, delegate
					{
						RegionController component = global::UnityEngine.Object.Instantiate<GameObject>(this.regionControllerPrefab, this.transform).GetComponent<RegionController>();
						if (component != null)
						{
							component.Initialize(regionState, this);
							this.regionVisualizers.Add(component);
						}
					}, false, true);
				}
			}
			GameControl.eventManager.AddListener<NationFlashEvent>(new EventManager.EventDelegate<NationFlashEvent>(this.FlashNation), null, this.nationState, true, false);
			GameControl.eventManager.AddListener<NationStateSelected>(new EventManager.EventDelegate<NationStateSelected>(this.OnNationSelected), null, this.nationState, true, false);
			GameControl.eventManager.AddListener<CurrentOtherStateDeselected>(new EventManager.EventDelegate<CurrentOtherStateDeselected>(this.OnNationDeselected), null, this.nationState, true, false);
		}

		// Token: 0x06002458 RID: 9304 RVA: 0x000C0DF4 File Offset: 0x000BEFF4
		public void SetOutlineWidth(float newWidth)
		{
			foreach (RegionController regionController in this.regionVisualizers)
			{
				regionController.SetWidth(newWidth);
			}
		}

		// Token: 0x06002459 RID: 9305 RVA: 0x000C0E48 File Offset: 0x000BF048
		public void SetLiftValue(float newLift)
		{
			foreach (RegionController regionController in this.regionVisualizers)
			{
				regionController.SetLiftValue(newLift);
			}
		}

		// Token: 0x0600245A RID: 9306 RVA: 0x000C0E9C File Offset: 0x000BF09C
		public void UpdateRegionsTextures()
		{
			foreach (RegionController regionController in this.regionVisualizers)
			{
				if (regionController.isActiveAndEnabled)
				{
					regionController.RestoreRegionTexture();
				}
			}
		}

		// Token: 0x0600245B RID: 9307 RVA: 0x000C0EF8 File Offset: 0x000BF0F8
		public void FlashNation(NationFlashEvent e)
		{
			foreach (RegionController regionController in this.regionVisualizers)
			{
				if (regionController.isActiveAndEnabled)
				{
					base.StartCoroutine(regionController.FlashRegion());
				}
			}
		}

		// Token: 0x0600245C RID: 9308 RVA: 0x000C0F5C File Offset: 0x000BF15C
		public void OnNationSelected(NationStateSelected e)
		{
			this.UpdateRegionsTextures();
		}

		// Token: 0x0600245D RID: 9309 RVA: 0x000C0F64 File Offset: 0x000BF164
		public void OnNationDeselected(CurrentOtherStateDeselected e)
		{
			this.UpdateRegionsTextures();
		}

		// Token: 0x0600245E RID: 9310 RVA: 0x000C0F6C File Offset: 0x000BF16C
		public bool GetCouncilorLocation(TIRegionState region, out Vector3 location)
		{
			foreach (RegionController regionController in this.regionVisualizers)
			{
				if (regionController.region == region && regionController.GetCouncilorLocation(out location))
				{
					return true;
				}
			}
			location = Vector3.zero;
			return false;
		}

		// Token: 0x04001B59 RID: 7001
		public GameObject regionControllerPrefab;

		// Token: 0x04001B5A RID: 7002
		public GameStateID nationStateID;

		// Token: 0x04001B5B RID: 7003
		public TINationState nationState;

		// Token: 0x04001B5C RID: 7004
		public MapController mapVisualizer;

		// Token: 0x04001B5D RID: 7005
		public List<RegionController> regionVisualizers;

		// Token: 0x04001B5E RID: 7006
		public bool is3D;
	}
}
