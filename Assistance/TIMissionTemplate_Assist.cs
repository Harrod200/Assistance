using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

namespace Assistance
{
    public class TIMissionTemplate_Assist : TIMissionTemplate
    {
        public TIMissionTemplate_Assist() : base("Assist")
        {
            try
            {
                this.dataName = "Assist";
                this.friendlyName = "Assist Councilor";
                this.disable = false;
                this.baseMission = false;
                this.persistentEffect = false;

                // Match Inspire mission noise/hate profile (support mission that helps allies)
                this.noise = new float[] { 0f, -2f, -4f, 0f, -4f, -4f };
                this.hate = new float[] { 0f, 0f, 0f, 0f, 0f, 0f };

                this.specialPost = false;
                this.permanentAssignment = false;
                this.XPonSuccess = 2;
                this.sortOrder = 0; // Slightly after Inspire (22)
                this.missionContext = MissionContext.Unlimited;
                this.utilityScore = 1f;
                this.UIalertEnemyOnFail = false;
                this.AIDoubleUpAllowed = false;
                this.maximumTargetOptionCount = 20;
                this.resolutionOrder = 0; // Fastest resolution (0 = resolves first each turn)
                this.allowedForAutoDefense = true; // Match Inspire

                // Use Automatic resolution for guaranteed success (no dice roll)
                // Matches GoToGround and DefendInterests pattern - support mission has no opposition
                this.resolutionMethod = new TIMissionResolution_Automatic
                {
                    attackingModifiers = new List<TIMissionModifier>(),
                    defendingModifiers = new List<TIMissionModifier>()
                };

                // Use context lists with "None" entries to match vanilla pattern
                // This matches GoToGround and DefendInterests missions
                this.attackerContexts = new List<Context> { Context.None, Context.None };
                this.defenderContexts = new List<Context> { Context.None, Context.None };

                // Mission conditions for target validation
                // Assist targets ANY councilor in the player's faction
                // No restrictions on location, mission status, or other factors
                this.conditions = new List<TIMissionCondition>
                {
                    new TIMissionCondition_MyFactionCouncilor(),      // Target must be same faction
                    new TIMissionCondition_PlayerFactionOnly(),       // Faction must be player-controlled
                    new TIMissionCondition_NotCurrentlyAssisting()    // Target cannot be currently assisting
                };

                this.movementRule = MissionMovementRule.MoveToTarget;
                this.councilorEffects = new List<TIMissionEffect>();

                // Target: another councilor
                this.target = new TIMissionTarget_Councilor();

                this.targetEffects = new List<TIMissionEffect>
                {
                    new TIMissionEffect_Assist()
                };

                this.missionIconImagePath = "councilor_missions/ICO_inspire";
                this.targetingMethodType = typeof(TIMissionTargeting_Councilor);
                this.completedIllustrationResource = new List<string> { "illustrations/Mission_InspireCouncilor" };
            }
            catch (Exception ex)
            {
                if (Main.mod != null)
                {
                    Main.mod.Logger.Error("Error initializing TIMissionTemplate_Assist: " + ex);
                }
            }
        }
    }
}
