using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Vectrosity
{
	// Token: 0x020004A2 RID: 1186
	[Serializable]
	public class VectorLine
	{
		// Token: 0x0600194B RID: 6475 RVA: 0x00085441 File Offset: 0x00083641
		public static string Version()
		{
			return "Vectrosity version 5.6";
		}

		// Token: 0x1700036C RID: 876
		// (get) Token: 0x0600194C RID: 6476 RVA: 0x00085448 File Offset: 0x00083648
		public Vector3[] lineVertices
		{
			get
			{
				return this.m_lineVertices;
			}
		}

		// Token: 0x1700036D RID: 877
		// (get) Token: 0x0600194D RID: 6477 RVA: 0x00085450 File Offset: 0x00083650
		public Vector2[] lineUVs
		{
			get
			{
				return this.m_lineUVs;
			}
		}

		// Token: 0x1700036E RID: 878
		// (get) Token: 0x0600194E RID: 6478 RVA: 0x00085458 File Offset: 0x00083658
		public Color[] lineColors
		{
			get
			{
				return this.m_lineColors;
			}
		}

		// Token: 0x1700036F RID: 879
		// (get) Token: 0x0600194F RID: 6479 RVA: 0x00085460 File Offset: 0x00083660
		public List<int> lineTriangles
		{
			get
			{
				return this.m_lineTriangles;
			}
		}

		// Token: 0x17000370 RID: 880
		// (get) Token: 0x06001950 RID: 6480 RVA: 0x00085468 File Offset: 0x00083668
		public RectTransform rectTransform
		{
			get
			{
				if (this.m_go != null)
				{
					return this.m_rectTransform;
				}
				return null;
			}
		}

		// Token: 0x17000371 RID: 881
		// (get) Token: 0x06001951 RID: 6481 RVA: 0x00085480 File Offset: 0x00083680
		// (set) Token: 0x06001952 RID: 6482 RVA: 0x00085488 File Offset: 0x00083688
		public Color color
		{
			get
			{
				return this.m_color;
			}
			set
			{
				this.m_color = value;
				this.SetColor(value);
			}
		}

		// Token: 0x17000372 RID: 882
		// (get) Token: 0x06001953 RID: 6483 RVA: 0x00085498 File Offset: 0x00083698
		public bool is2D
		{
			get
			{
				return this.m_is2D;
			}
		}

		// Token: 0x17000373 RID: 883
		// (get) Token: 0x06001954 RID: 6484 RVA: 0x000854A0 File Offset: 0x000836A0
		// (set) Token: 0x06001955 RID: 6485 RVA: 0x000854CC File Offset: 0x000836CC
		public List<Vector2> points2
		{
			get
			{
				if (!this.m_is2D)
				{
					Debug.LogError("Line \"" + this.name + "\" uses points3 rather than points2");
					return null;
				}
				return this.m_points2;
			}
			set
			{
				if (value == null)
				{
					Debug.LogError("List for Line \"" + this.name + "\" must not be null");
					return;
				}
				this.m_points2 = value;
			}
		}

		// Token: 0x17000374 RID: 884
		// (get) Token: 0x06001956 RID: 6486 RVA: 0x000854F3 File Offset: 0x000836F3
		// (set) Token: 0x06001957 RID: 6487 RVA: 0x0008551F File Offset: 0x0008371F
		public List<Vector3> points3
		{
			get
			{
				if (this.m_is2D)
				{
					Debug.LogError("Line \"" + this.name + "\" uses points2 rather than points3");
					return null;
				}
				return this.m_points3;
			}
			set
			{
				if (value == null)
				{
					Debug.LogError("List for Line \"" + this.name + "\" must not be null");
					return;
				}
				this.m_points3 = value;
			}
		}

		// Token: 0x17000375 RID: 885
		// (get) Token: 0x06001958 RID: 6488 RVA: 0x00085546 File Offset: 0x00083746
		private int pointsCount
		{
			get
			{
				if (!this.m_is2D)
				{
					return this.m_points3.Count;
				}
				return this.m_points2.Count;
			}
		}

		// Token: 0x17000376 RID: 886
		// (get) Token: 0x06001959 RID: 6489 RVA: 0x00085567 File Offset: 0x00083767
		// (set) Token: 0x0600195A RID: 6490 RVA: 0x00085570 File Offset: 0x00083770
		public float lineWidth
		{
			get
			{
				return this.m_lineWidth;
			}
			set
			{
				this.m_lineWidth = value;
				float num = value * 0.5f;
				for (int i = 0; i < this.m_lineWidths.Length; i++)
				{
					this.m_lineWidths[i] = num;
				}
				this.m_maxWeldDistance = value * 2f * (value * 2f);
			}
		}

		// Token: 0x17000377 RID: 887
		// (get) Token: 0x0600195B RID: 6491 RVA: 0x000855BD File Offset: 0x000837BD
		// (set) Token: 0x0600195C RID: 6492 RVA: 0x000855CA File Offset: 0x000837CA
		public float maxWeldDistance
		{
			get
			{
				return Mathf.Sqrt(this.m_maxWeldDistance);
			}
			set
			{
				this.m_maxWeldDistance = value * value;
			}
		}

		// Token: 0x17000378 RID: 888
		// (get) Token: 0x0600195D RID: 6493 RVA: 0x000855D5 File Offset: 0x000837D5
		// (set) Token: 0x0600195E RID: 6494 RVA: 0x000855DD File Offset: 0x000837DD
		public string name
		{
			get
			{
				return this.m_name;
			}
			set
			{
				this.m_name = value;
				if (this.m_go != null)
				{
					this.m_go.name = value;
				}
				if (this.m_vectorObject != null)
				{
					this.m_vectorObject.SetName(value);
				}
			}
		}

		// Token: 0x17000379 RID: 889
		// (get) Token: 0x0600195F RID: 6495 RVA: 0x00085614 File Offset: 0x00083814
		// (set) Token: 0x06001960 RID: 6496 RVA: 0x0008561C File Offset: 0x0008381C
		public Material material
		{
			get
			{
				return this.m_material;
			}
			set
			{
				if (this.m_vectorObject != null)
				{
					this.m_vectorObject.SetMaterial(value);
				}
				this.m_material = value;
				this.m_useCustomMaterial = true;
			}
		}

		// Token: 0x1700037A RID: 890
		// (get) Token: 0x06001961 RID: 6497 RVA: 0x00085640 File Offset: 0x00083840
		// (set) Token: 0x06001962 RID: 6498 RVA: 0x00085648 File Offset: 0x00083848
		public Texture texture
		{
			get
			{
				return this.m_texture;
			}
			set
			{
				if (this.m_capType != EndCap.None)
				{
					this.m_originalTexture = value;
					return;
				}
				if (this.m_vectorObject != null)
				{
					this.m_vectorObject.SetTexture(value);
				}
				this.m_texture = value;
			}
		}

		// Token: 0x1700037B RID: 891
		// (get) Token: 0x06001963 RID: 6499 RVA: 0x00085676 File Offset: 0x00083876
		// (set) Token: 0x06001964 RID: 6500 RVA: 0x00085693 File Offset: 0x00083893
		public int layer
		{
			get
			{
				if (this.m_go != null)
				{
					return this.m_go.layer;
				}
				return 0;
			}
			set
			{
				if (this.m_go != null)
				{
					this.m_go.layer = Mathf.Clamp(value, 0, 31);
				}
			}
		}

		// Token: 0x1700037C RID: 892
		// (get) Token: 0x06001965 RID: 6501 RVA: 0x000856B7 File Offset: 0x000838B7
		// (set) Token: 0x06001966 RID: 6502 RVA: 0x000856BF File Offset: 0x000838BF
		public bool active
		{
			get
			{
				return this.m_active;
			}
			set
			{
				this.m_active = value;
				if (this.m_vectorObject != null)
				{
					this.m_vectorObject.Enable(value);
				}
			}
		}

		// Token: 0x1700037D RID: 893
		// (get) Token: 0x06001967 RID: 6503 RVA: 0x000856DC File Offset: 0x000838DC
		// (set) Token: 0x06001968 RID: 6504 RVA: 0x000856E4 File Offset: 0x000838E4
		public LineType lineType
		{
			get
			{
				return this.m_lineType;
			}
			set
			{
				if (value != this.m_lineType)
				{
					this.m_lineType = value;
					if (value == LineType.Points || (value == LineType.Discrete && this.m_joins == Joins.Fill))
					{
						this.m_joins = Joins.None;
					}
					if (value == LineType.Discrete)
					{
						this.drawStart = this.m_drawStart;
						this.drawEnd = this.m_drawEnd;
					}
					if (value != LineType.Continuous && ((this.m_points2 != null && this.m_points2.Count > 16383) || (this.m_points3 != null && this.m_points3.Count > 16383)))
					{
						this.Resize(16383);
					}
					if (this.collider)
					{
						Collider2D component = this.m_go.GetComponent<Collider2D>();
						if (component != null)
						{
							global::UnityEngine.Object.DestroyImmediate(component);
						}
						this.AddColliderIfNeeded();
					}
					this.ResetLine();
				}
			}
		}

		// Token: 0x1700037E RID: 894
		// (get) Token: 0x06001969 RID: 6505 RVA: 0x000857A8 File Offset: 0x000839A8
		// (set) Token: 0x0600196A RID: 6506 RVA: 0x000857B0 File Offset: 0x000839B0
		public float capLength
		{
			get
			{
				return this.m_capLength;
			}
			set
			{
				if (this.m_lineType == LineType.Points)
				{
					Debug.LogError("LineType.Points can't use capLength");
					return;
				}
				this.m_capLength = value;
			}
		}

		// Token: 0x1700037F RID: 895
		// (get) Token: 0x0600196B RID: 6507 RVA: 0x000857CD File Offset: 0x000839CD
		// (set) Token: 0x0600196C RID: 6508 RVA: 0x000857D5 File Offset: 0x000839D5
		public bool smoothWidth
		{
			get
			{
				return this.m_smoothWidth;
			}
			set
			{
				this.m_smoothWidth = this.m_lineType != LineType.Points && value;
			}
		}

		// Token: 0x17000380 RID: 896
		// (get) Token: 0x0600196D RID: 6509 RVA: 0x000857EA File Offset: 0x000839EA
		// (set) Token: 0x0600196E RID: 6510 RVA: 0x000857F4 File Offset: 0x000839F4
		public bool smoothColor
		{
			get
			{
				return this.m_smoothColor;
			}
			set
			{
				bool smoothColor = this.m_smoothColor;
				this.m_smoothColor = this.m_lineType != LineType.Points && value;
				if (this.m_smoothColor != smoothColor)
				{
					int segmentNumber = this.GetSegmentNumber();
					for (int i = 0; i < segmentNumber; i++)
					{
						this.SetColor(this.GetColor(i), i);
					}
				}
			}
		}

		// Token: 0x17000381 RID: 897
		// (get) Token: 0x0600196F RID: 6511 RVA: 0x00085845 File Offset: 0x00083A45
		// (set) Token: 0x06001970 RID: 6512 RVA: 0x00085850 File Offset: 0x00083A50
		public Joins joins
		{
			get
			{
				return this.m_joins;
			}
			set
			{
				if (this.m_lineType == LineType.Points || (this.m_lineType == LineType.Discrete && value == Joins.Fill))
				{
					return;
				}
				if ((this.m_joins == Joins.Fill && value != Joins.Fill) || (this.m_joins != Joins.Fill && value == Joins.Fill))
				{
					this.m_joins = value;
					this.ClearTriangles();
					this.SetupTriangles(0);
				}
				this.m_joins = value;
				if (this.m_joins == Joins.Weld)
				{
					if (this.m_canvasState == CanvasState.OnCanvas)
					{
						this.Draw();
						return;
					}
					if (this.m_canvasState == CanvasState.OffCanvas)
					{
						this.Draw3D();
					}
				}
			}
		}

		// Token: 0x17000382 RID: 898
		// (get) Token: 0x06001971 RID: 6513 RVA: 0x000858CC File Offset: 0x00083ACC
		public bool isAutoDrawing
		{
			get
			{
				return this.m_isAutoDrawing;
			}
		}

		// Token: 0x17000383 RID: 899
		// (get) Token: 0x06001972 RID: 6514 RVA: 0x000858D4 File Offset: 0x00083AD4
		// (set) Token: 0x06001973 RID: 6515 RVA: 0x000858DC File Offset: 0x00083ADC
		public int drawStart
		{
			get
			{
				return this.m_drawStart;
			}
			set
			{
				if (this.m_lineType == LineType.Discrete && (value & 1) != 0)
				{
					value++;
				}
				this.m_drawStart = Mathf.Clamp(value, 0, this.pointsCount - 1);
			}
		}

		// Token: 0x17000384 RID: 900
		// (get) Token: 0x06001974 RID: 6516 RVA: 0x00085906 File Offset: 0x00083B06
		// (set) Token: 0x06001975 RID: 6517 RVA: 0x0008590E File Offset: 0x00083B0E
		public int drawEnd
		{
			get
			{
				return this.m_drawEnd;
			}
			set
			{
				if (this.m_lineType == LineType.Discrete && value != 0 && (value & 1) == 0)
				{
					value++;
				}
				this.m_drawEnd = Mathf.Clamp(value, 0, this.pointsCount - 1);
			}
		}

		// Token: 0x17000385 RID: 901
		// (get) Token: 0x06001976 RID: 6518 RVA: 0x0008593B File Offset: 0x00083B3B
		// (set) Token: 0x06001977 RID: 6519 RVA: 0x0008595F File Offset: 0x00083B5F
		public int endPointsUpdate
		{
			get
			{
				if (this.m_lineType != LineType.Discrete)
				{
					return this.m_endPointsUpdate;
				}
				if (this.m_endPointsUpdate != 0)
				{
					return this.m_endPointsUpdate + 1;
				}
				return 0;
			}
			set
			{
				if (this.m_lineType == LineType.Discrete && value > 1 && (value & 1) == 0)
				{
					value--;
				}
				this.m_endPointsUpdate = Mathf.Max(0, value);
			}
		}

		// Token: 0x17000386 RID: 902
		// (get) Token: 0x06001978 RID: 6520 RVA: 0x00085985 File Offset: 0x00083B85
		// (set) Token: 0x06001979 RID: 6521 RVA: 0x00085990 File Offset: 0x00083B90
		public string endCap
		{
			get
			{
				return this.m_endCap;
			}
			set
			{
				if (this.m_lineType == LineType.Points)
				{
					Debug.LogError("LineType.Points can't use end caps");
					return;
				}
				if (this.m_endCap == value)
				{
					return;
				}
				if (value == null || value == "")
				{
					this.RemoveEndCap();
					return;
				}
				if (VectorLine.capDictionary == null || !VectorLine.capDictionary.ContainsKey(value))
				{
					Debug.LogError("End cap \"" + value + "\" is not set up");
					return;
				}
				if (this.m_capType != EndCap.None)
				{
					this.RemoveEndCap();
				}
				this.m_endCap = value;
				this.m_capType = VectorLine.capDictionary[value].capType;
				if (this.m_capType != EndCap.None)
				{
					this.SetupEndCap(VectorLine.capDictionary[value].uvHeights);
				}
			}
		}

		// Token: 0x17000387 RID: 903
		// (get) Token: 0x0600197A RID: 6522 RVA: 0x00085A4C File Offset: 0x00083C4C
		// (set) Token: 0x0600197B RID: 6523 RVA: 0x00085A54 File Offset: 0x00083C54
		public bool continuousTexture
		{
			get
			{
				return this.m_continuousTexture;
			}
			set
			{
				this.m_continuousTexture = value;
				if (!value)
				{
					this.ResetTextureScale();
				}
			}
		}

		// Token: 0x17000388 RID: 904
		// (get) Token: 0x0600197C RID: 6524 RVA: 0x00085A66 File Offset: 0x00083C66
		// (set) Token: 0x0600197D RID: 6525 RVA: 0x00085A6E File Offset: 0x00083C6E
		public Transform drawTransform
		{
			get
			{
				return this.m_drawTransform;
			}
			set
			{
				this.m_drawTransform = value;
			}
		}

		// Token: 0x17000389 RID: 905
		// (get) Token: 0x0600197E RID: 6526 RVA: 0x00085A77 File Offset: 0x00083C77
		// (set) Token: 0x0600197F RID: 6527 RVA: 0x00085A7F File Offset: 0x00083C7F
		public bool useViewportCoords
		{
			get
			{
				return this.m_viewportDraw;
			}
			set
			{
				if (this.m_is2D)
				{
					this.m_viewportDraw = value;
					return;
				}
				Debug.LogError("Line must use Vector2 points in order to use viewport coords");
			}
		}

		// Token: 0x1700038A RID: 906
		// (get) Token: 0x06001980 RID: 6528 RVA: 0x00085A9B File Offset: 0x00083C9B
		// (set) Token: 0x06001981 RID: 6529 RVA: 0x00085AA3 File Offset: 0x00083CA3
		[SerializeField]
		public float textureScale
		{
			get
			{
				return this.m_textureScale;
			}
			set
			{
				this.m_textureScale = value;
				if (this.m_textureScale == 0f)
				{
					this.m_useTextureScale = false;
					this.ResetTextureScale();
					return;
				}
				this.m_useTextureScale = true;
			}
		}

		// Token: 0x1700038B RID: 907
		// (get) Token: 0x06001982 RID: 6530 RVA: 0x00085ACE File Offset: 0x00083CCE
		// (set) Token: 0x06001983 RID: 6531 RVA: 0x00085AD6 File Offset: 0x00083CD6
		public float textureOffset
		{
			get
			{
				return this.m_textureOffset;
			}
			set
			{
				this.m_textureOffset = value;
				this.SetTextureScale();
			}
		}

		// Token: 0x1700038C RID: 908
		// (get) Token: 0x06001984 RID: 6532 RVA: 0x00085AE5 File Offset: 0x00083CE5
		// (set) Token: 0x06001985 RID: 6533 RVA: 0x00085AED File Offset: 0x00083CED
		public Matrix4x4 matrix
		{
			get
			{
				return this.m_matrix;
			}
			set
			{
				this.m_matrix = value;
				this.m_useMatrix = this.m_matrix != Matrix4x4.identity;
			}
		}

		// Token: 0x1700038D RID: 909
		// (get) Token: 0x06001986 RID: 6534 RVA: 0x00085B0C File Offset: 0x00083D0C
		// (set) Token: 0x06001987 RID: 6535 RVA: 0x00085B33 File Offset: 0x00083D33
		public int drawDepth
		{
			get
			{
				if (this.m_canvasState == CanvasState.OffCanvas)
				{
					Debug.LogError("VectorLine.drawDepth can't be used with lines made with Draw3D");
					return 0;
				}
				return this.m_go.transform.GetSiblingIndex();
			}
			set
			{
				if (this.m_canvasState == CanvasState.OffCanvas)
				{
					Debug.LogError("VectorLine.drawDepth can't be used with lines made with Draw3D");
					return;
				}
				this.m_go.transform.SetSiblingIndex(value);
			}
		}

		// Token: 0x1700038E RID: 910
		// (get) Token: 0x06001988 RID: 6536 RVA: 0x00085B5A File Offset: 0x00083D5A
		// (set) Token: 0x06001989 RID: 6537 RVA: 0x00085B62 File Offset: 0x00083D62
		public bool collider
		{
			get
			{
				return this.m_collider;
			}
			set
			{
				this.m_collider = value;
				this.AddColliderIfNeeded();
				this.m_go.GetComponent<Collider2D>().enabled = value;
			}
		}

		// Token: 0x1700038F RID: 911
		// (get) Token: 0x0600198A RID: 6538 RVA: 0x00085B82 File Offset: 0x00083D82
		// (set) Token: 0x0600198B RID: 6539 RVA: 0x00085B8A File Offset: 0x00083D8A
		public bool trigger
		{
			get
			{
				return this.m_trigger;
			}
			set
			{
				this.m_trigger = value;
				if (this.m_go.GetComponent<Collider2D>() != null)
				{
					this.m_go.GetComponent<Collider2D>().isTrigger = value;
				}
			}
		}

		// Token: 0x17000390 RID: 912
		// (get) Token: 0x0600198C RID: 6540 RVA: 0x00085BB7 File Offset: 0x00083DB7
		// (set) Token: 0x0600198D RID: 6541 RVA: 0x00085BBF File Offset: 0x00083DBF
		public PhysicsMaterial2D physicsMaterial
		{
			get
			{
				return this.m_physicsMaterial;
			}
			set
			{
				this.AddColliderIfNeeded();
				this.m_physicsMaterial = value;
				this.m_go.GetComponent<Collider2D>().sharedMaterial = value;
			}
		}

		// Token: 0x17000391 RID: 913
		// (get) Token: 0x0600198E RID: 6542 RVA: 0x00085BDF File Offset: 0x00083DDF
		// (set) Token: 0x0600198F RID: 6543 RVA: 0x00085BE8 File Offset: 0x00083DE8
		public bool alignOddWidthToPixels
		{
			get
			{
				return this.m_alignOddWidthToPixels;
			}
			set
			{
				float num = (value ? 0.5f : 0f);
				this.m_rectTransform.anchoredPosition = new Vector2(num, num);
				this.m_alignOddWidthToPixels = value;
			}
		}

		// Token: 0x17000392 RID: 914
		// (get) Token: 0x06001990 RID: 6544 RVA: 0x00085C1E File Offset: 0x00083E1E
		public static Canvas canvas
		{
			get
			{
				if (VectorLine.m_canvas == null)
				{
					VectorLine.SetupVectorCanvas();
				}
				return VectorLine.m_canvas;
			}
		}

		// Token: 0x17000393 RID: 915
		// (get) Token: 0x06001991 RID: 6545 RVA: 0x00085C38 File Offset: 0x00083E38
		public static Vector3 camTransformPosition
		{
			get
			{
				return VectorLine.camTransform.position;
			}
		}

		// Token: 0x17000394 RID: 916
		// (get) Token: 0x06001992 RID: 6546 RVA: 0x00085C44 File Offset: 0x00083E44
		public static bool camTransformExists
		{
			get
			{
				return VectorLine.camTransform != null;
			}
		}

		// Token: 0x17000395 RID: 917
		// (get) Token: 0x06001993 RID: 6547 RVA: 0x00085C51 File Offset: 0x00083E51
		public static LineManager lineManager
		{
			get
			{
				if (!VectorLine.lineManagerCreated)
				{
					VectorLine.lineManagerCreated = true;
					VectorLine.m_lineManager = new GameObject("LineManager").AddComponent<LineManager>();
					VectorLine.m_lineManager.enabled = false;
					global::UnityEngine.Object.DontDestroyOnLoad(VectorLine.m_lineManager);
				}
				return VectorLine.m_lineManager;
			}
		}

		// Token: 0x06001994 RID: 6548 RVA: 0x00085C90 File Offset: 0x00083E90
		private void AddColliderIfNeeded()
		{
			if (this.m_go.GetComponent<Collider2D>() == null)
			{
				this.m_go.AddComponent((this.m_lineType == LineType.Continuous) ? typeof(EdgeCollider2D) : typeof(PolygonCollider2D));
				this.m_go.GetComponent<Collider2D>().isTrigger = this.m_trigger;
				this.m_go.GetComponent<Collider2D>().sharedMaterial = this.m_physicsMaterial;
			}
		}

		// Token: 0x06001995 RID: 6549 RVA: 0x00085D06 File Offset: 0x00083F06
		public VectorLine(string name, List<Vector3> points, float width)
		{
			this.m_points3 = points;
			this.SetupLine(name, null, width, LineType.Discrete, Joins.None, false);
		}

		// Token: 0x06001996 RID: 6550 RVA: 0x00085D3D File Offset: 0x00083F3D
		public VectorLine(string name, List<Vector3> points, Texture texture, float width)
		{
			this.m_points3 = points;
			this.SetupLine(name, texture, width, LineType.Discrete, Joins.None, false);
		}

		// Token: 0x06001997 RID: 6551 RVA: 0x00085D75 File Offset: 0x00083F75
		public VectorLine(string name, List<Vector3> points, float width, LineType lineType)
		{
			this.m_points3 = points;
			this.SetupLine(name, null, width, lineType, Joins.None, false);
		}

		// Token: 0x06001998 RID: 6552 RVA: 0x00085DAD File Offset: 0x00083FAD
		public VectorLine(string name, List<Vector3> points, Texture texture, float width, LineType lineType)
		{
			this.m_points3 = points;
			this.SetupLine(name, texture, width, lineType, Joins.None, false);
		}

		// Token: 0x06001999 RID: 6553 RVA: 0x00085DE6 File Offset: 0x00083FE6
		public VectorLine(string name, List<Vector3> points, float width, LineType lineType, Joins joins)
		{
			this.m_points3 = points;
			this.SetupLine(name, null, width, lineType, joins, false);
		}

		// Token: 0x0600199A RID: 6554 RVA: 0x00085E1F File Offset: 0x0008401F
		public VectorLine(string name, List<Vector3> points, Texture texture, float width, LineType lineType, Joins joins)
		{
			this.m_points3 = points;
			this.SetupLine(name, texture, width, lineType, joins, false);
		}

		// Token: 0x0600199B RID: 6555 RVA: 0x00085E59 File Offset: 0x00084059
		public VectorLine(string name, List<Vector2> points, float width)
		{
			this.m_points2 = points;
			this.SetupLine(name, null, width, LineType.Discrete, Joins.None, true);
		}

		// Token: 0x0600199C RID: 6556 RVA: 0x00085E90 File Offset: 0x00084090
		public VectorLine(string name, List<Vector2> points, Texture texture, float width)
		{
			this.m_points2 = points;
			this.SetupLine(name, texture, width, LineType.Discrete, Joins.None, true);
		}

		// Token: 0x0600199D RID: 6557 RVA: 0x00085EC8 File Offset: 0x000840C8
		public VectorLine(string name, List<Vector2> points, float width, LineType lineType)
		{
			this.m_points2 = points;
			this.SetupLine(name, null, width, lineType, Joins.None, true);
		}

		// Token: 0x0600199E RID: 6558 RVA: 0x00085F00 File Offset: 0x00084100
		public VectorLine(string name, List<Vector2> points, Texture texture, float width, LineType lineType)
		{
			this.m_points2 = points;
			this.SetupLine(name, texture, width, lineType, Joins.None, true);
		}

		// Token: 0x0600199F RID: 6559 RVA: 0x00085F39 File Offset: 0x00084139
		public VectorLine(string name, List<Vector2> points, float width, LineType lineType, Joins joins)
		{
			this.m_points2 = points;
			this.SetupLine(name, null, width, lineType, joins, true);
		}

		// Token: 0x060019A0 RID: 6560 RVA: 0x00085F72 File Offset: 0x00084172
		public VectorLine(string name, List<Vector2> points, Texture texture, float width, LineType lineType, Joins joins)
		{
			this.m_points2 = points;
			this.SetupLine(name, texture, width, lineType, joins, true);
		}

		// Token: 0x060019A1 RID: 6561 RVA: 0x00085FAC File Offset: 0x000841AC
		protected void SetupLine(string lineName, Texture texture, float width, LineType lineType, Joins joins, bool use2D)
		{
			this.m_is2D = use2D;
			this.m_lineType = lineType;
			if (joins == Joins.Fill && this.m_lineType != LineType.Continuous)
			{
				Debug.LogError("VectorLine: Must use LineType.Continuous if using Joins.Fill for \"" + lineName + "\"");
				return;
			}
			if (joins == Joins.Weld && this.m_lineType == LineType.Points)
			{
				Debug.LogError("VectorLine: LineType.Points can't use Joins.Weld for \"" + lineName + "\"");
				return;
			}
			if ((this.m_is2D && this.m_points2 == null) || (!this.m_is2D && this.m_points3 == null))
			{
				Debug.LogError("VectorLine: the points array is null for \"" + lineName + "\"");
				return;
			}
			if (this.m_is2D)
			{
				this.m_pointsCount = ((this.m_points2.Capacity > 0 && this.m_points2.Count == 0) ? this.m_points2.Capacity : this.m_points2.Count);
				int num = this.m_pointsCount - this.m_points2.Count;
				for (int i = 0; i < num; i++)
				{
					this.m_points2.Add(Vector2.zero);
				}
			}
			else
			{
				this.m_pointsCount = ((this.m_points3.Capacity > 0 && this.m_points3.Count == 0) ? this.m_points3.Capacity : this.m_points3.Count);
				int num2 = this.m_pointsCount - this.m_points3.Count;
				for (int j = 0; j < num2; j++)
				{
					this.m_points3.Add(Vector3.zero);
				}
			}
			this.name = lineName;
			if (!this.SetVertexCount())
			{
				return;
			}
			this.m_go = new GameObject(this.name);
			this.m_canvasState = CanvasState.None;
			this.layer = LayerMask.NameToLayer("UI");
			this.m_rectTransform = this.m_go.AddComponent<RectTransform>();
			VectorLine.SetupTransform(this.m_rectTransform);
			this.m_texture = texture;
			this.m_lineVertices = new Vector3[this.m_vertexCount];
			this.m_lineUVs = new Vector2[this.m_vertexCount];
			this.m_lineColors = new Color[this.m_vertexCount];
			this.m_lineUVBottom = 0f;
			this.m_lineUVTop = 1f;
			this.SetUVs(0, this.GetSegmentNumber());
			this.m_lineTriangles = new List<int>();
			this.color = Color.white;
			this.m_maxWeldDistance = width * 2f * (width * 2f);
			this.m_joins = joins;
			this.m_lineWidths = new float[1];
			this.m_lineWidths[0] = width * 0.5f;
			this.m_lineWidth = width;
			if (!this.m_is2D)
			{
				this.m_screenPoints = new Vector3[this.m_vertexCount];
			}
			this.m_drawStart = 0;
			this.m_drawEnd = this.m_pointsCount - 1;
			if (this.m_material == null)
			{
				if (VectorLine.s_defaultMaterial == null)
				{
					VectorLine.s_defaultMaterial = Resources.Load("DefaultLine3D") as Material;
					if (VectorLine.s_defaultMaterial == null)
					{
						Debug.LogError("No DefaultLine3D material found in Resources");
						return;
					}
				}
				this.m_material = new Material(VectorLine.s_defaultMaterial);
				this.m_useCustomMaterial = false;
			}
			this.SetupTriangles(0);
		}

		// Token: 0x060019A2 RID: 6562 RVA: 0x000862B8 File Offset: 0x000844B8
		private void SetupTriangles(int startVert)
		{
			int num = 0;
			int num2 = 0;
			if (this.pointsCount > 0)
			{
				if (this.m_lineType == LineType.Points)
				{
					num = this.pointsCount * 6;
					num2 = this.pointsCount * 4;
				}
				else if (this.m_lineType == LineType.Continuous)
				{
					num = ((this.m_joins == Joins.Fill) ? ((this.pointsCount - 1) * 12) : ((this.pointsCount - 1) * 6));
					num2 = (this.pointsCount - 1) * 4;
				}
				else
				{
					num = this.pointsCount / 2 * 6;
					num2 = this.pointsCount * 2;
				}
			}
			if (this.m_capType != EndCap.None)
			{
				num += 12;
			}
			if (this.m_lineTriangles.Count <= num)
			{
				if (this.m_joins == Joins.Fill)
				{
					if (startVert >= 4)
					{
						int num3 = this.m_lineTriangles.Count - 6;
						this.m_lineTriangles[num3] = startVert - 3;
						this.m_lineTriangles[num3 + 1] = startVert;
						this.m_lineTriangles[num3 + 2] = startVert + 3;
						this.m_lineTriangles[num3 + 3] = startVert - 2;
						this.m_lineTriangles[num3 + 4] = startVert;
						this.m_lineTriangles[num3 + 5] = startVert + 3;
					}
					for (int i = startVert; i < num2; i += 4)
					{
						this.m_lineTriangles.Add(i);
						this.m_lineTriangles.Add(i + 1);
						this.m_lineTriangles.Add(i + 3);
						this.m_lineTriangles.Add(i + 1);
						this.m_lineTriangles.Add(i + 2);
						this.m_lineTriangles.Add(i + 3);
						this.m_lineTriangles.Add(i + 1);
						this.m_lineTriangles.Add(i + 4);
						this.m_lineTriangles.Add(i + 7);
						this.m_lineTriangles.Add(i + 2);
						this.m_lineTriangles.Add(i + 4);
						this.m_lineTriangles.Add(i + 7);
					}
					this.SetLastFillTriangles();
				}
				else
				{
					for (int j = startVert; j < num2; j += 4)
					{
						this.m_lineTriangles.Add(j);
						this.m_lineTriangles.Add(j + 1);
						this.m_lineTriangles.Add(j + 3);
						this.m_lineTriangles.Add(j + 1);
						this.m_lineTriangles.Add(j + 2);
						this.m_lineTriangles.Add(j + 3);
					}
				}
				if (this.m_vectorObject != null)
				{
					this.m_vectorObject.UpdateTris();
				}
				return;
			}
			this.m_lineTriangles.RemoveRange(num, this.m_lineTriangles.Count - num);
			if (this.m_joins == Joins.Fill)
			{
				this.SetLastFillTriangles();
				return;
			}
			if (this.m_vectorObject != null)
			{
				this.m_vectorObject.UpdateTris();
			}
		}

		// Token: 0x060019A3 RID: 6563 RVA: 0x00086548 File Offset: 0x00084748
		private void SetLastFillTriangles()
		{
			if (this.pointsCount < 2)
			{
				return;
			}
			int num = (this.pointsCount - 1) * 12 + ((this.m_capType != EndCap.None) ? 12 : 0);
			bool flag = false;
			if ((this.m_is2D && this.m_points2[0] == this.m_points2[this.points2.Count - 1]) || (!this.m_is2D && this.m_points3[0] == this.m_points3[this.points3.Count - 1]))
			{
				if (this.m_lineTriangles[num - 4] != 3 && this.m_lineTriangles[num - 1] != 3)
				{
					flag = true;
				}
				this.m_lineTriangles[num - 6] = this.m_vertexCount - 3;
				this.m_lineTriangles[num - 5] = 0;
				this.m_lineTriangles[num - 4] = 3;
				this.m_lineTriangles[num - 3] = this.m_vertexCount - 2;
				this.m_lineTriangles[num - 2] = 0;
				this.m_lineTriangles[num - 1] = 3;
			}
			else
			{
				if (this.m_lineTriangles[num - 4] == 3 && this.m_lineTriangles[num - 1] == 3)
				{
					flag = true;
				}
				this.m_lineTriangles[num - 6] = 0;
				this.m_lineTriangles[num - 5] = 0;
				this.m_lineTriangles[num - 4] = 0;
				this.m_lineTriangles[num - 3] = 0;
				this.m_lineTriangles[num - 2] = 0;
				this.m_lineTriangles[num - 1] = 0;
			}
			if (flag && this.m_vectorObject != null)
			{
				this.m_vectorObject.UpdateTris();
			}
		}

		// Token: 0x060019A4 RID: 6564 RVA: 0x0008670C File Offset: 0x0008490C
		private void SetupEndCap(float[] uvHeights)
		{
			int num = this.m_vertexCount + 8;
			if (num > 65534)
			{
				Debug.LogError("VectorLine: exceeded maximum vertex count of 65534 for \"" + this.m_name + "\"...use fewer points");
				return;
			}
			this.ResizeMeshArrays(num);
			int num2 = 0;
			if (this.m_joins == Joins.Fill)
			{
				for (int i = num - 8; i < num; i += 4)
				{
					this.m_lineTriangles.Insert(num2, i);
					this.m_lineTriangles.Insert(1 + num2, i + 1);
					this.m_lineTriangles.Insert(2 + num2, i + 3);
					this.m_lineTriangles.Insert(3 + num2, i + 1);
					this.m_lineTriangles.Insert(4 + num2, i + 2);
					this.m_lineTriangles.Insert(5 + num2, i + 3);
					num2 += 6;
				}
			}
			else
			{
				for (int j = num - 8; j < num; j += 4)
				{
					this.m_lineTriangles.Insert(num2, j);
					this.m_lineTriangles.Insert(1 + num2, j + 1);
					this.m_lineTriangles.Insert(2 + num2, j + 3);
					this.m_lineTriangles.Insert(3 + num2, j + 1);
					this.m_lineTriangles.Insert(4 + num2, j + 2);
					this.m_lineTriangles.Insert(5 + num2, j + 3);
					num2 += 6;
				}
			}
			int num3 = ((num >= 12) ? (num - 12) : 0);
			for (int k = num - 8; k < num - 4; k++)
			{
				this.m_lineColors[k] = this.m_lineColors[0];
				this.m_lineColors[k + 4] = this.m_lineColors[num3];
			}
			this.m_lineUVBottom = uvHeights[0];
			this.m_lineUVTop = uvHeights[1];
			this.m_backCapUVBottom = uvHeights[2];
			this.m_backCapUVTop = uvHeights[3];
			this.m_frontCapUVBottom = uvHeights[4];
			this.m_frontCapUVTop = uvHeights[5];
			this.SetUVs(0, this.GetSegmentNumber());
			this.SetEndCapUVs();
			if (this.m_vectorObject != null)
			{
				this.m_vectorObject.UpdateTris();
				this.m_vectorObject.UpdateUVs();
			}
			this.SetEndCapColors();
			this.m_originalTexture = this.m_texture;
			this.m_texture = VectorLine.capDictionary[this.m_endCap].texture;
			if (this.m_vectorObject != null)
			{
				this.m_vectorObject.SetTexture(this.m_texture);
			}
		}

		// Token: 0x060019A5 RID: 6565 RVA: 0x0008694C File Offset: 0x00084B4C
		private void ResetLine()
		{
			this.SetVertexCount();
			this.m_lineVertices = new Vector3[this.m_vertexCount];
			this.m_lineUVs = new Vector2[this.m_vertexCount];
			this.m_lineColors = new Color[this.m_vertexCount];
			if (!this.m_is2D)
			{
				this.m_screenPoints = new Vector3[this.m_vertexCount];
			}
			this.SetUVs(0, this.GetSegmentNumber());
			this.SetColor(this.m_color);
			int segmentNumber = this.GetSegmentNumber();
			this.SetupWidths(segmentNumber);
			this.ClearTriangles();
			this.SetupTriangles(0);
			if (this.m_vectorObject != null)
			{
				this.m_vectorObject.UpdateMeshAttributes();
			}
			if (this.m_canvasState == CanvasState.OnCanvas)
			{
				this.Draw();
				return;
			}
			if (this.m_canvasState == CanvasState.OffCanvas)
			{
				this.Draw3D();
			}
		}

		// Token: 0x060019A6 RID: 6566 RVA: 0x00086A14 File Offset: 0x00084C14
		private void SetEndCapUVs()
		{
			this.m_lineUVs[this.m_vertexCount + 3] = new Vector2(0f, this.m_frontCapUVTop);
			this.m_lineUVs[this.m_vertexCount] = new Vector2(1f, this.m_frontCapUVTop);
			this.m_lineUVs[this.m_vertexCount + 1] = new Vector2(1f, this.m_frontCapUVBottom);
			this.m_lineUVs[this.m_vertexCount + 2] = new Vector2(0f, this.m_frontCapUVBottom);
			if (VectorLine.capDictionary[this.m_endCap].capType == EndCap.Mirror)
			{
				this.m_lineUVs[this.m_vertexCount + 7] = new Vector2(0f, this.m_frontCapUVBottom);
				this.m_lineUVs[this.m_vertexCount + 4] = new Vector2(1f, this.m_frontCapUVBottom);
				this.m_lineUVs[this.m_vertexCount + 5] = new Vector2(1f, this.m_frontCapUVTop);
				this.m_lineUVs[this.m_vertexCount + 6] = new Vector2(0f, this.m_frontCapUVTop);
				return;
			}
			this.m_lineUVs[this.m_vertexCount + 7] = new Vector2(0f, this.m_backCapUVTop);
			this.m_lineUVs[this.m_vertexCount + 4] = new Vector2(1f, this.m_backCapUVTop);
			this.m_lineUVs[this.m_vertexCount + 5] = new Vector2(1f, this.m_backCapUVBottom);
			this.m_lineUVs[this.m_vertexCount + 6] = new Vector2(0f, this.m_backCapUVBottom);
		}

		// Token: 0x060019A7 RID: 6567 RVA: 0x00086BE0 File Offset: 0x00084DE0
		private void RemoveEndCap()
		{
			if (this.m_capType == EndCap.None)
			{
				return;
			}
			this.m_endCap = null;
			this.m_capType = EndCap.None;
			this.ResizeMeshArrays(this.m_vertexCount);
			this.m_lineTriangles.RemoveRange(0, 12);
			this.m_lineUVBottom = 0f;
			this.m_lineUVTop = 1f;
			this.SetUVs(0, this.GetSegmentNumber());
			if (this.m_useTextureScale)
			{
				this.SetTextureScale();
			}
			this.texture = this.m_originalTexture;
			this.m_vectorObject.UpdateMeshAttributes();
			if (this.m_collider)
			{
				this.SetCollider(this.m_canvasState == CanvasState.OnCanvas);
			}
		}

		// Token: 0x060019A8 RID: 6568 RVA: 0x00086C80 File Offset: 0x00084E80
		private static void SetupTransform(RectTransform rectTransform)
		{
			rectTransform.offsetMin = Vector2.zero;
			rectTransform.offsetMax = Vector2.zero;
			rectTransform.anchorMin = Vector2.zero;
			rectTransform.anchorMax = Vector2.zero;
			rectTransform.pivot = Vector2.zero;
			rectTransform.anchoredPosition = Vector2.zero;
		}

		// Token: 0x060019A9 RID: 6569 RVA: 0x00086CCF File Offset: 0x00084ECF
		private void ResizeMeshArrays(int newCount)
		{
			Array.Resize<Vector3>(ref this.m_lineVertices, newCount);
			Array.Resize<Vector2>(ref this.m_lineUVs, newCount);
			Array.Resize<Color>(ref this.m_lineColors, newCount);
		}

		// Token: 0x060019AA RID: 6570 RVA: 0x00086CF8 File Offset: 0x00084EF8
		public void Resize(int newCount)
		{
			if (newCount < 0)
			{
				Debug.LogError("VectorLine.Resize: the new count must be >= 0");
				return;
			}
			if (newCount == this.pointsCount)
			{
				return;
			}
			if (this.m_is2D)
			{
				if (newCount > this.m_pointsCount)
				{
					for (int i = 0; i < newCount - this.m_pointsCount; i++)
					{
						this.m_points2.Add(Vector2.zero);
					}
				}
				else
				{
					this.m_points2.RemoveRange(newCount, this.m_pointsCount - newCount);
				}
			}
			else if (newCount > this.m_pointsCount)
			{
				for (int j = 0; j < newCount - this.m_pointsCount; j++)
				{
					this.m_points3.Add(VectorLine.v3zero);
				}
			}
			else
			{
				this.m_points3.RemoveRange(newCount, this.m_pointsCount - newCount);
			}
			this.Resize();
		}

		// Token: 0x060019AB RID: 6571 RVA: 0x00086DB4 File Offset: 0x00084FB4
		private void Resize()
		{
			int pointsCount = this.m_pointsCount;
			int num = this.m_pointsCount;
			if (this.m_lineType != LineType.Points)
			{
				num = ((this.m_lineType == LineType.Continuous) ? Mathf.Max(0, this.m_pointsCount - 1) : (this.m_pointsCount / 2));
			}
			bool flag = this.m_drawEnd == this.m_pointsCount - 1 || this.m_drawEnd < 1;
			if (!this.SetVertexCount())
			{
				return;
			}
			this.m_pointsCount = this.pointsCount;
			int i = this.m_lineVertices.Length - ((this.m_capType == EndCap.None) ? 0 : 8);
			if (i < this.m_vertexCount)
			{
				if (i == 0)
				{
					i = 4;
				}
				while (i < this.m_pointsCount)
				{
					i *= 2;
				}
				i = Mathf.Min(i, this.MaxPoints());
				this.ResizeMeshArrays((this.m_capType == EndCap.None) ? (i * 4) : (i * 4 + 8));
				if (!this.m_is2D)
				{
					Array.Resize<Vector3>(ref this.m_screenPoints, i * 4);
				}
			}
			if (this.m_lineWidths.Length > 1)
			{
				if (this.m_lineType != LineType.Points)
				{
					i = ((this.m_lineType == LineType.Continuous) ? (i - 1) : (i / 2));
				}
				if (i > this.m_lineWidths.Length)
				{
					this.ResizeLineWidths(i);
				}
			}
			if (flag)
			{
				this.m_drawEnd = this.m_pointsCount - 1;
			}
			this.m_drawStart = Mathf.Clamp(this.m_drawStart, 0, this.m_pointsCount - 1);
			this.m_drawEnd = Mathf.Clamp(this.m_drawEnd, 0, this.m_pointsCount - 1);
			if (this.m_pointsCount > num)
			{
				this.SetColor(this.m_color, num, this.GetSegmentNumber());
				this.SetUVs(num, this.GetSegmentNumber());
			}
			if (this.m_pointsCount < pointsCount)
			{
				this.ZeroVertices(this.m_pointsCount, pointsCount);
			}
			if (this.m_capType != EndCap.None)
			{
				this.SetEndCapUVs();
				this.SetEndCapColors();
			}
			this.SetupTriangles(num * 4);
			if (this.m_vectorObject != null)
			{
				this.m_vectorObject.UpdateMeshAttributes();
			}
		}

		// Token: 0x060019AC RID: 6572 RVA: 0x00086F88 File Offset: 0x00085188
		private void ResizeLineWidths(int newSize)
		{
			if (newSize > this.m_lineWidths.Length)
			{
				float[] array = new float[newSize];
				for (int i = 0; i < this.m_lineWidths.Length; i++)
				{
					array[i] = this.m_lineWidths[i];
				}
				for (int j = this.m_lineWidths.Length; j < newSize; j++)
				{
					array[j] = this.m_lineWidth * 0.5f;
				}
				this.m_lineWidths = array;
			}
		}

		// Token: 0x060019AD RID: 6573 RVA: 0x00086FF0 File Offset: 0x000851F0
		private void SetUVs(int startIndex, int endIndex)
		{
			Vector2 vector = new Vector2(0f, this.m_lineUVTop);
			Vector2 vector2 = new Vector2(1f, this.m_lineUVTop);
			Vector2 vector3 = new Vector2(1f, this.m_lineUVBottom);
			Vector2 vector4 = new Vector2(0f, this.m_lineUVBottom);
			int num = startIndex * 4;
			for (int i = startIndex; i < endIndex; i++)
			{
				this.m_lineUVs[num] = vector;
				this.m_lineUVs[num + 1] = vector2;
				this.m_lineUVs[num + 2] = vector3;
				this.m_lineUVs[num + 3] = vector4;
				num += 4;
			}
			if (this.m_vectorObject != null)
			{
				this.m_vectorObject.UpdateUVs();
			}
		}

		// Token: 0x060019AE RID: 6574 RVA: 0x000870B4 File Offset: 0x000852B4
		private bool SetVertexCount()
		{
			this.m_vertexCount = Mathf.Max(0, this.GetSegmentNumber() * 4);
			if (this.m_lineType == LineType.Discrete && (this.pointsCount & 1) != 0)
			{
				this.m_vertexCount += 4;
			}
			int num = 65534;
			if (this.m_capType != EndCap.None)
			{
				num -= 8;
			}
			if (this.m_vertexCount > num)
			{
				Debug.LogError("VectorLine: exceeded maximum vertex count of 65534 for \"" + this.name + "\"...use fewer points (maximum is 16383 points for continuous lines and points, and 32767 points for discrete lines, minus two if end caps are used)");
				return false;
			}
			return true;
		}

		// Token: 0x060019AF RID: 6575 RVA: 0x0008712F File Offset: 0x0008532F
		private int MaxPoints()
		{
			if (this.m_capType != EndCap.None)
			{
				return 16381;
			}
			return 16383;
		}

		// Token: 0x060019B0 RID: 6576 RVA: 0x00087145 File Offset: 0x00085345
		public void AddNormals()
		{
			this.m_useNormals = true;
			this.m_normalsCalculated = false;
		}

		// Token: 0x060019B1 RID: 6577 RVA: 0x00087155 File Offset: 0x00085355
		public void AddTangents()
		{
			if (!this.m_useNormals)
			{
				this.m_useNormals = true;
				this.m_normalsCalculated = false;
			}
			this.m_useTangents = true;
			this.m_tangentsCalculated = false;
		}

		// Token: 0x060019B2 RID: 6578 RVA: 0x0008717C File Offset: 0x0008537C
		public Vector4[] CalculateTangents(Vector3[] normals)
		{
			if (!this.m_useNormals)
			{
				this.m_vectorObject.UpdateNormals();
				this.m_useNormals = true;
				this.m_normalsCalculated = true;
			}
			int num = this.m_vectorObject.VertexCount();
			Vector3[] array = new Vector3[num];
			Vector3[] array2 = new Vector3[num];
			int count = this.m_lineTriangles.Count;
			for (int i = 0; i < count; i += 3)
			{
				int num2 = this.m_lineTriangles[i];
				int num3 = this.m_lineTriangles[i + 1];
				int num4 = this.m_lineTriangles[i + 2];
				Vector3 vector = this.m_lineVertices[num2];
				Vector3 vector2 = this.m_lineVertices[num3];
				Vector3 vector3 = this.m_lineVertices[num4];
				Vector2 vector4 = this.m_lineUVs[num2];
				Vector2 vector5 = this.m_lineUVs[num3];
				Vector2 vector6 = this.m_lineUVs[num4];
				float num5 = vector2.x - vector.x;
				float num6 = vector3.x - vector.x;
				float num7 = vector2.y - vector.y;
				float num8 = vector3.y - vector.y;
				float num9 = vector2.z - vector.z;
				float num10 = vector3.z - vector.z;
				float num11 = vector5.x - vector4.x;
				float num12 = vector6.x - vector4.x;
				float num13 = vector5.y - vector4.y;
				float num14 = vector6.y - vector4.y;
				float num15 = 1f / (num11 * num14 - num12 * num13);
				Vector3 vector7 = new Vector3((num14 * num5 - num13 * num6) * num15, (num14 * num7 - num13 * num8) * num15, (num14 * num9 - num13 * num10) * num15);
				Vector3 vector8 = new Vector3((num11 * num6 - num12 * num5) * num15, (num11 * num8 - num12 * num7) * num15, (num11 * num10 - num12 * num9) * num15);
				array[num2] += vector7;
				array[num3] += vector7;
				array[num4] += vector7;
				array2[num2] += vector8;
				array2[num3] += vector8;
				array2[num4] += vector8;
			}
			Vector4[] array3 = new Vector4[num];
			for (int j = 0; j < this.m_vertexCount; j++)
			{
				Vector3 vector9 = normals[j];
				Vector3 vector10 = array[j];
				array3[j] = (vector10 - vector9 * Vector3.Dot(vector9, vector10)).normalized;
				array3[j].w = ((Vector3.Dot(Vector3.Cross(vector9, vector10), array2[j]) < 0f) ? (-1f) : 1f);
			}
			return array3;
		}

		// Token: 0x060019B3 RID: 6579 RVA: 0x000874C4 File Offset: 0x000856C4
		public static GameObject SetupVectorCanvas()
		{
			GameObject gameObject = GameObject.Find("VectorCanvas");
			Canvas canvas;
			if (gameObject != null)
			{
				canvas = gameObject.GetComponent<Canvas>();
			}
			else
			{
				gameObject = new GameObject("VectorCanvas");
				gameObject.layer = LayerMask.NameToLayer("UI");
				canvas = gameObject.AddComponent<Canvas>();
			}
			canvas.renderMode = RenderMode.ScreenSpaceOverlay;
			canvas.sortingOrder = 1;
			VectorLine.m_canvas = canvas;
			return gameObject;
		}

		// Token: 0x060019B4 RID: 6580 RVA: 0x00087525 File Offset: 0x00085725
		public static void SetCanvasCamera(Camera cam)
		{
			if (VectorLine.m_canvas == null)
			{
				VectorLine.SetupVectorCanvas();
			}
			VectorLine.m_canvas.renderMode = RenderMode.ScreenSpaceCamera;
			VectorLine.m_canvas.worldCamera = cam;
		}

		// Token: 0x060019B5 RID: 6581 RVA: 0x00087550 File Offset: 0x00085750
		public void SetCanvas(GameObject canvasObject)
		{
			this.SetCanvas(canvasObject, true);
		}

		// Token: 0x060019B6 RID: 6582 RVA: 0x0008755C File Offset: 0x0008575C
		public void SetCanvas(GameObject canvasObject, bool worldPositionStays)
		{
			Canvas component = canvasObject.GetComponent<Canvas>();
			if (component == null)
			{
				Debug.LogError("VectorLine.SetCanvas: canvas object must have a Canvas component");
				return;
			}
			this.SetCanvas(component, worldPositionStays);
		}

		// Token: 0x060019B7 RID: 6583 RVA: 0x0008758C File Offset: 0x0008578C
		public void SetCanvas(Canvas canvas)
		{
			this.SetCanvas(canvas, true);
		}

		// Token: 0x060019B8 RID: 6584 RVA: 0x00087598 File Offset: 0x00085798
		public void SetCanvas(Canvas canvas, bool worldPositionStays)
		{
			if (this.m_canvasState == CanvasState.OffCanvas)
			{
				Debug.LogError("VectorLine.SetCanvas only works with lines made with Draw, not Draw3D.");
				return;
			}
			if (canvas == null)
			{
				Debug.LogError("VectorLine.SetCanvas: canvas must not be null");
				return;
			}
			if (canvas.renderMode == RenderMode.WorldSpace)
			{
				Debug.LogError("VectorLine.SetCanvas: canvas must be screen space overlay or screen space camera");
				return;
			}
			this.m_go.transform.SetParent(canvas.transform, worldPositionStays);
		}

		// Token: 0x060019B9 RID: 6585 RVA: 0x000875F8 File Offset: 0x000857F8
		public void SetMask(GameObject maskObject)
		{
			this.SetMask(maskObject, true);
		}

		// Token: 0x060019BA RID: 6586 RVA: 0x00087604 File Offset: 0x00085804
		public void SetMask(GameObject maskObject, bool worldPositionStays)
		{
			Mask component = maskObject.GetComponent<Mask>();
			if (component == null)
			{
				Debug.LogError("VectorLine.SetMask: mask object must have a Mask component");
				return;
			}
			this.SetMask(component, worldPositionStays);
		}

		// Token: 0x060019BB RID: 6587 RVA: 0x00087634 File Offset: 0x00085834
		public void SetMask(Mask mask)
		{
			this.SetMask(mask, true);
		}

		// Token: 0x060019BC RID: 6588 RVA: 0x00087640 File Offset: 0x00085840
		public void SetMask(Mask mask, bool worldPositionStays)
		{
			if (this.m_canvasState == CanvasState.OffCanvas)
			{
				Debug.LogError("VectorLine.SetMask only works with lines made with Draw, not Draw3D.");
				return;
			}
			if (mask == null)
			{
				Debug.LogError("VectorLine.SetMask: mask must not be null");
				return;
			}
			this.m_go.transform.SetParent(mask.transform, worldPositionStays);
		}

		// Token: 0x060019BD RID: 6589 RVA: 0x0008768C File Offset: 0x0008588C
		private bool CheckCamera3D()
		{
			if (!this.m_is2D && !VectorLine.cam3D)
			{
				VectorLine.SetCamera3D();
				if (!VectorLine.cam3D)
				{
					Debug.LogError("No camera available...use VectorLine.SetCamera3D to assign a camera");
					return false;
				}
			}
			return true;
		}

		// Token: 0x060019BE RID: 6590 RVA: 0x000876C0 File Offset: 0x000858C0
		public static void SetCamera3D()
		{
			if (Camera.main == null)
			{
				Debug.LogError("VectorLine.SetCamera3D: no camera tagged \"Main Camera\" found. Please call SetCamera3D with a specific camera instead.");
				return;
			}
			VectorLine.SetCamera3D(Camera.main);
		}

		// Token: 0x060019BF RID: 6591 RVA: 0x000876E4 File Offset: 0x000858E4
		public static void SetCamera3D(GameObject cameraObject)
		{
			Camera component = cameraObject.GetComponent<Camera>();
			if (component == null)
			{
				Debug.LogError("VectorLine.SetCamera3D: camera object must have a Camera component");
				return;
			}
			VectorLine.SetCamera3D(component);
		}

		// Token: 0x060019C0 RID: 6592 RVA: 0x00087714 File Offset: 0x00085914
		public static void SetCamera3D(Camera camera)
		{
			VectorLine.camTransform = camera.transform;
			VectorLine.cam3D = camera;
			VectorLine.oldPosition = VectorLine.camTransform.position + Vector3.one;
			VectorLine.oldRotation = VectorLine.camTransform.eulerAngles + Vector3.one;
		}

		// Token: 0x060019C1 RID: 6593 RVA: 0x00087764 File Offset: 0x00085964
		public static bool CameraHasMoved()
		{
			return VectorLine.oldPosition != VectorLine.camTransform.position || VectorLine.oldRotation != VectorLine.camTransform.eulerAngles;
		}

		// Token: 0x060019C2 RID: 6594 RVA: 0x00087792 File Offset: 0x00085992
		public static void UpdateCameraInfo()
		{
			VectorLine.oldPosition = VectorLine.camTransform.position;
			VectorLine.oldRotation = VectorLine.camTransform.eulerAngles;
		}

		// Token: 0x060019C3 RID: 6595 RVA: 0x000877B2 File Offset: 0x000859B2
		public int GetSegmentNumber()
		{
			if (this.m_lineType == LineType.Points)
			{
				return this.pointsCount;
			}
			if (this.m_lineType != LineType.Continuous)
			{
				return this.pointsCount / 2;
			}
			if (this.pointsCount != 0)
			{
				return this.pointsCount - 1;
			}
			return 0;
		}

		// Token: 0x060019C4 RID: 6596 RVA: 0x000877E8 File Offset: 0x000859E8
		private void SetEndCapColors()
		{
			if (this.m_lineVertices.Length < 4)
			{
				return;
			}
			if (this.m_capType <= EndCap.Mirror)
			{
				int num = ((this.m_lineType == LineType.Continuous) ? (this.m_drawStart * 4) : (this.m_drawStart * 2));
				for (int i = 0; i < 4; i++)
				{
					this.m_lineColors[i + this.m_vertexCount] = (this.m_useCapColors ? this.m_frontColor : this.m_lineColors[i + num]);
				}
			}
			if (this.m_capType >= EndCap.Both)
			{
				int num2 = this.m_drawEnd;
				if (this.m_lineType == LineType.Continuous)
				{
					if (this.m_drawEnd == this.pointsCount)
					{
						num2--;
					}
				}
				else if (num2 < this.pointsCount)
				{
					num2++;
				}
				int num3 = num2 * ((this.m_lineType == LineType.Continuous) ? 4 : 2) - 2;
				if (num3 < 0)
				{
					num3 = 0;
				}
				for (int j = 4; j < 8; j++)
				{
					this.m_lineColors[j + this.m_vertexCount] = (this.m_useCapColors ? this.m_backColor : this.m_lineColors[num3]);
				}
			}
			if (this.m_vectorObject != null)
			{
				this.m_vectorObject.UpdateColors();
			}
		}

		// Token: 0x060019C5 RID: 6597 RVA: 0x00087912 File Offset: 0x00085B12
		public void SetEndCapColor(Color32 color)
		{
			this.SetEndCapColor(color, color);
		}

		// Token: 0x060019C6 RID: 6598 RVA: 0x0008791C File Offset: 0x00085B1C
		public void SetEndCapColor(Color32 frontColor, Color32 backColor)
		{
			if (this.m_capType == EndCap.None)
			{
				Debug.LogError("VectorLine.SetEndCapColor: the line \"" + this.name + "\" does not have any end caps");
				return;
			}
			this.m_useCapColors = true;
			this.m_frontColor = frontColor;
			this.m_backColor = backColor;
			this.SetEndCapColors();
		}

		// Token: 0x060019C7 RID: 6599 RVA: 0x00087968 File Offset: 0x00085B68
		public void SetEndCapIndex(EndCap endCap, int index)
		{
			if (this.m_capType == EndCap.None)
			{
				Debug.LogError("VectorLine.SetEndCapIndex: the line \"" + this.name + "\" does not have any end caps");
				return;
			}
			if (endCap != EndCap.Front && endCap != EndCap.Back)
			{
				Debug.LogError("VectorLine.SetEndCapIndex: endCap must be EndCap.Front or EndCap.Back");
				return;
			}
			if (index < 0)
			{
				index = 0;
			}
			if (endCap == EndCap.Front)
			{
				this.m_frontEndCapIndex = index;
				return;
			}
			if (endCap == EndCap.Back)
			{
				this.m_backEndCapIndex = index;
			}
		}

		// Token: 0x060019C8 RID: 6600 RVA: 0x000879C8 File Offset: 0x00085BC8
		public void SetColor(Color color)
		{
			this.SetColor(color, 0, this.pointsCount);
		}

		// Token: 0x060019C9 RID: 6601 RVA: 0x000879D8 File Offset: 0x00085BD8
		public void SetColor(Color color, int index)
		{
			this.SetColor(color, index, index);
		}

		// Token: 0x060019CA RID: 6602 RVA: 0x000879E4 File Offset: 0x00085BE4
		public void SetColor(Color color, int startIndex, int endIndex)
		{
			if (this.pointsCount != this.m_pointsCount)
			{
				this.Resize();
			}
			int segmentNumber = this.GetSegmentNumber();
			startIndex = Mathf.Clamp(startIndex * 4, 0, segmentNumber * 4);
			endIndex = Mathf.Clamp((endIndex + 1) * 4, 0, segmentNumber * 4);
			if (!this.m_smoothColor)
			{
				for (int i = startIndex; i < endIndex; i++)
				{
					this.m_lineColors[i] = color;
				}
			}
			else
			{
				if (startIndex == 0)
				{
					this.m_lineColors[0] = color;
					this.m_lineColors[3] = color;
				}
				for (int j = startIndex; j < endIndex; j += 4)
				{
					this.m_lineColors[j + 1] = color;
					this.m_lineColors[j + 2] = color;
					if (j + 4 < this.m_vertexCount)
					{
						this.m_lineColors[j + 4] = color;
						this.m_lineColors[j + 7] = color;
					}
				}
			}
			if (this.m_capType != EndCap.None && (startIndex <= 0 || endIndex >= segmentNumber - 1))
			{
				this.SetEndCapColors();
			}
			if (this.m_vectorObject != null)
			{
				this.m_vectorObject.UpdateColors();
			}
		}

		// Token: 0x060019CB RID: 6603 RVA: 0x00087AEC File Offset: 0x00085CEC
		public void SetColors(List<Color> lineColors)
		{
			if (lineColors == null)
			{
				Debug.LogError("VectorLine.SetColors: lineColors list must not be null");
				return;
			}
			if (this.pointsCount != this.m_pointsCount)
			{
				this.Resize();
			}
			if (this.m_lineType != LineType.Points)
			{
				if (this.WrongArrayLength(lineColors.Count, VectorLine.FunctionName.SetColors))
				{
					return;
				}
			}
			else if (lineColors.Count != this.pointsCount)
			{
				Debug.LogError("VectorLine.SetColors: Length of lineColors list in \"" + this.name + "\" must be same length as points list");
				return;
			}
			int num;
			int num2;
			this.SetSegmentStartEnd(out num, out num2);
			if (num == 0 && num2 == 0)
			{
				return;
			}
			int num3 = num * 4;
			if (this.m_lineType == LineType.Points)
			{
				num2++;
			}
			if (this.smoothColor)
			{
				this.m_lineColors[num3] = lineColors[num];
				this.m_lineColors[num3 + 3] = lineColors[num];
				this.m_lineColors[num3 + 2] = lineColors[num];
				this.m_lineColors[num3 + 1] = lineColors[num];
				num3 += 4;
				for (int i = num + 1; i < num2; i++)
				{
					this.m_lineColors[num3] = lineColors[i - 1];
					this.m_lineColors[num3 + 3] = lineColors[i - 1];
					this.m_lineColors[num3 + 2] = lineColors[i];
					this.m_lineColors[num3 + 1] = lineColors[i];
					num3 += 4;
				}
			}
			else
			{
				for (int j = num; j < num2; j++)
				{
					this.m_lineColors[num3] = lineColors[j];
					this.m_lineColors[num3 + 1] = lineColors[j];
					this.m_lineColors[num3 + 2] = lineColors[j];
					this.m_lineColors[num3 + 3] = lineColors[j];
					num3 += 4;
				}
			}
			if (this.m_capType != EndCap.None)
			{
				this.SetEndCapColors();
			}
			if (this.m_vectorObject != null)
			{
				this.m_vectorObject.UpdateColors();
			}
		}

		// Token: 0x060019CC RID: 6604 RVA: 0x00087CD6 File Offset: 0x00085ED6
		public void SetMaterial(Material material, bool ownsMaterial)
		{
			this.m_material = material;
			this.m_useCustomMaterial = !ownsMaterial;
		}

		// Token: 0x060019CD RID: 6605 RVA: 0x00087CEC File Offset: 0x00085EEC
		private void SetSegmentStartEnd(out int start, out int end)
		{
			start = ((this.m_lineType != LineType.Discrete) ? this.m_drawStart : (this.m_drawStart / 2));
			end = this.m_drawEnd;
			if (this.m_lineType == LineType.Discrete)
			{
				end = this.m_drawEnd / 2;
				if (this.m_drawEnd % 2 != 0)
				{
					end++;
				}
			}
		}

		// Token: 0x060019CE RID: 6606 RVA: 0x00087D40 File Offset: 0x00085F40
		public Color GetColor(int index)
		{
			if (this.pointsCount != this.m_pointsCount)
			{
				this.Resize();
			}
			if (this.m_vertexCount == 0)
			{
				return this.m_color;
			}
			int num = index * 4 + 2;
			if (num < 0 || num >= this.m_vertexCount)
			{
				Debug.LogError("VectorLine.GetColor: index " + index.ToString() + " out of range");
				return Color.clear;
			}
			return this.m_lineColors[num];
		}

		// Token: 0x060019CF RID: 6607 RVA: 0x00087DB0 File Offset: 0x00085FB0
		private void SetupWidths(int max)
		{
			if ((max >= 2 && this.m_lineWidths.Length == 1) || (max >= 2 && this.m_lineWidths.Length != max))
			{
				this.ResizeLineWidths(max);
			}
		}

		// Token: 0x060019D0 RID: 6608 RVA: 0x00087DD7 File Offset: 0x00085FD7
		public void SetWidth(float width)
		{
			this.m_lineWidth = width;
			this.SetWidth(width, 0, this.pointsCount);
		}

		// Token: 0x060019D1 RID: 6609 RVA: 0x00087DEE File Offset: 0x00085FEE
		public void SetWidth(float width, int index)
		{
			this.SetWidth(width, index, index);
		}

		// Token: 0x060019D2 RID: 6610 RVA: 0x00087DFC File Offset: 0x00085FFC
		public void SetWidth(float width, int startIndex, int endIndex)
		{
			if (this.pointsCount != this.m_pointsCount)
			{
				this.Resize();
			}
			int segmentNumber = this.GetSegmentNumber();
			this.SetupWidths(segmentNumber);
			startIndex = Mathf.Clamp(startIndex, 0, Mathf.Max(segmentNumber - 1, 0));
			endIndex = Mathf.Clamp(endIndex, 0, Mathf.Max(segmentNumber - 1, 0));
			for (int i = startIndex; i <= endIndex; i++)
			{
				this.m_lineWidths[i] = width * 0.5f;
			}
		}

		// Token: 0x060019D3 RID: 6611 RVA: 0x00087E6A File Offset: 0x0008606A
		public void SetWidths(List<float> lineWidths)
		{
			this.SetWidths(lineWidths, null, lineWidths.Count, true);
		}

		// Token: 0x060019D4 RID: 6612 RVA: 0x00087E7B File Offset: 0x0008607B
		public void SetWidths(List<int> lineWidths)
		{
			this.SetWidths(null, lineWidths, lineWidths.Count, false);
		}

		// Token: 0x060019D5 RID: 6613 RVA: 0x00087E8C File Offset: 0x0008608C
		private void SetWidths(List<float> lineWidthsFloat, List<int> lineWidthsInt, int arrayLength, bool doFloat)
		{
			if ((doFloat && lineWidthsFloat == null) || (!doFloat && lineWidthsInt == null))
			{
				Debug.LogError("VectorLine.SetWidths: line widths list must not be null");
				return;
			}
			if (this.pointsCount != this.m_pointsCount)
			{
				this.Resize();
			}
			if (this.m_lineType == LineType.Points)
			{
				if (arrayLength != this.pointsCount)
				{
					Debug.LogError("VectorLine.SetWidths: line widths list must be the same length as the points list for \"" + this.name + "\"");
					return;
				}
			}
			else if (this.WrongArrayLength(arrayLength, VectorLine.FunctionName.SetWidths))
			{
				return;
			}
			if (this.m_lineWidths.Length != arrayLength)
			{
				Array.Resize<float>(ref this.m_lineWidths, arrayLength);
			}
			if (doFloat)
			{
				for (int i = 0; i < arrayLength; i++)
				{
					this.m_lineWidths[i] = lineWidthsFloat[i] * 0.5f;
				}
				return;
			}
			for (int j = 0; j < arrayLength; j++)
			{
				this.m_lineWidths[j] = (float)lineWidthsInt[j] * 0.5f;
			}
		}

		// Token: 0x060019D6 RID: 6614 RVA: 0x00087F60 File Offset: 0x00086160
		public float GetWidth(int index)
		{
			if (this.pointsCount != this.m_pointsCount)
			{
				this.Resize();
			}
			int segmentNumber = this.GetSegmentNumber();
			if (index < 0 || index >= segmentNumber)
			{
				Debug.LogError("VectorLine.GetWidth: index " + index.ToString() + " out of range...must be >= 0 and < " + segmentNumber.ToString());
				return 0f;
			}
			if (index >= this.m_lineWidths.Length)
			{
				return this.m_lineWidth;
			}
			return this.m_lineWidths[index] * 2f;
		}

		// Token: 0x060019D7 RID: 6615 RVA: 0x00087FD8 File Offset: 0x000861D8
		public static VectorLine SetLine(Color color, params Vector2[] points)
		{
			return VectorLine.SetLine(color, 0f, points);
		}

		// Token: 0x060019D8 RID: 6616 RVA: 0x00087FE8 File Offset: 0x000861E8
		public static VectorLine SetLine(Color color, float time, params Vector2[] points)
		{
			if (points.Length < 2)
			{
				Debug.LogError("VectorLine.SetLine needs at least two points");
				return null;
			}
			VectorLine vectorLine = new VectorLine("Line", new List<Vector2>(points), null, 1f, LineType.Continuous, Joins.None);
			vectorLine.color = color;
			if (time > 0f)
			{
				VectorLine.lineManager.DisableLine(vectorLine, time);
			}
			vectorLine.Draw();
			return vectorLine;
		}

		// Token: 0x060019D9 RID: 6617 RVA: 0x00088042 File Offset: 0x00086242
		public static VectorLine SetLine(Color color, params Vector3[] points)
		{
			return VectorLine.SetLine(color, 0f, points);
		}

		// Token: 0x060019DA RID: 6618 RVA: 0x00088050 File Offset: 0x00086250
		public static VectorLine SetLine(Color color, float time, params Vector3[] points)
		{
			if (points.Length < 2)
			{
				Debug.LogError("VectorLine.SetLine needs at least two points");
				return null;
			}
			VectorLine vectorLine = new VectorLine("SetLine", new List<Vector3>(points), null, 1f, LineType.Continuous, Joins.None);
			vectorLine.color = color;
			if (time > 0f)
			{
				VectorLine.lineManager.DisableLine(vectorLine, time);
			}
			vectorLine.Draw();
			return vectorLine;
		}

		// Token: 0x060019DB RID: 6619 RVA: 0x000880AA File Offset: 0x000862AA
		public static VectorLine SetLine3D(Color color, params Vector3[] points)
		{
			return VectorLine.SetLine3D(color, 0f, points);
		}

		// Token: 0x060019DC RID: 6620 RVA: 0x000880B8 File Offset: 0x000862B8
		public static VectorLine SetLine3D(Color color, float time, params Vector3[] points)
		{
			if (points.Length < 2)
			{
				Debug.LogError("VectorLine.SetLine3D needs at least two points");
				return null;
			}
			VectorLine vectorLine = new VectorLine("SetLine3D", new List<Vector3>(points), null, 1f, LineType.Continuous, Joins.None);
			vectorLine.color = color;
			vectorLine.Draw3DAuto(time);
			return vectorLine;
		}

		// Token: 0x060019DD RID: 6621 RVA: 0x000880F2 File Offset: 0x000862F2
		public static VectorLine SetRay(Color color, Vector3 origin, Vector3 direction)
		{
			return VectorLine.SetRay(color, 0f, origin, direction);
		}

		// Token: 0x060019DE RID: 6622 RVA: 0x00088104 File Offset: 0x00086304
		public static VectorLine SetRay(Color color, float time, Vector3 origin, Vector3 direction)
		{
			VectorLine vectorLine = new VectorLine("SetRay", new List<Vector3>(new Vector3[]
			{
				origin,
				new Ray(origin, direction).GetPoint(direction.magnitude)
			}), null, 1f, LineType.Continuous, Joins.None);
			vectorLine.color = color;
			if (time > 0f)
			{
				VectorLine.lineManager.DisableLine(vectorLine, time);
			}
			vectorLine.Draw();
			return vectorLine;
		}

		// Token: 0x060019DF RID: 6623 RVA: 0x00088176 File Offset: 0x00086376
		public static VectorLine SetRay3D(Color color, Vector3 origin, Vector3 direction)
		{
			return VectorLine.SetRay3D(color, 0f, origin, direction);
		}

		// Token: 0x060019E0 RID: 6624 RVA: 0x00088188 File Offset: 0x00086388
		public static VectorLine SetRay3D(Color color, float time, Vector3 origin, Vector3 direction)
		{
			VectorLine vectorLine = new VectorLine("SetRay3D", new List<Vector3>(new Vector3[]
			{
				origin,
				new Ray(origin, direction).GetPoint(direction.magnitude)
			}), null, 1f, LineType.Continuous, Joins.None);
			vectorLine.color = color;
			vectorLine.Draw3DAuto(time);
			return vectorLine;
		}

		// Token: 0x060019E1 RID: 6625 RVA: 0x000881E8 File Offset: 0x000863E8
		private void CheckNormals()
		{
			if (this.m_useNormals && !this.m_normalsCalculated)
			{
				this.m_vectorObject.UpdateNormals();
				this.m_normalsCalculated = true;
			}
			if (this.m_useTangents && !this.m_tangentsCalculated)
			{
				this.m_vectorObject.UpdateTangents();
				this.m_tangentsCalculated = true;
			}
		}

		// Token: 0x060019E2 RID: 6626 RVA: 0x00088239 File Offset: 0x00086439
		private void CheckLine(bool draw3D)
		{
			if (this.m_capType != EndCap.None)
			{
				this.DrawEndCap(draw3D);
			}
			if (this.m_continuousTexture)
			{
				this.SetContinuousTexture();
			}
			if (this.m_joins == Joins.Fill)
			{
				this.SetLastFillTriangles();
			}
		}

		// Token: 0x060019E3 RID: 6627 RVA: 0x00088268 File Offset: 0x00086468
		private void DrawEndCap(bool draw3D)
		{
			if (this.m_capType <= EndCap.Mirror)
			{
				int num;
				if (this.m_frontEndCapIndex != -1)
				{
					num = this.m_frontEndCapIndex;
					if (this.m_lineType == LineType.Discrete && (num & 1) != 0)
					{
						num++;
					}
					num = Mathf.Clamp(num, this.drawStart, this.drawEnd) * 4;
				}
				else
				{
					num = this.m_drawStart * 4;
				}
				int num2 = ((this.m_lineWidths.Length > 1) ? this.m_drawStart : 0);
				if (this.m_lineType == LineType.Discrete)
				{
					num2 /= 2;
					num /= 2;
				}
				if (!draw3D)
				{
					Vector3 vector = (this.m_lineVertices[num] - this.m_lineVertices[num + 1]).normalized * this.m_lineWidths[num2] * 2f * VectorLine.capDictionary[this.m_endCap].ratio1;
					Vector3 vector2 = vector * VectorLine.capDictionary[this.m_endCap].offset1;
					this.m_lineVertices[this.m_vertexCount] = this.m_lineVertices[num] + vector + vector2;
					this.m_lineVertices[this.m_vertexCount + 3] = this.m_lineVertices[num + 3] + vector + vector2;
					this.m_lineVertices[num] += vector2;
					this.m_lineVertices[num + 3] += vector2;
				}
				else
				{
					Vector3 vector3 = (this.m_screenPoints[num] - this.m_screenPoints[num + 1]).normalized * this.m_lineWidths[num2] * 2f * VectorLine.capDictionary[this.m_endCap].ratio1;
					Vector3 vector4 = vector3 * VectorLine.capDictionary[this.m_endCap].offset1;
					this.m_lineVertices[this.m_vertexCount] = VectorLine.cam3D.ScreenToWorldPoint(this.m_screenPoints[num] + vector3 + vector4);
					this.m_lineVertices[this.m_vertexCount + 3] = VectorLine.cam3D.ScreenToWorldPoint(this.m_screenPoints[num + 3] + vector3 + vector4);
					this.m_lineVertices[num] = VectorLine.cam3D.ScreenToWorldPoint(this.m_screenPoints[num] + vector4);
					this.m_lineVertices[num + 3] = VectorLine.cam3D.ScreenToWorldPoint(this.m_screenPoints[num + 3] + vector4);
				}
				this.m_lineVertices[this.m_vertexCount + 2] = this.m_lineVertices[num + 3];
				this.m_lineVertices[this.m_vertexCount + 1] = this.m_lineVertices[num];
				if (VectorLine.capDictionary[this.m_endCap].scale1 != 1f)
				{
					this.ScaleCapVertices(this.m_vertexCount, VectorLine.capDictionary[this.m_endCap].scale1, (this.m_lineVertices[this.m_vertexCount + 1] + this.m_lineVertices[this.m_vertexCount + 2]) / 2f);
				}
				this.m_lineTriangles[0] = this.m_vertexCount;
				this.m_lineTriangles[1] = this.m_vertexCount + 1;
				this.m_lineTriangles[2] = this.m_vertexCount + 3;
				this.m_lineTriangles[3] = this.m_vertexCount + 1;
				this.m_lineTriangles[4] = this.m_vertexCount + 2;
				this.m_lineTriangles[5] = this.m_vertexCount + 3;
			}
			if (this.m_capType >= EndCap.Both)
			{
				int num3 = this.m_drawEnd;
				if (this.m_lineType == LineType.Continuous)
				{
					if (this.m_drawEnd == this.pointsCount)
					{
						num3--;
					}
				}
				else if (num3 < this.pointsCount)
				{
					num3++;
				}
				int num;
				if (this.m_backEndCapIndex != -1)
				{
					num = this.m_backEndCapIndex;
					if (this.m_lineType == LineType.Discrete && (num & 1) != 0)
					{
						num++;
					}
					num = Mathf.Clamp(num, this.drawStart, num3) * 4;
				}
				else
				{
					num = num3 * 4;
				}
				int num4 = ((this.m_lineWidths.Length > 1) ? (num3 - 1) : 0);
				if (num4 < 0)
				{
					num4 = 0;
				}
				if (this.m_lineType == LineType.Discrete)
				{
					num4 /= 2;
					num /= 2;
				}
				if (num < 4)
				{
					num = 4;
				}
				if (!draw3D)
				{
					Vector3 vector5 = (this.m_lineVertices[num - 2] - this.m_lineVertices[num - 1]).normalized * this.m_lineWidths[num4] * 2f * VectorLine.capDictionary[this.m_endCap].ratio2;
					Vector3 vector6 = vector5 * VectorLine.capDictionary[this.m_endCap].offset2;
					this.m_lineVertices[this.m_vertexCount + 6] = this.m_lineVertices[num - 2] + vector5 + vector6;
					this.m_lineVertices[this.m_vertexCount + 5] = this.m_lineVertices[num - 3] + vector5 + vector6;
					this.m_lineVertices[num - 3] += vector6;
					this.m_lineVertices[num - 2] += vector6;
				}
				else
				{
					Vector3 vector7 = (this.m_screenPoints[num - 2] - this.m_screenPoints[num - 1]).normalized * this.m_lineWidths[num4] * 2f * VectorLine.capDictionary[this.m_endCap].ratio2;
					Vector3 vector8 = vector7 * VectorLine.capDictionary[this.m_endCap].offset2;
					this.m_lineVertices[this.m_vertexCount + 6] = VectorLine.cam3D.ScreenToWorldPoint(this.m_screenPoints[num - 2] + vector7 + vector8);
					this.m_lineVertices[this.m_vertexCount + 5] = VectorLine.cam3D.ScreenToWorldPoint(this.m_screenPoints[num - 3] + vector7 + vector8);
					this.m_lineVertices[num - 3] = VectorLine.cam3D.ScreenToWorldPoint(this.m_screenPoints[num - 3] + vector8);
					this.m_lineVertices[num - 2] = VectorLine.cam3D.ScreenToWorldPoint(this.m_screenPoints[num - 2] + vector8);
				}
				this.m_lineVertices[this.m_vertexCount + 4] = this.m_lineVertices[num - 3];
				this.m_lineVertices[this.m_vertexCount + 7] = this.m_lineVertices[num - 2];
				if (VectorLine.capDictionary[this.m_endCap].scale2 != 1f)
				{
					this.ScaleCapVertices(this.m_vertexCount + 4, VectorLine.capDictionary[this.m_endCap].scale2, (this.m_lineVertices[this.m_vertexCount + 4] + this.m_lineVertices[this.m_vertexCount + 7]) / 2f);
				}
				this.m_lineTriangles[6] = this.m_vertexCount + 4;
				this.m_lineTriangles[7] = this.m_vertexCount + 5;
				this.m_lineTriangles[8] = this.m_vertexCount + 7;
				this.m_lineTriangles[9] = this.m_vertexCount + 5;
				this.m_lineTriangles[10] = this.m_vertexCount + 6;
				this.m_lineTriangles[11] = this.m_vertexCount + 7;
			}
			if (this.m_drawStart > 0 || this.m_drawEnd < this.pointsCount)
			{
				this.SetEndCapColors();
			}
		}

		// Token: 0x060019E4 RID: 6628 RVA: 0x00088AD0 File Offset: 0x00086CD0
		private void ScaleCapVertices(int offset, float scale, Vector3 center)
		{
			this.m_lineVertices[offset] = (this.m_lineVertices[offset] - center) * scale + center;
			this.m_lineVertices[offset + 1] = (this.m_lineVertices[offset + 1] - center) * scale + center;
			this.m_lineVertices[offset + 2] = (this.m_lineVertices[offset + 2] - center) * scale + center;
			this.m_lineVertices[offset + 3] = (this.m_lineVertices[offset + 3] - center) * scale + center;
		}

		// Token: 0x060019E5 RID: 6629 RVA: 0x00088B94 File Offset: 0x00086D94
		private void SetContinuousTexture()
		{
			int num = 0;
			float num2 = 0f;
			this.SetDistances();
			int num3 = this.m_distances.Length - 1;
			float num4 = this.m_distances[num3];
			for (int i = 0; i < num3; i++)
			{
				this.m_lineUVs[num].x = num2;
				this.m_lineUVs[num + 3].x = num2;
				num2 = 1f / (num4 / this.m_distances[i + 1]);
				this.m_lineUVs[num + 1].x = num2;
				this.m_lineUVs[num + 2].x = num2;
				num += 4;
			}
			if (this.m_vectorObject != null)
			{
				this.m_vectorObject.UpdateUVs();
			}
		}

		// Token: 0x060019E6 RID: 6630 RVA: 0x00088C4C File Offset: 0x00086E4C
		private bool UseMatrix(out Matrix4x4 thisMatrix)
		{
			if (this.m_drawTransform != null)
			{
				thisMatrix = this.m_drawTransform.localToWorldMatrix;
				return true;
			}
			if (this.m_useMatrix)
			{
				thisMatrix = this.m_matrix;
				return true;
			}
			thisMatrix = Matrix4x4.identity;
			return false;
		}

		// Token: 0x060019E7 RID: 6631 RVA: 0x00088C9C File Offset: 0x00086E9C
		private bool CheckPointCount()
		{
			if (this.pointsCount < ((this.m_lineType == LineType.Points) ? 1 : 2))
			{
				this.ClearTriangles();
				this.m_vectorObject.ClearMesh();
				this.m_pointsCount = this.pointsCount;
				this.m_drawEnd = 0;
				return false;
			}
			return true;
		}

		// Token: 0x060019E8 RID: 6632 RVA: 0x00088CDA File Offset: 0x00086EDA
		private void ClearTriangles()
		{
			if (this.m_capType == EndCap.None)
			{
				this.m_lineTriangles.Clear();
				return;
			}
			this.m_lineTriangles.RemoveRange(12, this.m_lineTriangles.Count - 12);
		}

		// Token: 0x060019E9 RID: 6633 RVA: 0x00088D0C File Offset: 0x00086F0C
		private void SetupDrawStartEnd(out int start, out int end, bool clearVertices)
		{
			start = 0;
			end = this.m_pointsCount - 1;
			if (this.m_drawStart > 0)
			{
				start = this.m_drawStart;
				if (this.m_lineType == LineType.Discrete && start == this.pointsCount - 1)
				{
					start++;
				}
				if (clearVertices)
				{
					this.ZeroVertices(0, start);
				}
			}
			if (this.m_drawEnd < this.m_pointsCount - 1)
			{
				end = this.m_drawEnd;
				if (end < 0)
				{
					end = 0;
				}
				if (clearVertices)
				{
					this.ZeroVertices(end, this.m_pointsCount);
				}
			}
			if (this.m_endPointsUpdate > 0)
			{
				start = Mathf.Max(0, end - this.m_endPointsUpdate);
			}
		}

		// Token: 0x060019EA RID: 6634 RVA: 0x00088DAC File Offset: 0x00086FAC
		private void ZeroVertices(int startIndex, int endIndex)
		{
			if (this.m_lineType != LineType.Discrete)
			{
				startIndex *= 4;
				endIndex *= 4;
				if (endIndex > this.m_vertexCount)
				{
					endIndex -= 4;
				}
				for (int i = startIndex; i < endIndex; i += 4)
				{
					this.m_lineVertices[i] = VectorLine.v3zero;
					this.m_lineVertices[i + 1] = VectorLine.v3zero;
					this.m_lineVertices[i + 2] = VectorLine.v3zero;
					this.m_lineVertices[i + 3] = VectorLine.v3zero;
				}
				return;
			}
			startIndex *= 2;
			endIndex *= 2;
			for (int j = startIndex; j < endIndex; j += 2)
			{
				this.m_lineVertices[j] = VectorLine.v3zero;
				this.m_lineVertices[j + 1] = VectorLine.v3zero;
			}
		}

		// Token: 0x060019EB RID: 6635 RVA: 0x00088E6C File Offset: 0x0008706C
		private void SetupCanvasState(CanvasState wantedState)
		{
			if (wantedState == CanvasState.OnCanvas)
			{
				if (this.m_go == null)
				{
					return;
				}
				Transform transform = this.m_go.transform.parent;
				bool flag = true;
				while (transform != null)
				{
					if (transform.GetComponent<Canvas>() != null)
					{
						flag = false;
						break;
					}
					transform = transform.parent;
				}
				if (flag)
				{
					if (VectorLine.m_canvas == null)
					{
						VectorLine.SetupVectorCanvas();
					}
					this.m_go.transform.SetParent(VectorLine.m_canvas.transform, true);
				}
				this.m_canvasState = CanvasState.OnCanvas;
				if (this.m_go.GetComponent<VectorObject3D>() != null)
				{
					global::UnityEngine.Object.DestroyImmediate(this.m_go.GetComponent<VectorObject3D>());
					global::UnityEngine.Object.DestroyImmediate(this.m_go.GetComponent<MeshFilter>());
					global::UnityEngine.Object.DestroyImmediate(this.m_go.GetComponent<MeshRenderer>());
				}
				if (this.m_go.GetComponent<VectorObject2D>() == null)
				{
					this.m_vectorObject = this.m_go.AddComponent<VectorObject2D>();
				}
				else
				{
					this.m_vectorObject = this.m_go.GetComponent<VectorObject2D>();
				}
				this.m_vectorObject.SetVectorLine(this, this.m_texture, this.m_material, false);
				return;
			}
			else
			{
				if (this.m_go == null)
				{
					return;
				}
				this.m_go.transform.SetParent(null);
				this.m_canvasState = CanvasState.OffCanvas;
				if (this.m_go.GetComponent<VectorObject2D>() != null)
				{
					this.m_go.GetComponent<VectorObject2D>().DestroyNow();
					global::UnityEngine.Object.DestroyImmediate(this.m_go.GetComponent<VectorObject2D>());
				}
				this.m_vectorObject = this.m_go.GetComponent<VectorObject3D>();
				if (this.m_vectorObject == null)
				{
					this.m_vectorObject = this.m_go.AddComponent<VectorObject3D>();
				}
				this.m_vectorObject.SetVectorLine(this, this.m_texture, this.m_material, this.m_useCustomMaterial);
				return;
			}
		}

		// Token: 0x060019EC RID: 6636 RVA: 0x00089034 File Offset: 0x00087234
		public void Draw()
		{
			if (!this.m_active)
			{
				return;
			}
			if (this.m_canvasState != CanvasState.OnCanvas)
			{
				this.SetupCanvasState(CanvasState.OnCanvas);
			}
			if (this.m_vectorObject == null)
			{
				this.m_vectorObject = this.m_go.GetComponent<VectorObject2D>();
			}
			if (!this.CheckPointCount() || this.m_lineWidths == null)
			{
				return;
			}
			if (this.pointsCount != this.m_pointsCount)
			{
				this.Resize();
			}
			if (this.m_lineType == LineType.Points)
			{
				this.DrawPoints();
				return;
			}
			Matrix4x4 matrix4x;
			bool flag = this.UseMatrix(out matrix4x);
			int num = 0;
			int num2 = 0;
			this.SetupDrawStartEnd(out num, out num2, true);
			if (this.m_is2D)
			{
				this.Line2D(num, num2, matrix4x, flag);
			}
			else
			{
				this.Line3D(num, num2, matrix4x, flag);
			}
			this.CheckNormals();
			this.CheckLine(false);
			if (this.m_useTextureScale)
			{
				this.SetTextureScale();
			}
			this.m_vectorObject.UpdateVerts();
			if (this.m_collider)
			{
				this.SetCollider(true);
			}
		}

		// Token: 0x060019ED RID: 6637 RVA: 0x00089114 File Offset: 0x00087314
		private void Line2D(int start, int end, Matrix4x4 thisMatrix, bool useTransformMatrix)
		{
			Vector3 vector = VectorLine.v3zero;
			Vector3 vector2 = VectorLine.v3zero;
			Vector3 vector3 = VectorLine.v3zero;
			Vector3 vector4 = VectorLine.v3zero;
			Vector2 vector5 = new Vector2((float)Screen.width, (float)Screen.height);
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			if (this.m_lineWidths.Length > 1)
			{
				num2 = start;
				num3 = 1;
			}
			int num4;
			if (this.m_lineType == LineType.Continuous)
			{
				num4 = 1;
				num = start * 4;
			}
			else
			{
				num4 = 2;
				num2 /= 2;
				num = start * 2;
			}
			bool flag = this.smoothWidth && this.m_lineWidths.Length > 1;
			for (int i = start; i < end; i += num4)
			{
				if (useTransformMatrix)
				{
					vector = thisMatrix.MultiplyPoint3x4(this.m_points2[i]);
					vector2 = thisMatrix.MultiplyPoint3x4(this.m_points2[i + 1]);
				}
				else
				{
					vector.x = this.m_points2[i].x;
					vector.y = this.m_points2[i].y;
					vector2.x = this.m_points2[i + 1].x;
					vector2.y = this.m_points2[i + 1].y;
				}
				if (this.m_viewportDraw)
				{
					vector.x *= vector5.x;
					vector.y *= vector5.y;
					vector2.x *= vector5.x;
					vector2.y *= vector5.y;
				}
				if (vector.x == vector2.x && vector.y == vector2.y)
				{
					this.SkipQuad(ref num, ref num2, ref num3);
				}
				else
				{
					if (this.m_capLength == 0f)
					{
						vector4.x = vector2.y - vector.y;
						vector4.y = vector.x - vector2.x;
						float num5 = 1f / (float)Math.Sqrt((double)(vector4.x * vector4.x + vector4.y * vector4.y));
						vector4 *= num5 * this.m_lineWidths[num2];
						this.m_lineVertices[num].x = vector.x - vector4.x;
						this.m_lineVertices[num].y = vector.y - vector4.y;
						this.m_lineVertices[num + 3].x = vector.x + vector4.x;
						this.m_lineVertices[num + 3].y = vector.y + vector4.y;
						if (flag && i < end - num4)
						{
							vector4.x = vector2.y - vector.y;
							vector4.y = vector.x - vector2.x;
							vector4 *= num5 * this.m_lineWidths[num2 + 1];
						}
					}
					else
					{
						vector4.x = vector2.x - vector.x;
						vector4.y = vector2.y - vector.y;
						vector4 *= 1f / (float)Math.Sqrt((double)(vector4.x * vector4.x + vector4.y * vector4.y));
						vector -= vector4 * this.m_capLength;
						vector2 += vector4 * this.m_capLength;
						vector3.x = vector4.y;
						vector3.y = -vector4.x;
						vector4 = vector3 * this.m_lineWidths[num2];
						this.m_lineVertices[num].x = vector.x - vector4.x;
						this.m_lineVertices[num].y = vector.y - vector4.y;
						this.m_lineVertices[num + 3].x = vector.x + vector4.x;
						this.m_lineVertices[num + 3].y = vector.y + vector4.y;
						if (flag && i < end - num4)
						{
							vector4 = vector3 * this.m_lineWidths[num2 + 1];
						}
					}
					this.m_lineVertices[num + 2].x = vector2.x + vector4.x;
					this.m_lineVertices[num + 2].y = vector2.y + vector4.y;
					this.m_lineVertices[num + 1].x = vector2.x - vector4.x;
					this.m_lineVertices[num + 1].y = vector2.y - vector4.y;
					num += 4;
					num2 += num3;
				}
			}
			if (this.m_joins == Joins.Weld)
			{
				if (this.m_lineType == LineType.Continuous)
				{
					this.WeldJoins(start * 4 + ((start == 0) ? 4 : 0), end * 4, this.Approximately(this.m_points2[0], this.m_points2[this.m_pointsCount - 1]));
				}
				else
				{
					if ((end & 1) == 0)
					{
						end--;
					}
					this.WeldJoinsDiscrete(start + 1, end, this.Approximately(this.m_points2[0], this.m_points2[this.m_pointsCount - 1]));
				}
			}
			this.CheckDrawStartFill(start);
		}

		// Token: 0x060019EE RID: 6638 RVA: 0x0008968C File Offset: 0x0008788C
		private void Line3D(int start, int end, Matrix4x4 thisMatrix, bool useTransformMatrix)
		{
			if (!this.CheckCamera3D())
			{
				return;
			}
			Vector3 vector = VectorLine.v3zero;
			Vector3 vector2 = VectorLine.v3zero;
			Vector3 vector3 = VectorLine.v3zero;
			Vector3 vector4 = VectorLine.v3zero;
			Vector3 vector5 = VectorLine.v3zero;
			Vector3 vector6 = VectorLine.v3zero;
			int num = 0;
			int num2 = 0;
			if (this.m_lineWidths.Length > 1)
			{
				num = start;
				num2 = 1;
			}
			int num3 = start * 2;
			int num4 = 2;
			if (this.m_lineType == LineType.Continuous)
			{
				num3 = start * 4;
				num4 = 1;
			}
			Plane plane = new Plane(VectorLine.camTransform.forward, VectorLine.camTransform.position + VectorLine.camTransform.forward * VectorLine.cam3D.nearClipPlane);
			Ray ray = new Ray(VectorLine.v3zero, VectorLine.v3zero);
			float num5 = (float)Screen.height;
			bool flag = this.smoothWidth && this.m_lineWidths.Length > 1;
			for (int i = start; i < end; i += num4)
			{
				if (useTransformMatrix)
				{
					vector5 = thisMatrix.MultiplyPoint3x4(this.m_points3[i]);
					vector6 = thisMatrix.MultiplyPoint3x4(this.m_points3[i + 1]);
				}
				else
				{
					vector5 = this.m_points3[i];
					vector6 = this.m_points3[i + 1];
				}
				vector = VectorLine.cam3D.WorldToScreenPoint(vector5);
				vector2 = VectorLine.cam3D.WorldToScreenPoint(vector6);
				if ((vector.x == vector2.x && vector.y == vector2.y) || this.IntersectAndDoSkip(ref vector, ref vector2, ref vector5, ref vector6, ref num5, ref ray, ref plane))
				{
					this.SkipQuad(ref num3, ref num, ref num2);
				}
				else
				{
					if (this.m_capLength == 0f)
					{
						vector4.x = vector2.y - vector.y;
						vector4.y = vector.x - vector2.x;
						float num6 = 1f / (float)Math.Sqrt((double)(vector4.x * vector4.x + vector4.y * vector4.y));
						vector4.x *= num6 * this.m_lineWidths[num];
						vector4.y *= num6 * this.m_lineWidths[num];
						this.m_lineVertices[num3].x = vector.x - vector4.x;
						this.m_lineVertices[num3].y = vector.y - vector4.y;
						this.m_lineVertices[num3 + 3].x = vector.x + vector4.x;
						this.m_lineVertices[num3 + 3].y = vector.y + vector4.y;
						if (flag && i < end - num4)
						{
							vector4.x = vector2.y - vector.y;
							vector4.y = vector.x - vector2.x;
							vector4.x *= num6 * this.m_lineWidths[num + 1];
							vector4.y *= num6 * this.m_lineWidths[num + 1];
						}
					}
					else
					{
						vector4.x = vector2.x - vector.x;
						vector4.y = vector2.y - vector.y;
						vector4 *= 1f / (float)Math.Sqrt((double)(vector4.x * vector4.x + vector4.y * vector4.y));
						vector -= vector4 * this.m_capLength;
						vector2 += vector4 * this.m_capLength;
						vector3.x = vector4.y;
						vector3.y = -vector4.x;
						vector4 = vector3 * this.m_lineWidths[num];
						this.m_lineVertices[num3].x = vector.x - vector4.x;
						this.m_lineVertices[num3].y = vector.y - vector4.y;
						this.m_lineVertices[num3 + 3].x = vector.x + vector4.x;
						this.m_lineVertices[num3 + 3].y = vector.y + vector4.y;
						if (flag && i < end - num4)
						{
							vector4 = vector3 * this.m_lineWidths[num + 1];
						}
					}
					this.m_lineVertices[num3 + 2].x = vector2.x + vector4.x;
					this.m_lineVertices[num3 + 2].y = vector2.y + vector4.y;
					this.m_lineVertices[num3 + 1].x = vector2.x - vector4.x;
					this.m_lineVertices[num3 + 1].y = vector2.y - vector4.y;
					num3 += 4;
					num += num2;
				}
			}
			if (this.m_joins == Joins.Weld && end - start > 1)
			{
				if (this.m_lineType == LineType.Continuous)
				{
					this.WeldJoins(start * 4 + ((start == 0) ? 4 : 0), end * 4, start == 0 && end == this.m_pointsCount - 1 && this.Approximately(this.m_points3[0], this.m_points3[this.m_pointsCount - 1]));
				}
				else
				{
					if ((end & 1) == 0)
					{
						end--;
					}
					this.WeldJoinsDiscrete(start + 1, end, start == 0 && end == this.m_pointsCount - 1 && this.Approximately(this.m_points3[0], this.m_points3[this.m_pointsCount - 1]));
				}
			}
			this.CheckDrawStartFill(start);
		}

		// Token: 0x060019EF RID: 6639 RVA: 0x00089C44 File Offset: 0x00087E44
		private void CheckDrawStartFill(int start)
		{
			if (this.m_joins == Joins.Fill)
			{
				int num = start * 4;
				if (this.m_drawStart > 0 && this.m_lineVertices.Length > num && num - 3 >= 0)
				{
					this.m_lineVertices[num - 1] = this.m_lineVertices[num];
					this.m_lineVertices[num - 2] = this.m_lineVertices[num];
					this.m_lineVertices[num - 3] = this.m_lineVertices[num];
				}
			}
		}

		// Token: 0x060019F0 RID: 6640 RVA: 0x00089CC8 File Offset: 0x00087EC8
		public void Draw3D()
		{
			if (!this.m_active)
			{
				return;
			}
			if (this.m_is2D)
			{
				Debug.LogError("VectorLine.Draw3D can only be used with a Vector3 array, which \"" + this.name + "\" doesn't have");
				return;
			}
			if (this.m_canvasState != CanvasState.OffCanvas)
			{
				this.SetupCanvasState(CanvasState.OffCanvas);
			}
			if (!this.CheckPointCount() || this.m_lineWidths == null)
			{
				return;
			}
			if (this.pointsCount != this.m_pointsCount)
			{
				this.Resize();
			}
			if (!this.CheckCamera3D())
			{
				return;
			}
			if (this.m_lineType == LineType.Points)
			{
				this.DrawPoints3D();
				return;
			}
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			this.SetupDrawStartEnd(out num, out num2, true);
			Matrix4x4 matrix4x;
			bool flag = this.UseMatrix(out matrix4x);
			bool flag2 = this.smoothWidth && this.m_lineWidths.Length > 1;
			int num4 = 0;
			int num5 = 0;
			if (this.m_lineWidths.Length > 1)
			{
				num3 = num;
				num5 = 1;
			}
			int num6;
			if (this.m_lineType == LineType.Continuous)
			{
				num6 = 1;
				num4 = num * 4;
			}
			else
			{
				num3 /= 2;
				num6 = 2;
				num4 = num * 2;
			}
			Vector3 vector = VectorLine.v3zero;
			Vector3 vector2 = VectorLine.v3zero;
			Vector3 vector3 = VectorLine.v3zero;
			Vector3 vector4 = VectorLine.v3zero;
			Vector3 vector5 = VectorLine.v3zero;
			Vector3 vector6 = VectorLine.v3zero;
			Plane plane = new Plane(VectorLine.camTransform.forward, VectorLine.camTransform.position + VectorLine.camTransform.forward * VectorLine.cam3D.nearClipPlane);
			Ray ray = new Ray(VectorLine.v3zero, VectorLine.v3zero);
			float num7 = (float)Screen.height;
			for (int i = num; i < num2; i += num6)
			{
				if (flag)
				{
					vector5 = matrix4x.MultiplyPoint3x4(this.m_points3[i]);
					vector6 = matrix4x.MultiplyPoint3x4(this.m_points3[i + 1]);
				}
				else
				{
					vector5 = this.m_points3[i];
					vector6 = this.m_points3[i + 1];
				}
				vector3 = VectorLine.cam3D.WorldToScreenPoint(vector5);
				vector4 = VectorLine.cam3D.WorldToScreenPoint(vector6);
				if ((vector3.x == vector4.x && vector3.y == vector4.y) || this.IntersectAndDoSkip(ref vector3, ref vector4, ref vector5, ref vector6, ref num7, ref ray, ref plane))
				{
					this.SkipQuad3D(ref num4, ref num3, ref num5);
				}
				else
				{
					vector2.x = vector4.y - vector3.y;
					vector2.y = vector3.x - vector4.x;
					vector = vector2 / (float)Math.Sqrt((double)(vector2.x * vector2.x + vector2.y * vector2.y));
					vector2.x = vector.x * this.m_lineWidths[num3];
					vector2.y = vector.y * this.m_lineWidths[num3];
					this.m_screenPoints[num4].x = vector3.x - vector2.x;
					this.m_screenPoints[num4].y = vector3.y - vector2.y;
					this.m_screenPoints[num4].z = vector3.z - vector2.z;
					this.m_screenPoints[num4 + 3].x = vector3.x + vector2.x;
					this.m_screenPoints[num4 + 3].y = vector3.y + vector2.y;
					this.m_screenPoints[num4 + 3].z = vector3.z + vector2.z;
					this.m_lineVertices[num4] = VectorLine.cam3D.ScreenToWorldPoint(this.m_screenPoints[num4]);
					this.m_lineVertices[num4 + 3] = VectorLine.cam3D.ScreenToWorldPoint(this.m_screenPoints[num4 + 3]);
					if (flag2 && i < num2 - num6)
					{
						vector2.x = vector.x * this.m_lineWidths[num3 + 1];
						vector2.y = vector.y * this.m_lineWidths[num3 + 1];
					}
					this.m_screenPoints[num4 + 2].x = vector4.x + vector2.x;
					this.m_screenPoints[num4 + 2].y = vector4.y + vector2.y;
					this.m_screenPoints[num4 + 2].z = vector4.z + vector2.z;
					this.m_screenPoints[num4 + 1].x = vector4.x - vector2.x;
					this.m_screenPoints[num4 + 1].y = vector4.y - vector2.y;
					this.m_screenPoints[num4 + 1].z = vector4.z - vector2.z;
					this.m_lineVertices[num4 + 2] = VectorLine.cam3D.ScreenToWorldPoint(this.m_screenPoints[num4 + 2]);
					this.m_lineVertices[num4 + 1] = VectorLine.cam3D.ScreenToWorldPoint(this.m_screenPoints[num4 + 1]);
					num4 += 4;
					num3 += num5;
				}
			}
			if (this.m_joins == Joins.Weld && num2 - num > 1)
			{
				if (this.m_lineType == LineType.Continuous)
				{
					this.WeldJoins3D(num * 4 + ((num == 0) ? 4 : 0), num2 * 4, num == 0 && num2 == this.m_pointsCount - 1 && this.Approximately(this.m_points3[0], this.m_points3[this.m_pointsCount - 1]));
				}
				else
				{
					if ((num2 & 1) == 0)
					{
						num2--;
					}
					this.WeldJoinsDiscrete3D(num + 1, num2, num == 0 && num2 == this.m_pointsCount - 1 && this.Approximately(this.m_points3[0], this.m_points3[this.m_pointsCount - 1]));
				}
			}
			this.CheckDrawStartFill(num);
			this.CheckLine(true);
			if (this.m_useTextureScale)
			{
				this.SetTextureScale();
			}
			this.m_vectorObject.UpdateVerts();
			this.CheckNormals();
			if (this.m_collider)
			{
				this.SetCollider(false);
			}
		}

		// Token: 0x060019F1 RID: 6641 RVA: 0x0008A2F8 File Offset: 0x000884F8
		private bool IntersectAndDoSkip(ref Vector3 pos1, ref Vector3 pos2, ref Vector3 p1, ref Vector3 p2, ref float screenHeight, ref Ray ray, ref Plane cameraPlane)
		{
			if (pos1.z < 0f)
			{
				if (pos2.z < 0f)
				{
					return true;
				}
				pos1 = VectorLine.cam3D.WorldToScreenPoint(this.PlaneIntersectionPoint(ref ray, ref cameraPlane, ref p2, ref p1));
				Vector3 vector = VectorLine.camTransform.InverseTransformPoint(p1);
				if ((vector.y < -1f && pos1.y > screenHeight) || (vector.y > 1f && pos1.y < 0f))
				{
					return true;
				}
			}
			if (pos2.z < 0f)
			{
				pos2 = VectorLine.cam3D.WorldToScreenPoint(this.PlaneIntersectionPoint(ref ray, ref cameraPlane, ref p1, ref p2));
				Vector3 vector2 = VectorLine.camTransform.InverseTransformPoint(p2);
				if ((vector2.y < -1f && pos2.y > screenHeight) || (vector2.y > 1f && pos2.y < 0f))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060019F2 RID: 6642 RVA: 0x0008A3F4 File Offset: 0x000885F4
		private Vector3 PlaneIntersectionPoint(ref Ray ray, ref Plane plane, ref Vector3 p1, ref Vector3 p2)
		{
			ray.origin = p1;
			ray.direction = p2 - p1;
			float num = 0f;
			plane.Raycast(ray, out num);
			return ray.GetPoint(num);
		}

		// Token: 0x060019F3 RID: 6643 RVA: 0x0008A444 File Offset: 0x00088644
		private void DrawPoints()
		{
			if (!this.CheckCamera3D())
			{
				return;
			}
			Matrix4x4 matrix4x;
			bool flag = this.UseMatrix(out matrix4x);
			int num;
			int num2;
			this.SetupDrawStartEnd(out num, out num2, true);
			Vector2 vector = new Vector2((float)Screen.width, (float)Screen.height);
			int num3 = num * 4;
			int num4 = ((this.m_lineWidths.Length > 1) ? 1 : 0);
			int num5 = num;
			Vector3 vector2 = new Vector3(this.m_lineWidths[0], this.m_lineWidths[0], 0f);
			Vector3 vector3 = new Vector3(-this.m_lineWidths[0], this.m_lineWidths[0], 0f);
			if (this.m_is2D)
			{
				for (int i = num; i <= num2; i++)
				{
					Vector3 vector4;
					if (flag)
					{
						vector4 = matrix4x.MultiplyPoint3x4(this.m_points2[i]);
					}
					else
					{
						vector4.x = this.m_points2[i].x;
						vector4.y = this.m_points2[i].y;
					}
					if (this.m_viewportDraw)
					{
						vector4.x *= vector.x;
						vector4.y *= vector.y;
					}
					if (num4 != 0)
					{
						vector2.x = (vector2.y = (vector3.y = this.m_lineWidths[num5]));
						vector3.x = -this.m_lineWidths[num5];
						num5++;
					}
					this.m_lineVertices[num3].x = vector4.x + vector3.x;
					this.m_lineVertices[num3].y = vector4.y + vector3.y;
					this.m_lineVertices[num3 + 3].x = vector4.x - vector2.x;
					this.m_lineVertices[num3 + 3].y = vector4.y - vector2.y;
					this.m_lineVertices[num3 + 1].x = vector4.x + vector2.x;
					this.m_lineVertices[num3 + 1].y = vector4.y + vector2.y;
					this.m_lineVertices[num3 + 2].x = vector4.x - vector3.x;
					this.m_lineVertices[num3 + 2].y = vector4.y - vector3.y;
					num3 += 4;
				}
			}
			else
			{
				for (int j = num; j <= num2; j++)
				{
					Vector3 vector4 = (flag ? VectorLine.cam3D.WorldToScreenPoint(matrix4x.MultiplyPoint3x4(this.m_points3[j])) : VectorLine.cam3D.WorldToScreenPoint(this.m_points3[j]));
					if (vector4.z < 0f)
					{
						this.SkipQuad(ref num3, ref num5, ref num4);
					}
					else
					{
						if (num4 != 0)
						{
							vector2.x = (vector2.y = (vector3.y = this.m_lineWidths[num5]));
							vector3.x = -this.m_lineWidths[num5];
							num5++;
						}
						this.m_lineVertices[num3].x = vector4.x + vector3.x;
						this.m_lineVertices[num3].y = vector4.y + vector3.y;
						this.m_lineVertices[num3 + 3].x = vector4.x - vector2.x;
						this.m_lineVertices[num3 + 3].y = vector4.y - vector2.y;
						this.m_lineVertices[num3 + 1].x = vector4.x + vector2.x;
						this.m_lineVertices[num3 + 1].y = vector4.y + vector2.y;
						this.m_lineVertices[num3 + 2].x = vector4.x - vector3.x;
						this.m_lineVertices[num3 + 2].y = vector4.y - vector3.y;
						num3 += 4;
					}
				}
			}
			this.CheckNormals();
			this.m_vectorObject.UpdateVerts();
		}

		// Token: 0x060019F4 RID: 6644 RVA: 0x0008A8B4 File Offset: 0x00088AB4
		private void DrawPoints3D()
		{
			if (!this.m_active)
			{
				return;
			}
			Matrix4x4 matrix4x;
			bool flag = this.UseMatrix(out matrix4x);
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			this.SetupDrawStartEnd(out num, out num2, true);
			int num4 = num * 4;
			int num5 = 0;
			if (this.m_lineWidths.Length > 1)
			{
				num3 = num;
				num5 = 1;
			}
			Vector3 vector = VectorLine.v3zero;
			Vector3 vector2 = VectorLine.v3zero;
			Vector3 vector3 = VectorLine.v3zero;
			for (int i = num; i <= num2; i++)
			{
				vector = (flag ? VectorLine.cam3D.WorldToScreenPoint(matrix4x.MultiplyPoint3x4(this.m_points3[i])) : VectorLine.cam3D.WorldToScreenPoint(this.m_points3[i]));
				if (vector.z < 0f)
				{
					this.SkipQuad(ref num4, ref num3, ref num5);
				}
				else
				{
					vector2.x = (vector2.y = (vector3.y = this.m_lineWidths[num3]));
					vector3.x = -this.m_lineWidths[num3];
					this.m_lineVertices[num4] = VectorLine.cam3D.ScreenToWorldPoint(vector + vector3);
					this.m_lineVertices[num4 + 3] = VectorLine.cam3D.ScreenToWorldPoint(vector - vector2);
					this.m_lineVertices[num4 + 1] = VectorLine.cam3D.ScreenToWorldPoint(vector + vector2);
					this.m_lineVertices[num4 + 2] = VectorLine.cam3D.ScreenToWorldPoint(vector - vector3);
					num4 += 4;
					num3 += num5;
				}
			}
			this.CheckNormals();
			this.m_vectorObject.UpdateVerts();
		}

		// Token: 0x060019F5 RID: 6645 RVA: 0x0008AA64 File Offset: 0x00088C64
		private void SkipQuad(ref int idx, ref int widthIdx, ref int widthIdxAdd)
		{
			this.m_lineVertices[idx] = VectorLine.v3zero;
			this.m_lineVertices[idx + 1] = VectorLine.v3zero;
			this.m_lineVertices[idx + 2] = VectorLine.v3zero;
			this.m_lineVertices[idx + 3] = VectorLine.v3zero;
			idx += 4;
			widthIdx += widthIdxAdd;
		}

		// Token: 0x060019F6 RID: 6646 RVA: 0x0008AACC File Offset: 0x00088CCC
		private void SkipQuad3D(ref int idx, ref int widthIdx, ref int widthIdxAdd)
		{
			this.m_lineVertices[idx] = VectorLine.v3zero;
			this.m_lineVertices[idx + 1] = VectorLine.v3zero;
			this.m_lineVertices[idx + 2] = VectorLine.v3zero;
			this.m_lineVertices[idx + 3] = VectorLine.v3zero;
			this.m_screenPoints[idx] = VectorLine.v3zero;
			this.m_screenPoints[idx + 1] = VectorLine.v3zero;
			this.m_screenPoints[idx + 2] = VectorLine.v3zero;
			this.m_screenPoints[idx + 3] = VectorLine.v3zero;
			idx += 4;
			widthIdx += widthIdxAdd;
		}

		// Token: 0x060019F7 RID: 6647 RVA: 0x0008AB84 File Offset: 0x00088D84
		private void WeldJoins(int start, int end, bool connectFirstAndLast)
		{
			if (connectFirstAndLast)
			{
				this.SetIntersectionPoint(this.m_vertexCount - 4, this.m_vertexCount - 3, 0, 1);
				this.SetIntersectionPoint(this.m_vertexCount - 1, this.m_vertexCount - 2, 3, 2);
			}
			for (int i = start; i < end; i += 4)
			{
				this.SetIntersectionPoint(i - 4, i - 3, i, i + 1);
				this.SetIntersectionPoint(i - 1, i - 2, i + 3, i + 2);
			}
		}

		// Token: 0x060019F8 RID: 6648 RVA: 0x0008ABF4 File Offset: 0x00088DF4
		private void WeldJoinsDiscrete(int start, int end, bool connectFirstAndLast)
		{
			if (connectFirstAndLast)
			{
				this.SetIntersectionPoint(this.m_vertexCount - 4, this.m_vertexCount - 3, 0, 1);
				this.SetIntersectionPoint(this.m_vertexCount - 1, this.m_vertexCount - 2, 3, 2);
			}
			int num = (start + 1) / 2 * 4;
			if (this.m_is2D)
			{
				for (int i = start; i < end; i += 2)
				{
					if (this.m_points2[i] == this.m_points2[i + 1])
					{
						this.SetIntersectionPoint(num - 4, num - 3, num, num + 1);
						this.SetIntersectionPoint(num - 1, num - 2, num + 3, num + 2);
					}
					num += 4;
				}
				return;
			}
			for (int j = start; j < end; j += 2)
			{
				if (this.m_points3[j] == this.m_points3[j + 1])
				{
					this.SetIntersectionPoint(num - 4, num - 3, num, num + 1);
					this.SetIntersectionPoint(num - 1, num - 2, num + 3, num + 2);
				}
				num += 4;
			}
		}

		// Token: 0x060019F9 RID: 6649 RVA: 0x0008ACEC File Offset: 0x00088EEC
		private void SetIntersectionPoint(int p1, int p2, int p3, int p4)
		{
			Vector3 vector = this.m_lineVertices[p1];
			Vector3 vector2 = this.m_lineVertices[p2];
			Vector3 vector3 = this.m_lineVertices[p3];
			Vector3 vector4 = this.m_lineVertices[p4];
			if ((vector.x == vector2.x && vector.y == vector2.y) || (vector3.x == vector4.x && vector3.y == vector4.y))
			{
				return;
			}
			float num = (vector4.y - vector3.y) * (vector2.x - vector.x) - (vector4.x - vector3.x) * (vector2.y - vector.y);
			if (num > -0.005f && num < 0.005f)
			{
				if (Mathf.Abs(vector2.x - vector3.x) < 0.005f && Mathf.Abs(vector2.y - vector3.y) < 0.005f)
				{
					this.m_lineVertices[p2] = (vector2 + vector3) * 0.5f;
					this.m_lineVertices[p3] = this.m_lineVertices[p2];
				}
				return;
			}
			float num2 = ((vector4.x - vector3.x) * (vector.y - vector3.y) - (vector4.y - vector3.y) * (vector.x - vector3.x)) / num;
			Vector3 vector5 = new Vector3(vector.x + num2 * (vector2.x - vector.x), vector.y + num2 * (vector2.y - vector.y), vector.z);
			if ((vector5 - vector2).sqrMagnitude > this.m_maxWeldDistance)
			{
				return;
			}
			this.m_lineVertices[p2] = vector5;
			this.m_lineVertices[p3] = vector5;
		}

		// Token: 0x060019FA RID: 6650 RVA: 0x0008AEC8 File Offset: 0x000890C8
		private void WeldJoins3D(int start, int end, bool connectFirstAndLast)
		{
			if (connectFirstAndLast)
			{
				this.SetIntersectionPoint3D(this.m_vertexCount - 4, this.m_vertexCount - 3, 0, 1);
				this.SetIntersectionPoint3D(this.m_vertexCount - 1, this.m_vertexCount - 2, 3, 2);
			}
			if (this.m_drawStart > 0)
			{
				start += 4;
			}
			for (int i = start; i < end; i += 4)
			{
				this.SetIntersectionPoint3D(i - 4, i - 3, i, i + 1);
				this.SetIntersectionPoint3D(i - 1, i - 2, i + 3, i + 2);
			}
		}

		// Token: 0x060019FB RID: 6651 RVA: 0x0008AF44 File Offset: 0x00089144
		private void WeldJoinsDiscrete3D(int start, int end, bool connectFirstAndLast)
		{
			if (connectFirstAndLast)
			{
				this.SetIntersectionPoint3D(this.m_vertexCount - 4, this.m_vertexCount - 3, 0, 1);
				this.SetIntersectionPoint3D(this.m_vertexCount - 1, this.m_vertexCount - 2, 3, 2);
			}
			int num = (start + 1) / 2 * 4;
			for (int i = start; i < end; i += 2)
			{
				if (this.m_points3[i] == this.m_points3[i + 1])
				{
					this.SetIntersectionPoint3D(num - 4, num - 3, num, num + 1);
					this.SetIntersectionPoint3D(num - 1, num - 2, num + 3, num + 2);
				}
				num += 4;
			}
		}

		// Token: 0x060019FC RID: 6652 RVA: 0x0008AFE0 File Offset: 0x000891E0
		private void SetIntersectionPoint3D(int p1, int p2, int p3, int p4)
		{
			Vector3 vector = this.m_screenPoints[p1];
			Vector3 vector2 = this.m_screenPoints[p2];
			Vector3 vector3 = this.m_screenPoints[p3];
			Vector3 vector4 = this.m_screenPoints[p4];
			if ((vector.x == vector2.x && vector.y == vector2.y) || (vector3.x == vector4.x && vector3.y == vector4.y))
			{
				return;
			}
			float num = (vector4.y - vector3.y) * (vector2.x - vector.x) - (vector4.x - vector3.x) * (vector2.y - vector.y);
			if (num > -0.005f && num < 0.005f)
			{
				if (Mathf.Abs(vector2.x - vector3.x) < 0.005f && Mathf.Abs(vector2.y - vector3.y) < 0.005f)
				{
					this.m_lineVertices[p2] = VectorLine.cam3D.ScreenToWorldPoint((vector2 + vector3) * 0.5f);
					this.m_lineVertices[p3] = this.m_lineVertices[p2];
				}
				return;
			}
			float num2 = ((vector4.x - vector3.x) * (vector.y - vector3.y) - (vector4.y - vector3.y) * (vector.x - vector3.x)) / num;
			Vector3 vector5 = new Vector3(vector.x + num2 * (vector2.x - vector.x), vector.y + num2 * (vector2.y - vector.y), vector.z);
			if ((vector5 - vector2).sqrMagnitude > this.m_maxWeldDistance)
			{
				return;
			}
			this.m_lineVertices[p2] = VectorLine.cam3D.ScreenToWorldPoint(vector5);
			this.m_lineVertices[p3] = this.m_lineVertices[p2];
		}

		// Token: 0x060019FD RID: 6653 RVA: 0x0008B1D9 File Offset: 0x000893D9
		public static void LineManagerCheckDistance()
		{
			VectorLine.lineManager.StartCheckDistance();
		}

		// Token: 0x060019FE RID: 6654 RVA: 0x0008B1E5 File Offset: 0x000893E5
		public static void LineManagerDisable()
		{
			VectorLine.lineManager.DisableIfUnused();
		}

		// Token: 0x060019FF RID: 6655 RVA: 0x0008B1F1 File Offset: 0x000893F1
		public static void LineManagerEnable()
		{
			VectorLine.lineManager.EnableIfUsed();
		}

		// Token: 0x06001A00 RID: 6656 RVA: 0x0008B1FD File Offset: 0x000893FD
		public void Draw3DAuto()
		{
			this.Draw3DAuto(0f);
		}

		// Token: 0x06001A01 RID: 6657 RVA: 0x0008B20A File Offset: 0x0008940A
		public void Draw3DAuto(float time)
		{
			if (time < 0f)
			{
				time = 0f;
			}
			VectorLine.lineManager.AddLine(this, this.m_drawTransform, time);
			this.m_isAutoDrawing = true;
			this.Draw3D();
		}

		// Token: 0x06001A02 RID: 6658 RVA: 0x0008B23A File Offset: 0x0008943A
		public void StopDrawing3DAuto()
		{
			VectorLine.lineManager.RemoveLine(this);
			this.m_isAutoDrawing = false;
		}

		// Token: 0x06001A03 RID: 6659 RVA: 0x0008B250 File Offset: 0x00089450
		private void SetTextureScale()
		{
			if (this.pointsCount != this.m_pointsCount)
			{
				this.Resize();
			}
			int num;
			int num2;
			this.SetupDrawStartEnd(out num, out num2, false);
			int num3 = ((this.m_lineType != LineType.Discrete) ? 1 : 2);
			int num4 = 0;
			int num5 = 0;
			int num6 = ((this.m_lineWidths.Length == 1) ? 0 : 1);
			float num7 = 1f / this.m_textureScale;
			bool flag = this.m_drawTransform != null;
			Matrix4x4 matrix4x = (flag ? this.m_drawTransform.localToWorldMatrix : Matrix4x4.identity);
			Vector2 vector = Vector2.zero;
			Vector2 vector2 = Vector2.zero;
			Vector2 zero = Vector2.zero;
			float num8 = this.m_textureOffset;
			float num9 = this.m_capLength * 2f;
			if (this.m_is2D)
			{
				for (int i = 0; i < num2; i += num3)
				{
					if (!this.m_viewportDraw)
					{
						if (flag)
						{
							vector = matrix4x.MultiplyPoint3x4(this.m_points2[i]);
							vector2 = matrix4x.MultiplyPoint3x4(this.m_points2[i + 1]);
						}
						else
						{
							vector.x = this.m_points2[i].x;
							vector.y = this.m_points2[i].y;
							vector2.x = this.m_points2[i + 1].x;
							vector2.y = this.m_points2[i + 1].y;
						}
					}
					else if (flag)
					{
						vector = matrix4x.MultiplyPoint3x4(new Vector2(this.m_points2[i].x * (float)Screen.width, this.m_points2[i].y * (float)Screen.height));
						vector2 = matrix4x.MultiplyPoint3x4(new Vector2(this.m_points2[i + 1].x * (float)Screen.width, this.m_points2[i + 1].y * (float)Screen.height));
					}
					else
					{
						vector = new Vector2(this.m_points2[i].x * (float)Screen.width, this.m_points2[i].y * (float)Screen.height);
						vector2 = new Vector2(this.m_points2[i + 1].x * (float)Screen.width, this.m_points2[i + 1].y * (float)Screen.height);
					}
					zero.x = vector2.x - vector.x;
					zero.y = vector2.y - vector.y;
					float num10 = num7 / (this.m_lineWidths[num5] * 2f / ((float)Math.Sqrt((double)(zero.x * zero.x + zero.y * zero.y)) + num9));
					this.m_lineUVs[num4].x = num8;
					this.m_lineUVs[num4 + 3].x = num8;
					this.m_lineUVs[num4 + 2].x = num10 + num8;
					this.m_lineUVs[num4 + 1].x = num10 + num8;
					num4 += 4;
					num8 = (num8 + num10) % 1f;
					num5 += num6;
				}
			}
			else
			{
				if (!this.CheckCamera3D())
				{
					return;
				}
				for (int j = 0; j < num2; j += num3)
				{
					if (flag)
					{
						vector = VectorLine.cam3D.WorldToScreenPoint(matrix4x.MultiplyPoint3x4(this.m_points3[j]));
						vector2 = VectorLine.cam3D.WorldToScreenPoint(matrix4x.MultiplyPoint3x4(this.m_points3[j + 1]));
					}
					else
					{
						vector = VectorLine.cam3D.WorldToScreenPoint(this.m_points3[j]);
						vector2 = VectorLine.cam3D.WorldToScreenPoint(this.m_points3[j + 1]);
					}
					zero.x = vector.x - vector2.x;
					zero.y = vector.y - vector2.y;
					float num11 = num7 / (this.m_lineWidths[num5] * 2f / (float)Math.Sqrt((double)(zero.x * zero.x + zero.y * zero.y)));
					this.m_lineUVs[num4].x = num8;
					this.m_lineUVs[num4 + 3].x = num8;
					this.m_lineUVs[num4 + 2].x = num11 + num8;
					this.m_lineUVs[num4 + 1].x = num11 + num8;
					num4 += 4;
					num8 = (num8 + num11) % 1f;
					num5 += num6;
				}
			}
			if (this.m_vectorObject != null)
			{
				this.m_vectorObject.UpdateUVs();
			}
		}

		// Token: 0x06001A04 RID: 6660 RVA: 0x0008B774 File Offset: 0x00089974
		private void ResetTextureScale()
		{
			for (int i = 0; i < this.m_vertexCount; i += 4)
			{
				this.m_lineUVs[i].x = 0f;
				this.m_lineUVs[i + 3].x = 0f;
				this.m_lineUVs[i + 2].x = 1f;
				this.m_lineUVs[i + 1].x = 1f;
			}
			if (this.m_vectorObject != null)
			{
				this.m_vectorObject.UpdateUVs();
			}
		}

		// Token: 0x06001A05 RID: 6661 RVA: 0x0008B804 File Offset: 0x00089A04
		private void SetCollider(bool convertToWorldSpace)
		{
			if (!VectorLine.cam3D)
			{
				VectorLine.SetCamera3D();
				if (!VectorLine.cam3D)
				{
					Debug.LogError("No camera available...use VectorLine.SetCamera3D to assign a camera");
					return;
				}
			}
			if (VectorLine.cam3D.transform.rotation != Quaternion.identity)
			{
				Debug.LogWarning("The line collider will not be correct if the camera is rotated");
			}
			Vector3 vector = new Vector3(0f, 0f, -VectorLine.cam3D.transform.position.z);
			int drawStart = this.drawStart;
			int drawEnd = this.drawEnd;
			bool flag = this.m_capType != EndCap.None && this.m_capType <= EndCap.Mirror && this.drawStart == 0;
			bool flag2 = this.m_capType != EndCap.None && this.m_capType >= EndCap.Both && this.drawEnd == this.pointsCount - 1;
			int i = 0;
			if (this.m_lineType == LineType.Continuous)
			{
				EdgeCollider2D edgeCollider2D = this.m_go.GetComponent(typeof(EdgeCollider2D)) as EdgeCollider2D;
				int num = (drawEnd - drawStart) * 4 + 1;
				if (flag)
				{
					num += 4;
				}
				if (flag2)
				{
					num += 4;
				}
				Vector2[] array = new Vector2[num];
				int num2 = 0;
				int num3 = array.Length - 2;
				if (convertToWorldSpace)
				{
					if (flag)
					{
						i = this.m_vertexCount;
						this.SetPathWorldVerticesContinuous(ref i, ref vector, ref num2, ref num3, array);
					}
					for (i = drawStart * 4; i < drawEnd * 4; i += 4)
					{
						this.SetPathWorldVerticesContinuous(ref i, ref vector, ref num2, ref num3, array);
					}
					if (flag2)
					{
						i = this.m_vertexCount + 4;
						this.SetPathWorldVerticesContinuous(ref i, ref vector, ref num2, ref num3, array);
					}
				}
				else
				{
					if (flag)
					{
						i = this.m_vertexCount;
						this.SetPathVerticesContinuous(ref i, ref num2, ref num3, array);
					}
					for (i = drawStart * 4; i < drawEnd * 4; i += 4)
					{
						this.SetPathVerticesContinuous(ref i, ref num2, ref num3, array);
					}
					if (flag)
					{
						i = this.m_vertexCount + 4;
						this.SetPathVerticesContinuous(ref i, ref num2, ref num3, array);
					}
				}
				array[array.Length - 1] = array[0];
				edgeCollider2D.points = array;
				return;
			}
			PolygonCollider2D polygonCollider2D = this.m_go.GetComponent(typeof(PolygonCollider2D)) as PolygonCollider2D;
			Vector2[] array2 = new Vector2[4];
			int num4 = (drawEnd - drawStart + 1) / 2;
			if (flag)
			{
				num4++;
			}
			if (flag2)
			{
				num4++;
			}
			polygonCollider2D.pathCount = num4;
			int num5 = (drawEnd + 1) / 2 * 4;
			int num6 = 0;
			if (convertToWorldSpace)
			{
				if (flag)
				{
					i = this.m_vertexCount;
					this.SetPathWorldVerticesDiscrete(ref i, ref vector, ref num6, array2, polygonCollider2D);
				}
				for (i = drawStart / 2 * 4; i < num5; i += 4)
				{
					this.SetPathWorldVerticesDiscrete(ref i, ref vector, ref num6, array2, polygonCollider2D);
				}
				if (flag2)
				{
					i = this.m_vertexCount + 4;
					this.SetPathWorldVerticesDiscrete(ref i, ref vector, ref num6, array2, polygonCollider2D);
					return;
				}
			}
			else
			{
				if (flag)
				{
					i = this.m_vertexCount;
					this.SetPathVerticesDiscrete(ref i, ref num6, array2, polygonCollider2D);
				}
				for (i = drawStart / 2 * 4; i < num5; i += 4)
				{
					this.SetPathVerticesDiscrete(ref i, ref num6, array2, polygonCollider2D);
				}
				if (flag2)
				{
					i = this.m_vertexCount + 4;
					this.SetPathVerticesDiscrete(ref i, ref num6, array2, polygonCollider2D);
				}
			}
		}

		// Token: 0x06001A06 RID: 6662 RVA: 0x0008BB18 File Offset: 0x00089D18
		private void SetPathVerticesContinuous(ref int i, ref int startIdx, ref int endIdx, Vector2[] path)
		{
			path[startIdx].x = this.m_lineVertices[i].x;
			path[startIdx].y = this.m_lineVertices[i].y;
			path[startIdx + 1].x = this.m_lineVertices[i + 1].x;
			path[startIdx + 1].y = this.m_lineVertices[i + 1].y;
			path[endIdx].x = this.m_lineVertices[i + 3].x;
			path[endIdx].y = this.m_lineVertices[i + 3].y;
			path[endIdx - 1].x = this.m_lineVertices[i + 2].x;
			path[endIdx - 1].y = this.m_lineVertices[i + 2].y;
			startIdx += 2;
			endIdx -= 2;
		}

		// Token: 0x06001A07 RID: 6663 RVA: 0x0008BC48 File Offset: 0x00089E48
		private void SetPathWorldVerticesContinuous(ref int i, ref Vector3 v3, ref int startIdx, ref int endIdx, Vector2[] path)
		{
			v3.x = this.m_lineVertices[i].x;
			v3.y = this.m_lineVertices[i].y;
			path[startIdx] = VectorLine.cam3D.ScreenToWorldPoint(v3);
			v3.x = this.m_lineVertices[i + 1].x;
			v3.y = this.m_lineVertices[i + 1].y;
			path[startIdx + 1] = VectorLine.cam3D.ScreenToWorldPoint(v3);
			v3.x = this.m_lineVertices[i + 3].x;
			v3.y = this.m_lineVertices[i + 3].y;
			path[endIdx] = VectorLine.cam3D.ScreenToWorldPoint(v3);
			v3.x = this.m_lineVertices[i + 2].x;
			v3.y = this.m_lineVertices[i + 2].y;
			path[endIdx - 1] = VectorLine.cam3D.ScreenToWorldPoint(v3);
			startIdx += 2;
			endIdx -= 2;
		}

		// Token: 0x06001A08 RID: 6664 RVA: 0x0008BDB0 File Offset: 0x00089FB0
		private void SetPathVerticesDiscrete(ref int i, ref int pIdx, Vector2[] path, PolygonCollider2D collider)
		{
			path[0].x = this.m_lineVertices[i].x;
			path[0].y = this.m_lineVertices[i].y;
			path[1].x = this.m_lineVertices[i + 3].x;
			path[1].y = this.m_lineVertices[i + 3].y;
			path[2].x = this.m_lineVertices[i + 2].x;
			path[2].y = this.m_lineVertices[i + 2].y;
			path[3].x = this.m_lineVertices[i + 1].x;
			path[3].y = this.m_lineVertices[i + 1].y;
			int num = pIdx;
			pIdx = num + 1;
			collider.SetPath(num, path);
		}

		// Token: 0x06001A09 RID: 6665 RVA: 0x0008BECC File Offset: 0x0008A0CC
		private void SetPathWorldVerticesDiscrete(ref int i, ref Vector3 v3, ref int pIdx, Vector2[] path, PolygonCollider2D collider)
		{
			v3.x = this.m_lineVertices[i].x;
			v3.y = this.m_lineVertices[i].y;
			path[0] = VectorLine.cam3D.ScreenToWorldPoint(v3);
			v3.x = this.m_lineVertices[i + 3].x;
			v3.y = this.m_lineVertices[i + 3].y;
			path[1] = VectorLine.cam3D.ScreenToWorldPoint(v3);
			v3.x = this.m_lineVertices[i + 2].x;
			v3.y = this.m_lineVertices[i + 2].y;
			path[2] = VectorLine.cam3D.ScreenToWorldPoint(v3);
			v3.x = this.m_lineVertices[i + 1].x;
			v3.y = this.m_lineVertices[i + 1].y;
			path[3] = VectorLine.cam3D.ScreenToWorldPoint(v3);
			int num = pIdx;
			pIdx = num + 1;
			collider.SetPath(num, path);
		}

		// Token: 0x06001A0A RID: 6666 RVA: 0x0008C02C File Offset: 0x0008A22C
		public static List<Vector3> BytesToVector3List(byte[] lineBytes)
		{
			if (lineBytes.Length % 12 != 0)
			{
				Debug.LogError("VectorLine.BytesToVector3Array: Incorrect input byte length...must be a multiple of 12");
				return null;
			}
			VectorLine.SetupByteBlock();
			List<Vector3> list = new List<Vector3>(lineBytes.Length / 12);
			for (int i = 0; i < lineBytes.Length; i += 12)
			{
				list.Add(new Vector3(VectorLine.ConvertToFloat(lineBytes, i), VectorLine.ConvertToFloat(lineBytes, i + 4), VectorLine.ConvertToFloat(lineBytes, i + 8)));
			}
			return list;
		}

		// Token: 0x06001A0B RID: 6667 RVA: 0x0008C094 File Offset: 0x0008A294
		public static List<Vector2> BytesToVector2List(byte[] lineBytes)
		{
			if (lineBytes.Length % 8 != 0)
			{
				Debug.LogError("VectorLine.BytesToVector2Array: Incorrect input byte length...must be a multiple of 8");
				return null;
			}
			VectorLine.SetupByteBlock();
			List<Vector2> list = new List<Vector2>(lineBytes.Length / 8);
			for (int i = 0; i < lineBytes.Length; i += 8)
			{
				list.Add(new Vector2(VectorLine.ConvertToFloat(lineBytes, i), VectorLine.ConvertToFloat(lineBytes, i + 4)));
			}
			return list;
		}

		// Token: 0x06001A0C RID: 6668 RVA: 0x0008C0EE File Offset: 0x0008A2EE
		private static void SetupByteBlock()
		{
			if (VectorLine.byteBlock == null)
			{
				VectorLine.byteBlock = new byte[4];
			}
			if (BitConverter.IsLittleEndian)
			{
				VectorLine.endianDiff1 = 0;
				VectorLine.endianDiff2 = 0;
				return;
			}
			VectorLine.endianDiff1 = 3;
			VectorLine.endianDiff2 = 1;
		}

		// Token: 0x06001A0D RID: 6669 RVA: 0x0008C124 File Offset: 0x0008A324
		private static float ConvertToFloat(byte[] bytes, int i)
		{
			VectorLine.byteBlock[VectorLine.endianDiff1] = bytes[i];
			VectorLine.byteBlock[1 + VectorLine.endianDiff2] = bytes[i + 1];
			VectorLine.byteBlock[2 - VectorLine.endianDiff2] = bytes[i + 2];
			VectorLine.byteBlock[3 - VectorLine.endianDiff1] = bytes[i + 3];
			return BitConverter.ToSingle(VectorLine.byteBlock, 0);
		}

		// Token: 0x06001A0E RID: 6670 RVA: 0x0008C180 File Offset: 0x0008A380
		public static void Destroy(ref VectorLine line)
		{
			VectorLine.DestroyLine(ref line);
		}

		// Token: 0x06001A0F RID: 6671 RVA: 0x0008C188 File Offset: 0x0008A388
		public static void Destroy(VectorLine[] lines)
		{
			for (int i = 0; i < lines.Length; i++)
			{
				VectorLine.DestroyLine(ref lines[i]);
			}
		}

		// Token: 0x06001A10 RID: 6672 RVA: 0x0008C1B0 File Offset: 0x0008A3B0
		public static void Destroy(List<VectorLine> lines)
		{
			for (int i = 0; i < lines.Count; i++)
			{
				VectorLine vectorLine = lines[i];
				VectorLine.DestroyLine(ref vectorLine);
			}
		}

		// Token: 0x06001A11 RID: 6673 RVA: 0x0008C1DD File Offset: 0x0008A3DD
		private static void DestroyLine(ref VectorLine line)
		{
			if (line != null)
			{
				global::UnityEngine.Object.Destroy(line.m_go);
				if (line.m_vectorObject != null)
				{
					line.m_vectorObject.Destroy();
				}
				if (line.isAutoDrawing)
				{
					line.StopDrawing3DAuto();
				}
				line = null;
			}
		}

		// Token: 0x06001A12 RID: 6674 RVA: 0x0008C217 File Offset: 0x0008A417
		public static void Destroy(ref VectorLine line, GameObject go)
		{
			VectorLine.Destroy(ref line);
			if (go != null)
			{
				global::UnityEngine.Object.Destroy(go);
			}
		}

		// Token: 0x06001A13 RID: 6675 RVA: 0x0008C230 File Offset: 0x0008A430
		public void SetDistances()
		{
			if (this.m_lineType == LineType.Points)
			{
				return;
			}
			if (this.m_distances == null || this.m_distances.Length != ((this.m_lineType != LineType.Discrete) ? this.pointsCount : (this.pointsCount / 2 + 1)))
			{
				this.m_distances = new float[(this.m_lineType != LineType.Discrete) ? this.pointsCount : (this.pointsCount / 2 + 1)];
			}
			double num = 0.0;
			int num2 = this.pointsCount - 1;
			if (this.is2D)
			{
				if (this.m_lineType != LineType.Discrete)
				{
					for (int i = 0; i < num2; i++)
					{
						Vector2 vector = this.m_points2[i] - this.m_points2[i + 1];
						num += Math.Sqrt((double)(vector.x * vector.x + vector.y * vector.y));
						this.m_distances[i + 1] = (float)num;
					}
					return;
				}
				int num3 = 1;
				for (int j = 0; j < num2; j += 2)
				{
					Vector2 vector2 = this.m_points2[j] - this.m_points2[j + 1];
					num += Math.Sqrt((double)(vector2.x * vector2.x + vector2.y * vector2.y));
					this.m_distances[num3++] = (float)num;
				}
				return;
			}
			else
			{
				if (this.m_lineType != LineType.Discrete)
				{
					for (int k = 0; k < num2; k++)
					{
						Vector3 vector3 = this.m_points3[k] - this.m_points3[k + 1];
						num += Math.Sqrt((double)(vector3.x * vector3.x + vector3.y * vector3.y + vector3.z * vector3.z));
						this.m_distances[k + 1] = (float)num;
					}
					return;
				}
				int num4 = 1;
				for (int l = 0; l < num2; l += 2)
				{
					Vector3 vector4 = this.m_points3[l] - this.m_points3[l + 1];
					num += Math.Sqrt((double)(vector4.x * vector4.x + vector4.y * vector4.y + vector4.z * vector4.z));
					this.m_distances[num4++] = (float)num;
				}
				return;
			}
		}

		// Token: 0x06001A14 RID: 6676 RVA: 0x0008C490 File Offset: 0x0008A690
		public float GetLength()
		{
			if (this.m_distances == null || this.m_distances.Length != ((this.m_lineType != LineType.Discrete) ? this.pointsCount : (this.pointsCount / 2 + 1)))
			{
				this.SetDistances();
			}
			return this.m_distances[this.m_distances.Length - 1];
		}

		// Token: 0x06001A15 RID: 6677 RVA: 0x0008C4E4 File Offset: 0x0008A6E4
		public Vector2 GetPoint01(float distance)
		{
			int num;
			return this.GetPoint(Mathf.Lerp(0f, this.GetLength(), distance), out num);
		}

		// Token: 0x06001A16 RID: 6678 RVA: 0x0008C50A File Offset: 0x0008A70A
		public Vector2 GetPoint01(float distance, out int index)
		{
			return this.GetPoint(Mathf.Lerp(0f, this.GetLength(), distance), out index);
		}

		// Token: 0x06001A17 RID: 6679 RVA: 0x0008C524 File Offset: 0x0008A724
		public Vector2 GetPoint(float distance)
		{
			int num;
			return this.GetPoint(distance, out num);
		}

		// Token: 0x06001A18 RID: 6680 RVA: 0x0008C53C File Offset: 0x0008A73C
		public Vector2 GetPoint(float distance, out int index)
		{
			if (!this.m_is2D)
			{
				Debug.LogError("VectorLine.GetPoint only works with Vector2 points");
				index = 0;
				return Vector2.zero;
			}
			this.SetDistanceIndex(out index, distance);
			Vector2 vector;
			if (this.m_lineType != LineType.Discrete)
			{
				vector = Vector2.Lerp(this.m_points2[index - 1], this.m_points2[index], Mathf.InverseLerp(this.m_distances[index - 1], this.m_distances[index], distance));
			}
			else
			{
				vector = Vector2.Lerp(this.m_points2[(index - 1) * 2], this.m_points2[(index - 1) * 2 + 1], Mathf.InverseLerp(this.m_distances[index - 1], this.m_distances[index], distance));
			}
			if (this.m_drawTransform)
			{
				vector = this.m_drawTransform.localToWorldMatrix.MultiplyPoint3x4(vector);
			}
			index--;
			return vector;
		}

		// Token: 0x06001A19 RID: 6681 RVA: 0x0008C62C File Offset: 0x0008A82C
		public Vector3 GetPoint3D01(float distance)
		{
			int num;
			return this.GetPoint3D(Mathf.Lerp(0f, this.GetLength(), distance), out num);
		}

		// Token: 0x06001A1A RID: 6682 RVA: 0x0008C652 File Offset: 0x0008A852
		public Vector3 GetPoint3D01(float distance, out int index)
		{
			return this.GetPoint3D(Mathf.Lerp(0f, this.GetLength(), distance), out index);
		}

		// Token: 0x06001A1B RID: 6683 RVA: 0x0008C66C File Offset: 0x0008A86C
		public Vector3 GetPoint3D(float distance)
		{
			int num;
			return this.GetPoint3D(distance, out num);
		}

		// Token: 0x06001A1C RID: 6684 RVA: 0x0008C684 File Offset: 0x0008A884
		public Vector3 GetPoint3D(float distance, out int index)
		{
			if (this.m_is2D)
			{
				Debug.LogError("VectorLine.GetPoint3D only works with Vector3 points");
				index = 0;
				return Vector3.zero;
			}
			this.SetDistanceIndex(out index, distance);
			Vector3 vector;
			if (this.m_lineType != LineType.Discrete)
			{
				vector = Vector3.Lerp(this.m_points3[index - 1], this.m_points3[index], Mathf.InverseLerp(this.m_distances[index - 1], this.m_distances[index], distance));
			}
			else
			{
				vector = Vector3.Lerp(this.m_points3[(index - 1) * 2], this.m_points3[(index - 1) * 2 + 1], Mathf.InverseLerp(this.m_distances[index - 1], this.m_distances[index], distance));
			}
			if (this.m_drawTransform)
			{
				vector = this.m_drawTransform.localToWorldMatrix.MultiplyPoint3x4(vector);
			}
			index--;
			return vector;
		}

		// Token: 0x06001A1D RID: 6685 RVA: 0x0008C768 File Offset: 0x0008A968
		private void SetDistanceIndex(out int i, float distance)
		{
			if (this.m_distances == null)
			{
				this.SetDistances();
			}
			i = this.m_drawStart + 1;
			if (this.m_lineType == LineType.Discrete)
			{
				i = (i + 1) / 2;
			}
			if (i >= this.m_distances.Length)
			{
				i = this.m_distances.Length - 1;
			}
			int num = this.m_drawEnd;
			if (this.m_lineType == LineType.Discrete)
			{
				num = (num + 1) / 2;
			}
			while (distance > this.m_distances[i] && i < num)
			{
				i++;
			}
		}

		// Token: 0x06001A1E RID: 6686 RVA: 0x0008C7E5 File Offset: 0x0008A9E5
		public static void SetEndCap(string name, EndCap capType)
		{
			VectorLine.SetEndCap(name, capType, 0f, 0f, 1f, 1f, null);
		}

		// Token: 0x06001A1F RID: 6687 RVA: 0x0008C803 File Offset: 0x0008AA03
		public static void SetEndCap(string name, EndCap capType, params Texture2D[] textures)
		{
			VectorLine.SetEndCap(name, capType, 0f, 0f, 1f, 1f, textures);
		}

		// Token: 0x06001A20 RID: 6688 RVA: 0x0008C821 File Offset: 0x0008AA21
		public static void SetEndCap(string name, EndCap capType, float offset, params Texture2D[] textures)
		{
			VectorLine.SetEndCap(name, capType, offset, offset, 1f, 1f, textures);
		}

		// Token: 0x06001A21 RID: 6689 RVA: 0x0008C837 File Offset: 0x0008AA37
		public static void SetEndCap(string name, EndCap capType, float offsetFront, float offsetBack, params Texture2D[] textures)
		{
			VectorLine.SetEndCap(name, capType, offsetFront, offsetBack, 1f, 1f, textures);
		}

		// Token: 0x06001A22 RID: 6690 RVA: 0x0008C850 File Offset: 0x0008AA50
		public static void SetEndCap(string name, EndCap capType, float offsetFront, float offsetBack, float scaleFront, float scaleBack, params Texture2D[] textures)
		{
			if (VectorLine.capDictionary == null)
			{
				VectorLine.capDictionary = new Dictionary<string, CapInfo>();
			}
			if (name == null || name == "")
			{
				Debug.LogError("VectorLine.SetEndCap: must supply a name");
				return;
			}
			if (VectorLine.capDictionary.ContainsKey(name) && capType != EndCap.None)
			{
				Debug.LogError("VectorLine.SetEndCap: end cap \"" + name + "\" has already been set up");
				return;
			}
			if (capType == EndCap.None)
			{
				VectorLine.RemoveEndCap(name);
				return;
			}
			if ((capType == EndCap.Front || capType == EndCap.Back || capType == EndCap.Mirror) && textures.Length < 2)
			{
				Debug.LogError("VectorLine.SetEndCap (\"" + name + "\"): must supply two textures when using SetEndCap with EndCap.Front, EndCap.Back, or EndCap.Mirror");
				return;
			}
			if (textures[0] == null || textures[1] == null)
			{
				Debug.LogError("VectorLine.SetEndCap (\"" + name + "\"): end cap textures must not be null");
				return;
			}
			if (textures[0].width != textures[0].height)
			{
				Debug.LogError("VectorLine.SetEndCap (\"" + name + "\"): the line texture must be square");
				return;
			}
			if (textures[1].height != textures[0].height)
			{
				Debug.LogError("VectorLine.SetEndCap (\"" + name + "\"): all textures must be the same height");
				return;
			}
			if (capType == EndCap.Both)
			{
				if (textures.Length < 3)
				{
					Debug.LogError("VectorLine.SetEndCap (\"" + name + "\"): must supply three textures when using SetEndCap with EndCap.Both");
					return;
				}
				if (textures[2] == null)
				{
					Debug.LogError("VectorLine.SetEndCap (\"" + name + "\"): end cap textures must not be null");
					return;
				}
				if (textures[2].height != textures[0].height)
				{
					Debug.LogError("VectorLine.SetEndCap (\"" + name + "\"): all textures must be the same height");
					return;
				}
			}
			Texture2D texture2D = textures[0];
			Texture2D texture2D2 = textures[1];
			Texture2D texture2D3 = ((textures.Length == 3) ? textures[2] : null);
			int num = 4;
			int width = texture2D.width;
			float num2 = 0f;
			float num3 = 0f;
			int num4 = 0;
			int num5 = 0;
			Color32[] array = null;
			Color32[] array2 = null;
			if (capType == EndCap.Front)
			{
				array = VectorLine.GetRotatedPixels(texture2D2);
				num4 = texture2D2.width;
				array2 = VectorLine.GetRowPixels(array, num, 0, width);
				num5 = num;
				num2 = (float)texture2D2.width / (float)texture2D2.height;
			}
			else if (capType == EndCap.Back)
			{
				array2 = VectorLine.GetRotatedPixels(texture2D2);
				num5 = texture2D2.width;
				array = VectorLine.GetRowPixels(array2, num, num5 - 1, width);
				num4 = num;
				num3 = (float)texture2D2.width / (float)texture2D2.height;
			}
			else if (capType == EndCap.Both)
			{
				array = VectorLine.GetRotatedPixels(texture2D2);
				num4 = texture2D2.width;
				array2 = VectorLine.GetRotatedPixels(texture2D3);
				num5 = texture2D3.width;
				num2 = (float)texture2D2.width / (float)texture2D2.height;
				num3 = (float)texture2D3.width / (float)texture2D3.height;
			}
			else if (capType == EndCap.Mirror)
			{
				array = VectorLine.GetRotatedPixels(texture2D2);
				num4 = texture2D2.width;
				array2 = VectorLine.GetRowPixels(array, num, 0, width);
				num5 = num;
				num2 = (float)texture2D2.width / (float)texture2D2.height;
				num3 = num2;
			}
			int num6 = texture2D.height + num4 + num5 + num * 4;
			Color32[] pixels = texture2D.GetPixels32();
			Color32[] array3 = new Color32[num * width];
			Color32 color = Color.clear;
			for (int i = 0; i < num * width; i++)
			{
				array3[i] = color;
			}
			Color32[] rowPixels = VectorLine.GetRowPixels(array2, num, num5 - 1, width);
			Color32[] rowPixels2 = VectorLine.GetRowPixels(array, num, 0, width);
			bool flag = texture2D.mipmapCount > 1;
			Texture2D texture2D4 = new Texture2D(width, num6, TextureFormat.ARGB32, flag);
			texture2D4.name = texture2D.name + " end cap";
			texture2D4.wrapMode = texture2D.wrapMode;
			texture2D4.filterMode = texture2D.filterMode;
			float num7 = 1f / (float)num6;
			float[] array4 = new float[6];
			int num8 = 0;
			texture2D4.SetPixels32(0, 0, width, num, array3);
			num8 += num;
			array4[0] = num7 * (float)num8;
			texture2D4.SetPixels32(0, num8, width, texture2D.height, pixels);
			num8 += texture2D.height;
			array4[1] = num7 * (float)num8;
			texture2D4.SetPixels32(0, num8, width, num, array3);
			num8 += num;
			array4[2] = num7 * (float)num8;
			texture2D4.SetPixels32(0, num8, width, num5, array2);
			num8 += num5;
			array4[3] = num7 * (float)num8;
			texture2D4.SetPixels32(0, num8, width, num, rowPixels);
			num8 += num;
			texture2D4.SetPixels32(0, num8, width, num, rowPixels2);
			num8 += num;
			array4[4] = num7 * (float)num8;
			texture2D4.SetPixels32(0, num8, width, num4, array);
			array4[5] = num7 * (float)(num8 + num4);
			texture2D4.Apply(flag, true);
			VectorLine.capDictionary.Add(name, new CapInfo(capType, texture2D4, num2, num3, offsetFront, offsetBack, scaleFront, scaleBack, array4));
		}

		// Token: 0x06001A23 RID: 6691 RVA: 0x0008CCD4 File Offset: 0x0008AED4
		private static Color32[] GetRowPixels(Color32[] texPixels, int numberOfRows, int row, int w)
		{
			Color32[] array = new Color32[w * numberOfRows];
			for (int i = 0; i < numberOfRows; i++)
			{
				Array.Copy(texPixels, row * w, array, i * w, w);
			}
			return array;
		}

		// Token: 0x06001A24 RID: 6692 RVA: 0x0008CD08 File Offset: 0x0008AF08
		private static Color32[] GetRotatedPixels(Texture2D tex)
		{
			Color32[] pixels = tex.GetPixels32();
			Color32[] array = new Color32[pixels.Length];
			int width = tex.width;
			int height = tex.height;
			int num = 0;
			for (int i = 0; i < height; i++)
			{
				int num2 = tex.width - 1;
				for (int j = 0; j < width; j++)
				{
					array[num2 * height + num] = pixels[i * width + j];
					num2--;
				}
				num++;
			}
			return array;
		}

		// Token: 0x06001A25 RID: 6693 RVA: 0x0008CD88 File Offset: 0x0008AF88
		public static void RemoveEndCap(string name)
		{
			if (!VectorLine.capDictionary.ContainsKey(name))
			{
				Debug.LogError("VectorLine: RemoveEndCap: \"" + name + "\" has not been set up");
				return;
			}
			global::UnityEngine.Object.Destroy(VectorLine.capDictionary[name].texture);
			VectorLine.capDictionary.Remove(name);
		}

		// Token: 0x06001A26 RID: 6694 RVA: 0x0008CDDC File Offset: 0x0008AFDC
		public bool Selected(Vector2 p)
		{
			int num;
			return this.Selected(p, 0, 0, out num, VectorLine.cam3D);
		}

		// Token: 0x06001A27 RID: 6695 RVA: 0x0008CDF9 File Offset: 0x0008AFF9
		public bool Selected(Vector2 p, out int index)
		{
			return this.Selected(p, 0, 0, out index, VectorLine.cam3D);
		}

		// Token: 0x06001A28 RID: 6696 RVA: 0x0008CE0A File Offset: 0x0008B00A
		public bool Selected(Vector2 p, int extraDistance, out int index)
		{
			return this.Selected(p, extraDistance, 0, out index, VectorLine.cam3D);
		}

		// Token: 0x06001A29 RID: 6697 RVA: 0x0008CE1B File Offset: 0x0008B01B
		public bool Selected(Vector2 p, int extraDistance, int extraLength, out int index)
		{
			return this.Selected(p, extraDistance, extraLength, out index, VectorLine.cam3D);
		}

		// Token: 0x06001A2A RID: 6698 RVA: 0x0008CE30 File Offset: 0x0008B030
		public bool Selected(Vector2 p, Camera cam)
		{
			int num;
			return this.Selected(p, 0, 0, out num, cam);
		}

		// Token: 0x06001A2B RID: 6699 RVA: 0x0008CE49 File Offset: 0x0008B049
		public bool Selected(Vector2 p, out int index, Camera cam)
		{
			return this.Selected(p, 0, 0, out index, cam);
		}

		// Token: 0x06001A2C RID: 6700 RVA: 0x0008CE56 File Offset: 0x0008B056
		public bool Selected(Vector2 p, int extraDistance, out int index, Camera cam)
		{
			return this.Selected(p, extraDistance, 0, out index, cam);
		}

		// Token: 0x06001A2D RID: 6701 RVA: 0x0008CE64 File Offset: 0x0008B064
		public bool Selected(Vector2 p, int extraDistance, int extraLength, out int index, Camera cam)
		{
			if (cam == null)
			{
				VectorLine.SetCamera3D();
				if (!VectorLine.cam3D)
				{
					Debug.LogError("VectorLine.Selected: camera cannot be null. If there is no camera tagged \"MainCamera\", supply one manually");
					index = 0;
					return false;
				}
				cam = VectorLine.cam3D;
			}
			int num = ((this.m_lineWidths.Length == 1) ? 0 : 1);
			int num2 = ((this.m_lineType != LineType.Discrete) ? (this.m_drawStart - num) : (this.m_drawStart / 2 - num));
			if (this.m_lineWidths.Length == 1)
			{
				num = 0;
				num2 = 0;
			}
			else
			{
				num = 1;
			}
			int num3 = this.m_drawEnd;
			bool flag = this.m_drawTransform != null;
			Matrix4x4 matrix4x = (flag ? this.m_drawTransform.localToWorldMatrix : Matrix4x4.identity);
			Vector2 vector = new Vector2((float)Screen.width, (float)Screen.height);
			if (this.m_lineType == LineType.Points)
			{
				if (num3 == this.pointsCount)
				{
					num3--;
				}
				if (this.m_is2D)
				{
					for (int i = this.m_drawStart; i <= num3; i++)
					{
						num2 += num;
						float num4 = this.m_lineWidths[num2] + (float)extraDistance;
						Vector2 vector2 = (flag ? matrix4x.MultiplyPoint3x4(this.m_points2[i]) : this.m_points2[i]);
						if (this.m_viewportDraw)
						{
							vector2.x *= vector.x;
							vector2.y *= vector.y;
						}
						if (p.x >= vector2.x - num4 && p.x <= vector2.x + num4 && p.y >= vector2.y - num4 && p.y <= vector2.y + num4)
						{
							index = i;
							return true;
						}
					}
					index = -1;
					return false;
				}
				for (int j = this.m_drawStart; j <= num3; j++)
				{
					num2 += num;
					float num5 = this.m_lineWidths[num2] + (float)extraDistance;
					Vector2 vector2 = (flag ? cam.WorldToScreenPoint(matrix4x.MultiplyPoint3x4(this.m_points3[j])) : cam.WorldToScreenPoint(this.m_points3[j]));
					if (p.x >= vector2.x - num5 && p.x <= vector2.x + num5 && p.y >= vector2.y - num5 && p.y <= vector2.y + num5)
					{
						index = j;
						return true;
					}
				}
				index = -1;
				return false;
			}
			else
			{
				int num6 = ((this.m_lineType != LineType.Discrete) ? 1 : 2);
				Vector2 vector3 = Vector2.zero;
				if (this.m_lineType != LineType.Discrete && this.m_drawEnd == this.pointsCount)
				{
					num3--;
				}
				Vector2 vector4;
				Vector2 vector5;
				if (this.m_is2D)
				{
					for (int k = this.m_drawStart; k < num3; k += num6)
					{
						num2 += num;
						if (flag)
						{
							vector4 = matrix4x.MultiplyPoint3x4(this.m_points2[k]);
							vector5 = matrix4x.MultiplyPoint3x4(this.m_points2[k + 1]);
						}
						else
						{
							vector4.x = this.m_points2[k].x;
							vector4.y = this.m_points2[k].y;
							vector5.x = this.m_points2[k + 1].x;
							vector5.y = this.m_points2[k + 1].y;
						}
						if (this.m_viewportDraw)
						{
							vector4.x *= vector.x;
							vector4.y *= vector.y;
							vector5.x *= vector.x;
							vector5.y *= vector.y;
						}
						if (extraLength > 0)
						{
							vector3 = (vector4 - vector5).normalized * (float)extraLength;
							vector4.x += vector3.x;
							vector4.y += vector3.y;
							vector5.x -= vector3.x;
							vector5.y -= vector3.y;
						}
						float num7 = Vector2.Dot(p - vector4, vector5 - vector4) / (vector5 - vector4).sqrMagnitude;
						if (num7 >= 0f && num7 <= 1f && (p - (vector4 + num7 * (vector5 - vector4))).sqrMagnitude <= (this.m_lineWidths[num2] + (float)extraDistance) * (this.m_lineWidths[num2] + (float)extraDistance))
						{
							index = ((this.m_lineType != LineType.Discrete) ? k : (k / 2));
							return true;
						}
					}
					index = -1;
					return false;
				}
				Vector3 vector6 = VectorLine.v3zero;
				for (int l = this.m_drawStart; l < num3; l += num6)
				{
					num2 += num;
					Vector3 vector7;
					if (flag)
					{
						vector7 = cam.WorldToScreenPoint(matrix4x.MultiplyPoint3x4(this.m_points3[l]));
						vector6 = cam.WorldToScreenPoint(matrix4x.MultiplyPoint3x4(this.m_points3[l + 1]));
					}
					else
					{
						vector7 = cam.WorldToScreenPoint(this.m_points3[l]);
						vector6 = cam.WorldToScreenPoint(this.m_points3[l + 1]);
					}
					if (vector7.z >= 0f && vector6.z >= 0f)
					{
						vector4.x = (float)((int)vector7.x);
						vector5.x = (float)((int)vector6.x);
						vector4.y = (float)((int)vector7.y);
						vector5.y = (float)((int)vector6.y);
						if (vector4.x != vector5.x || vector4.y != vector5.y)
						{
							if (extraLength > 0)
							{
								vector3 = (vector4 - vector5).normalized * (float)extraLength;
								vector4.x += vector3.x;
								vector4.y += vector3.y;
								vector5.x -= vector3.x;
								vector5.y -= vector3.y;
							}
							float num7 = Vector2.Dot(p - vector4, vector5 - vector4) / (vector5 - vector4).sqrMagnitude;
							if (num7 >= 0f && num7 <= 1f && (p - (vector4 + num7 * (vector5 - vector4))).sqrMagnitude <= (this.m_lineWidths[num2] + (float)extraDistance) * (this.m_lineWidths[num2] + (float)extraDistance))
							{
								index = ((this.m_lineType != LineType.Discrete) ? l : (l / 2));
								return true;
							}
						}
					}
				}
				index = -1;
				return false;
			}
		}

		// Token: 0x06001A2E RID: 6702 RVA: 0x0008D55B File Offset: 0x0008B75B
		private bool Approximately(Vector2 p1, Vector2 p2)
		{
			return this.Approximately(p1.x, p2.x) && this.Approximately(p1.y, p2.y);
		}

		// Token: 0x06001A2F RID: 6703 RVA: 0x0008D585 File Offset: 0x0008B785
		private bool Approximately(Vector3 p1, Vector3 p2)
		{
			return this.Approximately(p1.x, p2.x) && this.Approximately(p1.y, p2.y) && this.Approximately(p1.z, p2.z);
		}

		// Token: 0x06001A30 RID: 6704 RVA: 0x0008D5C3 File Offset: 0x0008B7C3
		private bool Approximately(float a, float b)
		{
			return Mathf.Round(a * 100f) / 100f == Mathf.Round(b * 100f) / 100f;
		}

		// Token: 0x06001A31 RID: 6705 RVA: 0x0008D5EC File Offset: 0x0008B7EC
		private bool WrongArrayLength(int arrayLength, VectorLine.FunctionName functionName)
		{
			if (this.m_lineType == LineType.Continuous)
			{
				if (arrayLength != this.pointsCount - 1)
				{
					Debug.LogError(string.Concat(new string[]
					{
						VectorLine.functionNames[(int)functionName],
						" list for \"",
						this.name,
						"\" must be length of points array minus one for a continuous line (one entry per line segment). Expected ",
						(this.pointsCount - 1).ToString(),
						", got ",
						arrayLength.ToString()
					}));
					return true;
				}
			}
			else if (arrayLength != this.pointsCount / 2)
			{
				Debug.LogError(string.Concat(new string[]
				{
					VectorLine.functionNames[(int)functionName],
					" list in \"",
					this.name,
					"\" must be exactly half the length of points array for a discrete line (one entry per line segment). Expected ",
					(this.pointsCount / 2).ToString(),
					", got ",
					arrayLength.ToString()
				}));
				return true;
			}
			return false;
		}

		// Token: 0x06001A32 RID: 6706 RVA: 0x0008D6D0 File Offset: 0x0008B8D0
		private bool CheckArrayLength(VectorLine.FunctionName functionName, int segments, int index)
		{
			if (segments < 1)
			{
				Debug.LogError("VectorLine." + VectorLine.functionNames[(int)functionName] + " needs at least 1 segment");
				return false;
			}
			if (index < 0)
			{
				Debug.LogError(string.Concat(new string[]
				{
					"VectorLine.",
					VectorLine.functionNames[(int)functionName],
					": The index value for \"",
					this.name,
					"\" must be >= 0"
				}));
				return false;
			}
			if (this.m_lineType != LineType.Points)
			{
				if (this.m_lineType == LineType.Continuous)
				{
					if (index + (segments + 1) > this.pointsCount)
					{
						if (index == 0)
						{
							Debug.LogError(string.Concat(new string[]
							{
								"VectorLine.",
								VectorLine.functionNames[(int)functionName],
								": The length of the array for continuous lines needs to be at least the number of segments plus one for \"",
								this.name,
								"\""
							}));
							return false;
						}
						Debug.LogError(string.Concat(new string[]
						{
							"VectorLine: Calling ",
							VectorLine.functionNames[(int)functionName],
							" with an index of ",
							index.ToString(),
							" would exceed the length of the Vector array (",
							this.pointsCount.ToString(),
							") for \"",
							this.name,
							"\""
						}));
						return false;
					}
				}
				else if (index + segments * 2 > this.pointsCount)
				{
					if (index == 0)
					{
						Debug.LogError(string.Concat(new string[]
						{
							"VectorLine.",
							VectorLine.functionNames[(int)functionName],
							": The length of the array for discrete lines needs to be at least twice the number of segments for \"",
							this.name,
							"\""
						}));
						return false;
					}
					Debug.LogError(string.Concat(new string[]
					{
						"VectorLine: Calling ",
						VectorLine.functionNames[(int)functionName],
						" with an index of ",
						index.ToString(),
						" would exceed the length of the Vector array (",
						this.pointsCount.ToString(),
						") for \"",
						this.name,
						"\""
					}));
					return false;
				}
				return true;
			}
			if (index + segments <= this.pointsCount)
			{
				return true;
			}
			if (index == 0)
			{
				Debug.LogError(string.Concat(new string[]
				{
					"VectorLine.",
					VectorLine.functionNames[(int)functionName],
					": The number of segments cannot exceed the number of points in the array for \"",
					this.name,
					"\""
				}));
				return false;
			}
			Debug.LogError(string.Concat(new string[]
			{
				"VectorLine: Calling ",
				VectorLine.functionNames[(int)functionName],
				" with an index of ",
				index.ToString(),
				" would exceed the length of the Vector array for \"",
				this.name,
				"\""
			}));
			return false;
		}

		// Token: 0x06001A33 RID: 6707 RVA: 0x0008D968 File Offset: 0x0008BB68
		public void MakeRect(Rect rect)
		{
			this.MakeRect(new Vector2(rect.x, rect.y), new Vector2(rect.x + rect.width, rect.y + rect.height), 0);
		}

		// Token: 0x06001A34 RID: 6708 RVA: 0x0008D9BC File Offset: 0x0008BBBC
		public void MakeRect(Rect rect, int index)
		{
			this.MakeRect(new Vector2(rect.x, rect.y), new Vector2(rect.x + rect.width, rect.y + rect.height), index);
		}

		// Token: 0x06001A35 RID: 6709 RVA: 0x0008DA10 File Offset: 0x0008BC10
		public void MakeRect(Vector3 bottomLeft, Vector3 topRight)
		{
			this.MakeRect(bottomLeft, topRight, 0);
		}

		// Token: 0x06001A36 RID: 6710 RVA: 0x0008DA1C File Offset: 0x0008BC1C
		public void MakeRect(Vector3 bottomLeft, Vector3 topRight, int index)
		{
			if (this.m_lineType != LineType.Discrete)
			{
				if (index + 5 > this.pointsCount)
				{
					if (index == 0)
					{
						Debug.LogError("VectorLine.MakeRect: The length of the array for continuous lines needs to be at least 5 for \"" + this.name + "\"");
						return;
					}
					Debug.LogError(string.Concat(new string[]
					{
						"Calling VectorLine.MakeRect with an index of ",
						index.ToString(),
						" would exceed the length of the Vector2 array for \"",
						this.name,
						"\""
					}));
					return;
				}
				else
				{
					if (this.m_is2D)
					{
						this.m_points2[index] = new Vector2(bottomLeft.x, bottomLeft.y);
						this.m_points2[index + 1] = new Vector2(topRight.x, bottomLeft.y);
						this.m_points2[index + 2] = new Vector2(topRight.x, topRight.y);
						this.m_points2[index + 3] = new Vector2(bottomLeft.x, topRight.y);
						this.m_points2[index + 4] = new Vector2(bottomLeft.x, bottomLeft.y);
						return;
					}
					this.m_points3[index] = new Vector3(bottomLeft.x, bottomLeft.y, bottomLeft.z);
					this.m_points3[index + 1] = new Vector3(topRight.x, bottomLeft.y, bottomLeft.z);
					this.m_points3[index + 2] = new Vector3(topRight.x, topRight.y, topRight.z);
					this.m_points3[index + 3] = new Vector3(bottomLeft.x, topRight.y, topRight.z);
					this.m_points3[index + 4] = new Vector3(bottomLeft.x, bottomLeft.y, bottomLeft.z);
					return;
				}
			}
			else if (index + 8 > this.pointsCount)
			{
				if (index == 0)
				{
					Debug.LogError("VectorLine.MakeRect: The length of the array for discrete lines needs to be at least 8 for \"" + this.name + "\"");
					return;
				}
				Debug.LogError(string.Concat(new string[]
				{
					"Calling VectorLine.MakeRect with an index of ",
					index.ToString(),
					" would exceed the length of the Vector2 array for \"",
					this.name,
					"\""
				}));
				return;
			}
			else
			{
				if (this.m_is2D)
				{
					this.m_points2[index] = new Vector2(bottomLeft.x, bottomLeft.y);
					this.m_points2[index + 1] = new Vector2(topRight.x, bottomLeft.y);
					this.m_points2[index + 2] = new Vector2(topRight.x, bottomLeft.y);
					this.m_points2[index + 3] = new Vector2(topRight.x, topRight.y);
					this.m_points2[index + 4] = new Vector2(topRight.x, topRight.y);
					this.m_points2[index + 5] = new Vector2(bottomLeft.x, topRight.y);
					this.m_points2[index + 6] = new Vector2(bottomLeft.x, topRight.y);
					this.m_points2[index + 7] = new Vector2(bottomLeft.x, bottomLeft.y);
					return;
				}
				this.m_points3[index] = new Vector3(bottomLeft.x, bottomLeft.y, bottomLeft.z);
				this.m_points3[index + 1] = new Vector3(topRight.x, bottomLeft.y, bottomLeft.z);
				this.m_points3[index + 2] = new Vector3(topRight.x, bottomLeft.y, bottomLeft.z);
				this.m_points3[index + 3] = new Vector3(topRight.x, topRight.y, topRight.z);
				this.m_points3[index + 4] = new Vector3(topRight.x, topRight.y, topRight.z);
				this.m_points3[index + 5] = new Vector3(bottomLeft.x, topRight.y, topRight.z);
				this.m_points3[index + 6] = new Vector3(bottomLeft.x, topRight.y, topRight.z);
				this.m_points3[index + 7] = new Vector3(bottomLeft.x, bottomLeft.y, bottomLeft.z);
				return;
			}
		}

		// Token: 0x06001A37 RID: 6711 RVA: 0x0008DE84 File Offset: 0x0008C084
		public void MakeRoundedRect(Rect rect, float cornerRadius, int cornerSegments)
		{
			this.MakeRoundedRect(new Vector2(rect.x, rect.y), new Vector2(rect.x + rect.width, rect.y + rect.height), cornerRadius, cornerSegments, 0);
		}

		// Token: 0x06001A38 RID: 6712 RVA: 0x0008DEDC File Offset: 0x0008C0DC
		public void MakeRoundedRect(Rect rect, float cornerRadius, int cornerSegments, int index)
		{
			this.MakeRoundedRect(new Vector2(rect.x, rect.y), new Vector2(rect.x + rect.width, rect.y + rect.height), cornerRadius, cornerSegments, index);
		}

		// Token: 0x06001A39 RID: 6713 RVA: 0x0008DF33 File Offset: 0x0008C133
		public void MakeRoundedRect(Vector3 bottomLeft, Vector3 topRight, float cornerRadius, int cornerSegments)
		{
			this.MakeRoundedRect(bottomLeft, topRight, cornerRadius, cornerSegments, 0);
		}

		// Token: 0x06001A3A RID: 6714 RVA: 0x0008DF44 File Offset: 0x0008C144
		public void MakeRoundedRect(Vector3 bottomLeft, Vector3 topRight, float cornerRadius, int cornerSegments, int index)
		{
			if (cornerSegments < 1)
			{
				Debug.LogError("VectorLine.MakeRoundedRect: cornerSegments value must be >= 1");
				return;
			}
			if (index < 0)
			{
				Debug.LogError("VectorLine.MakeRoundedRect: index value must be >= 0");
				return;
			}
			if (!this.m_is2D && bottomLeft.z != topRight.z)
			{
				Debug.LogError("VectorLine.MakeRoundedRect only works on the X/Y plane");
				return;
			}
			int num = ((this.m_lineType != LineType.Discrete) ? (cornerSegments * 4 + 5 + index) : (cornerSegments * 8 + 8 + index));
			if (this.pointsCount < num)
			{
				this.Resize(num);
			}
			if (bottomLeft.x > topRight.x)
			{
				this.Exchange(ref bottomLeft, ref topRight, 0);
			}
			if (bottomLeft.y > topRight.y)
			{
				this.Exchange(ref bottomLeft, ref topRight, 1);
			}
			bottomLeft += new Vector3(cornerRadius, cornerRadius);
			topRight -= new Vector3(cornerRadius, cornerRadius);
			this.MakeCircle(bottomLeft, cornerRadius, 4 * cornerSegments, index);
			int num2 = ((this.m_lineType != LineType.Discrete) ? (cornerSegments + 1) : (cornerSegments * 2));
			int num3 = ((this.m_lineType != LineType.Discrete) ? cornerSegments : (cornerSegments * 2));
			if (this.m_is2D)
			{
				this.CopyAndAddPoints(num2, num3, 3, new Vector2(0f, topRight.y - bottomLeft.y), index);
				this.CopyAndAddPoints(num2, num3, 2, Vector2.zero, index);
				this.CopyAndAddPoints(num2, num3, 1, new Vector2(topRight.x - bottomLeft.x, 0f), index);
				this.CopyAndAddPoints(num2, num3, 0, new Vector2(topRight.x - bottomLeft.x, topRight.y - bottomLeft.y), index);
				if (this.m_lineType != LineType.Discrete)
				{
					this.m_points2[num2 * 4 + index] = this.m_points2[index];
					return;
				}
				this.m_points2[num2 * 4 + 7 + index] = this.m_points2[index];
				this.m_points2[num2 * 3 + 5 + index] = this.m_points2[num2 * 3 + 6 + index];
				this.m_points2[num2 * 2 + 3 + index] = this.m_points2[num2 * 2 + 4 + index];
				this.m_points2[num2 + 1 + index] = this.m_points2[num2 + 2 + index];
				return;
			}
			else
			{
				this.CopyAndAddPoints(num2, num3, 3, Vector2.zero, index);
				this.CopyAndAddPoints(num2, num3, 2, new Vector2(0f, topRight.y - bottomLeft.y), index);
				this.CopyAndAddPoints(num2, num3, 1, new Vector2(topRight.x - bottomLeft.x, topRight.y - bottomLeft.y), index);
				this.CopyAndAddPoints(num2, num3, 0, new Vector2(topRight.x - bottomLeft.x, 0f), index);
				if (this.m_lineType != LineType.Discrete)
				{
					this.m_points3[num2 * 4 + index] = this.m_points3[index];
					return;
				}
				this.m_points3[num2 * 4 + 7 + index] = this.m_points3[index];
				this.m_points3[num2 * 3 + 5 + index] = this.m_points3[num2 * 3 + 6 + index];
				this.m_points3[num2 * 2 + 3 + index] = this.m_points3[num2 * 2 + 4 + index];
				this.m_points3[num2 + 1 + index] = this.m_points3[num2 + 2 + index];
				return;
			}
		}

		// Token: 0x06001A3B RID: 6715 RVA: 0x0008E2B8 File Offset: 0x0008C4B8
		private void CopyAndAddPoints(int cornerPointCount, int originalCount, int sectionNumber, Vector2 add, int index)
		{
			Vector3 vector = add;
			for (int i = cornerPointCount - 1; i >= 0; i--)
			{
				if (this.m_lineType != LineType.Discrete)
				{
					if (this.m_is2D)
					{
						this.m_points2[cornerPointCount * sectionNumber + i + index] = this.m_points2[originalCount * sectionNumber + i + index] + add;
					}
					else
					{
						this.m_points3[cornerPointCount * sectionNumber + i + index] = this.m_points3[originalCount * sectionNumber + i + index] + vector;
					}
				}
				else if (this.m_is2D)
				{
					this.m_points2[cornerPointCount * sectionNumber + sectionNumber * 2 + i + index] = this.m_points2[originalCount * sectionNumber + i + index] + add;
				}
				else
				{
					this.m_points3[cornerPointCount * sectionNumber + sectionNumber * 2 + i + index] = this.m_points3[originalCount * sectionNumber + i + index] + vector;
				}
			}
			if (this.m_lineType == LineType.Discrete)
			{
				int num = cornerPointCount * (sectionNumber + 1) + sectionNumber * 2 + index;
				if (this.m_is2D)
				{
					this.m_points2[num] = this.m_points2[num - 1];
					return;
				}
				this.m_points3[num] = this.m_points3[num - 1];
			}
		}

		// Token: 0x06001A3C RID: 6716 RVA: 0x0008E410 File Offset: 0x0008C610
		private void Exchange(ref Vector3 v1, ref Vector3 v2, int i)
		{
			float num = v1[i];
			v1[i] = v2[i];
			v2[i] = num;
		}

		// Token: 0x06001A3D RID: 6717 RVA: 0x0008E43C File Offset: 0x0008C63C
		public void MakeCircle(Vector3 origin, float radius)
		{
			this.MakeEllipse(origin, Vector3.forward, radius, radius, 0f, 0f, this.GetSegmentNumber(), 0f, 0);
		}

		// Token: 0x06001A3E RID: 6718 RVA: 0x0008E470 File Offset: 0x0008C670
		public void MakeCircle(Vector3 origin, float radius, int segments)
		{
			this.MakeEllipse(origin, Vector3.forward, radius, radius, 0f, 0f, segments, 0f, 0);
		}

		// Token: 0x06001A3F RID: 6719 RVA: 0x0008E49C File Offset: 0x0008C69C
		public void MakeCircle(Vector3 origin, float radius, int segments, float pointRotation)
		{
			this.MakeEllipse(origin, Vector3.forward, radius, radius, 0f, 0f, segments, pointRotation, 0);
		}

		// Token: 0x06001A40 RID: 6720 RVA: 0x0008E4C8 File Offset: 0x0008C6C8
		public void MakeCircle(Vector3 origin, float radius, int segments, int index)
		{
			this.MakeEllipse(origin, Vector3.forward, radius, radius, 0f, 0f, segments, 0f, index);
		}

		// Token: 0x06001A41 RID: 6721 RVA: 0x0008E4F8 File Offset: 0x0008C6F8
		public void MakeCircle(Vector3 origin, float radius, int segments, float pointRotation, int index)
		{
			this.MakeEllipse(origin, Vector3.forward, radius, radius, 0f, 0f, segments, pointRotation, index);
		}

		// Token: 0x06001A42 RID: 6722 RVA: 0x0008E524 File Offset: 0x0008C724
		public void MakeCircle(Vector3 origin, Vector3 upVector, float radius)
		{
			this.MakeEllipse(origin, upVector, radius, radius, 0f, 0f, this.GetSegmentNumber(), 0f, 0);
		}

		// Token: 0x06001A43 RID: 6723 RVA: 0x0008E554 File Offset: 0x0008C754
		public void MakeCircle(Vector3 origin, Vector3 upVector, float radius, int segments)
		{
			this.MakeEllipse(origin, upVector, radius, radius, 0f, 0f, segments, 0f, 0);
		}

		// Token: 0x06001A44 RID: 6724 RVA: 0x0008E580 File Offset: 0x0008C780
		public void MakeCircle(Vector3 origin, Vector3 upVector, float radius, int segments, float pointRotation)
		{
			this.MakeEllipse(origin, upVector, radius, radius, 0f, 0f, segments, pointRotation, 0);
		}

		// Token: 0x06001A45 RID: 6725 RVA: 0x0008E5A8 File Offset: 0x0008C7A8
		public void MakeCircle(Vector3 origin, Vector3 upVector, float radius, int segments, int index)
		{
			this.MakeEllipse(origin, upVector, radius, radius, 0f, 0f, segments, 0f, index);
		}

		// Token: 0x06001A46 RID: 6726 RVA: 0x0008E5D4 File Offset: 0x0008C7D4
		public void MakeCircle(Vector3 origin, Vector3 upVector, float radius, int segments, float pointRotation, int index)
		{
			this.MakeEllipse(origin, upVector, radius, radius, 0f, 0f, segments, pointRotation, index);
		}

		// Token: 0x06001A47 RID: 6727 RVA: 0x0008E5FC File Offset: 0x0008C7FC
		public void MakeEllipse(Vector3 origin, float xRadius, float yRadius)
		{
			this.MakeEllipse(origin, Vector3.forward, xRadius, yRadius, 0f, 0f, this.GetSegmentNumber(), 0f, 0);
		}

		// Token: 0x06001A48 RID: 6728 RVA: 0x0008E630 File Offset: 0x0008C830
		public void MakeEllipse(Vector3 origin, float xRadius, float yRadius, int segments)
		{
			this.MakeEllipse(origin, Vector3.forward, xRadius, yRadius, 0f, 0f, segments, 0f, 0);
		}

		// Token: 0x06001A49 RID: 6729 RVA: 0x0008E660 File Offset: 0x0008C860
		public void MakeEllipse(Vector3 origin, float xRadius, float yRadius, int segments, int index)
		{
			this.MakeEllipse(origin, Vector3.forward, xRadius, yRadius, 0f, 0f, segments, 0f, index);
		}

		// Token: 0x06001A4A RID: 6730 RVA: 0x0008E690 File Offset: 0x0008C890
		public void MakeEllipse(Vector3 origin, float xRadius, float yRadius, int segments, float pointRotation)
		{
			this.MakeEllipse(origin, Vector3.forward, xRadius, yRadius, 0f, 0f, segments, pointRotation, 0);
		}

		// Token: 0x06001A4B RID: 6731 RVA: 0x0008E6BC File Offset: 0x0008C8BC
		public void MakeEllipse(Vector3 origin, Vector3 upVector, float xRadius, float yRadius)
		{
			this.MakeEllipse(origin, upVector, xRadius, yRadius, 0f, 0f, this.GetSegmentNumber(), 0f, 0);
		}

		// Token: 0x06001A4C RID: 6732 RVA: 0x0008E6EC File Offset: 0x0008C8EC
		public void MakeEllipse(Vector3 origin, Vector3 upVector, float xRadius, float yRadius, int segments)
		{
			this.MakeEllipse(origin, upVector, xRadius, yRadius, 0f, 0f, segments, 0f, 0);
		}

		// Token: 0x06001A4D RID: 6733 RVA: 0x0008E718 File Offset: 0x0008C918
		public void MakeEllipse(Vector3 origin, Vector3 upVector, float xRadius, float yRadius, int segments, int index)
		{
			this.MakeEllipse(origin, upVector, xRadius, yRadius, 0f, 0f, segments, 0f, index);
		}

		// Token: 0x06001A4E RID: 6734 RVA: 0x0008E744 File Offset: 0x0008C944
		public void MakeEllipse(Vector3 origin, Vector3 upVector, float xRadius, float yRadius, int segments, float pointRotation)
		{
			this.MakeEllipse(origin, upVector, xRadius, yRadius, 0f, 0f, segments, pointRotation, 0);
		}

		// Token: 0x06001A4F RID: 6735 RVA: 0x0008E76C File Offset: 0x0008C96C
		public void MakeEllipse(Vector3 origin, Vector3 upVector, float xRadius, float yRadius, int segments, float pointRotation, int index)
		{
			this.MakeEllipse(origin, upVector, xRadius, yRadius, 0f, 0f, segments, pointRotation, index);
		}

		// Token: 0x06001A50 RID: 6736 RVA: 0x0008E794 File Offset: 0x0008C994
		public void MakeArc(Vector3 origin, float xRadius, float yRadius, float startDegrees, float endDegrees)
		{
			this.MakeEllipse(origin, Vector3.forward, xRadius, yRadius, startDegrees, endDegrees, this.GetSegmentNumber(), 0f, 0);
		}

		// Token: 0x06001A51 RID: 6737 RVA: 0x0008E7C0 File Offset: 0x0008C9C0
		public void MakeArc(Vector3 origin, float xRadius, float yRadius, float startDegrees, float endDegrees, int segments)
		{
			this.MakeEllipse(origin, Vector3.forward, xRadius, yRadius, startDegrees, endDegrees, segments, 0f, 0);
		}

		// Token: 0x06001A52 RID: 6738 RVA: 0x0008E7E8 File Offset: 0x0008C9E8
		public void MakeArc(Vector3 origin, float xRadius, float yRadius, float startDegrees, float endDegrees, int segments, int index)
		{
			this.MakeEllipse(origin, Vector3.forward, xRadius, yRadius, startDegrees, endDegrees, segments, 0f, index);
		}

		// Token: 0x06001A53 RID: 6739 RVA: 0x0008E810 File Offset: 0x0008CA10
		public void MakeArc(Vector3 origin, Vector3 upVector, float xRadius, float yRadius, float startDegrees, float endDegrees)
		{
			this.MakeEllipse(origin, upVector, xRadius, yRadius, startDegrees, endDegrees, this.GetSegmentNumber(), 0f, 0);
		}

		// Token: 0x06001A54 RID: 6740 RVA: 0x0008E838 File Offset: 0x0008CA38
		public void MakeArc(Vector3 origin, Vector3 upVector, float xRadius, float yRadius, float startDegrees, float endDegrees, int segments)
		{
			this.MakeEllipse(origin, upVector, xRadius, yRadius, startDegrees, endDegrees, segments, 0f, 0);
		}

		// Token: 0x06001A55 RID: 6741 RVA: 0x0008E85C File Offset: 0x0008CA5C
		public void MakeArc(Vector3 origin, Vector3 upVector, float xRadius, float yRadius, float startDegrees, float endDegrees, int segments, int index)
		{
			this.MakeEllipse(origin, upVector, xRadius, yRadius, startDegrees, endDegrees, segments, 0f, index);
		}

		// Token: 0x06001A56 RID: 6742 RVA: 0x0008E884 File Offset: 0x0008CA84
		private void MakeEllipse(Vector3 origin, Vector3 upVector, float xRadius, float yRadius, float startDegrees, float endDegrees, int segments, float pointRotation, int index)
		{
			if (segments < 3)
			{
				Debug.LogError("VectorLine.MakeEllipse needs at least 3 segments");
				return;
			}
			if (!this.CheckArrayLength(VectorLine.FunctionName.MakeEllipse, segments, index))
			{
				return;
			}
			startDegrees = Mathf.Repeat(startDegrees, 360f);
			endDegrees = Mathf.Repeat(endDegrees, 360f);
			float num;
			float num2;
			if (startDegrees == endDegrees)
			{
				num = 360f;
				num2 = -pointRotation * 0.017453292f;
			}
			else
			{
				num = ((endDegrees > startDegrees) ? (endDegrees - startDegrees) : (360f - startDegrees + endDegrees));
				num2 = startDegrees * 0.017453292f;
			}
			float num3 = num / (float)segments * 0.017453292f;
			if (this.m_lineType != LineType.Discrete)
			{
				if (startDegrees != endDegrees)
				{
					segments++;
				}
				if (this.m_is2D)
				{
					Vector2 vector = origin;
					int i;
					for (i = 0; i < segments; i++)
					{
						this.m_points2[index + i] = vector + new Vector2(0.5f + Mathf.Sin(num2) * xRadius, 0.5f + Mathf.Cos(num2) * yRadius);
						num2 += num3;
					}
					if (this.m_lineType != LineType.Points && startDegrees == endDegrees)
					{
						this.m_points2[index + i] = this.m_points2[index + (i - segments)];
						return;
					}
				}
				else
				{
					Matrix4x4 matrix4x = Matrix4x4.TRS(Vector3.zero, Quaternion.LookRotation(-upVector, upVector), Vector3.one);
					int i;
					for (i = 0; i < segments; i++)
					{
						this.m_points3[index + i] = origin + matrix4x.MultiplyPoint3x4(new Vector3(Mathf.Sin(num2) * xRadius, Mathf.Cos(num2) * yRadius, 0f));
						num2 += num3;
					}
					if (this.m_lineType != LineType.Points && startDegrees == endDegrees)
					{
						this.m_points3[index + i] = this.m_points3[index + (i - segments)];
						return;
					}
				}
			}
			else
			{
				if (this.m_is2D)
				{
					Vector2 vector2 = origin;
					for (int j = 0; j < segments * 2; j++)
					{
						this.m_points2[index + j] = vector2 + new Vector2(0.5f + Mathf.Sin(num2) * xRadius, 0.5f + Mathf.Cos(num2) * yRadius);
						num2 += num3;
						j++;
						this.m_points2[index + j] = vector2 + new Vector2(0.5f + Mathf.Sin(num2) * xRadius, 0.5f + Mathf.Cos(num2) * yRadius);
					}
					return;
				}
				Matrix4x4 matrix4x2 = Matrix4x4.TRS(Vector3.zero, Quaternion.LookRotation(-upVector, upVector), Vector3.one);
				for (int k = 0; k < segments * 2; k++)
				{
					this.m_points3[index + k] = origin + matrix4x2.MultiplyPoint3x4(new Vector3(Mathf.Sin(num2) * xRadius, Mathf.Cos(num2) * yRadius, 0f));
					num2 += num3;
					k++;
					this.m_points3[index + k] = origin + matrix4x2.MultiplyPoint3x4(new Vector3(Mathf.Sin(num2) * xRadius, Mathf.Cos(num2) * yRadius, 0f));
				}
			}
		}

		// Token: 0x06001A57 RID: 6743 RVA: 0x0008EBB3 File Offset: 0x0008CDB3
		public void MakeCurve(Vector2[] curvePoints)
		{
			this.MakeCurve(curvePoints, this.GetSegmentNumber(), 0);
		}

		// Token: 0x06001A58 RID: 6744 RVA: 0x0008EBC3 File Offset: 0x0008CDC3
		public void MakeCurve(Vector2[] curvePoints, int segments)
		{
			this.MakeCurve(curvePoints, segments, 0);
		}

		// Token: 0x06001A59 RID: 6745 RVA: 0x0008EBD0 File Offset: 0x0008CDD0
		public void MakeCurve(Vector2[] curvePoints, int segments, int index)
		{
			if (curvePoints.Length != 4)
			{
				Debug.LogError("VectorLine.MakeCurve needs exactly 4 points in the curve points array");
				return;
			}
			this.MakeCurve(curvePoints[0], curvePoints[1], curvePoints[2], curvePoints[3], segments, index);
		}

		// Token: 0x06001A5A RID: 6746 RVA: 0x0008EC26 File Offset: 0x0008CE26
		public void MakeCurve(Vector3[] curvePoints)
		{
			this.MakeCurve(curvePoints, this.GetSegmentNumber(), 0);
		}

		// Token: 0x06001A5B RID: 6747 RVA: 0x0008EC36 File Offset: 0x0008CE36
		public void MakeCurve(Vector3[] curvePoints, int segments)
		{
			this.MakeCurve(curvePoints, segments, 0);
		}

		// Token: 0x06001A5C RID: 6748 RVA: 0x0008EC41 File Offset: 0x0008CE41
		public void MakeCurve(Vector3[] curvePoints, int segments, int index)
		{
			if (curvePoints.Length != 4)
			{
				Debug.LogError("VectorLine.MakeCurve needs exactly 4 points in the curve points array");
				return;
			}
			this.MakeCurve(curvePoints[0], curvePoints[1], curvePoints[2], curvePoints[3], segments, index);
		}

		// Token: 0x06001A5D RID: 6749 RVA: 0x0008EC78 File Offset: 0x0008CE78
		public void MakeCurve(Vector3 anchor1, Vector3 control1, Vector3 anchor2, Vector3 control2)
		{
			this.MakeCurve(anchor1, control1, anchor2, control2, this.GetSegmentNumber(), 0);
		}

		// Token: 0x06001A5E RID: 6750 RVA: 0x0008EC8C File Offset: 0x0008CE8C
		public void MakeCurve(Vector3 anchor1, Vector3 control1, Vector3 anchor2, Vector3 control2, int segments)
		{
			this.MakeCurve(anchor1, control1, anchor2, control2, segments, 0);
		}

		// Token: 0x06001A5F RID: 6751 RVA: 0x0008EC9C File Offset: 0x0008CE9C
		public void MakeCurve(Vector3 anchor1, Vector3 control1, Vector3 anchor2, Vector3 control2, int segments, int index)
		{
			if (!this.CheckArrayLength(VectorLine.FunctionName.MakeCurve, segments, index))
			{
				return;
			}
			if (this.m_lineType != LineType.Discrete)
			{
				int num = ((this.m_lineType == LineType.Points) ? segments : (segments + 1));
				if (this.m_is2D)
				{
					Vector2 vector = anchor1;
					Vector2 vector2 = anchor2;
					Vector2 vector3 = control1;
					Vector2 vector4 = control2;
					for (int i = 0; i < num; i++)
					{
						this.m_points2[index + i] = VectorLine.GetBezierPoint(ref vector, ref vector3, ref vector2, ref vector4, (float)i / (float)segments);
					}
					return;
				}
				for (int j = 0; j < num; j++)
				{
					this.m_points3[index + j] = VectorLine.GetBezierPoint3D(ref anchor1, ref control1, ref anchor2, ref control2, (float)j / (float)segments);
				}
				return;
			}
			else
			{
				int num2 = 0;
				if (this.m_is2D)
				{
					Vector2 vector5 = anchor1;
					Vector2 vector6 = anchor2;
					Vector2 vector7 = control1;
					Vector2 vector8 = control2;
					for (int k = 0; k < segments; k++)
					{
						this.m_points2[index + num2++] = VectorLine.GetBezierPoint(ref vector5, ref vector7, ref vector6, ref vector8, (float)k / (float)segments);
						this.m_points2[index + num2++] = VectorLine.GetBezierPoint(ref vector5, ref vector7, ref vector6, ref vector8, (float)(k + 1) / (float)segments);
					}
					return;
				}
				for (int l = 0; l < segments; l++)
				{
					this.m_points3[index + num2++] = VectorLine.GetBezierPoint3D(ref anchor1, ref control1, ref anchor2, ref control2, (float)l / (float)segments);
					this.m_points3[index + num2++] = VectorLine.GetBezierPoint3D(ref anchor1, ref control1, ref anchor2, ref control2, (float)(l + 1) / (float)segments);
				}
				return;
			}
		}

		// Token: 0x06001A60 RID: 6752 RVA: 0x0008EE60 File Offset: 0x0008D060
		private static Vector2 GetBezierPoint(ref Vector2 anchor1, ref Vector2 control1, ref Vector2 anchor2, ref Vector2 control2, float t)
		{
			float num = 3f * (control1.x - anchor1.x);
			float num2 = 3f * (control2.x - control1.x) - num;
			float num3 = anchor2.x - anchor1.x - num - num2;
			float num4 = 3f * (control1.y - anchor1.y);
			float num5 = 3f * (control2.y - control1.y) - num4;
			float num6 = anchor2.y - anchor1.y - num4 - num5;
			return new Vector2(num3 * (t * t * t) + num2 * (t * t) + num * t + anchor1.x, num6 * (t * t * t) + num5 * (t * t) + num4 * t + anchor1.y);
		}

		// Token: 0x06001A61 RID: 6753 RVA: 0x0008EF28 File Offset: 0x0008D128
		private static Vector3 GetBezierPoint3D(ref Vector3 anchor1, ref Vector3 control1, ref Vector3 anchor2, ref Vector3 control2, float t)
		{
			float num = 3f * (control1.x - anchor1.x);
			float num2 = 3f * (control2.x - control1.x) - num;
			float num3 = anchor2.x - anchor1.x - num - num2;
			float num4 = 3f * (control1.y - anchor1.y);
			float num5 = 3f * (control2.y - control1.y) - num4;
			float num6 = anchor2.y - anchor1.y - num4 - num5;
			float num7 = 3f * (control1.z - anchor1.z);
			float num8 = 3f * (control2.z - control1.z) - num7;
			float num9 = anchor2.z - anchor1.z - num7 - num8;
			return new Vector3(num3 * (t * t * t) + num2 * (t * t) + num * t + anchor1.x, num6 * (t * t * t) + num5 * (t * t) + num4 * t + anchor1.y, num9 * (t * t * t) + num8 * (t * t) + num7 * t + anchor1.z);
		}

		// Token: 0x06001A62 RID: 6754 RVA: 0x0008F051 File Offset: 0x0008D251
		public void MakeSpline(Vector2[] splinePoints)
		{
			this.MakeSpline(splinePoints, null, this.GetSegmentNumber(), 0, false);
		}

		// Token: 0x06001A63 RID: 6755 RVA: 0x0008F063 File Offset: 0x0008D263
		public void MakeSpline(Vector2[] splinePoints, bool loop)
		{
			this.MakeSpline(splinePoints, null, this.GetSegmentNumber(), 0, loop);
		}

		// Token: 0x06001A64 RID: 6756 RVA: 0x0008F075 File Offset: 0x0008D275
		public void MakeSpline(Vector2[] splinePoints, int segments)
		{
			this.MakeSpline(splinePoints, null, segments, 0, false);
		}

		// Token: 0x06001A65 RID: 6757 RVA: 0x0008F082 File Offset: 0x0008D282
		public void MakeSpline(Vector2[] splinePoints, int segments, bool loop)
		{
			this.MakeSpline(splinePoints, null, segments, 0, loop);
		}

		// Token: 0x06001A66 RID: 6758 RVA: 0x0008F08F File Offset: 0x0008D28F
		public void MakeSpline(Vector2[] splinePoints, int segments, int index)
		{
			this.MakeSpline(splinePoints, null, segments, index, false);
		}

		// Token: 0x06001A67 RID: 6759 RVA: 0x0008F09C File Offset: 0x0008D29C
		public void MakeSpline(Vector2[] splinePoints, int segments, int index, bool loop)
		{
			this.MakeSpline(splinePoints, null, segments, index, loop);
		}

		// Token: 0x06001A68 RID: 6760 RVA: 0x0008F0AA File Offset: 0x0008D2AA
		public void MakeSpline(Vector3[] splinePoints)
		{
			this.MakeSpline(null, splinePoints, this.GetSegmentNumber(), 0, false);
		}

		// Token: 0x06001A69 RID: 6761 RVA: 0x0008F0BC File Offset: 0x0008D2BC
		public void MakeSpline(Vector3[] splinePoints, bool loop)
		{
			this.MakeSpline(null, splinePoints, this.GetSegmentNumber(), 0, loop);
		}

		// Token: 0x06001A6A RID: 6762 RVA: 0x0008F0CE File Offset: 0x0008D2CE
		public void MakeSpline(Vector3[] splinePoints, int segments)
		{
			this.MakeSpline(null, splinePoints, segments, 0, false);
		}

		// Token: 0x06001A6B RID: 6763 RVA: 0x0008F0DB File Offset: 0x0008D2DB
		public void MakeSpline(Vector3[] splinePoints, int segments, bool loop)
		{
			this.MakeSpline(null, splinePoints, segments, 0, loop);
		}

		// Token: 0x06001A6C RID: 6764 RVA: 0x0008F0E8 File Offset: 0x0008D2E8
		public void MakeSpline(Vector3[] splinePoints, int segments, int index)
		{
			this.MakeSpline(null, splinePoints, segments, index, false);
		}

		// Token: 0x06001A6D RID: 6765 RVA: 0x0008F0F5 File Offset: 0x0008D2F5
		public void MakeSpline(Vector3[] splinePoints, int segments, int index, bool loop)
		{
			this.MakeSpline(null, splinePoints, segments, index, loop);
		}

		// Token: 0x06001A6E RID: 6766 RVA: 0x0008F104 File Offset: 0x0008D304
		private void MakeSpline(Vector2[] splinePoints2, Vector3[] splinePoints3, int segments, int index, bool loop)
		{
			int num = ((splinePoints2 != null) ? splinePoints2.Length : splinePoints3.Length);
			if (num < 2)
			{
				Debug.LogError("VectorLine.MakeSpline needs at least 2 spline points");
				return;
			}
			if (splinePoints2 != null && !this.m_is2D)
			{
				Debug.LogError("VectorLine.MakeSpline was called with a Vector2 spline points array, but the line uses Vector3 points");
				return;
			}
			if (splinePoints3 != null && this.m_is2D)
			{
				Debug.LogError("VectorLine.MakeSpline was called with a Vector3 spline points array, but the line uses Vector2 points");
				return;
			}
			if (!this.CheckArrayLength(VectorLine.FunctionName.MakeSpline, segments, index))
			{
				return;
			}
			int num2 = index;
			int num3 = (loop ? num : (num - 1));
			float num4 = 1f / (float)segments * (float)num3;
			float num5 = 0f;
			int num6 = 0;
			int num7 = 0;
			int num8 = 0;
			int i;
			for (i = 0; i < num3; i++)
			{
				num6 = i - 1;
				num7 = i + 1;
				num8 = i + 2;
				if (num6 < 0)
				{
					num6 = (loop ? (num3 - 1) : 0);
				}
				if (loop && num7 > num3 - 1)
				{
					num7 -= num3;
				}
				if (num8 > num3 - 1)
				{
					num8 = (loop ? (num8 - num3) : num3);
				}
				float num9;
				if (this.m_lineType != LineType.Discrete)
				{
					if (this.m_is2D)
					{
						for (num9 = num5; num9 <= 1f; num9 += num4)
						{
							this.m_points2[num2++] = VectorLine.GetSplinePoint(ref splinePoints2[num6], ref splinePoints2[i], ref splinePoints2[num7], ref splinePoints2[num8], num9);
						}
					}
					else
					{
						Vector4 zero = Vector4.zero;
						Vector4 zero2 = Vector4.zero;
						Vector4 zero3 = Vector4.zero;
						VectorLine.GetSplineCubic3D(ref splinePoints3[num6], ref splinePoints3[i], ref splinePoints3[num7], ref splinePoints3[num8], ref zero, ref zero2, ref zero3);
						this.m_points3[num2++] = splinePoints3[i];
						float num10 = 1f - num4 * 0.5f;
						for (num9 = num4; num9 <= num10; num9 += num4)
						{
							this.m_points3[num2++] = VectorLine.SolveSplineCubic3D(ref zero, ref zero2, ref zero3, num9);
						}
					}
				}
				else if (this.m_is2D)
				{
					for (num9 = num5; num9 <= 1f; num9 += num4)
					{
						this.m_points2[num2++] = VectorLine.GetSplinePoint(ref splinePoints2[num6], ref splinePoints2[i], ref splinePoints2[num7], ref splinePoints2[num8], num9);
						if (num2 > index + 1 && num2 < index + segments * 2)
						{
							this.m_points2[num2++] = this.m_points2[num2 - 2];
						}
					}
				}
				else
				{
					Vector4 zero4 = Vector4.zero;
					Vector4 zero5 = Vector4.zero;
					Vector4 zero6 = Vector4.zero;
					VectorLine.GetSplineCubic3D(ref splinePoints3[num6], ref splinePoints3[i], ref splinePoints3[num7], ref splinePoints3[num8], ref zero4, ref zero5, ref zero6);
					for (num9 = num5; num9 <= 1f; num9 += num4)
					{
						this.m_points3[num2++] = VectorLine.SolveSplineCubic3D(ref zero4, ref zero5, ref zero6, num9);
						if (num2 > index + 1 && num2 < index + segments * 2)
						{
							this.m_points3[num2++] = this.m_points3[num2 - 2];
						}
					}
				}
				num5 = num9 - 1f;
			}
			if ((this.m_lineType != LineType.Discrete && num2 < index + (segments + 1)) || (this.m_lineType == LineType.Discrete && num2 < index + segments * 2))
			{
				if (this.m_is2D)
				{
					this.m_points2[num2] = VectorLine.GetSplinePoint(ref splinePoints2[num6], ref splinePoints2[i - 1], ref splinePoints2[num7], ref splinePoints2[num8], 1f);
					return;
				}
				int num11 = splinePoints3.Length - 1;
				for (int j = num2; j < this.m_points3.Count; j++)
				{
					this.m_points3[j] = splinePoints3[num11];
				}
			}
		}

		// Token: 0x06001A6F RID: 6767 RVA: 0x0008F4BC File Offset: 0x0008D6BC
		private static Vector2 GetSplinePoint(ref Vector2 p0, ref Vector2 p1, ref Vector2 p2, ref Vector2 p3, float t)
		{
			Vector4 zero = Vector4.zero;
			Vector4 zero2 = Vector4.zero;
			float num = Mathf.Pow(VectorLine.VectorDistanceSquared(ref p0, ref p1), 0.25f);
			float num2 = Mathf.Pow(VectorLine.VectorDistanceSquared(ref p1, ref p2), 0.25f);
			float num3 = Mathf.Pow(VectorLine.VectorDistanceSquared(ref p2, ref p3), 0.25f);
			if (num2 < 0.0001f)
			{
				num2 = 1f;
			}
			if (num < 0.0001f)
			{
				num = num2;
			}
			if (num3 < 0.0001f)
			{
				num3 = num2;
			}
			VectorLine.InitNonuniformCatmullRom(p0.x, p1.x, p2.x, p3.x, num, num2, num3, ref zero);
			VectorLine.InitNonuniformCatmullRom(p0.y, p1.y, p2.y, p3.y, num, num2, num3, ref zero2);
			return new Vector2(VectorLine.EvalCubicPoly(ref zero, t), VectorLine.EvalCubicPoly(ref zero2, t));
		}

		// Token: 0x06001A70 RID: 6768 RVA: 0x0008F590 File Offset: 0x0008D790
		private static void GetSplineCubic3D(ref Vector3 p0, ref Vector3 p1, ref Vector3 p2, ref Vector3 p3, ref Vector4 px, ref Vector4 py, ref Vector4 pz)
		{
			float num = Mathf.Pow(VectorLine.VectorDistanceSquared(ref p0, ref p1), 0.25f);
			float num2 = Mathf.Pow(VectorLine.VectorDistanceSquared(ref p1, ref p2), 0.25f);
			float num3 = Mathf.Pow(VectorLine.VectorDistanceSquared(ref p2, ref p3), 0.25f);
			if (num2 < 0.0001f)
			{
				num2 = 1f;
			}
			if (num < 0.0001f)
			{
				num = num2;
			}
			if (num3 < 0.0001f)
			{
				num3 = num2;
			}
			VectorLine.InitNonuniformCatmullRom(p0.x, p1.x, p2.x, p3.x, num, num2, num3, ref px);
			VectorLine.InitNonuniformCatmullRom(p0.y, p1.y, p2.y, p3.y, num, num2, num3, ref py);
			VectorLine.InitNonuniformCatmullRom(p0.z, p1.z, p2.z, p3.z, num, num2, num3, ref pz);
		}

		// Token: 0x06001A71 RID: 6769 RVA: 0x0008F65B File Offset: 0x0008D85B
		private static Vector3 SolveSplineCubic3D(ref Vector4 px, ref Vector4 py, ref Vector4 pz, float t)
		{
			return new Vector3(VectorLine.EvalCubicPoly(ref px, t), VectorLine.EvalCubicPoly(ref py, t), VectorLine.EvalCubicPoly(ref pz, t));
		}

		// Token: 0x06001A72 RID: 6770 RVA: 0x0008F678 File Offset: 0x0008D878
		private static Vector3 GetSplinePoint3D(ref Vector3 p0, ref Vector3 p1, ref Vector3 p2, ref Vector3 p3, float t)
		{
			Vector4 zero = Vector4.zero;
			Vector4 zero2 = Vector4.zero;
			Vector4 zero3 = Vector4.zero;
			float num = Mathf.Pow(VectorLine.VectorDistanceSquared(ref p0, ref p1), 0.25f);
			float num2 = Mathf.Pow(VectorLine.VectorDistanceSquared(ref p1, ref p2), 0.25f);
			float num3 = Mathf.Pow(VectorLine.VectorDistanceSquared(ref p2, ref p3), 0.25f);
			if (num2 < 0.0001f)
			{
				num2 = 1f;
			}
			if (num < 0.0001f)
			{
				num = num2;
			}
			if (num3 < 0.0001f)
			{
				num3 = num2;
			}
			VectorLine.InitNonuniformCatmullRom(p0.x, p1.x, p2.x, p3.x, num, num2, num3, ref zero);
			VectorLine.InitNonuniformCatmullRom(p0.y, p1.y, p2.y, p3.y, num, num2, num3, ref zero2);
			VectorLine.InitNonuniformCatmullRom(p0.z, p1.z, p2.z, p3.z, num, num2, num3, ref zero3);
			return new Vector3(VectorLine.EvalCubicPoly(ref zero, t), VectorLine.EvalCubicPoly(ref zero2, t), VectorLine.EvalCubicPoly(ref zero3, t));
		}

		// Token: 0x06001A73 RID: 6771 RVA: 0x0008F784 File Offset: 0x0008D984
		private static float VectorDistanceSquared(ref Vector2 p, ref Vector2 q)
		{
			float num = q.x - p.x;
			float num2 = q.y - p.y;
			return num * num + num2 * num2;
		}

		// Token: 0x06001A74 RID: 6772 RVA: 0x0008F7B4 File Offset: 0x0008D9B4
		private static float VectorDistanceSquared(ref Vector3 p, ref Vector3 q)
		{
			float num = q.x - p.x;
			float num2 = q.y - p.y;
			float num3 = q.z - p.z;
			return num * num + num2 * num2 + num3 * num3;
		}

		// Token: 0x06001A75 RID: 6773 RVA: 0x0008F7F4 File Offset: 0x0008D9F4
		private static void InitNonuniformCatmullRom(float x0, float x1, float x2, float x3, float dt0, float dt1, float dt2, ref Vector4 p)
		{
			float num = ((x1 - x0) / dt0 - (x2 - x0) / (dt0 + dt1)) * dt1 + (x2 - x1);
			float num2 = (-(x3 - x1) / (dt1 + dt2) + (x3 - x2) / dt2) * dt1 + (x2 - x1);
			p.x = x1;
			p.y = num;
			p.z = -3f * x1 + 3f * x2 - 2f * num - num2;
			p.w = 2f * x1 - 2f * x2 + num + num2;
		}

		// Token: 0x06001A76 RID: 6774 RVA: 0x0008F87C File Offset: 0x0008DA7C
		private static float EvalCubicPoly(ref Vector4 p, float t)
		{
			return p.x + p.y * t + p.z * (t * t) + p.w * (t * t * t);
		}

		// Token: 0x06001A77 RID: 6775 RVA: 0x0008F8A5 File Offset: 0x0008DAA5
		public void MakeText(string text, Vector3 startPos, float size)
		{
			this.MakeText(text, startPos, size, 1f, 1.5f, true);
		}

		// Token: 0x06001A78 RID: 6776 RVA: 0x0008F8BB File Offset: 0x0008DABB
		public void MakeText(string text, Vector3 startPos, float size, bool uppercaseOnly)
		{
			this.MakeText(text, startPos, size, 1f, 1.5f, uppercaseOnly);
		}

		// Token: 0x06001A79 RID: 6777 RVA: 0x0008F8D2 File Offset: 0x0008DAD2
		public void MakeText(string text, Vector3 startPos, float size, float charSpacing, float lineSpacing)
		{
			this.MakeText(text, startPos, size, charSpacing, lineSpacing, true);
		}

		// Token: 0x06001A7A RID: 6778 RVA: 0x0008F8E4 File Offset: 0x0008DAE4
		public void MakeText(string text, Vector3 startPos, float size, float charSpacing, float lineSpacing, bool uppercaseOnly)
		{
			if (this.m_lineType != LineType.Discrete)
			{
				Debug.LogError("VectorLine.MakeText only works with a discrete line");
				return;
			}
			int num = 0;
			for (int i = 0; i < text.Length; i++)
			{
				int num2 = Convert.ToInt32(text[i]);
				if (num2 < 0 || num2 > 256)
				{
					Debug.LogError("VectorLine.MakeText: Character '" + text[i].ToString() + "' is not valid");
					return;
				}
				if (uppercaseOnly && num2 >= 97 && num2 <= 122)
				{
					num2 -= 32;
				}
				if (VectorChar.data[num2] != null)
				{
					num += VectorChar.data[num2].Length;
				}
			}
			if (num != this.pointsCount)
			{
				this.Resize(num);
			}
			float num3 = 0f;
			float num4 = 0f;
			int num5 = 0;
			Vector2 vector = new Vector2(size, size);
			for (int j = 0; j < text.Length; j++)
			{
				int num6 = Convert.ToInt32(text[j]);
				if (num6 == 10)
				{
					num4 -= lineSpacing;
					num3 = 0f;
				}
				else if (num6 == 32)
				{
					num3 += charSpacing;
				}
				else
				{
					if (uppercaseOnly && num6 >= 97 && num6 <= 122)
					{
						num6 -= 32;
					}
					if (VectorChar.data[num6] != null)
					{
						int num7 = VectorChar.data[num6].Length;
						if (this.m_is2D)
						{
							for (int k = 0; k < num7; k++)
							{
								this.m_points2[num5++] = Vector2.Scale(VectorChar.data[num6][k] + new Vector2(num3, num4), vector) + startPos;
							}
						}
						else
						{
							for (int l = 0; l < num7; l++)
							{
								this.m_points3[num5++] = Vector3.Scale(VectorChar.data[num6][l] + new Vector3(num3, num4, 0f), vector) + startPos;
							}
						}
						num3 += charSpacing;
					}
					else
					{
						num3 += charSpacing;
					}
				}
			}
		}

		// Token: 0x06001A7B RID: 6779 RVA: 0x0008FAF8 File Offset: 0x0008DCF8
		public void MakeWireframe(Mesh mesh)
		{
			if (this.m_lineType != LineType.Discrete)
			{
				Debug.LogError("VectorLine.MakeWireframe only works with a discrete line");
				return;
			}
			if (this.m_is2D)
			{
				Debug.LogError("VectorLine.MakeWireframe can only be used with Vector3 points, which \"" + this.name + "\" doesn't have");
				return;
			}
			if (mesh == null)
			{
				Debug.LogError("VectorLine.MakeWireframe can't use a null mesh");
				return;
			}
			Vector3[] vertices = mesh.vertices;
			Dictionary<Vector3Pair, bool> dictionary = new Dictionary<Vector3Pair, bool>();
			List<Vector3> list = new List<Vector3>();
			for (int i = 0; i < mesh.subMeshCount; i++)
			{
				int[] indices = mesh.GetIndices(i);
				int num = ((mesh.GetTopology(i) == MeshTopology.Triangles) ? 3 : 4);
				for (int j = 0; j < indices.Length; j += num)
				{
					for (int k = 0; k < num; k++)
					{
						VectorLine.CheckPairPoints(dictionary, vertices[indices[j + k]], vertices[indices[j + (k + 1) % num]], list);
					}
				}
			}
			if (list.Count != this.m_pointsCount)
			{
				this.Resize(list.Count);
			}
			for (int l = 0; l < this.m_pointsCount; l++)
			{
				this.m_points3[l] = list[l];
			}
		}

		// Token: 0x06001A7C RID: 6780 RVA: 0x0008FC20 File Offset: 0x0008DE20
		private static void CheckPairPoints(Dictionary<Vector3Pair, bool> pairs, Vector3 p1, Vector3 p2, List<Vector3> linePoints)
		{
			Vector3Pair vector3Pair = new Vector3Pair(p1, p2);
			Vector3Pair vector3Pair2 = new Vector3Pair(p2, p1);
			if (!pairs.ContainsKey(vector3Pair) && !pairs.ContainsKey(vector3Pair2))
			{
				pairs[vector3Pair] = true;
				pairs[vector3Pair2] = true;
				linePoints.Add(p1);
				linePoints.Add(p2);
			}
		}

		// Token: 0x06001A7D RID: 6781 RVA: 0x0008FC6F File Offset: 0x0008DE6F
		public void MakeCube(Vector3 position, float xSize, float ySize, float zSize)
		{
			this.MakeCube(position, xSize, ySize, zSize, 0);
		}

		// Token: 0x06001A7E RID: 6782 RVA: 0x0008FC80 File Offset: 0x0008DE80
		public void MakeCube(Vector3 position, float xSize, float ySize, float zSize, int index)
		{
			if (this.m_lineType != LineType.Discrete)
			{
				Debug.LogError("VectorLine.MakeCube only works with a discrete line");
				return;
			}
			if (this.m_is2D)
			{
				Debug.LogError("VectorLine.MakeCube can only be used with Vector3 points, which \"" + this.name + "\" doesn't have");
				return;
			}
			if (index + 24 <= this.pointsCount)
			{
				xSize /= 2f;
				ySize /= 2f;
				zSize /= 2f;
				this.m_points3[index] = position + new Vector3(-xSize, ySize, -zSize);
				this.m_points3[index + 1] = position + new Vector3(xSize, ySize, -zSize);
				this.m_points3[index + 2] = position + new Vector3(xSize, ySize, -zSize);
				this.m_points3[index + 3] = position + new Vector3(xSize, ySize, zSize);
				this.m_points3[index + 4] = position + new Vector3(xSize, ySize, zSize);
				this.m_points3[index + 5] = position + new Vector3(-xSize, ySize, zSize);
				this.m_points3[index + 6] = position + new Vector3(-xSize, ySize, zSize);
				this.m_points3[index + 7] = position + new Vector3(-xSize, ySize, -zSize);
				this.m_points3[index + 8] = position + new Vector3(-xSize, -ySize, -zSize);
				this.m_points3[index + 9] = position + new Vector3(-xSize, ySize, -zSize);
				this.m_points3[index + 10] = position + new Vector3(xSize, -ySize, -zSize);
				this.m_points3[index + 11] = position + new Vector3(xSize, ySize, -zSize);
				this.m_points3[index + 12] = position + new Vector3(-xSize, -ySize, zSize);
				this.m_points3[index + 13] = position + new Vector3(-xSize, ySize, zSize);
				this.m_points3[index + 14] = position + new Vector3(xSize, -ySize, zSize);
				this.m_points3[index + 15] = position + new Vector3(xSize, ySize, zSize);
				this.m_points3[index + 16] = position + new Vector3(-xSize, -ySize, -zSize);
				this.m_points3[index + 17] = position + new Vector3(xSize, -ySize, -zSize);
				this.m_points3[index + 18] = position + new Vector3(xSize, -ySize, -zSize);
				this.m_points3[index + 19] = position + new Vector3(xSize, -ySize, zSize);
				this.m_points3[index + 20] = position + new Vector3(xSize, -ySize, zSize);
				this.m_points3[index + 21] = position + new Vector3(-xSize, -ySize, zSize);
				this.m_points3[index + 22] = position + new Vector3(-xSize, -ySize, zSize);
				this.m_points3[index + 23] = position + new Vector3(-xSize, -ySize, -zSize);
				return;
			}
			if (index == 0)
			{
				Debug.LogError("VectorLine.MakeCube: The number of Vector3 points needs to be at least 24 for \"" + this.name + "\"");
				return;
			}
			Debug.LogError(string.Concat(new string[]
			{
				"Calling VectorLine.MakeCube with an index of ",
				index.ToString(),
				" would exceed the length of the Vector3 points for \"",
				this.name,
				"\""
			}));
		}

		// Token: 0x0400165E RID: 5726
		private static Material s_defaultMaterial = null;

		// Token: 0x0400165F RID: 5727
		private bool m_useCustomMaterial;

		// Token: 0x04001660 RID: 5728
		[SerializeField]
		private Vector3[] m_lineVertices;

		// Token: 0x04001661 RID: 5729
		[SerializeField]
		private Vector2[] m_lineUVs;

		// Token: 0x04001662 RID: 5730
		[SerializeField]
		private Color[] m_lineColors;

		// Token: 0x04001663 RID: 5731
		[SerializeField]
		private List<int> m_lineTriangles;

		// Token: 0x04001664 RID: 5732
		[SerializeField]
		private int m_vertexCount;

		// Token: 0x04001665 RID: 5733
		[SerializeField]
		private GameObject m_go;

		// Token: 0x04001666 RID: 5734
		[SerializeField]
		private RectTransform m_rectTransform;

		// Token: 0x04001667 RID: 5735
		private IVectorObject m_vectorObject;

		// Token: 0x04001668 RID: 5736
		[SerializeField]
		private Color m_color;

		// Token: 0x04001669 RID: 5737
		[SerializeField]
		private CanvasState m_canvasState;

		// Token: 0x0400166A RID: 5738
		[SerializeField]
		private bool m_is2D;

		// Token: 0x0400166B RID: 5739
		[SerializeField]
		private List<Vector2> m_points2;

		// Token: 0x0400166C RID: 5740
		[SerializeField]
		private List<Vector3> m_points3;

		// Token: 0x0400166D RID: 5741
		[SerializeField]
		private int m_pointsCount;

		// Token: 0x0400166E RID: 5742
		[SerializeField]
		private Vector3[] m_screenPoints;

		// Token: 0x0400166F RID: 5743
		[SerializeField]
		private float[] m_lineWidths;

		// Token: 0x04001670 RID: 5744
		[SerializeField]
		private float m_lineWidth;

		// Token: 0x04001671 RID: 5745
		[SerializeField]
		private float m_maxWeldDistance;

		// Token: 0x04001672 RID: 5746
		[SerializeField]
		private float[] m_distances;

		// Token: 0x04001673 RID: 5747
		[SerializeField]
		private string m_name;

		// Token: 0x04001674 RID: 5748
		[SerializeField]
		private Material m_material;

		// Token: 0x04001675 RID: 5749
		[SerializeField]
		private Texture m_originalTexture;

		// Token: 0x04001676 RID: 5750
		[SerializeField]
		private Texture m_texture;

		// Token: 0x04001677 RID: 5751
		[SerializeField]
		private bool m_active = true;

		// Token: 0x04001678 RID: 5752
		[SerializeField]
		private LineType m_lineType;

		// Token: 0x04001679 RID: 5753
		[SerializeField]
		private float m_capLength;

		// Token: 0x0400167A RID: 5754
		[SerializeField]
		private bool m_smoothWidth;

		// Token: 0x0400167B RID: 5755
		[SerializeField]
		private bool m_smoothColor;

		// Token: 0x0400167C RID: 5756
		[SerializeField]
		private Joins m_joins;

		// Token: 0x0400167D RID: 5757
		[SerializeField]
		private bool m_isAutoDrawing;

		// Token: 0x0400167E RID: 5758
		[SerializeField]
		private int m_drawStart;

		// Token: 0x0400167F RID: 5759
		[SerializeField]
		private int m_drawEnd;

		// Token: 0x04001680 RID: 5760
		[SerializeField]
		private int m_endPointsUpdate;

		// Token: 0x04001681 RID: 5761
		[SerializeField]
		private bool m_useNormals;

		// Token: 0x04001682 RID: 5762
		[SerializeField]
		private bool m_useTangents;

		// Token: 0x04001683 RID: 5763
		[SerializeField]
		private bool m_normalsCalculated;

		// Token: 0x04001684 RID: 5764
		[SerializeField]
		private bool m_tangentsCalculated;

		// Token: 0x04001685 RID: 5765
		[SerializeField]
		private EndCap m_capType = EndCap.None;

		// Token: 0x04001686 RID: 5766
		[SerializeField]
		private string m_endCap;

		// Token: 0x04001687 RID: 5767
		[SerializeField]
		private bool m_useCapColors;

		// Token: 0x04001688 RID: 5768
		[SerializeField]
		private Color32 m_frontColor;

		// Token: 0x04001689 RID: 5769
		[SerializeField]
		private Color32 m_backColor;

		// Token: 0x0400168A RID: 5770
		[SerializeField]
		private int m_frontEndCapIndex = -1;

		// Token: 0x0400168B RID: 5771
		[SerializeField]
		private int m_backEndCapIndex = -1;

		// Token: 0x0400168C RID: 5772
		[SerializeField]
		private float m_lineUVBottom;

		// Token: 0x0400168D RID: 5773
		[SerializeField]
		private float m_lineUVTop;

		// Token: 0x0400168E RID: 5774
		[SerializeField]
		private float m_frontCapUVBottom;

		// Token: 0x0400168F RID: 5775
		[SerializeField]
		private float m_frontCapUVTop;

		// Token: 0x04001690 RID: 5776
		[SerializeField]
		private float m_backCapUVBottom;

		// Token: 0x04001691 RID: 5777
		[SerializeField]
		private float m_backCapUVTop;

		// Token: 0x04001692 RID: 5778
		[SerializeField]
		private bool m_continuousTexture;

		// Token: 0x04001693 RID: 5779
		[SerializeField]
		private Transform m_drawTransform;

		// Token: 0x04001694 RID: 5780
		[SerializeField]
		private bool m_viewportDraw;

		// Token: 0x04001695 RID: 5781
		[SerializeField]
		private float m_textureScale;

		// Token: 0x04001696 RID: 5782
		[SerializeField]
		private bool m_useTextureScale;

		// Token: 0x04001697 RID: 5783
		[SerializeField]
		private float m_textureOffset;

		// Token: 0x04001698 RID: 5784
		[SerializeField]
		private bool m_useMatrix;

		// Token: 0x04001699 RID: 5785
		[SerializeField]
		private Matrix4x4 m_matrix;

		// Token: 0x0400169A RID: 5786
		[SerializeField]
		private bool m_collider;

		// Token: 0x0400169B RID: 5787
		[SerializeField]
		private bool m_trigger;

		// Token: 0x0400169C RID: 5788
		[SerializeField]
		private PhysicsMaterial2D m_physicsMaterial;

		// Token: 0x0400169D RID: 5789
		[SerializeField]
		private bool m_alignOddWidthToPixels;

		// Token: 0x0400169E RID: 5790
		private static Vector3 v3zero = Vector3.zero;

		// Token: 0x0400169F RID: 5791
		private static Canvas m_canvas;

		// Token: 0x040016A0 RID: 5792
		private static Transform camTransform;

		// Token: 0x040016A1 RID: 5793
		private static Camera cam3D;

		// Token: 0x040016A2 RID: 5794
		private static Vector3 oldPosition;

		// Token: 0x040016A3 RID: 5795
		private static Vector3 oldRotation;

		// Token: 0x040016A4 RID: 5796
		private static bool lineManagerCreated = false;

		// Token: 0x040016A5 RID: 5797
		private static LineManager m_lineManager;

		// Token: 0x040016A6 RID: 5798
		private static Dictionary<string, CapInfo> capDictionary;

		// Token: 0x040016A7 RID: 5799
		private static int endianDiff1;

		// Token: 0x040016A8 RID: 5800
		private static int endianDiff2;

		// Token: 0x040016A9 RID: 5801
		private static byte[] byteBlock;

		// Token: 0x040016AA RID: 5802
		private static string[] functionNames = new string[] { "VectorLine.SetColors: Length of color", "VectorLine.SetWidths: Length of line widths", "MakeCurve", "MakeSpline", "MakeEllipse" };

		// Token: 0x02000C5C RID: 3164
		private enum FunctionName
		{
			// Token: 0x04004E22 RID: 20002
			SetColors,
			// Token: 0x04004E23 RID: 20003
			SetWidths,
			// Token: 0x04004E24 RID: 20004
			MakeCurve,
			// Token: 0x04004E25 RID: 20005
			MakeSpline,
			// Token: 0x04004E26 RID: 20006
			MakeEllipse
		}
	}
}
