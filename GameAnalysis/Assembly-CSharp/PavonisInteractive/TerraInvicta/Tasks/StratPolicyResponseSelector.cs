using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.Tasks
{
	// Token: 0x02000953 RID: 2387
	public class StratPolicyResponseSelector : IPolicyResponseSelectionStrategy
	{
		// Token: 0x06005AEC RID: 23276 RVA: 0x002BBF40 File Offset: 0x002BA140
		public static float ChanceFederation(TINationState proposingNation, TINationState respondingNation)
		{
			TIFactionState executiveFaction = proposingNation.executiveFaction;
			TIFactionState executiveFaction2 = respondingNation.executiveFaction;
			if (executiveFaction != null && executiveFaction2 != null && (executiveFaction == executiveFaction2 || (executiveFaction.IsAlienFaction && executiveFaction2.IsAlienProxy)))
			{
				return 1f;
			}
			if (executiveFaction2 != null)
			{
				return 0f;
			}
			List<TINationState> enemies = proposingNation.enemies;
			List<TINationState> enemies2 = respondingNation.enemies;
			float respondingNationMilitaryStrength = respondingNation.militaryStrength;
			return Mathf.Clamp((0f + 2f * respondingNation.unrest - 2f * proposingNation.unrest + 1f * proposingNation.cohesion - 1f * respondingNation.cohesion - 2f * Mathf.Abs(proposingNation.democracy - respondingNation.democracy) + 0.0005f * (proposingNation.perCapitaGDP - respondingNation.perCapitaGDP) + 0.1f * (proposingNation.militaryStrength - respondingNationMilitaryStrength) + (float)(2 * enemies2.Where<TINationState>((TINationState x) => x.militaryStrength > respondingNationMilitaryStrength).Count<TINationState>()) + (float)(2 * respondingNation.wars.Where<TINationState>((TINationState x) => x.militaryStrength > respondingNationMilitaryStrength).Count<TINationState>()) + 1.5f * (float)(proposingNation.numStandardArmies - respondingNation.numStandardArmies) - (float)(10 * enemies.Intersect<TINationState>(respondingNation.allies).Count<TINationState>()) - (float)(10 * proposingNation.allies.Intersect<TINationState>(enemies2).Count<TINationState>()) - 5f * TINationState.GetIdeologicalDistance((executiveFaction != null) ? executiveFaction.ideology.ideologyCoordinates : Vector3.zero, (executiveFaction2 != null) ? executiveFaction2.ideology.ideologyCoordinates : Vector3.zero) - (float)(5 * (proposingNation.IsAdjacentToNation(respondingNation, false) ? 0 : (-5)))) / 100f, 0f, 1f);
		}

		// Token: 0x06005AED RID: 23277 RVA: 0x002BC120 File Offset: 0x002BA320
		public static float ChanceUnification(TINationState proposingNation, TINationState respondingNation)
		{
			TIFactionState executiveFaction = proposingNation.executiveFaction;
			TIFactionState executiveFaction2 = respondingNation.executiveFaction;
			if (executiveFaction != null && executiveFaction2 != null && (executiveFaction == executiveFaction2 || (executiveFaction.IsAlienFaction && executiveFaction2.proAlien)))
			{
				return 1f;
			}
			if (executiveFaction2 != null)
			{
				return 0f;
			}
			if (proposingNation.breakawayParent == respondingNation)
			{
				return 1f;
			}
			List<TINationState> enemies = proposingNation.enemies;
			List<TINationState> enemies2 = respondingNation.enemies;
			float militaryStrength = respondingNation.militaryStrength;
			return Mathf.Clamp((0f + 2f * respondingNation.unrest - 2f * proposingNation.unrest + 1f * proposingNation.cohesion - 1f * respondingNation.cohesion - 2f * Mathf.Abs(proposingNation.democracy - respondingNation.democracy) + (float)(3 * proposingNation.numControlPoints - respondingNation.numControlPoints) + 0.0005f * (proposingNation.perCapitaGDP - respondingNation.perCapitaGDP) + 0.1f * (proposingNation.militaryStrength - respondingNation.militaryStrength) + (float)(2 * enemies2.Where<TINationState>((TINationState x) => x.militaryStrength > respondingNation.militaryStrength).Count<TINationState>()) + (float)(2 * respondingNation.wars.Where<TINationState>((TINationState x) => x.militaryStrength > respondingNation.militaryStrength).Count<TINationState>()) + 1.5f * (float)(proposingNation.numStandardArmies - respondingNation.numStandardArmies) - (float)(10 * enemies.Intersect<TINationState>(respondingNation.allies).Count<TINationState>()) - (float)(10 * proposingNation.allies.Intersect<TINationState>(enemies2).Count<TINationState>()) - 5f * TINationState.GetIdeologicalDistance((executiveFaction != null) ? executiveFaction.ideology.ideologyCoordinates : Vector3.zero, (executiveFaction2 != null) ? executiveFaction2.ideology.ideologyCoordinates : Vector3.zero) - (float)(5 * (proposingNation.IsAdjacentToNation(respondingNation, false) ? 0 : (-5)))) / 100f, 0f, 1f);
		}

		// Token: 0x06005AEE RID: 23278 RVA: 0x002BC36C File Offset: 0x002BA56C
		public static float ChanceEndWar(TINationState proposingNation, TIWarState war)
		{
			TIFactionState executiveFaction = proposingNation.executiveFaction;
			TINationState tinationState = war.EnemyWarLeader(proposingNation, false);
			TIFactionState respondingFaction = tinationState.executiveFaction;
			if (tinationState.alienNation)
			{
				respondingFaction = GameStateManager.AlienFaction();
				if (tinationState.regions.Count == 0)
				{
					return 0f;
				}
			}
			if (executiveFaction != null && respondingFaction != null && (executiveFaction == respondingFaction || (executiveFaction.IsAlienFaction && respondingFaction.veryProAlien) || (executiveFaction.IsAlienProxy && respondingFaction.IsAlienFaction) || AIEvaluators.AlwaysEndConflict(tinationState, proposingNation) || tinationState.numStandardArmies == 0))
			{
				return 1f;
			}
			if (war.Alliance(tinationState).Any<TINationState>((TINationState x) => AIEvaluators.NuclearDeterred(respondingFaction, x, proposingNation, 1, war)))
			{
				return 1f;
			}
			if (war.stalemate && !proposingNation.alienNation && !tinationState.alienNation)
			{
				return 1f;
			}
			if (war.attackingAlliance.Contains(tinationState) && war.ActiveOccupations(tinationState, true, true).Count > 0 && war.ActiveOccupations(proposingNation, true, false).Count == 0)
			{
				return 0f;
			}
			if (tinationState.AssessOverallWarStatus() == tinationState.historyWarStatus[6])
			{
				return 0.5f;
			}
			return Mathf.Clamp((0f + 10f * tinationState.unrest - 1f * proposingNation.cohesion - 3f * TINationState.GetIdeologicalDistance((executiveFaction != null) ? executiveFaction.ideology.ideologyCoordinates : Vector3.zero, (respondingFaction != null) ? respondingFaction.ideology.ideologyCoordinates : Vector3.zero) + (float)(5 * (proposingNation.numControlPoints_unclamped - tinationState.numControlPoints_unclamped)) - tinationState.WinningWarBy(proposingNation)) / 100f, 0f, 1f);
		}

		// Token: 0x06005AEF RID: 23279 RVA: 0x002BC5A4 File Offset: 0x002BA7A4
		public static float ChanceFormAlliance(TINationState proposingNation, TINationState respondingNation)
		{
			TIFactionState executiveFaction = proposingNation.executiveFaction;
			TIFactionState executiveFaction2 = respondingNation.executiveFaction;
			if (executiveFaction != null && executiveFaction2 != null)
			{
				int num = executiveFaction.AI_WarWithFactionImportance(executiveFaction2);
				int num2 = executiveFaction2.AI_WarWithFactionImportance(executiveFaction);
				if (num >= 20 || num2 >= 20)
				{
					return 0f;
				}
				if (executiveFaction == executiveFaction2 || (executiveFaction.IsAlienFaction && executiveFaction2.veryProAlien) || (executiveFaction.veryProAlien && executiveFaction2.IsAlienFaction))
				{
					return 1f;
				}
				if (executiveFaction.IsAlienFaction && executiveFaction2.antiAlien)
				{
					return 0f;
				}
				if (executiveFaction.veryAntiAlien && executiveFaction2.veryProAlien)
				{
					return 0f;
				}
				if (executiveFaction.veryProAlien && executiveFaction2.veryAntiAlien)
				{
					return 0f;
				}
				if (executiveFaction.extremist && executiveFaction.veryProAlien && executiveFaction2.antiAlien)
				{
					return 0f;
				}
				if (executiveFaction.extremist && executiveFaction.veryAntiAlien && executiveFaction2.proAlien)
				{
					return 0f;
				}
				if (executiveFaction.antiAlien && executiveFaction2.veryProAlien && executiveFaction2.extremist)
				{
					return 0f;
				}
				if (executiveFaction.proAlien && executiveFaction2.veryAntiAlien && executiveFaction2.extremist)
				{
					return 0f;
				}
				if (proposingNation.wars.Count == 0 && respondingNation.wars.Count == 0 && (num > 0 || num2 > 0))
				{
					return 0f;
				}
			}
			bool flag = proposingNation.IsAdjacentToNation(respondingNation, false);
			if ((executiveFaction2 == null || executiveFaction == null || (!executiveFaction.veryProAlien && !executiveFaction2.veryProAlien)) && (respondingNation.wars.Contains(GameStateManager.AlienNation()) || (respondingNation.MegaFaunaArmiesOnSoil().Any<TIArmyState>() && respondingNation.armies.Count == 0)) && ((flag && proposingNation.armies.Count > 0) || proposingNation.numNavies > 0))
			{
				return 1f;
			}
			List<TINationState> enemies = proposingNation.enemies;
			List<TINationState> respondingNationEnemies = respondingNation.enemies;
			float respondingNationMilitaryStrength = respondingNation.militaryStrength;
			return Mathf.Clamp((0f + (float)(10 * proposingNation.allies.Intersect<TINationState>(respondingNation.allies).Count<TINationState>()) + (float)(3 * enemies.Intersect<TINationState>(respondingNationEnemies).Count<TINationState>()) + (float)(25 * enemies.Intersect<TINationState>(respondingNationEnemies.Where<TINationState>((TINationState enemy) => enemy.militaryStrength > respondingNationMilitaryStrength)).Count<TINationState>()) - (float)(10 * enemies.Intersect<TINationState>(from otherNation in GameStateManager.AllNations()
				where otherNation.extant && !respondingNationEnemies.Contains(otherNation) && otherNation.militaryStrength > respondingNationMilitaryStrength
				select otherNation).Count<TINationState>()) - (float)(5 * enemies.Intersect<TINationState>(respondingNation.allies).Count<TINationState>()) - (float)(5 * proposingNation.allies.Intersect<TINationState>(respondingNationEnemies).Count<TINationState>()) + 0.5f * (proposingNation.militaryStrength - respondingNationMilitaryStrength) + (float)(3 * (proposingNation.numControlPoints - respondingNation.numControlPoints)) - 10f * TINationState.GetIdeologicalDistance((executiveFaction != null) ? executiveFaction.ideology.ideologyCoordinates : Vector3.zero, (executiveFaction2 != null) ? executiveFaction2.ideology.ideologyCoordinates : Vector3.zero) + (float)(3 * (flag ? 5 : (-5))) - 5f * Mathf.Abs(proposingNation.democracy - respondingNation.democracy)) / 100f, 0f, 1f);
		}

		// Token: 0x06005AF0 RID: 23280 RVA: 0x002BC904 File Offset: 0x002BAB04
		public static float ChanceEndRivalry(TINationState proposingNation, TINationState respondingNation)
		{
			TIFactionState executiveFaction = proposingNation.executiveFaction;
			TIFactionState executiveFaction2 = respondingNation.executiveFaction;
			if (executiveFaction != null && executiveFaction2 != null && AIEvaluators.AlwaysEndConflict(respondingNation, proposingNation))
			{
				return 1f;
			}
			List<TINationState> enemies = proposingNation.enemies;
			List<TINationState> enemies2 = respondingNation.enemies;
			float respondingNationMilitaryStrength = respondingNation.militaryStrength;
			return Mathf.Clamp((0f + (float)(10 * proposingNation.allies.Intersect<TINationState>(respondingNation.allies).Count<TINationState>()) + (float)(3 * enemies.Intersect<TINationState>(enemies2).Count<TINationState>()) + (float)(10 * enemies.Intersect<TINationState>(enemies2.Where<TINationState>((TINationState enemy) => enemy.militaryStrength > respondingNationMilitaryStrength)).Count<TINationState>()) - (float)(10 * respondingNation.claims.Intersect<TIRegionState>(proposingNation.regions).Count<TIRegionState>()) - (float)(5 * enemies.Intersect<TINationState>(respondingNation.allies).Count<TINationState>()) - (float)(5 * proposingNation.allies.Intersect<TINationState>(enemies2).Count<TINationState>()) + 0.5f * (proposingNation.militaryStrength - respondingNationMilitaryStrength) + (float)(5 * (proposingNation.numControlPoints - respondingNation.numControlPoints)) - 10f * TINationState.GetIdeologicalDistance((executiveFaction != null) ? executiveFaction.ideology.ideologyCoordinates : Vector3.zero, (executiveFaction2 != null) ? executiveFaction2.ideology.ideologyCoordinates : Vector3.zero) - 10f * Mathf.Abs(proposingNation.democracy - respondingNation.democracy)) / 100f, 0f, 1f);
		}

		// Token: 0x06005AF1 RID: 23281 RVA: 0x002BCA90 File Offset: 0x002BAC90
		public static float ChanceSurrenderRegion(TINationState askingNation, TIRegionState proposedRegion)
		{
			TIFactionState executiveFaction = askingNation.executiveFaction;
			TIFactionState executiveFaction2 = proposedRegion.nation.executiveFaction;
			if (executiveFaction != null && executiveFaction2 != null)
			{
				if (executiveFaction == executiveFaction2)
				{
					return 1f;
				}
				if (askingNation.alienNation && executiveFaction2.IsAlienProxy)
				{
					return 1f;
				}
				if (executiveFaction2.enemyWarFactions.Contains(executiveFaction))
				{
					return 0f;
				}
				if ((executiveFaction.proAlien && executiveFaction2.antiAlien) || (executiveFaction.antiAlien && executiveFaction2.proAlien))
				{
					return 0f;
				}
			}
			if (proposedRegion.nation.alienNation)
			{
				return 0f;
			}
			if (AIEvaluators.BadRegion(proposedRegion.nation, proposedRegion))
			{
				return 0.1f;
			}
			return 0f;
		}

		// Token: 0x06005AF2 RID: 23282 RVA: 0x002BCB50 File Offset: 0x002BAD50
		public static float ChanceAllowDarkFederationDeparture(TINationState askingNation)
		{
			TIFactionState executiveFaction = askingNation.executiveFaction;
			TINationState leadNation = askingNation.federation.leadNation;
			TIFactionState executiveFaction2 = leadNation.executiveFaction;
			if (executiveFaction != null && executiveFaction2 != null)
			{
				if (executiveFaction == executiveFaction2)
				{
					return 1f;
				}
				if (executiveFaction2.player.isAI)
				{
					if (executiveFaction2.GoalsWithTarget(askingNation, GoalType.NeutralizeNation, true).Any<TIFactionGoalState>())
					{
						return 1f;
					}
					if (executiveFaction2.AI_AtWarWithFaction(executiveFaction) && leadNation.federation.NetTaker(askingNation, FactionResource.Boost))
					{
						return 1f;
					}
				}
			}
			return 0f;
		}

		// Token: 0x06005AF3 RID: 23283 RVA: 0x002BCBE0 File Offset: 0x002BADE0
		public bool SelectPolicyReply(TINationState proposingNation, TINationState respondingNation, TIPolicyOptionWithConfirm policy)
		{
			float num = policy.AIAgreeChance(proposingNation, respondingNation);
			return TIUtilities.RandomFloatValue() <= num;
		}

		// Token: 0x06005AF4 RID: 23284 RVA: 0x002BCC04 File Offset: 0x002BAE04
		public bool SelectPolicyReply(TINationState proposingNation, TINationState respondingNation, TIWarState war, TIPolicyOptionWithConfirm policy)
		{
			float num = policy.AIAgreeChance(proposingNation, war);
			return TIUtilities.RandomFloatValue() <= num;
		}

		// Token: 0x06005AF5 RID: 23285 RVA: 0x002BCC28 File Offset: 0x002BAE28
		public bool SelectPolicyReply(TINationState proposingNation, TINationState respondingNation, TIPolicyOptionWithConfirm policy, TIRegionState region)
		{
			float num = policy.AIAgreeChance(proposingNation, region);
			return TIUtilities.RandomFloatValue() <= num;
		}
	}
}
