using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.CSharp.RuntimeBinder;
using PavonisInteractive.TerraInvicta.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020008B6 RID: 2230
	public class TargetSelectionTool : MonoBehaviour
	{
		// Token: 0x17000F04 RID: 3844
		// (get) Token: 0x060054FB RID: 21755 RVA: 0x00269DD7 File Offset: 0x00267FD7
		// (set) Token: 0x060054FC RID: 21756 RVA: 0x00269DE0 File Offset: 0x00267FE0
		public IEnumerable<TIGameState> Targets
		{
			get
			{
				return this.targets;
			}
			set
			{
				if (this.targets.Count<TIGameState>() == value.Count<TIGameState>() && this.targets.All<TIGameState>((TIGameState x) => value.Contains(x)))
				{
					return;
				}
				this.targets = value;
				this.UpdateListUI();
			}
		}

		// Token: 0x17000F05 RID: 3845
		// (get) Token: 0x060054FD RID: 21757 RVA: 0x00269E3E File Offset: 0x0026803E
		// (set) Token: 0x060054FE RID: 21758 RVA: 0x00269E48 File Offset: 0x00268048
		public TIGameState Filter
		{
			get
			{
				return this.filter;
			}
			set
			{
				if (this.filter == value)
				{
					return;
				}
				this.filter = value;
				if (this.onFilterSelected != null)
				{
					this.onFilterSelected(this.filter);
				}
				if (this.onTargetSelected != null && this.newTargetOnFilterChange && this.FilteredTargets.Count<TIGameState>() > 0)
				{
					this.onTargetSelected(this.GetArbitraryTarget());
				}
				this.UpdateListUI();
			}
		}

		// Token: 0x17000F06 RID: 3846
		// (get) Token: 0x060054FF RID: 21759 RVA: 0x00269EB9 File Offset: 0x002680B9
		// (set) Token: 0x06005500 RID: 21760 RVA: 0x00269EC1 File Offset: 0x002680C1
		public IOperation Operation { get; set; }

		// Token: 0x17000F07 RID: 3847
		// (get) Token: 0x06005501 RID: 21761 RVA: 0x00269ECA File Offset: 0x002680CA
		public bool IsInOrbitSelectionMode
		{
			get
			{
				return this.Targets.Any<TIGameState>((TIGameState x) => x is TIOrbitState);
			}
		}

		// Token: 0x17000F08 RID: 3848
		// (get) Token: 0x06005502 RID: 21762 RVA: 0x00269EF6 File Offset: 0x002680F6
		public bool IsinHabSiteSelectionMode
		{
			get
			{
				return this.Targets.Any<TIGameState>((TIGameState x) => x is TIHabSiteState);
			}
		}

		// Token: 0x17000F09 RID: 3849
		// (get) Token: 0x06005503 RID: 21763 RVA: 0x00269F22 File Offset: 0x00268122
		public bool IsInNavigatorMode
		{
			get
			{
				return this.navigatorListObject.activeSelf;
			}
		}

		// Token: 0x06005504 RID: 21764 RVA: 0x00269F2F File Offset: 0x0026812F
		private void Awake()
		{
			this.InitializeNavigator();
		}

		// Token: 0x17000F0A RID: 3850
		// (get) Token: 0x06005505 RID: 21765 RVA: 0x00269F38 File Offset: 0x00268138
		public IEnumerable<TIGameState> FilteredTargets
		{
			get
			{
				if (this.Filter == null)
				{
					return this.Targets;
				}
				IEnumerable<TIGameState> enumerable = this.Targets;
				if (this.Filter.isNaturalSpaceObjectState)
				{
					if (this.IsInNavigatorMode)
					{
						enumerable = enumerable.Where<TIGameState>(delegate(TIGameState x)
						{
							if (!TIGameState.Valid(x) || !(x.ref_naturalSpaceObject == this.Filter))
							{
								TISpaceFleetState tispaceFleetState = x as TISpaceFleetState;
								return tispaceFleetState != null && TIGameState.Valid(x) && tispaceFleetState.transferAssigned && tispaceFleetState.GetSphereOfInfluence(true) == this.Filter;
							}
							return true;
						});
					}
					else
					{
						enumerable = enumerable.Where<TIGameState>((TIGameState x) => x.ref_naturalSpaceObject == this.Filter);
					}
				}
				return enumerable;
			}
		}

		// Token: 0x06005506 RID: 21766 RVA: 0x00269FA0 File Offset: 0x002681A0
		public void UpdateListUI()
		{
			if (!base.gameObject.activeSelf || this.Filter == null)
			{
				return;
			}
			if (this.Operation != null)
			{
				OperationTargetingUIType operationTargetingUIType = this.Operation.GetOperationTargeting().UIType();
				if (operationTargetingUIType == OperationTargetingUIType.Transfer)
				{
					this.navigatorListObject.SetActive(true);
				}
				else if (operationTargetingUIType == OperationTargetingUIType.TwoStage)
				{
					this.navigatorListObject.SetActive(false);
				}
			}
			List<TIGameState> list = this.FilteredTargets.ToList<TIGameState>();
			if (this.inIntelScreen)
			{
				list.RemoveAll((TIGameState x) => x.ref_spaceAsset != null && !x.ref_spaceAsset.VisibleToFaction(GameControl.control.activePlayer));
			}
			if (this.IsInNavigatorMode)
			{
				TINaturalSpaceObjectState ref_naturalSpaceObject = this.Filter.ref_naturalSpaceObject;
				if (ref_naturalSpaceObject != null)
				{
					list.AddRange(TINaturalSpaceObjectState.GetFilteredSolarSystemGroupObjects(ref_naturalSpaceObject.ref_spaceBody, false));
				}
			}
			this.targetListManager.SetListSize<OperationTargetListItemController>(list.Count, false, false);
			if (list.Count > 0)
			{
				int num = 0;
				using (IEnumerator<object> enumerator = this.targetListManager.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (TargetSelectionTool.<>o__30.<>p__0 == null)
						{
							TargetSelectionTool.<>o__30.<>p__0 = CallSite<Func<CallSite, object, OperationTargetListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(OperationTargetListItemController), typeof(TargetSelectionTool)));
						}
						TargetSelectionTool.<>o__30.<>p__0.Target(TargetSelectionTool.<>o__30.<>p__0, enumerator.Current).SetListItem(this.Operation, list[num++], this.inIntelScreen);
					}
				}
				LayoutElement component = this.targetScrollView.transform.GetComponent<LayoutElement>();
				component.minHeight = Mathf.Clamp((float)(num * 25), 25f, 25f + component.flexibleHeight);
				RectTransform component2 = this.targetScrollViewContentObject.GetComponent<RectTransform>();
				component2.sizeDelta = new Vector2(component2.sizeDelta.x, (float)(num * 25));
			}
			this.UpdateLabels();
		}

		// Token: 0x06005507 RID: 21767 RVA: 0x0026A188 File Offset: 0x00268388
		public void Open(IEnumerable<TIGameState> targets, TIGameState filter, IOperation operation = null)
		{
			base.gameObject.SetActive(false);
			this.Targets = targets;
			this.Filter = filter;
			this.Operation = operation;
			base.gameObject.SetActive(true);
			base.transform.localPosition = new Vector3(base.transform.localPosition.x, 257f, base.transform.localPosition.z);
			this.UpdateListUI();
		}

		// Token: 0x06005508 RID: 21768 RVA: 0x0026A1FD File Offset: 0x002683FD
		public void Open()
		{
			base.gameObject.SetActive(true);
			this.UpdateListUI();
		}

		// Token: 0x06005509 RID: 21769 RVA: 0x0026A211 File Offset: 0x00268411
		public void Close()
		{
			base.gameObject.SetActive(false);
		}

		// Token: 0x0600550A RID: 21770 RVA: 0x0026A21F File Offset: 0x0026841F
		public TIGameState GetArbitraryTarget()
		{
			return this.FilteredTargets.First<TIGameState>();
		}

		// Token: 0x0600550B RID: 21771 RVA: 0x0026A22C File Offset: 0x0026842C
		public void SetTargetsToAllOrbitsAndSpaceAssets()
		{
			this.Targets = (from x in GameStateManager.IterateByClass<TIHabState>(false)
				where x.habType != HabType.Base
				select x).AsEnumerable<TIGameState>().Union<TIGameState>(GameStateManager.IterateByClass<TISpaceFleetState>(false).AsEnumerable<TIGameState>()).Union<TIGameState>(GameStateManager.IterateByClass<TIOrbitState>(false).AsEnumerable<TIGameState>());
		}

		// Token: 0x0600550C RID: 21772 RVA: 0x0026A28E File Offset: 0x0026848E
		public void UpdateLabels()
		{
			if (this.GetHeaderString != null)
			{
				this.targetDetailPanelHeader.text = this.GetHeaderString(this);
			}
		}

		// Token: 0x0600550D RID: 21773 RVA: 0x0026A2AF File Offset: 0x002684AF
		public void OnElementClicked(TIGameState gameState)
		{
			if (gameState.isNaturalSpaceObjectState)
			{
				this.Filter = gameState;
				return;
			}
			if (this.onTargetSelected != null)
			{
				this.onTargetSelected(gameState);
			}
		}

		// Token: 0x0600550E RID: 21774 RVA: 0x0026A2D8 File Offset: 0x002684D8
		public void InitializeNavigator()
		{
			if (this.navigatorBodies.Count == 0)
			{
				foreach (string text in TargetSelectionTool.primaryNavigatorBodyTemplateNames)
				{
					TINaturalSpaceObjectState tinaturalSpaceObjectState = GameStateManager.FindByTemplate<TINaturalSpaceObjectState>(text, true);
					if (tinaturalSpaceObjectState != null)
					{
						this.navigatorBodies.Add(tinaturalSpaceObjectState);
					}
				}
				this.navigatorButtonListManager.SetListSize<NavPanelListItemController>(this.navigatorBodies.Count, false, false);
				int num = 0;
				using (IEnumerator<object> enumerator2 = this.navigatorButtonListManager.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						if (TargetSelectionTool.<>o__43.<>p__0 == null)
						{
							TargetSelectionTool.<>o__43.<>p__0 = CallSite<Func<CallSite, object, NavPanelListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(NavPanelListItemController), typeof(TargetSelectionTool)));
						}
						TargetSelectionTool.<>o__43.<>p__0.Target(TargetSelectionTool.<>o__43.<>p__0, enumerator2.Current).SetListItem(this.navigatorBodies[num++]);
					}
				}
			}
		}

		// Token: 0x0600550F RID: 21775 RVA: 0x0026A3F4 File Offset: 0x002685F4
		public void OnNavigatorListButtonClicked(TINaturalSpaceObjectState naturalSpaceObject)
		{
			this.Filter = naturalSpaceObject;
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_ListLineSelect", false, false);
		}

		// Token: 0x04003B33 RID: 15155
		public bool inIntelScreen;

		// Token: 0x04003B34 RID: 15156
		public TMP_Text targetDetailPanelHeader;

		// Token: 0x04003B35 RID: 15157
		public Button targetDetailCloseButton;

		// Token: 0x04003B36 RID: 15158
		public ListManagerBase targetListManager;

		// Token: 0x04003B37 RID: 15159
		public ScrollRect targetScrollView;

		// Token: 0x04003B38 RID: 15160
		public GameObject targetScrollViewContentObject;

		// Token: 0x04003B39 RID: 15161
		public Image highlightImage;

		// Token: 0x04003B3A RID: 15162
		public bool newTargetOnFilterChange;

		// Token: 0x04003B3B RID: 15163
		private IEnumerable<TIGameState> targets = Enumerable.Empty<TIGameState>();

		// Token: 0x04003B3C RID: 15164
		private TIGameState filter;

		// Token: 0x04003B3E RID: 15166
		public Func<TargetSelectionTool, string> GetHeaderString;

		// Token: 0x04003B3F RID: 15167
		public TargetSelectionTool.OnTargetSelected onTargetSelected;

		// Token: 0x04003B40 RID: 15168
		public GameObject navigatorListObject;

		// Token: 0x04003B41 RID: 15169
		public ListManagerBase navigatorButtonListManager;

		// Token: 0x04003B42 RID: 15170
		private List<TINaturalSpaceObjectState> navigatorBodies = new List<TINaturalSpaceObjectState>();

		// Token: 0x04003B43 RID: 15171
		public TargetSelectionTool.OnFilterSelected onFilterSelected;

		// Token: 0x04003B44 RID: 15172
		public static readonly List<string> primaryNavigatorBodyTemplateNames = new List<string>
		{
			"Sol", "Mercury", "Venus", "Earth", "Luna", "433Eros", "Mars", "4Vesta", "Ceres", "10Hygiea",
			"Jupiter", "Saturn", "10199Chariklo", "Uranus", "Neptune", "Pluto"
		};

		// Token: 0x0200117D RID: 4477
		// (Invoke) Token: 0x060087C2 RID: 34754
		public delegate void OnTargetSelected(TIGameState gameState);

		// Token: 0x0200117E RID: 4478
		// (Invoke) Token: 0x060087C6 RID: 34758
		public delegate void OnFilterSelected(TIGameState gameState);
	}
}
