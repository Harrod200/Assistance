using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace FullSerializer.Internal
{
	// Token: 0x02000481 RID: 1153
	public class fsCyclicReferenceManager
	{
		// Token: 0x060018A0 RID: 6304 RVA: 0x0007FAE5 File Offset: 0x0007DCE5
		public void Enter()
		{
			this._depth++;
		}

		// Token: 0x060018A1 RID: 6305 RVA: 0x0007FAF8 File Offset: 0x0007DCF8
		public bool Exit()
		{
			this._depth--;
			if (this._depth == 0)
			{
				this._objectIds = new Dictionary<object, int>(fsCyclicReferenceManager.ObjectReferenceEqualityComparator.Instance);
				this._nextId = 0;
				this._marked = new Dictionary<int, object>();
			}
			if (this._depth < 0)
			{
				this._depth = 0;
				throw new InvalidOperationException("Internal Error - Mismatched Enter/Exit. Please report a bug at https://github.com/jacobdufault/fullserializer/issues with the serialization data.");
			}
			return this._depth == 0;
		}

		// Token: 0x060018A2 RID: 6306 RVA: 0x0007FB61 File Offset: 0x0007DD61
		public object GetReferenceObject(int id)
		{
			if (!this._marked.ContainsKey(id))
			{
				throw new InvalidOperationException("Internal Deserialization Error - Object definition has not been encountered for object with id=" + id.ToString() + "; have you reordered or modified the serialized data? If this is an issue with an unmodified Full Serializer implementation and unmodified serialization data, please report an issue with an included test case.");
			}
			return this._marked[id];
		}

		// Token: 0x060018A3 RID: 6307 RVA: 0x0007FB99 File Offset: 0x0007DD99
		public void AddReferenceWithId(int id, object reference)
		{
			this._marked[id] = reference;
		}

		// Token: 0x060018A4 RID: 6308 RVA: 0x0007FBA8 File Offset: 0x0007DDA8
		public int GetReferenceId(object item)
		{
			int num;
			if (!this._objectIds.TryGetValue(item, out num))
			{
				int nextId = this._nextId;
				this._nextId = nextId + 1;
				num = nextId;
				this._objectIds[item] = num;
			}
			return num;
		}

		// Token: 0x060018A5 RID: 6309 RVA: 0x0007FBE5 File Offset: 0x0007DDE5
		public bool IsReference(object item)
		{
			return this._marked.ContainsKey(this.GetReferenceId(item));
		}

		// Token: 0x060018A6 RID: 6310 RVA: 0x0007FBFC File Offset: 0x0007DDFC
		public void MarkSerialized(object item)
		{
			int referenceId = this.GetReferenceId(item);
			if (this._marked.ContainsKey(referenceId))
			{
				throw new InvalidOperationException("Internal Error - " + ((item != null) ? item.ToString() : null) + " has already been marked as serialized");
			}
			this._marked[referenceId] = item;
		}

		// Token: 0x04001616 RID: 5654
		private Dictionary<object, int> _objectIds = new Dictionary<object, int>(fsCyclicReferenceManager.ObjectReferenceEqualityComparator.Instance);

		// Token: 0x04001617 RID: 5655
		private int _nextId;

		// Token: 0x04001618 RID: 5656
		private Dictionary<int, object> _marked = new Dictionary<int, object>();

		// Token: 0x04001619 RID: 5657
		private int _depth;

		// Token: 0x02000C57 RID: 3159
		private class ObjectReferenceEqualityComparator : IEqualityComparer<object>
		{
			// Token: 0x06006C60 RID: 27744 RVA: 0x00306879 File Offset: 0x00304A79
			bool IEqualityComparer<object>.Equals(object x, object y)
			{
				return x == y;
			}

			// Token: 0x06006C61 RID: 27745 RVA: 0x0030687F File Offset: 0x00304A7F
			int IEqualityComparer<object>.GetHashCode(object obj)
			{
				return RuntimeHelpers.GetHashCode(obj);
			}

			// Token: 0x04004E0F RID: 19983
			public static readonly IEqualityComparer<object> Instance = new fsCyclicReferenceManager.ObjectReferenceEqualityComparator();
		}
	}
}
