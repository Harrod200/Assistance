using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;
using UnityEngine;

// Token: 0x02000367 RID: 871
public class TIFormationTemplate : TIDataTemplate
{
	// Token: 0x06000F74 RID: 3956 RVA: 0x0004E834 File Offset: 0x0004CA34
	public static Vector3d[] GetSpacingOffset_km(bool isCombatSetup = false, bool forStratLayer = false)
	{
		float num = (((TIGlobalValuesState.isSpaceCombatEnabled || isCombatSetup) && !forStratLayer) ? SpaceCombatManager.GetFormationScalingFactor() : 1f);
		return new Vector3d[]
		{
			new Vector3d(60f * num, 60f * num, 100f * num),
			new Vector3d(75f * num, 75f * num, 125f * num),
			new Vector3d(100f * num, 100f * num, 150f * num),
			new Vector3d(125f * num, 125f * num, 200f * num),
			new Vector3d(150f * num, 150f * num, 250f * num)
		};
	}

	// Token: 0x06000F75 RID: 3957 RVA: 0x0004E905 File Offset: 0x0004CB05
	public double radius_km(FormationSpacing spacing, int ships)
	{
		return (double)ships * TIFormationTemplate.GetSpacingOffset_km(false, false)[(int)spacing].z;
	}

	// Token: 0x170001B3 RID: 435
	// (get) Token: 0x06000F76 RID: 3958 RVA: 0x0004E91C File Offset: 0x0004CB1C
	private List<Vector3d> filteredPositions
	{
		get
		{
			List<Vector3d> list = new List<Vector3d>();
			for (int i = 0; i < this.pos.Length; i++)
			{
				if (i == 0 || this.pos[i] != Vector3.zero)
				{
					list.Add(new Vector3d(this.pos[i].x, this.pos[i].y, this.pos[i].z));
				}
			}
			return list;
		}
	}

	// Token: 0x06000F77 RID: 3959 RVA: 0x0004E99C File Offset: 0x0004CB9C
	public Dictionary<TISpaceShipState, Vector3d> RelativeShipPositions_Units(List<TISpaceShipState> shipsInFormation, Formation formation, int numberOfPositions, bool invertZForCombat = false)
	{
		if (this.relativeShipPositionsCache.ShipsInFormation != null && shipsInFormation.SequenceEqual<TISpaceShipState>(this.relativeShipPositionsCache.ShipsInFormation) && formation.Equals(this.relativeShipPositionsCache.Formation) && numberOfPositions == this.relativeShipPositionsCache.NumberOfPositions && invertZForCombat == this.relativeShipPositionsCache.InvertZForCombat)
		{
			return this.relativeShipPositionsCache.RelativeShipPositions;
		}
		Dictionary<TISpaceShipState, Vector3d> dictionary = new Dictionary<TISpaceShipState, Vector3d>(shipsInFormation.Count);
		List<Vector3d> filteredPositions = this.filteredPositions;
		List<TISpaceShipState> list = new List<TISpaceShipState>(shipsInFormation);
		if (numberOfPositions > list.Count)
		{
			numberOfPositions = list.Count;
		}
		list.RemoveRange(numberOfPositions, shipsInFormation.Count - numberOfPositions);
		switch (formation.focus)
		{
		case FormationFocus.Ranged:
			list = list.OrderBy<TISpaceShipState, int>((TISpaceShipState x) => TIFormationTemplate.RoleAssignmentOrderForFormations.IndexOf(x.role)).ThenByDescending<TISpaceShipState, float>((TISpaceShipState y) => y.sumArmorValue).ToList<TISpaceShipState>();
			break;
		case FormationFocus.Heavy:
			list = list.OrderByDescending<TISpaceShipState, float>((TISpaceShipState x) => x.hull.length_m).ThenByDescending<TISpaceShipState, double>((TISpaceShipState y) => y.wetMass_tons).ToList<TISpaceShipState>();
			break;
		case FormationFocus.Armored:
			list = list.OrderByDescending<TISpaceShipState, float>((TISpaceShipState y) => y.sumArmorValue).ToList<TISpaceShipState>();
			break;
		case FormationFocus.Battle:
			list = list.OrderByDescending<TISpaceShipState, float>((TISpaceShipState x) => x.SpaceCombatValue(false, 0f)).ThenByDescending<TISpaceShipState, float>((TISpaceShipState y) => y.sumArmorValue).ToList<TISpaceShipState>();
			break;
		case FormationFocus.Swift:
			list = list.OrderByDescending<TISpaceShipState, float>((TISpaceShipState x) => x.manueverRating).ThenByDescending<TISpaceShipState, float>((TISpaceShipState y) => y.sumArmorValue).ToList<TISpaceShipState>();
			break;
		case FormationFocus.PointDefense:
			list = list.OrderByDescending<TISpaceShipState, int>((TISpaceShipState x) => x.allWeaponTemplates.Count<TIShipWeaponTemplate>((TIShipWeaponTemplate x) => x.defenseMode)).ThenByDescending<TISpaceShipState, float>((TISpaceShipState y) => y.sumArmorValue).ToList<TISpaceShipState>();
			break;
		case FormationFocus.NoseWeapons:
			list = list.OrderByDescending<TISpaceShipState, float>((TISpaceShipState x) => x.noseWeaponTemplates.Sum<TIShipWeaponTemplate>((TIShipWeaponTemplate x) => x.GenericScore())).ThenByDescending<TISpaceShipState, float>((TISpaceShipState y) => y.noseArmorValue).ToList<TISpaceShipState>();
			break;
		}
		List<TISpaceShipState> list2 = new List<TISpaceShipState>(list.Where<TISpaceShipState>((TISpaceShipState x) => x.nonCombatant)).ToList<TISpaceShipState>();
		int num;
		if (list.Count > list2.Count)
		{
			num = list.Count - list2.Count - filteredPositions.Count;
		}
		else
		{
			num = list.Count - filteredPositions.Count;
		}
		if (num > 0)
		{
			int num2 = filteredPositions.Count - this.resetIdx;
			for (int i = 0; i < num; i++)
			{
				int num3 = Mathf.FloorToInt((float)(i / num2)) + 1;
				bool flag = false;
				int num4 = 0;
				Vector3d newPosition = Vector3d.zero;
				Func<Vector3d, bool> <>9__16;
				while (!flag && num4 < 5000)
				{
					Vector3d vector3d = filteredPositions[this.resetIdx + i];
					Vector3d vector3d2 = vector3d / (double)num3;
					newPosition = vector3d + vector3d2 * (double)num4;
					if (this.clampXpos)
					{
						newPosition.x = vector3d.x;
					}
					if (this.clampYpos)
					{
						newPosition.y = vector3d.y;
					}
					if (this.useZoffset)
					{
						newPosition.z = vector3d.z + (double)this.Zoffset;
					}
					IEnumerable<Vector3d> enumerable = filteredPositions;
					Func<Vector3d, bool> func;
					if ((func = <>9__16) == null)
					{
						func = (<>9__16 = (Vector3d x) => (x - newPosition).sqrMagnitude < 1.0);
					}
					bool flag2 = enumerable.Any<Vector3d>(func);
					if (!filteredPositions.Contains(newPosition) && !flag2)
					{
						flag = true;
					}
					else
					{
						num4++;
					}
				}
				filteredPositions.Add(newPosition);
			}
		}
		if (invertZForCombat)
		{
			filteredPositions.Min<Vector3d>((Vector3d x) => x.z);
			for (int j = 0; j < filteredPositions.Count; j++)
			{
				filteredPositions[j] = new Vector3d(filteredPositions[j].x * -1.0, filteredPositions[j].y, -1.0 * filteredPositions[j].z);
			}
		}
		if (num < 0)
		{
			if (list.Count > list2.Count)
			{
				filteredPositions.RemoveRange(list.Count - list2.Count, Mathf.Abs(num));
			}
			else
			{
				filteredPositions.RemoveRange(list.Count, Mathf.Abs(num));
			}
		}
		if (formation.pattern.patternShift)
		{
			double num5 = (filteredPositions.Min<Vector3d>((Vector3d pos) => pos.x) + filteredPositions.Max<Vector3d>((Vector3d pos) => pos.x)) / 2.0;
			for (int k = 0; k < filteredPositions.Count; k++)
			{
				filteredPositions[k] = new Vector3d(filteredPositions[k].x - (double)((float)num5), filteredPositions[k].y, filteredPositions[k].z);
			}
		}
		float num6 = 2f;
		if (list2.Any<TISpaceShipState>() && list.Count > list2.Count)
		{
			double num7;
			if (!invertZForCombat)
			{
				num7 = filteredPositions.Min<Vector3d>((Vector3d pos) => pos.z) - (double)(num6 * 3f);
			}
			else
			{
				num7 = filteredPositions.Max<Vector3d>((Vector3d pos) => pos.z) + (double)(num6 * 3f);
			}
			double num8 = num7;
			for (int l = 0; l < list2.Count; l++)
			{
				float num9 = (float)(invertZForCombat ? l : (l * -1));
				dictionary.Add(list2[l], new Vector3d((l % 3 == 0) ? 0.5 : ((l % 3 == 1) ? (-0.5) : 0.0), 0.0, num8 + (double)num9));
				list.Remove(list2[l]);
			}
		}
		if (list.Count > 0)
		{
			List<Vector3d> list3 = new List<Vector3d>(filteredPositions);
			list3 = list3.Take<Vector3d>(list.Count).ToList<Vector3d>();
			double formationCenterX = (list3.Min<Vector3d>((Vector3d xMin) => xMin.x) + list3.Max<Vector3d>((Vector3d xMax) => xMax.x)) / 2.0;
			double formationCenterY = (list3.Min<Vector3d>((Vector3d yMin) => yMin.y) + list3.Max<Vector3d>((Vector3d yMax) => yMax.y)) / 2.0;
			double formationCenterZ = (list3.Min<Vector3d>((Vector3d zMin) => zMin.z) + list3.Max<Vector3d>((Vector3d zMax) => zMax.z)) / 2.0;
			Func<Vector3d, double> <>9__30;
			Func<Vector3d, double> <>9__31;
			for (int m = 0; m < list.Count; m++)
			{
				TISpaceShipState tispaceShipState = list[m];
				Vector3d vector3d3 = list3[0];
				switch (formation.concentration)
				{
				case FormationConcentration.Dispersed:
				{
					Vector3d vector3d4;
					if (m % 2 != 0)
					{
						vector3d4 = list3.MaxBy<Vector3d, double>((Vector3d x) => Vector3d.Distance(in x, in Vector3d.zero));
					}
					else
					{
						vector3d4 = list3.MinBy<Vector3d, double>((Vector3d x) => Vector3d.Distance(in x, in Vector3d.zero));
					}
					vector3d3 = vector3d4;
					break;
				}
				case FormationConcentration.Center:
				{
					IEnumerable<Vector3d> enumerable2 = list3;
					Func<Vector3d, double> func2;
					if ((func2 = <>9__30) == null)
					{
						func2 = (<>9__30 = delegate(Vector3d x)
						{
							Vector3d vector3d7 = new Vector3d(formationCenterX, formationCenterY, formationCenterZ);
							return Vector3d.Distance(in x, in vector3d7);
						});
					}
					vector3d3 = enumerable2.MinBy<Vector3d, double>(func2);
					break;
				}
				case FormationConcentration.Extremities:
				{
					IEnumerable<Vector3d> enumerable3 = list3;
					Func<Vector3d, double> func3;
					if ((func3 = <>9__31) == null)
					{
						func3 = (<>9__31 = delegate(Vector3d x)
						{
							Vector3d vector3d8 = new Vector3d(formationCenterX, formationCenterY, formationCenterZ);
							return Vector3d.Distance(in x, in vector3d8);
						});
					}
					vector3d3 = enumerable3.MaxBy<Vector3d, double>(func3);
					break;
				}
				case FormationConcentration.Right:
					if (invertZForCombat)
					{
						vector3d3 = list3.MinBy<Vector3d, double>((Vector3d x) => x.x);
					}
					else
					{
						vector3d3 = list3.MaxBy<Vector3d, double>((Vector3d x) => x.x);
					}
					break;
				case FormationConcentration.Left:
					if (invertZForCombat)
					{
						vector3d3 = list3.MaxBy<Vector3d, double>((Vector3d x) => x.x);
					}
					else
					{
						vector3d3 = list3.MinBy<Vector3d, double>((Vector3d x) => x.x);
					}
					break;
				case FormationConcentration.Flanks:
				{
					Vector3d vector3d5;
					if (m % 2 != 0)
					{
						vector3d5 = list3.MinBy<Vector3d, double>((Vector3d x) => x.x);
					}
					else
					{
						vector3d5 = list3.MaxBy<Vector3d, double>((Vector3d x) => x.x);
					}
					vector3d3 = vector3d5;
					break;
				}
				case FormationConcentration.Back:
					if (invertZForCombat)
					{
						vector3d3 = list3.MaxBy<Vector3d, double>((Vector3d x) => x.z);
					}
					else
					{
						vector3d3 = list3.MinBy<Vector3d, double>((Vector3d x) => x.z);
					}
					break;
				case FormationConcentration.Front:
					if (invertZForCombat)
					{
						vector3d3 = list3.MinBy<Vector3d, double>((Vector3d x) => x.z);
					}
					else
					{
						vector3d3 = list3.MaxBy<Vector3d, double>((Vector3d x) => x.z);
					}
					break;
				case FormationConcentration.High:
					vector3d3 = list3.MaxBy<Vector3d, double>((Vector3d x) => x.y);
					break;
				case FormationConcentration.Low:
					vector3d3 = list3.MinBy<Vector3d, double>((Vector3d x) => x.y);
					break;
				case FormationConcentration.Fins:
				{
					Vector3d vector3d6;
					if (m % 2 != 0)
					{
						vector3d6 = list3.MinBy<Vector3d, double>((Vector3d x) => x.y);
					}
					else
					{
						vector3d6 = list3.MaxBy<Vector3d, double>((Vector3d x) => x.y);
					}
					vector3d3 = vector3d6;
					break;
				}
				}
				dictionary.Add(tispaceShipState, vector3d3);
				list3.Remove(vector3d3);
			}
		}
		this.relativeShipPositionsCache = new TIFormationTemplate.RelativeShipPositionsCache
		{
			ShipsInFormation = shipsInFormation,
			Formation = formation,
			NumberOfPositions = numberOfPositions,
			InvertZForCombat = invertZForCombat,
			RelativeShipPositions = dictionary
		};
		return dictionary;
	}

	// Token: 0x04000F8D RID: 3981
	public bool clampXpos;

	// Token: 0x04000F8E RID: 3982
	public bool clampYpos;

	// Token: 0x04000F8F RID: 3983
	public bool useZoffset;

	// Token: 0x04000F90 RID: 3984
	public bool patternShift;

	// Token: 0x04000F91 RID: 3985
	public float Zoffset;

	// Token: 0x04000F92 RID: 3986
	public int resetIdx;

	// Token: 0x04000F93 RID: 3987
	public Vector3[] pos;

	// Token: 0x04000F94 RID: 3988
	public float AICombatBaseWeight;

	// Token: 0x04000F95 RID: 3989
	public int AIMaximumAllowedShips;

	// Token: 0x04000F96 RID: 3990
	private static readonly List<ShipRole> RoleAssignmentOrderForFormations = new List<ShipRole>
	{
		ShipRole.LL_Intruder,
		ShipRole.ML_Standoff,
		ShipRole.SL_Defender,
		ShipRole.LM_Interdictor,
		ShipRole.MM_SpaceSuperiority,
		ShipRole.SM_Patrol,
		ShipRole.LS_Penetrator,
		ShipRole.MS_Strike,
		ShipRole.SS_Interceptor,
		ShipRole.Explorer,
		ShipRole.EarthSurveillance,
		ShipRole.CouncilorTransport,
		ShipRole.TroopCarrier,
		ShipRole.ArmyCarrier,
		ShipRole.InnerSystemColonyShip,
		ShipRole.OuterSystemColonyShip,
		ShipRole.NoRole
	};

	// Token: 0x04000F97 RID: 3991
	private TIFormationTemplate.RelativeShipPositionsCache relativeShipPositionsCache;

	// Token: 0x02000BAD RID: 2989
	private struct RelativeShipPositionsCache
	{
		// Token: 0x04004B8B RID: 19339
		public List<TISpaceShipState> ShipsInFormation;

		// Token: 0x04004B8C RID: 19340
		public Formation Formation;

		// Token: 0x04004B8D RID: 19341
		public int NumberOfPositions;

		// Token: 0x04004B8E RID: 19342
		public bool InvertZForCombat;

		// Token: 0x04004B8F RID: 19343
		public Dictionary<TISpaceShipState, Vector3d> RelativeShipPositions;
	}
}
