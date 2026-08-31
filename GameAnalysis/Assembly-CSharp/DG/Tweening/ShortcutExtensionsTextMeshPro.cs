using System;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UnityEngine;

namespace DG.Tweening
{
	// Token: 0x02000547 RID: 1351
	public static class ShortcutExtensionsTextMeshPro
	{
		// Token: 0x060022A5 RID: 8869 RVA: 0x000B3BF8 File Offset: 0x000B1DF8
		public static Tweener DOColor(this TextMeshPro target, Color endValue, float duration)
		{
			return DOTween.To(() => target.color, delegate(Color x)
			{
				target.color = x;
			}, endValue, duration).SetTarget(target);
		}

		// Token: 0x060022A6 RID: 8870 RVA: 0x000B3C3C File Offset: 0x000B1E3C
		public static Tweener DOFaceColor(this TextMeshPro target, Color32 endValue, float duration)
		{
			return DOTween.To(() => target.faceColor, delegate(Color x)
			{
				target.faceColor = x;
			}, endValue, duration).SetTarget(target);
		}

		// Token: 0x060022A7 RID: 8871 RVA: 0x000B3C88 File Offset: 0x000B1E88
		public static Tweener DOOutlineColor(this TextMeshPro target, Color32 endValue, float duration)
		{
			return DOTween.To(() => target.outlineColor, delegate(Color x)
			{
				target.outlineColor = x;
			}, endValue, duration).SetTarget(target);
		}

		// Token: 0x060022A8 RID: 8872 RVA: 0x000B3CD1 File Offset: 0x000B1ED1
		public static Tweener DOGlowColor(this TextMeshPro target, Color endValue, float duration, bool useSharedMaterial = false)
		{
			if (!useSharedMaterial)
			{
				return target.fontMaterial.DOColor(endValue, "_GlowColor", duration).SetTarget(target);
			}
			return target.fontSharedMaterial.DOColor(endValue, "_GlowColor", duration).SetTarget(target);
		}

		// Token: 0x060022A9 RID: 8873 RVA: 0x000B3D08 File Offset: 0x000B1F08
		public static Tweener DOFade(this TextMeshPro target, float endValue, float duration)
		{
			return DOTween.ToAlpha(() => target.color, delegate(Color x)
			{
				target.color = x;
			}, endValue, duration).SetTarget(target);
		}

		// Token: 0x060022AA RID: 8874 RVA: 0x000B3D4C File Offset: 0x000B1F4C
		public static Tweener DOFaceFade(this TextMeshPro target, float endValue, float duration)
		{
			return DOTween.ToAlpha(() => target.faceColor, delegate(Color x)
			{
				target.faceColor = x;
			}, endValue, duration).SetTarget(target);
		}

		// Token: 0x060022AB RID: 8875 RVA: 0x000B3D90 File Offset: 0x000B1F90
		public static Tweener DOScale(this TextMeshPro target, float endValue, float duration)
		{
			Transform t = target.transform;
			Vector3 vector = new Vector3(endValue, endValue, endValue);
			return DOTween.To(() => t.localScale, delegate(Vector3 x)
			{
				t.localScale = x;
			}, vector, duration).SetTarget(target);
		}

		// Token: 0x060022AC RID: 8876 RVA: 0x000B3DE0 File Offset: 0x000B1FE0
		public static Tweener DOFontSize(this TextMeshPro target, float endValue, float duration)
		{
			return DOTween.To(() => target.fontSize, delegate(float x)
			{
				target.fontSize = x;
			}, endValue, duration).SetTarget(target);
		}

		// Token: 0x060022AD RID: 8877 RVA: 0x000B3E24 File Offset: 0x000B2024
		public static Tweener DOMaxVisibleCharacters(this TextMeshPro target, int endValue, float duration)
		{
			return DOTween.To(() => target.maxVisibleCharacters, delegate(int x)
			{
				target.maxVisibleCharacters = x;
			}, endValue, duration).SetTarget(target);
		}

		// Token: 0x060022AE RID: 8878 RVA: 0x000B3E68 File Offset: 0x000B2068
		public static Tweener DOText(this TextMeshPro target, string endValue, float duration, bool richTextEnabled = true, ScrambleMode scrambleMode = ScrambleMode.None, string scrambleChars = null)
		{
			return DOTween.To(() => target.text, delegate(string x)
			{
				target.text = x;
			}, endValue, duration).SetOptions(richTextEnabled, scrambleMode, scrambleChars).SetTarget(target);
		}
	}
}
