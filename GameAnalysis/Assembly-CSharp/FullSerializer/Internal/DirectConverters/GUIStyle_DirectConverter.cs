using System;
using System.Collections.Generic;
using UnityEngine;

namespace FullSerializer.Internal.DirectConverters
{
	// Token: 0x02000490 RID: 1168
	public class GUIStyle_DirectConverter : fsDirectConverter<GUIStyle>
	{
		// Token: 0x0600190A RID: 6410 RVA: 0x00080D48 File Offset: 0x0007EF48
		protected override fsResult DoSerialize(GUIStyle model, Dictionary<string, fsData> serialized)
		{
			return fsResult.Success + base.SerializeMember<GUIStyleState>(serialized, null, "active", model.active) + base.SerializeMember<TextAnchor>(serialized, null, "alignment", model.alignment) + base.SerializeMember<RectOffset>(serialized, null, "border", model.border) + base.SerializeMember<TextClipping>(serialized, null, "clipping", model.clipping) + base.SerializeMember<Vector2>(serialized, null, "contentOffset", model.contentOffset) + base.SerializeMember<float>(serialized, null, "fixedHeight", model.fixedHeight) + base.SerializeMember<float>(serialized, null, "fixedWidth", model.fixedWidth) + base.SerializeMember<GUIStyleState>(serialized, null, "focused", model.focused) + base.SerializeMember<Font>(serialized, null, "font", model.font) + base.SerializeMember<int>(serialized, null, "fontSize", model.fontSize) + base.SerializeMember<FontStyle>(serialized, null, "fontStyle", model.fontStyle) + base.SerializeMember<GUIStyleState>(serialized, null, "hover", model.hover) + base.SerializeMember<ImagePosition>(serialized, null, "imagePosition", model.imagePosition) + base.SerializeMember<RectOffset>(serialized, null, "margin", model.margin) + base.SerializeMember<string>(serialized, null, "name", model.name) + base.SerializeMember<GUIStyleState>(serialized, null, "normal", model.normal) + base.SerializeMember<GUIStyleState>(serialized, null, "onActive", model.onActive) + base.SerializeMember<GUIStyleState>(serialized, null, "onFocused", model.onFocused) + base.SerializeMember<GUIStyleState>(serialized, null, "onHover", model.onHover) + base.SerializeMember<GUIStyleState>(serialized, null, "onNormal", model.onNormal) + base.SerializeMember<RectOffset>(serialized, null, "overflow", model.overflow) + base.SerializeMember<RectOffset>(serialized, null, "padding", model.padding) + base.SerializeMember<bool>(serialized, null, "richText", model.richText) + base.SerializeMember<bool>(serialized, null, "stretchHeight", model.stretchHeight) + base.SerializeMember<bool>(serialized, null, "stretchWidth", model.stretchWidth) + base.SerializeMember<bool>(serialized, null, "wordWrap", model.wordWrap);
		}

		// Token: 0x0600190B RID: 6411 RVA: 0x00080FCC File Offset: 0x0007F1CC
		protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref GUIStyle model)
		{
			fsResult success = fsResult.Success;
			GUIStyleState active = model.active;
			fsResult fsResult = success + base.DeserializeMember<GUIStyleState>(data, null, "active", out active);
			model.active = active;
			TextAnchor alignment = model.alignment;
			fsResult fsResult2 = fsResult + base.DeserializeMember<TextAnchor>(data, null, "alignment", out alignment);
			model.alignment = alignment;
			RectOffset border = model.border;
			fsResult fsResult3 = fsResult2 + base.DeserializeMember<RectOffset>(data, null, "border", out border);
			model.border = border;
			TextClipping clipping = model.clipping;
			fsResult fsResult4 = fsResult3 + base.DeserializeMember<TextClipping>(data, null, "clipping", out clipping);
			model.clipping = clipping;
			Vector2 contentOffset = model.contentOffset;
			fsResult fsResult5 = fsResult4 + base.DeserializeMember<Vector2>(data, null, "contentOffset", out contentOffset);
			model.contentOffset = contentOffset;
			float fixedHeight = model.fixedHeight;
			fsResult fsResult6 = fsResult5 + base.DeserializeMember<float>(data, null, "fixedHeight", out fixedHeight);
			model.fixedHeight = fixedHeight;
			float fixedWidth = model.fixedWidth;
			fsResult fsResult7 = fsResult6 + base.DeserializeMember<float>(data, null, "fixedWidth", out fixedWidth);
			model.fixedWidth = fixedWidth;
			GUIStyleState focused = model.focused;
			fsResult fsResult8 = fsResult7 + base.DeserializeMember<GUIStyleState>(data, null, "focused", out focused);
			model.focused = focused;
			Font font = model.font;
			fsResult fsResult9 = fsResult8 + base.DeserializeMember<Font>(data, null, "font", out font);
			model.font = font;
			int fontSize = model.fontSize;
			fsResult fsResult10 = fsResult9 + base.DeserializeMember<int>(data, null, "fontSize", out fontSize);
			model.fontSize = fontSize;
			FontStyle fontStyle = model.fontStyle;
			fsResult fsResult11 = fsResult10 + base.DeserializeMember<FontStyle>(data, null, "fontStyle", out fontStyle);
			model.fontStyle = fontStyle;
			GUIStyleState hover = model.hover;
			fsResult fsResult12 = fsResult11 + base.DeserializeMember<GUIStyleState>(data, null, "hover", out hover);
			model.hover = hover;
			ImagePosition imagePosition = model.imagePosition;
			fsResult fsResult13 = fsResult12 + base.DeserializeMember<ImagePosition>(data, null, "imagePosition", out imagePosition);
			model.imagePosition = imagePosition;
			RectOffset margin = model.margin;
			fsResult fsResult14 = fsResult13 + base.DeserializeMember<RectOffset>(data, null, "margin", out margin);
			model.margin = margin;
			string name = model.name;
			fsResult fsResult15 = fsResult14 + base.DeserializeMember<string>(data, null, "name", out name);
			model.name = name;
			GUIStyleState normal = model.normal;
			fsResult fsResult16 = fsResult15 + base.DeserializeMember<GUIStyleState>(data, null, "normal", out normal);
			model.normal = normal;
			GUIStyleState onActive = model.onActive;
			fsResult fsResult17 = fsResult16 + base.DeserializeMember<GUIStyleState>(data, null, "onActive", out onActive);
			model.onActive = onActive;
			GUIStyleState onFocused = model.onFocused;
			fsResult fsResult18 = fsResult17 + base.DeserializeMember<GUIStyleState>(data, null, "onFocused", out onFocused);
			model.onFocused = onFocused;
			GUIStyleState onHover = model.onHover;
			fsResult fsResult19 = fsResult18 + base.DeserializeMember<GUIStyleState>(data, null, "onHover", out onHover);
			model.onHover = onHover;
			GUIStyleState onNormal = model.onNormal;
			fsResult fsResult20 = fsResult19 + base.DeserializeMember<GUIStyleState>(data, null, "onNormal", out onNormal);
			model.onNormal = onNormal;
			RectOffset overflow = model.overflow;
			fsResult fsResult21 = fsResult20 + base.DeserializeMember<RectOffset>(data, null, "overflow", out overflow);
			model.overflow = overflow;
			RectOffset padding = model.padding;
			fsResult fsResult22 = fsResult21 + base.DeserializeMember<RectOffset>(data, null, "padding", out padding);
			model.padding = padding;
			bool richText = model.richText;
			fsResult fsResult23 = fsResult22 + base.DeserializeMember<bool>(data, null, "richText", out richText);
			model.richText = richText;
			bool stretchHeight = model.stretchHeight;
			fsResult fsResult24 = fsResult23 + base.DeserializeMember<bool>(data, null, "stretchHeight", out stretchHeight);
			model.stretchHeight = stretchHeight;
			bool stretchWidth = model.stretchWidth;
			fsResult fsResult25 = fsResult24 + base.DeserializeMember<bool>(data, null, "stretchWidth", out stretchWidth);
			model.stretchWidth = stretchWidth;
			bool wordWrap = model.wordWrap;
			fsResult fsResult26 = fsResult25 + base.DeserializeMember<bool>(data, null, "wordWrap", out wordWrap);
			model.wordWrap = wordWrap;
			return fsResult26;
		}

		// Token: 0x0600190C RID: 6412 RVA: 0x000813B2 File Offset: 0x0007F5B2
		public override object CreateInstance(fsData data, Type storageType)
		{
			return new GUIStyle();
		}
	}
}
