using System;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000772 RID: 1906
	public abstract class TIRegionEntityState : TIGameState
	{
		// Token: 0x17000A03 RID: 2563
		// (get) Token: 0x060039E9 RID: 14825 RVA: 0x0015637F File Offset: 0x0015457F
		// (set) Token: 0x060039EA RID: 14826 RVA: 0x00156387 File Offset: 0x00154587
		public TIRegionState region { get; protected set; }

		// Token: 0x17000A04 RID: 2564
		// (get) Token: 0x060039EB RID: 14827 RVA: 0x00156390 File Offset: 0x00154590
		public override TIRegionState ref_region
		{
			get
			{
				return this.region;
			}
		}

		// Token: 0x17000A05 RID: 2565
		// (get) Token: 0x060039EC RID: 14828 RVA: 0x00156398 File Offset: 0x00154598
		public override TINationState ref_nation
		{
			get
			{
				return this.region.nation;
			}
		}

		// Token: 0x17000A06 RID: 2566
		// (get) Token: 0x060039ED RID: 14829 RVA: 0x001563A5 File Offset: 0x001545A5
		public override bool hasMapObject
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000A07 RID: 2567
		// (get) Token: 0x060039EE RID: 14830 RVA: 0x001563A8 File Offset: 0x001545A8
		public override TISpaceBodyState ref_spaceBody
		{
			get
			{
				return this.region.spaceBody;
			}
		}

		// Token: 0x17000A08 RID: 2568
		// (get) Token: 0x060039EF RID: 14831 RVA: 0x001563B5 File Offset: 0x001545B5
		public override TINaturalSpaceObjectState ref_naturalSpaceObject
		{
			get
			{
				return this.region.spaceBody;
			}
		}

		// Token: 0x17000A09 RID: 2569
		// (get) Token: 0x060039F0 RID: 14832 RVA: 0x001563C2 File Offset: 0x001545C2
		public override TISpaceObjectState ref_spaceObject
		{
			get
			{
				return this.region.spaceBody;
			}
		}

		// Token: 0x17000A0A RID: 2570
		// (get) Token: 0x060039F1 RID: 14833 RVA: 0x001563CF File Offset: 0x001545CF
		public override bool hasEarthMapObject
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060039F2 RID: 14834
		public abstract bool Extant();

		// Token: 0x17000A0B RID: 2571
		// (get) Token: 0x060039F3 RID: 14835
		public abstract string descriptor { get; }

		// Token: 0x17000A0C RID: 2572
		// (get) Token: 0x060039F4 RID: 14836
		public abstract string description { get; }

		// Token: 0x060039F5 RID: 14837
		public abstract string GetIllustrationPath(TIFactionState faction);

		// Token: 0x060039F6 RID: 14838 RVA: 0x001563D2 File Offset: 0x001545D2
		public virtual Sprite GetIcon(TIFactionState faction)
		{
			return GameControl.assetLoader.LoadAsset<Sprite>(this.GetIconResourcePath(faction));
		}

		// Token: 0x060039F7 RID: 14839
		public abstract string GetIconResourcePath(TIFactionState faction);

		// Token: 0x060039F8 RID: 14840 RVA: 0x001563E5 File Offset: 0x001545E5
		public void SetRegionEntityDataDirty()
		{
			GameControl.eventManager.TriggerEvent(new RegionEntityUpdated(this), null, new object[] { this, this.region });
		}

		// Token: 0x04002573 RID: 9587
		[SerializeField]
		protected bool gameStateSubjectCreated;
	}
}
