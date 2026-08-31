using System;
using UnityEngine;

namespace Pixelplacement
{
	// Token: 0x0200051B RID: 1307
	[ExecuteInEditMode]
	public class SplineAnchor : MonoBehaviour
	{
		// Token: 0x1700047B RID: 1147
		// (get) Token: 0x06002038 RID: 8248 RVA: 0x000A79B4 File Offset: 0x000A5BB4
		// (set) Token: 0x06002039 RID: 8249 RVA: 0x000A79BC File Offset: 0x000A5BBC
		public bool RenderingChange { get; set; }

		// Token: 0x1700047C RID: 1148
		// (get) Token: 0x0600203A RID: 8250 RVA: 0x000A79C5 File Offset: 0x000A5BC5
		// (set) Token: 0x0600203B RID: 8251 RVA: 0x000A79CD File Offset: 0x000A5BCD
		public bool Changed { get; set; }

		// Token: 0x1700047D RID: 1149
		// (get) Token: 0x0600203C RID: 8252 RVA: 0x000A79D6 File Offset: 0x000A5BD6
		// (set) Token: 0x0600203D RID: 8253 RVA: 0x000A79EC File Offset: 0x000A5BEC
		public Transform Anchor
		{
			get
			{
				if (!this._initialized)
				{
					this.Initialize();
				}
				return this._anchor;
			}
			private set
			{
				this._anchor = value;
			}
		}

		// Token: 0x1700047E RID: 1150
		// (get) Token: 0x0600203E RID: 8254 RVA: 0x000A79F5 File Offset: 0x000A5BF5
		// (set) Token: 0x0600203F RID: 8255 RVA: 0x000A7A0B File Offset: 0x000A5C0B
		public Transform InTangent
		{
			get
			{
				if (!this._initialized)
				{
					this.Initialize();
				}
				return this._inTangent;
			}
			private set
			{
				this._inTangent = value;
			}
		}

		// Token: 0x1700047F RID: 1151
		// (get) Token: 0x06002040 RID: 8256 RVA: 0x000A7A14 File Offset: 0x000A5C14
		// (set) Token: 0x06002041 RID: 8257 RVA: 0x000A7A2A File Offset: 0x000A5C2A
		public Transform OutTangent
		{
			get
			{
				if (!this._initialized)
				{
					this.Initialize();
				}
				return this._outTangent;
			}
			private set
			{
				this._outTangent = value;
			}
		}

		// Token: 0x06002042 RID: 8258 RVA: 0x000A7A33 File Offset: 0x000A5C33
		private void Awake()
		{
			this.Initialize();
		}

		// Token: 0x06002043 RID: 8259 RVA: 0x000A7A3C File Offset: 0x000A5C3C
		private void Update()
		{
			base.transform.localScale = Vector3.one;
			if (!this._initialized)
			{
				this.Initialize();
			}
			this.Anchor.localPosition = Vector3.zero;
			if (this._previousAnchorPosition != base.transform.position)
			{
				this.Changed = true;
				this.RenderingChange = true;
				this._previousAnchorPosition = base.transform.position;
			}
			if (this._previousTangentMode != this.tangentMode)
			{
				this.Changed = true;
				this.RenderingChange = true;
				this.TangentChanged();
				this._previousTangentMode = this.tangentMode;
			}
			if (this.InTangent.localPosition != this._previousInPosition)
			{
				this.Changed = true;
				this.RenderingChange = true;
				this._previousInPosition = this.InTangent.localPosition;
				this._masterTangent = this.InTangent;
				this._slaveTangent = this.OutTangent;
				this.TangentChanged();
				return;
			}
			if (this.OutTangent.localPosition != this._previousOutPosition)
			{
				this.Changed = true;
				this.RenderingChange = true;
				this._previousOutPosition = this.OutTangent.localPosition;
				this._masterTangent = this.OutTangent;
				this._slaveTangent = this.InTangent;
				this.TangentChanged();
				return;
			}
		}

		// Token: 0x06002044 RID: 8260 RVA: 0x000A7B88 File Offset: 0x000A5D88
		private void TangentChanged()
		{
			switch (this.tangentMode)
			{
			case TangentMode.Mirrored:
			{
				Vector3 vector = this._masterTangent.position - base.transform.position;
				this._slaveTangent.position = base.transform.position - vector;
				break;
			}
			case TangentMode.Aligned:
			{
				float num = Vector3.Distance(this._slaveTangent.position, base.transform.position);
				Vector3 normalized = (this._masterTangent.position - base.transform.position).normalized;
				this._slaveTangent.position = base.transform.position - normalized * num;
				break;
			}
			}
			this._previousInPosition = this.InTangent.localPosition;
			this._previousOutPosition = this.OutTangent.localPosition;
		}

		// Token: 0x06002045 RID: 8261 RVA: 0x000A7C74 File Offset: 0x000A5E74
		private void Initialize()
		{
			this._initialized = true;
			this.InTangent = base.transform.GetChild(0);
			this.OutTangent = base.transform.GetChild(1);
			this.Anchor = base.transform.GetChild(2);
			this._masterTangent = this.InTangent;
			this._slaveTangent = this.OutTangent;
			this.Anchor.hideFlags = HideFlags.HideInHierarchy;
			foreach (Renderer renderer in base.GetComponentsInChildren<Renderer>())
			{
				if (Application.isEditor)
				{
					renderer.sharedMaterial.hideFlags = HideFlags.HideInInspector;
				}
				else
				{
					global::UnityEngine.Object.Destroy(renderer);
				}
			}
			foreach (MeshFilter meshFilter in base.GetComponentsInChildren<MeshFilter>())
			{
				if (Application.isEditor)
				{
					meshFilter.hideFlags = HideFlags.HideInInspector;
				}
				else
				{
					global::UnityEngine.Object.Destroy(meshFilter);
				}
			}
			foreach (MeshRenderer meshRenderer in base.GetComponentsInChildren<MeshRenderer>())
			{
				if (Application.isEditor)
				{
					meshRenderer.hideFlags = HideFlags.HideInInspector;
				}
				else
				{
					global::UnityEngine.Object.Destroy(meshRenderer);
				}
			}
			foreach (SkinnedMeshRenderer skinnedMeshRenderer in base.GetComponentsInChildren<SkinnedMeshRenderer>())
			{
				if (Application.isEditor)
				{
					skinnedMeshRenderer.hideFlags = HideFlags.HideInInspector;
				}
			}
			this._previousTangentMode = this.tangentMode;
			this._previousInPosition = this.InTangent.localPosition;
			this._previousOutPosition = this.OutTangent.localPosition;
			this._previousAnchorPosition = base.transform.position;
		}

		// Token: 0x06002046 RID: 8262 RVA: 0x000A7DE8 File Offset: 0x000A5FE8
		public void SetTangentStatus(bool inStatus, bool outStatus)
		{
			this.InTangent.gameObject.SetActive(inStatus);
			this.OutTangent.gameObject.SetActive(outStatus);
		}

		// Token: 0x06002047 RID: 8263 RVA: 0x000A7E0C File Offset: 0x000A600C
		public void Tilt(Vector3 angles)
		{
			Quaternion localRotation = base.transform.localRotation;
			base.transform.Rotate(angles);
			Vector3 position = this.InTangent.position;
			Vector3 position2 = this.OutTangent.position;
			this.InTangent.position = position;
			this.OutTangent.position = position2;
		}

		// Token: 0x040018F2 RID: 6386
		public TangentMode tangentMode;

		// Token: 0x040018F5 RID: 6389
		private bool _initialized;

		// Token: 0x040018F6 RID: 6390
		[SerializeField]
		[HideInInspector]
		private Transform _masterTangent;

		// Token: 0x040018F7 RID: 6391
		[SerializeField]
		[HideInInspector]
		private Transform _slaveTangent;

		// Token: 0x040018F8 RID: 6392
		private TangentMode _previousTangentMode;

		// Token: 0x040018F9 RID: 6393
		private Vector3 _previousInPosition;

		// Token: 0x040018FA RID: 6394
		private Vector3 _previousOutPosition;

		// Token: 0x040018FB RID: 6395
		private Vector3 _previousAnchorPosition;

		// Token: 0x040018FC RID: 6396
		private Bounds _skinnedBounds;

		// Token: 0x040018FD RID: 6397
		private Transform _anchor;

		// Token: 0x040018FE RID: 6398
		private Transform _inTangent;

		// Token: 0x040018FF RID: 6399
		private Transform _outTangent;
	}
}
