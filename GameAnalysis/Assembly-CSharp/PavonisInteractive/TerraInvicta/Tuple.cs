using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020006F4 RID: 1780
	public sealed class Tuple
	{
		// Token: 0x060029EA RID: 10730 RVA: 0x000E353C File Offset: 0x000E173C
		public Tuple(string newId = "", int capacity = 4)
		{
			this.data = new List<Tuple.TupleValue>(capacity);
			this.Id = newId;
		}

		// Token: 0x170005C8 RID: 1480
		// (get) Token: 0x060029EB RID: 10731 RVA: 0x000E35D2 File Offset: 0x000E17D2
		// (set) Token: 0x060029EC RID: 10732 RVA: 0x000E35DA File Offset: 0x000E17DA
		public string Id { get; private set; }

		// Token: 0x170005C9 RID: 1481
		// (get) Token: 0x060029ED RID: 10733 RVA: 0x000E35E3 File Offset: 0x000E17E3
		public int Count
		{
			get
			{
				return this.data.Count;
			}
		}

		// Token: 0x060029EE RID: 10734 RVA: 0x000E35F0 File Offset: 0x000E17F0
		public T? Get<T>(int index) where T : struct
		{
			if (index < 0 || index >= this.data.Count)
			{
				return null;
			}
			Type typeFromHandle = typeof(T);
			if (Nullable.GetUnderlyingType(typeFromHandle) != null)
			{
				return null;
			}
			Tuple.TupleValue tupleValue = this.data[index];
			if (typeFromHandle != tupleValue.type)
			{
				return null;
			}
			object obj = null;
			if (typeFromHandle == this.boolType)
			{
				obj = tupleValue.b;
			}
			else if (typeFromHandle == this.intType)
			{
				obj = tupleValue.i;
			}
			else if (typeFromHandle == this.floatType)
			{
				obj = tupleValue.f;
			}
			else if (typeFromHandle == this.doubleType)
			{
				obj = tupleValue.d;
			}
			else if (typeFromHandle == this.stringType)
			{
				obj = tupleValue.s;
			}
			else if (typeFromHandle == this.objectType)
			{
				obj = tupleValue.o;
			}
			else if (typeFromHandle == this.listType)
			{
				obj = tupleValue.l;
			}
			return new T?((T)((object)obj));
		}

		// Token: 0x060029EF RID: 10735 RVA: 0x000E3724 File Offset: 0x000E1924
		public int? Add<T>(int index, T value) where T : struct
		{
			if (index < 0)
			{
				return null;
			}
			Type typeFromHandle = typeof(T);
			Tuple.TupleValue tupleValue = default(Tuple.TupleValue);
			if (typeFromHandle == this.boolType || typeFromHandle == this.intType || typeFromHandle == this.floatType || typeFromHandle == this.doubleType || typeFromHandle == this.stringType || typeFromHandle == this.objectType || typeFromHandle == this.listType)
			{
				tupleValue.o = value;
				tupleValue.type = typeFromHandle;
				this.data.Add(tupleValue);
				return new int?(this.data.Count);
			}
			return null;
		}

		// Token: 0x060029F0 RID: 10736 RVA: 0x000E37F0 File Offset: 0x000E19F0
		public bool Set<T>(int index, T value) where T : struct
		{
			if (index < 0 || index >= this.data.Count)
			{
				return false;
			}
			Type typeFromHandle = typeof(T);
			Tuple.TupleValue tupleValue = default(Tuple.TupleValue);
			if (typeFromHandle == this.boolType || typeFromHandle == this.intType || typeFromHandle == this.floatType || typeFromHandle == this.doubleType || typeFromHandle == this.stringType || typeFromHandle == this.objectType || typeFromHandle == this.listType)
			{
				tupleValue.o = value;
				tupleValue.type = typeFromHandle;
				this.data[index] = tupleValue;
				return true;
			}
			return false;
		}

		// Token: 0x060029F1 RID: 10737 RVA: 0x000E38AB File Offset: 0x000E1AAB
		public Type GetType(int index)
		{
			if (index < 0 || index >= this.data.Count)
			{
				return null;
			}
			return this.data[index].type;
		}

		// Token: 0x0400204C RID: 8268
		private Type boolType = typeof(bool?);

		// Token: 0x0400204D RID: 8269
		private Type intType = typeof(int?);

		// Token: 0x0400204E RID: 8270
		private Type floatType = typeof(float?);

		// Token: 0x0400204F RID: 8271
		private Type doubleType = typeof(double?);

		// Token: 0x04002050 RID: 8272
		private Type stringType = typeof(string);

		// Token: 0x04002051 RID: 8273
		private Type objectType = typeof(object);

		// Token: 0x04002052 RID: 8274
		private Type listType = typeof(List<object>);

		// Token: 0x04002053 RID: 8275
		private List<Tuple.TupleValue> data;

		// Token: 0x02000D1A RID: 3354
		[StructLayout(LayoutKind.Explicit, Size = 16)]
		private struct TupleValue
		{
			// Token: 0x04005086 RID: 20614
			[FieldOffset(0)]
			public bool? b;

			// Token: 0x04005087 RID: 20615
			[FieldOffset(0)]
			public int? i;

			// Token: 0x04005088 RID: 20616
			[FieldOffset(0)]
			public float? f;

			// Token: 0x04005089 RID: 20617
			[FieldOffset(0)]
			public double? d;

			// Token: 0x0400508A RID: 20618
			[FieldOffset(0)]
			public string s;

			// Token: 0x0400508B RID: 20619
			[FieldOffset(0)]
			public object o;

			// Token: 0x0400508C RID: 20620
			[FieldOffset(0)]
			public List<object> l;

			// Token: 0x0400508D RID: 20621
			[FieldOffset(8)]
			public Type type;
		}
	}
}
