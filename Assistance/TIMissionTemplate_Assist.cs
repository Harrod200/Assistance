using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

namespace Assistance
{
    public class TIMissionTemplate_Assist : TIMissionTemplate
    {
        public TIMissionTemplate_Assist() : base("Assist")
        {
            this.dataName = "Assist";
            this.friendlyName = "Assist Councilor";
            this.disable = false;
            this.baseMission = false;
            this.persistentEffect = false;

            // Noise and hate modifiers
            this.noise = new float[] { 0f, 2f, 0f, 0f, 0f, 0f };
            this.hate = new float[] { 0f, 0f, 0f, 0f, 0f, 0f };

            this.specialPost = false;
            this.permanentAssignment = false;
            this.XPonSuccess = 2;
            this.sortOrder = 50;
            this.missionContext = 0;
            this.utilityScore = 5f;
            this.UIalertEnemyOnFail = false;
            this.AIDoubleUpAllowed = false;
            this.maximumTargetOptionCount = 50;
            this.resolutionOrder = 0; // Fastest resolution (0 = resolves first each turn)
            this.allowedForAutoDefense = false;

            // Use Contested resolution (required for UI to work properly)
            this.resolutionMethod = new TIMissionResolution_Contested
            {
                attackingModifiers = new List<TIMissionModifier>
                {
                    new TIMissionModifier_AssistStat()
                },
                defendingModifiers = new List<TIMissionModifier>()
            };

            this.attackerContexts = new List<Context> { 0 };
            this.defenderContexts = new List<Context>();

            // Mission conditions for target validation
            // Match Inspire mission targeting rules
            this.conditions = new List<TIMissionCondition>
            {
                new TIMissionCondition_TargetInRange(),
                new TIMissionCondition_MyFactionCouncilor()
                // Note: Inspire uses TIMissionCondition_FreeCouncilor which checks for detained councilors
                // Since Assist is for friendly councilors only, we don't need that check
            };

            this.movementRule = (MissionMovementRule)1;
            this.councilorEffects = new List<TIMissionEffect>();

            // Target: another councilor
            this.target = new TIMissionTarget_Councilor();

            this.targetEffects = new List<TIMissionEffect>
            {
                new TIMissionEffect_Assist()
            };

            // No resource cost for assistance (free mission)
            this.cost = new TIMissionCost_Flat
            {
                resourceType = FactionResource.None,
                value = 0f
            };

            this.missionIconImagePath = "operations/Inspire";
            this.targetingMethodType = typeof(TIMissionTargeting_Councilor);
            this.completedIllustrationResource = new List<string> { "illustrations/Event_Diplomat" };
        }
    }
}
