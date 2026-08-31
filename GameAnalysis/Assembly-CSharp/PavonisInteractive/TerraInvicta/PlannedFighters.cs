using System;
using System.Collections.Generic;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020007B5 RID: 1973
	public class PlannedFighters
	{
		// Token: 0x06004346 RID: 17222 RVA: 0x001B42AA File Offset: 0x001B24AA
		public PlannedFighters(TISpaceShipTemplate fighter, int count)
		{
			this.SetDesign(fighter);
			this.SetCount(count);
		}

		// Token: 0x06004347 RID: 17223 RVA: 0x001B42CB File Offset: 0x001B24CB
		public PlannedFighters()
		{
		}

		// Token: 0x06004348 RID: 17224 RVA: 0x001B42DE File Offset: 0x001B24DE
		public void SetDesign(TISpaceShipTemplate fighter)
		{
			this.fighter = fighter;
		}

		// Token: 0x06004349 RID: 17225 RVA: 0x001B42E7 File Offset: 0x001B24E7
		public void SetCount(int count)
		{
			this.count = count;
		}

		// Token: 0x17000CC4 RID: 3268
		// (get) Token: 0x0600434A RID: 17226 RVA: 0x001B42F0 File Offset: 0x001B24F0
		public float singleFighterBoostCost
		{
			get
			{
				return this.fighter.wetMass_tons * TemplateManager.global.spaceResourceToTons;
			}
		}

		// Token: 0x17000CC5 RID: 3269
		// (get) Token: 0x0600434B RID: 17227 RVA: 0x001B4308 File Offset: 0x001B2508
		public float boostCost
		{
			get
			{
				return (float)this.count * this.singleFighterBoostCost;
			}
		}

		// Token: 0x0600434C RID: 17228 RVA: 0x001B4318 File Offset: 0x001B2518
		public void AddFighterState(TISpaceShipState fighter)
		{
			this.fighterStates.AddUnique(fighter);
		}

		// Token: 0x04002829 RID: 10281
		public TINationState nation;

		// Token: 0x0400282A RID: 10282
		public TISpaceShipTemplate fighter;

		// Token: 0x0400282B RID: 10283
		public int count;

		// Token: 0x0400282C RID: 10284
		public List<TISpaceShipState> fighterStates = new List<TISpaceShipState>();
	}
}
