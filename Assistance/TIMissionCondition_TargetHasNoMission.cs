using System;
using PavonisInteractive.TerraInvicta;

namespace Assistance
{
    /// <summary>
    /// Mission condition that ensures the target councilor does not have an active mission.
    /// This prevents the Assist mission from targeting councilors who are already performing other missions.
    /// Matches the Inspire mission's targeting constraints.
    /// </summary>
    public class TIMissionCondition_TargetHasNoMission : TIMissionCondition
    {
        public override string CanTarget(TICouncilorState councilor, TIGameState possibleTarget)
        {
            // Check if target is a councilor
            if (!possibleTarget.isCouncilorState)
            {
                return "TIMissionCondition_TargetHasNoMission";
            }

            TICouncilorState targetCouncilor = possibleTarget.ref_councilor;

            // Target must not have an active mission
            if (targetCouncilor != null && !targetCouncilor.HasMission)
            {
                return "_Pass";
            }

            return "TIMissionCondition_TargetHasNoMission";
        }
    }
}
