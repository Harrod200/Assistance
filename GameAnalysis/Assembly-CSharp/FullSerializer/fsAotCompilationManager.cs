using System;
using System.Collections.Generic;
using System.Text;
using FullSerializer.Internal;

namespace FullSerializer
{
	// Token: 0x02000457 RID: 1111
	public class fsAotCompilationManager
	{
		// Token: 0x0600177C RID: 6012 RVA: 0x00079EA8 File Offset: 0x000780A8
		private static bool HasMember(fsAotVersionInfo versionInfo, fsAotVersionInfo.Member member)
		{
			fsAotVersionInfo.Member[] members = versionInfo.Members;
			for (int i = 0; i < members.Length; i++)
			{
				if (members[i] == member)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600177D RID: 6013 RVA: 0x00079EDC File Offset: 0x000780DC
		public static bool IsAotModelUpToDate(fsMetaType currentModel, fsIAotConverter aotModel)
		{
			if (currentModel.IsDefaultConstructorPublic != aotModel.VersionInfo.IsConstructorPublic)
			{
				return false;
			}
			if (currentModel.Properties.Length != aotModel.VersionInfo.Members.Length)
			{
				return false;
			}
			foreach (fsMetaProperty fsMetaProperty in currentModel.Properties)
			{
				if (!fsAotCompilationManager.HasMember(aotModel.VersionInfo, new fsAotVersionInfo.Member(fsMetaProperty)))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x0600177E RID: 6014 RVA: 0x00079F48 File Offset: 0x00078148
		public static string RunAotCompilationForType(fsConfig config, Type type)
		{
			fsMetaType fsMetaType = fsMetaType.Get(config, type);
			fsMetaType.EmitAotData(true);
			return fsAotCompilationManager.GenerateDirectConverterForTypeInCSharp(type, fsMetaType.Properties, fsMetaType.IsDefaultConstructorPublic);
		}

		// Token: 0x0600177F RID: 6015 RVA: 0x00079F78 File Offset: 0x00078178
		private static string EmitVersionInfo(string prefix, Type type, fsMetaProperty[] members, bool isConstructorPublic)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("new fsAotVersionInfo {");
			stringBuilder.AppendLine(prefix + "    IsConstructorPublic = " + (isConstructorPublic ? "true" : "false") + ",");
			stringBuilder.AppendLine(prefix + "    Members = new fsAotVersionInfo.Member[] {");
			foreach (fsMetaProperty fsMetaProperty in members)
			{
				stringBuilder.AppendLine(prefix + "        new fsAotVersionInfo.Member {");
				stringBuilder.AppendLine(prefix + "            MemberName = \"" + fsMetaProperty.MemberName + "\",");
				stringBuilder.AppendLine(prefix + "            JsonName = \"" + fsMetaProperty.JsonName + "\",");
				stringBuilder.AppendLine(prefix + "            StorageType = \"" + fsMetaProperty.StorageType.CSharpName(true) + "\",");
				if (fsMetaProperty.OverrideConverterType != null)
				{
					stringBuilder.AppendLine(prefix + "            OverrideConverterType = \"" + fsMetaProperty.OverrideConverterType.CSharpName(true) + "\",");
				}
				stringBuilder.AppendLine(prefix + "        },");
			}
			stringBuilder.AppendLine(prefix + "    }");
			stringBuilder.Append(prefix + "}");
			return stringBuilder.ToString();
		}

		// Token: 0x06001780 RID: 6016 RVA: 0x0007A0C5 File Offset: 0x000782C5
		private static string GetConverterString(fsMetaProperty member)
		{
			if (member.OverrideConverterType == null)
			{
				return "null";
			}
			return string.Format("typeof({0})", member.OverrideConverterType.CSharpName(true));
		}

		// Token: 0x06001781 RID: 6017 RVA: 0x0007A0F1 File Offset: 0x000782F1
		public static string GetQualifiedConverterNameForType(Type type)
		{
			return "FullSerializer.Speedup." + type.CSharpName(true, true) + "_DirectConverter";
		}

		// Token: 0x06001782 RID: 6018 RVA: 0x0007A10C File Offset: 0x0007830C
		private static string GenerateDirectConverterForTypeInCSharp(Type type, fsMetaProperty[] members, bool isConstructorPublic)
		{
			StringBuilder stringBuilder = new StringBuilder();
			string text = type.CSharpName(true);
			string text2 = type.CSharpName(true, true);
			stringBuilder.AppendLine("using System;");
			stringBuilder.AppendLine("using System.Collections.Generic;");
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("namespace FullSerializer {");
			stringBuilder.AppendLine("    partial class fsConverterRegistrar {");
			stringBuilder.AppendLine(string.Concat(new string[] { "        public static Speedup.", text2, "_DirectConverter Register_", text2, ";" }));
			stringBuilder.AppendLine("    }");
			stringBuilder.AppendLine("}");
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("namespace FullSerializer.Speedup {");
			stringBuilder.AppendLine(string.Concat(new string[] { "    public class ", text2, "_DirectConverter : fsDirectConverter<", text, ">, fsIAotConverter {" }));
			stringBuilder.AppendLine("        private fsAotVersionInfo _versionInfo = " + fsAotCompilationManager.EmitVersionInfo("        ", type, members, isConstructorPublic) + ";");
			stringBuilder.AppendLine("        fsAotVersionInfo fsIAotConverter.VersionInfo { get { return _versionInfo; } }");
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("        protected override fsResult DoSerialize(" + text + " model, Dictionary<string, fsData> serialized) {");
			stringBuilder.AppendLine("            var result = fsResult.Success;");
			stringBuilder.AppendLine();
			foreach (fsMetaProperty fsMetaProperty in members)
			{
				stringBuilder.AppendLine(string.Concat(new string[]
				{
					"            result += SerializeMember(serialized, ",
					fsAotCompilationManager.GetConverterString(fsMetaProperty),
					", \"",
					fsMetaProperty.JsonName,
					"\", model.",
					fsMetaProperty.MemberName,
					");"
				}));
			}
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("            return result;");
			stringBuilder.AppendLine("        }");
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("        protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref " + text + " model) {");
			stringBuilder.AppendLine("            var result = fsResult.Success;");
			stringBuilder.AppendLine();
			for (int j = 0; j < members.Length; j++)
			{
				fsMetaProperty fsMetaProperty2 = members[j];
				stringBuilder.AppendLine(string.Concat(new string[]
				{
					"            var t",
					j.ToString(),
					" = model.",
					fsMetaProperty2.MemberName,
					";"
				}));
				stringBuilder.AppendLine(string.Concat(new string[]
				{
					"            result += DeserializeMember(data, ",
					fsAotCompilationManager.GetConverterString(fsMetaProperty2),
					", \"",
					fsMetaProperty2.JsonName,
					"\", out t",
					j.ToString(),
					");"
				}));
				stringBuilder.AppendLine(string.Concat(new string[]
				{
					"            model.",
					fsMetaProperty2.MemberName,
					" = t",
					j.ToString(),
					";"
				}));
				stringBuilder.AppendLine();
			}
			stringBuilder.AppendLine("            return result;");
			stringBuilder.AppendLine("        }");
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("        public override object CreateInstance(fsData data, Type storageType) {");
			if (isConstructorPublic)
			{
				stringBuilder.AppendLine("            return new " + text + "();");
			}
			else
			{
				stringBuilder.AppendLine("            return Activator.CreateInstance(typeof(" + text + "), /*nonPublic:*/true);");
			}
			stringBuilder.AppendLine("        }");
			stringBuilder.AppendLine("    }");
			stringBuilder.AppendLine("}");
			return stringBuilder.ToString();
		}

		// Token: 0x040015C0 RID: 5568
		public static HashSet<Type> AotCandidateTypes = new HashSet<Type>();
	}
}
