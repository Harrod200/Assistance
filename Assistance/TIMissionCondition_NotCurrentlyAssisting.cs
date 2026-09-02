using PavonisInteractive.TerraInvicta;

namespace Assistance
{
    /// <summary>
    /// Condition that prevents targeting a councilor who is currently performing an Assist mission.
    /// This ensures councillors actively assisting others cannot be assisted themselves until
    /// their current assist mission completes.
    /// </summary>
    public class TIMissionCondition_NotCurrentlyAssisting : TIMissionCondition
    {
        public override string CanTarget(TICouncilorState councilor, TIGameState possibleTarget)
        {
            // Verify target is a councilor
            if (!possibleTarget.isCouncilorState)
                return TIMissionCondition.fail;

            TICouncilorState targetCouncilor = possibleTarget.ref_councilor;
            if (targetCouncilor == null)
                return TIMissionCondition.fail;

            // Check if target is currently performing an Assist mission
            if (targetCouncilor.activeMission != null && 
                targetCouncilor.activeMission.missionTemplate != null &&
                targetCouncilor.activeMission.missionTemplate.dataName == "Assist")
            {
                return TIMissionCondition.fail;
            }

            // Target is not currently assisting, so they can be targeted
            return TIMissionCondition.pass;
        }
    }
}
