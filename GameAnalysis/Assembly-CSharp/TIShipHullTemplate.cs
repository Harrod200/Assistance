using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PavonisInteractive.TerraInvicta;
using UnityEngine;

// Token: 0x020003CD RID: 973
public class TIShipHullTemplate : TIShipModuleTemplate
{
	// Token: 0x170001F9 RID: 505
	// (get) Token: 0x06001216 RID: 4630 RVA: 0x00057738 File Offset: 0x00055938
	public override string description
	{
		get
		{
			return Loc.T("UI.Fleets.HullDescription", new object[]
			{
				this.noseHardpoints,
				this.hullHardpoints,
				this.internalModules,
				TemplateManager.global.missionControlInlineSpritePath,
				this.missionControl,
				TemplateManager.global.moneyInlineSpritePath,
				-this.monthlyIncome_Money
			});
		}
	}

	// Token: 0x170001FA RID: 506
	// (get) Token: 0x06001217 RID: 4631 RVA: 0x000577B6 File Offset: 0x000559B6
	public override List<ShipModuleSlotType> allowedSlots
	{
		get
		{
			return new List<ShipModuleSlotType>();
		}
	}

	// Token: 0x170001FB RID: 507
	// (get) Token: 0x06001218 RID: 4632 RVA: 0x000577BD File Offset: 0x000559BD
	public float volume_m3
	{
		get
		{
			return 3.1415927f * (this.width_m / 2f) * (this.width_m / 2f) * this.length_m;
		}
	}

	// Token: 0x170001FC RID: 508
	// (get) Token: 0x06001219 RID: 4633 RVA: 0x000577E5 File Offset: 0x000559E5
	public float capSurfaceArea_m2
	{
		get
		{
			return 3.1415927f * (this.width_m / 2f) * (this.width_m / 2f);
		}
	}

	// Token: 0x170001FD RID: 509
	// (get) Token: 0x0600121A RID: 4634 RVA: 0x00057806 File Offset: 0x00055A06
	public bool largeHull
	{
		get
		{
			if (!this.alien)
			{
				return this.length_m >= 200f;
			}
			return this.length_m >= 250f && this.length_m < 500f;
		}
	}

	// Token: 0x170001FE RID: 510
	// (get) Token: 0x0600121B RID: 4635 RVA: 0x0005783D File Offset: 0x00055A3D
	public bool smallHull
	{
		get
		{
			if (!this.alien)
			{
				return this.length_m <= 100f;
			}
			return this.length_m <= 125f;
		}
	}

	// Token: 0x170001FF RID: 511
	// (get) Token: 0x0600121C RID: 4636 RVA: 0x00057868 File Offset: 0x00055A68
	public bool hugeHull
	{
		get
		{
			return this.length_m >= 500f;
		}
	}

	// Token: 0x17000200 RID: 512
	// (get) Token: 0x0600121D RID: 4637 RVA: 0x0005787C File Offset: 0x00055A7C
	public bool mediumHull
	{
		get
		{
			if (!this.alien)
			{
				return this.length_m > 100f && this.length_m < 200f;
			}
			return this.length_m > 125f && this.length_m < 250f;
		}
	}

	// Token: 0x0600121E RID: 4638 RVA: 0x000578CC File Offset: 0x00055ACC
	public int slotIndex(TIShipHullTemplate.ShipModuleSlot shipModuleSlot)
	{
		return this.shipModuleSlots.FindIndex((TIShipHullTemplate.ShipModuleSlot x) => x.x == shipModuleSlot.x && x.y == shipModuleSlot.y);
	}

	// Token: 0x17000201 RID: 513
	// (get) Token: 0x0600121F RID: 4639 RVA: 0x000578FD File Offset: 0x00055AFD
	public float maxNoseArmorDepth_m
	{
		get
		{
			return this.length_m * (this.simpleHull ? 0.018f : 0.036f);
		}
	}

	// Token: 0x17000202 RID: 514
	// (get) Token: 0x06001220 RID: 4640 RVA: 0x0005791A File Offset: 0x00055B1A
	public float maxTailArmorDepth_m
	{
		get
		{
			return this.length_m * (this.simpleHull ? 0f : 0.036f);
		}
	}

	// Token: 0x17000203 RID: 515
	// (get) Token: 0x06001221 RID: 4641 RVA: 0x00057937 File Offset: 0x00055B37
	public float maxLateralArmorDepth_m
	{
		get
		{
			return this.width_m * (this.simpleHull ? 0.06f : 0.12f);
		}
	}

	// Token: 0x17000204 RID: 516
	// (get) Token: 0x06001222 RID: 4642 RVA: 0x00057954 File Offset: 0x00055B54
	public float baseArmorCapAngleCoverage_deg_realisticScaling
	{
		get
		{
			return 57.29578f * Mathf.Tan(this.width_m / this.length_m);
		}
	}

	// Token: 0x06001223 RID: 4643 RVA: 0x0005796E File Offset: 0x00055B6E
	public float noShipyardConstructionTime_Days(TIFactionState faction)
	{
		return (this.baseConstructionTime_days + TIEffectsState.SumEffectsModifiers(Context.ShipConstructionTime, faction, this.baseConstructionTime_days, null)) * TIGlobalValuesState.GetShipConstructionTimeSettingsModifier(faction);
	}

	// Token: 0x06001224 RID: 4644 RVA: 0x00057990 File Offset: 0x00055B90
	public float constructionTime_Days(TIHabModuleState shipyard)
	{
		float num = this.baseConstructionTime_days * shipyard.moduleTemplate.ShipyardConstructionSpeedModifier(this) * TIGlobalValuesState.GetShipConstructionTimeSettingsModifier(shipyard.ref_faction);
		return num + TIEffectsState.SumEffectsModifiers(Context.ShipConstructionTime, shipyard.ref_faction, num, null);
	}

	// Token: 0x06001225 RID: 4645 RVA: 0x000579D4 File Offset: 0x00055BD4
	public float constructionTime_Days(int shipyardTier, TIFactionState faction)
	{
		TIHabModuleTemplate tihabModuleTemplate = TemplateManager.IterateByClass<TIHabModuleTemplate>(true).First<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.allowsShipConstruction && x.tier == shipyardTier && faction.IsAlienFaction == x.alienModule);
		float num = this.baseConstructionTime_days * tihabModuleTemplate.ShipyardConstructionSpeedModifier(this) * TIGlobalValuesState.GetShipConstructionTimeSettingsModifier(faction);
		return num + TIEffectsState.SumEffectsModifiers(Context.ShipConstructionTime, faction, num, null);
	}

	// Token: 0x06001226 RID: 4646 RVA: 0x00057A3E File Offset: 0x00055C3E
	public string combatUINosePath_OK(int idx)
	{
		return new StringBuilder(this.combatUIpath[idx]).Append("_head_A").ToString();
	}

	// Token: 0x06001227 RID: 4647 RVA: 0x00057A5C File Offset: 0x00055C5C
	public string combatUINosePath_Damaged(int idx)
	{
		return new StringBuilder(this.combatUIpath[idx]).Append("_head_B").ToString();
	}

	// Token: 0x06001228 RID: 4648 RVA: 0x00057A7A File Offset: 0x00055C7A
	public string combatUINosePath_Destroyed(int idx)
	{
		return new StringBuilder(this.combatUIpath[idx]).Append("_head_C").ToString();
	}

	// Token: 0x06001229 RID: 4649 RVA: 0x00057A98 File Offset: 0x00055C98
	public string combatUIMidPath_OK(int idx)
	{
		return new StringBuilder(this.combatUIpath[idx]).Append("_mid_A").ToString();
	}

	// Token: 0x0600122A RID: 4650 RVA: 0x00057AB6 File Offset: 0x00055CB6
	public string combatUIMidPath_Damaged(int idx)
	{
		return new StringBuilder(this.combatUIpath[idx]).Append("_mid_B").ToString();
	}

	// Token: 0x0600122B RID: 4651 RVA: 0x00057AD4 File Offset: 0x00055CD4
	public string combatUIMidPath_Destroyed(int idx)
	{
		return new StringBuilder(this.combatUIpath[idx]).Append("_mid_C").ToString();
	}

	// Token: 0x0600122C RID: 4652 RVA: 0x00057AF2 File Offset: 0x00055CF2
	public string combatUITailPath_OK(int idx)
	{
		return new StringBuilder(this.combatUIpath[idx]).Append("_tail_A").ToString();
	}

	// Token: 0x0600122D RID: 4653 RVA: 0x00057B10 File Offset: 0x00055D10
	public string combatUITailPath_Damaged(int idx)
	{
		return new StringBuilder(this.combatUIpath[idx]).Append("_tail_B").ToString();
	}

	// Token: 0x0600122E RID: 4654 RVA: 0x00057B2E File Offset: 0x00055D2E
	public string combatUITailPath_Destroyed(int idx)
	{
		return new StringBuilder(this.combatUIpath[idx]).Append("_tail_C").ToString();
	}

	// Token: 0x0600122F RID: 4655 RVA: 0x00057B4C File Offset: 0x00055D4C
	public string combatUINoseArmorPath_OK(int idx)
	{
		return new StringBuilder(this.combatUIpath[idx]).Append("_head_armor_A").ToString();
	}

	// Token: 0x06001230 RID: 4656 RVA: 0x00057B6A File Offset: 0x00055D6A
	public string combatUINoseArmorPath_Destroyed(int idx)
	{
		return new StringBuilder(this.combatUIpath[idx]).Append("_head_armor_B").ToString();
	}

	// Token: 0x06001231 RID: 4657 RVA: 0x00057B88 File Offset: 0x00055D88
	public string combatUIPortArmorPath_OK(int idx)
	{
		return new StringBuilder(this.combatUIpath[idx]).Append("_mid_armor_up_A").ToString();
	}

	// Token: 0x06001232 RID: 4658 RVA: 0x00057BA6 File Offset: 0x00055DA6
	public string combatUIPortArmorPath_Destroyed(int idx)
	{
		return new StringBuilder(this.combatUIpath[idx]).Append("_mid_armor_up_B").ToString();
	}

	// Token: 0x06001233 RID: 4659 RVA: 0x00057BC4 File Offset: 0x00055DC4
	public string combatUIStarboardArmorPath_OK(int idx)
	{
		return new StringBuilder(this.combatUIpath[idx]).Append("_mid_armor_down_A").ToString();
	}

	// Token: 0x06001234 RID: 4660 RVA: 0x00057BE2 File Offset: 0x00055DE2
	public string combatUIStarboardArmorPath_Destroyed(int idx)
	{
		return new StringBuilder(this.combatUIpath[idx]).Append("_mid_armor_down_B").ToString();
	}

	// Token: 0x06001235 RID: 4661 RVA: 0x00057C00 File Offset: 0x00055E00
	public string combatUITailArmorPath_OK(int idx)
	{
		return new StringBuilder(this.combatUIpath[idx]).Append("_tail_armor_A").ToString();
	}

	// Token: 0x06001236 RID: 4662 RVA: 0x00057C1E File Offset: 0x00055E1E
	public string combatUITailArmorPath_Destroyed(int idx)
	{
		return new StringBuilder(this.combatUIpath[idx]).Append("_tail_armor_B").ToString();
	}

	// Token: 0x06001237 RID: 4663 RVA: 0x00057C3C File Offset: 0x00055E3C
	public string largeCombatUIPath(int idx)
	{
		return this.combatUIpath[idx];
	}

	// Token: 0x06001238 RID: 4664 RVA: 0x00057C46 File Offset: 0x00055E46
	public string noseUIResourcePath(int idx)
	{
		return new StringBuilder("Objects/").Append(this.path1[idx]).Append(this.path2[idx]).Append("_head_A")
			.ToString();
	}

	// Token: 0x06001239 RID: 4665 RVA: 0x00057C7B File Offset: 0x00055E7B
	public string midUIResourcePath(int idx)
	{
		return new StringBuilder("Objects/").Append(this.path1[idx]).Append(this.path2[idx]).Append("_mid_A")
			.ToString();
	}

	// Token: 0x0600123A RID: 4666 RVA: 0x00057CB0 File Offset: 0x00055EB0
	public string tailUIResourcePath(int idx)
	{
		return new StringBuilder("Objects/").Append(this.path1[idx]).Append(this.path2[idx]).Append("_tail_A")
			.ToString();
	}

	// Token: 0x0600123B RID: 4667 RVA: 0x00057CE8 File Offset: 0x00055EE8
	public Vector2Int GetUniqueSlotCoordinates(ShipModuleSlotType slotType)
	{
		foreach (TIShipHullTemplate.ShipModuleSlot shipModuleSlot in this.shipModuleSlots)
		{
			if (slotType == shipModuleSlot.moduleSlotType)
			{
				return shipModuleSlot.slotPosition;
			}
		}
		return new Vector2Int(-1, -1);
	}

	// Token: 0x0600123C RID: 4668 RVA: 0x00057D50 File Offset: 0x00055F50
	public int GetUniqueSlotIndex(ShipModuleSlotType slotType)
	{
		for (int i = 0; i < this.shipModuleSlots.Count; i++)
		{
			if (slotType == this.shipModuleSlots[i].moduleSlotType)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x0600123D RID: 4669 RVA: 0x00057D8C File Offset: 0x00055F8C
	public TIShipHullTemplate.ShipModuleSlot GetSlotByCoordinates(int x, int y)
	{
		foreach (TIShipHullTemplate.ShipModuleSlot shipModuleSlot in this.shipModuleSlots)
		{
			if (shipModuleSlot.x == x && shipModuleSlot.y == y)
			{
				return shipModuleSlot;
			}
		}
		return default(TIShipHullTemplate.ShipModuleSlot);
	}

	// Token: 0x0600123E RID: 4670 RVA: 0x00057DFC File Offset: 0x00055FFC
	public TIShipHullTemplate.ShipModuleSlot GetSlotByCoordinates(Vector2 coordinates)
	{
		return this.GetSlotByCoordinates((int)coordinates.x, (int)coordinates.y);
	}

	// Token: 0x0600123F RID: 4671 RVA: 0x00057E14 File Offset: 0x00056014
	public Vector2 GetCoordinatesForSlot(int slot)
	{
		return this.shipModuleSlots[slot].slotPosition;
	}

	// Token: 0x06001240 RID: 4672 RVA: 0x00057E3C File Offset: 0x0005603C
	public List<TIShipHullTemplate.ShipModuleSlot> GetAllSlotsOfType(ShipModuleSlotType slotType)
	{
		List<TIShipHullTemplate.ShipModuleSlot> list = new List<TIShipHullTemplate.ShipModuleSlot>();
		foreach (TIShipHullTemplate.ShipModuleSlot shipModuleSlot in this.shipModuleSlots)
		{
			if (shipModuleSlot.moduleSlotType == slotType)
			{
				list.Add(shipModuleSlot);
			}
		}
		return list;
	}

	// Token: 0x06001241 RID: 4673 RVA: 0x00057EA0 File Offset: 0x000560A0
	public TIShipHullTemplate.ShipModuleSlot AdjacentRightSlot(TIShipHullTemplate.ShipModuleSlot testModuleSlot)
	{
		foreach (TIShipHullTemplate.ShipModuleSlot shipModuleSlot in this.shipModuleSlots)
		{
			if (testModuleSlot.x + 1 == shipModuleSlot.x && testModuleSlot.y == shipModuleSlot.y)
			{
				return shipModuleSlot;
			}
		}
		return default(TIShipHullTemplate.ShipModuleSlot);
	}

	// Token: 0x06001242 RID: 4674 RVA: 0x00057F1C File Offset: 0x0005611C
	public TIShipHullTemplate.ShipModuleSlot AdjacentDownSlot(TIShipHullTemplate.ShipModuleSlot testModuleSlot)
	{
		foreach (TIShipHullTemplate.ShipModuleSlot shipModuleSlot in this.shipModuleSlots)
		{
			if (testModuleSlot.y + 2 == shipModuleSlot.y && testModuleSlot.x == shipModuleSlot.x)
			{
				return shipModuleSlot;
			}
		}
		return default(TIShipHullTemplate.ShipModuleSlot);
	}

	// Token: 0x06001243 RID: 4675 RVA: 0x00057F98 File Offset: 0x00056198
	public TIShipHullTemplate.ShipModuleSlot AdjacentHorizNoseSlot(TIShipHullTemplate.ShipModuleSlot testModuleSlot)
	{
		foreach (TIShipHullTemplate.ShipModuleSlot shipModuleSlot in this.shipModuleSlots)
		{
			if (testModuleSlot.x + 1 == shipModuleSlot.x && (testModuleSlot.y == shipModuleSlot.y || testModuleSlot.y + 1 == shipModuleSlot.y))
			{
				return shipModuleSlot;
			}
		}
		return default(TIShipHullTemplate.ShipModuleSlot);
	}

	// Token: 0x06001244 RID: 4676 RVA: 0x00058024 File Offset: 0x00056224
	public List<TIShipHullTemplate.ShipModuleSlot> WeaponSlotSet(TIShipHullTemplate.ShipModuleSlot coreSlot, Mount mount)
	{
		List<TIShipHullTemplate.ShipModuleSlot> list = new List<TIShipHullTemplate.ShipModuleSlot>();
		switch (mount)
		{
		default:
			list.Add(coreSlot);
			break;
		case Mount.TwoHullHoriz:
		{
			TIShipHullTemplate.ShipModuleSlot shipModuleSlot = this.AdjacentRightSlot(coreSlot);
			if (shipModuleSlot.moduleSlotType == ShipModuleSlotType.HullHardPoint)
			{
				list.Add(coreSlot);
				list.Add(shipModuleSlot);
			}
			break;
		}
		case Mount.TwoHullVert:
		{
			TIShipHullTemplate.ShipModuleSlot shipModuleSlot2 = this.AdjacentDownSlot(coreSlot);
			if (shipModuleSlot2.moduleSlotType == ShipModuleSlotType.HullHardPoint)
			{
				list.Add(coreSlot);
				list.Add(shipModuleSlot2);
			}
			break;
		}
		case Mount.ThreeHullHoriz:
		{
			TIShipHullTemplate.ShipModuleSlot shipModuleSlot3 = this.AdjacentRightSlot(coreSlot);
			if (shipModuleSlot3.moduleSlotType == ShipModuleSlotType.HullHardPoint)
			{
				TIShipHullTemplate.ShipModuleSlot shipModuleSlot4 = this.AdjacentRightSlot(shipModuleSlot3);
				if (shipModuleSlot4.moduleSlotType == ShipModuleSlotType.HullHardPoint)
				{
					list.Add(coreSlot);
					list.Add(shipModuleSlot3);
					list.Add(shipModuleSlot4);
				}
			}
			break;
		}
		case Mount.FourHull:
		{
			TIShipHullTemplate.ShipModuleSlot shipModuleSlot5 = this.AdjacentRightSlot(coreSlot);
			if (shipModuleSlot5.moduleSlotType == ShipModuleSlotType.HullHardPoint)
			{
				TIShipHullTemplate.ShipModuleSlot shipModuleSlot6 = this.AdjacentDownSlot(coreSlot);
				if (shipModuleSlot6.moduleSlotType == ShipModuleSlotType.HullHardPoint)
				{
					TIShipHullTemplate.ShipModuleSlot shipModuleSlot7 = this.AdjacentDownSlot(shipModuleSlot5);
					if (shipModuleSlot7.moduleSlotType == ShipModuleSlotType.HullHardPoint)
					{
						list.Add(coreSlot);
						list.Add(shipModuleSlot5);
						list.Add(shipModuleSlot6);
						list.Add(shipModuleSlot7);
					}
				}
			}
			break;
		}
		case Mount.TwoNoseHoriz:
		{
			TIShipHullTemplate.ShipModuleSlot shipModuleSlot8 = this.AdjacentHorizNoseSlot(coreSlot);
			if (shipModuleSlot8.moduleSlotType == ShipModuleSlotType.NoseHardPoint)
			{
				list.Add(coreSlot);
				list.Add(shipModuleSlot8);
			}
			break;
		}
		case Mount.TwoNoseVert:
		{
			TIShipHullTemplate.ShipModuleSlot shipModuleSlot9 = this.AdjacentDownSlot(coreSlot);
			if (shipModuleSlot9.moduleSlotType == ShipModuleSlotType.NoseHardPoint)
			{
				list.Add(coreSlot);
				list.Add(shipModuleSlot9);
			}
			break;
		}
		case Mount.ThreeNoseAngle:
		{
			TIShipHullTemplate.ShipModuleSlot shipModuleSlot10 = this.AdjacentDownSlot(coreSlot);
			if (shipModuleSlot10.moduleSlotType == ShipModuleSlotType.NoseHardPoint)
			{
				TIShipHullTemplate.ShipModuleSlot shipModuleSlot11 = this.AdjacentHorizNoseSlot(coreSlot);
				if (shipModuleSlot11.moduleSlotType == ShipModuleSlotType.NoseHardPoint)
				{
					list.Add(coreSlot);
					list.Add(shipModuleSlot10);
					list.Add(shipModuleSlot11);
				}
			}
			break;
		}
		case Mount.FourNose:
		{
			TIShipHullTemplate.ShipModuleSlot shipModuleSlot12 = this.AdjacentDownSlot(coreSlot);
			if (shipModuleSlot12.moduleSlotType == ShipModuleSlotType.NoseHardPoint)
			{
				TIShipHullTemplate.ShipModuleSlot shipModuleSlot13 = this.AdjacentRightSlot(shipModuleSlot12);
				if (shipModuleSlot13.moduleSlotType == ShipModuleSlotType.NoseHardPoint)
				{
					TIShipHullTemplate.ShipModuleSlot shipModuleSlot14 = this.AdjacentDownSlot(shipModuleSlot12);
					if (shipModuleSlot13.moduleSlotType == ShipModuleSlotType.NoseHardPoint)
					{
						list.Add(coreSlot);
						list.Add(shipModuleSlot12);
						list.Add(shipModuleSlot13);
						list.Add(shipModuleSlot14);
					}
				}
			}
			break;
		}
		}
		return list;
	}

	// Token: 0x06001245 RID: 4677 RVA: 0x0005826C File Offset: 0x0005646C
	public List<List<TIShipHullTemplate.ShipModuleSlot>> ValidBigWeaponSlotSets(Mount mount)
	{
		List<List<TIShipHullTemplate.ShipModuleSlot>> list = new List<List<TIShipHullTemplate.ShipModuleSlot>>();
		if (mount - Mount.TwoHullHoriz > 3)
		{
			if (mount - Mount.TwoNoseHoriz > 3)
			{
				return list;
			}
		}
		else
		{
			List<TIShipHullTemplate.ShipModuleSlot> allSlotsOfType = this.GetAllSlotsOfType(ShipModuleSlotType.HullHardPoint);
			if (allSlotsOfType.Count <= 0)
			{
				return list;
			}
			using (List<TIShipHullTemplate.ShipModuleSlot>.Enumerator enumerator = allSlotsOfType.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIShipHullTemplate.ShipModuleSlot shipModuleSlot = enumerator.Current;
					List<TIShipHullTemplate.ShipModuleSlot> list2 = this.WeaponSlotSet(shipModuleSlot, mount);
					if (list2.Count > 0)
					{
						list.Add(list2);
					}
				}
				return list;
			}
		}
		List<TIShipHullTemplate.ShipModuleSlot> allSlotsOfType2 = this.GetAllSlotsOfType(ShipModuleSlotType.NoseHardPoint);
		if (allSlotsOfType2.Count > 0)
		{
			foreach (TIShipHullTemplate.ShipModuleSlot shipModuleSlot2 in allSlotsOfType2)
			{
				List<TIShipHullTemplate.ShipModuleSlot> list3 = this.WeaponSlotSet(shipModuleSlot2, mount);
				if (list3.Count > 0)
				{
					list.Add(list3);
				}
			}
		}
		return list;
	}

	// Token: 0x06001246 RID: 4678 RVA: 0x00058364 File Offset: 0x00056564
	public static TIShipHullTemplate.ShipModuleSlot AssignCoreSlotOnMultiMountPlacement(TISpaceShipTemplate ship, TIShipWeaponTemplate weapon, int droppedSlot)
	{
		List<List<TIShipHullTemplate.ShipModuleSlot>> list = ship.hullTemplate.ValidBigWeaponSlotSets(weapon.mount);
		List<List<TIShipHullTemplate.ShipModuleSlot>> list2 = new List<List<TIShipHullTemplate.ShipModuleSlot>>();
		Vector2Int slotCoordinates = ship.hullTemplate.shipModuleSlots[droppedSlot].slotPosition;
		Func<TIShipHullTemplate.ShipModuleSlot, bool> <>9__0;
		foreach (List<TIShipHullTemplate.ShipModuleSlot> list3 in list)
		{
			IEnumerable<TIShipHullTemplate.ShipModuleSlot> enumerable = list3;
			Func<TIShipHullTemplate.ShipModuleSlot, bool> func;
			if ((func = <>9__0) == null)
			{
				func = (<>9__0 = (TIShipHullTemplate.ShipModuleSlot x) => x.slotPosition == slotCoordinates);
			}
			if (enumerable.Any<TIShipHullTemplate.ShipModuleSlot>(func))
			{
				bool flag = false;
				foreach (TIShipHullTemplate.ShipModuleSlot shipModuleSlot in list3)
				{
					if (ship.GetPartInHullSlot(shipModuleSlot, true) != null)
					{
						flag = true;
					}
				}
				if (!flag)
				{
					list2.Add(list3);
				}
			}
		}
		if (list2.Count == 1)
		{
			return list[0][0];
		}
		int num = 999;
		TIShipHullTemplate.ShipModuleSlot shipModuleSlot2 = list[0][0];
		foreach (List<TIShipHullTemplate.ShipModuleSlot> list4 in list2)
		{
			for (int i = 0; i < list4.Count; i++)
			{
				if (ship.hullTemplate.slotIndex(list4[i]) == droppedSlot && i < num)
				{
					shipModuleSlot2 = list4[i];
					num = i;
				}
			}
		}
		return shipModuleSlot2;
	}

	// Token: 0x06001247 RID: 4679 RVA: 0x00058514 File Offset: 0x00056714
	public string GetLocalizedMaximums(TISpaceShipTemplate ship, ShipModuleSlotType slot)
	{
		if (ship != null && ship.GetArmorFacingTemplateInSlot(slot).materialTemplate != null)
		{
			switch (slot)
			{
			case ShipModuleSlotType.NoseArmor:
			{
				float num;
				return Loc.T("TIShipArmorTemplate.MaxArmorPoints_Nose", new object[] { ship.GetMaxAllowedArmorBySlot(slot, out num, null) });
			}
			case ShipModuleSlotType.LateralArmor:
			{
				float num;
				return Loc.T("TIShipArmorTemplate.MaxArmorPoints_Lateral", new object[] { ship.GetMaxAllowedArmorBySlot(slot, out num, null) });
			}
			case ShipModuleSlotType.TailArmor:
			{
				float num;
				return Loc.T("TIShipArmorTemplate.MaxArmorPoints_Tail", new object[] { ship.GetMaxAllowedArmorBySlot(slot, out num, null) });
			}
			}
		}
		else
		{
			switch (slot)
			{
			case ShipModuleSlotType.NoseArmor:
			{
				string text = "TIShipArmorTemplate.MaxArmorThickness_Nose";
				object[] array = new object[1];
				int num2 = 0;
				float num = this.maxNoseArmorDepth_m * 100f;
				array[num2] = num.ToString("N0");
				return Loc.T(text, array);
			}
			case ShipModuleSlotType.LateralArmor:
			{
				string text2 = "TIShipArmorTemplate.MaxArmorThickness_Lateral";
				object[] array2 = new object[1];
				int num3 = 0;
				float num = this.maxLateralArmorDepth_m * 100f;
				array2[num3] = num.ToString("N0");
				return Loc.T(text2, array2);
			}
			case ShipModuleSlotType.TailArmor:
			{
				string text3 = "TIShipArmorTemplate.MaxArmorThickness_Tail";
				object[] array3 = new object[1];
				int num4 = 0;
				float num = this.maxTailArmorDepth_m * 100f;
				array3[num4] = num.ToString("N0");
				return Loc.T(text3, array3);
			}
			}
		}
		return string.Empty;
	}

	// Token: 0x06001248 RID: 4680 RVA: 0x00058664 File Offset: 0x00056864
	public override string GetDescriptionData(TISpaceShipState ship = null, TISpaceShipTemplate shipTemplate = null, bool prospective = false, ShipModuleSlotType slot = ShipModuleSlotType.None, bool splitFireModes = false)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine(this.GetLocalizedMaximums(shipTemplate, ShipModuleSlotType.NoseArmor));
		stringBuilder.AppendLine(this.GetLocalizedMaximums(shipTemplate, ShipModuleSlotType.LateralArmor));
		stringBuilder.AppendLine(this.GetLocalizedMaximums(shipTemplate, ShipModuleSlotType.TailArmor));
		stringBuilder.AppendLine(this.GetLocalizedMass());
		if (this.crew > 0)
		{
			stringBuilder.AppendLine(base.GetLocalizedCrew());
		}
		stringBuilder.AppendLine(this.GetLocalizedCost());
		return stringBuilder.ToString();
	}

	// Token: 0x040010D8 RID: 4312
	public int noseHardpoints;

	// Token: 0x040010D9 RID: 4313
	public int hullHardpoints;

	// Token: 0x040010DA RID: 4314
	public int internalModules;

	// Token: 0x040010DB RID: 4315
	public float length_m;

	// Token: 0x040010DC RID: 4316
	public float width_m;

	// Token: 0x040010DD RID: 4317
	public int thrusterMultiplier = 1;

	// Token: 0x040010DE RID: 4318
	public int structuralIntegrity;

	// Token: 0x040010DF RID: 4319
	public float monthlyIncome_Money;

	// Token: 0x040010E0 RID: 4320
	public int missionControl;

	// Token: 0x040010E1 RID: 4321
	public bool alien;

	// Token: 0x040010E2 RID: 4322
	public float baseConstructionTime_days;

	// Token: 0x040010E3 RID: 4323
	public int consTier;

	// Token: 0x040010E4 RID: 4324
	public int maxOfficers;

	// Token: 0x040010E5 RID: 4325
	public bool simpleHull;

	// Token: 0x040010E6 RID: 4326
	public bool noShipyardBuild;

	// Token: 0x040010E7 RID: 4327
	public string[] path1;

	// Token: 0x040010E8 RID: 4328
	public string[] path2;

	// Token: 0x040010E9 RID: 4329
	public float[] shipyardyOffset = new float[3];

	// Token: 0x040010EA RID: 4330
	public new string[] modelResource;

	// Token: 0x040010EB RID: 4331
	public new string[] combatUIpath;

	// Token: 0x040010EC RID: 4332
	public List<TIShipHullTemplate.ShipModuleSlot> shipModuleSlots = new List<TIShipHullTemplate.ShipModuleSlot>();

	// Token: 0x02000BE0 RID: 3040
	public struct ShipModuleSlot
	{
		// Token: 0x1700113A RID: 4410
		// (get) Token: 0x06006A8C RID: 27276 RVA: 0x003039B2 File Offset: 0x00301BB2
		public Vector2Int slotPosition
		{
			get
			{
				return new Vector2Int(this.x, this.y);
			}
		}

		// Token: 0x1700113B RID: 4411
		// (get) Token: 0x06006A8D RID: 27277 RVA: 0x003039C5 File Offset: 0x00301BC5
		public bool weaponSlot
		{
			get
			{
				return this.moduleSlotType == ShipModuleSlotType.HullHardPoint || this.moduleSlotType == ShipModuleSlotType.NoseHardPoint;
			}
		}

		// Token: 0x1700113C RID: 4412
		// (get) Token: 0x06006A8E RID: 27278 RVA: 0x003039DC File Offset: 0x00301BDC
		public bool armorSlot
		{
			get
			{
				return this.moduleSlotType == ShipModuleSlotType.LateralArmor || this.moduleSlotType == ShipModuleSlotType.NoseArmor || this.moduleSlotType == ShipModuleSlotType.TailArmor;
			}
		}

		// Token: 0x04004C5F RID: 19551
		public int x;

		// Token: 0x04004C60 RID: 19552
		public int y;

		// Token: 0x04004C61 RID: 19553
		public ShipModuleSlotType moduleSlotType;
	}
}
