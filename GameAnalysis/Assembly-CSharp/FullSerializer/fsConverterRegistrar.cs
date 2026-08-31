using System;
using System.Collections.Generic;
using System.Reflection;
using FullSerializer.Internal;
using FullSerializer.Internal.DirectConverters;

namespace FullSerializer
{
	// Token: 0x0200045B RID: 1115
	public class fsConverterRegistrar
	{
		// Token: 0x0600178A RID: 6026 RVA: 0x0007A594 File Offset: 0x00078794
		static fsConverterRegistrar()
		{
			foreach (FieldInfo fieldInfo in typeof(fsConverterRegistrar).GetDeclaredFields())
			{
				if (fieldInfo.Name.StartsWith("Register_"))
				{
					fsConverterRegistrar.Converters.Add(fieldInfo.FieldType);
				}
			}
			foreach (MethodInfo methodInfo in typeof(fsConverterRegistrar).GetDeclaredMethods())
			{
				if (methodInfo.Name.StartsWith("Register_"))
				{
					methodInfo.Invoke(null, null);
				}
			}
			List<Type> list = new List<Type>(fsConverterRegistrar.Converters);
			foreach (Type type in fsConverterRegistrar.Converters)
			{
				object obj = null;
				try
				{
					obj = Activator.CreateInstance(type);
				}
				catch (Exception)
				{
				}
				fsIAotConverter fsIAotConverter = obj as fsIAotConverter;
				if (fsIAotConverter != null && !fsAotCompilationManager.IsAotModelUpToDate(fsMetaType.Get(new fsConfig(), fsIAotConverter.ModelType), fsIAotConverter))
				{
					list.Remove(type);
				}
			}
			fsConverterRegistrar.Converters = list;
		}

		// Token: 0x040015C5 RID: 5573
		public static AnimationCurve_DirectConverter Register_AnimationCurve_DirectConverter;

		// Token: 0x040015C6 RID: 5574
		public static Bounds_DirectConverter Register_Bounds_DirectConverter;

		// Token: 0x040015C7 RID: 5575
		public static GUIStyleState_DirectConverter Register_GUIStyleState_DirectConverter;

		// Token: 0x040015C8 RID: 5576
		public static GUIStyle_DirectConverter Register_GUIStyle_DirectConverter;

		// Token: 0x040015C9 RID: 5577
		public static Gradient_DirectConverter Register_Gradient_DirectConverter;

		// Token: 0x040015CA RID: 5578
		public static Keyframe_DirectConverter Register_Keyframe_DirectConverter;

		// Token: 0x040015CB RID: 5579
		public static LayerMask_DirectConverter Register_LayerMask_DirectConverter;

		// Token: 0x040015CC RID: 5580
		public static RectOffset_DirectConverter Register_RectOffset_DirectConverter;

		// Token: 0x040015CD RID: 5581
		public static Rect_DirectConverter Register_Rect_DirectConverter;

		// Token: 0x040015CE RID: 5582
		public static List<Type> Converters = new List<Type>();
	}
}
