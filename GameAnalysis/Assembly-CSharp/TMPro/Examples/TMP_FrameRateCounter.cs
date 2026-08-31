using System;
using PavonisInteractive.TerraInvicta.SpaceCombat.UI;
using UnityEngine;

namespace TMPro.Examples
{
	// Token: 0x02000507 RID: 1287
	public class TMP_FrameRateCounter : MonoBehaviour
	{
		// Token: 0x06001FCD RID: 8141 RVA: 0x000A5188 File Offset: 0x000A3388
		private void Awake()
		{
			if (!base.enabled)
			{
				return;
			}
			this.m_camera = Camera.main;
			GameObject gameObject = new GameObject("Frame Counter");
			this.m_TextMeshPro = gameObject.AddComponent<TextMeshPro>();
			this.m_TextMeshPro.font = Resources.Load<TMP_FontAsset>("Arcon-Regular SDF");
			this.m_TextMeshPro.fontSharedMaterial = Resources.Load<Material>("Arcon-Regular SDF Material");
			MeshRenderer component = gameObject.GetComponent<MeshRenderer>();
			Material material = global::UnityEngine.Object.Instantiate<Material>(component.material);
			component.material = material;
			component.material.shader = Shader.Find("TextMeshPro/Distance Field Overlay");
			this.m_frameCounter_transform = gameObject.transform;
			this.m_frameCounter_transform.SetParent(this.m_camera.transform);
			this.m_frameCounter_transform.localRotation = Quaternion.identity;
			this.m_TextMeshPro.textWrappingMode = TextWrappingModes.NoWrap;
			this.m_TextMeshPro.fontSize = 16f;
			this.m_TextMeshPro.isOverlay = true;
			this.m_TextMeshPro.sortingOrder = 100;
			this.Set_FrameCounter_Position(this.AnchorPosition);
			this.last_AnchorPosition = this.AnchorPosition;
			if (!TIPlayerProfileManager.showFPS || base.gameObject.GetComponent<SpaceCombatCanvasController>() != null)
			{
				base.enabled = false;
			}
		}

		// Token: 0x06001FCE RID: 8142 RVA: 0x000A52B7 File Offset: 0x000A34B7
		private void Start()
		{
			this.m_LastInterval = Time.realtimeSinceStartup;
			this.m_Frames = 0;
		}

		// Token: 0x06001FCF RID: 8143 RVA: 0x000A52CC File Offset: 0x000A34CC
		private void Update()
		{
			this.last_AnchorPosition = this.AnchorPosition;
			this.m_Frames++;
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			if (realtimeSinceStartup > this.m_LastInterval + this.UpdateInterval)
			{
				float num = (float)this.m_Frames / (realtimeSinceStartup - this.m_LastInterval);
				float num2 = 1000f / Mathf.Max(num, 1E-05f);
				if (num < 30f)
				{
					this.htmlColorTag = "<color=yellow>";
				}
				else if (num < 10f)
				{
					this.htmlColorTag = "<color=red>";
				}
				else
				{
					this.htmlColorTag = "<color=green>";
				}
				this.m_TextMeshPro.SetText(this.htmlColorTag + "{0:2}</color> <#8080ff>FPS \n<#FF8000>{1:2} <#8080ff>MS", num, num2);
				this.m_Frames = 0;
				this.m_LastInterval = realtimeSinceStartup;
			}
		}

		// Token: 0x06001FD0 RID: 8144 RVA: 0x000A538F File Offset: 0x000A358F
		public void Clear()
		{
			this.m_TextMeshPro.text = "";
		}

		// Token: 0x06001FD1 RID: 8145 RVA: 0x000A53A4 File Offset: 0x000A35A4
		private void Set_FrameCounter_Position(TMP_FrameRateCounter.FpsCounterAnchorPositions anchor_position)
		{
			this.m_TextMeshPro.margin = new Vector4(1f, 1f, 1f, 1f);
			switch (anchor_position)
			{
			case TMP_FrameRateCounter.FpsCounterAnchorPositions.TopLeft:
				this.m_TextMeshPro.alignment = TextAlignmentOptions.TopLeft;
				this.m_TextMeshPro.rectTransform.pivot = new Vector2(0f, 1f);
				this.m_frameCounter_transform.position = this.m_camera.ViewportToWorldPoint(new Vector3(0f, 1f, 100f));
				return;
			case TMP_FrameRateCounter.FpsCounterAnchorPositions.BottomLeft:
				this.m_TextMeshPro.alignment = TextAlignmentOptions.BottomLeft;
				this.m_TextMeshPro.rectTransform.pivot = new Vector2(0f, 0f);
				this.m_frameCounter_transform.position = this.m_camera.ViewportToWorldPoint(new Vector3(0.015f, 0f, 100f));
				return;
			case TMP_FrameRateCounter.FpsCounterAnchorPositions.TopRight:
				this.m_TextMeshPro.alignment = TextAlignmentOptions.TopRight;
				this.m_TextMeshPro.rectTransform.pivot = new Vector2(1f, 1f);
				this.m_frameCounter_transform.position = this.m_camera.ViewportToWorldPoint(new Vector3(1f, 1f, 100f));
				return;
			case TMP_FrameRateCounter.FpsCounterAnchorPositions.BottomRight:
				this.m_TextMeshPro.alignment = TextAlignmentOptions.BottomRight;
				this.m_TextMeshPro.rectTransform.pivot = new Vector2(1f, 0f);
				this.m_frameCounter_transform.position = this.m_camera.ViewportToWorldPoint(new Vector3(1f, 0f, 100f));
				return;
			default:
				return;
			}
		}

		// Token: 0x04001882 RID: 6274
		public float UpdateInterval = 5f;

		// Token: 0x04001883 RID: 6275
		private float m_LastInterval;

		// Token: 0x04001884 RID: 6276
		private int m_Frames;

		// Token: 0x04001885 RID: 6277
		public TMP_FrameRateCounter.FpsCounterAnchorPositions AnchorPosition = TMP_FrameRateCounter.FpsCounterAnchorPositions.TopRight;

		// Token: 0x04001886 RID: 6278
		private string htmlColorTag;

		// Token: 0x04001887 RID: 6279
		private const string fpsLabel = "{0:2}</color> <#8080ff>FPS \n<#FF8000>{1:2} <#8080ff>MS";

		// Token: 0x04001888 RID: 6280
		private TextMeshPro m_TextMeshPro;

		// Token: 0x04001889 RID: 6281
		private Transform m_frameCounter_transform;

		// Token: 0x0400188A RID: 6282
		private Camera m_camera;

		// Token: 0x0400188B RID: 6283
		private TMP_FrameRateCounter.FpsCounterAnchorPositions last_AnchorPosition;

		// Token: 0x02000C7B RID: 3195
		public enum FpsCounterAnchorPositions
		{
			// Token: 0x04004E86 RID: 20102
			TopLeft,
			// Token: 0x04004E87 RID: 20103
			BottomLeft,
			// Token: 0x04004E88 RID: 20104
			TopRight,
			// Token: 0x04004E89 RID: 20105
			BottomRight
		}
	}
}
