using System;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020007A2 RID: 1954
	public class TIHabDistrictState : TIGameState
	{
		// Token: 0x17000B33 RID: 2867
		// (get) Token: 0x06003ED0 RID: 16080 RVA: 0x00196345 File Offset: 0x00194545
		// (set) Token: 0x06003ED1 RID: 16081 RVA: 0x0019634D File Offset: 0x0019454D
		public TIHabState hab { get; private set; }

		// Token: 0x17000B34 RID: 2868
		// (get) Token: 0x06003ED2 RID: 16082 RVA: 0x00196356 File Offset: 0x00194556
		// (set) Token: 0x06003ED3 RID: 16083 RVA: 0x0019635E File Offset: 0x0019455E
		public TIFactionState faction { get; private set; }

		// Token: 0x17000B35 RID: 2869
		// (get) Token: 0x06003ED4 RID: 16084 RVA: 0x00196367 File Offset: 0x00194567
		// (set) Token: 0x06003ED5 RID: 16085 RVA: 0x0019636F File Offset: 0x0019456F
		public TIDateTime defendExpiration { get; private set; }

		// Token: 0x17000B36 RID: 2870
		// (get) Token: 0x06003ED6 RID: 16086 RVA: 0x00196378 File Offset: 0x00194578
		public override TIFactionState ref_faction
		{
			get
			{
				return this.faction;
			}
		}

		// Token: 0x17000B37 RID: 2871
		// (get) Token: 0x06003ED7 RID: 16087 RVA: 0x00196380 File Offset: 0x00194580
		public override TIHabState ref_hab
		{
			get
			{
				return this.hab;
			}
		}

		// Token: 0x17000B38 RID: 2872
		// (get) Token: 0x06003ED8 RID: 16088 RVA: 0x00196388 File Offset: 0x00194588
		public override TIOrbitState ref_orbit
		{
			get
			{
				return this.hab.ref_orbit;
			}
		}

		// Token: 0x17000B39 RID: 2873
		// (get) Token: 0x06003ED9 RID: 16089 RVA: 0x00196395 File Offset: 0x00194595
		public override TISpaceBodyState ref_spaceBody
		{
			get
			{
				return this.hab.ref_spaceBody;
			}
		}

		// Token: 0x17000B3A RID: 2874
		// (get) Token: 0x06003EDA RID: 16090 RVA: 0x001963A2 File Offset: 0x001945A2
		public override TISpaceObjectState ref_spaceObject
		{
			get
			{
				if (!this.hab.IsBase)
				{
					return this.hab;
				}
				return this.hab.ref_spaceBody;
			}
		}

		// Token: 0x17000B3B RID: 2875
		// (get) Token: 0x06003EDB RID: 16091 RVA: 0x001963C3 File Offset: 0x001945C3
		public override TINaturalSpaceObjectState ref_naturalSpaceObject
		{
			get
			{
				if (!this.hab.IsBase)
				{
					return this.hab.barycenter;
				}
				return this.hab.ref_spaceBody;
			}
		}

		// Token: 0x17000B3C RID: 2876
		// (get) Token: 0x06003EDC RID: 16092 RVA: 0x001963E9 File Offset: 0x001945E9
		public override TISpaceAssetState ref_spaceAsset
		{
			get
			{
				return this.hab;
			}
		}

		// Token: 0x17000B3D RID: 2877
		// (get) Token: 0x06003EDD RID: 16093 RVA: 0x001963F1 File Offset: 0x001945F1
		public override bool hasMapObject
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000B3E RID: 2878
		// (get) Token: 0x06003EDE RID: 16094 RVA: 0x001963F4 File Offset: 0x001945F4
		public override bool inSpace
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0400271C RID: 10012
		public bool defended;

		// Token: 0x0400271E RID: 10014
		[SerializeField]
		private bool gameStateSubjectCreated;

		// Token: 0x0400271F RID: 10015
		[SerializeField]
		private bool createdFromTemplate;
	}
}
