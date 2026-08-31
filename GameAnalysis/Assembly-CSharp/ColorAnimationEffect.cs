using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta.Systems.GameTime;
using Unity.Entities;
using UnityEngine;

// Token: 0x02000013 RID: 19
public class ColorAnimationEffect : AbstractEffectController
{
	// Token: 0x06000085 RID: 133 RVA: 0x0000587A File Offset: 0x00003A7A
	public void Awake()
	{
		this.m_targetUniform = Shader.PropertyToID(this.m_targetUniformName);
	}

	// Token: 0x06000086 RID: 134 RVA: 0x0000588D File Offset: 0x00003A8D
	protected override void OnEnable()
	{
		if (this.m_useScaledGameTimeCheck)
		{
			this.m_gameTime = World.Active.GetExistingManager<GameTimeManager>();
			if (this.m_gameTime != null)
			{
				this.m_useScaledTime = true;
			}
		}
		else
		{
			this.m_useScaledTime = false;
		}
		base.OnEnable();
	}

	// Token: 0x06000087 RID: 135 RVA: 0x000058C5 File Offset: 0x00003AC5
	public override void CleanUp()
	{
		this.m_reversed = false;
		this.m_targetMaterials.Clear();
	}

	// Token: 0x06000088 RID: 136 RVA: 0x000058D9 File Offset: 0x00003AD9
	public void PlayReversed()
	{
		base.Play();
		this.m_reversed = true;
	}

	// Token: 0x06000089 RID: 137 RVA: 0x000058E8 File Offset: 0x00003AE8
	public void Resume()
	{
		this.m_isPlaying = true;
	}

	// Token: 0x0600008A RID: 138 RVA: 0x000058F1 File Offset: 0x00003AF1
	public new void Pause()
	{
		this.m_isPlaying = false;
	}

	// Token: 0x0600008B RID: 139 RVA: 0x000058FC File Offset: 0x00003AFC
	public void SetColors(params Color[] args)
	{
		GradientColorKey[] array = new GradientColorKey[args.Length];
		GradientAlphaKey[] array2 = new GradientAlphaKey[args.Length];
		for (int i = 0; i < args.Length; i++)
		{
			float num = (float)i / (float)(args.Length - 1);
			array[i].color = args[i];
			array[i].time = num;
			array2[i].alpha = args[i].a;
			array2[i].time = num;
		}
		this.m_colorAnimation.SetKeys(array, array2);
	}

	// Token: 0x0600008C RID: 140 RVA: 0x00005988 File Offset: 0x00003B88
	protected override void OnPlay()
	{
		Renderer[] targetRenderers = this.m_targetRenderers;
		for (int i = 0; i < targetRenderers.Length; i++)
		{
			foreach (Material material in targetRenderers[i].materials)
			{
				if (material.HasProperty(this.m_targetUniform))
				{
					this.m_targetMaterials.Add(new ValueTuple<Material, Color>(material, material.GetColor(this.m_targetUniform)));
				}
			}
		}
	}

	// Token: 0x0600008D RID: 141 RVA: 0x000059F4 File Offset: 0x00003BF4
	protected override void OnStop()
	{
	}

	// Token: 0x0600008E RID: 142 RVA: 0x000059F8 File Offset: 0x00003BF8
	protected override void OnUpdate(float deltaTime)
	{
		if (this.m_useScaledTime)
		{
			deltaTime *= this.m_gameTime.currentSpeed;
		}
		if (this.m_reversed)
		{
			this.m_progress -= deltaTime / this.m_duration;
		}
		else
		{
			this.m_progress += deltaTime / this.m_duration;
		}
		Color color = this.m_colorAnimation.Evaluate(this.m_progress);
		float num = (this.m_squareIntensity ? Mathf.Pow(2f, this.m_intensityAnimation.Evaluate(this.m_progress)) : this.m_intensityAnimation.Evaluate(this.m_progress));
		switch (this.m_blendMode)
		{
		case ColorAnimationEffect.ColorBlendMode.OVERRIDE:
		{
			using (List<ValueTuple<Material, Color>>.Enumerator enumerator = this.m_targetMaterials.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ValueTuple<Material, Color> valueTuple = enumerator.Current;
					Material item = valueTuple.Item1;
					if (!(item == null))
					{
						Color color2 = color;
						color2 = new Color(color2.r * num, color2.g * num, color2.b * num, color2.a);
						item.SetColor(this.m_targetUniform, color2);
					}
				}
				goto IL_0246;
			}
			break;
		}
		case ColorAnimationEffect.ColorBlendMode.ADDITIVE:
			break;
		case ColorAnimationEffect.ColorBlendMode.MULTIPLICATIVE:
			goto IL_01BD;
		default:
			goto IL_0246;
		}
		using (List<ValueTuple<Material, Color>>.Enumerator enumerator = this.m_targetMaterials.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				ValueTuple<Material, Color> valueTuple2 = enumerator.Current;
				Material item2 = valueTuple2.Item1;
				Color item3 = valueTuple2.Item2;
				if (!(item2 == null))
				{
					Color color3 = color + item3;
					color3 = new Color(color3.r * num, color3.g * num, color3.b * num, color3.a);
					item2.SetColor(this.m_targetUniform, color3);
				}
			}
			goto IL_0246;
		}
		IL_01BD:
		foreach (ValueTuple<Material, Color> valueTuple3 in this.m_targetMaterials)
		{
			Material item4 = valueTuple3.Item1;
			Color item5 = valueTuple3.Item2;
			if (!(item4 == null))
			{
				Color color4 = color * item5;
				color4 = new Color(color4.r * num, color4.g * num, color4.b * num, color4.a);
				item4.SetColor(this.m_targetUniform, color4);
			}
		}
		IL_0246:
		if (this.m_reversed)
		{
			if (this.m_progress <= 0f)
			{
				base.EffectCompleted();
				return;
			}
		}
		else if (this.m_progress >= 1f)
		{
			base.EffectCompleted();
		}
	}

	// Token: 0x0600008F RID: 143 RVA: 0x00005CA4 File Offset: 0x00003EA4
	protected override void OnPause()
	{
	}

	// Token: 0x06000090 RID: 144 RVA: 0x00005CA6 File Offset: 0x00003EA6
	protected override void OnUnPause()
	{
	}

	// Token: 0x04000072 RID: 114
	[SerializeField]
	[Tooltip("This value is only checked OnEnable and will be ignored any time after.")]
	private bool m_useScaledGameTimeCheck;

	// Token: 0x04000073 RID: 115
	[SerializeField]
	private bool m_squareIntensity = true;

	// Token: 0x04000074 RID: 116
	[SerializeField]
	private ColorAnimationEffect.ColorBlendMode m_blendMode = ColorAnimationEffect.ColorBlendMode.MULTIPLICATIVE;

	// Token: 0x04000075 RID: 117
	[SerializeField]
	[GradientUsage(true)]
	private Gradient m_colorAnimation;

	// Token: 0x04000076 RID: 118
	[SerializeField]
	private AnimationCurve m_intensityAnimation = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 1f),
		new Keyframe(1f, 0f)
	});

	// Token: 0x04000077 RID: 119
	[SerializeField]
	private float m_duration = 1f;

	// Token: 0x04000078 RID: 120
	[SerializeField]
	private Renderer[] m_targetRenderers = new Renderer[0];

	// Token: 0x04000079 RID: 121
	[SerializeField]
	private string m_targetUniformName = "_EmissionColor";

	// Token: 0x0400007A RID: 122
	private int m_targetUniform;

	// Token: 0x0400007B RID: 123
	private List<ValueTuple<Material, Color>> m_targetMaterials = new List<ValueTuple<Material, Color>>();

	// Token: 0x0400007C RID: 124
	private float m_progress;

	// Token: 0x0400007D RID: 125
	private bool m_reversed;

	// Token: 0x0400007E RID: 126
	private bool m_useScaledTime;

	// Token: 0x0400007F RID: 127
	private GameTimeManager m_gameTime;

	// Token: 0x02000AB7 RID: 2743
	public enum ColorBlendMode
	{
		// Token: 0x04004843 RID: 18499
		OVERRIDE,
		// Token: 0x04004844 RID: 18500
		ADDITIVE,
		// Token: 0x04004845 RID: 18501
		MULTIPLICATIVE
	}
}
