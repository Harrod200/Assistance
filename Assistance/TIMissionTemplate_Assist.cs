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
                this.sortOrder = 23; // Slightly after Inspire (22)
                this.missionContext = MissionContext.Unlimited;
                this.utilityScore = 1f;
                this.UIalertEnemyOnFail = false;
                this.AIDoubleUpAllowed = false;
                this.maximumTargetOptionCount = 20;
                this.resolutionOrder = 0; // Fastest resolution (0 = resolves first each turn)
                this.allowedForAutoDefense = true; // Match Inspire

                // Use Contested resolution (required for UI to work properly)
                this.resolutionMethod = new TIMissionResolution_Contested
                {
                    attackingModifiers = new List<TIMissionModifier>
                    {
                        new TIMissionModifier_FlatModifier
                        {
                            flatModifier = 0
                        }
                    },
                    defendingModifiers = new List<TIMissionModifier>
                    {
                        new TIMissionModifier_FlatModifier
                        {
                            flatModifier = 0
                        }
                    }
                };

                // Use empty context lists - Inspire mission uses similar pattern
                // Empty lists prevent AI planner from attempting dictionary lookups
                this.attackerContexts = new List<Context>();
                this.defenderContexts = new List<Context>();

                // Mission conditions for target validation
                // Assist targets ANY councilor in the player's faction
                // No restrictions on location, mission status, or other factors
                this.conditions = new List<TIMissionCondition>
                {
                    new TIMissionCondition_MyFactionCouncilor(),  // Target must be same faction
                    new TIMissionCondition_PlayerFactionOnly()    // Faction must be player-controlled
                };

                this.movementRule = MissionMovementRule.MoveToTarget;
                this.councilorEffects = new List<TIMissionEffect>();

                // Target: another councilor
                this.target = new TIMissionTarget_Councilor();

                this.targetEffects = new List<TIMissionEffect>
                {
                    new TIMissionEffect_Assist()
                };

                // Free mission - no cost (match Inspire pattern)
                this.cost = new TIMissionCost_Bonus
                {
                    resourceType = FactionResource.None
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
