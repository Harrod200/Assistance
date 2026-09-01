using PavonisInteractive.TerraInvicta;

namespace Assistance
{
    /// <summary>
    /// Condition that ensures the target is a councilor from the same faction as the source.
    /// This is a custom implementation to ensure consistent behavior for the Assist mission.
    /// </summary>
    public class TIMissionCondition_MyFactionCouncilor : TIMissionCondition
    {
        public override string CanTarget(TICouncilorState councilor, TIGameState possibleTarget)
        {
            // Verify we have valid input
            if (councilor == null || councilor.faction == null)
                return TIMissionCondition.fail;

            // Check if target is a councilor state
            if (!possibleTarget.isCouncilorState)
                return TIMissionCondition.fail;

            TICouncilorState targetCouncilor = possibleTarget.ref_councilor;

            // Verify target councilor exists and is valid
            if (targetCouncilor == null || targetCouncilor.faction == null)
                return TIMissionCondition.fail;

            // Check if target is from same faction
            if (targetCouncilor.faction != councilor.faction)
                return TIMissionCondition.fail;

            // Check if target is not the source councilor (can't assist yourself)
            if (councilor == targetCouncilor)
                return TIMissionCondition.fail;

            // All checks passed
            return TIMissionCondition.pass;
        }
    }
}
