using System;
using System.Collections.Generic;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.Ship
{
	// Token: 0x02000976 RID: 2422
	public interface IHull
	{
		// Token: 0x17000FC1 RID: 4033
		// (get) Token: 0x06005C33 RID: 23603
		IList<IHullSection> sections { get; }

		// Token: 0x17000FC2 RID: 4034
		// (get) Token: 0x06005C34 RID: 23604
		float length { get; }

		// Token: 0x17000FC3 RID: 4035
		// (get) Token: 0x06005C35 RID: 23605
		float width { get; }

		// Token: 0x06005C36 RID: 23606
		bool AddComponentMap<T>(string name = "") where T : IComponent;

		// Token: 0x06005C37 RID: 23607
		bool AddComponentMap<T>(ComponentMap map, string name = "") where T : IComponent;

		// Token: 0x06005C38 RID: 23608
		bool Attach<T>(T component, string name = "", params IHullSection[] sections) where T : IComponent;

		// Token: 0x06005C39 RID: 23609
		bool Attach<T>(T component, int heightOffset, int widthOffset, string name = "", params IHullSection[] sections) where T : IComponent;

		// Token: 0x06005C3A RID: 23610
		IEnumerable<T> IterateByClass<T>() where T : IComponent;

		// Token: 0x06005C3B RID: 23611
		float ApplyDamage(DamageSource source, Transform transform);

		// Token: 0x06005C3C RID: 23612
		bool IsDestroyed();
	}
}
