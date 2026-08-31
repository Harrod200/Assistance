using System;
using System.Collections.Generic;
using System.Linq;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020007B3 RID: 1971
	public class CombatInfo
	{
		// Token: 0x17000CC0 RID: 3264
		// (get) Token: 0x06004340 RID: 17216 RVA: 0x001B4194 File Offset: 0x001B2394
		public IEnumerable<TIFactionState> factions
		{
			get
			{
				IEnumerable<TIFactionState> enumerable = this.ships.Select<TISpaceShipState, TIFactionState>((TISpaceShipState x) => x.faction);
				TIHabState tihabState = this.hab;
				return (from x in enumerable.Append((tihabState != null) ? tihabState.faction : null)
					where x != null
					select x).Distinct<TIFactionState>();
			}
		}

		// Token: 0x17000CC1 RID: 3265
		// (get) Token: 0x06004341 RID: 17217 RVA: 0x001B420B File Offset: 0x001B240B
		public TIFactionState factionA
		{
			get
			{
				return this.factions.First<TIFactionState>();
			}
		}

		// Token: 0x17000CC2 RID: 3266
		// (get) Token: 0x06004342 RID: 17218 RVA: 0x001B4218 File Offset: 0x001B2418
		public TIFactionState factionB
		{
			get
			{
				return this.factions.First<TIFactionState>((TIFactionState x) => x != this.factionA);
			}
		}

		// Token: 0x17000CC3 RID: 3267
		// (get) Token: 0x06004343 RID: 17219 RVA: 0x001B4234 File Offset: 0x001B2434
		public IEnumerable<TISpaceFleetState> fleets
		{
			get
			{
				return (from x in this.ships
					select x.fleet into x
					where x != null
					select x).Distinct<TISpaceFleetState>();
			}
		}

		// Token: 0x0400281E RID: 10270
		public TISpaceCombatState Combat;

		// Token: 0x0400281F RID: 10271
		public IEnumerable<TISpaceShipState> ships;

		// Token: 0x04002820 RID: 10272
		public TIHabState hab;

		// Token: 0x04002821 RID: 10273
		public CombatRecord combatRecord;
	}
}
