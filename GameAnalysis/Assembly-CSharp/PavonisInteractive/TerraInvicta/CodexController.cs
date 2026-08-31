using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.CSharp.RuntimeBinder;
using PavonisInteractive.TerraInvicta.Audio;
using PavonisInteractive.TerraInvicta.Systems.UI;
using TMPro;
using Unity.Entities;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020008B8 RID: 2232
	public class CodexController : CanvasControllerBase
	{
		// Token: 0x06005537 RID: 21815 RVA: 0x0026B448 File Offset: 0x00269648
		public override void Initialize()
		{
			base.Initialize();
			this.codexSearchTitle.SetText(Loc.T("UI.GeneralControls.GlobalSearch"));
			this.allCodexEntries = TemplateManager.IterateByClass<TICodexEntryTemplate>(true).ToList<TICodexEntryTemplate>();
			this.allCodexEntries = this.allCodexEntries.OrderBy<TICodexEntryTemplate, float>((TICodexEntryTemplate o) => o.index).ToList<TICodexEntryTemplate>();
			this.codexTopicListManager.SetListSize<CodexTopicListItemController>(this.allCodexEntries.Count, false, false);
			this.TutorialButtonVisibility();
			int num = 0;
			using (IEnumerator<object> enumerator = this.codexTopicListManager.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (CodexController.<>o__12.<>p__0 == null)
					{
						CodexController.<>o__12.<>p__0 = CallSite<Func<CallSite, object, CodexTopicListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(CodexTopicListItemController), typeof(CodexController)));
					}
					CodexTopicListItemController codexTopicListItemController = CodexController.<>o__12.<>p__0.Target(CodexController.<>o__12.<>p__0, enumerator.Current);
					codexTopicListItemController.Init(this, this.allCodexEntries[num++]);
					bool flag = num == this.codexTopicListManager.size;
					bool flag2 = !flag && this.allCodexEntries[num].mainTopic;
					codexTopicListItemController.UpdateListItem(flag, flag2);
				}
			}
			base.gameObject.SetActive(true);
		}

		// Token: 0x06005538 RID: 21816 RVA: 0x0026B5A8 File Offset: 0x002697A8
		private void TutorialButtonVisibility()
		{
			this.resetTutorialButton.SetActive(TIGlobalValuesState.isTutorialActive);
		}

		// Token: 0x06005539 RID: 21817 RVA: 0x0026B5BA File Offset: 0x002697BA
		public void SelectTopic(string topic)
		{
			base.StartCoroutine(this.SelectTopicIE(topic));
		}

		// Token: 0x0600553A RID: 21818 RVA: 0x0026B5CC File Offset: 0x002697CC
		public void UpdateCodexSearch()
		{
			string text = this.codexSearch.text;
			text = TIUtilities.StripDiacriticsFromString(text);
			using (IEnumerator<object> enumerator = this.codexTopicListManager.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (CodexController.<>o__15.<>p__0 == null)
					{
						CodexController.<>o__15.<>p__0 = CallSite<Func<CallSite, object, CodexTopicListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(CodexTopicListItemController), typeof(CodexController)));
					}
					CodexTopicListItemController codexTopicListItemController = CodexController.<>o__15.<>p__0.Target(CodexController.<>o__15.<>p__0, enumerator.Current);
					if (text.Length < 2 || TIUtilities.StripDiacriticsFromString(codexTopicListItemController.template.titleText.ToLower()).Contains(text.ToLower()) || (this.codexSearchDictionary.ContainsKey(codexTopicListItemController.template) && TIUtilities.StripDiacriticsFromString(this.codexSearchDictionary[codexTopicListItemController.template]).ToLower().Contains(text.ToLower())))
					{
						codexTopicListItemController.gameObject.SetActive(true);
					}
					else
					{
						codexTopicListItemController.gameObject.SetActive(false);
					}
				}
			}
		}

		// Token: 0x0600553B RID: 21819 RVA: 0x0026B6EC File Offset: 0x002698EC
		public void BuildCodexSearchDictionary()
		{
			using (IEnumerator<object> enumerator = this.codexTopicListManager.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (CodexController.<>o__16.<>p__0 == null)
					{
						CodexController.<>o__16.<>p__0 = CallSite<Func<CallSite, object, CodexTopicListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(CodexTopicListItemController), typeof(CodexController)));
					}
					CodexTopicListItemController codexTopicListItemController = CodexController.<>o__16.<>p__0.Target(CodexController.<>o__16.<>p__0, enumerator.Current);
					if (!string.IsNullOrEmpty(codexTopicListItemController.template.templateToPull))
					{
						string templateToPull = codexTopicListItemController.template.templateToPull;
						if (templateToPull != null)
						{
							uint num = <PrivateImplementationDetails>.ComputeStringHash(templateToPull);
							if (num <= 1288716985U)
							{
								if (num <= 841690254U)
								{
									if (num != 398618724U)
									{
										if (num == 841690254U)
										{
											if (templateToPull == "TIOfficerTemplate")
											{
												this.HandleOfficerTypeTemplate(codexTopicListItemController.template, true);
												continue;
											}
										}
									}
									else if (templateToPull == "TITraitTemplate")
									{
										this.HandleTraitTemplate(codexTopicListItemController.template, true);
										continue;
									}
								}
								else if (num != 881470450U)
								{
									if (num == 1288716985U)
									{
										if (templateToPull == "TMissionTemplate_Alien")
										{
											this.HandleAlienMissionTemplate(codexTopicListItemController.template, true);
											continue;
										}
									}
								}
								else if (templateToPull == "TIArmyOperationTemplate")
								{
									this.HandleArmyOperationTemplate(codexTopicListItemController.template, true);
									continue;
								}
							}
							else if (num <= 2969098861U)
							{
								if (num != 2016828202U)
								{
									if (num == 2969098861U)
									{
										if (templateToPull == "TechCategory")
										{
											this.HandleTechCategoryList(codexTopicListItemController.template, true);
											continue;
										}
									}
								}
								else if (templateToPull == "TIPolicyTemplate")
								{
									this.HandlePolicyTemplate(codexTopicListItemController.template, true);
									continue;
								}
							}
							else if (num != 2985795757U)
							{
								if (num != 3594136090U)
								{
									if (num == 3798852350U)
									{
										if (templateToPull == "TICouncilorTypeTemplate")
										{
											this.HandleCouncilorTypeTemplate(codexTopicListItemController.template, true);
											continue;
										}
									}
								}
								else if (templateToPull == "TIMissionTemplate")
								{
									this.HandleMissionTemplate(codexTopicListItemController.template, true);
									continue;
								}
							}
							else if (templateToPull == "TIFleetOperationTemplate")
							{
								this.HandleFleetOperationTemplate(codexTopicListItemController.template, true);
								continue;
							}
						}
						Log.Warn("Bad template name " + codexTopicListItemController.template.templateToPull + " in " + codexTopicListItemController.template.dataName, Array.Empty<object>());
					}
				}
			}
		}

		// Token: 0x0600553C RID: 21820 RVA: 0x0026B9B0 File Offset: 0x00269BB0
		private void addSearchKey(TICodexEntryTemplate entry, string searchString)
		{
			if (this.codexSearchDictionary.ContainsKey(entry))
			{
				this.codexSearchDictionary.Remove(entry);
				this.codexSearchDictionary.Add(entry, searchString);
				return;
			}
			this.codexSearchDictionary.Add(entry, searchString);
		}

		// Token: 0x0600553D RID: 21821 RVA: 0x0026B9E8 File Offset: 0x00269BE8
		private IEnumerator SelectTopicIE(string topic)
		{
			if (topic == "")
			{
				topic = "codex_welcome";
			}
			if (this.selectedTopic != null && this.selectedTopic.dataName == TemplateManager.Find<TICodexEntryTemplate>(topic, false).dataName)
			{
				yield return null;
			}
			else
			{
				this.selectedTopic = TemplateManager.Find<TICodexEntryTemplate>(topic, false);
				if (this.selectedTopic == null)
				{
					Log.Warn("Couldn't find codex topic- " + topic, Array.Empty<object>());
					yield return null;
				}
				else
				{
					this.codexInfoListManager.gameObject.SetActive(false);
					this.infoLocCount = 0;
					if (!string.IsNullOrEmpty(this.selectedTopic.locPath))
					{
						int num = 0;
						while (num < 19 && Loc.T(this.selectedTopic.locPath + num.ToString()) != this.selectedTopic.locPath + num.ToString())
						{
							this.infoLocCount++;
							num++;
						}
					}
					if (!string.IsNullOrEmpty(this.selectedTopic.templateToPull))
					{
						string templateToPull = this.selectedTopic.templateToPull;
						if (templateToPull != null)
						{
							uint num2 = <PrivateImplementationDetails>.ComputeStringHash(templateToPull);
							if (num2 <= 1288716985U)
							{
								if (num2 <= 841690254U)
								{
									if (num2 != 398618724U)
									{
										if (num2 == 841690254U)
										{
											if (templateToPull == "TIOfficerTemplate")
											{
												this.HandleOfficerTypeTemplate(this.selectedTopic, false);
												goto IL_03BE;
											}
										}
									}
									else if (templateToPull == "TITraitTemplate")
									{
										this.HandleTraitTemplate(this.selectedTopic, false);
										goto IL_03BE;
									}
								}
								else if (num2 != 881470450U)
								{
									if (num2 == 1288716985U)
									{
										if (templateToPull == "TMissionTemplate_Alien")
										{
											this.HandleAlienMissionTemplate(this.selectedTopic, false);
											goto IL_03BE;
										}
									}
								}
								else if (templateToPull == "TIArmyOperationTemplate")
								{
									this.HandleArmyOperationTemplate(this.selectedTopic, false);
									goto IL_03BE;
								}
							}
							else if (num2 <= 2969098861U)
							{
								if (num2 != 2016828202U)
								{
									if (num2 == 2969098861U)
									{
										if (templateToPull == "TechCategory")
										{
											this.HandleTechCategoryList(this.selectedTopic, false);
											goto IL_03BE;
										}
									}
								}
								else if (templateToPull == "TIPolicyTemplate")
								{
									this.HandlePolicyTemplate(this.selectedTopic, false);
									goto IL_03BE;
								}
							}
							else if (num2 != 2985795757U)
							{
								if (num2 != 3594136090U)
								{
									if (num2 == 3798852350U)
									{
										if (templateToPull == "TICouncilorTypeTemplate")
										{
											this.HandleCouncilorTypeTemplate(this.selectedTopic, false);
											goto IL_03BE;
										}
									}
								}
								else if (templateToPull == "TIMissionTemplate")
								{
									this.HandleMissionTemplate(this.selectedTopic, false);
									goto IL_03BE;
								}
							}
							else if (templateToPull == "TIFleetOperationTemplate")
							{
								this.HandleFleetOperationTemplate(this.selectedTopic, false);
								goto IL_03BE;
							}
						}
						Log.Warn("Bad template name " + this.selectedTopic.templateToPull + " in " + this.selectedTopic.dataName, Array.Empty<object>());
					}
					else
					{
						this.codexInfoListManager.SetListSize<CodexInfoListItemController>(this.infoLocCount, true, false);
						this.DisplayMainCodexText();
					}
					IL_03BE:
					yield return null;
					using (IEnumerator<object> enumerator = this.codexInfoListManager.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if (CodexController.<>o__18.<>p__0 == null)
							{
								CodexController.<>o__18.<>p__0 = CallSite<Func<CallSite, object, CodexInfoListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(CodexInfoListItemController), typeof(CodexController)));
							}
							CodexController.<>o__18.<>p__0.Target(CodexController.<>o__18.<>p__0, enumerator.Current).gameObject.SetActive(true);
						}
					}
					this.codexInfoListManager.gameObject.SetActive(true);
				}
			}
			yield break;
		}

		// Token: 0x0600553E RID: 21822 RVA: 0x0026BA00 File Offset: 0x00269C00
		public void DisplayMainCodexText()
		{
			int num = 0;
			if (this.infoLocCount > 0)
			{
				using (IEnumerator<object> enumerator = this.codexInfoListManager.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (CodexController.<>o__19.<>p__0 == null)
						{
							CodexController.<>o__19.<>p__0 = CallSite<Func<CallSite, object, CodexInfoListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(CodexInfoListItemController), typeof(CodexController)));
						}
						CodexInfoListItemController codexInfoListItemController = CodexController.<>o__19.<>p__0.Target(CodexController.<>o__19.<>p__0, enumerator.Current);
						codexInfoListItemController.Init(this, this.selectedTopic, num);
						num++;
						codexInfoListItemController.UpdateListItem();
					}
				}
			}
		}

		// Token: 0x0600553F RID: 21823 RVA: 0x0026BAA8 File Offset: 0x00269CA8
		public void ShowCodex(string topic = "codex_welcome")
		{
			this.Show();
			this.SelectTopic(topic);
			int num = 0;
			using (IEnumerator<object> enumerator = this.codexTopicListManager.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (CodexController.<>o__20.<>p__0 == null)
					{
						CodexController.<>o__20.<>p__0 = CallSite<Func<CallSite, object, CodexTopicListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(CodexTopicListItemController), typeof(CodexController)));
					}
					CodexTopicListItemController codexTopicListItemController = CodexController.<>o__20.<>p__0.Target(CodexController.<>o__20.<>p__0, enumerator.Current);
					num++;
					codexTopicListItemController.UpdateListItem(num == this.codexTopicListManager.size, false);
				}
			}
			this.BuildCodexSearchDictionary();
		}

		// Token: 0x06005540 RID: 21824 RVA: 0x0026BB5C File Offset: 0x00269D5C
		public override void UpdateUIScaling()
		{
			base.UpdateUIScaling();
			this.primaryPanelTransform.anchoredPosition = new Vector2(0f, (float)((base.VerticalScaleValueLimit() >= 940f) ? (-100) : (-85)));
		}

		// Token: 0x06005541 RID: 21825 RVA: 0x0026BB8D File Offset: 0x00269D8D
		public void OnClickCloseCodex()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CloseLarge", false, false);
			this.HideCodex();
		}

		// Token: 0x06005542 RID: 21826 RVA: 0x0026BBA1 File Offset: 0x00269DA1
		public void HideCodex()
		{
			this.codexTutorialController.HideTutorial();
			this.Hide();
		}

		// Token: 0x06005543 RID: 21827 RVA: 0x0026BBB4 File Offset: 0x00269DB4
		public static void ShowCodexPanel(string topic = "codex_welcome")
		{
			CodexController codexController = World.Active.GetExistingManager<CanvasManager>().Codex as CodexController;
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_OpenLarge", false, false);
			codexController.ShowCodex(topic);
			codexController.codexTutorialController.HoldTutorial(CampaignMilestone.UITutorial_CodexCanvas, false, true);
		}

		// Token: 0x06005544 RID: 21828 RVA: 0x0026BBEE File Offset: 0x00269DEE
		public static void HideCodexPanel()
		{
			(World.Active.GetExistingManager<CanvasManager>().Codex as CodexController).HideCodex();
		}

		// Token: 0x06005545 RID: 21829 RVA: 0x0026BC09 File Offset: 0x00269E09
		public void OnClickResetTutorial()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
			GameControl.control.activePlayer.ResetAllTutorialMilestones();
		}

		// Token: 0x06005546 RID: 21830 RVA: 0x0026BC28 File Offset: 0x00269E28
		private void HandleMissionTemplate(TICodexEntryTemplate template, bool cacheSearchMode = false)
		{
			List<TIMissionTemplate> list = (from x in TemplateManager.IterateByClass<TIMissionTemplate>(true)
				where x.baseMission
				select x).ToList<TIMissionTemplate>();
			foreach (TICouncilorTypeTemplate ticouncilorTypeTemplate in TemplateManager.IterateByClass<TICouncilorTypeTemplate>(true).ToList<TICouncilorTypeTemplate>())
			{
				if (ticouncilorTypeTemplate.weight > 0f)
				{
					foreach (TIMissionTemplate timissionTemplate in ticouncilorTypeTemplate.missions)
					{
						if (!list.Contains(timissionTemplate))
						{
							list.Add(timissionTemplate);
						}
					}
				}
			}
			list = list.OrderBy<TIMissionTemplate, int>((TIMissionTemplate x) => x.sortOrder).ToList<TIMissionTemplate>();
			if (cacheSearchMode)
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (TIMissionTemplate timissionTemplate2 in list)
				{
					stringBuilder.Append(timissionTemplate2.displayName);
				}
				this.addSearchKey(template, stringBuilder.ToString());
				return;
			}
			this.codexInfoListManager.SetListSize<CodexInfoListItemController>(this.infoLocCount + list.Count, true, false);
			this.DisplayMainCodexText();
			int num = 0;
			for (int i = this.infoLocCount; i < this.codexInfoListManager.size; i++)
			{
				CodexInfoListItemController component = this.codexInfoListManager.transform.GetChild(i).GetComponent<CodexInfoListItemController>();
				StringBuilder stringBuilder2 = new StringBuilder("<align=\"center\">").Append(list[num].displayName).AppendLine().AppendLine(list[num].description)
					.Append("</align>");
				if (list[num].ContestedMission)
				{
					stringBuilder2.AppendLine(list[num].MissionDetailText());
				}
				component.InitCodexTemplateItemWithIcon(this, template, stringBuilder2.ToString(), list[num].missionIconImagePath_Off);
				component.UpdateListItem();
				num++;
			}
		}

		// Token: 0x06005547 RID: 21831 RVA: 0x0026BE74 File Offset: 0x0026A074
		private void HandleAlienMissionTemplate(TICodexEntryTemplate template, bool cacheSearchMode = false)
		{
			List<TIMissionTemplate> list = new List<TIMissionTemplate>();
			foreach (TIMissionTemplate timissionTemplate in TemplateManager.IterateByClass<TIMissionTemplate>(true))
			{
				if (timissionTemplate.knowledgeProject != string.Empty && GameControl.control.activePlayer.finishedProjectNames.Contains(timissionTemplate.knowledgeProject))
				{
					list.Add(timissionTemplate);
				}
			}
			if (cacheSearchMode)
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (TIMissionTemplate timissionTemplate2 in list)
				{
					stringBuilder.Append(timissionTemplate2.displayName);
				}
				this.addSearchKey(template, stringBuilder.ToString());
				return;
			}
			this.codexInfoListManager.SetListSize<CodexInfoListItemController>(this.infoLocCount + list.Count, true, false);
			this.DisplayMainCodexText();
			int num = 0;
			for (int i = this.infoLocCount; i < this.codexInfoListManager.size; i++)
			{
				CodexInfoListItemController component = this.codexInfoListManager.transform.GetChild(i).GetComponent<CodexInfoListItemController>();
				StringBuilder stringBuilder2 = new StringBuilder("<align=\"center\">").Append(list[num].displayName).AppendLine().AppendLine(list[num].description)
					.Append("</align>");
				if (list[num].ContestedMission)
				{
					stringBuilder2.AppendLine(list[num].MissionDetailText());
				}
				component.InitCodexTemplateItemWithIcon(this, template, stringBuilder2.ToString(), list[num].missionIconImagePath_Off);
				component.UpdateListItem();
				num++;
			}
		}

		// Token: 0x06005548 RID: 21832 RVA: 0x0026C038 File Offset: 0x0026A238
		private void HandleTraitTemplate(TICodexEntryTemplate template, bool cacheSearchMode = false)
		{
			List<TITraitTemplate> list = (from x in TemplateManager.IterateByClass<TITraitTemplate>(true)
				where x.dataName != "dummy"
				select x).ToList<TITraitTemplate>();
			list = list.Where<TITraitTemplate>((TITraitTemplate x) => !x.requiresProject || GameControl.control.activePlayer.completedProjects.Any<TIProjectTemplate>((TIProjectTemplate y) => x.IsMatchingProject(y))).ToList<TITraitTemplate>();
			if (cacheSearchMode)
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (TITraitTemplate titraitTemplate in list)
				{
					stringBuilder.Append(titraitTemplate.displayName);
				}
				this.addSearchKey(template, stringBuilder.ToString());
				return;
			}
			this.codexInfoListManager.SetListSize<CodexInfoListItemController>(this.infoLocCount + list.Count, true, false);
			this.DisplayMainCodexText();
			int num = 0;
			for (int i = this.infoLocCount; i < this.codexInfoListManager.size; i++)
			{
				CodexInfoListItemController component = this.codexInfoListManager.transform.GetChild(i).GetComponent<CodexInfoListItemController>();
				component.InitCodexTemplateItem(this, template, list[num].fullTraitSummary);
				component.UpdateListItem();
				num++;
			}
		}

		// Token: 0x06005549 RID: 21833 RVA: 0x0026C178 File Offset: 0x0026A378
		private void HandleArmyOperationTemplate(TICodexEntryTemplate template, bool cacheSearchMode = false)
		{
			List<IOperation> list = OperationsManager.armyOperations.Where<IOperation>((IOperation x) => (x as TIArmyOperationTemplate).dataName != "CancelArmyOperation").ToList<IOperation>();
			if (cacheSearchMode)
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (IOperation operation in list)
				{
					stringBuilder.Append(operation.GetDisplayName());
				}
				this.addSearchKey(template, stringBuilder.ToString());
				return;
			}
			this.codexInfoListManager.SetListSize<CodexInfoListItemController>(this.infoLocCount + list.Count, true, false);
			this.DisplayMainCodexText();
			int num = 0;
			for (int i = this.infoLocCount; i < this.codexInfoListManager.size; i++)
			{
				CodexInfoListItemController component = this.codexInfoListManager.transform.GetChild(i).GetComponent<CodexInfoListItemController>();
				component.InitCodexTemplateItemWithIcon(this, template, list[num].GetDisplayName() + "\n" + list[num].GetDescription(null, null), list[num].GetOperationIconImagePath_Off());
				component.UpdateListItem();
				num++;
			}
		}

		// Token: 0x0600554A RID: 21834 RVA: 0x0026C2B0 File Offset: 0x0026A4B0
		private void HandlePolicyTemplate(TICodexEntryTemplate template, bool cacheSearchMode = false)
		{
			List<TIPolicyOption> list = (from x in TemplateManager.IterateByClass<TIPolicyOption>(true)
				where !x.HandledAtFactionLevel() && x.GetPolicyType() != PolicyType.CancelOption
				select x).ToList<TIPolicyOption>();
			if (cacheSearchMode)
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (TIPolicyOption tipolicyOption in list)
				{
					stringBuilder.Append(tipolicyOption.displayName);
				}
				this.addSearchKey(template, stringBuilder.ToString());
				return;
			}
			this.codexInfoListManager.SetListSize<CodexInfoListItemController>(this.infoLocCount + list.Count, false, false);
			this.DisplayMainCodexText();
			int num = 0;
			for (int i = this.infoLocCount; i < this.codexInfoListManager.size; i++)
			{
				CodexInfoListItemController component = this.codexInfoListManager.transform.GetChild(i).GetComponent<CodexInfoListItemController>();
				component.InitCodexTemplateItem(this, template, list[num].displayName + "\n" + list[num].GetDescription());
				num++;
				component.UpdateListItem();
			}
		}

		// Token: 0x0600554B RID: 21835 RVA: 0x0026C3D8 File Offset: 0x0026A5D8
		private void HandleFleetOperationTemplate(TICodexEntryTemplate template, bool cacheSearchMode = false)
		{
			List<IOperation> list = OperationsManager.fleetOperations.Where<IOperation>((IOperation x) => !(x as TISpaceFleetOperationTemplate).isAlien()).ToList<IOperation>();
			if (cacheSearchMode)
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (IOperation operation in list)
				{
					stringBuilder.Append(operation.GetDisplayName());
				}
				this.addSearchKey(template, stringBuilder.ToString());
				return;
			}
			this.codexInfoListManager.SetListSize<CodexInfoListItemController>(this.infoLocCount + list.Count, true, false);
			this.DisplayMainCodexText();
			int num = 0;
			for (int i = this.infoLocCount; i < this.codexInfoListManager.size; i++)
			{
				CodexInfoListItemController component = this.codexInfoListManager.transform.GetChild(i).GetComponent<CodexInfoListItemController>();
				component.InitCodexTemplateItemWithIcon(this, template, list[num].GetDisplayName() + "\n" + list[num].GetDescription(null, null), list[num].GetOperationIconImagePath_Off());
				num++;
				component.UpdateListItem();
			}
		}

		// Token: 0x0600554C RID: 21836 RVA: 0x0026C510 File Offset: 0x0026A710
		private void HandleTechCategoryList(TICodexEntryTemplate template, bool cacheSearchMode = false)
		{
			List<TechCategory> list = Enums.TechCategories.ToList<TechCategory>();
			if (cacheSearchMode)
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (TechCategory techCategory in list)
				{
					stringBuilder.Append(TIGenericTechTemplate.GetTechCategoryString(techCategory));
				}
				this.addSearchKey(template, stringBuilder.ToString());
				return;
			}
			this.codexInfoListManager.SetListSize<CodexInfoListItemController>(this.infoLocCount + list.Count, true, false);
			this.DisplayMainCodexText();
			int num = 0;
			for (int i = this.infoLocCount; i < this.codexInfoListManager.size; i++)
			{
				CodexInfoListItemController component = this.codexInfoListManager.transform.GetChild(i).GetComponent<CodexInfoListItemController>();
				component.InitCodexTemplateItemWithIcon(this, template, TIGenericTechTemplate.GetTechCategoryString(list[num]) + "\n" + TIGenericTechTemplate.GetTechCategoryDescription(list[num]), TIGenericTechTemplate.PathTechCategoryIcon(list[num]));
				num++;
				component.UpdateListItem();
			}
		}

		// Token: 0x0600554D RID: 21837 RVA: 0x0026C620 File Offset: 0x0026A820
		private void HandleCouncilorTypeTemplate(TICodexEntryTemplate template, bool cacheSearchMode = false)
		{
			List<TICouncilorTypeTemplate> list = (from x in TemplateManager.IterateByClass<TICouncilorTypeTemplate>(true)
				where x.weight > 0f
				orderby x.displayName
				select x).ToList<TICouncilorTypeTemplate>();
			if (cacheSearchMode)
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (TICouncilorTypeTemplate ticouncilorTypeTemplate in list)
				{
					stringBuilder.Append(ticouncilorTypeTemplate.displayName);
				}
				this.addSearchKey(template, stringBuilder.ToString());
				return;
			}
			this.codexInfoListManager.SetListSize<CodexInfoListItemController>(this.infoLocCount + list.Count, true, false);
			this.DisplayMainCodexText();
			int num = 0;
			for (int i = this.infoLocCount; i < this.codexInfoListManager.size; i++)
			{
				CodexInfoListItemController component = this.codexInfoListManager.transform.GetChild(i).GetComponent<CodexInfoListItemController>();
				component.InitCodexTemplateItem(this, template, new StringBuilder(list[num].displayName).AppendLine().AppendLine(list[num].description).ToString());
				num++;
				component.UpdateListItem();
			}
		}

		// Token: 0x0600554E RID: 21838 RVA: 0x0026C778 File Offset: 0x0026A978
		private void HandleOfficerTypeTemplate(TICodexEntryTemplate template, bool cacheSearchMode = false)
		{
			List<TIOfficerTemplate> list = TemplateManager.IterateByClass<TIOfficerTemplate>(true).ToList<TIOfficerTemplate>();
			if (cacheSearchMode)
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (TIOfficerTemplate tiofficerTemplate in list)
				{
					for (int i = 1; i < 3; i++)
					{
						stringBuilder.Append(tiofficerTemplate.flagOfficerAndRank(i));
					}
				}
				this.addSearchKey(template, stringBuilder.ToString());
				return;
			}
			this.codexInfoListManager.SetListSize<CodexInfoListItemController>(this.infoLocCount + list.Count * 3, true, false);
			this.DisplayMainCodexText();
			int num = 0;
			int num2 = 1;
			for (int j = this.infoLocCount; j < this.codexInfoListManager.size; j++)
			{
				CodexInfoListItemController component = this.codexInfoListManager.transform.GetChild(j).GetComponent<CodexInfoListItemController>();
				component.InitCodexTemplateItemWithIcon(this, template, list[num].FullDescriptionAtRank(num2, null, false, null), list[num].GetIconPath(num2));
				component.UpdateListItem();
				num2++;
				if (num2 > 3)
				{
					num2 = 1;
					num++;
				}
			}
		}

		// Token: 0x04003B6F RID: 15215
		public RectTransform primaryPanelTransform;

		// Token: 0x04003B70 RID: 15216
		public List<TICodexEntryTemplate> allCodexEntries;

		// Token: 0x04003B71 RID: 15217
		public ListManagerBase codexTopicListManager;

		// Token: 0x04003B72 RID: 15218
		public ListManagerBase codexInfoListManager;

		// Token: 0x04003B73 RID: 15219
		public TICodexEntryTemplate selectedTopic;

		// Token: 0x04003B74 RID: 15220
		public GameObject resetTutorialButton;

		// Token: 0x04003B75 RID: 15221
		public RectTransform codexItemsContainer;

		// Token: 0x04003B76 RID: 15222
		public TMP_Text codexSearchTitle;

		// Token: 0x04003B77 RID: 15223
		public TMP_InputField codexSearch;

		// Token: 0x04003B78 RID: 15224
		public Dictionary<TICodexEntryTemplate, string> codexSearchDictionary = new Dictionary<TICodexEntryTemplate, string>();

		// Token: 0x04003B79 RID: 15225
		private int infoLocCount;

		// Token: 0x04003B7A RID: 15226
		public UITutorialController codexTutorialController;

		// Token: 0x04003B7B RID: 15227
		private readonly List<CampaignMilestone> resetMilestones = new List<CampaignMilestone>
		{
			CampaignMilestone.UITutorial_GeneralControlsCanvas,
			CampaignMilestone.UITutorial_CouncilorMissionControlsCanvas1,
			CampaignMilestone.UITutorial_CouncilorMissionControlsCanvas2,
			CampaignMilestone.UITutorial_ObjectivesScreenCanvas,
			CampaignMilestone.UITutorial_CouncilManagementCanvas_Grid,
			CampaignMilestone.UITutorial_CouncilManagementCanvas_Detail,
			CampaignMilestone.UITutorial_CouncilManagementCanvas_Recruiting,
			CampaignMilestone.UITutorial_NationsScreenCanvas_Nations,
			CampaignMilestone.UITutorial_NationsScreenCanvas_DesignPreset,
			CampaignMilestone.UITutorial_NationsInfoCanvas_NationPanel,
			CampaignMilestone.UITutorial_NationsInfoCanvas_Priorities,
			CampaignMilestone.UITutorial_NationsInfoCanvas_DirectInvestment,
			CampaignMilestone.UITutorial_HabScreenCanvas,
			CampaignMilestone.UITutorial_HabScreenCanvasManagement,
			CampaignMilestone.UITutorial_FleetsScreenCanvas_FleetsList,
			CampaignMilestone.UITutorial_FleetsScreenCanvas_ClassList,
			CampaignMilestone.UITutorial_FleetsScreenCanvas_ShipDetail,
			CampaignMilestone.UITutorial_FleetsScreenCanvas_ShipDesigner,
			CampaignMilestone.UITutorial_FleetsScreenCanvas_ConstructionManager,
			CampaignMilestone.UITutorial_FleetsScreenCanvas_DesignModuleWeapons,
			CampaignMilestone.UITutorial_FleetsScreenCanvas_DesignModuleDrive,
			CampaignMilestone.UITutorial_FleetsScreenCanvas_DesignModulePower,
			CampaignMilestone.UITutorial_FleetsScreenCanvas_DesignModuleRadiator,
			CampaignMilestone.UITutorial_FleetsScreenCanvas_DesignModuleHeatSink,
			CampaignMilestone.UITutorial_FleetsScreenCanvas_DesignModuleBattery,
			CampaignMilestone.UITutorial_ResearchScreenCanvas_Primary,
			CampaignMilestone.UITutorial_IntelScreenCanvas_Factions,
			CampaignMilestone.UITutorial_IntelScreenCanvas_Aliens,
			CampaignMilestone.UITutorial_IntelScreenCanvas_Global,
			CampaignMilestone.UITutorial_IntelScreenCanvas_SolarSystem,
			CampaignMilestone.UITutorial_SpaceCombatCanvas,
			CampaignMilestone.UITutorial_SpaceCombatCanvas_FriendlyShipDetail,
			CampaignMilestone.UITutorial_SpaceCombatCanvas_Waypoints,
			CampaignMilestone.UITutorial_OperationScreenCanvas_ArmyOperations,
			CampaignMilestone.UITutorial_OperationScreenCanvas_SpacebodyOperations,
			CampaignMilestone.UITutorial_OperationScreenCanvas_FleetOperations,
			CampaignMilestone.UITutorial_OperationScreenCanvas_FleetTransfer,
			CampaignMilestone.UITutorial_SpaceObjectDetailCanvas_Spacebody,
			CampaignMilestone.UITutorial_SpaceObjectDetailCanvas_Lagrange,
			CampaignMilestone.UITutorial_SpaceObjectDetailCanvas_Hab,
			CampaignMilestone.UITutorial_SpaceObjectDetailCanvas_MyFleet,
			CampaignMilestone.UITutorial_SpaceObjectDetailCanvas_EnemyFleet
		};
	}
}
