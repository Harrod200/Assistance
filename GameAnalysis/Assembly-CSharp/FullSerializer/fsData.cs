using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace FullSerializer
{
	// Token: 0x02000465 RID: 1125
	public sealed class fsData
	{
		// Token: 0x060017B3 RID: 6067 RVA: 0x0007B48D File Offset: 0x0007968D
		public fsData()
		{
			this._value = null;
		}

		// Token: 0x060017B4 RID: 6068 RVA: 0x0007B49C File Offset: 0x0007969C
		public fsData(bool boolean)
		{
			this._value = boolean;
		}

		// Token: 0x060017B5 RID: 6069 RVA: 0x0007B4B0 File Offset: 0x000796B0
		public fsData(double f)
		{
			this._value = f;
		}

		// Token: 0x060017B6 RID: 6070 RVA: 0x0007B4C4 File Offset: 0x000796C4
		public fsData(long i)
		{
			this._value = i;
		}

		// Token: 0x060017B7 RID: 6071 RVA: 0x0007B4D8 File Offset: 0x000796D8
		public fsData(string str)
		{
			this._value = str;
		}

		// Token: 0x060017B8 RID: 6072 RVA: 0x0007B4E7 File Offset: 0x000796E7
		public fsData(Dictionary<string, fsData> dict)
		{
			this._value = dict;
		}

		// Token: 0x060017B9 RID: 6073 RVA: 0x0007B4F6 File Offset: 0x000796F6
		public fsData(List<fsData> list)
		{
			this._value = list;
		}

		// Token: 0x060017BA RID: 6074 RVA: 0x0007B505 File Offset: 0x00079705
		public static fsData CreateDictionary()
		{
			return new fsData(new Dictionary<string, fsData>(fsGlobalConfig.IsCaseSensitive ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase));
		}

		// Token: 0x060017BB RID: 6075 RVA: 0x0007B524 File Offset: 0x00079724
		public static fsData CreateList()
		{
			return new fsData(new List<fsData>());
		}

		// Token: 0x060017BC RID: 6076 RVA: 0x0007B530 File Offset: 0x00079730
		public static fsData CreateList(int capacity)
		{
			return new fsData(new List<fsData>(capacity));
		}

		// Token: 0x060017BD RID: 6077 RVA: 0x0007B53D File Offset: 0x0007973D
		internal void BecomeDictionary()
		{
			this._value = new Dictionary<string, fsData>();
		}

		// Token: 0x060017BE RID: 6078 RVA: 0x0007B54A File Offset: 0x0007974A
		internal fsData Clone()
		{
			return new fsData
			{
				_value = this._value
			};
		}

		// Token: 0x17000348 RID: 840
		// (get) Token: 0x060017BF RID: 6079 RVA: 0x0007B560 File Offset: 0x00079760
		public fsDataType Type
		{
			get
			{
				if (this._value == null)
				{
					return fsDataType.Null;
				}
				if (this._value is double)
				{
					return fsDataType.Double;
				}
				if (this._value is long)
				{
					return fsDataType.Int64;
				}
				if (this._value is bool)
				{
					return fsDataType.Boolean;
				}
				if (this._value is string)
				{
					return fsDataType.String;
				}
				if (this._value is Dictionary<string, fsData>)
				{
					return fsDataType.Object;
				}
				if (this._value is List<fsData>)
				{
					return fsDataType.Array;
				}
				throw new InvalidOperationException("unknown JSON data type");
			}
		}

		// Token: 0x17000349 RID: 841
		// (get) Token: 0x060017C0 RID: 6080 RVA: 0x0007B5DB File Offset: 0x000797DB
		public bool IsNull
		{
			get
			{
				return this._value == null;
			}
		}

		// Token: 0x1700034A RID: 842
		// (get) Token: 0x060017C1 RID: 6081 RVA: 0x0007B5E6 File Offset: 0x000797E6
		public bool IsDouble
		{
			get
			{
				return this._value is double;
			}
		}

		// Token: 0x1700034B RID: 843
		// (get) Token: 0x060017C2 RID: 6082 RVA: 0x0007B5F6 File Offset: 0x000797F6
		public bool IsInt64
		{
			get
			{
				return this._value is long;
			}
		}

		// Token: 0x1700034C RID: 844
		// (get) Token: 0x060017C3 RID: 6083 RVA: 0x0007B606 File Offset: 0x00079806
		public bool IsBool
		{
			get
			{
				return this._value is bool;
			}
		}

		// Token: 0x1700034D RID: 845
		// (get) Token: 0x060017C4 RID: 6084 RVA: 0x0007B616 File Offset: 0x00079816
		public bool IsString
		{
			get
			{
				return this._value is string;
			}
		}

		// Token: 0x1700034E RID: 846
		// (get) Token: 0x060017C5 RID: 6085 RVA: 0x0007B626 File Offset: 0x00079826
		public bool IsDictionary
		{
			get
			{
				return this._value is Dictionary<string, fsData>;
			}
		}

		// Token: 0x1700034F RID: 847
		// (get) Token: 0x060017C6 RID: 6086 RVA: 0x0007B636 File Offset: 0x00079836
		public bool IsList
		{
			get
			{
				return this._value is List<fsData>;
			}
		}

		// Token: 0x17000350 RID: 848
		// (get) Token: 0x060017C7 RID: 6087 RVA: 0x0007B646 File Offset: 0x00079846
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public double AsDouble
		{
			get
			{
				return this.Cast<double>();
			}
		}

		// Token: 0x17000351 RID: 849
		// (get) Token: 0x060017C8 RID: 6088 RVA: 0x0007B64E File Offset: 0x0007984E
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public long AsInt64
		{
			get
			{
				return this.Cast<long>();
			}
		}

		// Token: 0x17000352 RID: 850
		// (get) Token: 0x060017C9 RID: 6089 RVA: 0x0007B656 File Offset: 0x00079856
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public bool AsBool
		{
			get
			{
				return this.Cast<bool>();
			}
		}

		// Token: 0x17000353 RID: 851
		// (get) Token: 0x060017CA RID: 6090 RVA: 0x0007B65E File Offset: 0x0007985E
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public string AsString
		{
			get
			{
				return this.Cast<string>();
			}
		}

		// Token: 0x17000354 RID: 852
		// (get) Token: 0x060017CB RID: 6091 RVA: 0x0007B666 File Offset: 0x00079866
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public Dictionary<string, fsData> AsDictionary
		{
			get
			{
				return this.Cast<Dictionary<string, fsData>>();
			}
		}

		// Token: 0x17000355 RID: 853
		// (get) Token: 0x060017CC RID: 6092 RVA: 0x0007B66E File Offset: 0x0007986E
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public List<fsData> AsList
		{
			get
			{
				return this.Cast<List<fsData>>();
			}
		}

		// Token: 0x060017CD RID: 6093 RVA: 0x0007B678 File Offset: 0x00079878
		private T Cast<T>()
		{
			if (this._value is T)
			{
				return (T)((object)this._value);
			}
			string[] array = new string[6];
			array[0] = "Unable to cast <";
			array[1] = ((this != null) ? this.ToString() : null);
			array[2] = "> (with type = ";
			int num = 3;
			Type type = this._value.GetType();
			array[num] = ((type != null) ? type.ToString() : null);
			array[4] = ") to type ";
			int num2 = 5;
			Type typeFromHandle = typeof(T);
			array[num2] = ((typeFromHandle != null) ? typeFromHandle.ToString() : null);
			throw new InvalidCastException(string.Concat(array));
		}

		// Token: 0x060017CE RID: 6094 RVA: 0x0007B709 File Offset: 0x00079909
		public override string ToString()
		{
			return fsJsonPrinter.CompressedJson(this);
		}

		// Token: 0x060017CF RID: 6095 RVA: 0x0007B711 File Offset: 0x00079911
		public override bool Equals(object obj)
		{
			return this.Equals(obj as fsData);
		}

		// Token: 0x060017D0 RID: 6096 RVA: 0x0007B720 File Offset: 0x00079920
		public bool Equals(fsData other)
		{
			if (other == null || this.Type != other.Type)
			{
				return false;
			}
			switch (this.Type)
			{
			case fsDataType.Array:
			{
				List<fsData> asList = this.AsList;
				List<fsData> asList2 = other.AsList;
				if (asList.Count != asList2.Count)
				{
					return false;
				}
				for (int i = 0; i < asList.Count; i++)
				{
					if (!asList[i].Equals(asList2[i]))
					{
						return false;
					}
				}
				return true;
			}
			case fsDataType.Object:
			{
				Dictionary<string, fsData> asDictionary = this.AsDictionary;
				Dictionary<string, fsData> asDictionary2 = other.AsDictionary;
				if (asDictionary.Count != asDictionary2.Count)
				{
					return false;
				}
				foreach (string text in asDictionary.Keys)
				{
					if (!asDictionary2.ContainsKey(text))
					{
						return false;
					}
					if (!asDictionary[text].Equals(asDictionary2[text]))
					{
						return false;
					}
				}
				return true;
			}
			case fsDataType.Double:
				return this.AsDouble == other.AsDouble || Math.Abs(this.AsDouble - other.AsDouble) < double.Epsilon;
			case fsDataType.Int64:
				return this.AsInt64 == other.AsInt64;
			case fsDataType.Boolean:
				return this.AsBool == other.AsBool;
			case fsDataType.String:
				return this.AsString == other.AsString;
			case fsDataType.Null:
				return true;
			default:
				throw new Exception("Unknown data type");
			}
		}

		// Token: 0x060017D1 RID: 6097 RVA: 0x0007B8C0 File Offset: 0x00079AC0
		public static bool operator ==(fsData a, fsData b)
		{
			if (a == b)
			{
				return true;
			}
			if (a == null || b == null)
			{
				return false;
			}
			if (a.IsDouble && b.IsDouble)
			{
				return Math.Abs(a.AsDouble - b.AsDouble) < double.Epsilon;
			}
			return a.Equals(b);
		}

		// Token: 0x060017D2 RID: 6098 RVA: 0x0007B910 File Offset: 0x00079B10
		public static bool operator !=(fsData a, fsData b)
		{
			return !(a == b);
		}

		// Token: 0x060017D3 RID: 6099 RVA: 0x0007B91C File Offset: 0x00079B1C
		public override int GetHashCode()
		{
			return this._value.GetHashCode();
		}

		// Token: 0x040015EC RID: 5612
		private object _value;

		// Token: 0x040015ED RID: 5613
		public static readonly fsData True = new fsData(true);

		// Token: 0x040015EE RID: 5614
		public static readonly fsData False = new fsData(false);

		// Token: 0x040015EF RID: 5615
		public static readonly fsData Null = new fsData();
	}
}
