using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020008FB RID: 2299
	public class GraphUI : MonoBehaviour
	{
		// Token: 0x17000F26 RID: 3878
		// (get) Token: 0x060057FE RID: 22526 RVA: 0x00285ADF File Offset: 0x00283CDF
		private IEnumerable<TIGameState> OrderedTargets
		{
			get
			{
				return TIHistoricalData.States;
			}
		}

		// Token: 0x17000F27 RID: 3879
		// (get) Token: 0x060057FF RID: 22527 RVA: 0x00285AE6 File Offset: 0x00283CE6
		private IEnumerable<string> OrderedAttributes
		{
			get
			{
				if (!(this.Target != null))
				{
					return Enumerable.Empty<string>();
				}
				return TIHistoricalData.GetAttributes(this.Target);
			}
		}

		// Token: 0x17000F28 RID: 3880
		// (get) Token: 0x06005800 RID: 22528 RVA: 0x00285B07 File Offset: 0x00283D07
		public TIGameState Target
		{
			get
			{
				return this.OrderedTargets.ToList<TIGameState>().ElementAtOrDefault<TIGameState>(this.TargetDropDown.value);
			}
		}

		// Token: 0x17000F29 RID: 3881
		// (get) Token: 0x06005801 RID: 22529 RVA: 0x00285B24 File Offset: 0x00283D24
		public string Attribute
		{
			get
			{
				return this.OrderedAttributes.ToList<string>()[this.AttributeDropdown.value];
			}
		}

		// Token: 0x06005802 RID: 22530 RVA: 0x00285B44 File Offset: 0x00283D44
		private void Start()
		{
			this.XMarks = new List<GameObject>();
			for (int i = 0; i < this.XMarkResolution; i++)
			{
				if (i == 0)
				{
					this.XMarks.Add(this.XMark0);
				}
				else
				{
					this.XMarks.Add(global::UnityEngine.Object.Instantiate<GameObject>(this.XMark0));
				}
				this.XMarks[i].transform.SetParent(this.XMark0.transform.parent, false);
			}
			this.YMarks = new List<GameObject>();
			for (int j = 0; j < this.YMarkResolution; j++)
			{
				if (j == 0)
				{
					this.YMarks.Add(this.YMark0);
				}
				else
				{
					this.YMarks.Add(global::UnityEngine.Object.Instantiate<GameObject>(this.YMark0));
				}
				this.YMarks[j].transform.SetParent(this.YMark0.transform.parent, false);
			}
			foreach (GameObject gameObject in this.XMarks)
			{
				gameObject.transform.localPosition = new Vector3(this.XMarks[0].transform.localPosition.x + (float)(this.XStride * this.XMarks.IndexOf(gameObject)), this.XMarks[0].transform.localPosition.y);
			}
			foreach (GameObject gameObject2 in this.YMarks)
			{
				gameObject2.transform.localPosition = new Vector3(this.YMarks[0].transform.localPosition.x, this.YMarks[0].transform.localPosition.y + (float)(this.YStride * this.YMarks.IndexOf(gameObject2)));
			}
			this.SetLabels();
			this.Lines = new List<GameObject> { this.LinePrefab };
			for (int k = 1; k < this.Resolution - 1; k++)
			{
				GameObject gameObject3 = global::UnityEngine.Object.Instantiate<GameObject>(this.LinePrefab);
				gameObject3.transform.SetParent(this.LinePrefab.transform.parent, false);
				this.Lines.Add(gameObject3);
			}
		}

		// Token: 0x06005803 RID: 22531 RVA: 0x00285DD0 File Offset: 0x00283FD0
		private void FillTargetDropdown()
		{
			this.TargetDropDown.ClearOptions();
			this.TargetDropDown.AddOptions(this.OrderedTargets.Select<TIGameState, TMP_Dropdown.OptionData>((TIGameState x) => new TMP_Dropdown.OptionData(x.displayName)).ToList<TMP_Dropdown.OptionData>());
		}

		// Token: 0x06005804 RID: 22532 RVA: 0x00285E24 File Offset: 0x00284024
		private void FillAttributeDropdown()
		{
			this.AttributeDropdown.ClearOptions();
			this.AttributeDropdown.AddOptions(this.OrderedAttributes.Select<string, TMP_Dropdown.OptionData>((string x) => new TMP_Dropdown.OptionData(x)).ToList<TMP_Dropdown.OptionData>());
		}

		// Token: 0x06005805 RID: 22533 RVA: 0x00285E78 File Offset: 0x00284078
		private void SetLabels()
		{
			if (this.Target == null)
			{
				return;
			}
			this.YLabel.text = this.Attribute;
			foreach (GameObject gameObject in this.XMarks)
			{
				gameObject.GetComponentInChildren<TMP_Text>().text = TIHistoricalData.GetLerpDate(this.Target, this.Attribute, (float)this.XMarks.IndexOf(gameObject) / (float)(this.XMarks.Count - 1)).ToShortDateString();
			}
			ValueTuple<float, float> valueRange_Tight = TIHistoricalData.GetValueRange_Tight(this.Target, this.Attribute);
			float item = valueRange_Tight.Item1;
			float item2 = valueRange_Tight.Item2;
			foreach (GameObject gameObject2 in this.YMarks)
			{
				gameObject2.GetComponentInChildren<TMP_Text>().text = Mathf.Lerp(item, item2, (float)this.YMarks.IndexOf(gameObject2) / (float)(this.YMarks.Count - 1)).ToString("N2");
			}
		}

		// Token: 0x06005806 RID: 22534 RVA: 0x00285FBC File Offset: 0x002841BC
		private void Update()
		{
			if (this.OrderedTargets.Count<TIGameState>() != this.TargetDropDown.options.Count)
			{
				this.FillTargetDropdown();
			}
			if (this.OrderedAttributes.Count<string>() != this.AttributeDropdown.options.Count)
			{
				this.FillAttributeDropdown();
			}
			TIGameState target = this.Target;
			if (target == null)
			{
				return;
			}
			string attribute = this.Attribute;
			ValueTuple<float, float> valueRange_Tight = TIHistoricalData.GetValueRange_Tight(target, attribute);
			float item = valueRange_Tight.Item1;
			float item2 = valueRange_Tight.Item2;
			float num = -1f;
			float num2 = -1f;
			int i = 0;
			while (i < this.Resolution)
			{
				float num3 = (float)i / (float)this.Lines.Count;
				float num4 = (TIHistoricalData.Sample(target, attribute, num3) - item) / (item2 - item);
				if (i <= 0)
				{
					goto IL_02C3;
				}
				GameObject gameObject = this.Lines[i - 1];
				RectTransform rectTransform = gameObject.transform as RectTransform;
				Vector2 vector = new Vector2(this.XMarks.First<GameObject>().transform.localPosition.x + (float)((this.XMarks.Count - 1) * this.XStride) * num, this.YMarks.First<GameObject>().transform.localPosition.y + (float)((this.YMarks.Count - 1) * this.YStride) * num2);
				Vector2 vector2 = new Vector2(this.XMarks.First<GameObject>().transform.localPosition.x + (float)((this.XMarks.Count - 1) * this.XStride) * num3, this.YMarks.First<GameObject>().transform.localPosition.y + (float)((this.YMarks.Count - 1) * this.YStride) * num4);
				if (!float.IsNaN(vector.x) && !float.IsNaN(vector.y) && !float.IsNaN(vector2.x) && !float.IsNaN(vector2.y))
				{
					gameObject.transform.localPosition = Vector2.Lerp(gameObject.transform.localPosition, vector, Time.deltaTime * this.AnimationSpeed);
					float num5 = Vector3.SignedAngle(Vector2.right, vector2 - vector, Vector3.forward);
					gameObject.transform.rotation = Quaternion.Lerp(gameObject.transform.rotation, Quaternion.Euler(0f, 0f, num5), Time.deltaTime * this.AnimationSpeed);
					float num6 = Vector2.Distance(vector, vector2);
					Vector2 vector3 = new Vector2(num6, this.LineThickness);
					rectTransform.sizeDelta = Vector2.Lerp(rectTransform.sizeDelta, vector3, Time.deltaTime * this.AnimationSpeed);
					goto IL_02C3;
				}
				rectTransform.sizeDelta = Vector2.zero;
				IL_02CB:
				i++;
				continue;
				IL_02C3:
				num = num3;
				num2 = num4;
				goto IL_02CB;
			}
		}

		// Token: 0x06005807 RID: 22535 RVA: 0x002862A7 File Offset: 0x002844A7
		public void OnTargetChanged()
		{
			this.FillAttributeDropdown();
			this.AttributeDropdown.SetValueWithoutNotify(0);
			this.SetLabels();
		}

		// Token: 0x06005808 RID: 22536 RVA: 0x002862C1 File Offset: 0x002844C1
		public void OnAttributeChanged()
		{
			this.SetLabels();
		}

		// Token: 0x04003F7F RID: 16255
		public TMP_Dropdown TargetDropDown;

		// Token: 0x04003F80 RID: 16256
		public TMP_Dropdown AttributeDropdown;

		// Token: 0x04003F81 RID: 16257
		public TMP_Text YLabel;

		// Token: 0x04003F82 RID: 16258
		public GameObject XMark0;

		// Token: 0x04003F83 RID: 16259
		public GameObject YMark0;

		// Token: 0x04003F84 RID: 16260
		public int XMarkResolution;

		// Token: 0x04003F85 RID: 16261
		public int XStride;

		// Token: 0x04003F86 RID: 16262
		public int YMarkResolution;

		// Token: 0x04003F87 RID: 16263
		public int YStride;

		// Token: 0x04003F88 RID: 16264
		public int Resolution;

		// Token: 0x04003F89 RID: 16265
		public float LineThickness = 2f;

		// Token: 0x04003F8A RID: 16266
		public float AnimationSpeed = 4f;

		// Token: 0x04003F8B RID: 16267
		private List<GameObject> XMarks;

		// Token: 0x04003F8C RID: 16268
		private List<GameObject> YMarks;

		// Token: 0x04003F8D RID: 16269
		public GameObject LinePrefab;

		// Token: 0x04003F8E RID: 16270
		private List<GameObject> Lines;
	}
}
