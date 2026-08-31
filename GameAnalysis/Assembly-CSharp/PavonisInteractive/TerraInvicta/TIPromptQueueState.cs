using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PavonisInteractive.TerraInvicta.Actions;
using PavonisInteractive.TerraInvicta.Systems.GameTime;
using PavonisInteractive.TerraInvicta.Tasks;
using Unity.Entities;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000788 RID: 1928
	public class TIPromptQueueState : TIGameState
	{
		// Token: 0x17000AD3 RID: 2771
		// (get) Token: 0x06003D2D RID: 15661 RVA: 0x0017FDE0 File Offset: 0x0017DFE0
		// (set) Token: 0x06003D2E RID: 15662 RVA: 0x0017FDE8 File Offset: 0x0017DFE8
		public List<Prompt> activePlayerNationPromptList { get; private set; }

		// Token: 0x17000AD4 RID: 2772
		// (get) Token: 0x06003D2F RID: 15663 RVA: 0x0017FDF1 File Offset: 0x0017DFF1
		// (set) Token: 0x06003D30 RID: 15664 RVA: 0x0017FDF9 File Offset: 0x0017DFF9
		public List<Prompt> activePlayerFactionPromptList { get; private set; }

		// Token: 0x17000AD5 RID: 2773
		// (get) Token: 0x06003D31 RID: 15665 RVA: 0x0017FE02 File Offset: 0x0017E002
		public bool anyBlocking
		{
			get
			{
				return this.nationList.Count > 0 || this.factionList.Count > 0;
			}
		}

		// Token: 0x17000AD6 RID: 2774
		// (get) Token: 0x06003D32 RID: 15666 RVA: 0x0017FE22 File Offset: 0x0017E022
		public static bool anyBlockingPrompt
		{
			get
			{
				return GameStateManager.PromptQueue().anyBlocking;
			}
		}

		// Token: 0x17000AD7 RID: 2775
		// (get) Token: 0x06003D33 RID: 15667 RVA: 0x0017FE2E File Offset: 0x0017E02E
		public bool anyActivePlayerBlocking
		{
			get
			{
				return this.activePlayerNationPromptList.Count > 0 || this.activePlayerFactionPromptList.Count > 0;
			}
		}

		// Token: 0x17000AD8 RID: 2776
		// (get) Token: 0x06003D34 RID: 15668 RVA: 0x0017FE4E File Offset: 0x0017E04E
		public static bool anyActivePlayerBlockingPrompt
		{
			get
			{
				return GameStateManager.PromptQueue().anyActivePlayerBlocking;
			}
		}

		// Token: 0x06003D35 RID: 15669 RVA: 0x0017FE5C File Offset: 0x0017E05C
		public override void PostGameStateCreateInit_OnCreationOnly_1()
		{
			if (!this.gameStateSubjectCreated)
			{
				this.nationList = new List<Prompt>();
				this.factionList = new List<Prompt>();
				this.activePlayerNationPromptList = new List<Prompt>();
				this.activePlayerFactionPromptList = new List<Prompt>();
			}
			if (this.gameTime == null)
			{
				this.gameTime = World.Active.GetExistingManager<GameTimeManager>();
			}
		}

		// Token: 0x06003D36 RID: 15670 RVA: 0x0017FEB8 File Offset: 0x0017E0B8
		public override void PostGlobalGameStateCreateInit_2()
		{
			this.projectSelectionStrategy = new StratProjectSelector();
			this.techSelectionStrategy = new StratTechSelector();
			this.policyResponseSelectionStrategy = new StratPolicyResponseSelector();
			this.combatInitStrategy = new StratCombatInitStrategy();
			this.narrativeResponseStrategy = new StratNarrativeResponseSelector();
			this.globalValues = GameStateManager.GlobalValues();
			this.councilorMissionPlanner = AICouncilorMissionPlanner.singleton;
		}

		// Token: 0x06003D37 RID: 15671 RVA: 0x0017FF12 File Offset: 0x0017E112
		public override void PostCanvasManagerCreateInit_3()
		{
			GameControl.eventManager.TriggerEvent(new BlockingPromptUpdated(), null, Array.Empty<object>());
		}

		// Token: 0x06003D38 RID: 15672 RVA: 0x0017FF2C File Offset: 0x0017E12C
		public override void PostAllStartUpInit_5()
		{
			foreach (Prompt prompt in this.nationList.ToList<Prompt>())
			{
				if (!TIGameState.Valid(prompt.actingState) || !TIGameState.Valid(prompt.promptingGameState) || (prompt.relatedGameState != null && !TIGameState.Valid(prompt.relatedGameState)))
				{
					Log.Error("Prompt left with bad game state. Deleting.", Array.Empty<object>());
					this.nationList.Remove(prompt);
				}
			}
			if (this.gameStateSubjectCreated && this.activePlayerNationPromptList.Count > 0)
			{
				GameControl.eventManager.TriggerEvent(new BlockingPromptOnStartup(), null, Array.Empty<object>());
			}
			this.gameStateSubjectCreated = true;
		}

		// Token: 0x06003D39 RID: 15673 RVA: 0x00180004 File Offset: 0x0017E204
		public static string GetBlockingDetailStr()
		{
			TIPromptQueueState tipromptQueueState = GameStateManager.FindGameState<TIPromptQueueState>();
			StringBuilder stringBuilder = new StringBuilder(Loc.T("UI.GeneralControls.TimeIsBlocked"));
			foreach (Prompt prompt in tipromptQueueState.activePlayerFactionPromptList)
			{
				stringBuilder.AppendLine(Loc.T(new StringBuilder("UI.GeneralControls.").Append(prompt.name).ToString()));
			}
			foreach (Prompt prompt2 in tipromptQueueState.activePlayerNationPromptList)
			{
				stringBuilder.AppendLine(Loc.T(new StringBuilder("UI.GeneralControls.").Append(prompt2.name).ToString()));
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06003D3A RID: 15674 RVA: 0x001800F8 File Offset: 0x0017E2F8
		public void AddPrompt(Prompt newPrompt)
		{
			if (newPrompt.actingState.isFactionState)
			{
				if (!this.factionList.Contains(newPrompt))
				{
					this.factionList.Add(newPrompt);
					if (newPrompt.actingState == GameControl.control.activePlayer)
					{
						this.activePlayerFactionPromptList.Add(newPrompt);
						GameControl.eventManager.TriggerEvent(new BlockingPromptUpdated(), null, Array.Empty<object>());
						return;
					}
				}
			}
			else
			{
				TINationState ref_nation = newPrompt.actingState.ref_nation;
				if (ref_nation != null && !this.nationList.Contains(newPrompt))
				{
					this.nationList.Add(newPrompt);
					if (ref_nation.executiveFaction == GameControl.control.activePlayer)
					{
						this.activePlayerNationPromptList.Add(newPrompt);
						GameControl.eventManager.TriggerEvent(new BlockingPromptUpdated(), null, Array.Empty<object>());
					}
				}
			}
		}

		// Token: 0x06003D3B RID: 15675 RVA: 0x001801D8 File Offset: 0x0017E3D8
		public void AddPrompt(TIGameState actingState, TIGameState promptingGameState, TIGameState relatedGameState, string name, int value = 0)
		{
			if (actingState.isFactionState)
			{
				Prompt prompt = new Prompt(actingState, promptingGameState, relatedGameState, name, value);
				if (!this.factionList.Contains(prompt))
				{
					this.factionList.Add(prompt);
					if (actingState == GameControl.control.activePlayer)
					{
						this.activePlayerFactionPromptList.Add(prompt);
						GameControl.eventManager.TriggerEvent(new BlockingPromptUpdated(), null, Array.Empty<object>());
						return;
					}
				}
			}
			else
			{
				TINationState ref_nation = actingState.ref_nation;
				Prompt prompt2 = new Prompt(ref_nation, promptingGameState, relatedGameState, name, value);
				if (ref_nation != null)
				{
					if (this.nationList.Contains(prompt2))
					{
						Log.Error("Duplicate Prompt: " + prompt2.ToString(), Array.Empty<object>());
						return;
					}
					this.nationList.Add(prompt2);
					if (ref_nation.executiveFaction == GameControl.control.activePlayer)
					{
						this.activePlayerNationPromptList.Add(prompt2);
						GameControl.eventManager.TriggerEvent(new BlockingPromptUpdated(), null, Array.Empty<object>());
						return;
					}
				}
				else
				{
					Log.Error("Invalid Prompt: " + prompt2.ToString(), Array.Empty<object>());
				}
			}
		}

		// Token: 0x06003D3C RID: 15676 RVA: 0x00180303 File Offset: 0x0017E503
		public static void AddPromptStatic(Prompt prompt)
		{
			GameStateManager.FindGameState<TIPromptQueueState>().AddPrompt(prompt);
		}

		// Token: 0x06003D3D RID: 15677 RVA: 0x00180310 File Offset: 0x0017E510
		public static void AddPromptStatic(TIGameState actingState, TIGameState promptingGameState, TIGameState relatedGameState, string name, int value = 0)
		{
			GameStateManager.FindGameState<TIPromptQueueState>().AddPrompt(actingState, promptingGameState, relatedGameState, name, value);
		}

		// Token: 0x06003D3E RID: 15678 RVA: 0x00180324 File Offset: 0x0017E524
		public bool RemovePrompt(Prompt prompt)
		{
			bool flag = false;
			if (prompt.actingState.isFactionState)
			{
				flag = this.factionList.Remove(prompt);
				if (prompt.actingState == GameControl.control.activePlayer)
				{
					this.activePlayerFactionPromptList.Remove(prompt);
					GameControl.eventManager.TriggerEvent(new BlockingPromptUpdated(), null, Array.Empty<object>());
				}
			}
			else
			{
				TIPolityState tipolityState = prompt.actingState as TIPolityState;
				if (tipolityState != null)
				{
					flag = this.nationList.Remove(prompt);
					if (tipolityState.ref_nation.executiveFaction == GameControl.control.activePlayer || (tipolityState.ref_nation.executiveFaction == null && !tipolityState.ref_nation.extant))
					{
						this.activePlayerNationPromptList.Remove(prompt);
						GameControl.eventManager.TriggerEvent(new BlockingPromptUpdated(), null, Array.Empty<object>());
					}
				}
			}
			if (!this.factionList.Any<Prompt>() && !this.nationList.Any<Prompt>())
			{
				GameControl.eventManager.TriggerEvent(new PromptQueueCleared(), null, Array.Empty<object>());
			}
			return flag;
		}

		// Token: 0x06003D3F RID: 15679 RVA: 0x00180441 File Offset: 0x0017E641
		public void RemovePrompt(TIGameState actingState, TIGameState promptingGameState, TIGameState relatedGameState, string name, int value = 0)
		{
			this.RemovePrompt(new Prompt(actingState, promptingGameState, relatedGameState, name, value));
		}

		// Token: 0x06003D40 RID: 15680 RVA: 0x00180456 File Offset: 0x0017E656
		public static void RemovePromptStatic(Prompt prompt)
		{
			GameStateManager.PromptQueue().RemovePrompt(prompt);
		}

		// Token: 0x06003D41 RID: 15681 RVA: 0x00180464 File Offset: 0x0017E664
		public static void RemovePromptStatic(TIGameState actingState, TIGameState promptingGameState, TIGameState relatedGameState, string name, int value = 0)
		{
			TIPromptQueueState.RemovePromptStatic(new Prompt(actingState, promptingGameState, relatedGameState, name, value));
		}

		// Token: 0x06003D42 RID: 15682 RVA: 0x00180476 File Offset: 0x0017E676
		public bool HasPrompt(Prompt promptToCheck)
		{
			if (promptToCheck.actingState.isFactionState)
			{
				return this.factionList.Contains(promptToCheck);
			}
			return promptToCheck.actingState is TIPolityState && this.nationList.Contains(promptToCheck);
		}

		// Token: 0x06003D43 RID: 15683 RVA: 0x001804AF File Offset: 0x0017E6AF
		public bool HasPrompt(TIGameState actingState, TIGameState promptingGameState, TIGameState relatedGameState, string name, int value = 0)
		{
			return this.HasPrompt(new Prompt(actingState, promptingGameState, relatedGameState, name, value));
		}

		// Token: 0x06003D44 RID: 15684 RVA: 0x001804C3 File Offset: 0x0017E6C3
		public static bool HasPromptStatic(Prompt promptToCheck)
		{
			return GameStateManager.PromptQueue().HasPrompt(promptToCheck);
		}

		// Token: 0x06003D45 RID: 15685 RVA: 0x001804D0 File Offset: 0x0017E6D0
		public static bool HasPromptStatic(TIGameState actingState, TIGameState promptingGameState, TIGameState relatedGameState, string name, int value = 0)
		{
			return TIPromptQueueState.HasPromptStatic(new Prompt(actingState, promptingGameState, relatedGameState, name, value));
		}

		// Token: 0x06003D46 RID: 15686 RVA: 0x001804E4 File Offset: 0x0017E6E4
		public bool HasAnyPromptofType(string name, bool factionOnly = false, bool nationOnly = false)
		{
			return (!nationOnly && this.factionList.Any<Prompt>((Prompt x) => x.name == name)) || (!factionOnly && this.nationList.Any<Prompt>((Prompt x) => x.name == name));
		}

		// Token: 0x06003D47 RID: 15687 RVA: 0x00180538 File Offset: 0x0017E738
		public void HandlePrompts()
		{
			foreach (Prompt prompt in new List<Prompt>(this.factionList))
			{
				TIFactionState ref_faction = prompt.actingState.ref_faction;
				if (ref_faction.player.isAI)
				{
					string text = prompt.name;
					if (text != null)
					{
						uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
						if (num <= 629440155U)
						{
							if (num <= 282092187U)
							{
								if (num != 31176079U)
								{
									if (num != 215492961U)
									{
										if (num == 282092187U)
										{
											if (text == "PromptSelectTech")
											{
												this.HandleSelectTech(ref_faction, prompt.value);
											}
										}
									}
									else if (text == "PromptFactionContactRespondToOffer")
									{
										TIMissionState timissionState = prompt.relatedGameState as TIMissionState;
										this.HandleFactionContactRespondToOffer(ref_faction, timissionState.target.ref_faction, timissionState);
									}
								}
								else if (text == "PromptStealProject")
								{
									TIMissionState timissionState2 = prompt.relatedGameState as TIMissionState;
									this.HandleStealProject(ref_faction, timissionState2.councilor, timissionState2.target, timissionState2);
								}
							}
							else if (num != 492343306U)
							{
								if (num != 610075336U)
								{
									if (num == 629440155U)
									{
										if (text == "PromptSelectSpaceCombatStance")
										{
											this.HandleSelectSpaceCombatStance(ref_faction, prompt.relatedGameState as TISpaceCombatState);
										}
									}
								}
								else if (text == "PromptSelectProject")
								{
									this.HandleSelectProject(ref_faction, prompt.value);
								}
							}
							else if (text == "PromptSelectSpaceCombatBid")
							{
								this.HandleSelectSpaceCombatBid(ref_faction, prompt.relatedGameState as TISpaceCombatState);
							}
						}
						else if (num <= 2071856233U)
						{
							if (num != 1482437579U)
							{
								if (num != 1947397857U)
								{
									if (num == 2071856233U)
									{
										if (text == "PromptFactionContactMakeOffer")
										{
											TIMissionState timissionState3 = prompt.relatedGameState as TIMissionState;
											this.HandleFactionContactMakeOffer(ref_faction, timissionState3.target.ref_faction, timissionState3);
										}
									}
								}
								else if (text == "PromptDropOrgs")
								{
									this.HandleDropUnassignedOrgs(ref_faction);
								}
							}
							else if (text == "PromptAddressNarrativeEvent")
							{
								this.HandleRespondToNarrativeEvent(prompt, ref_faction, prompt.promptingGameState, prompt.relatedGameState);
							}
						}
						else if (num != 2075706110U)
						{
							if (num != 3207435152U)
							{
								if (num == 4042397024U)
								{
									if (text == "PromptSabotageProject")
									{
										TIMissionState timissionState4 = prompt.relatedGameState as TIMissionState;
										this.HandleSabotageProject(ref_faction, timissionState4.councilor, timissionState4.target, timissionState4);
									}
								}
							}
							else if (text == "PromptSelectCouncilorMissions")
							{
								this.HandlePlanMissions(ref_faction, prompt);
								return;
							}
						}
						else if (text == "PromptChangeTrajectory")
						{
							TIFactionState tifactionState = ref_faction;
							TISpaceFleetState ref_fleet = prompt.promptingGameState.ref_fleet;
							TIGameState relatedGameState = prompt.relatedGameState;
							this.HandlePromptChangeTrajectory(tifactionState, ref_fleet, (relatedGameState != null) ? relatedGameState.ref_fleet : null, prompt.promptingGameState.ref_fleet.proposedTrajectories);
						}
					}
				}
			}
			foreach (Prompt prompt2 in new List<Prompt>(this.nationList))
			{
				TINationState ref_nation = prompt2.actingState.ref_nation;
				if (ref_nation.executiveFaction == null || ref_nation.executiveFaction.player.isAI)
				{
					string text = prompt2.name;
					if (text != null)
					{
						uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
						if (num <= 2900450226U)
						{
							if (num <= 2355149519U)
							{
								if (num != 1481396765U)
								{
									if (num != 1482437579U)
									{
										if (num == 2355149519U)
										{
											if (text == "PromptRespondToEndRivalryCall")
											{
												this.HandleEndRivalry(ref_nation, prompt2.promptingGameState as TINationState);
											}
										}
									}
									else if (text == "PromptAddressNarrativeEvent")
									{
										this.HandleRespondToNarrativeEvent(prompt2, ref_nation, prompt2.promptingGameState, prompt2.relatedGameState);
									}
								}
								else if (text == "PromptSelectPolicy")
								{
									this.HandleSelectPolicy(ref_nation, prompt2.promptingGameState as TIFactionState, prompt2.relatedGameState as TICouncilorState);
								}
							}
							else if (num != 2737292978U)
							{
								if (num != 2883068329U)
								{
									if (num == 2900450226U)
									{
										if (text == "PromptRespondToTransferRegionCall")
										{
											this.HandleRegionDemanded(ref_nation, prompt2.promptingGameState.ref_nation, prompt2.relatedGameState.ref_region);
										}
									}
								}
								else if (text == "PromptNationLeavesDarkFederation_Violent")
								{
									this.HandleNationLeavesMyDarkFederation_Violent(ref_nation, prompt2.promptingGameState.ref_nation, prompt2);
								}
							}
							else if (text == "PromptRespondToFormAllianceCall")
							{
								this.HandleProposedAlliance(ref_nation, prompt2.promptingGameState as TINationState);
							}
						}
						else if (num <= 3716459916U)
						{
							if (num != 2995569124U)
							{
								if (num != 3394191324U)
								{
									if (num == 3716459916U)
									{
										if (text == "PromptRespondToJoinFederationCall")
										{
											this.HandleFederation(ref_nation, prompt2.promptingGameState.ref_nation);
										}
									}
								}
								else if (text == "PromptArmyOrderedToDepart")
								{
									this.HandleResponseToArmyBooted(prompt2);
								}
							}
							else if (text == "PromptNationLeavesDarkFederation_Policy")
							{
								this.HandleNationLeavesMyDarkFederation_Policy(ref_nation, prompt2.promptingGameState.ref_nation, prompt2);
							}
						}
						else if (num != 3757877436U)
						{
							if (num != 4016174268U)
							{
								if (num == 4202808030U)
								{
									if (text == "PromptRespondToUnificationCall")
									{
										this.HandleUnification(ref_nation, prompt2.promptingGameState as TINationState);
									}
								}
							}
							else if (text == "PromptRespondToAllyOffensiveWarCall")
							{
								this.HandleCallToOffensiveWar(ref_nation, prompt2.promptingGameState as TINationState, prompt2.relatedGameState as TIWarState);
							}
						}
						else if (text == "PromptRespondToEndWarCall")
						{
							if (!prompt2.relatedGameState.deleted)
							{
								this.HandleEndWar(ref_nation, prompt2.promptingGameState as TINationState, prompt2.relatedGameState as TIWarState);
							}
							else
							{
								Log.Error("tried to resolve EndWar prompt on a deleted, nonexistant war", Array.Empty<object>());
								this.nationList.Remove(prompt2);
							}
						}
					}
				}
				else if (!this.activePlayerNationPromptList.Contains(prompt2))
				{
					Log.Error("Bad Player Prompt " + prompt2.name + ". Game may be in bad state.", Array.Empty<object>());
					this.nationList.Remove(prompt2);
				}
			}
		}

		// Token: 0x06003D48 RID: 15688 RVA: 0x00180D08 File Offset: 0x0017EF08
		public static bool PlayerMissionPrompt(Prompt prompt)
		{
			string name = prompt.name;
			return name != null && (name == "PromptSabotageProject" || name == "PromptStealProject" || name == "PromptFactionContactMakeOffer");
		}

		// Token: 0x06003D49 RID: 15689 RVA: 0x00180D4C File Offset: 0x0017EF4C
		public static bool PlayerOperationPrompt(Prompt prompt)
		{
			string name = prompt.name;
			return name != null && name == "PromptChangeTrajectory";
		}

		// Token: 0x06003D4A RID: 15690 RVA: 0x00180D74 File Offset: 0x0017EF74
		public static bool ActivePlayerHasSaveBlockingPrompt()
		{
			TIPromptQueueState tipromptQueueState = GameStateManager.FindGameState<TIPromptQueueState>();
			List<Prompt> list = new List<Prompt>(tipromptQueueState.activePlayerFactionPromptList);
			list.AddRange(tipromptQueueState.activePlayerNationPromptList);
			foreach (Prompt prompt in list)
			{
				string name = prompt.name;
				if (name != null)
				{
					uint num = <PrivateImplementationDetails>.ComputeStringHash(name);
					if (num <= 2462446810U)
					{
						if (num <= 629440155U)
						{
							if (num <= 215492961U)
							{
								if (num != 31176079U)
								{
									if (num != 215492961U)
									{
										continue;
									}
									if (!(name == "PromptFactionContactRespondToOffer"))
									{
										continue;
									}
								}
								else if (!(name == "PromptStealProject"))
								{
									continue;
								}
							}
							else if (num != 492343306U)
							{
								if (num != 629440155U)
								{
									continue;
								}
								if (!(name == "PromptSelectSpaceCombatStance"))
								{
									continue;
								}
							}
							else if (!(name == "PromptSelectSpaceCombatBid"))
							{
								continue;
							}
						}
						else if (num <= 1482437579U)
						{
							if (num != 1481396765U)
							{
								if (num != 1482437579U)
								{
									continue;
								}
								if (!(name == "PromptAddressNarrativeEvent"))
								{
									continue;
								}
							}
							else if (!(name == "PromptSelectPolicy"))
							{
								continue;
							}
						}
						else if (num != 2071856233U)
						{
							if (num != 2355149519U)
							{
								if (num != 2462446810U)
								{
									continue;
								}
								if (!(name == "PromptSelectTrajectory"))
								{
									continue;
								}
							}
							else if (!(name == "PromptRespondToEndRivalryCall"))
							{
								continue;
							}
						}
						else if (!(name == "PromptFactionContactMakeOffer"))
						{
							continue;
						}
					}
					else if (num <= 2995569124U)
					{
						if (num <= 2737292978U)
						{
							if (num != 2623500372U)
							{
								if (num != 2737292978U)
								{
									continue;
								}
								if (!(name == "PromptRespondToFormAllianceCall"))
								{
									continue;
								}
							}
							else if (!(name == "PromptBeginCombat"))
							{
								continue;
							}
						}
						else if (num != 2883068329U)
						{
							if (num != 2900450226U)
							{
								if (num != 2995569124U)
								{
									continue;
								}
								if (!(name == "PromptNationLeavesDarkFederation_Policy"))
								{
									continue;
								}
							}
							else if (!(name == "PromptRespondToTransferRegionCall"))
							{
								continue;
							}
						}
						else if (!(name == "PromptNationLeavesDarkFederation_Violent"))
						{
							continue;
						}
					}
					else if (num <= 3757877436U)
					{
						if (num != 3394191324U)
						{
							if (num != 3757877436U)
							{
								continue;
							}
							if (!(name == "PromptRespondToEndWarCall"))
							{
								continue;
							}
						}
						else if (!(name == "PromptArmyOrderedToDepart"))
						{
							continue;
						}
					}
					else if (num != 4016174268U)
					{
						if (num != 4042397024U)
						{
							if (num != 4202808030U)
							{
								continue;
							}
							if (!(name == "PromptRespondToUnificationCall"))
							{
								continue;
							}
						}
						else if (!(name == "PromptSabotageProject"))
						{
							continue;
						}
					}
					else if (!(name == "PromptRespondToAllyOffensiveWarCall"))
					{
						continue;
					}
					return true;
				}
			}
			return false;
		}

		// Token: 0x06003D4B RID: 15691 RVA: 0x001810A4 File Offset: 0x0017F2A4
		private void HandleSelectPolicy(TINationState nation, TIFactionState faction, TICouncilorState triggeringCouncilor)
		{
			IPlayerActionRunner playerActionRunner = GameControl.playerManager.FindPlayerComponent(faction);
			List<PolicyOptionWithTarget> plannedPolicies = faction.plannedPolicies;
			PolicyOptionWithTarget[] array = ((plannedPolicies != null) ? plannedPolicies.Where<PolicyOptionWithTarget>((PolicyOptionWithTarget x) => x.actingNation == nation).ToArray<PolicyOptionWithTarget>() : null);
			if (array != null && array.Any<PolicyOptionWithTarget>())
			{
				for (int i = 0; i < array.Count<PolicyOptionWithTarget>(); i++)
				{
					PolicyOptionWithTarget policyOptionWithTarget = array[i];
					if (!EqualityComparer<PolicyOptionWithTarget>.Default.Equals(policyOptionWithTarget, null) && policyOptionWithTarget.policy.Allowed(nation) && policyOptionWithTarget.policy.GetPossibleTargets(nation).Contains(policyOptionWithTarget.target))
					{
						TINationState tinationState = null;
						if (policyOptionWithTarget.target.isWarState)
						{
							tinationState = policyOptionWithTarget.target.ref_war.EnemyWarLeader(policyOptionWithTarget.actingNation, false);
						}
						else if (policyOptionWithTarget.target.isNationState)
						{
							tinationState = policyOptionWithTarget.target.ref_nation;
						}
						if (!(tinationState != null) || AICouncilorMissionPlanner.ScorePolicyOption(policyOptionWithTarget, faction, 10, null, null) > 0f)
						{
							playerActionRunner.StartAction(new ConfirmPolicyAction(nation, faction, policyOptionWithTarget.target, triggeringCouncilor, policyOptionWithTarget.policy));
							return;
						}
					}
				}
			}
			playerActionRunner.StartAction(new ConfirmPolicyAction(nation, faction, faction, triggeringCouncilor, new CancelOption()));
		}

		// Token: 0x06003D4C RID: 15692 RVA: 0x0018120C File Offset: 0x0017F40C
		private void HandleSelectTech(TIFactionState faction, int slot)
		{
			IPlayerActionRunner playerActionRunner = GameControl.playerManager.FindPlayerComponent(faction);
			TITechTemplate titechTemplate = this.techSelectionStrategy.SelectTech(faction);
			if (slot < 0 || slot > 2)
			{
				Log.Error(faction.displayName + " trying to pick tech when all slots assigned ", Array.Empty<object>());
			}
			PlayerAction playerAction = new SelectTechAction(faction, slot, titechTemplate);
			playerActionRunner.StartAction(playerAction);
		}

		// Token: 0x06003D4D RID: 15693 RVA: 0x00181264 File Offset: 0x0017F464
		private void HandleSelectProject(TIFactionState faction, int slot)
		{
			IPlayerActionRunner playerActionRunner = GameControl.playerManager.FindPlayerComponent(faction);
			TIProjectTemplate tiprojectTemplate = this.projectSelectionStrategy.SelectProject(faction, slot);
			playerActionRunner.StartAction(new SelectProjectForDevelopmentAction(faction, slot, tiprojectTemplate));
		}

		// Token: 0x06003D4E RID: 15694 RVA: 0x00181297 File Offset: 0x0017F497
		private void HandleMissionPhasePrep(TIFactionState faction, Prompt prompt)
		{
			this.councilorMissionPlanner.MissionPhasePrepCoroutine(faction);
			this.RemovePrompt(prompt);
		}

		// Token: 0x06003D4F RID: 15695 RVA: 0x001812AD File Offset: 0x0017F4AD
		private void HandlePlanMissions(TIFactionState faction, Prompt prompt)
		{
			this.councilorMissionPlanner.PlanMissionsCoroutine(faction);
			this.RemovePrompt(prompt);
		}

		// Token: 0x06003D50 RID: 15696 RVA: 0x001812C4 File Offset: 0x0017F4C4
		private void HandleSelectSpaceCombatStance(TIFactionState faction, TISpaceCombatState combatState)
		{
			IPlayerActionRunner playerActionRunner = GameControl.playerManager.FindPlayerComponent(faction);
			Dictionary<TINationState, PlannedFighters> dictionary = new Dictionary<TINationState, PlannedFighters>();
			if (combatState.CanContributeSTOFightersToCombat(faction))
			{
				dictionary = AIDailyFactionPlanner.DetermineSTOFighterPlan(faction, combatState.fleets, combatState.hab, true, false);
			}
			CombatStance combatStance = this.combatInitStrategy.SelectStance(faction, combatState, dictionary);
			if (combatStance == CombatStance.Evade)
			{
				dictionary.Clear();
			}
			if (dictionary.Values.Sum<PlannedFighters>((PlannedFighters x) => x.count) > 0)
			{
				faction.playerControl.StartAction(new SetSTOFightersForCombatAction(combatState, faction, dictionary));
			}
			playerActionRunner.StartAction(new SelectCombatStance(combatState, faction, combatStance));
		}

		// Token: 0x06003D51 RID: 15697 RVA: 0x00181368 File Offset: 0x0017F568
		private void HandleSelectSpaceCombatBid(TIFactionState faction, TISpaceCombatState combatState)
		{
			IPlayerActionRunner playerActionRunner = GameControl.playerManager.FindPlayerComponent(faction);
			CombatStance combatStance;
			List<TISpaceShipState> list;
			float num = this.combatInitStrategy.SelectBid_kps(faction, combatState, out combatStance, out list);
			if (combatStance == CombatStance.ExtendedPursuit_Envelop || combatStance == CombatStance.ExtendedPursuit_Stretch)
			{
				playerActionRunner.StartAction(new SelectCombatBid(combatState, faction, num, combatStance, list));
				return;
			}
			playerActionRunner.StartAction(new SelectCombatBid(combatState, faction, num, CombatStance.NotYetSet, new List<TISpaceShipState>()));
		}

		// Token: 0x06003D52 RID: 15698 RVA: 0x001813C0 File Offset: 0x0017F5C0
		private void HandleRespondToNarrativeEvent(Prompt prompt, TIFactionState faction, TIGameState eventTarget, TIGameState secondaryTarget)
		{
			IPlayerActionRunner playerActionRunner = GameControl.playerManager.FindPlayerComponent(faction);
			TINarrativeEventTemplate narrativeEvent = TIGlobalValuesState.GetCurrentNarrativeEvent(prompt).narrativeEvent;
			if (narrativeEvent == null || !TIGameState.Valid(eventTarget))
			{
				Log.Warn("Removed null narrative evente prompt with dataName " + TIGlobalValuesState.GetCurrentNarrativeEvent(prompt).dataName, Array.Empty<object>());
				this.RemovePrompt(faction, eventTarget, secondaryTarget, "PromptAddressNarrativeEvent", 0);
				TIGlobalValuesState.ClearNarrativeEvent(prompt);
				return;
			}
			int num = this.narrativeResponseStrategy.SelectOption(faction, eventTarget, secondaryTarget, narrativeEvent);
			playerActionRunner.StartAction(new SelectNarrativeEventOption(faction, eventTarget, secondaryTarget, narrativeEvent, num, TIGlobalValuesState.GetCurrentNarrativeEvent(prompt).allTargetsandSeconds, prompt));
		}

		// Token: 0x06003D53 RID: 15699 RVA: 0x00181458 File Offset: 0x0017F658
		private void HandleStealProject(TIFactionState promptedFaction, TICouncilorState councilor, TIGameState target, TIMissionState mission)
		{
			TIFactionState faction = councilor.faction;
			if (!(faction != null))
			{
				TIPromptQueueState.RemovePromptStatic(promptedFaction, mission.councilor, mission, "PromptStealProject", 0);
				return;
			}
			IPlayerActionRunner playerActionRunner = GameControl.playerManager.FindPlayerComponent(faction);
			List<TIProjectTemplate> list = target.ref_faction.StealableProjects(faction);
			if (list.Count > 0)
			{
				TIProjectTemplate tiprojectTemplate = AIEvaluators.SelectProject(faction, list, false, false);
				playerActionRunner.StartAction(new StealProjectAction(mission, tiprojectTemplate));
				return;
			}
			TIPromptQueueState.RemovePromptStatic(faction, mission.councilor, mission, "PromptStealProject", 0);
			Log.Warn("HandleStealProject against " + target.ref_faction.displayName + " found no project to steal", Array.Empty<object>());
		}

		// Token: 0x06003D54 RID: 15700 RVA: 0x00181500 File Offset: 0x0017F700
		private void HandleSabotageProject(TIFactionState promptedFaction, TICouncilorState councilor, TIGameState target, TIMissionState mission)
		{
			TIFactionState faction = councilor.faction;
			TIFactionState tifactionState = ((target != null) ? target.ref_faction : null);
			if (!(faction != null) || !(tifactionState != null))
			{
				TIPromptQueueState.RemovePromptStatic(promptedFaction, mission.councilor, mission, "PromptSabotageProject", 0);
				return;
			}
			IPlayerActionRunner playerActionRunner = GameControl.playerManager.FindPlayerComponent(faction);
			List<TIProjectTemplate> vulnerableProjects = target.ref_faction.ProjectsVulnerableToSabotage(faction).ToList<TIProjectTemplate>();
			if (vulnerableProjects.Count > 0)
			{
				IEnumerable<ProjectProgress> enumerable = target.ref_faction.currentProjectProgress.Where<ProjectProgress>((ProjectProgress x) => vulnerableProjects.Contains(x.projectTemplate));
				TIProjectTemplate tiprojectTemplate;
				if (enumerable == null)
				{
					tiprojectTemplate = null;
				}
				else
				{
					tiprojectTemplate = enumerable.MaxBy<ProjectProgress, float>((ProjectProgress x) => x.accumulatedResearch).projectTemplate;
				}
				TIProjectTemplate tiprojectTemplate2 = tiprojectTemplate;
				playerActionRunner.StartAction(new SabotageProjectAction(mission, tiprojectTemplate2));
				return;
			}
			TIPromptQueueState.RemovePromptStatic(promptedFaction, mission.councilor, mission, "PromptSabotageProject", 0);
		}

		// Token: 0x06003D55 RID: 15701 RVA: 0x001815FC File Offset: 0x0017F7FC
		private void HandleFactionContactMakeOffer(TIFactionState contactingFaction, TIFactionState contactedFaction, TIMissionState mission)
		{
			if (contactedFaction == GameControl.control.activePlayer)
			{
				GameControl.eventManager.TriggerEvent(new TradeToPlayerInitiated(mission, mission.target.ref_councilor, contactedFaction), null, Array.Empty<object>());
				TIPromptQueueState.RemovePromptStatic(mission.councilor.faction, mission.councilor, mission, "PromptFactionContactMakeOffer", 0);
				TIPromptQueueState.AddPromptStatic(contactedFaction, mission.target.ref_councilor, mission, "PromptFactionContactMakeOffer", 0);
			}
			if (contactingFaction != GameControl.control.activePlayer && contactedFaction != GameControl.control.activePlayer)
			{
				TradeAI.PrepareCachesForTrading(contactingFaction, contactedFaction);
				TradeOffer.TradeAgreement tradeAgreement = TradeAI.CreateTradeAgreement(contactingFaction, contactedFaction);
				TradeOffer offer = tradeAgreement.GetOffer(contactingFaction);
				TradeOffer offer2 = tradeAgreement.GetOffer(contactedFaction);
				if (TradeAI.ScoreAgreement(tradeAgreement, contactedFaction) > 0f)
				{
					mission.ref_faction.playerControl.StartAction(new DiplomacyTradeAction(mission.ref_faction, contactedFaction, offer, offer2, 1f));
				}
				TIPromptQueueState.RemovePromptStatic(mission.councilor.faction, mission.councilor, mission, "PromptFactionContactMakeOffer", 0);
			}
		}

		// Token: 0x06003D56 RID: 15702 RVA: 0x00181708 File Offset: 0x0017F908
		private void HandleFactionContactRespondToOffer(TIFactionState contactingFaction, TIFactionState contactedFaction, TIMissionState mission)
		{
			TIPromptQueueState.RemovePromptStatic(mission.councilor.faction, mission.councilor, mission, "PromptFactionContactRespondToOffer", 0);
		}

		// Token: 0x06003D57 RID: 15703 RVA: 0x00181728 File Offset: 0x0017F928
		private void HandleDropUnassignedOrgs(TIFactionState faction)
		{
			List<TIMissionTemplate> list = faction.RequiredMissions(true);
			List<TIMissionTemplate> list2 = faction.MissingRequiredMissions(list);
			bool currentlyDetectingHydra = faction.currentlyDetectingHydra;
			int count = faction.GoalsOfType(GoalType.WarOnFaction, false, true).Count;
			Dictionary<TICouncilorState, Dictionary<FactionResource, float>> dictionary = faction.councilors.ToDictionary<TICouncilorState, TICouncilorState, Dictionary<FactionResource, float>>((TICouncilorState x) => x, (TICouncilorState y) => TIFactionState.councilorResources.ToDictionary<FactionResource, FactionResource, float>((FactionResource z) => z, (FactionResource z) => y.GetMonthlyIncome(z)));
			AIDailyFactionPlanner.TransferOrgsFromPool(faction, new Dictionary<TIOrgState, TICouncilorState>(), list, list2, ref dictionary, currentlyDetectingHydra, count, AIDailyFactionPlanner.AI_ControllingNeutralPowers(faction));
			int num = faction.UnassignedPoolOverage();
			if (num > 0)
			{
				AIDailyFactionPlanner.SellOrgs(faction, list, num);
			}
			this.RemovePrompt(faction, faction, null, "PromptDropOrgs", 0);
		}

		// Token: 0x06003D58 RID: 15704 RVA: 0x001817E4 File Offset: 0x0017F9E4
		private void HandlePromptChangeTrajectory(TIFactionState faction, TISpaceFleetState maneuveringFleet, TISpaceFleetState targetFleet, Trajectory[] validTrajectories = null)
		{
			bool flag = false;
			if (validTrajectories == null || validTrajectories.Length == 0)
			{
				flag = true;
			}
			else
			{
				maneuveringFleet.AssignTrajectory(validTrajectories[0]);
				maneuveringFleet.LaunchFleet(true);
			}
			if (flag && maneuveringFleet.trajectory != null)
			{
				maneuveringFleet.trajectory.DeTargetFleet();
				maneuveringFleet.LaunchFleet(true);
				Trajectory destinationFleetTrajectory = maneuveringFleet.trajectory.destinationFleetTrajectory;
				if (destinationFleetTrajectory != null && destinationFleetTrajectory.launchTime < TITimeState.Now() && destinationFleetTrajectory.arrivalTime > TITimeState.Now())
				{
					Trajectory trajectory = maneuveringFleet.trajectory;
					Trajectory destinationFleetTrajectory2 = maneuveringFleet.trajectory.destinationFleetTrajectory;
					trajectory.nextTrajectory = ((destinationFleetTrajectory2 != null) ? destinationFleetTrajectory2.ShallowCopy(maneuveringFleet) : null);
				}
			}
			maneuveringFleet.destroyProposedTrajectories();
			this.RemovePrompt(faction, maneuveringFleet, targetFleet, "PromptChangeTrajectory", 0);
		}

		// Token: 0x06003D59 RID: 15705 RVA: 0x00181899 File Offset: 0x0017FA99
		private IPlayerActionRunner GetNationRunner(TINationState respondingNation)
		{
			if (!(respondingNation.executiveFaction != null))
			{
				return GameControl.playerManager.FindPlayerComponent(GameControl.control.activePlayer);
			}
			return GameControl.playerManager.FindPlayerComponent(respondingNation.executiveFaction);
		}

		// Token: 0x06003D5A RID: 15706 RVA: 0x001818D0 File Offset: 0x0017FAD0
		private void HandleProposedAlliance(TINationState respondingNation, TINationState promptingNation)
		{
			TIPolicyOptionWithConfirm tipolicyOptionWithConfirm = (TIPolicyOptionWithConfirm)PolicyManager.policies[PolicyType.ProposeAllianceOption];
			bool flag = this.policyResponseSelectionStrategy.SelectPolicyReply(promptingNation, respondingNation, tipolicyOptionWithConfirm);
			this.GetNationRunner(respondingNation).StartAction(new RespondToPolicyProposalAction(respondingNation, promptingNation, null, tipolicyOptionWithConfirm, flag));
		}

		// Token: 0x06003D5B RID: 15707 RVA: 0x00181914 File Offset: 0x0017FB14
		private void HandleEndWar(TINationState respondingNation, TINationState promptingNation, TIWarState war)
		{
			TIPolicyOptionWithConfirm tipolicyOptionWithConfirm = (TIPolicyOptionWithConfirm)PolicyManager.policies[PolicyType.EndWarOption];
			bool flag = this.policyResponseSelectionStrategy.SelectPolicyReply(promptingNation, respondingNation, war, tipolicyOptionWithConfirm);
			this.GetNationRunner(respondingNation).StartAction(new RespondToPolicyProposalAction(respondingNation, promptingNation, war, tipolicyOptionWithConfirm, flag));
		}

		// Token: 0x06003D5C RID: 15708 RVA: 0x00181958 File Offset: 0x0017FB58
		private void HandleEndRivalry(TINationState respondingNation, TINationState promptingNation)
		{
			TIPolicyOptionWithConfirm tipolicyOptionWithConfirm = (TIPolicyOptionWithConfirm)PolicyManager.policies[PolicyType.EndRivalryOption];
			bool flag = this.policyResponseSelectionStrategy.SelectPolicyReply(promptingNation, respondingNation, tipolicyOptionWithConfirm);
			this.GetNationRunner(respondingNation).StartAction(new RespondToPolicyProposalAction(respondingNation, promptingNation, null, tipolicyOptionWithConfirm, flag));
		}

		// Token: 0x06003D5D RID: 15709 RVA: 0x0018199C File Offset: 0x0017FB9C
		private void HandleFederation(TINationState respondingNation, TINationState promptingNation)
		{
			TIPolicyOptionWithConfirm tipolicyOptionWithConfirm = (TIPolicyOptionWithConfirm)PolicyManager.policies[PolicyType.JoinFederationOption];
			bool flag = this.policyResponseSelectionStrategy.SelectPolicyReply(promptingNation, respondingNation, tipolicyOptionWithConfirm);
			this.GetNationRunner(respondingNation).StartAction(new RespondToPolicyProposalAction(respondingNation, promptingNation, null, tipolicyOptionWithConfirm, flag));
		}

		// Token: 0x06003D5E RID: 15710 RVA: 0x001819E0 File Offset: 0x0017FBE0
		private void HandleUnification(TINationState respondingNation, TINationState promptingNation)
		{
			TIPolicyOptionWithConfirm tipolicyOptionWithConfirm = (TIPolicyOptionWithConfirm)PolicyManager.policies[PolicyType.UnificationOption];
			bool flag = this.policyResponseSelectionStrategy.SelectPolicyReply(promptingNation, respondingNation, tipolicyOptionWithConfirm);
			this.GetNationRunner(respondingNation).StartAction(new RespondToPolicyProposalAction(respondingNation, promptingNation, null, tipolicyOptionWithConfirm, flag));
		}

		// Token: 0x06003D5F RID: 15711 RVA: 0x00181A24 File Offset: 0x0017FC24
		private void HandleRegionDemanded(TINationState respondingNation, TINationState promptingNation, TIRegionState region)
		{
			TIPolicyOptionWithConfirm tipolicyOptionWithConfirm = (TIPolicyOptionWithConfirm)PolicyManager.policies[PolicyType.TransferRegionsOption];
			bool flag = this.policyResponseSelectionStrategy.SelectPolicyReply(promptingNation, respondingNation, tipolicyOptionWithConfirm, region);
			this.GetNationRunner(respondingNation).StartAction(new RespondToPolicyProposalAction(respondingNation, promptingNation, region, tipolicyOptionWithConfirm, flag));
		}

		// Token: 0x06003D60 RID: 15712 RVA: 0x00181A6C File Offset: 0x0017FC6C
		private void HandleCallToOffensiveWar(TINationState respondingNation, TINationState promptingNation, TIWarState war)
		{
			float num;
			List<TINationState> list;
			bool flag = AIEvaluators.AIWillingToJoinOffensiveAllysWar(respondingNation, promptingNation, war.defender) && AIEvaluators.AIAlliesCollectivelyWillingToJoinOffensiveWar(promptingNation, war.defender, out num, out list);
			this.GetNationRunner(respondingNation).StartAction(new RespondToCallAllyAction(respondingNation, promptingNation, war, flag));
			TIPromptQueueState.RemovePromptStatic(respondingNation, promptingNation, war, "PromptRespondToAllyOffensiveWarCall", 0);
			GeneralControlsController.UpdateBlockedPause();
		}

		// Token: 0x06003D61 RID: 15713 RVA: 0x00181AC4 File Offset: 0x0017FCC4
		private void HandleNationLeavesMyDarkFederation_Violent(TINationState fedLeader, TINationState departingNation, Prompt nationPrompt)
		{
			if (fedLeader.AllowedWarTarget_NoRivalryCheck(fedLeader, fedLeader.WarCapableAllies))
			{
				float num = AICouncilorMissionPlanner.ScorePolicyOption(new PolicyOptionWithTarget(fedLeader, PolicyType.WarOption, departingNation), fedLeader.executiveFaction, departingNation.numControlPoints_unclamped * 3, null, new Dictionary<TINationState, float> { 
				{
					departingNation,
					AIEvaluators.EvaluateNation(fedLeader.executiveFaction, departingNation)
				} });
				if (num > 0f && TIUtilities.RandomFloatValue() < num / 500f)
				{
					this.GetNationRunner(fedLeader).StartAction(new ConfirmPolicyAction(fedLeader, fedLeader.executiveFaction, departingNation, null, new WarOption()));
				}
			}
			TIPromptQueueState.RemovePromptStatic(nationPrompt);
		}

		// Token: 0x06003D62 RID: 15714 RVA: 0x00181B50 File Offset: 0x0017FD50
		private void HandleNationLeavesMyDarkFederation_Policy(TINationState fedLeader, TINationState departingNation, Prompt nationPrompt)
		{
			TIPolicyOptionWithConfirm tipolicyOptionWithConfirm = (TIPolicyOptionWithConfirm)PolicyManager.policies[PolicyType.LeaveFederationOption];
			bool flag = this.policyResponseSelectionStrategy.SelectPolicyReply(departingNation, fedLeader, tipolicyOptionWithConfirm);
			this.GetNationRunner(fedLeader).StartAction(new RespondToPolicyProposalAction(fedLeader, departingNation, fedLeader.federation, tipolicyOptionWithConfirm, flag));
			TIPromptQueueState.RemovePromptStatic(nationPrompt);
			GeneralControlsController.UpdateBlockedPause();
		}

		// Token: 0x06003D63 RID: 15715 RVA: 0x00181BA4 File Offset: 0x0017FDA4
		private void HandleRespondToNarrativeEvent(Prompt prompt, TINationState nation, TIGameState eventTarget, TIGameState secondaryTarget)
		{
			TINarrativeEventTemplate narrativeEvent = TIGlobalValuesState.GetCurrentNarrativeEvent(prompt).narrativeEvent;
			if (narrativeEvent != null)
			{
				int num = this.narrativeResponseStrategy.SelectOption(nation, eventTarget, secondaryTarget, narrativeEvent);
				this.globalValues.ExecuteNarrativeEventOption(TIGlobalValuesState.GetCurrentNarrativeEvent(prompt).narrativeEvent, nation, eventTarget, secondaryTarget, num, TIGlobalValuesState.GetCurrentNarrativeEvent(prompt).allTargetsandSeconds, prompt);
			}
			else
			{
				Log.Debug("Missing event template for HandleRespondToNarrativeEvent", Array.Empty<object>());
			}
			this.RemovePrompt(nation, eventTarget, secondaryTarget, "PromptAddressNarrativeEvent", 0);
		}

		// Token: 0x06003D64 RID: 15716 RVA: 0x00181C20 File Offset: 0x0017FE20
		private void HandleResponseToArmyBooted(Prompt prompt)
		{
			ArmyOrderedToDepartOptions armyOrderedToDepartOptions = ArmyOrderedToDepartOptions.Depart;
			TINationState ref_nation = prompt.actingState.ref_nation;
			TIFactionState executiveFaction = prompt.actingState.ref_nation.executiveFaction;
			TINationState ref_nation2 = prompt.promptingGameState.ref_nation;
			if (executiveFaction != null)
			{
				if (ref_nation.CanAllyForRemoveArmyPrompt(ref_nation2) && AIEvaluators.ScoreFormAlliance(ref_nation, ref_nation2, executiveFaction == ref_nation2.executiveFaction, true) * new ProposeAllianceOption().AIAgreeChance_Prospective(ref_nation, ref_nation2) > 10f)
				{
					armyOrderedToDepartOptions = ArmyOrderedToDepartOptions.OfferAlliance;
				}
				if (armyOrderedToDepartOptions == ArmyOrderedToDepartOptions.Depart && ref_nation.CanDeclareWarForRemoveArmyPrompt(ref_nation2, prompt.relatedGameState != null) && AIEvaluators.ScoreIncreasingConflict(ref_nation, ref_nation2, executiveFaction == ref_nation2.executiveFaction, PolicyType.WarOption) > 50f)
				{
					armyOrderedToDepartOptions = ArmyOrderedToDepartOptions.DeclareWar;
				}
			}
			prompt.actingState.ref_nation.HandlePromptArmyOrderedToDepartDecision(prompt.promptingGameState.ref_nation, armyOrderedToDepartOptions, prompt);
		}

		// Token: 0x06003D65 RID: 15717 RVA: 0x00181CEC File Offset: 0x0017FEEC
		public string DumpActivePlayerPrompts()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (Prompt prompt in this.activePlayerFactionPromptList)
			{
				stringBuilder.AppendLine(prompt.ToString());
			}
			foreach (Prompt prompt2 in this.activePlayerNationPromptList)
			{
				stringBuilder.AppendLine(prompt2.ToString());
			}
			return stringBuilder.ToString();
		}

		// Token: 0x04002682 RID: 9858
		[SerializeField]
		private List<Prompt> nationList;

		// Token: 0x04002683 RID: 9859
		[SerializeField]
		private List<Prompt> factionList;

		// Token: 0x04002686 RID: 9862
		private IProjectSelectionStrategy projectSelectionStrategy;

		// Token: 0x04002687 RID: 9863
		private ITechSelectionStrategy techSelectionStrategy;

		// Token: 0x04002688 RID: 9864
		private IPolicyResponseSelectionStrategy policyResponseSelectionStrategy;

		// Token: 0x04002689 RID: 9865
		private ICombatInitStrategy combatInitStrategy;

		// Token: 0x0400268A RID: 9866
		private INarrativeResponseSelectionStrategy narrativeResponseStrategy;

		// Token: 0x0400268B RID: 9867
		[SerializeField]
		private bool gameStateSubjectCreated;

		// Token: 0x0400268C RID: 9868
		[Inject]
		private GameTimeManager gameTime;

		// Token: 0x0400268D RID: 9869
		private TIGlobalValuesState globalValues;

		// Token: 0x0400268E RID: 9870
		private AICouncilorMissionPlanner councilorMissionPlanner;
	}
}
