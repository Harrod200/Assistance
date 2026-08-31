using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200001A RID: 26
public class RandomEffectSequence : AbstractEffectController
{
	// Token: 0x060000B4 RID: 180 RVA: 0x000065DD File Offset: 0x000047DD
	public override void CleanUp()
	{
		this.m_playingEffects.Clear();
		this.m_sequence.Clear();
		this.m_elapsed = 0f;
	}

	// Token: 0x060000B5 RID: 181 RVA: 0x00006600 File Offset: 0x00004800
	protected override void OnPlay()
	{
		foreach (AbstractEffectController abstractEffectController in this.m_targetEffects)
		{
			for (int i = 0; i < this.m_iterations; i++)
			{
				float num = global::UnityEngine.Random.Range(0f, this.m_duration);
				this.m_sequence.Add(new ValueTuple<float, AbstractEffectController>(num, abstractEffectController));
			}
		}
		this.m_sequence.Sort((ValueTuple<float, AbstractEffectController> a, ValueTuple<float, AbstractEffectController> b) => b.Item1.CompareTo(a.Item1));
	}

	// Token: 0x060000B6 RID: 182 RVA: 0x000066AC File Offset: 0x000048AC
	protected override void OnStop()
	{
		for (int i = this.m_playingEffects.Count - 1; i >= 0; i--)
		{
			this.m_playingEffects[i].Stop();
		}
	}

	// Token: 0x060000B7 RID: 183 RVA: 0x000066E4 File Offset: 0x000048E4
	protected override void OnUpdate(float deltaTime)
	{
		if (this.m_sequence.Count == 0)
		{
			if (this.m_playingEffects.Count == 0)
			{
				base.EffectCompleted();
				return;
			}
		}
		else
		{
			this.m_elapsed += deltaTime;
			int num = this.m_sequence.Count - 1;
			ValueTuple<float, AbstractEffectController> valueTuple = this.m_sequence[num];
			float item = valueTuple.Item1;
			AbstractEffectController nextEffect = valueTuple.Item2;
			if (this.m_elapsed >= item)
			{
				if (nextEffect.isActiveAndEnabled)
				{
					nextEffect.OnCompleted += delegate
					{
						this.m_playingEffects.Remove(nextEffect);
					};
					if (!this.m_playingEffects.Contains(nextEffect))
					{
						this.m_playingEffects.Add(nextEffect);
					}
					nextEffect.Play();
				}
				this.m_sequence.RemoveAt(num);
			}
		}
	}

	// Token: 0x060000B8 RID: 184 RVA: 0x000067C6 File Offset: 0x000049C6
	protected override void OnPause()
	{
	}

	// Token: 0x060000B9 RID: 185 RVA: 0x000067C8 File Offset: 0x000049C8
	protected override void OnUnPause()
	{
	}

	// Token: 0x0400009A RID: 154
	[SerializeField]
	private float m_duration = 1f;

	// Token: 0x0400009B RID: 155
	[SerializeField]
	private int m_iterations = 1;

	// Token: 0x0400009C RID: 156
	[SerializeField]
	private List<AbstractEffectController> m_targetEffects;

	// Token: 0x0400009D RID: 157
	private List<ValueTuple<float, AbstractEffectController>> m_sequence = new List<ValueTuple<float, AbstractEffectController>>();

	// Token: 0x0400009E RID: 158
	private float m_elapsed;

	// Token: 0x0400009F RID: 159
	private List<AbstractEffectController> m_playingEffects = new List<AbstractEffectController>();
}
