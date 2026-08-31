using System;
using PavonisInteractive.TerraInvicta;

namespace Assistance
{
    /// <summary>
    /// Condition that restricts a mission to only player-controlled factions.
    /// AI factions cannot use missions with this condition.
    /// </summary>
    public class TIMissionCondition_PlayerFactionOnly : TIMissionCondition
    {
        public override string CanTarget(TICouncilorState councilor, TIGameState possibleTarget)
        {
            if (councilor == null || councilor.faction == null)
                return TIMissionCondition.fail;

            // Only allow player-controlled factions
            // playerControl returns null for AI factions, non-null for player factions
            if (councilor.faction.playerControl != null)
                return TIMissionCondition.pass;
            else
                return TIMissionCondition.fail;
        }
    }
}
