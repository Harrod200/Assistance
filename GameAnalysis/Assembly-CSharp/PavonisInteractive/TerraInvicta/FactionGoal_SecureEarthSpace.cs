using System;
using System.Collections.Generic;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200074D RID: 1869
	public class FactionGoal_SecureEarthSpace : FactionGoal_DefendWithFleet
	{
		// Token: 0x06002FFE RID: 12286 RVA: 0x001055D5 File Offset: 0x001037D5
		public FactionGoal_SecureEarthSpace()
		{
		}

		// Token: 0x06002FFF RID: 12287 RVA: 0x001055DD File Offset: 0x001037DD
		public FactionGoal_SecureEarthSpace(TIFactionState faction, int importance)
			: base(faction, importance, GameStateManager.Earth(), "")
		{
		}

		// Token: 0x06003000 RID: 12288 RVA: 0x001055F1 File Offset: 0x001037F1
		public static FactionGoal_SecureEarthSpace CreateGoal(FactionGoal_SecureEarthSpace p)
		{
			FactionGoal_SecureEarthSpace factionGoal_SecureEarthSpace = GameStateManager.CreateNewGameState<FactionGoal_SecureEarthSpace>();
			factionGoal_SecureEarthSpace.defendTarget = GameStateManager.Earth();
			return factionGoal_SecureEarthSpace;
		}

		// Token: 0x06003001 RID: 12289 RVA: 0x00105603 File Offset: 0x00103803
		public override GoalType GetGoalType()
		{
			return GoalType.SecureEarthSpace;
		}

		// Token: 0x06003002 RID: 12290 RVA: 0x00105607 File Offset: 0x00103807
		public override bool ShouldDiscardGoal()
		{
			return false;
		}

		// Token: 0x06003003 RID: 12291 RVA: 0x0010560A File Offset: 0x0010380A
		public override bool IsDuplicate(TIFactionGoalState testGoal, TIGameState testTarget)
		{
			return testGoal is FactionGoal_SecureEarthSpace;
		}

		// Token: 0x06003004 RID: 12292 RVA: 0x00105615 File Offset: 0x00103815
		public override void ChangeTarget(TIGameState newTarget)
		{
			throw new InvalidOperationException("Tried to change the target of a FactionGoal_SecureEarthSpace goal.");
		}

		// Token: 0x06003005 RID: 12293 RVA: 0x00105621 File Offset: 0x00103821
		public override float ComputeDesiredFleetCombatValue()
		{
			return (FactionGoal_Fleet.ComputeBaselineFleetCombatValue(this.faction, this.target()) * 1f + 1000f) * TemplateManager.global.AI_AlienEarthFleetSizeModifier();
		}

		// Token: 0x06003006 RID: 12294 RVA: 0x0010564C File Offset: 0x0010384C
		public override float GetMaximumFleetCombatValueRatio()
		{
			float num = 1.25f;
			if (this.faction.enemyTotalWarFactions.Count > 0)
			{
				num = 2.25f;
			}
			else if (this.faction.enemyWarFactions.Count > 0)
			{
				num = 1.5f;
			}
			float num2 = 1.25f;
			num = Mathf.Max(num, num2);
			return (num - num2) * TemplateManager.global.AI_AlienEarthFleetExcessModifier() + num2;
		}

		// Token: 0x06003007 RID: 12295 RVA: 0x001056B1 File Offset: 0x001038B1
		public override Dictionary<ShipRole, float> GetSecondaryShipRoles()
		{
			return FactionGoal_SecureEarthSpace.preferredRoles;
		}

		// Token: 0x04002254 RID: 8788
		private static readonly Dictionary<ShipRole, float> preferredRoles = new Dictionary<ShipRole, float>
		{
			{
				ShipRole.ML_Standoff,
				0.5f
			},
			{
				ShipRole.MM_SpaceSuperiority,
				0.5f
			},
			{
				ShipRole.MS_Strike,
				0.5f
			},
			{
				ShipRole.LL_Intruder,
				1f
			},
			{
				ShipRole.LM_Interdictor,
				1f
			},
			{
				ShipRole.LS_Penetrator,
				1f
			},
			{
				ShipRole.LM_Protector,
				0.75f
			}
		};
	}
}
