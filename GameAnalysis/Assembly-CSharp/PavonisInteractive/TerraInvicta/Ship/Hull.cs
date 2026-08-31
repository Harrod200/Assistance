using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta.SpaceCombat;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.Ship
{
	// Token: 0x02000977 RID: 2423
	public class Hull : IHull
	{
		// Token: 0x17000FC4 RID: 4036
		// (get) Token: 0x06005C3D RID: 23613 RVA: 0x002C06AF File Offset: 0x002BE8AF
		// (set) Token: 0x06005C3E RID: 23614 RVA: 0x002C06B7 File Offset: 0x002BE8B7
		public IList<IHullSection> sections { get; private set; }

		// Token: 0x17000FC5 RID: 4037
		// (get) Token: 0x06005C3F RID: 23615 RVA: 0x002C06C0 File Offset: 0x002BE8C0
		// (set) Token: 0x06005C40 RID: 23616 RVA: 0x002C06C8 File Offset: 0x002BE8C8
		public TISpaceShipState shipState { get; private set; }

		// Token: 0x17000FC6 RID: 4038
		// (get) Token: 0x06005C41 RID: 23617 RVA: 0x002C06D1 File Offset: 0x002BE8D1
		public float length
		{
			get
			{
				return this.shipState.hull.length_m;
			}
		}

		// Token: 0x17000FC7 RID: 4039
		// (get) Token: 0x06005C42 RID: 23618 RVA: 0x002C06E3 File Offset: 0x002BE8E3
		public float width
		{
			get
			{
				return this.shipState.hull.width_m;
			}
		}

		// Token: 0x06005C43 RID: 23619 RVA: 0x002C06F5 File Offset: 0x002BE8F5
		public Hull(IList<IHullSection> sections, CombatShipController shipController)
		{
			this.sections = sections;
			this.shipState = shipController.ShipState;
			this.mountKeys = new Dictionary<Type, IList<Hull.MapKey>>();
			this.mountMaps = new Dictionary<Hull.MapKey, ComponentMap>();
			this.components = new Dictionary<Hull.MapKey, List<IComponent>>();
		}

		// Token: 0x06005C44 RID: 23620 RVA: 0x002C0731 File Offset: 0x002BE931
		public Hull(IList<IHullSection> sections, TISpaceShipState state)
		{
			this.sections = sections;
			this.shipState = state;
			this.mountKeys = new Dictionary<Type, IList<Hull.MapKey>>();
			this.mountMaps = new Dictionary<Hull.MapKey, ComponentMap>();
			this.components = new Dictionary<Hull.MapKey, List<IComponent>>();
		}

		// Token: 0x06005C45 RID: 23621 RVA: 0x002C0768 File Offset: 0x002BE968
		public bool AddComponentMap<T>(ComponentMap map, string name = "") where T : IComponent
		{
			Hull.MapKey mapKey = new Hull.MapKey(typeof(T), name);
			if (this.mountMaps.ContainsKey(mapKey))
			{
				return false;
			}
			this.mountMaps[mapKey] = map;
			this.components[mapKey] = new List<IComponent>();
			IList<Hull.MapKey> list;
			if (!this.mountKeys.TryGetValue(typeof(T), out list))
			{
				list = new List<Hull.MapKey>();
				this.mountKeys[typeof(T)] = list;
			}
			list.Add(mapKey);
			return true;
		}

		// Token: 0x06005C46 RID: 23622 RVA: 0x002C07F2 File Offset: 0x002BE9F2
		public bool AddComponentMap<T>(string name = "") where T : IComponent
		{
			return this.AddComponentMap<T>(ComponentMap.single, name);
		}

		// Token: 0x06005C47 RID: 23623 RVA: 0x002C0800 File Offset: 0x002BEA00
		public bool Attach<T>(T component, int heightOffset, int widthOffset, string name = "", params IHullSection[] sections) where T : IComponent
		{
			Hull.MapKey mapKey = new Hull.MapKey(typeof(T), name);
			ComponentMap componentMap;
			if (this.mountMaps.TryGetValue(mapKey, out componentMap) && componentMap.Attach(component.map, heightOffset, widthOffset))
			{
				this.components[mapKey].Add(component);
				return true;
			}
			return false;
		}

		// Token: 0x06005C48 RID: 23624 RVA: 0x002C0861 File Offset: 0x002BEA61
		public bool Attach<T>(T component, string name = "", params IHullSection[] sections) where T : IComponent
		{
			return this.Attach<T>(component, 0, 0, name, Array.Empty<IHullSection>());
		}

		// Token: 0x06005C49 RID: 23625 RVA: 0x002C0872 File Offset: 0x002BEA72
		public IEnumerable<T> IterateByClass<T>() where T : IComponent
		{
			IList<Hull.MapKey> list;
			if (this.mountKeys.TryGetValue(typeof(T), out list))
			{
				foreach (Hull.MapKey mapKey in list)
				{
					List<IComponent> list2 = this.components[mapKey];
					foreach (IComponent component in list2)
					{
						yield return (T)((object)component);
					}
					List<IComponent>.Enumerator enumerator2 = default(List<IComponent>.Enumerator);
				}
				IEnumerator<Hull.MapKey> enumerator = null;
			}
			yield break;
			yield break;
		}

		// Token: 0x06005C4A RID: 23626 RVA: 0x002C0884 File Offset: 0x002BEA84
		public ArmorFacing BearingFacing(Transform shooter, Transform target, out float struckAngle)
		{
			Vector3 normalized = (shooter.position - target.position).normalized;
			float angle = Vector3.SignedAngle(target.forward, normalized, Vector3.up);
			struckAngle = angle;
			Func<Facing, bool> <>9__0;
			foreach (IHullSection hullSection in this.sections)
			{
				HullSection hullSection2 = (HullSection)hullSection;
				if (hullSection2.Contains(angle))
				{
					IEnumerable<Facing> facings = hullSection2.facings;
					Func<Facing, bool> func;
					if ((func = <>9__0) == null)
					{
						func = (<>9__0 = (Facing x) => x.Contains(angle));
					}
					return facings.First<Facing>(func).armorFacing;
				}
			}
			return ArmorFacing.Core;
		}

		// Token: 0x06005C4B RID: 23627 RVA: 0x002C095C File Offset: 0x002BEB5C
		public ArmorFacing StruckFacing(DamageSource source, Vector3 position, Vector3 forward, out float struckAngle)
		{
			Vector3 normalized = (source.hitPosition - position).normalized;
			float angle = Vector3.SignedAngle(forward, normalized, Vector3.up);
			struckAngle = angle;
			Func<Facing, bool> <>9__0;
			foreach (IHullSection hullSection in this.sections)
			{
				HullSection hullSection2 = (HullSection)hullSection;
				if (hullSection2.Contains(angle))
				{
					IEnumerable<Facing> facings = hullSection2.facings;
					Func<Facing, bool> func;
					if ((func = <>9__0) == null)
					{
						func = (<>9__0 = (Facing x) => x.Contains(angle));
					}
					return facings.First<Facing>(func).armorFacing;
				}
			}
			return ArmorFacing.Core;
		}

		// Token: 0x06005C4C RID: 23628 RVA: 0x002C0A2C File Offset: 0x002BEC2C
		public float ApplyDamage(DamageSource source, Transform transform)
		{
			Vector3 normalized = (source.hitPosition - transform.position).normalized;
			float num = Vector3.SignedAngle(transform.forward, normalized, Vector3.up);
			float num2 = 0f;
			foreach (IHullSection hullSection in this.sections)
			{
				if (hullSection.Contains(num))
				{
					Damage damage = source.damage;
					if ((damage.amount > 0f || damage.chippingAmount > 0f || (float)damage.shreddingAmount > 0f) && !this.IsDestroyed())
					{
						float num3;
						damage = hullSection.ApplyDamage(damage, num, out num3);
						num2 += num3;
						break;
					}
					break;
				}
			}
			return num2;
		}

		// Token: 0x06005C4D RID: 23629 RVA: 0x002C0B08 File Offset: 0x002BED08
		public bool IsDestroyed()
		{
			return this.shipState.ShipDestroyed();
		}

		// Token: 0x040041D3 RID: 16851
		private IDictionary<Type, IList<Hull.MapKey>> mountKeys;

		// Token: 0x040041D4 RID: 16852
		private IDictionary<Hull.MapKey, ComponentMap> mountMaps;

		// Token: 0x040041D5 RID: 16853
		private readonly IDictionary<Hull.MapKey, List<IComponent>> components;

		// Token: 0x02001333 RID: 4915
		private struct MapKey : IEquatable<Hull.MapKey>
		{
			// Token: 0x0600908D RID: 37005 RVA: 0x00344E21 File Offset: 0x00343021
			public MapKey(Type type, string name)
			{
				this.type = type;
				this.name = name;
			}

			// Token: 0x0600908E RID: 37006 RVA: 0x00344E31 File Offset: 0x00343031
			public bool Equals(Hull.MapKey key)
			{
				return this.type == key.type && this.name == key.name;
			}

			// Token: 0x0600908F RID: 37007 RVA: 0x00344E59 File Offset: 0x00343059
			public override int GetHashCode()
			{
				return HashCode.Combine<Type, string>(this.type, this.name);
			}

			// Token: 0x06009090 RID: 37008 RVA: 0x00344E6C File Offset: 0x0034306C
			public override bool Equals(object obj)
			{
				return obj != null && !(obj.GetType() != typeof(Hull.MapKey)) && this.Equals((Hull.MapKey)obj);
			}

			// Token: 0x04006F67 RID: 28519
			public string name;

			// Token: 0x04006F68 RID: 28520
			public Type type;
		}
	}
}
