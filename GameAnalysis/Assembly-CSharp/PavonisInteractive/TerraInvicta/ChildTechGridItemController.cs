using System;
using System.Collections;
using System.Collections.Generic;
using ModelShark;
using PavonisInteractive.TerraInvicta.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Vectrosity;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020008BF RID: 2239
	public class ChildTechGridItemController : MonoBehaviour
	{
		// Token: 0x06005585 RID: 21893 RVA: 0x0026E2A0 File Offset: 0x0026C4A0
		public void Init(ResearchScreenController controller, TIGenericTechTemplate tech)
		{
			this.controller = controller;
			this.tech = tech;
			this.techNameString = tech.displayName;
		}

		// Token: 0x06005586 RID: 21894 RVA: 0x0026E2BC File Offset: 0x0026C4BC
		public void UpdateGridItem()
		{
			base.gameObject.name = this.techNameString;
			if (!TIGlobalResearchState.GetAllTechs().Contains(this.tech as TITechTemplate))
			{
				if (this.projectIcon != null)
				{
					this.projectIconObject.SetActive(true);
				}
			}
			else if (this.projectIcon != null)
			{
				this.projectIconObject.SetActive(false);
			}
			this.gradientImage.sprite = this.controller.techStatusGradient[ResearchScreenController.GetTechStatusAppearanceIndex(this.tech, this.controller.activePlayer)];
			ResearchScreenController.ShowTech(this.controller.activePlayer, this.tech, base.gameObject, this.techName, this.techStatus, this.techIcon);
			float num = 1f;
			if (this.tech.ref_project != null)
			{
				num = this.controller.activePlayer.GetProjectUnlockChance(this.tech.ref_project, this.controller.activePlayer.TechContributionBonus(this.tech.ref_project)) / 100f;
			}
			int techStatusAppearanceIndex = ResearchScreenController.GetTechStatusAppearanceIndex(this.tech, this.controller.activePlayer);
			this.lockIcon.color = this.controller.techStatusIconColors[techStatusAppearanceIndex];
			this.xIcon.color = this.controller.techStatusIconColors[techStatusAppearanceIndex];
			this.checkIcon.color = this.controller.techStatusIconColors[techStatusAppearanceIndex];
			this.lockIcon.gameObject.SetActive(false);
			this.targetHighlightObject.SetActive(this.controller.activePlayer.longtermTechTarget == this.tech.dataName);
			this.techUnlockOrProgressText.gameObject.SetActive(false);
			this.techUnlockOrProgressText.SetText(num.ToPercent("P0"));
			if (techStatusAppearanceIndex == 1)
			{
				this.techUnlockOrProgressText.gameObject.SetActive(true);
				TIProjectTemplate tiprojectTemplate = this.tech as TIProjectTemplate;
				if (tiprojectTemplate != null)
				{
					float projectProgressValueByTemplateFraction = this.controller.activePlayer.GetProjectProgressValueByTemplateFraction(tiprojectTemplate);
					this.techUnlockOrProgressText.SetText(projectProgressValueByTemplateFraction.ToPercent("P0"));
				}
			}
			if (techStatusAppearanceIndex == 5)
			{
				this.techUnlockOrProgressText.gameObject.SetActive(true);
				float accumulatedResearchByTech = TIGlobalResearchState.GetAccumulatedResearchByTech(this.tech as TITechTemplate);
				this.techUnlockOrProgressText.SetText((accumulatedResearchByTech / this.tech.GetResearchCost(this.controller.activePlayer)).ToPercent("P0"));
			}
			if (techStatusAppearanceIndex == 10 || techStatusAppearanceIndex == 11)
			{
				this.lockIcon.gameObject.SetActive(true);
				this.techUnlockOrProgressText.gameObject.SetActive(true);
				this.techStatus.SetText("");
				this.techUnlockOrProgressText.SetText(num.ToPercent("P0"));
			}
			if (techStatusAppearanceIndex == 2 || techStatusAppearanceIndex == 6 || techStatusAppearanceIndex == 9 || techStatusAppearanceIndex == 7)
			{
				this.lockIcon.gameObject.SetActive(true);
			}
			if (techStatusAppearanceIndex == 0 || techStatusAppearanceIndex == 4 || techStatusAppearanceIndex == 8)
			{
				this.checkIcon.gameObject.SetActive(true);
				this.techUnlockOrProgressText.gameObject.SetActive(true);
			}
			else
			{
				this.checkIcon.gameObject.SetActive(false);
			}
			this.xIcon.gameObject.SetActive(techStatusAppearanceIndex == 3 || techStatusAppearanceIndex == 7);
		}

		// Token: 0x06005587 RID: 21895 RVA: 0x0026E624 File Offset: 0x0026C824
		public void UpdateTooltip()
		{
			this.controller.activePlayer.SetCachedTechTooltipString(this.tech, false);
			this.toolTipString = this.controller.activePlayer.GetCachedTechTooltipString(this.tech);
			this.techTooltip.SetDelegate("BodyText", () => this.toolTipString);
		}

		// Token: 0x06005588 RID: 21896 RVA: 0x0026E680 File Offset: 0x0026C880
		public void OnGridItemClicked()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_ListLineSelect", false, false);
			this.controller.DisplayTechTree(this.tech);
		}

		// Token: 0x06005589 RID: 21897 RVA: 0x0026E69F File Offset: 0x0026C89F
		public void OnClickFullTechItem(bool baseTechOverride)
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericSelect", false, false);
			this.SelectFullTechItem(baseTechOverride);
		}

		// Token: 0x0600558A RID: 21898 RVA: 0x0026E6B4 File Offset: 0x0026C8B4
		public void SelectFullTechItem(bool baseTechOverride = false)
		{
			if (!baseTechOverride)
			{
				this.controller.UpdateSelectedTechPanel(this.tech, this);
			}
			if (!baseTechOverride)
			{
				this.controller.ResetAllConnectionColors();
				this.controller.selectedFullTech = base.gameObject;
			}
			this.techName.color = this.controller.techNameColorSelected;
			this.selected = true;
			this.borderHighlightObject.SetActive(true);
			foreach (GameObject gameObject in this.connectionLines)
			{
				if (!baseTechOverride)
				{
					this.ToggleActiveLineColor(gameObject, true, true, false);
				}
				if (baseTechOverride)
				{
					this.ToggleActiveLineColor(gameObject, false, true, true);
				}
			}
			if (this.connectionLines.Count == 0)
			{
				foreach (GameObject gameObject2 in this.enablesList)
				{
					gameObject2.GetComponent<ChildTechGridItemController>().SelectFullTechItem(true);
				}
			}
			this.cachedColors.Clear();
		}

		// Token: 0x0600558B RID: 21899 RVA: 0x0026E7DC File Offset: 0x0026C9DC
		public void OnMouseEnter()
		{
		}

		// Token: 0x0600558C RID: 21900 RVA: 0x0026E7DE File Offset: 0x0026C9DE
		public void OnMouseExit()
		{
		}

		// Token: 0x0600558D RID: 21901 RVA: 0x0026E7E0 File Offset: 0x0026C9E0
		public void OnRightClickTechItem()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
			this.OpenSelectiveTechTreeFromRightClick();
		}

		// Token: 0x0600558E RID: 21902 RVA: 0x0026E7F4 File Offset: 0x0026C9F4
		private void OpenSelectiveTechTreeFromRightClick()
		{
			if (!this.controller.selectiveTechTreeCanvas.enabled && !this.controller.openingSelectiveTree)
			{
				this.controller.openingSelectiveTree = true;
				this.controller.controllerForSelectiveTree = this;
				this.SelectFullTechItem(false);
				base.StartCoroutine(this.OpenSelectiveTechTree());
			}
		}

		// Token: 0x0600558F RID: 21903 RVA: 0x0026E84C File Offset: 0x0026CA4C
		public IEnumerator OpenSelectiveTechTree()
		{
			yield return new WaitForSeconds(0.5f);
			bool enabled = this.controller.fullTechTreeCanvas.enabled;
			this.controller.InitializeSelectiveTechTree(this.tech.dataName, this.tech.displayName, enabled);
			yield break;
		}

		// Token: 0x06005590 RID: 21904 RVA: 0x0026E85B File Offset: 0x0026CA5B
		public void ToggleActiveLineColor(GameObject connection, bool downstream = true, bool upstream = true, bool baseTechOverride = false)
		{
			if (base.gameObject.activeSelf)
			{
				base.StartCoroutine(this.SetOrangeLineColor(connection, downstream, upstream, baseTechOverride));
			}
		}

		// Token: 0x06005591 RID: 21905 RVA: 0x0026E87C File Offset: 0x0026CA7C
		public IEnumerator SetOrangeLineColor(GameObject connection, bool downstream = true, bool upstream = true, bool baseTechOverride = false)
		{
			yield return null;
			GameObject preTech = connection.GetComponent<TechTreeConnection>().preTech;
			TIGenericTechTemplate tigenericTechTemplate = preTech.GetComponent<ChildTechGridItemController>().tech;
			VectorObject2D component = connection.GetComponent<VectorObject2D>();
			VectorLine vectorLine = component.vectorLine;
			if (downstream)
			{
				if ((TIGlobalResearchState.TechFinished(tigenericTechTemplate.ref_tech) || (((tigenericTechTemplate != null) ? tigenericTechTemplate.ref_project : null) != null && (GameControl.control.activePlayer.completedProjects.Contains(tigenericTechTemplate.ref_project) || (tigenericTechTemplate.ref_project.oneTimeGlobally && tigenericTechTemplate.ref_project.SomeoneHasDoneIt())))) && !baseTechOverride)
				{
					vectorLine.SetColor(this.controller.connectionColorResearched);
				}
				else if (preTech == this.altPrereq0 || preTech == this.altPrereq1)
				{
					vectorLine.SetColor(this.controller.connectionColorOrPrereq);
				}
				else if ((this.altPrereq0 != null && preTech == this.prereqList[0]) || (this.altPrereq1 != null && preTech == this.prereqList[1]))
				{
					vectorLine.SetColor(this.controller.connectionColorOrPrereq);
				}
				else
				{
					vectorLine.SetColor(this.controller.connectionColorDownstream);
				}
			}
			if (upstream && !downstream)
			{
				if (TIGlobalResearchState.TechFinished(tigenericTechTemplate.ref_tech) && !baseTechOverride)
				{
					vectorLine.SetColor(this.controller.connectionColorResearched);
				}
				else
				{
					vectorLine.SetColor(this.controller.connectionColorUpstream);
				}
			}
			this.borderHighlightObject.SetActive(true);
			vectorLine.SetWidth(this.controller.highlightedConnectionLineWidth);
			component.raycastTarget = false;
			vectorLine.Draw();
			if (downstream)
			{
				this.selected = true;
				connection.GetComponent<TechTreeConnection>().preTech.GetComponent<ChildTechGridItemController>().techName.color = this.controller.techNameColorSelected;
				vectorLine.SetWidth(this.controller.highlightedConnectionLineWidth);
				if (connection.GetComponent<TechTreeConnection>().preTech.GetComponent<ChildTechGridItemController>().connectionLines.Count > 0)
				{
					using (List<GameObject>.Enumerator enumerator = this.connectionLines.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							GameObject gameObject = enumerator.Current;
							TechTreeConnection component2 = gameObject.GetComponent<TechTreeConnection>();
							if (component2.preTech.GetComponent<ChildTechGridItemController>().connectionLines.Count > 0)
							{
								foreach (GameObject gameObject2 in component2.preTech.GetComponent<ChildTechGridItemController>().connectionLines)
								{
									this.selected = true;
									if (component2.preTech != this.controller.selectedFullTech)
									{
										component2.preTech.GetComponent<ChildTechGridItemController>().techName.color = this.controller.techNameColorSelected;
									}
									component2.preTech.GetComponent<ChildTechGridItemController>().SetOrangeLineColor(gameObject2, true, false, false);
									gameObject2.GetComponent<TechTreeConnection>().endTech.GetComponent<ChildTechGridItemController>().ToggleActiveLineColor(gameObject2, true, false, false);
								}
							}
						}
						goto IL_03BD;
					}
				}
				connection.GetComponent<TechTreeConnection>().preTech.GetComponent<ChildTechGridItemController>().borderHighlightObject.SetActive(true);
			}
			IL_03BD:
			if (upstream)
			{
				using (List<GameObject>.Enumerator enumerator = this.enablesList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						GameObject gameObject3 = enumerator.Current;
						foreach (GameObject gameObject4 in gameObject3.GetComponent<ChildTechGridItemController>().connectionLines)
						{
							VectorObject2D component3 = gameObject4.GetComponent<VectorObject2D>();
							TechTreeConnection component4 = gameObject4.GetComponent<TechTreeConnection>();
							if (component4.preTech == base.gameObject)
							{
								this.selected = true;
								if (component4.endTech != this.controller.selectedFullTech)
								{
									component4.endTech.GetComponent<ChildTechGridItemController>().techName.color = this.controller.techNameColorSelected;
								}
								component3.raycastTarget = false;
								component3.vectorLine.SetColor(this.controller.connectionColorUpstream);
								component3.vectorLine.SetWidth(this.controller.highlightedConnectionLineWidth);
								component3.vectorLine.Draw();
								if (gameObject3.GetComponent<ChildTechGridItemController>().enablesList.Count > 0)
								{
									using (List<GameObject>.Enumerator enumerator3 = gameObject3.GetComponent<ChildTechGridItemController>().enablesList.GetEnumerator())
									{
										while (enumerator3.MoveNext())
										{
											GameObject gameObject5 = enumerator3.Current;
											foreach (GameObject gameObject6 in gameObject5.GetComponent<ChildTechGridItemController>().connectionLines)
											{
												TechTreeConnection component5 = gameObject6.GetComponent<TechTreeConnection>();
												if (component5.preTech == gameObject3.gameObject && !component5.dirty)
												{
													gameObject3.GetComponent<ChildTechGridItemController>().ToggleActiveLineColor(gameObject6, false, true, false);
													component5.dirty = true;
												}
											}
										}
										continue;
									}
								}
								gameObject3.GetComponent<ChildTechGridItemController>().borderHighlightObject.SetActive(true);
							}
						}
					}
					yield break;
				}
			}
			yield break;
		}

		// Token: 0x06005592 RID: 21906 RVA: 0x0026E8A8 File Offset: 0x0026CAA8
		public void ResetLineColors()
		{
			this.selected = false;
			this.techName.color = this.controller.techNameColorDeSelected;
			this.borderHighlightObject.SetActive(false);
			foreach (GameObject gameObject in this.connectionLines)
			{
				VectorObject2D component = gameObject.GetComponent<VectorObject2D>();
				component.vectorLine.SetColor(this.controller.connectionColorDeSelected);
				component.vectorLine.SetWidth(this.controller.normalConnectionLineWidth);
				component.raycastTarget = false;
				component.vectorLine.Draw();
				gameObject.GetComponent<TechTreeConnection>().dirty = false;
			}
		}

		// Token: 0x06005593 RID: 21907 RVA: 0x0026E974 File Offset: 0x0026CB74
		public void ClearConnections()
		{
			this.connectionLines.Clear();
		}

		// Token: 0x06005594 RID: 21908 RVA: 0x0026E981 File Offset: 0x0026CB81
		public void SetConnection(Vector2 targetPoint, GameObject targetObject)
		{
			base.StartCoroutine(this.DrawLine(targetPoint, targetObject, false));
			this.connectionTarget = targetPoint;
			this.connectionInit = true;
		}

		// Token: 0x06005595 RID: 21909 RVA: 0x0026E9A1 File Offset: 0x0026CBA1
		public IEnumerator DrawLine(Vector2 targetPoint, GameObject targetObject, bool categoryColor = false)
		{
			yield return null;
			GameObject gameObject = global::UnityEngine.Object.Instantiate<GameObject>(this.lineObjectPrefab, base.transform, true);
			gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
			gameObject.transform.SetAsFirstSibling();
			VectorObject2D component = gameObject.GetComponent<VectorObject2D>();
			VectorLine vectorLine = component.vectorLine;
			vectorLine.SetWidth(this.controller.normalConnectionLineWidth);
			vectorLine.points2[0] = new Vector2(-(base.GetComponent<RectTransform>().sizeDelta.x / 2f * this.controller.Canvas.GetComponent<RectTransform>().localScale.x), 0f);
			vectorLine.points2[1] = targetPoint;
			if (categoryColor)
			{
				vectorLine.color = TemplateManager.global.techColor[(int)this.tech.techCategory];
			}
			vectorLine.color = this.controller.connectionColorDeSelected;
			component.raycastTarget = false;
			vectorLine.Draw();
			gameObject.transform.SetParent(this.controller.currentPrereqLineContainer.transform);
			this.connectionLines.Add(gameObject);
			this.connectionsTarget.Add(targetPoint);
			TechTreeConnection component2 = gameObject.GetComponent<TechTreeConnection>();
			component2.preTech = targetObject;
			component2.endTech = base.gameObject;
			gameObject.gameObject.name = targetObject.name + "_" + base.gameObject.name;
			if (!this.showLines)
			{
				gameObject.SetActive(false);
			}
			yield break;
		}

		// Token: 0x04003BDF RID: 15327
		private ResearchScreenController controller;

		// Token: 0x04003BE0 RID: 15328
		public string techNameString;

		// Token: 0x04003BE1 RID: 15329
		public TIGenericTechTemplate tech;

		// Token: 0x04003BE2 RID: 15330
		public TMP_Text techName;

		// Token: 0x04003BE3 RID: 15331
		public TMP_Text techStatus;

		// Token: 0x04003BE4 RID: 15332
		public TMP_Text techUnlockOrProgressText;

		// Token: 0x04003BE5 RID: 15333
		public Image gradientImage;

		// Token: 0x04003BE6 RID: 15334
		public Image techIcon;

		// Token: 0x04003BE7 RID: 15335
		public Image lockIcon;

		// Token: 0x04003BE8 RID: 15336
		public Image projectIcon;

		// Token: 0x04003BE9 RID: 15337
		public Image checkIcon;

		// Token: 0x04003BEA RID: 15338
		public Image xIcon;

		// Token: 0x04003BEB RID: 15339
		public GameObject borderHighlightObject;

		// Token: 0x04003BEC RID: 15340
		public GameObject targetHighlightObject;

		// Token: 0x04003BED RID: 15341
		public GameObject lineObjectPrefab;

		// Token: 0x04003BEE RID: 15342
		public GameObject projectIconObject;

		// Token: 0x04003BEF RID: 15343
		public TooltipTrigger techTooltip;

		// Token: 0x04003BF0 RID: 15344
		public string toolTipString;

		// Token: 0x04003BF1 RID: 15345
		public int node;

		// Token: 0x04003BF2 RID: 15346
		public bool visited;

		// Token: 0x04003BF3 RID: 15347
		public List<GameObject> connectionLines = new List<GameObject>();

		// Token: 0x04003BF4 RID: 15348
		public List<GameObject> enablesList = new List<GameObject>();

		// Token: 0x04003BF5 RID: 15349
		public List<GameObject> prereqList = new List<GameObject>();

		// Token: 0x04003BF6 RID: 15350
		public GameObject altPrereq0;

		// Token: 0x04003BF7 RID: 15351
		public GameObject altPrereq1;

		// Token: 0x04003BF8 RID: 15352
		public List<Vector2> connectionsTarget = new List<Vector2>();

		// Token: 0x04003BF9 RID: 15353
		public List<string> connectionList;

		// Token: 0x04003BFA RID: 15354
		public bool connectionInit;

		// Token: 0x04003BFB RID: 15355
		public Vector2 connectionTarget;

		// Token: 0x04003BFC RID: 15356
		public float prereqY;

		// Token: 0x04003BFD RID: 15357
		public bool hidden;

		// Token: 0x04003BFE RID: 15358
		public bool showLines = true;

		// Token: 0x04003BFF RID: 15359
		public bool imageLoaded;

		// Token: 0x04003C00 RID: 15360
		public bool selected;

		// Token: 0x04003C01 RID: 15361
		public List<Color> cachedColors;
	}
}
