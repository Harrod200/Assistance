using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.CSharp.RuntimeBinder;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000893 RID: 2195
	public class NationRelationsPaneController : MonoBehaviour
	{
		// Token: 0x17000EEE RID: 3822
		// (get) Token: 0x060052E4 RID: 21220 RVA: 0x0024BC3D File Offset: 0x00249E3D
		// (set) Token: 0x060052E5 RID: 21221 RVA: 0x0024BC45 File Offset: 0x00249E45
		public TIFactionState faction { get; private set; }

		// Token: 0x060052E6 RID: 21222 RVA: 0x0024BC4E File Offset: 0x00249E4E
		public void SetFactionAndNation(TIFactionState faction, TINationState nation, NationInfoController mainController)
		{
			this.faction = faction;
			this.nation = nation;
			this.unaligned = faction == null;
			this.controller = mainController;
			this.initialized = true;
		}

		// Token: 0x060052E7 RID: 21223 RVA: 0x0024BC79 File Offset: 0x00249E79
		public void OnEnable()
		{
			if (this.initialized)
			{
				this.controller.ResetProposedChanges();
				this.SetNationsList();
			}
		}

		// Token: 0x060052E8 RID: 21224 RVA: 0x0024BC94 File Offset: 0x00249E94
		public void SetNationsList()
		{
			TIFactionState activePlayer = GameControl.control.activePlayer;
			List<TINationState> list2;
			if (this.unaligned)
			{
				IEnumerable<TINationState> enumerable = from x in GameStateManager.AllExtantNations()
					where x.executiveFaction == null
					select x;
				List<TINationState> list;
				if (enumerable == null)
				{
					list = null;
				}
				else
				{
					list = enumerable.OrderByDescending<TINationState, double>((TINationState x) => x.GDP).ToList<TINationState>();
				}
				list2 = list;
			}
			else
			{
				IOrderedEnumerable<TINationState> orderedEnumerable = this.faction.executiveNations.OrderByDescending<TINationState, double>((TINationState x) => x.GDP);
				list2 = ((orderedEnumerable != null) ? orderedEnumerable.ToList<TINationState>() : null);
				list2.Remove(this.nation);
			}
			this.nationsList.SetListSize<NationRelationsListItemController>(list2.Count, false, false);
			int num = 0;
			using (IEnumerator<object> enumerator = this.nationsList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (NationRelationsPaneController.<>o__11.<>p__0 == null)
					{
						NationRelationsPaneController.<>o__11.<>p__0 = CallSite<Func<CallSite, object, NationRelationsListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(NationRelationsListItemController), typeof(NationRelationsPaneController)));
					}
					NationRelationsPaneController.<>o__11.<>p__0.Target(NationRelationsPaneController.<>o__11.<>p__0, enumerator.Current).SetListItem(this.nation, list2[num++], this);
				}
			}
		}

		// Token: 0x060052E9 RID: 21225 RVA: 0x0024BE00 File Offset: 0x0024A000
		public void UpdateNationRelationsList()
		{
			using (IEnumerator<object> enumerator = this.nationsList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (NationRelationsPaneController.<>o__12.<>p__0 == null)
					{
						NationRelationsPaneController.<>o__12.<>p__0 = CallSite<Func<CallSite, object, NationRelationsListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(NationRelationsListItemController), typeof(NationRelationsPaneController)));
					}
					NationRelationsPaneController.<>o__12.<>p__0.Target(NationRelationsPaneController.<>o__12.<>p__0, enumerator.Current).UpdateListItem();
				}
			}
		}

		// Token: 0x060052EA RID: 21226 RVA: 0x0024BE8C File Offset: 0x0024A08C
		public bool Allof(RelationChange change)
		{
			int num = 0;
			using (IEnumerator<object> enumerator = this.nationsList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (NationRelationsPaneController.<>o__13.<>p__0 == null)
					{
						NationRelationsPaneController.<>o__13.<>p__0 = CallSite<Func<CallSite, object, NationRelationsListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(NationRelationsListItemController), typeof(NationRelationsPaneController)));
					}
					NationRelationsListItemController nationRelationsListItemController = NationRelationsPaneController.<>o__13.<>p__0.Target(NationRelationsPaneController.<>o__13.<>p__0, enumerator.Current);
					switch (change)
					{
					case RelationChange.NormalToAlly:
						num += (nationRelationsListItemController.allyToggle.isOn ? 1 : 0);
						break;
					case RelationChange.AllyToNormal:
					case RelationChange.RivalToNormal:
						num += (nationRelationsListItemController.normalToggle.isOn ? 1 : 0);
						break;
					case RelationChange.NormalToRival:
						num += (nationRelationsListItemController.rivalToggle.isOn ? 1 : 0);
						break;
					}
				}
			}
			return num == this.nationsList.size;
		}

		// Token: 0x040037DD RID: 14301
		public ListManagerBase nationsList;

		// Token: 0x040037DF RID: 14303
		private TINationState nation;

		// Token: 0x040037E0 RID: 14304
		private bool unaligned;

		// Token: 0x040037E1 RID: 14305
		private bool initialized;

		// Token: 0x040037E2 RID: 14306
		[HideInInspector]
		public NationInfoController controller;
	}
}
