using System;
using System.Collections.Generic;

namespace MoreRealisticNukes
{
	// Token: 0x02000008 RID: 8
	public class TIMissionTemplate_DisarmNukes : TIMissionTemplate
	{
		// Token: 0x0600001A RID: 26 RVA: 0x00002A88 File Offset: 0x00000C88
		public TIMissionTemplate_DisarmNukes()
			: base("DisarmNukes")
		{
			base.dataName = "DisarmNukes";
			base.friendlyName = "Disarm Nukes";
			base.disable = false;
			this.baseMission = false;
			this.persistentEffect = false;
			this.noise = new float[] { 0f, 12f, 2f, 0f, 0f, -6f };
			float[] array = new float[6];
			array[1] = 2f;
			array[4] = 3f;
			this.hate = array;
			this.specialPost = false;
			this.permanentAssignment = false;
			this.XPonSuccess = 4;
			this.sortOrder = 11;
			this.missionContext = 1;
			this.utilityScore = Math.Max(0f, (Main.settings != null) ? Main.settings.DisarmMissionUtilityScore : 0f);
			this.UIalertEnemyOnFail = true;
			this.AIDoubleUpAllowed = false;
			this.maximumTargetOptionCount = 20;
			this.resolutionOrder = 2;
			this.allowedForAutoDefense = false;
			this.resolutionMethod = TIMissionTemplate_DisarmNukes.BuildResolution();
			this.attackerContexts = new List<Context> { 89, 0 };
			this.defenderContexts = new List<Context> { 90 };
			this.conditions = new List<TIMissionCondition>
			{
				new TIMissionCondition_TargetInRange(),
				new TIMissionCondition_CouncilorOnEarth(),
				new TIMissionCondition_HasNukes()
			};
			this.movementRule = 1;
			this.councilorEffects = new List<TIMissionEffect>();
			this.target = new TIMissionTarget_Nation();
			this.targetEffects = new List<TIMissionEffect>
			{
				new TIMissionEffect_DisarmNukes()
			};
			this.cost = new TIMissionCost_Bonus
			{
				resourceType = 3
			};
			this.missionIconImagePath = "operations/Launch_Nuke";
			this.targetingMethodType = typeof(TIMissionTargeting_Nation);
			this.completedIllustrationResource = new List<string> { "illustrations/Event_LaunchPadFire" };
		}

		// Token: 0x0600001B RID: 27 RVA: 0x00002C70 File Offset: 0x00000E70
		private static TIMissionResolution BuildResolution()
		{
			return new TIMissionResolution_Contested
			{
				attackingModifiers = new List<TIMissionModifier>
				{
					new TIMissionModifier_CouncilorAttackStat
					{
						attackerAttribute = 3
					},
					new TIMissionModifier_ResourceSpent(),
					new TIMissionModifier_NationUnrest()
				},
				defendingModifiers = new List<TIMissionModifier>
				{
					new TIMissionModifier_FlatModifier
					{
						flatModifier = 10f
					},
					new TIMissionModifier_TargetNationGDP()
				}
			};
		}
	}
}
