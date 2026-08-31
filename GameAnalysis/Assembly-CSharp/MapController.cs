using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ModelShark;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Audio;
using UnityEngine;

// Token: 0x02000007 RID: 7
public class MapController : MonoBehaviour
{
	// Token: 0x17000003 RID: 3
	// (get) Token: 0x06000012 RID: 18 RVA: 0x00002606 File Offset: 0x00000806
	public bool initializing
	{
		get
		{
			return this._initializing;
		}
	}

	// Token: 0x17000004 RID: 4
	// (get) Token: 0x06000013 RID: 19 RVA: 0x0000260E File Offset: 0x0000080E
	public bool isActive
	{
		get
		{
			return this._isActive;
		}
	}

	// Token: 0x06000014 RID: 20 RVA: 0x00002616 File Offset: 0x00000816
	private void Awake()
	{
		this.mapTransform = base.gameObject.transform;
	}

	// Token: 0x06000015 RID: 21 RVA: 0x00002629 File Offset: 0x00000829
	public NationController GetNation(string nationName)
	{
		if (this.nationControllerLookup == null)
		{
			this.nationControllerLookup = new Dictionary<string, NationController>();
			return null;
		}
		if (this.nationControllerLookup.ContainsKey(nationName))
		{
			return this.nationControllerLookup[nationName];
		}
		return null;
	}

	// Token: 0x06000016 RID: 22 RVA: 0x0000265C File Offset: 0x0000085C
	public NationController GetNation(TINationState nation)
	{
		return this.GetNation(nation.templateName);
	}

	// Token: 0x06000017 RID: 23 RVA: 0x0000266C File Offset: 0x0000086C
	public RegionController GetRegionController(TIRegionState region)
	{
		NationController nation = this.GetNation(region.nation.templateName);
		if (nation == null)
		{
			return null;
		}
		return nation.regionVisualizers.FirstOrDefault<RegionController>((RegionController x) => x.region == region);
	}

	// Token: 0x06000018 RID: 24 RVA: 0x000026BF File Offset: 0x000008BF
	public RegionController GetRegionController(string regionName)
	{
		return this.GetRegionController(GameStateManager.FindByTemplate<TIRegionState>(regionName, false));
	}

	// Token: 0x17000005 RID: 5
	// (get) Token: 0x06000019 RID: 25 RVA: 0x000026CE File Offset: 0x000008CE
	public IList<NationController> GetNationControllers
	{
		get
		{
			return this.nationControllerLookup.Values.ToList<NationController>();
		}
	}

	// Token: 0x0600001A RID: 26 RVA: 0x000026E0 File Offset: 0x000008E0
	public void AddNationToLookup(NationController newNation, bool replace = false)
	{
		if (this.GetNation(newNation.nationState.templateName) == null)
		{
			this.nationControllerLookup[newNation.nationState.templateName] = newNation;
		}
	}

	// Token: 0x0600001B RID: 27 RVA: 0x00002714 File Offset: 0x00000914
	public void SetOutlineWidths(float newWidth)
	{
		foreach (KeyValuePair<string, NationController> keyValuePair in this.nationControllerLookup)
		{
			keyValuePair.Value.SetOutlineWidth(newWidth);
		}
	}

	// Token: 0x0600001C RID: 28 RVA: 0x00002770 File Offset: 0x00000970
	public IEnumerator SetRegionVisualizers(bool active)
	{
		if (this.lockout)
		{
			yield return null;
		}
		this.lockout = true;
		int i = 0;
		if (active)
		{
			foreach (NationController nationController in this.GetNationControllers)
			{
				foreach (RegionController regionController in nationController.regionVisualizers)
				{
					regionController.EnableRegionVisualizers(true);
				}
				int num = i;
				i = num + 1;
				if (i >= 12)
				{
					i = 0;
					yield return null;
				}
			}
			IEnumerator<NationController> enumerator = null;
			yield return null;
			foreach (NationController nationController2 in this.GetNationControllers)
			{
				foreach (RegionController regionController2 in nationController2.regionVisualizers)
				{
					regionController2.EnableMarkerVisualizers(true);
				}
				int num = i;
				i = num + 1;
				if (i >= 12)
				{
					i = 0;
					yield return null;
				}
			}
			enumerator = null;
		}
		else
		{
			foreach (NationController nationController3 in this.GetNationControllers)
			{
				foreach (RegionController regionController3 in nationController3.regionVisualizers)
				{
					regionController3.EnableMarkerVisualizers(active);
				}
				int num = i;
				i = num + 1;
				if (i >= 48)
				{
					i = 0;
					yield return null;
				}
			}
			IEnumerator<NationController> enumerator = null;
			yield return null;
			foreach (NationController nationController4 in this.GetNationControllers)
			{
				foreach (RegionController regionController4 in nationController4.regionVisualizers)
				{
					regionController4.EnableRegionVisualizers(active);
				}
				int num = i;
				i = num + 1;
				if (i >= 24)
				{
					i = 0;
					yield return null;
				}
			}
			enumerator = null;
		}
		yield return null;
		this.lockout = false;
		yield break;
		yield break;
	}

	// Token: 0x0600001D RID: 29 RVA: 0x00002788 File Offset: 0x00000988
	public void MakeActive(bool active)
	{
		if (this._isActive != active)
		{
			this._isActive = active;
			base.StopAllCoroutines();
			base.StartCoroutine(this.SetRegionVisualizers(this._isActive));
			GameControl.eventManager.TriggerEvent(new MapActivationChangedEvent(active, this), null, Array.Empty<object>());
		}
	}

	// Token: 0x0600001E RID: 30 RVA: 0x000027D8 File Offset: 0x000009D8
	public void ResetMapColors()
	{
		foreach (NationController nationController in this.nationControllerLookup.Values)
		{
			nationController.UpdateRegionsTextures();
		}
	}

	// Token: 0x0600001F RID: 31 RVA: 0x00002830 File Offset: 0x00000A30
	public void ActivateRegionTooltips()
	{
		if (!this.regionTooltipsActive)
		{
			this.regionTooltipsActive = true;
			foreach (TooltipTrigger tooltipTrigger in this.regionTooltips)
			{
				tooltipTrigger.enabled = true;
			}
		}
	}

	// Token: 0x06000020 RID: 32 RVA: 0x00002890 File Offset: 0x00000A90
	public void DeactivateRegionTooltips()
	{
		if (this.regionTooltipsActive)
		{
			this.regionTooltipsActive = false;
			foreach (TooltipTrigger tooltipTrigger in this.regionTooltips)
			{
				tooltipTrigger.enabled = false;
			}
		}
	}

	// Token: 0x06000021 RID: 33 RVA: 0x000028F0 File Offset: 0x00000AF0
	private void OnEnable()
	{
	}

	// Token: 0x06000022 RID: 34 RVA: 0x000028F2 File Offset: 0x00000AF2
	private IEnumerator LerpActive(bool show)
	{
		if (this.lerpLockout)
		{
			yield return null;
		}
		this.lerpLockout = true;
		float start = 1f;
		float target = 0f;
		if (show)
		{
			start = 0f;
			target = 1f;
		}
		float changeTime = MapController.mapTransitionTime;
		float deltaTime = 0f;
		while (deltaTime < changeTime)
		{
			float num = deltaTime / changeTime;
			float num2 = Mathf.Lerp(start, target, Utilities.SinEase(num));
			float num3 = this.maxLift * (1f - num2);
			this.SetLiftValue(num3, "");
			deltaTime += Time.unscaledDeltaTime;
			yield return null;
		}
		if (show)
		{
			this.SetLiftValue(0f, "");
		}
		this.lerpLockout = false;
		yield break;
	}

	// Token: 0x06000023 RID: 35 RVA: 0x00002908 File Offset: 0x00000B08
	public void SetLiftValue(float newLift, string nationName = "")
	{
		if (this.nationControllerLookup == null)
		{
			return;
		}
		foreach (KeyValuePair<string, NationController> keyValuePair in this.nationControllerLookup)
		{
			if (!(nationName != "") || !(nationName != keyValuePair.Value.nationState.templateName))
			{
				keyValuePair.Value.SetLiftValue(newLift);
			}
		}
	}

	// Token: 0x06000024 RID: 36 RVA: 0x00002990 File Offset: 0x00000B90
	public TIRegionOutline GetOutlineData(string regionName)
	{
		if (this.currentOutlines == null)
		{
			return null;
		}
		foreach (TIRegionOutline tiregionOutline in this.currentOutlines.regionOutlines)
		{
			if (tiregionOutline.regionName == regionName)
			{
				return tiregionOutline;
			}
		}
		Log.Error("Couldn't find outline for " + regionName, Array.Empty<object>());
		return null;
	}

	// Token: 0x06000025 RID: 37 RVA: 0x00002A1C File Offset: 0x00000C1C
	public void InitializeMap(SpaceObjectController controller, string regionAssetPath)
	{
		Log.Time("<color=#00cc00>LoadTime:</color> MapController InitializeMap", delegate
		{
			this.spaceController = controller;
			this.outlineName = regionAssetPath;
			this.currentOutlines = GameControl.assetLoader.LoadAsset<RegionOutlineCollection>(this.outlineName);
			if (this.nationContainerPrefab == null)
			{
				Debug.LogWarning("Failed to load nation container");
			}
			foreach (TINationState tinationState in (this.spaceController.spaceObjectState as TISpaceBodyState).nations)
			{
				if (tinationState != null)
				{
					NationController component = global::UnityEngine.Object.Instantiate<GameObject>(this.nationContainerPrefab, this.transform).GetComponent<NationController>();
					if (component != null)
					{
						component.Initialize(tinationState, this);
						this.AddNationToLookup(component, false);
					}
				}
			}
			this.StartCoroutine(this.SetRegionVisualizers(false));
			GameControl.eventManager.AddListener<CouncilorMissionAssigned>(new EventManager.EventDelegate<CouncilorMissionAssigned>(this.OnMissionAssigned), null, GameControl.control.activePlayer, false, false);
			GameControl.eventManager.AddListener<TimeEventComplete>(new EventManager.EventDelegate<TimeEventComplete>(this.OnMissionPhaseComplete), "CouncilorMissionUpdate", null, false, false);
		}, true, true);
	}

	// Token: 0x06000026 RID: 38 RVA: 0x00002A5C File Offset: 0x00000C5C
	private void OnMissionPhaseComplete(TimeEventComplete e)
	{
		foreach (TIFactionState tifactionState in GameStateManager.AllFactions())
		{
			if (tifactionState != GameControl.control.activePlayer)
			{
				foreach (TICouncilorState ticouncilorState in tifactionState.councilors)
				{
					TIMissionState activeMission = ticouncilorState.activeMission;
					if (activeMission != null && ticouncilorState.priorLocation != activeMission.targetLocation && GameControl.control.activePlayer.HasIntelOnCouncilorLocation(ticouncilorState))
					{
						this.Fly(ticouncilorState, activeMission.targetLocation);
					}
				}
			}
		}
		foreach (TIFactionState tifactionState2 in GameStateManager.AllFactions())
		{
			if (tifactionState2 == GameControl.control.activePlayer)
			{
				foreach (TICouncilorState ticouncilorState2 in tifactionState2.councilors)
				{
					TIMissionState activeMission2 = ticouncilorState2.activeMission;
					if (activeMission2 != null && ticouncilorState2.location != activeMission2.targetLocation && GameControl.control.activePlayer.HasIntelOnCouncilorLocation(ticouncilorState2))
					{
						this.Fly(ticouncilorState2, activeMission2.targetLocation);
					}
				}
			}
		}
	}

	// Token: 0x06000027 RID: 39 RVA: 0x00002BDC File Offset: 0x00000DDC
	private string GetKey(TICouncilorState councilorState)
	{
		return councilorState.ID.ToString() + " Movement";
	}

	// Token: 0x06000028 RID: 40 RVA: 0x00002C07 File Offset: 0x00000E07
	private void OnMissionAssigned(CouncilorMissionAssigned e)
	{
		this.Fly(e.councilor, e.mission.GetInitialMissionLocation());
	}

	// Token: 0x06000029 RID: 41 RVA: 0x00002C20 File Offset: 0x00000E20
	private void Fly(TICouncilorState councilor, TIGameState destination)
	{
		string key = this.GetKey(councilor);
		MapArc arc = this.GetArc(key);
		if (arc != null)
		{
			this.RemoveArc(key);
			global::UnityEngine.Object.Destroy(arc.gameObject);
		}
		if (councilor.OnEarth && destination.ref_region != null)
		{
			this.airplaneTexture = councilor.GetAirplaneTexture();
			Vector3 vector;
			if (TIMissionPhaseState.InMissionPhase())
			{
				vector = this.GetCouncilorLocation(TIMissionPhaseState.CouncilorLastKnownLocation(GameControl.control.activePlayer, councilor).ref_region, null);
			}
			else
			{
				vector = this.GetCouncilorLocation(councilor.priorLocation.ref_region, null);
			}
			Vector3 councilorLocation = this.GetCouncilorLocation(destination.ref_region, null);
			if (vector != councilorLocation)
			{
				AudioManager.PlayOneShot("event:/SFX/Environment/trig_SFX_Councilor_flies_on_Plane_to_Mission", false, false);
				councilor.EnterTransit();
				councilor.ChangeLocation(destination);
				this.DrawArc(key, this.airplaneTexture, vector, councilorLocation, councilor);
			}
		}
	}

	// Token: 0x0600002A RID: 42 RVA: 0x00002CFC File Offset: 0x00000EFC
	private Vector3 GetCouncilorLocation(TIRegionState region, TINationState nation = null)
	{
		Vector3 zero = Vector3.zero;
		NationController nationController = null;
		if (nation != null)
		{
			nationController = this.GetNation(nation.templateName);
		}
		if (nationController == null)
		{
			using (Dictionary<string, NationController>.ValueCollection.Enumerator enumerator = this.nationControllerLookup.Values.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.GetCouncilorLocation(region, out zero))
					{
						break;
					}
				}
				return zero;
			}
		}
		nationController.GetCouncilorLocation(region, out zero);
		return zero;
	}

	// Token: 0x0600002B RID: 43 RVA: 0x00002D8C File Offset: 0x00000F8C
	public void DrawArc(string key, Sprite sprite, Vector3 start, Vector3 end, TICouncilorState councilor)
	{
		GameObject gameObject = new GameObject(key);
		gameObject.transform.SetParent(base.transform, false);
		gameObject.transform.localScale = Vector3.one;
		gameObject.transform.localPosition = Vector3.zero;
		MapArc mapArc = gameObject.AddComponent<MapArc>();
		mapArc.Init(start, end, sprite, councilor, this.lockout);
		this.arcs[key] = mapArc;
	}

	// Token: 0x0600002C RID: 44 RVA: 0x00002DF8 File Offset: 0x00000FF8
	public void RemoveAllArc()
	{
		foreach (MapArc mapArc in this.arcs.Values)
		{
			if (mapArc != null)
			{
				global::UnityEngine.Object.Destroy(mapArc.gameObject);
			}
		}
		this.arcs.Clear();
	}

	// Token: 0x0600002D RID: 45 RVA: 0x00002E68 File Offset: 0x00001068
	public void RemoveArc(string lookup)
	{
		if (this.arcs.ContainsKey(lookup))
		{
			MapArc mapArc = this.arcs[lookup];
			if (mapArc != null)
			{
				global::UnityEngine.Object.Destroy(mapArc.gameObject);
			}
			this.arcs.Remove(lookup);
		}
	}

	// Token: 0x0600002E RID: 46 RVA: 0x00002EB4 File Offset: 0x000010B4
	public MapArc GetArc(string lookup)
	{
		MapArc mapArc;
		this.arcs.TryGetValue(lookup, out mapArc);
		return mapArc;
	}

	// Token: 0x0400000B RID: 11
	public static float mapTransitionTime = 0.5f;

	// Token: 0x0400000C RID: 12
	public float maxLift = 0.5f;

	// Token: 0x0400000D RID: 13
	public GameObject markerContainerPrefab;

	// Token: 0x0400000E RID: 14
	public GameObject nationContainerPrefab;

	// Token: 0x0400000F RID: 15
	protected Dictionary<string, NationController> nationControllerLookup;

	// Token: 0x04000010 RID: 16
	protected Dictionary<string, MapArc> arcs = new Dictionary<string, MapArc>();

	// Token: 0x04000011 RID: 17
	public Sprite airplaneTexture;

	// Token: 0x04000012 RID: 18
	public List<TooltipTrigger> regionTooltips;

	// Token: 0x04000013 RID: 19
	private bool regionTooltipsActive = true;

	// Token: 0x04000014 RID: 20
	public Transform mapTransform;

	// Token: 0x04000015 RID: 21
	public RegionClickHandler lastLeftClickedRegion;

	// Token: 0x04000016 RID: 22
	public RegionClickHandler lastRightClickedRegion;

	// Token: 0x04000017 RID: 23
	[Header("Debug Inspect")]
	[SerializeField]
	private bool _initializing;

	// Token: 0x04000018 RID: 24
	[SerializeField]
	private bool _isActive;

	// Token: 0x04000019 RID: 25
	[SerializeField]
	private SpaceObjectController spaceController;

	// Token: 0x0400001A RID: 26
	[SerializeField]
	private string outlineName;

	// Token: 0x0400001B RID: 27
	[SerializeField]
	private RegionOutlineCollection currentOutlines;

	// Token: 0x0400001C RID: 28
	public bool lockout;

	// Token: 0x0400001D RID: 29
	private bool lerpLockout;
}
