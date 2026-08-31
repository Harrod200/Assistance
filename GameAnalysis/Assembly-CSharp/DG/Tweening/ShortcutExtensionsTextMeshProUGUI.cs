using System;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UnityEngine;

namespace DG.Tweening
{
	// Token: 0x02000548 RID: 1352
	public static class ShortcutExtensionsTextMeshProUGUI
	{
		// Token: 0x060022AF RID: 8879 RVA: 0x000B3EB8 File Offset: 0x000B20B8
		public static Tweener DOColor(this TextMeshProUGUI target, Color endValue, float duration)
		{
			return DOTween.To(() => target.color, delegate(Color x)
			{
				target.color = x;
			}, endValue, duration).SetTarget(target);
		}

		// Token: 0x060022B0 RID: 8880 RVA: 0x000B3EFC File Offset: 0x000B20FC
		public static Tweener DOFaceColor(this TextMeshProUGUI target, Color32 endValue, float duration)
		{
			return DOTween.To(() => target.faceColor, delegate(Color x)
			{
				target.faceColor = x;
			}, endValue, duration).SetTarget(target);
		}

		// Token: 0x060022B1 RID: 8881 RVA: 0x000B3F48 File Offset: 0x000B2148
		public static Tweener DOOutlineColor(this TextMeshProUGUI target, Color32 endValue, float duration)
		{
			return DOTween.To(() => target.outlineColor, delegate(Color x)
			{
				target.outlineColor = x;
			}, endValue, duration).SetTarget(target);
		}

		// Token: 0x060022B2 RID: 8882 RVA: 0x000B3F91 File Offset: 0x000B2191
		public static Tweener DOGlowColor(this TextMeshProUGUI target, Color endValue, float duration, bool useSharedMaterial = false)
		{
			if (!useSharedMaterial)
			{
				return target.fontMaterial.DOColor(endValue, "_GlowColor", duration).SetTarget(target);
			}
			return target.fontSharedMaterial.DOColor(endValue, "_GlowColor", duration).SetTarget(target);
		}

		// Token: 0x060022B3 RID: 8883 RVA: 0x000B3FC8 File Offset: 0x000B21C8
		public static Tweener DOFade(this TextMeshProUGUI target, float endValue, float duration)
		{
			return DOTween.ToAlpha(() => target.color, delegate(Color x)
			{
				target.color = x;
			}, endValue, duration).SetTarget(target);
		}

		// Token: 0x060022B4 RID: 8884 RVA: 0x000B400C File Offset: 0x000B220C
		public static Tweener DOFaceFade(this TextMeshProUGUI target, float endValue, float duration)
		{
			return DOTween.ToAlpha(() => target.faceColor, delegate(Color x)
			{
				target.faceColor = x;
			}, endValue, duration).SetTarget(target);
		}

		// Token: 0x060022B5 RID: 8885 RVA: 0x000B4050 File Offset: 0x000B2250
		public static Tweener DOScale(this TextMeshProUGUI target, float endValue, float duration)
		{
			Transform t = target.transform;
			Vector3 vector = new Vector3(endValue, endValue, endValue);
			return DOTween.To(() => t.localScale, delegate(Vector3 x)
			{
				t.localScale = x;
			}, vector, duration).SetTarget(target);
		}

		// Token: 0x060022B6 RID: 8886 RVA: 0x000B40A0 File Offset: 0x000B22A0
		public static Tweener DOFontSize(this TextMeshProUGUI target, float endValue, float duration)
		{
			return DOTween.To(() => target.fontSize, delegate(float x)
			{
				target.fontSize = x;
			}, endValue, duration).SetTarget(target);
		}

		// Token: 0x060022B7 RID: 8887 RVA: 0x000B40E4 File Offset: 0x000B22E4
		public static Tweener DOMaxVisibleCharacters(this TextMeshProUGUI target, int endValue, float duration)
		{
			return DOTween.To(() => target.maxVisibleCharacters, delegate(int x)
			{
				target.maxVisibleCharacters = x;
			}, endValue, duration).SetTarget(target);
		}

		// Token: 0x060022B8 RID: 8888 RVA: 0x000B4128 File Offset: 0x000B2328
		public static Tweener DOText(this TextMeshProUGUI target, string endValue, float duration, bool richTextEnabled = true, ScrambleMode scrambleMode = ScrambleMode.None, string scrambleChars = null)
		{
			return DOTween.To(() => target.text, delegate(string x)
			{
				target.text = x;
			}, endValue, duration).SetOptions(richTextEnabled, scrambleMode, scrambleChars).SetTarget(target);
		}
	}
}
