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
            // Check if faction's player is AI. If it is AI, return fail; if it's player-controlled, return pass
            if (councilor.faction.player != null && councilor.faction.player.isAI)
                return TIMissionCondition.fail;
            else
                return TIMissionCondition.pass;
        }
    }
}
