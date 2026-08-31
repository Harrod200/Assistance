using System;
using System.Collections.Generic;
using UnityEngine;

namespace Vectrosity
{
	// Token: 0x020004A3 RID: 1187
	public class VectorManager
	{
		// Token: 0x06001A80 RID: 6784 RVA: 0x000900A2 File Offset: 0x0008E2A2
		public static void SetBrightnessParameters(float fadeOutDistance, float fullBrightDistance, int levels, float frequency, Color color)
		{
			VectorManager.minBrightnessDistance = fadeOutDistance * fadeOutDistance;
			VectorManager.maxBrightnessDistance = fullBrightDistance * fullBrightDistance;
			VectorManager.brightnessLevels = levels;
			VectorManager.distanceCheckFrequency = frequency;
			VectorManager.fogColor = color;
		}

		// Token: 0x06001A81 RID: 6785 RVA: 0x000900C8 File Offset: 0x0008E2C8
		public static float GetBrightnessValue(Vector3 pos)
		{
			if (!VectorLine.camTransformExists)
			{
				VectorLine.SetCamera3D();
			}
			return Mathf.InverseLerp(VectorManager.minBrightnessDistance, VectorManager.maxBrightnessDistance, (pos - VectorLine.camTransformPosition).sqrMagnitude);
		}

		// Token: 0x06001A82 RID: 6786 RVA: 0x00090103 File Offset: 0x0008E303
		public static void ObjectSetup(GameObject go, VectorLine line, Visibility visibility, Brightness brightness)
		{
			VectorManager.ObjectSetup(go, line, visibility, brightness, true);
		}

		// Token: 0x06001A83 RID: 6787 RVA: 0x00090110 File Offset: 0x0008E310
		public static void ObjectSetup(GameObject go, VectorLine line, Visibility visibility, Brightness brightness, bool makeBounds)
		{
			VisibilityControl visibilityControl = go.GetComponent(typeof(VisibilityControl)) as VisibilityControl;
			VisibilityControlStatic visibilityControlStatic = go.GetComponent(typeof(VisibilityControlStatic)) as VisibilityControlStatic;
			VisibilityControlAlways visibilityControlAlways = go.GetComponent(typeof(VisibilityControlAlways)) as VisibilityControlAlways;
			BrightnessControl brightnessControl = go.GetComponent(typeof(BrightnessControl)) as BrightnessControl;
			if (go.GetComponent(typeof(MeshFilter)) as MeshFilter == null)
			{
				go.AddComponent<MeshFilter>();
			}
			if (go.GetComponent(typeof(MeshRenderer)) as MeshRenderer == null)
			{
				go.AddComponent<MeshRenderer>();
			}
			if (visibility == Visibility.Dynamic)
			{
				if (visibilityControlStatic)
				{
					visibilityControlStatic.DontDestroyLine();
					global::UnityEngine.Object.Destroy(visibilityControlStatic);
					VectorManager.ResetLinePoints(visibilityControlStatic, line);
				}
				if (visibilityControlAlways)
				{
					visibilityControlAlways.DontDestroyLine();
					global::UnityEngine.Object.Destroy(visibilityControlAlways);
				}
				if (visibilityControl == null)
				{
					visibilityControl = go.AddComponent(typeof(VisibilityControl)) as VisibilityControl;
					visibilityControl.Setup(line, makeBounds);
					if (brightnessControl != null)
					{
						brightnessControl.SetUseLine(false);
					}
				}
			}
			else if (visibility == Visibility.Static)
			{
				if (visibilityControl)
				{
					visibilityControl.DontDestroyLine();
					global::UnityEngine.Object.Destroy(visibilityControl);
				}
				if (visibilityControlAlways)
				{
					visibilityControlAlways.DontDestroyLine();
					global::UnityEngine.Object.Destroy(visibilityControlAlways);
				}
				if (visibilityControlStatic == null)
				{
					visibilityControlStatic = go.AddComponent(typeof(VisibilityControlStatic)) as VisibilityControlStatic;
					visibilityControlStatic.Setup(line, makeBounds);
					if (brightnessControl != null)
					{
						brightnessControl.SetUseLine(false);
					}
				}
			}
			else if (visibility == Visibility.Always)
			{
				if (visibilityControl)
				{
					visibilityControl.DontDestroyLine();
					global::UnityEngine.Object.Destroy(visibilityControl);
				}
				if (visibilityControlStatic)
				{
					visibilityControlStatic.DontDestroyLine();
					global::UnityEngine.Object.Destroy(visibilityControlStatic);
					VectorManager.ResetLinePoints(visibilityControlStatic, line);
				}
				if (visibilityControlAlways == null)
				{
					visibilityControlAlways = go.AddComponent(typeof(VisibilityControlAlways)) as VisibilityControlAlways;
					visibilityControlAlways.Setup(line);
					if (brightnessControl != null)
					{
						brightnessControl.SetUseLine(false);
					}
				}
			}
			if (brightness == Brightness.Fog)
			{
				if (brightnessControl == null)
				{
					brightnessControl = go.AddComponent(typeof(BrightnessControl)) as BrightnessControl;
					if (visibilityControl == null && visibilityControlStatic == null && visibilityControlAlways == null)
					{
						brightnessControl.Setup(line, true);
						return;
					}
					brightnessControl.Setup(line, false);
					return;
				}
			}
			else if (brightnessControl)
			{
				global::UnityEngine.Object.Destroy(brightnessControl);
			}
		}

		// Token: 0x06001A84 RID: 6788 RVA: 0x00090364 File Offset: 0x0008E564
		private static void ResetLinePoints(VisibilityControlStatic vcs, VectorLine line)
		{
			Matrix4x4 inverse = vcs.GetMatrix().inverse;
			for (int i = 0; i < line.points3.Count; i++)
			{
				line.points3[i] = inverse.MultiplyPoint3x4(line.points3[i]);
			}
		}

		// Token: 0x17000396 RID: 918
		// (get) Token: 0x06001A85 RID: 6789 RVA: 0x000903B5 File Offset: 0x0008E5B5
		public static int arrayCount
		{
			get
			{
				return VectorManager._arrayCount;
			}
		}

		// Token: 0x06001A86 RID: 6790 RVA: 0x000903BC File Offset: 0x0008E5BC
		public static void VisibilityStaticSetup(VectorLine line, out RefInt objectNum)
		{
			if (VectorManager.vectorLines == null)
			{
				VectorManager.vectorLines = new List<VectorLine>();
				VectorManager.objectNumbers = new List<RefInt>();
			}
			line.drawTransform = null;
			VectorManager.vectorLines.Add(line);
			objectNum = new RefInt(VectorManager._arrayCount++);
			VectorManager.objectNumbers.Add(objectNum);
			VectorLine.LineManagerEnable();
		}

		// Token: 0x06001A87 RID: 6791 RVA: 0x0009041C File Offset: 0x0008E61C
		public static void VisibilityStaticRemove(int objectNumber)
		{
			if (objectNumber >= VectorManager.vectorLines.Count)
			{
				Debug.LogError("VectorManager: object number exceeds array length in VisibilityStaticRemove");
				return;
			}
			for (int i = objectNumber + 1; i < VectorManager._arrayCount; i++)
			{
				VectorManager.objectNumbers[i].i--;
			}
			VectorManager.vectorLines.RemoveAt(objectNumber);
			VectorManager.objectNumbers.RemoveAt(objectNumber);
			VectorManager._arrayCount--;
			VectorLine.LineManagerDisable();
		}

		// Token: 0x17000397 RID: 919
		// (get) Token: 0x06001A88 RID: 6792 RVA: 0x00090492 File Offset: 0x0008E692
		public static int arrayCount2
		{
			get
			{
				return VectorManager._arrayCount2;
			}
		}

		// Token: 0x06001A89 RID: 6793 RVA: 0x0009049C File Offset: 0x0008E69C
		public static void VisibilitySetup(Transform thisTransform, VectorLine line, out RefInt objectNum)
		{
			if (VectorManager.vectorLines2 == null)
			{
				VectorManager.vectorLines2 = new List<VectorLine>();
				VectorManager.objectNumbers2 = new List<RefInt>();
			}
			line.drawTransform = thisTransform;
			VectorManager.vectorLines2.Add(line);
			objectNum = new RefInt(VectorManager._arrayCount2++);
			VectorManager.objectNumbers2.Add(objectNum);
			VectorLine.LineManagerEnable();
		}

		// Token: 0x06001A8A RID: 6794 RVA: 0x000904FC File Offset: 0x0008E6FC
		public static void VisibilityRemove(int objectNumber)
		{
			if (objectNumber >= VectorManager.vectorLines2.Count)
			{
				Debug.LogError("VectorManager: object number exceeds array length in VisibilityRemove");
				return;
			}
			for (int i = objectNumber + 1; i < VectorManager._arrayCount2; i++)
			{
				VectorManager.objectNumbers2[i].i--;
			}
			VectorManager.vectorLines2.RemoveAt(objectNumber);
			VectorManager.objectNumbers2.RemoveAt(objectNumber);
			VectorManager._arrayCount2--;
			VectorLine.LineManagerDisable();
		}

		// Token: 0x06001A8B RID: 6795 RVA: 0x00090574 File Offset: 0x0008E774
		public static void CheckDistanceSetup(Transform thisTransform, VectorLine line, Color color, RefInt objectNum)
		{
			VectorLine.LineManagerEnable();
			if (VectorManager.vectorLines3 == null)
			{
				VectorManager.vectorLines3 = new List<VectorLine>();
				VectorManager.transforms3 = new List<Transform>();
				VectorManager.oldDistances = new List<int>();
				VectorManager.colors = new List<Color>();
				VectorManager.objectNumbers3 = new List<RefInt>();
				VectorLine.LineManagerCheckDistance();
			}
			VectorManager.transforms3.Add(thisTransform);
			VectorManager.vectorLines3.Add(line);
			VectorManager.oldDistances.Add(-1);
			VectorManager.colors.Add(color);
			objectNum.i = VectorManager._arrayCount3++;
			VectorManager.objectNumbers3.Add(objectNum);
		}

		// Token: 0x06001A8C RID: 6796 RVA: 0x00090610 File Offset: 0x0008E810
		public static void DistanceRemove(int objectNumber)
		{
			if (objectNumber >= VectorManager.vectorLines3.Count)
			{
				Debug.LogError("VectorManager: object number exceeds array length in DistanceRemove");
				return;
			}
			for (int i = objectNumber + 1; i < VectorManager._arrayCount3; i++)
			{
				VectorManager.objectNumbers3[i].i--;
			}
			VectorManager.transforms3.RemoveAt(objectNumber);
			VectorManager.vectorLines3.RemoveAt(objectNumber);
			VectorManager.oldDistances.RemoveAt(objectNumber);
			VectorManager.colors.RemoveAt(objectNumber);
			VectorManager.objectNumbers3.RemoveAt(objectNumber);
			VectorManager._arrayCount3--;
		}

		// Token: 0x06001A8D RID: 6797 RVA: 0x000906A4 File Offset: 0x0008E8A4
		public static void CheckDistance()
		{
			for (int i = 0; i < VectorManager._arrayCount3; i++)
			{
				VectorManager.SetDistanceColor(i);
			}
		}

		// Token: 0x06001A8E RID: 6798 RVA: 0x000906C7 File Offset: 0x0008E8C7
		public static void SetOldDistance(int objectNumber, int val)
		{
			VectorManager.oldDistances[objectNumber] = val;
		}

		// Token: 0x06001A8F RID: 6799 RVA: 0x000906D8 File Offset: 0x0008E8D8
		public static void SetDistanceColor(int i)
		{
			if (!VectorManager.vectorLines3[i].active)
			{
				return;
			}
			float brightnessValue = VectorManager.GetBrightnessValue(VectorManager.transforms3[i].position);
			int num = (int)(brightnessValue * (float)VectorManager.brightnessLevels);
			if (num != VectorManager.oldDistances[i])
			{
				VectorManager.vectorLines3[i].SetColor(Color.Lerp(VectorManager.fogColor, VectorManager.colors[i], brightnessValue));
			}
			VectorManager.oldDistances[i] = num;
		}

		// Token: 0x06001A90 RID: 6800 RVA: 0x00090758 File Offset: 0x0008E958
		public static void DrawArrayLine(int i)
		{
			if (VectorManager.useDraw3D)
			{
				VectorManager.vectorLines[i].Draw3D();
				return;
			}
			VectorManager.vectorLines[i].Draw();
		}

		// Token: 0x06001A91 RID: 6801 RVA: 0x00090782 File Offset: 0x0008E982
		public static void DrawArrayLine2(int i)
		{
			if (VectorManager.useDraw3D)
			{
				VectorManager.vectorLines2[i].Draw3D();
				return;
			}
			VectorManager.vectorLines2[i].Draw();
		}

		// Token: 0x06001A92 RID: 6802 RVA: 0x000907AC File Offset: 0x0008E9AC
		public static void DrawArrayLines()
		{
			if (VectorManager.useDraw3D)
			{
				for (int i = 0; i < VectorManager._arrayCount; i++)
				{
					VectorManager.vectorLines[i].Draw3D();
				}
				return;
			}
			for (int j = 0; j < VectorManager._arrayCount; j++)
			{
				VectorManager.vectorLines[j].Draw();
			}
		}

		// Token: 0x06001A93 RID: 6803 RVA: 0x00090804 File Offset: 0x0008EA04
		public static void DrawArrayLines2()
		{
			if (VectorManager.useDraw3D)
			{
				for (int i = 0; i < VectorManager._arrayCount2; i++)
				{
					VectorManager.vectorLines2[i].Draw3D();
				}
				return;
			}
			for (int j = 0; j < VectorManager._arrayCount2; j++)
			{
				VectorManager.vectorLines2[j].Draw();
			}
		}

		// Token: 0x06001A94 RID: 6804 RVA: 0x0009085C File Offset: 0x0008EA5C
		public static Bounds GetBounds(VectorLine line)
		{
			if (line.points3 == null)
			{
				Debug.LogError("VectorManager: GetBounds can only be used with a Vector3 array");
				return default(Bounds);
			}
			return VectorManager.GetBounds(line.points3);
		}

		// Token: 0x06001A95 RID: 6805 RVA: 0x00090890 File Offset: 0x0008EA90
		public static Bounds GetBounds(List<Vector3> points3)
		{
			Bounds bounds = default(Bounds);
			Vector3 vector = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
			Vector3 vector2 = new Vector3(float.MinValue, float.MinValue, float.MinValue);
			int count = points3.Count;
			for (int i = 0; i < count; i++)
			{
				if (points3[i].x < vector.x)
				{
					vector.x = points3[i].x;
				}
				else if (points3[i].x > vector2.x)
				{
					vector2.x = points3[i].x;
				}
				if (points3[i].y < vector.y)
				{
					vector.y = points3[i].y;
				}
				else if (points3[i].y > vector2.y)
				{
					vector2.y = points3[i].y;
				}
				if (points3[i].z < vector.z)
				{
					vector.z = points3[i].z;
				}
				else if (points3[i].z > vector2.z)
				{
					vector2.z = points3[i].z;
				}
			}
			bounds.min = vector;
			bounds.max = vector2;
			return bounds;
		}

		// Token: 0x06001A96 RID: 6806 RVA: 0x000909FC File Offset: 0x0008EBFC
		private static Mesh MakeBoundsMesh(Bounds bounds)
		{
			return new Mesh
			{
				vertices = new Vector3[]
				{
					bounds.center + new Vector3(-bounds.extents.x, bounds.extents.y, bounds.extents.z),
					bounds.center + new Vector3(bounds.extents.x, bounds.extents.y, bounds.extents.z),
					bounds.center + new Vector3(-bounds.extents.x, bounds.extents.y, -bounds.extents.z),
					bounds.center + new Vector3(bounds.extents.x, bounds.extents.y, -bounds.extents.z),
					bounds.center + new Vector3(-bounds.extents.x, -bounds.extents.y, bounds.extents.z),
					bounds.center + new Vector3(bounds.extents.x, -bounds.extents.y, bounds.extents.z),
					bounds.center + new Vector3(-bounds.extents.x, -bounds.extents.y, -bounds.extents.z),
					bounds.center + new Vector3(bounds.extents.x, -bounds.extents.y, -bounds.extents.z)
				}
			};
		}

		// Token: 0x06001A97 RID: 6807 RVA: 0x00090C08 File Offset: 0x0008EE08
		public static void SetupBoundsMesh(GameObject go, VectorLine line)
		{
			MeshFilter meshFilter = go.GetComponent<MeshFilter>();
			if (meshFilter == null)
			{
				meshFilter = go.AddComponent<MeshFilter>();
			}
			MeshRenderer meshRenderer = go.GetComponent<MeshRenderer>();
			if (meshRenderer == null)
			{
				meshRenderer = go.AddComponent<MeshRenderer>();
			}
			meshRenderer.enabled = true;
			if (VectorManager.meshTable == null)
			{
				VectorManager.meshTable = new Dictionary<string, Mesh>();
			}
			if (!VectorManager.meshTable.ContainsKey(line.name))
			{
				VectorManager.meshTable.Add(line.name, VectorManager.MakeBoundsMesh(VectorManager.GetBounds(line)));
				VectorManager.meshTable[line.name].name = line.name + " Bounds";
			}
			meshFilter.mesh = VectorManager.meshTable[line.name];
		}

		// Token: 0x040016AB RID: 5803
		public static float minBrightnessDistance = 500f;

		// Token: 0x040016AC RID: 5804
		public static float maxBrightnessDistance = 250f;

		// Token: 0x040016AD RID: 5805
		private static int brightnessLevels = 32;

		// Token: 0x040016AE RID: 5806
		public static float distanceCheckFrequency = 0.2f;

		// Token: 0x040016AF RID: 5807
		private static Color fogColor;

		// Token: 0x040016B0 RID: 5808
		public static bool useDraw3D = false;

		// Token: 0x040016B1 RID: 5809
		private static List<VectorLine> vectorLines;

		// Token: 0x040016B2 RID: 5810
		private static List<RefInt> objectNumbers;

		// Token: 0x040016B3 RID: 5811
		public static int _arrayCount = 0;

		// Token: 0x040016B4 RID: 5812
		private static List<VectorLine> vectorLines2;

		// Token: 0x040016B5 RID: 5813
		private static List<RefInt> objectNumbers2;

		// Token: 0x040016B6 RID: 5814
		private static int _arrayCount2 = 0;

		// Token: 0x040016B7 RID: 5815
		private static List<Transform> transforms3;

		// Token: 0x040016B8 RID: 5816
		private static List<VectorLine> vectorLines3;

		// Token: 0x040016B9 RID: 5817
		private static List<int> oldDistances;

		// Token: 0x040016BA RID: 5818
		private static List<Color> colors;

		// Token: 0x040016BB RID: 5819
		private static List<RefInt> objectNumbers3;

		// Token: 0x040016BC RID: 5820
		private static int _arrayCount3 = 0;

		// Token: 0x040016BD RID: 5821
		private static Dictionary<string, Mesh> meshTable;
	}
}
